using AntalyaTaksiAccount.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AntalyaTaksiAccount.Utils
{
    public class JwtTokenGenerator
    {
        public IConfiguration _configuration;
        private readonly ATAccountContext _context;
        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
           // _context = aTAccount;
        }
        private string Generator(int id, string userName, string mail, string appType, string issuer, string audience, byte[] key, int userid)
        {
            //var passenger= _context.Passengers.Where(x=>x.AllUserID==id);
            //var driver=_context.Drivers.Where(x=>x.AllUserID==id).FirstOrDefault();
            //var station = _context.Stations.Where(x=>x.AllUserID== id).FirstOrDefault();
            if (appType == "1")
                appType  = "Driver";
              //  userid = driver.DriverID;
            if (appType == "2")
                appType = "User";
              //  userid= driver.DriverID;
            if (appType == "3")
                appType = "Station";
            // userid = station.StationID;
            if (appType == "4")
                appType = "Admin";

            var securityDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),

                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    //TODO Implemented Further. 
                   new Claim("GuidId", Guid.NewGuid().ToString()),
                   new Claim("id",id.ToString()),
                   new Claim(JwtRegisteredClaimNames.Sub, userName),
                   new Claim(JwtRegisteredClaimNames.Email,mail),
                   new Claim("app_type",appType.ToString()),
                   new Claim("userid",userid.ToString()),
                   new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

                })
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
        public string Generate(int id, string userName, string mail, string appType,int userid)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            return Generator(id, userName, mail, appType, _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], key, userid);
        }
        public string GeneratorTest( int id, string userName, string mail, string appType, string issuer, string audience, string key, int userid)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            return Generator(id, userName, mail, appType, issuer, audience, keyBytes,userid);
        }
    }
}
