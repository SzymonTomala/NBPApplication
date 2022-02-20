using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Core.DataAccessLayer
{
    public class Configurations : IEntityTypeConfiguration<CurrencyRate>
    {
        public void Configure(EntityTypeBuilder<CurrencyRate> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(3);

            builder.Property(x => x.Value)
                .IsRequired();

            builder.Property(x => x.Date)
                .IsRequired();
        }
    }
}
