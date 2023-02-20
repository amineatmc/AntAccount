using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AntalyaTaksiAccount.ValidationRules;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using AntalyaTaksiAccount.Services.AntalyaTaksiAccount.Services;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {

        private readonly ATAccountContext _context;
        private readonly DriverNodeServiceOld _driverNodeService;

        public PassengerController(ATAccountContext context, DriverNodeServiceOld driverNodeService)
        {
            _context = context;
            _driverNodeService = driverNodeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            if (_context.Passengers == null)
            {
                return NotFound();
            }
            var passengers= await _context.Passengers.ToListAsync();
            if (passengers.Count > 0)
            {
                foreach (var item in passengers)
                {
                    item.AllUser.Password = "";
                }
            }
            return passengers;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
        {
            if (_context.Passengers == null)
            {
                return NotFound();
            }
            var passenger = await _context.Passengers.FindAsync(id);
            passenger.AllUser.Password = "";
            if (passenger == null)
            {
                return NotFound();
            }

            return passenger;
        }

        [HttpPost]
        public async Task<ActionResult<Passenger>> PostPassenger(Passenger passenger)
        {
            if (_context.Passengers == null)
            {
                return Problem("Entity set 'ATAccountContext.Passengers'  is null.");
            }
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPassenger", new { id = passenger.PassengerID }, passenger);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutStation(int id, Passenger passenger)
        {
            if (id != passenger.PassengerID)
            {
                return BadRequest("All User ID is not valid");
            }
            if (passenger.AllUserID == null)
            {
                return BadRequest("All User ID is not valid");
            }
                if (passenger.AllUserID ==0)
            {
                return BadRequest("All User ID is not valid");
            }

            //AllUserValidator validations = new AllUserValidator();
            //var validationResult = validations.Validate(passenger.AllUser);
            //if (!validationResult.IsValid)
            //{
            //    return BadRequest(validationResult.Errors);
            //}
            AllUser user1 = await (from c in _context.AllUsers where c.AllUserID == passenger.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
            if (user1 == null)
            { return NoContent(); }
            if (passenger.AllUser.Name != null && passenger.AllUser.Name != "")
            {
                user1.Name = passenger.AllUser.Name;
            }
            if (passenger.AllUser.Surname != null && passenger.AllUser.Surname != "")
            {
                user1.Surname = passenger.AllUser.Surname;
            }

            if (passenger.AllUser.MailAdress != null && passenger.AllUser.MailAdress != "")
            {
                user1.MailAdress = passenger.AllUser.MailAdress;
            }

            if (passenger.AllUser.Phone != null && passenger.AllUser.Phone != "")
            {
                user1.Phone = passenger.AllUser.Phone;
            }


            Passenger passenger1 = await (from c in _context.Passengers where c.PassengerID == passenger.PassengerID select c).FirstOrDefaultAsync();
            if (passenger1 == null)
            { return NoContent(); }
            if (passenger.IdNo != null && passenger.IdNo != "")
            {
                passenger1.IdNo = passenger.IdNo;
            }
            if (passenger.Birthday != null)
            {
                passenger1.Birthday = passenger.Birthday;
            }
            if (passenger.Lang != null && passenger.Lang != "")
            {
                passenger1.Lang = passenger.Lang;
            }



            //_context.Entry(passenger).State = EntityState.Modified;
            //_context.AllUsers.Update(user1);
            //_context.Passengers.Update(passenger1);


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PassengerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok("Güncellendi");
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePassenger(int id)
        {
            try
            {
                if (_context.Passengers == null)
                {
                    return NotFound();
                }
                var passenger = await _context.Passengers.FindAsync(id);
                if (passenger == null)
                {
                    return NotFound();
                }
                passenger.Activity = 0;
                var allUser = await _context.AllUsers.FindAsync(passenger.AllUserID);
                allUser.Activity = 0;

                _context.AllUsers.Update(allUser);
                _context.Passengers.Update(passenger);
                await _context.SaveChangesAsync();
                _driverNodeService.DeletePassenger(id);

                return Ok("Kayıt Silindi");
            }
            catch (Exception )
            {
                return BadRequest("hata");
            }
        }

        private bool PassengerExists(int id)
        {
            return (_context.Passengers?.Any(e => e.PassengerID == id)).GetValueOrDefault();
        }

        [HttpPost("passengerwithstation")]
        public async Task<ActionResult> AddPassengerWithStation(AddPassengerWithStationRequest addPassengerWithStationRequest)
        {
            if (!Helper.UnicEmailControl(addPassengerWithStationRequest.MailAddress, _context))
            {
                return BadRequest("Var olan bir email adresi.");
            }

            if (!Helper.UnicPhoneNumberControl(addPassengerWithStationRequest.Phone, _context))
            {
                return BadRequest("Var olan bir Telefon numarası.");
            }

            AllUser allUser = new AllUser();
            allUser.Surname = addPassengerWithStationRequest.Surname;
            allUser.MailVerify = 1;
            allUser.Activity = 1;
            allUser.Name = addPassengerWithStationRequest.Name;
            allUser.MailAdress = addPassengerWithStationRequest.MailAddress;
            allUser.Phone = addPassengerWithStationRequest.Phone;
            allUser.Password ="123456";
            allUser.UserType = 2;

            AllUserValidator validations = new AllUserValidator();
            var validationResult = validations.Validate(allUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            allUser.Password = Helper.PasswordEncode(allUser.Password);
            _context.AllUsers.Add(allUser);

            Passenger passenger = new Passenger();
            passenger.Activity = 1;
            passenger.IdNo = "123";
            passenger.Birthday = DateTime.UtcNow;
            passenger.Pet = false;
            passenger.Travel = false;
            passenger.Banned = false;
            passenger.Lang = "1";
            passenger.Lat = "1";
            passenger.Created = DateTime.UtcNow;
            passenger.AllUser = allUser;
            _context.Passengers.Add(passenger);
            _context.SaveChanges();

            bool resultOfNodeService = await _driverNodeService.SendPassenger(passenger.PassengerID, Convert.ToInt32(passenger.AllUserID));

            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }

    }
}
