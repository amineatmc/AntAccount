using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Services.AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
using AntalyaTaksiAccount.ValidationRules;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.Xml;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {

        private readonly ATAccountContext _aTAccountContext;
        private readonly DriverNodeServiceOld _driverNodeService;
         private readonly ILogger<DriverController> _logger;
        public DriverController(ATAccountContext aTAccountContext, DriverNodeServiceOld driverNodeService,ILogger<DriverController> logger)
        {
             _logger = logger;
            _aTAccountContext = aTAccountContext;
            _driverNodeService = driverNodeService;
        }

        [HttpGet("Get")]
        [Authorize]
        public async Task<List<Driver>> Get()
        {
            try
            {
                var users = await _aTAccountContext.Drivers.Where(c => c.Activity == 1).ToListAsync();
                if (users.Count>0)
                {
                    foreach (var item in users)
                    {
                        item.AllUser.Password = "";
                    }
                }
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
        [Authorize]
        public async Task<Driver> Get(int id)
        {
            try
            {
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.DriverID == id).FirstOrDefaultAsync();
                _logger.LogInformation("test log", DateTime.Now.ToString());
                if (user != null)
                    user.AllUser.Password = "";
                return user;
            }
            catch (Exception)
            {
                _logger.LogInformation("test log", DateTime.Now.ToString());
                Driver user = new Driver();
                return user;
            }
        }
        [HttpGet("GetByUserID/{id}")]
        [Authorize]
        public async Task<Driver> GetByUserID(int id)
        {
            try
            {
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.AllUserID == id).FirstOrDefaultAsync();
                if (user != null)
                    user.AllUser.Password = "";
                return user;
            }
            catch (Exception)
            {
                Driver user = new Driver();
                return user;
            }
        }

        [HttpPost("Post")]
        private async Task<ActionResult> Post(Driver user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }

                Driver user1 = new Driver();


                user1.AllUserID = user.AllUserID;
                user1.Activity = 1;
                _aTAccountContext.Drivers.Add(user1);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult> PutDriver(int id, Driver user)
        {
            try
            {
                if (id != user.DriverID)
                {
                    return BadRequest();
                }


                if (user != null && user.DriverID != 0)
                {
                    Models.Driver driver = await (from c in _aTAccountContext.Drivers where c.DriverID == user.DriverID && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (driver == null) { return NoContent(); }

                    //if (driver.RoleID != user.RoleID)
                    //{
                    //    driver.RoleID = user.RoleID;
                    //}
                    //driver.IdNo = user.IdNo;
                    //driver.DriverLicenseNo = user.DriverLicenseNo;
                    //driver.Rating = user.Rating;
                    //driver.BirthDay = user.BirthDay;
                    driver.Pet = user.Pet;
                    //AllUserValidator validations = new AllUserValidator();
                    //var validationResult = validations.Validate(user.AllUser);
                    //if (!validationResult.IsValid)
                    //{
                    //    return BadRequest(validationResult.Errors);
                    //}
                    //AllUser user2 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == user.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
                    //    user2.Name = user.AllUser.Name;
                    //    user2.Surname = user.AllUser.Surname;
                    //    user2.MailAdress = user.AllUser.MailAdress;
                    //    user2.Phone =  user.AllUser.Phone;


                    //    _aTAccountContext.Entry(user).State = EntityState.Modified;
                    //_aTAccountContext.Drivers.Update(user);
                    //_aTAccountContext.AllUsers.Update(user2);
                }
                try
                {
                    await _aTAccountContext.SaveChangesAsync();
                    //  await _driverNodeService.(id);
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return Ok("Güncellendi");


                //}
                //else return NoContent();
            }
            catch (Exception ex)
            {
                return Problem();
            }
        }

        //[HttpDelete("{id}")]
        //public async void Delete(int id)
        //{
        //    try
        //    {

        //        var user = await (from c in _aTAccountContext.Drivers where c.DriverID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
        //        user.Activity = 0;


        //        _aTAccountContext.Entry(user).State = EntityState.Modified;
        //        _aTAccountContext.Drivers.Update(user);
        //        await _driverNodeService.DeleteDriver(id);
        //        _aTAccountContext.SaveChanges();

        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}


        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDrivers(int id)
        {
            try
            {
                if (_aTAccountContext.Drivers == null)
                {
                    return NotFound();
                }
                var driver = await _aTAccountContext.Drivers.FindAsync(id);
                if (driver == null)
                {
                    return NotFound();
                }
                driver.Activity = 0;

                var allUser = await _aTAccountContext.AllUsers.FindAsync(driver.AllUserID);
                allUser.Activity = 0;

                _aTAccountContext.AllUsers.Update(allUser);
                _aTAccountContext.Drivers.Update(driver);
                await _aTAccountContext.SaveChangesAsync();
                _driverNodeService.DeleteDriver(id);

                return Ok("Kayıt Silindi");
            }
            catch (Exception)
            {
                return BadRequest("hata");
            }
        }


        [HttpPost("driverwithstation")]
        public async Task<ActionResult> AddDriverWithStation(AddDriverWithStationRequest addDriverWithStation)
        {
            if (!Helper.UnicEmailControl(addDriverWithStation.MailAddress, _aTAccountContext))
            {
                return BadRequest("Var olan bir email adresi.");
            }

            if (!Helper.UnicIdNoControl(addDriverWithStation.IdNo, _aTAccountContext))
            {
                return BadRequest("Var olan bir Id Numarası.");
            }

            if (!Helper.UnicPhoneNumberControl(addDriverWithStation.Phone, _aTAccountContext))
            {
                return BadRequest("Var olan bir Telefon numarası.");
            }

            AllUser allUser = new AllUser();
            allUser.Surname = addDriverWithStation.Surname;
            allUser.MailVerify = 1;
            allUser.Activity = 1;
            allUser.Name = addDriverWithStation.Name;
            allUser.MailAdress = addDriverWithStation.MailAddress;
            allUser.Phone = addDriverWithStation.Phone;
            allUser.Password ="123456";
            allUser.UserType = 1;

            AllUserValidator validations = new AllUserValidator();
            var validationResult = validations.Validate(allUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            allUser.Password = Helper.PasswordEncode(allUser.Password);
            _aTAccountContext.AllUsers.Add(allUser);

            Driver driver = new Driver();
            driver.IdNo = addDriverWithStation.IdNo;
            driver.Ip = "0.0.0.0";
            driver.BirthDay = addDriverWithStation.Birthday;
            driver.CreatedDate = DateTime.UtcNow;
            driver.AllUser = allUser;
            driver.DriverLicenseNo = addDriverWithStation.DriverLicenseNo;
            driver.Rating = 1;
            driver.StationID = addDriverWithStation.StationID.Value;
            driver.Penalty = false;
            driver.Pet = false;
            driver.RoleID = 1;
            driver.Activity = 1;
            driver.AllUser = allUser;

            //RoleController roleController = new RoleController(_aTAccountContext);
            //Role role=await roleController.Get(1);
            //driver.Role = role;
            //driver.Activity = 1;
            //allUser.UserType = role.RoleID;

            //var stationController = new StationsController(_aTAccountContext, null);
            //var stationResult = await stationController.GetStation(10);
            // driver.Station = stationResult.Value;  //Todo Get From Request.

            _aTAccountContext.Add(driver);

            await _aTAccountContext.SaveChangesAsync();

            bool resultOfNodeService = await _driverNodeService.SendDriver(driver.DriverID, driver.StationID, driver.AllUser.AllUserID);
            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }


        [HttpPut("PutDriverPanel")]
        public async Task<ActionResult> PutDriverPanel(int id, Driver driver)
        {
            if (id != driver.DriverID)
            {
                return BadRequest();
            }
            if (driver.AllUserID != null && driver.AllUserID != 0)
            {
                return BadRequest();
            }
            //AllUserValidator validations = new AllUserValidator();
            //var validationResult = validations.Validate(passenger.AllUser);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.Errors);
            //}
            AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == driver.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
            if (user1 == null)
            { return NoContent(); }
            if (driver.AllUser.Name != null && driver.AllUser.Name != "")
            {
                user1.Name = driver.AllUser.Name;
            }
            if (driver.AllUser.Surname != null && driver.AllUser.Surname != "")
            {
                user1.Surname = driver.AllUser.Surname;
            }

            if (driver.AllUser.MailAdress != null && driver.AllUser.MailAdress != "")
            {
                user1.MailAdress = driver.AllUser.MailAdress;
            }

            if (driver.AllUser.Phone != null && driver.AllUser.Phone != "")
            {
                user1.Phone = driver.AllUser.Phone;
            }


            Driver user = await (from c in _aTAccountContext.Drivers where c.DriverID == driver.DriverID select c).FirstOrDefaultAsync();
            if (user == null)
            { return NoContent(); }
            if (driver.IdNo != null && driver.IdNo != "")
            {
                user.IdNo = driver.IdNo;
            }
            if (user.BirthDay != null)
            {
                user.BirthDay = driver.BirthDay;
            }

                await _aTAccountContext.SaveChangesAsync();
           
            return Ok("Güncellendi");
        }

        [HttpPut("PutDriverPanel2")]
        public async Task<ActionResult> PutDriverPanel2(int id, PutDriverRequest putDriverRequest)
        {
            if (id != putDriverRequest.DriverID)
            {
                return BadRequest();
            }
            var driver=await _aTAccountContext.Drivers.Where(d => d.DriverID == putDriverRequest.DriverID && d.Activity == 1).FirstOrDefaultAsync();
            if(driver == null)
            {
                return NoContent();
            }



            var allUserID=driver.AllUserID;

            AllUser user1 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == driver.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();

            if (user1 == null)
            {
                return NoContent();
            }

            if (putDriverRequest.DriverLicenseNo != null) { 
            
                driver.DriverLicenseNo = putDriverRequest.DriverLicenseNo;
            }
            if (putDriverRequest.BirthDay != null)
            {
                driver.BirthDay = putDriverRequest.BirthDay.Value;

            }
            if (putDriverRequest.Pet != null)
            {
                driver.Pet = putDriverRequest.Pet.Value;
            }
            if (putDriverRequest.Penalty != null)
            {
                driver.Penalty = putDriverRequest.Penalty.Value;
            }
            if (putDriverRequest.IDNo != null)
            {
                driver.IdNo = putDriverRequest.IDNo;
            }
            if (putDriverRequest.StationID != null)
            {
                driver.StationID = Convert.ToInt32(putDriverRequest.StationID);
            }

            if (driver.AllUser.Name != null && driver.AllUser.Name != "")
            {
                user1.Name = putDriverRequest.Name;
            }
            if (driver.AllUser.Surname != null && driver.AllUser.Surname != "")
            {
                user1.Surname = putDriverRequest.Surname;
            }

            if (driver.AllUser.MailAdress != null && driver.AllUser.MailAdress != "")
            {
                user1.MailAdress= putDriverRequest.MailAdress;
            }

            if (driver.AllUser.Phone != null && driver.AllUser.Phone != "")
            {
                user1.Phone = putDriverRequest.Phone;
            }

            await _aTAccountContext.SaveChangesAsync();

            bool resultOfNodeService = await _driverNodeService.UpdateDriver(id, Convert.ToInt32(putDriverRequest.StationID));
            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }

    }
}
