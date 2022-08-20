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
using Microsoft.AspNet.Identity;
using System.Data.Entity.Infrastructure;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class ItemsController : Controller
  {
    private InventoryPortalEntities db = new InventoryPortalEntities();

    // GET: Items
    public async Task<ActionResult> Index()
    {
      var items = db.Items.Where(x => x.IsDeleted == false).Include(i => i.AspNetUser).Include(i => i.DeviceType).Include(i => i.Location).Include(i => i.Status);
      return View(await items.ToListAsync());
    }


    // GET: Items/Create
    [Authorize(Roles = "Admin,Manager")]
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
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Create([Bind(Include = "Id,AspNetUserId,Barcode,SerialNumber,DeviceTypeId,Manufacturer,Model,Description,ReceivingDate,WarrantyExpiryDate,LocationId,StatusId,Racked,RackId,UID,Remarks,ItemLogId,DateCreated,DateModified")] Item item)
    {

      item.Id = Guid.NewGuid().ToString();
      item.DateCreated = DateTime.Now;
      item.AspNetUserId = User.Identity.GetUserId();

      if (ModelState.IsValid)
      {
        db.Items.Add(item);

        ItemLog itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = item.AspNetUserId, Description = "Device Added", EventDateTime = DateTime.Now, ItemId = item.Id };
        db.ItemLogs.Add(itemLog);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", item.AspNetUserId);
      ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type", item.DeviceTypeId);
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
      ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type", item.DeviceTypeId);
      ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName", item.LocationId);
      ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName", item.StatusId);
      Item orginalItem = db.Items.FirstOrDefault(x => x.Id == item.Id);
      TempData["orginalItem"] = orginalItem;


      return View(item);
    }

    // POST: Items/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<ActionResult> Edit([Bind(Include = "Id,AspNetUserId,Barcode,SerialNumber,DeviceTypeId,Manufacturer,Model,Description,ReceivingDate,WarrantyExpiryDate,LocationId,StatusId,Racked,RackId,UID,Remarks,ItemLogId,DateCreated,DateModified")] Item item)
    {
      //Get Orignal value before save changes // this code working fine but not needed as done by in only one line.
      //Item entityBeforeChange = db.Items.Single(x => x.Id == item.Id);
      //db.Entry(entityBeforeChange).State = EntityState.Detached; // breaks up the connection to the Context
      //Location oldLocation = db.Locations.Single(x => x.Id == entityBeforeChange.LocationId);//Orignal value

      Item orginalItem = (Item)TempData["orginalItem"];


      item.DateModified = DateTime.Now;
      if (ModelState.IsValid)
      {
        db.Entry(item).State = EntityState.Modified;
        db.Entry(item).Property(x => x.AspNetUserId).IsModified = false;
        db.Entry(item).Property(x => x.DateCreated).IsModified = false;


        if (orginalItem != null)
        {
          //item.AspNetUserId = User.Identity.GetUserId();
          //Location newLocation = db.Locations.Single(x => x.Id == item.LocationId);
          ItemLog itemLog;
          

          //Barcode changed
          if (item.Barcode != orginalItem.Barcode)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Barcode: " + orginalItem.Barcode + " → " + item.Barcode, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Description changed
          if (item.Description != orginalItem.Description)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Description: " + orginalItem.Description + " → " + item.Description, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //DeviceType changed
          if (item.DeviceTypeId != orginalItem.DeviceTypeId)
          {
            DeviceType deviceType = db.DeviceTypes.Find(item.DeviceTypeId);
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Device Type: " + (orginalItem==null?"": orginalItem.DeviceType.Type) + " → " + (deviceType==null?"": deviceType.Type), EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Location changed
          if (item.LocationId != orginalItem.LocationId)
          {
            Location location= db.Locations.Find(item.LocationId);
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Location: " + (orginalItem.Location==null ? "": orginalItem.Location.LocationName) + " → " + (location==null?"": location.LocationName), EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Manufacturer changed
          if (item.Manufacturer != orginalItem.Manufacturer)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Manufacturer: " + orginalItem.Manufacturer + " → " + item.Manufacturer, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Model changed
          if (item.Model != orginalItem.Model)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Model: " + orginalItem.Model + " → " + item.Model, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Racked changed
          if (item.Racked != orginalItem.Racked)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Racked: " + (orginalItem.Racked ? "Yes" : "No") + " → " + (item.Racked ? "Yes" : "No"), EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //RackId changed
          if (item.RackId != orginalItem.RackId)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "RackId: " + orginalItem.RackId + " → " + item.RackId, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //ReceivingDate changed
          if (item.ReceivingDate != orginalItem.ReceivingDate)
          {
            //itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Receiving: " + orginalItem.ReceivingDate.Value.ToString("dd MMM yyyy") + " → " + item.ReceivingDate.Value.ToString("dd MMM yyyy"), EventDateTime = DateTime.Now, ItemId = item.Id };
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "ReceivingDate: " + (orginalItem.ReceivingDate.HasValue ? orginalItem.ReceivingDate.Value.ToString("dd MMM yyyy") : string.Empty) + " → " + (item.ReceivingDate.HasValue ? item.ReceivingDate.Value.ToString("dd MMM yyyy") : string.Empty), EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //Remarks changed
          if (item.Remarks != orginalItem.Remarks)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Remarks: " + orginalItem.Remarks + " → " + item.Remarks, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //SerialNumber changed
          if (item.SerialNumber != orginalItem.SerialNumber)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Serial: " + orginalItem.SerialNumber + " → " + item.SerialNumber, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //StatusName changed
          if (item.StatusId != orginalItem.StatusId)
          {
            Status status= db.Status.Find(item.StatusId);
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Status: " + (orginalItem.Status == null? "" : orginalItem.Status.StatusName) + " → " + (status==null?"": status.StatusName), EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //UID changed
          if (item.UID != orginalItem.UID)
          {
            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "UID: " + orginalItem.UID + " → " + item.UID, EventDateTime = DateTime.Now, ItemId = item.Id };
            db.ItemLogs.Add(itemLog);
          }

          //WarrantyExpiryDate changed
          if (item.WarrantyExpiryDate != orginalItem.WarrantyExpiryDate)
          {

            //if (orginalItem.WarrantyExpiryDate != null ||  orginalItem.WarrantyExpiryDate.Value != null)
            //{

            //}

            //if (item.WarrantyExpiryDate != null || item.WarrantyExpiryDate.Value != null)
            //{

            //}

            itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "WarrantyExpiryDate: " + (orginalItem.WarrantyExpiryDate.HasValue ? orginalItem.WarrantyExpiryDate.Value.ToString("dd MMM yyyy") : string.Empty) + " → " + (item.WarrantyExpiryDate.HasValue ? item.WarrantyExpiryDate.Value.ToString("dd MMM yyyy") : string.Empty), EventDateTime = DateTime.Now, ItemId = item.Id };

            db.ItemLogs.Add(itemLog);
          }
         




        }





        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      ViewBag.AspNetUserId = new SelectList(db.AspNetUsers, "Id", "Hometown", item.AspNetUserId);
      ViewBag.DeviceTypeId = new SelectList(db.DeviceTypes, "Id", "Type", item.DeviceTypeId);
      ViewBag.LocationId = new SelectList(db.Locations, "Id", "LocationName", item.LocationId);
      ViewBag.StatusId = new SelectList(db.Status, "Id", "StatusName", item.StatusId);
      return View(item);
    }

    // GET: Items/Delete/5
    [Authorize(Roles = "Admin")]
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
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteConfirmed(string id)
    {
      Item item = await db.Items.FindAsync(id);
      //db.Items.Remove(item);
      item.IsDeleted = true;
      db.Entry(item).State = EntityState.Modified;
      db.Entry(item).Property(x => x.IsDeleted).IsModified = true;

      ItemLog itemLog = new ItemLog { Id = Guid.NewGuid().ToString(), AspNetUserId = User.Identity.GetUserId(), Description = "Device Deleted", EventDateTime = DateTime.Now, ItemId = item.Id };
      db.ItemLogs.Add(itemLog);
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
