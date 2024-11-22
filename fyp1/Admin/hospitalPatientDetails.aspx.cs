using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalPatientDetails : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string patientID = Request.QueryString["patientID"];

                if (!string.IsNullOrEmpty(patientID))
                {
                    LoadUpcomingAppointments(patientID);
                    LoadPastAppointments(patientID);
                    LoadPatientData(patientID);
                }
                else
                {
                    Response.Write("Invalid Patient ID.");
                }
            }
        }

        private void LoadUpcomingAppointments(string patientID)
        {
            string query = @"
            SELECT 
                a.appointmentID,
                av.availableDate AS Date,
                av.availableFrom AS startTime,
                av.availableTo AS endTime,
                d.name AS DoctorName,
                d.doctorID AS DoctorID,
                dep.name AS DepartmentName
            FROM 
                Appointment a
            INNER JOIN 
                Availability av ON a.availabilityID = av.availabilityID
            INNER JOIN 
                Doctor d ON av.doctorID = d.doctorID
            INNER JOIN 
                Department dep ON d.departmentID = dep.departmentID
            WHERE 
                a.patientID = @patientID
                AND av.availableDate > CONVERT(date, GETDATE())
            ORDER BY 
                av.availableDate, av.availableFrom";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@patientID", patientID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                rptUpcomingAppointments.DataSource = reader;
                                rptUpcomingAppointments.DataBind();
                            }
                            else
                            {
                                rptUpcomingAppointments.DataSource = null;
                                rptUpcomingAppointments.DataBind();
                                Response.Write("No upcoming appointments found.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Response.Write("Error: " + ex.Message);
                    }
                }
            }
        }
        private void LoadPastAppointments(string patientID)
        {
            string query = @"
                SELECT a.appointmentID, 
                       a.patientID, 
                       a.doctorID, 
                       ab.availableFrom AS time, 
                       CONVERT(DATE, ab.availableDate) AS date,
                       a.status
                       FROM Appointment a
                       INNER JOIN Availability ab ON a.availabilityID = ab.availabilityID
                       WHERE ab.availableDate < CONVERT(date, GETDATE())
                       AND a.patientID = @patientID
                       ORDER BY ab.availableDate, ab.availableFrom";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@patientID", patientID);
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    try
                    {
                        conn.Open();
                        da.Fill(dt);
                        lvPastAppointment.DataSource = dt;
                        lvPastAppointment.DataBind();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("Error: " + ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
        }
        protected void lvPastAppointment_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAppointment")
            {
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect("~/Admin/hospitalAppointmentDetails.aspx?appointmentID=" + appointmentID);
            }
        }

        protected void btnDetails_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument != null)
            {
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect($"~/Admin/hospitalAppointmentDetails.aspx?appointmentID={appointmentID}");
            }
        }

        private void LoadPatientData(string patientID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    string patientQuery = @"
                    SELECT p.name, p.ICNumber, p.DOB, p.gender, p.contactInfo, p.email, p.address, p.bloodtype, p.photo,
                           (SELECT COUNT(*) FROM MedicalRecord WHERE patientID = @patientID) AS MedicalRecordCount,
                           (SELECT COUNT(*) FROM Appointment WHERE patientID = @patientID) AS AppointmentRecordCount,
                           (SELECT COUNT(*) FROM MedicineDelivery WHERE patientID = @patientID) AS DeliveryRecordCount
                    FROM Patient p
                    WHERE p.patientID = @patientID";

                    using (SqlCommand command = new SqlCommand(patientQuery, connection))
                    {
                        command.Parameters.AddWithValue("@patientID", patientID);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                lblPatientName.Text = reader["name"].ToString();
                                lblPatientEmail.Text = reader["email"].ToString();
                                lblGender.Text = reader["gender"].ToString() == "M" ? "Male" : "Female";
                                lblIc.Text = reader["ICNumber"].ToString();
                                lblBirthday.Text = Convert.ToDateTime(reader["DOB"]).ToString("dd MMM yyyy");
                                lblContact.Text = reader["contactInfo"].ToString();
                                lblBloodType.Text = reader["bloodtype"].ToString();
                                lblAddress.Text = reader["address"].ToString();
                                lblMedical.Text = reader["MedicalRecordCount"].ToString();
                                lblAppointment.Text = reader["AppointmentRecordCount"].ToString();
                                lblDelivery.Text = reader["DeliveryRecordCount"].ToString();

                                if (reader["photo"] != DBNull.Value)
                                {
                                    byte[] photoData = (byte[])reader["photo"];
                                    string base64String = Convert.ToBase64String(photoData);
                                    imgAvatar.ImageUrl = "data:image/jpeg;base64," + base64String;
                                }
                                else
                                {
                                    imgAvatar.ImageUrl = "../hospitalImg/defaultAvatar.jpg"; 
                                }
                            }
                            else
                            {
                                lblPatientName.Text = "Patient not found";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log or display the error
                    lblPatientName.Text = "Error loading data: " + ex.Message;
                }
            }
        }
    }
}