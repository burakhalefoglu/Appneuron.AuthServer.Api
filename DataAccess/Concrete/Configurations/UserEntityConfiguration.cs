using Core.Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Concrete.Configurations
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(x => x.UserId);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.Property(x => x.Email).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Status).IsRequired();
            builder.Property(x => x.RecordDate);
            builder.Property(x => x.MobilePhones).HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(500);
            builder.Property(x => x.ResetPasswordToken).HasMaxLength(500);
            builder.Property(x => x.ResetPasswordExpires);
        }
    }
}