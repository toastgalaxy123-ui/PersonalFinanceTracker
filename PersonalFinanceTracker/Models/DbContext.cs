using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using PersonalFinanceTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace PersonalFinanceTracker.Models
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
