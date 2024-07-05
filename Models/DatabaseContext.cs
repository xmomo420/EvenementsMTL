using Microsoft.EntityFrameworkCore;

namespace TestReactOther.Models;

public class DatabaseContext : DbContext
{
    public DbSet<Utilisateur> Utilisateurs { get; set; }
    public DbSet<Evenement> Evenements { get; set; }

    public DatabaseContext(DbContextOptions options) : base(options) {  }
    
    
}