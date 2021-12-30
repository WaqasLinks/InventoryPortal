using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using InventoryRepo.Models;

namespace LeaveON.Controllers
{
    public class ItemLogsController : Controller
    {
        private InventoryPortalEntities db = new InventoryPortalEntities();

        // GET: ItemLogs
        public async Task<ActionResult> Index()
        {
            var itemLogs = db.ItemLogs.Include(i => i.AspNetUser).Include(i => i.Item);
            return View(await itemLogs.ToListAsync());
        }

        // GET: ItemLogs/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemLog itemLog = await db.ItemLogs.FindAsync(id);
            if (itemLog == null)
            {
                return HttpNotFound();
            }
            return View(itemLog);
        }

        // GET: ItemLogs/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown");
            ViewBag.ItemId = new SelectList(db.Items, "Id", "AspNetUserId");
            return View();
        }

        // POST: ItemLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,ItemId,AspNetUserId,Description,EventDateTime")] ItemLog itemLog)
        {
            if (ModelState.IsValid)
            {
                db.ItemLogs.Add(itemLog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", itemLog.AspNetUserId);
            ViewBag.ItemId = new SelectList(db.Items, "Id", "AspNetUserId", itemLog.ItemId);
            return View(itemLog);
        }

        // GET: ItemLogs/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemLog itemLog = await db.ItemLogs.FindAsync(id);
            if (itemLog == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", itemLog.AspNetUserId);
            ViewBag.ItemId = new SelectList(db.Items, "Id", "AspNetUserId", itemLog.ItemId);
            return View(itemLog);
        }

        // POST: ItemLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,ItemId,AspNetUserId,Description,EventDateTime")] ItemLog itemLog)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itemLog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", itemLog.AspNetUserId);
            ViewBag.ItemId = new SelectList(db.Items, "Id", "AspNetUserId", itemLog.ItemId);
            return View(itemLog);
        }

        // GET: ItemLogs/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemLog itemLog = await db.ItemLogs.FindAsync(id);
            if (itemLog == null)
            {
                return HttpNotFound();
            }
            return View(itemLog);
        }

        // POST: ItemLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ItemLog itemLog = await db.ItemLogs.FindAsync(id);
            db.ItemLogs.Remove(itemLog);
            await db.SaveChangesAsync();
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
