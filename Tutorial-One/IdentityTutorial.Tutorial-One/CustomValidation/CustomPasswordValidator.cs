using IdentityTutorial.Tutorial_One.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.CustomValidation
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();

            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsUserName",
                    Description = "Şifre kullanıcı adı içeremez"
                });
            }
            if (password.ToLower().Contains("1234"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContains1234",
                    Description = "Şifre alanı ardışık sayı içeremez"
                });
            }
            if (password.ToLower().Contains(user.Email.ToLower()))
            {
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsEmail",
                    Description = "Şifre email içeremez"
                });
            }

            if (errors.Count == 0)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }
        }
    }
}