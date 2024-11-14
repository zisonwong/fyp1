using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace hospital
{
    public partial class adminSideBar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateStaffName();
                EncodeStaffRole();
                lblServerTime.Text = DateTime.Now.ToString("F"); // Set initial time on page load

                // Load the search term from the query string, if it exists
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    txtSearch.Text = HttpUtility.UrlDecode(searchTerm); // Set the TextBox value
                }
                else
                {
                    txtSearch.Text = string.Empty; // Clear the TextBox if no search term is provided
                }
            }

        }
        private void PopulateStaffName()
        {
            // Retrieve staff information from the database
            string staffID = GetStaffIDCookie();
            if (staffID != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string query = "SELECT staffName, staffRole FROM Staff WHERE staffID = @staffID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@staffID", staffID);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            // Populate textboxes with staff information
                            lblStaffName.Text = reader["staffName"].ToString();
                        }
                    }
                }
            }
        }
        private void EncodeStaffRole()

        {

            // Retrieve staff information from the database

            string staffID = GetStaffIDCookie();

            if (staffID != null)
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                string query = "SELECT staffRole FROM Staff WHERE staffID = @staffID";

                using (SqlConnection connection = new SqlConnection(connectionString))

                {

                    using (SqlCommand command = new SqlCommand(query, connection))

                    {

                        command.Parameters.AddWithValue("@staffID", staffID);

                        connection.Open();

                        string staffRole = (string)command.ExecuteScalar(); // Assuming staffRole is a string

                        // Encode the staffRole value into a JavaScript variable

                        string script = "var userRole = '" + staffRole + "';";

                        // Register the script block on the page

                        ScriptManager.RegisterStartupScript(this, this.GetType(), "UserRoleScript", script, true);

                    }

                }
            }

        }

        public string GetStaffIDCookie()
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["staffID"];
            if (cookie != null)
            {
                return cookie.Value;
            }
            return null; // Return null if the cookie does not exist
        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            lblServerTime.Text = DateTime.Now.ToString("F"); // Update time on each tick
        }
        protected void lBtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                // Redirect to the same page with the search term as a query parameter
                string currentUrl = Request.Url.AbsoluteUri.Split('?')[0];
                string newUrl = $"{currentUrl}?q={HttpUtility.UrlEncode(searchTerm)}";
                Response.Redirect(newUrl);
            }
            else
            {
                // If the search box is cleared, redirect to the page without the query parameter
                string currentUrl = Request.Url.AbsoluteUri.Split('?')[0];
                Response.Redirect(currentUrl);
            }
        }

    }

}