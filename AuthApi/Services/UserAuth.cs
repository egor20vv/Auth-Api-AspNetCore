using AuthApi.Services.Interfaces;
using AuthApi.Services.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace AuthApi.Services;

public class UserAuth : IUserAuth
{
    private readonly IConfiguration _configuration;
    private readonly UserRepository _userRep;

    public UserAuth(IConfiguration configuration, UserRepository userRep)
    {
        _configuration = configuration;
        // _userPrincipal = userPrincipal;
        _userRep = userRep;
    }

    public async Task<string?> GetJwt(Guid id)
    {
        var user = await _userRep.GetByIdAsync(id);
        if (user == null)
            return null;

        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

        var claims = new List<Claim>
        {
            new Claim("Id", id.ToString()),
            new Claim(JwtRegisteredClaimNames.Sub, user.NickName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString())
        };
        if (user.IsEmailVerified && user.Email != null)
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));

        var signing = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature);

        var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(5),
                signingCredentials: signing
            );

        return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task<Guid> GetGuidFromPrincipal(ClaimsPrincipal user)
    {
        return await Task.FromResult(Guid.Parse(user.FindFirst("Id")!.Value));
    }

}
