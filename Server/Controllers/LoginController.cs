using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApeGamaContext _context;

        public LoginController(ApeGamaContext context)
        {
            _context = context;
        }

        //POST: api/Login/1
        [HttpPost("{type}")]
        public async Task<IActionResult> PostLogin(LoginModel loginModel, int type = 0)
        {
            HttpContext.Session.Clear();
            var user = new UserModel();
            switch (type)
            {
                case 0:
                    return NotFound();

                case 1:
                    user = await _context.Users.FirstOrDefaultAsync(x => x.UserEmail == loginModel.userEmail && x.UserPass == loginModel.userPassword && x.UserFlag == 1);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        HttpContext.Session.SetInt32("UID", user.UserId);
                        HttpContext.Session.SetString("UName", user.UserName);
                        HttpContext.Session.SetString("UEmail", user.UserEmail);
                        HttpContext.Session.SetInt32("URole", 1);
                        return Ok();
                    }
                case 2:
                    try
                    {
                        user = await _context.Users
                        .Include("OnlineShop")
                        .FirstOrDefaultAsync(x => x.UserEmail == loginModel.userEmail && x.UserPass == loginModel.userPassword && x.UserFlag == 2);
                        if (user == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            HttpContext.Session.SetInt32("UID", user.UserId);
                            HttpContext.Session.SetString("UName", user.UserName);
                            HttpContext.Session.SetString("UEmail", user.UserEmail);
                            HttpContext.Session.SetInt32("URole", 2);
                            HttpContext.Session.SetInt32("ShopID", user.OnlineShop != null ? user.OnlineShop.ShopId : 0);
                            return Ok();
                        }
                    }
                    catch (System.Exception)
                    {

                        throw;
                    }
                    
                default:
                    return NotFound();
            }
        }

        [HttpDelete]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return Ok();
        }

        [HttpGet]
        public LoginModel Authenticate()
        {
            var loginModel = new LoginModel();
            if (!string.IsNullOrWhiteSpace(HttpContext.Session.GetString("UName")))
            {
                loginModel.userName = HttpContext.Session.GetString("UName");
                loginModel.userEmail = HttpContext.Session.GetString("UEmail");
                loginModel.userType = (int)HttpContext.Session.GetInt32("URole");
            }

            return loginModel;
        }

        [HttpGet("{type}")]
        public IActionResult GetHandler(int type = 0)
        {
            switch (type)
            {
                case 1:
                    return IsShopAvailable();
                default:
                    return NotFound();
            }
        }

        private IActionResult IsShopAvailable()
        {
            if (HttpContext.Session.TryGetValue("ShopID", out byte[] _) && HttpContext.Session.GetInt32("ShopID")>0)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }
    }
}
