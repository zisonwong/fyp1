using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1
{
    public partial class hospitalDoctorDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string doctorID = Request.QueryString["doctorID"];
                if (!string.IsNullOrEmpty(doctorID))
                {
                    LoadDoctorDetails(doctorID);
                    btnEdit.CommandArgument = doctorID;
                }
                else
                {
                    lblDoctorName.Text = "Doctor not found";
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
                    lblDoctorName.Text = $"Dr. {reader["name"]}";

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
                    txtGender.Text = reader["gender"].ToString() == "M" ? "Male" : "Female";
                    txtContactInfo.Text = reader["contactInfo"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtPassword.Attributes["value"] = "***********";
                    txtDepartmentId.Text = reader["departmentID"].ToString();
                    txtRole.Text = reader["role"].ToString();
                    txtStatus.Text = reader["status"].ToString();

                    // Convert the binary photo data to a Base64 string and set it as the image source
                    if (reader["photo"] != DBNull.Value)
                    {
                        byte[] photoData = (byte[])reader["photo"];
                        string base64String = Convert.ToBase64String(photoData);
                        imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64String;
                    }
                }
                else
                {
                    lblDoctorName.Text = "Doctor details not found.";
                }

                reader.Close();
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Redirect($"hospitalDoctorEdit.aspx?doctorID={doctorID}");
            }
        }
    }
}