using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital;
using Hospital.Models;
using Hospital.Helpers;  // Don't forget this for PaginatedList


namespace Hospital.Controllers
{
    public class PatientsController : Controller
    {
        /*
         * Dependency Injection (DI) is a design pattern used to make code more modular, testable, and maintainable. Instead of creating objects manually using new, you inject them where needed.
          Without DI:csharp

            var context = new AppDbContext(); // tightly coupled, hard to test
         * 
         *
       */

        private readonly AppDbContext _context;

        public PatientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Patients
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            ViewData["CurrentSort"] = sortOrder;  /*it still for single request*/
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var patients = from p in _context.Patients.Include(p => p.Room)  
                           select p;

            if (!string.IsNullOrEmpty(searchString))
            {
                patients = patients.Where(p => p.FullName.Contains(searchString));
            }

            // Sorting only by name
            patients = sortOrder switch
            {
                "name_desc" => patients.OrderByDescending(p => p.FullName),
                _ => patients.OrderBy(p => p.FullName)
            };
            int pageSize =5;
            return View(await PaginatedList<Patient>.CreateAsync(patients.AsNoTracking(), pageIndex ?? 1, pageSize));
        }

        // GET: Patients/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
              .Include(p => p.Room)
              .Include(p => p.Appointments)
                    .ThenInclude(a => a.Doctor)
                         .ThenInclude(d => d.Department)
              .Include(p => p.Appointments)
              .Include(p => p.Prescriptions)
                     .ThenInclude(pr => pr.Doctor)
                     .ThenInclude(d => d.Department)
            .Include(p => p.Prescriptions)
                    .ThenInclude(pr => pr.Appointment)
            .Include(p => p.Bills)
            .Include(p => p.Surgeries)
  
           .Include(p => p.Surgeries)
                .ThenInclude(s => s.Room)
                  .FirstOrDefaultAsync(p => p.PatientId == id);

            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        [Authorize(Roles = "Admin")]
        // GET: Patients/Create
        public IActionResult Create()
        {
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId"); 
            return View();
        }

        // POST: Patients/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PatientId,FullName,Gender,Dob,Age,PatientPhone,PatientHistory,RoomId")] Patient patient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(patient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", patient.RoomId /*defualt value*/);
            return View(patient);
        }

        // GET: Patients/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", patient.RoomId); //viewBag.roomif
            return View(patient);
        }/*
           * 
           *  Concurrency Handling
csharp

    catch (DbUpdateConcurrencyException)
    {
        if (!PatientExists(patient.PatientId))
        {
            return NotFound();
        }
        else
        {
            throw;
        }
    }
Catches concurrency exceptions (e.g., another user modified or deleted the record at the same time).

Checks if the patient still exists:

If not, return 404 Not Found.

Otherwise, re-throw the exception.
           * */

        // POST: Patients/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("PatientId,FullName,Gender,Dob,Age,PatientPhone,PatientHistory,RoomId")] Patient patient)
        {
            if (id != patient.PatientId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(patient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PatientExists(patient.PatientId))
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
            ViewData["RoomId"] = new SelectList(_context.Rooms, "RoomId", "RoomId", patient.RoomId);
            return View(patient);
        }

        // GET: Patients/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var patient = await _context.Patients
                .Include(p => p.Room)
                .FirstOrDefaultAsync(m => m.PatientId == id);
            if (patient == null)
            {
                return NotFound();
            }

            return View(patient);
        }

        // POST: Patients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PatientExists(string id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
    }
}
