using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class OrdersController : Controller
    {
        private readonly Database1Context _context;

        public OrdersController(Database1Context context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string sortOrder, string[] NumberSelectParms, int[] ProviderSelectParms, DateTime DateSelectParm1, DateTime DateSelectParm2)
        {
            ViewData["IdSortParm"] = String.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            ViewData["NumberSortParm"] = sortOrder == "Number" ? "number_desc" : "Number";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["ProviderSortParm"] = sortOrder == "Provider" ? "provider_desc" : "Provider";

            ViewData["NumberSelectParms"] = new SelectList(_context.Orders.GroupBy(g => g.Number).Select(s => s.Key).ToList(), NumberSelectParms);
            ViewData["ProviderSelectParms"] = new SelectList(_context.Providers, "Id", "Name", ProviderSelectParms);

            ViewData["DateSelectParm1"] = DateSelectParm1 == DateTime.MinValue ? DateTime.UtcNow.AddMonths(-1).ToString("yyyy-MM-dd") : DateSelectParm1.ToString("yyyy-MM-dd");
            ViewData["DateSelectParm2"] = DateSelectParm2 == DateTime.MinValue ? DateTime.UtcNow.ToString("yyyy-MM-dd") : DateSelectParm2.ToString("yyyy-MM-dd");

            var database1Context = _context.Orders.Include(o => o.Provider);

            NumberSelectParms = NumberSelectParms.Length == 0 ? _context.Orders.GroupBy(g => g.Number).Select(s => s.Key).ToArray() : NumberSelectParms;
			ProviderSelectParms = ProviderSelectParms.Length == 0 ? _context.Orders.GroupBy(g => g.Provider.Id).Select(s => s.Key).ToArray() : ProviderSelectParms;

			DateSelectParm1 = DateSelectParm1 == DateTime.MinValue ? DateTime.UtcNow.AddMonths(-1) : DateSelectParm1;
			DateSelectParm2 = DateSelectParm2 == DateTime.MinValue ? DateTime.UtcNow: DateSelectParm2;

			database1Context = database1Context.Where(w => NumberSelectParms.Contains(w.Number) && ProviderSelectParms.Contains(w.Provider.Id) && (w.Date.Date >= DateSelectParm1.Date || w.Date.Date <= DateSelectParm2.Date)).Include(o => o.Provider);

            switch (sortOrder)
            {
                case "id_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Id).Include(o => o.Provider);
                    break;
                case "Number":
                    database1Context = database1Context.OrderBy(s => s.Number).Include(o => o.Provider);
                    break;
                case "number_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Number).Include(o => o.Provider);
                    break;
                case "Date":
                    database1Context = database1Context.OrderBy(s => s.Date).Include(o => o.Provider);
                    break;
                case "date_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Date).Include(o => o.Provider);
                    break;
                case "Provider":
                    database1Context = database1Context.OrderBy(s => s.Provider).Include(o => o.Provider);
                    break;
                case "provider_desc":
                    database1Context = database1Context.OrderByDescending(s => s.Provider).Include(o => o.Provider);
                    break;
                default:
                    database1Context = database1Context.OrderBy(s => s.Id).Include(o => o.Provider);
                    break;
            }
            return View(await database1Context.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Provider)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Number,Date,ProviderId")] Order order)
        {
            if (ModelState.IsValid)
            {
                var filter = await _context.Orders.FirstOrDefaultAsync(x => x.Number == order.Number && x.ProviderId == order.ProviderId);
                if (filter?.Number == order.Number)
                {
                    ModelState.AddModelError(nameof(order.Number), "Number must be unique to this Provider");
                }
                else
                {
                    _context.Add(order);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name", order.ProviderId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name", order.ProviderId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Number,Date,ProviderId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
				var filter = await _context.Orders.FirstOrDefaultAsync(x => x.Number == order.Number && x.ProviderId == order.ProviderId);
                if (filter?.Number == order.Number)
                {
                    ModelState.AddModelError(nameof(order.Number), "Number must be unique to this Provider");
                }
                else
                {
                    try
                    {
                        _context.Update(order);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!OrderExists(order.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Details), new { id = id });
                }
            }
            ViewData["ProviderId"] = new SelectList(_context.Providers, "Id", "Name", order.ProviderId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Provider)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'Database1Context.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
