using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingTracker.Models
{

    public enum ClothingType
    {
        Shirt = 1,
        Pants = 2,
        Sweater = 3,
    }
    public enum SimpleClothingColor
    {
        Black = 1,
        White = 2,
        Grey = 3,
        Blue = 4,
        Green = 5,
        Red = 6,
        Brown = 7,
    }

    public class ClothingItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(30, MinimumLength = 3)]
        [Column(TypeName = "nchar(30)")]
        [Required]
        public required string Name { get; set; }


        [StringLength(1000)]
        [Column(TypeName = "nvarchar(1000)")]
        [Display(Name = "Detailed Description")]
        public string? DetailedDescription { get; set; }

        public ClothingType Type { get; set; }

        public SimpleClothingColor Color { get; set; }

        [Display(Name = "Wears Before Wash")]
        [Required]
        public int WearsBeforeWash { get; set; }

        [Display(Name = "Wears Remaining Until Wash Needed")]
        public int? WearsRemaining { get; set; }

        [Display(Name = "Total Times Worn")]
        public int? TotalWears { get; set; }
    }
}