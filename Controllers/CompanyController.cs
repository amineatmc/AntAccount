using AntalyaTaksiAccount.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ATAccountContext _aTAccountContext;
        public CompanyController(ATAccountContext aTAccountContext)
        {
            _aTAccountContext= aTAccountContext;
        }
        [HttpGet("Get")]
        public async Task<List<Company>> Get()
        {
            var companies=await _aTAccountContext.Companies.Where(c => c.Activity == 1).ToListAsync();
            return companies;
        }

        
        [HttpGet("Get/{id}")]
        public async Task<Company> Get(int id)
        {
            var company=await _aTAccountContext.Companies.Where(c => c.CompanyID == id && c.Activity == 1).FirstOrDefaultAsync();
            return company;
        }

        [HttpPost("Post")]
        public async Task<ActionResult> Post(Company company)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Model bilgileri doğru değil.");
                }
                Company company1 = new Company();
                company1.CompanyName=company.CompanyName;
                _aTAccountContext.Companies.Add(company1);
                _aTAccountContext.SaveChanges();

                return Ok("Kayıt Eklendi.");
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        // PUT api/<CompanyController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<CompanyController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
