using GestaoEspacosDesportivos.Dominio.Reservas;

namespace GestaoEspacosDesportivos.Dominio.Utilizadores;

/// <summary>
/// Representa um gestor ou funcionário com privilégios administrativos.
/// Herda de Utilizador.
/// </summary>
public class Administrador : Utilizador
{
    public string Departamento { get; set; } = string.Empty;
    public int NivelAcesso { get; set; } = 1;

    public Administrador() { }

    public Administrador(
        string id, 
        string nome, 
        string email, 
        string password, 
        string departamento = "Gestão de Instalações", 
        int nivelAcesso = 1)
        : base(id, nome, email, password, PerfilUtilizador.Administrador)
    {
        this.Departamento = departamento;
        this.NivelAcesso = nivelAcesso;
    }

    /// <summary>
    /// Administradores têm permissão para gerir recintos, aprovar reservas e consultar relatórios.
    /// </summary>
    public override bool PodeGerirInstalacoes() => true;

    /// <summary>
    /// Permite ao administrador aprovar formalmente uma reserva pendente.
    /// </summary>
    public void AprovarReserva(Reserva reserva)
    {
        ArgumentNullException.ThrowIfNull(reserva);
        reserva.Confirmar();
    }
}
