using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
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
        private readonly DriverNodeService _driverNodeService;
        // private readonly ILogger<DriverController> _logger;
        public DriverController(ATAccountContext aTAccountContext, DriverNodeService driverNodeService)
        {
            // _logger = logger;
            _aTAccountContext = aTAccountContext;
            _driverNodeService = driverNodeService;
        }

        [HttpGet("Get")]
        public async Task<List<Driver>> Get()
        {
            try
            {
                var users = await _aTAccountContext.Drivers.Where(c => c.Activity == 1).ToListAsync();

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
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.DriverID == id).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception)
            {
                Driver user = new Driver();
                return user;
            }
        }
        [HttpGet("GetByUserID/{id}")]
        public async Task<Driver> GetByUserID(int id)
        {
            try
            {
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.AllUserID == id).FirstOrDefaultAsync();
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
        public async Task<ActionResult> PutDriver(int id,Driver user)
        {
            try
            {
                if (id != user.DriverID)
                {
                    return BadRequest();
                }


                //if (user != null && user.DriverID != 0)
                //{
                //    Models.Driver driver = await (from c in _aTAccountContext.Drivers where c.DriverID == user.DriverID && c.Activity == 1 select c).FirstOrDefaultAsync();
                //    if (driver == null) { return NoContent(); }

                //    if (driver.RoleID != user.RoleID)
                //    {
                //        driver.RoleID = user.RoleID;
                //    }
                    //driver.IdNo=user.IdNo;
                    //driver.DriverLicenseNo=user.DriverLicenseNo;
                    //driver.Rating=user.Rating;
                    //driver.BirthDay=user.BirthDay;
                    //driver.Pet=user.Pet;

                    AllUser user2 = await (from c in _aTAccountContext.AllUsers where c.AllUserID == user.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
                    user2.Name = user.AllUser.Name;
                    user2.Surname = user.AllUser.Surname;
                    user2.MailAdress = user.AllUser.MailAdress;
                    user2.Phone =  user.AllUser.Phone;


                    _aTAccountContext.Entry(user).State = EntityState.Modified;
                _aTAccountContext.Drivers.Update(user);
                _aTAccountContext.AllUsers.Update(user2);

                try
                {
                    await _aTAccountContext.SaveChangesAsync();
                  //  await _driverNodeService.(id);
                }
                catch (DbUpdateConcurrencyException)
                {                   
                  
                }
                return Ok("Güncellendi");


                return Ok("Kayıt Güncellendi.");
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
        public async Task<IActionResult> DeleteDrivers(int id)
        {
            if (_aTAccountContext.Drivers == null)
            {
                return NotFound();
            }
            var passenger = await _aTAccountContext.Drivers.FindAsync(id);
            if (passenger == null)
            {
                return NotFound();
            }
            passenger.Activity = 0;

            var allUser = await _aTAccountContext.AllUsers.FindAsync(passenger.AllUserID);
            allUser.Activity = 0;

            _aTAccountContext.AllUsers.Update(allUser);
            await _aTAccountContext.SaveChangesAsync();
            _driverNodeService.DeleteDriver(id);

            return NoContent();
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
            allUser.Password = Helper.PasswordEncode("123456");
            allUser.UserType = 1;
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

            bool resultOfNodeService = await _driverNodeService.SendDriver(driver.DriverID, driver.StationID,driver.AllUser.AllUserID);
            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }

    }
}
