# RadarSantista — Monitoramento Portuário 🚢

Um sistema autônomo de raspagem de dados (Web Scraper) desenvolvido para monitorar o fluxo de navios atracados e programados no Porto de Santos.

## O que o sistema faz

* **Coleta Autônoma:** Realiza a varredura cíclica de portais da web, extraindo dados brutos sobre a movimentação dos navios.
* **Tratamento de Dados:** Converte informações textuais não estruturadas da internet em dados limpos e organizados.
* **Filtro de Mudança de Estado:** Compara as informações capturadas com o último registro salvo no banco de dados. O sistema só efetua um novo `INSERT` se o navio sofrer alguma alteração real (como mudança de status, terminal ou carga).
* **Histórico por Eventos:** Gera uma linha do tempo enxuta da atividade portuária, armazenando apenas as movimentações relevantes e eliminando registros duplicados.

## Tecnologias Utilizadas

* **Linguagem:** C# / .NET 10
* **Banco de Dados:** SQLite
* **Acesso a Dados:** Dapper (Micro-ORM)
* **Extração de Dados:** HtmlAgilityPack
