using Microsoft.AspNetCore.Mvc;


namespace AntalyaTaksiAccount.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private bool LoginUser(string username, string password)
        {
            try
            {

               
                //HttpContext.Session.SetObject("User", signIn.User);
                //HttpContext.Session.SetObject("OrganizationLicenses", signIn.OrganizationLicenses);
                //HttpContext.Session.SetString("Jwt", signIn.JWTAuthToken);

                // should be store as string. Use json serialize and deserialize
                //HttpContext.Session.SetString("Licenses",JsonConvert.SerializeObject(signIn.Licenses) );


                return true;




            }
            catch (Exception ex)
            {
                return false;
            }
        }
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<LoginController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<LoginController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<LoginController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LoginController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
