using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApeGamaContext _context;
        public AdminController(ApeGamaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public AdminDashboard GetDashboardData()
        {
            var model = new AdminDashboard();

            model.cusCount = _context.Users.Where(e => e.UserFlag == 1 && e.UserStatus == true).Count();
            model.supCount = _context.Users.Where(e => e.UserFlag == 2 && e.UserStatus == true).Count();
            model.odrMade = _context.Orders.Where(e => e.OrderDate.Year == DateTime.Now.Year && e.OrderDate.Month == DateTime.Now.Month).Count();
            model.odrComplete = _context.Orders.Where(e => e.OrderStatus == 3 && e.OrderDate.Year == DateTime.Now.Year && e.OrderDate.Month == DateTime.Now.Month).Count();

            return model;
        }

    }
}
