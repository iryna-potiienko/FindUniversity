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
                                Specialties newcat;
                                var c = (from cat in _context.Specialties
                                         where cat.Name.Contains(worksheet.Name)
                                         select cat).ToList();
                                if (c.Count > 0)
                                {
                                    newcat = c[0];
                                }
                                else
                                {
                                    newcat = new Specialties();
                                    newcat.Name = worksheet.Name;
                                    newcat.Info = "from EXCEL";
                                    //додати в контекст
                                    _context.Specialties.Add(newcat);
                                }
                                //перегляд усіх рядків                    
                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    // try
                                    //  {
                                    Universities university = new Universities();
                                     Faculties faculties;

                                        EducationalProg educationalProg = new EducationalProg();
                                        educationalProg.Name = row.Cell(1).Value.ToString();
                                        //educationalProg.Info = row.Cell(6).Value.ToString();
                                        educationalProg.Specialties = newcat;
                                        _context.EducationalProg.Add(educationalProg);
                                        //у разі наявності автора знайти його, у разі відсутності - додати
                                        for (int i = 2; i <= 10; i=i+2)
                                        {
                                            if (row.Cell(i).Value.ToString().Length > 0)
                                            {
                                               // Faculties faculties;

                                                var a = (from fac in _context.Faculties
                                                         where fac.Name.Contains(row.Cell(i).Value.ToString())
                                                         select fac).ToList();
                                                if (a.Count > 0)
                                                     {
                                                        faculties = a[0];
                                                     }
                                                else
                                                {
                                                    for (int j = 3; j <= 10; j = j + 2)
                                                    {
                                                        if (row.Cell(j).Value.ToString().Length > 0)
                                                        {   
                                                        //Universities universities;

                                                            var u = (from univ in _context.Universities
                                                                 where univ.Name.Contains(row.Cell(j).Value.ToString())
                                                                 select univ).ToList();
                                                            if (u.Count > 0)
                                                            {
                                                                university = u[0];
                                                            }
                                                            else
                                                            {
                                                            //universities = new Universities();
                                                                university.Name = row.Cell(j).Value.ToString();
                                                                //university.Faculties.Add(faculties);
                                                        //universities.Info = "from EXCEL";
                                                        //додати в контекст
                                                                _context.Universities.Add(university);
                                                            }
                                                        }
                                                       // faculties.Name = row.Cell(i).Value.ToString();
                                                       /// faculties.University = university;
                                                        //додати в контекст
                                                      //  _context.Add(faculties);
                                                    }
                                               // }
                                                faculties = new Faculties();
                                                faculties.Name = row.Cell(i).Value.ToString();
                                                //faculties.Info = "from EXCEL";
                                                //university.Faculties.Add(faculties);
                                                faculties.University = university;
                                                //додати в контекст
                                                _context.Faculties.Add(faculties);
                                                }
                                                FacultyEducationalProg fe = new FacultyEducationalProg();
                                                fe.EducationalProg = educationalProg;
                                                fe.Faculty = faculties;
                                                _context.FacultyEducationalProg.Add(fe);
                                            }
                                        }
                                   // }
                                   // catch (Exception e)
                                    //{
                                        //logging самостійно :)

                                  //  }
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
                    var worksheet = workbook.Worksheets.Add(c.Name.Substring(0,3));

                    worksheet.Cell("A1").Value = "Назва освітньої програми";
                    worksheet.Cell("B1").Value = "Факультет 1";
                    worksheet.Cell("C1").Value = "Університет 1";
                    worksheet.Cell("D1").Value = "Факультет 2";
                    worksheet.Cell("E1").Value = "Університет 2";
                    worksheet.Cell("F1").Value = "Спеціальність";
                    worksheet.Cell("G1").Value = "Інформація про спеціальність";
                    worksheet.Row(1).Style.Font.Bold = true;
                    var educationalProgs = c.EducationalProg.ToList();

                    //нумерація рядків/стовпчиків починається з індекса 1 (не 0)
                    for (int i = 0; i < educationalProgs.Count; i++)
                    {
                        worksheet.Cell(i + 2, 1).Value = educationalProgs[i].Name;
                        worksheet.Cell(i + 2, 6).Value = educationalProgs[i].Specialties.Name;
                       // worksheet.Cell(i+2,4).Value=educationalProgs[i].FacultyEducationalProg.Faculty
                        worksheet.Cell(i + 2, 7).Value = educationalProgs[i].Specialties.Info;

                        var fe = _context.FacultyEducationalProg.Where(a => a.EducationalProgId == educationalProgs[i].Id).Include("Faculty").ToList();
                        //більше 4-ох нікуди писати
                        int j = 0;
                        foreach (var f in fe)
                        {
                            if (j < 6)
                            {
                                worksheet.Cell(i + 2, j + 2).Value = f.Faculty.Name;
                                //worksheet.Cell(i + 2, 4).Value = f.Faculty.University.Name;
                                //       j++;
                                //    }
                                var faculties = _context.Faculties.Where(c => c.Id == f.FacultyId).Include("University").ToList();
                                foreach (var fac in faculties)
                                {
                                    worksheet.Cell(i + 2, j+3).Value = fac.University.Name;
                                }
                                j=j+2;
                            }
                        }

                        //var univ = _context.FacultyEducationalProg.Where(a => a.FacultyId == educationalProgs[i].Id).Include("Faculty").ToList();
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


        //Vika Kharchenko, [26.03.20 21:07]
/*public async Task<IActionResult> Import1(IFormFile fileExcel)
        {
            string ErrorMes;
            if (ModelState.IsValid)
            {
                if (fileExcel != null)
                {
                    using (var stream = new FileStream(fileExcel.FileName, FileMode.Create))
                    {
                        await fileExcel.CopyToAsync(stream);
                        using (XLWorkbook workBook = new XLWorkbook(stream, XLEventTracking.Disabled))
                        {
                            foreach (IXLWorksheet worksheet in workBook.Worksheets)
                            {
                                Categories newcat;
                                var c = (from cat in _context.Categories
                                         where cat.Category.Equals(worksheet.Name)
                                         select cat).ToList();
                                if (c.Count > 0)
                                {
                                    newcat = c[0];
                                }
                                else
                                {
                                    newcat = new Categories();
                                    newcat.Category = worksheet.Name;
                                    _context.Categories.Add(newcat);
                                    await _context.SaveChangesAsync();
                                }

                                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                                {
                                    try
                                    {
                                        Documents doc = new Documents();
                                        doc.Type = new Types();
                                        doc.Broker = new Brokers();
                                        doc.Client = new Clients();
                                        try
                                        {
                                            doc.Number = row.Cell(1).Value.ToString();
                                            if (doc.Number.Length != 10)
                                            {
                                                throw new Exception("Невірно вказаний номер документа в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку");
                                            }
                                            for (int i = 0; i < 10; i++)
                                            {
                                                if (doc.Number[i] < '0' || doc.Number[i] > '9') { throw new Exception("Невірно вказаний номер документа в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку"); }
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Невірно вказаний номер документа в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку");
                                        }

                                        doc.Type.Type = row.Cell(2).Value.ToString();

                                        var t = (from ty in _context.Types
                                                 where ty.Type.Equals(row.Cell(2).Value.ToString())
                                                 select ty).ToList();
                                        if (t.Count > 0)
                                        {
                                            doc.Type = t[0];
                                        }
                                        else
                                        {
                                            var cat = (from ca in _context.Categories
                                                       where ca.Category.Equals(newcat.Category)


                                                        select ca).ToList();
                                            doc.Type.Category = cat[0];
                                            _context.Types.Add(doc.Type);
                                            await _context.SaveChangesAsync();
                                            //throw new Exception("Невірий тип договору в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку");

                                        }

                                        //doc.Broker.FullName = row.Cell(3).Value.ToString();
                                        char[] param = { ' ' };
                                        try
                                        {
                                            string name = row.Cell(3).Value.ToString().Split(param)[0];
                                            string surname = row.Cell(3).Value.ToString().Split(param)[1];
                                            var a = (from br in _context.Brokers
                                                     where br.Name.Equals(name) && br.Surname.Equals(surname)
                                                     select br).ToList();
                                            if (a.Count > 0)
                                            {
                                                doc.Broker = a[0];
                                            }
                                            else
                                            {
                                                throw new Exception("Невівне ім'я брокера в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку" +
                                                    "Такого брокера немає в базі");
                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Невівне ім'я брокера в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку" +
                                                   "Такого брокера немає в базі");
                                        }


                                        // doc.Client.FullName = row.Cell(4).Value.ToString();
                                        try
                                        {
                                            string name = row.Cell(4).Value.ToString().Split(param)[0];
                                            string surname = row.Cell(4).Value.ToString().Split(param)[1];

                                            var cli = (from clnt in _context.Clients
                                                       where clnt.Name.Equals(name) && clnt.Surname.Equals(surname)
                                                       select clnt).ToList();
                                            if (cli.Count > 0)
                                            {
                                                doc.Client = cli[0];
                                            }
                                            else
                                            {
                                                throw new Exception("Невірне ім'я клієнта в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку." +
                                                    " Такого клієнта немає в базі");

                                            }
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Невірне ім'я клієнта в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку." +
                                                    " Такого клієнта немає в базі");
                                        }

                                        
try
                                        {
                                            doc.Date = Convert.ToDateTime(row.Cell(5).Value);
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Невірно вказана дата в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку");
                                        }
                                        try
                                        {
                                            doc.Sum = Convert.ToDecimal(row.Cell(6).Value);
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception("Невірно вказана ціна в категорії " + worksheet.Name + " в " + row.RowNumber().ToString() + " рядку");
                                        }

                                        var d = (from dd in _context.Documents
                                                 where dd.Number.Equals(doc.Number)
                                                 select dd).ToList();

                                        if (d.Count == 0)
                                        {
                                            _context.Documents.Add(doc);
                                        }
                                        else
                                        {
                                            _context.Documents.Find(d[0].Id).Type = doc.Type;
                                            _context.Documents.Find(d[0].Id).Broker = doc.Broker;
                                            _context.Documents.Find(d[0].Id).Client = doc.Client;
                                            _context.Documents.Find(d[0].Id).Date = doc.Date;
                                            _context.Documents.Find(d[0].Id).Sum = doc.Sum;
                                        }



                                    }
                                    catch (Exception e)
                                    {
                                        ViewBag.ErrorMes = e.Message;
                                        return RedirectToAction("Index", "Documents", new { brokerId = -1, clientId = -1, error = e.Message });
                                    }

                                }


                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }*/
    }
}
