    using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;
using System.IO;
using ClosedXML.Excel;
using OfficeOpenXml;

namespace fyp1.Admin
{
    public partial class hospitalDoctor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDoctors();
            }
        }
        private void LoadDoctors()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT doctorID, name, role, email, photo FROM Doctor WHERE status = 'Activate'";

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

                            dataTable.Columns.Add("doctorPhoto", typeof(string));

                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (row["photo"] != DBNull.Value)
                                {
                                    if (row["photo"] is byte[] photoData)
                                    {
                                        string base64String = Convert.ToBase64String(photoData);
                                        string mimeType = "image/png"; 

                                        row["doctorPhoto"] = $"data:{mimeType};base64,{base64String}";
                                    }
                                    else
                                    {
                                        row["doctorPhoto"] = ""; 
                                    }
                                }
                                else
                                {
                                    row["doctorPhoto"] = ""; 
                                }
                            }

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

        protected void lbAddDoctor_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddDoctor.aspx");
        }

        protected void lbDoctorDetails_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalDoctorDetails.aspx?doctorID={doctorID}");
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string query = "UPDATE Doctor SET status = 'UnActivate' WHERE doctorID = @DoctorID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", doctorID);
                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "StatusUpdated",
                                "alert('Doctor status updated to UnActivate successfully.');", true);

                            LoadDoctors();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "UpdateError",
                                "alert('Error updating doctor status.');", true);
                        }
                    }
                }
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalDoctorEdit.aspx?doctorID={doctorID}");
            }
        }
        protected void btnExportToExcel_Click(object sender, EventArgs e)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            DataTable doctorTable = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT 
                d.doctorID, 
                d.ICNumber, 
                d.name, 
                CAST(d.DOB AS DATE) AS DOB, 
                d.gender, 
                d.role, 
                d.email, 
                d.contactInfo, 
                d.status, 
                d.date,
                dep.name AS DepartmentName,
                br.name AS BranchName
            FROM 
                Doctor d
            JOIN 
                DoctorDepartment dd ON d.doctorID = dd.doctorID
            JOIN 
                Department dep ON dd.departmentID = dep.departmentID
            LEFT JOIN 
                Branch br ON dep.branchID = br.branchID;
        ";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(doctorTable);
                    }
                }
            }

            if (doctorTable.Rows.Count > 0)
            {
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Doctors");

                    worksheet.Cells["A1"].LoadFromDataTable(doctorTable, true);

                    using (var range = worksheet.Cells["A1:L1"])  
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
                        Response.AddHeader("content-disposition", "attachment;filename=DoctorData.xlsx");
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