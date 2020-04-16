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
    public class EducationalProgsController : Controller
    {
        private readonly FindUnivContext _context;

        public EducationalProgsController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: EducationalProgs
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", "Specialties");
            ViewBag.SpecialtiesId = id;
            ViewBag.SpecialtiesName = _context.Specialties.Find(id).Name;

            //var findUnivContext = _context.EducationalProg.Include(e => e.Specialties);
            var eduProgsBySpecialties = _context.EducationalProg.Where(b => b.SpecialtiesId == id).Include(b => b.Specialties);
            return View(await eduProgsBySpecialties.ToListAsync());
        }

        // GET: EducationalProgs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var educationalProg = await _context.EducationalProg
                .Include(e => e.Specialties)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (educationalProg == null)
            {
                return NotFound();
            }

            return View(educationalProg);
           // return RedirectToAction("Index", "FacultyEducationalProgs", new { id = educationalProg.Id, name = educationalProg.Name });
            
        }

        // GET: EducationalProgs/Create
        public IActionResult Create(int? specialtiesId)
        {
            //ViewData["SpecialtiesId"] = new SelectList(_context.Specialties, "Id", "Name");
            ViewBag.SpecialtiesId = specialtiesId;
            ViewBag.SpecialtiesName = _context.Specialties.Where(c => c.Id == specialtiesId).FirstOrDefault().Name;
            return View();
        }

        // POST: EducationalProgs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int specialtiesId, [Bind("Id,Name,SpecialtiesId,Price")] EducationalProg educationalProg)
        {
            educationalProg.SpecialtiesId = specialtiesId;
            if (ModelState.IsValid)
            {
                _context.Add(educationalProg);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "EducationalProgs", new { id = specialtiesId, name = _context.Specialties.Where(c => c.Id == specialtiesId).FirstOrDefault().Name });
            }
            //ViewData["SpecialtiesId"] = new SelectList(_context.Specialties, "Id", "Name", educationalProg.SpecialtiesId);
            //return View(educationalProg);
            return RedirectToAction("Index", "EducationalProgs", new { id = specialtiesId, name = _context.Specialties.Where(c => c.Id == specialtiesId).FirstOrDefault().Name });
        }

        // GET: EducationalProgs/Edit/5
        public async Task<IActionResult> Edit(int? id, int? specialtiesId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var educationalProg = await _context.EducationalProg.FindAsync(id);
            if (educationalProg == null)
            {
                return NotFound();
            }

            ViewBag.SpecialtiesId = specialtiesId;
            ViewData["SpecialtiesId"] = new SelectList(_context.Specialties, "Id", "Name", educationalProg.SpecialtiesId);
            return View(educationalProg);
        }

        // POST: EducationalProgs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int? specialtiesId, [Bind("Id,Name,SpecialtiesId,Price")] EducationalProg educationalProg)
        {
            if (id != educationalProg.Id)
            {
                return NotFound();
            }

            ViewBag.SpecialtiesId = specialtiesId;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(educationalProg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EducationalProgExists(educationalProg.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index","EducationalProgs",new { id=specialtiesId});
            }
            ViewData["SpecialtiesId"] = new SelectList(_context.Specialties, "Id", "Name", educationalProg.SpecialtiesId);
            //return View(educationalProg);
            return RedirectToAction("Index", "EducationalProgs", new { id = specialtiesId });
        }

        // GET: EducationalProgs/Delete/5
        public async Task<IActionResult> Delete(int? specialtiesId, int? id, string? error)
        {
            ViewBag.ErrorMes = error;
            if (id == null)
            {
                return NotFound();
            }

            var educationalProg = await _context.EducationalProg
                .Include(e => e.Specialties)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.SpecialtiesId = specialtiesId;
            if (educationalProg == null)
            {
                return NotFound();
            }

            return View(educationalProg);
        }

        // POST: EducationalProgs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int specialtiesId, int id)
        {
            
            try
            {
                string error=null;
                ViewBag.SpecialtiesId = specialtiesId;
                var educationalProg = await _context.EducationalProg.FindAsync(id);
                var facEdPr = _context.FacultyEducationalProg.Where(b => b.EducationalProgId == id);
                if (facEdPr.Count() != 0)
                {
                    
                    // if (specialties.EducationalProg.Count() != 0) 
                    foreach(var fac in facEdPr)
                    {
                        var faculties = _context.Faculties.Where(f => f.Id == fac.FacultyId);
                        foreach (var fa in faculties)
                        {
                            error += fa.Name+"; ";
                        }
                       
                    }
                    throw new Exception("Освітня програма викладається на таких факультетах: \n"+error);
                }
                _context.EducationalProg.Remove(educationalProg);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "EducationalProgs", new { id = specialtiesId });
            }
            catch(Exception e)
            {
                ViewBag.ErrorMes = e.Message;
                return RedirectToAction("Delete", "EducationalProgs", new { error = e.Message });
            }
        }

        private bool EducationalProgExists(int id)
        {
            return _context.EducationalProg.Any(e => e.Id == id);
        }

        public IActionResult Validation(string? name, int? id)
        {
            var educationalProgs = _context.EducationalProg.Where(s => s.Name == name).Where(s => s.Id != id);
            if (educationalProgs.Count() > 0)
            {
                return Json(data: "Така освітня програма вже є в базі");
            }
            return Json(data: true);
        }
    }
}
