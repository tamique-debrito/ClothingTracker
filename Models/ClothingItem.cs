using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClothingTracker.Models
{

    public enum ClothingType
    {
        // Regular clothing types
        TShirt = 1,
        Pants = 2,
        Sweater = 3,
        CollaredShirt = 4,
        Coat = 5,
        Shoes = 6,
        // Accessory types
        Hat = 100,
        Belt = 101,
        // Non-clothes
        Towel = 201,
        Bedsheets = 202,
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
        Tan = 8,
        Purple = 9,
        Orange = 10,
    }

    public enum WashType
    // Represents how it's determined when a clothing item should be washed
    {
        [Display(Name = "By number of wears")]
        NumberOfWears = 1,
        [Display(Name = "By number of days used")]
        NumberOfDays = 2, // This mostly applies to things like bedsheets and towels in kitchen/bathroom.
        [Display(Name = "Item does not need washing")]
        NoWash = 3, // This mostly applies to things like shoes, hats, and gloves
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

        // ########### Wear Tracking

        [Required]
        [Display(Name = "Wash Type")]
        public WashType WashType { get; set; }

        [Display(Name = "Wears Before Wash Needed")]
        public int? WearsBeforeWash { get; set; }

        [Display(Name = "Wears Until Next Wash")]
        public int? WearsRemaining { get; set; }

        [Display(Name = "Days of Use Before Wash Needed")]
        public int? DaysBeforeWash { get; set; }

        [Display(Name = "Date Of Next Wash")]
        public DateTime? NextWashDate { get; set; } // If the clothing item is washed after a certain number of days, the next wash date. Whether this value is null indicates whether the item is currently in use (will need to add extra properties to track multiple interchangeable items actually)

        [Display(Name = "Total Times Worn")]
        public int? TotalWears { get; set; }

        internal void Init() // Initialize the fields that must be filled in on the creation of a new item
        {
            // Assumes WearTracking.WearsBeforeWash has been populated
            if (WashType == WashType.NoWash) { WearsBeforeWash = null; DaysBeforeWash = null; }
            else if (WashType == WashType.NumberOfWears) { WearsRemaining = WearsBeforeWash; DaysBeforeWash = null; }
            else if (WashType == WashType.NumberOfDays) { WearsBeforeWash = null; }
            else { throw new NotImplementedException("Unrecognized wash type"); }
            TotalWears = 0;
        }
        public bool NeedsWash() // Whether this clothing item currently needs to be washed
        {
            if (WashType == WashType.NoWash) { return false; }
            else if (WashType == WashType.NumberOfWears) { return WearsRemaining <= 0; }
            else if (WashType == WashType.NumberOfDays)
            {
                if (NextWashDate == null) return false;
                else return NextWashDate <= DateTime.Today;
            }
            else { throw new NotImplementedException("Unrecognized wash type"); }
        }
        public void MarkWorn()
        {
            if (WashType == WashType.NoWash) { }
            else if (WashType == WashType.NumberOfWears) { WearsRemaining--; }
            else if (WashType == WashType.NumberOfDays) { }
            else { throw new NotImplementedException("Unrecognized wash type"); }
            TotalWears++;
        }
        public void MarkWashed()
        {
            if (WashType == WashType.NoWash) { throw new InvalidDataException("A NoWash type should not be marked as washed"); }
            else if (WashType == WashType.NumberOfWears)
            {
                if (WearsBeforeWash == null) { throw new InvalidDataException("A NumberOfWears type needs WearsBeforeWash specified"); }
                WearsRemaining = WearsBeforeWash;
            }
            else if (WashType == WashType.NumberOfDays)
            {
                if (DaysBeforeWash == null) throw new InvalidDataException("A NumberOfDays type needs DaysBeforeWash specified");
                NextWashDate = DateTime.Today.AddDays((double)DaysBeforeWash);
            }
            else { throw new NotImplementedException("Unrecognized wash type"); }
        }
        public string NextWashText()
        {
            if (WashType == WashType.NoWash) { return "N/A"; }
            else if (WashType == WashType.NumberOfWears)
            {
                if (WearsRemaining == null) { throw new InvalidDataException("A NumberOfWears type needs WearsBeforeWash specified"); } // This kind of validation should be moved somewhere else...
                else if (WearsRemaining > 0)
                {
                    return "After " + WearsRemaining + " more wears.";
                }
                else
                {
                    return "Before next use.";
                }
            }
            else if (WashType == WashType.NumberOfDays)
            {
                if (NextWashDate == null)
                {
                    return "Not in use";
                }
                else if (NextWashDate > DateTime.Today)
                {
                    return "On " + NextWashDate + ".";
                }
                else
                {
                    return "Before next use.";
                }
            }
            else { throw new NotImplementedException("Unrecognized wash type"); }
        }
    }
}