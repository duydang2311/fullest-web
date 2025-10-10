using FractionalIndexing;
using Microsoft.EntityFrameworkCore;
using WebApp.Application.Data;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.Priorities.Create;

public sealed class CreateDefaultPriorities(BaseDbContext db) : IProjectCreatedHandler
{
    public const string BASE_95_DIGITS =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

    public async Task HandleAsync(ProjectCreated created, CancellationToken ct)
    {
        var ranks = OrderKeyGenerator.GenerateNKeysBetween(
            null,
            null,
            PriorityDefaults.All.Length,
            BASE_95_DIGITS
        );
        var priorities = PriorityDefaults
            .All.Select(
                (a, i) =>
                    new Priority
                    {
                        ProjectId = created.ProjectId,
                        Name = a.Name,
                        Color = a.Color,
                        Description = a.Description,
                        Rank = ranks[i],
                    }
            )
            .ToList();
        await db.Priorities.AddRangeAsync(priorities, ct);
        await db.SaveChangesAsync(ct).ConfigureAwait(false);
        await db
            .Projects.Where(a => a.Id == created.ProjectId)
            .ExecuteUpdateAsync(
                a => a.SetProperty(b => b.DefaultPriorityId, b => priorities[0].Id),
                ct
            );
    }
}
