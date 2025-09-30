using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Constants;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserAuthConfiguration : IEntityTypeConfiguration<UserAuth>
{
    public void Configure(EntityTypeBuilder<UserAuth> builder)
    {
        builder.ToTable("user_auths");
        builder.UseTphMappingStrategy();
        builder
            .HasDiscriminator(a => a.Provider)
            .HasValue<UserAuthCredentials>(AuthProvider.Credentials)
            .HasValue<UserAuthGoogle>(AuthProvider.Google);

        builder.Property(a => a.Id).UseHiLo("UserAuthHiLoSequence");
        builder.Property(a => a.UserId).ValueGeneratedNever();
        builder.Property(a => a.Provider);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => new { a.UserId, a.Provider }).IsUnique();
        builder.HasOne(a => a.User).WithMany(a => a.Auths).HasForeignKey(a => a.UserId);
        builder.HasQueryFilter(a => a.User.DeletedTime == null);
    }
}
