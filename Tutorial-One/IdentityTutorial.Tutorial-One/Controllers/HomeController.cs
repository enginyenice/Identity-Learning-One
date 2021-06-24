using IdentityTutorial.Tutorial_One.Helper;
using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Controllers
{
    public class HomeController : BaseController
    {

        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager) : base(userManager, signInManager)
        {
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Member");
            }
            return View();
        }

        public IActionResult Privacy()
        {

            return View();
        }

        public IActionResult Error()
        {
            return View();
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
                    if (await _userManager.IsEmailConfirmedAsync(appUser) == false)
                    {
                        ModelState.AddModelError("", "Lütfen eposta adresinize gönderilen mail üzerinden hesabınızı aktif ediniz.");
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
                    string confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(appUser);
                    string link = Url.Action("ConfirmEmail", "Home", new
                    {
                        userId = appUser.Id,
                        token = confirmationToken
                    }, HttpContext.Request.Scheme);
                    EmailConfirmation.EmailConfirmationSendMail(link, user.Email);


                    return RedirectToAction("LogIn");
                }
                else
                {
                    AddModelError(identityResult);

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
            if (user != null)
            {
                string passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                string passwordResetLink = Url.Action("ResetPasswordConfirm", "Home", new
                {
                    userId = user.Id,
                    token = passwordResetToken
                }, HttpContext.Request.Scheme);

                Helper.PasswordReset.PasswordResetSendMail(passwordResetLink, user.Email);
                ViewBag.status = "success";


            }
            else
            {
                ModelState.AddModelError("", "Sistemde kayıtlı bir email adresi bulunamadı");
            }

            return View(passwordResetViewModel);
        }

        public IActionResult ResetPasswordConfirm(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordConfirm([Bind("PasswordNew")] PasswordResetViewModel passwordResetViewModel)
        {
            string token = TempData["token"].ToString();
            string userId = TempData["userId"].ToString();

            AppUser appUser = await _userManager.FindByIdAsync(userId);
            if (appUser != null)
            {
                IdentityResult result = await _userManager.ResetPasswordAsync(appUser, token, passwordResetViewModel.PasswordNew);

                if (result.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(appUser);
                    ViewBag.status = "success";

                }
                else
                {
                    AddModelError(result);
                }

            }
            else
            {
                ModelState.AddModelError("", "Hata meydana geldi. Lütfen daha sonra tekrar deneyiniz....");
            }
            return View(passwordResetViewModel);
        }



        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var appUser = await _userManager.FindByIdAsync(userId);
            IdentityResult result = await _userManager.ConfirmEmailAsync(appUser, token);
            if (result.Succeeded)
            {
                ViewBag.status = "Email adresiniz onaylanmıştır. Login ekranından giriş yapabilirsiniz";
            }
            else
            {
                ViewBag.status = "Bir hata meydana geldi. Lütfen daha sonra tekrar deneyiniz.";
            }

            return View();
        }

        public IActionResult FacebookLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new
            {
                ReturnUrl = ReturnUrl
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", RedirectUrl);

            return new ChallengeResult("Facebook", properties);
        }
        public IActionResult GoogleLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new
            {
                ReturnUrl = ReturnUrl
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", RedirectUrl);

            return new ChallengeResult("Google", properties);
        }

        public IActionResult MicrosoftLogin(string ReturnUrl)
        {
            string RedirectUrl = Url.Action("ExternalResponse", "Home", new
            {
                ReturnUrl = ReturnUrl
            });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Microsoft", RedirectUrl);

            return new ChallengeResult("Microsoft", properties);
        }

        public async Task<IActionResult> ExternalResponse(string ReturnUrl = "/")
        {
            ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction("LogIn");
            }
            else
            {
                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                if (signInResult.Succeeded)
                {
                    return Redirect(ReturnUrl);
                }
                else
                {
                    AppUser appUser = new AppUser();
                    appUser.Email = info.Principal.FindFirst(ClaimTypes.Email).Value;
                    string externalUserId = info.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                    if (info.Principal.HasClaim(p => p.Type == ClaimTypes.Name))
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Name).Value;
                        userName = userName.Replace(' ', '-').ToLower() + externalUserId.Substring(0, 5).ToString();
                        appUser.UserName = userName;
                    }
                    else
                    {
                        string userName = info.Principal.FindFirst(ClaimTypes.Email).Value;
                        appUser.UserName = userName;
                    }

                    AppUser appUser2 = await _userManager.FindByEmailAsync(appUser.Email);

                    if (appUser2 == null)
                    {

                        IdentityResult createResult = await _userManager.CreateAsync(appUser);
                        if (createResult.Succeeded)
                        {
                            IdentityResult loginResult = await _userManager.AddLoginAsync(appUser, info);
                            if (loginResult.Succeeded)
                            {
                                //External Login işlemi yaptık
                                await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                AddModelError(loginResult);
                            }

                        }
                        else
                        {
                            AddModelError(createResult);
                        }
                    } else
                    {
                        IdentityResult identityResult = await _userManager.AddLoginAsync(appUser2,info);
                        await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true);
                        return Redirect(ReturnUrl);
                    }






                }

                List<string> errors = ModelState.Values.SelectMany(p => p.Errors).Select(x => x.ErrorMessage).ToList();
                return View("Error", errors);
            }

        }

    }
}
