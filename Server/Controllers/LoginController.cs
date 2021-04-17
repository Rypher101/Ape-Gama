using ApeGama.Server.Data;
using ApeGama.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ApeGama.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApeGamaContext context;

        public LoginController(ApeGamaContext context)
        {
            this.context = context;
        }

        //POST: api/Login/1
        [HttpPost("{type}")]
        public async Task<IActionResult> PostLogin(LoginModel loginModel, int type=0)
        {
            var user = new User();
            switch (type)
            {
                case 0:
                    return NotFound();
                    
                case 1:
                    user = await context.Users.FirstOrDefaultAsync(x=>x.UserEmail == loginModel.userEmail && x.UserPass == loginModel.userPassword && x.UserFlag == 1);
                    if(user == null)
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
                    user = await context.Users.FirstOrDefaultAsync(x => x.UserEmail == loginModel.userEmail && x.UserPass == loginModel.userPassword && x.UserFlag == 2);
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
                        return Ok();
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
    }
}
