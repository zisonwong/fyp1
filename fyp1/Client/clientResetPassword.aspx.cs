using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string patientId = Session["PatientID"]?.ToString();

            // Check if the session is valid
            if (!string.IsNullOrEmpty(patientId))
            {
                string newPassword = txtNewPassword.Text.Trim();
                string retypePassword = txtReEnterPassword.Text.Trim();
                string enteredVerificationCode = txtVerificationCode.Text.Trim(); 

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(retypePassword) || string.IsNullOrEmpty(enteredVerificationCode))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password fields and verification code cannot be empty.');", true);
                    return;
                }

                string storedVerificationCode = Session["VerificationCode"]?.ToString();

                if (enteredVerificationCode != storedVerificationCode)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Incorrect verification code. Please try again.');", true);
                    return;
                }

                // Check if passwords match
                if (newPassword == retypePassword)
                {
                    try
                    {
                        // Hash the new password
                        string hashedPassword = HashPassword(newPassword);

                        using (SqlConnection sqlConnection = new SqlConnection(conn))
                        {
                            sqlConnection.Open();

                            string updatePasswd = "UPDATE Patient SET password = @Password WHERE patientID = @PatientId";

                            using (SqlCommand updateCommand = new SqlCommand(updatePasswd, sqlConnection))
                            {
                                updateCommand.Parameters.AddWithValue("@Password", hashedPassword);
                                updateCommand.Parameters.AddWithValue("@PatientId", patientId);

                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password updated successfully.');", true);
                                    Session["PasswordChanged"] = true;
                                    Response.Redirect("~/Client/clientLogin.aspx");
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to update password.');", true);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('An error occurred: " + ex.Message + "');", true);
                    }
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Passwords do not match. Please try again.');", true);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Session expired. Please log in again.');", true);
                Response.Redirect("~/Client/clientLogin.aspx");
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        protected void btnSendVerificationCode_Click(object sender, EventArgs e)
        {
            string userId = string.Empty;
            string userTable = string.Empty;
            string userEmail = string.Empty;

            if (Session["PatientID"] != null)
            {
                userId = Session["PatientID"].ToString();
                userTable = "Patient";  
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Session expired. Please log in again.');", true);
                return;
            }

            string connStr = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = $"SELECT email FROM {userTable} WHERE {userTable.ToLower()}ID = @UserId";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            userEmail = result.ToString();
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('User not found.');", true);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    return;
                }
            }

            string verificationCode = GenerateVerificationCode();

            bool emailSent = SendVerificationCodeEmail(userEmail, verificationCode);

            if (emailSent)
            {
                Session["VerificationCode"] = verificationCode;
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Verification code sent successfully.');", true);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Failed to send verification code.');", true);
            }
        }
        private string GenerateVerificationCode()
        {
            Random random = new Random();
            string code = random.Next(100000, 999999).ToString();
            return code;
        }

        private bool SendVerificationCodeEmail(string email, string verificationCode)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587)
                {
                    Port = 587,
                    Credentials = new NetworkCredential("yhchan6@gmail.com", "xhbwlasavhxfrtrz"),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress("yhchan6@gmail.com"),
                    Subject = "Verification Code",
                    Body = $"Your verification code is: {verificationCode}",
                    IsBodyHtml = false
                };
                mail.To.Add(email);

                smtpClient.Send(mail);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}