using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using fyp1.Admin;
using System.Web.Services;
using Newtonsoft.Json;
using System.Web.Script.Services;
using System.IO;
using System.Net;
using System.Web.UI;
using Stripe.Billing;

namespace fyp1
{
    public partial class NavFooter : System.Web.UI.MasterPage
    {
        private const string DEFAULT_ADDRESS = "57A, Lorong Ikan Emas 5, Jalan Cheras, Batu 3 1/2, 56000, Kuala Lumpur";
        private const string GOOGLE_API_KEY = "AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                HttpCookie addressCookie = Request.Cookies["UserAddress"];

                if (addressCookie == null || string.IsNullOrEmpty(addressCookie.Value))
                {
                    SetDefaultUserLocation();
                }

                string userIp = GetUserIpAddress();
                if (!string.IsNullOrEmpty(userIp))
                {
                    var location = GetLocationFromGoogleApi(userIp);

                    if (!string.IsNullOrEmpty(location))
                    {
                        SaveUserLocation(location);

                        List<Branch> branches = GetBranchesFromDatabase();

                        // Use the synchronous version of the method
                        Branch nearestBranch = FindNearestBranchByDistanceSync(location, branches);

                        if (nearestBranch != null)
                        {
                            lblBranchAddress.InnerText = $"{nearestBranch.Address}";
                        }
                        else
                        {
                            lblBranchAddress.InnerText = "No nearest branch found.";
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('Unable to retrieve location.');</script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Unable to fetch IP address.');</script>");
                }
            }
        }

        protected string GetProfileOrLoginLink()
        {
            HttpCookie usernameCookie = Request.Cookies["Username"];
            HttpCookie patientIDCookie = Request.Cookies["PatientID"];

            if (usernameCookie == null || string.IsNullOrEmpty(usernameCookie.Value) ||
                patientIDCookie == null || string.IsNullOrEmpty(patientIDCookie.Value))
            {
                return "../Client/clientLogin.aspx";
            }
            return "../Client/clientProfile.aspx";
        }

        private async Task<Branch> FindNearestBranchByDistance(string userAddress, List<Branch> branches)
        {
            try
            {
                // Log input parameters
                System.Diagnostics.Debug.WriteLine($"User Address: {userAddress}");
                System.Diagnostics.Debug.WriteLine($"Total Branches: {branches.Count}");
                foreach (var branch in branches)
                {
                    System.Diagnostics.Debug.WriteLine($"Branch: {branch.Name}, Address: {branch.Address}");
                }

                string origins = HttpUtility.UrlEncode(userAddress);
                string destinations = string.Join("|", branches.Select(b => HttpUtility.UrlEncode(b.Address)));

                string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origins}&destinations={destinations}&key={GOOGLE_API_KEY}";

                // Log the full URL for debugging
                System.Diagnostics.Debug.WriteLine($"Distance Matrix API URL: {url}");

                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);

                    // Log the raw response
                    System.Diagnostics.Debug.WriteLine($"Raw API Response: {response}");

                    dynamic result = JsonConvert.DeserializeObject(response);

                    // Detailed status checking with logging
                    if (result == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Result is null");
                        return null;
                    }

                    if (result.status == null || result.status.ToString() != "OK")
                    {
                        System.Diagnostics.Debug.WriteLine($"API Status: {result.status}");
                        return null;
                    }

                    if (result.rows == null || result.rows.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("No rows in the result");
                        return null;
                    }

                    var branchDistances = new List<BranchDistance>();

                    for (int i = 0; i < branches.Count; i++)
                    {
                        // More detailed logging for each branch
                        System.Diagnostics.Debug.WriteLine($"Processing Branch {i}: {branches[i].Name}");

                        // Additional null checks with logging
                        if (result.rows[0].elements == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Elements are null for branch {i}");
                            continue;
                        }

                        if (result.rows[0].elements[i] == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Element is null for branch {i}");
                            continue;
                        }

                        if (result.rows[0].elements[i].status.ToString() != "OK")
                        {
                            System.Diagnostics.Debug.WriteLine($"Status not OK for branch {i}: {result.rows[0].elements[i].status}");
                            continue;
                        }

                        // Null checks for distance and duration
                        if (result.rows[0].elements[i].distance == null ||
                            result.rows[0].elements[i].duration == null)
                        {
                            System.Diagnostics.Debug.WriteLine($"Distance or duration is null for branch {i}");
                            continue;
                        }

                        int distance = result.rows[0].elements[i].distance.value / 1000; // Convert to kilometers
                        int time = result.rows[0].elements[i].duration.value / 60; // Convert to minutes

                        System.Diagnostics.Debug.WriteLine($"Branch {i} - Distance: {distance} km, Time: {time} minutes");

                        branchDistances.Add(new BranchDistance
                        {
                            Branch = branches[i],
                            Distance = distance,
                            Time = time
                        });
                    }

