namespace WebApp.Api.Common.Hashing;

public interface IPasswordHasher
{
    public char[] Hash(string password);
    public bool Verify(char[] hash, string password);
}
