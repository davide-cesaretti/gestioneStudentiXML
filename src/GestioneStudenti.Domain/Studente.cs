namespace GestioneStudenti.Domain;

/// <summary>
/// Entità di dominio. Non sa NULLA di file, JSON o database:
/// contiene solo i dati e le regole che li rendono validi.
/// </summary>
public class Studente
{
    public int Id { get; }
    public string Nome { get; private set; }
    public string Cognome { get; private set; }
    public string Classe { get; private set; }

    public Studente(int id, string nome, string cognome, string classe)
    {
        if (id <= 0)
            throw new ArgumentException("L'id deve essere un numero positivo.", nameof(id));

        Id = id;
        Nome = Obbligatorio(nome, "nome");
        Cognome = Obbligatorio(cognome, "cognome");
        Classe = ValidaClasse(classe);
    }

    /// <summary>Regola di dominio: lo studente può cambiare classe solo verso una classe valida.</summary>
    public void CambiaClasse(string nuovaClasse)
    {
        Classe = ValidaClasse(nuovaClasse);
    }

    public override string ToString()
    {
        return $"[{Id}] {Cognome} {Nome} - {Classe}";
    }

    private static string Obbligatorio(string valore, string campo)
    {
        if (string.IsNullOrWhiteSpace(valore))
            throw new ArgumentException($"Il campo '{campo}' è obbligatorio.", campo);
        return valore.Trim();
    }

    // Formato atteso: anno da 1 a 5 seguito da una o più lettere (es. 3I, 4E, 5AINF)
    private static string ValidaClasse(string classe)
    {
        classe = Obbligatorio(classe, "classe").ToUpperInvariant();

        bool valida = classe.Length >= 2 && classe[0] >= '1' && classe[0] <= '5';

        // dal secondo carattere in poi devono esserci solo lettere
        for (int i = 1; i < classe.Length && valida; i++)
        {
            if (!char.IsLetter(classe[i]))
                valida = false;
        }

        if (!valida)
            throw new ArgumentException($"Classe '{classe}' non valida (esempio corretto: 3I).", nameof(classe));

        return classe;
    }
}
