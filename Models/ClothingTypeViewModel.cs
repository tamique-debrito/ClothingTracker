using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothingTracker.Models
{
    public class ClothingTypeViewModel
    {
        public List<ClothingItem>? ClothingItems { get; set; }
        public ClothingType? SearchType { get; set; }
        public string? SearchString { get; set; }
    }
}
