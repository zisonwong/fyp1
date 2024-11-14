using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalAppointmentDetails : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string appointmentID = Request.QueryString["appointmentID"];
                if (!string.IsNullOrEmpty(appointmentID))
                {
                    LoadAppointmentDetails(appointmentID);
                }
                else
                {
                    Response.Write("<script>alert('Invalid Appointment ID.');</script>");
                }
            }
        }
        private void LoadAppointmentDetails(string appointmentID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT a.patientID, d.name AS DoctorName, d.email AS DoctorEmail, d.contactInfo AS DoctorPhone, dep.name AS DoctorDepartment,
                p.name AS PatientName, p.email AS PatientEmail, p.contactInfo AS PatientPhone, p.address AS PatientAddress,
                pay.paymentAmount, pay.paymentMethod, pay.paymentDate, pay.status AS PaymentStatus
                FROM Appointment a
                LEFT JOIN Doctor d ON a.doctorID = d.doctorID
                LEFT JOIN Patient p ON a.patientID = p.patientID
                LEFT JOIN Department dep ON d.departmentID = dep.departmentID
                LEFT JOIN Payment pay ON a.paymentID = pay.paymentID
                WHERE a.appointmentID = @appointmentID";


                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@appointmentID", appointmentID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string patientID = reader["patientID"].ToString();
                    // Doctor Information
                    txtDoctorName.Text = reader["DoctorName"].ToString();
                    txtDoctorEmail.Text = reader["DoctorEmail"].ToString();
                    txtDoctorPhone.Text = reader["DoctorPhone"].ToString();
                    txtDoctorDepartment.Text = reader["DoctorDepartment"].ToString();

                    // Patient Information
                    txtPatientName.Text = reader["PatientName"].ToString();
                    txtPatientEmail.Text = reader["PatientEmail"].ToString();
                    txtPatientPhone.Text = reader["PatientPhone"].ToString();
                    txtPatientAddress.Text = reader["PatientAddress"].ToString();

                    // Payment Information
                    txtPaymentAmount.Text = string.Format("{0:C}", reader["paymentAmount"]);
                    txtPaymentMethod.Text = reader["paymentMethod"].ToString();
                    txtPaymentDate.Text = reader["paymentDate"] != DBNull.Value
                    ? Convert.ToDateTime(reader["paymentDate"]).ToString("dd/MM/yyyy")
                    : "N/A";
                    txtPaymentStatus.Text = reader["PaymentStatus"].ToString();
                    LoadAppointmentsHistory(patientID);
                }
                else
                {
                    Response.Write("<script>alert('No appointment details found for this ID.');</script>");
                }
                conn.Close();
            }
        }
        private void LoadAppointmentsHistory(string patientID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT a.appointmentID, a.patientID, a.doctorID, av.availableFrom AS startTime, av.availableTo AS endTime, av.availableDate AS date, a.status
                    FROM Appointment a
                    LEFT JOIN Availability av ON a.availabilityID = av.availabilityID
                    WHERE a.patientID = @patientID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // Bind the data to the ListView
                lvAppointmentHistory.DataSource = reader;
                lvAppointmentHistory.DataBind();
            }
        }
        protected void lvAppointmentHistory_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAppointment")
            {
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect("hospitalAppointmentDetails.aspx?appointmentID=" + appointmentID);
            }
        }
    }
}