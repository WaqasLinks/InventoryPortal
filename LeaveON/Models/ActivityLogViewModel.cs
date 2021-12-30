using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeaveON.Models
{
  public class ActivityLogViewModel
  {
    public string ActivityByUser { get; set; }
    public string DeviceDesc { get; set; }
    public DateTime ActivityDateTime { get; set; }
    public string ActivityDesc { get; set; }
  }
}
