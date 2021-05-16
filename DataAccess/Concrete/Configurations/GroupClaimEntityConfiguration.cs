using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class GroupClaimEntityConfiguration : IEntityTypeConfiguration<GroupClaim>
    {
        public void Configure(EntityTypeBuilder<GroupClaim> builder)
        {
            builder.HasKey(e => new { e.GroupId, e.ClaimId });

            builder.HasOne(d => d.OperationClaim)
                .WithMany(p => p.GroupClaims)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupClaims_Id_ClaimId");

            builder.HasOne(d => d.Group)
                .WithMany(p => p.GroupClaims)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupClaims_Id_GroupId");
        }
    }
}