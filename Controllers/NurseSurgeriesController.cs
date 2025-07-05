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
    public class NurseSurgeriesController : Controller
    {
        private readonly AppDbContext _context;

        public NurseSurgeriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NurseSurgeries
        public async Task<IActionResult> Index(
        string sortOrder,
        string currentFilter,
        string searchString,
        int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            // Sorting params for links
            string nurseSortParm = String.IsNullOrEmpty(sortOrder) ? "nurse_desc" : "";
            string surgerySortParm = sortOrder == "Surgery" ? "surgery_desc" : "Surgery";

            if (searchString != null)
            {
                pageNumber = 1; // reset page when searching
            }
            else
            {
                searchString = currentFilter;
            }

            // Base query including related Nurse and Surgery entities
            var nurseSurgeryQuery = _context.NurseSurgeries
                .Include(ns => ns.Nurse)
                .Include(ns => ns.Surgery)
                .AsQueryable();

            // Filtering (search)
            if (!String.IsNullOrEmpty(searchString))
            {
                nurseSurgeryQuery = nurseSurgeryQuery.Where(ns =>
                    ns.Nurse.Name.Contains(searchString) ||
                    ns.Surgery.Name.Contains(searchString));
            }

            // Sorting
            nurseSurgeryQuery = sortOrder switch
            {
                "nurse_desc" => nurseSurgeryQuery.OrderByDescending(ns => ns.Nurse.Name),
                "Surgery" => nurseSurgeryQuery.OrderBy(ns => ns.Surgery.Name),
                "surgery_desc" => nurseSurgeryQuery.OrderByDescending(ns => ns.Surgery.Name),
                _ => nurseSurgeryQuery.OrderBy(ns => ns.Nurse.Name),
            };

            int pageSize = 10; // adjust page size as needed

            var paginatedList = await PaginatedList<NurseSurgery>.CreateAsync(nurseSurgeryQuery.AsNoTracking(), pageNumber ?? 1, pageSize);

            return View(paginatedList);
        }


        // GET: NurseSurgeries/Details?nurseId=xxx&surgeryId=yyy
        public async Task<IActionResult> Details(string nurseId, string surgeryId)
        {
            if (nurseId == null || surgeryId == null)
            {
                return NotFound();
            }

            var nurseSurgery = await _context.NurseSurgeries
                .Include(n => n.Nurse)
                .Include(n => n.Surgery)
                .FirstOrDefaultAsync(m => m.NurseId == nurseId && m.SurgeryId == surgeryId);
            if (nurseSurgery == null)
            {
                return NotFound();
            }

            return View(nurseSurgery);
        }

        // GET: NurseSurgeries/Create
        public IActionResult Create()
        {
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "Name");
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId");
            return View();
        }

        // POST: NurseSurgeries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NurseId,SurgeryId")] NurseSurgery nurseSurgery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nurseSurgery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "Name", nurseSurgery.NurseId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", nurseSurgery.SurgeryId);
            return View(nurseSurgery);
        }

        // GET: NurseSurgeries/Edit?nurseId=xxx&surgeryId=yyy
        public async Task<IActionResult> Edit(string nurseId, string surgeryId)
        {
            if (nurseId == null || surgeryId == null)
            {
                return NotFound();
            }

            var nurseSurgery = await _context.NurseSurgeries
                .FirstOrDefaultAsync(ns => ns.NurseId == nurseId && ns.SurgeryId == surgeryId);

            if (nurseSurgery == null)
            {
                return NotFound();
            }

            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "Name", nurseSurgery.NurseId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", nurseSurgery.SurgeryId);
            return View(nurseSurgery);
        }

        // POST: NurseSurgeries/Edit?nurseId=xxx&surgeryId=yyy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string nurseId, string surgeryId, [Bind("NurseId,SurgeryId")] NurseSurgery nurseSurgery)
        {
            if (nurseId != nurseSurgery.NurseId || surgeryId != nurseSurgery.SurgeryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nurseSurgery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NurseSurgeryExists(nurseSurgery.NurseId, nurseSurgery.SurgeryId))
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
            ViewData["NurseId"] = new SelectList(_context.Nurses, "NurseId", "Name", nurseSurgery.NurseId);
            ViewData["SurgeryId"] = new SelectList(_context.Surgeries, "SurgeryId", "SurgeryId", nurseSurgery.SurgeryId);
            return View(nurseSurgery);
        }

        // GET: NurseSurgeries/Delete?nurseId=xxx&surgeryId=yyy
        public async Task<IActionResult> Delete(string nurseId, string surgeryId)
        {
            if (nurseId == null || surgeryId == null)
            {
                return NotFound();
            }

            var nurseSurgery = await _context.NurseSurgeries
                .Include(n => n.Nurse)
                .Include(n => n.Surgery)
                .FirstOrDefaultAsync(m => m.NurseId == nurseId && m.SurgeryId == surgeryId);

            if (nurseSurgery == null)
            {
                return NotFound();
            }

            return View(nurseSurgery);
        }

        // POST: NurseSurgeries/Delete?nurseId=xxx&surgeryId=yyy
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string nurseId, string surgeryId)
        {
            var nurseSurgery = await _context.NurseSurgeries
                .FirstOrDefaultAsync(ns => ns.NurseId == nurseId && ns.SurgeryId == surgeryId);

            if (nurseSurgery != null)
            {
                _context.NurseSurgeries.Remove(nurseSurgery);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool NurseSurgeryExists(string nurseId, string surgeryId)
        {
            return _context.NurseSurgeries.Any(e => e.NurseId == nurseId && e.SurgeryId == surgeryId);
        }
    }
}
