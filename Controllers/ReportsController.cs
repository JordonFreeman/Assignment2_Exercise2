using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OrderManagement.Web.Models;
using System.Web.Mvc;

namespace OrderManagement.Web.Controllers
{
    public class ReportsController : Controller
    {
        private OrderManagementDBEntities db = new OrderManagementDBEntities();

        public ActionResult BestItems()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var items = db.OrderDetails
                .GroupBy(d => d.ItemID)
                .Select(g => new {
                    ItemID = g.Key,
                    TotalQuantity = g.Sum(d => d.Quantity)
                })
                .OrderByDescending(x => x.TotalQuantity)
                .Take(10)
                .ToList();

            var itemIds = items.Select(x => x.ItemID).ToList();
            var bestItems = db.Items.Where(i => itemIds.Contains(i.ItemID)).ToList();

            return View(bestItems);
        }

        public ActionResult ItemsByAgent()
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName");
            return View();
        }

        [HttpPost]
        public ActionResult ItemsByAgent(int agentId)
        {
            if (Session["UserID"] == null)
                return RedirectToAction("Index", "Login");

            var agent = db.Agents.Find(agentId);
            ViewBag.AgentName = agent.AgentName;

            var itemIds = db.Orders
                .Where(o => o.AgentID == agentId)
                .SelectMany(o => o.OrderDetails.Select(d => d.ItemID))
                .Distinct()
                .ToList();

            var items = db.Items.Where(i => itemIds.Contains(i.ItemID)).ToList();

            ViewBag.Agents = new SelectList(db.Agents, "AgentID", "AgentName", agentId);
            return View(items);
        }
    }
}