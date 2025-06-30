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
    public class MaterialUsedMap : AuditableBaseEntityMap<MaterialUsed>
    {
        public override void Configure(EntityTypeBuilder<MaterialUsed> builder)
        {
            base.Configure(builder);

            builder.ToTable("MaterialsUsed");

            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,4)");

            builder.HasOne(e => e.Inventory)
                .WithMany()
                .HasForeignKey(e => e.InventoryId);

            builder.HasOne(e => e.Order)
                .WithMany(o => o.MaterialsUsed)
                .HasForeignKey(e => e.OrderId);
        }
    }
}