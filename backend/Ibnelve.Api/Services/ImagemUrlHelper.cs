namespace Ibnelve.Api.Services;

/// <summary>
/// Monta a URL pública de uma imagem servida pelo próprio Postgres. Não fica salva no banco —
/// é sempre computada a partir do host da requisição, com cache-busting baseado em
/// DataAtualizacao (a URL muda sozinha quando a imagem é substituída).
/// </summary>
public static class ImagemUrlHelper
{
    public static string Construir(HttpRequest request, string chave, DateTimeOffset dataAtualizacao) =>
        $"{request.Scheme}://{request.Host}/api/imagens-site/{chave}/arquivo?v={dataAtualizacao.UtcTicks}";
}
