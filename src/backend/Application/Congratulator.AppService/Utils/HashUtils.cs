namespace Congratulator.AppService.Utils;

public static class HashUtils
{
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    public static bool VerifyPassword(string userInputPassword, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(userInputPassword, hashedPassword);
    }
}
