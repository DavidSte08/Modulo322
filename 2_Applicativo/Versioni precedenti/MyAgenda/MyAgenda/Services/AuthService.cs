using System.Security.Cryptography;
using System.Text;

namespace MyAgenda.Services;

/// <summary>
/// Gestisce autenticazione e gestione account utente.
/// Le password sono hashate con SHA-256 + salt casuale per ogni utente.
/// </summary>
public static class AuthService
{
    // ── Hashing ────────────────────────────────────────────────────────────

    private static string HashPassword(string password, string salt)
    {
        string salted = salt + password + salt;
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(salted));
        return Convert.ToHexString(bytes);
    }

    private static string GenerateSalt()
    {
        byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToHexString(saltBytes);
    }

    // ── Chiavi Preferences ─────────────────────────────────────────────────

    private static string SaltKey(string username) => $"user_{username}_salt";
    private static string HashKey(string username) => $"user_{username}_hash";

    // ── API pubblica ───────────────────────────────────────────────────────

    /// <summary>
    /// Verifica se un utente con questo username esiste già.
    /// </summary>
    public static bool UserExists(string username)
        => Preferences.ContainsKey(HashKey(username));

    /// <summary>
    /// Registra un nuovo utente. Restituisce false se l'username è già in uso.
    /// </summary>
    public static Task<bool> RegisterAsync(string username, string password)
    {
        if (UserExists(username))
            return Task.FromResult(false);

        string salt = GenerateSalt();
        string hash = HashPassword(password, salt);

        Preferences.Set(SaltKey(username), salt);
        Preferences.Set(HashKey(username), hash);

        return Task.FromResult(true);
    }

    /// <summary>
    /// Verifica le credenziali. Restituisce true se sono corrette.
    /// </summary>
    public static Task<bool> LoginAsync(string username, string password)
    {
        if (!UserExists(username))
            return Task.FromResult(false);

        string salt = Preferences.Get(SaltKey(username), string.Empty);
        string savedHash = Preferences.Get(HashKey(username), string.Empty);
        string inputHash = HashPassword(password, salt);

        return Task.FromResult(savedHash == inputHash);
    }

    /// <summary>
    /// Cambia l'username dell'utente corrente.
    /// Restituisce false se il nuovo username è già occupato.
    /// </summary>
    public static bool CambiaUsername(string usernameAttuale, string nuovoUsername)
    {
        if (string.IsNullOrWhiteSpace(nuovoUsername)) return false;
        if (usernameAttuale == nuovoUsername) return true;
        if (UserExists(nuovoUsername)) return false;

        string salt = Preferences.Get(SaltKey(usernameAttuale), string.Empty);
        string hash = Preferences.Get(HashKey(usernameAttuale), string.Empty);

        Preferences.Set(SaltKey(nuovoUsername), salt);
        Preferences.Set(HashKey(nuovoUsername), hash);

        Preferences.Remove(SaltKey(usernameAttuale));
        Preferences.Remove(HashKey(usernameAttuale));

        return true;
    }

    /// <summary>
    /// Cambia la password dell'utente corrente dopo aver verificato quella vecchia.
    /// </summary>
    public static bool CambiaPassword(string username, string vecchiaPassword, string nuovaPassword)
    {
        if (!UserExists(username)) return false;

        string saltAttuale = Preferences.Get(SaltKey(username), string.Empty);
        string hashAttuale = Preferences.Get(HashKey(username), string.Empty);

        if (HashPassword(vecchiaPassword, saltAttuale) != hashAttuale)
            return false;

        string nuovoSalt = GenerateSalt();
        string nuovoHash = HashPassword(nuovaPassword, nuovoSalt);

        Preferences.Set(SaltKey(username), nuovoSalt);
        Preferences.Set(HashKey(username), nuovoHash);

        return true;
    }
}
