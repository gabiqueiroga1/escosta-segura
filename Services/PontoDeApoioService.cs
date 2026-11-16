using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models;

namespace EncostaSegura.Services
{
    public class PontoDeApoioService
    {
        private readonly List<PontoDeApoio> _pontos;

        public PontoDeApoioService(List<PontoDeApoio> pontos)
        {
            _pontos = pontos;
        }

        public List<PontoDeApoio> ObterTodos()
        {
            return _pontos;
        }

        public PontoDeApoio? BuscarPorCodigo(string codigo)
        {
            foreach (PontoDeApoio ponto in _pontos)
            {
                if (ponto.Codigo.Equals(
                    codigo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return ponto;
                }
            }

            return null;
        }

        public void Adicionar(PontoDeApoio ponto)
        {
            if (string.IsNullOrWhiteSpace(ponto.Codigo))
                throw new ArgumentException("Informe o código.");

            if (string.IsNullOrWhiteSpace(ponto.Nome))
                throw new ArgumentException("Informe o nome.");

            if (string.IsNullOrWhiteSpace(ponto.Endereco))
                throw new ArgumentException("Informe o endereço.");

            if (string.IsNullOrWhiteSpace(ponto.RotaSegura))
                throw new ArgumentException("Informe a rota segura.");

            if (BuscarPorCodigo(ponto.Codigo) is not null)
                throw new ArgumentException(
                    "Já existe um ponto com esse código.");

            _pontos.Add(ponto);
        }

        public void Atualizar(PontoDeApoio ponto)
        {
            PontoDeApoio? existente = BuscarPorCodigo(ponto.Codigo);

            if (existente is null)
                throw new ArgumentException("Ponto não encontrado.");

            existente.Nome = ponto.Nome;
            existente.Endereco = ponto.Endereco;
            existente.RotaSegura = ponto.RotaSegura;
            existente.Observacoes = ponto.Observacoes;
        }

        public void Desativar(string codigo)
        {
            PontoDeApoio? ponto = BuscarPorCodigo(codigo);

            if (ponto is null)
                throw new ArgumentException("Ponto não encontrado.");

            ponto.Ativo = false;
        }
    }
}
