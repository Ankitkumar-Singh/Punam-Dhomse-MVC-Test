using BookStore.Models;
using System.Linq;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class UserDetailsController : Controller
    {
        #region "Private variable"
        private BookStoreContext db = new BookStoreContext();
        #endregion

        #region "Available Books"
        /// <summary>Availables the books.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult AvailableBooks()
        {
            if (Session["UserEmail"] != null)
            {
                var availableBooks = db.BookDetails.ToList();
                ViewBag.products = availableBooks;
                return View();
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion
    }
}
