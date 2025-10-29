using System.ComponentModel.DataAnnotations;

namespace WebApp.Infrastructure.Integrations;

public sealed record IntegrationKeysOptions
{
    public const string Section = "IntegrationKeys";

    public required ServiceOptions AssetWorkers { get; init; }

    public sealed record ServiceOptions
    {
        [Required]
        public required string Name { get; init; }

        [Required]
        public required string Issuer { get; init; }

        [Required]
        public required string Audience { get; init; }

        [Required]
        public required string PrivateKeyPem { get; init; }

        [Required]
        public required string PublicKeyPem { get; init; }
    }
}
