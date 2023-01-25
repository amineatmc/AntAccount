using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {

        private readonly ATAccountContext _aTAccountContext;
        private readonly ILogger<DriverController> _logger;
        public DriverController(ILogger<DriverController> logger, ATAccountContext aTAccountContext)
        {
            _logger = logger;
            _aTAccountContext = aTAccountContext;
        }

        [HttpGet("Get")]
        public async Task<List<Driver>> Get()
        {
            try
            {
                var users = await _aTAccountContext.Users.Where(c => c.Activity == 1).Include(c => c.Role).ToListAsync();

                return users;
            }
            catch (Exception)
            {
                //Serilog.Sinks.MSSqlServer use
                // _logger.LogInformation("test log", DateTime.Now.ToString());
                return new List<Driver>();
            }
        }


        [HttpGet("Get/{id}")]
        public async Task<Driver> Get(int id)
        {
            try
            {
                var user = await _aTAccountContext.Users.Where(c => c.Activity == 1 && c.UserID == id).Include(c => c.Role).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception)
            {
                Driver user = new Driver();
                return user;
            }
        }
        [HttpGet("GetByMail/{mailAdress}")]
        private async Task<Driver> GetByMail(string mailAdress)
        {
            try
            {

                Task<Driver> user = (from c in _aTAccountContext.Users where c.Activity == 1 && c.MailAdress == mailAdress select c).Include(c => c.Role).FirstAsync();
                var user1 = await user;
                return user1;

            }
            catch (Exception)
            {
                return new Driver();
            }
        }

        [HttpPost("Post")]
        public async Task<ActionResult> Post(Driver user)
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

                Driver user1 = new Driver();
                user1.Name = user.Name;
                user1.Surname = user.Surname;
                user1.RoleID = user.RoleID;
                user1.MailAdress = user.MailAdress;
               

                
                user1.Password = Helper.PasswordEncode(user.Password);
               
                user1.Phone = user.Phone;
              
                user1.MailVerify = 0;
             
                user1.RoleID = user.RoleID;
              
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

        [HttpPut("Put")]
        public async Task<ActionResult> Put(Driver user)
        {
            try
            {
                if (user != null && user.UserID != 0)
                {
                    Models.Driver user1 = await (from c in _aTAccountContext.Users where c.UserID == user.UserID && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (user1 == null) { return NoContent(); }
                    if (user1.Name != user.Name)
                    {
                        user1.Name = user.Name;
                    }
                    if (user1.Surname != user.Surname)
                    {
                        user1.Surname = user.Surname;
                    }
                   
                    if (user1.MailAdress != user.MailAdress)
                    {
                        user1.MailAdress = user.MailAdress;
                        user1.MailVerify = 0;
                    }
                    if (user1.Phone != user.Phone)
                    {
                        user1.Phone = user.Phone;
                    }
                    if (user1.RoleID != user.RoleID)
                    {
                        user1.RoleID = user.RoleID;
                    }
                    _aTAccountContext.SaveChanges();
                    return Ok("Kayıt Güncellendi.");
                }
                else return NoContent();
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }

        [HttpDelete("{id}")]
        public async void Delete(int id)
        {
            try 
            {
                Models.Driver user =await (from c in _aTAccountContext.Users where c.UserID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
            }
        }
        [HttpGet("VerifyMail/{mail}")]
        public async Task<ActionResult> VerifyMail(string mailAdress)
        {
            try
            {
                var user = await _aTAccountContext.Users.Select(u => u).Where(u => u.MailAdress.Equals(mailAdress)).FirstOrDefaultAsync();
                Driver user1 = user;
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
                Models.Driver user = await GetByMail(mail);
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
                Models.Driver user = (from c in _aTAccountContext.Users where c.UserID == id && c.Activity == 1 select c).FirstOrDefault();
                if (user != null)
                {
                    string sifre = Helper.PasswordEncode(NewPassword);
                    user.Password = sifre;
                   
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
