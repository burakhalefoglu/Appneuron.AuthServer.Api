using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class UserClaimEntityConfiguration : IEntityTypeConfiguration<UserClaim>
    {
        public void Configure(EntityTypeBuilder<UserClaim> builder)
        {
            builder.HasKey(e => new { e.ClaimId, e.UsersId });

            builder.HasIndex(e => new { e.UsersId, e.ClaimId }, "Uk_Uniqe")
                .IsUnique();

            builder.HasOne(d => d.Claim)
                .WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserClaims_Id_ClaimId");

            builder.HasOne(d => d.Users)
                .WithMany(p => p.UserClaims)
                .HasForeignKey(d => d.UsersId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserClaims_Id_UserId");
        }
    }
}