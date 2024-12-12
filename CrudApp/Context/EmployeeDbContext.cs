using CrudApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CrudApp.Context
{
    public class EmployeeDbContext : DbContext
    {
        public EmployeeDbContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Employee> Employees { get; set; }
    }
}
