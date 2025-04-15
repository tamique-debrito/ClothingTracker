using Microsoft.EntityFrameworkCore;
using ClothingTracker.Data;
using ClothingTracker.Models.Shared;

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
                        Color = SimpleDiscreteColor.Black,
                        DetailedDescription = "Jeans with a grainy texture",
                        WashType = WashType.NumberOfWears,
                        WearsBeforeWash = 3,
                        TotalWears = 0,
                    },

                    new ClothingItem
                    {
                        Name = "White T-shirt",
                        Type = ClothingType.TShirt,
                        Color = SimpleDiscreteColor.White,
                        DetailedDescription = "A plain, white T-shirt. Nothing special",
                        WashType = WashType.NumberOfWears,
                        WearsBeforeWash = 1,
                        TotalWears = 0,
                    },

                    new ClothingItem
                    {
                        Name = "Grey T-shirt",
                        Type = ClothingType.TShirt,
                        Color = SimpleDiscreteColor.Grey,
                        DetailedDescription = "",
                        WashType = WashType.NumberOfWears,
                        WearsBeforeWash = 1,
                        TotalWears = 0,
                    },

                    new ClothingItem
                    {
                        Name = "Tan Towel",
                        Type = ClothingType.Towel,
                        Color = SimpleDiscreteColor.Tan,
                        DetailedDescription = "",
                        WashType = WashType.NumberOfDays,
                        DaysBeforeWash = 8,
                        TotalWears = 0,
                    }
                );
                context.SaveChanges();
            }
        }
    }
}