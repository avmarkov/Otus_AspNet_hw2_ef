using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Otus.Teaching.PromoCodeFactory.Core.Domain.PromoCodeManagement;
using Otus.Teaching.PromoCodeFactory.DataAccess.Data;

namespace Otus.Teaching.PromoCodeFactory.DataAccess.Configuration
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasOne(p => p.Preference)
                   .WithMany(pr => pr.PromoCodes)
                   .HasForeignKey(e => e.PreferenceId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade); ;

            builder.HasOne(p => p.Customer)
                   .WithMany(c => c.PromoCodes)
                   .HasForeignKey(p => p.CustomerId)
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Code)
                   .HasMaxLength(32)
                   .IsRequired();
            builder.Property(p => p.ServiceInfo)
                   .HasMaxLength(128);
            builder.Property(p => p.PartnerName)
                   .HasMaxLength(128);

            builder.HasIndex(p => p.Code)
                   .IsUnique();

            builder.HasData(FakeDataFactory.PromoCodes);
        }
    }
}
