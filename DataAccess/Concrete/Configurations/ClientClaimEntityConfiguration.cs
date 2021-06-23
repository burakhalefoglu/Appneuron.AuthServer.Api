using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class ClientClaimEntityConfiguration : IEntityTypeConfiguration<ClientClaim>
    {
        public void Configure(EntityTypeBuilder<ClientClaim> builder)
        {
            builder.HasKey(e => new { e.ClaimId, e.ClientId });

            builder.HasOne(d => d.Claim)
                .WithMany(p => p.ClientClaims)
                .HasForeignKey(d => d.ClaimId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_ClientGroups_Claim");

            builder.HasOne(d => d.Clients)
                .WithMany(p => p.ClientClaims)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_ClientClaims_Client");
        }
    }
}