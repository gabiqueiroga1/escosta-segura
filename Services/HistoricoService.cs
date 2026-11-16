using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models;

namespace EncostaSegura.Services
{
    public class HistoricoService
    {
        private readonly List<EventoHistorico> _eventos;

        public HistoricoService(List<EventoHistorico> eventos)
        {
            _eventos = eventos;
        }

        public List<EventoHistorico> ObterTodos()
        {
            return _eventos;
        }

        public void Registrar(
            string tipo,
            string codigo,
            string descricao)
        {
            EventoHistorico evento = new(
                tipo,
                codigo,
                descricao);

            _eventos.Add(evento);
        }
    }
}
