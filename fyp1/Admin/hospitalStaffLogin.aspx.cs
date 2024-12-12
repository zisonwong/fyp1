﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalStaffLogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["signup"]) && Request.QueryString["signup"] == "success")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SignupSuccessPopup",
                    "<script>alert('Sign up successful! You can now login with your credentials.');</script>");
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string loginInput = txtEmail.Text;
            string password = txtPassword.Text;
            bool isValidUser = IsValidUser(loginInput, password, out string userID);

            if (isValidUser)
            {
                string role;

                if (userID == "Admin")
                {
                    role = "Admin";
                }
                else
                {
                    role = DetermineRole(userID);
                }

                // Set cookies
                SetIDCookie(userID);
                SetEmailCookie(loginInput);
                SetRoleCookie(role);

                Session["Role"] = role;

                Response.Redirect("~/Admin/adminHome.aspx");
            }
            else
            {
                lblErrorMessage.Text = "Invalid username or password. Please try again.";
            }
        }
        private string DetermineRole(string userID)
        {
            if (userID.StartsWith("D"))
                return "Doctor";
            else if (userID.StartsWith("N"))
                return "Nurse";

            return "Unknown"; 
        }
        public void SetRoleCookie(string role)
        {
            HttpCookie roleCookie = new HttpCookie("Role", role)
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(1)
            };
            HttpContext.Current.Response.Cookies.Add(roleCookie);
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

        public void SetEmailCookie(string email)
        {
            HttpCookie emailCookie = new HttpCookie("Email", email)
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTime.Now.AddDays(3)
            };
            HttpContext.Current.Response.Cookies.Add(emailCookie);
        }
        private bool IsValidUser(string loginInput, string password, out string userID)
        {
            bool isValid = false;
            userID = null;
            string hashedPassword = HashPassword(password);

            // Hardcoded admin credentials
            const string adminEmail = "admin@gmail.com";
            const string adminPassword = "admin123";

            if (loginInput.Equals(adminEmail, StringComparison.OrdinalIgnoreCase) && password.Equals(adminPassword))
            {
                userID = "Admin"; 
                isValid = true;
                return isValid;
            }

            // If not admin, check against database
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                string query = @"
            SELECT doctorID AS UserID FROM Doctor 
            WHERE (email = @loginInput) AND password = @password
            UNION
            SELECT nurseID AS UserID FROM Nurse 
            WHERE (email = @loginInput) AND password = @password";

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@loginInput", loginInput);
                    cmd.Parameters.AddWithValue("@password", hashedPassword);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        userID = result.ToString();
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