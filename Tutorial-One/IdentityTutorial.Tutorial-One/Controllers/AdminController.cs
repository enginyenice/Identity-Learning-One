using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.Controllers
{
    public class AdminController : BaseController
    {

        public AdminController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager) : base(userManager, null, roleManager)
        {

        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View(_userManager.Users.ToList());
        }

        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult RoleCreate(RoleViewModel roleViewModel)
        {
            AppRole appRole = new AppRole();
            appRole.Name = roleViewModel.Name;
            IdentityResult identityResult = _roleManager.CreateAsync(appRole).Result;
            if (identityResult.Succeeded)
            {
                return RedirectToAction("Roles");
            }
            else
            {
                AddModelError(identityResult);
            }


            return View(roleViewModel);
        }

        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }

        [HttpPost]
        public IActionResult RoleDelete(string id)
        {
            AppRole appRole = _roleManager.FindByIdAsync(id).Result;
            if (appRole != null)
            {
                IdentityResult result = _roleManager.DeleteAsync(appRole).Result;
            }
            return RedirectToAction("Roles");
        }
    
        public IActionResult RoleUpdate(string id)
        {
            AppRole appRole = _roleManager.FindByIdAsync(id).Result;
            if(appRole == null)
            {
                return RedirectToAction("Roles");
            }
            RoleViewModel roleViewModel = appRole.Adapt<RoleViewModel>();
            return View(roleViewModel);
        }
        [HttpPost]
        public IActionResult RoleUpdate(RoleViewModel roleViewModel)
        {
            AppRole appRole = _roleManager.FindByIdAsync(roleViewModel.Id).Result;

            if(appRole != null)
            {
                appRole.Name = roleViewModel.Name;
                IdentityResult result = _roleManager.UpdateAsync(appRole).Result;
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                } else
                {
                    AddModelError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "Güncelleme işlemi başarısız oldu");
            }
            return View(roleViewModel);
        }

    }
}
