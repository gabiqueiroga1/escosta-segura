using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// registrar acoes realizadas pela defesa civil

namespace EncostaSegura.Models
{
    public class EventoHistorico
    {
        public DateTime DataHora { get; set; }

        public string Tipo { get; set; } = "";

        public string CodigoRelacionado { get; set; } = "";

        public string Descricao { get; set; } = "";

        public EventoHistorico()
        {
            DataHora = DateTime.Now;
        }

        public EventoHistorico(
            string tipo,
            string codigoRelacionado,
            string descricao)
        {
            DataHora = DateTime.Now;
            Tipo = tipo;
            CodigoRelacionado = codigoRelacionado;
            Descricao = descricao;
        }
    }
}
