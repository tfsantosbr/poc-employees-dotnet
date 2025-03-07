using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id).ValueGeneratedNever();

            // PersonName Value Object
            builder.OwnsOne(e => e.Name, name =>
            {
                name.Property(n => n.FirstName)
                    .HasColumnName("FirstName")
                    .HasMaxLength(50)
                    .IsRequired();

                name.Property(n => n.LastName)
                    .HasColumnName("LastName")
                    .HasMaxLength(50)
                    .IsRequired();
            });

            // Email Value Object
            builder.OwnsOne(e => e.Email, email =>
            {
                email.Property(e => e.Value)
                    .HasColumnName("Email")
                    .HasMaxLength(254)
                    .IsRequired();

                email.HasIndex(e => e.Value).IsUnique();
            });

            // Document Value Object
            builder.OwnsOne(e => e.Document, doc =>
            {
                doc.Property(d => d.Value)
                    .HasColumnName("Document")
                    .HasMaxLength(14)
                    .IsRequired();

                doc.Property(d => d.Type)
                    .HasColumnName("DocumentType")
                    .HasConversion<string>()
                    .HasMaxLength(4)
                    .IsRequired();

                doc.HasIndex(d => d.Value).IsUnique();
            });

            // Money Value Object
            builder.OwnsOne(e => e.Salary, money =>
            {
                money.Property(m => m.Amount)
                    .HasColumnName("Salary")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();

                money.Property(m => m.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });

            builder.Property(e => e.Position)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(e => e.BirthDate)
                .IsRequired();

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt)
                .IsRequired();

            builder.Property(e => e.IsActive)
                .IsRequired();

            // EmployeeAddress Entity Collection
            builder.HasMany(e => e.Addresses)
                .WithOne()
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
