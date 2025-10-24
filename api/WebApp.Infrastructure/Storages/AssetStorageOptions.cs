using System.ComponentModel.DataAnnotations;

namespace WebApp.Infrastructure.Storages;

public sealed record AssetStorageOptions
{
    public const string Section = "AssetStorage";

    [Required]
    public required string SigningPrivateKeyPem { get; init; }

    [Required]
    public required string SigningPublicKeyPem { get; init; }
}
