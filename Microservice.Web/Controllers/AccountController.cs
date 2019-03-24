using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microservice.Web.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    /// <summary>
    /// https://github.com/aws/aws-aspnet-cognito-identity-provider
    /// </summary>
    public class AccountController : Controller
    {
        private readonly SignInManager<CognitoUser> signInManager;
        private readonly CognitoUserManager<CognitoUser> userManager;
        private readonly CognitoUserPool pool;

        public AccountController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
        {
            this.signInManager = signInManager;
            this.userManager = userManager as CognitoUserManager<CognitoUser>;
            this.pool = pool;
        }

        public async Task<IActionResult> SignUp()
        {
            var model = new Signup();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(Signup model)
        {
            if (ModelState.IsValid)
            {
                var user = pool.GetUser(model.Email);
                if (user.Status != null) // user exists
                {
                    ModelState.AddModelError("UserExists", "User with this email already exists");
                    return View(model);
                }

                try
                {
                    user.Attributes.Add("name", model.Email);
                    var createdUser = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
                    if (createdUser.Succeeded)
                    {
                        return RedirectToAction("Confirm");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Signup", "Unable to create new user");
                    ModelState.AddModelError("SignupUnhandleError", ex.Message);
                }

            }
            return View("Signup", model);
        }

        public async Task<IActionResult> Confirm()
        {
            return View(new Confirm());
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(Confirm model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError("NotFound", "A user with given email address was not found");
                    return View(model);
                }
                try
                {
                    var result = await userManager.ConfirmSignUpAsync(user, model.Code, false).ConfigureAwait(false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        foreach (var item in result.Errors)
                        {
                            ModelState.AddModelError(item.Code, item.Description);
                        }

                        return View(model);
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Confirm", "Unable to confirm new user");
                    ModelState.AddModelError("ConfirmUnhandleError", ex.Message);
                }
            }

            return View("Confirm", model);
        }

        public async Task<IActionResult> Login()
        {
            var model = new Login();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false).ConfigureAwait(false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {

                    ModelState.AddModelError("LoginError", "Email and password do not match");
                    return View(model);
                }
            }
            return View("Login", model);
        }
    }
}