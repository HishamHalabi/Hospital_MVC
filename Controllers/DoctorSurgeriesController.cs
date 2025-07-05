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
    public class DoctorSurgeriesController : Controller
    {
        private readonly AppDbContext _context;

        public DoctorSurgeriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: DoctorSurgeries
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var doctorSurgeries = _context.DoctorSurgeries
                .Include(ds => ds.Doctor)
                .Include(ds => ds.Surgery)
                .AsQueryable();

            if (!String.IsNullOrEmpty(searchString))
            {
                doctorSurgeries = doctorSurgeries.Where(ds =>
                    ds.Doctor.Name.Contains(searchString) ||
                    ds.Surgery.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    doctorSurgeries = doctorSurgeries.OrderByDescending(ds => ds.Doctor.Name);
                    break;
                default:
                    doctorSurgeries = doctorSurgeries.OrderBy(ds => ds.Doctor.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<DoctorSurgery>.CreateAsync(doctorSurgeries.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: DoctorSurgeries/Details?doctorId=xxx&surgeryId=yyy
        public async Task<IActionResult> Details(string doctorId, string surgeryId)
        {
            if (doctorId == null || surgeryId == null)
            {
                return NotFound();
            }

            var doctorSurgery = await _context.DoctorSurgeries
                .Include(d => d.Doctor)
                .Include(d => d.Surgery)
                .FirstOrDefaultAsync(m => m.DoctorId == doctorId && m.SurgeryId == surgeryId);

            if (doctorSurgery == null)
            {
                return NotFound();
            }

            return View(doctorSurgery);
        }

        // GET: DoctorSurgeries/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId");
            return View();
        }

        // POST: DoctorSurgeries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,SurgeryId")] DoctorSurgery doctorSurgery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(doctorSurgery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", doctorSurgery.DoctorId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", doctorSurgery.SurgeryId);
            return View(doctorSurgery);
        }

        // GET: DoctorSurgeries/Edit?doctorId=xxx&surgeryId=yyy
        public async Task<IActionResult> Edit(string doctorId, string surgeryId)
        {
            if (doctorId == null || surgeryId == null)
            {
                return NotFound();
            }

            var doctorSurgery = await _context.DoctorSurgeries.FindAsync(doctorId, surgeryId);
            if (doctorSurgery == null)
            {
                return NotFound();
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", doctorSurgery.DoctorId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", doctorSurgery.SurgeryId);
            return View(doctorSurgery);
        }

        // POST: DoctorSurgeries/Edit?doctorId=xxx&surgeryId=yyy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string doctorId, string surgeryId, [Bind("DoctorId,SurgeryId")] DoctorSurgery doctorSurgery)
        {
            if (doctorId != doctorSurgery.DoctorId || surgeryId != doctorSurgery.SurgeryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(doctorSurgery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorSurgeryExists(doctorSurgery.DoctorId, doctorSurgery.SurgeryId))
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
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", doctorSurgery.DoctorId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", doctorSurgery.SurgeryId);
            return View(doctorSurgery);
        }

        // GET: DoctorSurgeries/Delete?doctorId=xxx&surgeryId=yyy
        public async Task<IActionResult> Delete(string doctorId, string surgeryId)
        {
            if (doctorId == null || surgeryId == null)
            {
                return NotFound();
            }

            var doctorSurgery = await _context.DoctorSurgeries
                .Include(d => d.Doctor)
                .Include(d => d.Surgery)
                .FirstOrDefaultAsync(m => m.DoctorId == doctorId && m.SurgeryId == surgeryId);

            if (doctorSurgery == null)
            {
                return NotFound();
            }

            return View(doctorSurgery);
        }

        // POST: DoctorSurgeries/Delete?doctorId=xxx&surgeryId=yyy
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string doctorId, string surgeryId)
        {
            var doctorSurgery = await _context.DoctorSurgeries.FindAsync(doctorId, surgeryId);
            if (doctorSurgery != null)
            {
                _context.DoctorSurgeries.Remove(doctorSurgery);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorSurgeryExists(string doctorId, string surgeryId)
        {
            return _context.DoctorSurgeries.Any(e => e.DoctorId == doctorId && e.SurgeryId == surgeryId);
        }
    }
}
