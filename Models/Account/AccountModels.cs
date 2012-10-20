using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Core.Data.Entities;
using ModelRes.Account;

namespace TolokaStudio.Models
{

    public class ChangePasswordModel
    {
        [Required(ErrorMessageResourceName = "Required_current_password",
                ErrorMessageResourceType = typeof(Account))]
        [DataType(DataType.Password)]
        [Display(Name = "Current_password", ResourceType = typeof(Account))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "Required_new_password",
                ErrorMessageResourceType = typeof(Account))]
        [StringLength(100, ErrorMessageResourceName =  "{0} має бути  довжиною не манше {2} символів" , MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New_password", ResourceType = typeof(Account))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm_new_password", ResourceType = typeof(Account))]
        [Compare("NewPassword", ErrorMessageResourceName = "The_new_password_and_confirmation_password_do_not_match",
                ErrorMessageResourceType = typeof(Account))]
        public string ConfirmPassword { get; set; }
    }

    public class LogOnModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} має бути  довжиною не манше {2} символів", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

}
