using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BankSystem.Data.Contexts
{
    public class BankingContextFactory : IDesignTimeDbContextFactory<BankingContext>
    {
        public BankingContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BankingContext>();
            optionsBuilder.UseSqlServer("Server=.;Database=BankSystemDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new BankingContext(optionsBuilder.Options);
        }
    }
}
