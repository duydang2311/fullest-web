using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class NamespaceConfiguration : IEntityTypeConfiguration<Namespace>
{
    public void Configure(EntityTypeBuilder<Namespace> builder)
    {
        builder.ToTable("namespaces");
        builder.Property(a => a.Id);
        builder.Property(a => a.Kind);
        builder.Property(a => a.UserId);
        builder.Ignore(a => a.IsUserNamespace);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.UserId).IsUnique();
        builder.HasOne(a => a.User).WithOne().HasForeignKey<Namespace>(a => a.UserId);
        builder.HasQueryFilter(a => a.User!.DeletedTime == null);
    }
}
