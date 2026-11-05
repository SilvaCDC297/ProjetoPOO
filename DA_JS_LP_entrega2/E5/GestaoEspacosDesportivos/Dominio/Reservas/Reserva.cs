using GestaoEspacosDesportivos.Dominio.Espacos;
using GestaoEspacosDesportivos.Dominio.Utilizadores;
using GestaoEspacosDesportivos.Excecoes;

namespace GestaoEspacosDesportivos.Dominio.Reservas;

/// <summary>
/// Entidade central de Reserva que orquestra utilizador, espaço, horários, equipamentos e ciclo de vida.
/// </summary>
public class Reserva
{
    public string Id { get; set; } = string.Empty;
    public Utilizador Utilizador { get; set; } = null!;
    public EspacoDesportivo Espaco { get; set; } = null!;
    public DateOnly Data { get; set; }
    public TimeOnly HoraInicio { get; set; }
    public TimeOnly HoraFim { get; set; }
    public EstadoReserva Estado { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<ItemEquipamentoReserva> ItensEquipamento { get; set; } = new();

    public Reserva() { }

    public Reserva(
        string id, 
        Utilizador utilizador, 
        EspacoDesportivo espaco, 
        DateOnly data, 
        TimeOnly horaInicio, 
        TimeOnly horaFim)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("ID da reserva obrigatório.", nameof(id));
        ArgumentNullException.ThrowIfNull(utilizador);
        ArgumentNullException.ThrowIfNull(espaco);

        if (horaFim <= horaInicio)
            throw new DadosInvalidosException("HoraFim", "A hora de fim deve ser estritamente posterior à hora de início.");

        this.Id = id;
        this.Utilizador = utilizador;
        this.Espaco = espaco;
        this.Data = data;
        this.HoraInicio = horaInicio;
        this.HoraFim = horaFim;
        this.Estado = EstadoReserva.Pendente;
        this.DataCriacao = DateTime.Now;
        this.ItensEquipamento = new List<ItemEquipamentoReserva>();
    }

    /// <summary>
    /// Transita o estado de Pendente para Confirmada (bloqueio definitivo).
    /// </summary>
    public void Confirmar()
    {
        if (Estado != EstadoReserva.Pendente)
            throw new EstadoReservaInvalidoException($"Não é possível confirmar uma reserva com estado '{Estado}'. Apenas reservas 'Pendente' podem ser confirmadas.");

        Estado = EstadoReserva.Confirmada;
    }

    /// <summary>
    /// Transita o estado para Cancelada, libertando equipamentos alocados.
    /// </summary>
    public void Cancelar()
    {
        if (Estado != EstadoReserva.Pendente && Estado != EstadoReserva.Confirmada)
            throw new EstadoReservaInvalidoException($"Não é possível cancelar uma reserva no estado '{Estado}'.");

        // Libertar equipamentos alocados
        foreach (var item in ItensEquipamento)
        {
            item.Equipamento.Libertar(item.QuantidadeRequisitada);
        }

        Estado = EstadoReserva.Cancelada;
    }

    /// <summary>
    /// Transita o estado de Confirmada para Concluída após o decurso do horário de jogo.
    /// </summary>
    public void Concluir()
    {
        if (Estado != EstadoReserva.Confirmada)
            throw new EstadoReservaInvalidoException($"Apenas reservas 'Confirmada' podem ser concluídas. Estado atual: '{Estado}'.");

        // Devolução operacional dos equipamentos
        foreach (var item in ItensEquipamento)
        {
            item.Equipamento.Libertar(item.QuantidadeRequisitada);
        }

        Estado = EstadoReserva.Concluida;
    }

    /// <summary>
    /// Invalida reservas pendentes cujo prazo de aprovação expirou.
    /// </summary>
    public void Expirar()
    {
        if (Estado != EstadoReserva.Pendente)
            throw new EstadoReservaInvalidoException($"Apenas reservas 'Pendente' podem expirar. Estado atual: '{Estado}'.");

        foreach (var item in ItensEquipamento)
        {
            item.Equipamento.Libertar(item.QuantidadeRequisitada);
        }

        Estado = EstadoReserva.Expirada;
    }

    /// <summary>
    /// Verifica se este agendamento conflitua (sobreposição temporal) com uma data e intervalo fornecidos.
    /// Reservas Canceladas ou Expiradas não causam sobreposição.
    /// Fórmula de interseção: (ini < horaFim) && (fim > horaInicio)
    /// </summary>
    public bool TemSobreposicao(DateOnly d, TimeOnly ini, TimeOnly fim)
    {
        if (this.Data != d) return false;
        if (this.Estado == EstadoReserva.Cancelada || this.Estado == EstadoReserva.Expirada) return false;

        return (ini < this.HoraFim) && (fim > this.HoraInicio);
    }

    /// <summary>
    /// Associa equipamentos adicionais à reserva após validação e alocação de stock.
    /// </summary>
    public void AdicionarEquipamento(Equipamento eq, int qtd)
    {
        ArgumentNullException.ThrowIfNull(eq);
        if (qtd <= 0) return;

        if (!eq.Alocar(qtd))
            throw new DadosInvalidosException("Equipamento", $"Stock insuficiente do equipamento '{eq.Nome}'. Disponível: {eq.QuantidadeDisponivel}.");

        ItensEquipamento.Add(new ItemEquipamentoReserva(eq, qtd));
    }

    /// <summary>
    /// Duração da reserva em fração de horas (ex: 1h30 = 1.5 horas).
    /// </summary>
    public double DuracaoHoras()
    {
        var duracao = (HoraFim.ToTimeSpan() - HoraInicio.ToTimeSpan()).TotalHours;
        return Math.Max(0.5, duracao);
    }

    /// <summary>
    /// Calcula polimorficamente o custo total da reserva com base nas regras do espaço desportivo.
    /// </summary>
    public double CalcularCustoTotal()
    {
        return Espaco.CalcularCustoReserva(DuracaoHoras());
    }

    public string GetId() => Id;
    public EstadoReserva GetEstado() => Estado;
    public EspacoDesportivo GetEspaco() => Espaco;
    public Utilizador GetUtilizador() => Utilizador;

    public override string ToString()
    {
        return $"Reserva #{Id} | {Data:dd/MM/yyyy} {HoraInicio:HH:mm}-{HoraFim:HH:mm} | {Espaco.Nome} | {Utilizador.Nome} | [{Estado}] | {CalcularCustoTotal():C}";
    }
}
