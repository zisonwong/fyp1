using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class DoctorAvailability : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDepartments();
                LoadDoctors();
            }
        }

        private void LoadDepartments()
        {
            ddlDepartment.DataSource = GetDepartments();
            ddlDepartment.DataTextField = "name";
            ddlDepartment.DataValueField = "departmentID";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
        }

        private void LoadDoctors()
        {
            ddlDoctor.DataSource = GetDoctors();
            ddlDoctor.DataTextField = "name";
            ddlDoctor.DataValueField = "doctorID";
            ddlDoctor.DataBind();
            ddlDoctor.Items.Insert(0, new ListItem("Select Doctor", ""));
        }


        protected void btnBook_Click(object sender, EventArgs e)
        {
            // Logic for booking an appointment with the selected doctor.
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDeptId = ddlDepartment.SelectedValue;
            ddlDoctor.DataSource = GetDoctorsByDepartment(selectedDeptId);
            ddlDoctor.DataTextField = "name";
            ddlDoctor.DataValueField = "doctorID";
            ddlDoctor.DataBind();
            ddlDoctor.Items.Insert(0, new ListItem("Select Doctor", ""));
        }



        protected void ddlDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            string doctorId = ddlDoctor.SelectedValue;
            if (!string.IsNullOrEmpty(doctorId))
            {
                LoadAvailability(doctorId);
            }
        }

        private void LoadAvailability(string doctorId)
        {
            rptAvailability.DataSource = GetDoctorAvailability(doctorId);
            rptAvailability.DataBind();
        }


        private DataTable GetDoctorsByDepartment(string departmentId)
        {
            string query = @"SELECT doctorID, name FROM Doctor WHERE departmentID = @departmentId";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@departmentId", departmentId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt;
            }
        }

        private DataTable GetDoctorAvailability(string doctorId)
        {
            string query = @"
        SELECT 
            availableDate, 
            availableFrom, 
            availableTo 
        FROM Availability 
        WHERE doctorID = @doctorId";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorId", doctorId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt;
            }
        }

        private DataTable GetDepartments()
        {
            string query = "SELECT departmentID, name FROM Department";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt;
            }
        }


        private DataTable GetDoctors()
        {
            string query = @"
        SELECT 
            d.doctorID, 
            d.name, 
            d.role, 
            d.contactInfo, 
            d.photo,
            CASE 
                WHEN a.availabilityID IS NOT NULL THEN 'Available' 
                ELSE 'Not Available' 
            END AS Availability,
            dept.name AS DepartmentName
        FROM Doctor d
        INNER JOIN Department dept ON d.departmentID = dept.departmentID
        LEFT JOIN Availability a ON d.doctorID = a.doctorID 
            AND a.availableDate = CAST(GETDATE() AS DATE)";

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                // Add a new column for ImageUrl
                dt.Columns.Add("ImageUrl", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["photo"] != DBNull.Value)
                    {
                        byte[] photoBytes = (byte[])row["photo"];
                        string base64String = Convert.ToBase64String(photoBytes);
                        row["ImageUrl"] = "data:image/jpeg;base64," + base64String;
                    }
                    else
                    {
                        // Set a default image if no photo is available
                        row["ImageUrl"] = "~/Images/default-doctor.png";
                    }
                }

                return dt;
            }
        }

    }
}