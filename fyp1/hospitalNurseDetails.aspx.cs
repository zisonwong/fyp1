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
    public partial class hospitalNurseDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string nurseID = Request.QueryString["nurseID"];
                if (!string.IsNullOrEmpty(nurseID))
                {
                    LoadNurseDetails(nurseID);
                    btnEdit.CommandArgument = nurseID;
                }
                else
                {
                    lblNurseName.Text = "Nurse not found";
                }
            }
        }

        private void LoadNurseDetails(string nurseID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT nurseID, name, DOB, ICNumber, gender, contactInfo, email, password,
                         branchID, role, status, photo FROM Nurse WHERE nurseID = @NurseID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@NurseID", nurseID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblNurseName.Text = reader["name"].ToString();

                    // Split full name into first and last name
                    string fullName = reader["name"].ToString();
                    string[] nameParts = fullName.Split(' ');

                    txtFirstName.Text = nameParts.Length > 0 ? nameParts[0] : ""; // First Name
                    txtLastName.Text = nameParts.Length > 1 ? nameParts[nameParts.Length - 1] : ""; // Last Name (handles cases where there's only one name)

                    txtEmployeeId.Text = reader["branchID"].ToString();
                    txtDateOfBirth.Text = Convert.ToDateTime(reader["DOB"]).ToString("yyyy-MM-dd");
                    txtIc.Text = reader["ICNumber"].ToString();
                    txtGender.Text = reader["gender"].ToString() == "M" ? "Male" : "Female";
                    txtContactInfo.Text = reader["contactInfo"].ToString();
                    txtEmail.Text = reader["email"].ToString();
                    txtPassword.Attributes["value"] = "***********";
                    txtBranchId.Text = reader["branchID"].ToString();
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
                    lblNurseName.Text = "Nurse details not found.";
                }

                reader.Close();
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();
                Response.Redirect($"hospitalNurseEdit.aspx?nurseID={nurseID}");
            }
        }
    }
}