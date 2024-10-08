﻿using AspNetCoreIdentityApp.Web.CustomValidations;
using AspNetCoreIdentityApp.Web.Localizations;
using AspNetCoreIdentityApp.Repository.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentityApp.Web.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2);
            });

            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.User.RequireUniqueEmail = true; //email benzersiz olsun
                options.User.AllowedUserNameCharacters = "abcdefghijklmnprstuvwxyx123456789_."; //kullanıcı adı belirlerken kullanılabilecek karakterler

                options.Password.RequiredLength = 6; //en az 6 karakter olsun
                options.Password.RequireNonAlphanumeric = false; //alfanumerice karakterler zorunlu olmasın
                options.Password.RequireLowercase = true; //kücük karakter zorunlu olsun 
                options.Password.RequireUppercase = false; //büyük karakter zorunlu olmasın
                options.Password.RequireDigit = false; //sayısal karakter zorunlu olmasın

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3); //3 dakika boyunca kitlensin
                options.Lockout.MaxFailedAccessAttempts = 3; //3 kere yanlış deneme hakkı olsun

            }).AddPasswordValidator<PasswordValidator>().
            AddUserValidator<UserValidator>().
            AddErrorDescriber<LocalizationIdentityErrorDescriber>().
            AddDefaultTokenProviders().
            AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
