using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models;
using System.Web.Mvc;
using System.Data.Entity;

namespace OrderManagement.Web.Controllers
{
    public class OrdersController : Controller
    {
        private OrderManagementDBEntities db = new OrderManagementDBEntities();

        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var orders = db.Orders.Include(o => o.Agent).ToList();
            return View(orders);
        }

        public ActionResult Create()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Order order, int[] itemIds, int[] quantities)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.Status = "Pending";

                // Calculate total and create order details
                order.OrderDetails = new List<OrderDetail>();
                decimal total = 0;

                for (int i = 0; i < itemIds.Length; i++)
                {
                    var item = db.Items.Find(itemIds[i]);
                    var detail = new OrderDetail
                    {
                        ItemID = itemIds[i],
                        Quantity = quantities[i],
                        UnitAmount = item.UnitPrice,
                        TotalAmount = item.UnitPrice * quantities[i]
                    };

                    order.OrderDetails.Add(detail);
                    total += detail.TotalAmount;
                }

                order.TotalAmount = total;

                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            return View(order);
        }

        public ActionResult Print(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var order = db.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails.Select(od => od.Item))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null) {
                var items = order.OrderDetails.Select(od => od.Item).ToList();
                return HttpNotFound();
            }

            return View(order);
        }
    }
}