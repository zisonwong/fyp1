using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalStaffSettings : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            string userID = GetUserID();
            string userRole = Request.Cookies["Role"]?.Value.ToLower();

            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(userRole))
            {
                return;
            }

            DateTime? lastNameChangeDate = null;
            string queryCheckDate = "";
            if (userRole == "doctor")
            {
                queryCheckDate = "SELECT lastNameChangeDate FROM Doctor WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                queryCheckDate = "SELECT lastNameChangeDate FROM Nurse WHERE nurseID = @UserID";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryCheckDate, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        lastNameChangeDate = cmd.ExecuteScalar() as DateTime?;

                        if (lastNameChangeDate.HasValue && (DateTime.Now - lastNameChangeDate.Value).TotalDays < 60)
                        {
                            modalEditNameLink.Attributes["class"] += " disabled"; 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception if needed
                Console.WriteLine("Error: " + ex.Message);
            }
            if (!IsPostBack)
            {
                PopulateUserInfo();
                LoadUserImage();
            }
        }
        private void PopulateUserInfo()
        {
            string userID = "";
            string userRole = "";

            // Retrieve user ID and role from cookies
            if (Request.Cookies["Role"] != null)
            {
                userRole = Request.Cookies["Role"].Value.ToLower();

                if (userRole == "doctor" && Request.Cookies["DoctorID"] != null)
                {
                    userID = Request.Cookies["DoctorID"].Value;
                }
                else if (userRole == "nurse" && Request.Cookies["nurseID"] != null)
                {
                    userID = Request.Cookies["nurseID"].Value;
                }
                else
                {
                    txtName.Text = "Error: User ID or Role not found.";
                    return;
                }
            }
            else
            {
                txtName.Text = "Error: Role not found.";
                return;
            }

            // Query construction based on user role
            string query = "";
            if (userRole == "doctor")
            {
                query = "SELECT name, ICNumber, contactInfo, email FROM Doctor WHERE doctorID = @userID";
            }
            else if (userRole == "nurse")
            {
                query = "SELECT name, ICNumber, contactInfo, email FROM Nurse WHERE nurseID = @userID";
            }
            else
            {
                txtName.Text = "Error: Invalid role.";
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Add parameter dynamically
                        cmd.Parameters.AddWithValue("@userID", userID);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                // Use null-coalescing operator to handle null values
                                txtName.Text = reader["name"]?.ToString() ?? "N/A";
                                txtIC.Text = reader["ICNumber"]?.ToString() ?? "N/A";
                                txtPhone.Text = reader["contactInfo"]?.ToString() ?? "N/A";
                                txtEmail.Text = reader["email"]?.ToString() ?? "N/A";
                            }
                            else
                            {
                                txtName.Text = "Error: User not found.";
                            }
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                txtName.Text = "Database Error: " + sqlEx.Message;
            }
            catch (Exception ex)
            {
                txtName.Text = "Error: " + ex.Message;
            }
        }
        private string GetUserID()
        {
            if (Request.Cookies["Role"] != null)
            {
                string userRole = Request.Cookies["Role"].Value.ToLower();

                if (userRole == "doctor" && Request.Cookies["DoctorID"] != null)
                {
                    return Request.Cookies["DoctorID"].Value;
                }
                else if (userRole == "nurse" && Request.Cookies["nurseID"] != null)
                {
                    return Request.Cookies["nurseID"].Value;
                }
            }

            return null; 
        }
        protected void btnSaveName_Click(object sender, EventArgs e)
        {
            string userID = GetUserID();

            if (string.IsNullOrEmpty(userID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID not found.');", true);
                return;
            }

            string userRole = Request.Cookies["Role"]?.Value.ToLower();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string fullName = lastName + " " + firstName;

            if (string.IsNullOrEmpty(lastName))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Last name cannot be empty.');", true);
                return;
            }
            if (string.IsNullOrEmpty(firstName))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('First name cannot be empty.');", true);
                return;
            }

            DateTime? lastNameChangeDate = null;
            string queryCheckDate = "";
            if (userRole == "doctor")
            {
                queryCheckDate = "SELECT lastNameChangeDate FROM Doctor WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                queryCheckDate = "SELECT lastNameChangeDate FROM Nurse WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryCheckDate, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        lastNameChangeDate = cmd.ExecuteScalar() as DateTime?;

                        if (lastNameChangeDate.HasValue && (DateTime.Now - lastNameChangeDate.Value).TotalDays < 60)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('You cannot change your name again within 60 days.');", true);
                            return;
                        }
                    }

                    string queryUpdateName = "";
                    if (userRole == "doctor")
                    {
                        queryUpdateName = "UPDATE Doctor SET name = @FullName, lastNameChangeDate = @LastNameChangeDate WHERE doctorID = @UserID";
                    }
                    else if (userRole == "nurse")
                    {
                        queryUpdateName = "UPDATE Nurse SET name = @FullName, lastNameChangeDate = @LastNameChangeDate WHERE nurseID = @UserID";
                    }

                    using (SqlCommand cmd = new SqlCommand(queryUpdateName, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.Parameters.AddWithValue("@LastNameChangeDate", DateTime.Now);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Name updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update name.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }

            txtName.Text = fullName;

            ClearFields();
        }


        protected void btnSaveIC_Click(object sender, EventArgs e)
        {
            string userID = GetUserID(); 
            if (string.IsNullOrEmpty(userID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID not found.');", true);
                return;
            }

            string userRole = Request.Cookies["Role"]?.Value.ToLower();
            string icNumber = txtEditICNumber.Text.Trim();

            if (!IsValidICNumber(icNumber))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid IC Number format. Use format: 123456-11-1111');", true);
                return;
            }

            string yymmdd = icNumber.Substring(0, 6);
            DateTime dob;
            if (!TryParseICToDOB(yymmdd, out dob))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to parse DOB from IC Number.');", true);
                return;
            }

            string query = "";
            if (userRole == "doctor")
            {
                query = "UPDATE Doctor SET ICNumber = @ICNumber, DOB = @DOB WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "UPDATE Nurse SET ICNumber = @ICNumber, DOB = @DOB WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ICNumber", icNumber);
                        cmd.Parameters.AddWithValue("@DOB", dob.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('IC Number and DOB updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update IC Number and DOB.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
            txtIC.Text = icNumber;
            ClearFields();
        }

        protected void btnSavePhone_Click(object sender, EventArgs e)
        {
            string userID = GetUserID(); 
            if (string.IsNullOrEmpty(userID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID not found.');", true);
                return;
            }

            string userRole = Request.Cookies["Role"]?.Value.ToLower();
            string phoneNumber = txtEditPhone.Text.Trim();

            if (!ValidatePhone(phoneNumber))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid phone number. Must be up to 11 digits.');", true);
                return;
            }

            string query = "";
            if (userRole == "doctor")
            {
                query = "UPDATE Doctor SET contactInfo = @PhoneNumber WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "UPDATE Nurse SET contactInfo = @PhoneNumber WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Phone number updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update phone number.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
            txtPhone.Text = phoneNumber;
            ClearFields();
        }
        protected void btnSaveEmail_Click(object sender, EventArgs e)
        {
            string userID = GetUserID(); 
            string userRole = Request.Cookies["Role"]?.Value.ToLower();
            string email = txtEditEmail.Text.Trim();

            if (!ValidateEmailFormat(email))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid email format.');", true);
                return;
            }

            if (IsEmailExists(email, userID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email already exists. Please use a different email.');", true);
                return;
            }

            string query = "";
            if (userRole == "doctor")
            {
                query = "UPDATE Doctor SET email = @Email WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "UPDATE Nurse SET email = @Email WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update email.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
            txtEmail.Text = email;
            ClearFields();
        }
        protected void btnSavePassword_Click(object sender, EventArgs e)
        {
            string userID = GetUserID(); // Get the current user's ID
            string userRole = Request.Cookies["Role"]?.Value.ToLower(); // Get user role
            string currentPasswordInput = txtCurrentPassword.Text.Trim();
            string newPassword = txtNewPassword.Text.Trim();
            string confirmPassword = txtConfirmPassword.Text.Trim();

            // Validate new password format
            if (!ValidatePasswordFormat(newPassword))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('New password must contain at least 8 characters, one uppercase letter, one lowercase letter, one number, and one symbol.');", true);
                return;
            }

            // Confirm new password matches confirm password
            if (newPassword != confirmPassword)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('New password and confirm password do not match.');", true);
                return;
            }

            // Retrieve hashed password from database
            string hashedCurrentPassword = HashPassword(currentPasswordInput);
            if (!IsCurrentPasswordCorrect(userID, userRole, hashedCurrentPassword))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Current password is incorrect.');", true);
                return;
            }

            // Update password in the database
            string hashedNewPassword = HashPassword(newPassword);
            string query = "";

            if (userRole == "doctor")
            {
                query = "UPDATE Doctor SET password = @NewPassword WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "UPDATE Nurse SET password = @NewPassword WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewPassword", hashedNewPassword);
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update password.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
            ClearFields();
        }
        private bool ValidatePasswordFormat(string password)
        {
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";
            return System.Text.RegularExpressions.Regex.IsMatch(password, passwordPattern);
        }
        private bool IsCurrentPasswordCorrect(string userID, string userRole, string hashedPassword)
        {
            string query = "";

            if (userRole == "doctor")
            {
                query = "SELECT COUNT(*) FROM Doctor WHERE doctorID = @UserID AND password = @Password";
            }
            else if (userRole == "nurse")
            {
                query = "SELECT COUNT(*) FROM Nurse WHERE nurseID = @UserID AND password = @Password";
            }
            else
            {
                return false;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
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

        private bool IsValidICNumber(string icNumber)
        {
            string icPattern = @"^\d{6}-\d{2}-\d{4}$"; 
            return Regex.IsMatch(icNumber, icPattern);
        }
        private bool ValidatePhone(string phoneNumber)
        {
            var phonePattern = @"^\d{1,11}$"; 
            return Regex.IsMatch(phoneNumber, phonePattern);
        }
        private bool ValidateEmailFormat(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }
        private bool IsEmailExists(string email, string userID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT COUNT(*) FROM (
            SELECT email FROM Doctor WHERE email = @Email AND doctorID != @UserID
            UNION ALL
            SELECT email FROM Nurse WHERE email = @Email AND nurseID != @UserID
        ) AS EmailCheck";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@UserID", userID);

                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0; 
                }
            }
        }

        private bool TryParseICToDOB(string yymmdd, out DateTime dob)
        {
            dob = DateTime.MinValue;

            try
            {
                int year = int.Parse(yymmdd.Substring(0, 2));
                int month = int.Parse(yymmdd.Substring(2, 2));
                int day = int.Parse(yymmdd.Substring(4, 2));

                year += (year >= 30) ? 1900 : 2000;

                // Create the DateTime
                dob = new DateTime(year, month, day);
                return true;
            }
            catch
            {
                return false; // Parsing failed
            }
        }
        private void ClearFields()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtCurrentPassword.Text = string.Empty;
            txtNewPassword.Text = string.Empty;
            txtConfirmPassword.Text = string.Empty;
            txtEditICNumber.Text = string.Empty;
            txtEditPhone.Text = string.Empty;
            txtEditEmail.Text = string.Empty;
        }
        protected void btnClearImage_Click(object sender, EventArgs e)
        {
            imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";

            byte[] defaultAvatarData;
            string userID = GetUserID(); 
            string defaultImagePath = Server.MapPath("~/hospitalImg/defaultAvatar.jpg");
            defaultAvatarData = System.IO.File.ReadAllBytes(defaultImagePath);

            string userRole = Request.Cookies["Role"]?.Value.ToLower();

            string query = "";
            if (userRole == "doctor")
            {
                query = "UPDATE Doctor SET photo = @Photo WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "UPDATE Nurse SET photo = @Photo WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid user role.');", true);
                return;
            }

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", userID); 
                        command.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = defaultAvatarData;

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Image cleared successfully.');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
        }
        protected void btnSavePhoto_Click(object sender, EventArgs e)
        {
            string userID = GetUserID(); 

            if (string.IsNullOrEmpty(userID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User ID not found.');", true);
                return;
            }

            byte[] avatarData = null;

            if (FileUploadAvatar.HasFile)
            {
                try
                {
                    using (System.IO.Stream fs = FileUploadAvatar.PostedFile.InputStream)
                    {
                        using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                        {
                            avatarData = br.ReadBytes((int)fs.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error reading file: {ex.Message}');", true);
                    return;
                }
            }
            else
            {
                avatarData = null;
            }

            string query = "";
            string role = Request.Cookies["Role"]?.Value.ToLower();

            if (role == "doctor")
            {
                query = "UPDATE Doctor SET photo = @Photo WHERE doctorID = @UserID";
            }
            else if (role == "nurse")
            {
                query = "UPDATE Nurse SET photo = @Photo WHERE nurseID = @UserID";
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid role.');", true);
                return;
            }

            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        if (avatarData != null)
                        {
                            cmd.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = avatarData;
                        }

                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Photo updated successfully.');", true);
                            imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update photo.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
            }
            LoadUserImage();
        }
        private void LoadUserImage()
        {
            string userID = GetUserID();
            string role = Request.Cookies["Role"]?.Value.ToLower(); 

            if (string.IsNullOrEmpty(userID) || string.IsNullOrEmpty(role))
            {
                imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            byte[] imageData = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "";

                if (role == "doctor")
                {
                    query = "SELECT photo FROM Doctor WHERE doctorID = @UserID";
                }
                else if (role == "nurse")
                {
                    query = "SELECT photo FROM Nurse WHERE nurseID = @UserID";
                }
                else
                {
                    imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                    return;
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID); 

                    try
                    {
                        connection.Open();
                        object result = command.ExecuteScalar();

                        if (result != DBNull.Value && result != null)
                        {
                            imageData = (byte[])result;
                            string base64Image = Convert.ToBase64String(imageData);
                            imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64Image;
                        }
                        else
                        {
                            imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                        }
                    }
                    catch (Exception ex)
                    {
                        imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                        Console.WriteLine("Error loading image: " + ex.Message);
                    }
                }
            }
        }
    }
}