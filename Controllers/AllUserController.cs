using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Services.AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllUserController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        private readonly ILogger<AllUserController> _logger;
        private readonly DriverNodeServiceOld _driverNodeService;

        public AllUserController(ILogger<AllUserController> logger, ATAccountContext aTAccountContext, DriverNodeServiceOld driverNodeService)
        {
            _logger = logger;
            _aTAccountContext = aTAccountContext;
            _driverNodeService = driverNodeService;
        }
        [HttpGet("Get")]
        public async Task<List<AllUser>> Get()
        {
            try
            {
                var users = await _aTAccountContext.AllUsers.Where(c => c.Activity == 1).ToListAsync();

                _logger.LogInformation("istek geldi");

                return users;
            }
            catch (Exception ex)
            {
                return new List<AllUser>();
            }
        }
        [HttpGet("GetByMail/{mailAdress}")]
        [Authorize]
        private async Task<AllUser> GetByMail(string mailAdress)
        {
            try
            {
                Task<AllUser> user = (from c in _aTAccountContext.AllUsers where c.Activity == 1 && c.MailAdress == mailAdress select c).FirstAsync();
                var user1 = await user;
                return user1;

            }
            catch (Exception)
            {
                return new AllUser();
            }
        }
        [HttpPost("Post")]
        public async Task<ActionResult> Post(AllUser user)
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

                AllUser user1 = new AllUser();
                user1.Name = user.Name;
                user1.Surname = user.Surname;
                user1.MailAdress = user.MailAdress;
                user1.Password = Helper.PasswordEncode(user.Password);
                user1.Phone = user.Phone;
                user1.MailVerify = 1;
                user1.Activity = 1;
                user1.UserType = user.UserType;
                _aTAccountContext.AllUsers.Add(user1);
                _aTAccountContext.SaveChanges();

                switch (user.UserType)
                {
                    case 1:
                        Driver driver = new Driver();
                        driver.Activity = 1;
                        driver.AllUserID = user1.AllUserID;
                        driver.StationID = 10;
                        driver.RoleID = 1;
                        driver.CreatedDate = DateTime.Now;
                        _aTAccountContext.Drivers.Add(driver);
                        _aTAccountContext.SaveChanges();
                        break;
                    case 2:
                        Passenger passenger = new Passenger();
                        passenger.Activity = 1;
                        passenger.AllUserID = user1.AllUserID;
                        passenger.IdNo = "0";
                        passenger.Created = DateTime.Now;
                        passenger.Birthday = DateTime.Now;
                        passenger.Pet = true;
                        passenger.Travel = false;
                        passenger.Disabled= false;
                        passenger.Banned= false;
                        passenger.Lang= "tr";
                        passenger.Lat = "";

                        _aTAccountContext.Passengers.Add(passenger);
                        _aTAccountContext.SaveChanges();

                        bool resultOfNodeService = await _driverNodeService.SendPassenger(passenger.PassengerID,Convert.ToInt32(passenger.AllUserID));
                        break;
                    case 3:
                        Station station = new Station();
                        station.Activity = 1;
                        station.AllUserID = user1.AllUserID;
                        station.CreatedDate = DateTime.Now;
                        station.StationStatu = true;
                        station.StationAuto = true;
                        _aTAccountContext.Stations.Add(station);
                        _aTAccountContext.SaveChanges();
                        break;

                    default:
                        break;
                }

                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost("Post2")]
        public async Task<ActionResult> Add(AllUser user)
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


                AllUser user1 = new AllUser();
                user1.Name = user.Name;
                user1.Surname = user.Surname;
                user1.MailAdress = user.MailAdress;
                user1.Password = Helper.PasswordEncode(user.Password);
                user1.Phone = user.Phone;
                user1.MailVerify = 0;
                user1.Activity = 1;
                user1.UserType = 1;
                _aTAccountContext.AllUsers.Add(user1);
                _aTAccountContext.SaveChanges();


                Driver driver = new Driver();
                driver.Activity = 1;
                driver.AllUserID = user1.AllUserID;
                driver.StationID = 10;
                driver.RoleID = 1;
                driver.CreatedDate = DateTime.Now;
                _aTAccountContext.Drivers.Add(driver);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("Put")]
        [Authorize]
        public async Task<ActionResult> Put(AllUser user)
        {
            try
            {
                if (user != null && user.AllUserID != 0)
                {
                    AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == user.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
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
        [HttpPut("PutPasssword")]
        [Authorize]
        public async Task<ActionResult> Put(int id, string password)
        {
            var userClaims = Request.HttpContext.User.Claims;
            var userId = int.Parse(userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            try
            {
                if (id==userId)
                {
                    AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (user1 == null) { return NoContent(); }

                    user1.Password = Helper.PasswordEncode(password);
                    _aTAccountContext.AllUsers.Update(user1);
                    _aTAccountContext.SaveChanges();
                    return Ok("Kayıt Güncellendi.");
                }
                if (id != 0)
                {
                    AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (user1 == null) { return NoContent(); }

                    user1.Password = Helper.PasswordEncode(password);
                    _aTAccountContext.AllUsers.Update(user1);
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
        [Authorize]
        public async void Delete(int id)
        {
            try
            {
                AllUser user = await (from c in _aTAccountContext.AllUsers where c.AllUserID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
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
                var user = await _aTAccountContext.AllUsers.Select(u => u).Where(u => u.MailAdress.Equals(mailAdress)).FirstOrDefaultAsync();
                AllUser user1 = user;
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
                Models.AllUser user = await GetByMail(mail);
                if (user != null)
                {
                    string newPass = Helper.GeneratePassword();
                    bool ResetPassControl = ResetPassword(user.AllUserID, newPass);
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
        [HttpGet]
        private bool ResetPassword(int id, string NewPassword)
        {
            try
            {
                Models.AllUser user = (from c in _aTAccountContext.AllUsers where c.AllUserID == id && c.Activity == 1 select c).FirstOrDefault();
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
