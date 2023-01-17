using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        public LoginController(ATAccountContext aTAccountContext)
        {
            _aTAccountContext = aTAccountContext;
        }
        [HttpGet("LoginUser")]
        public async Task<ActionResult> LoginUser(SignIn signIn)
        {
            try
            {
                if (string.IsNullOrEmpty(signIn.username))
                {
                    BadRequest("Mail or Password is invalid");
                }
                else if (string.IsNullOrEmpty(signIn.password))
                {
                    BadRequest("Mail or Password is invalid");
                }
                User user = _aTAccountContext.Users.Where(c => c.MailAdress == signIn.username && c.Password == signIn.password).FirstOrDefaultAsync().Result;
                if (user == null)
                {
                    return NoContent();
                }
                else if (user.PasswordChangeDate.AddDays(user.PasswordExpiration) <= DateTime.Now)
                {
                    user.ResetPasswordVerify = 0;
                    _aTAccountContext.SaveChanges();
                }

                //JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                //string token = jwtTokenGenerator.Generate(signIn.User.Name, signIn.User.Email);
                //signIn.JWTAuthToken = token;

                return Ok("Hosgeldiniz "+ user.Name);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
       
    }
}
