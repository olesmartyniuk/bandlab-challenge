using Imagegram.Api.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Imagegram.Api.Database
{
    public class ApplicationContext : DbContext
    {
        public DbSet<AccountModel> Accounts { get; set; }
        public DbSet<PostModel> Posts { get; set; }
        public DbSet<CommentModel> Comments { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
        {
        }
    }
}
