using AntalyaTaksiAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenderController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        public GenderController(ATAccountContext aTAccountContext)
        {
            _aTAccountContext = aTAccountContext;
        }

        [HttpGet("Get")]
        public async Task<List<Gender>> Get()
        {
            var genders=await _aTAccountContext.Genders.Where(c => c.Activity == 1).ToListAsync();
            return genders;
        }

        
        [HttpGet("Get/{id}")]
        public async Task<Gender> Get(int id)
        {
            var gender =await _aTAccountContext.Genders.Where(c => c.GenderID == id && c.Activity == 1).FirstOrDefaultAsync();
            return gender;
        }

       
        [HttpPost("Post")]
        public async Task<ActionResult> Post(Gender gender)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }
                Gender gender1 = new Gender();
                gender1.GenderName = gender.GenderName;
                _aTAccountContext.Genders.Add(gender1);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT api/<GenderController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GenderController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
