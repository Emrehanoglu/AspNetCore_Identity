using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class UserEditViewModel
    {
        public UserEditViewModel()
        {

        }
        public UserEditViewModel(string userName, string email, string phone)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
        }

        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır")]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz")]
        [Display(Name = "Email :")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Doğum tarihi alanı boş bırakılamaz")]
        [Display(Name = "Telefon :")]
        public string Birthday { get; set; }
    }
}
