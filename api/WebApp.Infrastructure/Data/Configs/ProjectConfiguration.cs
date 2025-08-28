using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("ProjectHiLoSequence");
        builder.Property(a => a.CreatorId);
        builder.Property(a => a.Name);
        builder.Property(a => a.Identifier).HasMaxLength(32);
        builder.Property(a => a.DeletedTime);

        builder.HasIndex(a => new { a.CreatorId, a.Identifier }).IsUnique();
        builder.HasIndex(a => a.DeletedTime);
        builder.HasKey(a => a.Id);
        builder.HasOne(a => a.Creator).WithMany(a => a.Projects).HasForeignKey(a => a.CreatorId);
        builder.HasQueryFilter(a => a.DeletedTime == null);
    }
}
