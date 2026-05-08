using System.Security.Cryptography;
using System.Text;

namespace MyAgenda;

public static class AuthService
{
    // Hash della password con SHA256 + salt
    private static string HashPassword(string password, string salt)
    {
        string salted = salt + password + salt;
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(salted));
        return Convert.ToHexString(bytes);
    }

    // Genera un salt casuale unico per ogni utente
    private static string GenerateSalt()
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToHexString(saltBytes);
    }

    // Registra un nuovo utente
    public static Task<bool> RegisterAsync(string username, string password)
    {
        if (Preferences.ContainsKey($"user_{username}_hash"))
            return Task.FromResult(false); // utente già esistente

        string salt = GenerateSalt();
        string hash = HashPassword(password, salt);

        // Salva hash e salt separatamente
        Preferences.Set($"user_{username}_salt", salt);
        Preferences.Set($"user_{username}_hash", hash);

        return Task.FromResult(true);
    }

    // Verifica le credenziali al login
    public static Task<bool> LoginAsync(string username, string password)
    {
        if (!Preferences.ContainsKey($"user_{username}_hash"))
            return Task.FromResult(false); // utente non trovato

        string salt = Preferences.Get($"user_{username}_salt", "");
        string savedHash = Preferences.Get($"user_{username}_hash", "");
        string inputHash = HashPassword(password, salt);

        return Task.FromResult(savedHash == inputHash);
    }

    // Salva l'utente corrente dopo il login
    public static void SaveCurrentUser(string username)
    {
        Preferences.Set("current_user", username);
    }

    // Restituisce l'utente attualmente loggato
    public static string GetCurrentUser()
    {
        return Preferences.Get("current_user", "");
    }

    // Logout
    public static void Logout()
    {
        Preferences.Remove("current_user");
    }
}
