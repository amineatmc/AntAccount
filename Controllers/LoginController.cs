using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using Azure.Core.Serialization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        //private readonly Otp _otp;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ATAccountContext _aTAccountContext;
        private IConfiguration _configuration;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public LoginController(ATAccountContext aTAccountContext, IConfiguration configuration,IConnectionMultiplexer connectionMultiplexer)
        {
            _aTAccountContext = aTAccountContext;
            _configuration = configuration;
            _connectionMultiplexer = connectionMultiplexer;
            // _otp = otp;
            //_signInManager = signInManager;
        }

        [HttpPost("LoginUserWeb")]
        public async Task<ActionResult<string>> LoginUser(SignIn signIn)
        {
            try
            {
                AllUser user = new AllUser();
                if (!signIn.OtherAuthentication)
                {
                    if (string.IsNullOrEmpty(signIn.username))
                    {
                       return  BadRequest("Mail or Password is invalid");
                    }
                    else if (string.IsNullOrEmpty(signIn.password))
                    {
                     return   BadRequest("Mail or Password is invalid");
                    }

                    string encodedPassword = Helper.PasswordEncode(signIn.password);

                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username && c.Password == encodedPassword&& c.Activity==1).FirstOrDefaultAsync().Result;
                }
                else
                {
                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username && c.Activity==1).FirstOrDefaultAsync().Result;
                }
                if (user == null)
                {
                   return  BadRequest("Mail or Password is invalid");
                }
                int userid=0;
                if (user.UserType == 1)
                {
                    var driver = _aTAccountContext.Drivers.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = driver.DriverID;
                   // var station = _context.Stations.Where(x => x.AllUserID == id).FirstOrDefault();
                }
                if (user.UserType==2)
                {
                    var passenger = _aTAccountContext.Passengers.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = passenger.PassengerID;
                }
                if (user.UserType==3)
                {
                    var station = _aTAccountContext.Stations.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = station.StationID;
                }
                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType.ToString(),userid);
                return Ok(token);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("LoginUserMob")]
        public async Task<ActionResult<string>> LoginUserMob(SignInMob signIn)
        {
            try
            {
                AllUser user = new AllUser();

                if (string.IsNullOrEmpty(signIn.Phone))
                {
                   return  BadRequest("Phone or Password is invalid");
                }
                else if (string.IsNullOrEmpty(signIn.Password))
                {
                   return  BadRequest("Phone or Password is invalid");
                }
                string encodedPassword = Helper.PasswordEncode(signIn.Password);
                user = _aTAccountContext.AllUsers.Where(c => c.Phone == signIn.Phone && c.Password == encodedPassword ).FirstOrDefaultAsync().Result;


                if (user == null)
                {
                    return BadRequest("Phone or Password is invalid");
                }

                int userid = 0;
                if (user.UserType == 1)
                {
                    var driver = _aTAccountContext.Drivers.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = driver.DriverID;
                    // var station = _context.Stations.Where(x => x.AllUserID == id).FirstOrDefault();
                }
                if (user.UserType == 2)
                {
                    var passenger = _aTAccountContext.Passengers.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = passenger.PassengerID;
                }
                if (user.UserType == 3)
                {
                    var station = _aTAccountContext.Stations.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                    userid = station.StationID;
                }
                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType.ToString(),userid);
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
            var claims = result.Principal.Claims.ToList();
            Models.DummyModels.SignIn signIn = new SignIn();
            signIn.username = claims[4].Value;
            signIn.OtherAuthentication = true;
            return await LoginUser(signIn);
        }

        [HttpPost("OtpSend")]
        public async Task<ActionResult> OtpSend(CheckOtpDto checkOtpDto)
        {
            try
            {
                var gsm = _aTAccountContext.AllUsers.Where(x=>x.AllUserID==checkOtpDto.UserID).FirstOrDefault();
                if (gsm == null) 
                {
                    return BadRequest("Geçersiz Gsm");
                }
                checkOtpDto.Phone=gsm.Phone;
                Otp _otp = new Otp(_aTAccountContext, _connectionMultiplexer);
                var result = _otp.CheckOtpSendMethod(checkOtpDto);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok("Otp Gönderimi Başarılı.");
        }

        [HttpPost("CheckOtp")]
        public async Task<ActionResult> CheckOtp(CheckOtpDto checkOtpDto)
        {
            Otp _otp = new Otp(_aTAccountContext,_connectionMultiplexer);
            var result =  _otp.CheckOtpVerification(checkOtpDto);
            if (result=="false")
            {
                return BadRequest("Doğrulama Başarısız");
            }
            return Ok("Otp Eşleştirme Başarılı.");
        }
        [HttpGet("isTokenExpired")]
        [Authorize]
        public string IsTokenExpired()
        {
            return "Geçerli";
        }
    }
}