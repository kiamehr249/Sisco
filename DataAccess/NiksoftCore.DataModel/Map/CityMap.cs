using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class CityMap : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("Cities");

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryId).IsRequired(true);

            builder.HasOne(x => x.Province)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.ProvinceId).IsRequired(false);
        }
    }
}