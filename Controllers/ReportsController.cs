using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OrderManagement.Web.Models;
using System.Data.Entity;

namespace OrderManagement.Web.Controllers
{
    public class ReportsController : Controller
    {
        private OrderManagementDBEntities db = new OrderManagementDBEntities();

        // GET: Reports
        public ActionResult Index()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            return View("Index"); // Be explicit about the view name
        }

        // GET: Reports/BestItems
        public ActionResult BestItems()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            try
            {
                // Try to use the database view if it exists
                var bestItems = db.Database.SqlQuery<BestSellingItemViewModel>(
                    "SELECT i.ItemID, i.ItemName, i.Size, i.UnitPrice, i.StockQuantity, " +
                    "SUM(od.Quantity) AS TotalQuantitySold, SUM(od.TotalAmount) AS TotalRevenue " +
                    "FROM Item i " +
                    "JOIN OrderDetail od ON i.ItemID = od.ItemID " +
                    "GROUP BY i.ItemID, i.ItemName, i.Size, i.UnitPrice, i.StockQuantity " +
                    "ORDER BY TotalQuantitySold DESC"
                ).ToList();

                return View("BestItems", bestItems); // Be explicit about the view name and model type
            }
            catch (Exception ex)
            {
                // Fallback method if the view or query fails
                ViewBag.ErrorMessage = "Unable to load best selling items: " + ex.Message;

                // Get best selling items manually
                var orderDetails = db.OrderDetails
                    .Include(od => od.Item)
                    .ToList();

                var bestItems = orderDetails
                    .GroupBy(od => od.ItemID)
                    .Select(g => new BestSellingItemViewModel
                    {
                        ItemID = g.Key,
                        ItemName = g.First().Item.ItemName,
                        Size = g.First().Item.Size,
                        UnitPrice = g.First().Item.UnitPrice,
                        StockQuantity = g.First().Item.StockQuantity,
                        TotalQuantitySold = g.Sum(od => od.Quantity),
                        TotalRevenue = g.Sum(od => od.TotalAmount)
                    })
                    .OrderByDescending(i => i.TotalQuantitySold)
                    .ToList();

                return View("BestItems", bestItems as IEnumerable<BestSellingItemViewModel>); // Be explicit
            }
        }

        // GET: Reports/ItemsByAgent
        public ActionResult ItemsByAgent(int? agentId)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            // Populate agents dropdown
            ViewBag.Agents = new SelectList(db.Agents.OrderBy(a => a.AgentName), "AgentID", "AgentName");

            // If agent is selected, show items sold by that agent
            if (agentId.HasValue)
            {
                var agent = db.Agents.Find(agentId.Value);
                ViewBag.AgentName = agent?.AgentName;

                try
                {
                    // Try to use stored procedure if it exists
                    var itemsSold = db.Database.SqlQuery<Item>(
                        "EXEC sp_GetItemsPurchasedByAgent @AgentID",
                        new System.Data.SqlClient.SqlParameter("@AgentID", agentId.Value)
                    ).ToList();

                    // If no items found, get them manually
                    if (itemsSold.Count == 0)
                    {
                        throw new Exception("No items found via stored procedure");
                    }

                    // Get quantities sold
                    var orderIds = db.Orders
                        .Where(o => o.AgentID == agentId.Value)
                        .Select(o => o.OrderID)
                        .ToList();

                    var itemQuantities = new Dictionary<int, object>();

                    foreach (var item in itemsSold)
                    {
                        var quantity = db.OrderDetails
                            .Where(od => orderIds.Contains(od.OrderID) && od.ItemID == item.ItemID)
                            .Sum(od => od.Quantity);

                        var amount = db.OrderDetails
                            .Where(od => orderIds.Contains(od.OrderID) && od.ItemID == item.ItemID)
                            .Sum(od => od.TotalAmount);

                        itemQuantities[item.ItemID] = new { Quantity = quantity, Amount = amount };
                    }

                    ViewBag.Quantities = itemQuantities;
                    ViewBag.TotalAmount = itemQuantities.Sum(q => ((dynamic)q.Value).Amount);

                    return View("ItemsByAgent", itemsSold as IEnumerable<Item>); // Be explicit
                }
                catch (Exception ex)
                {
                    // Fallback method if stored procedure fails
                    ViewBag.ErrorMessage = "Using manual query instead of stored procedure: " + ex.Message;

                    // Get order IDs for this agent
                    var orderIds = db.Orders
                        .Where(o => o.AgentID == agentId.Value)
                        .Select(o => o.OrderID)
                        .ToList();

                    // Get unique item IDs from these orders
                    var itemIds = db.OrderDetails
                        .Where(od => orderIds.Contains(od.OrderID))
                        .Select(od => od.ItemID)
                        .Distinct()
                        .ToList();

                    // Get the items
                    var items = db.Items
                        .Where(i => itemIds.Contains(i.ItemID))
                        .ToList();

                    // Get quantities for each item
                    var itemQuantities = new Dictionary<int, object>();

                    foreach (var itemId in itemIds)
                    {
                        var quantity = db.OrderDetails
                            .Where(od => orderIds.Contains(od.OrderID) && od.ItemID == itemId)
                            .Sum(od => od.Quantity);

                        var amount = db.OrderDetails
                            .Where(od => orderIds.Contains(od.OrderID) && od.ItemID == itemId)
                            .Sum(od => od.TotalAmount);

                        itemQuantities[itemId] = new { Quantity = quantity, Amount = amount };
                    }

                    ViewBag.Quantities = itemQuantities;
                    ViewBag.TotalAmount = itemQuantities.Sum(q => ((dynamic)q.Value).Amount);

                    return View("ItemsByAgent", items as IEnumerable<Item>); // Be explicit
                }
            }

            return View("ItemsByAgent", new List<Item>() as IEnumerable<Item>); // Be explicit
        }

        // GET: Reports/OrderDetails
        public ActionResult OrderDetails(int? orderId)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            // Get orders for dropdown
            var ordersData = db.Orders
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.OrderID,
                    o.OrderDate,
                    o.TotalAmount
                })
                .ToList();

            // Then format the display text in memory
            var orders = ordersData
                .Select(o => new
                {
                    o.OrderID,
                    DisplayText = string.Format("Order #{0} - {1:MM/dd/yyyy} - ${2}",
                        o.OrderID, o.OrderDate, o.TotalAmount)
                })
                .ToList();

            ViewBag.Orders = new SelectList(orders, "OrderID", "DisplayText");

            if (orderId.HasValue)
            {
                // Get order header
                var order = db.Orders
                    .Include(o => o.Agent)
                    .FirstOrDefault(o => o.OrderID == orderId.Value);

                if (order == null)
                {
                    return HttpNotFound();
                }

                // Get order details
                var orderDetails = db.OrderDetails
                    .Where(od => od.OrderID == orderId.Value)
                    .ToList();

                // Get items for each order detail
                foreach (var detail in orderDetails)
                {
                    detail.Item = db.Items.Find(detail.ItemID);
                }

                ViewBag.OrderDetails = orderDetails;
                return View("OrderDetails", order); // Be explicit
            }

            return View("OrderDetails", (Order)null); // Be explicit with null cast
        }

        // GET: Reports/Print
        public ActionResult Print(int id)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var order = db.Orders
                .Include(o => o.Agent)
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            // Get order details
            var orderDetails = db.OrderDetails
                .Where(od => od.OrderID == id)
                .ToList();

            // For each order detail, get the item info
            foreach (var detail in orderDetails)
            {
                detail.Item = db.Items.Find(detail.ItemID);
            }

            ViewBag.OrderDetails = orderDetails;

            return View("Print", order); // Be explicit
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

    // ViewModel for Best Selling Items
    public class BestSellingItemViewModel
    {
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public string Size { get; set; }
        public decimal UnitPrice { get; set; }
        public int StockQuantity { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}