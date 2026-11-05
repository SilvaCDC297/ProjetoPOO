namespace GestaoEspacosDesportivos.Excecoes;

/// <summary>
/// Exceção base de domínio para a aplicação de reservas desportivas.
/// </summary>
public class SistemaReservasException : Exception
{
    public SistemaReservasException(string mensagem) : base(mensagem) { }
    public SistemaReservasException(string mensagem, Exception inner) : base(mensagem, inner) { }
}

/// <summary>
/// Lançada quando existe colisão temporal de horários no mesmo recinto desportivo.
/// </summary>
public class ConflitoHorariosException : SistemaReservasException
{
    public string EspacoId { get; }
    public DateOnly DataReserva { get; }
    public TimeOnly HoraInicio { get; }
    public TimeOnly HoraFim { get; }

    public ConflitoHorariosException(string espacoId, DateOnly data, TimeOnly ini, TimeOnly fim)
        : base($"Conflito de agendamento: O recinto '{espacoId}' já possui reserva ativa no dia {data:dd/MM/yyyy} entre as {ini:HH:mm} e {fim:HH:mm}.")
    {
        EspacoId = espacoId;
        DataReserva = data;
        HoraInicio = ini;
        HoraFim = fim;
    }
}

/// <summary>
/// Lançada quando as credenciais de login são inválidas ou o utilizador não existe.
/// </summary>
public class LoginInvalidoException : SistemaReservasException
{
    public LoginInvalidoException(string mensagem = "Email ou password incorretos.") : base(mensagem) { }
}

/// <summary>
/// Lançada quando se tenta aceder ou reservar uma instalação inexistente ou eliminada.
/// </summary>
public class EspacoInexistenteException : SistemaReservasException
{
    public string EspacoId { get; }

    public EspacoInexistenteException(string id)
        : base($"O espaço desportivo com identificador '{id}' não foi encontrado no sistema.")
    {
        EspacoId = id;
    }
}

/// <summary>
/// Lançada quando parâmetros de negócio ou dados de formulário violam invariantes.
/// </summary>
public class DadosInvalidosException : SistemaReservasException
{
    public string Campo { get; }

    public DadosInvalidosException(string campo, string motivo)
        : base($"Dado inválido no campo '{campo}': {motivo}")
    {
        Campo = campo;
    }
}

/// <summary>
/// Lançada quando um utilizador anónimo/não autenticado tenta submeter reservas.
/// </summary>
public class TentativaReservaSemAutenticacaoException : SistemaReservasException
{
    public TentativaReservaSemAutenticacaoException(string mensagem = "É obrigatório iniciar sessão para poder reservar um espaço desportivo.")
        : base(mensagem) { }
}

/// <summary>
/// Lançada em caso de erro de leitura/escrita de ficheiros de persistência.
/// </summary>
public class PersistenciaException : SistemaReservasException
{
    public string Ficheiro { get; }

    public PersistenciaException(string ficheiro, string detalhe)
        : base($"Erro de persistência no ficheiro '{ficheiro}': {detalhe}")
    {
        Ficheiro = ficheiro;
    }
}

/// <summary>
/// Lançada em tentativas de transições ilegais na máquina de estados da reserva.
/// </summary>
public class EstadoReservaInvalidoException : SistemaReservasException
{
    public EstadoReservaInvalidoException(string mensagem) : base(mensagem) { }
}

/// <summary>
/// Lançada em operações que violam o estado do sistema (ex: apagar recinto com reservas ativas).
/// </summary>
public class OperacaoInvalidaException : SistemaReservasException
{
    public OperacaoInvalidaException(string mensagem) : base(mensagem) { }
}
