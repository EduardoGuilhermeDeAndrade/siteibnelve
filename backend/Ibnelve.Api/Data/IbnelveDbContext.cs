using Ibnelve.Api.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Data;

public class IbnelveDbContext(DbContextOptions<IbnelveDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Local> Locais => Set<Local>();
    public DbSet<Evento> Eventos => Set<Evento>();
    public DbSet<ImagemSite> ImagensSite => Set<ImagemSite>();
    public DbSet<ConteudoTexto> ConteudosTexto => Set<ConteudoTexto>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Local>(entity =>
        {
            entity.Property(l => l.Nome).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Endereco).HasMaxLength(300).IsRequired();
        });

        builder.Entity<Evento>(entity =>
        {
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.HasOne(e => e.Local)
                  .WithMany()
                  .HasForeignKey(e => e.LocalId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.Visibilidade, e.Status, e.DataHoraInicio });
        });

        builder.Entity<ImagemSite>(entity =>
        {
            entity.Property(i => i.Chave).HasMaxLength(100).IsRequired();
            entity.HasIndex(i => i.Chave).IsUnique();
        });

        builder.Entity<ConteudoTexto>(entity =>
        {
            entity.Property(c => c.Chave).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Titulo).HasMaxLength(200).IsRequired();
            entity.HasIndex(c => c.Chave).IsUnique();
        });

        builder.Entity<RefreshToken>(entity =>
        {
            entity.Property(r => r.HashDoToken).HasMaxLength(200).IsRequired();
            entity.HasIndex(r => r.UsuarioId);
        });
    }
}
