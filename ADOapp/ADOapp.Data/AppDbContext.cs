using Microsoft.EntityFrameworkCore;
namespace ADOapp.Data;
public class AppDbContext : DbContext
{
    public DbSet<Clients> Clients => Set<Clients>();
    public DbSet<Games> Games => Set<Games>();
    public DbSet<Rentals> Rentals => Set<Rentals>();
    public static string DbPath
    {
        get
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Directory.CreateDirectory(folder);
            return Path.Combine(folder, "adoapp.db");
        }
    }
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}