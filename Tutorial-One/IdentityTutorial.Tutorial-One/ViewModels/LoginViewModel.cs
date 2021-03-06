using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.ViewModels
{
    public class LoginViewModel
    {
        [Display(Name ="Email")]
        [Required(ErrorMessage ="Email alanı gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre alanı gereklidir.")]
        [DataType(DataType.Password)]
        [MinLength(4,ErrorMessage ="Şifreniz en az 4 karakterden oluşmalıdır")]
        public string Password { get; set; }
        [Display(Name = "Beni hatırla")]
        public bool RememberMe { get; set; }
    }
}
