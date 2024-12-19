using fyp1.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            string branchAddress = Request.QueryString["branchAddress"];
            string branchName = Request.QueryString["branchName"];
            string distance = Request.QueryString["distance"];
            string time = Request.QueryString["time"];

            if (!string.IsNullOrEmpty(branchAddress))
            {
                try
                {
                    // Geocode branch address
                    var (lat, lng) = GetBranchLatLng(HttpUtility.UrlDecode(branchAddress));

                    // Pass data to JavaScript
                    ClientScript.RegisterStartupScript(this.GetType(), "MapScript",
                        $"const BRANCH_LAT = {lat.ToString(CultureInfo.InvariantCulture)};" +
                        $"const BRANCH_LNG = {lng.ToString(CultureInfo.InvariantCulture)};" +
                        $"const BRANCH_ADDRESS = '{HttpUtility.JavaScriptStringEncode(branchAddress)}';" +
                        $"const DISTANCE = '{distance}';" +
                        $"const TIME = '{time}';" +
                        "initMap();", true);
                }
                catch (Exception ex)
                {
                    // Handle geocoding failure
                    ClientScript.RegisterStartupScript(this.GetType(), "ErrorScript",
                        $"alert('Failed to find branch location: {HttpUtility.JavaScriptStringEncode(ex.Message)}');", true);
                }
            }

            if (!IsPostBack)
            {
                lblBranchName.Text = !string.IsNullOrEmpty(branchName) ? branchName : "N/A";
                lblBranchAddress.Text = !string.IsNullOrEmpty(branchAddress) ? branchAddress : "N/A";
                lblDistance.Text = !string.IsNullOrEmpty(distance) ? $"{distance} km" : "N/A";
                lblTime.Text = !string.IsNullOrEmpty(time) ? $"{time} mins" : "N/A";
            }
        }

        private (double, double) GetBranchLatLng(string branchAddress)
        {
            string requestUrl = $"https://maps.googleapis.com/maps/api/geocode/json?address={Uri.EscapeDataString(branchAddress)}&key={GOOGLE_API_KEY}";

            using (HttpClient client = new HttpClient())
            {
                var response = client.GetStringAsync(requestUrl).Result;
                dynamic jsonResponse = Newtonsoft.Json.JsonConvert.DeserializeObject(response);

                if (jsonResponse.status == "OK")
                {
                    double lat = jsonResponse.results[0].geometry.location.lat;
                    double lng = jsonResponse.results[0].geometry.location.lng;
                    return (lat, lng);
                }
                else
                {
                    // Log the error for debugging
                    System.Diagnostics.Debug.WriteLine($"Geocoding failed: {jsonResponse.status}");
                    throw new Exception($"Unable to geocode address: {branchAddress}");
                }
            }
        }
    }
}
