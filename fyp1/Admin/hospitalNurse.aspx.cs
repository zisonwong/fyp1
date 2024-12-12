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
    public partial class hospitalNurse : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNurse();
            }
        }
        private void LoadNurse()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = "SELECT nurseID, name, role, email, photo FROM Nurse WHERE status = 'Activate'";

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
                            dataTable.Columns.Add("nursePhoto", typeof(string));

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
                                        row["nursePhoto"] = $"data:{mimeType};base64,{base64String}";
                                    }
                                    else
                                    {
                                        // Handle unexpected types if necessary
                                        row["nursePhoto"] = ""; // or set a default image
                                    }
                                }
                                else
                                {
                                    // Handle the case where there is no photo
                                    row["nursePhoto"] = ""; // or set to a default image URL
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

        protected void lbAddNurse_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/hospitalAddNurse.aspx");
        }

        protected void lbNurseDetails_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalNurseDetails.aspx?nurseID={nurseID}");
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
                string query = "UPDATE Nurse SET status = 'UnActivate' WHERE nurseID = @NurseID";

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NurseID", nurseID);
                        connection.Open();

                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "DeleteSuccess",
                                "alert('Nurse deleted successfully.');", true);

                            LoadNurse();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "DeleteError",
                                "alert('Error deleting nurse.');", true);
                        }
                    }
                }
            }
        }
        protected void btnEdit_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string nurseID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalNurseEdit.aspx?nurseID={nurseID}");
            }
        }
    }
}