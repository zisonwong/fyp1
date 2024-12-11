using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Stripe.Checkout;
using System.Data;

namespace fyp1.Client
{
    public partial class clientPayment : System.Web.UI.Page
    {
        double consultationFee = 30.00;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblDoctorName.Text = Request.QueryString["doctorName"];
                lblAppointmentDate.Text = Request.QueryString["appointmentDate"];
                lblAppointmentTime.Text = Request.QueryString["appointmentTime"];
                lblConsultationFee.Text = Request.QueryString["consultationFee"];
                lblConsultationFee.Text = consultationFee.ToString();

                paymentSuccessModal.Visible = false;
            }
        }

        protected void btnConfirmPayment_Click(object sender, EventArgs e)
        {
        string paymentMethod = "";

            if (radioBankTransfer.Checked)
            {
                paymentMethod = "Bank Transfer";
            }
            else if (radioCreditCard.Checked)
            {
                paymentMethod = "Credit Card";
            }
            else if (radioCash.Checked)
            {
                paymentMethod = "Cash";
            }


            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            {
                connection.Open();

                string paymentID = GenerateNextPaymentID();
                string consultationFee = lblConsultationFee.Text;
                string appointmentID = Request.QueryString["appointmentID"];

                string insertPaymentQuery = @"
                INSERT INTO Payment (paymentID, paymentAmount, paymentMethod, paymentDate, status)
                VALUES (@paymentID, @paymentAmount, @paymentMethod, @paymentDate, @status)";

                using (SqlCommand cmd = new SqlCommand(insertPaymentQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentID", paymentID);
                    cmd.Parameters.AddWithValue("@paymentAmount", consultationFee);
                    cmd.Parameters.AddWithValue("@paymentMethod", paymentMethod);
                    cmd.Parameters.AddWithValue("@paymentDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@status", "Completed");

                    cmd.ExecuteNonQuery();
                }

                string updateAppointmentQuery = "UPDATE Appointment SET paymentID = @paymentID, status = 'Accepted' WHERE appointmentID = @appointmentID";
                using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentID", paymentID);
                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);

                    cmd.ExecuteNonQuery();
                }
                // Check and update availability status to 'Occupied'
                string updateAvailabilityQuery = @"
                                                    UPDATE Availability
                                                    SET status = 'Occupied'
                                                    WHERE availabilityID = (
                                                        SELECT availabilityID 
                                                        FROM Appointment 
                                                        WHERE appointmentID = @appointmentID
                                                    )
                                                    AND status = 'Available'";

                using (SqlCommand cmd = new SqlCommand(updateAvailabilityQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        lblError.Text = "The selected time slot is already occupied.";
                        return;
                    }
                }
            }
            ShowPaymentSuccessModal();
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
        private void ShowPaymentSuccessModal()
        {
            paymentSuccessModal.Visible = true;
        }

        protected void GoToProfile(object sender, EventArgs e)
        {
            Response.Redirect("clientProfile.aspx");
        }

        protected void CloseModal(object sender, EventArgs e)
        {
            paymentSuccessModal.Visible = false;
        }
        private DataTable GetAppointmentDetails(string appointmentID)
        {
            string query = @"SELECT 
                        a.appointmentID,
                        d.name AS DoctorName,
                        av.availableDate AS AppointmentDate,
                        av.availableFrom AS StartTime,
                        av.availableTo AS EndTime,
                        a.status,
                        pm.paymentAmount,
                        pm.paymentDate,
                        pm.paymentMethod
                    FROM 
                        Appointment a
                    JOIN 
                        Doctor d ON a.doctorID = d.doctorID
                    JOIN 
                        Availability av ON a.availabilityID = av.availabilityID
                    LEFT JOIN 
                        Payment pm ON a.paymentID = pm.paymentID
                    WHERE 
                        a.appointmentID = @appointmentID";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ToString()))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@appointmentID", appointmentID);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adapter.Fill(dt);

                return dt;
            }
        }
    }
}