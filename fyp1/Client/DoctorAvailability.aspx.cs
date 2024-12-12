using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class doctorAvailability : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
        private string selectedDoctorId;

        protected void Page_Load(object sender, EventArgs e)
        {
            selectedDoctorId = Request.QueryString["doctorID"];

            if (!IsPostBack)
            {
                // Load initial doctor details
                LoadDoctorDetails();

                // Populate branches
                PopulateBranches();
            }
        }

        private void LoadDoctorDetails()
        {
            if (string.IsNullOrEmpty(selectedDoctorId))
            {
                // Redirect or show error if no doctor selected
                Response.Redirect("SelectDoctor.aspx");
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        d.Name, 
                        d.photo,
                        -- Departments (distinct)
                        STUFF((
                            SELECT DISTINCT ', ' + dep.Name
                            FROM DoctorDepartment dd
                            JOIN Department dep ON dd.DepartmentID = dep.DepartmentID
                            WHERE dd.DoctorID = d.DoctorID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS DepartmentNames,
    
                        -- Branches (distinct)
                        STUFF((
                            SELECT DISTINCT ', ' + br.Name
                            FROM DoctorDepartment dd
                            JOIN Department dep ON dd.DepartmentID = dep.DepartmentID
                            JOIN Branch br ON dep.BranchID = br.BranchID
                            WHERE dd.DoctorID = d.DoctorID
                            FOR XML PATH(''), TYPE
                        ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS BranchNames

                    FROM 
                        Doctor d
                    WHERE 
                        d.DoctorID = @doctorID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorId", selectedDoctorId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblDoctorName.Text = reader["name"].ToString();
                        lblDepartment.Text = reader["DepartmentNames"].ToString();
                        lblBranch.Text = reader["BranchNames"].ToString();

                        // Handle doctor photo
                        if (reader["photo"] != DBNull.Value)
                        {
                            byte[] imageData = (byte[])reader["photo"];
                            imgDoctor.ImageUrl = "data:image/jpeg;base64," +
                                Convert.ToBase64String(imageData);
                        }
                    }
                }
            }
        }

        private void PopulateBranches()
        {
            string doctorID = Request.QueryString["doctorID"];

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT b.branchID, b.name FROM Branch b " +
                    "JOIN Department dep ON dep.branchID = b.branchID " +
                    "JOIN DoctorDepartment dd ON dd.departmentID = dep.departmentID " +
                    "JOIN Doctor d ON d.doctorID = dd.doctorID " +
                    "WHERE b.status = 'activated' " +
                    "AND d.doctorID = @doctorID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                da.SelectCommand.Parameters.AddWithValue("@doctorID", doctorID);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlBranch.DataSource = dt;
                ddlBranch.DataTextField = "name";
                ddlBranch.DataValueField = "branchID";
                ddlBranch.DataBind();
                ddlBranch.Items.Insert(0, new ListItem("Select Branch", ""));
            }
        }

        protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlBranch.SelectedValue))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT DISTINCT d.departmentID, d.name 
                        FROM Department d
                        INNER JOIN DoctorDepartment dd ON d.departmentID = dd.departmentID
                        WHERE d.branchID = @BranchID 
                        AND dd.doctorID = @DoctorID 
                        AND d.status = 'Activated'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@BranchID", ddlBranch.SelectedValue);
                    cmd.Parameters.AddWithValue("@DoctorID", selectedDoctorId);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    ddlDepartment.DataSource = dt;
                    ddlDepartment.DataTextField = "name";
                    ddlDepartment.DataValueField = "departmentID";
                    ddlDepartment.DataBind();
                    ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
                }
            }
            else
            {
                ddlDepartment.Items.Clear();
                ddlDepartment.Items.Insert(0, new ListItem("Select Department", ""));
            }

            rptAvailableSlots.DataSource = null;
            rptAvailableSlots.DataBind();
        }

        protected void ddlDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {
            rptAvailableSlots.DataSource = null;
            rptAvailableSlots.DataBind();
        }

        protected void ddlConsultationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadAvailableSlots();
        }

        protected void txtSelectedDate_TextChanged(object sender, EventArgs e)
        {
            LoadAvailableSlots();
        }

        private void LoadAvailableSlots()
        {
            if (string.IsNullOrEmpty(ddlBranch.SelectedValue) ||
                string.IsNullOrEmpty(ddlDepartment.SelectedValue) ||
                string.IsNullOrEmpty(txtSelectedDate.Text))
            {
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        a.availabilityID,
                        b.name AS BranchName,
                        dept.name AS DepartmentName,
                        a.availableDate,
                        FORMAT(a.availableFrom, 'hh:mm tt') AS AvailableTime,
                        @ConsultationType AS ConsultationType
                    FROM Availability a
                    INNER JOIN Doctor d ON a.doctorID = d.doctorID
                    INNER JOIN DoctorDepartment dd ON d.doctorID = dd.doctorID
                    INNER JOIN Department dept ON dd.departmentID = dept.departmentID
                    INNER JOIN Branch b ON dept.branchID = b.branchID
                    WHERE 
                        d.doctorID = @DoctorID AND
                        b.branchID = @BranchID AND
                        dept.departmentID = @DepartmentID AND
                        a.availableDate = @SelectedDate AND
                        a.status = 'Active'";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", selectedDoctorId);
                cmd.Parameters.AddWithValue("@BranchID", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@DepartmentID", ddlDepartment.SelectedValue);
                cmd.Parameters.AddWithValue("@SelectedDate", DateTime.Parse(txtSelectedDate.Text));
                cmd.Parameters.AddWithValue("@ConsultationType", ddlConsultationType.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                rptAvailableSlots.DataSource = dt;
                rptAvailableSlots.DataBind();
            }
        }

        protected void rptAvailableTimes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectTime")
            {
                string selectedTime = e.CommandArgument.ToString();

                // Store selected appointment details
                if (DateTime.TryParse(txtSelectedDate.Text, out DateTime selectedDate))
                {
                    lblSelectedAppointment.Text = string.Format(
                        "Selected Appointment: {0} on {1} with Dr. {2}",
                        selectedTime,
                        selectedDate.ToString("MMMM dd, yyyy"),
                        lblDoctorName.Text
                    );
                }
                else
                {
                    lblSelectedAppointment.Text = "Please select a valid date.";
                }


                // You can add additional logic here to save the appointment
                // For example, insert into an Appointments table
            }
        }

        protected void rptAvailableSlots_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectSlot")
            {
                string availabilityId = e.CommandArgument.ToString();

                // Store selected slot in session
                Session["SelectedAvailabilityId"] = availabilityId;

                // Redirect to confirmation or next step
                Response.Redirect("clientAppointment.aspx");
            }
        }
    }
}