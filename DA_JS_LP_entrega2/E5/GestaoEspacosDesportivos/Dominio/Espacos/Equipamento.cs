namespace GestaoEspacosDesportivos.Dominio.Espacos;

/// <summary>
/// Equipamento desportivo associável a instalações (ex: bolas, redes, raquetes, colchões).
/// </summary>
public class Equipamento
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public int QuantidadeTotal { get; set; }
    public int QuantidadeDisponivel { get; set; }
    public EstadoEquipamento Estado { get; set; }

    public Equipamento() { }

    public Equipamento(string id, string nome, int quantidadeTotal, EstadoEquipamento estado = EstadoEquipamento.Bom)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID do equipamento obrigatório.", nameof(id));
        if (string.IsNullOrWhiteSpace(nome)) throw new ArgumentException("Nome do equipamento obrigatório.", nameof(nome));
        if (quantidadeTotal < 0) throw new ArgumentOutOfRangeException(nameof(quantidadeTotal), "Quantidade não pode ser negativa.");

        this.Id = id;
        this.Nome = nome.Trim();
        this.QuantidadeTotal = quantidadeTotal;
        this.QuantidadeDisponivel = quantidadeTotal;
        this.Estado = estado;
    }

    /// <summary>
    /// Aloca uma determinada quantidade de equipamento, se houver stock suficiente.
    /// </summary>
    public bool Alocar(int qtd)
    {
        if (qtd <= 0 || qtd > QuantidadeDisponivel) return false;
        QuantidadeDisponivel -= qtd;
        return true;
    }

    /// <summary>
    /// Liberta equipamento devolvido para o inventário disponível.
    /// </summary>
    public void Libertar(int qtd)
    {
        if (qtd <= 0) return;
        QuantidadeDisponivel = Math.Min(QuantidadeTotal, QuantidadeDisponivel + qtd);
    }

    public void AtualizarEstado(EstadoEquipamento novoEstado)
    {
        this.Estado = novoEstado;
    }

    public string GetId() => Id;
    public string GetNome() => Nome;
    public int GetQuantidadeDisponivel() => QuantidadeDisponivel;

    public override string ToString()
    {
        return $"{Nome} (Disponível: {QuantidadeDisponivel}/{QuantidadeTotal}) [{Estado}]";
    }
}
