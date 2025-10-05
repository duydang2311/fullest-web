using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
{
    public void Configure(EntityTypeBuilder<TaskEntity> builder)
    {
        builder.ToTable("tasks");
        builder.Property(a => a.CreatedTime);
        builder.Property(a => a.UpdatedTime);
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("TaskHiLoSequence");
        builder.Property(a => a.ProjectId).ValueGeneratedNever();
        builder.Property(a => a.PublicId).ValueGeneratedOnAdd().UseHiLo("TaskPublicIdHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.Description);
        builder.Property(a => a.DeletedTime);

        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Project).WithMany(a => a.Tasks).HasForeignKey(a => a.ProjectId);
        builder.HasOne(a => a.Author).WithMany().HasForeignKey(a => a.AuthorId);
        builder
            .HasMany(a => a.Assignees)
            .WithMany()
            .UsingEntity<TaskEntityAssignee>(
                r => r.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId),
                l => l.HasOne(a => a.Task).WithMany().HasForeignKey(a => a.TaskId)
            );
        builder.HasQueryFilter(a => a.DeletedTime == null && a.Project.DeletedTime == null);
    }
}
