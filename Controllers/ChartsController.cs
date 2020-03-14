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
    public class ChartsController : ControllerBase
    {
        private readonly FindUnivContext _context;
        public ChartsController(FindUnivContext context)
        {
            _context = context;
        }
        [HttpGet("JsonData")]
        public JsonResult JsonData()
        {
            var countries = _context.Countries.Include(b => b.Universities).ToList();
            List<object> catBook = new List<object>();

            catBook.Add(new[] { "Країна", "Кількість університетів" });

            foreach(var c in countries)
            {
                catBook.Add(new object[] { c.Name, c.Universities.Count() });
            }
            return new JsonResult(catBook);
        }
    }
}