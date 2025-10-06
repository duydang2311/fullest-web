using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        builder.ToTable("labels");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("LabelHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.Description);
        builder.Property(a => a.Color);

        builder.HasKey(a => a.Id);
    }
}
