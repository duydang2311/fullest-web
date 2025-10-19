using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Common.Http;
using WebApp.Api.Common.Projection;
using WebApp.Domain.Entities;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Statuses.GetMany;

public sealed class Endpoint(AppDbContext db)
    : Endpoint<Request, Ok<PaginatedList<Projectable>>>
{
    public override void Configure()
    {
        Get("statuses");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Ok<PaginatedList<Projectable>>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var query = db.Statuses.AsQueryable();
        if (req.ProjectId.HasValue)
        {
            query = query.Where(a => a.ProjectId == req.ProjectId.Value);
        }

        if (!string.IsNullOrEmpty(req.Fields))
        {
            query = query.Select(FieldProjector.Project<Status>(req.Fields));
        }

        var totalCount = await query.CountAsync(ct).ConfigureAwait(false);

        var pagination = OffsetPagination.From(req);
        var order = Orderable.From(req);
        var list = await query
            .SortOrDefault(order, q => q.OrderByDescending(a => a.Id))
            .Paginate(pagination)
            .ToListAsync(ct)
            .ConfigureAwait(false);
        return TypedResults.Ok(
            PaginatedList.From(
                list.Select(task => task.ToProjectable(req.Fields)),
                pagination,
                totalCount
            )
        );
    }
}
