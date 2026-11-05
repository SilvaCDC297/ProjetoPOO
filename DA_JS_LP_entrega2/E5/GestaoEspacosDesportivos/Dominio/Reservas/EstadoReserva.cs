namespace GestaoEspacosDesportivos.Dominio.Reservas;

/// <summary>
/// Máquina de estados formal de uma reserva segundo a especificação do projeto.
/// </summary>
public enum EstadoReserva
{
    Pendente,
    Confirmada,
    Cancelada,
    Concluida,
    Expirada
}
