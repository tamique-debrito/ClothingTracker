using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClothingTracker.Data;
using ClothingTracker.Models;

namespace ClothingTracker.Controllers
{
    public class ClothingItemsController : Controller
    {
        private readonly ClothingItemContext _context;

        public ClothingItemsController(ClothingItemContext context)
        {
            _context = context;
        }

        // GET: ClothingItems
        public async Task<IActionResult> Index(ClothingType? searchTypeSelection = null, List<SimpleClothingColor>? searchColorSelections = null, string? searchString = null, bool? onlyShowDirty = null)
        {
            if (_context.ClothingItem == null)
            {
                return Problem("Entity set 'ClothingTracker.ClothingItem' is null.");
            }

            var clothingItems = from m in _context.ClothingItem
                         select m;

            if (!string.IsNullOrEmpty(searchString))
            {
                clothingItems = clothingItems.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));
            }

            if (searchTypeSelection != null)
            {
                clothingItems = clothingItems.Where(x => x.Type == searchTypeSelection);
            }

            if (searchColorSelections != null && searchColorSelections.Count > 0)
            {
                clothingItems = clothingItems.Where(x => searchColorSelections.Contains(x.Color));
            }

            var colorSelectList = new SelectList(Enum.GetValues(typeof(SimpleClothingColor)).Cast<SimpleClothingColor>().ToList());

            var filteredItems = await clothingItems.ToListAsync();

            if (onlyShowDirty.HasValue && (bool)onlyShowDirty)
            {
                filteredItems = filteredItems.Where(x => x.NeedsWash()).ToList();
            }

            var clothingTypeVM = new ClothingTypeViewModel
            {
                SearchTypeSelection = searchTypeSelection,
                SearchColorSelections = searchColorSelections,
                SearchColorOptions = colorSelectList,
                ClothingItems = filteredItems,
            };

            return View(clothingTypeVM);
        }

        // GET: ClothingItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ClothingItem == null)
            {
                return NotFound();
            }

            var clothingItem = await _context.ClothingItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clothingItem == null)
            {
                return NotFound();
            }

            return View(clothingItem);
        }

        // GET: ClothingItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClothingItems/Create
        // More info about Bind: http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Color,DetailedDescription,WashType,WearsBeforeWash,DaysBeforeWash")] ClothingItem clothingItem) // Maybe don't restrict anything here
        {
            if (ModelState.IsValid)
            {
                clothingItem.Init();
                _context.Add(clothingItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(clothingItem);
        }

        // POST: ClothingItems/MarkWorn
        // Decrements the number of wears the item has
        [HttpPost("MarkWorn/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWorn(int id)
        {
            var clothingItem = _context.ClothingItem.Find(id);
            if (clothingItem != null)
            {
                clothingItem.MarkWorn();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // POST: ClothingItems/MarkWashed
        // Resets the number of wears the item has
        [HttpPost("MarkWashed/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkWashed(int id)
        {
            var clothingItem = _context.ClothingItem.Find(id);
            if (clothingItem != null)
            {
                clothingItem.MarkWashed();
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        // GET: ClothingItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ClothingItem == null)
            {
                return NotFound();
            }

            var clothingItem = await _context.ClothingItem.FindAsync(id);
            if (clothingItem == null)
            {
                return NotFound();
            }
            return View(clothingItem);
        }

        // POST: ClothingItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,DetailedDescription,Color")] ClothingItem clothingItem)
        {
            if (id != clothingItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(clothingItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClothingItemExists(clothingItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(clothingItem);
        }

        // GET: ClothingItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ClothingItem == null)
            {
                return NotFound();
            }

            var clothingItem = await _context.ClothingItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (clothingItem == null)
            {
                return NotFound();
            }

            return View(clothingItem);
        }

        // POST: ClothingItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ClothingItem == null)
            {
                return Problem("Entity set 'ClothingTracker.ClothingItem'  is null.");
            }
            var clothingItem = await _context.ClothingItem.FindAsync(id);
            if (clothingItem != null)
            {
                _context.ClothingItem.Remove(clothingItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClothingItemExists(int id)
        {
          return (_context.ClothingItem?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
