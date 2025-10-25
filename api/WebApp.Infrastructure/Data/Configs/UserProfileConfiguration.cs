using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("user_profiles");
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.UserId).ValueGeneratedNever();
        builder.Property(a => a.DisplayName);
        builder.Property(b => b.ImageKey);
        builder.Property(b => b.ImageVersion);

        builder.HasKey(a => a.UserId);
        builder
            .HasOne(a => a.User)
            .WithOne(a => a.Profile)
            .HasForeignKey<UserProfile>(a => a.UserId);
    }
}
