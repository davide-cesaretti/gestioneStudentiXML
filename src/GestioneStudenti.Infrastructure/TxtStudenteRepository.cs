using GestioneStudenti.Application;
using GestioneStudenti.Domain;

namespace GestioneStudenti.Infrastructure;

/// <summary>
/// Persistenza su file di testo: una riga per studente, campi separati da ';'
///   1;Mario;Rossi;3I
/// Ogni operazione rilegge il file, modifica la lista in memoria e riscrive tutto.
/// </summary>
public class TxtStudenteRepository : IStudenteRepository
{
    private const char Separatore = ';';
    private readonly string _percorso;

    public TxtStudenteRepository(string percorso)
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

        string[] righe = File.ReadAllLines(_percorso);
        foreach (string riga in righe)
        {
            if (!string.IsNullOrWhiteSpace(riga))
                studenti.Add(DaRiga(riga));
        }
        return studenti;
    }

    private void Salva(List<Studente> studenti)
    {
        // Prima converto TUTTO in righe: se una conversione fallisce
        // il file non viene toccato (niente file mezzo scritto).
        var righe = new List<string>();
        foreach (Studente s in studenti)
            righe.Add(ARiga(s));

        File.WriteAllLines(_percorso, righe);
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

    // Mapping Studente -> riga di testo
    private static string ARiga(Studente s)
    {
        string[] campi = { s.Id.ToString(), s.Nome, s.Cognome, s.Classe };

        foreach (string campo in campi)
        {
            if (campo.Contains(Separatore))
                throw new InvalidOperationException($"Il carattere '{Separatore}' non può essere salvato nel file di testo.");
        }

        return string.Join(Separatore, campi);
    }

    // Mapping riga di testo -> Studente (le regole del dominio vengono riapplicate)
    private static Studente DaRiga(string riga)
    {
        string[] campi = riga.Split(Separatore);
        if (campi.Length != 4)
            throw new FormatException($"Riga non valida nel file: \"{riga}\"");

        return new Studente(int.Parse(campi[0]), campi[1], campi[2], campi[3]);
    }
}
