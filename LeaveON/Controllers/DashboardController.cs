using LeaveON.UtilityClasses;
using Microsoft.AspNet.Identity;
using InventoryRepo.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class DashboardController : Controller
  {
    // GET: Dashboard
    private InventoryPortalEntities db = new InventoryPortalEntities();
    public ActionResult Index()
    {
      Dashboard dashboard = new Dashboard();
      // db.Leaves.Sum(x=>x..LeaveTypeId!=0)

      //string userId = User.Identity.GetUserId();
      //int policyId = db.AspNetUsers.FirstOrDefault(x => x.Id == userId).UserLeavePolicyId.GetValueOrDefault();

      dashboard.totalStores= db.Locations.Count();
      dashboard.totalOprational = db.Items.Where(x => x.StatusId == "1" && x.IsDeleted==false).Count();
      dashboard.totalDecommissioned = db.Items.Where(x => x.StatusId == "2" && x.IsDeleted == false).Count();
      dashboard.totalDisposed = db.Items.Where(x => x.StatusId == "5" && x.IsDeleted == false).Count();

      return View(dashboard);

    }
  }
}
