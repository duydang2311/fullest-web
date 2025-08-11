using System.ComponentModel.DataAnnotations;

namespace WebApp.Api.Common.Codecs;

public sealed record NumberEncoderOptions
{
    public const string Section = "NumberEncoder";

    [Required]
    public required string Alphabet { get; init; }

    [Required]
    public required int MinLength { get; init; }
}
