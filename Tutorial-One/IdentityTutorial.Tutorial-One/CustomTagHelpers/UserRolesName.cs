using IdentityTutorial.Tutorial_One.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.CustomTagHelpers
{
    [HtmlTargetElement("td",Attributes = "user-roles")]
    public class UserRolesName : TagHelper
    {
        public UserManager<AppUser> _userManager;

        public UserRolesName(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [HtmlAttributeName("user-roles")]
        public string UserId { get; set; }
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            AppUser appUser = await _userManager.FindByIdAsync(UserId);
            IList<string> roles = await _userManager.GetRolesAsync(appUser);
            string html = String.Empty;
            roles.ToList().ForEach(x =>
            {

                html += $"<span class='badge badge-primary'> {x} </span>";
            });

            output.Content.SetHtmlContent(html);
        }
    }
}
