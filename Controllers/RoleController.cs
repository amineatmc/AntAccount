using AntalyaTaksiAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        public RoleController(ATAccountContext aTAccountContext)
        {
            _aTAccountContext= aTAccountContext;
        }
        [HttpGet("Get")]
        public Task<List<Role>> Get()
        {
            return _aTAccountContext.Roles.Where(c => c.Activity == 1).ToListAsync();
        }


        [HttpGet("Get/{id}")]
        public Task<Role> Get(int id)
        {
            return _aTAccountContext.Roles.Where(c => c.RoleID == id && c.Activity == 1).FirstOrDefaultAsync();
        }


        [HttpPost("Post")]
        public ActionResult Post(Role role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }
                Role role1 = new Role();
                role1.RoleName = role.RoleName;
                _aTAccountContext.Roles.Add(role1);
                _aTAccountContext.SaveChanges();
                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT api/<RoleController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RoleController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
