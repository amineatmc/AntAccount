﻿using AntalyaTaksiAccount.Models;
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
        //private readonly Otp _otp;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ATAccountContext _aTAccountContext;
        private IConfiguration _configuration;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public LoginController(ATAccountContext aTAccountContext, IConfiguration configuration)
        {
            _aTAccountContext = aTAccountContext;
            _configuration = configuration;
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

                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username && c.Password == encodedPassword).FirstOrDefaultAsync().Result;
                }
                else
                {
                    user = _aTAccountContext.AllUsers.Where(c => c.MailAdress == signIn.username).FirstOrDefaultAsync().Result;
                }
                if (user == null)
                {
                   return  BadRequest("Mail or Password is invalid");
                }

                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType);
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


                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.AllUserID, user.Name, user.MailAdress, user.UserType);
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
            Otp _otp = new Otp(_aTAccountContext, "");
            var result = _otp.CheckOtpSendMethod(checkOtpDto);
            return Ok(result);
        }
    }
}