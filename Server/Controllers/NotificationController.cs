using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace ApeGama.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class NotificationController : ControllerBase
	{
		private readonly ApeGamaContext _context;

		public NotificationController(ApeGamaContext context)
		{
			_context = context;
		}

		[HttpGet]
		public List<NotificationModel> GetNotofications(int id)
		{
			try
			{
				int uid = (int)HttpContext.Session.GetInt32("UID");

				return _context.Notifications.Where(e => e.UserId == uid).ToList();
			}
			catch (System.Exception)
			{
				return new List<NotificationModel>();
			}
		}

		[HttpPost]
		public NotificationModel PostNotification(int id)
		{
			try
			{
				var uid = (int)HttpContext.Session.GetInt32("UID");
				var temp = _context.Notifications.Where(e => e.UserId == uid && e.Category == id).FirstOrDefault();
				if (temp != null)
				{
					_context.Notifications.Remove(temp);
					_context.SaveChanges();
				}
			}
			catch (System.Exception)
			{

				throw;
			}
			return null;
		}

		[HttpGet("{id}")]
		public NotificationModel RemoveNotification(int id)
		{
			try
			{
				var uid = (int)HttpContext.Session.GetInt32("UID");
				var temp = _context.Notifications.Where(e => e.UserId == uid && e.Category == id).FirstOrDefault();
				if (temp != null)
				{
					_context.Notifications.Remove(temp);
					_context.SaveChanges();
				}
			}
			catch (System.Exception)
			{

				throw;
			}
			return new NotificationModel();
		}

	}
}
