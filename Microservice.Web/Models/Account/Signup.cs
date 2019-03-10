﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Microservice.Web.Models.Account
{
    public class Signup
    {
		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[StringLength(6, ErrorMessage = "Password must be at leasts 6 characters long")]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[Required]
		[DataType(DataType.Password)]
		[Compare("Password", ErrorMessage = "Password and its confirmation do not match")]
		public string ConfirmPassword { get; set; }
	}
}
