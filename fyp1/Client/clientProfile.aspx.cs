using ClosedXML.Excel;
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
            panelAddContact.Visible = false;
            if (Request.Cookies["Username"] == null)
            {
                Response.Redirect("clientLogin.aspx");
                return;
            }


            if (!IsPostBack)
            {
                ShowTabContent(ActiveTab);
                SetFieldsReadOnly(true);
                LoadProfileData();
                LoadAppointmentData();
                BindEmergencyGrid();
                LoadPaymentData();
                lblErrorMessage.Text = "";
            }
        }

        protected void btnHome_Click(object sender, EventArgs e)
        {
            Response.Redirect("clientHome.aspx");
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
            HttpCookie IDCookie = Request.Cookies["PatientID"];
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
            HttpCookie cookie = Request.Cookies["Username"];

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
            HttpCookie IDCookie = Request.Cookies["PatientID"];
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
                        patientID = @patientID
                    ORDER BY 
                        a.appointmentID DESC";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@patientID", patientID);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    gridAppointment.DataSource = dt;
                    gridAppointment.DataBind();
                }
                else
                {
                    gridAppointment.DataSource = null;
                    gridAppointment.DataBind();
                    lblNoAppointments.Text = "No appointments found. Please schedule a new appointment.";
                    lblNoAppointments.Visible = true;
                }
            }
        }

        private void RedirectToPayment(string appointmentID)
        {
            if (!string.IsNullOrEmpty(appointmentID))
            {
                string query = @"
            SELECT 
                d.name AS DoctorName,
                av.availableDate,
                av.availableFrom,
                av.availableTo
            FROM 
                Appointment a
            JOIN 
                Doctor d ON a.doctorID = d.doctorID
            JOIN 
                Availability av ON a.availabilityID = av.availabilityID
            WHERE 
                a.appointmentID = @appointmentID";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        string doctorName = reader["DoctorName"].ToString();
                        DateTime appointmentDate = (DateTime)reader["availableDate"];
                        TimeSpan fromTimeSpan = reader["availableFrom"] != DBNull.Value ? (TimeSpan)reader["availableFrom"] : TimeSpan.Zero;
                        TimeSpan toTimeSpan = reader["availableTo"] != DBNull.Value ? (TimeSpan)reader["availableTo"] : TimeSpan.Zero;

                        // Combine the date and time
                        DateTime fromTime = appointmentDate.Add(fromTimeSpan);
                        DateTime toTime = appointmentDate.Add(toTimeSpan);

                        // Construct the redirection URL
                        string redirectUrl = $"clientPayment.aspx?doctorName={HttpUtility.UrlEncode(doctorName)}" +
                                             $"&appointmentDate={HttpUtility.UrlEncode(appointmentDate.ToString("yyyy-MM-dd"))}" +
                                             $"&appointmentTime={HttpUtility.UrlEncode($"{fromTime:hh:mm tt} - {toTime:hh:mm tt}")}" +
                                             $"&appointmentID={appointmentID}";

                        Response.Redirect(redirectUrl);
                    }
                    else
                    {
                        lblErrorMessage.Text = "Appointment details could not be retrieved for payment.";
                    }

                    reader.Close();
                }
            }
            else
            {
                lblErrorMessage.Text = "Appointment ID is missing.";
            }
        }

        protected void gridAppointment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string paymentStatus = DataBinder.Eval(e.Row.DataItem, "paymentAmount")?.ToString();

                Button btnPay = (Button)e.Row.FindControl("btnPay");

                // Show or hide the Pay button based on the payment status
                if (!string.IsNullOrEmpty(paymentStatus) && decimal.TryParse(paymentStatus, out decimal payment) && payment > 0)
                {
                    btnPay.Visible = false;
                }
                else
                {
                    btnPay.Visible = true;
                }
            }
        }

        protected void gridAppointment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditAppointment")
            {
                // Redirect to the Edit Appointment page with the selected AppointmentID
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect($"EditAppointment.aspx?AppointmentID={appointmentID}");
            }
            else if (e.CommandName == "PayAppointment")
            {
                // Redirect to the Payment page with the selected AppointmentID
                string appointmentID = e.CommandArgument.ToString();
                RedirectToPayment(appointmentID);
            }
        }


        protected void btnAddAppointment_Click(object sender, EventArgs e)
        {
            Response.Redirect("BranchDoctorSelection.aspx");
        }

        protected void BindEmergencyGrid()
        {
            HttpCookie IDCookie = Request.Cookies["PatientID"];
            string patientID = IDCookie?.Value;
            string query = "SELECT ContactID, ContactName, Relationship, Phone, Email FROM EmergencyContact WHERE PatientID = @patientID";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                gridEmergency.DataSource = dt;
                gridEmergency.DataBind();
            }
        }
        protected void gridEmergency_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gridEmergency.EditIndex = e.NewEditIndex;
            BindEmergencyGrid();
        }
        protected void gridEmergency_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gridEmergency.EditIndex = -1;
            BindEmergencyGrid();
        }
        protected void gridEmergency_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            string contactID = gridEmergency.DataKeys[e.RowIndex].Value.ToString();

            string newName = null;
            TextBox txtContactName = (TextBox)gridEmergency.Rows[e.RowIndex].FindControl("txtContactName");
            if (txtContactName != null)
            {
                newName = txtContactName.Text;
                System.Diagnostics.Debug.WriteLine($"newName: {newName}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("txtContactName not found.");
            }

            string newRelationship = null;
            TextBox txtRelationship = (TextBox)gridEmergency.Rows[e.RowIndex].FindControl("txtRelationship");
            if (txtRelationship != null)
            {
                newRelationship = txtRelationship.Text;
                System.Diagnostics.Debug.WriteLine($"newRelationship: {newRelationship}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("txtRelationship not found.");
            }

            string newPhone = null;
            TextBox txtPhone = (TextBox)gridEmergency.Rows[e.RowIndex].FindControl("txtPhone");
            if (txtPhone != null)
            {
                newPhone = txtPhone.Text;
                System.Diagnostics.Debug.WriteLine($"newPhone: {newPhone}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("txtPhone not found.");
            }

            string newEmail = null;
            TextBox txtEmail = (TextBox)gridEmergency.Rows[e.RowIndex].FindControl("txtEmail");
            if (txtEmail != null)
            {
                newEmail = txtEmail.Text;
                System.Diagnostics.Debug.WriteLine($"newEmail: {newEmail}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("txtEmail not found.");
            }

            // Ensure that newName is not null or empty
            if (string.IsNullOrWhiteSpace(newName))
            {
                System.Diagnostics.Debug.WriteLine("ContactName cannot be null or empty.");
                Response.Write("<script>alert('ContactName cannot be null or empty.');</script>");
                return;
            }

            // Update contact
            UpdateContact(contactID, newName, newRelationship, newPhone, newEmail);

            // Exit edit mode and rebind the grid
            gridEmergency.EditIndex = -1;
            BindEmergencyGrid();
            System.Diagnostics.Debug.WriteLine("RowUpdating event fired.");
        }
        private void UpdateContact(string contactID, string newName, string newRelationship, string newPhone, string newEmail)
        {
            HttpCookie CookieID = Request.Cookies["PatientID"];
            string patientID = CookieID.Value;

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // Update query to set all fields
                    string query = "UPDATE EmergencyContact SET ContactName = @ContactName, Relationship = @Relationship, Phone = @Phone, Email = @Email WHERE ContactID = @ContactID AND PatientID = @PatientID";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    // Add parameters
                    cmd.Parameters.AddWithValue("@ContactName", newName ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Relationship", newRelationship ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Phone", newPhone ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Email", newEmail ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ContactID", contactID);
                    cmd.Parameters.AddWithValue("@PatientID", patientID);

                    // Execute query
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Contact updated successfully.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("No rows affected. Update failed.");
                    }
                }
            }
            catch (SqlException ex)
            {
                // Log the exception details
                System.Diagnostics.Debug.WriteLine($"SQL Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"SQL Error Number: {ex.Number}");
                System.Diagnostics.Debug.WriteLine($"SQL Error State: {ex.State}");
                System.Diagnostics.Debug.WriteLine($"SQL Error Procedure: {ex.Procedure}");
                System.Diagnostics.Debug.WriteLine($"SQL Error Line Number: {ex.LineNumber}");

                // Handle exceptions
                Response.Write($"<script>alert('SQL Error: {ex.Message}');</script>");
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                Response.Write($"<script>alert('Error: {ex.Message}');</script>");
            }
        }

        protected void gridEmergency_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Get the ContactID of the row being deleted
            string contactID = gridEmergency.DataKeys[e.RowIndex].Value.ToString();

            // Perform the delete query
            DeleteContact(contactID);

            // Rebind the GridView to reflect the changes
            BindEmergencyGrid();
        }

        private void DeleteContact(string contactID)
        {
            try
            {
                string query = "DELETE FROM EmergencyContact WHERE ContactID = @ContactID";

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@ContactID", contactID);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Response.Write($"<script>alert('Error: {ex.Message}');</script>");
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
            // Make all textboxes editable
            txtName.ReadOnly = false;
            txtDOB.ReadOnly = false;
            txtEmail.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtBloodType.ReadOnly = false;

            // Show Save and Cancel buttons
            btnSave.CssClass = "bg-green-500 text-white py-2 px-4 rounded";
            btnCancel.CssClass = "bg-gray-500 text-white py-2 px-4 rounded";

            // Hide the Edit button
            btnEdit.CssClass = "hidden";
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                HttpCookie IDCookie = HttpContext.Current.Request.Cookies["patientID"];
                string patientID = IDCookie.Value;

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
                    btnSave.CssClass = "hidden";
                    btnCancel.CssClass = "hidden";
                    btnEdit.CssClass = "bg-blue-500 text-white py-2 px-4 rounded";

                    // Optionally, show a success message
                    Response.Write("<script>alert('Profile updated successfully!');</script>");
                }
                else
                {
                    Response.Write("<script>alert('Patient ID is missing or invalid.');</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Error: {ex.Message}');</script>");
            }
        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadProfileData();
            SetFieldsReadOnly(true);
            btnSave.CssClass = "hidden";
            btnCancel.CssClass = "hidden";
            btnEdit.CssClass = "bg-blue-500 text-white py-2 px-4 rounded";
        }
        private void SetFieldsReadOnly(bool isReadOnly)
        {
            txtName.ReadOnly = isReadOnly;
            txtDOB.ReadOnly = isReadOnly;
            txtEmail.ReadOnly = isReadOnly;
            txtPhone.ReadOnly = isReadOnly;
            txtBloodType.ReadOnly = isReadOnly;
        }
        protected void btnAddContact_Click(object sender, EventArgs e)
        {
            panelAddContact.Visible = true;
        }
        protected void btnSaveContact_Click(object sender, EventArgs e)
        {
            string contactID = GenerateNextContactID();
            string name = txtNewName.Text;
            string relationship = txtNewRelationship.Text;
            string phone = txtNewPhone.Text;
            string email = txtNewEmail.Text;
            HttpCookie IDCookie = Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                string query = "INSERT INTO EmergencyContact (ContactID, patientID, ContactName, Relationship, Phone, Email) VALUES (@ContactID, @PatientID, @ContactName, @Relationship, @Phone, @Email)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ContactID", contactID);
                cmd.Parameters.AddWithValue("@PatientID", patientID);
                cmd.Parameters.AddWithValue("@ContactName", name);
                cmd.Parameters.AddWithValue("@Relationship", relationship);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }

            panelAddContact.Visible = false;
            BindEmergencyGrid();
            txtNewName.Text = string.Empty;
            txtNewRelationship.Text = string.Empty;
            txtNewPhone.Text = string.Empty;
            txtNewEmail.Text = string.Empty;
        }

        private string GenerateNextContactID()
        {
            string nextContactID = "EC0001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(ContactID) FROM EmergencyContact", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(2)) + 1;
                            nextContactID = "EC" + idNumber.ToString("D4");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextContactID;
        }
    }
}
