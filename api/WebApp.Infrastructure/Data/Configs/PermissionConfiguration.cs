using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("PermissionHiLoSequence");
        builder.Property(a => a.Name);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.Name).IsUnique();
    }
}
