﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NiksoftCore.DataModel
{
    public class UserTokenMap : IEntityTypeConfiguration<UserToken>
    {
        public void Configure(EntityTypeBuilder<UserToken> builder)
        {
            builder.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });
            builder.Property(t => t.LoginProvider).HasMaxLength(128);
            builder.Property(t => t.Name).HasMaxLength(128);
            builder.ToTable("UserTokens");
        }
    }
}
