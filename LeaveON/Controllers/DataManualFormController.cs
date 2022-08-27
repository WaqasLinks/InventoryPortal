using LeaveON.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static LeaveON.Controllers.DataEntryFormController;

namespace LeaveON.Controllers
{
  public class DataManualFormController : Controller
  {
    public string myConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    
    // GET: DataManualForm
    public ActionResult Index()
    {
      var tableNames = GetAllDynamicTables();
      return View(tableNames);
    }

    [HttpPost]
    public List<TableName> GetAllDynamicTables()
    {
      var result = new List<TableName>();
      using (SqlConnection conn = new SqlConnection(myConnectionString))
      using (SqlCommand cmd = new SqlCommand("select Id,TableName,FormName from DynamicForm", conn))
      {
        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
        adapt.SelectCommand.CommandType = CommandType.Text;

        DataTable dt = new DataTable();
        adapt.Fill(dt);

        if (dt.Rows.Count > 0)
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            var record = new TableName();
            record.Id = dt.Rows[i]["Id"].ToString();
            record.Name = dt.Rows[i]["TableName"].ToString();
            record.FormName = dt.Rows[i]["FormName"].ToString();
            result.Add(record);
          }

        }
        return result;
      }
    }

    [HttpPost]
    public JsonResult GetTableColumnsWithDataType(string tableName)
    {
      var result = new List<TableType>();
      using (SqlConnection conn = new SqlConnection(myConnectionString))
      using (SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' and UPPER(COLUMN_NAME) <> 'DC_ID' and UPPER(COLUMN_NAME) <> 'STATUS'", conn))
      {
        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
        adapt.SelectCommand.CommandType = CommandType.Text;

        DataTable dt = new DataTable();
        adapt.Fill(dt);

        if (dt.Rows.Count > 0)
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            var data = new TableType();
            data.ColumnName = dt.Rows[i][0].ToString();
            data.DataType = dt.Rows[i][1].ToString();
            result.Add(data);
          }

        }
        return Json(result, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public ActionResult SubmitData(List<string> inputData,List<TableType> inputDataType,string tableName)
    {
      var query = "insert into " + tableName + " select ";
      for (int i = 0; i < inputData.Count; i++)
      {
        var dataType = inputDataType[i].DataType;
        if (dataType == "nvarchar") query += "'" + inputData[i] + "' ,";
        if (dataType == "bit") query += "CAST('" + (inputData[i] == "true" ? 1 : 0) + "' as bit) ,";
        if (dataType == "datetime") query += "CAST('" + inputData[i] + "' as datetime) ,";
        if (dataType == "int") query += "CAST('" + inputData[i] + "' as int) ,";
        if (dataType == "decimal") query += "CAST('" + inputData[i] + "' as decimal(18,2)) ,";
      }
      query += "0";

      try
      {
        using (SqlConnection conn = new SqlConnection(myConnectionString))
        using (SqlCommand cmd = new SqlCommand(query, conn))
        {
          SqlDataAdapter adapt = new SqlDataAdapter(cmd);
          adapt.SelectCommand.CommandType = CommandType.Text;

          DataTable dt = new DataTable();
          adapt.Fill(dt);

          return Json(new {result = "Success"}, JsonRequestBehavior.AllowGet);
        }
      }
      catch (Exception ex)
      {
        return Json(new { result = "Failed" }, JsonRequestBehavior.AllowGet);
      }
      
      

        
      }


    }
}
