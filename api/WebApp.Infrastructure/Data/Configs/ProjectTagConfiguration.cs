using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectTagConfiguration : IEntityTypeConfiguration<ProjectTag>
{
    public void Configure(EntityTypeBuilder<ProjectTag> builder)
    {
        builder.ToTable("project_tags");
        builder.Property(a => a.ProjectId).ValueGeneratedNever();
        builder.Property(a => a.TagId).ValueGeneratedNever();

        builder.HasKey(a => new { a.ProjectId, a.TagId });
    }
}
