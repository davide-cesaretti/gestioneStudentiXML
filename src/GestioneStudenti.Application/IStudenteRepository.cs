using GestioneStudenti.Domain;

namespace GestioneStudenti.Application;

/// <summary>
/// "Porta" verso la persistenza. L'Application dichiara COSA le serve,
/// l'Infrastructure decide COME farlo (memoria, .txt, .json, database...).
/// </summary>
public interface IStudenteRepository
{
    List<Studente> GetAll();
    Studente? GetById(int id);
    void Add(Studente studente);
    void Update(Studente studente);
    bool Delete(int id);
}
