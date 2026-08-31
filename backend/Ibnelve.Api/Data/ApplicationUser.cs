using Microsoft.AspNetCore.Identity;

namespace Ibnelve.Api.Data;

public class ApplicationUser : IdentityUser<Guid>
{
    public string NomeCompleto { get; set; } = string.Empty;
}
