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
       // private readonly ILogger<DriverController> _logger;
        public DriverController( ATAccountContext aTAccountContext)
        {
           // _logger = logger;
            _aTAccountContext = aTAccountContext;
        }

        [HttpGet("Get")]
        public async Task<List<Driver>> Get()
        {
            try
            {
                var users = await _aTAccountContext.Drivers.Where(c => c.Activity == 1).Include(c => c.Role).ToListAsync();

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
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.DriverID == id).Include(c => c.Role).FirstOrDefaultAsync();
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
                var user = await _aTAccountContext.Drivers.Where(c => c.Activity == 1 && c.AllUserID == id).Include(c => c.Role).FirstOrDefaultAsync();
                return user;
            }
            catch (Exception)
            {
                Driver user = new Driver();
                return user;
            }
        }

        [HttpPost("Post")]
        public async Task<ActionResult> Post(Driver user)
        {
            try
            {
               
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }

                Driver user1 = new Driver();
               
                
               user1.AllUserID= user.AllUserID;
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

        [HttpPut("Put")]
        public async Task<ActionResult> Put(Driver user)
        {
            try
            {
                if (user != null && user.DriverID != 0)
                {
                    Models.Driver user1 = await (from c in _aTAccountContext.Drivers where c.DriverID == user.DriverID && c.Activity == 1 select c).FirstOrDefaultAsync();
                    if (user1 == null) { return NoContent(); }
                   
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
                Models.Driver user =await (from c in _aTAccountContext.Drivers where c.DriverID == id && c.Activity == 1 select c).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
            }
        }
        
    }
}
