using DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class AppDbContext : IdentityDbContext
{
    public DbSet<User> Users { get; set; }
    
    public AppDbContext(DbContextOptions options) : base(options)
    {
        
    }
}