using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Imagegram.Api.Database.Models
{
    public class AccountModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class AccountModelConfiguration : IEntityTypeConfiguration<AccountModel>
    {
        public void Configure(EntityTypeBuilder<AccountModel> builder)
        {
            builder
                .Property(p => p.Id)
                .IsRequired();
        }
    }
}
