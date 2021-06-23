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
        public IActionResult PasswordChange()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel passwordChangeViewModel)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByNameAsync(User.Identity.Name);
                if (appUser != null)
                {
                    bool exits = await _userManager.CheckPasswordAsync(appUser, passwordChangeViewModel.PasswordOld);

                    if (exits)
                    {
                        IdentityResult result = await _userManager.ChangePasswordAsync(appUser, passwordChangeViewModel.PasswordOld, passwordChangeViewModel.PasswordNew);

                        if (result.Succeeded)
                        {
                            await _userManager.UpdateSecurityStampAsync(appUser); //SecurityStamp günlledik
                            //Çıkış yaptırıp tekrar giriş yaptırdık kullanıcıyı yaptırmazsak securityStamp değiştiği için 30 dakika sonra kullanıcıyı sistemden atar.
                            await _signInManager.SignOutAsync();
                            await _signInManager.PasswordSignInAsync(appUser, passwordChangeViewModel.PasswordNew, true, false);
                            ViewBag.success = "True";

                        }
                        else
                        {
                            foreach (var item in result.Errors)
                            {
                                ModelState.AddModelError("", item.Description);
                            }

                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Eski şifreniz yanlış");
                    }
                }

            }
            return View(passwordChangeViewModel);
        }
    }
}
