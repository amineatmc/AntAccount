using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Authentication;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ATAccountContext _aTAccountContext;
        private IConfiguration _configuration;
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public LoginController(ATAccountContext aTAccountContext, IConfiguration configuration, SignInManager<ApplicationUser> signInManager)
        {
            _aTAccountContext = aTAccountContext;
            _configuration = configuration;
            _signInManager = signInManager;
        }

        [HttpGet("GetTest")]
        public async Task GetTest()
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        [HttpPost("LoginUser")]
        public async Task<ActionResult<string>> LoginUser(SignIn signIn)
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

                JwtTokenGenerator jwtTokenGenerator = new JwtTokenGenerator(_configuration);
                string token = jwtTokenGenerator.Generate(user.Name, user.MailAdress);
                

                return Ok(token);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpGet("GoogleLogin")]
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string redirectUrl = Url.Action("ExternalResponse", "Login", new { ReturnUrl = ReturnUrl });
            //Google'a yapılan Login talebi neticesinde kullanıcıyı yönlendirmesini istediğimiz url'i oluşturuyoruz.
            AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            //Bağlantı kurulacak harici platformun hangisi olduğunu belirtiyor ve bağlantı özelliklerini elde ediyoruz.
            return new ChallengeResult("Google", properties);
            //ChallengeResult; kimlik doğrulamak için gerekli olan tüm özellikleri kapsayan AuthenticationProperties nesnesini alır ve ayarlar.
        }

        [HttpGet("ExternalResponse")]
        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo loginInfo = await _signInManager.GetExternalLoginInfoAsync();
            //Kullanıcıyla ilgili dış kaynaktan gelen tüm bilgileri taşıyan nesnedir.
            //Bu nesnesnin 'LoginProvider' propertysinin değerine göz atarsanız eğer hangi dış kaynaktan geliniyorsa onun bilgisinin yazdığını göreceksiniz.
            if (loginInfo == null)
                return RedirectToAction("Login");
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult loginResult = await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                //Giriş yapıyoruz.
                if (loginResult.Succeeded)
                    return Redirect(ReturnUrl);
                //else
                //{
                //    //Eğer ki akış bu bloğa girerse ilgili kullanıcı uygulamamıza kayıt olmadığından dolayı girişi başarısız demektir.
                //    //O halde kayıt işlemini yapıp, ardından giriş yaptırmamız gerekmektedir.
                //    AppUser user = new AppUser
                //    {
                //        Email = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value,
                //        UserName = loginInfo.Principal.FindFirst(ClaimTypes.Email).Value
                //    };
                //    //Dış kaynaktan gelen Claimleri uygun eşlendikleri propertylere atıyoruz.
                //    IdentityResult createResult = await _userManager.CreateAsync(user);
                //    //Kullanıcı kaydını yapıyoruz.
                //    if (createResult.Succeeded)
                //    {
                //        //Eğer kayıt başarılıysa ilgili kullanıcı bilgilerini AspNetUserLogins tablosuna kaydetmemiz gerekmektedir ki
                //        //bir sonraki dış kaynak login talebinde Identity mimarisi ilgili kullanıcının hangi dış kaynaktan geldiğini anlayabilsin.
                //        IdentityResult addLoginResult = await _userManager.AddLoginAsync(user, loginInfo);
                //        //Kullanıcı bilgileri dış kaynaktan gelen bilgileriyle AspNetUserLogins tablosunda eşleştirilmek suretiyle kaydedilmiştir.
                //        if (addLoginResult.Succeeded)
                //        {
                //            await _signInManager.SignInAsync(user, true);
                //            //await _signInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, true);
                //            return Redirect(ReturnUrl);
                //        }
                //    }

                //}
            }
            return Redirect(ReturnUrl);
        }
    }
}
