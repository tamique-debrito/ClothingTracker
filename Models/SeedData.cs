using Microsoft.EntityFrameworkCore;
using ClothingTracker.Data;

namespace ClothingTracker.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ClothingItemContext(
                serviceProvider.GetRequiredService<
                    DbContextOptions<ClothingItemContext>>()))
            {
                // Look for any clothingItems.
                if (context.ClothingItem.Any())
                {
                    return;   // DB has been seeded
                }

                context.ClothingItem.AddRange(
                    new ClothingItem
                    {
                        Name = "Greyish-Black Jeans",
                        Type = ClothingType.Pants,
                        Color = SimpleClothingColor.Black,
                        WearsBeforeWash = 3,
                        TotalWears = 0,
                    },

                    new ClothingItem
                    {
                        Name = "White T-shirt",
                        Type = ClothingType.Shirt,
                        Color = SimpleClothingColor.White,
                        WearsBeforeWash = 1,
                        TotalWears = 0,
                    }
                );
                context.SaveChanges();
            }
        }
    }
}