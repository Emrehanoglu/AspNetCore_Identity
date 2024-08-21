using AspNetCoreIdentityApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Seeds
{
    public static class PermissionSeed
    {
        public static async Task Seed(RoleManager<AppRole> roleManager)
        {
            //bakalım bu rol tabloda var mı
            var hasBasicRole = await roleManager.RoleExistsAsync("BasicRole");

            if (!hasBasicRole)
            {
                //AspNetRoles tablosunda yok o zaman ekleyelim
                await roleManager.CreateAsync(new AppRole { Name = "BasicRole"});

                //rolü ekledik ama id bilgisi elimizde değil, alalım...
                var basicRole = (await roleManager.FindByNameAsync("BasicRole"))!;

                //AspNetRoleClaims tablosuna BasicRole ile ilişkili kayıtlar atıldı.
                await roleManager.AddClaimAsync(basicRole, 
                    new Claim("Permission", Permissions.Permission.Stock.Read));
                
                await roleManager.AddClaimAsync(basicRole,
                    new Claim("Permission", Permissions.Permission.Order.Read));
                
                await roleManager.AddClaimAsync(basicRole,
                    new Claim("Permission", Permissions.Permission.Catalog.Read));
            }
        }
    }
}
