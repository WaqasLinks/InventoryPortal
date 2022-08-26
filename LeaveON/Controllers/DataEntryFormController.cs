using LeaveON.Models;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeaveON.Controllers
{
  [Authorize(Roles = "Admin,Manager,User")]
  public class DataEntryFormController : Controller
  {

    public string myConnectionString = "Data Source=.;Initial Catalog=DMS;User Id=sa;Password=abc;";
    // GET: DataEntryForm
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
      using (SqlCommand cmd = new SqlCommand("select Id,TableName,FormName from  DynamicForm", conn))
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
    public JsonResult GetTableColumns(string tableName)
    {
      var result = new List<string>();
      using (SqlConnection conn = new SqlConnection(myConnectionString))
      using (SqlCommand cmd = new SqlCommand("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + tableName + "' and UPPER(COLUMN_NAME) <> 'DC_ID' and UPPER(COLUMN_NAME) <> 'STATUS'", conn))
      {
        SqlDataAdapter adapt = new SqlDataAdapter(cmd);
        adapt.SelectCommand.CommandType = CommandType.Text;

        DataTable dt = new DataTable();
        adapt.Fill(dt);

        if (dt.Rows.Count > 0)
        {
          for (int i = 0; i < dt.Rows.Count; i++)
          {
            result.Add(dt.Rows[i][0].ToString());
          }

        }
        return Json(result, JsonRequestBehavior.AllowGet);
      }
    }

    [HttpPost]
    public ActionResult UploadExcelFile()
    {
      if (Request.Files.Count > 0)
      {
        List<string> allColumns = new List<string>();
        List<string> allRows = new List<string>();
        List<string> allErrors = new List<string>();
        var tableName = Convert.ToString(Request.Form["tableName"]);


        try
        {
          HttpFileCollectionBase postedFiles = Request.Files;
          HttpPostedFileBase postedFile = postedFiles[0];
          string filePath = string.Empty;
          var noOfCol = 0;
          var noOfRow = 0;
          var tableType = new List<TableType>();

          if (postedFile != null)
          {
            string path = Server.MapPath("~/Uploads/");
            if (!Directory.Exists(path))
            {
              Directory.CreateDirectory(path);
            }

            filePath = path + Path.GetFileName(postedFile.FileName);
            string extension = Path.GetExtension(postedFile.FileName);
            postedFile.SaveAs(filePath);



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
                  var tableTpe = new TableType();
                  tableTpe.ColumnName = dt.Rows[i][0].ToString();
                  tableTpe.DataType = dt.Rows[i][1].ToString();
                  tableType.Add(tableTpe);
                }

              }
            }


            string fileName = postedFile.FileName;
            string fileContentType = postedFile.ContentType;
            byte[] fileBytes = new byte[postedFile.ContentLength];
            var data = postedFile.InputStream.Read(fileBytes, 0, Convert.ToInt32(postedFile.ContentLength));

            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(postedFile.InputStream))
            {
              var currentSheet = package.Workbook.Worksheets;
              var workSheet = currentSheet.First();
              noOfCol = workSheet.Dimension.End.Column;
              noOfRow = workSheet.Dimension.End.Row;

              for (int rowIterator = 1; rowIterator <= noOfRow; rowIterator++)
              {
                for (int colIterator = 1; colIterator <= noOfCol; colIterator++)
                {
                  if (rowIterator == 1)
                    allColumns.Add(Convert.ToString(workSheet.Cells[1, colIterator].Value));
                  else
                  {
                    allRows.Add(Convert.ToString(workSheet.Cells[rowIterator, colIterator].Value));
                    var dataType = tableType.FirstOrDefault(x => x.ColumnName.ToUpper() == allColumns[colIterator - 1].ToUpper()).DataType;

                    if (dataType == "nvarchar")
                    {
                      try
                      {
                        string test = Convert.ToString(workSheet.Cells[rowIterator, colIterator].Value);
                      }
                      catch (Exception ex)
                      {
                        allErrors.Add("Value must be of type string at Col: " + colIterator + " Row: " + rowIterator);
                      }
                    }
                    else if (dataType == "int")
                    {
                      try
                      {
                        int test = Convert.ToInt32(workSheet.Cells[rowIterator, colIterator].Value);
                      }
                      catch (Exception ex)
                      {
                        allErrors.Add("Value must be of type int at Col: " + colIterator + " Row: " + rowIterator);
                      }
                    }
                    else if (dataType == "datetime")
                    {
                      try
                      {
                        DateTime test = Convert.ToDateTime(workSheet.Cells[rowIterator, colIterator].Value);
                      }
                      catch (Exception ex)
                      {
                        allErrors.Add("Value must be of type datetime at Col: " + colIterator + " Row: " + rowIterator);
                      }
                    }
                    else if (dataType == "bit")
                    {
                      try
                      {
                        int test = Convert.ToInt32(workSheet.Cells[rowIterator, colIterator].Value);
                        if (test != 0 && test != 1)
                          allErrors.Add("Value must be of type boolean at Col: " + colIterator + " Row: " + rowIterator);
                      }
                      catch (Exception ex)
                      {
                        allErrors.Add("Value must be of type boolean at Col: " + colIterator + " Row: " + rowIterator);
                      }
                    }
                    else if (dataType == "decimal")
                    {
                      try
                      {
                        decimal test = Convert.ToDecimal(workSheet.Cells[rowIterator, colIterator].Value);
                      }
                      catch (Exception ex)
                      {
                        allErrors.Add("Value must be of type decimal at Col: " + colIterator + " Row: " + rowIterator);
                      }
                    }
                  }
                }


              }
            }

          }



          if (allErrors.Count == 0)
          {

            int colNo = 0;
            for (int u = 0; u < (noOfRow - 1); u++)
            {
              string query = "insert into " + tableName + " select ";
              for (int o = 0; o < noOfCol; o++)
              {
                var dataType = tableType.FirstOrDefault(x => x.ColumnName.ToUpper() == allColumns[o].ToUpper()).DataType;
                if (dataType == "nvarchar") query += "'" + allRows[colNo] + "' ,";
                if (dataType == "bit") query += "CAST('" + allRows[colNo] + "' as bit) ,";
                if (dataType == "datetime") query += "CAST('" + allRows[colNo] + "' as datetime) ,";
                if (dataType == "int") query += "CAST('" + allRows[colNo] + "' as int) ,";
                if (dataType == "decimal") query += "CAST('" + allRows[colNo] + "' as decimal(18,2)) ,";
                ++colNo;
              }
              query += "0";
              //Inserting into DB

              try
              {
                using (SqlConnection con = new SqlConnection(myConnectionString))

                using (SqlCommand cmnd = new SqlCommand(query, con))
                {
                  SqlDataAdapter adapter = new SqlDataAdapter(cmnd);
                  adapter.SelectCommand.CommandType = CommandType.Text;

                  DataTable dtble = new DataTable();
                  adapter.Fill(dtble);
                }

              }
              catch (Exception ex)
              {

              }
            }
          }


          return Json(new { allColumns = allColumns, allRows = allRows, allErrors = allErrors, noOfRows = noOfRow }, JsonRequestBehavior.AllowGet);

        }
        catch (Exception ex)
        {
          return Json("Error occurred. Error details: " + ex.Message);
        }
      }
      else
      {
        return Json("No file selected.");
      }
    }


    public class TableType
    {
      public string ColumnName { get; set; }
      public string DataType { get; set; }
    }

  }
}
