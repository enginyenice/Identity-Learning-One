using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult LogIn(string ReturnUrl)
        {
            TempData["ReturnUrl"] = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LoginViewModel user)
        {

            if (ModelState.IsValid)
            {
                AppUser appUser = await _userManager.FindByEmailAsync(user.Email);
                if (appUser != null)
                {

                    if (await _userManager.IsLockedOutAsync(appUser))
                    {
                        ModelState.AddModelError("", "Hesabınız bir süreliğine kilitlenmiştir. Lütfen daha sonra tekrar deneyiniz");
                        return View(user);
                    }



                    //Eski bir cookie varsa silelim
                    await _signInManager.SignOutAsync();

                    Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(appUser, user.Password, user.RememberMe, false);

                    if (signInResult.Succeeded)
                    {

                        await _userManager.ResetAccessFailedCountAsync(appUser); // Hatalı giriş sayısını sıfırla



                        if (TempData["ReturnUrl"] != null)
                        {
                            return Redirect(TempData["ReturnUrl"].ToString());
                        }
                        return RedirectToAction(actionName: "Index", routeValues: "Member");
                    }
                    else
                    {
                        await _userManager.AccessFailedAsync(appUser);
                        int fail = await _userManager.GetAccessFailedCountAsync(appUser);
                        ModelState.AddModelError("", $"{fail} kere başarısız giriş yaptınız. 3 Olması durumunda hesabınız belirli bir süreliğine kitlenecektir.");
                        if (fail == 3)
                        {
                            await _userManager.SetLockoutEndDateAsync(appUser, new DateTimeOffset(DateTime.Now.AddMinutes(20))); //20 dakika kickledik
                            ModelState.AddModelError("", "Hesabınız 3 başarısız giriş sonucunda 20 dakika kilitlendi. Lütfen daha sonra tekrar deneyiniz.");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Geçersiz email adresi veya şifresi");
                        }
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Bu email adresine ait bir kullanıcı bulunamadı");
            }
            return View(user);
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser();
                appUser.UserName = user.UserName;
                appUser.Email = user.Email;
                appUser.PhoneNumber = user.PhoneNumber;
                IdentityResult identityResult = await _userManager.CreateAsync(appUser, user.Password);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("LogIn");
                }
                else
                {
                    foreach (IdentityError errorItem in identityResult.Errors)
                    {
                        ModelState.AddModelError("", errorItem.Description);
                    }

                }


            }
            return View(user);
        }
    
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(PasswordResetViewModel passwordResetViewModel)
        {
            AppUser user = _userManager.FindByEmailAsync(passwordResetViewModel.Email).Result;
            if(user != null)
            {
                string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                string passwordResetLink = Url.Action("ResetPasswordConfirm","Home",new { 
                    userId= user.Id,
                    token = passwordResetToken
                    },HttpContext.Request.Scheme);

                Helper.PasswordReset.PasswordResetSendMail(passwordResetLink);
                ViewBag.status = "success";


            } else
            {
                ModelState.AddModelError("","Sistemde kayıtlı bir email adresi bulunamadı");
            }

            return View(passwordResetViewModel);
        }

        public IActionResult ResetPasswordConfirm(string userId,string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")]PasswordResetViewModel passwordResetViewModel)
        {
            string token  = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();

            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if(appUser != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(appUser, token, passwordResetViewModel.PasswordNew);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(appUser);
                    ViewBag.status = "success";

                } else
                {
                    foreach (var item in result.Errors)
                    {

                        ModelState.AddModelError("", item.Description);
                    }
                }

            }
            else
            {
                ModelState.AddModelError("", "Hata meydana geldi. Lütfen daha sonra tekrar deneyiniz....");
            }
            return View(passwordResetViewModel);
        }
    }
}
