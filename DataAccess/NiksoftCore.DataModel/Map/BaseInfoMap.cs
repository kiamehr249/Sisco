using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class BaseInfoMap : IEntityTypeConfiguration<BaseInfo>
    {
        public void Configure(EntityTypeBuilder<BaseInfo> builder)
        {
            builder.HasKey(x => x.Id);
            builder.ToTable("BaseInfos");

            builder.HasOne(x => x.User)
                .WithMany(x => x.BaseInfos)
                .HasForeignKey(x => x.UserId);

        }
    }
}