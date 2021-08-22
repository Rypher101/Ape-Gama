using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
                orders = orders.OrderBy(e => e.OrderStatus).ThenByDescending(e=>e.OrderId).ToList();

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
                .Where(e => e.OrderId == id)
                .Include(e => e.Shop)
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

        [HttpPost]
        public IActionResult ConfirmOrder(ConfirmOrderModel model)
        {
            try
            {
                var cartDetails =new List<CartModel>();
                var filterCart = new List<CartModel>();
                var sessionCart = HttpContext.Session.GetString("Cart");
                var cusID = HttpContext.Session.GetInt32("UID");
                var order = new OrderModel()
                {
                    ShopId = model.shopID,
                    CusId = (int)cusID,
                    OrderDate = DateTime.Now.Date,
                    OrderStatus = 1,
                    OrderAddress = model.address,
                    OrderContact = model.contact
                    
                };

                _context.Orders.Add(order);
                _context.SaveChanges();

                int oID = order.OrderId;

                if (!string.IsNullOrEmpty(sessionCart))
                    cartDetails = JsonConvert.DeserializeObject<List<CartModel>>(sessionCart);
                else
                    return BadRequest();

                filterCart = cartDetails.Where(e => e.shopID == model.shopID).ToList();

                foreach (var item in filterCart)
                {
                    var tempProd = _context.Products.Find(item.prodID);
                    var temp = new OrderProductModel()
                    {
                        OrderId = oID,
                        ProdId = item.prodID,
                        Qty = item.qty,
                        UnitPrice = tempProd.ProdPrice
                    };
                    
                    _context.OrderProducts.Add(temp);
                    cartDetails.Remove(item);
                }

                _context.SaveChanges();

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartDetails));
                return Ok(oID);
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
        }

        [HttpGet]
        public List<OrderModel> GetOrdersSupplier()
        {
            try
            {
                var shopID = HttpContext.Session.GetInt32("ShopID");
                if (shopID == 0)
                    return null;
                
                var modelList = _context.Orders.Where(e=>e.ShopId == shopID).OrderBy(e=>e.OrderStatus).ThenByDescending(e=>e.OrderId).Include(e=>e.Cus).ToList();
                return modelList;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }

        [HttpGet("{id}")]
        public async Task<OrderModel> MarkAsShipped(int id)
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
                    temp.OrderStatus = 2;
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
