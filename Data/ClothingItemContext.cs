using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClothingTracker.Models;

namespace ClothingTracker.Data
{
    public class ClothingItemContext : DbContext
    {
        public ClothingItemContext (DbContextOptions<ClothingItemContext> options)
            : base(options)
        {
        }

        public DbSet<ClothingItem> ClothingItem { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClothingItem>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();  // Ensures ID is auto-generated
        }
    }
}
