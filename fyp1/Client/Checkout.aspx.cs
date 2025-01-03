﻿using ClosedXML.Excel;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class Checkout : System.Web.UI.Page
    {
        private const double CONSULTATION_FEE = 30.00;
        public class PaymentInfo
        {
            public string PaymentMethod { get; set; }
            public string CardNumber { get; set; }
            public DateTime? ExpirationDate { get; set; }
            public string CVV { get; set; }
        }

        string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Handle PayPal return
                string status = Request.QueryString["status"];
                string appointmentId = Request.QueryString["appointmentID"];
                string paymentSuccess = Request.QueryString["paymentSuccess"];

                if (status == "success" && !string.IsNullOrEmpty(appointmentId))
                {
                    if (ProcessPayPalPayment(appointmentId))
                    {
                        // Store success in session to persist across redirect
                        Session["PaymentSuccessful"] = true;
                        Response.Redirect($"Checkout.aspx?appointmentID={appointmentId}&paymentSuccess=true");
                        return;
                    }
                }

                // Check for payment success message after redirect
                if (Session["PaymentSuccessful"] != null && (bool)Session["PaymentSuccessful"])
                {
                    ShowPaymentModal();
                    Session["PaymentSuccessful"] = null; // Clear the session
                }

                InitializeCheckoutPage(appointmentId);
            }
        }
        private void DisplayAppointmentDetails()
        {
            string appointmentId = Request.QueryString["appointmentID"];

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                string query = @"SELECT A.appointmentID, D.name AS DoctorName, 
                           AV.availableDate, AV.availableFrom, AV.availableTo
                           FROM Appointment A 
                           INNER JOIN Doctor D ON A.doctorID = D.doctorID
                           INNER JOIN Availability AV ON A.availabilityID = AV.availabilityID
                           WHERE A.appointmentID = @AppointmentID";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);

                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    lblDoctorName.Text = reader["DoctorName"].ToString();
                    lblAppointmentDate.Text = Convert.ToDateTime(reader["availableDate"]).ToShortDateString();
                    lblAppointmentTime.Text = $"{reader["availableFrom"]} - {reader["availableTo"]}";
                    lblConsultationFee.Text = $"RM {CONSULTATION_FEE:N2}";
                    lblSummaryFee.Text = $"RM {CONSULTATION_FEE:N2}";
                    lblTotal.Text = $"RM {CONSULTATION_FEE:N2}";
                }
            }
        }

        private void InitializeCheckoutPage(string appointmentId)
        {
            try
            {
                creditCard.Checked = false;
                paypal.Checked = false;
                creditCardForm.Visible = false;

                if (string.IsNullOrEmpty(appointmentId))
                {
                    Response.Redirect("clientProfile.aspx");
                    return;
                }

                DisplayAppointmentDetails();
                UpdatePaymentSummary();

                HttpCookie cookieID = Request.Cookies["PatientID"];
                if (cookieID == null || string.IsNullOrEmpty(cookieID.Value))
                {
                    Response.Redirect("clientLogin.aspx");
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Error loading payment page: " + ex.Message;
                Response.Redirect("clientHome.aspx");
            }
        }

        protected void btnSubmitPayment_Click(object sender, EventArgs e)
        {
            string appointmentId = Request.QueryString["appointmentID"];

            if (paypal.Checked)
            {
                // Get the current page's URL
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Request.ApplicationPath.TrimEnd('/');

                // Construct return URLs using the base URL
                string returnURL = $"{baseUrl}/Client/Checkout.aspx?status=success&appointmentID={appointmentId}";
                string cancelURL = $"{baseUrl}/Client/Checkout.aspx?appointmentID={appointmentId}";

                string paypalSandboxEmail = "zisonwong25@gmail.com";
                string paypalURL = $"https://www.sandbox.paypal.com/cgi-bin/webscr?" +
                                 $"cmd=_xclick&business={HttpUtility.UrlEncode(paypalSandboxEmail)}" +
                                 $"&amount={CONSULTATION_FEE}" +
                                 $"&currency_code=MYR" +
                                 $"&return={HttpUtility.UrlEncode(returnURL)}" +
                                 $"&cancel_return={HttpUtility.UrlEncode(cancelURL)}";

                Response.Redirect(paypalURL);
            }
            else if (creditCard.Checked)
            {
                // Existing credit card processing code...
                if (!ValidateCardInputs())
                {
                    return;
                }

                if (ProcessCreditCardPayment())
                {
                    ShowPaymentModal();
                }
            }
        }

        private bool ProcessPayPalPayment(string appointmentId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string paymentId = GenerateNextPaymentID();

                            string insertPaymentQuery = @"
                    INSERT INTO Payment (PaymentID, PaymentAmount, PaymentMethod, PaymentDate, Status)
                    VALUES (@PaymentID, @PaymentAmount, @PaymentMethod, @PaymentDate, @Status)";

                            using (SqlCommand cmd = new SqlCommand(insertPaymentQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                                cmd.Parameters.AddWithValue("@PaymentAmount", CONSULTATION_FEE);
                                cmd.Parameters.AddWithValue("@PaymentMethod", "PayPal");
                                cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@Status", "Completed");
                                cmd.ExecuteNonQuery();
                            }

                            string updateAppointmentQuery = @"
                    UPDATE Appointment 
                    SET PaymentID = @PaymentID, Status = 'Accepted'
                    WHERE AppointmentID = @AppointmentID";

                            using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "PayPal payment processing failed: " + ex.Message;
                return false;
            }
        }

        private bool ValidateCardInputs()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtCardNumber.Text) || txtCardNumber.Text.Length != 16)
            {
                lblCreditError.Text = "Please enter a valid 16-digit card number";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtExpirationDate.Text) ||
                !Regex.IsMatch(txtExpirationDate.Text, @"^(0[1-9]|1[0-2])\/([0-9]{2})$"))
            {
                lblDateError.Text = "Please enter a valid expiration date (MM/YY)";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtCvv.Text) || !Regex.IsMatch(txtCvv.Text, @"^\d{3}$"))
            {
                lblCvvError.Text = "Please enter a valid 3-digit CVV";
                isValid = false;
            }

            return isValid;
        }

        private bool ProcessCreditCardPayment()
        {
            string appointmentId = Request.QueryString["appointmentID"];

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            string paymentId = GenerateNextPaymentID();

                            string insertPaymentQuery = @"
                            INSERT INTO Payment (PaymentID, PaymentAmount, PaymentMethod, PaymentDate, Status)
                            VALUES (@PaymentID, @PaymentAmount, @PaymentMethod, @PaymentDate, @Status)";

                            using (SqlCommand cmd = new SqlCommand(insertPaymentQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                                cmd.Parameters.AddWithValue("@PaymentAmount", CONSULTATION_FEE);
                                cmd.Parameters.AddWithValue("@PaymentMethod", "Credit Card");
                                cmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                                cmd.Parameters.AddWithValue("@Status", "Completed");
                                cmd.ExecuteNonQuery();
                            }

                            string updateAppointmentQuery = @"
                            UPDATE Appointment 
                            SET PaymentID = @PaymentID, Status = 'Accepted'
                            WHERE AppointmentID = @AppointmentID";

                            using (SqlCommand cmd = new SqlCommand(updateAppointmentQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@PaymentID", paymentId);
                                cmd.Parameters.AddWithValue("@AppointmentID", appointmentId);
                                cmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "Payment processing failed: " + ex.Message;
                return false;
            }
        }

        private void UpdatePaymentSummary()
        {
            lblTotal.Text = $"RM {CONSULTATION_FEE:N2}";
        }

        private string GenerateNextPaymentID()
        {
            string nextPaymentID = "PAY0001";
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(PaymentID) FROM Payment", conn))
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
                lblError.Text = "Error generating payment ID: " + ex.Message;
            }
            return nextPaymentID;
        }

        protected void PaymentMethod_CheckedChanged(object sender, EventArgs e)
        {
            if (creditCard.Checked)
            {
                creditCardForm.Visible = true;

                creditCardContainer.CssClass = "relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none selected";

                paypalContainer.CssClass = "relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none";
            }
            else if (paypal.Checked)
            {
                creditCardForm.Visible = false;

                paypalContainer.CssClass = "relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none selected";

                creditCardContainer.CssClass = "relative flex cursor-pointer rounded-lg border p-4 shadow-sm focus:outline-none";
            }
        }

        protected void GoToProfile_click(object sender, EventArgs e)
        {
            Response.Redirect("clientProfile.aspx");
        }

        protected void CloseModal_click(object sender, EventArgs e)
        {
            paymentSuccessModal.CssClass += " hidden";
        }

        protected void ShowPaymentModal()
        {
            paymentSuccessModal.CssClass = paymentSuccessModal.CssClass.Replace("hidden", "").Trim();

            paymentSuccessModal.Visible = true;
        }

    }
}