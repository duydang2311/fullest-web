using WebApp.Application.Data;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;
using WebApp.Domain.Events;

namespace WebApp.Application.Features.ProjectMembers.Create;

public sealed class AddProjectCreatorAsProjectOwner(IAppDbContext db) : IProjectCreatedHandler
{
    public async Task HandleAsync(ProjectCreated created, CancellationToken ct)
    {
        await db.ProjectMembers.AddAsync(
            new ProjectMember
            {
                ProjectId = created.ProjectId,
                UserId = created.CreatorId,
                RoleId = ProjectRoleDefaults.Owner.Id,
            },
            ct
        );
    }
}
