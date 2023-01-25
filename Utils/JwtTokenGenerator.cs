using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AntalyaTaksiAccount.Utils
{
    public class JwtTokenGenerator
    {
        public IConfiguration _configuration;

        public JwtTokenGenerator(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        private string Generator(string userName, string mail,int roleID, string issuer, string audience, byte[] key)
        {

            var securityDescriptor = new SecurityTokenDescriptor
            {
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature),

                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    //TODO Implemented Further. 
                   new Claim("Id", Guid.NewGuid().ToString()),
                   new Claim(JwtRegisteredClaimNames.Sub, userName),
                   new Claim(JwtRegisteredClaimNames.Email,mail),
                   new Claim(JwtRegisteredClaimNames.UniqueName,roleID.ToString()),
                   new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                })
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityDescriptor);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
        public string Generate(string userName, string mail,int roleID)
        {
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            return Generator(userName, mail, roleID, _configuration["Jwt:Issuer"], _configuration["Jwt:Audience"], key);
        }
        public string GeneratorTest(string userName, string mail,int roleID, string issuer, string audience, string key)
        {
            var keyBytes = Encoding.ASCII.GetBytes(key);
            return Generator(userName, mail,roleID, issuer, audience, keyBytes);
        }
    }
}
