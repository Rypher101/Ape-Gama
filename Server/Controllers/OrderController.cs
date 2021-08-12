using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApeGamaContext _context;
        public OrderController(ApeGamaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public List<OrderModel> GetOrders()
        {
            var orders = new List<OrderModel>(); 
            try
            {
                var uid = HttpContext.Session.GetInt32("UID");
                if (uid == null || uid == 0)
                    return orders;

                orders = _context.Orders.Where(e=>e.CusId == uid).Include(e=>e.Shop).OrderBy(e=>e.OrderStatus).OrderByDescending(e=>e.OrderDate).ToList();

                return orders;
            }
            catch (Exception)
            {
                return orders;
            }               
        }
    }
}
