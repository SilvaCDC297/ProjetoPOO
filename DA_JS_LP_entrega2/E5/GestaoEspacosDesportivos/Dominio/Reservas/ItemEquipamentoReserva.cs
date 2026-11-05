using GestaoEspacosDesportivos.Dominio.Espacos;

namespace GestaoEspacosDesportivos.Dominio.Reservas;

/// <summary>
/// Representa a requisição de uma determinada quantidade de um equipamento para uma reserva.
/// </summary>
public class ItemEquipamentoReserva
{
    public Equipamento Equipamento { get; set; } = null!;
    public int QuantidadeRequisitada { get; set; }

    public ItemEquipamentoReserva() { }

    public ItemEquipamentoReserva(Equipamento equipamento, int quantidadeRequisitada)
    {
        ArgumentNullException.ThrowIfNull(equipamento);
        if (quantidadeRequisitada <= 0) 
            throw new ArgumentOutOfRangeException(nameof(quantidadeRequisitada), "A quantidade de equipamento deve ser positiva.");

        this.Equipamento = equipamento;
        this.QuantidadeRequisitada = quantidadeRequisitada;
    }

    public Equipamento GetEquipamento() => Equipamento;
    public int GetQuantidadeRequisitada() => QuantidadeRequisitada;

    public override string ToString()
    {
        return $"{QuantidadeRequisitada}x {Equipamento.Nome}";
    }
}
