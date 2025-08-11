namespace WebApp.Api.Common.Hashing;

public interface IPasswordHasher
{
    public byte[] Hash(string password);
    public bool Verify(byte[] hash, string password);
}
