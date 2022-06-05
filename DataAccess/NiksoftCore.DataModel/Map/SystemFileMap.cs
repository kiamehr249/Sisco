using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class SystemFileMap : IEntityTypeConfiguration<SystemFile>
    {
        public void Configure(EntityTypeBuilder<SystemFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("SystemFiles");
        }
    }
}