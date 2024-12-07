using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalDoctor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDoctors();
            }
        }
        private void LoadDoctors()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT doctorID, name, role, email, photo FROM Doctor WHERE status = 'Activate'";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            // Add a new column for the Base64 image string
                            dataTable.Columns.Add("doctorPhoto", typeof(string));

                            // Convert binary photo to Base64 string
                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (row["photo"] != DBNull.Value)
                                {
                                    if (row["photo"] is byte[] photoData)
                                    {
                                        string base64String = Convert.ToBase64String(photoData);
                                        string mimeType = "image/png"; // Adjust accordingly if you store file type

                                        // Assign to the new column
                                        row["doctorPhoto"] = $"data:{mimeType};base64,{base64String}";
                                    }
                                    else
                                    {
                                        // Handle unexpected types if necessary
                                        row["doctorPhoto"] = ""; // or set a default image
                                    }
                                }
                                else
                                {
                                    // Handle the case where there is no photo
                                    row["doctorPhoto"] = ""; // or set to a default image URL
                                }
                            }

                            // Bind to the ListView using the new column for the image
                            lvStaff.DataSource = dataTable;
                            lvStaff.DataBind();
                        }
                        else
                        {
                            lvStaff.DataSource = null;
                            lvStaff.DataBind();
                        }
                    }
                }
            }
        }

        protected void lbAddDoctor_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddDoctor.aspx");
        }

        protected void lbDoctorDetails_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalDoctorDetails.aspx?doctorID={doctorID}");
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string query = "UPDATE Doctor SET status = 'UnActivate' WHERE doctorID = @DoctorID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@DoctorID", doctorID);
                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "StatusUpdated",
                                "alert('Doctor status updated to UnActivate successfully.');", true);

                            LoadDoctors();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "UpdateError",
                                "alert('Error updating doctor status.');", true);
                        }
                    }
                }
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalDoctorEdit.aspx?doctorID={doctorID}");
            }
        }
    }
}