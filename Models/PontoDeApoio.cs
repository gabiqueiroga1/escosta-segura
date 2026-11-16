using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncostaSegura.Models
{
    public class PontoDeApoio
    {
        public string Codigo { get; set; } = "";

        public string Nome { get; set; } = "";

        public string Endereco { get; set; } = "";

        public string RotaSegura { get; set; } = "";

        public string Observacoes { get; set; } = "";

        public bool Ativo { get; set; } = true;
    }
}
