using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace fyp1.Client
{
    public partial class clientAppointment : System.Web.UI.Page
    {
        private DateTime fromTime;
        private DateTime toTime;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (Request.Cookies["PatientID"] == null)
                {
                    Response.Redirect("clientLogin.aspx");
                }

                LoadPatientDetails();
                LoadAppointmentDetails();
                string doctorID = Request.QueryString["doctorID"];
                if (!string.IsNullOrEmpty(doctorID))
                {
                    LoadDoctorDetails(doctorID);
                }
                else
                {
                    lblError.Text = "Doctor ID is missing from the URL.";
                    btnConfirmAppointment.Enabled = false;
                }

                string dateTimeParam = Request.QueryString["dateTime"];

                if (!string.IsNullOrEmpty(dateTimeParam))
                {
                    try
                    {
                        // Expected format is: "MM/dd/yyyy hh:mm tt - hh:mm tt"
                        // Parse the date and time range directly
                        string[] dateTimeParts = dateTimeParam.Split(new[] { " - " }, StringSplitOptions.None);

                        if (dateTimeParts.Length == 2)
                        {
                            string datePart = dateTimeParts[0].Substring(0, 10);
                            string fromTimeString = dateTimeParts[0].Substring(11).Trim();
                            string toTimeString = dateTimeParts[1].Trim();

                            // Parse each component, checking for validity
                            if (DateTime.TryParseExact(datePart, "MM/dd/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime appointmentDate) &&
                                DateTime.TryParse($"{datePart} {fromTimeString}", out fromTime) &&
                                DateTime.TryParse($"{datePart} {toTimeString}", out toTime))
                            {
                                // Display formatted appointment date and time
                                lblAppointmentDate.Text = appointmentDate.ToString("D");
                                lblAppointmentTime.Text = $"{fromTime:hh:mm tt} - {toTime:hh:mm tt}";
                            }
                            else
                            {
                                lblError.Text = "Invalid date or time format in the query string.";
                                btnConfirmAppointment.Enabled = false;
                            }
                        }
                        else
                        {
                            lblError.Text = "Date and time range format is invalid. Please use 'MM/dd/yyyy hh:mm tt - hh:mm tt'.";
                            btnConfirmAppointment.Enabled = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        lblError.Text = "Unexpected error parsing appointment date and time. " + ex.Message;
                        btnConfirmAppointment.Enabled = false;
                    }
                }
                else
                {
                    lblError.Text = "No date and time provided.";
                    btnConfirmAppointment.Enabled = false;
                }
            }
        }

        private void LoadDoctorDetails(string doctorID)
        {
            string query = @"SELECT d.Name, d.ContactInfo, d.photo, dept.name AS DepartmentName
                            FROM Doctor d
                            INNER JOIN Department dept ON d.DepartmentID = dept.DepartmentID
                            WHERE d.doctorID = @doctorID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Check each field and handle nulls
                    lblDoctorName.Text = reader["Name"]?.ToString() ?? "N/A";
                    lblDoctorContact.Text = reader["ContactInfo"]?.ToString() ?? "N/A";
                    lblDepartmentName.Text = reader["DepartmentName"]?.ToString() ?? "N/A";

                    // Set doctor's photo if available, otherwise use a default image
                    if (reader["photo"] != DBNull.Value)
                    {
                        byte[] photoBytes = (byte[])reader["photo"];
                        string base64String = Convert.ToBase64String(photoBytes);
                        imgDoctorPhoto.ImageUrl = "data:image/png;base64," + base64String;
                    }
                    else
                    {
                        imgDoctorPhoto.ImageUrl = "~/Images/default-doctor.png";
                    }
                }
                else
                {
                    lblError.Text = "Doctor not found or invalid ID.";
                    btnConfirmAppointment.Enabled = false;
                }

                reader.Close();
            }
        }

        private void LoadPatientDetails()
        {
            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];

            // Check if the cookie exists and has a value
            if (IDCookie == null || string.IsNullOrEmpty(IDCookie.Value))
            {
                lblError.Text = "Patient ID cookie not found. Please log in again.";
                btnConfirmAppointment.Enabled = false;
                return;
            }

            string patientID = IDCookie.Value;
            string query = "SELECT name, ICNumber, contactInfo FROM Patient WHERE PatientID = @patientID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    // Ensure the parameter name matches the query
                    cmd.Parameters.AddWithValue("@patientID", patientID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Set fields if data is found
                            litPatientName.Text = reader["name"]?.ToString() ?? "N/A";
                            litPatientNRIC.Text = reader["ICNumber"]?.ToString() ?? "N/A";
                            litPatientContact.Text = reader["contactInfo"]?.ToString() ?? "N/A";
                        }
                        else
                        {
                            lblError.Text = "Patient not found.";
                            btnConfirmAppointment.Enabled = false;
                        }
                    }
                }
            }
        }

        private void LoadAppointmentDetails()
        {
            string dateTimeParam = Request.QueryString["dateTime"];

            if (string.IsNullOrEmpty(dateTimeParam))
            {
                lblError.Text = "Appointment date and time not found.";
                btnConfirmAppointment.Enabled = false;
                return;
            }

            try
            {
                string[] dateTimeParts = dateTimeParam.Split(new string[] { " - " }, StringSplitOptions.None);

                if (dateTimeParts.Length != 2)
                {
                    lblError.Text = "Invalid appointment date and time format.";
                    btnConfirmAppointment.Enabled = false;
                    return;
                }

                string datePart = dateTimeParts[0].Substring(0, 10);
                string fromTimePart = dateTimeParts[0].Substring(11).Trim();
                string toTimePart = dateTimeParts[1].Trim();

                DateTime appointmentDate;
                if (!DateTime.TryParse(datePart, out appointmentDate))
                {
                    lblError.Text = "Invalid date format.";
                    btnConfirmAppointment.Enabled = false;
                    return;
                }

                if (!DateTime.TryParse($"{datePart} {fromTimePart}", out fromTime))
                {
                    lblError.Text = "Invalid 'from' time format.";
                    btnConfirmAppointment.Enabled = false;
                    return;
                }

                if (!DateTime.TryParse($"{datePart} {toTimePart}", out toTime))
                {
                    lblError.Text = "Invalid 'to' time format.";
                    btnConfirmAppointment.Enabled = false;
                    return;
                }

                lblAppointmentDate.Text = appointmentDate.ToString("D");
                lblAppointmentTime.Text = $"{fromTime:hh:mm tt} - {toTime:hh:mm tt}";

                // Save fromTime and toTime to ViewState
                ViewState["fromTime"] = fromTime;
                ViewState["toTime"] = toTime;
            }
            catch (Exception ex)
            {
                lblError.Text = "Unexpected error parsing appointment date and time. " + ex.Message;
                btnConfirmAppointment.Enabled = false;
            }
        }



        protected void btnConfirmAppointment_Click(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            lblError.Text = "";

            HttpCookie IDCookie = HttpContext.Current.Request.Cookies["PatientID"];
            string patientID = IDCookie?.Value;

            string doctorID = Request.QueryString["doctorID"];
            string date = lblAppointmentDate.Text;

            // Retrieve fromTime from ViewState
            if (ViewState["fromTime"] != null)
            {
                fromTime = (DateTime)ViewState["fromTime"];
            }
            else
            {
                lblError.Text = "The appointment time is not available. Please try again.";
                return;
            }

            // Check for valid date and patient ID
            if (string.IsNullOrEmpty(doctorID) || string.IsNullOrEmpty(date) || string.IsNullOrEmpty(patientID))
            {
                lblError.Text = "Invalid appointment details. Please try again.";
                return;
            }

            string formattedDate = DateTime.Parse(date).ToString("yyyy-MM-dd");

            string appointmentID = GenerateNextAppointmentID();
            string paymentID = GenerateNextPaymentID();

            string insertQuery = @"
    INSERT INTO Appointment (appointmentID, doctorID, patientID, date, time, paymentID, status)
    VALUES (@appointmentID, @doctorID, @patientID, @date, @time, @paymentID, 'Pending')";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
            {
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@appointmentID", appointmentID);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                cmd.Parameters.AddWithValue("@date", formattedDate);
                cmd.Parameters.AddWithValue("@time", fromTime); // Now fromTime has a value
                cmd.Parameters.AddWithValue("@paymentID", paymentID);

                try
                {
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Response.Redirect($"paymentPage.aspx?paymentID={paymentID}");
                    }
                    else
                    {
                        lblError.Text = "Failed to confirm the appointment. Please try again.";
                    }
                }
                catch (SqlException ex)
                {
                    lblError.Text = "An error occurred while booking the appointment. Please try again later. " + ex.Message;
                }
            }
        }






        private string GenerateNextAppointmentID()
        {
            string nextAppointmentID = "A0001";
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
                            nextAppointmentID = "A" + idNumber.ToString();
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

        private string GenerateNextPaymentID()
        {
            string nextPaymentID = "PAY0001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(paymentID) FROM Payment", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(3)) + 1;
                            nextPaymentID = "PAY" + idNumber.ToString("D4");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "An error occurred while generating payment ID: " + ex.Message;
            }
            return nextPaymentID;
        }

    }
}