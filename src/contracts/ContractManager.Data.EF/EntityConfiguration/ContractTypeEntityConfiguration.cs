using ContractManager.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractManager.Data.EF.EntityConfiguration;
public class ContractTypeEntityConfiguration : IEntityTypeConfiguration<ContractType>
{
    public void Configure(EntityTypeBuilder<ContractType> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .ValueGeneratedNever(); // porque lo manejas tú como enum

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);
    }
}
