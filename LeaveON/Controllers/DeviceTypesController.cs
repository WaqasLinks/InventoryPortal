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
  public class DeviceTypesController : Controller
  {
    private InventoryPortalEntities db = new InventoryPortalEntities();

    // GET: DeviceTypes
    public async Task<ActionResult> Index()
    {
      return View(await db.DeviceTypes.ToListAsync());
    }

    // GET: DeviceTypes/Details/5
    public async Task<ActionResult> Details(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      DeviceType deviceType = await db.DeviceTypes.FindAsync(id);
      if (deviceType == null)
      {
        return HttpNotFound();
      }
      return View(deviceType);
    }

    // GET: DeviceTypes/Create
    public ActionResult Create()
    {
      return View();
    }

    // POST: DeviceTypes/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create([Bind(Include = "Id,Type,Remarks,DateCreated,DateModified")] DeviceType deviceType)
    {
      deviceType.Id = Guid.NewGuid().ToString();
      deviceType.DateCreated = DateTime.Now;
      if (ModelState.IsValid)
      {
        db.DeviceTypes.Add(deviceType);
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }

      return View(deviceType);
    }

    // GET: DeviceTypes/Edit/5
    public async Task<ActionResult> Edit(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      DeviceType deviceType = await db.DeviceTypes.FindAsync(id);
      if (deviceType == null)
      {
        return HttpNotFound();
      }
      return View(deviceType);
    }

    // POST: DeviceTypes/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to, for 
    // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit([Bind(Include = "Id,Type,Remarks,DateCreated,DateModified")] DeviceType deviceType)
    {
      deviceType.DateModified = DateTime.Now;
      if (ModelState.IsValid)
      {
        db.Entry(deviceType).State = EntityState.Modified;
        db.Entry(deviceType).Property(x => x.DateCreated).IsModified = false;
        await db.SaveChangesAsync();
        return RedirectToAction("Index");
      }
      return View(deviceType);
    }

    // GET: DeviceTypes/Delete/5
    public async Task<ActionResult> Delete(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      DeviceType deviceType = await db.DeviceTypes.FindAsync(id);
      if (deviceType == null)
      {
        return HttpNotFound();
      }
      return View(deviceType);
    }

    // POST: DeviceTypes/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> DeleteConfirmed(string id)
    {
      DeviceType deviceType = await db.DeviceTypes.FindAsync(id);
      db.DeviceTypes.Remove(deviceType);
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
