using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using mvcCookieAuthSample.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mvcCookieAuthSample.Services
{
    public class ProfileService : IProfileService
    {
        private UserManager<ApplicationUser> _userManager;
        public ProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        private async Task<List<Claim>> GetClaimsFromUser(ApplicationUser user)
        {
            var claims = new List<Claim>()
            {
                new Claim(JwtClaimTypes.Subject,user.Id.ToString()),
                new Claim(JwtClaimTypes.PreferredUserName,user.UserName)
            };
         var roles=await    _userManager.GetRolesAsync(user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(JwtClaimTypes.Role,role));
            }
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                claims.Add(new Claim("avatar", user.Avatar));
            }
            return claims;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.Claims.FirstOrDefault(r => r.Type == "sub").Value;

            var user = await _userManager.FindByIdAsync(subjectId);

            context.IssuedClaims = await GetClaimsFromUser(user);
        }

        public async  Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = false;

           var subjectId= context.Subject.Claims.FirstOrDefault(r => r.Type == "sub").Value;

            var user =await  _userManager.FindByIdAsync(subjectId);

            context.IsActive = user != null;
        }
    }
}
