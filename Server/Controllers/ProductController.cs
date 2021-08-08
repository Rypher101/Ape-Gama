using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApeGamaContext _context;
        private readonly IWebHostEnvironment _env;

        public ProductController(ApeGamaContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: api/Product
        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts(int id = -1)
        {
            var products = new List<ProductModel>();
            try
            {
                int shopID;

                if (id == -1)
                {
                    shopID = (int)HttpContext.Session.GetInt32("ShopID");
                }
                else
                {
                    shopID = id;
                }

                products = await _context.Products.Where(x => x.Shop.ShopId == shopID).ToListAsync();

                foreach (var item in products)
                {
                    string path = "";
                    if (string.IsNullOrWhiteSpace(item.ProdDp))
                    {
                        path = Path.Combine(_env.ContentRootPath, "Images", "NoImage.jpg");
                    }
                    else
                    {
                        path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", item.ProdId.ToString(), item.ProdDp);
                    }

                    if (System.IO.File.Exists(path))
                    {
                        using (Image image = Image.FromFile(path))
                        {
                            using (MemoryStream ms = new())
                            {
                                image.Save(ms, image.RawFormat);
                                byte[] buffer = ms.ToArray();
                                item.fileString = $"data:{image.GetType()};base64,{Convert.ToBase64String(buffer)}";
                            }
                        }
                    }
                    else
                    {
                        item.ProdDp = null;
                        _context.Entry(item).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProductFromShop(int id)
        {
            var products = await _context.Products.Where(x => x.ShopId == id).ToListAsync();
            if (products == null)
            {
                return NotFound();
            }

            return products;
        }

        // GET: api/Product/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductModel>> GetProductModel(int id)
        {
            int index = 0;
            var removeList = new List<ImageModel>();
            var productModel = await _context.Products.Include(e => e.Images).FirstOrDefaultAsync(e => e.ProdId == id);

            if (productModel == null)
            {
                return NotFound();
            }

            productModel.ImageList = productModel.Images.ToList();
            productModel.Images = null;

            try
            {
                foreach (var item in productModel.ImageList)
                {
                    if (String.Equals(item.ImgName, productModel.ProdDp))
                    {
                        item.isDP = true;
                        index = productModel.ImageList.IndexOf(item);
                    }
                    string path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", id.ToString(), item.ImgName);

                    if (System.IO.File.Exists(path))
                    {
                        using (Image image = Image.FromFile(path))
                        {

                            using (MemoryStream ms = new())
                            {
                                image.Save(ms, image.RawFormat);
                                byte[] buffer = ms.ToArray();
                                item.fileString = $"data:{image.GetType()};base64,{Convert.ToBase64String(buffer)}";
                            }
                        }
                    }
                }

                foreach (var item in removeList)
                {
                    productModel.Images.Remove(item);
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }

                if (index > 0)
                {
                    var tempImg1 = productModel.ImageList[0];
                    productModel.ImageList[0] = productModel.ImageList[index];
                    productModel.ImageList[index] = tempImg1;
                }
            }
            catch (Exception ex)
            {
                productModel.fileString = ex.Message;
                return productModel;
            }

            var cart = HttpContext.Session.GetString("Cart");
            if (!string.IsNullOrWhiteSpace(cart))
            {
                var temp = JsonConvert.DeserializeObject<List<CartModel>>(cart);
                var tempItem = temp.Where(e => e.prodID == id).FirstOrDefault();
                if (tempItem != null)
                    productModel.qty = tempItem.qty;
                else
                    productModel.qty = 0;
            }
            return productModel;
        }

        // PUT: api/Product/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductModel(int id, ProductModel productModel)
        {
            if (id != productModel.ProdId)
            {
                return BadRequest();
            }

            _context.Entry(productModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Product
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult> PostProductModel(ProductModel productModel)
        {
            try
            {
                productModel.ShopId = (int)HttpContext.Session.GetInt32("ShopID");
                _context.Products.Add(productModel);
                await _context.SaveChangesAsync();

                int newId = _context.Products.Max(e => e.ProdId);
                return Ok(newId);
            }
            catch (Exception)
            {
                return BadRequest();
            }

        }

        // DELETE: api/Product/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductModel(int id)
        {
            var productModel = await _context.Products.FindAsync(id);
            if (productModel == null)
            {
                return NotFound();
            }

            _context.Products.Remove(productModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult> ProductImageUpload(ImageModel imageModel)
        {
            if (imageModel == null)
            {
                return NoContent();
            }

            try
            {
                int ShopID = (int)HttpContext.Session.GetInt32("ShopID");
                if (_context.Products.Any(e => e.ProdId == imageModel.ProdId && e.ShopId == ShopID))
                {
                    string path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", imageModel.ProdId.ToString());
                    Directory.CreateDirectory(path);

                    byte[] bytes = Convert.FromBase64String(imageModel.fileString);
                    Image image;
                    await using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        image = Image.FromStream(ms);

                        if (System.IO.File.Exists(Path.Combine(path, imageModel.ImgName)))
                        {
                            int i = 1;
                            while (true)
                            {
                                if (System.IO.File.Exists(Path.Combine(path, " (" + i + ")" + imageModel.ImgName)))
                                {
                                    i++;
                                }
                                else
                                {
                                    path = Path.Combine(path, " (" + i + ")" + imageModel.ImgName);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            path = Path.Combine(path, imageModel.ImgName);
                        }

                        image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
                    }

                    var product = _context.Products.FirstOrDefault(e => e.ProdId == imageModel.ProdId);
                    if (string.IsNullOrEmpty(product.ProdDp))
                    {
                        product.ProdDp = imageModel.ImgName;
                        _context.Entry(product).State = EntityState.Modified;
                    }

                    _context.Images.Add(imageModel);
                    var response = await _context.SaveChangesAsync();
                    if (response > 0)
                    {
                        return Ok();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return Unauthorized();
                }

            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<ImageModel>>> GetProductImages(int id)
        {
            var Images = new List<ImageModel>();
            var removeList = new List<ImageModel>();
            int index = 0;
            try
            {
                Images = await _context.Images.Where(e => e.ProdId == id).ToListAsync();
                var prod = await _context.Products.FirstOrDefaultAsync(e => e.ProdId == id);

                foreach (var item in Images)
                {
                    if (String.Equals(item.ImgName, prod.ProdDp))
                    {
                        item.isDP = true;
                        index = Images.FindIndex(e => e.ImgId == item.ImgId);
                    }

                    string path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", id.ToString(), item.ImgName);

                    if (System.IO.File.Exists(path))
                    {
                        using (Image image = Image.FromFile(path))
                        {

                            using (MemoryStream ms = new())
                            {
                                image.Save(ms, image.RawFormat);
                                byte[] buffer = ms.ToArray();
                                item.fileString = $"data:{image.GetType()};base64,{Convert.ToBase64String(buffer)}";
                            }
                        }
                    }
                    else
                    {
                        removeList.Add(item);
                    }
                }

                foreach (var item in removeList)
                {
                    Images.Remove(item);
                    _context.Remove(item);
                    await _context.SaveChangesAsync();
                }

                if (index > 0)
                {
                    var tempImg1 = Images[0];
                    Images[0] = Images[index];
                    Images[index] = tempImg1;
                }
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Images;
        }

        [HttpPost]
        public async Task<ActionResult> ChangeDP(ImageModel img)
        {
            if (!string.IsNullOrWhiteSpace(img.ImgName) && img.ProdId != -1)
            {
                var prod = await _context.Products.FirstOrDefaultAsync(e => e.ProdId == img.ProdId);
                if (prod == null) return NotFound();

                prod.ProdDp = img.ImgName;
                _context.Entry(prod).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            else
            {
                return NotFound();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteImage(ImageModel imgModel)
        {
            if (imgModel == null)
            {
                return NotFound();
            }


            try
            {
                _context.Images.Remove(imgModel);
                if (imgModel.isDP)
                {
                    var tempImg = await _context.Images.AsNoTracking().FirstOrDefaultAsync(e => e.ImgId != imgModel.ImgId && e.ProdId == imgModel.ProdId);
                    var prod = await _context.Products.FindAsync(imgModel.ProdId);

                    if (tempImg != null)
                    {
                        prod.ProdDp = tempImg.ImgName;
                    }
                    else
                    {
                        prod.ProdDp = null;
                    }

                    _context.Entry(prod).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }


                //await _context.SaveChangesAsync();

                string path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", imgModel.ProdId.ToString(), imgModel.ImgName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
            catch (Exception)
            {
                return NotFound();
            }

            return Ok();
        }

        //[HttpPost]
        //public IActionResult AddToCart(CartModel model)
        //{
        //    var inCart = false;
        //    List<CartModel> cartModel = new List<CartModel>();
        //    var cart = HttpContext.Session.GetString("Cart");
        //    int status = 0;

        //    if (!string.IsNullOrWhiteSpace(cart))
        //        cartModel = JsonConvert.DeserializeObject<List<CartModel>>(cart);

        //    if (model.qty > 0)
        //    {
        //        foreach (var item in cartModel)
        //        {
        //            if (item.prodID == model.prodID)
        //            {
        //                item.qty = model.qty;
        //                inCart = true;
        //                status = 1;
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var tempRemove = cartModel.FirstOrDefault(e => e.prodID == model.prodID);
        //        if (tempRemove != null)
        //        {
        //            cartModel.Remove(tempRemove);
        //            status = 2;
        //            inCart= true;
        //        }

        //    }

        //    if (!inCart && model.qty>0)
        //    {
        //        cartModel.Add(model);
        //        status = 1;
        //    }

        //    HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cartModel));

        //    switch (status)
        //    {
        //        case 0:
        //            return BadRequest();
        //        case 1:
        //            return Ok();
        //        case 2:
        //            return Accepted();
        //        default:
        //            return NotFound();
        //    }

        //}

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.ProdId == id);
        }
    }
}
