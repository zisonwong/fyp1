using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1
{
    public partial class hospitalStaffLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["signup"]) && Request.QueryString["signup"] == "success")
            {
                // Display the signup success popup
                ClientScript.RegisterStartupScript(this.GetType(), "SignupSuccessPopup",
                    "<script>alert('Sign up successful! You can now login with your credentials.');</script>");
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string loginInput = txtEmail.Text;
            string password = txtPassword.Text;
            bool isValidUser = IsValidUser(loginInput, password, out string doctorID);
            if (isValidUser)
            {
                // Set cookies and redirect

                SetIDCookie(doctorID);
                Response.Redirect("adminHome.aspx");
            }
            else
            {
                lblErrorMessage.Text = "Invalid username or password. Please try again.";
            }
        }
        public void SetIDCookie(string doctorID)
        {
            HttpCookie IDCookie = new HttpCookie("DoctorID", doctorID)
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };
            HttpContext.Current.Response.Cookies.Add(IDCookie);
        }
        private bool IsValidUser(string loginInput, string password, out string doctorID)
        {
            bool isValid = false;
            doctorID = null;
            string hashedPassword = HashPassword(password);
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // Query to check if the input is name or email and validates against password
                string query = @"SELECT doctorID 
                                 FROM Doctor 
                                 WHERE (email = @loginInput) AND password = @password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@loginInput", loginInput);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        doctorID = result.ToString();
                        isValid = true;
                    }
                }
            }
            return isValid;
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
    }
}