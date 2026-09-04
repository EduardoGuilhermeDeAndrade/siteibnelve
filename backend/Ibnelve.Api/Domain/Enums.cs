namespace Ibnelve.Api.Domain;

public enum CategoriaEvento
{
    Culto,
    Ceia,
    EscolaBiblica,
    Jovens,
    Mulheres,
    Homens,
    Obreiros,
    Lideranca,
    Batismo,
    Casamento,
    Reuniao,
    EventoEspecial,
    Administrativo,
    Pessoal
}

public enum VisibilidadeEvento
{
    Publico,
    Interno,
    Pessoal
}

public enum StatusEvento
{
    Rascunho,
    Publicado,
    Cancelado
}

public enum TipoRecorrencia
{
    Nenhuma,
    Semanal,
    ACadaNSemanas,
    MensalPorDia,
    MensalPorPosicao,
    Anual
}
