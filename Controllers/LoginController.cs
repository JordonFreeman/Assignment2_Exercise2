using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models;
using static System.Collections.Specialized.BitVector32;
using System.Web.Mvc;
using System.Data.Entity;

namespace OrderManagement.Web.Controllers
{
    public class LoginController : Controller
    {
        private OrderManagementDBEntities db = new OrderManagementDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == username && u.Password == password);
            if (user != null)
            {
                Session["UserID"] = user.UserID;
                Session["Username"] = user.UserName;
                return RedirectToAction("Index", "Items");
            }

            ViewBag.Error = "Invalid username or password";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}