using GestioneStudenti.Domain;

namespace GestioneStudenti.Application;

/// <summary>
/// Casi d'uso dell'applicazione. Lavora solo con l'interfaccia IStudenteRepository:
/// non sa (e non deve sapere) dove finiscono davvero i dati.
/// </summary>
public class StudenteService
{
    private readonly IStudenteRepository _repository;

    public StudenteService(IStudenteRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Elenco ordinato per classe, poi cognome, poi nome.</summary>
    public List<Studente> ElencoStudenti()
    {
        List<Studente> studenti = _repository.GetAll();
        studenti.Sort(ConfrontaStudenti);
        return studenti;
    }

    public Studente IscriviStudente(string nome, string cognome, string classe)
    {
        var studente = new Studente(ProssimoId(), nome, cognome, classe);
        _repository.Add(studente);
        return studente;
    }

    public void CambiaClasse(int id, string nuovaClasse)
    {
        Studente? studente = _repository.GetById(id);
        if (studente == null)
            throw new InvalidOperationException($"Studente con id {id} non trovato.");

        studente.CambiaClasse(nuovaClasse);   // regola di dominio
        _repository.Update(studente);         // persistenza
    }

    public void RitiraStudente(int id)
    {
        if (!_repository.Delete(id))
            throw new InvalidOperationException($"Studente con id {id} non trovato.");
    }

    // ---------- metodi di supporto ----------

    private int ProssimoId()
    {
        int max = 0;
        foreach (Studente s in _repository.GetAll())
        {
            if (s.Id > max)
                max = s.Id;
        }
        return max + 1;
    }

    // Restituisce <0 se a viene prima di b, >0 se viene dopo, 0 se sono "uguali"
    private static int ConfrontaStudenti(Studente a, Studente b)
    {
        int risultato = string.Compare(a.Classe, b.Classe);
        if (risultato == 0)
            risultato = string.Compare(a.Cognome, b.Cognome);
        if (risultato == 0)
            risultato = string.Compare(a.Nome, b.Nome);
        return risultato;
    }
}
