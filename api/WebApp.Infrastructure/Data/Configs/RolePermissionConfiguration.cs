using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> builder)
    {
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.Property(a => a.RoleId).ValueGeneratedNever();
        builder.Property(a => a.Permission);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.RoleId, a.Permission }).IsUnique();
        builder.HasOne(a => a.Role).WithMany(a => a.Permissions).HasForeignKey(a => a.RoleId);
    }
}
