using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Web.ViewModels
{
    public class SignUpViewModel
    {
        public SignUpViewModel()
        {
            
        }
        public SignUpViewModel(string userName, string email, string phone, string password, string passwordConfirm)
        {
            UserName = userName;
            Email = email;
            Phone = phone;
            Password = password;
            PasswordConfirm = passwordConfirm;
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

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmemektedir")]
        [Required(ErrorMessage = "Şifre Tekrar alanı boş bırakılamaz")]
        [Display(Name = "Şifre Tekrar :")]
        public string PasswordConfirm { get; set; }
    }
}
