using IdentityTutorial.Tutorial_One.Enums;
using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Controllers
{
    [Authorize]
    public class MemberController : BaseController
    {

        public MemberController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager,signInManager)
        {
        }

        public IActionResult Index()
        {
            AppUser appUser = CurrentUser;
            UserViewModel userViewModel = appUser.Adapt<UserViewModel>(); // AppUser ı UserViewModel Mappledik. (Mapster paketi kullandık)
            return View(userViewModel);
        }




        public void LogOut()
        {
            _signInManager.SignOutAsync();
        }

        public IActionResult UserEdit()
        {


            AppUser appUser = CurrentUser;
            UserViewModel userViewModel = appUser.Adapt<UserViewModel>();
            ViewBag.Gender = new SelectList(Enum.GetNames(typeof(Gender)));

            return View(userViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserViewModel userViewModel, IFormFile userPicture)
        {
            ModelState.Remove("Password");
            if (ModelState.IsValid)
            {
                AppUser appUser = CurrentUser;

                if (userPicture != null && userPicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(userPicture.FileName);
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/UserPicture", fileName);
                    using (var stream = new FileStream(path,FileMode.Create))
                    {
                        await userPicture.CopyToAsync(stream);
                        appUser.Picture = "/UserPicture/" + fileName;
                    }
                }
                
                appUser.UserName = userViewModel.UserName;
                appUser.Email = userViewModel.Email;
                appUser.PhoneNumber = userViewModel.PhoneNumber;
                appUser.City = userViewModel.City;
                appUser.BirthDay = userViewModel.BirthDay;
                appUser.Gender = (int)userViewModel.Gender;

                IdentityResult result = await _userManager.UpdateAsync(appUser);
                if (result.Succeeded)
                {

                    await _userManager.UpdateSecurityStampAsync(appUser); //SecurityStamp günlledik
                                                                          //Çıkış yaptırıp tekrar giriş yaptırdık kullanıcıyı yaptırmazsak securityStamp değiştiği için 30 dakika sonra kullanıcıyı sistemden atar.
                    await _signInManager.SignOutAsync();
                    await _signInManager.SignInAsync(appUser,true);
                    ViewBag.success = "True";

                } else
                {
                    AddModelError(result);
                }
            }

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
                AppUser appUser = CurrentUser;
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
                            AddModelError(result);

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
    
        public IActionResult AccessDenied()
        {
            return View();
        }
   
    
        [Authorize(Roles = "Editör,Admin")]
        public IActionResult Editor()
        {
            return View();
        }
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult Manager()
        {
            return View();
        }


    }
}
