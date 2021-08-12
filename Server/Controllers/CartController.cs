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
    public class CartController : ControllerBase
    {
        private readonly ApeGamaContext _context;
        public CartController(ApeGamaContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult AddToCart(CartModel model)
        {
            var inCart = false;
            List<CartModel> cartModel = new List<CartModel>();
            var cart = HttpContext.Session.GetString("Cart");
            int status = 0;

            if (!string.IsNullOrWhiteSpace(cart))
                cartModel = JsonConvert.DeserializeObject<List<CartModel>>(cart);

            if (model.qty > 0)
            {
                foreach (var item in cartModel)
                {
                    if (item.prodID == model.prodID)
                    {
                        item.qty = model.qty;
                        inCart = true;
                        status = 1;
                        break;
                    }
                }
            }
            else
            {
                var tempRemove = cartModel.FirstOrDefault(e => e.prodID == model.prodID);
                if (tempRemove != null)
                {
                    cartModel.Remove(tempRemove);
                    status = 2;
                    inCart = true;
                }

            }

            if (!inCart && model.qty > 0)
            {
                var temp = _context.Products.Include(e=>e.Shop).FirstOrDefault(e => e.ProdId == model.prodID);
                model.shopID = temp.ShopId;
                model.shopName = temp.Shop.ShopName;

                cartModel.Add(model);
                status = 1;
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartModel));

            switch (status)
            {
                case 0:
                    return BadRequest();
                case 1:
                    return Ok();
                case 2:
                    return Accepted();
                default:
                    return NotFound();
            }

        }

        [HttpGet]
        public ActionResult<IEnumerable<CartModel>> GetCartDetails()
        {
            var cartDetails = new List<CartModel>();
            var sessionCart = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(sessionCart))
                cartDetails = JsonConvert.DeserializeObject<List<CartModel>>(sessionCart);
            else
                return cartDetails;

            foreach (var item in cartDetails)
            {
                var temp = _context.Products.FirstOrDefault(e => e.ProdId == item.prodID);
                if (temp == null)
                {
                    HttpContext.Session.SetString("Cart", "");
                    cartDetails.Clear();
                    return cartDetails;
                }

                item.product = temp.ProdName;
                item.qtyString = item.qty.ToString();
                item.price = temp.ProdPrice;
            }

            return cartDetails;
        }

        [HttpPost]
        public ActionResult ConfirmOrder(List<CartModel> model)
        {
            try
            {
                if (model.Count > 0)
                {
                    var cusID = HttpContext.Session.GetInt32("UID");
                    var order = new OrderModel()
                    {
                        ShopId = model.First().shopID,
                        CusId = (int)cusID,
                        OrderDate = DateTime.Now.Date,
                        OrderStatus = 0
                    };

                    _context.Orders.Add(order);
                    _context.SaveChanges();

                    int oID = order.OrderId;

                    foreach (var item in model)
                    {
                        var temp = new OrderProductModel()
                        {
                            OrderId = oID,
                            ProdId = item.prodID,
                            Qty = item.qty
                        };
                        _context.OrderProducts.Add(temp);
                    }

                    _context.SaveChanges();
                    return Ok(oID);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }    
        }
    }
}
