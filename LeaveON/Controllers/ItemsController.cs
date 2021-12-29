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
    public class ItemsController : Controller
    {
        private InventoryPortalEntities db = new InventoryPortalEntities();

        // GET: Items
        public async Task<ActionResult> Index()
        {
            var items = db.Items.Include(i => i.AspNetUser).Include(i => i.DeviceType).Include(i => i.ItemLog).Include(i => i.Location).Include(i => i.Status);
            return View(await items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown");
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type");
            ViewBag.ItemLogId = new SelectList(db.ItemLogs, "Id", "Description");
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName");
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName");
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,AspNetUserId,Barcode,SerialNumber,DeviceTypeId,Manufacturer,Model,Description,ReceivingDate,WarrantyExpiryDate,LocationId,StatusId,Racked,Remarks,ItemLogId,DateCreated,DateModified")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(item);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", item.AspNetUserId);
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type");
            ViewBag.ItemLogId = new SelectList(db.ItemLogs, "Id", "Description", item.ItemLogId);
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName", item.LocationId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName", item.StatusId);
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", item.AspNetUserId);
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type");
            ViewBag.ItemLogId = new SelectList(db.ItemLogs, "Id", "Description", item.ItemLogId);
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName", item.LocationId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName", item.StatusId);
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,AspNetUserId,Barcode,SerialNumber,DeviceTypeId,Manufacturer,Model,Description,ReceivingDate,WarrantyExpiryDate,LocationId,StatusId,Racked,Remarks,ItemLogId,DateCreated,DateModified")] Item item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(item).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", item.AspNetUserId);
            ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type");
            ViewBag.ItemLogId = new SelectList(db.ItemLogs, "Id", "Description", item.ItemLogId);
            ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName", item.LocationId);
            ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName", item.StatusId);
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item item = await db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Item item = await db.Items.FindAsync(id);
            db.Items.Remove(item);
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
