using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Hospital.Models;
using Hospital.Helpers;
using Hospital;

public class PrescriptionsController : Controller
{
    private readonly AppDbContext _context;

    public PrescriptionsController(AppDbContext context)
    {
        _context = context;
    }

    // GET: Prescriptions
    public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
        ViewData["PatientSortParm"] = sortOrder == "patient" ? "patient_desc" : "patient";
        ViewData["DoctorSortParm"] = sortOrder == "doctor" ? "doctor_desc" : "doctor";

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var prescriptions = _context.prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Appointment)
            .AsQueryable();

        if (!String.IsNullOrEmpty(searchString))
        {
            prescriptions = prescriptions.Where(p =>
                p.Patient.FullName.Contains(searchString) ||
                p.Doctor.Name.Contains(searchString));
        }

        prescriptions = sortOrder switch
        {
            "date_desc" => prescriptions.OrderByDescending(p => p.Date),
            "patient" => prescriptions.OrderBy(p => p.Patient.FullName),
            "patient_desc" => prescriptions.OrderByDescending(p => p.Patient.FullName),
            "doctor" => prescriptions.OrderBy(p => p.Doctor.Name),
            "doctor_desc" => prescriptions.OrderByDescending(p => p.Doctor.Name),
            _ => prescriptions.OrderBy(p => p.Date),
        };

        int pageSize = 10;
        return View(await PaginatedList<Prescription>.CreateAsync(prescriptions.AsNoTracking(), pageNumber ?? 1, pageSize));
    }


    // GET: Prescriptions/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var prescription = await _context.prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Appointment)
            .Include(p => p.PreMeds)
                .ThenInclude(pm => pm.Medicine)
            .Include(p => p.Scans)  // Include related scans here
            .FirstOrDefaultAsync(m => m.prescriptionId == id);

        if (prescription == null)
        {
            return NotFound();
        }

        return View(prescription);
    }

    // GET: Prescriptions/Create
    public IActionResult Create()
    {
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name");
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName");
        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId");
        return View();
    }

    // POST: Prescriptions/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("prescriptionId,DiagonsisDescription,Date,DoctorId,PatientId,AppointmentId")] Prescription prescription)
    {
        if (ModelState.IsValid)
        {
            _context.Add(prescription);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", prescription.DoctorId);
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", prescription.PatientId);
        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", prescription.AppointmentId);
        return View(prescription);
    }

    // GET: Prescriptions/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var prescription = await _context.prescriptions.FindAsync(id);
        if (prescription == null)
        {
            return NotFound();
        }
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", prescription.DoctorId);
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", prescription.PatientId);
        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", prescription.AppointmentId);
        return View(prescription);
    }

    // POST: Prescriptions/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, [Bind("prescriptionId,DiagonsisDescription,Date,DoctorId,PatientId,AppointmentId")] Prescription prescription)
    {
        if (id != prescription.prescriptionId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(prescription);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PrescriptionExists(prescription.prescriptionId))
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
        ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "Name", prescription.DoctorId);
        ViewData["PatientId"] = new SelectList(_context.Patients, "PatientId", "FullName", prescription.PatientId);
        ViewData["AppointmentId"] = new SelectList(_context.Appointments, "AppointmentId", "AppointmentId", prescription.AppointmentId);
        return View(prescription);
    }

    // GET: Prescriptions/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        var prescription = await _context.prescriptions
            .Include(p => p.Patient)
            .Include(p => p.Doctor)
            .Include(p => p.Appointment)
            .FirstOrDefaultAsync(m => m.prescriptionId == id);
        if (prescription == null)
        {
            return NotFound();
        }

        return View(prescription);
    }

    // POST: Prescriptions/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var prescription = await _context.prescriptions.FindAsync(id);
        if (prescription != null)
        {
            _context.prescriptions.Remove(prescription);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool PrescriptionExists(string id)
    {
        return _context.prescriptions.Any(e => e.prescriptionId == id);
    }
}
