using ClothingTracker.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClothingTracker.Models
{
    public class ClothingTypeViewModel
    {
        public ClothingTypeViewModel() { }
        public List<ClothingItem>? ClothingItems { get; set; }
        public ClothingType? SearchTypeSelection { get; set; }
        public List<SimpleDiscreteColor>? SearchColorSelections { get; set; }
        public SelectList? SearchColorOptions { get; set; }
        public string? SearchString { get; set; }
        public bool OnlyShowDirty { get; set; }
    }
}
