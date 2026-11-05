using GestaoEspacosDesportivos.Dominio.Espacos;
using GestaoEspacosDesportivos.Excecoes;
using GestaoEspacosDesportivos.Persistencia;

namespace GestaoEspacosDesportivos.Servicos;

/// <summary>
/// Serviço responsável pela gestão de catálogo, CRUD e inventário dos recintos desportivos.
/// </summary>
public class GestorEspacos
{
    private readonly List<EspacoDesportivo> espacos;
    private readonly IPersistencia<EspacoDesportivo> persistencia;

    public GestorEspacos(IPersistencia<EspacoDesportivo> persistencia)
    {
        this.persistencia = persistencia ?? throw new ArgumentNullException(nameof(persistencia));
        this.espacos = persistencia.Carregar();
    }

    public void AdicionarEspaco(EspacoDesportivo espaco)
    {
        ArgumentNullException.ThrowIfNull(espaco);

        if (espacos.Any(e => e.Id.Equals(espaco.Id, StringComparison.OrdinalIgnoreCase)))
            throw new DadosInvalidosException("Id", $"Já existe um espaço com o ID '{espaco.Id}'.");

        espacos.Add(espaco);
        persistencia.Guardar(espacos);
    }

    public void AtualizarEspaco(EspacoDesportivo espacoAtualizado)
    {
        ArgumentNullException.ThrowIfNull(espacoAtualizado);

        var index = espacos.FindIndex(e => e.Id.Equals(espacoAtualizado.Id, StringComparison.OrdinalIgnoreCase));
        if (index == -1)
            throw new EspacoInexistenteException(espacoAtualizado.Id);

        espacos[index] = espacoAtualizado;
        persistencia.Guardar(espacos);
    }

    public void RemoverEspaco(string id)
    {
        var espaco = ObterPorId(id);
        espacos.Remove(espaco);
        persistencia.Guardar(espacos);
    }

    public EspacoDesportivo ObterPorId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new DadosInvalidosException("Id", "O identificador do espaço é obrigatório.");

        var espaco = espacos.FirstOrDefault(e => e.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (espaco == null)
            throw new EspacoInexistenteException(id);

        return espaco;
    }

    public List<EspacoDesportivo> ListarEspacos(TipoEspaco? tipo = null, EstadoEspaco? estado = null)
    {
        IEnumerable<EspacoDesportivo> query = espacos;

        if (tipo.HasValue)
            query = query.Where(e => e.Tipo == tipo.Value);

        if (estado.HasValue)
            query = query.Where(e => e.Estado == estado.Value);

        return query.ToList();
    }

    public List<EspacoDesportivo> ListarTodos() => new(espacos);
}
