using RadarSantista.src.Models;
using RadarSantista.src.Services;

namespace RadarSantista.Tests;

public class NavioStateServiceTests
{
    private readonly NavioStateService _service = new();

    [Fact]
    public void DevePersistirAtracado_WhenNaoHaUltimoRegistro()
    {
        var navio = new Navio { Nome = "NAVIO 1", Terminal = "T1", Status = "OPERANDO" };

        var resultado = _service.DevePersistirAtracado(navio, null);

        Assert.True(resultado);
    }

    [Fact]
    public void NaoDevePersistirAtracado_WhenEstadoNaoMudou()
    {
        var novo = new Navio { Nome = "NAVIO 1", Terminal = "T1", Status = "OPERANDO" };
        var anterior = new Navio { Nome = "NAVIO 1", Terminal = "T1", Status = "OPERANDO" };

        var resultado = _service.DevePersistirAtracado(novo, anterior);

        Assert.False(resultado);
    }

    [Fact]
    public void DevePersistirAtracado_WhenStatusMudou()
    {
        var novo = new Navio { Nome = "NAVIO 1", Terminal = "T1", Status = "OPERANDO" };
        var anterior = new Navio { Nome = "NAVIO 1", Terminal = "T1", Status = "PROGRAMADO" };

        var resultado = _service.DevePersistirAtracado(novo, anterior);

        Assert.True(resultado);
    }

    [Fact]
    public void DevePersistirProgramado_WhenDataMudou()
    {
        var novo = new Navio { Nome = "NAVIO 1", DataPrevisao = new DateTime(2026, 7, 5, 10, 0, 0) };
        var anterior = new Navio { Nome = "NAVIO 1", DataPrevisao = new DateTime(2026, 7, 4, 10, 0, 0) };

        var resultado = _service.DevePersistirProgramado(novo, anterior);

        Assert.True(resultado);
    }

    [Fact]
    public void SnapshotDeveNormalizarECompararEstadoDeFormaCanonica()
    {
        var navio = new Navio { Nome = "  navio  ", Terminal = "  t1  ", Status = " operando " };

        var snapshot = NavioEstadoSnapshot.Criar(navio);

        Assert.Equal("NAVIO", snapshot.Nome);
        Assert.Equal("T1", snapshot.Terminal);
        Assert.Equal("OPERANDO", snapshot.Status);
    }
}
