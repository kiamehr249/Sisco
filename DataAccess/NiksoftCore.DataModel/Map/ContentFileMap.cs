using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class ContentFileMap : IEntityTypeConfiguration<ContentFile>
    {
        public void Configure(EntityTypeBuilder<ContentFile> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("ContentFiles");

            builder.HasOne(x => x.Content)
                .WithMany(x => x.ContentFiles)
                .HasForeignKey(x => x.ContentId).IsRequired(true);
        }
    }
}