using Ibnelve.Api.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Ibnelve.Api.Data;

public class IbnelveDbContext(DbContextOptions<IbnelveDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>(options)
{
    public DbSet<Local> Locais => Set<Local>();
    public DbSet<SerieEvento> SeriesEvento => Set<SerieEvento>();
    public DbSet<ExcecaoEvento> ExcecoesEvento => Set<ExcecaoEvento>();
    public DbSet<Ministerio> Ministerios => Set<Ministerio>();
    public DbSet<ImagemSite> ImagensSite => Set<ImagemSite>();
    public DbSet<ConteudoTexto> ConteudosTexto => Set<ConteudoTexto>();
    public DbSet<ConfiguracaoContribuicao> ConfiguracoesContribuicao => Set<ConfiguracaoContribuicao>();
    public DbSet<PedidoOracao> PedidosOracao => Set<PedidoOracao>();
    public DbSet<FotoGaleria> FotosGaleria => Set<FotoGaleria>();
    public DbSet<ItemPatrimonio> ItensPatrimonio => Set<ItemPatrimonio>();
    public DbSet<EmprestimoPatrimonio> EmprestimosPatrimonio => Set<EmprestimoPatrimonio>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Local>(entity =>
        {
            entity.Property(l => l.Nome).HasMaxLength(200).IsRequired();
            entity.Property(l => l.Endereco).HasMaxLength(300).IsRequired();
        });

        builder.Entity<SerieEvento>(entity =>
        {
            entity.Property(e => e.Titulo).HasMaxLength(200).IsRequired();
            entity.HasOne(e => e.Local)
                  .WithMany()
                  .HasForeignKey(e => e.LocalId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.Visibilidade, e.Status, e.Ativo });
        });

        builder.Entity<ExcecaoEvento>(entity =>
        {
            entity.HasOne(e => e.SerieEvento)
                  .WithMany(s => s.Excecoes)
                  .HasForeignKey(e => e.SerieEventoId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.LocalSubstituto)
                  .WithMany()
                  .HasForeignKey(e => e.LocalSubstitutoId)
                  .OnDelete(DeleteBehavior.SetNull);
            entity.HasIndex(e => new { e.SerieEventoId, e.DataOriginal }).IsUnique();
        });

        builder.Entity<Ministerio>(entity =>
        {
            entity.Property(m => m.Nome).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Lideres).HasMaxLength(200).IsRequired();
        });

        builder.Entity<ImagemSite>(entity =>
        {
            entity.Property(i => i.Chave).HasMaxLength(100).IsRequired();
            entity.Property(i => i.ContentType).HasMaxLength(100).IsRequired();
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

        builder.Entity<ConfiguracaoContribuicao>(entity =>
        {
            entity.Property(c => c.ChavePix).HasMaxLength(200).IsRequired();
            entity.Property(c => c.Favorecido).HasMaxLength(200).IsRequired();
        });

        builder.Entity<PedidoOracao>(entity =>
        {
            entity.Property(p => p.Mensagem).IsRequired();
            entity.HasIndex(p => p.DataCriacao);
        });

        builder.Entity<FotoGaleria>(entity =>
        {
            entity.Property(f => f.ContentType).HasMaxLength(100).IsRequired();
            entity.Property(f => f.Legenda).HasMaxLength(300);
            entity.HasIndex(f => f.Ordem);
        });

        builder.Entity<ItemPatrimonio>(entity =>
        {
            entity.Property(i => i.Descricao).HasMaxLength(300).IsRequired();
            entity.Property(i => i.NumeroPatrimonio).HasMaxLength(100);
            entity.Property(i => i.FotoContentType).HasMaxLength(100);
            entity.Property(i => i.FotoDefeitoContentType).HasMaxLength(100);
            entity.Property(i => i.Observacao).HasMaxLength(1000);
            entity.Property(i => i.ObservacaoBaixa).HasMaxLength(1000);
            entity.HasOne(i => i.Local)
                  .WithMany()
                  .HasForeignKey(i => i.LocalId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<EmprestimoPatrimonio>(entity =>
        {
            entity.Property(e => e.QuemRetirou).HasMaxLength(200).IsRequired();
            entity.Property(e => e.ObservacaoRetirada).HasMaxLength(1000).IsRequired();
            entity.Property(e => e.QuemDevolveu).HasMaxLength(200);
            entity.Property(e => e.ObservacaoDevolucao).HasMaxLength(1000);
            entity.HasOne(e => e.Item)
                  .WithMany(i => i.Emprestimos)
                  .HasForeignKey(e => e.ItemPatrimonioId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => e.ItemPatrimonioId);
        });
    }
}
