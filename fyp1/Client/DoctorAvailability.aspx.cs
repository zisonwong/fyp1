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
                    LoadDoctorAvailability(doctorID);
                }
                else
                {
                    // Handle if doctorID is not provided, e.g., show an error message or redirect back
                }
            }
        }


        protected void LoadDoctorDetails(string doctorID)
        {
            string query = "SELECT Name, Role, ContactInfo, photo FROM Doctor WHERE doctorID = @doctorID";
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

                    // Set doctor's name
                    Literal litDoctorName = (Literal)pnlDoctorDetails.FindControl("litDoctorName");
                    if (litDoctorName != null)
                    {
                        litDoctorName.Text = reader["Name"].ToString();
                    }

                    // Set doctor's role
                    Literal litDoctorRole = (Literal)pnlDoctorDetails.FindControl("litDoctorRole");
                    if (litDoctorRole != null)
                    {
                        litDoctorRole.Text = reader["Role"].ToString();
                    }

                    // Set doctor's contact info
                    Literal litDoctorContact = (Literal)pnlDoctorDetails.FindControl("litDoctorContact");
                    if (litDoctorContact != null)
                    {
                        litDoctorContact.Text = reader["ContactInfo"].ToString();
                    }
                }

                reader.Close();
            }
        }

        private DataTable GetAvailability(string doctorID)
        {
            string query = @"
        SELECT availableDate, availableFrom, availableTo 
        FROM Availability 
        WHERE doctorID = @doctorID 
        ORDER BY availableDate, availableFrom";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                return dt;
            }
        }
        protected void btnSelectTime_Click(object sender, EventArgs e)
        {
            string selectedDateTime = ((Button)sender).CommandArgument;
            string doctorID = Request.QueryString["doctorID"];

            // Redirect to appointment confirmation page with details
            Response.Redirect("clientAppointment.aspx?doctorID=" + doctorID + "&dateTime=" + Server.UrlEncode(selectedDateTime));
        }
        protected void LoadDoctorAvailability(string doctorID)
        {
            string query = "SELECT availableDate, availableFrom, availableTo FROM Availability WHERE doctorID = @doctorID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataRow row in dt.Rows)
                {
                    // Check if "availableFrom" is not null
                    if (row["availableFrom"] != DBNull.Value)
                    {
                        TimeSpan availableFrom = (TimeSpan)row["availableFrom"];
                        row["availableFrom"] = availableFrom.ToString(@"hh\:mm");
                    }
                    else
                    {
                        row["availableFrom"] = "No Time Provided";
                    }

                    // Check if "availableTo" is not null
                    if (row["availableTo"] != DBNull.Value)
                    {
                        TimeSpan availableTo = (TimeSpan)row["availableTo"];
                        row["availableTo"] = availableTo.ToString(@"hh\:mm");
                    }
                    else
                    {
                        row["availableTo"] = "No Time Provided";
                    }
                }

                rptAvailability.DataSource = dt;
                rptAvailability.DataBind();

                reader.Close();
            }
        }

    }
}