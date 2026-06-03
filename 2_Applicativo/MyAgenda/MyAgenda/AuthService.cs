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

    // METODO DA AGGIUNGERE: Modifica l'username dell'utente corrente
    public static bool CambiaUsername(string nuovoUsername)
    {
        string utenteAttuale = GetCurrentUser();

        // Controlli di sicurezza di base
        if (string.IsNullOrEmpty(utenteAttuale)) return false;
        if (utenteAttuale == nuovoUsername) return true;
        if (Preferences.ContainsKey($"user_{nuovoUsername}_hash")) return false; // Nome già occupato

        // 1. Recupera i vecchi dati esistenti
        string salt = Preferences.Get($"user_{utenteAttuale}_salt", "");
        string hash = Preferences.Get($"user_{utenteAttuale}_hash", "");

        // 2. Salva i dati sotto il nuovo nome (n)
        Preferences.Set($"user_{nuovoUsername}_salt", salt);
        Preferences.Set($"user_{nuovoUsername}_hash", hash);

        // 3. Cancella i vecchi dati per non lasciare spazzatura
        Preferences.Remove($"user_{utenteAttuale}_salt");
        Preferences.Remove($"user_{utenteAttuale}_hash");

        // 4. Aggiorna l'utente della sessione con il nuovo nome
        SaveCurrentUser(nuovoUsername);

        return true;
    }

    public static bool CambiaPassword(string vecchiaPassword, string nuovaPassword)
    {
        string utenteAttuale = GetCurrentUser();

        // 1. Verifica che ci sia un utente loggato
        if (string.IsNullOrEmpty(utenteAttuale)) return false;

        // 2. Recupera salt e hash attuali per il controllo
        string saltAttuale = Preferences.Get($"user_{utenteAttuale}_salt", "");
        string hashAttuale = Preferences.Get($"user_{utenteAttuale}_hash", "");

        // 3. Calcola l'hash della vecchia password inserita per vedere se coincide
        string hashVerifica = HashPassword(vecchiaPassword, saltAttuale);

        if (hashAttuale != hashVerifica)
        {
            return false; // La vecchia password è errata, nega la modifica
        }

        // 4. Se la vecchia password è corretta, genera un nuovo salt e il nuovo hash
        string nuovoSalt = GenerateSalt();
        string nuovoHash = HashPassword(nuovaPassword, nuovoSalt);

        // 5. Sovrascrivi i valori esistenti con i nuovi
        Preferences.Set($"user_{utenteAttuale}_salt", nuovoSalt);
        Preferences.Set($"user_{utenteAttuale}_hash", nuovoHash);

        return true;
    }

    // Logout
    public static void Logout()
    {
        Preferences.Remove("current_user");
    }
}
