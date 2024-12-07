using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Security.Cryptography;
using System.Text;

namespace fyp1.Admin
{
    public partial class hospitalAddDoctor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulatePrimaryDepartmentDropdown();
                GenerateDoctorID();
                PopulateGenderDropdown();
                PopulateStatusDropdown();
            }
        }
        private void GenerateDoctorID()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = "SELECT TOP 1 doctorID FROM Doctor ORDER BY doctorID DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                string newDoctorID = "DT001";
                if (result != null)
                {
                    // Increment ID (assuming format DT001, DT002, etc.)
                    int lastID = int.Parse(result.ToString().Substring(2));
                    newDoctorID = "DT" + (lastID + 1).ToString("D3");
                }

                txtEmployeeId.Text = newDoctorID; // Display new ID in textbox
            }
        }
        private void PopulatePrimaryDepartmentDropdown()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT d.departmentID, d.name, b.branchID, b.name AS branchName
            FROM Department d
            INNER JOIN Branch b ON d.branchID = b.branchID
            WHERE d.status = 'Activated'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataRow row in dt.Rows)
                {
                    row["name"] = $"{row["name"]} - {row["branchName"]}";
                }

                ddlDepartmentId.DataSource = dt;
                ddlDepartmentId.DataTextField = "name";
                ddlDepartmentId.DataValueField = "departmentID";
                ddlDepartmentId.DataBind();

                // Add a default "Select" option
                ddlDepartmentId.Items.Insert(0, new ListItem("- Select -", ""));
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

        protected void btnConfirmAddDoctor_Click(object sender, EventArgs e)
        {
            txtRole.Text = "Doctor";
            string doctorID = txtEmployeeId.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string lastName = txtLastName.Text.Trim();
            string fullName = lastName + " " + firstName;
            string dob = txtDateOfBirth.Text.Trim();
            string icNumber = txtIc.Text.Trim();
            string gender = ddlGender.SelectedValue; // "M" or "F"
            string contactInfo = txtContactInfo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string role = txtRole.Text.Trim();
            string status = ddlStatus.SelectedValue; 
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
                connection.Open();

                // Insert into Doctor table
                string insertDoctorQuery = "INSERT INTO Doctor (doctorID, name, DOB, ICNumber, gender, contactInfo, email, password, role, status, photo) " +
                                            "VALUES (@DoctorID, @Name, @DOB, @ICNumber, @Gender, @ContactInfo, @Email, @Password, @Role, @Status, @Photo)";
                using (SqlCommand doctorCommand = new SqlCommand(insertDoctorQuery, connection))
                {
                    doctorCommand.Parameters.AddWithValue("@DoctorID", doctorID);
                    doctorCommand.Parameters.AddWithValue("@Name", fullName);
                    doctorCommand.Parameters.AddWithValue("@DOB", dob);
                    doctorCommand.Parameters.AddWithValue("@ICNumber", icNumber);
                    doctorCommand.Parameters.AddWithValue("@Gender", gender);
                    doctorCommand.Parameters.AddWithValue("@ContactInfo", contactInfo);
                    doctorCommand.Parameters.AddWithValue("@Email", email);
                    doctorCommand.Parameters.AddWithValue("@Password", hashedPassword);
                    doctorCommand.Parameters.AddWithValue("@Role", role);
                    doctorCommand.Parameters.AddWithValue("@Status", status);
                    doctorCommand.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = avatarData;

                    doctorCommand.ExecuteNonQuery();
                }

                string insertDoctorDepartmentQuery = "INSERT INTO DoctorDepartment (doctorID, departmentID) VALUES (@DoctorID, @DepartmentID)";
                using (SqlCommand doctorDepartmentCommand = new SqlCommand(insertDoctorDepartmentQuery, connection))
                {
                    doctorDepartmentCommand.Parameters.AddWithValue("@DoctorID", doctorID);
                    doctorDepartmentCommand.Parameters.AddWithValue("@DepartmentID", ddlDepartmentId.SelectedValue);
                    doctorDepartmentCommand.ExecuteNonQuery();

                    if (!string.IsNullOrEmpty(ddlDepartmentId2.SelectedValue))
                    {
                        doctorDepartmentCommand.Parameters["@DepartmentID"].Value = ddlDepartmentId2.SelectedValue;
                        doctorDepartmentCommand.ExecuteNonQuery();
                    }
                }

                Response.Write("<script>alert('Doctor added successfully.');</script>");
                ClearSection();
                GenerateDoctorID();
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
            ddlDepartmentId.SelectedIndex = 0;
            ddlDepartmentId2.SelectedIndex = 0;
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
        protected void ddlDepartmentId_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDepartmentId.SelectedValue))
            {
                secondaryDepartmentContainer.Visible = true;

                PopulateSecondaryDepartmentDropdown(ddlDepartmentId.SelectedValue);
            }
            else
            {
                secondaryDepartmentContainer.Visible = false;
            }
        }
        private void PopulateSecondaryDepartmentDropdown(string excludedDepartmentId)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT d.departmentID, d.name, b.branchID, b.name AS branchName 
            FROM Department d
            INNER JOIN Branch b ON d.branchID = b.branchID
            WHERE d.departmentID != @ExcludedDepartmentID AND d.status = 'Activated'";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ExcludedDepartmentID", excludedDepartmentId);

                SqlDataReader reader = cmd.ExecuteReader();

                DataTable dt = new DataTable();
                dt.Load(reader);

                foreach (DataRow row in dt.Rows)
                {
                    row["name"] = $"{row["name"]} - {row["branchName"]}";
                }

                ddlDepartmentId2.DataSource = dt;
                ddlDepartmentId2.DataTextField = "name"; 
                ddlDepartmentId2.DataValueField = "departmentID"; 
                ddlDepartmentId2.DataBind();

                ddlDepartmentId2.Items.Insert(0, new ListItem("- Select -", ""));
            }
        }
        protected void txtDateOfBirth_TextChanged(object sender, EventArgs e)
        {
            if (DateTime.TryParse(txtDateOfBirth.Text, out DateTime selectedDate))
            {
                string formattedDate = selectedDate.ToString("yyMMdd");

                txtIc.Text = formattedDate;
            }
            else
            {
                txtIc.Text = string.Empty;
            }
        }
        protected void txtIc_TextChanged(object sender, EventArgs e)
        {
            string icNumber = txtIc.Text.Trim();

            if (icNumber.Length >= 6)
            {
                string yearPart = icNumber.Substring(0, 2);
                string monthPart = icNumber.Substring(2, 2);
                string dayPart = icNumber.Substring(4, 2);

                int year = int.Parse(yearPart) < 50 ? 2000 + int.Parse(yearPart) : 1900 + int.Parse(yearPart);

                if (DateTime.TryParse($"{year}-{monthPart}-{dayPart}", out DateTime dob))
                {
                    txtDateOfBirth.Text = dob.ToString("yyyy-MM-dd");
                }
                else
                {
                    txtDateOfBirth.Text = string.Empty;
                }
            }
            else
            {
                txtDateOfBirth.Text = string.Empty;
            }
        }

    }
}
