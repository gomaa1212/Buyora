using AutoMapper;
using Buyora.Areas.Identity.ViewModels;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Buyora.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AccountController(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager, IMapper mapper, IEmailService emailService, IEmailTemplateService emailTemplateService, IConfiguration config)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _config = config;
        }

        private string AbsoluteUrl(string? path)
            => _config["App:BaseUrl"]!.TrimEnd('/') + path;
        public async Task<IActionResult> Register()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = _mapper.Map<ApplicationUser>(model);
            var res = await _userManager.CreateAsync(user, model.Password);
            await _userManager.AddToRoleAsync(user, "Customer");
            if (res.Succeeded)
            {
                await SendConfirmationEmailAsync(user);

                TempData["Email"] = user.Email;
                return View("RegisterConfirmation");
            }
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var res = await _userManager.ConfirmEmailAsync(user, token);
            if (res.Succeeded)
            {
                return View();
            }
            return BadRequest();
        }
        private async Task SendConfirmationEmailAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return;
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var path = Url.Action("ConfirmEmail", "Account",
                new { area = "Identity", userId = user.Id, token });

            var subject = "Confirm your Buyora Account";
            var body = _emailTemplateService.Build(
                subject,
                "Account Verification",
                user.Name,
                "Welcome to Buyora! We are excited to have you join our community. To activate your account and start exploring exclusive deals, please verify your email address by clicking the button below.",
                "Confirm Email Address",
                AbsoluteUrl(path),
                "If you did not create an account on Buyora, you can safely ignore this email.");

            await _emailService.SendEmailAsync(user.Email!, subject, body);
        }
        public IActionResult Login(string? returnUrl)
        {
            var loginModel = new LoginViewModel();
            loginModel.ReturnUrl = returnUrl;
            return View(loginModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewBag.ShowResend = false;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is wrong");
                return View(model);
            }
            var res = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, lockoutOnFailure: true);
            if (res.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                {
                    return LocalRedirect(model.ReturnUrl);
                }
                return RedirectToAction("Index", "Home", new { Area = "Customer" });
            }
            if (res.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Account locked, try again later.");
                return View(model);
            }
            if (res.IsNotAllowed)
            {
                ViewBag.ShowResend = true;
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email or password is wrong");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReSendConfirmationEmail(string userEmail)
        {
            TempData["Email"] = userEmail;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null || !user.EmailConfirmed)
            {
                return View("RegisterConfirmation");
            }
            await SendConfirmationEmailAsync(user);

            return View("RegisterConfirmation");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home", new { Area = "Customer" });
        }
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            TempData["Email"] = model.Email;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return View("ForgetPasswordConfirmation");
            }
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your email");
                return View(model);
            }

            await SendResetPasswordEmailAsync(user);

            return View("ForgetPasswordConfirmation");
        }
        public async Task<IActionResult> ConfirmForgetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }
            var changePasswordModel = new ResetPasswordViewModel()
            {
                Id = user.Id,
                Token = token
            };
            return View("ResetPassword", changePasswordModel);
        }
        private async Task SendResetPasswordEmailAsync(ApplicationUser user)
        {
            if (user == null)
            {
                return;
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var path = Url.Action("ConfirmForgetPassword", "Account",
                new { area = "Identity", userId = user.Id, token });

            var subject = "Reset Your Buyora Password";
            var body = _emailTemplateService.Build(
                subject,
                "Password Assistance",
                user.Name,
                "We received a request to reset the password for your Buyora account. If you made this request, please click the button below to choose a new password.",
                "Reset Password",
                AbsoluteUrl(path),
                "If you did not request a password reset, you can safely ignore this email. Your password will remain unchanged.");

            await _emailService.SendEmailAsync(user.Email!, subject, body);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }
            var res = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (res.Succeeded)
            {
                TempData["SuccessMessage"] = "Password changes Successfully";
                return RedirectToAction("Login");
            }
            foreach (var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("ResetPassword", model);
        }
        public IActionResult AccessDenied(string? ReturnUrl)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action(nameof(GoogleCallback), "Account",
                new { area = "Identity", returnUrl });

            var properties = _signInManager.ConfigureExternalAuthenticationProperties(
                GoogleDefaults.AuthenticationScheme, redirectUrl);

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        // جوجل بيرجّع المستخدم هنا بعد ما يوافق (ده الـ Login والـ Register مع بعض)
        [HttpGet]
        public async Task<IActionResult> GoogleCallback(string? returnUrl = null, string? remoteError = null)
        {
            if (remoteError != null)
            {
                TempData["ErrorMessage"] = "Google sign-in failed. Please try again.";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Google sign-in failed. Please try again.";
                return RedirectToAction(nameof(Login));
            }

            // 1) الحساب مربوط بجوجل قبل كده؟ ادخّله (Login)
            var signIn = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (signIn.Succeeded)
                return RedirectAfterLogin(returnUrl);

            if (signIn.IsLockedOut)
            {
                TempData["ErrorMessage"] = "Account locked, try again later.";
                return RedirectToAction(nameof(Login));
            }

            // 2) أول مرة: نجيب الإيميل من جوجل
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                TempData["ErrorMessage"] = "We could not get your email from Google.";
                return RedirectToAction(nameof(Login));
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // مفيش حساب بالإيميل ده: اعمل حساب جديد (Register)
                var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,      // جوجل أكّد الإيميل
                    Name = name                 // عدّل حسب خصائص ApplicationUser عندك
                };

                var create = await _userManager.CreateAsync(user);
                if (!create.Succeeded)
                {
                    TempData["ErrorMessage"] = "Could not create your account.";
                    return RedirectToAction(nameof(Login));
                }

                await _userManager.AddToRoleAsync(user, "Customer");
            }
            else if (!user.EmailConfirmed)
            {
                // فيه حساب بنفس الإيميل بس لسه ما اتأكدش (ممكن حد تاني سجّل بإيميلك قبل كده).
                // نشيل الباسوورد القديم عشان اللي سجّله ما يفضلش يقدر يدخل بيه.
                if (await _userManager.HasPasswordAsync(user))
                    await _userManager.RemovePasswordAsync(user);

                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }

            if (await _userManager.IsLockedOutAsync(user))
            {
                TempData["ErrorMessage"] = "Account locked, try again later.";
                return RedirectToAction(nameof(Login));
            }

            // 3) اربط جوجل بالحساب، وسجّل الدخول
            var link = await _userManager.AddLoginAsync(user, info);
            if (!link.Succeeded)
            {
                TempData["ErrorMessage"] = "Could not link your Google account.";
                return RedirectToAction(nameof(Login));
            }

            await _signInManager.SignInAsync(user, isPersistent: false, info.LoginProvider);
            return RedirectAfterLogin(returnUrl);
        }

        private IActionResult RedirectAfterLogin(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home", new { Area = "Customer" });

        }
    }
}
