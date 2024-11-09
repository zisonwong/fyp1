using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1
{
    public partial class hospitalDoctorAvailability : System.Web.UI.Page
    {
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ViewState["CurrentMonth"] == null || ViewState["CurrentYear"] == null)
                {
                    ViewState["CurrentMonth"] = DateTime.Now.Month;
                    ViewState["CurrentYear"] = DateTime.Now.Year;
                }
                PopulateTimeDropdown(ddlAvailableFrom, "Choose Time");
                PopulateTimeDropdown(ddlAvailableTo, "Choose Time");
                PopulateIntervalDropdown(ddlIntervalTime, "Choose Duration");

                DisplayCalendar();
                BindDaysOfWeek();

                rbNoRepeat.Checked = true;
                pnlRepeatDays.Visible = false; // Hide panel initially
                lblMonth2.Text = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            }


            // Attach event handlers for radio buttons
            rbRepeat.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbNoRepeat.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
        }

        protected void btnPrevious_Click(object sender, EventArgs e)
        {
            // Decrease the month by 1
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            if (currentMonth == 1)
            {
                currentMonth = 12;
                currentYear--;
            }
            else
            {
                currentMonth--;
            }

            // Update ViewState with the new month and year
            ViewState["CurrentMonth"] = currentMonth;
            ViewState["CurrentYear"] = currentYear;

            // Render the calendar and update lblMonth text using JavaScript
            string monthYearText = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "updateMonth", $"document.getElementById('lblMonth').textContent = '{monthYearText}';", true);

            // Refresh the calendar display with the updated month and year
            DisplayCalendar();
            UpdateMonthLabel();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            // Increase the month by 1
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            if (currentMonth == 12)
            {
                currentMonth = 1;
                currentYear++;
            }
            else
            {
                currentMonth++;
            }

            // Update ViewState with the new month and year
            ViewState["CurrentMonth"] = currentMonth;
            ViewState["CurrentYear"] = currentYear;

            // Render the calendar and update lblMonth text using JavaScript
            string monthYearText = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            ScriptManager.RegisterStartupScript(this, this.GetType(), "updateMonth", $"document.getElementById('lblMonth').textContent = '{monthYearText}';", true);

            // Refresh the calendar display with the updated month and year
            DisplayCalendar();
            UpdateMonthLabel(); 
        }

        private void UpdateMonthLabel()
        {
            // Get the month and year from ViewState
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            // Set the label text with the current month and year
            lblMonth2.Text = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            // Make sure the label is visible for JS to access
            lblMonth2.Visible = true;
        }

        private void DisplayCalendar()
        {
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            List<CalendarDay> calendarDays = new List<CalendarDay>();

            // Add placeholders for days before the 1st
            for (int i = 0; i < startDay; i++)
            {
                calendarDays.Add(new CalendarDay { Day = "", IsPlaceholder = true });
            }

            // Add days of the month and retrieve availability data for each day
            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(currentYear, currentMonth, day);

                // Retrieve availability times from the database (this should be your actual method)
                List<string> availabilityTimes = GetAvailabilityTimes(currentDate);

                calendarDays.Add(new CalendarDay
                {
                    Day = day.ToString(),
                    IsPlaceholder = false,
                    AvailabilityTimes = availabilityTimes // Assign retrieved times
                });
            }

            // Add placeholders for remaining days in the last week
            int totalDays = startDay + daysInMonth;
            int remainingDays = (7 - (totalDays % 7)) % 7;
            for (int i = 0; i < remainingDays; i++)
            {
                calendarDays.Add(new CalendarDay { Day = "", IsPlaceholder = true });
            }

            rptCalendar.DataSource = calendarDays;
            rptCalendar.DataBind();
        }


        private List<string> GetAvailabilityTimes(DateTime date)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<string> availabilityTimes = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Query to get all availability periods for the given date
                string query = @"SELECT availableFrom, availableTo 
                         FROM Availability 
                         WHERE availableDate = @date
                         ORDER BY availableFrom ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    // Keep track of the first time slot
                    TimeSpan? currentStartTime = null;
                    TimeSpan? currentEndTime = null;

                    // Loop through all the rows
                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1))
                        {
                            // Get start and end time from the database
                            TimeSpan startTime = (TimeSpan)reader["availableFrom"];
                            TimeSpan endTime = (TimeSpan)reader["availableTo"];

                            // If currentEndTime is null, this is the first record, set currentStartTime and currentEndTime
                            if (currentEndTime == null)
                            {
                                currentStartTime = startTime;
                                currentEndTime = endTime;
                            }
                            else
                            {
                                // Check if the current time slot overlaps or is exactly continuous with the previous one
                                if (currentEndTime.Value >= startTime)
                                {
                                    // If it overlaps, extend the current end time to the maximum of the current and new end times
                                    currentEndTime = currentEndTime > endTime ? currentEndTime : endTime;
                                }
                                else
                                {
                                    // If no overlap, save the previous time slot and start a new one
                                    string fromFormatted = (new DateTime(1, 1, 1) + currentStartTime.Value).ToString("hh:mm tt");
                                    string toFormatted = (new DateTime(1, 1, 1) + currentEndTime.Value).ToString("hh:mm tt");
                                    availabilityTimes.Add($"{fromFormatted} - {toFormatted}");

                                    // Move to the next time slot
                                    currentStartTime = startTime;
                                    currentEndTime = endTime;
                                }
                            }
                        }
                    }

                    // Add the last time slot after exiting the loop
                    if (currentStartTime.HasValue && currentEndTime.HasValue)
                    {
                        string fromFormatted = (new DateTime(1, 1, 1) + currentStartTime.Value).ToString("hh:mm tt");
                        string toFormatted = (new DateTime(1, 1, 1) + currentEndTime.Value).ToString("hh:mm tt");
                        availabilityTimes.Add($"{fromFormatted} - {toFormatted}");
                    }
                }
            }

            return availabilityTimes;
        }


        private string FormatTimeSpan(TimeSpan time)
        {
            return (new DateTime(1, 1, 1) + time).ToString("hh:mm tt");
        }

        private void BindDaysOfWeek()
        {
            // Data for the days of the week
            var daysOfWeek = new[]
            {
            new { Day = "S", DayName = "Sunday" },
            new { Day = "M", DayName = "Monday" },
            new { Day = "T", DayName = "Tuesday" },
            new { Day = "W", DayName = "Wednesday" },
            new { Day = "T", DayName = "Thursday" },
            new { Day = "F", DayName = "Friday" },
            new { Day = "S", DayName = "Saturday" }
        };

            rptDaysOfWeek.DataSource = daysOfWeek;
            rptDaysOfWeek.DataBind();
        }

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            // Show or hide the panel based on the "Repeat Weekly" radio button selection
            pnlRepeatDays.Visible = rbRepeat.Checked;
        }
        private void PopulateTimeDropdown(DropDownList ddl, string defaultText = "")
        {
            ddl.Items.Clear();

            if (!string.IsNullOrEmpty(defaultText))
            {
                ddl.Items.Add(new ListItem(defaultText, ""));
            }

            DateTime startTime = DateTime.Parse("08:00 AM");
            DateTime endTime = DateTime.Parse("06:00 PM");

            while (startTime <= endTime)
            {
                ddl.Items.Add(new ListItem(startTime.ToString("hh:mm tt"), startTime.ToString("HH:mm")));
                startTime = startTime.AddMinutes(30);
            }
        }
        private void PopulateIntervalDropdown(DropDownList ddl, string defaultText = "")
        {
            ddl.Items.Clear();

            if (!string.IsNullOrEmpty(defaultText))
            {
                ddl.Items.Add(new ListItem(defaultText, ""));
            }

            // Populate with interval times (in minutes)
            ddl.Items.Add(new ListItem("30 minutes", "30"));
            ddl.Items.Add(new ListItem("45 minutes", "45"));
            ddl.Items.Add(new ListItem("60 minutes", "60"));
        }
        private void GenerateAvailabilityRecords(string doctorID, DateTime date, DateTime availableFrom, DateTime availableTo, int interval)
        {
            DateTime currentStartTime = availableFrom;

            while (currentStartTime.AddMinutes(interval) <= availableTo)
            {
                // Generate availability ID
                string availabilityID = GenerateNextAvailabilityID();

                // Calculate end time for this interval
                DateTime currentEndTime = currentStartTime.AddMinutes(interval);

                // Save to database
                SaveRecordToDatabase(availabilityID, doctorID, date, currentStartTime, currentEndTime, interval);

                // Update start time for the next interval
                currentStartTime = currentEndTime;
            }
        }

        private void SaveRecordToDatabase(string availabilityID, string doctorID, DateTime date, DateTime fromTime, DateTime toTime, int interval)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                // Check if the record already exists
                string checkQuery = "SELECT COUNT(1) FROM Availability WHERE doctorID = @doctorID AND availableDate = @date AND availableFrom = @availableFrom AND availableTo = @availableTo";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@doctorID", doctorID);
                    checkCmd.Parameters.AddWithValue("@date", date);
                    checkCmd.Parameters.AddWithValue("@availableFrom", fromTime.TimeOfDay);
                    checkCmd.Parameters.AddWithValue("@availableTo", toTime.TimeOfDay);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        // Record already exists, so we don't need to insert it again
                        return;
                    }
                }

                // If the record doesn't exist, insert the new record
                string insertQuery = "INSERT INTO Availability (availabilityID, doctorID, availableDate, availableFrom, availableTo, intervalTime, status) " +
                                     "VALUES (@availabilityID, @doctorID, @date, @availableFrom, @availableTo, @intervalTime, NULL)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@availabilityID", availabilityID);
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@availableFrom", fromTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@availableTo", toTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@intervalTime", TimeSpan.FromMinutes(interval));

                    cmd.ExecuteNonQuery();
                }
            }
        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlAvailableFrom.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a start time.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlAvailableTo.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a end time.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlIntervalTime.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a interval time.');", true);
                return;
            }
            string doctorID = "DT003";  // Assume id is dt003, now havent do login staff cannot get id first
            DateTime date = DateTime.Parse(txtDate.Text);
            DateTime availableFrom = DateTime.Parse(ddlAvailableFrom.SelectedValue);
            DateTime availableTo = DateTime.Parse(ddlAvailableTo.SelectedValue);
            int intervalMinutes = int.Parse(ddlIntervalTime.SelectedValue);
            GenerateAvailabilityRecords(doctorID, date, availableFrom, availableTo, intervalMinutes);
            // Refresh calendar to show updated availability times
            DisplayCalendar();
            ClearSection();
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Availability saved successfully.');", true);
        }


        private string GenerateNextAvailabilityID()
        {
            string latestAvailabilityID = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 availabilityID FROM Availability ORDER BY availabilityID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    latestAvailabilityID = reader["availabilityID"].ToString();
                }
            }

            // Ensure the latestAvailabilityID has the expected format before attempting to increment
            if (!string.IsNullOrEmpty(latestAvailabilityID) && latestAvailabilityID.Length > 2)
            {
                string numericPart = latestAvailabilityID.Substring(2);

                if (int.TryParse(numericPart, out int lastNumber))
                {
                    return "AB" + (lastNumber + 1).ToString("D3");
                }
            }

            // If no valid ID is found or format is unexpected, return the first ID
            return "AB001"; // Fallback to first ID
        }


        private void ClearSection()
        {
            ddlAvailableFrom.SelectedIndex = 0;
            ddlAvailableTo.SelectedIndex = 0;
            ddlIntervalTime.SelectedIndex = 0;
        }

    }
    public class CalendarDay
    {
        public string Day { get; set; }
        public bool IsPlaceholder { get; set; }
        public List<string> AvailabilityTimes { get; set; } = new List<string>();
    }
    public class TimeSlot
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
}