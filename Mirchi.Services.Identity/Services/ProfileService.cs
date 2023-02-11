using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Mirchi.Services.Identity.Models;
using System.Security.Claims;

namespace Mirchi.Services.Identity.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> _userClaimsPrincipalFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ProfileService(IUserClaimsPrincipalFactory<ApplicationUser> userClaimsPrincipalFactory, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            string sub = context.Subject.GetSubjectId();
            if (string.IsNullOrWhiteSpace(sub))
            {
                throw new ArgumentException();
            }

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(sub);
            if (applicationUser == null)
            {
                throw new ArgumentException();
            }

            ClaimsPrincipal claimsPrincipal = await _userClaimsPrincipalFactory.CreateAsync(applicationUser);
            List<Claim> claims = claimsPrincipal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            if (_userManager.SupportsUserRole)
            {
                IList<string> roles = await _userManager.GetRolesAsync(applicationUser);
                foreach (string role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                    claims.Add(new Claim(JwtClaimTypes.GivenName, applicationUser.FirstName));
                    claims.Add(new Claim(JwtClaimTypes.FamilyName, applicationUser.LastName));
                    if (_roleManager.SupportsRoleClaims)
                    {
                        IdentityRole identityRole = await _roleManager.FindByNameAsync(role);
                        if (identityRole != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                        }
                    }
                }
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            string sub = context.Subject.GetSubjectId();
            if (string.IsNullOrWhiteSpace(sub))
            {
                throw new ArgumentException();
            }

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(sub);            
            context.IsActive = applicationUser != null;
        }
    }
}
