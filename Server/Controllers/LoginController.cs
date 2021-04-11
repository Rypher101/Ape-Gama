using ApeGama.Server.Data;
using ApeGama.Shared;
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
                    user = await context.Users.FirstOrDefaultAsync(x=>x.UserEmail == loginModel.userEmalil && x.UserPass == loginModel.userPassword && x.UserFlag == 1);
                    if(user == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok();
                    }
                case 2:
                    user = await context.Users.FirstOrDefaultAsync(x => x.UserEmail == loginModel.userEmalil && x.UserPass == loginModel.userPassword && x.UserFlag == 2);
                    if (user == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        return Ok();
                    }
                default:
                    return NotFound();

            }
        }
    }
}
