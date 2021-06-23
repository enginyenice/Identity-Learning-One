using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.ViewModels
{
    public class PasswordResetViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email alanı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
