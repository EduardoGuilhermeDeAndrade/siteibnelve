using Ibnelve.Api.Data;

namespace Ibnelve.Api.Services.Auth;

public interface ITokenService
{
    (string Token, DateTimeOffset ExpiraEm) GerarAccessToken(ApplicationUser usuario, IList<string> roles);
    string GerarRefreshTokenBruto();
    string HashRefreshToken(string tokenBruto);
    TimeSpan DuracaoRefreshToken { get; }
}
