using ContractManager.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ContractManager.Data.EF.EntityConfiguration;
public class ContractEntityConfiguration : IEntityTypeConfiguration<Contract>
{
    public void Configure(EntityTypeBuilder<Contract> builder)
    {
        //Properties
        builder.Property(t => t.ClientId)
            .IsRequired();

        builder.HasIndex(t => t.ClientId);

        builder.Property(c => c.ContractTypeId)
            .IsRequired();

        builder.Property(c => c.TrackingId)
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(t => t.TrackingId)
            .IsUnique();

        builder.Property(c => c.Value)
            .IsRequired();

        builder.Property(c => c.DigitalFileUrl)
            .HasMaxLength(1250);

        //Relationships
        builder.HasOne(c => c.ContractType)
            .WithMany()
            .HasForeignKey(c => c.ContractTypeId);
    }
}
