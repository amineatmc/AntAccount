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
using Microsoft.AspNetCore.Authorization;
using AntalyaTaksiAccount.Services.AntalyaTaksiAccount.Services;
using AntalyaTaksiAccount.ValidationRules;

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
            _driverNodeService = driverNodeService;
        }

        // GET: api/Stations
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Station>>> GetStations()
        {
            if (_context.Stations == null)
            {
                return NotFound();
            }
            var stations= await _context.Stations.ToListAsync();
            if (stations.Count>0)
            {
                foreach (var item in stations)
                {
                    item.AllUser.Password = "";
                }
            }
            return stations;
        }

        // GET: api/Stations/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Station>> GetStation(int id)
        {
            if (_context.Stations == null)
            {
                return NotFound();
            }
            var station = await _context.Stations.FindAsync(id);
            station.AllUser.Password = "";
            if (station == null)
            {
                return NotFound();
            }

            return station;
        }

        // PUT: api/Stations/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutStation(int id, Station station)
        {
            if (id != station.StationID)
            {
                return BadRequest();
            }


            AllUser user2 = await (from c in _context.AllUsers where c.AllUserID == station.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
            user2.Name = station.AllUser.Name;
            user2.Surname = station.AllUser.Surname;
            user2.MailAdress = station.AllUser.MailAdress;
            user2.Phone = station.AllUser.Phone;


            _context.Entry(station).State = EntityState.Modified;
            _context.Stations.Update(station);
            _context.AllUsers.Update(user2);


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

            return Ok();
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
        [Authorize]
        public async Task<IActionResult> DeleteStation(int id)
        {
            //Station ss= await (from c in _context.Stations where c.StationID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
            //if (ss == null)
            //{
            //    return NotFound("Kayıt Bulunamadı");
            //}
            try
            {
                if (_context.Stations == null)
                {
                    return NotFound("Kayıt Bulunamadı");
                }
                var station = await _context.Stations.FindAsync(id);
                if (station == null)
                {
                    return NotFound("Kayıt Bulunamadı");
                }
                station.Activity = 0;
                AllUser user2 = await (from c in _context.AllUsers where c.AllUserID == station.AllUserID && c.Activity == 1 select c).FirstOrDefaultAsync();
                if (user2 == null)
                {
                    return NotFound("Kayıt Bulunamadı");
                }
                user2.Activity = 0;

                _context.AllUsers.Update(user2);
                _context.Stations.Update(station);
                await _context.SaveChangesAsync();

                _driverNodeService.DeleteStation(id);

                return Ok("Kayıt Silindi");
            }
            catch (Exception)
            {
                return BadRequest();
            }
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
            allUser.Password = "123456";
            allUser.UserType = 3;

            AllUserValidator validations = new AllUserValidator();
            var validationResult = validations.Validate(allUser);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            allUser.Password = Helper.PasswordEncode(allUser.Password);
            _context.AllUsers.Add(allUser);

            Station station = new Station();
            station.Latitude = addStationWithStationRequest.Latitude;
            station.Longitude = addStationWithStationRequest.Longtitude;
            station.Activity = 1;
            station.AllUser = allUser;
            station.CreatedDate = DateTime.UtcNow;
            station.StationArea = addStationWithStationRequest.StationArea;
            station.StationNumber = 0;
            station.StationStatu = false;
            station.Ip = "0.0.0.0";
            _context.Add(station);

            _context.SaveChanges();
            try
            {                   
                bool resultOfNodeService = await _driverNodeService.SendStation(station.StationID, station.Latitude, station.Longitude, Convert.ToInt32(station.AllUserID));
                if (!resultOfNodeService)
                {
                    //TODO Add POlly for this logic. 
                }

                return Ok();
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
            
        }

    }
}
