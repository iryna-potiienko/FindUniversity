using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FindUniversity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartSpecialtyController : ControllerBase
    {
        private readonly FindUnivContext _context;
        public ChartSpecialtyController(FindUnivContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var specialties = _context.Specialties.Include(b => b.EducationalProg).ToList();
            List<object> catBook = new List<object>();

            catBook.Add(new[] { "Спеціальність", "Кількість освітніх програм" });

            foreach (var c in specialties)
            {
                catBook.Add(new object[] { c.Name, c.EducationalProg.Count() });
            }
            return new JsonResult(catBook);
        }
    }
}