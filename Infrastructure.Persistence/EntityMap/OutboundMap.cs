using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.EntityMap
{
    public class OutboundMap : AuditableBaseEntityMap<Outbound>
    {
        public override void Configure(EntityTypeBuilder<Outbound> builder)
        {
            base.Configure(builder);

            builder.ToTable("Outbounds");

            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.Remark).HasMaxLength(500);
            builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,4)"); ;

            builder.HasOne(e => e.Inventory)
                .WithMany(i => i.Outbounds)
                .HasForeignKey(e => e.InventoryId);

            builder.HasOne(e => e.Order)
                .WithMany()
                .HasForeignKey(e => e.OrderId);

            builder.HasOne(e => e.Inbound)
                .WithMany()
                .HasForeignKey(e => e.InboundId);
        }
    }
}