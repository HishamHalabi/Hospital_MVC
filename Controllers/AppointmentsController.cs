using System;
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
    public class AppointmentsController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index(string sortOrder, string searchString, string currentFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["PatientSortParam"] = String.IsNullOrEmpty(sortOrder) ? "patient_desc" : "";
            ViewData["DoctorSortParam"] = sortOrder == "doctor" ? "doctor_desc" : "doctor";
            ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Doctor)
                // Room removed here as requested
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                appointments = appointments.Where(a =>
                    a.Patient.FullName.Contains(searchString) ||
                    a.Doctor.Name.Contains(searchString));
            }

            appointments = sortOrder switch
            {
                "patient_desc" => appointments.OrderByDescending(a => a.Patient.FullName),
                "doctor" => appointments.OrderBy(a => a.Doctor.Name),
                "doctor_desc" => appointments.OrderByDescending(a => a.Doctor.Name),
                "date" => appointments.OrderBy(a => a.Date),
                "date_desc" => appointments.OrderByDescending(a => a.Date),
                _ => appointments.OrderBy(a => a.Patient.FullName),
            };

            int pageSize = 10;
            return View(await PaginatedList<Appointment>.CreateAsync(appointments.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                    .ThenInclude(d => d.Department)
                .Include(a => a.Patient)
                .Include(a => a.Prescription)
                    .ThenInclude(p => p.Doctor)
                        .ThenInclude(d => d.Department)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,PatientId,DoctorId,Date,Time")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Appointment created successfully!";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(appointment);
            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            PopulateDropdowns(appointment);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("AppointmentId,PatientId,DoctorId,Date,Time")] Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Appointment updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.AppointmentId))
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
            PopulateDropdowns(appointment);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(m => m.AppointmentId == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Appointment deleted successfully!";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper: Check if appointment exists by Id
        private bool AppointmentExists(string id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }

        // Helper: Populate dropdown lists for Doctors, Patients (Room removed)
        private void PopulateDropdowns(Appointment appointment = null)
        {
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", appointment?.DoctorId);
            ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", appointment?.PatientId);
            // Room dropdown removed
        }
    }
}
