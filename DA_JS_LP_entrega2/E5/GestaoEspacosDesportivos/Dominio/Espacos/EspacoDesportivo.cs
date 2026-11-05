using System.Text.Json.Serialization;

namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Classe abstrata base para instalações desportivas.
/// Modela atributos e comportamentos comuns, definindo métodos polimórficos de cálculo de custos e fichas técnicas.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(CampoFutebol), "campo_futebol")]
[JsonDerivedType(typeof(CourtTenis), "court_tenis")]
[JsonDerivedType(typeof(Pavilhao), "pavilhao")]
[JsonDerivedType(typeof(SalaFitness), "sala_fitness")]
public abstract class EspacoDesportivo
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoEspaco Tipo { get; set; }
    public string Localizacao { get; set; } = string.Empty;
    public int CapacidadeMaxima { get; set; }
    public EstadoEspaco Estado { get; set; }
    public double PrecoHora { get; set; }
    public List<Equipamento> Equipamentos { get; set; } = new();

    protected EspacoDesportivo() { }

    protected EspacoDesportivo(
        string id, 
        string nome, 
        TipoEspaco tipo, 
        string localizacao, 
        int capacidadeMaxima, 
        double precoHora)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID do espaço obrigatório.", nameof(id));
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do espaço obrigatório.", nameof(nome));
        if (capacidadeMaxima <= 0) throw new ArgumentOutOfRangeException(nameof(capacidadeMaxima), "Capacidade máxima deve ser superior a zero.");
        if (precoHora < 0) throw new ArgumentOutOfRangeException(nameof(precoHora), "Preço por hora inválido.");

        this.Id = id;
        this.Nome = nome.Trim();
        this.Tipo = tipo;
        this.Localizacao = localizacao.Trim();
        this.CapacidadeMaxima = capacidadeMaxima;
        this.PrecoHora = precoHora;
        this.Estado = EstadoEspaco.Disponivel;
        this.Equipamentos = new List<Equipamento>();
    }

    public virtual bool IsDisponivel() => Estado == EstadoEspaco.Disponivel;

    public virtual void SetEstado(EstadoEspaco novo)
    {
        this.Estado = novo;
    }

    public void AdicionarEquipamento(Equipamento eq)
    {
        ArgumentNullException.ThrowIfNull(eq);
        Equipamentos.Add(eq);
    }

    public void RemoverEquipamento(string idEquipamento)
    {
        Equipamentos.RemoveAll(e => e.Id == idEquipamento);
    }

    public List<Equipamento> GetEquipamentos() => new(Equipamentos);

    public string GetId() => Id;
    public string GetNome() => Nome;
    public double GetPrecoHora() => PrecoHora;

    /// <summary>
    /// Método abstrato que cada tipo de espaço implementa para calcular o custo (polimorfismo).
    /// </summary>
    public abstract double CalcularCustoReserva(double duracaoHoras);

    /// <summary>
    /// Retorna detalhes técnicos específicos da instalação.
    /// </summary>
    public abstract string ObterDescricaoTecnica();

    public override string ToString()
    {
        return $"[{Tipo}] {Nome} ({Localizacao}) - Lotação: {CapacidadeMaxima} pess. - {PrecoHora:C}/h [{Estado}]";
    }
}
