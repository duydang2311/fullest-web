using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectTaskCounterConfiguration : IEntityTypeConfiguration<ProjectTaskCounter>
{
    public void Configure(EntityTypeBuilder<ProjectTaskCounter> builder)
    {
        builder.ToTable("project_task_counters");
        builder.Property(a => a.ProjectId).ValueGeneratedNever();
        builder.Property(a => a.Count);
        builder.HasKey(a => a.ProjectId);
    }
}
