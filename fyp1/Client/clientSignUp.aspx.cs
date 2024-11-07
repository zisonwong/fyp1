using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientSignUp : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
            }
        }

        protected void btnSignup_Click(object sender, EventArgs e)
        {
            string icNumber = identityCard.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();
            string address1 = txtAddress1.Text.Trim();
            string address2 = txtAddress2.Text.Trim();
            string postalCode = txtPostalCode.Text.Trim();

            // Enhanced Validation
            if (!ValidateInputs(icNumber, username, email, password, confirmPassword, address1, postalCode))
                return;

            string dob = ExtractDOBFromIC(icNumber);
            if (dob == null)
            {
                lblErrorMessage.Text = "Invalid IC number format for DOB extraction.";
                return;
            }

            // Hash the password before storing it
            string hashedPassword = HashPassword(password);

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    if (IsUsernameTaken(con, username))
                    {
                        lblErrorMessage.Text = "Username is already taken.";
                        return;
                    }

                    // Insert new user record
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Patient (patientID, ICNumber, name, DOB, email, password, address) VALUES (@patientID, @ICNumber, @username, @DOB, @Email, @Password, @Address)", con))
                    {
                        string nextPatientID = GenerateNextPatientID();
                        cmd.Parameters.AddWithValue("@patientID", nextPatientID);
                        cmd.Parameters.AddWithValue("@ICNumber", icNumber);
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@DOB", dob);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Address", $"{address1} {address2}");

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            Response.Redirect("clientLogin.aspx?signup=success");
                        }
                        else
                        {
                            lblErrorMessage.Text = "Sign up failed. Please try again.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred during signup: " + ex.Message;
            }
        }

        private bool ValidateInputs(string icNumber, string username, string email, string password, string confirmPassword, string address1, string postalCode)
        {
            // Basic Required Fields Check
            if (string.IsNullOrWhiteSpace(icNumber) || string.IsNullOrWhiteSpace(username) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword) || string.IsNullOrWhiteSpace(address1) ||
                string.IsNullOrWhiteSpace(postalCode))
            {
                lblErrorMessage.Text = "Please fill in all required fields.";
                return false;
            }

            // IC Number Validation
            if (icNumber.Length != 12 || !Regex.IsMatch(icNumber, @"^\d{12}$"))
            {
                lblErrorMessage.Text = "IC number must be exactly 12 digits.";
                return false;
            }

            // Password Match Check
            if (password != confirmPassword)
            {
                lblErrorMessage.Text = "Passwords do not match.";
                return false;
            }

            // Password Strength Check
            if (password.Length < 8 || !Regex.IsMatch(password, @"[A-Z]") || !Regex.IsMatch(password, @"[0-9]") || !Regex.IsMatch(password, @"[@$!%*?&]"))
            {
                lblErrorMessage.Text = "Password must be at least 8 characters, including an uppercase letter, a number, and a special character.";
                return false;
            }

            // Email Format Check
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                lblErrorMessage.Text = "Please enter a valid email address.";
                return false;
            }

            // Postal Code Check
            if (!Regex.IsMatch(postalCode, @"^\d{5}$"))
            {
                lblErrorMessage.Text = "Postal code must be a 5-digit number.";
                return false;
            }

            return true;
        }

        private string ExtractDOBFromIC(string icNumber)
        {
            try
            {
                string year = icNumber.Substring(0, 2);
                string month = icNumber.Substring(2, 2);
                string day = icNumber.Substring(4, 2);
                string fullYear = int.Parse(year) > 24 ? "19" + year : "20" + year;

                DateTime dob;
                if (DateTime.TryParseExact($"{fullYear}-{month}-{day}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob))
                {
                    return dob.ToString("yyyy-MM-dd");
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred while extracting DOB from IC number" + ex.Message;
            }
            return null;
        }

        private bool IsUsernameTaken(SqlConnection con, string username)
        {
            using (SqlCommand checkUserCmd = new SqlCommand("SELECT COUNT(*) FROM Patient WHERE name = @username", con))
            {
                checkUserCmd.Parameters.AddWithValue("@username", username);
                return Convert.ToInt32(checkUserCmd.ExecuteScalar()) > 0;
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateNextPatientID()
        {
            string nextPatientID = "P1001";
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(patientID) FROM Patient", con))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextPatientID = "P" + idNumber.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = "An error occurred while generating patient ID: " + ex.Message;
            }
            return nextPatientID;
        }
    }
}
