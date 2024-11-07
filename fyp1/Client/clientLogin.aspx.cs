using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
namespace fyp1.Client
{
    public partial class clientLogin : System.Web.UI.Page
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
            string loginInput = txtUsername.Text;
            string password = txtPassword.Text;
            bool isValidUser = IsValidUser(loginInput, password, out string patientID);
            if (isValidUser)
            {
                // Set cookies and redirect

                SetUsernameCookie(loginInput);
                SetIDCookie(patientID);
                Response.Redirect("clienthome.aspx");
            }
            else
            {
                lblErrorMessage.Text = "Invalid username or password. Please try again.";
            }
        }
        public void SetUsernameCookie(string username)
        {
            HttpCookie usernameCookie = new HttpCookie("Username", username)
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };
            HttpContext.Current.Response.Cookies.Add(usernameCookie);
        }
        public void SetIDCookie(string patientID)
        {
            HttpCookie IDCookie = new HttpCookie("PatientID", patientID)
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };
            HttpContext.Current.Response.Cookies.Add(IDCookie);
        }
        private bool IsValidUser(string loginInput, string password, out string patientID)
        {
            bool isValid = false;
            patientID = null;
            string hashedPassword = HashPassword(password);
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                // Query to check if the input is name or email and validates against password
                string query = @"SELECT patientID 
                                 FROM Patient 
                                 WHERE (name = @loginInput OR email = @loginInput) AND password = @password";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@loginInput", loginInput);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        patientID = result.ToString();
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
