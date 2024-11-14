using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hosipitalDoctorEdit : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateDepartmentDropdown();
                PopulateGenderDropdown();
                PopulateStatusDropdown();

                string doctorID = Request.QueryString["doctorID"];
                if (!string.IsNullOrEmpty(doctorID))
                {
                    LoadDoctorDetails(doctorID);
                }
            }
        }

        private void LoadDoctorDetails(string doctorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT doctorID, name, DOB, ICNumber, gender, contactInfo, email, password,
                         departmentID, role, status, photo FROM Doctor WHERE doctorID = @DoctorID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    // Split full name into first and last name
                    // Split full name into first and last name
                    string fullName = reader["name"].ToString();
                    string[] nameParts = fullName.Split(' ');

                    // Assign the last name (the first part)
                    txtLastName.Text = nameParts.Length > 0 ? nameParts[0] : "";

                    // Join all parts (except the first one) for the first name
                    txtFirstName.Text = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                    txtEmployeeId.Text = reader["doctorID"].ToString();
                    txtDateOfBirth.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                    txtIc.Text = reader["ICNumber"].ToString();
                    // Set the selected value for Gender dropdown
                    ddlGender.SelectedValue = reader["gender"].ToString();
                    txtContactInfo.Text = reader["contactInfo"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtPassword.Attributes["value"] = "***********"; // Mask Password Field
                                                                     // Set the selected value for Department dropdown
                    ddlDepartmentId.SelectedValue = reader["departmentID"].ToString();
                    txtRole.Text = reader["role"].ToString();
                    // Set the selected value for Status dropdown
                    ddlStatus.SelectedValue = reader["status"].ToString();
                    // Convert the binary photo data to a Base64 string and set it as the image source
                    if (reader["photo"] != DBNull.Value)
                    {
                        byte[] photoData = (byte[])reader["photo"];
                        string base64String = Convert.ToBase64String(photoData);
                        imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64String;
                    }
                    else
                    {
                        imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; // Fallback to default image if no photo is available
                    }
                }
                reader.Close();
            }
        }
        private void PopulateDepartmentDropdown()
        {
            // Populate Department dropdown from Department table
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT departmentID, name FROM Department";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                ddlDepartmentId.DataSource = reader;
                ddlDepartmentId.DataTextField = "name";
                ddlDepartmentId.DataValueField = "departmentID";
                ddlDepartmentId.DataBind();

                ddlDepartmentId.Items.Insert(0, new ListItem("- Select -", ""));
            }
        }
        private void PopulateGenderDropdown()
        {
            ddlGender.Items.Clear();
            ddlGender.Items.Add(new ListItem("- Select -", ""));
            ddlGender.Items.Add(new ListItem("Male", "M"));
            ddlGender.Items.Add(new ListItem("Female", "F"));
        }

        private void PopulateStatusDropdown()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add(new ListItem("- Select -", ""));
            ddlStatus.Items.Add(new ListItem("Activate", "Activate"));
            ddlStatus.Items.Add(new ListItem("UnActivate", "UnActivate"));
        }

        protected void btnClearImage_Click(object sender, EventArgs e)
        {
            // Set the default avatar image in the UI
            imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";

            // Prepare the default avatar data to store in the database
            byte[] defaultAvatarData;
            string doctorID = txtEmployeeId.Text.Trim();
            string defaultImagePath = Server.MapPath("~/hospitalImg/defaultAvatar.jpg");
            defaultAvatarData = System.IO.File.ReadAllBytes(defaultImagePath);

            // Update the doctor's photo to the default avatar in the database
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Doctor SET photo = @Photo WHERE doctorID = @DoctorID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorID); // Use the doctor's ID here
                    command.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = defaultAvatarData;

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }

            // Notify the user (optional)
            Response.Write("<script>alert('Image cleared successfully.');</script>");
        }


        protected void btnConfirmEditDoctor_Click(object sender, EventArgs e)
        {
            string doctorID = txtEmployeeId.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string fullName = lastName + " " + firstName;
            string dob = txtDateOfBirth.Text.Trim();
            string icNumber = txtIc.Text.Trim();
            string gender = ddlGender.SelectedValue; // "M" or "F"
            string contactInfo = txtContactInfo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string departmentID = ddlDepartmentId.SelectedValue;
            string role = txtRole.Text.Trim();
            string status = ddlStatus.SelectedValue; // "Activated", "Deactivated", or "Deleted"
            byte[] avatarData = null; // To store the image binary data

            if (!CheckFields())
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Added Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }

            // Validate IC and DOB
            if (!ValidateICAndDOB(icNumber, dob))
            {
                // If validation fails, show an error and return
                Page.ClientScript.RegisterStartupScript(GetType(), "Invalid IC",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('IC Number does not match the date of " +
                    "birth or is in the wrong format.'));", true);
                return;
            }

            // Validate phone
            if (!ValidatePhone(contactInfo))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Invalid Phone",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Contact Info must only contain digits" +
                    " and be no longer than 11 digits.'));", true);
                return;
            }

            // Validate email
            if (!ValidateEmailFormat(email))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Invalid Email",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please Enter Valid Email'));",
                    true);
                return;
            }

            if (IsEmailExists(email, doctorID))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Email Exists",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('The email address already exists in the system. Please use a different email.'));",
                    true);
                return;
            }

            if (FileUploadAvatar.HasFile)
            {
                using (System.IO.Stream fs = FileUploadAvatar.PostedFile.InputStream)
                {
                    using (System.IO.BinaryReader br = new System.IO.BinaryReader(fs))
                    {
                        avatarData = br.ReadBytes((int)fs.Length);
                    }
                }
            }
            else
            {
                // No new photo uploaded, keep the current image in the database
                avatarData = null; 
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Dynamically build query based on whether password is "***********"
                string query = "UPDATE Doctor SET name = @Name, DOB = @DOB, ICNumber = @ICNumber, gender = @Gender, " +
                               "contactInfo = @ContactInfo, email = @Email, departmentID = @DepartmentID, role = @Role, " +
                               "status = @Status";
                
                // Add photo field only if a new photo is provided
                if (avatarData != null)
                {
                    query += ", photo = @Photo";
                }

                query += " WHERE doctorID = @DoctorID"; // Finalize the query

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorID);
                    command.Parameters.AddWithValue("@Name", fullName);
                    command.Parameters.AddWithValue("@DOB", dob);
                    command.Parameters.AddWithValue("@ICNumber", icNumber);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@DepartmentID", departmentID);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Status", status);

                    if (avatarData != null)
                    {
                        command.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = avatarData;
                    }

                    // Add the actual password only if it's not "***********"
                    if (txtPassword.Text.Trim() != "***********")
                    {
                        command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    }

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Doctor updated successfully.');</script>");
                    }
                    else
                    {
                        Response.Write("<script>alert('Error updating doctor.');</script>");
                    }
                }
            }
            LoadDoctorImage();
        }


        private bool CheckFields()
        {
            return !string.IsNullOrWhiteSpace(txtFirstName.Text) &&
                   !string.IsNullOrWhiteSpace(txtLastName.Text) &&
                   !string.IsNullOrWhiteSpace(txtDateOfBirth.Text) &&
                   !string.IsNullOrWhiteSpace(txtIc.Text) &&
                   ddlGender.SelectedIndex > 0 &&
                   !string.IsNullOrWhiteSpace(txtContactInfo.Text) &&
                   !string.IsNullOrWhiteSpace(txtEmail.Text) &&
                   ddlDepartmentId.SelectedIndex > 0 &&
                   ddlStatus.SelectedIndex > 0;
        }

        private bool ValidateICAndDOB(string icNumber, string dob)
        {
            // Check if IC matches the pattern YYMMDD-XXXXXXX (where only YYMMDD is validated here)
            var icPattern = @"^\d{6}-\d{2}-\d{4}$";  // Basic IC pattern with YYMMDD
            if (!Regex.IsMatch(icNumber, icPattern))
            {
                return false;
            }

            // Extract YYMMDD from IC
            string icDOBPart = icNumber.Substring(0, 6);

            // Parse the DOB to YYMMDD format
            DateTime dobDate;
            if (!DateTime.TryParse(dob, out dobDate))
            {
                return false;
            }
            string dobYYMMDD = dobDate.ToString("yyMMdd");

            // Check if IC YYMMDD matches DOB YYMMDD
            return icDOBPart == dobYYMMDD;
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
        private bool IsEmailExists(string email, string doctorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT COUNT(*) FROM (
                SELECT email FROM Doctor WHERE email = @Email AND doctorID != @DoctorID
                UNION ALL
                SELECT email FROM Nurse WHERE email = @Email
            ) AS EmailCheck";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@DoctorID", doctorID);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        // Get and display the doctor's image from the database
        private void LoadDoctorImage()
        {
            string doctorID = txtEmployeeId.Text.Trim();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT photo FROM Doctor WHERE doctorID = @DoctorID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorID); // Use the doctor's ID here

                    connection.Open();
                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value)
                    {
                        byte[] imageData = (byte[])result;
                        string base64Image = Convert.ToBase64String(imageData);
                        imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64Image;
                    }
                    else
                    {
                        // Set default image if no image data found
                        imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                    }
                }
            }
        }
    }
}
