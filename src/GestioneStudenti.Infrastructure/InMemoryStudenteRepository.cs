using GestioneStudenti.Application;
using GestioneStudenti.Domain;

namespace GestioneStudenti.Infrastructure;

/// <summary>
/// Persistenza "volatile": i dati vivono in un Dictionary e spariscono
/// alla chiusura del programma. Utile per test e prototipi.
/// </summary>
public class InMemoryStudenteRepository : IStudenteRepository
{
    private readonly Dictionary<int, Studente> _studenti = new Dictionary<int, Studente>();

    public List<Studente> GetAll()
    {
        // restituisco una COPIA: chi chiama non può alterare il dizionario interno
        return new List<Studente>(_studenti.Values);
    }

    public Studente? GetById(int id)
    {
        if (_studenti.TryGetValue(id, out Studente? studente))
            return studente;
        return null;
    }

    public void Add(Studente studente)
    {
        if (_studenti.ContainsKey(studente.Id))
            throw new InvalidOperationException($"Esiste già uno studente con id {studente.Id}.");

        _studenti.Add(studente.Id, studente);
    }

    public void Update(Studente studente)
    {
        if (!_studenti.ContainsKey(studente.Id))
            throw new InvalidOperationException($"Studente con id {studente.Id} non trovato.");

        _studenti[studente.Id] = studente;
    }

    public bool Delete(int id)
    {
        return _studenti.Remove(id);
    }
}
