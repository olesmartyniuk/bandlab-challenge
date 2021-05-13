using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Imagegram.Api.Database.Models
{
    public class PostModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public AccountModel Creator { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CommentsCount { get; set; }
        public CommentModel CommentLast { get; set; }
        public CommentModel CommentBeforeLast { get; set; }
    }

    public class PostModelConfiguration : IEntityTypeConfiguration<PostModel>
    {
        public void Configure(EntityTypeBuilder<PostModel> builder)
        {
            builder
                .Property(p => p.Id)
                .IsRequired();
            builder
                .HasIndex(p => p.CommentsCount);
            builder
                .HasOne(p => p.CommentLast);
            builder
                .HasOne(p => p.CommentBeforeLast);
        }
    }
}
