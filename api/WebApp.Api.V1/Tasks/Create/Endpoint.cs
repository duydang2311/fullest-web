using System.Text;
using System.Text.Json;
using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Http;
using WebApp.Application.Common;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Create;

public sealed class Endpoint(
    AppDbContext db,
    LinkGenerator linkGenerator,
    INumberEncoder numberEncoder,
    IEnumerable<ITaskCreatedHandler> taskCreatedHandlers
) : Endpoint<Request, Results<BadRequest<Problem>, NotFound<Problem>, Created<Response>>>
{
    public override void Configure()
    {
        Post("tasks");
        PreProcessor<Authorize>();
        Version(1);
    }

    public override async Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Created<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.ProjectId);
        Guard.Against.Null(req.NormalizedTitle);

        await using var tx = await db.Database.BeginTransactionAsync(ct).ConfigureAwait(false);
        var (json, preview) = TextDocumentHelper.ParseDocumentPreview(req.DescriptionJson);
        var task = new TaskEntity
        {
            ProjectId = req.ProjectId.Value,
            AuthorId = req.CallerId,
            Title = req.NormalizedTitle,
            DescriptionJson = json,
            DescriptionPreview = preview,
        };
        await db.AddAsync(task, ct).ConfigureAwait(false);
        try
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (ReferenceConstraintException e)
            when (e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(TaskEntity.ProjectId), StringComparison.Ordinal)
                )
            )
        {
            return TypedResults.NotFound(
                Problem.FromError(nameof(Request.ProjectId), ErrorCodes.NotFound)
            );
        }
        catch (ReferenceConstraintException e)
            when (e.ConstraintProperties.Any(a =>
                    a.Equals(nameof(TaskEntity.AuthorId), StringComparison.Ordinal)
                )
            )
        {
            return TypedResults.BadRequest(
                Problem.FromError(nameof(TaskEntity.AuthorId), ErrorCodes.NotFound)
            );
        }

        var taskCreated = new TaskCreated(task);
        foreach (var handler in taskCreatedHandlers)
        {
            await handler.HandleAsync(taskCreated, ct).ConfigureAwait(false);
        }
        await db.SaveChangesAsync(ct).ConfigureAwait(false);

        var url = linkGenerator.GetUriByName(
            HttpContext,
            IEndpoint.GetName<GetOne.ById.Endpoint>(),
            new
            {
                ProjectId = numberEncoder.Encode(task.ProjectId.Value),
                TaskId = numberEncoder.Encode(task.Id.Value),
            }
        );

        await tx.CommitAsync(ct).ConfigureAwait(false);
        return TypedResults.Created(url, new Response(task.Id, task.PublicId));
    }

    private static string? ExtractText(string text, int size)
    {
        if (text.Length <= size)
        {
            return text;
        }

        if (text[size] == ' ')
        {
            return text[..size];
        }

        var lastDelimiter = -1;
        for (int i = size; i >= 0; --i)
        {
            if (text[i] == ' ')
            {
                lastDelimiter = i;
                break;
            }
        }
        if (lastDelimiter == -1)
        {
            return text[..size];
        }
        return text[..lastDelimiter];
    }

    private static string? ExtractText(JsonDocument doc, int size)
    {
        var stack = new Stack<JsonElement>();
        var sb = new StringBuilder(size);
        stack.Push(doc.RootElement);
        while (sb.Length < size && stack.TryPop(out var el))
        {
            if (!el.TryGetProperty("type", out var type))
            {
                continue;
            }

            var value = type.GetString();
            if (string.Equals(value, "text", StringComparison.OrdinalIgnoreCase))
            {
                if (el.TryGetProperty("text", out var text))
                {
                    var trimmed = text.GetString()?.Trim();
                    if (sb.Length + (trimmed?.Length ?? 0) >= size)
                    {
                        break;
                    }
                    sb.Append(trimmed).Append(' ');
                }
                continue;
            }

            if (
                el.TryGetProperty("content", out var content)
                && content.ValueKind == JsonValueKind.Array
            )
            {
                foreach (var childEl in content.EnumerateArray())
                {
                    stack.Push(childEl);
                }
            }
        }

        if (sb.Length == 0)
        {
            return null;
        }
        if (sb[^1] == ' ')
        {
            return sb.ToString(0, sb.Length - 1);
        }
        return sb.ToString();
    }
}
