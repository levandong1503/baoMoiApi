using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using BaoMoiAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cors;
using Microsoft.Extensions.Logging;

namespace BaoMoiAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly BaoMoiDbContext _context;
        private readonly ILogger<AccountsController> logger;
        public AccountsController(BaoMoiDbContext context, ILogger<AccountsController> logger)
        {
            this.logger = logger;
            _context = context;
        }

        // GET: api/Accounts
        [HttpGet("notuse")]
        
        public IActionResult GetAccounts()
        {
            Response.Headers["Access-Control-Allow-Origin"] = "*";
            Response.Headers["infor"] = "le van dong";
            return Ok(_context.Accounts.ToList());
        }

        // GET: api/Accounts//login
        [HttpPost("login")]
        
        public IActionResult GetAccount(Account account)
        {
            logger.LogInformation("mot tai khoan dang dang nhap: " + account.Username);
            var acc =  _context.Accounts.Where(a => a.Username == account.Username && a.Password == account.Password).FirstOrDefault();
            Response.Headers["Access-Control-Allow-Origin"] = "http://127.0.0.1:5500";
            Response.Headers.Append("Origin", "localhost");
            Response.Headers.Append("allowCredentials", "true");
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            Response.Headers.Append("Access-Control-Allow-Methods", "POST, GET, OPTIONS, PUT");
            if (acc == null)
            {
                return BadRequest(new { message = "khong tim thay du lieu" });
            }


            CookieOptions cookieOptions = new CookieOptions()
            {
                SameSite = SameSiteMode.None,
                Secure = true,
                //Expires =  new DateTimeOffset(DateTime.Now),
                //MaxAge = TimeSpan.FromMinutes(30),
            };


            Response.Cookies.Append("username", acc.Username,cookieOptions);

            return Ok(new { message = "Dang nhap thanh cong" });
        }

        // PUT: api/Accounts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("suadoi/{id}")]
        public async Task<IActionResult> PutAccount(int id, Account account)
        {
            if (id != account.Id)
            {
                return BadRequest();
            }

            _context.Entry(account).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccountExists(id))
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

        // POST: api/Accounts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Account>> PostAccount(Account account)
        {
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccount", new { id = account.Id }, account);
        }

        // DELETE: api/Accounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }

            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.Id == id);
        }


        [HttpOptions("login")]
        public IActionResult Check()
        {

            //logger.LogInformation("check call api login");
            //int? a = null;
            //Console.Write(a.Value);
            Response.Headers["access-control-allow-headers"] = "access-control-allow-origin,content-type,credentials";
            //Response.Headers["Access-Control-Max-Age"] = "86400";
            Response.Headers["access-control-allow-methods"] = "POST , GET , OPTIONS";
            Response.Headers["access-control-allow-origin"] = "http://127.0.0.1:5500";
            
            Response.Headers["Access-Control-Allow-Credentials"] = "true";
            Response.Headers["allowcredentials"] = "true";
            return NoContent();
        }
    }
}
