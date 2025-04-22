using ClothingTracker.Models.Shared;
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

    public enum InUseStatus
    // Whether the item is "in use" or not. This is used for the "NumberOfDays" wash type to signal whether the item is currently being used (e.g. is the Tan Towel currently on the towel rack in the bathroom)
    // The correct state change for this use case is (wash date not null, in use) -[mark not in use]> (wash date not null, not in use) -[mark washed]> (wash date null, not in use) -[mark in use]> (wash date not null, in use)
    {
        [Display(Name = "In use")]
        InUse = 1,
        [Display(Name = "Not in use")]
        NotInUse = 2,
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

        public SimpleDiscreteColor Color { get; set; }

        // ########### Wear Tracking

        [Required]
        [Display(Name = "Wash Type")]
        public WashType WashType { get; set; }

        [Display(Name = "In Use")]
        public InUseStatus InUse { get; set; }

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

        // Methods

        internal void InitNextWashDate()
        {
            NextWashDate = DateTime.Today.AddDays((double)DaysBeforeWash);
        }

        internal void Init() // Initialize the fields that must be filled in on the creation of a new item
        {
            // Assumes WearTracking.WearsBeforeWash has been populated
            if (WashType == WashType.NoWash) { WearsBeforeWash = null; DaysBeforeWash = null; InUse = InUseStatus.InUse; }
            else if (WashType == WashType.NumberOfWears) { WearsRemaining = WearsBeforeWash; DaysBeforeWash = null; InUse = InUseStatus.InUse;  }
            else if (WashType == WashType.NumberOfDays) { WearsBeforeWash = null; InUse = InUseStatus.NotInUse; }
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
        public void MarkInUse()
        {
            InUse = InUseStatus.InUse;
            if (WashType == WashType.NumberOfDays && NextWashDate == null) 
            {
                InitNextWashDate();
            }
        }
        public void MarkNotInUse()
        {
            InUse = InUseStatus.NotInUse;
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
                if (InUse == InUseStatus.InUse)
                {
                    InitNextWashDate();
                }
                else
                {
                    NextWashDate = null;
                }
            }
            else { throw new NotImplementedException("Unrecognized wash type"); }
        }
        public string NextWashText()
        {
            if (WashType == WashType.NoWash) { return "N/A"; }
            else if (WashType == WashType.NumberOfWears)
            {
                if (WearsRemaining == null) { throw new InvalidDataException("A NumberOfWears type needs WearsBeforeWash specified"); } // This kind of validation should be moved somewhere else...
                else if (WearsRemaining == 1)
                {
                    return "After one more wear.";
                }
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
                    return "On " + DateOnly.FromDateTime((DateTime)NextWashDate) + ".";
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