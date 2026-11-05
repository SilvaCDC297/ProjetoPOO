using System.Text.Json.Serialization;

namespace GestaoEspacosDesportivos.Dominio.Utilizadores;

/// <summary>
/// Classe abstrata base para todos os utilizadores do sistema.
/// Demonstra abstração, encapsulamento e serve de raiz para polimorfismo de utilizadores.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$tipo")]
[JsonDerivedType(typeof(UtilizadorNormal), "normal")]
[JsonDerivedType(typeof(Administrador), "admin")]
public abstract class Utilizador
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public PerfilUtilizador Perfil { get; set; }

    protected Utilizador() { }

    protected Utilizador(string id, string nome, string email, string password, PerfilUtilizador perfil)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("O identificador do utilizador é obrigatório.", nameof(id));
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("O nome do utilizador é obrigatório.", nameof(nome));
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            throw new ArgumentException("Email inválido.", nameof(email));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password inválida.", nameof(password));

        this.Id = id;
        this.Nome = nome.Trim();
        this.Email = email.Trim().ToLowerInvariant();
        this.PasswordHash = HashPassword(password);
        this.Perfil = perfil;
    }

    /// <summary>
    /// Valida se a password fornecida corresponde à hash guardada.
    /// </summary>
    public virtual bool Autenticar(string password)
    {
        if (string.IsNullOrWhiteSpace(password)) return false;
        return this.PasswordHash == HashPassword(password);
    }

    /// <summary>
    /// Permite alterar a password mediante confirmação da password atual.
    /// </summary>
    public virtual void AlterarPassword(string atual, string nova)
    {
        if (!Autenticar(atual))
            throw new InvalidOperationException("A password atual está incorreta.");
        if (string.IsNullOrWhiteSpace(nova) || nova.Length < 6)
            throw new ArgumentException("A nova password deve conter pelo menos 6 caracteres.", nameof(nova));

        this.PasswordHash = HashPassword(nova);
    }

    /// <summary>
    /// Método abstrato que define polimorficamente se o utilizador pode gerir instalações.
    /// </summary>
    public abstract bool PodeGerirInstalacoes();

    public string GetId() => Id;
    public string GetNome() => Nome;
    public string GetEmail() => Email;
    public PerfilUtilizador GetPerfil() => Perfil;

    /// <summary>
    /// Algoritmo simples de hash SHA256 para não armazenar passwords em texto limpo.
    /// </summary>
    public static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    public override string ToString()
    {
        return $"[{Perfil}] {Nome} ({Email}) - ID: {Id}";
    }
}
