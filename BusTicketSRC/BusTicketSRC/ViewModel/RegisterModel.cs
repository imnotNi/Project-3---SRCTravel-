using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusTicketSRC.ViewModel
{
    public class RegisterModel
	{
        public string Username { get; set; }

		[DisplayName("Email")]
		public string Email { get; set; }
		public string Phone { get; set; }
        [Display(Name = "Mật khẩu")]
		[Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		public string Password { get; set; }

		[MinLength(5, ErrorMessage = "Bạn cần đặt mật khẩu tối thiểu 5 ký tự")]
		[Display(Name = "Nhập lại mật khẩu")]
		[Compare("Password", ErrorMessage = "Nhập lại mật khẩu không đúng")]
		public string ConfirmPassword { get; set; }
	}
}
