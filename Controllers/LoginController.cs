﻿using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using AntalyaTaksiAccount.ValidationRules;
using Azure.Core.Serialization;
using Indice.AspNetCore.Authentication.Apple;
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
using Newtonsoft.Json;

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
        public LoginController(ATAccountContext aTAccountContext, IConfiguration configuration, IConnectionMultiplexer connectionMultiplexer)
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
                        return BadRequest("Mail or Password is invalid");
                    }
                    else if (string.IsNullOrEmpty(signIn.password))
                    {
                        return BadRequest("Mail or Password is invalid");
                    }

                    string encodedPassword = Helper.PasswordEncode(signIn.password);

                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username && c.Password == encodedPassword && c.Activity==1).FirstOrDefaultAsync().Result;
                }
                else
                {
                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username && c.Activity==1).FirstOrDefaultAsync().Result;
                }
                if (user == null)
                {
                    var db = _connectionMultiplexer.GetDatabase(4);
                    string key = signIn.username.ToString() + "_" + "count";
                    var jsonsd = db.StringGet(key);
                    if (jsonsd.IsNull == true)
                    {
                        BanControl obj = new BanControl()
                        {
                            Count = 0,
                            DateTime = DateTime.Now.AddMinutes(10),
                        };
                        db.KeyExpire(key, DateTime.Now.AddMinutes(10));
                        string json = JsonConvert.SerializeObject(obj);
                        var set = db.StringSet(key, json);
                    }

                    var jsons = db.StringGet(key);
                    BanControl person = JsonConvert.DeserializeObject<BanControl>(jsons);
                    person.Count++;                    
                    jsons = JsonConvert.SerializeObject(person);
                    db.StringSet(key, jsons);
                    db.KeyExpire(key, DateTime.Now.AddMinutes(10));

                    if (person.Count >= 5)
                    {                      
                        return BadRequest("Lütfen 15 dakika sonra tekrar deneyin.");
                    }

                    return BadRequest("Mail or Password is invalid");
                }



                var data = _connectionMultiplexer.GetDatabase(4);
                string keys = signIn.username.ToString() + "_" + "count";

                var jsonss = data.StringGet(keys);
                if (jsonss.IsNull==false)/*jsonss dolu*/
                {
                    BanControl persons = JsonConvert.DeserializeObject<BanControl>(jsonss);

                    jsonss = JsonConvert.SerializeObject(persons);
                    var value = data.StringGet(keys);
                    //int news = Convert.ToInt32(value);

                    if (persons.Count >= 5)
                    {
                        return BadRequest("Lütfen 15 dakika sonra tekrar deneyin.");
                    }
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
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType.ToString(), userid);
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
                    return BadRequest("Phone or Password is invalid");
                }
                else if (string.IsNullOrEmpty(signIn.Password))
                {
                    return BadRequest("Phone or Password is invalid");
                }
                string encodedPassword = Helper.PasswordEncode(signIn.Password);
                user = _aTAccountContext.AllUsers.Where(c => c.Phone == signIn.Phone && c.Password == encodedPassword && c.Activity==1 ).FirstOrDefaultAsync().Result;


                if (user == null)
                {
                    var db = _connectionMultiplexer.GetDatabase(4);
                    string key = signIn.Phone.ToString() + "_" + "count";
                    var jsonsd = db.StringGet(key);
                    if (jsonsd.IsNull == true)
                    {
                        BanControl obj = new BanControl()
                        {
                            Count = 0,
                            DateTime = DateTime.Now,                          
                        };
                        string json = JsonConvert.SerializeObject(obj);
                        var set = db.StringSet(key, json);
                        db.KeyExpire(key, DateTime.Now.AddSeconds(900));

                    }

                    var jsons = db.StringGet(key);
                    BanControl person = JsonConvert.DeserializeObject<BanControl>(jsons);
                    person.Count++;
                    jsons = JsonConvert.SerializeObject(person);
                    db.StringSet(key, jsons);
                    db.KeyExpire(key, DateTime.Now.AddSeconds(900));

                    if (person.Count >= 5)
                    {                      
                        return BadRequest("Lütfen 15 dakika sonra tekrar deneyin.");
                    }
                    return BadRequest("Phone or Password is invalid");
                }

                var data = _connectionMultiplexer.GetDatabase(4);
                string keys = signIn.Phone.ToString() + "_" + "count";

                var jsonss = data.StringGet(keys);
                if (jsonss.IsNull == false)
                {
                    BanControl persons = JsonConvert.DeserializeObject<BanControl>(jsonss);
                    jsonss = JsonConvert.SerializeObject(persons);
                    var value = data.StringGet(keys);
                   
                    if (persons.Count >= 5)
                    {
                        return BadRequest("Lütfen 15 dakika sonra tekrar deneyin.");
                    }
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
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType.ToString(), userid);
                return Ok(token);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("Login1")]
        public async  Task Login1()
        {

            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
      
        [AllowAnonymous]
        [HttpGet("AppleLogin")]
        public async Task SignInApple()
        {

            await HttpContext.ChallengeAsync(AppleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action(nameof(HandleAppleLogin))
            });

        }
        [AllowAnonymous]
        [HttpGet("HandleAppleLogin")]
        public async Task<ActionResult<string>> HandleAppleLogin()
        {
            try
            {
                var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                var claims = authenticateResult.Principal.Claims;
                var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                Models.DummyModels.SignIn signIn = new SignIn();
                signIn.username = email;
                signIn.OtherAuthentication = true;
                return await LoginUser(signIn);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }



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
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgetMyPassword( [FromQuery] int Id, [FromBody] ForgetPassword forgetPassword)
        {
            var userClaims = Request.HttpContext.User.Claims.ToList();
            var userId = int.Parse(userClaims[1].Value);
            try
            {
                if (Id == userId)
                {
                    AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == Id && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (user1 == null) { return NoContent(); }

                    if (forgetPassword.newPassword1==forgetPassword.newPassword2)
                    {
                        user1.Password = forgetPassword.newPassword1;
                        AllUserValidator validations = new AllUserValidator();
                        var validationResult = validations.Validate(user1);
                        if (!validationResult.IsValid)
                        {
                            return BadRequest(validationResult.Errors);
                        }
                        user1.Password = Helper.PasswordEncode(forgetPassword.newPassword1);
                        
                        _aTAccountContext.AllUsers.Update(user1);
                        _aTAccountContext.SaveChanges();
                        return Ok("Şifreniz Değiştirildi.");
                    }

                }
                return NoContent();

            }
            catch (Exception ex)
            {
                return Problem();
            }
        }



        [HttpPost("OtpSend")]
        public async Task<ActionResult> OtpSend(CheckOtpDto checkOtpDto)
        {
            try
            {
                var gsm = _aTAccountContext.AllUsers.Where(x => x.AllUserID == checkOtpDto.UserID).FirstOrDefault();
                if (gsm == null)
                {
                    return BadRequest("Geçersiz Gsm");
                }
                checkOtpDto.Phone = gsm.Phone;
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
            Otp _otp = new Otp(_aTAccountContext, _connectionMultiplexer);
            var result = _otp.CheckOtpVerification(checkOtpDto);
            if (result == "false")
            {
                return BadRequest("Doğrulama Başarısız");
            }
            return Ok("Otp Eşleştirme Başarılı.");
        }
        #region ForgotMyPassword OTP 

        [HttpPost("OtpSendForForgetPassword")]
        public async Task<ActionResult> OtpSendForgetPassword(CheckOtpDto checkOtpDto)
        {
            try
            {
                var gsm = _aTAccountContext.AllUsers.Where(x => x.Phone == checkOtpDto.Phone).FirstOrDefault();
                if (gsm == null)
                {
                    return BadRequest("Geçersiz Gsm");
                }
                checkOtpDto.Phone = gsm.Phone;
                checkOtpDto.UserID = gsm.AllUserID;
                Otp _otp = new Otp(_aTAccountContext, _connectionMultiplexer);
                var result = _otp.OTPSendForgotPassword(checkOtpDto);
            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
            return Ok("Otp Gönderimi Başarılı.");
        }

        [HttpPost("CheckOtpForgetPassword")]
        public async Task<ActionResult> CheckOtpForgetPassword(CheckOtpDto checkOtpDto)
        {
            Otp _otp = new Otp(_aTAccountContext, _connectionMultiplexer);
          
            var user = _aTAccountContext.AllUsers.Where(x => x.Phone == checkOtpDto.Phone).FirstOrDefault();
            if (user==null)
            {
                return BadRequest("Girilen numaraya ait kullanıcı bulanamadı!");
            }
            checkOtpDto.UserID = user.AllUserID;
            var result = _otp.CheckOtpVerification(checkOtpDto);
            if (result == "false")
            {
                return BadRequest("Doğrulama Başarısız");
            }
            int userid = 0;
            if (user.UserType == 1)
            {
                var driver = _aTAccountContext.Drivers.Where(x => x.AllUserID == user.AllUserID).FirstOrDefault();
                userid = driver.DriverID;
                
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
            TimeSpan timeSpan= TimeSpan.FromMinutes(20);
            JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(timeSpan, _configuration);
            string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType.ToString(), userid);
            return Ok(token);
        }
        #endregion
        [HttpGet("isTokenExpired")]
        [Authorize]
        public string IsTokenExpired()
        {
            return "Geçerli";
        }
    }
}