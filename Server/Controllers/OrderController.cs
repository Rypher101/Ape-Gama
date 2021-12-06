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
using MailKit.Net.Smtp;
using MimeKit;

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
                orders = orders.OrderBy(e => e.OrderStatus).ThenByDescending(e => e.OrderId).ToList();

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
                    temp.ReceivedDate = DateTime.Now;
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
                var cartDetails = new List<CartModel>();
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

                var shop = _context.OnlineShops.Where(e => e.ShopId == order.ShopId).FirstOrDefault();
                var notifi = _context.Notifications.Where(e => e.UserId == shop.SupId && e.Category == 2).FirstOrDefault();
                if(notifi == null)
				{
                    var temp = new NotificationModel();
                    temp.UserId = shop.SupId;    
                    temp.Category = 2;
                    _context.Add(temp);
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

                var modelList = _context.Orders.Where(e => e.ShopId == shopID).OrderBy(e => e.OrderStatus).ThenByDescending(e => e.OrderId).Include(e => e.Cus).ToList();
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
                .AsNoTracking()
                .FirstOrDefaultAsync();

                if (temp != null)
                {
                    temp.OrderStatus = 2;
                    _context.Attach(temp);
                    _context.Entry(temp).Property(e => e.OrderStatus).IsModified = true;
                    await _context.SaveChangesAsync();
                    SendDeliveryEmail(temp.OrderId);
                }
                else
                {
                    return null;
                }

                _context.ChangeTracker.Clear();

                foreach (var item in temp.OrderProducts)
                {
                    var tempProd = await _context.Products.AsNoTracking().Where(e => e.ProdId == item.ProdId).FirstOrDefaultAsync();
                    tempProd.ProdStock = (int)(tempProd.ProdStock - item.Qty);
                    _context.Entry(tempProd).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                var notifi = _context.Notifications.Where(e => e.UserId == temp.CusId && e.Category == 2).FirstOrDefault();
                if (notifi == null)
                {
                    var temp2 = new NotificationModel();
                    temp2.UserId = temp.CusId;
                    temp2.Category = 2;
                    _context.Add(temp2);
                    _context.SaveChanges();
                }

                return temp;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void SendDeliveryEmail(int orderId)
        {
            try
            {
                var shopID = HttpContext.Session.GetInt32("ShopID");
                var shop = _context.OnlineShops.Where(e => e.ShopId == shopID).Include(e => e.Sup).FirstOrDefault();

                var message = new MimeMessage();
                var from = new MailboxAddress(shop.ShopName, shop.Sup.UserEmail);
                message.From.Add(from);

                var order = _context.Orders.Where(e => e.OrderId == orderId).Include(e => e.Cus).Include(e => e.OrderProducts).ThenInclude(e => e.Prod).FirstOrDefault();
                var to = new MailboxAddress(order.Cus.UserName, order.Cus.UserEmail);
                message.To.Add(to);

                message.Subject = "Your order has been dispatched. Order Number : " + order.OrderId;

                var body = "Dear Customer," + Environment.NewLine + Environment.NewLine +  "Your order number (" + order.OrderId + ") has been dispatched." + Environment.NewLine + Environment.NewLine + "Order contains these products, ";
                foreach (var item in order.OrderProducts)
                {
                    body = body + Environment.NewLine + item.Prod.ProdName + " : " + item.Qty;
                }
                body = body + Environment.NewLine + Environment.NewLine + "Thank You!";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = body;
                message.Body = bodyBuilder.ToMessageBody();

                var client = new SmtpClient();
                client.Connect("smtp.gmail.com", 465, true);
                client.Authenticate("gamaape3@gmail.com", "apegama123");

                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
