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
    public class InventoryMap : AuditableBaseEntityMap<Inventory>
    {
        public override void Configure(EntityTypeBuilder<Inventory> builder)
        {
            base.Configure(builder);

            builder.ToTable("Inventories");

            builder.Property(e => e.Code).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
            builder.Property(e => e.Category).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Unit).IsRequired().HasMaxLength(50);
            builder.Property(e => e.TotalQty).IsRequired();
            builder.Property(e => e.ReservedQty).IsRequired();
            builder.Property(e => e.AvailableQty).IsRequired();
            builder.Property(e => e.MinQty).IsRequired();
            builder.Property(e => e.LastUnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.AverageUnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.LastInDate).HasColumnType("timestamp with time zone");
            builder.Property(e => e.LastOutDate).HasColumnType("timestamp with time zone");
            builder.Property(e => e.LastOutDate).HasColumnType("timestamp with time zone");
            builder.Property(e => e.IsDeleted).IsRequired().HasColumnType("bool").HasDefaultValue(false);

            builder.HasOne(e => e.Supplier)
                .WithMany(s => s.Inventories)
                .HasForeignKey(e => e.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.Inbounds)
                .WithOne(i => i.Inventory)
                .HasForeignKey(i => i.InventoryId);

            builder.HasMany(e => e.Outbounds)
                .WithOne(o => o.Inventory)
                .HasForeignKey(o => o.InventoryId);
        }
    }
}