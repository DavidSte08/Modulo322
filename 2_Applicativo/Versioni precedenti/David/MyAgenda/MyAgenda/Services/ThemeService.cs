namespace MyAgenda.Services;

/// <summary>
/// Gestisce il caricamento e l'applicazione delle preferenze visive salvate.
/// Va chiamato all'avvio dell'app e ogni volta che l'utente salva le impostazioni.
/// </summary>
public static class ThemeService
{
    // ── Chiavi Preferences ─────────────────────────────────────────────────

    private const string KeyColori = "stile_colori";
    private const string KeyFont = "stile_font";
    private const string KeySuono = "suono_notifiche";

    // ── Valori di default ──────────────────────────────────────────────────

    public const string DefaultColori = "Predefinito";
    public const string DefaultFont = "Normale";
    public const string DefaultSuono = "Predefinito";

    // ── Proprietà correnti ─────────────────────────────────────────────────

    public static string StileColori => Preferences.Get(KeyColori, DefaultColori);
    public static string StileFont => Preferences.Get(KeyFont, DefaultFont);
    public static string SuonoNotifiche => Preferences.Get(KeySuono, DefaultSuono);

    // ── Salvataggio ────────────────────────────────────────────────────────

    public static void SalvaColori(string valore)
    {
        Preferences.Set(KeyColori, valore);
        ApplicaColori(valore);
    }

    public static void SalvaFont(string valore)
    {
        Preferences.Set(KeyFont, valore);
        ApplicaFont(valore);
    }

    public static void SalvaSuono(string valore)
    {
        Preferences.Set(KeySuono, valore);
        // Il suono verrà applicato al momento delle notifiche
    }

    // ── Applica tutte le preferenze salvate (da chiamare all'avvio) ────────

    public static void ApplicaTutto()
    {
        ApplicaColori(StileColori);
        ApplicaFont(StileFont);
    }

    // ── Applica tema colori ────────────────────────────────────────────────

    public static void ApplicaColori(string tema)
    {
        var dict = Application.Current?.Resources;
        if (dict == null) return;

        switch (tema)
        {
            case "Oceano":
                dict["Primary"] = Color.FromArgb("#0891B2");        // Ciano oceano
                dict["PrimaryDark"] = Color.FromArgb("#0E7490");
                dict["PrimaryLight"] = Color.FromArgb("#ECFEFF");
                dict["Secondary"] = Color.FromArgb("#0F766E");      // Verde acqua
                dict["SecondaryLight"] = Color.FromArgb("#F0FDFA");
                dict["Accent"] = Color.FromArgb("#0284C7");
                dict["AccentLight"] = Color.FromArgb("#E0F2FE");
                break;

            case "Foresta":
                dict["Primary"] = Color.FromArgb("#16A34A");        // Verde foresta
                dict["PrimaryDark"] = Color.FromArgb("#15803D");
                dict["PrimaryLight"] = Color.FromArgb("#F0FDF4");
                dict["Secondary"] = Color.FromArgb("#65A30D");      // Verde lime
                dict["SecondaryLight"] = Color.FromArgb("#F7FEE7");
                dict["Accent"] = Color.FromArgb("#059669");
                dict["AccentLight"] = Color.FromArgb("#ECFDF5");
                break;

            case "Tramonto":
                dict["Primary"] = Color.FromArgb("#EA580C");        // Arancione tramonto
                dict["PrimaryDark"] = Color.FromArgb("#C2410C");
                dict["PrimaryLight"] = Color.FromArgb("#FFF7ED");
                dict["Secondary"] = Color.FromArgb("#DB2777");      // Rosa acceso
                dict["SecondaryLight"] = Color.FromArgb("#FDF2F8");
                dict["Accent"] = Color.FromArgb("#D97706");
                dict["AccentLight"] = Color.FromArgb("#FFFBEB");
                break;

            default: // "Predefinito"
                dict["Primary"] = Color.FromArgb("#2563EB");
                dict["PrimaryDark"] = Color.FromArgb("#1D4ED8");
                dict["PrimaryLight"] = Color.FromArgb("#EEF2FF");
                dict["Secondary"] = Color.FromArgb("#7C3AED");
                dict["SecondaryLight"] = Color.FromArgb("#F5F3FF");
                dict["Accent"] = Color.FromArgb("#059669");
                dict["AccentLight"] = Color.FromArgb("#ECFDF5");
                break;
        }
    }

    // ── Applica dimensione font ────────────────────────────────────────────

    public static void ApplicaFont(string dimensione)
    {
        var dict = Application.Current?.Resources;
        if (dict == null) return;

        // Scala tutti i font dell'app in base alla scelta
        switch (dimensione)
        {
            case "Piccolo":
                dict["FontSizeBody"] = 12.0;
                dict["FontSizeLabel"] = 11.0;
                dict["FontSizeTitle"] = 24.0;
                dict["FontSizeCard"] = 15.0;
                break;

            case "Grande":
                dict["FontSizeBody"] = 17.0;
                dict["FontSizeLabel"] = 15.0;
                dict["FontSizeTitle"] = 32.0;
                dict["FontSizeCard"] = 21.0;
                break;

            default: // "Normale"
                dict["FontSizeBody"] = 14.0;
                dict["FontSizeLabel"] = 13.0;
                dict["FontSizeTitle"] = 28.0;
                dict["FontSizeCard"] = 18.0;
                break;
        }
    }
}
