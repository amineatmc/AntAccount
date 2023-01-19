using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ATAccountContext _aTAccountContext;
        public UserController(IConfiguration configuration, ATAccountContext aTAccountContext)
        {
            _configuration = configuration;
            _aTAccountContext = aTAccountContext;
        }

        [HttpGet("Get")]
        public async Task<List<User>> Get()
        {
            try
            {
                var users = await _aTAccountContext.Users.Where(c => c.Activity == 1).Include(c=>c.Role).Include(c=>c.Company).Include(c=>c.Department).Include(c=>c.Gender).ToListAsync();
                return users;
            }
            catch (Exception)
            {
                return new List<User>();
            }
        }


        [HttpGet("Get/{id}")]
        public async Task<User> Get(int id)
        {
            try
            {
                var user = await _aTAccountContext.Users.Where(c => c.Activity == 1 && c.UserID == id).Include(c => c.Role).Include(c => c.Company).Include(c => c.Department).Include(c => c.Gender).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception)
            {
                User user = new User();
                return user;
            }
        }
        [HttpGet("GetByMail/{mailAdress}")]
        public async Task<User> GetByMail(string mailAdress)
        {
            try
            {
               
                Task<User> user = (from c in _aTAccountContext.Users where c.Activity == 1 && c.MailAdress == mailAdress select c).Include(c => c.Role).Include(c => c.Company).Include(c => c.Department).Include(c => c.Gender).FirstAsync();
                var user1 = await user;
                return user1;

            }
            catch (Exception)
            {
                return new User();
            }
        }

        [HttpPost("Post")]
        public async Task<ActionResult> Post(User user)
        {
            try
            {
                if (!Helper.UnicEmailControl(user.MailAdress, _aTAccountContext))
                {
                    return BadRequest("Var olan bir email adresi.");
                }

                // string tempPassword = Helper.GeneratePassword();
                //user.Password = Helper.PasswordEncode(tempPassword);

                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }

                User user1 = new User();
                user1.Name = user.Name;
                user1.Surname = user.Surname;
                user1.RoleID = user.RoleID;
                user1.MailAdress = user.MailAdress;
                user1.PasswordChangeDate = DateTime.Now;

                //must do ui
                //if (Helper.PasswordControl(signUp.User.Password))
                user1.Password = Helper.PasswordEncode(user.Password);
                //else
                //   return BadRequest("Password istenen şartlarda değil.");
                user1.Phone = user.Phone;
                user1.ProfilePhotoPath = user.ProfilePhotoPath;
                user1.MailVerify = 0;
                //user create own password. Below field should be 0 if system create first password.
                user1.ResetPasswordVerify = 1;
                user1.InsertedDate = DateTime.Now;
                user1.RoleID = user.RoleID;
                user1.GenderID = user.GenderID;
                user1.DepartmentID = user.DepartmentID;
                user1.CompanyID = user.CompanyID;
                user1.PasswordExpiration = 60;
                user1.Activity = 1;
                _aTAccountContext.Users.Add(user1);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public async Task<ActionResult> VerifyMail(string mailAdress)
        {
            try
            {
                var user = await _aTAccountContext.Users.Select(u => u).Where(u => u.MailAdress.Equals(mailAdress)).FirstOrDefaultAsync();
                User user1 = user;
                if (user1 != null)
                {
                    user1.MailVerify = 1;
                    _aTAccountContext.SaveChanges();
                    return Ok("Kayıt Eklendi.");
                }
                else return NoContent();
            }
            catch (Exception ex)
            {
                return Problem("hata.");
            }
        }
        [HttpGet("ForgotMyPasssword/{mail}")]
        public async Task<ActionResult> ForgotMyPasssword(string mail)
        {
            try
            {
                Models.User user = await GetByMail(mail);
                if (user != null)
                {
                    string newPass = Helper.GeneratePassword();
                    bool ResetPassControl = ResetPassword(user.UserID, newPass);
                    if (ResetPassControl)
                    {
                       // await Task.Run(() => Helper.SendMail(user.Email, "Yeni şifreniz: " + newPass + "\nLütfen şifrenizi değiştiriniz.", "BAMS Şifre Değişikliği"));
                        return Ok("Şifre Değiştirilmiştir.");
                    }
                    else
                        return BadRequest("Yeni Şifre Kaydedilemedi. Tekrar Deneyiniz.");
                }
                else
                    return NoContent();
            }
            catch (Exception)
            {

                return Problem();
            }
        }
        private bool ResetPassword(int id, string NewPassword)
        {
            try
            {
                Models.User user = (from c in _aTAccountContext.Users where c.UserID == id && c.Activity == 1 select c).FirstOrDefault();
                if (user != null)
                {
                    string sifre = Helper.PasswordEncode(NewPassword);
                    user.Password = sifre;
                    user.ResetPasswordVerify = 0;
                    _aTAccountContext.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
