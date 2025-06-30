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
    public class OrderMap : AuditableBaseEntityMap<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);

            builder.ToTable("Orders");

            builder.Property(e => e.Model).IsRequired().HasMaxLength(50);
            builder.Property(e => e.ModelCode).IsRequired().HasMaxLength(100);
            builder.Property(e => e.ModelCategory).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Status).IsRequired().HasMaxLength(50);
            builder.Property(e => e.ReceivedDate).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(e => e.RequiredDelDate).IsRequired().HasColumnType("timestamp with time zone");
            builder.Property(e => e.ActualDelDate).HasColumnType("timestamp with time zone");
            builder.Property(e => e.Quantity).IsRequired();
            builder.Property(e => e.UnitPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.TotalPrice).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.UnitWages).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.OtherCost).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.HardwareCost).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.MaterialCost).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.TotalRevenue).IsRequired().HasColumnType("decimal(18,4)");
            builder.Property(e => e.Remark).HasMaxLength(500);
            builder.Property(e => e.filePath).IsRequired(false);
        }
    }
}