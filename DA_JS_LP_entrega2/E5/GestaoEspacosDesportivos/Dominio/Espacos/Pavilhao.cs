namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Subclasse que representa um Pavilhão Polidesportivo (futsal, basquetebol, andebol, voleibol).
/// </summary>
public class Pavilhao : EspacoDesportivo
{
    public string PisoPolidesportivo { get; set; } = string.Empty;
    public List<string> Modalidades { get; set; } = new();

    public Pavilhao() { }

    public Pavilhao(
        string id, 
        string nome, 
        string localizacao, 
        int capacidadeMaxima, 
        double precoHora, 
        string pisoPolidesportivo = "Madeira Flutuante")
        : base(id, nome, TipoEspaco.Pavilhao, localizacao, capacidadeMaxima, precoHora)
    {
        this.PisoPolidesportivo = pisoPolidesportivo;
        this.Modalidades = new List<string> { "Futsal", "Basquetebol", "Andebol", "Voleibol" };
    }

    public override double CalcularCustoReserva(double duracaoHoras)
    {
        double taxaManutencao = 8.0;
        return (PrecoHora * duracaoHoras) + taxaManutencao;
    }

    public override string ObterDescricaoTecnica()
    {
        return $"Pavilhão Multidesportivo ({PisoPolidesportivo}). Modalidades: {string.Join(", ", Modalidades)}. Bancadas com lotação até {CapacidadeMaxima} pessoas.";
    }
}
