using DAL.AuthEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Service.IService;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Service.ImpService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

      
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<string> CreatTokeAysnc(AppUser user, UserManager<AppUser> userManager)
        {
            var authClims = new List<Claim>()
            {
                new Claim (ClaimTypes.Name , user.FullName),
                new Claim (ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
            };
            var rols = await userManager.GetRolesAsync(user);
            foreach (var r in rols)
            {
                authClims.Add(new Claim(ClaimTypes.Role, r));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:key"]));

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issure"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationTime"])),
                claims: authClims,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
