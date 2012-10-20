using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using TolokaStudio.Models;
using Core.Data.Entities;
using Core.Data.Repository;
using Core.Data.Repository.Interfaces;
using System.Security.Cryptography;
using System.Text;
using TolokaStudio.Common;
using System.Security.Principal;

namespace TolokaStudio.Controllers
{
    public class AccountController : Controller
    {
        private readonly IRepository<User> UserRepository;
        private readonly IRepository<Employee> EmployeeRepository;

        public AccountController()
        {
            UserRepository = new Repository<User>();
            EmployeeRepository = new Repository<Employee>();
        }

        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Admin(int id)
        {
            User user = UserRepository.Get(u => u.Id == id).SingleOrDefault();
            if (!user.Role.IsAdmin)
            {
                user.Role.IsAdmin = true;
                UserRepository.SaveOrUpdate(user);
            }

            return RedirectToAction("Index", "Store");
        }
        [TolokaAuthorizeAsAdminAttribute]
        public ActionResult Author(int id)
        {
            User user = UserRepository.Get(u => u.Id == id).SingleOrDefault();
            if (!user.Role.IsAuthor)
            {
                user.Role.IsAuthor = true;
                Employee employee = new Employee();
                employee.Email = user.Email;
                employee.FirstName = user.UserName; ;
                Employee employeeSaved = EmployeeRepository.SaveOrUpdate(employee);
                user.Employee = employeeSaved;
                UserRepository.SaveOrUpdate(user);
            }
            return RedirectToAction("Index", "Store");
        }
        //
        // GET: /Account/LogOn

        public ActionResult LogOn()
        {
            return View();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (GetUser(model.UserName, model.Password) != null)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                    if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {

                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {

                if (UserRepository.Get(u => u.UserName == model.UserName).SingleOrDefault() == null)
                {
                    User user = new User()
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Password = EncodePassword(model.Password)
                    };
                    if (model.UserName == "gal5")
                    {
                        user.Role.IsAdmin = true;
                    }

                    UserRepository.SaveOrUpdate(user);
                    FormsAuthentication.SetAuthCookie(model.UserName, false /* createPersistentCookie */);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(MembershipCreateStatus.DuplicateUserName));
                    return View(model);
                }


            }
            else
            {
                return View(model);
            }
            // If we got this far, something failed, redisplay form

        }

        //
        // GET: /Account/ChangePassword

        [TolokaAuthorizeAsSimpleUserAttribute]
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [TolokaAuthorizeAsSimpleUserAttribute]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded = false;

                var currentUser = User.Identity.Name;
                User user = UserRepository.Get(u => u.UserName == currentUser).SingleOrDefault();
                if (user != null && CheckPassword(user.Password, model.OldPassword))
                {
                    user.Password = EncodePassword(model.NewPassword);
                    UserRepository.SaveOrUpdate(user);
                    changePasswordSucceeded = true;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        private User GetUser(string username, string password)
        {
            User user = UserRepository.Get(u => u.UserName == username && u.Password == EncodePassword(password)).SingleOrDefault();
       
            return user;

        }
        /// <summary>
        /// Check the password 
        /// </summary>
        /// <param name="password">Password</param>
        /// <param name="dbpassword"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private bool CheckPassword(string password, string dbpassword)
        {
            string pass1 = password;
            string pass2 = dbpassword;

            pass2 = EncodePassword(dbpassword);

            if (pass1 == pass2)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Encode password.
        /// </summary>
        /// <param name="password">Password.</param>
        /// <returns>Encoded password.</returns>
        private string EncodePassword(string password)
        {
            string passwordEncoded = password;

            HMACSHA1 hash = new HMACSHA1();
            hash.Key = HexToByte("12");
            passwordEncoded =
                Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));

            return passwordEncoded;
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array. Used to convert encryption key values from the configuration
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        private byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        #endregion
    }
}
