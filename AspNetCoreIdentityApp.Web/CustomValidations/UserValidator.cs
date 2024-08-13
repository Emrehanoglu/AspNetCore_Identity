using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            var errors = new List<IdentityError>();

            //2. parametre kısmında bir parametre belirtseydim user.UserName 'i
            //integer 'a cevirebilseydi, o parametreye o degeri atayacaktı
            //ama belirtmediğim için burada bana sadece true false dönecek
            var isNumeric = int.TryParse(user.UserName![0].ToString(), out _);

            if (isNumeric)
            {
                errors.Add(new()
                {
                    Code = "UserNameContainFirstLetterDigit",
                    Description = "Kullanıcı adının ilk karakteri sayısal karakter içeremez."
                });
            }

            //errors içerisinde bir veri varsa yani hata var ise
            if (errors.Any())
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            //hata yok ise
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
