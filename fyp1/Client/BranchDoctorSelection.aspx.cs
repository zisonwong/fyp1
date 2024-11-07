using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace fyp1.Client
{
    public partial class BranchDoctorSelection : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBranches();
                LoadDoctors();
            }
        }

        private void LoadBranches()
        {
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT branchID, name FROM Branch", conn);
                conn.Open();
                ddlBranch.DataSource = cmd.ExecuteReader();
                ddlBranch.DataTextField = "name";
                ddlBranch.DataValueField = "branchID";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("Select Branch", ""));
            }
        }
        private void LoadDoctors(string filter = "")
        {
            string query = @"SELECT d.doctorID, d.name, d.role, d.contactInfo, d.photo
                     FROM Doctor d
                     INNER JOIN Department dept ON d.departmentID = dept.departmentID
                     WHERE (d.name LIKE @filter OR @filter = '')
                     AND (dept.branchID = @branchID OR @branchID = '')";

            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                cmd.Parameters.AddWithValue("@branchID", ddlBranch.SelectedValue);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                // Add a new column to store the ImageUrl in Base64 format
                dt.Columns.Add("ImageUrl", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["photo"] != DBNull.Value)
                    {
                        byte[] photoBytes = (byte[])row["photo"];
                        if (photoBytes.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(photoBytes);
                            System.Diagnostics.Debug.WriteLine("Base64 String: " + base64String); // Log for debugging
                            row["ImageUrl"] = "data:image/png;base64," + base64String;
                            // Test only with one doctor's photo for debugging
                            imgTestDoctorPhoto.ImageUrl = "data:image/png;base64," + base64String;

                        }

                    }
                    else
                    {
                        // Provide a default image if no photo is available
                        row["ImageUrl"] = ResolveUrl("~/Images/default-doctor.png");
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptDoctors.DataSource = dt;
                    rptDoctors.DataBind();
                    pnlRecommendations.Visible = false;
                }
                else
                {
                    rptDoctors.DataSource = null;
                    rptDoctors.DataBind();
                    ShowRecommendations(filter);
                }
            }
        }

        protected void rptDoctors_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image imgDoctorPhoto = (Image)e.Item.FindControl("imgDoctorPhoto");
                DataRowView row = (DataRowView)e.Item.DataItem;

                // Log to confirm ImageUrl is correctly set
                System.Diagnostics.Debug.WriteLine("ImageUrl: " + row["ImageUrl"].ToString());

                imgDoctorPhoto.ImageUrl = row["ImageUrl"].ToString();
            }
        }


        private void ShowRecommendations(string filter)
        {
            string query = @"SELECT TOP 8 doctorID, name, role 
                             FROM Doctor 
                             WHERE name LIKE @filter";
            using (SqlConnection conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                rptRecommendations.DataSource = dt;
                rptRecommendations.DataBind();
                pnlRecommendations.Visible = true;
            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctors(txtDoctorSearch.Text);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LoadDoctors(txtDoctorSearch.Text);
        }

        protected void btnMakeAppointment_Click(object sender, EventArgs e)
        {
            string doctorID = ((Button)sender).CommandArgument;
            Response.Redirect("DoctorAvailability.aspx?doctorID=" + doctorID);
        }

        protected void btnChatOnline_Click(object sender, EventArgs e)
        {
            string doctorID = ((Button)sender).CommandArgument;
            Response.Redirect("ChatSession.aspx?doctorID=" + doctorID);
        }
    }
}


