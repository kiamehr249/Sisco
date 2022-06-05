using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class GeneralContentMap : IEntityTypeConfiguration<GeneralContent>
    {
        public void Configure(EntityTypeBuilder<GeneralContent> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("GeneralContents");

            builder.HasOne(x => x.ContentCategory)
                .WithMany(x => x.Contents)
                .HasForeignKey(x => x.CategoryId).IsRequired(true);
        }
    }
}