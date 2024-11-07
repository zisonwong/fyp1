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
                ddlDepartment.DataSource = GetDepartments();
                ddlDepartment.DataTextField = "departmentName";
                ddlDepartment.DataValueField = "departmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));

                // Bind doctors to the repeater
                rptDoctors.DataSource = GetDoctors();
                rptDoctors.DataBind();
            }
        }

        private void LoadDepartments()
        {
            // Load department data into ddlDepartment dropdown.
            ddlDepartment.DataSource = GetDepartments(); // Method to get departments
            ddlDepartment.DataTextField = "DepartmentName";
            ddlDepartment.DataValueField = "DepartmentID";
            ddlDepartment.DataBind();
            ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
        }

        private void LoadDoctors()
        {
            // Load doctor data and bind it to rptDoctors repeater.
            rptDoctors.DataSource = GetDoctors(); // Method to get doctors
            rptDoctors.DataBind();
        }

        protected void btnBook_Click(object sender, EventArgs e)
        {
            // Logic for booking an appointment with the selected doctor.
        }

        private DataTable GetDepartments()
        {
            string query = "SELECT departmentID, departmentName FROM Department";
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
            CASE WHEN d.availability = 1 THEN 'Available' ELSE 'Not Available' END AS Availability,
            dept.departmentName
        FROM Doctor d
        INNER JOIN Department dept ON d.departmentID = dept.departmentID";

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

    }
}