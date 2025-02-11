

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PersonalFinances.DAL.Helpers
{
    public static class TokenHelper
    {
        public static string GenerateToken(string userId, string userName, string terminal, int expiryInYears = 1)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(CommonStrings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("usstamp", userId),
                    new Claim("userName", userName),
                    new Claim("terminal", terminal)
                }),
                Expires = DateTime.UtcNow.AddYears(expiryInYears),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
