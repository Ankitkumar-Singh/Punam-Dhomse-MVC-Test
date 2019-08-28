using BookStore.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace BookStore.Controllers
{
    public class AuthenticationController : Controller
    {
        #region "Private variable"
        private BookStoreContext db = new BookStoreContext();
        #endregion

        #region "Sign In"
        /// <summary>Signs the in.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult SignIn()
        {
            return View();
        }

        /// <summary>Signs the in.</summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        public ActionResult SignIn(UserDetail userDetail)
        {
            using (var context = new BookStoreContext())
            {
                var email = userDetail.Email;
                var passsword = userDetail.Password;

                if (context.UserDetails.Any(x => x.Email.Equals(userDetail.Email, StringComparison.Ordinal) && x.Password.Equals(userDetail.Password, StringComparison.Ordinal)))
                {
                    UserDetail user = context.UserDetails.Single(x => x.Email == userDetail.Email);

                    Session["UserEmail"] = user.Email;
                    Session["UserRole"] = user.UserType_Id;

                    FormsAuthentication.SetAuthCookie(user.Email, false);

                    if (user.UserType_Id == 1)
                    {
                        return RedirectToAction("Books", "BookDetails");
                    }
                    else if (user.UserType_Id == 2)
                    {
                        Session["UserName"] = user.Name;
                        return RedirectToAction("AvailableBooks", "UserDetails");
                    }
                    else
                    {
                        return View();
                    }
                }
            }
            ModelState.AddModelError("", "Invalid email and password");
            return View();
        }
        #endregion

        #region "IsEmailAvailable"
        /// <summary>Determines whether [is email available] [the specified email].</summary>
        /// <param name="Email">The email.</param>
        /// <returns>JsonResult.</returns>
        public JsonResult IsEmailAvailable(string Email)
        {
            return Json(!db.UserDetails.Any(x => x.Email == Email), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Sign Up"
        /// <summary>Signs up.</summary>
        /// <returns>ActionResult.</returns>
        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }

        /// <summary>Signs up.</summary>
        /// <param name="userDetail">The user detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(UserDetail userDetail)
        {
            if (ModelState.IsValid)
            {
                userDetail.UserType_Id = 2;
                using (var context = new BookStoreContext())
                {
                    context.UserDetails.Add(userDetail);
                    context.SaveChanges();
                    return RedirectToAction("SignIn");
                }
            }
            return View();
        }
        #endregion

        #region "Sign out"
        /// <summary>Represents an event that is raised when the sign-out operation is complete.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult SignOut()
        {
            Session.Clear();
            return RedirectToAction("SignIn");
        }
        #endregion

        #region "Error"
        /// <summary>Errors this instance.</summary>
        /// <returns></returns>
        public ActionResult Error()
        {
            return View();
        }
        #endregion
    }
}
