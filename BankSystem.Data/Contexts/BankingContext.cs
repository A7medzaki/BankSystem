using BankSystem.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BankSystem.Data.Contexts
{
    public class BankingContext : DbContext
    {

        public BankingContext(DbContextOptions<BankingContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankingContext).Assembly);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
    }
}
