using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class EmergencyPage : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        static private string GenerateNextLocationID()
        {
            string nextLocationID = "L0001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(locationID) FROM Location", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextLocationID = "L" + idNumber.ToString("D4");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextLocationID;
        }

        static private string GenerateNextAlertID()
        {
            string nextAlertID = "AL0001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(AlertID) FROM EmergencyAlert", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(2)) + 1;
                            nextAlertID = "AL" + idNumber.ToString("D4");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextAlertID;
        }

        [WebMethod]
        public static object EmergencyAlert(double latitude, double longitude)
        {
            try
            {
                HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
                string patientID = IDCookie.Value;
                string locationID = GenerateNextLocationID();
                string alertID = GenerateNextAlertID();

                // Database connection
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string insertLocationQuery = @"INSERT INTO Location (locationID, latitude, longitude) VALUES (@locationID, @latitude, @longitude)";
                    using (SqlCommand cmd = new SqlCommand(insertLocationQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@locationID", locationID);
                        cmd.Parameters.AddWithValue("@latitude", latitude);
                        cmd.Parameters.AddWithValue("@longitude", longitude);
                        cmd.ExecuteNonQuery();
                    }

                    // Insert alert into the EmergencyAlert table
                    string insertAlertQuery = @"
                    INSERT INTO EmergencyAlert (alertID, patientID, locationID, timestamp, status)
                    VALUES (@alertID, @patientID, @locationID, @timestamp, @status)";
                    using (SqlCommand cmd = new SqlCommand(insertAlertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@alertID", alertID);
                        cmd.Parameters.AddWithValue("@patientID", patientID);
                        cmd.Parameters.AddWithValue("@locationID", locationID);
                        cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@status", "Pending");
                        cmd.ExecuteNonQuery();
                    }
                }

                // Return success response
                return new { success = true };
            }
            catch (Exception ex)
            {
                // Return error response
                return new { success = false, error = ex.Message };
            }
        }
        
    }
}