using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital;
using Hospital.Models;
using Hospital.Helpers;

namespace Hospital.Controllers
{
    public class ScansController : Controller
    {
        private readonly AppDbContext _context;

        public ScansController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Scans
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var scans = _context.Scans
                .Include(s => s.Patient)
                .Include(s => s.Doctor)
                .Include(s => s.Prescription)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                scans = scans.Where(s => s.Patient.FullName.Contains(searchString));
            }

            scans = sortOrder switch
            {
                "date_desc" => scans.OrderByDescending(s => s.ScanDate),
                "name" => scans.OrderBy(s => s.Patient.FullName),
                "name_desc" => scans.OrderByDescending(s => s.Patient.FullName),
                _ => scans.OrderBy(s => s.ScanDate),
            };

            int pageSize = 10;
            return View(await PaginatedList<Scan>.CreateAsync(scans.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: Scans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scan = await _context.Scans
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Prescription)
                .FirstOrDefaultAsync(m => m.ScanId == id);
            if (scan == null)
            {
                return NotFound();
            }

            return View(scan);
        }

        // GET: Scans/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            ViewData["PrescriptionId"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId");
            return View();
        }

        // POST: Scans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScanId,Type,ScanDate,ResultDescription,PatientId,DoctorId,PrescriptionId")] Scan scan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(scan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", scan.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", scan.PatientId);
            ViewData["PrescriptionId"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId", scan.PrescriptionId);
            return View(scan);
        }

        // GET: Scans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scan = await _context.Scans.FindAsync(id);
            if (scan == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", scan.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", scan.PatientId);
            ViewData["PrescriptionId"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId", scan.PrescriptionId);
            return View(scan);
        }

        // POST: Scans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ScanId,Type,ScanDate,ResultDescription,PatientId,DoctorId,PrescriptionId")] Scan scan)
        {
            if (id != scan.ScanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(scan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScanExists(scan.ScanId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "DoctorId", scan.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", scan.PatientId);
            ViewData["PrescriptionId"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId", scan.PrescriptionId);
            return View(scan);
        }

        // GET: Scans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var scan = await _context.Scans
                .Include(s => s.Doctor)
                .Include(s => s.Patient)
                .Include(s => s.Prescription)
                .FirstOrDefaultAsync(m => m.ScanId == id);
            if (scan == null)
            {
                return NotFound();
            }

            return View(scan);
        }

        // POST: Scans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var scan = await _context.Scans.FindAsync(id);
            if (scan != null)
            {
                _context.Scans.Remove(scan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScanExists(int id)
        {
            return _context.Scans.Any(e => e.ScanId == id);
        }
    }
}
