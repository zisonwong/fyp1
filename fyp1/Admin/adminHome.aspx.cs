using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class adminHome : System.Web.UI.Page
    {
        protected string pieChartDataAppointments = "[]";
        protected string pieChartDataDoctors = "[]";
        protected string pieChartDataNurses = "[]";
        protected string barChartData = "[]";
        protected string barChartCategories = "[]";
        protected int currentChartIndex = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Session["Role"]?.ToString();
            string doctorId = Session["DoctorId"]?.ToString();
            string nurseId = Session["nurseId"]?.ToString();

            if (string.IsNullOrEmpty(role) && Request.Cookies["Role"] != null)
            {
                role = Request.Cookies["Role"].Value.ToLower();
            }

            if (string.IsNullOrEmpty(doctorId) && Request.Cookies["DoctorID"] != null)
            {
                doctorId = Request.Cookies["DoctorID"].Value;
            }

            if (string.IsNullOrEmpty(nurseId) && Request.Cookies["nurseID"] != null)
            {
                nurseId = Request.Cookies["nurseID"].Value;
            }

            if (string.IsNullOrEmpty(role) || (role == "doctor" && string.IsNullOrEmpty(doctorId)) || (role == "nurse" && string.IsNullOrEmpty(nurseId)))
            {
                Response.Redirect("~/Admin/error404.html");
                return;
            }

            if (ViewState["currentChartIndex"] == null)
            {
                ViewState["currentChartIndex"] = 0;
            }
            if (!IsPostBack)
            {
                string userRole = Request.Cookies["Role"]?.Value.ToLower();
                string userId = userRole == "doctor" ? doctorId : nurseId;
                LoadStaffName(userRole, userId);
                populateAppointmentsComparisonChart();
                populateDoctorJoinComparisonChart();
                populateNurseJoinComparisonChart();
                PopulateAppointmentsLineChart();
                PopulateBranchAppointmentChart();
                PopulateLabels();
            }
        }
        protected void nextChartButton_Click(object sender, EventArgs e)
        {
            int currentChartIndex = (int)ViewState["currentChartIndex"];

            if (currentChartIndex < 2) 
            {
                currentChartIndex++;
            }
            else
            {
                currentChartIndex = 2; 
            }

            ViewState["currentChartIndex"] = currentChartIndex;

            UpdatePanel1.Update();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "RenderChart",
                $"renderPieChart({currentChartIndex});", true);
        }

        protected void prevChartButton_Click(object sender, EventArgs e)
        {
            int currentChartIndex = (int)ViewState["currentChartIndex"];

            if (currentChartIndex > 0) 
            {
                currentChartIndex--;
            }
            else
            {
                currentChartIndex = 0; 
            }

            ViewState["currentChartIndex"] = currentChartIndex;

            UpdatePanel1.Update();

            ScriptManager.RegisterStartupScript(this, this.GetType(), "RenderChart",
                $"renderPieChart({currentChartIndex});", true);
        }


        private void loadPieChart()
        {
            string pieChartData = string.Empty;
            string chartTitle = string.Empty;

            if (currentChartIndex == 0)
            {
                pieChartData = pieChartDataAppointments;
                chartTitle = "Appointments Comparison";
            }
            else if (currentChartIndex == 1)
            {
                pieChartData = pieChartDataDoctors;
                chartTitle = "Doctors Join Comparison";
            }
            else if (currentChartIndex == 2)
            {
                pieChartData = pieChartDataNurses;
                chartTitle = "Nurses Join Comparison";
            }

            pieChartDataAppointments = pieChartData;
            pieChartDataDoctors = pieChartData;
            pieChartDataNurses = pieChartData;
        }
        private void populateAppointmentsComparisonChart()
        {
            int currentMonthAppointments = 0;
            int previousMonthAppointments = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN MONTH(availableDate) = MONTH(GETDATE()) AND YEAR(availableDate) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS CurrentMonthAppointments,
            SUM(CASE WHEN MONTH(availableDate) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(availableDate) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS PreviousMonthAppointments
        FROM 
            Appointment a
        INNER JOIN 
            Availability av ON a.availabilityID = av.availabilityID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentMonthAppointments = reader["CurrentMonthAppointments"] != DBNull.Value
                                ? Convert.ToInt32(reader["CurrentMonthAppointments"])
                                : 0;
                            previousMonthAppointments = reader["PreviousMonthAppointments"] != DBNull.Value
                                ? Convert.ToInt32(reader["PreviousMonthAppointments"])
                                : 0;
                        }
                    }
                }
            }

            int totalAppointments = currentMonthAppointments + previousMonthAppointments;
            double currentMonthPercentage = totalAppointments > 0
                ? (double)currentMonthAppointments / totalAppointments * 100
                : 0;
            double previousMonthPercentage = totalAppointments > 0
                ? (double)previousMonthAppointments / totalAppointments * 100
                : 0;

            pieChartDataAppointments = $@"
    [
        {{
            name: 'This Month', 
            y: {currentMonthPercentage.ToString("F2")},
            totalAppointments: {currentMonthAppointments}
        }},
        {{
            name: 'Last Month', 
            y: {previousMonthPercentage.ToString("F2")},
            totalAppointments: {previousMonthAppointments}
        }}
    ]";
        }
        private void populateDoctorJoinComparisonChart()
        {
            int currentMonthDoctors = 0;
            int previousMonthDoctors = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN MONTH(date) = MONTH(GETDATE()) AND YEAR(date) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS CurrentMonthDoctors,
            SUM(CASE WHEN MONTH(date) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(date) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS PreviousMonthDoctors
        FROM 
            Doctor";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentMonthDoctors = reader["CurrentMonthDoctors"] != DBNull.Value
                                ? Convert.ToInt32(reader["CurrentMonthDoctors"])
                                : 0;
                            previousMonthDoctors = reader["PreviousMonthDoctors"] != DBNull.Value
                                ? Convert.ToInt32(reader["PreviousMonthDoctors"])
                                : 0;
                        }
                    }
                }
            }

            int totalDoctors = currentMonthDoctors + previousMonthDoctors;
            double currentMonthPercentage = totalDoctors > 0
                ? (double)currentMonthDoctors / totalDoctors * 100
                : 0;
            double previousMonthPercentage = totalDoctors > 0
                ? (double)previousMonthDoctors / totalDoctors * 100
                : 0;

            pieChartDataDoctors = $@"
