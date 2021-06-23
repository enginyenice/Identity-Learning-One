using IdentityTutorial.Tutorial_One.Models;
using IdentityTutorial.Tutorial_One.ViewModels;
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
        
        public AdminController(UserManager<AppUser> userManager,RoleManager<AppRole> roleManager) : base(userManager,null,roleManager)
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
            } else
            {
                AddModelError(identityResult);
            }


            return View(roleViewModel);
        }

        public IActionResult Roles()
        {
            return View(_roleManager.Roles.ToList());
        }
    }
}
