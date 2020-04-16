using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using ClosedXML.Attributes;
using ClosedXML.Utils;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FindUniversity.Controllers
{
   // [Authorize(Roles ="admin,user")]
    public class CountriesController : Controller
    {
        private readonly FindUnivContext _context;

        public CountriesController(FindUnivContext context)
        {
            _context = context;
        }

        // GET: Countries
        public async Task<IActionResult> Index()
        {
            return View(await _context.Countries.ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }

            return RedirectToAction("Index","Universities",new { id=countries.Id,name=countries.Name});
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Countries countries)
        {
            if (ModelState.IsValid)
            {
                _context.Add(countries);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(countries);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries.FindAsync(id);
            if (countries == null)
            {
                return NotFound();
            }
            return View(countries);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Countries countries)
        {
            if (id != countries.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(countries);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountriesExists(countries.Id))
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
            return View(countries);
        }

       // [Authorize(Roles ="admin")]
        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id, string? error)
        {
            ViewBag.ErrorMes = error;
            //string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            //return Content($"ваша роль: {role}");
            if (id == null)
            {
                return NotFound();
            }

            var countries = await _context.Countries
                .FirstOrDefaultAsync(m => m.Id == id);
            if (countries == null)
            {
                return NotFound();
            }

            return View(countries);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
       // [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                string role = "user";
              /*  if (ClaimsIdentity.DefaultRoleClaimType != null)
                {
                    role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;

                    if (role != "admin")
                    {
                        //throw new Exception("Видалити може лише адміністратор!");
                        return Content($"ваша роль: {role} Видалити може лише адміністратор!");
                    }
                }*/
                var countries = await _context.Countries.FindAsync(id);
                if (_context.Universities.Where(b => b.CountryId == id).Count() != 0)
                    throw new Exception("Ця країна містить університети!");
                _context.Countries.Remove(countries);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.ErrorMes = e.Message;
                return RedirectToAction("Delete", "Countries", new { error = e.Message });
            }
        }

        private bool CountriesExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        public IActionResult Validation(string? name, int? id)
        {
            var countries = _context.Countries.Where(s => s.Name == name).Where(s => s.Id != id);
            if (countries.Count() > 0)
            {
                return Json(data: "Така країна вже є в базі");
            }
            return Json(data: true);
        }


    }
}
