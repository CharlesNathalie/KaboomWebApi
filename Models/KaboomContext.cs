using Microsoft.EntityFrameworkCore;
namespace KaboomWebApi.Models;

public class KaboomDbContext : DbContext
{
    public virtual DbSet<User> Users { get; set; }
    public virtual DbSet<Game> Games { get; set; }

    public KaboomDbContext()
    {
    }
    public KaboomDbContext(DbContextOptions<KaboomDbContext> options) : base(options) { }
}