                    var sortedBranches = branchDistances
                        .OrderBy(bd => bd.Distance)
                        .ToList();

                    System.Diagnostics.Debug.WriteLine($"Total branches with valid distances: {sortedBranches.Count}");

                    if (sortedBranches.Any())
                    {
                        var nearestBranch = sortedBranches.First().Branch;
                        nearestBranch.Distance = sortedBranches.First().Distance;
                        nearestBranch.Time = sortedBranches.First().Time;

                        System.Diagnostics.Debug.WriteLine($"Nearest Branch: {nearestBranch.Name}, Distance: {nearestBranch.Distance} km, Time: {nearestBranch.Time} minutes");

                        return nearestBranch;
                    }

                    return null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding nearest branch: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
                return null;
            }
        }

        private Branch FindNearestBranchByDistanceSync(string userAddress, List<Branch> branches)
        {
            return Task.Run(() => FindNearestBranchByDistance(userAddress, branches)).GetAwaiter().GetResult();
        }


        private static List<Branch> GetBranchesFromDatabase()
        {
            var branches = new List<Branch>();
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string query = @"SELECT branchID, name, address FROM Branch";

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var branch = new Branch
                                {
                                    BranchID = reader["branchID"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Address = reader["address"].ToString()
                                };

                                branches.Add(branch);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
            }

            return branches;
        }

        private string GetUserAddress()
        {
            HttpCookie addressCookie = Request.Cookies["UserAddress"];

            if (addressCookie != null && !string.IsNullOrEmpty(addressCookie.Value))
            {
                return HttpUtility.UrlDecode(addressCookie.Value);
            }

            return DEFAULT_ADDRESS;
        }

        [WebMethod]
        [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
        public static void SaveUserLocation(string address)
        {
            if (string.IsNullOrEmpty(address)) return;

            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string locationId = generateNextLocationID();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Upsert logic: insert or update location
                    string query = @"
                MERGE INTO Location AS target
                USING (SELECT @LocationId AS locationID, @Address AS address) AS source
                ON (target.locationID = source.locationID)
                WHEN MATCHED THEN 
                    UPDATE SET address = source.address
                WHEN NOT MATCHED THEN 
                    INSERT (locationID, address) 
                    VALUES (source.locationID, source.address);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LocationId", locationId);
                        cmd.Parameters.AddWithValue("@Address", address);

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                // Still set the cookie for immediate use
                HttpCookie addressCookie = new HttpCookie("UserAddress")
                {
                    Value = HttpUtility.UrlEncode(address),
                    Expires = DateTime.Now.AddDays(30),
                    Path = "/",
                    HttpOnly = true
                };
                HttpContext.Current.Response.Cookies.Add(addressCookie);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveUserLocation Error: {ex.Message}");
                throw;
            }
        }

        [WebMethod]
        public static void ClearUserLocation()
        {
            HttpCookie addressCookie = new HttpCookie("UserAddress")
            {
                Expires = DateTime.Now.AddDays(-1),
                Path = "/"
            };

            HttpContext.Current.Response.Cookies.Add(addressCookie);
        }

        private string GetUserIpAddress()
        {
            string ip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = Request.ServerVariables["REMOTE_ADDR"];
            }
            return ip;
        }

        private string GetLocationFromGoogleApi(string ipAddress)
        {
            try
            {
                string apiKey = "AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo";
                string geolocationUrl = $"https://www.googleapis.com/geolocation/v1/geolocate?key={apiKey}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geolocationUrl);
                request.Method = "POST";
                request.ContentType = "application/json";

                using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
                {
                    string payload = "{}";
                    writer.Write(payload);
                }

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        dynamic locationData = JsonConvert.DeserializeObject(jsonResponse);

                        double lat = locationData.location.lat;
                        double lng = locationData.location.lng;

                        return GetAddressFromGoogleApi(lat, lng, apiKey);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error fetching location: " + ex.Message);
            }

            return null;
        }

        private string GetAddressFromGoogleApi(double lat, double lng, string apiKey)
        {
            try
            {
                string geocodingUrl = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={lat},{lng}&key={apiKey}";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(geocodingUrl);
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string jsonResponse = reader.ReadToEnd();
                        dynamic addressData = JsonConvert.DeserializeObject(jsonResponse);

                        if (addressData.status == "OK" && addressData.results.Count > 0)
                        {
                            return addressData.results[0].formatted_address;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error fetching address: " + ex.Message);
            }
            return null;
        }

        protected void EmergencyButton_Click(object sender, EventArgs e)
        {
            string userAddress = GetUserAddress();

            HttpCookie patientCookie = Request.Cookies["patientID"];

            string locationID = GetLocationIDByAddress(userAddress);
            if (string.IsNullOrEmpty(userAddress))
            {
                lblEmergencyMessage.Text = "Please enable location permissions and try again.";
                return;
            }

            try
            {
                var branches = GetBranchesFromDatabase();
                Branch nearestBranch = FindNearestBranchBySynchronousMethod(userAddress, branches);

                if (nearestBranch == null)
                {
                    lblEmergencyMessage.Text = "Could not find the nearest branch.";
                    return;
                }
                string alertID = generateNextAlertID();
                string patientID = patientCookie.Value;

                SaveEmergencyAlert(alertID, patientID, locationID);

                string redirectUrl = $"../Client/EmergencyPage.aspx?branchName={HttpUtility.UrlEncode(nearestBranch.Name)}" +
                                     $"&branchAddress={HttpUtility.UrlEncode(nearestBranch.Address)}" +
                                     $"&distance={nearestBranch.Distance}" +
                                     $"&time={nearestBranch.Time}";

                Response.Redirect(redirectUrl, false);
            }
            catch (Exception ex)
            {
                lblEmergencyMessage.Text = "Unable to fetch the nearest branch. Please try again.";
                System.Diagnostics.Debug.WriteLine($"Emergency Error: {ex.Message}");
            }
        }

        private string GetLocationIDByAddress(string userAddress)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                {
                    conn.Open();
                    string query = "SELECT locationID FROM Location WHERE address = @userAddress";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userAddress", userAddress);

                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : null;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
        }

        private void SaveEmergencyAlert(string alertID, string patientID, string locationID)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString))
                {
                    conn.Open();
                    string query = @"
                INSERT INTO EmergencyAlert (alertID, patientID, locationID, timestamp, status)
                VALUES (@alertID, @patientID, @locationID, @timestamp, @status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@alertID", alertID);
                        cmd.Parameters.AddWithValue("@patientID",patientID);
                        cmd.Parameters.AddWithValue("@locationID",locationID);
                        cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                        cmd.Parameters.AddWithValue("@status", "Active");
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database Error: {ex.Message}");
                throw;
            }
        }

        private Branch FindNearestBranchBySynchronousMethod(string userAddress, List<Branch> branches)
        {
            try
            {
                string origins = HttpUtility.UrlEncode(userAddress);
                var branchDistances = new List<BranchDistance>();

                foreach (var branch in branches)
                {
                    string destination = HttpUtility.UrlEncode(branch.Address);
                    string url = $"https://maps.googleapis.com/maps/api/directions/json?origin={origins}&destination={destination}&departure_time=now&key={GOOGLE_API_KEY}";

                    using (WebClient client = new WebClient())
                    {
                        string response = client.DownloadString(url);
                        dynamic result = JsonConvert.DeserializeObject(response);

                        // Check for valid response
                        if (result == null ||
                            result.status == null ||
                            result.status.ToString() != "OK" ||
                            result.routes == null ||
                            result.routes.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid response from Google Directions API: {response}");
                            continue;
                        }

                        // Extract the first route's distance and duration
                        var leg = result.routes[0].legs[0];
                        if (leg != null)
                        {
                            int distance = leg.distance.value / 1000; // Convert to kilometers
                            int time = leg.duration_in_traffic != null
                                ? leg.duration_in_traffic.value / 60 // Real-time traffic duration in minutes
                                : leg.duration.value / 60; // Fallback to standard duration

                            branchDistances.Add(new BranchDistance
                            {
                                Branch = branch,
                                Distance = distance,
                                Time = time
                            });
                        }
                    }
                }

                var sortedBranches = branchDistances
                    .OrderBy(bd => bd.Distance)
                    .ToList();

                if (sortedBranches.Any())
                {
                    var nearestBranch = sortedBranches.First().Branch;
                    nearestBranch.Distance = sortedBranches.First().Distance;
                    nearestBranch.Time = sortedBranches.First().Time;
                    return nearestBranch;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error finding nearest branch: {ex.Message}");
                return null;
            }
        }

        private static string generateNextLocationID()
        {
            string nextLocationID = "LOC00001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(LocationID) FROM Location", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(3)) + 1;
                            nextLocationID = "LOC" + idNumber.ToString("D5");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextLocationID;
        }

        private static string generateNextAlertID()
        {
            string nextAlertID = "A00001";
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
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextAlertID = "A" + idNumber.ToString("D5");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextAlertID;
        }

        private void SetDefaultUserLocation()
        {
            HttpCookie addressCookie = new HttpCookie("UserAddress")
            {
                Value = HttpUtility.UrlEncode(DEFAULT_ADDRESS),
                Expires = DateTime.Now.AddDays(30),
                Path = "/"
            };

            Response.Cookies.Add(addressCookie);
        }

        private class BranchDistance
        {
            public Branch Branch { get; set; }
            public int Distance { get; set; }
            public int Time { get; set; }
        }
    }

    public class Branch
    {
        public string BranchID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public int Distance { get; set; } // in km
        public int Time { get; set; } // in minutes
    }
}