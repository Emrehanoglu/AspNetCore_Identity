﻿using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {
            var errors = new List<IdentityError>();

            if (password!.ToLower().Contains(user.UserName!.ToLower()))
            {
                errors.Add(new() { 
                    Code="PasswordContainUserName",
                    Description="Şifre alanı kullanıcı adı içeremez"
                });
            }
            if (password.ToLower().StartsWith("1234"))
            {
                errors.Add(new()
                {
                    Code="PasswordContain1234",
                    Description="Şifre alanı ardışık sayı içeremez"
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
