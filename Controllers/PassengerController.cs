using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

            _context.Entry(passenger).State = EntityState.Modified;

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

            return NoContent();
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
            await _context.SaveChangesAsync();
            //_driverNodeService.DeleteStation(id);

            return NoContent();
        }

        private bool PassengerExists(int id)
        {
            return (_context.Passengers?.Any(e => e.PassengerID == id)).GetValueOrDefault();
        }

        [HttpPost("stationwithstation")]
        public async Task<ActionResult> AddStationWithStation(AddPassengerWithStationRequest addPassengerWithStationRequest)
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
            _context.AllUsers.Add(allUser);

            //i need role build

            Passenger passenger = new Passenger();
            passenger.Activity = 1;
            _context.Passengers.Add(passenger);
            _context.SaveChangesAsync();

            bool resultOfNodeService = await _driverNodeService.SendPassenger(passenger.PassengerID);

            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }

    }
}
