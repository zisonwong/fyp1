using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientForgotPassword : System.Web.UI.Page
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

                string query = "SELECT patientID FROM Patient WHERE email = @Email";

                using (SqlCommand sqlCommand = new SqlCommand(query, sqlConnection))
                {
                    sqlCommand.Parameters.AddWithValue("@Email", receiverEmail);

                    object result = sqlCommand.ExecuteScalar();

                    if (result != null)
                    {
                        string patientId = result.ToString();
                        Session["PatientID"] = patientId;

                        try
                        {
                            string senderEmail = "yhchan6@gmail.com";
                            MailMessage verificationMail = new MailMessage(senderEmail, receiverEmail)
                            {
                                Subject = "Password Recovery from Trinity Medical Center",
                                Body = "<h3>Please click the button link below to verify that you are the user and proceed to the password reset page.</h3><br><br>" +
                                       "<a href=\"https://localhost:44387/Client/clientResetPassword.aspx\" style=\"color:white;border:1px solid black;background-color:black;padding: 15px 10px;\">Reset Password</a>",
                                IsBodyHtml = true
                            };

                            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                            {
                                EnableSsl = true,
                                UseDefaultCredentials = false,
                                Credentials = new NetworkCredential(senderEmail, "xhbwlasavhxfrtrz")
                            };

                            smtpClient.Send(verificationMail);
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email sent successfully.');", true);
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email failed to send: " + ex.Message + "');", true);
                        }
                    }
                    else
                    {
                        // Email does not exist in the database
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Email not found. Please check your email or register first.');", true);
                    }
                }
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