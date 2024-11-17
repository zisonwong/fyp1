using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalAddNurse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateBranchDropdown();
                GenerateNurseID();
                PopulateGenderDropdown();
                PopulateStatusDropdown();
            }
        }
        private void GenerateNurseID()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT TOP 1 nurseID FROM Nurse ORDER BY nurseID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                string newNurseID = "NS001";
                if (result != null)
                {
                    // Increment ID (assuming format DT001, DT002, etc.)
                    int lastID = int.Parse(result.ToString().Substring(2));
                    newNurseID = "NS" + (lastID + 1).ToString("D3");
                }

                txtEmployeeId.Text = newNurseID; // Display new ID in textbox
            }
        }
        private void PopulateBranchDropdown()
        {
            // Populate Department dropdown from Department table
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT branchID, name FROM Branch";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                ddlBranchId.DataSource = reader;
                ddlBranchId.DataTextField = "name";
                ddlBranchId.DataValueField = "branchID";
                ddlBranchId.DataBind();

                ddlBranchId.Items.Insert(0, new ListItem("- Select -", ""));
            }
        }

        protected void btnClearImage_Click(object sender, EventArgs e)
        {
            // Clear the image display and reset the FileUpload control
            imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; // Replace with your default image path
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

        protected void btnConfirmAddNurse_Click(object sender, EventArgs e)
        {
            txtRole.Text = "Nurse";
            string nurseID = txtEmployeeId.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string fullName = firstName + " " + lastName;
            string dob = txtDateOfBirth.Text.Trim();
            string icNumber = txtIc.Text.Trim();
            string gender = ddlGender.SelectedValue; // "M" or "F"
            string contactInfo = txtContactInfo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string branchID = ddlBranchId.SelectedValue;
            string role = txtRole.Text.Trim();
            string status = ddlStatus.SelectedValue; // "Activated", "Deactivated"
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

            if (IsEmailExists(email))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Email Exists",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('The email address already exists in the " +
                    "system. Please use a different email.'));",
                    true);
                return;
            }

            // Check if an avatar is uploaded
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
                // Load the default avatar image into a byte array if no file is uploaded
                string defaultImagePath = Server.MapPath("~/hospitalImg/defaultAvatar.jpg");
                avatarData = System.IO.File.ReadAllBytes(defaultImagePath);
            }

            string hashedPassword = HashPassword(password);

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Nurse (nurseID, name, DOB, ICNumber, gender, contactInfo, email, password, branchID, role, status, photo) " +
                               "VALUES (@NurseID, @Name, @DOB, @ICNumber, @Gender, @ContactInfo, @Email, @Password, @branchID, @Role, @Status, @Photo)";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@NurseID", nurseID);
                    command.Parameters.AddWithValue("@Name", fullName);
                    command.Parameters.AddWithValue("@DOB", dob);
                    command.Parameters.AddWithValue("@ICNumber", icNumber);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@BranchID", branchID);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Status", status);

                    // Add the avatar data (either the uploaded image or the default image)
                    command.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = avatarData;

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Response.Write("<script>alert('Nurse added successfully.');</script>");
                        ClearSection();
                        GenerateNurseID();
                    }
                    else
                    {
                        Response.Write("<script>alert('Error adding nurse.');</script>");
                    }
                }
            }
        }
        private void ClearSection()
        {
            txtFirstName.Text = string.Empty;
            txtLastName.Text = string.Empty;
            txtDateOfBirth.Text = string.Empty;
            txtIc.Text = string.Empty;
            ddlGender.SelectedIndex = 0;
            txtContactInfo.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtPassword.Text = string.Empty;
            ddlBranchId.SelectedIndex = 0;
            txtRole.Text = string.Empty;
            ddlStatus.SelectedIndex = 0;
            FileUploadAvatar.Attributes.Clear();
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
                   !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                   ddlBranchId.SelectedIndex > 0 &&
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
        private bool IsEmailExists(string email)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Use UNION to check both Doctor and Nurse tables for the email
                string query = @"
            SELECT COUNT(*) FROM (
                SELECT email FROM Doctor WHERE email = @Email
                UNION
                SELECT email FROM Nurse WHERE email = @Email
            ) AS EmailCheck";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2")); // Convert byte to hexadecimal
                }
                return builder.ToString();
            }
        }
    }
}