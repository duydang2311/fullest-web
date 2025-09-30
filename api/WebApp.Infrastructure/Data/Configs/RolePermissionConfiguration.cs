using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.ToTable("role_permissions");
        builder.Property(a => a.PermissionId).ValueGeneratedNever();
        builder.Property(a => a.RoleId).ValueGeneratedNever();

        builder.HasKey(a => new { a.PermissionId, a.RoleId });
        builder.HasIndex(a => a.RoleId);
        builder.HasOne(a => a.Role).WithMany(a => a.RolePermissions).HasForeignKey(a => a.RoleId);
        builder.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.PermissionId);
    }
}
