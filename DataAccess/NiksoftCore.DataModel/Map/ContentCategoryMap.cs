using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class ContentCategoryMap : IEntityTypeConfiguration<ContentCategory>
    {
        public void Configure(EntityTypeBuilder<ContentCategory> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("ContentCategories");

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Childs)
                .HasForeignKey(x => x.ParentId).IsRequired(false);
        }
    }
}