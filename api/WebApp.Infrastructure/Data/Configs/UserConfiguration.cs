using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("UserHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.DisplayName);
        builder.Property(b => b.ImageKey);
        builder.Property(b => b.ImageVersion);
        builder.Property(a => a.DeletedTime);

        builder.Property(a => a.DeletedTime);
        builder
            .HasIndex(a => a.Name, "ix_users_name_unique")
            .IsUnique()
            .HasDatabaseName("ix_users_name_unique");
        builder
            .HasIndex(a => a.Name, "ix_users_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("ix_users_name_trgm");
        builder
            .HasIndex(a => a.DisplayName, "ix_users_display_name_trgm")
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops")
            .HasDatabaseName("ix_users_display_name_trgm");
        builder.HasIndex(a => a.DeletedTime);
        builder.HasKey(a => a.Id);
    }
}
