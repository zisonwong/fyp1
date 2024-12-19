using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace fyp1.Admin
{
    public partial class hospitalMedicalRecord : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadMedicalRecords();
                }
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
            doc.name AS doctorName, 
            mr.recordDate
        FROM 
            MedicalRecord mr
        LEFT JOIN 
            Doctor doc ON mr.doctorID = doc.doctorID
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
            //else if(e.CommandName == "Edit")
            //{
            //    string recordID = e.CommandArgument.ToString();
            //    Response.Redirect($"~/Admin/hospitalEditMedicalRecord.aspx?recordID="+ recordID);
            //}
        }
        protected void lbAddMedicalRecord_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddMedicalRecord.aspx");
        }

        private void LoadFilteredData(string searchTerm)
        {
            string query = @"
        SELECT 
            mr.recordID, 
            mr.patientID, 
            mr.doctorID, 
            d.name AS doctorName,
            mr.recordDate
        FROM MedicalRecord mr
        INNER JOIN Doctor d ON mr.doctorID = d.doctorID
        WHERE 1 = 1";

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += @"
            AND (mr.recordID LIKE @searchTerm 
            OR mr.patientID LIKE @searchTerm 
            OR mr.doctorID LIKE @searchTerm 
            OR d.name LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }
            query += " ORDER BY mr.recordID";

            DataTable medicalRecordTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(medicalRecordTable);
            }
            lvMedicalRecord.DataSource = medicalRecordTable;
            lvMedicalRecord.DataBind();
        }

        protected void lvMedicalRecord_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dpMedicalRecord.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadMedicalRecords();
            }
        }

    }
}