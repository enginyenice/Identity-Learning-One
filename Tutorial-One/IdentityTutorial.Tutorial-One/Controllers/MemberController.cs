using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            AppUser appUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            UserViewModel userViewModel = appUser.Adapt<UserViewModel>(); // AppUser ı UserViewModel Mappledik. (Mapster paketi kullandık)
            return View(userViewModel);
        }
    }
}
