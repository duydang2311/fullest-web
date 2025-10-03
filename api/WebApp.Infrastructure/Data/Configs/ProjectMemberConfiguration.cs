using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ProjectMemberConfiguration : IEntityTypeConfiguration<ProjectMember>
{
    public void Configure(EntityTypeBuilder<ProjectMember> builder)
    {
        builder.ToTable("project_members");
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("ProjectMemberHiLoSequence");
        builder.Property(a => a.ProjectId);
        builder.Property(a => a.UserId);
        builder.Property(a => a.RoleId);

        builder.HasIndex(a => new { a.ProjectId, a.UserId }).IsUnique();
        builder.HasKey(a => a.Id);
        builder
            .HasOne(a => a.Project)
            .WithMany(a => a.ProjectMembers)
            .HasForeignKey(a => a.ProjectId);
        builder.HasOne(a => a.User).WithMany(a => a.ProjectMembers).HasForeignKey(a => a.UserId);
        builder.HasOne(a => a.Role).WithMany().HasForeignKey(a => a.RoleId);
        builder.HasQueryFilter(a => a.Project.DeletedTime == null);
    }
}
