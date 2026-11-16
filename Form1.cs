
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using EncostaSegura.Data;
using EncostaSegura.Models;
using EncostaSegura.Models.Enumeracoes;
using EncostaSegura.Services;

namespace EncostaSegura
{
    public partial class Form1 : Form
    {
        private readonly ConfiguracaoRepository _repository = new();

        private DadosSistema _dados = new();

        private EstacaoService _estacaoService = null!;

        private PontoDeApoioService _pontoService = null!;

        private HistoricoService _historicoService = null!;

        private readonly DataGridView _gridResumo = CriarGrade();

        private readonly DataGridView _gridEstacoes = CriarGrade();

        private readonly DataGridView _gridPontos = CriarGrade();

        private readonly DataGridView _gridHistorico = CriarGrade();

        private readonly Label _lblEstacoesAtivas = new();

        private readonly Label _lblAlertas = new();

        private readonly Label _lblRiscoEncosta = new();

        private readonly Label _lblRiscoCorrego = new();
        private readonly Label _lblSirene = new();
        private readonly Label _lblNotificacoes = new();

        public Form1()
        {
            InitializeComponent();

            Text = "Encosta Segura - Defesa Civil";

            StartPosition = FormStartPosition.CenterScreen;

            Size = new Size(1150, 750);

            MinimumSize = new Size(950, 600);

            _dados = _repository.Carregar();

            _estacaoService = new EstacaoService(_dados.Estacoes);

            _pontoService = new PontoDeApoioService(
                _dados.PontosDeApoio);

            _historicoService = new HistoricoService(
                _dados.Eventos);

            CriarDadosIniciais();

            ConstruirInterface();

            AtualizarTelas();
        }

        // ================================================
        // INICIALIZAÇÃO E SALVAMENTO
        // ================================================

        private void CriarDadosIniciais()
        {
            if (_dados.Estacoes.Count == 0)
            {
                EstacaoEncosta encosta = new()
                {
                    Codigo = "ENC-01",
                    Nome = "Estação da Encosta",
                    Localizacao = "Área de encosta"
                };

                EstacaoCorrego corrego = new()
                {
                    Codigo = "NIV-01",
                    Nome = "Estação do Córrego",
                    Localizacao = "Próximo à ponte"
                };

                _estacaoService.Adicionar(encosta);
                _estacaoService.Adicionar(corrego);

                _estacaoService.AdicionarLeitura(
                    "ENC-01",
                    new Leitura
                    {
                        UmidadeSolo = 35,
                        Chuva72Horas = 10,
                        Inclinacao = 1.5
                    });

                _estacaoService.AdicionarLeitura(
                    "NIV-01",
                    new Leitura
                    {
                        NivelAgua = 20,
                        VelocidadeSubida = 5
                    });

                _historicoService.Registrar(
                    "Sistema",
                    "ENC-01",
                    "Estação de encosta cadastrada.");

                _historicoService.Registrar(
                    "Sistema",
                    "NIV-01",
                    "Estação de córrego cadastrada.");
            }

            if (_dados.PontosDeApoio.Count == 0)
            {
                _pontoService.Adicionar(
                    new PontoDeApoio
                    {
                        Codigo = "PA-01",
                        Nome = "Escola Municipal",
                        Endereco = "Região da comunidade",
                        RotaSegura = "Rota verde / escadaria",
                        Observacoes = "Ponto de apoio em área segura."
                    });

                _pontoService.Adicionar(
                    new PontoDeApoio
                    {
                        Codigo = "PA-02",
                        Nome = "Centro Comunitário",
                        Endereco = "Região mais elevada",
                        RotaSegura = "Rota azul",
                        Observacoes = "Localizado em terreno elevado."
                    });
            }

            SalvarDados();
        }

        private void SalvarDados()
        {
            _repository.Salvar(_dados);
        }

        // ================================================
        // INTERFACE PRINCIPAL
        // ================================================

        private void ConstruirInterface()
        {
            TabControl abas = new()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10)
            };

            TabPage inicio = new("Visão geral");
            TabPage estacoes = new("Estações");
            TabPage pontos = new("Pontos de apoio");
            TabPage defesaCivil = new("Defesa Civil");
            TabPage historico = new("Histórico");

            abas.TabPages.Add(inicio);
            abas.TabPages.Add(estacoes);
            abas.TabPages.Add(pontos);
            abas.TabPages.Add(defesaCivil);
            abas.TabPages.Add(historico);

            ConstruirAbaInicio(inicio);
            ConstruirAbaEstacoes(estacoes);
            ConstruirAbaPontos(pontos);
            ConstruirAbaDefesaCivil(defesaCivil);
            ConstruirAbaHistorico(historico);

