namespace MyAgenda.Services;

/// <summary>
/// Centralizza le regole di validazione della password.
/// Usato sia in Register che in Account (cambio password).
/// </summary>
public static class PasswordValidator
{
    private const string SpecialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

    /// <summary>
    /// Valida la password secondo le regole di sicurezza.
    /// Restituisce null se valida, altrimenti il messaggio di errore.
    /// </summary>
    public static string? Validate(string password)
    {
        if (string.IsNullOrEmpty(password))
            return "La password non può essere vuota.";

        if (password.Length < 8)
            return "La password deve avere almeno 8 caratteri.";

        if (password.Length > 64)
            return "La password non può superare i 64 caratteri.";

        if (!password.Any(char.IsUpper))
            return "La password deve contenere almeno una lettera maiuscola.";

        if (!password.Any(char.IsLower))
            return "La password deve contenere almeno una lettera minuscola.";

        if (!password.Any(char.IsDigit))
            return "La password deve contenere almeno un numero.";

        if (!password.Any(c => SpecialChars.Contains(c)))
            return "La password deve contenere almeno un carattere speciale (es. !, @, #, $).";

        return null; // valida
    }

    /// <summary>
    /// Testo con i requisiti da mostrare all'utente nell'UI.
    /// </summary>
    public static string RequisitiTesto =>
        "• Minimo 8 caratteri\n" +
        "• Almeno una maiuscola (A-Z)\n" +
        "• Almeno una minuscola (a-z)\n" +
        "• Almeno un numero (0-9)\n" +
        "• Almeno un carattere speciale (!@#$...)";
}
