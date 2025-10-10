using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class PriorityConfiguration : IEntityTypeConfiguration<Priority>
{
    public void Configure(EntityTypeBuilder<Priority> builder)
    {
        builder.ToTable("priorities");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("PriorityHiLoSequence");
        builder.Property(a => a.Name);
        builder.Property(a => a.Color);
        builder.Property(a => a.Description);
        builder.Property(a => a.Rank).UseCollation("C");

        builder.HasOne(a => a.Project).WithMany(a => a.Priorities).HasForeignKey(a => a.ProjectId);
        builder.HasKey(a => a.Id);
    }
}
