using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Ibnelve.Api.Data;
using Microsoft.IdentityModel.Tokens;

namespace Ibnelve.Api.Services.Auth;

public class JwtTokenService(IConfiguration configuration) : ITokenService
{
    private readonly string _key = configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("Configuração ausente: Jwt:Key");
    private readonly string _issuer = configuration["Jwt:Issuer"] ?? "Ibnelve.Api";
    private readonly string _audience = configuration["Jwt:Audience"] ?? "Ibnelve.Admin";
    private readonly int _minutosAccessToken = configuration.GetValue("Jwt:AccessTokenMinutes", 20);
    private readonly int _diasRefreshToken = configuration.GetValue("Jwt:RefreshTokenDays", 14);

    public TimeSpan DuracaoRefreshToken => TimeSpan.FromDays(_diasRefreshToken);

    public (string Token, DateTimeOffset ExpiraEm) GerarAccessToken(ApplicationUser usuario, IList<string> roles)
    {
        var expiraEm = DateTimeOffset.UtcNow.AddMinutes(_minutosAccessToken);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email ?? string.Empty),
            new(ClaimTypes.Name, usuario.NomeCompleto),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var credenciais = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: expiraEm.UtcDateTime,
            signingCredentials: credenciais);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiraEm);
    }

    public string GerarRefreshTokenBruto() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public string HashRefreshToken(string tokenBruto)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(tokenBruto));
        return Convert.ToHexString(bytes);
    }
}
