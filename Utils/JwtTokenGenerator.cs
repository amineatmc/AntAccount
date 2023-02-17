using AntalyaTaksiAccount.Models;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
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
                   new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, userName),
                   new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email,mail),
                   new Claim("app_type",appType.ToString()),
                   new Claim("userid",userid.ToString()),
                   new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

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

        public static string CreateNewToken()
        {
            const string iss = "62QM29578N"; // your accounts team ID found in the dev portal
            const string aud = "https://appleid.apple.com";
            const string sub = "com.scottbrady91.authdemo.service"; // same as client_id
            var now = DateTime.UtcNow;

            const string privateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgnbfHJQO9feC7yKOenScNctvHUP+Hp3AdOKnjUC3Ee9GgCgYIKoZIzj0DAQehRANCAATMgckuqQ1MhKALhLT/CA9lZrLA+VqTW/iIJ9GKimtC2GP02hCc5Vac8WuN6YjynF3JPWKTYjg2zqex5Sdn9Wj+";
            var ecdsa = ECDsa.Create();
            ecdsa?.ImportPkcs8PrivateKey(Convert.FromBase64String(privateKey), out _);

            var handler = new JsonWebTokenHandler();
            return handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = iss,
                Audience = aud,
                Claims = new Dictionary<string, object> { { "sub", sub } },
                Expires = now.AddMinutes(5), // expiry can be a maximum of 6 months - generate one per request or re-use until expiration
                IssuedAt = now,
                NotBefore = now,
                SigningCredentials = new SigningCredentials(new ECDsaSecurityKey(ecdsa), SecurityAlgorithms.EcdsaSha256)
            });
        }
    }
}
