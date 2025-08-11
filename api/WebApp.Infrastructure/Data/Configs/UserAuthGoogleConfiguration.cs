using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserAuthGoogleConfiguration : IEntityTypeConfiguration<UserAuthGoogle>
{
    public void Configure(EntityTypeBuilder<UserAuthGoogle> builder)
    {
        builder.Property(a => a.GoogleId);
    }
}
