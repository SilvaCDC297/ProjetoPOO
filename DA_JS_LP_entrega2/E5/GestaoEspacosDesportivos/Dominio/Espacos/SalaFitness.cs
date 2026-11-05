namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Subclasse que representa uma Sala de Fitness / Estúdio de Aulas de Grupo.
/// </summary>
public class SalaFitness : EspacoDesportivo
{
    public bool Climatizada { get; set; }
    public bool EspelhosIntegrais { get; set; }

    public SalaFitness() { }

    public SalaFitness(
        string id, 
        string nome, 
        string localizacao, 
        int capacidadeMaxima, 
        double precoHora, 
        bool climatizada = true, 
        bool espelhosIntegrais = true)
        : base(id, nome, TipoEspaco.SalaFitness, localizacao, capacidadeMaxima, precoHora)
    {
        this.Climatizada = climatizada;
        this.EspelhosIntegrais = espelhosIntegrais;
    }

    public override double CalcularCustoReserva(double duracaoHoras)
    {
        return PrecoHora * duracaoHoras;
    }

    public override string ObterDescricaoTecnica()
    {
        return $"Estúdio de Fitness e Musculação. Climatização: {(Climatizada ? "Sim" : "Não")}. Espelhos de parede: {(EspelhosIntegrais ? "Sim" : "Não")}. Sistema de som integrado.";
    }
}
