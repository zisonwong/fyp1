using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
                PopulatePrimaryDepartmentDropdown();
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
                string query = @"SELECT d.doctorID, d.name, d.DOB, d.ICNumber, d.gender, d.contactInfo, d.email, d.password,
         d.role, d.status, d.photo, dd.departmentID, dep.name AS departmentName
         FROM Doctor d 
         LEFT JOIN DoctorDepartment dd ON d.doctorID = dd.doctorID
         LEFT JOIN Department dep ON dd.departmentID = dep.departmentID
         WHERE d.doctorID = @DoctorID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                List<Department> departments = new List<Department>();
                string fullName = "";
                string doctorIDFromDb = "";
                DateTime? dob = null;
                string icNumber = "";
                string gender = "";
                string contactInfo = "";
                string email = "";
                string password = "";
                string role = "";
                string status = "";
                byte[] photo = null;
                List<string> doctorDepartmentIDs = new List<string>(); 

                while (reader.Read())
                {
                    fullName = reader["name"].ToString();
                    doctorIDFromDb = reader["doctorID"].ToString();
                    dob = reader["DOB"] != DBNull.Value ? Convert.ToDateTime(reader["DOB"]) : (DateTime?)null;
                    icNumber = reader["ICNumber"].ToString();
                    gender = reader["gender"].ToString();
                    contactInfo = reader["contactInfo"].ToString();
                    email = reader["email"].ToString();
                    password = reader["password"].ToString();  
                    role = reader["role"].ToString();
                    status = reader["status"].ToString();
                    photo = reader["photo"] as byte[]; 
                    string departmentID = reader["departmentID"].ToString(); 

                    doctorDepartmentIDs.Add(departmentID); 

                    departments.Add(new Department
                    {
                        DepartmentID = departmentID,
                        DepartmentName = reader["departmentName"].ToString()
                    });
                }
                reader.Close();  

                string[] nameParts = fullName.Split(' ');

                txtLastName.Text = nameParts.Length > 0 ? nameParts[0] : "";
                txtFirstName.Text = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";
                txtEmployeeId.Text = doctorIDFromDb;
                txtDateOfBirth.Text = dob.HasValue ? dob.Value.ToString("yyyy-MM-dd") : "";
                txtIc.Text = icNumber;
                ddlGender.SelectedValue = gender;
                txtContactInfo.Text = contactInfo;
                txtEmail.Text = email;
                txtPassword.Attributes["value"] = "***********";
                txtRole.Text = role;
                ddlStatus.SelectedValue = status;

                PopulatePrimaryDepartmentDropdown();  

                if (doctorDepartmentIDs.Count > 0)
                {
                    ddlPrimaryDepartment.SelectedValue = doctorDepartmentIDs[0];  
                }

                PopulateSecondaryDepartmentDropdown(doctorDepartmentIDs.Count > 0 ? doctorDepartmentIDs[0] : "");

                if (doctorDepartmentIDs.Count > 1)
                {
                    ddlSecondaryDepartment.SelectedValue = doctorDepartmentIDs[1];  
                }

                // Handle photo
                if (photo != null)
                {
                    string base64String = Convert.ToBase64String(photo);
                    imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64String;
                }
                else
                {
                    imgAvatar.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                }
            }
        }



        private void PopulatePrimaryDepartmentDropdown()
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT d.departmentID, 
                   d.name + ' - ' + b.name AS DisplayText
            FROM Department d
            LEFT JOIN Branch b ON d.branchID = b.branchID
            WHERE d.status = 'Activated'";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    ddlPrimaryDepartment.DataSource = reader;
                    ddlPrimaryDepartment.DataTextField = "DisplayText";
                    ddlPrimaryDepartment.DataValueField = "departmentID";
                    ddlPrimaryDepartment.DataBind();
                }
                else
                {
                    // If no data, add default item
                    ddlPrimaryDepartment.Items.Clear();
                    ddlPrimaryDepartment.Items.Add(new ListItem("- No departments available -", ""));
                }

                // Add a default "Select" option
                ddlPrimaryDepartment.Items.Insert(0, new ListItem("- Select -", ""));
            }
        }

        private void PopulateSecondaryDepartmentDropdown(string excludedDepartmentId)
        {
            string connString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                string query = @"
            SELECT d.departmentID, 
                   d.name + ' - ' + b.name AS DisplayText
            FROM Department d
            LEFT JOIN Branch b ON d.branchID = b.branchID
            WHERE d.departmentID != @ExcludedDepartmentID AND d.status = 'Activated'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ExcludedDepartmentID", excludedDepartmentId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    ddlSecondaryDepartment.DataSource = reader;
                    ddlSecondaryDepartment.DataTextField = "DisplayText";
                    ddlSecondaryDepartment.DataValueField = "departmentID";
                    ddlSecondaryDepartment.DataBind();
                }
                else
                {
                    // If no data, add default item
                    ddlSecondaryDepartment.Items.Clear();
                    ddlSecondaryDepartment.Items.Add(new ListItem("- No departments available -", ""));
                }

                // Add a default "Select" option
                ddlSecondaryDepartment.Items.Insert(0, new ListItem("- Select -", ""));
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
            string gender = ddlGender.SelectedValue; 
            string contactInfo = txtContactInfo.Text.Trim();
            string email = txtEmail.Text.Trim();
            string primaryDepartmentID = ddlPrimaryDepartment.SelectedValue; 
            string secondaryDepartmentID = ddlSecondaryDepartment.SelectedValue; 
            string role = txtRole.Text.Trim();
            string status = ddlStatus.SelectedValue; 
            byte[] avatarData = null; 

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
            if (primaryDepartmentID == secondaryDepartmentID)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Department Error",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('First and Second Department cannot be the same.'));",
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
                avatarData = null;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Doctor SET name = @Name, DOB = @DOB, ICNumber = @ICNumber, gender = @Gender, " +
                               "contactInfo = @ContactInfo, email = @Email, role = @Role, status = @Status";

                if (avatarData != null)
                {
                    query += ", photo = @Photo";
                }

                query += " WHERE doctorID = @DoctorID"; 

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@DoctorID", doctorID);
                    command.Parameters.AddWithValue("@Name", fullName);
                    command.Parameters.AddWithValue("@DOB", dob);
                    command.Parameters.AddWithValue("@ICNumber", icNumber);
                    command.Parameters.AddWithValue("@Gender", gender);
                    command.Parameters.AddWithValue("@ContactInfo", contactInfo);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@Role", role);
                    command.Parameters.AddWithValue("@Status", status);

                    if (avatarData != null)
                    {
                        command.Parameters.Add("@Photo", SqlDbType.VarBinary).Value = avatarData;
                    }

                    if (txtPassword.Text.Trim() != "***********")
                    {
                        command.Parameters.AddWithValue("@Password", txtPassword.Text.Trim());
                    }

                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        UpdateDoctorDepartments(doctorID, primaryDepartmentID, secondaryDepartmentID);
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

        private void UpdateDoctorDepartments(string doctorID, string primaryDepartmentID, string secondaryDepartmentID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                List<string> currentDepartments = new List<string>();
                string currentDepartmentsQuery = "SELECT departmentID FROM DoctorDepartment WHERE doctorID = @DoctorID";

                using (SqlCommand currentDepartmentsCommand = new SqlCommand(currentDepartmentsQuery, connection))
                {
                    currentDepartmentsCommand.Parameters.AddWithValue("@DoctorID", doctorID);
                    using (SqlDataReader reader = currentDepartmentsCommand.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentDepartments.Add(reader.GetString(0));
                        }
                    }
                }

                if (!string.IsNullOrEmpty(primaryDepartmentID))
                {
                    if (currentDepartments.Contains(primaryDepartmentID))
                    {
                        Console.WriteLine("Primary department exists, no action taken.");
                    }
                    else
                    {
                        string insertPrimaryQuery = "INSERT INTO DoctorDepartment (doctorID, departmentID) VALUES (@DoctorID, @PrimaryDepartmentID)";
                        using (SqlCommand command = new SqlCommand(insertPrimaryQuery, connection))
                        {
                            command.Parameters.AddWithValue("@DoctorID", doctorID);
                            command.Parameters.AddWithValue("@PrimaryDepartmentID", primaryDepartmentID);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                // Handle the secondary department
                if (!string.IsNullOrEmpty(secondaryDepartmentID))
                {
                    if (currentDepartments.Contains(secondaryDepartmentID))
                    {
                        Console.WriteLine("Secondary department exists, no action taken.");
                    }
                    else
                    {
                        string insertSecondaryQuery = "INSERT INTO DoctorDepartment (doctorID, departmentID) VALUES (@DoctorID, @SecondaryDepartmentID)";
                        using (SqlCommand command = new SqlCommand(insertSecondaryQuery, connection))
                        {
                            command.Parameters.AddWithValue("@DoctorID", doctorID);
                            command.Parameters.AddWithValue("@SecondaryDepartmentID", secondaryDepartmentID);
                            command.ExecuteNonQuery();
                        }
                    }
                }

                List<string> selectedDepartments = new List<string> { primaryDepartmentID, secondaryDepartmentID };

                foreach (var dept in currentDepartments)
                {
                    if (!selectedDepartments.Contains(dept))
                    {
                        string deleteQuery = "DELETE FROM DoctorDepartment WHERE doctorID = @DoctorID AND departmentID = @DepartmentID";
                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@DoctorID", doctorID);
                            command.Parameters.AddWithValue("@DepartmentID", dept);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
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
                   (ddlPrimaryDepartment.SelectedIndex > 0 || ddlSecondaryDepartment.SelectedIndex > 0) &&
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
    public class Department
    {
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }
    }

}
