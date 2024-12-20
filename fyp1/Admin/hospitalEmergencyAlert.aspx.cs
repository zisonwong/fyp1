using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace fyp1.Admin
{
    public partial class hospitalEmergencyAlert : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateFilterStatus();
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    BindEmergencyAlerts();
                }
            }
        }
        private void BindEmergencyAlerts()
        {
            string query = @"
        SELECT EA.alertID, EA.timestamp AS time, EA.status, P.patientID, 
               P.name AS patientName, L.address
        FROM EmergencyAlert EA
        INNER JOIN Patient P ON EA.patientID = P.patientID
        INNER JOIN Location L ON EA.locationID = L.locationID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    lvEmergency.DataSource = reader;
                    lvEmergency.DataBind();
                }
            }
        }
        private void LoadFilteredData(string searchTerm)
        {
            string query = @"
        SELECT EA.alertID, ea.timestamp AS time, ea.status, p.patientID, 
               p.name AS patientName, l.address
        FROM EmergencyAlert ea
        INNER JOIN Patient p ON ea.patientID = p.patientID
        INNER JOIN Location l ON ea.locationID = l.locationID
        WHERE 1=1";

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += @"
            AND (
                ea.alertID LIKE @searchTerm OR 
                ea.patientID LIKE @searchTerm OR 
                p.name LIKE @searchTerm OR 
                l.address LIKE @searchTerm
            )";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable alertTable = new DataTable();
                    conn.Open();
                    adapter.Fill(alertTable);
                    lvEmergency.DataSource = alertTable; 
                    lvEmergency.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "FilterError", $"alert('Error filtering data: {ex.Message}');", true);
                }
            }
        }
        protected void lvEmergency_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvEmergency.EditIndex = e.NewEditIndex;
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                BindEmergencyAlerts();
            }
        }
        protected void lvEmergency_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            lvEmergency.EditIndex = -1;
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                BindEmergencyAlerts();
            }
        }

        protected void lvEmergency_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            ListViewItem item = lvEmergency.Items[e.ItemIndex];
            TextBox txtStatus = (TextBox)item.FindControl("txtStatus");
            Label lblAlertId = (Label)item.FindControl("lblAlertId");

            if (txtStatus == null || lblAlertId == null)
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "ControlError", "alert('Error: Could not find required controls.');", true);
                return;
            }

            string status = txtStatus.Text.Trim();
            string alertId = lblAlertId.Text.Trim();

            if (string.IsNullOrEmpty(status))
            {
                ScriptManager.RegisterStartupScript(this, GetType(),
                    "ValidationError", "alert('Please fill in the status field.');", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string updateQuery = "UPDATE EmergencyAlert SET status = @status WHERE alertID = @alertID";
                    SqlCommand cmd = new SqlCommand(updateQuery, conn);
                    cmd.Parameters.AddWithValue("@status", status);
                    cmd.Parameters.AddWithValue("@alertID", alertId);
                    cmd.ExecuteNonQuery();

                    lvEmergency.EditIndex = -1;
                    string searchTerm = ViewState["SearchTerm"] as string;
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        LoadFilteredData(searchTerm);
                    }
                    else
                    {
                        BindEmergencyAlerts();
                    }

                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "UpdateSuccess", "alert('Status updated successfully');", true);
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "UpdateError", $"alert('Error updating status: {ex.Message}');", true);
                }
            }
        }
        private void PopulateFilterStatus()
        {
            ddlFilterStatus.Items.Clear();
            ddlFilterStatus.Items.Add(new ListItem("Select Status", "")); 
            ddlFilterStatus.Items.Add(new ListItem("Pending", "Pending")); 
            ddlFilterStatus.Items.Add(new ListItem("Completed", "Completed")); 
        }
        protected void ddlFilterStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedStatus = ddlFilterStatus.SelectedValue;
            ViewState["SelectedStatus"] = selectedStatus;

            if (string.IsNullOrEmpty(selectedStatus)) 
            {
                BindEmergencyAlerts(); 
            }
            else
            {
                FilterEmergencyAlertsByStatus(selectedStatus);
            }
        }


        private void FilterEmergencyAlertsByStatus(string status)
        {
            string query = @"
        SELECT EA.alertID, EA.timestamp AS time, EA.status, P.patientID, 
               P.name AS patientName, L.address
        FROM EmergencyAlert EA
        INNER JOIN Patient P ON EA.patientID = P.patientID
        INNER JOIN Location L ON EA.locationID = L.locationID
        WHERE EA.status = @status";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@status", status);

                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    DataTable alertTable = new DataTable();
                    conn.Open();
                    adapter.Fill(alertTable);

                    lvEmergency.DataSource = alertTable;
                    lvEmergency.DataBind();
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(),
                        "FilterError", $"alert('Error filtering data: {ex.Message}');", true);
                }
            }
        }
    }
}