            Controls.Add(abas);
        }

        private void ConstruirAbaInicio(TabPage aba)
        {
            Panel painel = CriarPainel();

            painel.Controls.Add(
                CriarTitulo("Painel da Defesa Civil"));

            painel.Controls.Add(
                CriarDescricao(
                    "Resumo do monitoramento e da situação das estações."));

            FlowLayoutPanel indicadores = new()
            {
                Location = new Point(20, 110),
                Size = new Size(1050, 130),
                AutoScroll = true
            };

            indicadores.Controls.Add(
                CriarIndicador("Estações ativas", _lblEstacoesAtivas));

            indicadores.Controls.Add(
                CriarIndicador("Alertas altos", _lblAlertas));

            indicadores.Controls.Add(
                CriarIndicador("Risco na encosta", _lblRiscoEncosta));

            indicadores.Controls.Add(
                CriarIndicador("Risco no córrego", _lblRiscoCorrego));

            Label subtitulo = CriarTituloMenor(
                "Situação das estações");

            subtitulo.Location = new Point(20, 270);

            _gridResumo.Location = new Point(20, 310);
            _gridResumo.Size = new Size(1050, 300);

            _gridResumo.Columns.Add("Codigo", "Código");
            _gridResumo.Columns.Add("Nome", "Estação");
            _gridResumo.Columns.Add("Tipo", "Monitoramento");
            _gridResumo.Columns.Add("Localizacao", "Localização");
            _gridResumo.Columns.Add("Risco", "Risco");
            _gridResumo.Columns.Add("Status", "Status");
            _gridResumo.Columns.Add("Atualizacao", "Última leitura");

            Button atualizar = CriarBotao(
                "Atualizar painel", 20, 625, 180);

            atualizar.Click += (_, _) => AtualizarTelas();

            painel.Controls.Add(indicadores);
            painel.Controls.Add(subtitulo);
            painel.Controls.Add(_gridResumo);
            painel.Controls.Add(atualizar);

            aba.Controls.Add(painel);
        }

        private void ConstruirAbaEstacoes(TabPage aba)
        {
            Panel painel = CriarPainel();

            painel.Controls.Add(
                CriarTitulo("Gerenciamento de estações"));

            painel.Controls.Add(
                CriarDescricao(
                    "Cadastre, edite, desative e monitore as estações."));

            _gridEstacoes.Location = new Point(20, 110);
            _gridEstacoes.Size = new Size(1050, 390);

            _gridEstacoes.Columns.Add("Codigo", "Código");
            _gridEstacoes.Columns.Add("Nome", "Nome");
            _gridEstacoes.Columns.Add("Tipo", "Tipo");
            _gridEstacoes.Columns.Add("Localizacao", "Localização");
            _gridEstacoes.Columns.Add("Risco", "Risco");
            _gridEstacoes.Columns.Add("Status", "Status");
            _gridEstacoes.Columns.Add("Leitura", "Última leitura");

            Button adicionar = CriarBotao(
                "Adicionar estação", 20, 525, 180);

            adicionar.Click += (_, _) => AdicionarEstacao();

            Button editar = CriarBotao(
                "Editar selecionada", 210, 525, 180);

            editar.Click += (_, _) => EditarEstacao();

            Button desativar = CriarBotao(
                "Desativar estação", 400, 525, 180);

            desativar.Click += (_, _) => DesativarEstacao();

            Button simular = CriarBotao(
                "Nova leitura", 590, 525, 180);

            simular.Click += (_, _) => SimularLeitura();

            Button historico = CriarBotao(
                "Ver histórico", 780, 525, 180);

            historico.Click += (_, _) => AbrirHistoricoEstacao();

            painel.Controls.Add(_gridEstacoes);
            painel.Controls.Add(adicionar);
            painel.Controls.Add(editar);
            painel.Controls.Add(desativar);
            painel.Controls.Add(simular);
            painel.Controls.Add(historico);

            aba.Controls.Add(painel);
        }
        private void ConstruirAbaPontos(TabPage aba)
        {
            Panel painel = CriarPainel();

            painel.Controls.Add(
                CriarTitulo("Gerenciamento de pontos de apoio"));

            painel.Controls.Add(
                CriarDescricao(
                    "Cadastre, edite e gerencie os locais seguros para a população."));

            _gridPontos.Location = new Point(20, 110);
            _gridPontos.Size = new Size(1050, 390);

            _gridPontos.Columns.Add("Codigo", "Código");
            _gridPontos.Columns.Add("Nome", "Nome");
            _gridPontos.Columns.Add("Endereco", "Endereço");
            _gridPontos.Columns.Add("Rota", "Rota segura");
            _gridPontos.Columns.Add("Status", "Status");

            Button adicionar = CriarBotao(
                "Adicionar ponto", 20, 525, 180);

            adicionar.Click += (_, _) => AdicionarPonto();

            Button editar = CriarBotao(
                "Editar selecionado", 210, 525, 180);

            editar.Click += (_, _) => EditarPonto();

            Button desativar = CriarBotao(
                "Desativar ponto", 400, 525, 180);

            desativar.Click += (_, _) => DesativarPonto();

            painel.Controls.Add(_gridPontos);
            painel.Controls.Add(adicionar);
            painel.Controls.Add(editar);
            painel.Controls.Add(desativar);

            aba.Controls.Add(painel);
        }
        private void ConstruirAbaDefesaCivil(TabPage aba)
        {
            Panel painel = CriarPainel();

            painel.Controls.Add(
                CriarTitulo("Central da Defesa Civil"));

            painel.Controls.Add(
                CriarDescricao(
                    "Área destinada à simulação de ações de alerta e comunicação com moradores."));

            Panel painelSirene = CriarIndicador(
                "Acionamentos de sirene",
                _lblSirene);

            painelSirene.Location = new Point(30, 130);

            Panel painelNotificacao = CriarIndicador(
                "Notificações registradas",
                _lblNotificacoes);

            painelNotificacao.Location = new Point(300, 130);

            Button sirene = CriarBotao(
                "Acionar sirene", 30, 270, 220);

            sirene.Click += (_, _) => AcionarSirene();

            Button notificar = CriarBotao(
                "Notificar moradores", 300, 270, 220);

            notificar.Click += (_, _) => NotificarMoradores();

            Label observacao = CriarDescricao(
                "Observação: as ações desta versão são simuladas e não enviam alertas reais.");

            observacao.Location = new Point(30, 340);

            painel.Controls.Add(painelSirene);
            painel.Controls.Add(painelNotificacao);
            painel.Controls.Add(sirene);
            painel.Controls.Add(notificar);
            painel.Controls.Add(observacao);

            aba.Controls.Add(painel);
        }
        private void ConstruirAbaHistorico(TabPage aba)
        {
            Panel painel = CriarPainel();

            painel.Controls.Add(
                CriarTitulo("Histórico de eventos"));

            painel.Controls.Add(
                CriarDescricao(
                    "Consulte as medições realizadas pelas estações e os eventos registrados no sistema."));

            _gridHistorico.Location = new Point(20, 110);
            _gridHistorico.Size = new Size(1050, 430);

            _gridHistorico.Columns.Add("Data", "Data e hora");
            _gridHistorico.Columns.Add("Tipo", "Tipo de evento");
            _gridHistorico.Columns.Add("Codigo", "Código relacionado");
            _gridHistorico.Columns.Add("Descricao", "Descrição");

            Button exportar = CriarBotao(
                "Exportar histórico (CSV)", 20, 565, 220);

            exportar.Click += (_, _) => ExportarHistorico();

            painel.Controls.Add(_gridHistorico);
            painel.Controls.Add(exportar);

            aba.Controls.Add(painel);
        }

        private void AdicionarEstacao()
        {
            Estacao? estacao = AbrirFormularioEstacao(null);

            if (estacao is null)
                return;

            try
            {
                _estacaoService.Adicionar(estacao);

                _historicoService.Registrar(
                    "Cadastro",
                    estacao.Codigo,
                    "Nova estação cadastrada.");

                SalvarDados();
                AtualizarTelas();

                MessageBox.Show("Estação cadastrada com sucesso.");
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void EditarEstacao()
        {
            Estacao? original = ObterEstacaoSelecionada();

            if (original is null)
                return;

            Estacao? alterada = AbrirFormularioEstacao(original);

            if (alterada is null)
                return;

            try
            {
                _estacaoService.Atualizar(alterada);

                _historicoService.Registrar(
                    "Alteração",
                    original.Codigo,
                    "Dados cadastrais da estação alterados.");

                SalvarDados();
                AtualizarTelas();
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void DesativarEstacao()
        {
            Estacao? estacao = ObterEstacaoSelecionada();

            if (estacao is null)
                return;

            if (MessageBox.Show(
                $"Deseja desativar {estacao.Codigo}?",
                "Confirmar",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                _estacaoService.Desativar(estacao.Codigo);

                _historicoService.Registrar(
                    "Desativação",
                    estacao.Codigo,
                    "Estação desativada pela Defesa Civil.");

                SalvarDados();
                AtualizarTelas();
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private Estacao? ObterEstacaoSelecionada()
        {
            if (_gridEstacoes.CurrentRow is null)
            {
                MostrarErro("Selecione uma estação.");
                return null;
            }

            string codigo = _gridEstacoes.CurrentRow
                .Cells["Codigo"].Value?.ToString() ?? "";

            return _estacaoService.BuscarPorCodigo(codigo);
        }

        private Estacao? AbrirFormularioEstacao(Estacao? original)
        {
            using Form janela = CriarJanelaFormulario(
                original is null ? "Nova estação" : "Editar estação",
                500,
                350);

            TextBox codigo = CriarCampo(130, 20, 300);
            TextBox nome = CriarCampo(130, 70, 300);
            TextBox local = CriarCampo(130, 120, 300);

            ComboBox tipo = new()
            {
                Location = new Point(130, 170),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            tipo.Items.Add("Monitoramento de encosta");
            tipo.Items.Add("Monitoramento de córrego");

            codigo.Text = original?.Codigo ?? "";
            nome.Text = original?.Nome ?? "";
            local.Text = original?.Localizacao ?? "";

            tipo.SelectedIndex = original is EstacaoCorrego ? 1 : 0;

            if (original is not null)
            {
                codigo.ReadOnly = true;
                tipo.Enabled = false;
            }

            janela.Controls.Add(CriarLabel("Código:", 20, 25));
            janela.Controls.Add(codigo);
            janela.Controls.Add(CriarLabel("Nome:", 20, 75));
            janela.Controls.Add(nome);
            janela.Controls.Add(CriarLabel("Localização:", 20, 125));
            janela.Controls.Add(local);
            janela.Controls.Add(CriarLabel("Tipo:", 20, 175));
            janela.Controls.Add(tipo);

            Estacao? resultado = null;

            Button salvar = CriarBotao("Salvar", 130, 225, 130);

            salvar.Click += (_, _) =>
            {
                if (
                    string.IsNullOrWhiteSpace(codigo.Text) ||
                    string.IsNullOrWhiteSpace(nome.Text) ||
                    string.IsNullOrWhiteSpace(local.Text) ||
                    tipo.SelectedIndex < 0)
                {
                    MostrarErro("Preencha todos os campos.");
                    return;
                }

                resultado = original is not null
                    ? original
                    : tipo.SelectedIndex == 0
                        ? new EstacaoEncosta()
                        : new EstacaoCorrego();

                resultado.Codigo = codigo.Text.Trim();
                resultado.Nome = nome.Text.Trim();
                resultado.Localizacao = local.Text.Trim();

                janela.DialogResult = DialogResult.OK;
                janela.Close();
            };

            Button cancelar = CriarBotao("Cancelar", 270, 225, 130);

            cancelar.Click += (_, _) => janela.Close();

            janela.Controls.Add(salvar);
            janela.Controls.Add(cancelar);

            return janela.ShowDialog(this) == DialogResult.OK
                ? resultado
                : null;
        }

        // ================================================
        // LEITURAS DAS ESTAÇÕES
        // ================================================

        private void SimularLeitura()
        {
            Estacao? selecionada = ObterEstacaoSelecionada();

            if (selecionada is null)
                return;

            if (!selecionada.Ativa)
            {
                MostrarErro("A estação está desativada.");
                return;
            }

            bool ehEncosta = selecionada is EstacaoEncosta;

            using Form janela = CriarJanelaFormulario(
                "Registrar leitura",
                500,
                ehEncosta ? 380 : 300);

            Label titulo = CriarTituloMenor(
                $"Nova leitura - {selecionada.Codigo}");

            titulo.Location = new Point(20, 15);

            NumericUpDown campo1 = CriarNumero(130, 80);
            NumericUpDown campo2 = CriarNumero(130, 135);
            NumericUpDown campo3 = CriarNumero(130, 190);

            Label label1;
            Label label2;
            Label? label3 = null;

            if (ehEncosta)
            {
                label1 = CriarLabel("Umidade (%):", 20, 85);
                label2 = CriarLabel("Chuva 72h (mm):", 20, 140);
                label3 = CriarLabel("Inclinação (°):", 20, 195);

                campo1.Maximum = 100;
                campo2.Maximum = 1000;
                campo3.Maximum = 90;
                campo3.DecimalPlaces = 1;
            }
            else
            {
                label1 = CriarLabel("Nível da água (cm):", 20, 85);
                label2 = CriarLabel("Subida (cm/h):", 20, 140);

                campo1.Maximum = 1000;
                campo2.Maximum = 1000;
            }

            int posicaoBotao = ehEncosta ? 260 : 205;

            Button salvar = CriarBotao(
                "Registrar",
                130,
                posicaoBotao,
                150);

            salvar.Click += (_, _) =>
            {
                Leitura leitura = new();

                if (ehEncosta)
                {
                    leitura.UmidadeSolo = (double)campo1.Value;
                    leitura.Chuva72Horas = (double)campo2.Value;
                    leitura.Inclinacao = (double)campo3.Value;
                }
                else
                {
                    leitura.NivelAgua = (double)campo1.Value;
                    leitura.VelocidadeSubida = (double)campo2.Value;
                }

                NivelRisco riscoAnterior = selecionada.RiscoAtual;

                try
                {
                    _estacaoService.AdicionarLeitura(
                        selecionada.Codigo,
                        leitura);

                    _historicoService.Registrar(
                        "Medição",
                        selecionada.Codigo,
                        selecionada.DescreverLeitura(leitura));

                    if (riscoAnterior != selecionada.RiscoAtual)
                    {
                        _historicoService.Registrar(
                            "Alteração de risco",
                            selecionada.Codigo,
                            $"Risco alterado de {FormatarRisco(riscoAnterior)} " +
                            $"para {FormatarRisco(selecionada.RiscoAtual)}.");
                    }

                    SalvarDados();

                    janela.DialogResult = DialogResult.OK;
                    janela.Close();
                }
                catch (Exception ex)
                {
                    MostrarErro(ex.Message);
                }
            };

            janela.Controls.Add(titulo);
            janela.Controls.Add(label1);
            janela.Controls.Add(campo1);
            janela.Controls.Add(label2);
            janela.Controls.Add(campo2);

            if (label3 is not null)
            {
                janela.Controls.Add(label3);
                janela.Controls.Add(campo3);
            }

            janela.Controls.Add(salvar);

            janela.ShowDialog(this);

            AtualizarTelas();
        }

        private void AbrirHistoricoEstacao()
        {
            Estacao? estacao = ObterEstacaoSelecionada();

            if (estacao is null)
                return;

            using Form janela = new()
            {
                Text = $"Histórico - {estacao.Codigo}",
                Size = new Size(850, 450),
                StartPosition = FormStartPosition.CenterParent
            };

            DataGridView grade = CriarGrade();
            grade.Dock = DockStyle.Fill;

            grade.Columns.Add("Data", "Data e hora");
            grade.Columns.Add("Risco", "Risco");
            grade.Columns.Add("Leitura", "Leitura");

            foreach (Leitura leitura in estacao.Historico)
            {
                NivelRisco risco = estacao.CalcularRisco(leitura);

                int indice = grade.Rows.Add(
                    leitura.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    FormatarRisco(risco),
                    estacao.DescreverLeitura(leitura));

                ColorirLinha(grade.Rows[indice], risco);
            }

            janela.Controls.Add(grade);
            janela.ShowDialog(this);
        }

        // ================================================
        // PONTOS DE APOIO
        // ================================================

        private void AdicionarPonto()
        {
            PontoDeApoio? ponto = AbrirFormularioPonto(null);

            if (ponto is null)
                return;

            try
            {
                _pontoService.Adicionar(ponto);

                _historicoService.Registrar(
                    "Cadastro",
                    ponto.Codigo,
                    "Novo ponto de apoio cadastrado.");

                SalvarDados();
                AtualizarTelas();
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void EditarPonto()
        {
            PontoDeApoio? original = ObterPontoSelecionado();

            if (original is null)
                return;

            PontoDeApoio? alterado = AbrirFormularioPonto(original);

            if (alterado is null)
                return;

            try
            {
                _pontoService.Atualizar(alterado);

                _historicoService.Registrar(
                    "Alteração",
                    original.Codigo,
                    "Dados do ponto de apoio alterados.");

                SalvarDados();
                AtualizarTelas();
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private void DesativarPonto()
        {
            PontoDeApoio? ponto = ObterPontoSelecionado();

            if (ponto is null)
                return;

            if (MessageBox.Show(
                $"Deseja desativar {ponto.Nome}?",
                "Confirmar",
                MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            try
            {
                _pontoService.Desativar(ponto.Codigo);

                _historicoService.Registrar(
                    "Desativação",
                    ponto.Codigo,
                    "Ponto de apoio desativado.");

                SalvarDados();
                AtualizarTelas();
            }
            catch (ArgumentException ex)
            {
                MostrarErro(ex.Message);
            }
        }

        private PontoDeApoio? ObterPontoSelecionado()
        {
            if (_gridPontos.CurrentRow is null)
            {
                MostrarErro("Selecione um ponto de apoio.");
                return null;
            }

            string codigo = _gridPontos.CurrentRow
                .Cells["Codigo"].Value?.ToString() ?? "";

            return _pontoService.BuscarPorCodigo(codigo);
        }
        private PontoDeApoio? AbrirFormularioPonto(
    PontoDeApoio? original)
        {
            using Form janela = CriarJanelaFormulario(
                original is null ? "Novo ponto de apoio" : "Editar ponto",
                500,
                430);

            TextBox codigo = CriarCampo(130, 20, 300);
            TextBox nome = CriarCampo(130, 65, 300);
            TextBox endereco = CriarCampo(130, 110, 300);
            TextBox rota = CriarCampo(130, 155, 300);
            TextBox observacoes = CriarCampo(130, 200, 300);

            observacoes.Multiline = true;
            observacoes.Height = 60;

            codigo.Text = original?.Codigo ?? "";
            nome.Text = original?.Nome ?? "";
            endereco.Text = original?.Endereco ?? "";
            rota.Text = original?.RotaSegura ?? "";
            observacoes.Text = original?.Observacoes ?? "";

            if (original is not null)
                codigo.ReadOnly = true;

            janela.Controls.Add(CriarLabel("Código:", 20, 25));
            janela.Controls.Add(codigo);
            janela.Controls.Add(CriarLabel("Nome:", 20, 70));
            janela.Controls.Add(nome);
            janela.Controls.Add(CriarLabel("Endereço:", 20, 115));
            janela.Controls.Add(endereco);
            janela.Controls.Add(CriarLabel("Rota:", 20, 160));
            janela.Controls.Add(rota);
            janela.Controls.Add(CriarLabel("Observações:", 20, 205));
            janela.Controls.Add(observacoes);

            PontoDeApoio? resultado = null;

            Button salvar = CriarBotao("Salvar", 130, 300, 130);

            salvar.Click += (_, _) =>
            {
                if (
                    string.IsNullOrWhiteSpace(codigo.Text) ||
                    string.IsNullOrWhiteSpace(nome.Text) ||
                    string.IsNullOrWhiteSpace(endereco.Text) ||
                    string.IsNullOrWhiteSpace(rota.Text))
                {
                    MostrarErro("Preencha os campos obrigatórios.");
                    return;
                }

                resultado = original ?? new PontoDeApoio();

                resultado.Codigo = codigo.Text.Trim();
                resultado.Nome = nome.Text.Trim();
                resultado.Endereco = endereco.Text.Trim();
                resultado.RotaSegura = rota.Text.Trim();
                resultado.Observacoes = observacoes.Text.Trim();

                janela.DialogResult = DialogResult.OK;
                janela.Close();
            };

            Button cancelar = CriarBotao("Cancelar", 270, 300, 130);

            cancelar.Click += (_, _) => janela.Close();

            janela.Controls.Add(salvar);
            janela.Controls.Add(cancelar);

            return janela.ShowDialog(this) == DialogResult.OK
                ? resultado
                : null;
        }

        // ================================================
        // DEFESA CIVIL
        // ================================================

        private void AcionarSirene()
        {
            if (MessageBox.Show(
                "Deseja registrar o acionamento da sirene?\n\n" +
                "Esta ação é apenas uma simulação.",
                "Confirmar acionamento",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            _historicoService.Registrar(
                "Sirene",
                "GERAL",
                "Acionamento simulado da sirene pela Defesa Civil.");

            SalvarDados();
            AtualizarHistorico();
            AtualizarIndicadoresDefesaCivil();

            MessageBox.Show(
                "Acionamento simulado registrado no histórico.");
        }

        private void NotificarMoradores()
        {
            using Form janela = CriarJanelaFormulario(
                "Notificar moradores", 500, 330);

            TextBox mensagem = new()
            {
                Location = new Point(20, 65),
                Size = new Size(420, 130),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text =
                    "A Defesa Civil informa que foi identificado " +
                    "um risco na região. Procure um ponto de apoio seguro."
            };

            janela.Controls.Add(CriarLabel("Mensagem:", 20, 25));
            janela.Controls.Add(mensagem);

            Button registrar = CriarBotao(
                "Registrar notificação", 20, 220, 200);

            registrar.Click += (_, _) =>
            {
                if (string.IsNullOrWhiteSpace(mensagem.Text))
                {
                    MostrarErro("Digite uma mensagem.");
                    return;
                }

                _historicoService.Registrar(
                    "Notificação",
                    "GERAL",
                    "Notificação simulada: " + mensagem.Text.Trim());

                SalvarDados();

                janela.DialogResult = DialogResult.OK;
                janela.Close();
            };

            janela.Controls.Add(registrar);

            if (janela.ShowDialog(this) == DialogResult.OK)
            {
                AtualizarHistorico();
                AtualizarIndicadoresDefesaCivil();

                MessageBox.Show(
                    "Notificação simulada registrada.");
            }
        }

        // ================================================
        // HISTÓRICO E EXPORTAÇÃO
        // ================================================

        private void AtualizarHistorico()
        {
            _gridHistorico.Rows.Clear();

            foreach (Estacao estacao in _dados.Estacoes)
            {
                foreach (Leitura leitura in estacao.Historico)
                {
                    NivelRisco risco = estacao.CalcularRisco(leitura);

                    _gridHistorico.Rows.Add(
                        leitura.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                        "Medição",
                        estacao.Codigo,
                        estacao.DescreverLeitura(leitura) +
                        " | Risco: " + FormatarRisco(risco));
                }
            }

            foreach (EventoHistorico evento in _historicoService.ObterTodos())
            {
                _gridHistorico.Rows.Add(
                    evento.DataHora.ToString("dd/MM/yyyy HH:mm:ss"),
                    evento.Tipo,
                    evento.CodigoRelacionado,
                    evento.Descricao);
            }

            if (_gridHistorico.Rows.Count > 0)
            {
                _gridHistorico.Sort(
                    _gridHistorico.Columns["Data"],
                    System.ComponentModel.ListSortDirection.Descending);
            }
        }

        private void ExportarHistorico()
        {
            using SaveFileDialog dialogo = new()
            {
                Filter = "Arquivo CSV (*.csv)|*.csv",
                FileName = "historico-encosta-segura.csv"
            };

            if (dialogo.ShowDialog() != DialogResult.OK)
                return;

            try
            {
                List<string> linhas = new()
                {
                    "Data;Tipo;Codigo;Descricao"
                };

                foreach (Estacao estacao in _dados.Estacoes)
                {
                    foreach (Leitura leitura in estacao.Historico)
                    {
                        NivelRisco risco = estacao.CalcularRisco(leitura);

                        linhas.Add(
                            $"{EscaparCsv(leitura.DataHora.ToString("dd/MM/yyyy HH:mm:ss"))};" +
                            $"{EscaparCsv("Medição")};" +
                            $"{EscaparCsv(estacao.Codigo)};" +
                            $"{EscaparCsv(estacao.DescreverLeitura(leitura) + " | Risco: " + FormatarRisco(risco))}");
                    }
                }

                foreach (EventoHistorico evento in _historicoService.ObterTodos())
                {
                    linhas.Add(
                        $"{EscaparCsv(evento.DataHora.ToString("dd/MM/yyyy HH:mm:ss"))};" +
                        $"{EscaparCsv(evento.Tipo)};" +
                        $"{EscaparCsv(evento.CodigoRelacionado)};" +
                        $"{EscaparCsv(evento.Descricao)}");
                }

                File.WriteAllLines(
                    dialogo.FileName,
                    linhas,
                    System.Text.Encoding.UTF8);

                MessageBox.Show("Histórico exportado com sucesso.");
            }
            catch (IOException ex)
            {
                MostrarErro("Erro ao exportar: " + ex.Message);
            }
        }

        private static string EscaparCsv(string texto)
        {
            return "\"" + texto.Replace("\"", "\"\"") + "\"";
        }

        // ================================================
        // ATUALIZAÇÃO DAS TELAS
        // ================================================

        private void AtualizarIndicadoresDefesaCivil()
        {
            int acionamentos = 0;
            int notificacoes = 0;

            foreach (EventoHistorico evento in _historicoService.ObterTodos())
            {
                if (evento.Tipo == "Sirene")
                    acionamentos++;

                if (evento.Tipo == "Notificação")
                    notificacoes++;
            }

            _lblSirene.Text = acionamentos.ToString();
            _lblNotificacoes.Text = notificacoes.ToString();
        }

        private void AtualizarTelas()
        {
            AtualizarEstacoes();
            AtualizarPontos();
            AtualizarResumo();
            AtualizarHistorico();
            AtualizarIndicadoresDefesaCivil();

        }

        private void AtualizarEstacoes()
        {
            _gridEstacoes.Rows.Clear();

            foreach (Estacao estacao in _estacaoService.ObterTodas())
            {
                string tipo = estacao is EstacaoEncosta
                    ? "Encosta"
                    : "Córrego";

                string leitura = estacao.UltimaLeitura is null
                    ? "Sem leituras"
                    : estacao.DescreverLeitura(estacao.UltimaLeitura);

                int indice = _gridEstacoes.Rows.Add(
                    estacao.Codigo,
                    estacao.Nome,
                    tipo,
                    estacao.Localizacao,
                    FormatarRisco(estacao.RiscoAtual),
                    estacao.Ativa ? "Ativa" : "Inativa",
                    leitura);

                ColorirLinha(
                    _gridEstacoes.Rows[indice],
                    estacao.RiscoAtual);

                if (!estacao.Ativa)
                {
                    _gridEstacoes.Rows[indice]
                        .DefaultCellStyle.ForeColor = Color.Gray;
                }
            }

            AtualizarGradeResumo();
        }

        private void AtualizarGradeResumo()
        {
            _gridResumo.Rows.Clear();

            foreach (Estacao estacao in _dados.Estacoes)
            {
                string tipo = estacao is EstacaoEncosta
                    ? "Encosta"
                    : "Córrego";

                string leitura = estacao.UltimaLeitura is null
                    ? "Sem leituras"
                    : estacao.UltimaLeitura.DataHora.ToString(
                        "dd/MM/yyyy HH:mm");

                int indice = _gridResumo.Rows.Add(
                    estacao.Codigo,
                    estacao.Nome,
                    tipo,
                    estacao.Localizacao,
                    FormatarRisco(estacao.RiscoAtual),
                    estacao.Ativa ? "Ativa" : "Inativa",
                    leitura);

                DataGridViewRow linha = _gridResumo.Rows[indice];

                // Verde para estações ativas e vermelho para inativas.
                if (estacao.Ativa)
                {
                    linha.DefaultCellStyle.BackColor = Color.LightGreen;
                    linha.DefaultCellStyle.ForeColor = Color.DarkGreen;
                }
                else
                {
                    linha.DefaultCellStyle.BackColor = Color.LightCoral;
                    linha.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
            }
        }

        private void AtualizarPontos()
        {
            _gridPontos.Rows.Clear();

            foreach (PontoDeApoio ponto in _pontoService.ObterTodos())
            {
                _gridPontos.Rows.Add(
                    ponto.Codigo,
                    ponto.Nome,
                    ponto.Endereco,
                    ponto.RotaSegura,
                    ponto.Ativo ? "Ativo" : "Inativo");
            }
        }

        private void AtualizarResumo()
        {
            int ativos = 0;
            int alertas = 0;

            Estacao? encosta = null;
            Estacao? corrego = null;

            foreach (Estacao estacao in _dados.Estacoes)
            {
                if (!estacao.Ativa)
                    continue;

                ativos++;

                if (estacao.RiscoAtual == NivelRisco.Alto)
                    alertas++;

                if (estacao is EstacaoEncosta)
                {
                    if (encosta is null ||
                        (int)estacao.RiscoAtual > (int)encosta.RiscoAtual)
                    {
                        encosta = estacao;
                    }
                }

                if (estacao is EstacaoCorrego)
                {
                    if (corrego is null ||
                        (int)estacao.RiscoAtual > (int)corrego.RiscoAtual)
                    {
                        corrego = estacao;
                    }
                }
            }

            _lblEstacoesAtivas.Text = ativos.ToString();
            _lblAlertas.Text = alertas.ToString();

            _lblRiscoEncosta.Text = encosta is null
                ? "Sem estação"
                : FormatarRisco(encosta.RiscoAtual);

            _lblRiscoCorrego.Text = corrego is null
                ? "Sem estação"
                : FormatarRisco(corrego.RiscoAtual);

            ColorirIndicador(_lblRiscoEncosta, encosta?.RiscoAtual);
            ColorirIndicador(_lblRiscoCorrego, corrego?.RiscoAtual);
        }


        // ================================================
        // MÉTODOS AUXILIARES
        // ================================================

        private static string FormatarRisco(NivelRisco risco)
        {
            return risco switch
            {
                NivelRisco.Baixo => "BAIXO",
                NivelRisco.Atencao => "ATENÇÃO",
                NivelRisco.Alto => "ALTO",
                _ => "DESCONHECIDO"
            };
        }

        private static void ColorirLinha(
            DataGridViewRow linha,
            NivelRisco risco)
        {
            linha.DefaultCellStyle.BackColor = risco switch
            {
                NivelRisco.Baixo => Color.LightGreen,
                NivelRisco.Atencao => Color.LightYellow,
                NivelRisco.Alto => Color.LightCoral,
                _ => Color.White
            };
        }

        private static void ColorirIndicador(
            Label label,
            NivelRisco? risco)
        {
            label.ForeColor = risco switch
            {
                NivelRisco.Baixo => Color.DarkGreen,
                NivelRisco.Atencao => Color.DarkOrange,
                NivelRisco.Alto => Color.DarkRed,
                _ => Color.Black
            };
        }

        private static Panel CriarIndicador(string titulo, Label valor)
        {
            Panel painel = new()
            {
                Size = new Size(235, 100),
                BackColor = Color.FromArgb(240, 245, 242),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(8)
            };

            Label labelTitulo = new()
            {
                Text = titulo,
                Location = new Point(12, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(55, 75, 65)
            };

            valor.Location = new Point(12, 42);
            valor.AutoSize = true;
            valor.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            valor.ForeColor = Color.FromArgb(35, 95, 75);
            valor.Text = "0";

            painel.Controls.Add(labelTitulo);
            painel.Controls.Add(valor);

            return painel;
        }

        private static DataGridView CriarGrade()
        {
            return new DataGridView
            {
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private static Panel CriarPainel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20)
            };
        }

        private static Label CriarTitulo(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                Location = new Point(20, 20)
            };
        }

        private static Label CriarTituloMenor(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold)
            };
        }

        private static Label CriarDescricao(string texto)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Location = new Point(20, 70),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.DimGray
            };
        }

        private static Label CriarLabel(
            string texto,
            int x,
            int y)
        {
            return new Label
            {
                Text = texto,
                AutoSize = true,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 10)
            };
        }

        private static TextBox CriarCampo(
            int x,
            int y,
            int largura)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = largura,
                Font = new Font("Segoe UI", 10)
            };
        }

        private static NumericUpDown CriarNumero(
            int x,
            int y)
        {
            return new NumericUpDown
            {
                Location = new Point(x, y),
                Width = 300,
                Minimum = 0,
                Maximum = 1000,
                DecimalPlaces = 1,
                Font = new Font("Segoe UI", 10)
            };
        }

        private static Button CriarBotao(
            string texto,
            int x,
            int y,
            int largura)
        {
            return new Button
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(largura, 38),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(35, 95, 75),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        private static Form CriarJanelaFormulario(
            string titulo,
            int largura,
            int altura)
        {
            return new Form
            {
                Text = titulo,
                Size = new Size(largura, altura),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };
        }

        private static void MostrarErro(string mensagem)
        {
            MessageBox.Show(
                mensagem,
                "Atenção",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}