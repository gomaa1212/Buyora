using AutoMapper;
using Buyora.Areas.Identity.ViewModels;
using BuyoraBusiness.Services.Interfaces;
using BuyoraModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Buyora.Areas.Identity.Controllers
{
    [Area("Identity")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AccountController(UserManager<ApplicationUser> userManger, SignInManager<ApplicationUser> signInManager, IMapper mapper, IEmailService emailService, IEmailTemplateService emailTemplateService, IConfiguration config)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _mapper = mapper;
            _emailService = emailService;
            _emailTemplateService = emailTemplateService;
            _config = config;
        }

        private string AbsoluteUrl(string? path)
            => _config["App:BaseUrl"]!.TrimEnd('/') + path;
        public IActionResult Register()
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
            return RedirectToAction("Index", "Home", new {Area="Customer"});
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
            if(user == null)
            {
                return View("ForgetPasswordConfirmation");
            }
            if(!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Please confirm your email");
                return View(model);
            }

            await SendResetPasswordEmailAsync(user);

            return View("ForgetPasswordConfirmation");
        }
        public async Task<IActionResult> ConfirmForgetPassword(string userId,string token)
        {
            if(string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            
            if(user == null)
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
            if(user == null)
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
            if(!ModelState.IsValid)
            {
                return View("ResetPassword", model);
            }
            var user = await _userManager.FindByIdAsync(model.Id);
            if(user == null)
            {
                return NotFound();
            }
            var res = await _userManager.ResetPasswordAsync(user,model.Token,model.Password);
            if(res.Succeeded)
            {
                TempData["SuccessMessage"] = "Password changes Successfully";
                return RedirectToAction("Login");
            }
            foreach(var error in res.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View("ResetPassword", model);
        }


    }
}
