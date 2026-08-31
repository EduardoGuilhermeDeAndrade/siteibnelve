namespace Ibnelve.Api.Data.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UsuarioId { get; set; }
    public string HashDoToken { get; set; } = string.Empty;
    public DateTimeOffset ExpiraEm { get; set; }
    public DateTimeOffset CriadoEm { get; set; }
    public DateTimeOffset? RevogadoEm { get; set; }
    public Guid? SubstituidoPorId { get; set; }

    public bool Ativo => RevogadoEm is null && DateTimeOffset.UtcNow < ExpiraEm;
}
