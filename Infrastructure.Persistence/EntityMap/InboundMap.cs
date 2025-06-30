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
    public class InboundMap : AuditableBaseEntityMap<Inbound>
    {
        public override void Configure(EntityTypeBuilder<Inbound> builder)
        {
            base.Configure(builder);

            builder.ToTable("Inbounds");

            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.RemainingQuantity).IsRequired();
            builder.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.Remark).HasMaxLength(500);

            builder.HasOne(e => e.Inventory)
                .WithMany(i => i.Inbounds)
                .HasForeignKey(e => e.InventoryId);

            builder.HasOne(e => e.Acquisition)
                .WithMany()
                .HasForeignKey(e => e.AcquisitionId);
        }
    }
}