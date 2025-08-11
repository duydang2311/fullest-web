using System.Text;
using Geralt;

namespace WebApp.Api.Common.Hashing;

public sealed class PasswordHasher : IPasswordHasher
{
    public byte[] Hash(string password)
    {
        const int memorySize = 128 * 1024 * 1024;
        Span<byte> hash = stackalloc byte[Argon2id.MaxHashSize];
        Argon2id.ComputeHash(hash, Encoding.UTF8.GetBytes(password), 3, memorySize);
        return hash.ToArray();
    }

    public bool Verify(byte[] hash, string password)
    {
        return Argon2id.VerifyHash(hash, Encoding.UTF8.GetBytes(password));
    }
}
