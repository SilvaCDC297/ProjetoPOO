using GestaoEspacosDesportivos.Dominio.Reservas;

namespace GestaoEspacosDesportivos.Servicos;

/// <summary>
/// Módulo analítico para geração de indicadores, estatísticas e taxas de ocupação.
/// </summary>
public class GestorRelatorios
{
    private readonly GestorReservas gestorReservas;
    private readonly GestorEspacos gestorEspacos;

    public GestorRelatorios(GestorReservas gestorReservas, GestorEspacos gestorEspacos)
    {
        this.gestorReservas = gestorReservas ?? throw new ArgumentNullException(nameof(gestorReservas));
        this.gestorEspacos = gestorEspacos ?? throw new ArgumentNullException(nameof(gestorEspacos));
    }

    public int GetTotalReservas()
    {
        return gestorReservas.ListarTodas().Count;
    }

    /// <summary>
    /// Agrupa o número de reservas por cada espaço desportivo.
    /// </summary>
    public Dictionary<string, int> GetReservasPorEspaco()
    {
        return gestorReservas.ListarTodas()
            .GroupBy(r => r.Espaco.Nome)
            .ToDictionary(g => g.Key, g => g.Count());
    }

    /// <summary>
    /// Retorna os espaços desportivos com maior volume de procura.
    /// </summary>
    public List<KeyValuePair<string, int>> GetEspacosMaisUtilizados(int limite = 5)
    {
        return GetReservasPorEspaco()
            .OrderByDescending(kv => kv.Value)
            .Take(limite)
            .ToList();
    }

    /// <summary>
    /// Calcula a taxa percentual de ocupação de um recinto num intervalo temporal.
    /// Taxa = (Horas Reservadas / Horas Disponíveis Teóricas) * 100
    /// </summary>
    public double CalcularTaxaOcupacao(string espacoId, DateOnly inicio, DateOnly fim, double horasDisponiveisPorDia = 14.0)
    {
        var espaco = gestorEspacos.ObterPorId(espacoId);
        int totalDias = Math.Max(1, fim.DayNumber - inicio.DayNumber + 1);
        double horasDisponiveisTotais = totalDias * horasDisponiveisPorDia;

        var reservasNoPeriodo = gestorReservas.ListarTodas()
            .Where(r => r.Espaco.Id.Equals(espaco.Id, StringComparison.OrdinalIgnoreCase)
                     && r.Data >= inicio 
                     && r.Data <= fim
                     && (r.Estado == EstadoReserva.Confirmada || r.Estado == EstadoReserva.Concluida));

        double horasReservadas = reservasNoPeriodo.Sum(r => r.DuracaoHoras());

        if (horasDisponiveisTotais <= 0) return 0.0;
        return Math.Min(100.0, (horasReservadas / horasDisponiveisTotais) * 100.0);
    }
}
