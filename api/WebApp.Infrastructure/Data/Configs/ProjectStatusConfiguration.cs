using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectStatusConfiguration : IEntityTypeConfiguration<ProjectStatus>
{
    public void Configure(EntityTypeBuilder<ProjectStatus> builder)
    {
        builder.ToTable("project_statuses");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("ProjectStatusHiLoSequence");
        builder.Property(a => a.ProjectId).ValueGeneratedNever();
        builder.Property(a => a.StatusId).ValueGeneratedNever();
        builder.Property(a => a.IsDefault);

        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Project).WithMany().HasForeignKey(a => a.ProjectId);
        builder.HasOne(a => a.Status).WithMany().HasForeignKey(a => a.StatusId);
    }
}
