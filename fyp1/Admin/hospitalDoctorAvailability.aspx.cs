using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class hospitalDoctorAvailability : System.Web.UI.Page
    {
        private int currentYear = DateTime.Now.Year;
        private int currentMonth = DateTime.Now.Month;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.Cookies["DoctorID"] == null)
                {
                    Response.Redirect("~/Admin/hospitalStaffLogin.aspx");
                }

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
            string doctorID = null;
            if (Request.Cookies["DoctorID"] != null)
            {
                doctorID = Request.Cookies["DoctorID"].Value;
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Doctor ID not found. Please log in again.');", true);
                return;
            }

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

                // Retrieve only the availability slots for the specific doctor and date
                List<AvailabilitySlot> availabilitySlots = GetAvailabilityTimes(currentDate, doctorID);

                // Create a badge text for the day (e.g., time slots available)
                string badgeText = string.Join(", ", availabilitySlots.Select(slot => slot.TimeSlot));

                calendarDays.Add(new CalendarDay
                {
                    Day = day.ToString(),
                    IsPlaceholder = false,
                    AvailabilityTimes = availabilitySlots, // Assign retrieved AvailabilitySlot objects
                    Badge = badgeText // Assign the badge text for the day
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


        private List<AvailabilitySlot> GetAvailabilityTimes(DateTime date, string doctorID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            List<AvailabilitySlot> availabilitySlots = new List<AvailabilitySlot>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT availabilityID, availableFrom, availableTo 
                 FROM Availability 
                 WHERE availableDate = @date AND doctorID = @doctorID
                 ORDER BY availableFrom ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    TimeSpan? currentStartTime = null;
                    TimeSpan? currentEndTime = null;
                    List<string> currentAvailableIDs = new List<string>();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                        {
                            string availableID = reader["availabilityID"].ToString();
                            TimeSpan startTime = (TimeSpan)reader["availableFrom"];
                            TimeSpan endTime = (TimeSpan)reader["availableTo"];

                            if (currentEndTime == null)
                            {
                                currentStartTime = startTime;
                                currentEndTime = endTime;
                                currentAvailableIDs.Add(availableID);
                            }
                            else if (currentEndTime.Value >= startTime)
                            {
                                currentEndTime = currentEndTime > endTime ? currentEndTime : endTime;
                                currentAvailableIDs.Add(availableID);
                            }
                            else
                            {
                                string fromFormatted = (new DateTime(1, 1, 1) + currentStartTime.Value).ToString("hh:mm tt");
                                string toFormatted = (new DateTime(1, 1, 1) + currentEndTime.Value).ToString("hh:mm tt");
                                availabilitySlots.Add(new AvailabilitySlot
                                {
                                    TimeSlot = $"{fromFormatted} - {toFormatted}",
                                    AvailableIDs = string.Join(",", currentAvailableIDs)
                                });

                                currentStartTime = startTime;
                                currentEndTime = endTime;
                                currentAvailableIDs.Clear();
                                currentAvailableIDs.Add(availableID);
                            }
                        }
                    }

                    // Add the last time slot
                    if (currentStartTime.HasValue && currentEndTime.HasValue)
                    {
                        string fromFormatted = (new DateTime(1, 1, 1) + currentStartTime.Value).ToString("hh:mm tt");
                        string toFormatted = (new DateTime(1, 1, 1) + currentEndTime.Value).ToString("hh:mm tt");
                        availabilitySlots.Add(new AvailabilitySlot
                        {
                            TimeSlot = $"{fromFormatted} - {toFormatted}",
                            AvailableIDs = string.Join(",", currentAvailableIDs)
                        });
                    }
                }
            }

            return availabilitySlots;
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
                // Check if the current time is 1:30 PM
                if (startTime.Hour == 13 && startTime.Minute == 30)
                {
                    startTime = startTime.AddMinutes(30); // Skip this time slot
                    continue;
                }

                // Add the time to the dropdown
                ddl.Items.Add(new ListItem(startTime.ToString("hh:mm tt"), startTime.ToString("HH:mm")));

                // Increment time by 30 minutes
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
                                     "VALUES (@availabilityID, @doctorID, @date, @availableFrom, @availableTo, @intervalTime, @status)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@availabilityID", availabilityID);
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@availableFrom", fromTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@availableTo", toTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@intervalTime", TimeSpan.FromMinutes(interval));
                    cmd.Parameters.AddWithValue("@status", "Available");
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

            string doctorID = null;
            if (Request.Cookies["DoctorID"] != null)
            {
                doctorID = Request.Cookies["DoctorID"].Value;
            }

            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Doctor ID not found. Please log in again.');", true);
                return;
            }
            DateTime date = DateTime.Parse(txtDate.Text);
            DateTime availableFrom = DateTime.Parse(ddlAvailableFrom.SelectedValue);
            DateTime availableTo = DateTime.Parse(ddlAvailableTo.SelectedValue);

            if (availableFrom >= availableTo)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Start time must be earlier than end time.');", true);
                return;
            }

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
        protected void btnSaveEdit_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime selectedDate = DateTime.Parse(txtEditDate.Text);
                DateTime fromTime = DateTime.Parse(txtEditAvailableFrom.Text);
                DateTime toTime = DateTime.Parse(txtEditAvailableTo.Text);

                TimeSpan newFromTime = fromTime.TimeOfDay;
                TimeSpan newToTime = toTime.TimeOfDay;

                // Retrieve the list of availability IDs from the hidden field
                string[] availableIDs = hdnAvailableIDs.Value.Split(',');

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    List<string> idsToRemove = new List<string>();
                    List<string> unavailableTimes = new List<string>();

                    bool anyChangesMade = false; // Flag to track if any changes were made

                    foreach (string id in availableIDs)
                    {
                        string query = "SELECT availableFrom, availableTo, status FROM Availability WHERE availabilityID = @availabilityID";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@availabilityID", id);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    TimeSpan dbFromTime = reader.GetTimeSpan(0);
                                    TimeSpan dbToTime = reader.GetTimeSpan(1);
                                    string status = reader.GetString(2);

                                    // If the status is not "Available", store the time and status
                                    if (status != "Available")
                                    {
                                        unavailableTimes.Add($"{dbFromTime:hh\\:mm} - {dbToTime:hh\\:mm}"); // Format for time range
                                    }

                                    // Check if the current time interval falls outside the new range and should be removed
                                    if (dbFromTime >= newToTime || dbToTime <= newFromTime)
                                    {
                                        idsToRemove.Add(id); // Mark this ID for removal
                                        anyChangesMade = true; // A change is being made
                                    }
                                }
                            }
                        }
                    }

                    // If there are any unavailable times, display an error and exit
                    if (unavailableTimes.Count > 0)
                    {
                        string unavailableTimesMessage = string.Join(", ", unavailableTimes);
                        ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('The following times are not available for editing: {unavailableTimesMessage}');", true);
                        return; // Stop further processing
                    }

                    // Remove all IDs that are no longer within the specified range
                    foreach (string idToRemove in idsToRemove)
                    {
                        string deleteQuery = "DELETE FROM Availability WHERE availabilityID = @availabilityID";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@availabilityID", idToRemove);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    // Check if any changes were made (deletion or update)
                    if (anyChangesMade)
                    {
                        // Confirmation message on successful update
                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Availability updated successfully!');", true);
                    }
                    else
                    {
                        // If no changes were made, inform the user
                        ClientScript.RegisterStartupScript(this.GetType(), "info", "alert('No availability records were changed.');", true);
                    }
                }
            }
            catch (FormatException)
            {
                // Error handling for invalid date/time formats
                ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('Time format error. Please ensure times are in HH:mm or hh:mm tt format.');", true);
            }
            catch (Exception ex)
            {
                // General error handling
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
            DisplayCalendar();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            // Retrieve the availability IDs from the hidden field
            string[] availableIDs = hdnAvailableIDs.Value.Split(',');

            // Ensure that there are IDs to delete
            if (availableIDs.Length == 0 || string.IsNullOrWhiteSpace(availableIDs[0]))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('No availability records selected for deletion.');", true);
                return; // Exit if no IDs are selected
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Start a transaction for atomicity
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string availabilityID in availableIDs)
                        {
                            // Delete query to remove the availability record
                            string deleteQuery = "DELETE FROM Availability WHERE availabilityID = @availabilityID";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@availabilityID", availabilityID);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }

                        // Commit the transaction if all deletions are successful
                        transaction.Commit();
                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Selected availability records have been deleted successfully.');", true);
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction if any error occurs
                        transaction.Rollback();
                        ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('Error deleting records: {ex.Message}');", true);
                    }
                }
            }
            DisplayCalendar();
        }
        protected void ddlAvailableFrom_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedTime = ddlAvailableFrom.SelectedValue;

            // Clear existing items
            ddlAvailableTo.Items.Clear();
            ddlAvailableTo.Items.Add(new ListItem("Choose Time", ""));

            if (string.IsNullOrEmpty(selectedTime))
            {
                return; // Exit if no time is selected
            }

            DateTime selectedDateTime;
            if (DateTime.TryParse(selectedTime, out selectedDateTime))
            {
                // Populate ddlAvailableTo with time slots based on selected time
                if (selectedDateTime.Hour >= 8 && selectedDateTime.Hour < 13)
                {
                    // Available times from 8 AM to 1 PM, excluding 1:30 PM
                    for (int hour = 8; hour <= 13; hour++)
                    {
                        for (int minute = 0; minute < 60; minute += 30)
                        {
                            // Skip 1:30 PM
                            if (hour == 13 && minute == 30)
                                continue;

                            DateTime timeSlot = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, hour, minute, 0);
                            ddlAvailableTo.Items.Add(new ListItem(timeSlot.ToString("hh:mm tt"), timeSlot.ToString("HH:mm")));
                        }
                    }
                }
                else if (selectedDateTime.Hour >= 14)
                {
                    for (int hour = 14; hour <= 18; hour++)
                    {
                        for (int minute = 0; minute < 60; minute += 30)
                        {
                            if (hour == 18 && minute == 30)
                                continue;

                            DateTime timeSlot = new DateTime(selectedDateTime.Year, selectedDateTime.Month, selectedDateTime.Day, hour, minute, 0);
                            ddlAvailableTo.Items.Add(new ListItem(timeSlot.ToString("hh:mm tt"), timeSlot.ToString("HH:mm")));
                        }
                    }
                }
            }
        }

    }
    public class CalendarDay
    {
        public string Day { get; set; }
        public bool IsPlaceholder { get; set; }
        public List<AvailabilitySlot> AvailabilityTimes { get; set; }
        public string Badge { get; set; } // New property to hold the badge text
    }

    public class TimeSlot
    {
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
    }
    public class AvailabilitySlot
    {
        public string TimeSlot { get; set; }
        public string AvailableIDs { get; set; }
    }

}