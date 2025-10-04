using Ardalis.GuardClauses;
using EntityFramework.Exceptions.Common;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Codecs;
using WebApp.Api.Common.Http;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Projects.Create;

public sealed class Endpoint(
    AppDbContext db,
    INumberEncoder numberEncoder,
    LinkGenerator linkGenerator
) : Endpoint<Request, Results<Conflict<Problem>, BadRequest<Problem>, Created<Response>>>
{
    public override void Configure()
    {
        Post("projects");
        Version(1);
    }

    public override async Task<
        Results<Conflict<Problem>, BadRequest<Problem>, Created<Response>>
    > ExecuteAsync(Request req, CancellationToken ct)
    {
        Guard.Against.Null(req.Name);
        Guard.Against.Null(req.NormalizedIdentifier);

        var namespaceId = await db
            .Namespaces.Where(a => a.UserId == req.CallerId)
            .Select(a => (NamespaceId?)a.Id)
            .FirstOrDefaultAsync(ct);
        if (!namespaceId.HasValue)
        {
            return TypedResults.BadRequest(Problem.FromError(ErrorCodes.NamespaceNotFound));
        }

        var project = new Project
        {
            Name = req.Name,
            Identifier = req.NormalizedIdentifier,
            NamespaceId = namespaceId.Value,
            Summary = req.NormalizedSummary,
        };
        db.Projects.Add(project);
        try
        {
            await db.SaveChangesAsync(ct).ConfigureAwait(false);
        }
        catch (UniqueConstraintException e)
            when (e.ConstraintProperties.Any(a => a.Equals(nameof(Project.Identifier))))
        {
            return TypedResults.Conflict(
                Problem.FromError(nameof(req.Identifier), ErrorCodes.Conflict)
            );
        }

        return TypedResults.Created(
            linkGenerator.GetUriByName(
                HttpContext,
                IEndpoint.GetName<GetOne.ById.Endpoint>(),
                new { ProjectId = numberEncoder.Encode(project.Id.Value) }
            ),
            new Response(project.Id, project.Identifier)
        );
    }
}
