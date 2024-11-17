using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace fyp1.Admin
{
    public partial class hospitalPatient : System.Web.UI.Page
    {
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
                    LoadPatient();
                }
            }
        }

        private void LoadPatient()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT patientID, name, contactInfo, email, bloodtype from Patient";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();
                    da.Fill(dt);
                    lvPatient.DataSource = dt;
                    lvPatient.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "LoadError", $"alert('Error loading patients: {ex.Message}');", true);
                }
            }
        }

        protected void lvPatient_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectPatient")
            {
                string patientID = e.CommandArgument.ToString();
                Response.Redirect("~/Admin/hospitalPatientDetails.aspx?patientID=" + patientID);
            }
        }

        private void LoadFilteredData(string searchTerm)
        {
            string query = "SELECT patientID, name, contactInfo, email, bloodtype from Patient WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (patientID LIKE @searchTerm OR name LIKE @searchTerm OR email LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable branchTable = new DataTable();
                    conn.Open();
                    adapter.Fill(branchTable);
                    lvPatient.DataSource = branchTable;
                    lvPatient.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "FilterError", $"alert('Error filtering data: {ex.Message}');", true);
                }
            }
        }

        protected void lvPatient_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvPatient.EditIndex = e.NewEditIndex;
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadPatient();
            }
        }

        protected void lvPatient_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            lvPatient.EditIndex = -1;
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadPatient();
            }
        }

        protected void lvPatient_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            ListViewItem item = lvPatient.Items[e.ItemIndex];
            TextBox txtBloodType = (TextBox)item.FindControl("txtPatientBloodType");
            Label lblPatientID = (Label)item.FindControl("lblPatientID");

            if (txtBloodType == null || lblPatientID == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "ControlError", "alert('Error: Could not find required controls.');", true);
                return;
            }

            string bloodType = txtBloodType.Text.Trim();
            string patientID = lblPatientID.Text.Trim();

            if (string.IsNullOrEmpty(bloodType))
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "ValidationError", "alert('Please fill in all fields');", true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE Patient SET bloodtype = @bloodtype WHERE patientID = @patientID";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@bloodtype", bloodType);
                    cmd.Parameters.AddWithValue("@patientID", patientID);
                    cmd.ExecuteNonQuery();

                    lvPatient.EditIndex = -1;
                    string searchTerm = ViewState["SearchTerm"] as string;
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        LoadFilteredData(searchTerm);
                    }
                    else
                    {
                        LoadPatient();
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "UpdateSuccess", "alert('Record updated successfully');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "UpdateError", $"alert('Error updating record: {ex.Message}');", true);
                }
            }
        }
    }
}