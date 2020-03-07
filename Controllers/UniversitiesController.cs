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
    public class UniversitiesController : Controller
    {
        private readonly FindUnivContext _context;

        public UniversitiesController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: Universities
        public async Task<IActionResult> Index(int? id)
        {
            //var findUnivContext = _context.Universities.Include(u => u.Country);
            if (id == null)
            {
                var countryWithUniversity = _context.Universities.Include(v => v.Country);
                return View(await countryWithUniversity.ToListAsync());
            }
            else
            {
                ViewBag.CountryId = id;
                ViewBag.CountryName = _context.Countries.Find(id).Name;
                var universitiesByCountries = _context.Universities.Where(u => u.CountryId == id).Include(u => u.Country);

                return View(await universitiesByCountries.ToListAsync());
            }
        }

        // GET: Universities/Details/5
        public async Task<IActionResult> Details(int? id, int? countryId)
        {
            ViewBag.CountryId = countryId;
            if (id == null)
            {
                return NotFound();
            }

            var universities = await _context.Universities
                .Include(u => u.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (universities == null)
            {
                return NotFound();
            }

            //return View(universities);
            return RedirectToAction("Index", "Faculties", new { id = universities.Id, name = universities.Name });
        }

        // GET: Universities/Create
        public IActionResult Create(int? countryId)
        {
            if (countryId == null) return RedirectToAction("Index", "Countries");
            //ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name");
            ViewBag.CountryId = countryId;
            ViewBag.CountryName = _context.Countries.Where(c => c.Id == countryId).FirstOrDefault().Name;

            return View();
        }

        // POST: Universities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int countryId, [Bind("Id,Name")] Universities universities)
        {
            universities.CountryId = countryId;

            if (ModelState.IsValid)
            {
                _context.Add(universities);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Universities", new { id = countryId, name = _context.Countries.Where(v => v.Id == countryId).FirstOrDefault().Name });
            }
            //ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", universities.CountryId);
           // return View(universities);
            return RedirectToAction("Index", "Universities", new { id = countryId, name = _context.Countries.Where(v => v.Id == countryId).FirstOrDefault().Name });
        }

        // GET: Universities/Edit/5
        public async Task<IActionResult> Edit(int? id, int? countryId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universities = await _context.Universities.FindAsync(id);
            if (universities == null)
            {
                return NotFound();
            }
            ViewBag.CountryId = countryId;

            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", universities.CountryId);
            return View(universities);
        }

        // POST: Universities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int countryId, [Bind("Id,Name,CountryId")] Universities universities)
        {
            if (id != universities.Id)
            {
                return NotFound();
            }
            ViewBag.CountryId = countryId;

           if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(universities);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UniversitiesExists(universities.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "Universities", new { id = countryId });
            }
            ViewData["CountryId"] = new SelectList(_context.Countries, "Id", "Name", universities.CountryId);
            //return View(universities);
            return RedirectToAction("Index", "Universities", new { id = countryId });
        }

        // GET: Universities/Delete/5
        public async Task<IActionResult> Delete(int? countryId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var universities = await _context.Universities
                .Include(u => u.Country)
                .FirstOrDefaultAsync(m => m.Id == id);
            ViewBag.CountryId = countryId;
            if (universities == null)
            {
                return NotFound();
            }

            return View(universities);
        }

        // POST: Universities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int countryId, int id)
        {
            ViewBag.CountryId = countryId;
            var universities = await _context.Universities.FindAsync(id);
            _context.Universities.Remove(universities);
            await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            return RedirectToAction("Index", "Universities", new { id = countryId });
        }

        private bool UniversitiesExists(int id)
        {
            return _context.Universities.Any(e => e.Id == id);
        }
    }
}
