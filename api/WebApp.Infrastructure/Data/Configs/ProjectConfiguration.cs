using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("ProjectHiLoSequence");
        builder.Property(a => a.NamespaceId);
        builder.Property(a => a.Name);
        builder.Property(a => a.Identifier);
        builder.Property(a => a.Summary);
        builder.Property(a => a.About);
        builder.Property(a => a.DeletedTime);

        builder.HasIndex(a => new { a.NamespaceId, a.Identifier }).IsUnique();
        builder.HasIndex(a => a.DeletedTime);
        builder.HasKey(a => a.Id);
        builder
            .HasOne(a => a.Namespace)
            .WithMany(a => a.Projects)
            .HasForeignKey(a => a.NamespaceId);
        builder.HasQueryFilter(a => a.DeletedTime == null);
    }
}
