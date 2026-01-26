using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebApp.Infrastructure.Data;

namespace WebApp.Api.V1.Tasks.Delete;

public sealed class Endpoint(AppDbContext db) : Endpoint<Request, Results<NotFound, NoContent>>
{
    public override void Configure()
    {
        Delete("tasks/{TaskId}");
        Version(1);
        PreProcessor<Authorize>();
    }

    public override async Task<Results<NotFound, NoContent>> ExecuteAsync(
        Request req,
        CancellationToken ct
    )
    {
        var count = await db
            .Tasks.Where(a => a.DeletedTime == null && a.Id == req.TaskId)
            .ExecuteDeleteAsync(ct)
            .ConfigureAwait(false);
        if (count == 0)
        {
            return TypedResults.NotFound();
        }
        return TypedResults.NoContent();
    }
}
