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
        builder.Property(a => a.PublicId).ValueGeneratedOnAdd();
        builder.Property(a => a.Title);
        builder.Property(a => a.Description);
        builder.Property(a => a.DeletedTime);
        builder.Property(a => a.DueTime);
        builder.Property(a => a.DueTz);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.DeletedTime);
        builder.HasOne(a => a.Project).WithMany(a => a.Tasks).HasForeignKey(a => a.ProjectId);
        builder.HasOne(a => a.Author).WithMany().HasForeignKey(a => a.AuthorId);
        builder
            .HasMany(a => a.Assignees)
            .WithMany()
            .UsingEntity<TaskEntityAssignee>(
                r => r.HasOne(a => a.User).WithMany().HasForeignKey(a => a.UserId),
                l => l.HasOne(a => a.Task).WithMany().HasForeignKey(a => a.TaskId)
            );
        builder
            .HasMany(a => a.Labels)
            .WithMany()
            .UsingEntity<TaskLabel>(
                r => r.HasOne(a => a.Label).WithMany().HasForeignKey(a => a.LabelId),
                l => l.HasOne(a => a.Task).WithMany().HasForeignKey(a => a.TaskId)
            );
        builder.HasOne(a => a.Status).WithMany().HasForeignKey(a => a.StatusId);
        builder.HasOne(a => a.Priority).WithMany().HasForeignKey(a => a.PriorityId);
    }
}
