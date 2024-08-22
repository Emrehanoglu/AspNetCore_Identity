using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentityApp.Core.ViewModels
{
    public class PasswordChangeViewModel
    {
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Eski Şifre alanı boş bırakılamaz")]
        [Display(Name = "Eski Şifre :")]
        [MinLength(6,ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        public string PasswordOld { get; set; } = null!;

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Yeni Şifre alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        public string PasswordNew { get; set; } = null!;

        [DataType(DataType.Password)]
        [Compare(nameof(PasswordNew), ErrorMessage = "Şifreler eşleşmemektedir")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar alanı boş bırakılamaz")]
        [Display(Name = "Yeni Şifre Tekrar :")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
