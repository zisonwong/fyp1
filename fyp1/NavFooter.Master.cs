using fyp1.Admin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1
{
    public partial class NavFooter : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }
        public string GetUserLocation()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync("http://ip-api.com/json/").Result;
                dynamic locationData = JsonConvert.DeserializeObject(response);
                return locationData.state ?? "Unknown";
            }
        }
        public (double Latitude, double Longitude) GetUserCoordinates()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetStringAsync("http://ip-api.com/json/").Result;
                dynamic locationData = JsonConvert.DeserializeObject(response);
                double latitude = locationData.lat;
                double longitude = locationData.lon;
                return (latitude, longitude);
            }
        }
    }
}