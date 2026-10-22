using System.Security.Cryptography;
using System.Text;

namespace Saldoa.Application.Auth;

public static class RefreshTokenCrypto
{
    public static string GenerateToken() //client
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Base64UrlEncode(bytes);
    }
    
    public static string HashToken(string token) //banco
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes);
    }
    
    private static string Base64UrlEncode(byte[] bytes)
        => Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
}