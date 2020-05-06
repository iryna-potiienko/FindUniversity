using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FindUniversity;
using Microsoft.AspNetCore.Http;
using ClosedXML.Excel;
using System.IO;

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
        public async Task<IActionResult> Index(string error)
        {
            ViewBag.ErrorMes = error;
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
        public async Task<IActionResult> Delete(int? id, string error)
        {
            ViewBag.ErrorMes = error;
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
            try
            {
                var specialties = await _context.Specialties.FindAsync(id);
                if (_context.EducationalProg.Where(b => b.SpecialtiesId == id).Count() != 0)
                    throw new Exception("Ця спеціальність містить освітні програми!");
                _context.Specialties.Remove(specialties);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                ViewBag.ErrorMes = e.Message;
                return RedirectToAction("Delete", "Specialties", new { error = e.Message });
            }
        }

        private bool SpecialtiesExists(int id)
        {
            return _context.Specialties.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
            //string ErrorMes, err;
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            //перегляд усіх листів (в даному випадку категорій)
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                //worksheet.Name - назва категорії. Пробуємо знайти в БД, якщо відсутня, то створюємо нову
                                Specialties newcat;
                                if (worksheet.Name.Count() > 31) worksheet.Name = worksheet.Name.Substring(0, 30);

                                var c = (from cat in _context.Specialties
                                         where cat.Name == worksheet.Name +" "+ worksheet.Cell(2, 8).Value.ToString()
                                         select cat).ToList();
                                if (c.Count > 0)
                                {
                                    newcat = c[0];
                                }
                                else
                                {
                                    newcat = new Specialties();
                                    newcat.Name = worksheet.Name.Substring(0,3) + " " + worksheet.Cell(2,8).Value.ToString();
                                    newcat.Info = "from EXCEL";
                                    //додати в контекст
                                    _context.Specialties.Add(newcat);
                                }
                                //перегляд усіх рядків                    
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    Countries country = new Countries();
                                    Universities university = new Universities();
                                    Faculties faculties=new Faculties();
                                    EducationalProg educationalProg = new EducationalProg();

                                        var edu = (from ed in _context.EducationalProg
                                                 where ed.Name == row.Cell(1).Value.ToString()
                                                 select ed).ToList();
                                        if (edu.Count > 0)
                                        {
                                            educationalProg = edu[0];
                                        }
                                        else
                                        {
                                            educationalProg = new EducationalProg();
                                            educationalProg.Name = row.Cell(1).Value.ToString();
                                            //educationalProg.Info = row.Cell(6).Value.ToString();
                                            educationalProg.Specialties = newcat;
                                            _context.EducationalProg.Add(educationalProg);
                                        }
                                    //у разі наявності факультету знайти його, у разі відсутності - додати
                                    for (int i = 2; i <= 5; i = i + 3)
                                    {
                                        if (row.Cell(i).Value.ToString().Length > 0)
                                        {
                                            try
                                            {
                                                if (row.Cell(i + 2).Value.ToString().Length > 0)
                                                {


                                                    var co = (from coun in _context.Countries
                                                              where coun.Name.Contains(row.Cell(i + 2).Value.ToString())
                                                              select coun).ToList();
                                                    if (co.Count > 0)
                                                    {
                                                        country = co[0];
                                                    }
                                                    else
                                                    {
                                                        country = new Countries();
                                                        country.Name = row.Cell(i + 2).Value.ToString();
                                                        //додати в контекст
                                                        _context.Countries.Add(country);
                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception(" Країну не вірно вказано у спеціальності " + worksheet.Name + "у рядку " + row.RowNumber());
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ViewBag.ErrorMes = e.Message;
                                                return RedirectToAction("Index", "Specialties", new { error = e.Message });
                                            }
                                            try
                                            {
                                                if (row.Cell(i + 1).Value.ToString().Length > 0)
                                                {


                                                    var u = (from univ in _context.Universities
                                                             where univ.Country.Name.Contains(country.Name) && univ.Name==row.Cell(i+1).Value.ToString()
                                                             select univ).ToList();
                                                    if (u.Count > 0)
                                                    {
                                                        university = u[0];
                                                    }
                                                    else
                                                    {
                                                        university = new Universities();
                                                        university.Name = row.Cell(i + 1).Value.ToString();
                                                        university.Country = country;
                                                        //додати в контекст
                                                        _context.Universities.Add(university);

                                                    }
                                                }
                                                else
                                                {
                                                    throw new Exception("Університет не вказано у спеціальності " + worksheet.Name + " у рядку " + row.RowNumber().ToString());
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                ViewBag.ErrorMes = e.Message;
                                                return RedirectToAction("Index", "Specialties", new { error = e.Message });
                                            }
                                            var a = (from fac in _context.Faculties
                                                     where fac.University.Name == university.Name && fac.University.Country.Name.Contains(country.Name) && fac.Name == row.Cell(i).Value.ToString()
                                                     select fac).ToList();
                                            if (a.Count > 0)
                                            {
                                                faculties = a[0];
                                            }
                                            else
                                            {
                                                faculties = new Faculties();
                                                faculties.Name = row.Cell(i).Value.ToString();
                                                faculties.University = university;
                                                //додати в контекст
                                                _context.Faculties.Add(faculties);
                                            }
                                       
                                                FacultyEducationalProg fe;
                                                fe = new FacultyEducationalProg();
                                                fe.EducationalProg = educationalProg;
                                                fe.Faculty = faculties;
                                                _context.FacultyEducationalProg.Add(fe);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var specialties = _context.Specialties.Include("EducationalProg").ToList();
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проектах ТАК НЕ РОБИТИ (писати лише вибрані)
                foreach (var c in specialties)
                {
                    IXLWorksheet worksheet;
                    try
                    {
                        worksheet = workbook.Worksheets.Add(c.Name.Substring(0, 3));
                    }
                    catch (ArgumentException)
                    {
                        int length;
                        if (c.Name.Length > 32)
                            length = 31;
                        else
                            length = c.Name.Length;
                        worksheet = workbook.Worksheets.Add(c.Name.Substring(0, length));
                    }

                        worksheet.Cell("A1").Value = "Назва освітньої програми";
                        worksheet.Cell("B1").Value = "Факультет 1";
                        worksheet.Cell("C1").Value = "Університет 1";
                        worksheet.Cell("D1").Value = "Країна 1";
                        worksheet.Cell("E1").Value = "Факультет 2";
                        worksheet.Cell("F1").Value = "Університет 2";
                        worksheet.Cell("G1").Value = "Країна 2";
                        worksheet.Cell("H1").Value = "Спеціальність(лише назва без індексу)";
                        worksheet.Cell("I1").Value = "Інформація про спеціальність";
                        worksheet.Row(1).Style.Font.Bold = true;
                        var educationalProgs = c.EducationalProg.ToList();

                        //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                        for (int i = 0; i < educationalProgs.Count; i++)
                        {
                            worksheet.Cell(i + 2, 1).Value = educationalProgs[i].Name;
                            worksheet.Cell(i + 2, 8).Value = educationalProgs[i].Specialties.Name.Substring(4);
                            // worksheet.Cell(i+2,4).Value=educationalProgs[i].FacultyEducationalProg.Faculty
                            worksheet.Cell(i + 2, 9).Value = educationalProgs[i].Specialties.Info;

                            var fe = _context.FacultyEducationalProg.Where(a => a.EducationalProgId == educationalProgs[i].Id).Include("Faculty").ToList();
                            //більше 4-ох нікуди писати
                            int j = 0;
                            foreach (var f in fe)
                            {
                                if (j < 10)
                                {
                                    worksheet.Cell(i + 2, j + 2).Value = f.Faculty.Name;
                                    var faculties = _context.Faculties.Where(c => c.Id == f.FacultyId).Include("University").ToList();
                                    foreach (var fac in faculties)
                                    {
                                        worksheet.Cell(i + 2, j + 3).Value = fac.University.Name;

                                        var universities = _context.Universities.Where(d => d.Id == fac.UniversityId).Include("Country").ToList();
                                        foreach (var univ in universities)
                                        {
                                            worksheet.Cell(i + 2, j + 4).Value = univ.Country.Name;
                                        }
                                    }
                                    j = j + 3;
                                }
                            }

                        }
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Flush();

                    return new FileContentResult(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    {
                        FileDownloadName = $"library_{DateTime.UtcNow.ToShortDateString()}.xlsx"
                    };
                }
            }
        }

        public IActionResult Validation(string name, int? id)
        {
            int result;
            string numb = name.Substring(0, 3);
            if (!Int32.TryParse(numb, out result)) return Json(data: "Спеціальність має бути вказана з індексом");
            var sp = _context.Specialties.Where(s => s.Name == name).Where(s => s.Id != id);
            if (sp.Count() > 0)
            {
                return Json(data: "Така спеціальність вже є в базі");
            }
            return Json(data: true);
        }
    }
}
