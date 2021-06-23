using IdentityTutorial.Tutorial_One.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.ClaimProviders
{
    public class ClaimProvider : IClaimsTransformation
    {
        public UserManager<AppUser> _userManager;

        public ClaimProvider(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        //Bu method her zaman çalışıyor.
        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            //Kullanıcı üye mi değil mi
            if (principal != null && principal.Identity.IsAuthenticated)
            {//Üye bir kullanıcı
                ClaimsIdentity claimsIdentity = principal.Identity as ClaimsIdentity;
                AppUser appUser = await _userManager.FindByNameAsync(claimsIdentity.Name);
                if (appUser != null)
                {
                    if (appUser.City != null)
                    {
                        if (!principal.HasClaim(c => c.Type == "city"))
                        {
                            Claim cityClaim = new Claim("city", appUser.City, ClaimValueTypes.String, "Internal");
                            claimsIdentity.AddClaim(cityClaim);
                        }
                    }
                }


            }
            return principal;
        }
    }
}
