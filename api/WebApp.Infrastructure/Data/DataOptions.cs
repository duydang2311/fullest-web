using System.ComponentModel.DataAnnotations;

namespace WebApp.Infrastructure.Data;

public sealed record DataOptions
{
    public const string Section = "Data";

    [Required]
    public required string ConnectionString { get; init; }

    [Required]
    public required string MigrationsAssembly { get; init; }
}
