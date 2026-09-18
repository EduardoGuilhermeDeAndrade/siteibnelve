namespace Ibnelve.Api.Services;

/// <summary>
/// Validação de upload de imagem compartilhada entre os controllers que recebem arquivo
/// (Conteúdo do Site e Painel de Fotos) — tipo aceito, tamanho e assinatura binária (magic
/// bytes), já que o Content-Type enviado pelo cliente é só um header HTTP e pode ser forjado.
/// </summary>
public static class ValidacaoImagem
{
    public static readonly HashSet<string> TiposPermitidos = ["image/jpeg", "image/png", "image/webp"];

    public const long TamanhoMaximoBytes = 5 * 1024 * 1024; // 5 MB

    /// <summary>Retorna uma mensagem de erro em português se o arquivo for inválido, ou null se estiver ok.</summary>
    public static async Task<string?> ValidarAsync(IFormFile? arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
        {
            return "Envie um arquivo de imagem.";
        }

        if (arquivo.Length > TamanhoMaximoBytes)
        {
            return "Arquivo maior que o limite de 5 MB.";
        }

        if (!TiposPermitidos.Contains(arquivo.ContentType))
        {
            return "Formato não suportado. Envie JPEG, PNG ou WebP.";
        }

        if (!await TemAssinaturaValidaAsync(arquivo, arquivo.ContentType))
        {
            return "O conteúdo do arquivo não corresponde ao formato informado.";
        }

        return null;
    }

    private static async Task<bool> TemAssinaturaValidaAsync(IFormFile arquivo, string contentType)
    {
        var cabecalho = new byte[12];
        await using (var stream = arquivo.OpenReadStream())
        {
            var lidos = await stream.ReadAsync(cabecalho.AsMemory(0, (int)Math.Min(cabecalho.Length, arquivo.Length)));
            if (lidos < cabecalho.Length)
            {
                Array.Clear(cabecalho, lidos, cabecalho.Length - lidos);
            }
        }

        return contentType switch
        {
            "image/jpeg" => cabecalho[0] == 0xFF && cabecalho[1] == 0xD8 && cabecalho[2] == 0xFF,
            "image/png" => cabecalho[0] == 0x89 && cabecalho[1] == 0x50 && cabecalho[2] == 0x4E && cabecalho[3] == 0x47
                            && cabecalho[4] == 0x0D && cabecalho[5] == 0x0A && cabecalho[6] == 0x1A && cabecalho[7] == 0x0A,
            "image/webp" => cabecalho[0] == 0x52 && cabecalho[1] == 0x49 && cabecalho[2] == 0x46 && cabecalho[3] == 0x46
                             && cabecalho[8] == 0x57 && cabecalho[9] == 0x45 && cabecalho[10] == 0x42 && cabecalho[11] == 0x50,
            _ => false
        };
    }
}
