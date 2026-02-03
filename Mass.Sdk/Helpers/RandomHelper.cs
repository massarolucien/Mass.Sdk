using System.Security.Cryptography;
using System.Text;

namespace Mass.Sdk.Helpers;

public static class RandomHelper
{
    public static string GetString(int length, string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789")
    {
        if (length <= 0) return string.Empty;
        var charsLength = chars.Length;
        if (charsLength == 0)
            throw new ArgumentException("Character set cannot be empty.", nameof(chars));

        var sb = new StringBuilder(length);

        Span<byte> buffer = stackalloc byte[length];

        RandomNumberGenerator.Fill(buffer);

        for (var i = 0; i < length; i++)
        {
            sb.Append(chars[buffer[i] % charsLength]);
        }

        return sb.ToString();
    }
}