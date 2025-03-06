using Domain.Entities;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

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

            // Addresses Value Objects Collection
            builder.OwnsMany(e => e.Addresses, address =>
            {
                address.WithOwner().HasForeignKey("EmployeeId");
                
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
