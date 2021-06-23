using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Display(Name ="Eski Şifre")]
        [Required(ErrorMessage ="Eski şifreniz gereklidir")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmak zorundadır")]
        public string PasswordOld { get; set; }
        
        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni şifreniz gereklidir")]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmak zorundadır")]
        
        public string PasswordNew { get; set; }



        [Display(Name = "Yeni Şifre Tekrar")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni şifrenizin tekrarı gereklidir")]
        [MinLength(4, ErrorMessage = "Şifreniz en az 4 karakterli olmak zorundadır")]
        [Compare("PasswordNew",ErrorMessage ="Yeni şifreniz ve şifre tekrarınız farklıdır")]
        public string PasswordConfirm { get; set; }
    }
}
