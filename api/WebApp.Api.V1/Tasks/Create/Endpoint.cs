using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Create;

public sealed class Endpoint(
    AppDbContext db,
    LinkGenerator linkGenerator,
    INumberEncoder numberEncoder
) : Endpoint<Request, Results<BadRequest<Problem>, NotFound<Problem>, Created<Response>>>
{
    public override void Configure()
    {
        Post("projects/{ProjectId}/tasks");
        PreProcessor<Authorize>();
        Version(1);
    }

    public override async Task<
        Results<BadRequest<Problem>, NotFound<Problem>, Created<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.NormalizedTitle);

        var task = new TaskEntity
        {
            ProjectId = req.ProjectId,
            AuthorId = req.CallerId,
            Title = req.NormalizedTitle,
            Description = req.Description?.Trim(),
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

        var url = linkGenerator.GetUriByName(
            HttpContext,
            IEndpoint.GetName<GetOne.ById.Endpoint>(),
            new
            {
                ProjectId = numberEncoder.Encode(task.ProjectId.Value),
                TaskId = numberEncoder.Encode(task.Id.Value),
            }
        );
        return TypedResults.Created(url, new Response(task.Id, task.PublicId));
    }
}
