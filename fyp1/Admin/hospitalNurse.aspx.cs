using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeOpenXml;
using System.IO;

namespace fyp1.Admin
{
    public partial class hospitalNurse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNurse();
            }
        }
        private void LoadNurse()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT nurseID, name, role, email, photo FROM Nurse WHERE status = 'Activate'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            // Add a new column for the Base64 image string
                            dataTable.Columns.Add("nursePhoto", typeof(string));

                            // Convert binary photo to Base64 string
                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (row["photo"] != DBNull.Value)
                                {
                                    if (row["photo"] is byte[] photoData)
                                    {
                                        string base64String = Convert.ToBase64String(photoData);
                                        string mimeType = "image/png"; // Adjust accordingly if you store file type

                                        // Assign to the new column
                                        row["nursePhoto"] = $"data:{mimeType};base64,{base64String}";
                                    }
                                    else
                                    {
                                        // Handle unexpected types if necessary
                                        row["nursePhoto"] = ""; // or set a default image
                                    }
                                }
                                else
                                {
                                    // Handle the case where there is no photo
                                    row["nursePhoto"] = ""; // or set to a default image URL
                                }
                            }

                            // Bind to the ListView using the new column for the image
                            lvStaff.DataSource = dataTable;
                            lvStaff.DataBind();
                        }
                        else
                        {
                            lvStaff.DataSource = null;
                            lvStaff.DataBind();
                        }
                    }
                }
            }
        }

        protected void lbAddNurse_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddNurse.aspx");
        }

        protected void lbNurseDetails_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalNurseDetails.aspx?nurseID={nurseID}");
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string query = "UPDATE Nurse SET status = 'UnActivate' WHERE nurseID = @NurseID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NurseID", nurseID);
                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "DeleteSuccess",
                                "alert('Nurse deleted successfully.');", true);

                            LoadNurse();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "DeleteError",
                                "alert('Error deleting nurse.');", true);
                        }
                    }
                }
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalNurseEdit.aspx?nurseID={nurseID}");
            }
        }

        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataTable nurseTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                n.nurseID, 
                n.ICNumber, 
                n.name, 
                CAST(n.DOB AS DATE) AS DOB, 
                n.gender, 
                n.role, 
                n.email, 
                n.contactInfo, 
                n.status, 
                n.date,
                b.name AS BranchName
            FROM 
                Nurse n
            LEFT JOIN 
                Branch b ON n.branchID = b.branchID;
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(nurseTable);
                    }
                }
            }

            if (nurseTable.Rows.Count > 0)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Nurses");

                    worksheet.Cells["A1"].LoadFromDataTable(nurseTable, true);

                    using (var range = worksheet.Cells["A1:J1"])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    var dobColumn = worksheet.Column(4);
                    dobColumn.Style.Numberformat.Format = "yyyy-MM-dd";

                    var dateColumn = worksheet.Column(10);
                    dateColumn.Style.Numberformat.Format = "yyyy-MM-dd";

                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        package.SaveAs(memoryStream);
                        byte[] byteArray = memoryStream.ToArray();

                        Response.Clear();
                        Response.Buffer = true;
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=NurseData.xlsx");
                        Response.BinaryWrite(byteArray);
                        Response.End();
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('No doctor data found to export.');", true);
            }
        }
    }
}