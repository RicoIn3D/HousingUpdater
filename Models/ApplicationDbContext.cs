using Microsoft.EntityFrameworkCore;

namespace _3DUpdateTenenciesWithExcel.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Tenancy> Tenancies { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
