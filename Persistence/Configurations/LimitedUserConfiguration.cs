using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Models;

namespace Persistence.Configurations;

public class LimitedUserConfiguration : IEntityTypeConfiguration<LimitedUser>
{
    public void Configure(EntityTypeBuilder<LimitedUser> builder)
    {
        builder.HasKey(x => x.UserId);
    }
}
