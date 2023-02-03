using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AntalyaTaksiAccount.Models;
using AntalyaTaksiAccount.Models.DummyModels;
using AntalyaTaksiAccount.Utils;
using AntalyaTaksiAccount.Services;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StationsController : ControllerBase
    {
        private readonly ATAccountContext _context;
        private readonly DriverNodeService _driverNodeService;

        public StationsController(ATAccountContext context, DriverNodeService driverNodeService)
        {
            _context = context;
        }

        // GET: api/Stations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
          if (_context.Stations == null)
          {
              return NotFound();
          }
            return await _context.Stations.ToListAsync();
        }

        // GET: api/Stations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Station>> GetStation(int id)
        {
          if (_context.Stations == null)
          {
              return NotFound();
          }
            var station = await _context.Stations.FindAsync(id);

            if (station == null)
            {
                return NotFound();
            }

            return station;
        }

        // PUT: api/Stations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStation(int id, Station station)
        {
            if (id != station.StationID)
            {
                return BadRequest();
            }

            _context.Entry(station).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StationExists(id))
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

        // POST: api/Stations
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Station>> PostStation(Station station)
        {
          if (_context.Stations == null)
          {
              return Problem("Entity set 'ATAccountContext.Stations'  is null.");
          }
            _context.Stations.Add(station);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStation", new { id = station.StationID }, station);
        }

        // DELETE: api/Stations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStation(int id)
        {
            if (_context.Stations == null)
            {
                return NotFound();
            }
            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            station.Activity = 0;
            await _context.SaveChangesAsync();

            _driverNodeService.DeleteStation(id);

            return NoContent();
        }

        private bool StationExists(int id)
        {
            return (_context.Stations?.Any(e => e.StationID == id)).GetValueOrDefault();
        }

        [HttpPost("stationwithstation")]
        public async Task<ActionResult> AddStationWithStation(AddStationWithStationRequest addStationWithStationRequest)
        {
            if (!Helper.UnicEmailControl(addStationWithStationRequest.MailAddress, _context))
            {
                return BadRequest("Var olan bir email adresi.");
            }

            if (!Helper.UnicPhoneNumberControl(addStationWithStationRequest.Phone, _context))
            {
                return BadRequest("Var olan bir Telefon numarası.");
            }

            AllUser allUser = new AllUser();
            allUser.Surname = addStationWithStationRequest.Surname;
            allUser.MailVerify = 1;
            allUser.Activity = 1;
            allUser.Name = addStationWithStationRequest.Name;
            allUser.MailAdress = addStationWithStationRequest.MailAddress;
            allUser.Phone = addStationWithStationRequest.Phone;
            allUser.Password = Helper.PasswordEncode("123456");
            _context.AllUsers.Add(allUser);
            allUser.UserType = 3;

            Station station = new Station();
            station.Latitude = addStationWithStationRequest.Latitude;
            station.Longitude = addStationWithStationRequest.Longtitude;
            station.Activity = 1;
            station.AllUser = allUser;
            station.CreatedDate = DateTime.UtcNow;
            station.StationArea = addStationWithStationRequest.StationArea;
            

            _context.Stations.Add(station);

            _context.SaveChangesAsync();

            bool resultOfNodeService=await _driverNodeService.SendStation(station.StationID, addStationWithStationRequest.Latitude, addStationWithStationRequest.Longtitude);

            if (!resultOfNodeService)
            {
                //TODO Add POlly for this logic. 
            }

            return Ok();
        }

    }
}
