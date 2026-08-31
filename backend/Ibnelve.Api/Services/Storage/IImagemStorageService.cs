namespace Ibnelve.Api.Services.Storage;

public interface IImagemStorageService
{
    Task GarantirBucketAsync(CancellationToken ct = default);

    /// <summary>Salva o conteúdo no bucket e retorna a URL pública do objeto.</summary>
    Task<string> SalvarAsync(string chave, Stream conteudo, string contentType, string extensao, CancellationToken ct = default);
}
