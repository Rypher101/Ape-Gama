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
    [Route("api/[controller]")]
    [ApiController]
    public class OnlineShopController : ControllerBase
    {
        private readonly ApeGamaContext _context;

        public OnlineShopController(ApeGamaContext context)
        {
            _context = context;
        }

        // GET: api/OnlineShop
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OnlineShopModel>>> GetOnlineShops()
        {
            var shops = await _context.OnlineShops
                .Include("Sup")
                .ToListAsync();
            return shops;
        }

        // GET: api/OnlineShop/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OnlineShopModel>> GetOnlineShopModel(int id)
        {
            var onlineShopModel = await _context.OnlineShops.FindAsync(id);

            if (onlineShopModel == null)
            {
                return NotFound();
            }

            return onlineShopModel;
        }

        // PUT: api/OnlineShop/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOnlineShopModel(int id, OnlineShopModel onlineShopModel)
        {
            if (id != onlineShopModel.ShopId)
            {
                return BadRequest();
            }

            _context.Entry(onlineShopModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OnlineShopModelExists(id))
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

        // POST: api/OnlineShop
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OnlineShopModel>> PostOnlineShopModel(OnlineShopModel onlineShopModel)
        {
            try
            {
                if (HttpContext.Session.TryGetValue("UID", out byte[] _))
                {
                    onlineShopModel.SupId = (int)HttpContext.Session.GetInt32("UID");
                }
                else
                {
                    return Unauthorized();
                }

                _context.OnlineShops.Add(onlineShopModel);
                await _context.SaveChangesAsync();

                var newOnlineShop = await _context.OnlineShops.FirstOrDefaultAsync(x => x.SupId == onlineShopModel.SupId);
                HttpContext.Session.SetInt32("ShopID", newOnlineShop.ShopId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // DELETE: api/OnlineShop/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOnlineShopModel(int id)
        {
            var onlineShopModel = await _context.OnlineShops.FindAsync(id);
            if (onlineShopModel == null)
            {
                return NotFound();
            }

            _context.OnlineShops.Remove(onlineShopModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OnlineShopModelExists(int id)
        {
            return _context.OnlineShops.Any(e => e.ShopId == id);
        }
    }
}
