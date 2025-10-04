using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("TagHiLoSequence");
        builder.Property(a => a.Value);

        builder.HasKey(a => a.Id);
    }
}
