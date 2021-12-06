using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ApeGamaContext _context;

        public ReviewController(ApeGamaContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public List<ReviewModel> GetReviews(int id)
        {
            var reviews = _context.Reviews.Where(e=>e.ProdId == id).Include(e=>e.User).ToList();
            return reviews;
        }

        [HttpPost]
        public async Task<IActionResult> PostReview(ReviewModel model)
        {
			try
			{
				var userId = HttpContext.Session.GetInt32("UID");
				if (userId > 0)
				{
					model.UserId = (int)userId;
					if (_context.Reviews.Where(e => e.UserId == userId && e.ProdId == model.ProdId).AsNoTracking().FirstOrDefault() == null)
					{
						_context.Add(model);
					}
					else
					{
						_context.Entry(model).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
					}
				}
				else
				{
                    return NotFound();
				}
			}
			catch (System.Exception)
			{
                return BadRequest();
			}

            return Ok();
        }

        [HttpGet("{id}")]
        public ReviewModel GetReview(int id)
		{
            var uid = HttpContext.Session.GetInt32("UID");
            var model = _context.Reviews.Where(e => e.ProdId == id && e.UserId == uid).FirstOrDefault();
            
            if (model == null)
			{
                model= new ReviewModel();
			}

            return model;
		}
    }
}
