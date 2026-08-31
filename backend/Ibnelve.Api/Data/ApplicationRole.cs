using Microsoft.AspNetCore.Identity;

namespace Ibnelve.Api.Data;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole() { }

    public ApplicationRole(string roleName) : base(roleName) { }
}
