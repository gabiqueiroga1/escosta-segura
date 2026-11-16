using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models.Enumeracoes;

// monitorar nivel da agua e velocidade de subida desse nivel

namespace EncostaSegura.Models
{
    public class EstacaoCorrego : Estacao
    {
        public override NivelRisco CalcularRisco(Leitura leitura)
        {
            if (
                leitura.NivelAgua >= 70 ||
                leitura.VelocidadeSubida >= 30)
            {
                return NivelRisco.Alto;
            }

            if (
                leitura.NivelAgua >= 45 ||
                leitura.VelocidadeSubida >= 15)
            {
                return NivelRisco.Atencao;
            }

            return NivelRisco.Baixo;
        }

        public override string DescreverLeitura(Leitura leitura)
        {
            return
                $"Nível da água: {leitura.NivelAgua} cm | " +
                $"Subida: {leitura.VelocidadeSubida} cm/h";
        }
    }
}