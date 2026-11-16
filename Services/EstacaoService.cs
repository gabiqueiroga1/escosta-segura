using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncostaSegura.Models;

namespace EncostaSegura.Services
{
    public class EstacaoService
    {
        private readonly List<Estacao> _estacoes;

        public EstacaoService(List<Estacao> estacoes)
        {
            _estacoes = estacoes;
        }

        public List<Estacao> ObterTodas()
        {
            return _estacoes;
        }

        public Estacao? BuscarPorCodigo(string codigo)
        {
            foreach (Estacao estacao in _estacoes)
            {
                if (estacao.Codigo.Equals(
                    codigo,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return estacao;
                }
            }

            return null;
        }

        public void Adicionar(Estacao estacao)
        {
            if (string.IsNullOrWhiteSpace(estacao.Codigo))
                throw new ArgumentException("Informe o código.");

            if (string.IsNullOrWhiteSpace(estacao.Nome))
                throw new ArgumentException("Informe o nome.");

            if (string.IsNullOrWhiteSpace(estacao.Localizacao))
                throw new ArgumentException("Informe a localização.");

            if (BuscarPorCodigo(estacao.Codigo) is not null)
                throw new ArgumentException(
                    "Já existe uma estação com esse código.");

            _estacoes.Add(estacao);
        }

        public void Atualizar(Estacao estacao)
        {
            Estacao? existente = BuscarPorCodigo(estacao.Codigo);

            if (existente is null)
                throw new ArgumentException("Estação não encontrada.");

            existente.Nome = estacao.Nome;
            existente.Localizacao = estacao.Localizacao;
        }

        public void Desativar(string codigo)
        {
            Estacao? estacao = BuscarPorCodigo(codigo);

            if (estacao is null)
                throw new ArgumentException("Estação não encontrada.");

            estacao.Ativa = false;
        }

        public void AdicionarLeitura(string codigo, Leitura leitura)
        {
            Estacao? estacao = BuscarPorCodigo(codigo);

            if (estacao is null)
                throw new ArgumentException("Estação não encontrada.");

            if (!estacao.Ativa)
                throw new InvalidOperationException(
                    "A estação está desativada.");

            estacao.AdicionarLeitura(leitura);
        }
    }
}
