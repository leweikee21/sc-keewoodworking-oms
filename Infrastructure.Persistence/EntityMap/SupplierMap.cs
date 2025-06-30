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
    public class SupplierMap : AuditableBaseEntityMap<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            base.Configure(builder);

            builder.ToTable("Suppliers");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.PhoneNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ContactPerson)
                .IsRequired()
                .HasMaxLength(255);
        }
    }

}