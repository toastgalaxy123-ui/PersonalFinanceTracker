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
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // MUST call the base method first for Identity to be configured
            base.OnModelCreating(builder);

            // Ensure the User -> Account relationship uses RESTRICT
            builder.Entity<User>()
                .HasMany(u => u.Accounts)
                .WithOne(a => a.User)
                .OnDelete(DeleteBehavior.Restrict); // <-- NEW FIX 1

            // Ensure the User -> Category relationship uses RESTRICT
            builder.Entity<User>()
                .HasMany(u => u.Categories)
                .WithOne(c => c.User)
                .OnDelete(DeleteBehavior.Restrict); // <-- NEW FIX 2

            // 1. Disable cascade for Transaction -> Account relationship
            builder.Entity<Transaction>()
                .HasOne(t => t.Account)
                .WithMany(a => a.Transactions)
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict); // <-- FIX 1

            // 2. Disable cascade for Transaction -> Category relationship
            builder.Entity<Transaction>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Transactions)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // <-- FIX 2

            foreach (var relationship in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }


        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        
    }

}
