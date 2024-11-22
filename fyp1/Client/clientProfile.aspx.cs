using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientProfile : System.Web.UI.Page
    {
        protected string ActiveTab { get; set; } = "profile";

        protected void Page_Load(object sender, EventArgs e)
        {

            if (Request.Cookies["Username"] == null)
            {
                Response.Redirect("clientLogin.aspx");
            }
            else
            {
                Response.Redirect("clientProfile.aspx");
            }



            if (!IsPostBack)
            {
                ShowTabContent(ActiveTab);
                SetFieldsReadOnly(true);
                LoadProfileData();
                LoadAppointmentData();
                LoadEmergencyContactData();
                LoadPaymentData();
                //LoadSettingsData();
                
                lblErrorMessage.Text = "";
            }
        }

        protected void SelectTab(object sender, EventArgs e)
        {
            var button = sender as Button;
            ActiveTab = button.CommandArgument;
            ShowTabContent(ActiveTab);
        }

        private void ShowTabContent(string tab)
        {
            panelProfile.CssClass = "tab-content hidden";
            panelAppointment.CssClass = "tab-content hidden";
            panelEmergency.CssClass = "tab-content hidden";
            panelPayment.CssClass = "tab-content hidden";
            panelSettings.CssClass = "tab-content hidden";

            switch (tab)
            {
                case "profile":
                    panelProfile.CssClass = "tab-content";
                    break;
                case "appointment":
                    panelAppointment.CssClass = "tab-content";
                    break;
                case "emergency":
                    panelEmergency.CssClass = "tab-content";
                    break;
                case "payment":
                    panelPayment.CssClass = "tab-content";
                    break;
                case "settings":
                    panelSettings.CssClass = "tab-content";
                    break;
            }
        }

        protected string TabCssClass(string tabName)
        {
            return tabName == ActiveTab ? "tab-link block w-full text-left px-4 py-2 rounded bg-blue-100" : "tab-link block w-full text-left px-4 py-2 rounded hover:bg-blue-100";
        }

        private void LoadProfileData()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string query = "SELECT name, DOB, email, contactInfo,bloodType,photo, ICNumber FROM Patient WHERE patientID = @PatientID";

            string patientID = IDCookie.Value;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@PatientID", patientID);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["name"].ToString();
                            txtDOB.Text = Convert.ToDateTime(reader["DOB"]).ToString("MM/dd/yyyy");
                            txtEmail.Text = reader["email"].ToString();
                            txtPhone.Text = reader["contactInfo"].ToString();
                            txtBloodType.Text = reader["bloodType"].ToString();
                            lblName.InnerText = reader["name"].ToString();
                            lblIC.Text = reader["ICNumber"].ToString();

                            if (reader["photo"] != DBNull.Value)
                            {
                                byte[] photoData = (byte[])reader["photo"];
                                string base64String = Convert.ToBase64String(photoData, 0, photoData.Length);
                                imgProfilePicture.ImageUrl = "data:image/png;base64," + base64String;
                            }
                            else
                            {
                                imgProfilePicture.ImageUrl = "https://via.placeholder.com/100"; // Placeholder image
                            }
                        }
                    }
                }
            }
        }

        protected void signOut()
        {
            FormsAuthentication.SignOut();
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Username"];

            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }

            Response.Redirect("clientHome.aspx");
        }

        protected void signOutBtn_Click(object sender, EventArgs e)
        {
            signOut();
        }

        private void LoadPatientData()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                string query = "SELECT * FROM Patient WHERE patientID = @patientID";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@patientID", patientID);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtName.Text = reader["name"].ToString();
                            txtDOB.Text = Convert.ToDateTime(reader["DOB"]).ToString("dd-MM-yyyy");
                            txtEmail.Text = reader["email"].ToString();
                            txtPhone.Text = reader["contactInfo"].ToString();
                            txtBloodType.Text = reader["bloodtype"].ToString();
                            lblIC.Text = reader["ICNumber"].ToString();
                            byte[] imageBytes = reader["photo"] as byte[];
                            if (imageBytes != null)
                            {
                                string base64String = Convert.ToBase64String(imageBytes, 0, imageBytes.Length);
                                imgProfilePicture.ImageUrl = "data:image/jpeg;base64," + base64String;
                            }
                        }
                    }
                }
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (fileUploadProfile.HasFile)
            {
                string patientID = Request.Cookies["PatientID"]?.Value;
                byte[] uploadedFile = fileUploadProfile.FileBytes;
                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    string updateQuery = "UPDATE Patient SET photo = @photo WHERE patientID = @patientID";

                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        cmd.Parameters.AddWithValue("@photo", uploadedFile);
                        cmd.Parameters.AddWithValue("@patientID", patientID);
                        cmd.ExecuteNonQuery();
                    }
                }
                LoadPatientData();
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            string currentPassword = txtCurrentPassword.Text;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            // Validate new password confirmation
            if (newPassword != confirmPassword)
            {
                lblErrorMessage.Text = "Passwords do not match!";
                return;
            }

            // Validate new password strength
            if (!ValidateNewPassword(newPassword))
            {
                lblErrorMessage.Text = "New password must be at least 8 characters, contain one uppercase letter, one special character, and one number.";
                return;
            }

            // Retrieve the logged-in patient ID from session
            string patientId = Request.Cookies["PatientID"]?.Value;
            if (string.IsNullOrEmpty(patientId))
            {
                lblErrorMessage.Text = "Patient ID not found in cookies!";
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string currentStoredPassword;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Retrieve the stored hashed password
                using (SqlCommand cmd = new SqlCommand("SELECT password FROM Patient WHERE patientID = @patientID", conn))
                {
                    cmd.Parameters.AddWithValue("@patientID", patientId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentStoredPassword = result.ToString();
                    }
                    else
                    {
                        lblErrorMessage.Text = "User not found!";
                        return;
                    }
                }

                // Compare hashed version of input current password with the stored hashed password
                if (!VerifyHashedPassword(currentStoredPassword, currentPassword))
                {
                    lblErrorMessage.Text = "The current password is incorrect!";
                    return;
                }

                // Hash the new password
                string hashedNewPassword = HashPassword(newPassword);

                // Update the password in the database
                using (SqlCommand cmd = new SqlCommand("UPDATE Patient SET password = @newPassword WHERE patientID = @patientID", conn))
                {
                    cmd.Parameters.AddWithValue("@newPassword", hashedNewPassword);
                    cmd.Parameters.AddWithValue("@patientID", patientId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        lblErrorMessage.Text = "Password successfully updated!";
                    }
                    else
                    {
                        lblErrorMessage.Text = "Error updating password!";
                    }
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

        private bool VerifyHashedPassword(string storedHash, string inputPassword)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return storedHash == hashedInputPassword;
        }

        private bool ValidateNewPassword(string newPassword)
        {
            return Regex.IsMatch(newPassword, @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
        }



        protected void btnNotificationSettings_Click(object sender, EventArgs e)
        {
            bool emailNotifications = chkEmailNotifications.Checked;
            bool smsNotifications = chkSMSNotifications.Checked;
            bool pushNotifications = chkPushNotifications.Checked;

            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            if (IDCookie == null || string.IsNullOrEmpty(IDCookie.Value))
            {
                lblErrorMessage.Text = "Error: Patient ID not found.";
                return;
            }
            string patientId = IDCookie.Value;

            using (SqlConnection conn = new SqlConnection("connectionString"))
            {
                conn.Open();
                string query = "UPDATE Patient SET emailNotifications = @Email, smsNotifications = @SMS, pushNotifications = @Push WHERE patientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", emailNotifications);
                    cmd.Parameters.AddWithValue("@SMS", smsNotifications);
                    cmd.Parameters.AddWithValue("@Push", pushNotifications);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    lblErrorMessage.Text = rowsAffected > 0 ? "Notification settings updated successfully!" : "Error updating notification settings!";
                }
            }
        }

        protected void btnPrivacySettings_Click(object sender, EventArgs e)
        {
            string profileVisibility = ddlProfileVisibility.SelectedValue;
            bool dataSharing = chkDataSharing.Checked;
            bool adPreferences = chkAdPreferences.Checked;

            // Obtain patientId from session, cookie, or other context
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            if (IDCookie == null || string.IsNullOrEmpty(IDCookie.Value))
            {
                lblErrorMessage.Text = "Error: Patient ID not found.";
                return;
            }
            string patientId = IDCookie.Value;

            using (SqlConnection conn = new SqlConnection("connectionString"))
            {
                conn.Open();
                string query = "UPDATE Patient SET profileVisibility = @Visibility, dataSharing = @Sharing, adPreferences = @Ad WHERE patientID = @PatientID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Visibility", profileVisibility);
                    cmd.Parameters.AddWithValue("@Sharing", dataSharing);
                    cmd.Parameters.AddWithValue("@Ad", adPreferences);
                    cmd.Parameters.AddWithValue("@PatientID", patientId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    lblErrorMessage.Text = rowsAffected > 0 ? "Privacy settings updated successfully!" : "Error updating privacy settings!";
                }
            }
        }
        private void LoadAppointmentData()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;
            string query = @"SELECT 
                                a.appointmentID,
                                d.name,
                                av.availableDate AS AppointmentDate,
                                av.availableFrom AS StartTime,
                                av.availableTo AS EndTime,
                                a.status,
                                pm.paymentAmount,
                                pm.paymentDate,
                                pm.paymentMethod
                            FROM 
                                Appointment a
                            JOIN 
                                Doctor d ON a.doctorID = d.doctorID
                            JOIN 
                                Availability av ON a.availabilityID = av.availabilityID
                            LEFT JOIN 
                                Payment pm ON a.paymentID = pm.paymentID
                            WHERE 
                                patientID = @patientID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@patientID", patientID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gridAppointment.DataSource = dt;
                gridAppointment.DataBind();
            }
        }

        private void LoadEmergencyContactData()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;
            string query = "SELECT ContactName, Relationship, Phone, Email FROM EmergencyContact WHERE patientID = @patientID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@patientID", patientID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gridEmergency.DataSource = dt;
                gridEmergency.DataBind();
            }
        }

        private void LoadPaymentData()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;
            string query = @"SELECT 
                            p.paymentID,
                            p.paymentAmount,
                            p.paymentMethod,
                            p.paymentDate
                        FROM
                            Payment p
                        JOIN
                            Appointment a ON p.paymentID = a.paymentID
                        WHERE
                            a.patientID = @patientID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@patientID", patientID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                gridPayment.DataSource = dt;
                gridPayment.DataBind();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            SetFieldsReadOnly(false); 
            btnEdit.CssClass = "hidden"; 
            btnSave.CssClass = "visible"; 
            btnCancel.CssClass = "visible"; 
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string patientID = Session["patientID"]?.ToString();

            if (!string.IsNullOrEmpty(patientID))
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    string query = "UPDATE Patient SET name = @name, DOB = @dob, email = @email, " +
                                   "contactInfo = @contactInfo, bloodtype = @bloodtype WHERE patientID = @patientID";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@dob", Convert.ToDateTime(txtDOB.Text));
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@contactInfo", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@bloodtype", txtBloodType.Text);
                    cmd.Parameters.AddWithValue("@patientID", patientID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }

                // After saving, revert to read-only mode
                SetFieldsReadOnly(true);
                btnEdit.CssClass = "visible";
                btnSave.CssClass = "hidden";
                btnCancel.CssClass = "hidden";

                // Optionally, show a success message
                Response.Write("<script>alert('Profile updated successfully!');</script>");
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadProfileData();
            SetFieldsReadOnly(true); 
            btnEdit.CssClass = "visible";
            btnSave.CssClass = "hidden";
            btnCancel.CssClass = "hidden";
        }

        private void SetFieldsReadOnly(bool isReadOnly)
        {
            txtName.ReadOnly = isReadOnly;
            txtDOB.ReadOnly = isReadOnly;
            txtEmail.ReadOnly = isReadOnly;
            txtPhone.ReadOnly = isReadOnly;
            txtBloodType.ReadOnly = isReadOnly;
        }
    }
}