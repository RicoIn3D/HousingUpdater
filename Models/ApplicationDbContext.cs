using Microsoft.EntityFrameworkCore;

namespace _3DUpdateTenenciesWithExcel.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Address> Аddresses { get; set; }
        public DbSet<Tenancy> Tenancies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Client> Clients { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
    }
}
