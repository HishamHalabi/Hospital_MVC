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
    public class SurgeriesController : Controller
    {
        private readonly AppDbContext _context;

        public SurgeriesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Surgeries
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["PatientSortParm"] = sortOrder == "patient" ? "patient_desc" : "patient";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var surgeries = _context.Surgeries
                .Include(s => s.Patient)
                .Include(s => s.Room)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                surgeries = surgeries.Where(s =>
                    s.Patient.FullName.Contains(searchString) ||
                    s.Type.Contains(searchString));
            }

            surgeries = sortOrder switch
            {
                "date_desc" => surgeries.OrderByDescending(s => s.Date),
                "patient" => surgeries.OrderBy(s => s.Patient.FullName),
                "patient_desc" => surgeries.OrderByDescending(s => s.Patient.FullName),
                _ => surgeries.OrderBy(s => s.Date),
            };

            int pageSize = 5;
            return View(await PaginatedList<Surgery>.CreateAsync(surgeries.AsNoTracking(), page ?? 1, pageSize));
        }


        // GET: Surgeries/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgery = await _context.Surgeries
                
                .Include(s => s.Patient)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.SurgeryId == id);
            if (surgery == null)
            {
                return NotFound();
            }

            return View(surgery);
        }

        // GET: Surgeries/Create
        public IActionResult Create()
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId");
            return View();
        }

        // POST: Surgeries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name ,  Description , SurgeryId,DoctorId,PatientId,Date,Time,Type,RoomId")] Surgery surgery)
        {
            if (ModelState.IsValid)
            {
                _context.Add(surgery);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
          
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "PatientId", surgery.PatientId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", surgery.RoomId);
            return View(surgery);
        }

        // GET: Surgeries/Edit/5   
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgery = await _context.Surgeries.FindAsync(id);
            if (surgery == null)
            {
                return NotFound();
            }

       
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", surgery.PatientId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", surgery.RoomId);

            return View(surgery);
        }


        // POST: Surgeries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Name ,  Description , SurgeryId,DoctorId,PatientId,Date,Time,Type,RoomId")] Surgery surgery)
        {
            if (id != surgery.SurgeryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(surgery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SurgeryExists(surgery.SurgeryId))
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
           
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", surgery.PatientId);
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", surgery.RoomId);
            return View(surgery);
        }

        // GET: Surgeries/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var surgery = await _context.Surgeries
               
                .Include(s => s.Patient)
                .Include(s => s.Room)
                .FirstOrDefaultAsync(m => m.SurgeryId == id);
            if (surgery == null)
            {
                return NotFound();
            }

            return View(surgery);
        }

        // POST: Surgeries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var surgery = await _context.Surgeries.FindAsync(id);
            if (surgery != null)
            {
                _context.Surgeries.Remove(surgery);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SurgeryExists(string id)
        {
            return _context.Surgeries.Any(e => e.SurgeryId == id);
        }
    }
}
