using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrderItemsController : Controller
    {
        private readonly Database1Context _context;

        public OrderItemsController(Database1Context context)
        {
            _context = context;
        }

        // GET: OrderItems
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["QuantitySortParm"] = sortOrder == "Quantity" ? "quantity_desc" : "Quantity";
            ViewData["UnitSortParm"] = sortOrder == "Unit" ? "unit_desc" : "Unit";
            ViewData["OrderSortParm"] = sortOrder == "Order" ? "order_desc" : "Order";
            var database1Context = _context.OrderItems.Include(o => o.Order);
            switch (sortOrder)
            {
                case "name_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Name).Include(o => o.Order);
                    break;
                case "Quantity":
                    database1Context = database1Context.OrderBy(s => s.Quantity).Include(o => o.Order);
                    break;
                case "quantity_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Quantity).Include(o => o.Order);
                    break;
                case "Unit":
                    database1Context = database1Context.OrderBy(s => s.Unit).Include(o => o.Order);
                    break;
                case "unit_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Unit).Include(o => o.Order);
                    break;
                case "Order":
                    database1Context = database1Context.OrderBy(s => s.Order).Include(o => o.Order);
                    break;
                case "order_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Order).Include(o => o.Order);
                    break;
                default:
                    database1Context = database1Context.OrderBy(s => s.Name).Include(o => o.Order);
                    break;
            }
            return View(await database1Context.ToListAsync());
        }

        // GET: OrderItems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // GET: OrderItems/Create
        public IActionResult Create()
        {
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Number");
            return View();
        }

        // POST: OrderItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderId,Name,Quantity,Unit")] OrderItem orderItem)
        {
            if (ModelState.IsValid)
            {
				var filter = await _context.OrderItems.Select(s => s.Order).Where(w => w.Id == orderItem.OrderId).FirstOrDefaultAsync();
				if (filter?.Number == orderItem.Name)
                {
                    ModelState.AddModelError(nameof(orderItem.Name), "Name must be unique to this Order");
                }
                else
                {
                    _context.Add(orderItem);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem == null)
            {
                return NotFound();
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Number", orderItem.OrderId);
            return View(orderItem);
        }

        // POST: OrderItems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderId,Name,Quantity,Unit")] OrderItem orderItem)
        {
            if (id != orderItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				var filter = await _context.OrderItems.Select(s => s.Order).Where(w => w.Id == orderItem.OrderId).FirstOrDefaultAsync();
                if (filter?.Number == orderItem.Name)
                {
                    ModelState.AddModelError(nameof(orderItem.Name), "Name must be unique to this Order");
                }
                else
                {
                    try
                    {
                        _context.Update(orderItem);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrderItemExists(orderItem.Id))
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
            }
            ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderItem.OrderId);
            return View(orderItem);
        }

        // GET: OrderItems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.OrderItems == null)
            {
                return NotFound();
            }

            var orderItem = await _context.OrderItems
                .Include(o => o.Order)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderItem == null)
            {
                return NotFound();
            }

            return View(orderItem);
        }

        // POST: OrderItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.OrderItems == null)
            {
                return Problem("Entity set 'Database1Context.OrderItems'  is null.");
            }
            var orderItem = await _context.OrderItems.FindAsync(id);
            if (orderItem != null)
            {
                _context.OrderItems.Remove(orderItem);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderItemExists(int id)
        {
          return (_context.OrderItems?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
