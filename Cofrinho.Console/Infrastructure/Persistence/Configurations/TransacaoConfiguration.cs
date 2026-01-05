using Cofrinho.Console.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofrinho.Console.Infrastructure.Persistence.Configurations;

public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
{
    public void Configure(EntityTypeBuilder<Transacao> builder)
    {
        builder.ToTable("transacoes");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasColumnName("id");

        builder.Property(x => x.Data)
            .HasColumnName("data")
            .IsRequired();

        builder.Property(x => x.Valor)
            .HasColumnName("valor")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(200);

        builder.Property(x => x.Tipo)
            .HasColumnName("tipo")
            .IsRequired();

        // FK shadow para ligar com Meta (mantemos assim por enquanto)
        builder.Property<string>("meta_nome")
            .HasColumnName("meta_nome")
            .IsRequired();
    }
}