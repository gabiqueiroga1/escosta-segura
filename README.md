#  Encosta Segura

O Encosta Segura é uma aplicação desenvolvida em C# com Windows Forms para demonstrar o monitoramento de riscos de deslizamentos e alagamentos.

##  Sobre o projeto

Sistema desenvolvido com o objetivo de auxiliar no monitoramento de áreas suscetíveis a desastres naturais, contribuindo para a prevenção de acidentes e a proteção de pessoas que vivem em regiões de risco.

##  Problema escolhido

O projeto aborda a vulnerabilidade de comunidades localizadas próximas a encostas e córregos, que podem ser afetadas por deslizamentos de terra e alagamentos, especialmente durante períodos de chuvas intensas.

A falta de monitoramento contínuo e de centralização das informações sobre as condições dessas áreas pode dificultar a identificação de situações de risco e a organização de ações preventivas.

Diante desse cenário, o **Encosta Segura** propõe uma solução computacional para simular o monitoramento de variáveis ambientais, classificar níveis de risco e disponibilizar informações que auxiliem no acompanhamento dessas situações.

## Problema escolhido e Beneficiários

O problema escolhido é a vulnerabilidade de comunidades que vivem próximas a encostas e córregos, estando sujeitas a deslizamentos de terra e alagamentos.

Beneficiário real: moradores de regiões de risco, especialmente aqueles que vivem próximos a encostas e córregos e que podem ser diretamente afetados por esses desastres.

O sistema busca contribuir para a segurança dessas pessoas, oferecendo uma ferramenta de monitoramento e identificação de situações de risco.

##  Objetivos

- Monitorar condições que podem indicar riscos de deslizamentos e alagamentos.
- Classificar os níveis de risco das estações.
- Facilitar a visualização das informações em um painel central.
- Gerenciar pontos de apoio e rotas seguras.
- Registrar eventos e ações da Defesa Civil.

##  Funcionalidades

### 1. Visão geral

- Painel com resumo das estações monitoradas.
- Quantidade de estações cadastradas.
- Identificação de alertas de alto risco.
- Visualização dos níveis de risco por estação.

### 2. Gerenciamento de estações

- Cadastro de estações de monitoramento.
- Edição e desativação de estações.
- Registro de novas leituras.
- Consulta ao histórico de leituras.
- Classificação automática do nível de risco.

O sistema possui dois tipos de estações:

| Estação | Monitoramento |
|---|---|
| ENC-01 | Risco de deslizamento de terra |
| NIV-01 | Risco de alagamento |

**Estação ENC-01 — Encosta**

- Umidade do solo.
- Volume de chuva acumulado em 72 horas.
- Inclinação do terreno.

**Estação NIV-01 — Córrego**

- Nível da água.
- Velocidade de subida da água.

### 3. Pontos de apoio

- Cadastro de pontos de apoio.
- Edição e desativação de pontos.
- Consulta aos endereços e às rotas seguras.
- Identificação de locais de referência para a população.

Pontos cadastrados inicialmente:

- Escola Municipal.
- Centro Comunitário em área elevada.

### 4. Defesa Civil

- Registro simulado de acionamento de sirene.
- Registro simulado de notificações aos moradores.
- Contabilização das ações realizadas.
- Armazenamento das ações no histórico do sistema.

### 5. Histórico

- Registro de leituras e eventos.
- Consulta às ações realizadas.
- Exportação do histórico em arquivo CSV.

##  Classificação de risco

O sistema utiliza três níveis de classificação:

| Nível | Representação |
|---|---|
| BAIXO | 🟢 Verde |
| ATENÇÃO | 🟡 Amarelo |
| ALTO | 🔴 Vermelho |

A classificação é realizada com base nos parâmetros registrados em cada estação e nas regras de negócio implementadas no sistema.

##  Tecnologias utilizadas

- **C#** — linguagem de programação.
- **.NET 8** — plataforma de desenvolvimento.
- **Windows Forms** — criação da interface gráfica.
- **JSON** — armazenamento local dos dados.
- **CSV** — exportação do histórico.
- **Git e GitHub** — controle de versão e gerenciamento do código-fonte.
- **Visual Studio** — ambiente de desenvolvimento.

##  Estrutura do projeto

```text
EncostaSegura/
│
├── Models/
│   ├── Enumeracoes/
│   │   ├── NivelRisco.cs
│   │   └── TipoEstacao.cs
│   │
│   ├── Estacao.cs
│   ├── EstacaoEncosta.cs
│   ├── EstacaoCorrego.cs
│   ├── Leitura.cs
│   ├── PontoDeApoio.cs
│   └── EventoHistorico.cs
│
├── Services/
│   ├── EstacaoService.cs
│   ├── PontoDeApoioService.cs
│   └── HistoricoService.cs
│
├── Data/
│   ├── DadosSistema.cs
│   └── ConfiguracaoRepository.cs
│
├── Form1.cs
├── Form1.Designer.cs
├── Form1.resx
├── Program.cs
└── EncostaSegura.csproj

##  Equipe

- **Davi Correa Paião** | RM: 560438
- **Gabriela Queiroga** | RM: 560035
- **Maria Eduarda Ferrés** | RM: 560418
