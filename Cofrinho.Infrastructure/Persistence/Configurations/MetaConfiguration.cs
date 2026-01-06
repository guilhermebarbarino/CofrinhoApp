using Cofrinho.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cofrinho.Infrastructure.Persistence.Configurations;

public class MetaConfiguration : IEntityTypeConfiguration<Meta>
{
    public void Configure(EntityTypeBuilder<Meta> builder)
    {
        builder.ToTable("metas");

        builder.HasKey(x => x.Nome);

        builder.Property(x => x.Nome)
            .HasColumnName("nome")
            .HasMaxLength(120)
            .IsRequired();

        builder.HasMany(typeof(Transacao), "_transacoes")
            .WithOne()
            .HasForeignKey("meta_nome")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
