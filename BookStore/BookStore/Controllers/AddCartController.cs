using BookStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;

namespace BookStore.Controllers
{
    public class AddCartController : Controller
    {
        #region "Private variable"
        private BookStoreContext db = new BookStoreContext();
        #endregion

        #region "Index"
        /// <summary>Indexes this instance.</summary>
        /// <returns>ActionResult.</returns>
        public ActionResult Index()
        {
            if (Session["UserEmail"] == null)
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
            return View();
        }
        #endregion

        #region "Buy"
        /// <summary>Buys the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Buy(int id)
        {
            if (Session["UserRole"] != null && Session["UserRole"].ToString() == "2")
            {
                List<Cart> cart = new List<Cart>();
                if (cart == null)
                {
                    return RedirectToAction("Index");
                }
                BookDetail bookDetail = new BookDetail();
                if (Session["cart"] == null)
                {
                    cart.Add(new Cart { BookDetail = db.BookDetails.Single(p => p.ISBN == id), Quantity = 1 });
                    Session["cart"] = cart;
                }
                else
                {
                    cart = (List<Cart>)Session["cart"];
                    if (cart == null)
                    {
                        return RedirectToAction("Index");
                    }
                    int index = isExist(id);
                    if (index != -1)
                    {
                        cart[index].Quantity++;
                    }
                    else
                    {
                        cart.Add(new Cart { BookDetail = db.BookDetails.Single(p => p.ISBN == id), Quantity = 1 });
                    }
                    ViewBag.cartItem = cart.Count();
                    Session["cart"] = cart;
                }
                if (cart != null)
                {
                    ViewBag.cartItem = cart.Count();
                }
                else
                {
                    ViewBag.cartItem = "0";
                }
                Session["count"] = ViewBag.cartItem;
                return RedirectToAction("Index");
            }
            else
            {
                Response.Write("<script>alert('Please Login')</script>");
                Session.Clear();
                return RedirectToAction("SignIn", "Authentication");
            }
        }
        #endregion

        #region "Remove"
        /// <summary>Removes the specified identifier.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult Remove(int id)
        {
            List<Cart> cart = (List<Cart>)Session["cart"];
            int index = isExist(id);
            cart.RemoveAt(index);
            Session["cart"] = cart;
            return RedirectToAction("Index");
        }
        #endregion

        #region "Cash on delivery"
        /// <summary>Cashes the on delivery.</summary>
        /// <param name="orderIds">The order ids.</param>
        /// <returns>ActionResult.</returns>
        public ActionResult CashOnDelivery(string orderIds)
        {
            var Email = Session["UserEmail"].ToString();
            if (orderIds == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            else
            {
                List<string> lstOrderIds = Request.Params["orderIds"].Replace("[", "").Replace("]", "").Split(',').ToList();
                foreach (var orderId in lstOrderIds)
                {
                    Order order = new Order();
                    int orderIdToInsert = Convert.ToInt32(orderId);
                    BookDetail bookDetail = db.BookDetails.Where(x => x.ISBN == orderIdToInsert).FirstOrDefault();
                    UserDetail user = db.UserDetails.Single(x => x.Email == Email);
                    order.OrderDate = DateTime.Now;
                    order.User_Id = user.User_Id;
                    order.ISBM = bookDetail.ISBN;
                    db.Orders.Add(order);
                    db.SaveChanges();
                }
                var message = new MailMessage();
                message.To.Add(new MailAddress(Convert.ToString(Session["UserEmail"])));
                message.From = new MailAddress("aress.iphone5@gmail.com");
                message.Subject = "Order placed successfully";
                message.Body = "Your order is placed. Thank you for shopping.";
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient())
                {
                    var credential = new NetworkCredential
                    {
                        UserName = "aress.iphone5@gmail.com",
                        Password = "Aress123$"
                    };
                    smtp.Credentials = credential;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
                return RedirectToAction("AvailableBooks", "UserDetails");
            }
        }
        #endregion

        #region "If exist"
        /// <summary>Determines whether the specified identifier is exist.</summary>
        /// <param name="id">The identifier.</param>
        /// <returns>System.Int32.</returns>
        private int isExist(int id)
        {
            List<Cart> cart = (List<Cart>)Session["cart"];
            for (int i = 0; i < cart.Count; i++)
                if (cart[i].BookDetail.ISBN.Equals(id))
                    return i;
            return -1;
        }
        #endregion
    }
}