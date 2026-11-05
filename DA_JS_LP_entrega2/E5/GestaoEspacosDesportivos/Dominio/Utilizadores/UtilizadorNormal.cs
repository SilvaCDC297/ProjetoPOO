namespace GestaoEspacosDesportivos.Dominio.Utilizadores;

/// <summary>
/// Representa um cliente/atleta normal que pode reservar instalações e consultar o histórico.
/// Herda de Utilizador (Herança e Polimorfismo).
/// </summary>
public class UtilizadorNormal : Utilizador
{
    public string NumeroSocio { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;

    public UtilizadorNormal() { }

    public UtilizadorNormal(
        string id, 
        string nome, 
        string email, 
        string password, 
        string numeroSocio = "", 
        string telefone = "")
        : base(id, nome, email, password, PerfilUtilizador.UtilizadorNormal)
    {
        this.NumeroSocio = numeroSocio;
        this.Telefone = telefone;
    }

    public string GetNumeroSocio() => NumeroSocio;
    public string GetTelefone() => Telefone;

    public void AtualizarContacto(string telefone)
    {
        this.Telefone = telefone;
    }

    /// <summary>
    /// Utilizadores normais não possuem permissões de gestão do recinto.
    /// </summary>
    public override bool PodeGerirInstalacoes() => false;
}
