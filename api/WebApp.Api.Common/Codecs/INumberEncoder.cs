namespace WebApp.Api.Common.Codecs;

public interface INumberEncoder
{
    string Encode(long number);
    string Encode(int number);
    bool TryDecode(string encoded, out int value);
    bool TryDecode(string encoded, out long value);
}
