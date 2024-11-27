using fyp1.Admin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
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
            if (!IsPostBack)
            {
                string userAddress = GetUserAddressFromEmergencyAlert(); // Get the user's address

                if (!string.IsNullOrEmpty(userAddress))
                {
                    InitializeEmergencyDetails(userAddress); // Use the address for processing
                }
                else
                {
                    lblMessage.Text = "Could not retrieve the user's address from the emergency alert.";
                }
            }
        }


        private void InitializeEmergencyDetails(string userAddress)
        {
            string branchName = string.Empty;
            string branchAddress = string.Empty;

            GetNearestBranch(userAddress, ref branchName, ref branchAddress);

            if (!string.IsNullOrEmpty(branchName))
            {
                lblNearestBranch.Text = $"Nearest Branch: {branchName}";
                lblBranchAddress.Text = $"Address: {branchAddress}";

                var travelDetails = GetTravelDetails(userAddress, branchAddress);
                lblAmbulanceTime.Text = $"Estimated Ambulance Arrival Time: {travelDetails.Duration}";
                lblMessage.Text = "Emergency team has been dispatched.";
            }
            else
            {
                lblNearestBranch.Text = "Nearest Branch: Not Available";
                lblBranchAddress.Text = "Address: Not Available";
                lblAmbulanceTime.Text = "Estimated Ambulance Arrival Time: Unknown";
                lblMessage.Text = "No active emergency.";
            }
        }

        private void GetNearestBranch(string userAddress, ref string branchName, ref string branchAddress)
        {
            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                string query = "SELECT TOP 1 B.name, B.address FROM Branch B JOIN Location L ON B.locationID = L.locationID";
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    branchName = reader["name"].ToString();
                    branchAddress = reader["address"].ToString();
                }
            }
        }



        private (double Distance, string Duration) GetTravelDetails(string originAddress, string destAddress)
        {
            string apiKey = "AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo";
            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(originAddress)}&destinations={Uri.EscapeDataString(destAddress)}&key={apiKey}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;
                    dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonResponse);

                    if (data.rows.Count > 0 && data.rows[0].elements[0].status == "OK")
                    {
                        double distanceInKm = data.rows[0].elements[0].distance.value / 1000.0; // Convert meters to kilometers
                        string duration = data.rows[0].elements[0].duration.text;

                        return (distanceInKm, duration);
                    }
                }
            }

            return (0, "Unavailable");
        }




        private void RegisterMapScript(double latitude, double longitude)
        {
            string mapScript = $@"
                function initMap() {{
                    const branchLocation = {{ lat: {latitude}, lng: {longitude} }};
                    const map = new google.maps.Map(document.getElementById('map'), {{
                        center: branchLocation,
                        zoom: 14,
                    }});

                    new google.maps.Marker({{
                        position: branchLocation,
                        map: map,
                        title: 'Nearest Branch',
                    }});
                }}
                initMap();";

            ScriptManager.RegisterStartupScript(this, GetType(), "initMap", mapScript, true);
        }

        private void SimulateProgress()
        {
            string progressScript = @"
                let progress = 0;
                const progressBar = document.getElementById('progressBar');
                const interval = setInterval(() => {
                    if (progress >= 100) {
                        clearInterval(interval);
                        alert('Emergency assistance has arrived!');
                    } else {
                        progress += 0.2;
                        progressBar.style.width = progress + '%';
                    }
                }, 1000);";
            ScriptManager.RegisterStartupScript(this, GetType(), "simulateProgress", progressScript, true);
        }

        private string GetUserAddressFromEmergencyAlert()
        {
            string address = string.Empty;
            HttpCookie IDCookie = Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                string query = @"
        SELECT L.address
        FROM EmergencyAlert EA
        JOIN Location L ON EA.locationID = L.locationID
        WHERE EA.patientID = @patientID";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@patientID", patientID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    address = reader["address"].ToString();
                }
            }

            return address;
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
        public static string EmergencyAlert(string patientID, string address)
        {
            string resultMessage = string.Empty;

            try
            {
                // Save the address in the database (assuming you already have this method)
                SaveEmergencyAlert(patientID, address);

                // Get the nearest hospital/branch based on the address
                var branchInfo = GetNearestBranch(address); // Get the nearest branch based on the user's address

                if (branchInfo != null)
                {
                    string distance = branchInfo.Distance;
                    string duration = branchInfo.Duration;

                    resultMessage = $"Emergency alert sent successfully! Estimated time: {duration}, Distance: {distance}";
                }
                else
                {
                    resultMessage = "Unable to calculate time and distance to nearest branch.";
                }
            }
            catch (Exception ex)
            {
                resultMessage = $"Error: {ex.Message}";
            }

            return resultMessage;
        }

        private static void SaveEmergencyAlert(string patientID, string address)
        {
            // Save the emergency alert with the address in the database
            using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
            {
                string query = "INSERT INTO EmergencyAlert (PatientID, Address) VALUES (@patientID, @address)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@patientID", patientID);
                command.Parameters.AddWithValue("@address", address);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        // Method to calculate the nearest hospital/branch and get distance & time based on the user's address
        private static BranchInfo GetNearestBranch(string userAddress)
        {
            string apiKey = "AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo";

            // Replace NEAREST_HOSPITAL_ADDRESS with the address or a list of addresses of your branches/hospitals
            string nearestBranchApiUrl = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={Uri.EscapeDataString(userAddress)}&destinations=NEAREST_HOSPITAL_ADDRESS&key={apiKey}";

            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync(nearestBranchApiUrl).Result;
                dynamic responseJson = JsonConvert.DeserializeObject(response);

                if (responseJson.rows.Count > 0 && responseJson.rows[0].elements[0].status == "OK")
                {
                    string distance = responseJson.rows[0].elements[0].distance.text;
                    string duration = responseJson.rows[0].elements[0].duration.text;

                    return new BranchInfo
                    {
                        Distance = distance,
                        Duration = duration
                    };
                }
            }

            return null;
        }

        public class BranchInfo
        {
            public string Distance { get; set; }
            public string Duration { get; set; }
        }

    }
}
