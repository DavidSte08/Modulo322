using System.Text.Json;

namespace MyAgenda.Services;

/// <summary>
/// Servizio centralizzato per la persistenza dei dati su file JSON.
/// Gestisce errori di I/O in modo uniforme e fornisce percorsi consistenti.
/// </summary>
public static class StorageService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false // file più compatti
    };

    /// <summary>
    /// Costruisce il percorso del file dati per l'utente corrente.
    /// </summary>
    public static string GetFilePath(string suffix)
    {
        string username = SessionService.Username;
        if (string.IsNullOrEmpty(username))
            throw new InvalidOperationException("Nessun utente in sessione.");

        return Path.Combine(FileSystem.AppDataDirectory, $"{username}_{suffix}.json");
    }

    /// <summary>
    /// Salva un oggetto serializzato come JSON. Restituisce true se ha successo.
    /// </summary>
    public static bool Save<T>(string suffix, T data)
    {
        try
        {
            string path = GetFilePath(suffix);
            string json = JsonSerializer.Serialize(data, JsonOptions);
            File.WriteAllText(path, json);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[StorageService] Save error ({suffix}): {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Carica e deserializza un oggetto da file JSON.
    /// Restituisce il default se il file non esiste o c'è un errore.
    /// </summary>
    public static T? Load<T>(string suffix)
    {
        try
        {
            string path = GetFilePath(suffix);
            if (!File.Exists(path)) return default;

            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<T>(json, JsonOptions);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[StorageService] Load error ({suffix}): {ex.Message}");
            return default;
        }
    }

    /// <summary>
    /// Rinomina i file dati quando l'utente cambia username.
    /// </summary>
    public static void MigraFile(string vecchioUsername, string nuovoUsername)
    {
        string dir = FileSystem.AppDataDirectory;
        foreach (string file in Directory.GetFiles(dir, $"{vecchioUsername}_*.json"))
        {
            string nomefile = Path.GetFileName(file);
            string nuovoNome = nomefile.Replace(vecchioUsername + "_", nuovoUsername + "_");
            string nuovoPath = Path.Combine(dir, nuovoNome);
            try { File.Move(file, nuovoPath, overwrite: true); }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[StorageService] Migra error: {ex.Message}");
            }
        }
    }
}
