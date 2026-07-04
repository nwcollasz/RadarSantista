# RadarSantista — Monitoramento Portuário 🚢

Um sistema autônomo web scraper desenvolvido para monitorar o fluxo de navios atracados e programados no site do Porto de Santos.

É um projeto para fins de estudo, com o objetivo de praticar arquitetura de software e tratamento de dados em C#.

## O que o sistema faz

- Web scraping: Um script roda de tempos em tempos monitorando os portais do porto para extrair os dados brutos dos navios.

- Parsing: Trata textos zoados e remove espaços invisíveis ou quebras de linha do HTML.

- Filtro de mudança de estado: Usa snapshots na memória para checar se o navio mudou de verdade de estado. Se o dado for repetido, o sistema ignora e não polui o banco.

- Histórico por eventos: Cria uma linha do tempo de toda a atividade dos navios.

## O que foi utilizado

*  C# / .NET 10
*  SQL Server LocalDB
*  Dapper
*  HtmlAgilityPack
*  xUnit
