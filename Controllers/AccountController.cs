using System;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.Accounts;

namespace WebAdvert.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _pool;

        public AccountController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, 
            CognitoUserPool pool)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _pool = pool;
        }

        public IActionResult Signup()
        {
            var model = new SignupViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _pool.GetUser(model.Email);
                if (user.Status != null)
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                user.Attributes.Add(nameof(CognitoAttribute.Name).ToLower(), model.Email);
                var createdUser = await _userManager.CreateAsync(user, model.Passport);
                if (!createdUser.Succeeded)
                {
                    foreach (var item in createdUser.Errors)
                    {
                        ModelState.AddModelError(item.Code, item.Description);
                    }
                }

                if (createdUser.Succeeded)
                {
                    return await Task.Run(() => RedirectToAction("Index", "Home"));
                }
            }

            return View();
        }

        public IActionResult Confirm()
        {
            var model = new ConfirmViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with provided e-mail was not found");
                    return View(model);
                }

                var cognitoRef = (CognitoUserManager<CognitoUser>)_userManager;

                var result = await cognitoRef.ConfirmSignUpAsync(user, model.Code, true);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError(item.Code, item.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(LoginModel model)
        {
            return View(model);
        }

        [HttpPost]
        [ActionName(nameof(Login))]
        public async Task<IActionResult> LoginPost(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                    return await Task.Run(() => RedirectToAction("Index", "Home"));

                ModelState.AddModelError("LoginError", "Login failure");
            }

            return View(nameof(Login), model);
        }
    }
}
