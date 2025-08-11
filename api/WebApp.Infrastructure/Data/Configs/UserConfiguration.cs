using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("UserHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.DeletedTime);

        builder.HasIndex(a => a.Name).IsUnique();
        builder.HasIndex(a => a.DeletedTime);
        builder.HasKey(a => a.Id);
        builder.HasQueryFilter(a => a.DeletedTime == null);
    }
}
