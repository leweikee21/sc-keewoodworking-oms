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
    public class AcquisitionItemMap : AuditableBaseEntityMap<AcquisitionItem>
    {
        public override void Configure(EntityTypeBuilder<AcquisitionItem> builder)
        {
            base.Configure(builder);

            builder.ToTable("AcquisitionItems");

            builder.Property(e => e.Quantity).IsRequired();

            builder.HasOne(e => e.Acquisition)
                .WithMany(i => i.Items)
                .HasForeignKey(e => e.AcquisitionId);

            builder.HasOne(e => e.Inventory)
                .WithMany()
                .HasForeignKey(e => e.InventoryId);
        }
    }
}