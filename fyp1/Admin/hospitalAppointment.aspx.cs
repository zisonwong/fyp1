using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static fyp1.Admin.hospitalMedicine;

namespace fyp1.Admin
{
    public partial class hospitalAppointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ViewState["SortColumn"] = "date";
                ViewState["SortOrder"] = "ASC";
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadAppointments();
                }
            }
        }
        private void LoadAppointments(string sortExpression = null, string sortDirection = "ASC")
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT a.appointmentID, 
                   a.patientID, 
                   a.doctorID, 
                   ab.availableFrom AS time, 
                   CONVERT(DATE, ab.availableDate) AS date,
                   a.status
            FROM Appointment a
            INNER JOIN Availability ab ON a.availabilityID = ab.availabilityID";

                // Apply sorting if a sort expression is provided
                if (!string.IsNullOrEmpty(sortExpression))
                {
                    query += $" ORDER BY {sortExpression} {sortDirection}";
                }
                else
                {
                    query += " ORDER BY ab.availableDate, ab.availableFrom";
                }

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt);
                conn.Close();

                lvAppointment.DataSource = dt;
                lvAppointment.DataBind();
            }
        }

        private void LoadFilteredData(string searchTerm, string sortExpression = null, string sortDirection = "ASC")
        {
            string query = @"
        SELECT a.appointmentID, 
               a.patientID, 
               a.doctorID, 
               ab.availableFrom AS time, 
               CONVERT(DATE, ab.availableDate) AS date,
               a.status
        FROM Appointment a
        INNER JOIN Availability ab ON a.availabilityID = ab.availabilityID
        WHERE 1=1";

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (a.appointmentID LIKE @searchTerm OR a.patientID LIKE @searchTerm OR a.doctorID LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }

            // Apply sorting if a sort expression is provided
            if (!string.IsNullOrEmpty(sortExpression))
            {
                query += $" ORDER BY {sortExpression} {sortDirection}";
            }
            else
            {
                query += " ORDER BY ab.availableDate, ab.availableFrom";
            }

            DataTable branchTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(branchTable);
            }

            lvAppointment.DataSource = branchTable;
            lvAppointment.DataBind();
        }

        protected void lvAppointment_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAppointment")
            {
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect("~/Admin/hospitalAppointmentDetails.aspx?appointmentID=" + appointmentID);
            }
        }
        protected void lvAppointment_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dpAppointment.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadAppointments();
            }
        }
        protected void lvAppointment_Sorting(object sender, ListViewSortEventArgs e)
        {
            string currentSortExpression = ViewState["SortExpression"] as string;
            string currentSortDirection = ViewState["SortDirection"] as string;

            string newSortDirection = "ASC";
            if (currentSortExpression == e.SortExpression && currentSortDirection == "ASC")
            {
                newSortDirection = "DESC";
            }

            ViewState["SortExpression"] = e.SortExpression;
            ViewState["SortDirection"] = newSortDirection;

            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm, e.SortExpression, newSortDirection);
            }
            else
            {
                LoadAppointments(e.SortExpression, newSortDirection);
            }
        }

    }
}