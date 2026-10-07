using GestioneStudenti.Application;
using GestioneStudenti.Domain;
using GestioneStudenti.Infrastructure;

Console.OutputEncoding = System.Text.Encoding.UTF8;

// ===== Composition root =====
// È l'UNICO punto del programma che conosce le classi concrete dell'Infrastructure.
// Cambiando la scelta, il resto del codice (Service e Domain) resta identico.
IStudenteRepository repository = ScegliPersistenza();
var service = new StudenteService(repository);

bool continua = true;
while (continua)
{
    Console.WriteLine();
    Console.WriteLine("1) Elenco studenti   2) Iscrivi studente   3) Cambia classe   4) Ritira studente   0) Esci");
    Console.Write("> ");

    try
    {
        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                List<Studente> elenco = service.ElencoStudenti();
                if (elenco.Count == 0)
                    Console.WriteLine("Nessuno studente presente.");
                foreach (Studente s in elenco)
                    Console.WriteLine(s);
                break;

            case "2":
                Studente nuovo = service.IscriviStudente(Chiedi("Nome"), Chiedi("Cognome"), Chiedi("Classe"));
                Console.WriteLine($"Iscritto: {nuovo}");
                break;

            case "3":
                service.CambiaClasse(ChiediIntero("Id studente"), Chiedi("Nuova classe"));
                Console.WriteLine("Classe aggiornata.");
                break;

            case "4":
                service.RitiraStudente(ChiediIntero("Id studente"));
                Console.WriteLine("Studente ritirato.");
                break;

            case "0":
            case null:
                continua = false;
                break;

            default:
                Console.WriteLine("Scelta non valida.");
                break;
        }
    }
    catch (Exception ex)
    {
        // Gli errori di validazione del Domain arrivano fin qui come eccezioni
        Console.WriteLine($"Errore: {ex.Message}");
    }
}

// ===== Funzioni di supporto =====

static IStudenteRepository ScegliPersistenza()
{
    while (true)
    {
        Console.WriteLine("Dove salvo i dati?");
        Console.WriteLine("  1) In memoria (si perdono alla chiusura)");
        Console.WriteLine("  2) File di testo (studenti.txt)");
        Console.WriteLine("  3) File JSON (studenti.json)");
        Console.WriteLine("  4) File Xml (studenti.xml)");
        Console.Write("> ");

        switch (Console.ReadLine()?.Trim())
        {
            case "1":
                return new InMemoryStudenteRepository();
            case "2":
                Console.WriteLine($"File: {Path.GetFullPath("studenti.txt")}");
                return new TxtStudenteRepository("studenti.txt");
            case "3":
                Console.WriteLine($"File: {Path.GetFullPath("studenti.json")}");
                return new JsonStudenteRepository("studenti.json");
            case "4":
                Console.WriteLine($"File: {Path.GetFullPath("studenti.xml")}");
                return new XmlStudenteRepository("studenti.xml");
            case null:
                Environment.Exit(0);
                break;
        }
        Console.WriteLine("Scelta non valida.\n");
    }
}

static string Chiedi(string etichetta)
{
    Console.Write($"{etichetta}: ");
    return Console.ReadLine() ?? "";
}

static int ChiediIntero(string etichetta)
{
    while (true)
    {
        if (int.TryParse(Chiedi(etichetta), out int valore))
            return valore;
        Console.WriteLine("Inserisci un numero intero.");
    }
}
