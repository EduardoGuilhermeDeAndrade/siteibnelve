namespace Ibnelve.Api.Contracts;

public record FotoGaleriaDto(Guid Id, string Url, string? Legenda, int Ordem, DateTimeOffset DataAtualizacao);

public record FotoGaleriaAtualizarRequest(string? Legenda, int Ordem);
