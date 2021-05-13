using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Imagegram.Api.Database.Models
{
    public class CommentModel
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public AccountModel Creator { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentModelConfiguration : IEntityTypeConfiguration<CommentModel>
    {
        public void Configure(EntityTypeBuilder<CommentModel> builder)
        {
            builder
                .Property(p => p.Id)
                .IsRequired();
        }
    }
}
