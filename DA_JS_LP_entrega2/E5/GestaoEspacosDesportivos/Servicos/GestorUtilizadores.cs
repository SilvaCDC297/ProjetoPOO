using GestaoEspacosDesportivos.Dominio.Utilizadores;
using GestaoEspacosDesportivos.Excecoes;
using GestaoEspacosDesportivos.Persistencia;

namespace GestaoEspacosDesportivos.Servicos;

/// <summary>
/// Serviço responsável pelo ciclo de vida, registo, autenticação e listagem de utilizadores.
/// </summary>
public class GestorUtilizadores
{
    private readonly List<Utilizador> utilizadores;
    private readonly IPersistencia<Utilizador> persistencia;

    public GestorUtilizadores(IPersistencia<Utilizador> persistencia)
    {
        this.persistencia = persistencia ?? throw new ArgumentNullException(nameof(persistencia));
        this.utilizadores = persistencia.Carregar();
    }

    /// <summary>
    /// Regista um novo utilizador no sistema, prevenindo duplicação de e-mail.
    /// </summary>
    public Utilizador Registar(
        string nome, 
        string email, 
        string password, 
        PerfilUtilizador perfil = PerfilUtilizador.UtilizadorNormal,
        string extraParam1 = "", // numeroSocio ou departamento
        string extraParam2 = "") // telefone
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new DadosInvalidosException("Email", "O endereço de email não pode estar vazio.");
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            throw new DadosInvalidosException("Password", "A password deve ter no mínimo 6 caracteres.");

        email = email.Trim().ToLowerInvariant();
        if (utilizadores.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DadosInvalidosException("Email", $"Já existe um utilizador registado com o email '{email}'.");
        }

        string novoId = $"USR-{DateTime.Now.Ticks % 100000:D5}";

        Utilizador novoUtilizador = perfil switch
        {
            PerfilUtilizador.Administrador => new Administrador(novoId, nome, email, password, string.IsNullOrWhiteSpace(extraParam1) ? "Gestão Desportiva" : extraParam1),
            _ => new UtilizadorNormal(novoId, nome, email, password, extraParam1, extraParam2)
        };

        utilizadores.Add(novoUtilizador);
        persistencia.Guardar(utilizadores);
        return novoUtilizador;
    }

    /// <summary>
    /// Autentica um utilizador com validação segura de credenciais.
    /// Lança LoginInvalidoException em caso de falha.
    /// </summary>
    public Utilizador Autenticar(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            throw new LoginInvalidoException("Email e password devem ser preenchidos.");

        email = email.Trim().ToLowerInvariant();
        var user = utilizadores.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));

        if (user == null || !user.Autenticar(password))
        {
            throw new LoginInvalidoException("Credenciais inválidas. Verifique o email e password.");
        }

        return user;
    }

    public Utilizador? ObterPorId(string id)
    {
        return utilizadores.FirstOrDefault(u => u.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    }

    public Utilizador? ObterPorEmail(string email)
    {
        return utilizadores.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    public List<Utilizador> ListarTodos() => new(utilizadores);
}