[
    {{
        name: 'This Month', 
        y: {currentMonthPercentage.ToString("F2")},
        totalDoctors: {currentMonthDoctors}
    }},
    {{
        name: 'Last Month', 
        y: {previousMonthPercentage.ToString("F2")},
        totalDoctors: {previousMonthDoctors}
    }}
]";
        }
        private void populateNurseJoinComparisonChart()
        {
            int currentMonthNurses = 0;
            int previousMonthNurses = 0;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            SUM(CASE WHEN MONTH(date) = MONTH(GETDATE()) AND YEAR(date) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS CurrentMonthNurses,
            SUM(CASE WHEN MONTH(date) = MONTH(DATEADD(MONTH, -1, GETDATE())) AND YEAR(date) = YEAR(GETDATE()) THEN 1 ELSE 0 END) AS PreviousMonthNurses
        FROM 
            Nurse";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            currentMonthNurses = reader["CurrentMonthNurses"] != DBNull.Value
                                ? Convert.ToInt32(reader["CurrentMonthNurses"])
                                : 0;
                            previousMonthNurses = reader["PreviousMonthNurses"] != DBNull.Value
                                ? Convert.ToInt32(reader["PreviousMonthNurses"])
                                : 0;
                        }
                    }
                }
            }

            int totalNurses = currentMonthNurses + previousMonthNurses;
            double currentMonthPercentage = totalNurses > 0
                ? (double)currentMonthNurses / totalNurses * 100
                : 0;
            double previousMonthPercentage = totalNurses > 0
                ? (double)previousMonthNurses / totalNurses * 100
                : 0;

            pieChartDataNurses = $@"
