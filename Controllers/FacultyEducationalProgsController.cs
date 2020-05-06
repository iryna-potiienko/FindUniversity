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
    public class FacultyEducationalProgsController : Controller
    {
        private readonly FindUnivContext _context;

        public FacultyEducationalProgsController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: FacultyEducationalProgs
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null) return RedirectToAction("Index", "Faculties");
            ViewBag.FacultyId = id;
            //ViewBag.EducationalProgName = name;
            ViewBag.FacultyName = _context.Faculties.Find(id).Name;
            //var facultyEduProgsByEduProgs = _context.FacultyEducationalProg.Where(f => f.EducationalProgId == id).Include(f => f.EducationalProg).Where(f => f.FacultyId == id).Include(f => f.Faculty);
            //var findUnivContext = _context.FacultyEducationalProg.Include(f => f.EducationalProg).Include(f => f.Faculty);
            var eduProgsInFaculties = _context.FacultyEducationalProg.Where(e => e.FacultyId == id).Include(e => e.EducationalProg);
            return View(await eduProgsInFaculties.ToListAsync());
            //return View(eduProgsInFaculties.ToList());
        }

        // GET: FacultyEducationalProgs/Details/5
        public async Task<IActionResult> Details(int? id, int? facultyId)
        {
            ViewBag.FacultyId = facultyId;
            if (id == null)
            {
                return NotFound();
            }

            var facultyEducationalProg = await _context.FacultyEducationalProg
                .Include(f => f.EducationalProg)
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (facultyEducationalProg == null)
            {
                return NotFound();
            }

              return View(facultyEducationalProg);
            //return RedirectToAction("Index", "EducationalProgs", new { id = facultyEducationalProg.Id });
        }

        // GET: FacultyEducationalProgs/Create
        public IActionResult Create(int? facultyId)
        {
            //ViewBag.EducationalProgId = educationalProgId;
            //ViewBag.EducationalProgName = _context.EducationalProg.Where(e => e.Id == educationalProgId).FirstOrDefault().Name;
            ViewBag.FacultyId = facultyId;
            //ViewBag.FacultyName = _context.Faculties.Where(f => f.Id == facultyId).FirstOrDefault().Name;

            ViewData["EducationalProgId"] = new SelectList(_context.EducationalProg, "Id", "Name");
            ViewData["FacultyId"] = new SelectList(_context.Faculties.Where(b => b.Id == facultyId), "Id", "Name");
            return View();
        }

        // POST: FacultyEducationalProgs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create (int educationalProgId, int facultyId, [Bind("Id,EducationalProgId,FacultyId")] FacultyEducationalProg facultyEducationalProg)
        {
           // facultyEducationalProg.EducationalProgId = educationalProgId;
           // facultyEducationalProg.FacultyId = facultyId;
           // if (ModelState.IsValid)
           // {
                _context.Add(facultyEducationalProg);
                await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
            // return RedirectToAction("Index", "FacultyEducationalProgs", new { id = educationalProgId, name = _context.EducationalProg.Where(h => h.Id == educationalProgId).FirstOrDefault().Name });
            // }
            //ViewData["EducationalProgId"] = new SelectList(_context.EducationalProg, "Id", "Name", facultyEducationalProg.EducationalProgId);
            //ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", facultyEducationalProg.FacultyId);
            //return View(facultyEducationalProg);
            //return RedirectToAction("Index", "FacultyEducationalProgs", new { id = educationalProgId, name = _context.EducationalProg.Where(h => h.Id == educationalProgId).FirstOrDefault().Name });
            return RedirectToAction("Index", "FacultyEducationalProgs", new { id = facultyId });
        }

        // GET: FacultyEducationalProgs/Edit/5
        public async Task<IActionResult> Edit(int? id, int? facultyId)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyEducationalProg = await _context.FacultyEducationalProg.FindAsync(id);
            if (facultyEducationalProg == null)
            {
                return NotFound();
            }

            ViewBag.FacultyId = facultyId;
            ViewData["EducationalProgId"] = new SelectList(_context.EducationalProg, "Id", "Name", facultyEducationalProg.EducationalProgId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", facultyEducationalProg.FacultyId);
            return View(facultyEducationalProg);
        }

        // POST: FacultyEducationalProgs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int facultyId, [Bind("Id,EducationalProgId,FacultyId")] FacultyEducationalProg facultyEducationalProg)
        {
            if (id != facultyEducationalProg.Id)
            {
                return NotFound();
            }
            ViewBag.FacultyId = facultyId;
            ViewBag.EducationalProgId = facultyEducationalProg.EducationalProgId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(facultyEducationalProg);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FacultyEducationalProgExists(facultyEducationalProg.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index", "FacultyEducationalProgs", new { id = facultyId });
            }
            ViewData["EducationalProgId"] = new SelectList(_context.EducationalProg, "Id", "Name", facultyEducationalProg.EducationalProgId);
            ViewData["FacultyId"] = new SelectList(_context.Faculties, "Id", "Name", facultyEducationalProg.FacultyId);
            //return View(facultyEducationalProg);
            return RedirectToAction("Index", "FacultyEducationalProgs", new { id = facultyId });
        }

        // GET: FacultyEducationalProgs/Delete/5
        public async Task<IActionResult> Delete(int? facultyId, int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var facultyEducationalProg = await _context.FacultyEducationalProg
                .Include(f => f.EducationalProg)
                .Include(f => f.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);

            ViewBag.FacultyId = facultyId;
            if (facultyEducationalProg == null)
            {
                return NotFound();
            }

            return View(facultyEducationalProg);
        }

        // POST: FacultyEducationalProgs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int facultyId, int id)
        {
            ViewBag.FacultyId = facultyId;
            var facultyEducationalProg = await _context.FacultyEducationalProg.FindAsync(id);
            _context.FacultyEducationalProg.Remove(facultyEducationalProg);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "FacultyEducationalProgs", new { id = facultyId });
        }

        private bool FacultyEducationalProgExists(int id)
        {
            return _context.FacultyEducationalProg.Any(e => e.Id == id);
        }

        public IActionResult Validation(string facultyName, int? id)
        {
            var facEducationalProgs = _context.FacultyEducationalProg.Where(s => s.Faculty.Name == facultyName).Where(s => s.EducationalProgId != id);
            if (facEducationalProgs.Count() > 0)
            {
                return Json(data: "Така освітня програма вже є на цьому факультеті");
            }
            return Json(data: true);
        }
    }
}
