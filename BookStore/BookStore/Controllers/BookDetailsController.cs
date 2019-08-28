using BookStore.Models;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PagedList;

namespace BookStore.Controllers
{
    [Authorize]
    public class BookDetailsController : Controller
    {
        #region "Private variable"
        private BookStoreContext db = new BookStoreContext();
        #endregion

        #region "Index"
        /// <summary>Indexes this instance.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index(int? page)
        {
            if (Session["UserEmail"] == null)
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
            var orderDetails = db.Orders.Include(u => u.UserDetail);
            return View(orderDetails.ToList().ToPagedList(page ?? 1,3));
        }
        #endregion

        #region "Author"
        /// <summary>Adds the author.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult AddAuthor()
        {
            if (Session["UserEmail"] == null)
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
            return View();
        }

        /// <summary>Adds the author.</summary>
        /// <param name="authorDetail">The author detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAuthor([Bind(Include = "Name,DateOfBirth,Email")]AuthorDetail authorDetail)
        {
            if (Session["UserEmail"] != null)
            {
                if (ModelState.IsValid)
                {
                    db.AuthorDetails.Add(authorDetail);
                    db.SaveChanges();
                    return RedirectToAction("AuthorList");
                }
                return View(authorDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }

        /// <summary>Authors the list.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult AuthorList()
        {
            if (Session["UserEmail"] != null)
            {
                var authorList = db.AuthorDetails.ToList();
                return View(authorList);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion

        #region "Books"
        /// <summary>Bookses this instance.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Books()
        {
            if (Session["UserEmail"] != null)
            {
                var bookDetails = db.BookDetails.ToList();
                return View(bookDetails);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }

        }
        #endregion

        #region Books Details
        /// <summary>Detailses the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Details(int? id)
        {
            if (Session["UserEmail"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BookDetail bookDetail = db.BookDetails.Find(id);
                if (bookDetail == null)
                {
                    return HttpNotFound();
                }
                return View(bookDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion

        #region Books Insert
        /// <summary>Creates this instance.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Create()
        {
            if (Session["UserEmail"] != null)
            {
                ViewBag.Author_Id = new SelectList(db.AuthorDetails, "Author_Id", "Name");
                return View();
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }

        /// <summary>Creates the specified book detail.</summary>
        /// <param name="bookDetail">The book detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookDetail bookDetail)
        {
            if (Session["UserEmail"] != null)
            {
                string fileName = Path.GetFileName(bookDetail.ImageFile.FileName);
                bookDetail.Url = "~/BooksImages/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/BooksImages/"), fileName);
                bookDetail.ImageFile.SaveAs(fileName);
                ModelState.Clear();
                if (ModelState.IsValid)
                {
                    db.BookDetails.Add(bookDetail);
                    db.SaveChanges();
                    return RedirectToAction("Books");
                }

                ViewBag.Author_Id = new SelectList(db.AuthorDetails, "Author_Id", "Name", bookDetail.Author_Id);
                return View(bookDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion

        #region "Edit"
        /// <summary>Edits the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Edit(int? id)
        {
            if (Session["UserEmail"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BookDetail bookDetail = db.BookDetails.Find(id);
                if (bookDetail == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Author_Id = new SelectList(db.AuthorDetails, "Author_Id", "Name", bookDetail.Author_Id);
                return View(bookDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }


        /// <summary>Edits the specified book detail.</summary>
        /// <param name="bookDetail">The book detail.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookDetail bookDetail)
        {
            BookDetail bookDetailFromDB = db.BookDetails.Single(w => w.ISBN == bookDetail.ISBN);

            if (Session["UserEmail"] != null)
            {
                if (bookDetail.ImageFile != null)
                {
                    string fileName = Path.GetFileName(bookDetail.ImageFile.FileName);
                    bookDetail.Url = "~/BooksImages/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/BooksImages/"), fileName);
                    bookDetail.ImageFile.SaveAs(fileName);
                }
                else
                {
                    bookDetailFromDB.Url = bookDetail.Url;
                }

                if (ModelState.IsValid)
                {
                    bookDetailFromDB.Url = bookDetail.Url;
                    bookDetailFromDB.Tittle = bookDetail.Tittle;
                    bookDetailFromDB.Language = bookDetail.Language;
                    bookDetailFromDB.Publisher = bookDetail.Publisher;
                    bookDetailFromDB.Supplier = bookDetail.Supplier;
                    bookDetailFromDB.Price = bookDetail.Price;
                    bookDetailFromDB.InternationalNumber = bookDetail.InternationalNumber;
                    bookDetailFromDB.ISBN = bookDetail.ISBN;
                    db.Entry(bookDetailFromDB).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Books");
                }
                ViewBag.Author_Id = new SelectList(db.AuthorDetails, "Author_Id", "Name", bookDetail.Author_Id);
                return View(bookDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }

        }
        #endregion

        #region "Delete"
        /// <summary>Deletes the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Delete(int? id)
        {
            if (Session["UserEmail"] != null)
            {

                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                BookDetail bookDetail = db.BookDetails.Find(id);
                if (bookDetail == null)
                {
                    return HttpNotFound();
                }
                return View(bookDetail);
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }

        /// <summary>Deletes the confirmed.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserEmail"] != null)
            {

                BookDetail bookDetail = db.BookDetails.Find(id);
                db.BookDetails.Remove(bookDetail);
                db.SaveChanges();
                return RedirectToAction("Books");
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion

        #region "Dispose"
        /// <summary>Releases unmanaged resources and optionally releases managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
