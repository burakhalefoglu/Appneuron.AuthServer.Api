using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    internal class ClientGroupEntityConfiguration : IEntityTypeConfiguration<ClientGroup>
    {
        public void Configure(EntityTypeBuilder<ClientGroup> builder)
        {
            builder.HasKey(e => new { e.ClientId, e.GroupId });

            builder.HasOne(d => d.Group)
                .WithMany(p => p.ClientGroups)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserGroups_Id_GroupId");

            builder.HasOne(d => d.client)
                .WithMany(p => p.ClientGroups)
                .HasForeignKey(d => d.ClientId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserGroups_Id_UserId");
        }
    }
}