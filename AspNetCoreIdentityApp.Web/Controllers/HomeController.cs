using AspNetCoreIdentityApp.Web.Models;
using AspNetCoreIdentityApp.Web.Services;
using AspNetCoreIdentityApp.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.Diagnostics;
using System.Security.Claims;

namespace AspNetCoreIdentityApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IEmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ResetPassword(string userId, string token)
        {
            TempData["userId"] = userId;
            TempData["token"] = token;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var userId = TempData["userId"];
            var token = TempData["token"];

            if (userId == null || token == null)
            {
                throw new Exception("Bir hata meydana geldi.");
            }

            var hasUser = await _userManager.FindByIdAsync(userId.ToString()!);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Böyle bir kullanıcı bulunamamaktadır.");
                return View();
            }

            var result = await _userManager.ResetPasswordAsync(hasUser, token.ToString()!, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Şifreniz başarıyla yenilenmiştir.";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Bir hata meydana geldi.");
            }
            return View();
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            TempData["SuccessMessage"] = "Şifre yenileme linki, e-posta adresinize gönderilecektir.";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {
            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Bu email adresine sahip bir kullanıcı yoktur.");
                return View();
            }

            string passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(hasUser);

            var passwordResetLink = Url.Action("ResetPassword", "Home",
                new { userId = hasUser.Id, Token = passwordResetToken },
                HttpContext.Request.Scheme);

            //Email Send Service
            await _emailService.SendResetPasswordEmail(passwordResetLink!, hasUser.Email!);

            TempData["SuccessMessage"] = "Şifre sıfırlama linki email adresinize gönderilmiştir.";

            return RedirectToAction("ForgetPassword");
        }

        [HttpGet]
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Eksik bilgiler mevcut");
                return View();
            }

            var identityResult = await _userManager.CreateAsync(new AppUser
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone,
                City = "İstanbul"
            }, request.Password);

            if (!identityResult.Succeeded)
            {
                foreach (IdentityError item in identityResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View();
            }

            var exchangeExpireClaim = new Claim("ExchangeExpireDate", DateTime.Now.AddDays(10).ToString());
            var user = await _userManager.FindByNameAsync(request.UserName);
            var claimResult = await _userManager.AddClaimAsync(user!, exchangeExpireClaim);

            if (!claimResult.Succeeded)
            {
                foreach (IdentityError item in claimResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, item.Description);
                }

                return View();
            }

            TempData["SuccessMessage"] = "Üyelik kayıt işlemi başarıla gerçekleşmiştir.";

            return RedirectToAction("SignUp", "Home");
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel request, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
                return View();
            }

            //returnUrl null ise kullanıcı Index sayfasına yönlendirilecek
            returnUrl = returnUrl ?? Url.Action("Index", "Home");

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if (hasUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
                return View();
            }

            var identityResult = await _signInManager.PasswordSignInAsync(hasUser!, request.Password, request.RememberMe, true);


            if (identityResult.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "3 dakika boyunca giriş yapamazsınız");
                return View();
            }

            if (!identityResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış");
                ModelState.AddModelError(string.Empty, $"Başarısız giriş sayısı : {await _userManager.GetAccessFailedCountAsync(hasUser)}");
                return View();
            }

            if (hasUser.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(hasUser, request.RememberMe, new[]
                {
                    new Claim("birthdate",hasUser.BirthDate.Value.ToString())
                });
            }

            return Redirect(returnUrl!);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
