using GestioneStudenti.Application;
using GestioneStudenti.Domain;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GestioneStudenti.Infrastructure
{
    public class XmlStudenteRepository: IStudenteRepository
    {
        private List<Studente> Studenti = new();
        private readonly string _filePath;

        public XmlStudenteRepository(string filepath = "studente.xml")
        {
            var root = GetRoot();

            _filePath = Path.Combine(root, "src", "GestioneStudenti.Infrastructure", filepath);
            Studenti = Load();
        }

        private List<Studente> Load()
        {
            if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
                return new List<Studente>();

         
                var serializer = new XmlSerializer(typeof(List<StudenteDto>));
                var reader = new StreamReader(_filePath);
                var dtos = serializer.Deserialize(reader) as List<StudenteDto>;
                reader.Close();

                if (dtos is null)
                    return new List<Studente>();

                return dtos.Select(dto => new Studente(dto.Id, dto.Nome, dto.Cognome, dto.Classe)).ToList();
        }

        private void Salva()
        {
            var dtos = Studenti.Select(s => new StudenteDto(s.Id, s.Nome, s.Cognome, s.Classe)).ToList();
            var writer = new StreamWriter(_filePath);
            var serializer = new XmlSerializer(typeof(List<StudenteDto>));
            serializer.Serialize(writer, dtos);
            writer.Close();
        }

        public List<Studente> GetAll()
        {
            Studenti = Load();
            return Studenti;
        }

        public Studente GetById(int id)
        {
            var studente = Studenti.FirstOrDefault(s => s.Id == id);
            if (studente is null)
                throw new ArgumentException("id not valid");

            return studente;
        }

        public void Add(Studente studente)
        {
            Studenti.Add(studente);
            Salva();
        }

        public void Update(Studente studente)
        {
            for(int i = 0; i < Studenti.Count; i++)
            {
                if (Studenti[i].Id == studente.Id)
                {
                    Studenti[i] = studente;
                    Salva();
                    return;
                }
            }

            throw new ArgumentException("studente non valido");
        }

        public bool Delete(int id)
        {
            for (int i = 0; i < Studenti.Count; i++)
                if (Studenti[i].Id == id)
                {
                    Studenti.RemoveAt(i);
                    Salva();
                    return true;
                }

            return false;
        }

        private string GetRoot()
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while(dir != null && !Directory.Exists(Path.Combine(dir.FullName, "src")))
            {
                dir = dir.Parent;
            }

            return dir.FullName;
        }
    }
}