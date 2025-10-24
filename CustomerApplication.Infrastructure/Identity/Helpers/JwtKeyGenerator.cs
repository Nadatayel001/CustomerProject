using System.Security.Cryptography;

public static class JwtKeyGenerator
{
    public static string GenerateKey()
    {
        var keyBytes = RandomNumberGenerator.GetBytes(32); 
        return Convert.ToBase64String(keyBytes);
    }
    public static byte[] GenerateKeyBytes()
    {
        return RandomNumberGenerator.GetBytes(32); 
    }
}
