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
       
        private readonly TimeSpan _timeSpan;
        public JwtTokenGenerator(IConfiguration configuration) : this(TimeSpan.FromHours(1), configuration)
        {
        }

        public JwtTokenGenerator(TimeSpan timeSpan, IConfiguration configuration)
        {
            _timeSpan = timeSpan;
            _configuration = configuration;
        }

        private string Generator(int id, string userName, string mail, string appType, string issuer, string audience, byte[] key, int userid)
        {
           
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
                Expires = DateTime.UtcNow.Add(_timeSpan),
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
            const string iss = "GHDACT2N29"; // your accounts team ID found in the dev portal
            const string aud = "https://appleid.apple.com";
            const string sub = "tr.antalyataksiyolcu.ios"; // same as client_id
            var now = DateTime.UtcNow;

            const string privateKey = "MIGTAgEAMBMGByqGSM49AgEGCCqGSM49AwEHBHkwdwIBAQQgHtc5aplTMSMV6admyqNpImfrjJdqoTdKkpF6V6x2HuSgCgYIKoZIzj0DAQehRANCAATObUghCLKRpUScA5rDz0jDo5lbdnVqhfBwEjM8AIO8E50FbOqmymv31bMyCLE5vn5fR9F3dewDE1UUOTubFP9z";
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
