﻿using System;
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
    public class NursesController : Controller
    {
        private readonly AppDbContext _context;

        public NursesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Nurses
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Nurses.Include(n => n.Department);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Nurses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var nurse = await _context.Nurses
                    .Include(n => n.Department)
                    .Include(n => n.NurseSurgeries)
                        .ThenInclude(ns => ns.Surgery)
                            .ThenInclude(s => s.Patient)
                    .Include(n => n.NurseSurgeries)
                        .ThenInclude(ns => ns.Surgery)
                            .ThenInclude(s => s.Room)
                    .FirstOrDefaultAsync(m => m.NurseId == id);

            if (nurse == null)
            {
                return NotFound();
            }

            return View(nurse);
        }

        // GET: Nurses/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId");
            return View();
        }

        // POST: Nurses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NurseId,Name,Gender,Address,DepartmentId")] Nurse nurse)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nurse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", nurse.DepartmentId);
            return View(nurse);
        }

        // GET: Nurses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", nurse.DepartmentId);
            return View(nurse);
        }

        // POST: Nurses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("NurseId,Name,Gender,Address,DepartmentId")] Nurse nurse)
        {
            if (id != nurse.NurseId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nurse);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NurseExists(nurse.NurseId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "DepartmentId", nurse.DepartmentId);
            return View(nurse);
        }

        // GET: Nurses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nurse = await _context.Nurses
                .Include(n => n.Department)
                .FirstOrDefaultAsync(m => m.NurseId == id);
            if (nurse == null)
            {
                return NotFound();
            }

            return View(nurse);
        }

        // POST: Nurses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nurse = await _context.Nurses.FindAsync(id);
            if (nurse != null)
            {
                _context.Nurses.Remove(nurse);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NurseExists(string id)
        {
            return _context.Nurses.Any(e => e.NurseId == id);
        }
    }
}
