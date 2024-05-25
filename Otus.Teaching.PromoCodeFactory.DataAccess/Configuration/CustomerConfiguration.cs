using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.DataAccess.Data;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.FirstName)
                   .HasMaxLength(32)
                   .IsRequired();
            builder.Property(c => c.LastName)
                   .HasMaxLength(64);
            builder.Property(c => c.Email)
                   .HasMaxLength(256)
                   .IsRequired();

            builder.HasIndex(c => c.Email)
                   .IsUnique();

            builder.HasData(FakeDataFactory.Customers);
        }
    }
}
