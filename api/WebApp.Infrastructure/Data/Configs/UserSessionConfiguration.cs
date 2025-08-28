using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("UserSessionHiLoSequence");
        builder.Property(a => a.UserId).ValueGeneratedNever();
        builder.Property(a => a.Token);

        builder.HasIndex(a => new { a.UserId, a.Token }).IsUnique();
        builder.HasKey(a => a.Id);
        builder.HasQueryFilter(a => a.User.DeletedTime == null);
    }
}
