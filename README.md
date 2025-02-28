# Sistema de Relatórios de Pedágio

O **Sistema de Relatórios de Pedágio** é uma solução robusta e escalável desenvolvida para consolidar e processar dados de utilização de praças de pedágio em todo o país. O sistema foi concebido para atender às demandas de grandes volumes de dados, fornecendo relatórios detalhados para suporte à tomada de decisão e administração estratégica.

## Visão Geral do Projeto

A aplicação foi projetada utilizando a plataforma .NET Aspire e conta com integração a diversos serviços essenciais, como RabbitMQ para mensageria e SQL Server (ou outras opções, como Postgres, conforme a necessidade) para persistência de dados. A arquitetura do sistema possibilita a ingestão de milhões de registros diários, mantendo desempenho e confiabilidade, além de suportar OpenTelemetry para monitoramento.

### Funcionalidades Principais

- **Recepção de Dados:**  
  Uma API robusta permite o recebimento dos dados de utilização das praças, que incluem:
  - Data e hora de utilização
  - Identificação da praça
  - Cidade e estado
  - Valor pago
  - Tipo de veículo (Moto, Carro ou Caminhão)

- **Geração de Relatórios:**  
  O sistema processa os dados recebidos e gera relatórios customizados, tais como:
  - Valor total faturado por hora em cada cidade
  - Ranking das praças com maior faturamento por mês (com quantidade configurável)
  - Quantidade de veículos por tipo em cada praça

- **Processamento e Persistência:**  
  Os dados são armazenados e processados para garantir a consistência e a precisão das informações, permitindo a análise de grandes volumes de registros.

- **Integração e Escalabilidade:**  
  A arquitetura permite a integração com diferentes componentes e serviços, possibilitando a troca ou adição de tecnologias (ex.: migração de RabbitMQ para Kafka) sem prejuízo ao funcionamento do sistema.

## Estrutura do Repositório

O repositório está organizado da seguinte forma:

```
Thunders.TechTest/
    ├── Abstractions
    ├── ApiService
    │   ├── Controllers
    │   └── Validators
    ├── AppHost
    ├── Application
    │   ├── Interfaces
    │   ├── Messaging
    │   └── Services
    ├── Domain
    │   ├── Entities
    │   ├── Enums
    │   └── Models
    ├── Infrastructure
    │   ├── Data
    │   ├── Interfaces
    │   ├── Migrations
    │   └── Repositories
    ├── OutOfBox
    │   ├── Database
    │   └── Queues
    └── Tests
        ├── ApiService
        │   └── Controllers
        ├── Application
        │   ├── Messaging
        │   └── Services
        └── Infrastructure
            └── Repositories

```

## Como Executar o Projeto

1. **Clone o Repositório:**
   ```bash
   git clone https://github.com/pedro381/teste-tecnico-v2.git
   ```
2. **Configure a Solução:**
   - Abra a solução em sua IDE preferida.
   - Defina o projeto `AppHost` como startup project.
   - Ajuste as configurações necessárias no arquivo `Configuration/AppSettings.json` (conexões, timeout, etc.).

3. **Inicie a Aplicação:**
   - Certifique-se de que os serviços externos (RabbitMQ, SQL Server/Postgres, etc.) estejam configurados e em execução.
   - Execute o projeto `AppHost` para iniciar a aplicação.

4. **Executando Testes:**
   - Utilize o framework de testes incluído para validar as funcionalidades do sistema.  
   - Execute os testes unitários e de integração presentes na pasta `Tests`.

## Considerações Finais

O **Sistema de Relatórios de Pedágio** foi desenvolvido com foco em escalabilidade, desempenho e facilidade de manutenção. A arquitetura modular e a utilização de componentes modernos permitem que o sistema evolua conforme as necessidades do mercado e a integração com novas tecnologias.
