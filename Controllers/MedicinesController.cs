using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hospital;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class MedicinesController : Controller
    {
        private readonly AppDbContext _context;

        public MedicinesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Medicines
        // No server-side paging or sorting — DataTables or client handles that
        // Optional "q" lets you pre-filter if you add search box in future
        public async Task<IActionResult> Index(string q = null)
        {
            var medicines = _context.Medicines.AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                medicines = medicines.Where(m =>
                    m.Name.Contains(q) ||
                    m.Price.ToString().Contains(q) ||
                    m.Quantity.ToString().Contains(q));
            }

            return View(await medicines.AsNoTracking().ToListAsync());
        }

        // Create GET
        public IActionResult Create() => View();

        // Create POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicineId,Name,Price,Quantity")] Medicine medicine)
        {
            if (!ModelState.IsValid) return View(medicine);

            _context.Add(medicine);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Edit GET
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // Edit POST
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MedicineId,Name,Price,Quantity")] Medicine medicine)
        {
            if (id != medicine.MedicineId) return NotFound();
            if (!ModelState.IsValid) return View(medicine);

            try
            {
                _context.Update(medicine);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MedicineExists(medicine.MedicineId)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }

        // Delete GET
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var medicine = await _context.Medicines
                                         .FirstOrDefaultAsync(m => m.MedicineId == id);
            if (medicine == null) return NotFound();

            return View(medicine);
        }

        // Delete POST
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine != null) _context.Medicines.Remove(medicine);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicineExists(string id) =>
            _context.Medicines.Any(e => e.MedicineId == id);
    }
}
