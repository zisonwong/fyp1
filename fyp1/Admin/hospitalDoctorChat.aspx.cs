using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalDoctorChat : System.Web.UI.Page
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

                LoadPatientList();
            }
        }
        private void LoadPatientList()
        {
            string doctorID = Request.Cookies["DoctorID"]?.Value;

            if (string.IsNullOrEmpty(doctorID))
                return;

            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string query = "SELECT DISTINCT p.patientID, p.name, p.photo FROM ChatSession c JOIN Patient p ON c.patientID = p.patientID WHERE c.doctorID = @doctorID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);
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
                RepeaterPatientList.DataSource = dt;
                RepeaterPatientList.DataBind();
            }

        }
        protected void PatientSelected(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "SelectPatient")
            {
                string patientID = e.CommandArgument.ToString();
                string doctorID = Request.Cookies["DoctorID"]?.Value;

                if (!string.IsNullOrEmpty(patientID) && !string.IsNullOrEmpty(doctorID))
                {
                    string sessionID = GetOrCreateChatSession(patientID, doctorID);
                    Response.Redirect($"hospitalDoctorChat.aspx?sessionID={sessionID}&patientID={patientID}");
                    loadMessage();
                }
            }
        }

        private string GetOrCreateChatSession(string patientID, string doctorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string sessionID = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // First, try to get existing session
                string getSessionQuery = @"SELECT sessionID FROM ChatSession 
                                         WHERE doctorID = @doctorID AND patientID = @patientID 
                                         AND (endTime IS NULL OR endTime > GETDATE())";

                using (SqlCommand cmd = new SqlCommand(getSessionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@patientID", patientID);
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);

                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }

                // If no active session exists, create a new one
                sessionID = GenerateNextSessionID();
                string createSessionQuery = @"INSERT INTO ChatSession (sessionID, patientID, doctorID, startTime) 
                                            VALUES (@sessionID, @patientID, @doctorID, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(createSessionQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    cmd.Parameters.AddWithValue("@patientID", patientID);
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);
                    cmd.ExecuteNonQuery();
                }
            }

            return sessionID;
        }

        private string GenerateNextSessionID()
        {
            string nextSessionID = "S00001";
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SELECT MAX(sessionID) FROM ChatSession", conn))
                {
                    object result = cmd.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        int idNumber = int.Parse(result.ToString().Substring(1)) + 1;
                        nextSessionID = "S" + idNumber.ToString("D5");
                    }
                }
            }
            return nextSessionID;
        }

        private string GetChatSessionID(string doctorID)
        {
            string patientID = Request.Cookies["DoctorID"]?.Value;
            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            string sessionID = "";

            string query = "SELECT sessionID FROM ChatSession WHERE doctorID = @doctorID AND patientID = @patientID";
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
            if (string.IsNullOrEmpty(sessionID))
            {
                return; // Exit if no sessionID is provided
            }

            string connectionString = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT 
                                m.sender, 
                                m.content, 
                                m.timestamp,
                                CASE 
                                    WHEN m.sender = (SELECT name FROM Doctor WHERE doctorID = cs.doctorID) THEN 'doctor'
                                    WHEN m.sender = (SELECT name FROM Patient WHERE patientID = cs.patientID) THEN 'patient'
                                    ELSE 'unknown'
                                END AS senderRole
                            FROM Message m
                            JOIN ChatSession cs ON m.sessionID = cs.sessionID
                            WHERE m.sessionID = @sessionID
                            ORDER BY m.timestamp DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sessionID", sessionID);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var messages = new List<dynamic>();
                        while (reader.Read())
                        {
                            string senderRole = reader["senderRole"].ToString();
                            string alignmentClass = senderRole.Equals("patient", StringComparison.OrdinalIgnoreCase)
                                                    ? "text-left"
                                                    : "text-right";
                            string messageClass = senderRole.Equals("patient", StringComparison.OrdinalIgnoreCase)
                                                  ? "bg-blue-100"
                                                  : "bg-green-100";

                            messages.Add(new
                            {
                                sender = reader["sender"].ToString(),
                                content = reader["content"].ToString(),
                                messageClass = messageClass,
                                alignmentClass = alignmentClass,
                                timestamp = Convert.ToDateTime(reader["timestamp"]).ToString("g")
                            });
                        }
                        RepeaterMessages.DataSource = messages;
                        RepeaterMessages.DataBind();
                        loadMessage();
                    }
                }
            }
        }
        protected void btnSend_Click(object sender, EventArgs e)
        {
            string sessionID = Request.QueryString["sessionID"];
            string content = txtMessage.Text.Trim();
            string sendername = Request.Cookies["Email"]?.Value ?? "Unknown";
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

                //TimerRefresh_Tick(sender, e);
                LoadChatHistory(sessionID);
                loadMessage();
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
                    string messageClass = sender == "doctor" ? "bg-blue-100" : "bg-green-100";
                    string alignmentClass = sender == "doctor" ? "text-right" : "text-left";

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
            HttpCookie doctorCookie = Request.Cookies["Email"];
            string doctorEmail = doctorCookie.Value;

            if (!string.IsNullOrEmpty(sessionID) && !string.IsNullOrEmpty(doctorEmail))
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
                    cmd.Parameters.AddWithValue("@Username", doctorEmail);

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
            LoadPatientList();
        }

        public void loadMessage()
        {
            string sessionID = Request.QueryString["sessionID"];
            HttpCookie doctorCookie = Request.Cookies["Email"];
            string doctorEmail = doctorCookie.Value;

            if (!string.IsNullOrEmpty(sessionID) && !string.IsNullOrEmpty(doctorEmail))
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
                    cmd.Parameters.AddWithValue("@Username", doctorEmail);

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
            LoadPatientList();
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