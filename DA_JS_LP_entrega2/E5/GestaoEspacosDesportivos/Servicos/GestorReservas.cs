using GestaoEspacosDesportivos.Dominio.Espacos;
using GestaoEspacosDesportivos.Dominio.Reservas;
using GestaoEspacosDesportivos.Dominio.Utilizadores;
using GestaoEspacosDesportivos.Excecoes;
using GestaoEspacosDesportivos.Persistencia;

namespace GestaoEspacosDesportivos.Servicos;

/// <summary>
/// Serviço principal de gestão do agendamento, prevenção de conflitos e ciclo de vida de reservas.
/// </summary>
public class GestorReservas
{
    private readonly List<Reserva> reservas;
    private readonly IPersistencia<Reserva> persistencia;

    public GestorReservas(IPersistencia<Reserva> persistencia)
    {
        this.persistencia = persistencia ?? throw new ArgumentNullException(nameof(persistencia));
        this.reservas = persistencia.Carregar();
    }

    /// <summary>
    /// Regra de ouro do sistema: Garante a não sobreposição horária de reservas para o mesmo espaço.
    /// </summary>
    public bool VerificarConflito(string espacoId, DateOnly data, TimeOnly ini, TimeOnly fim)
    {
        return reservas.Any(r => 
            r.Espaco.Id.Equals(espacoId, StringComparison.OrdinalIgnoreCase) && 
            r.TemSobreposicao(data, ini, fim));
    }

    /// <summary>
    /// Efetua o agendamento de um espaço desportivo aplicando todas as validações de integridade e negócio.
    /// </summary>
    public Reserva CriarReserva(
        Utilizador utilizador, 
        EspacoDesportivo espaco, 
        DateOnly data, 
        TimeOnly horaInicio, 
        TimeOnly horaFim,
        Dictionary<string, int>? equipamentosRequisitados = null)
    {
        if (utilizador == null)
            throw new TentativaReservaSemAutenticacaoException();

        ArgumentNullException.ThrowIfNull(espaco);

        if (!espaco.IsDisponivel())
            throw new OperacaoInvalidaException($"O espaço '{espaco.Nome}' encontra-se temporariamente indisponível.");

        if (horaFim <= horaInicio)
            throw new DadosInvalidosException("Horário", "A hora de fim deve ser estritamente posterior à hora de início.");

        // Verificar conflitos de horário
        if (VerificarConflito(espaco.Id, data, horaInicio, horaFim))
        {
            throw new ConflitoHorariosException(espaco.Nome, data, horaInicio, horaFim);
        }

        string novoId = $"RES-{DateTime.Now.Ticks % 1000000:D6}";
        var novaReserva = new Reserva(novoId, utilizador, espaco, data, horaInicio, horaFim);

        // Processar equipamentos opcionais requeridos
        if (equipamentosRequisitados != null && equipamentosRequisitados.Count > 0)
        {
            foreach (var (idEq, qtd) in equipamentosRequisitados)
            {
                var eq = espaco.Equipamentos.FirstOrDefault(e => e.Id.Equals(idEq, StringComparison.OrdinalIgnoreCase));
                if (eq == null)
                    throw new DadosInvalidosException("Equipamento", $"O equipamento '{idEq}' não pertence ao recinto selecionado.");

                novaReserva.AdicionarEquipamento(eq, qtd);
            }
        }

        reservas.Add(novaReserva);
        persistencia.Guardar(reservas);
        return novaReserva;
    }

    /// <summary>
    /// Aprova e confirma formalmente uma reserva pendente, bloqueando definitivamente o recinto.
    /// </summary>
    public void ConfirmarReserva(string reservaId)
    {
        var reserva = ObterPorId(reservaId);
        reserva.Confirmar();
        persistencia.Guardar(reservas);
    }

    /// <summary>
    /// Cancela uma reserva verificando se o utilizador tem autorização (próprio titular ou administrador).
    /// </summary>
    public void CancelarReserva(string reservaId, Utilizador utilizador)
    {
        ArgumentNullException.ThrowIfNull(utilizador);
        var reserva = ObterPorId(reservaId);

        bool isAdmin = utilizador.PodeGerirInstalacoes();
        bool isTitular = reserva.Utilizador.Id.Equals(utilizador.Id, StringComparison.OrdinalIgnoreCase);

        if (!isAdmin && !isTitular)
            throw new OperacaoInvalidaException("Não tem permissões para cancelar uma reserva pertencente a outro utilizador.");

        reserva.Cancelar();
        persistencia.Guardar(reservas);
    }

    /// <summary>
    /// Marca uma reserva confirmada como concluída após a sua utilização.
    /// </summary>
    public void ConcluirReserva(string reservaId)
    {
        var reserva = ObterPorId(reservaId);
        reserva.Concluir();
        persistencia.Guardar(reservas);
    }

    /// <summary>
    /// Processa em lote as reservas pendentes que ultrapassaram o tempo limite de validação.
    /// </summary>
    public int ProcessarReservasExpiradas(TimeSpan prazoLimite)
    {
        var agora = DateTime.Now;
        int expiradas = 0;

        foreach (var r in reservas.Where(r => r.Estado == EstadoReserva.Pendente))
        {
            if (agora - r.DataCriacao > prazoLimite)
            {
                r.Expirar();
                expiradas++;
            }
        }

        if (expiradas > 0)
        {
            persistencia.Guardar(reservas);
        }

        return expiradas;
    }

    public Reserva ObterPorId(string reservaId)
    {
        var reserva = reservas.FirstOrDefault(r => r.Id.Equals(reservaId, StringComparison.OrdinalIgnoreCase));
        if (reserva == null)
            throw new DadosInvalidosException("ReservaId", $"A reserva #{reservaId} não existe.");

        return reserva;
    }

    public List<Reserva> ListarPorUtilizador(string utilizadorId)
    {
        return reservas.Where(r => r.Utilizador.Id.Equals(utilizadorId, StringComparison.OrdinalIgnoreCase))
                       .OrderByDescending(r => r.Data)
                       .ThenBy(r => r.HoraInicio)
                       .ToList();
    }

    public List<Reserva> ListarTodas()
    {
        return reservas.OrderByDescending(r => r.Data)
                       .ThenBy(r => r.HoraInicio)
                       .ToList();
    }
}
