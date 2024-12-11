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
                LoadDepartments();
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
        private void LoadDoctors(string filter = "", string branchID = "", string departmentID = "")
        {
            string query = @"
                        SELECT 
                    d.doctorID, 
                    d.name, 
                    d.role, 
                    d.contactInfo, 
                    d.photo,
                    STRING_AGG(dept.name, ', ') AS DepartmentNames,
                    STRING_AGG(b.name, ', ') AS BranchNames
                FROM Doctor d
                LEFT JOIN DoctorDepartment dd ON d.doctorID = dd.doctorID
                LEFT JOIN Department dept ON dd.departmentID = dept.departmentID
                LEFT JOIN Branch b ON dept.branchID = b.branchID
                WHERE (d.name LIKE @filter OR @filter = '' OR @filter IS NULL)
                  AND (@departmentID = '' OR @departmentID IS NULL OR dept.departmentID = @departmentID)
                  AND (@branchID = '' OR @branchID IS NULL OR b.branchID = @branchID)
                  AND d.status = 'Activated'
                GROUP BY 
                    d.doctorID, 
                    d.name, 
                    d.role, 
                    d.contactInfo, 
                    d.photo";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@filter", "%" + filter + "%");
                cmd.Parameters.AddWithValue("@branchID", branchID);
                cmd.Parameters.AddWithValue("@departmentID", departmentID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                // Process photo conversion
                dt.Columns.Add("ImageUrl", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["photo"] != DBNull.Value)
                    {
                        byte[] photoBytes = (byte[])row["photo"];
                        if (photoBytes.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(photoBytes);
                            row["ImageUrl"] = "data:image/png;base64," + base64String;
                        }
                    }
                    else
                    {
                        row["ImageUrl"] = ResolveUrl("~/Images/default-doctor.png");
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    rptDoctors.DataSource = dt;
                    rptDoctors.DataBind();
                    pnlRecommendations.Visible = false;
                    lblError.Text = ""; // Clear any previous error messages
                }
                else
                {
                    rptDoctors.DataSource = null;
                    rptDoctors.DataBind();
                    ShowRecommendations(filter);
                    lblError.Text = "No doctors found.";
                }
            }
        }

        private void LoadDepartments()
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand("SELECT departmentID, name FROM Department", conn);
                conn.Open();
                ddlDepartment.DataSource = cmd.ExecuteReader();
                ddlDepartment.DataTextField = "name";
                ddlDepartment.DataValueField = "departmentID";
                ddlDepartment.DataBind();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
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
            LoadDoctors(txtDoctorSearch.Text, ddlBranch.SelectedValue, ddlDepartment.SelectedValue);
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadDoctors(txtDoctorSearch.Text, ddlBranch.SelectedValue, ddlDepartment.SelectedValue);
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
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;
            string sessionID;

            sessionID = GetExistingSessionID(doctorID);

            if (sessionID == null)
            {
                sessionID = CreateNewChatSession(doctorID);
            }

            Response.Redirect("clientChat.aspx?sessionID=" + sessionID + "&doctorID=" + doctorID);
        }

        private string GetExistingSessionID(string doctorID)
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;

            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string query = "SELECT sessionID FROM ChatSession WHERE patientID = @patientID AND doctorID = @doctorID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : null;
            }
        }

        private string CreateNewChatSession(string doctorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string sessionID = GenerateNextSessionID();
            DateTime startTime = DateTime.Now;
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie.Value;

            string query = "INSERT INTO ChatSession (sessionID, patientID, doctorID) VALUES (@sessionID, @patientID, @doctorID)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                cmd.ExecuteNonQuery();
            }

            return sessionID;
        }

        private string GenerateNextSessionID()
        {
            string nextAppointmentID = "S00001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(sessionID) FROM ChatSession", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextAppointmentID = "S" + idNumber.ToString("D5");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while generating Session ID: " + ex.Message;
            }
            return nextAppointmentID;
        }
    }
}


