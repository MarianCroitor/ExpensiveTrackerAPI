using Microsoft.EntityFrameworkCore;
using ExpensiveTrackerAPI.Models;
   
namespace ExpensiveTrackerAPI.Data;

public class AppDbContext : DbContext
{
   public AppDbContext(DbContextOptions<AppDbContext> options) 
      : base(options) { }

   public DbSet<User> Users { get; set; }
   public DbSet<Category> Categories { get; set; }
   public DbSet<Transaction> Transactions { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
   {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<User>()
         .HasMany(u => u.Categories)
         .WithOne(c => c.User)
         .HasForeignKey(c => c.UserId)
         .IsRequired();

      modelBuilder.Entity<Category>()
         .HasMany(c => c.Transactions)
         .WithOne(t => t.Category)
         .HasForeignKey(t => t.CategoryId)
         .IsRequired();
   }
}
      
   
