using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductModel>>> GetProducts()
        {
            var products = new List<ProductModel>();
            try
            {
                products = await _context.Products
                .Include("Shop")
                .Where(x => x.Shop.ShopId == HttpContext.Session.GetInt32("ShopID"))
                .ToListAsync();
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
            var productModel = await _context.Products.FindAsync(id);

            if (productModel == null)
            {
                return NotFound();
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
                    string path = Path.Combine(_env.ContentRootPath, "Uploads", "Products", imageModel.ProdId.ToString())
                        ;
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

        private bool ProductModelExists(int id)
        {
            return _context.Products.Any(e => e.ProdId == id);
        }
    }
}
