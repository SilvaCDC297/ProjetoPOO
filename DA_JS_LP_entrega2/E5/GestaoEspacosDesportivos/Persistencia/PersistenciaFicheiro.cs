using System.Text.Json;
using GestaoEspacosDesportivos.Excecoes;

namespace GestaoEspacosDesportivos.Persistencia;

/// <summary>
/// Implementação de persistência atómica em ficheiro formato JSON.
/// Garante que se o ficheiro não existir é criado e trata falhas de escrita/leitura.
/// </summary>
/// <typeparam name="T">Tipo de entidade.</typeparam>
public class PersistenciaFicheiro<T> : IPersistencia<T>
{
    private readonly string caminhoFicheiro;
    private readonly JsonSerializerOptions jsonOptions;

    public string CaminhoFicheiro => caminhoFicheiro;

    public PersistenciaFicheiro(string caminhoFicheiro)
    {
        if (string.IsNullOrWhiteSpace(caminhoFicheiro))
            throw new ArgumentException("Caminho de ficheiro inválido.", nameof(caminhoFicheiro));

        this.caminhoFicheiro = caminhoFicheiro;
        
        this.jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        GarantirDiretorioExistente();
    }

    private void GarantirDiretorioExistente()
    {
        var dir = Path.GetDirectoryName(caminhoFicheiro);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
    }

    public List<T> Carregar()
    {
        try
        {
            if (!File.Exists(caminhoFicheiro))
            {
                return new List<T>();
            }

            var json = File.ReadAllText(caminhoFicheiro);
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<T>();
            }

            var dados = JsonSerializer.Deserialize<List<T>>(json, jsonOptions);
            return dados ?? new List<T>();
        }
        catch (Exception ex)
        {
            throw new PersistenciaException(caminhoFicheiro, $"Falha ao carregar dados do ficheiro: {ex.Message}");
        }
    }

    public void Guardar(List<T> dados)
    {
        try
        {
            GarantirDiretorioExistente();
            var json = JsonSerializer.Serialize(dados, jsonOptions);

            // Escrita atómica através de ficheiro temporário
            var tempFile = caminhoFicheiro + ".tmp";
            File.WriteAllText(tempFile, json);
            File.Move(tempFile, caminhoFicheiro, overwrite: true);
        }
        catch (Exception ex)
        {
            throw new PersistenciaException(caminhoFicheiro, $"Falha ao gravar dados no ficheiro: {ex.Message}");
        }
    }

    public bool ValidarIntegridade()
    {
        if (!File.Exists(caminhoFicheiro)) return true;
        try
        {
            var info = new FileInfo(caminhoFicheiro);
            return info.Length > 0;
        }
        catch
        {
            return false;
        }
    }
}
