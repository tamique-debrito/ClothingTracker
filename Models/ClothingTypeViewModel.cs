using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothingTracker.Models
{
    public class ClothingTypeViewModel
    {
        public List<ClothingItem>? ClothingItems { get; set; }
        public SelectList? ClothingTypes { get; set; }
        public string? SearchString { get; set; }
    }
}
