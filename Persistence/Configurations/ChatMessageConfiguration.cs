using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Persistence.Models;

namespace Persistence.Configurations;

public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
{
    public void Configure(EntityTypeBuilder<ChatMessage> builder)
    {
        builder.HasKey(x => new { x.MessageId, x.GetterMessageId });

        builder.HasOne(x => x.User)
            .WithMany();

        builder.HasOne(x => x.GetterUser)
            .WithMany();
    }
}
