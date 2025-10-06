using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class StatusConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.ToTable("statuses");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("StatusHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.Category);
        builder.Property(a => a.Color);
        builder.Property(a => a.Description);

        builder.HasKey(a => a.Id);
    }
}
