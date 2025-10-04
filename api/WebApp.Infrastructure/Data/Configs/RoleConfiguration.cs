using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("roles");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("RoleHiLoSequence");
        builder.Property(a => a.Name);

        builder
            .HasMany(a => a.Permissions)
            .WithMany()
            .UsingEntity<RolePermission>(
                r => r.HasOne(a => a.Permission).WithMany().HasForeignKey(a => a.PermissionId),
                l => l.HasOne(a => a.Role).WithMany().HasForeignKey(a => a.RoleId)
            );
        builder.HasKey(a => a.Id);
    }
}
