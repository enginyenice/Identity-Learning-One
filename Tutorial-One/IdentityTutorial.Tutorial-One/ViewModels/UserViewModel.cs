using IdentityTutorial.Tutorial_One.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityTutorial.Tutorial_One.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage ="Kullanıcı ismi gereklidir")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [Display(Name = "Telefon Numarası")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email adresi gereklidir")]
        [EmailAddress(ErrorMessage ="Geçerli bir email adresi giriniz")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Şifre gereklidir")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Display(Name = "Şehir")]
        public string City { get; set; }
        [Display(Name = "Profil Resmi")]
        public string Picture { get; set; }

        [Display(Name = "Doğum Tarihi")]
        [DataType(DataType.Date)]
        public DateTime? BirthDay { get; set; }
        [Display(Name = "Cinsiyet")]
        public Gender Gender { get; set; }
    }
}
