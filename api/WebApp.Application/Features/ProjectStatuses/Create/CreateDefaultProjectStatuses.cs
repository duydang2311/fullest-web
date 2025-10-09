using FractionalIndexing;
using WebApp.Application.Data;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.ProjectStatuses.Create;

public sealed class CreateDefaultProjectStatuses(IAppDbContext db) : IProjectCreatedHandler
{
    public const string BASE_95_DIGITS =
        " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_`abcdefghijklmnopqrstuvwxyz{|}~";

    public async Task HandleAsync(ProjectCreated created, CancellationToken ct)
    {
        var ranks = OrderKeyGenerator.GenerateNKeysBetween(
            null,
            null,
            StatusDefaults.All.Length,
            BASE_95_DIGITS
        );
        await db.ProjectStatuses.AddRangeAsync(
            StatusDefaults.All.Select(
                (a, i) =>
                    new ProjectStatus
                    {
                        ProjectId = created.ProjectId,
                        Status = new Status
                        {
                            Name = a.Name,
                            Color = a.Color,
                            Category = a.Category,
                            Description = a.Description,
                            Rank = ranks[i],
                        },
                    }
            ),
            ct
        );
    }
}
