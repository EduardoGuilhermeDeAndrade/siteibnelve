namespace Ibnelve.Api.Domain;

/// <summary>
/// Converte os enums do domínio para as mesmas strings que o Angular já usa em
/// frontend/src/app/shared/data/eventos.mock.ts (CategoriaEvento/VisibilidadeEvento/StatusEvento),
/// para a Fase 4 poder trocar o mock pela API sem mudar os valores.
/// </summary>
public static class EnumMappings
{
    public static string ToApiString(this CategoriaEvento categoria) => categoria switch
    {
        CategoriaEvento.Culto => "CULTO",
        CategoriaEvento.Ceia => "CEIA",
        CategoriaEvento.EscolaBiblica => "ESCOLA_BIBLICA",
        CategoriaEvento.Jovens => "JOVENS",
        CategoriaEvento.Mulheres => "MULHERES",
        CategoriaEvento.Homens => "HOMENS",
        CategoriaEvento.Obreiros => "OBREIROS",
        CategoriaEvento.Lideranca => "LIDERANCA",
        CategoriaEvento.Batismo => "BATISMO",
        CategoriaEvento.Casamento => "CASAMENTO",
        CategoriaEvento.Reuniao => "REUNIAO",
        CategoriaEvento.EventoEspecial => "EVENTO_ESPECIAL",
        CategoriaEvento.Administrativo => "ADMINISTRATIVO",
        CategoriaEvento.Pessoal => "PESSOAL",
        _ => throw new ArgumentOutOfRangeException(nameof(categoria))
    };

    public static CategoriaEvento ParseCategoria(string valor) => valor switch
    {
        "CULTO" => CategoriaEvento.Culto,
        "CEIA" => CategoriaEvento.Ceia,
        "ESCOLA_BIBLICA" => CategoriaEvento.EscolaBiblica,
        "JOVENS" => CategoriaEvento.Jovens,
        "MULHERES" => CategoriaEvento.Mulheres,
        "HOMENS" => CategoriaEvento.Homens,
        "OBREIROS" => CategoriaEvento.Obreiros,
        "LIDERANCA" => CategoriaEvento.Lideranca,
        "BATISMO" => CategoriaEvento.Batismo,
        "CASAMENTO" => CategoriaEvento.Casamento,
        "REUNIAO" => CategoriaEvento.Reuniao,
        "EVENTO_ESPECIAL" => CategoriaEvento.EventoEspecial,
        "ADMINISTRATIVO" => CategoriaEvento.Administrativo,
        "PESSOAL" => CategoriaEvento.Pessoal,
        _ => throw new ArgumentException($"Categoria inválida: {valor}")
    };

    public static string ToApiString(this VisibilidadeEvento visibilidade) => visibilidade switch
    {
        VisibilidadeEvento.Publico => "PUBLICO",
        VisibilidadeEvento.Interno => "INTERNO",
        VisibilidadeEvento.Pessoal => "PESSOAL",
        _ => throw new ArgumentOutOfRangeException(nameof(visibilidade))
    };

    public static VisibilidadeEvento ParseVisibilidade(string valor) => valor switch
    {
        "PUBLICO" => VisibilidadeEvento.Publico,
        "INTERNO" => VisibilidadeEvento.Interno,
        "PESSOAL" => VisibilidadeEvento.Pessoal,
        _ => throw new ArgumentException($"Visibilidade inválida: {valor}")
    };

    public static string ToApiString(this StatusEvento status) => status switch
    {
        StatusEvento.Rascunho => "RASCUNHO",
        StatusEvento.Publicado => "PUBLICADO",
        StatusEvento.Cancelado => "CANCELADO",
        _ => throw new ArgumentOutOfRangeException(nameof(status))
    };

    public static StatusEvento ParseStatus(string valor) => valor switch
    {
        "RASCUNHO" => StatusEvento.Rascunho,
        "PUBLICADO" => StatusEvento.Publicado,
        "CANCELADO" => StatusEvento.Cancelado,
        _ => throw new ArgumentException($"Status inválido: {valor}")
    };
}
