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
    public class AcquisitionMap : AuditableBaseEntityMap<Acquisition>
    {
        public override void Configure(EntityTypeBuilder<Acquisition> builder)
        {
            base.Configure(builder);

            builder.ToTable("Acquisitions");

            builder.Property(e => e.Status).IsRequired();
            builder.Property(e => e.TotalItems).IsRequired();
            builder.Property(e => e.ReceivedDate).IsRequired(false).HasColumnType("timestamp with time zone");
            builder.Property(e => e.filePath).IsRequired(false);

            builder.HasOne(e => e.Supplier)
                .WithMany(i => i.Acquisitions)
                .HasForeignKey(e => e.SupplierId);

            builder.HasMany(o => o.Items)
                .WithOne(mu => mu.Acquisition)
                .HasForeignKey(mu => mu.AcquisitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}