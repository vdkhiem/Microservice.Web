
using System.ComponentModel.DataAnnotations;

namespace Microservice.Web.Models.Account
{
    public class Confirm
    {
		[Required(ErrorMessage = "Email is required")]
		[Display(Name = "Email")]
		[EmailAddress]
		public string Email { get; set; }

		[Required(ErrorMessage = "Code is required")]
		public string Code { get; set; }
	}
}
