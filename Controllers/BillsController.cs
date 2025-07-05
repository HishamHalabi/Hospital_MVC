using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital;
using Hospital.Models;

namespace Hospital.Controllers
{
    public class BillsController : Controller
    {
        private readonly AppDbContext _context;

        public BillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bills
        public async Task<IActionResult> Index(string sortOrder, string searchString)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PatientSortParm"] = String.IsNullOrEmpty(sortOrder) ? "patient_desc" : "";
            ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";
            ViewData["CurrentFilter"] = searchString;

            var bills = _context.Bills.Include(b => b.Patient).AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                bills = bills.Where(b => b.Patient.FullName.Contains(searchString));
            }

            bills = sortOrder switch
            {
                "patient_desc" => bills.OrderByDescending(b => b.Patient.FullName),
                "Date" => bills.OrderBy(b => b.Date),
                "date_desc" => bills.OrderByDescending(b => b.Date),
                _ => bills.OrderBy(b => b.Patient.FullName),
            };

            return View(await bills.ToListAsync());
        }

        // GET: Bills/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var bill = await _context.Bills
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(m => m.BillId == id);

            if (bill == null) return NotFound();

            return View(bill);
        }

        // GET: Bills/Create
        public IActionResult Create()
        {
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            return View();
        }

        // POST: Bills/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BillId,PatientId,RoomCharges,DoctorCharges,MedicineCharges,Date")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                // If date is not manually set in form, set it to current time
                if (bill.Date == DateTime.MinValue)
                    bill.Date = DateTime.Now;

                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", bill.PatientId);
            return View(bill);
        }

        // GET: Bills/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var bill = await _context.Bills.FindAsync(id);
            if (bill == null) return NotFound();

            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", bill.PatientId);
            return View(bill);
        }

        // POST: Bills/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BillId,PatientId,RoomCharges,DoctorCharges,MedicineCharges,Date")] Bill bill)
        {
            if (id != bill.BillId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bill);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BillExists(bill.BillId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", bill.PatientId);
            return View(bill);
        }

        // GET: Bills/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var bill = await _context.Bills
                .Include(b => b.Patient)
                .FirstOrDefaultAsync(m => m.BillId == id);

            if (bill == null) return NotFound();

            return View(bill);
        }

        // POST: Bills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var bill = await _context.Bills.FindAsync(id);
            if (bill != null)
                _context.Bills.Remove(bill);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(string id)
        {
            return _context.Bills.Any(e => e.BillId == id);
        }
    }
}
