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
    public class ChartFacultyController : ControllerBase
    {
        private readonly FindUnivContext _context;
        public ChartFacultyController(FindUnivContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var universities = _context.Universities.Include(b => b.Faculties).ToList();
            List<object> catBook = new List<object>();

            catBook.Add(new[] { "Університет", "Кількість факультетів" });

            foreach (var c in universities)
            {
                catBook.Add(new object[] { c.Name, c.Faculties.Count() });
            }
            return new JsonResult(catBook);
        }
    }
}