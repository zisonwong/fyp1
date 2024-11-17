using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientChat : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string sessionID = Request.QueryString["sessionID"];

            if (!string.IsNullOrEmpty(sessionID))
            {
                LoadChatHistory(sessionID);
            }

            if (!IsPostBack)
            {
                LoadDoctorList();
            }
        }

        //private string GetPatientIDFromCookie()
        //{
        //    Retrieve patient ID from cookie
        //   var patientCookie = Request.Cookies["PatientID"];
        //    return patientCookie?.Value ?? string.Empty;
        //}

        private void LoadDoctorList()
        {
            string patientID = Request.Cookies["PatientID"]?.Value;

            if (string.IsNullOrEmpty(patientID))
                return;

            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string query = "SELECT DISTINCT d.doctorID, d.name, d.photo FROM ChatSession c JOIN Doctor d ON c.doctorID = d.doctorID WHERE c.patientID = @patientID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);

                // Adding a new column to store image URLs
                dt.Columns.Add("ImageUrl", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    if (row["photo"] != DBNull.Value)
                    {
                        byte[] photoBytes = (byte[])row["photo"];
                        if (photoBytes.Length > 0)
                        {
                            string base64String = Convert.ToBase64String(photoBytes);
                            row["ImageUrl"] = $"data:image/png;base64,{base64String}";
                        }
                    }
                    else
                    {
                        // Default placeholder image URL
                        row["ImageUrl"] = ResolveUrl("~/Images/doctor1.jpg");
                    }
                }

                RepeaterDoctorList.DataSource = dt;
                RepeaterDoctorList.DataBind();
            }
        }

        protected void DoctorSelected(object source, RepeaterCommandEventArgs e)
        {
            string doctorID = (string)e.CommandArgument;
            string sessionID = GetChatSessionID(doctorID);
            LoadChatHistory(sessionID);

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Image imgDoctorPhoto = (Image)e.Item.FindControl("imgDoctorPhoto");
                DataRowView row = (DataRowView)e.Item.DataItem;

                // Log to confirm ImageUrl is correctly set
                System.Diagnostics.Debug.WriteLine("ImageUrl: " + row["ImageUrl"].ToString());

                imgDoctorPhoto.ImageUrl = row["ImageUrl"].ToString();
            }
        }

        private string GetChatSessionID(string doctorID)
        {
            string patientID = Request.Cookies["PatientID"]?.Value;
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string sessionID = "";

            string query = "SELECT sessionID FROM ChatSession WHERE patientID = @patientID AND doctorID = @doctorID";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);
                conn.Open();
                sessionID = cmd.ExecuteScalar()?.ToString();
            }

            return sessionID;
        }

        private void LoadChatHistory(string sessionID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT sender, content, timestamp FROM Message WHERE sessionID = @sessionID ORDER BY timestamp";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                var messages = new List<dynamic>();

                while (reader.Read())
                {
                    messages.Add(new
                    {
                        sender = reader["sender"].ToString(),
                        content = reader["content"].ToString(),
                        messageClass = reader["sender"].ToString() == "patient" ? "bg-blue-100" : "bg-green-100"
                    });
                }

                RepeaterMessages.DataSource = messages;
                RepeaterMessages.DataBind();
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            SendMessage();
        }

        protected void SendMessage()
        {
            string sessionID = Request.QueryString["sessionID"];
            string content = txtMessage.Text.Trim();
            string sendername = Request.Cookies["Username"]?.Value ?? "Unknown";
            string messageID = GenerateNextMessageID();

            if (!string.IsNullOrEmpty(content))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO Message (messageID, sessionID, content, timestamp, sender) VALUES (@MessageID, @sessionID, @content, @timestamp, @sender)";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@messageID", messageID);
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    cmd.Parameters.AddWithValue("@content", content);
                    cmd.Parameters.AddWithValue("@timestamp", DateTime.Now);
                    cmd.Parameters.AddWithValue("@sender", sendername);

                    cmd.ExecuteNonQuery();
                }

                txtMessage.Text = "";
                LoadChatHistory(sessionID);
            }
        }

        public static string GetMessages(string sessionID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string messages = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM Message WHERE sessionID = @sessionID ORDER BY timestamp";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string sender = reader["sender"].ToString();
                    string messageContent = reader["content"].ToString();
                    string messageClass = sender == "patient" ? "bg-blue-100" : "bg-green-100";
                    messages += $"<div class='flex {messageClass} p-3 rounded-lg mb-2'><div class='flex-1'>{messageContent}</div>                             <div class='text-sm text-gray-500'>{sender}</div></div>";
                }
            }

            return messages;
        }

        private string GenerateNextMessageID()
        {
            string nextAppointmentID = "M00001";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionString"].ToString()))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT MAX(messageID) FROM Message", conn))
                    {
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value && result != null)
                        {
                            int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                            nextAppointmentID = "M" + idNumber.ToString("D5");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return nextAppointmentID;
        }
    }
}