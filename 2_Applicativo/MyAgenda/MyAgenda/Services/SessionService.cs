namespace MyAgenda.Services;

/// <summary>
/// Gestisce la sessione dell'utente correntemente loggato.
/// Sostituisce la classe statica Sessione con un servizio più robusto.
/// </summary>
public static class SessionService
{
    private static string _username = string.Empty;

    public static string Username
    {
        get => _username;
        private set => _username = value ?? string.Empty;
    }

    public static bool IsLoggedIn => !string.IsNullOrEmpty(_username);

    /// <summary>
    /// Avvia una nuova sessione per l'utente specificato.
    /// </summary>
    public static void Start(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username non può essere vuoto.", nameof(username));

        Username = username.Trim();
    }

    /// <summary>
    /// Termina la sessione corrente e pulisce i dati in memoria.
    /// </summary>
    public static void End()
    {
        Username = string.Empty;
    }
}
