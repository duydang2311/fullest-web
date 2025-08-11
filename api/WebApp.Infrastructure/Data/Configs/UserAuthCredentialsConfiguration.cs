using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class UserAuthCredentialsConfiguration : IEntityTypeConfiguration<UserAuthCredentials>
{
    public void Configure(EntityTypeBuilder<UserAuthCredentials> builder)
    {
        builder.Property(a => a.Hash);
    }
}
