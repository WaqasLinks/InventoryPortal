using LeaveON.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class ApproverReviewController : Controller
  {
    public string myConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    // GET: ApproverReview
    public ActionResult Index()
    {
      var records = GetAllApproverRecords();
      return View(records);

    }

    [HttpPost]
    public List<ApproverReview> GetAllApproverRecords()
    {
      var result = new List<ApproverReview>();
      using (SqlConnection conn = new SqlConnection(myConnectionString))
      //using (SqlCommand cmd = new SqlCommand("select Id,TableName BatchNo,FormName FormTitle,DateModified from  DynamicForm", conn))
      using (SqlCommand cmd = new SqlCommand("select * from  DynamicForm", conn))
      {
        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
        adapt.SelectCommand.CommandType = CommandType.Text;

        DataTable dt = new DataTable();
        adapt.Fill(dt);

        if (dt.Rows.Count > 0)
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            var record = new ApproverReview();
            //record.Id = dt.Rows[i]["Id"].ToString();
            //record.BatchNo = dt.Rows[i]["BatchNo"].ToString();
            //record.FormTitle = dt.Rows[i]["FormTitle"].ToString();
            //record.ModifiedDate = dt.Rows[i]["DateModified"].ToString();

            record.FormName = dt.Rows[i]["FormName"].ToString();
            record.TableName = dt.Rows[i]["TableName"].ToString();
            
            result.Add(record);
          }

        }
        return result;
      }
    }
  }
}
