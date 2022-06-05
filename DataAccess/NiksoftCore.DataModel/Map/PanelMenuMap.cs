using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class PanelMenuMap : IEntityTypeConfiguration<PanelMenu>
    {
        public void Configure(EntityTypeBuilder<PanelMenu> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("PanelMenus");

            builder.HasOne(x => x.Parent)
                .WithMany(x => x.Childs)
                .HasForeignKey(x => x.ParentId).IsRequired(false);
        }
    }
}