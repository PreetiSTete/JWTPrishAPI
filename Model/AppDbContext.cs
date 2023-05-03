using Microsoft.EntityFrameworkCore;
using login_api.Model;

namespace login_api.Model;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

  public DbSet<UserAuthentication> UserAuthentications { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Authenticate> Authenticates { get; set; }
//  protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<Authenticate>().ToTable("Authenticate");
//     }

  
}