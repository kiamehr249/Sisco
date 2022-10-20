using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class SiscoRecordMap : IEntityTypeConfiguration<SiscoRecord>
    {
        public void Configure(EntityTypeBuilder<SiscoRecord> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("SiscoRecords");
        }
    }
}