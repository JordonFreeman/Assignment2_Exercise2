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

        // GET: Orders
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var orders = db.Orders.Include(o => o.Agent).ToList();
            return View(orders);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName");
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            return View();
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Order order, int[] itemIds, int[] quantities)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    order.OrderDate = DateTime.Now;
                    order.Status = "Pending";

                    // Calculate total and create order details
                    order.OrderDetails = new List<OrderDetail>();
                    decimal total = 0;

                    // Verify we have valid items and quantities
                    if (itemIds == null || quantities == null || itemIds.Length == 0 || itemIds.Length != quantities.Length)
                    {
                        ModelState.AddModelError("", "Please select at least one item with a valid quantity.");
                        ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName", order.AgentID);
                        ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                        return View(order);
                    }

                    for (int i = 0; i < itemIds.Length; i++)
                    {
                        var item = db.Items.Find(itemIds[i]);
                        if (item == null)
                        {
                            ModelState.AddModelError("", $"Item with ID {itemIds[i]} not found.");
                            continue;
                        }

                        // Validate quantity is positive
                        if (quantities[i] <= 0)
                        {
                            ModelState.AddModelError("", $"Quantity for {item.ItemName} must be greater than zero.");
                            continue;
                        }

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

                    // If we have errors after processing items, return to form
                    if (!ModelState.IsValid)
                    {
                        ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName", order.AgentID);
                        ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
                        return View(order);
                    }

                    order.TotalAmount = total;
                    db.Orders.Add(order);
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Order created successfully!";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred: " + ex.Message);
                }
            }

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName", order.AgentID);
            ViewBag.Items = new SelectList(db.Items, "ItemID", "ItemName");
            return View(order);
        }

        // GET: Orders/Details/5
        public ActionResult Details(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var order = db.Orders
                .Include(o => o.Agent)
                .Include(o => o.OrderDetails.Select(od => od.Item))
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Orders/Print/5
        public ActionResult Print(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            // Use proper Include to fetch all related data
            var order = db.Orders
                .Include(o => o.Agent)
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Get order details directly to avoid issues with lazy loading
            var orderDetails = db.OrderDetails
                .Include(od => od.Item)
                .Where(od => od.OrderID == id)
                .ToList();

            ViewBag.OrderDetails = orderDetails;

            return View(order);
        }

        // Optional: Add Edit and Delete actions

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