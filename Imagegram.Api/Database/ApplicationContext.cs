using Imagegram.Api.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
