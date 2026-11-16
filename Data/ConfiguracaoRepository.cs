using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace EncostaSegura.Data
{
    public class ConfiguracaoRepository
    {
        private readonly string _caminhoArquivo;

        private readonly JsonSerializerOptions _opcoes = new()
        {
            WriteIndented = true
        };

        public ConfiguracaoRepository()
        {
            string pasta = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "EncostaSegura");

            Directory.CreateDirectory(pasta);

            _caminhoArquivo = Path.Combine(
                pasta,
                "dados.json");
        }

        public DadosSistema Carregar()
        {
            if (!File.Exists(_caminhoArquivo))
                return new DadosSistema();

            string json = File.ReadAllText(_caminhoArquivo);

            return JsonSerializer.Deserialize<DadosSistema>(
                json,
                _opcoes) ?? new DadosSistema();
        }

        public void Salvar(DadosSistema dados)
        {
            string json = JsonSerializer.Serialize(
                dados,
                _opcoes);

            File.WriteAllText(_caminhoArquivo, json);
        }
    }
}
