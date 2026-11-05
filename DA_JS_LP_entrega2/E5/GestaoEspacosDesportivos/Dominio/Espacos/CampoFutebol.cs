namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Subclasse concreta que representa um Campo de Futebol (ex: relva sintética, iluminação noturna).
/// </summary>
public class CampoFutebol : EspacoDesportivo
{
    public string TipoPiso { get; set; } = string.Empty;
    public bool TemIluminacao { get; set; }

    public CampoFutebol() { }

    public CampoFutebol(
        string id, 
        string nome, 
        string localizacao, 
        int capacidadeMaxima, 
        double precoHora, 
        string tipoPiso = "Sintético 4G", 
        bool temIluminacao = true)
        : base(id, nome, TipoEspaco.CampoFutebol, localizacao, capacidadeMaxima, precoHora)
    {
        this.TipoPiso = tipoPiso;
        this.TemIluminacao = temIluminacao;
    }

    /// <summary>
    /// Calcula o custo base e aplica uma taxa de iluminação noturna caso exista.
    /// </summary>
    public override double CalcularCustoReserva(double duracaoHoras)
    {
        double taxaLuz = TemIluminacao ? 5.0 * duracaoHoras : 0.0;
        return (PrecoHora * duracaoHoras) + taxaLuz;
    }

    public override string ObterDescricaoTecnica()
    {
        return $"Campo de Futebol de {CapacidadeMaxima} jogadores. Piso: {TipoPiso}. Iluminação artificial: {(TemIluminacao ? "Sim" : "Não")}.";
    }
}
