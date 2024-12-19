using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalStaffForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSendVerification_Click(object sender, EventArgs e)
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            if (string.IsNullOrEmpty(txt_fp_email.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email cannot be left empty.');", true);
                return;
            }

            if (!IsValidEmail(txt_fp_email.Text))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Invalid Email Format.');", true);
                return;
            }

            string receiverEmail = txt_fp_email.Text;

            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                sqlConnection.Open();

                string queryDoctor = "SELECT doctorID FROM Doctor WHERE email = @Email";
                string queryNurse = "SELECT nurseID FROM Nurse WHERE email = @Email";

                using (SqlCommand sqlCommand = new SqlCommand(queryDoctor, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Email", receiverEmail);
                    object doctorResult = sqlCommand.ExecuteScalar();

                    if (doctorResult != null)
                    {
                        string doctorId = doctorResult.ToString();
                        Session["DoctorId"] = doctorId;

                        SendVerificationEmail(receiverEmail, "Doctor");
                        return;
                    }
                }

                using (SqlCommand sqlCommand = new SqlCommand(queryNurse, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Email", receiverEmail);
                    object nurseResult = sqlCommand.ExecuteScalar();

                    if (nurseResult != null)
                    {
                        string nurseId = nurseResult.ToString();
                        Session["nurseId"] = nurseId;

                        SendVerificationEmail(receiverEmail, "Nurse");
                        return;
                    }
                }

                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email not found. Please check your email or register first.');", true);
            }
        }

        private void SendVerificationEmail(string receiverEmail, string userType)
        {
            try
            {
                string senderEmail = "yhchan6@gmail.com";
                MailMessage verificationMail = new MailMessage(senderEmail, receiverEmail)
                {
                    Subject = "Password Recovery from Trinity Medical Center",
                    Body = "<h3>Please click the button link below to verify that you are the user and proceed to the password reset page.</h3><br><br>" +
                           "<a href=\"https://localhost:44387/Admin/hospitalStaffResetPassword.aspx\" style=\"text-decoration:none;color:white;border:1px solid black;background-color:black;padding: 15px;\">Reset Password</a>",
                    IsBodyHtml = true
                };

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(senderEmail, "xhbwlasavhxfrtrz")
                };

                smtpClient.Send(verificationMail);
                ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Email sent successfully.');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email failed to send: " + ex.Message + "');", true);
            }
        }

        protected bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false;
            }

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }
    }
}