using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ApeGama.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class ComplaintController : ControllerBase
	{
		private readonly ApeGamaContext _context;
		public ComplaintController(ApeGamaContext context)
		{
			_context = context;
		}

		[HttpPost]
		public IActionResult FileComplaint(ComplaintModel model)
		{
			try
			{
				var temp = _context.Orders.Where(e => e.OrderId == model.OrderId).FirstOrDefault();
				var uid = HttpContext.Session.GetInt32("UID");

				model.UserId = (int)uid;
				model.ShopId = temp.ShopId;
				model.Date = System.DateTime.Today;

				_context.Add(model);
				_context.SaveChanges();

				return Ok();
			}
			catch (System.Exception)
			{
				return NotFound();
			}
		}

		[HttpGet]
		public List<ComplaintModel> GetComplaints()
        {
			return _context.Complaints.OrderBy(e => e.OrderId).ThenByDescending(e=>e.Status).ToList();
        }

		[HttpGet("{id}")]
		public ComplaintModel GetComplaint(int id)
        {
			 var model = _context.Complaints.Where(e=>e.CompId == id).Include(e=>e.User).Include(e=>e.Shop).FirstOrDefault();
			return model;

		}

		[HttpPost]
		public IActionResult BanSupplier(ComplaintModel model)
        {
            try
            {
                var user = _context.Users.Where(e => e.OnlineShop.ShopId == model.ShopId).FirstOrDefault();
                if (user == null) return NotFound();
                else
                {
                    user.UserStatus = false;
                    _context.Entry(user).State = EntityState.Modified;

					var complaits = _context.Complaints.Where(e => e.ShopId == model.ShopId && e.Status == 1).ToList();
					foreach(var item in complaits)
                    {
						item.Status = 0;
						_context.Entry(item).State = EntityState.Modified;
					}
					
                    _context.SaveChanges();
                    return Ok();
                }
            }
            catch (System.Exception)
            {
				return BadRequest();
            }
        }
	}
}
