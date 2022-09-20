using Identity.Core.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Databases.Configurations;

public class UserConfiguration: IEntityTypeConfiguration<UserReadModel>
{
    public void Configure(EntityTypeBuilder<UserReadModel> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Name).IsRequired().IsUnicode();
        builder.Property(u => u.Email).IsRequired().IsUnicode();
        builder.Property(u => u.Password).IsRequired().IsUnicode();
        builder.Property(u => u.AccessTokens).IsUnicode().IsRequired().HasColumnType("jsonb");
        builder.ToTable("Users");
    }
}