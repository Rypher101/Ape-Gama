using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

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
                var temp = _context.Products.Include(e => e.Shop).FirstOrDefault(e => e.ProdId == model.prodID);
                model.shopID = temp.ShopId;
                model.shopName = temp.Shop.ShopName;

                cartModel.Add(model);
                status = 1;

                var uid = (int)HttpContext.Session.GetInt32("UID");
                var notifi = _context.Notifications.Where(e => e.UserId == uid && e.Category == 1).FirstOrDefault();
                if (notifi == null)
                {
                    var temp2 = new NotificationModel();
                    temp2.UserId = uid;
                    temp2.Category = 1;
                    _context.Add(temp2);
                    _context.SaveChanges();
                }
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

        [HttpGet("{id}")]
        public ConfirmOrderModel ConfirmOrder(int id)
        {
            var user = new UserModel();
            var order = new ConfirmOrderModel();
            var cartDetails = new List<CartModel>();
            try
            {
                user = _context.Users.Find(HttpContext.Session.GetInt32("UID"));
                if (user == null)
                    return null;

                order.address = user.UserAddress;
                order.contact = user.UserTp;

                order.shopID = id;
                order.shop = _context.OnlineShops.Find(id).ShopName;

                var sessionCart = HttpContext.Session.GetString("Cart");

                if (!string.IsNullOrEmpty(sessionCart))
                    cartDetails = JsonConvert.DeserializeObject<List<CartModel>>(sessionCart);
                else
                    return null;

                foreach (var item in cartDetails)
                {
                    var temp = _context.Products.FirstOrDefault(e => e.ProdId == item.prodID);
                    if (temp == null)
                    {
                        HttpContext.Session.SetString("Cart", "");
                        cartDetails.Clear();
                        return null;
                    }

                    order.total += item.qty * temp.ProdPrice;
                }

                return order;
            }
            catch (Exception)
            {
                return null;
                throw;
            }

        }

        [HttpGet("{id}")]
        public IActionResult DeleteFromCart(int id)
        {
            try
            {
                List<CartModel> cartModel = new List<CartModel>();
                var cart = HttpContext.Session.GetString("Cart");
                if (!string.IsNullOrWhiteSpace(cart))
                    cartModel = JsonConvert.DeserializeObject<List<CartModel>>(cart);

                var tempRemove = cartModel.FirstOrDefault(e => e.prodID == id);
                if (tempRemove != null)
                {
                    cartModel.Remove(tempRemove);
                }

                HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartModel));
            }
            catch (Exception)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
