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
            if (!IsPostBack)
            {
                string sessionID = Request.QueryString["sessionID"];

                if (!string.IsNullOrEmpty(sessionID))
                {
                    LoadChatHistory(sessionID);
                }

                LoadDoctorList();
            }
        }
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
                        row["ImageUrl"] = ResolveUrl("~/Images/doctor1.jpg");
                    }
                }
                RepeaterDoctorList.DataSource = dt;
                RepeaterDoctorList.DataBind();
            }
        }
        protected void DoctorSelected(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectDoctor")
            {
                string doctorID = e.CommandArgument.ToString();
                Response.Write($"Doctor Selected: {doctorID}"); // Debug output

                string sessionID = GetChatSessionID(doctorID);
                LoadChatHistory(sessionID);

                // Redirect to ensure proper state
                Response.Redirect($"clientChat.aspx?sessionID={sessionID}&doctorID={doctorID}");
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
                string query = @"SELECT 
                                m.sender, 
                                m.content, 
                                m.timestamp, 
                                CASE 
                                    WHEN d.doctorID IS NOT NULL THEN 'doctor'
                                    WHEN p.patientID IS NOT NULL THEN 'patient'
                                    ELSE 'unknown'
                                END AS senderRole
                            FROM Message m
                            LEFT JOIN Doctor d ON m.sender = d.name
                            LEFT JOIN Patient p ON m.sender = p.name
                            WHERE m.sessionID = @sessionID
                            ORDER BY m.timestamp DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                conn.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                var messages = new List<dynamic>();

                while (reader.Read())
                {
                    string senderRole = reader["senderRole"].ToString();
                    string alignmentClass = senderRole.Equals("doctor", StringComparison.OrdinalIgnoreCase)
                                            ? "text-left"
                                            : "text-right";
                    string messageClass = senderRole.Equals("doctor", StringComparison.OrdinalIgnoreCase)
                                          ? "bg-blue-100"
                                          : "bg-green-100";

                    messages.Add(new
                    {
                        sender = reader["sender"].ToString(),
                        content = reader["content"].ToString(),
                        messageClass = messageClass,
                        alignmentClass = alignmentClass,
                        timestamp = Convert.ToDateTime(reader["timestamp"]).ToString("g") // Format timestamp if needed
                    });
                }
                RepeaterMessages.DataSource = messages;
                RepeaterMessages.DataBind();
            }
        }
        protected void btnSend_Click(object sender, EventArgs e)
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

                TimerRefresh_Tick(sender, e);
                LoadChatHistory(sessionID);
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ClearMessageBox", "document.getElementById('" + txtMessage.ClientID + "').value = '';", true);
        }

        public static string GetMessages(string sessionID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string messages = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT sender, content, timestamp FROM Message WHERE sessionID = @sessionID ORDER BY timestamp";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string sender = reader["sender"].ToString();
                    string messageContent = reader["content"].ToString();
                    string messageClass = sender == "patient" ? "bg-blue-100" : "bg-green-100";
                    string alignmentClass = sender == "patient" ? "text-right" : "text-left";

                    messages += $"<div class='flex {alignmentClass} mb-2'>" +
                                $"<div class='flex-1 {messageClass} p-3 rounded-lg max-w-md'>" +
                                $"<div>{messageContent}</div>" +
                                $"<div class='text-sm text-gray-500'>{reader["timestamp"]}</div>" +
                                $"</div></div>";
                }
            }
            return messages;
        }

        protected void TimerRefresh_Tick(object sender, EventArgs e)
        {
            string sessionID = Request.QueryString["sessionID"];
            HttpCookie patientCookie = Request.Cookies["Username"];
            string patientUsername = patientCookie.Value;

            if (!string.IsNullOrEmpty(sessionID) && !string.IsNullOrEmpty(patientUsername))
            {
                string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

                string query = @"
            SELECT Content, Timestamp, 
                   CASE WHEN Sender = @Username THEN 'text-right' ELSE 'text-left' END AS AlignmentClass, 
                   CASE WHEN Sender = @Username THEN 'bg-green-100' ELSE 'bg-blue-100' END AS MessageClass 
            FROM Message
            WHERE SessionID = @SessionID
            ORDER BY Timestamp DESC";

                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@SessionID", sessionID);
                    cmd.Parameters.AddWithValue("@Username", patientUsername);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dt = new DataTable();
                        dt.Load(reader);
                        DataView dv = dt.DefaultView;

                        RepeaterMessages.DataSource = dv;
                        RepeaterMessages.DataBind();
                    }
                }
            }
            LoadDoctorList();
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
        protected void btnVideoCall_Click(object sender, EventArgs e)
        {
            string doctorID = Request.QueryString["doctorID"];
            Response.Redirect("VideoCall.aspx?doctorId=" + doctorID);
        }

    }
}