using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalMedicalRecord : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) { 
                LoadMedicalRecords();
            }
        }
        private void LoadMedicalRecords()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT 
                    mr.recordID,
                    mr.patientID,
                    mr.doctorID,
                    d.name AS department,
                    mr.recordDate
                FROM 
                    MedicalRecord mr
                LEFT JOIN 
                    Doctor doc ON mr.doctorID = doc.doctorID
                LEFT JOIN 
                    Department d ON doc.departmentID = d.departmentID
                ORDER BY 
                    mr.recordID ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Bind data to ListView
                    lvMedicalRecord.DataSource = dt;
                    lvMedicalRecord.DataBind();
                }
            }
        }
        protected void lvMedicalRecord_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectRecord")
            {
                string recordID = e.CommandArgument.ToString();
                Response.Redirect("~/Admin/hospitalRecordDetails.aspx?recordID=" + recordID);
            }
        }
        protected void lbAddMedicalRecord_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddMedicalRecord.aspx");
        }
    }
}