using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
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
                PopulateDoctorDropdown();
                if (Request.Cookies["DoctorID"] == null)
                {
                    Response.Redirect("~/Admin/hospitalStaffLogin.aspx");
                }
                else
                {
                    String doctorID = GetDoctorID();
                    PopulateDoctorBranch(doctorID);
                }

                if (ViewState["CurrentMonth"] == null || ViewState["CurrentYear"] == null)
                {
                    ViewState["CurrentMonth"] = DateTime.Now.Month;
                    ViewState["CurrentYear"] = DateTime.Now.Year;
                }

                PopulateIntervalDropdown(ddlIntervalTime, "Choose Duration");

                DisplayCalendar();

                DateTime selectedDate;
                if (DateTime.TryParse(txtDate.Text, out selectedDate))
                {
                    BindDaysOfWeek(selectedDate);
                }
                else
                {
                    selectedDate = DateTime.Now;
                    BindDaysOfWeek(selectedDate);
                }

                rbNoRepeat.Checked = true;
                pnlRepeatDays.Visible = false;
                txtXMonth.Visible = false;
                txtDisplayMonthText.Visible = true;
                lblMonth2.Text = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            }
            // Attach event handlers for radio buttons
            rbRepeat.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbNoRepeat.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbRepeatMonth.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbRepeat3Month.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbRepeat6Month.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbRepeatYear.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            rbXMonth.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "updateMonth", $"document.getElementById('<%= lblMonth2.ClientID %>').textContent = '{monthYearText}';", true);

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
            ScriptManager.RegisterStartupScript(this, this.GetType(), "updateMonth", $"document.getElementById('<%= lblMonth2.ClientID %>').textContent = '{monthYearText}';", true);

            // Refresh the calendar display with the updated month and year
            DisplayCalendar();
            UpdateMonthLabel();
        }

        private void UpdateMonthLabel()
        {
            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            lblMonth2.Text = new DateTime(currentYear, currentMonth, 1).ToString("MMMM yyyy");
            lblMonth2.Visible = true;
        }

        private void DisplayCalendar()
        {
            string doctorID = GetDoctorID();

            if (string.IsNullOrEmpty(doctorID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Doctor ID not found. Please " +
                    "select a doctor or log in again.');", true);
                return;
            }

            int currentMonth = (int)ViewState["CurrentMonth"];
            int currentYear = (int)ViewState["CurrentYear"];

            DateTime firstDayOfMonth = new DateTime(currentYear, currentMonth, 1);
            int daysInMonth = DateTime.DaysInMonth(currentYear, currentMonth);
            int startDay = (int)firstDayOfMonth.DayOfWeek;

            List<CalendarDay> calendarDays = new List<CalendarDay>();

            for (int i = 0; i < startDay; i++)
            {
                calendarDays.Add(new CalendarDay { Day = "", IsPlaceholder = true });
            }

            for (int day = 1; day <= daysInMonth; day++)
            {
                DateTime currentDate = new DateTime(currentYear, currentMonth, day);

                List<AvailabilitySlot> availabilitySlots = GetAvailabilityTimes(currentDate, doctorID);

                string badgeText = string.Join(", ", availabilitySlots.Select(slot => slot.TimeSlot));

                calendarDays.Add(new CalendarDay
                {
                    Day = day.ToString(),
                    IsPlaceholder = false,
                    AvailabilityTimes = availabilitySlots, 
                    Badge = badgeText 
                });
            }

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
                string query = @"SELECT availabilityID, availableFrom, availableTo, branchID 
                         FROM Availability 
                         WHERE availableDate = @date AND doctorID = @doctorID
                         ORDER BY branchID, availableFrom ASC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@date", date);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    Dictionary<string, List<AvailabilitySlot>> branchAvailability = new Dictionary<string, List<AvailabilitySlot>>();

                    while (reader.Read())
                    {
                        if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2) && !reader.IsDBNull(3))
                        {
                            string availableID = reader["availabilityID"].ToString();
                            TimeSpan startTime = (TimeSpan)reader["availableFrom"];
                            TimeSpan endTime = (TimeSpan)reader["availableTo"];
                            string branchID = reader["branchID"].ToString();

                            string fromFormatted = (new DateTime(1, 1, 1) + startTime).ToString("hh:mm tt");
                            string toFormatted = (new DateTime(1, 1, 1) + endTime).ToString("hh:mm tt");

                            // Group availability slots by branch
                            if (!branchAvailability.ContainsKey(branchID))
                            {
                                branchAvailability[branchID] = new List<AvailabilitySlot>();
                            }

                            branchAvailability[branchID].Add(new AvailabilitySlot
                            {
                                TimeSlot = $"{fromFormatted} - {toFormatted}",
                                AvailableIDs = availableID,
                                BranchID = branchID
                            });
                        }
                    }

                    // Merge overlapping slots within each branch
                    foreach (var branchSlots in branchAvailability)
                    {
                        availabilitySlots.AddRange(MergeOverlappingSlots(branchSlots.Value));
                    }
                }
            }

            return availabilitySlots;
        }

        private List<AvailabilitySlot> MergeOverlappingSlots(List<AvailabilitySlot> slots)
        {
            if (slots == null || slots.Count == 0) return new List<AvailabilitySlot>();

            // Sort slots by start time
            slots = slots.OrderBy(s => DateTime.ParseExact(s.TimeSlot.Split('-')[0].Trim(), "hh:mm tt", CultureInfo.InvariantCulture)).ToList();

            List<AvailabilitySlot> mergedSlots = new List<AvailabilitySlot>();
            AvailabilitySlot currentSlot = slots[0];

            for (int i = 1; i < slots.Count; i++)
            {
                DateTime currentStart = DateTime.ParseExact(currentSlot.TimeSlot.Split('-')[0].Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime currentEnd = DateTime.ParseExact(currentSlot.TimeSlot.Split('-')[1].Trim(), "hh:mm tt", CultureInfo.InvariantCulture);

                DateTime nextStart = DateTime.ParseExact(slots[i].TimeSlot.Split('-')[0].Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
                DateTime nextEnd = DateTime.ParseExact(slots[i].TimeSlot.Split('-')[1].Trim(), "hh:mm tt", CultureInfo.InvariantCulture);

                if (nextStart <= currentEnd)
                {
                    // Merge slots
                    currentSlot.TimeSlot = $"{currentStart.ToString("hh:mm tt")} - {(nextEnd > currentEnd ? nextEnd : currentEnd).ToString("hh:mm tt")}";
                    currentSlot.AvailableIDs += "," + slots[i].AvailableIDs;
                }
                else
                {
                    mergedSlots.Add(currentSlot);
                    currentSlot = slots[i];
                }
            }

            mergedSlots.Add(currentSlot);

            return mergedSlots;
        }


        private string FormatTimeSpan(TimeSpan time)
        {
            return (new DateTime(1, 1, 1) + time).ToString("hh:mm tt");
        }

        protected void Calendar_SelectionChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = DateTime.Parse(txtDate.Text);
            ViewState["SelectedDate"] = selectedDate;

            // Debugging log
            string script = $"console.log('Selected Calendar Date: {selectedDate.ToString("yyyy-MM-dd")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "logCalendarDate", script, true);

            DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek); // Get Sunday of the week
            ViewState["StartOfWeek"] = startOfWeek;

            // Debugging log
            script = $"console.log('Start of Week: {startOfWeek.ToString("yyyy-MM-dd")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "logStartOfWeek", script, true);

            BindDaysOfWeek(selectedDate); // Rebind days of week
        }


        private void BindDaysOfWeek(DateTime selectedDate)
        {
            DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek);

            List<DayItem> daysOfWeek = new List<DayItem>();

            for (int i = 0; i < 7; i++)
            {
                DateTime currentDay = startOfWeek.AddDays(i);
                daysOfWeek.Add(new DayItem
                {
                    Day = currentDay.DayOfWeek.ToString().Substring(0, 3),
                    Date = currentDay.ToString("yyyy-MM-dd")
                });
            }

            // Bind data to the Repeater
            rptDaysOfWeek.DataSource = daysOfWeek;
            rptDaysOfWeek.DataBind();
        }

        protected void rptDaysOfWeek_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ToggleDay")
            {
                // Get the index of the clicked day
                int index = Convert.ToInt32(e.CommandArgument);

                // Get the selected date based on the clicked index
                List<DateTime> selectedWeek = GetSevenDayRange(DateTime.Parse(txtDate.Text));
                DateTime selectedDay = selectedWeek[index];

                // Log the selected day
                string dateLogScript = $"console.log('Selected Day: {selectedDay.ToString("yyyy-MM-dd")}');";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "logSelectedDay", dateLogScript, true);

                // Toggle selection logic
                if (SelectedDayIndices.Contains(index))
                {
                    SelectedDayIndices.Remove(index);
                }
                else
                {
                    SelectedDayIndices.Add(index);
                }

                BindDaysOfWeek(DateTime.Parse(txtDate.Text));
            }
        }


        private List<DateTime> GetSevenDayRange(DateTime selectedDate)
        {
            // Calculate start of week (Sunday)
            DateTime startOfWeek = selectedDate.AddDays(-(int)selectedDate.DayOfWeek);

            List<DateTime> weekDates = new List<DateTime>();
            for (int i = 0; i < 7; i++)
            {
                weekDates.Add(startOfWeek.AddDays(i));
            }

            return weekDates;
        }


        public List<int> SelectedDayIndices
        {
            get
            {
                if (ViewState["SelectedDayIndices"] == null)
                {
                    ViewState["SelectedDayIndices"] = new List<int>();
                }
                return (List<int>)ViewState["SelectedDayIndices"];
            }
            set
            {
                ViewState["SelectedDayIndices"] = value;
            }
        }

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            pnlRepeatDays.Visible = rbRepeat.Checked;
            txtXMonth.Visible = rbXMonth.Checked;
            txtDisplayMonthText.Visible = rbXMonth.Checked;

            if (rbRepeat.Checked)
            {
                pnlRepeatDays.Visible = true;
                txtDisplayMonthText.Visible = true;
            }
            else if (rbXMonth.Checked)
            {
                txtXMonth.Visible = true;
                txtDisplayMonthText.Visible = false;
            }
            else
            {
                pnlRepeatDays.Visible=false;
                txtDisplayMonthText.Visible = true;
            }
        }

        private void PopulateTimeDropdown(DropDownList ddl, string branchID, string defaultText = "")
        {
            ddl.Items.Clear();
            if (!string.IsNullOrEmpty(defaultText))
            {
                ddl.Items.Add(new ListItem(defaultText, ""));
            }

            if (string.IsNullOrEmpty(branchID))
            {
                ddl.Items.Add(new ListItem("No branch selected", ""));
                return;
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT openTime, closeTime FROM Branch WHERE branchID = @BranchID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchID", branchID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TimeSpan startTime = (TimeSpan)reader["openTime"];
                            TimeSpan closeTime = (TimeSpan)reader["closeTime"];

                            DateTime currentTime = DateTime.Today.Add(startTime);
                            DateTime endDateTime = DateTime.Today.Add(closeTime);

                            while (currentTime <= endDateTime)
                            {
                                if (currentTime.Hour == 13 && currentTime.Minute == 30)
                                {
                                    currentTime = currentTime.AddMinutes(30);
                                    continue;
                                }
                                ddl.Items.Add(new ListItem(
                                    currentTime.ToString("hh:mm tt"),
                                    currentTime.ToString("HH:mm")
                                ));
                                currentTime = currentTime.AddMinutes(30);
                            }
                        }
                        else
                        {
                            ddl.Items.Add(new ListItem("No times available", ""));
                        }
                    }
                }
            }
        }
        protected void ddlDoctorBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateTimeDropdown(ddlAvailableFrom, ddlDoctorBranch.SelectedValue, "Choose Time");
            PopulateTimeDropdown(ddlAvailableTo, ddlDoctorBranch.SelectedValue, "Choose Time");
        }

        protected void ddlDeleteDoctorBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateTimeDropdown(ddlDeleteAvailableFrom, ddlDeleteDoctorBranch.SelectedValue, "Choose Time");
            PopulateTimeDropdown(ddlDeleteAvailableTo, ddlDeleteDoctorBranch.SelectedValue, "Choose Time");
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
        private void GenerateAvailabilityRecords(string doctorID, string branchID, DateTime date, DateTime availableFrom, DateTime availableTo, int interval)
        {
            DateTime currentStartTime = availableFrom;

            while (currentStartTime.AddMinutes(interval) <= availableTo)
            {
                string availabilityID = GenerateNextAvailabilityID();

                DateTime currentEndTime = currentStartTime.AddMinutes(interval);

                SaveRecordToDatabase(availabilityID, doctorID, branchID, date, currentStartTime, currentEndTime, interval);

                currentStartTime = currentEndTime;
            }
        }



        private void SaveRecordToDatabase(string availabilityID, string doctorID, string branchID, DateTime date, DateTime fromTime, DateTime toTime, int interval)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string availabilityType;
            if (fromTime.TimeOfDay >= new TimeSpan(8, 0, 0) && toTime.TimeOfDay <= new TimeSpan(13, 0, 0))
            {
                availabilityType = "Walk-in";
            }
            else if (fromTime.TimeOfDay >= new TimeSpan(14, 0, 0) && toTime.TimeOfDay <= new TimeSpan(18, 0, 0))
            {
                availabilityType = "Online Consultation";
            }
            else
            {
                availabilityType = "Undefined"; 
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string checkQuery = "SELECT COUNT(1) FROM Availability WHERE doctorID = @doctorID AND branchID = @branchID AND availableDate = @date AND availableFrom = @availableFrom AND availableTo = @availableTo";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@doctorID", doctorID);
                    checkCmd.Parameters.AddWithValue("@branchID", branchID);
                    checkCmd.Parameters.AddWithValue("@date", date);
                    checkCmd.Parameters.AddWithValue("@availableFrom", fromTime.TimeOfDay);
                    checkCmd.Parameters.AddWithValue("@availableTo", toTime.TimeOfDay);

                    conn.Open();
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count > 0)
                    {
                        return;
                    }
                }

                string insertQuery = "INSERT INTO Availability (availabilityID, doctorID, branchID, availableDate, availableFrom, availableTo, intervalTime, status, type) " +
                                     "VALUES (@availabilityID, @doctorID, @branchID, @date, @availableFrom, @availableTo, @intervalTime, @status, @type)";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@availabilityID", availabilityID);
                    cmd.Parameters.AddWithValue("@doctorID", doctorID);
                    cmd.Parameters.AddWithValue("@branchID", branchID);
                    cmd.Parameters.AddWithValue("@date", date);
                    cmd.Parameters.AddWithValue("@availableFrom", fromTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@availableTo", toTime.TimeOfDay);
                    cmd.Parameters.AddWithValue("@intervalTime", TimeSpan.FromMinutes(interval));
                    cmd.Parameters.AddWithValue("@status", "Available"); 
                    cmd.Parameters.AddWithValue("@type", availabilityType);
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
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select an end time.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlIntervalTime.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select an interval time.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlDoctorBranch.SelectedValue))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a branch.');", true);
                return;
            }

            string doctorID = null;
            if (Request.Cookies["DoctorID"] != null)
            {
                doctorID = GetDoctorID();
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Doctor ID not found. Please log in again.');", true);
                return;
            }

            DateTime date = DateTime.Parse(txtDate.Text);
            DateTime availableFrom = DateTime.Parse(ddlAvailableFrom.SelectedValue);
            DateTime availableTo = DateTime.Parse(ddlAvailableTo.SelectedValue);
            string branchID = ddlDoctorBranch.SelectedValue;

            if (availableFrom >= availableTo)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Start time must be earlier than end time.');", true);
                return;
            }

            int intervalMinutes = int.Parse(ddlIntervalTime.SelectedValue);

            if (rbRepeatYear.Checked)
            {
                for (int month = 1; month <= 12; month++)
                {
                    int daysInMonth = DateTime.DaysInMonth(date.Year, month);
                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDay = new DateTime(date.Year, month, day);
                        GenerateAvailabilityRecords(doctorID, branchID, currentDay, availableFrom, availableTo, intervalMinutes);
                    }
                }
            }
            else if (rbRepeat3Month.Checked || rbRepeat6Month.Checked)
            {
                int monthsToRepeat = rbRepeat3Month.Checked ? 3 : 6;

                for (int i = 0; i < monthsToRepeat; i++)
                {
                    DateTime currentMonth = date.AddMonths(i);
                    int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDay = new DateTime(currentMonth.Year, currentMonth.Month, day);
                        GenerateAvailabilityRecords(doctorID, branchID, currentDay, availableFrom, availableTo, intervalMinutes);
                    }
                }
            }
            else if (rbXMonth.Checked)
            {
                int numOfMonths = 0;

                bool isValid = int.TryParse(txtXMonth.Text, out numOfMonths);

                if (!isValid || numOfMonths <= 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please enter a valid positive number of months.');", true);
                    return;
                }

                for (int i = 0; i < numOfMonths; i++)
                {
                    DateTime currentMonth = date.AddMonths(i);
                    int daysInMonth = DateTime.DaysInMonth(currentMonth.Year, currentMonth.Month);

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        DateTime currentDay = new DateTime(currentMonth.Year, currentMonth.Month, day);
                        GenerateAvailabilityRecords(doctorID, branchID, currentDay, availableFrom, availableTo, intervalMinutes);
                    }
                }
            }
            else if (rbRepeatMonth.Checked)
            {
                int daysInMonth = DateTime.DaysInMonth(date.Year, date.Month);

                for (int i = 1; i <= daysInMonth; i++)
                {
                    DateTime currentDay = new DateTime(date.Year, date.Month, i);
                    GenerateAvailabilityRecords(doctorID, branchID, currentDay, availableFrom, availableTo, intervalMinutes);
                }
            }
            else if (rbRepeat.Checked)
            {
                if (SelectedDayIndices.Count == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select at least one day for weekly repetition.');", true);
                    return;
                }

                List<DateTime> weekDates = GetSevenDayRange(date);

                foreach (int index in SelectedDayIndices)
                {
                    DateTime repeatDate = weekDates[index];
                    GenerateAvailabilityRecords(doctorID, branchID, repeatDate, availableFrom, availableTo, intervalMinutes);
                }
            }
            else
            {
                GenerateAvailabilityRecords(doctorID, branchID, date, availableFrom, availableTo, intervalMinutes);
            }

            DisplayCalendar();
            ClearSection();
            SelectedDayIndices.Clear();

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

            if (!string.IsNullOrEmpty(latestAvailabilityID) && latestAvailabilityID.Length > 2)
            {
                string numericPart = latestAvailabilityID.Substring(2);

                if (int.TryParse(numericPart, out int lastNumber))
                {
                    return "AB" + (lastNumber + 1).ToString("D3");
                }
            }

            return "AB001"; 
        }


        private void ClearSection()
        {
            ddlAvailableFrom.SelectedIndex = 0;
            ddlAvailableTo.SelectedIndex = 0;
            ddlIntervalTime.SelectedIndex = 0;
            ddlDoctorBranch.SelectedIndex = 0;
            txtXMonth.Text = "";

            SelectedDayIndices.Clear();
            BindDaysOfWeek(DateTime.Parse(txtDate.Text));
        }
        private void ClearSection2()
        {
            ddlDeleteAvailableFrom.SelectedIndex = 0;
            ddlDeleteAvailableTo.SelectedIndex = 0;
            ddlDeleteDoctorBranch.SelectedIndex = 0;
            txtDeleteStartDate.Text = "";
            txtDeleteEndDate.Text = "";
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

                string[] availableIDs = hdnAvailableIDs.Value.Split(',');

                string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    List<string> idsToRemove = new List<string>();
                    List<string> unavailableTimes = new List<string>();

                    bool anyChangesMade = false;

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

                                    if (status != "Available")
                                    {
                                        unavailableTimes.Add($"{dbFromTime:hh\\:mm} - {dbToTime:hh\\:mm}"); 
                                    }

                                    if (dbFromTime >= newToTime || dbToTime <= newFromTime)
                                    {
                                        idsToRemove.Add(id); 
                                        anyChangesMade = true; 
                                    }
                                }
                            }
                        }
                    }

                    if (unavailableTimes.Count > 0)
                    {
                        string unavailableTimesMessage = string.Join(", ", unavailableTimes);
                        ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('The following times are not available for editing: {unavailableTimesMessage}');", true);
                        return; 
                    }

                    foreach (string idToRemove in idsToRemove)
                    {
                        string deleteQuery = "DELETE FROM Availability WHERE availabilityID = @availabilityID";
                        using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn))
                        {
                            deleteCmd.Parameters.AddWithValue("@availabilityID", idToRemove);
                            deleteCmd.ExecuteNonQuery();
                        }
                    }

                    if (anyChangesMade)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Availability updated successfully!');", true);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "info", "alert('No availability records were changed.');", true);
                    }
                }
            }
            catch (FormatException)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('Time format error. Please ensure times are in HH:mm or hh:mm tt format.');", true);
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", $"alert('Error: {ex.Message}');", true);
            }
            DisplayCalendar();
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            string[] availableIDs = hdnAvailableIDs.Value.Split(',');

            if (availableIDs.Length == 0 || string.IsNullOrWhiteSpace(availableIDs[0]))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "error", "alert('No availability records selected for deletion.');", true);
                return; 
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (string availabilityID in availableIDs)
                        {
                            string deleteQuery = "DELETE FROM Availability WHERE availabilityID = @availabilityID";
                            using (SqlCommand deleteCmd = new SqlCommand(deleteQuery, conn, transaction))
                            {
                                deleteCmd.Parameters.AddWithValue("@availabilityID", availabilityID);
                                deleteCmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        ClientScript.RegisterStartupScript(this.GetType(), "success", "alert('Selected availability records have been deleted successfully.');", true);
                    }
                    catch (Exception ex)
                    {
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
            string branchID = ddlDoctorBranch.SelectedValue; 

            // Clear existing items
            ddlAvailableTo.Items.Clear();
            ddlAvailableTo.Items.Add(new ListItem("Choose Time", ""));

            if (string.IsNullOrEmpty(selectedTime))
            {
                return; // Exit if no time is selected
            }

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                conn.Open();
                string query = "SELECT openTime, closeTime FROM Branch WHERE branchID = @BranchID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BranchID", branchID);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TimeSpan openTime = (TimeSpan)reader["openTime"];
                            TimeSpan closeTime = (TimeSpan)reader["closeTime"];

                            DateTime selectedDateTime;
                            if (DateTime.TryParse(selectedTime, out selectedDateTime))
                            {
                                DateTime startDateTime = DateTime.Today.Add(openTime);
                                DateTime endDateTime = DateTime.Today.Add(closeTime);
                                DateTime currentTime = DateTime.Today.Add(selectedDateTime.TimeOfDay);

                                if (currentTime.TimeOfDay == new TimeSpan(13, 0, 0)) 
                                {
                                    ddlAvailableTo.Items.Clear();
                                    ddlAvailableTo.Items.Add(new ListItem("No available times", ""));
                                    return;
                                }

                                DateTime endTimeStart;
                                DateTime endTimeEnd;

                                if (currentTime.TimeOfDay < new TimeSpan(13, 0, 0)) 
                                {
                                    endTimeStart = currentTime;
                                    endTimeEnd = DateTime.Today.Add(new TimeSpan(13, 0, 0)); 
                                }
                                else 
                                {
                                    endTimeStart = currentTime;
                                    endTimeEnd = endDateTime; 
                                }

                                while (endTimeStart < endTimeEnd)
                                {
                                    endTimeStart = endTimeStart.AddMinutes(30);

                                    if (endTimeStart.TimeOfDay > closeTime)
                                        break;

                                    ddlAvailableTo.Items.Add(new ListItem(
                                        endTimeStart.ToString("hh:mm tt"),
                                        endTimeStart.ToString("HH:mm")
                                    ));
                                }
                            }
                        }
                    }
                }
            }
        }

        protected void PopulateDoctorBranch(string doctorID)
        {
            // Clear existing branches in the delete branch dropdown
            ddlDeleteDoctorBranch.Items.Clear();
            ddlDeleteDoctorBranch.Items.Add(new ListItem("Select Branch", "")); // Add default item

            ddlDoctorBranch.Items.Clear();
            ddlDoctorBranch.Items.Add(new ListItem("Choose Branch", "")); // Add default item

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT DISTINCT b.branchID, b.name 
            FROM Branch b
            INNER JOIN Department d ON b.branchID = d.branchID
            INNER JOIN DoctorDepartment dd ON d.departmentID = dd.departmentID
            WHERE dd.doctorID = @DoctorID AND b.status = 'Activated'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@DoctorID", doctorID);

                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string branchID = reader["branchID"].ToString();
                                string branchName = reader["name"].ToString();

                                ddlDoctorBranch.Items.Add(new ListItem(branchName, branchID));
                                ddlDeleteDoctorBranch.Items.Add(new ListItem(branchName, branchID));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
        }

        public string GetBranchColorClass(object branchID)
        {
            if (branchID == null) return "badge-default";

            string branchIdStr = branchID.ToString();

            int selectedIndex = ddlDoctorBranch.Items.IndexOf(ddlDoctorBranch.Items.FindByValue(branchIdStr));

            if (selectedIndex == -1) return "badge-secondary"; 

            string[] colorClasses = { "", "badge-primary", "badge-danger", "badge-success", "badge-warning", "badge-secondary" };

            return colorClasses[selectedIndex % colorClasses.Length];
        }
        public string GetBranchName(object branchID)
        {
            if (branchID == null) return "Unknown Branch";

            string branchIdStr = branchID.ToString();
            ListItem branchItem = ddlDoctorBranch.Items.FindByValue(branchIdStr);

            return branchItem != null ? branchItem.Text : "Unknown Branch";
        }
        protected void btnDeleteAvailability_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime startDate = DateTime.Parse(txtDeleteStartDate.Text);
                DateTime endDate = DateTime.Parse(txtDeleteEndDate.Text);
                string branchID = ddlDeleteDoctorBranch.SelectedValue;

                // Safely parse start time and end time as TimeSpan, using nullable TimeSpan (TimeSpan?)
                TimeSpan? startTime = null;
                TimeSpan? endTime = null;

                // Only parse if there's a valid value in the dropdown
                if (!string.IsNullOrEmpty(ddlDeleteAvailableFrom.SelectedValue))
                {
                    startTime = TimeSpan.Parse(ddlDeleteAvailableFrom.SelectedValue);
                }

                if (!string.IsNullOrEmpty(ddlDeleteAvailableTo.SelectedValue))
                {
                    endTime = TimeSpan.Parse(ddlDeleteAvailableTo.SelectedValue);
                }

                if (string.IsNullOrEmpty(txtDeleteStartDate.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a start date.');", true);
                    return;
                }

                if (string.IsNullOrEmpty(txtDeleteEndDate.Text))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a end date.');", true);
                    return;
                }

                if (startDate > endDate)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Start date must be earlier than end date.');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlDeleteDoctorBranch.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a branch.');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlDeleteAvailableFrom.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a start time.');", true);
                    return;
                }

                if (string.IsNullOrEmpty(ddlDeleteAvailableTo.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a end time.');", true);
                    return;
                }

                if (startTime >= endTime)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Start time must be earlier than end time.');", true);
                    return;
                }

                string doctorID = GetDoctorID();

                if (string.IsNullOrEmpty(doctorID))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Doctor ID not found. Please select a doctor or log in again.');", true);
                    return;
                }

                if (Request.Cookies["DoctorID"] == null && string.IsNullOrEmpty(ddlSelectDoctor.SelectedValue))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a doctor to proceed.');", true);
                    return;
                }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
                {
                    conn.Open();

                    // SQL query to check if status is "Available"
                    string checkStatusQuery = @"
                SELECT COUNT(*) FROM Availability
                WHERE doctorID = @DoctorID
                AND branchID = @BranchID
                AND availableDate BETWEEN @StartDate AND @EndDate
                AND availableFrom >= @StartTime
                AND availableTo <= @EndTime
                AND status != 'Available'";  // Check if status is not "Available"

                    using (SqlCommand cmd = new SqlCommand(checkStatusQuery, conn))
                    {
                        // Add parameters for the query
                        cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                        cmd.Parameters.AddWithValue("@BranchID", branchID);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);

                        int nonAvailableCount = (int)cmd.ExecuteScalar();

                        if (nonAvailableCount > 0)
                        {
                            // If there are records with status not equal to "Available"
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('Cannot delete availability records because some records' status is not \"Available\".');", true);
                            return; // Exit without deleting
                        }
                    }

                    // SQL query to delete availability records if status is "Available"
                    string deleteQuery = @"
                DELETE FROM Availability
                WHERE doctorID = @DoctorID
                AND branchID = @BranchID
                AND availableDate BETWEEN @StartDate AND @EndDate
                AND (@StartTime IS NULL OR availableFrom >= @StartTime)
                AND (@EndTime IS NULL OR availableTo <= @EndTime)";

                    using (SqlCommand cmd = new SqlCommand(deleteQuery, conn))
                    {
                        // Add parameters for the delete query
                        cmd.Parameters.AddWithValue("@DoctorID", doctorID);
                        cmd.Parameters.AddWithValue("@BranchID", branchID);
                        cmd.Parameters.AddWithValue("@StartDate", startDate);
                        cmd.Parameters.AddWithValue("@EndDate", endDate);
                        cmd.Parameters.AddWithValue("@StartTime", startTime);
                        cmd.Parameters.AddWithValue("@EndTime", endTime);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Provide feedback
                        if (rowsAffected > 0)
                        {
                            string successMessage = $"{rowsAffected} availability record(s) deleted successfully.";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{successMessage}');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", "alert('No availability records found matching the criteria.');", true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle errors
                string errorMessage = $"An error occurred: {ex.Message}";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "alert", $"alert('{errorMessage}');", true);
            }
            DisplayCalendar();
            ClearSection2();
        }
        private void PopulateDoctorDropdown()
        {
            ddlSelectDoctor.Items.Clear();
            ddlSelectDoctor.Items.Add(new ListItem("Select Doctor", ""));

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT doctorID, name 
            FROM Doctor
            WHERE status = 'Activate'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string doctorID = reader["doctorID"].ToString();
                                string doctorName = reader["name"].ToString();

                                ddlSelectDoctor.Items.Add(new ListItem($"{doctorName} - {doctorID}", doctorID));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error populating doctor dropdown: {ex.Message}");
                    }
                }
            }
        }

        protected void ddlSelectDoctor_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedDoctorID = ddlSelectDoctor.SelectedValue;

            if (!string.IsNullOrEmpty(selectedDoctorID))
            {
                PopulateDoctorBranch(selectedDoctorID);

                DisplayCalendar();
            }
            else
            {
                ddlDoctorBranch.Items.Clear();
                ddlDeleteDoctorBranch.Items.Clear();
                DisplayCalendar();
            }
        }


        private string GetDoctorID()
        {
            if (!string.IsNullOrEmpty(ddlSelectDoctor.SelectedValue))
            {
                Console.WriteLine($"Dropdown Selected DoctorID: {ddlSelectDoctor.SelectedValue}");
                return ddlSelectDoctor.SelectedValue;
            }

            if (Request.Cookies["DoctorID"] != null)
            {
                Console.WriteLine($"Cookie DoctorID: {Request.Cookies["DoctorID"].Value}");
                return Request.Cookies["DoctorID"].Value;
            }

            Console.WriteLine("No Doctor ID found.");
            return null;
        }


    }
    public class CalendarDay
    {
        public string Day { get; set; }
        public bool IsPlaceholder { get; set; }
        public List<AvailabilitySlot> AvailabilityTimes { get; set; }
        public string Badge { get; set; } 
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
        public string BranchID { get; set; }
    }
    public class DayItem
    {
        public string Day { get; set; }
        public string Date { get; set; }
    }
}