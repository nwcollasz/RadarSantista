# RadarSantista — Monitoramento Portuário 🚢

Um sistema autônomo web scraper desenvolvido para monitorar o fluxo de navios atracados e programados no site do Porto de Santos.

## O que o sistema faz

- Web scraping contínuo: Um script roda periodicamente monitorando os portais web para extrair os dados brutos dos navios.

- Parsing: Transforma os textos não estruturados coletados da internet em dados limpos e padronizados.

- Filtro de mudança de estado: Compara os dados capturados com o último registro do banco. Só faz um novo INSERT se houver alteração real de status, terminal ou carga, evitando escrita redundante.

- Histórico por eventos: Gera uma linha do tempo da atividade portuária, salvando apenas eventos relevantes e descartando duplicatas.

## Tecnologias

*  C# / .NET 10
* Banco de Dados: SQLite
* Acesso a Dados: Dapper (Micro-ORM)
* Extração de Dados: HtmlAgilityPack
