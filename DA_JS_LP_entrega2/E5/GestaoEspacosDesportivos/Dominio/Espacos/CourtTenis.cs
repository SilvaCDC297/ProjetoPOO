namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Subclasse que representa um Court de Ténis (ex: piso rápido ou terra batida, coberto/descoberto).
/// </summary>
public class CourtTenis : EspacoDesportivo
{
    public string TipoSuperficie { get; set; } = string.Empty;
    public bool IsCoberto { get; set; }

    public CourtTenis() { }

    public CourtTenis(
        string id, 
        string nome, 
        string localizacao, 
        int capacidadeMaxima, 
        double precoHora, 
        string tipoSuperficie = "Piso Rápido / Resina", 
        bool isCoberto = false)
        : base(id, nome, TipoEspaco.CourtTenis, localizacao, capacidadeMaxima, precoHora)
    {
        this.TipoSuperficie = tipoSuperficie;
        this.IsCoberto = isCoberto;
    }

    public override double CalcularCustoReserva(double duracaoHoras)
    {
        double multiplicador = IsCoberto ? 1.15 : 1.0;
        return (PrecoHora * duracaoHoras) * multiplicador;
    }

    public override string ObterDescricaoTecnica()
    {
        return $"Court de Ténis regulamentar. Superfície: {TipoSuperficie}. Cobertura: {(IsCoberto ? "Recinto Coberto" : "Ao Ar Livre")}.";
    }
}
