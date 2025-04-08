using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public async Task<IActionResult> Index(ClothingType? clothingTypes = null, string? searchString = null)
    {
        if (_context.ClothingItem == null)
        {
            return Problem("Entity set 'ClothingTracker.ClothingItem' is null.");
        }

        // Use LINQ to get list of types.
        IQueryable<ClothingType> typeQuery = (from m in _context.ClothingItem
                                        orderby m.Type
                                        select m.Type);
        var clothingItems = from m in _context.ClothingItem
                     select m;

        if (!string.IsNullOrEmpty(searchString))
        {
            clothingItems = clothingItems.Where(s => s.Name!.ToUpper().Contains(searchString.ToUpper()));
        }

        if (clothingTypes != null)
        {
            clothingItems = clothingItems.Where(x => x.Type == clothingTypes);
        }

        var clothingTypeVM = new ClothingTypeViewModel
        {
            ClothingTypes = new SelectList(await typeQuery.Distinct().ToListAsync()),
            ClothingItems = await clothingItems.ToListAsync()
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
        public async Task<IActionResult> Create([Bind("Id,Name,Type,Color,WearsBeforeWash")] ClothingItem clothingItem)
        {
            if (ModelState.IsValid)
            {
                clothingItem.WearsRemaining = clothingItem.WearsBeforeWash;
                clothingItem.TotalWears = 0;
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
            if (clothingItem != null && clothingItem.WearsRemaining > 0)
            {
                clothingItem.WearsRemaining--;
                clothingItem.TotalWears++;
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
                clothingItem.WearsRemaining = clothingItem.WearsBeforeWash;
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,Color")] ClothingItem clothingItem)
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
