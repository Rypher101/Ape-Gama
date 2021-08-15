using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

                orders = _context.Orders.Where(e => e.CusId == uid).Include(e => e.Shop).ToList();
                orders = orders.OrderBy(e => e.OrderStatus).ToList();

                return orders;
            }
            catch (Exception)
            {
                return orders;
            }
        }

        [HttpGet("{id}")]
        public async Task<OrderModel> GetOrder(int id)
        {
            var order = await _context.Orders
                .Where(e=>e.OrderId == id)
                .Include(e=>e.Shop)
                .Include(e => e.OrderProducts)
                .ThenInclude(e => e.Prod)
                .FirstOrDefaultAsync();

            return order;
        }

        [HttpGet("{id}")]
        public async Task<OrderModel> MarkAsReceived(int id)
        {
            try
            {
                var temp = await _context.Orders
                .Where(e => e.OrderId == id)
                .Include(e => e.Shop)
                .Include(e => e.OrderProducts)
                .ThenInclude(e => e.Prod)
                .FirstOrDefaultAsync();

                if (temp != null)
                {
                    temp.OrderStatus = 3;
                    _context.Attach(temp);
                    _context.Entry(temp).Property(e => e.OrderStatus).IsModified = true;
                    _context.SaveChanges();
                }
                else
                {
                    return null;
                }

                return temp;  
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
