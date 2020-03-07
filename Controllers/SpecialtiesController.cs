using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindUniversity;

namespace FindUniversity.Controllers
{
    public class SpecialtiesController : Controller
    {
        private readonly FindUnivContext _context;

        public SpecialtiesController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: Specialties
        public async Task<IActionResult> Index()
        {
            //var findUnivContext = _context.Specialties.Include(e => e.EducationalProg);
            return View(await _context.Specialties.ToListAsync());
        }

        // GET: Specialties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialties = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialties == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index","EducationalProgs",new {id=specialties.Id,name=specialties.Name });
        }

        // GET: Specialties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Specialties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Info")] Specialties specialties)
        {
            if (ModelState.IsValid)
            {
                _context.Add(specialties);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(specialties);
        }

        // GET: Specialties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialties = await _context.Specialties.FindAsync(id);
            if (specialties == null)
            {
                return NotFound();
            }
            return View(specialties);
        }

        // POST: Specialties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Info")] Specialties specialties)
        {
            if (id != specialties.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(specialties);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpecialtiesExists(specialties.Id))
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
            return View(specialties);
        }

        // GET: Specialties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var specialties = await _context.Specialties
                .FirstOrDefaultAsync(m => m.Id == id);
            if (specialties == null)
            {
                return NotFound();
            }

            return View(specialties);
        }

        // POST: Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialties = await _context.Specialties.FindAsync(id);
            _context.Specialties.Remove(specialties);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpecialtiesExists(int id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }
    }
}
