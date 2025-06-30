using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Infrastructure.Persistence.EntityMap;
public class AuditableBaseEntityMap<T> : IEntityTypeConfiguration<T> where T : AuditableBaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.Created)
            .IsRequired();

        builder.Property(e => e.LastModifiedBy)
            .HasMaxLength(100);

        builder.Property(e => e.LastModified)
            .IsRequired(false);
    }
}