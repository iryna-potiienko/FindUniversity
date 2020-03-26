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

namespace FindUniversity.Controllers
{
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

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(countries);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var countries = await _context.Countries.FindAsync(id);
            _context.Countries.Remove(countries);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountriesExists(int id)
        {
            return _context.Countries.Any(e => e.Id == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Import(IFormFile fileExcel)
        {
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
                                Countries newcat;
                                var c = (from cat in _context.Countries
                                         where cat.Name.Contains(worksheet.Name)
                                         select cat).ToList();
                                if (c.Count > 0)
                                {
                                    newcat = c[0];
                                }
                                else
                                {
                                    newcat = new Countries();
                                    newcat.Name = worksheet.Name;
                                    //newcat.Info = "from EXCEL";
                                    //додати в контекст
                                    _context.Countries.Add(newcat);
                                }
                                //перегляд усіх рядків                    
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Universities university = new Universities();
                                        university.Name = row.Cell(1).Value.ToString();
                                        //university.Info = row.Cell(6).Value.ToString();
                                        university.Country = newcat;
                                        _context.Universities.Add(university);
                                        //у разі наявності автора знайти його, у разі відсутності - додати
                                        for (int i = 2; i <= 5; i++)
                                        {
                                            if (row.Cell(i).Value.ToString().Length > 0)
                                            {
                                                Faculties faculty;

                                                var a = (from fac in _context.Faculties
                                                         where fac.Name.Contains(row.Cell(i).Value.ToString())
                                                         select fac).ToList();
                                                if (a.Count > 0)
                                                {
                                                    faculty = a[0];
                                                }
                                                else
                                                {
                                                    faculty = new Faculties();
                                                    faculty.Name = row.Cell(i).Value.ToString();
                                                    faculty.Info = "from EXCEL";
                                                    //додати в контекст
                                                    _context.Add(faculty);
                                                }
                                              /*  AuthorsBooks ab = new AuthorsBooks();
                                                ab.Book = book;
                                                ab.Author = author;
                                                _context.AuthorsBooks.Add(ab);*/
                                            }
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        //logging самостійно :)

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

   /*     public ActionResult Export()
        {
            using (XLWorkbook workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                var counries = _context.Countries.Include("Universities").ToList();
                //тут, для прикладу ми пишемо усі книжки з БД, в своїх проектах ТАК НЕ РОБИТИ (писати лише вибрані)
                foreach (var c in counries)
                {
                    var worksheet = workbook.Worksheets.Add(c.Name);

                    worksheet.Cell("A1").Value = "Назва";
                    worksheet.Cell("B1").Value = "Автор 1";
                    worksheet.Cell("C1").Value = "Автор 2";
                    worksheet.Cell("D1").Value = "Автор 3";
                    worksheet.Cell("E1").Value = "Автор 4";
                    worksheet.Cell("F1").Value = "Категорія";
                    worksheet.Cell("G1").Value = "Інформація";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var universities = c.Universities.ToList();

                    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                    for (int i = 0; i < universities.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = universities[i].Name;
                       // worksheet.Cell(i + 2, 7).Value = universities[i].Info;

                     /*   var ab = _context.AuthorsBooks.Where(a => a.BookId == books[i].Id).Include("Author").ToList();
                        //більше 4-ох нікуди писати
                        int j = 0;
                        foreach (var a in ab)
                        {
                            if (j < 5)
                            {
                                worksheet.Cell(i + 2, j + 2).Value = a.Author.Name;
                                j++;
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
        }*/


    }
}
