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
    public class FacultiesController : Controller
    {
        private readonly FindUnivContext _context;

        public FacultiesController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: Faculties
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var facultyWithUniversity = _context.Faculties.Include(v => v.University);
                return View(await facultyWithUniversity.ToListAsync());
            }
            else
            {
                ViewBag.UniversityId = id;
                ViewBag.UniversityName = _context.Universities.Find(id).Name;

                //var findUnivContext = _context.Faculties.Include(f => f.University);
                var facultiesByUniversities = _context.Faculties.Where(f => f.UniversityId == id).Include(f => f.University);
                return View(await facultiesByUniversities.ToListAsync());
            }
        }

        // GET: Faculties/Details/5
        public async Task<IActionResult> Details(int? id, int? universityId)
        {
            ViewBag.UniversityId = universityId;
            if (id == null)
            {
                return NotFound();
            }

            var faculties = await _context.Faculties
                .Include(f => f.University)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (faculties == null)
            {
                return NotFound();
            }

            //return View(faculties);
            return RedirectToAction("Index", "FacultyEducationalProgs", new { id = faculties.Id, name = faculties.Name });
        }

        // GET: Faculties/Create
        public IActionResult Create(int? universityId)
        {
            if (universityId == null) return RedirectToAction("Index", "Universities");
            ViewBag.UniversityId = universityId;
            ViewBag.UniversityName = _context.Universities.Where(w => w.Id == universityId).FirstOrDefault().Name;
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Name");
            return View();
        }

        // POST: Faculties/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int universityId, [Bind("Id,Name,Info,UniversityId")] Faculties faculties)
        {
            faculties.UniversityId = universityId;
            if (ModelState.IsValid)
            {
                _context.Add(faculties);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Faculties", new { id = universityId, name = _context.Universities.Where(x => x.Id == universityId).FirstOrDefault().Name });
            }
            //ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Name", faculties.UniversityId);
            return RedirectToAction("Index", "Faculties", new { id = universityId, name = _context.Universities.Where(x => x.Id == universityId).FirstOrDefault().Name });
        }

        // GET: Faculties/Edit/5
        public async Task<IActionResult> Edit(int? id, int? universityId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculties = await _context.Faculties.FindAsync(id);
            if (faculties == null)
            {
                return NotFound();
            }
            ViewBag.UniversityId = universityId;
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Name", faculties.UniversityId);
            return View(faculties);
        }

        // POST: Faculties/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int universityId, [Bind("Id,Name,Info,UniversityId")] Faculties faculties)
        {
            if (id != faculties.Id)
            {
                return NotFound();
            }
            ViewBag.UniversityId = universityId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faculties);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultiesExists(faculties.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Faculties", new { id = universityId });
            }
            ViewData["UniversityId"] = new SelectList(_context.Universities, "Id", "Name", faculties.UniversityId);
            //return View(faculties);
            return RedirectToAction("Index", "Faculties", new { id = universityId });
        }

        // GET: Faculties/Delete/5
        public async Task<IActionResult> Delete(int? universityId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faculties = await _context.Faculties
                .Include(f => f.University)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.UniversityId = universityId;
            if (faculties == null)
            {
                return NotFound();
            }

            return View(faculties);
        }

        // POST: Faculties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int universityId, int id)
        {
            ViewBag.UniversityId = universityId;
            var faculties = await _context.Faculties.FindAsync(id);
            _context.Faculties.Remove(faculties);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Faculties", new { id = universityId });
        }

        private bool FacultiesExists(int id)
        {
            return _context.Faculties.Any(e => e.Id == id);
        }
    }
}
