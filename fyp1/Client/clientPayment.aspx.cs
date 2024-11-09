using Stripe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientPayment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblDoctorName.Text = Request.QueryString["doctorName"];
                lblAppointmentDate.Text = Request.QueryString["appointmentDate"];
                lblAppointmentTime.Text = Request.QueryString["appointmentTime"];
                lblConsultationFee.Text = Request.QueryString["consultationFee"];
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

                string updateAppointmentQuery = "UPDATE Appointment SET paymentID = @paymentID WHERE appointmentID = @appointmentID";
                using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@paymentID", paymentID);
                    cmd.Parameters.AddWithValue("@appointmentID", appointmentID);

                    cmd.ExecuteNonQuery();
                }

                lblMessage.Text = "Payment confirmed and saved successfully!";
            }

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