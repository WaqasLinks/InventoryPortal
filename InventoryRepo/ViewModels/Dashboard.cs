using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace InventoryRepo.Models
{
  public class Dashboard
  {

        //[DisplayName("Emp.No.")]
        //public int EmployeeNumber { get; set; }
        //public string Country { get; set; }
        //[DisplayFormat(DataFormatString = "{0: dd/MM/yyyy}")]
        //public decimal MyTakenLeaves { get; set; }
        //public decimal MyBalanceLeaves { get; set; }

        [DisplayName("Locations")]
        public int totalStores { get; set; }
        
        [DisplayName("Oprational")]
        public int totalOprational { get; set; }
        
        [DisplayName("Decommissioned")]
        public int totalDecommissioned { get; set; }
        
        [DisplayName("Disposed")]
        public int totalDisposed { get; set; }

    }
}
