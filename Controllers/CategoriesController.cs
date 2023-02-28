using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BaoMoiAPI.Models;

namespace BaoMoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly BaoMoiDbContext _context;

        public CategoriesController(BaoMoiDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:5500";
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.Id }, category);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {

            Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:5500";
            Response.Headers.Append("Origin", "localhost");
            Response.Headers.Append("allowCredentials", "true");
            Response.Headers.Append("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT, DELETE");
            Response.Headers["Access-Control-Allow-Credentials"] = "true";

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }


        [HttpOptions("{id}")]
        public IActionResult Check()
        {
            //int? a = null;
            //Console.Write(a.Value);
            //logger.LogInformation("check call api api news");
            Response.Headers["access-control-allow-headers"] = "access-control-allow-origin,cors,credentials,mode";
            //Response.Headers["Access-Control-Max-Age"] = "86400";
            Response.Headers["access-control-allow-methods"] = "POST , GET , OPTIONS , DELETE";
            Response.Headers["access-control-allow-origin"] = "http://127.0.0.1:5500";
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            return Ok(new { message = "may k chay a" });
        }
    }
}
