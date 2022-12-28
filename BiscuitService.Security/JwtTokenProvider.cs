using BiscuitService.Domain.Adapters;
using BiscuitService.Domain.Models.Dtos;
using BiscuitService.Security.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BiscuitService.Security
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly JwtOptions _jwtOptions;

        public JwtTokenProvider(IOptions<JwtOptions> jwtOptions) 
        {
            _jwtOptions = jwtOptions.Value;
        }

        public string GenerateToken(UserDto user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(CreateTokenDescriptor(user));
            var stringToken = tokenHandler.WriteToken(securityToken);

            return stringToken;
        }

        private SecurityTokenDescriptor CreateTokenDescriptor(UserDto user)
        {
            var keyByteArray = Encoding.ASCII.GetBytes(_jwtOptions.Key);

            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim("Username", user.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryTimeMinutes),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyByteArray), SecurityAlgorithms.HmacSha512Signature)
            };
        }
    }
}
