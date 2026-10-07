using System.Text.Encodings.Web;
using System.Text.Json;
using GestioneStudenti.Application;
using GestioneStudenti.Domain;

namespace GestioneStudenti.Infrastructure;

/// <summary>
/// Persistenza su file JSON. Non serializza direttamente l'entità di dominio
/// (che ha setter privati e regole), ma un DTO "piatto": il formato del file
/// resta un dettaglio dell'Infrastructure.
/// </summary>
public class JsonStudenteRepository : IStudenteRepository
{
    private static readonly JsonSerializerOptions Opzioni = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping   // lettere accentate leggibili nel file
    };

    private readonly string _percorso;

    public JsonStudenteRepository(string percorso)
    {
        _percorso = percorso;
    }

    public List<Studente> GetAll()
    {
        return Carica();
    }

    public Studente? GetById(int id)
    {
        List<Studente> studenti = Carica();
        int indice = CercaIndice(studenti, id);
        if (indice < 0)
            return null;
        return studenti[indice];
    }

    public void Add(Studente studente)
    {
        List<Studente> studenti = Carica();
        if (CercaIndice(studenti, studente.Id) >= 0)
            throw new InvalidOperationException($"Esiste già uno studente con id {studente.Id}.");

        studenti.Add(studente);
        Salva(studenti);
    }

    public void Update(Studente studente)
    {
        List<Studente> studenti = Carica();
        int indice = CercaIndice(studenti, studente.Id);
        if (indice < 0)
            throw new InvalidOperationException($"Studente con id {studente.Id} non trovato.");

        studenti[indice] = studente;
        Salva(studenti);
    }

    public bool Delete(int id)
    {
        List<Studente> studenti = Carica();
        int indice = CercaIndice(studenti, id);
        if (indice < 0)
            return false;

        studenti.RemoveAt(indice);
        Salva(studenti);
        return true;
    }

    // ---------- lettura / scrittura del file ----------

    private List<Studente> Carica()
    {
        var studenti = new List<Studente>();
        if (!File.Exists(_percorso))
            return studenti;

        string json = File.ReadAllText(_percorso);
        if (string.IsNullOrWhiteSpace(json))
            return studenti;

        List<StudenteDto>? dati = JsonSerializer.Deserialize<List<StudenteDto>>(json, Opzioni);
        if (dati == null)
            return studenti;

        // DTO -> entità di dominio
        foreach (StudenteDto d in dati)
            studenti.Add(new Studente(d.Id, d.Nome, d.Cognome, d.Classe));

        return studenti;
    }

    private void Salva(List<Studente> studenti)
    {
        // entità di dominio -> DTO
        var dati = new List<StudenteDto>();
        foreach (Studente s in studenti)
            dati.Add(new StudenteDto(s.Id, s.Nome, s.Cognome, s.Classe));

        File.WriteAllText(_percorso, JsonSerializer.Serialize(dati, Opzioni));
    }

    private static int CercaIndice(List<Studente> studenti, int id)
    {
        for (int i = 0; i < studenti.Count; i++)
        {
            if (studenti[i].Id == id)
                return i;
        }
        return -1;
    }
}

/// <summary>Forma dei dati così come appare nel file JSON.</summary>
public record StudenteDto
{
    public StudenteDto() { }

    public StudenteDto(int id, string nome, string cognome, string classe)
    {
        Id = id;
        Nome = nome;
        Cognome = cognome;
        Classe = classe;
    }

    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string Classe { get; set; } = string.Empty;
}