[
    {{
        name: 'This Month', 
        y: {currentMonthPercentage.ToString("F2")},
        totalNurses: {currentMonthNurses}
    }},
    {{
        name: 'Last Month', 
        y: {previousMonthPercentage.ToString("F2")},
        totalNurses: {previousMonthNurses}
    }}
]";
        }
        private void PopulateAppointmentsLineChart()
        {
            DataTable data = new DataTable();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        SELECT 
            YEAR(av.availableDate) AS [Year],
            MONTH(av.availableDate) AS [Month],
            COUNT(*) AS TotalAppointments
        FROM Appointment a
        INNER JOIN Availability av ON a.availabilityID = av.availabilityID
        WHERE av.availableDate >= DATEADD(YEAR, -1, GETDATE())
        GROUP BY YEAR(av.availableDate), MONTH(av.availableDate)
        ORDER BY YEAR(av.availableDate), MONTH(av.availableDate)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(data);
                    }
                }
            }

            List<string> lineDataPoints = new List<string>();
            foreach (DataRow row in data.Rows)
            {
                int year = Convert.ToInt32(row["Year"]);
                int month = Convert.ToInt32(row["Month"]);
                int totalAppointments = Convert.ToInt32(row["TotalAppointments"]);

                DateTime date = new DateTime(year, month, 1);
                long unixTimestamp = new DateTimeOffset(date).ToUnixTimeMilliseconds();

                lineDataPoints.Add($"[{unixTimestamp}, {totalAppointments}]");
            }

            string lineData = "[" + string.Join(",", lineDataPoints) + "]";

            ViewState["lineData"] = lineData;
        }

        protected void PopulateBranchAppointmentChart()
        {
            DataTable branchData = new DataTable();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
        WITH LastSixMonths AS (
            SELECT FORMAT(DATEADD(MONTH, v.number - 1, DATEADD(MONTH, -6, GETDATE())), 'MMM yyyy') AS Month
            FROM master.dbo.spt_values v
            WHERE v.type = 'P' AND v.number BETWEEN 1 AND 6
        )
        SELECT 
            m.Month,
            b.name AS BranchName,
            COUNT(a.appointmentID) AS TotalAppointments
        FROM 
            LastSixMonths m
        CROSS JOIN 
            Branch b
        LEFT JOIN 
            Availability av ON b.branchID = av.branchID AND FORMAT(av.availableDate, 'MMM yyyy') = m.Month
        LEFT JOIN 
            Appointment a ON a.availabilityID = av.availabilityID
        GROUP BY 
            m.Month, b.name
        ORDER BY 
            CONVERT(DATE, '01 ' + m.Month, 106), b.name";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(branchData);
                    }
                }
            }

            if (branchData.Rows.Count > 0)
            {
                var months = branchData.AsEnumerable()
                                       .Select(row => row["Month"].ToString())
                                       .Distinct()
                                       .OrderBy(month => DateTime.ParseExact(month, "MMM yyyy", CultureInfo.InvariantCulture))
                                       .ToList();

                var branches = branchData.AsEnumerable()
                                         .Select(row => row["BranchName"].ToString())
                                         .Distinct()
                                         .ToList();

                barChartCategories = "[" + string.Join(", ", months.Select(m => $"'{m}'")) + "]";

                var seriesData = new List<string>();

                foreach (var branch in branches)
                {
                    var data = months.Select(month =>
                    {
                        var appointmentCount = branchData.AsEnumerable()
                                                         .Where(row => row["BranchName"].ToString() == branch && row["Month"].ToString() == month)
                                                         .Sum(row => Convert.ToInt32(row["TotalAppointments"]));
                        return appointmentCount;
                    });

                    seriesData.Add($@"{{ 
                name: '{branch}', 
                data: [{string.Join(", ", data)}] 
            }}");
                }

                barChartData = "[" + string.Join(", ", seriesData) + "]";
            }
        }
        private void PopulateLabels()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Get total patients
                string totalPatientQuery = "SELECT COUNT(*) FROM Patient";
                using (SqlCommand cmd = new SqlCommand(totalPatientQuery, conn))
                {
                    totalPatient.Text = cmd.ExecuteScalar().ToString();
                }

                // Get total staff (Doctors + Nurses)
                string totalStaffQuery = @"
                SELECT 
                    (SELECT COUNT(*) FROM Doctor) + 
                    (SELECT COUNT(*) FROM Nurse) AS TotalStaff";
                using (SqlCommand cmd = new SqlCommand(totalStaffQuery, conn))
                {
                    totalStaff.Text = cmd.ExecuteScalar().ToString();
                }

                // Get current month's total appointments
                string currentMonthAppointmentsQuery = @"
                SELECT COUNT(*)
                FROM Appointment a
                INNER JOIN Availability av ON a.availabilityID = av.availabilityID
                WHERE MONTH(av.availableDate) = MONTH(GETDATE())
                  AND YEAR(av.availableDate) = YEAR(GETDATE())";
                using (SqlCommand cmd = new SqlCommand(currentMonthAppointmentsQuery, conn))
                {
                    thisMonthAppointment.Text = cmd.ExecuteScalar().ToString();
                }
            }
        }
        private void LoadStaffName(string role, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = role == "doctor"
                ? "SELECT name FROM Doctor WHERE doctorID = @UserID"
                : "SELECT name FROM Nurse WHERE nurseID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            lblDashboardStaffName.Text = result.ToString();
                        }
                        else
                        {
                            lblDashboardStaffName.Text = "Admin";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblDashboardStaffName.Text = "Error loading name.";
                    }
                }
            }
        }
    }
}
