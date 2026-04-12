using System.Text;
using Geralt;

namespace WebApp.Api.Common.Hashing;

public sealed class PasswordHasher : IPasswordHasher
{
    public char[] Hash(string password)
    {
        const int memorySize = 128 * 1024 * 1024;
        Span<char> hash = stackalloc char[Argon2id.HashSize];
        Argon2id.ComputeHash(hash, Encoding.UTF8.GetBytes(password), 3, memorySize);
        return hash.ToArray();
    }

    public bool Verify(char[] hash, string password)
    {
        return Argon2id.VerifyHash(hash, Encoding.UTF8.GetBytes(password));
    }
}
