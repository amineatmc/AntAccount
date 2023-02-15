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

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {

        private readonly ATAccountContext _context;
        private readonly DriverNodeService _driverNodeService;

        public PassengerController(ATAccountContext context, DriverNodeService driverNodeService)
        {
            _context = context;
            _driverNodeService = driverNodeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Passenger>>> GetPassengers()
        {
            if (_context.Passengers == null)
            {
                return NotFound();
            }
            return await _context.Passengers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Passenger>> GetPassenger(int id)
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
        public async Task<IActionResult> PutStation(int id, Passenger passenger)
        {
            if (id != passenger.PassengerID)
            {
                return BadRequest();
            }
            AllUserValidator validations = new AllUserValidator();
            var validationResult = validations.Validate(passenger.AllUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            AllUser user1 = await (from c in _context.AllUsers where c.AllUserID == passenger.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
            user1.Name = passenger.AllUser.Name;
            user1.Surname = passenger.AllUser.Surname;
            user1.MailAdress = passenger.AllUser.MailAdress;
            user1.Phone = passenger.AllUser.Phone;

            _context.Entry(passenger).State = EntityState.Modified;
            _context.AllUsers.Update(user1);

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
        public async Task<IActionResult> DeletePassenger(int id)
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

            await _context.SaveChangesAsync();
            _driverNodeService.DeletePassenger(id);

            return NoContent();
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
            allUser.Password = Helper.PasswordEncode("123456");
            allUser.UserType = 2;
          
            AllUserValidator validations = new AllUserValidator();
            var validationResult = validations.Validate(allUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

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
