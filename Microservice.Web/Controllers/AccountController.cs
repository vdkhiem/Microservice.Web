using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Extensions.CognitoAuthentication;
using Microservice.Web.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Microservice.Web.Controllers
{
    public class AccountController : Controller
    {
		private readonly SignInManager<CognitoUser> signInManager;
		private readonly UserManager<CognitoUser> userManager;
		private readonly CognitoUserPool pool;

		public AccountController(SignInManager<CognitoUser> signInManager, UserManager<CognitoUser> userManager, CognitoUserPool pool)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
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
				user.Attributes.Add("Name", model.Email);
				if (user.Status != null) // user exists
				{
					ModelState.AddModelError("UserExists", "User with this email already exists");
				}

				try
				{
					var createdUser = await userManager.CreateAsync(user, model.Password).ConfigureAwait(false);
					if (createdUser.Succeeded)
					{
						RedirectToAction("Confirm");
					}
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("Signup", "Unable to create new user");
					ModelState.AddModelError("SignupUnhandleError", ex.Message);
				}
				
			}
			return View();
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
				var result = await userManager.ConfirmEmailAsync(user, model.Code);
				if (result.Succeeded)
				{
					RedirectToAction("Index", "Home");
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
			
			return View();
		}
    }
}