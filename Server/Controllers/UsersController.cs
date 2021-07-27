using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApeGamaContext _context;

        public UsersController(ApeGamaContext context)
        {
            _context = context;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserModel>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<UserModel>> GetUser()
        {
            UserModel user = new();
            try
            {
                if (HttpContext.Session.GetInt32("UID") != null)
                {
                    user = await _context.Users.FindAsync(HttpContext.Session.GetInt32("UID"));
                    if (!string.IsNullOrWhiteSpace(user.UserPass))
                        user.UserPass = "";
                    return user;
                }
                else
                {
                    return user;

                }
            }
            catch (Exception)
            {
                return user;
            }
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutUser(UserModel user)
        {
            var entity = _context.Users.Attach(user);
            entity.Property(x => x.UserName).IsModified = true;
            entity.Property(x => x.UserTp).IsModified = true;
            entity.Property(x => x.UserEmail).IsModified = true;

            if (user.passwordChange)
            {
                var temp = await _context.Users.FirstOrDefaultAsync(e => e.UserId == user.UserId);

                if (string.Equals(temp.UserPass, user.OldUserPass))
                    entity.Property(x => x.UserPass).IsModified = true;
                else
                    return BadRequest();
            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            return Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<IActionResult> PostUser(UserModel user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                if (ex.ToString().Contains("UNIQUE KEY"))
                {
                    return Conflict();
                }
                else
                {
                    return NotFound();
                }
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
