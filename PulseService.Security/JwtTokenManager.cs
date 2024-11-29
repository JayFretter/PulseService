using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PulseService.Domain.Adapters;
using PulseService.Domain.Models.Dtos;
using PulseService.Domain.Providers;
using PulseService.Security.Models;

namespace PulseService.Security;

public class JwtTokenManager : ITokenManager
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly JwtOptions _jwtOptions;

    public JwtTokenManager(IDateTimeProvider dateTimeProvider, IOptions<JwtOptions> jwtOptions)
    {
        _dateTimeProvider = dateTimeProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateToken(BasicUserCredentials user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(CreateTokenDescriptor(user));
        var stringToken = tokenHandler.WriteToken(securityToken);

        return stringToken;
    }

    public BasicUserCredentials GetUserFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

        return new BasicUserCredentials
        {
            Id = GetClaimValueFromToken(securityToken!, JwtRegisteredClaimNames.Sub),
            Username = GetClaimValueFromToken(securityToken!, "username"),
            CreatedAtUtc = DateTime.Parse(GetClaimValueFromToken(securityToken!, "created")),
        };
    }

    private SecurityTokenDescriptor CreateTokenDescriptor(BasicUserCredentials user)
    {
        var keyByteArray = Encoding.ASCII.GetBytes(_jwtOptions.Key);

        return new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim("username", user.Username),
                new Claim("created", user.CreatedAtUtc.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
            Expires = _dateTimeProvider.UtcNow.AddMinutes(_jwtOptions.ExpiryTimeMinutes),
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyByteArray), SecurityAlgorithms.HmacSha512Signature)
        };
    }

    private string GetClaimValueFromToken(JwtSecurityToken token, string claimType)
    {
        return token.Claims.First(c => c.Type == claimType).Value;
    }
}