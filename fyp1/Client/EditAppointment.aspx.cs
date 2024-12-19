using ClosedXML.Excel;
using Stripe.Billing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Diagnostics;

namespace fyp1.Client
{
    public partial class EditAppointment : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDoctorDetails();
                PopulateBranches();
            }
        }

        private string getDoctorIdFromAppointment()
        {
            string selectedDoctorId = null;
            string appointmentId = Request.QueryString["AppointmentID"];

            // Check if the AppointmentID is valid
            if (!string.IsNullOrEmpty(appointmentId))
            {
                // First, retrieve the availabilityID from the Appointment table
                string availabilityId = GetAvailabilityIdFromAppointment(appointmentId);

                if (!string.IsNullOrEmpty(availabilityId))
                {
                    // Next, use the availabilityID to get the doctorID from the Availability table
                    selectedDoctorId = GetDoctorIdFromAvailability(availabilityId);
                }
                else
                {
                    Debug.WriteLine("No Availability ID found for AppointmentID: " + appointmentId);
                }
            }
            else
            {
                Debug.WriteLine("AppointmentID is null or empty.");
            }

            return selectedDoctorId; // Return the doctorID
        }

        private string GetAvailabilityIdFromAppointment(string appointmentId)
        {
            string availabilityId = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT availabilityID FROM Appointment WHERE AppointmentID = @AppointmentID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        availabilityId = result.ToString();
                    }
                }
            }

            return availabilityId;
        }

        private string GetDoctorIdFromAvailability(string availabilityId)
        {
            string doctorId = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT doctorID FROM Availability WHERE availabilityID = @availabilityID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@availabilityID", availabilityId);
                    conn.Open();

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        doctorId = result.ToString();
                    }
                }
            }

            return doctorId;
        }

        private void LoadDoctorDetails()
        {
            string doctorId = getDoctorIdFromAppointment();
            Debug.WriteLine("Doctor ID: " + doctorId);
            if (string.IsNullOrEmpty(doctorId))
            {
                lblDoctorName.Text = "Invalid Doctor ID.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                                d.Name, 
                                d.photo,
                                STUFF((
                                    SELECT DISTINCT ', ' + dep.Name
                                    FROM DoctorDepartment dd
                                    JOIN Department dep ON dd.DepartmentID = dep.DepartmentID
                                    WHERE dd.DoctorID = d.DoctorID
                                    FOR XML PATH(''), TYPE
                                ).value('.', 'NVARCHAR(MAX)'), 1, 2, '') AS DepartmentNames,

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
                cmd.Parameters.AddWithValue("@doctorID", doctorId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        lblDoctorName.Text = reader["Name"].ToString();
                        lblDepartment.Text = reader["DepartmentNames"].ToString();
                        lblBranch.Text = reader["BranchNames"].ToString();

                        // Handle doctor photo
                        if (reader["photo"] != DBNull.Value)
                        {
                            byte[] imageData = (byte[])reader["photo"];
                            imgDoctor.ImageUrl = "data:image/jpeg;base64," + Convert.ToBase64String(imageData);
                        }
                        else
                        {
                            imgDoctor.ImageUrl = "~/Images/default-doctor.jpg";
                        }
                    }
                    else
                    {
                        lblDoctorName.Text = "Doctor details not found.";
                        lblDepartment.Text = "-";
                        lblBranch.Text = "-";
                        imgDoctor.ImageUrl = "~/Images/default-doctor.jpg";
                    }
                }
            }
        }

        private void PopulateBranches()
        {
            string doctorId = getDoctorIdFromAppointment();

            if (string.IsNullOrEmpty(doctorId))
            {
                lblDoctorName.Text = "Invalid Doctor ID.";
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT DISTINCT b.branchID, b.name FROM Branch b " +
                    "JOIN Department dep ON dep.branchID = b.branchID " +
                    "JOIN DoctorDepartment dd ON dd.departmentID = dep.departmentID " +
                    "JOIN Doctor d ON d.doctorID = dd.doctorID " +
                    "WHERE b.status = 'activated' " +
                    "AND d.doctorID = @doctorID";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);

                da.SelectCommand.Parameters.AddWithValue("@doctorID", doctorId);
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
            string doctorId = getDoctorIdFromAppointment();

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
                    cmd.Parameters.AddWithValue("@DoctorID", doctorId);

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
            string doctorId = getDoctorIdFromAppointment();

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
                cmd.Parameters.AddWithValue("@DoctorID", doctorId);
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
            string appointmentID = Request.QueryString["appointmentID"];
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
                            using (SqlConnection conn = new SqlConnection("connectionString"))
                            {
                                conn.Open();

                                // Step 1: Retrieve the old availability ID for the existing appointment
                                string oldAvailabilityId = null;
                                string getOldAvailabilityQuery = @"SELECT availabilityID FROM Appointment WHERE appointmentID = @appointmentID";
                                using (SqlCommand cmd = new SqlCommand(getOldAvailabilityQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                                    oldAvailabilityId = cmd.ExecuteScalar()?.ToString();
                                }

                                // Step 2: Update the old availability to 'Available'
                                if (!string.IsNullOrEmpty(oldAvailabilityId))
                                {
                                    string updateOldAvailabilityQuery = @"UPDATE Availability SET status = 'Available' WHERE availabilityID = @availabilityID";
                                    using (SqlCommand cmd = new SqlCommand(updateOldAvailabilityQuery, conn))
                                    {
                                        cmd.Parameters.AddWithValue("@availabilityID", oldAvailabilityId);
                                        cmd.ExecuteNonQuery();
                                    }
                                }

                                // Step 3: Update the appointment with new details
                                string updateAppointmentQuery = @"
                            UPDATE Appointment 
                            SET patientID = @patientID, 
                                doctorID = @doctorID, 
                                availabilityID = @newAvailabilityID, 
                                status = @status 
                            WHERE appointmentID = @appointmentID";
                                using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                                    cmd.Parameters.AddWithValue("@patientID", patientID ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@doctorID", doctorID ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@newAvailabilityID", availabilityId ?? (object)DBNull.Value);
                                    cmd.Parameters.AddWithValue("@status", "Pending");
                                    cmd.ExecuteNonQuery();
                                }

                                // Step 4: Update the new availability to 'Occupied'
                                string updateNewAvailabilityQuery = @"UPDATE Availability SET status = 'Occupied' WHERE availabilityID = @availabilityID";
                                using (SqlCommand cmd = new SqlCommand(updateNewAvailabilityQuery, conn))
                                {
                                    cmd.Parameters.AddWithValue("@availabilityID", availabilityId);
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
                                btnEditAppointment.Visible = true;
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
        protected void btnEditAppointment_Click(object sender, EventArgs e)
        {
            if (ViewState["SelectedAvailabilityId"] != null && Request.QueryString["appointmentID"] != null)
            {
                string newAvailabilityId = ViewState["SelectedAvailabilityId"].ToString();
                string appointmentID = Request.QueryString["appointmentID"];
                string patientID = Request.Cookies["PatientID"].Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string originalDoctorId = null;
                    string oldAvailabilityId = null;
                    string fetchAppointmentDetailsQuery = @"
                                                        SELECT doctorID, availabilityID 
                                                        FROM Appointment 
                                                        WHERE appointmentID = @appointmentID";

                    using (SqlCommand cmd = new SqlCommand(fetchAppointmentDetailsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                originalDoctorId = reader["doctorID"].ToString();
                                oldAvailabilityId = reader["availabilityID"].ToString();
                            }
                        }
                    }

                    if (string.IsNullOrEmpty(originalDoctorId))
                    {
                        lblError.Text = "Doctor ID not found for this appointment.";
                        return;
                    }

                    // Step 2: Validate if the new availability belongs to the same doctor
                    string availabilityDoctorId = null;
                    string validateAvailabilityQuery = @"
                                                    SELECT doctorID 
                                                    FROM Availability 
                                                    WHERE availabilityID = @availabilityID";

                    using (SqlCommand cmd = new SqlCommand(validateAvailabilityQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@availabilityID", newAvailabilityId);
                        availabilityDoctorId = cmd.ExecuteScalar()?.ToString();
                    }

                    if (availabilityDoctorId != originalDoctorId)
                    {
                        lblError.Text = "Selected availability does not belong to the same doctor.";
                        return;
                    }

                    if (!string.IsNullOrEmpty(oldAvailabilityId))
                    {
                        string updateOldAvailabilityQuery = @"UPDATE Availability SET status = 'Available' WHERE availabilityID = @availabilityID";
                        using (SqlCommand cmd = new SqlCommand(updateOldAvailabilityQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@availabilityID", oldAvailabilityId);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    string paymentId = null;
                    string checkPaymentQuery = @"SELECT paymentID FROM Appointment WHERE appointmentID = @appointmentID";
                    using (SqlCommand cmd = new SqlCommand(checkPaymentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                        paymentId = cmd.ExecuteScalar()?.ToString();
                    }

                    string appointmentStatus = string.IsNullOrEmpty(paymentId) ? "Pending" : "Accepted";

                    string updateAppointmentQuery = @"
                                                    UPDATE Appointment 
                                                    SET patientID = @patientID, 
                                                        availabilityID = @newAvailabilityID, 
                                                        status = @status 
                                                    WHERE appointmentID = @appointmentID";
                    using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                        cmd.Parameters.AddWithValue("@patientID", patientID ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@newAvailabilityID", newAvailabilityId ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@status", appointmentStatus);
                        cmd.ExecuteNonQuery();
                    }

                    string updateNewAvailabilityQuery = @"UPDATE Availability SET status = 'Occupied' WHERE availabilityID = @availabilityID";
                    using (SqlCommand cmd = new SqlCommand(updateNewAvailabilityQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@availabilityID", newAvailabilityId);
                        cmd.ExecuteNonQuery();
                    }

                    if (!string.IsNullOrEmpty(paymentId))
                    {
                        Response.Redirect("ClientHome.aspx");
                    }
                    else
                    {
                        Response.Redirect($"Checkout.aspx?appointmentID={appointmentID}");
                    }
                }
            }
        }
    }
}