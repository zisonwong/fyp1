using fyp1.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class EmergencyPage : System.Web.UI.Page
    {
        private const string GOOGLE_API_KEY = "AIzaSyAbFkinyyHf8XwboZ1KHr7yupFq2yo_ufo";

        public string BranchName { get; private set; }
        public string BranchAddress { get; private set; }
        public string Distance { get; private set; }
        public string EstimatedTime { get; private set; }
        public double BranchLatitude { get; private set; }
        public double BranchLongitude { get; private set; }
        public double PatientLatitude { get; private set; }
        public double PatientLongitude { get; private set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            string userAddress = Request.Cookies["UserAddress"]?.Value;
            if (!string.IsNullOrEmpty(userAddress))
            {
                userAddress = HttpUtility.UrlDecode(userAddress);
            }
            else
            {
                Response.Write("User address not found.");
            }

            if (!IsPostBack)
            {


                // Retrieve query string parameters
                string branchName = Request.QueryString["branchName"];
                string branchAddress = Request.QueryString["branchAddress"];
                string distance = Request.QueryString["distance"];
                string time = Request.QueryString["time"];

                // Set the labels
                lblBranchName.Text = !string.IsNullOrEmpty(branchName) ? branchName : "N/A";
                lblBranchAddress.Text = !string.IsNullOrEmpty(branchAddress) ? branchAddress : "N/A";
                lblDistance.Text = !string.IsNullOrEmpty(distance) ? $"{distance} km" : "N/A";
                lblTime.Text = !string.IsNullOrEmpty(time) ? $"{time} mins" : "N/A";
            }
        }

        private async Task<string> GetDistanceFromBranchAsync(string userAddress, double userLat, double userLng, List<Branch> branches)
        {
            string origins = HttpUtility.UrlEncode(userAddress);  // The user's address
            string destinations = string.Join("|", branches.Select(b => HttpUtility.UrlEncode(b.Address)));

            string url = $"https://maps.googleapis.com/maps/api/distancematrix/json?origins={origins}&destinations={destinations}&key={GOOGLE_API_KEY}";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                dynamic result = JsonConvert.DeserializeObject(response);

                // Check for errors in the API response
                if (result?.status?.ToString() != "OK")
                {
                    return "Error: Could not calculate distance.";
                }

                // You can now process the result and extract the distance and time.
                // Example: result.rows[0].elements[i].distance.text, result.rows[0].elements[i].duration.text
                return "Distance information retrieved successfully.";
            }
        }

    }
}
