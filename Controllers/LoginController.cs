using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ATAccountContext _aTAccountContext;
        private IConfiguration _configuration;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public LoginController(ATAccountContext aTAccountContext, IConfiguration configuration)
        {
            _aTAccountContext = aTAccountContext;
            _configuration = configuration;
            //_signInManager = signInManager;
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<string>> LoginUser(SignIn signIn)
        {
            try
            {
                User user = new User();
                if (!signIn.OtherAuthentication)
                {
                    if (string.IsNullOrEmpty(signIn.username))
                    {
                        BadRequest("Mail or Password is invalid");
                    }
                    else if (string.IsNullOrEmpty(signIn.password))
                    {
                        BadRequest("Mail or Password is invalid");
                    }
                     user = _aTAccountContext.Users.Where(c => c.MailAdress == signIn.username && c.Password == signIn.password).FirstOrDefaultAsync().Result; 
                }
                else
                {
                     user = _aTAccountContext.Users.Where(c => c.MailAdress == signIn.username).FirstOrDefaultAsync().Result;
                }
                if (user == null)
                {
                    return NoContent();
                }
                else if (user.PasswordChangeDate.AddDays(user.PasswordExpiration) <= DateTime.Now)
                {
                    user.ResetPasswordVerify = 0;
                    _aTAccountContext.SaveChanges();
                }

                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.Name, user.MailAdress);


                return Ok(token);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("Login1")]
        public async Task Login1()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        [HttpGet("GoogleResponse")]
        public async Task<ActionResult<string>> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            //var claims = result.Principal.Identities
            //    .FirstOrDefault().Claims.Select(claim => new
            //    {
            //        claim.Issuer,
            //        claim.OriginalIssuer,
            //        claim.Type,
            //        claim.Value
            //    });
            var claims = result.Principal.Claims.ToList();
            Models.DummyModels.SignIn signIn = new SignIn();
            signIn.username= claims[4].Value;
            signIn.OtherAuthentication = true;
          var token=  LoginUser(signIn);
            return Ok(token);
        }

    }
}
