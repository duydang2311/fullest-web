using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Domain.Entities;

namespace WebApp.Infrastructure.Data.Configs;

public sealed class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("comments");
        builder.Property(a => a.Id).ValueGeneratedOnAdd().UseHiLo("CommentHiLoSequence");
        builder.Property(a => a.TaskId).ValueGeneratedNever();
        builder.Property(a => a.AuthorId).ValueGeneratedNever();
        builder.Property(a => a.ContentJson).HasColumnType("jsonb");
        builder.Property(a => a.ContentPreview);
        builder.Property(a => a.DeletedTime);

        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.DeletedTime);
        builder.HasOne(a => a.Task).WithMany(a => a.Comments).HasForeignKey(a => a.TaskId);
        builder
            .HasOne<TaskEntity>()
            .WithOne(a => a.InitialComment)
            .HasForeignKey<TaskEntity>(a => a.InitialCommentId);
        builder.HasOne(a => a.Author).WithMany().HasForeignKey(a => a.AuthorId);
    }
}
