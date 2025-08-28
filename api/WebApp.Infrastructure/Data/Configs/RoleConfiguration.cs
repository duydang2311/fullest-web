using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("RoleHiLoSequence");
        builder.Property(a => a.Name);

        builder.HasKey(a => a.Id);
    }
}
