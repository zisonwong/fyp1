using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

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
                LoadDoctorDetails();
                PopulateBranches();
            }
        }

        private void LoadDoctorDetails()
        {
            if (string.IsNullOrEmpty(selectedDoctorId))
            {
                // Redirect or show error if no doctor selected
                Response.Redirect("BranchDoctorSelection.aspx");
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
                string.IsNullOrEmpty(txtSelectedDate.Text) ||
                string.IsNullOrEmpty(ddlConsultationType.SelectedValue))
            {
                lblError.Text = "Please fill in all filters to view available slots.";
                rptAvailableSlots.DataSource = null;
                rptAvailableSlots.DataBind();
                return;
            }

            DateTime selectedDate;
            if (!DateTime.TryParse(txtSelectedDate.Text, out selectedDate))
            {
                lblError.Text = "Please select a valid date.";
                rptAvailableSlots.DataSource = null;
                rptAvailableSlots.DataBind();
                return;
            }

            if (selectedDate < DateTime.Today)
            {
                lblError.Text = "You cannot select a past date. Please choose a valid date.";
                rptAvailableSlots.DataSource = null;
                rptAvailableSlots.DataBind();
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                                    a.availabilityID,
                                    a.availableFrom AS AvailableFromTime,
                                    a.availableTo AS AvailableToTime,
                                    @ConsultationType AS ConsultationType
                                FROM Availability a
                                INNER JOIN Doctor d ON a.doctorID = d.doctorID
                                INNER JOIN DoctorDepartment dd ON d.doctorID = dd.doctorID
                                INNER JOIN Department dept ON dd.departmentID = dept.departmentID
                                INNER JOIN Branch b ON dept.branchID = b.branchID
                                WHERE 
                                    d.doctorID = @DoctorID AND
                                    b.branchID = @BranchID AND
                                    a.availableDate = @SelectedDate AND
                                    a.type = @ConsultationType AND
                                    a.status = 'Available' AND
                                    (CAST(a.availableDate AS DATETIME) + CAST(a.availableFrom AS DATETIME)) >= GETDATE()";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DoctorID", selectedDoctorId);
                cmd.Parameters.AddWithValue("@BranchID", ddlBranch.SelectedValue);
                cmd.Parameters.AddWithValue("@SelectedDate", selectedDate);
                cmd.Parameters.AddWithValue("@ConsultationType", ddlConsultationType.SelectedValue);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    lblError.Text = "No available slots for the selected filters.";
                }
                else
                {
                    lblError.Text = string.Empty;
                }

                rptAvailableSlots.DataSource = dt;
                rptAvailableSlots.DataBind();
            }
        }

        protected void rptAvailableTimes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {

            string doctorID = Request.QueryString["doctorID"];
            string patientID = Request.Cookies["PatientID"].Value;
            if (e.CommandName == "SelectTime")
            {
                string availabilityId = e.CommandArgument.ToString();
                string selectedTime = e.CommandArgument.ToString();

                if (DateTime.TryParse(txtSelectedDate.Text, out DateTime selectedDate))
                {
                    if (DateTime.TryParse($"{selectedDate:yyyy-MM-dd} {selectedTime}", out DateTime selectedDateTime))
                    {
                        if (selectedDateTime > DateTime.Now)
                        {
                            string appointmentID = GenerateNextAppointmentID();

                            string AppointPatientID = patientID;
                            string AppointDoctorID = doctorID;
                            string availabilityID = availabilityId;
                            string status = "Pending";

                            using (SqlConnection conn = new SqlConnection("connectionString"))
                            {
                                string query = @"INSERT INTO Appointment 
                                         (appointmentID, patientID, doctorID, availabilityID, status) 
                                         VALUES (@appointmentID, @patientID, @doctorID, @availabilityID, @status)";

                                using (SqlCommand cmd = new SqlCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                                    cmd.Parameters.AddWithValue("@patientID", patientID ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@doctorID", doctorID ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@availabilityID", availabilityID ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@status", status);

                                    conn.Open();
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            Response.Redirect($"Checkout.aspx?appointmentID={appointmentID}");
                        }
                        else
                        {
                            lblSelectedAppointment.Text = "The selected time is in the past. Please select a valid future time.";
                        }
                    }
                    else
                    {
                        lblSelectedAppointment.Text = "Invalid time format. Please try again.";
                    }
                }
                else
                {
                    lblSelectedAppointment.Text = "Invalid date format. Please select a valid date.";
                }
            }
        }

        protected void rptAvailableSlots_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectSlot")
            {
                string availabilityId = e.CommandArgument.ToString();
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                SELECT 
                    a.availableDate, 
                    a.availableFrom,
                    a.availableTo,
                    b.name AS BranchName,
                    dept.name AS DepartmentName,
                    d.Name AS DoctorName,
                    a.type AS ConsultationType
                FROM Availability a
                INNER JOIN Doctor d ON a.doctorID = d.doctorID
                INNER JOIN DoctorDepartment dd ON d.doctorID = dd.doctorID
                INNER JOIN Department dept ON dd.departmentID = dept.departmentID
                INNER JOIN Branch b ON dept.branchID = b.branchID
                WHERE 
                    a.availabilityID = @AvailabilityID AND 
                    a.status = 'Available'";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@AvailabilityID", availabilityId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            try
                            {
                                DateTime availableDate = Convert.ToDateTime(reader["availableDate"]);

                                string availableFromStr = reader["availableFrom"].ToString();
                                string availableToStr = reader["availableTo"].ToString();

                                TimeSpan availableFrom = DateTime.ParseExact(availableFromStr, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;
                                TimeSpan availableTo = DateTime.ParseExact(availableToStr, "HH:mm:ss", CultureInfo.InvariantCulture).TimeOfDay;

                                if (!TimeSpan.TryParse(availableFromStr, out availableFrom))
                                {
                                    availableFrom = TimeSpan.ParseExact(availableFromStr, "hh\\:mm\\:ss", null);
                                }

                                if (!TimeSpan.TryParse(availableToStr, out availableTo))
                                {
                                    availableTo = TimeSpan.ParseExact(availableToStr, "hh\\:mm\\:ss", null);
                                }

                                DateTime fullDateTime = availableDate.Date + availableFrom;

                                if (fullDateTime < DateTime.Now)
                                {
                                    lblError.Text = "The selected time is no longer available. Please choose another slot.";
                                    return;
                                }

                                // Format with explicit culture
                                System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.CreateSpecificCulture("en-US");
                                lblSelectedAppointment.Text = string.Format(culture, @"
                                    <br/>Date: {0:dddd, MMMM dd, yyyy}
                                    <br/>Time: {1:hh\:mm} {1:tt} - {2:hh\:mm} {2:tt}
                                    <br/>Consultation Type: {3}
                                    ",
                                    availableDate,
                                    new DateTime(availableDate.Year, availableDate.Month, availableDate.Day, availableFrom.Hours, availableFrom.Minutes, 0),
                                    new DateTime(availableDate.Year, availableDate.Month, availableDate.Day, availableTo.Hours, availableTo.Minutes, 0),
                                    reader["ConsultationType"]?.ToString() ?? "N/A");

                                // Store the availability ID in ViewState for later use
                                ViewState["SelectedAvailabilityId"] = availabilityId;

                                // Show confirmation button
                                btnConfirmAppointment.Visible = true;
                            }
                            catch (Exception ex)
                            {
                                // Log the full exception details
                                lblError.Text = $"Error processing slot details: {ex.Message}\n{ex.StackTrace}";
                            }
                        }
                        else
                        {
                            lblError.Text = "No matching slot found.";
                        }
                    }
                }
            }
        }

        protected void btnConfirmAppointment_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedAvailabilityId"] != null)
            {
                string availabilityId = ViewState["SelectedAvailabilityId"].ToString();
                string doctorID = Request.QueryString["doctorID"];
                string patientID = Request.Cookies["PatientID"].Value;

                string appointmentID = GenerateNextAppointmentID();

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Appointment 
                     (appointmentID, patientID, doctorID, availabilityID, status) 
                     VALUES (@appointmentID, @patientID, @doctorID, @availabilityID, @status)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                        cmd.Parameters.AddWithValue("@patientID", patientID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@doctorID", doctorID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@availabilityID", availabilityId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", "Pending");

                        conn.Open();
                        cmd.ExecuteNonQuery();
                    }

                    string query2 = @"UPDATE Availability SET status = 'Occupied' WHERE availabilityID = @availabilityID";

                    using (SqlCommand cmd = new SqlCommand(query2, conn))
                    {
                        cmd.Parameters.AddWithValue("@availabilityID", availabilityId);
                        cmd.ExecuteNonQuery();
                    }
                    Response.Redirect($"Checkout.aspx?appointmentID={appointmentID}");
                }
            }
        }

        private string GenerateNextAppointmentID()
        {
            string nextAppointmentID = "A00001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(appointmentID) FROM Appointment", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextAppointmentID = "A" + idNumber.ToString("D5");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while generating patient ID: " + ex.Message;
            }
            return nextAppointmentID;
        }
    }
}