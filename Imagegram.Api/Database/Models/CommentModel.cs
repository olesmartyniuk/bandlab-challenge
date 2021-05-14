using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Imagegram.Api.Database.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int PostId { get; set; }
        public PostModel Post { get; set; }
        public Guid CreatorId { get; set; }
        public AccountModel Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentModelConfiguration : IEntityTypeConfiguration<CommentModel>
    {
        public void Configure(EntityTypeBuilder<CommentModel> builder)
        {
            builder
                .Property(c => c.Id)
                .IsRequired();
            builder
               .HasOne(c => c.Post);
        }
    }
}
