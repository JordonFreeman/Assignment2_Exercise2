using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models;
using System.Web.Mvc;
using System.Data.Entity;

namespace OrderManagement.Web.Controllers
{
    public class ItemsController : Controller
    {
        private OrderManagementDBEntities db = new OrderManagementDBEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            // Return all items from the database
            return View(db.Items.ToList());
        }

        public ActionResult Create()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            return View();
        }

        [HttpPost]
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public ActionResult Edit(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            Item item = db.Items.Find(id);
            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(item);
        }

        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            Item item = db.Items.Find(id);
            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        public ActionResult Delete(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            Item item = db.Items.Find(id);
            if (item == null)
                return HttpNotFound();

            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Item item = db.Items.Find(id);
            db.Items.Remove(item);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}