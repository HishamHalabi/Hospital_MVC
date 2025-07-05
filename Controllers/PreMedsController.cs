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
    public class PreMedsController : Controller
    {
        private readonly AppDbContext _context;

        public PreMedsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PreMeds
        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CurrentFilter"] = searchString;

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            IQueryable<PreMed> preMeds = _context.PreMeds.Include(p => p.Medicine);

            if (!string.IsNullOrEmpty(searchString))
            {
                preMeds = preMeds.Where(p => p.Medicine.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    preMeds = preMeds.OrderByDescending(p => p.Medicine.Name);
                    break;
                default:
                    preMeds = preMeds.OrderBy(p => p.Medicine.Name);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<PreMed>.CreateAsync(preMeds.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        // GET: PreMeds/Details?preID=...&medID=...
        public async Task<IActionResult> Details(string preID, string medID)
        {
            if (preID == null || medID == null)
                return NotFound();

            var preMed = await _context.PreMeds
                .Include(pm=>pm.Medicine)
                .Include(pm=>pm.Prescription)
                .FirstOrDefaultAsync(m => m.PreID == preID && m.MedID == medID);

            if (preMed == null)
                return NotFound();

            return View(preMed);
        }
        // GET: PreMeds/Create
        public IActionResult Create()
        {
            ViewData["MedID"] = new SelectList(_context.Medicines, "MedicineId", "Name");
            ViewData["PreID"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId");
            return View();
        }

        // POST: PreMeds/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PreID,MedID,Dsoage,Quantity,NTimes")] PreMed preMed)
        {
            if (ModelState.IsValid)
            {
                _context.Add(preMed);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["MedID"] = new SelectList(_context.Medicines, "MedicineId", "Name", preMed.MedID);
            ViewData["PreID"] = new SelectList(_context.prescriptions, "prescriptionId", "prescriptionId", preMed.PreID);
            return View(preMed);
        }


        // GET: PreMeds/Edit?preID=...&medID=...
        public async Task<IActionResult> Edit(string preID, string medID)
        {
            if (preID == null || medID == null)
                return NotFound();

            var preMed = await _context.PreMeds
                .FirstOrDefaultAsync(m => m.PreID == preID && m.MedID == medID);

            if (preMed == null)
                return NotFound();

            ViewData["MedID"] = new SelectList(_context.Medicines, "MedID", "Name", preMed.MedID);
            ViewData["PreID"] = new SelectList(_context.prescriptions, "PreID", "PreID", preMed.PreID);
            return View(preMed);
        }

        // POST: PreMeds/Edit?preID=...&medID=...
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string preID, string medID, [Bind("PreID,MedID,Dsoage,Quantity,NTimes")] PreMed preMed)
        {
            if (preID != preMed.PreID || medID != preMed.MedID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(preMed);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreMedExists(preMed.PreID, preMed.MedID))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MedID"] = new SelectList(_context.Medicines, "MedID", "Name", preMed.MedID);
            ViewData["PreID"] = new SelectList(_context.prescriptions, "PreID", "PreID", preMed.PreID);
            return View(preMed);
        }

        // GET: PreMeds/Delete?preID=...&medID=...
        public async Task<IActionResult> Delete(string preID, string medID)
        {
            if (preID == null || medID == null)
                return NotFound();

            var preMed = await _context.PreMeds
                .FirstOrDefaultAsync(m => m.PreID == preID && m.MedID == medID);

            if (preMed == null)
                return NotFound();

            return View(preMed);
        }

        // POST: PreMeds/DeleteConfirmed?preID=...&medID=...
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string preID, string medID)
        {
            var preMed = await _context.PreMeds
                .FirstOrDefaultAsync(m => m.PreID == preID && m.MedID == medID);

            if (preMed != null)
            {
                _context.PreMeds.Remove(preMed);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PreMedExists(string preID, string medID)
        {
            return _context.PreMeds.Any(e => e.PreID == preID && e.MedID == medID);
        }
    }
}
