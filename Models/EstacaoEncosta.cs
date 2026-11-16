using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models.Enumeracoes;

// monitorar umidade do solo, chuva acumulada e inclinacao do terreno

namespace EncostaSegura.Models
{
    public class EstacaoEncosta : Estacao
    {
        public override NivelRisco CalcularRisco(Leitura leitura)
        {
            if (
                leitura.Inclinacao >= 10 ||
                leitura.UmidadeSolo >= 90 ||
                leitura.Chuva72Horas >= 80)
            {
                return NivelRisco.Alto;
            }

            if (
                leitura.Inclinacao >= 5 ||
                leitura.UmidadeSolo >= 65 ||
                leitura.Chuva72Horas >= 40)
            {
                return NivelRisco.Atencao;
            }

            return NivelRisco.Baixo;
        }

        public override string DescreverLeitura(Leitura leitura)
        {
            return
                $"Umidade: {leitura.UmidadeSolo}% | " +
                $"Chuva 72h: {leitura.Chuva72Horas} mm | " +
                $"Inclinação: {leitura.Inclinacao}°";
        }
    }
}
