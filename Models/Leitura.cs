using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncostaSegura.Models
{
    public class Leitura
    {
        public DateTime DataHora { get; set; }

        // Dados de encosta
        public double UmidadeSolo { get; set; }

        public double Chuva72Horas { get; set; }

        public double Inclinacao { get; set; }

        // Dados de córrego
        public double NivelAgua { get; set; }

        public double VelocidadeSubida { get; set; }

        public Leitura()
        {
            DataHora = DateTime.Now;
        }
    }
}