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

namespace fyp1.Admin
{
    public partial class hospitalStaffResetPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnResetPassword_Click(object sender, EventArgs e)
        {
            string conn = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            // Check which session is set
            string userId = null;
            string userTable = null;

            if (Session["DoctorId"] != null)
            {
                userId = Session["DoctorId"].ToString();
                userTable = "Doctor";
            }
            else if (Session["nurseId"] != null)
            {
                userId = Session["nurseId"].ToString();
                userTable = "Nurse";
            }

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userTable))
            {
                string newPassword = txtNewPassword.Text.Trim();
                string retypePassword = txtReEnterPassword.Text.Trim();

                if (string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(retypePassword))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password fields cannot be empty.');", true);
                    return;
                }
                string enteredVerificationCode = txtVerificationCode.Text.Trim(); 

                string sentVerificationCode = Session["VerificationCode"]?.ToString(); 

                if (string.IsNullOrEmpty(enteredVerificationCode) || enteredVerificationCode != sentVerificationCode)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Verification code does not match.');", true);
                    return;
                }

                if (newPassword == retypePassword)
                {
                    try
                    {
                        // Hash the new password
                        string hashedPassword = HashPassword(newPassword);

                        using (SqlConnection sqlConnection = new SqlConnection(conn))
                        {
                            sqlConnection.Open();

                            string updatePasswd = $@"
                        UPDATE {userTable} 
                        SET password = @Password
                        WHERE {userTable.ToLower()}ID = @UserId";

                            using (SqlCommand updateCommand = new SqlCommand(updatePasswd, sqlConnection))
                            {
                                updateCommand.Parameters.AddWithValue("@Password", hashedPassword);
                                updateCommand.Parameters.AddWithValue("@UserId", userId);

                                int rowsAffected = updateCommand.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Password updated successfully.');", true);
                                    Session["PasswordChanged"] = true;
                                    Response.Redirect("~/Admin/hospitalStaffLogin.aspx");
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
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Session expired or unauthorized access. Please log in again.');", true);
                Response.Redirect("~/Admin/hospitalStaffLogin.aspx");
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

            if (Session["DoctorId"] != null)
            {
                userId = Session["DoctorId"].ToString();
                userTable = "Doctor";
            }
            else if (Session["nurseId"] != null)
            {
                userId = Session["nurseId"].ToString();
                userTable = "Nurse";
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