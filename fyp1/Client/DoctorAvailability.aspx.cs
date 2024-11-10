using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace fyp1.Client
{
    public partial class DoctorAvailability : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string doctorID = Request.QueryString["doctorID"];

                if (!string.IsNullOrEmpty(doctorID))
                {
                    LoadDoctorDetails(doctorID);
                    LoadAvailableDates(doctorID);
                    LoadDoctorAvailability(doctorID, null); // Initially load all dates
                }
                else
                {

                }
            }
        }

        // Load available dates into the dropdown list
        protected void LoadAvailableDates(string doctorID)
        {
            string query = @"
        SELECT DISTINCT availableDate 
        FROM Availability 
        WHERE doctorID = @doctorID AND status = 'Available'
        ORDER BY availableDate";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ddlAvailableDates.Items.Clear();
                ddlAvailableDates.Items.Add(new ListItem("Select Date", ""));

                while (reader.Read())
                {
                    DateTime availableDate = (DateTime)reader["availableDate"];
                    ddlAvailableDates.Items.Add(new ListItem(
                        availableDate.ToString("dddd, MMMM dd, yyyy"),
                        availableDate.ToString("yyyy-MM-dd")));
                }

                reader.Close();
            }
        }

        // Load doctor's available slots based on selected date
        protected void LoadDoctorAvailability(string doctorID, DateTime? selectedDate)
        {
            string query = @"
                        SELECT availabilityID, availableDate, availableFrom, availableTo 
                        FROM Availability 
                        WHERE doctorID = @doctorID AND status = 'Available'";

            if (selectedDate.HasValue)
            {
                query += " AND availableDate = @selectedDate";
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                if (selectedDate.HasValue)
                {
                    cmd.Parameters.AddWithValue("@selectedDate", selectedDate.Value);
                }

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataRow row in dt.Rows)
                {
                    // Format the time slots
                    row["availableFrom"] = row["availableFrom"] != DBNull.Value
                        ? ((TimeSpan)row["availableFrom"]).ToString(@"hh\:mm")
                        : "No Time Provided";

                    row["availableTo"] = row["availableTo"] != DBNull.Value
                        ? ((TimeSpan)row["availableTo"]).ToString(@"hh\:mm")
                        : "No Time Provided";
                }

                // Bind the data to the repeater
                rptAvailability.DataSource = dt;
                rptAvailability.DataBind();

                reader.Close();
            }
        }

        // Event handler for date selection change
        protected void ddlAvailableDates_SelectedIndexChanged(object sender, EventArgs e)
        {
            string doctorID = Request.QueryString["doctorID"];
            DateTime? selectedDate = null;

            if (!string.IsNullOrEmpty(ddlAvailableDates.SelectedValue))
            {
                selectedDate = DateTime.Parse(ddlAvailableDates.SelectedValue);
            }

            LoadDoctorAvailability(doctorID, selectedDate);
        }


        protected void LoadDoctorDetails(string doctorID)
        {
            string query = @"SELECT d.Name, d.ContactInfo, d.photo, dept.name AS DepartmentName
                            FROM Doctor d
                            INNER JOIN Department dept ON d.DepartmentID = dept.DepartmentID
                            WHERE d.doctorID = @doctorID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Set doctor's photo
                    Image imgDoctorPhoto = (Image)pnlDoctorDetails.FindControl("imgDoctorPhoto");
                    if (imgDoctorPhoto != null && reader["photo"] != DBNull.Value)
                    {
                        // Convert photo byte array to Base64 and set ImageUrl
                        byte[] photoBytes = (byte[])reader["photo"];
                        string base64String = Convert.ToBase64String(photoBytes);
                        imgDoctorPhoto.ImageUrl = "data:image/png;base64," + base64String;
                    }

                    Literal litDoctorName = (Literal)pnlDoctorDetails.FindControl("litDoctorName");
                    if (litDoctorName != null)
                    {
                        litDoctorName.Text = reader["Name"].ToString();
                    }

                    Literal litDoctorContact = (Literal)pnlDoctorDetails.FindControl("litDoctorContact");
                    if (litDoctorContact != null)
                    {
                        litDoctorContact.Text = reader["ContactInfo"].ToString();
                    }

                    Literal litDoctorDepartment = (Literal)pnlDoctorDetails.FindControl("litDoctorDepartment");
                    if (litDoctorDepartment != null)
                    {
                        litDoctorDepartment.Text = reader["DepartmentName"].ToString();
                    }
                }
                reader.Close();
            }
        }


        protected void btnSelectTime_Click(object sender, EventArgs e)
        {
            // Retrieve doctorID from query string
            string doctorID = Request.QueryString["doctorID"];
            string availabilityID = Request.QueryString["availabilityID"];

            if (ddlAvailableDates.SelectedValue == "")
            {
                lblMessage.Text = "Please select a date.";
                return;
            }

            DateTime selectedDate = DateTime.Parse(ddlAvailableDates.SelectedValue);

            // Define variables for date, fromTime, and toTime
            string fromTime = "";
            string toTime = "";


            // Database query to retrieve availability information
            string query = @"SELECT availableFrom, availableTo 
                     FROM Availability 
                     WHERE availabilityID = @availabilityID AND doctorID = @doctorID";

            // Connect to the database
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Set the parameters for doctorID and selectedDate
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);
                    cmd.Parameters.AddWithValue("@selectedDate", selectedDate);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            fromTime = reader["availableFrom"] != DBNull.Value ?
                                TimeSpan.Parse(reader["availableFrom"].ToString()).ToString(@"hh\:mm") : "N/A";

                            toTime = reader["availableTo"] != DBNull.Value ?
                                TimeSpan.Parse(reader["availableTo"].ToString()).ToString(@"hh\:mm") : "N/A";

                            availabilityID = reader["availabilityID"].ToString();
                        }
                    }
                }
            }

            // Redirect to appointment confirmation page with encoded query parameters
            Response.Redirect("clientAppointment.aspx?doctorID=" + doctorID
                              + "&availabilityID=" + availabilityID
                              + "&date=" + selectedDate.ToString("yyyy-MM-dd")
                              + "&fromTime=" + fromTime
                              + "&toTime=" + toTime);
        }


        protected void rptAvailability_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectTime")
            {
                string availabilityID = e.CommandArgument.ToString();

                // Store the selected availabilityID for use in redirection
                string doctorID = Request.QueryString["doctorID"];
                Response.Redirect("clientAppointment.aspx?doctorID=" + doctorID
                    + "&availabilityID=" + availabilityID);
            }
        }

    }
}