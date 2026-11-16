using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models;

namespace EncostaSegura.Data
{
    public class DadosSistema
    {
        public List<Estacao> Estacoes { get; set; } = new();

        public List<PontoDeApoio> PontosDeApoio { get; set; } = new();

        public List<EventoHistorico> Eventos { get; set; } = new();
    }
}
