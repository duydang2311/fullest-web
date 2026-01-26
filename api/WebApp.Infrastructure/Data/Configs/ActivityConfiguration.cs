using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class ActivityConfiguration : IEntityTypeConfiguration<Activity>
{
    public void Configure(EntityTypeBuilder<Activity> builder)
    {
        builder.ToTable("activities");
        builder.Property(a => a.CreatedTime).HasDefaultValueSql("now()");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("ActivityHiLoSequence");
        builder.Property(a => a.Kind);
        builder.Property(a => a.TaskId).ValueGeneratedNever();
        builder.Property(a => a.ActorId).ValueGeneratedNever();
        builder.Property(a => a.Data);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.TaskId);
        builder.HasIndex(a => a.ActorId);
        builder
            .HasOne(a => a.Task)
            .WithMany(a => a.Activities)
            .HasForeignKey(a => a.TaskId)
            .OnDelete(DeleteBehavior.Cascade);
        builder
            .HasOne(a => a.Actor)
            .WithMany()
            .HasForeignKey(a => a.ActorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
