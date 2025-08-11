using Microsoft.Extensions.Options;
using Sqids;

namespace WebApp.Api.Common.Codecs;

public sealed class NumberEncoder(IOptions<NumberEncoderOptions> numberEncoderOptions)
    : INumberEncoder
{
    private readonly SqidsEncoder<int> intEncoder = new(
        new SqidsOptions
        {
            Alphabet = numberEncoderOptions.Value.Alphabet,
            MinLength = numberEncoderOptions.Value.MinLength,
        }
    );

    private readonly SqidsEncoder<long> longEncoder = new(
        new SqidsOptions
        {
            Alphabet = numberEncoderOptions.Value.Alphabet,
            MinLength = numberEncoderOptions.Value.MinLength,
        }
    );

    public string Encode(int number)
    {
        return intEncoder.Encode(number);
    }

    public string Encode(long number)
    {
        return longEncoder.Encode(number);
    }

    public bool TryDecode(string encoded, out int value)
    {
        var decoded = intEncoder.Decode(encoded);
        if (decoded.Count == 0)
        {
            value = default;
            return false;
        }
        value = decoded[0];
        return true;
    }

    public bool TryDecode(string encoded, out long value)
    {
        var decoded = longEncoder.Decode(encoded);
        if (decoded.Count == 0)
        {
            value = default;
            return false;
        }
        value = decoded[0];
        return true;
    }
}
