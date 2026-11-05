namespace GestaoEspacosDesportivos.Persistencia;

/// <summary>
/// Contrato genérico de persistência de dados.
/// Permite desacoplar a lógica de negócio dos detalhes de armazenamento físico (DIP - Dependency Inversion Principle).
/// </summary>
/// <typeparam name="T">Tipo de entidade a persistir.</typeparam>
public interface IPersistencia<T>
{
    List<T> Carregar();
    void Guardar(List<T> dados);
    bool ValidarIntegridade();
}
