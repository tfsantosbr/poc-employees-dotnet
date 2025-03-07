using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeeAddressConfiguration : IEntityTypeConfiguration<EmployeeAddress>
    {
        public void Configure(EntityTypeBuilder<EmployeeAddress> builder)
        {
            builder.HasKey(ea => ea.Id);

            builder.Property(ea => ea.Id).ValueGeneratedNever();

            builder.Property(ea => ea.EmployeeId).IsRequired();

            builder.Property(ea => ea.CreatedAt).IsRequired();

            // Address Value Object
            builder.OwnsOne(ea => ea.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasMaxLength(100)
                    .IsRequired();
                
                address.Property(a => a.Number)
                    .HasMaxLength(20)
                    .IsRequired();
                
                address.Property(a => a.Complement)
                    .HasMaxLength(100);
                
                address.Property(a => a.Neighborhood)
                    .HasMaxLength(100)
                    .IsRequired();
                
                address.Property(a => a.City)
                    .HasMaxLength(100)
                    .IsRequired();
                
                address.Property(a => a.State)
                    .HasMaxLength(50)
                    .IsRequired();
                
                address.Property(a => a.ZipCode)
                    .HasMaxLength(20)
                    .IsRequired();
                
                address.Property(a => a.Country)
                    .HasMaxLength(50)
                    .IsRequired();
                
                address.Property(a => a.IsMain)
                    .IsRequired();
            });
        }
    }
}
