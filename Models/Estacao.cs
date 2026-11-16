using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models.Enumeracoes;
using System.Text.Json.Serialization;

namespace EncostaSegura.Models
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "tipoEstacao")]
    [JsonDerivedType(typeof(EstacaoEncosta), "encosta")]
    [JsonDerivedType(typeof(EstacaoCorrego), "corrego")]
    public abstract class Estacao
    {
        public string Codigo { get; set; } = "";

        public string Nome { get; set; } = "";

        public string Localizacao { get; set; } = "";

        public bool Ativa { get; set; } = true;

        public NivelRisco RiscoAtual { get; set; } =
            NivelRisco.Baixo;

        public List<Leitura> Historico { get; set; } = new();

        [JsonIgnore]
        public Leitura? UltimaLeitura
        {
            get
            {
                if (Historico.Count == 0)
                    return null;

                return Historico[Historico.Count - 1];
            }
        }

        public abstract NivelRisco CalcularRisco(Leitura leitura);

        public abstract string DescreverLeitura(Leitura leitura);

        public void AdicionarLeitura(Leitura leitura)
        {
            Historico.Add(leitura);

            RiscoAtual = CalcularRisco(leitura);
        }
    }
}