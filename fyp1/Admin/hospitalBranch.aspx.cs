using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace fyp1.Admin
{
    public partial class hospitalBranch : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateTimeDropdown(ddlFilterOpeningTime, "Opening Time");
                PopulateTimeDropdown(ddlFilterClosingTime, "Closing Time");
                // Use actual ClientID value
                ddlFilterOpeningTime.Attributes.Add("onchange", "filterClosingTimeOptions('" + ddlFilterOpeningTime.ClientID + "', '" + ddlFilterClosingTime.ClientID + "')");

                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadBranchData();
                }
                LoadFilteredData(ViewState["SearchTerm"] as string);
            }
        }

        private void LoadBranchData()

        {
            List<Branch> branches = new List<Branch>();

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string selectQuery = "SELECT [branchID], [name], [address], [openTime], [closeTime] FROM [Branch] WHERE status = 'Activated'";
                    SqlCommand cmd = new SqlCommand(selectQuery, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var branch = new Branch
                        {
                            BranchID = reader["branchID"].ToString(),
                            Name = reader["name"].ToString(),
                            Address = reader["address"].ToString(),
                            OpenTime = reader["openTime"].ToString(),
                            CloseTime = reader["closeTime"].ToString()
                        };
                        // Debugging output for loaded data
                        System.Diagnostics.Debug.WriteLine($"Loaded Branch: {branch.BranchID}, {branch.Name}, {branch.Address}, {branch.OpenTime}, {branch.CloseTime}");
                        branches.Add(branch);
                    }
                    if (branches.Count == 0)
                    {
                        // Log or display a message indicating no data was found
                        System.Diagnostics.Debug.WriteLine("No branches found in the database.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception message for debugging
                    System.Diagnostics.Debug.WriteLine("Error loading data: " + ex.Message);
                }
            }
            lvBranch.DataSource = branches;
            lvBranch.DataBind();
            System.Diagnostics.Debug.WriteLine("Data bound to ListView. Total branches: " + branches.Count);
        }

        protected void lbAddBranch_Click(object sender, EventArgs e)
        {
            lvBranch.EditIndex = -1; // Ensure we're not in edit mode
            lvBranch.InsertItemPosition = InsertItemPosition.FirstItem; // Show the insert template

            // Check if a search term exists in ViewState and load filtered data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm); // Load filtered data
            }
            else
            {
                LoadBranchData(); // Load all data if no search term
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }
        protected void lvBranch_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            string name = (e.Item.FindControl("txtBranchName") as TextBox).Text;
            string address = (e.Item.FindControl("txtBranchAddress") as TextBox).Text;
            string openTime = (e.Item.FindControl("ddlBranchOpeningTime") as DropDownList).SelectedValue;
            string closeTime = (e.Item.FindControl("ddlBranchClosingTime") as DropDownList).SelectedValue;
            string newBranchID = GenerateNextBranchID();
            string status = "Activated";

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(openTime) || string.IsNullOrEmpty(closeTime))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Add Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO Branch (branchID, name, address, openTime, closeTime, status) VALUES (@branchID, @name, @address, @openTime, @closeTime,@status)";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@branchID", newBranchID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@openTime", openTime);
                cmd.Parameters.AddWithValue("@closeTime", closeTime);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();
            }
            lvBranch.InsertItemPosition = InsertItemPosition.None;
            // Use the search term stored in ViewState to filter the data again
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadBranchData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }


        protected void lvBranch_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            // Remove the insert template
            lvBranch.InsertItemPosition = InsertItemPosition.None;
            lvBranch.EditIndex = -1;
            // Retrieve the search term from ViewState and load data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadBranchData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }


        private string GenerateNextBranchID()
        {
            string latestBranchID = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 branchID FROM Branch ORDER BY branchID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    latestBranchID = reader["branchID"].ToString();
                }
            }
            // Generate new ID (increment the last number)
            if (int.TryParse(latestBranchID.Substring(2), out int lastNumber))
            {
                return "BH" + (lastNumber + 1).ToString("D3");
            }
            return "BH001"; // Fallback to first ID
        }
        protected void lvBranch_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvBranch.EditIndex = e.NewEditIndex; // Set the index of the item being edited

            // Check if a search term exists in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm); // Load filtered data using the search term
            }
            else
            {
                LoadBranchData(); // Load all data if no search term
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }

        protected void lvBranch_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            string branchID = (lvBranch.Items[e.ItemIndex].FindControl("lblBranchId") as Label).Text;
            string name = (lvBranch.Items[e.ItemIndex].FindControl("txtEditBranchName") as TextBox).Text;
            string address = (lvBranch.Items[e.ItemIndex].FindControl("txtEditBranchAddress") as TextBox).Text;
            string openTime = (lvBranch.Items[e.ItemIndex].FindControl("ddlEditBranchOpeningTime") as DropDownList).SelectedValue;
            string closeTime = (lvBranch.Items[e.ItemIndex].FindControl("ddlEditBranchClosingTime") as DropDownList).SelectedValue;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(address) || string.IsNullOrEmpty(openTime) || string.IsNullOrEmpty(closeTime))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Update Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = "UPDATE Branch SET name = @name, address = @address, openTime = @openTime, closeTime = @closeTime WHERE branchID = @branchID";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@branchID", branchID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@address", address);
                cmd.Parameters.AddWithValue("@openTime", openTime);
                cmd.Parameters.AddWithValue("@closeTime", closeTime);
                cmd.ExecuteNonQuery();
            }
            lvBranch.EditIndex = -1; // Reset the edit index to end edit mode
                                     // Use the search term stored in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadBranchData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }
        protected void lvBranch_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var ddlEditBranchOpeningTime = (DropDownList)e.Item.FindControl("ddlEditBranchOpeningTime");
                var ddlEditBranchClosingTime = (DropDownList)e.Item.FindControl("ddlEditBranchClosingTime");

                if (ddlEditBranchOpeningTime != null)
                {
                    PopulateTimeDropdown(ddlEditBranchOpeningTime);
                    string openTime = DataBinder.Eval(e.Item.DataItem, "OpenTime")?.ToString();
                    openTime = DateTime.Parse(openTime).ToString("HH:mm");
                    if (!string.IsNullOrEmpty(openTime) && ddlEditBranchOpeningTime.Items.FindByValue(openTime) != null)
                    {
                        ddlEditBranchOpeningTime.SelectedValue = openTime;
                        ddlEditBranchOpeningTime.Attributes.Add("onchange", "updateClosingTimeOptions(this)");
                    }
                }

                if (ddlEditBranchClosingTime != null)
                {
                    PopulateTimeDropdown(ddlEditBranchClosingTime);
                    string closeTime = DataBinder.Eval(e.Item.DataItem, "CloseTime")?.ToString();
                    closeTime = DateTime.Parse(closeTime).ToString("HH:mm");
                    if (!string.IsNullOrEmpty(closeTime) && ddlEditBranchClosingTime.Items.FindByValue(closeTime) != null)
                    {
                        ddlEditBranchClosingTime.SelectedValue = closeTime;
                    }
                    ddlEditBranchClosingTime.Enabled = true; // Keep enabled in edit template
                }
            }
        }


        protected void lvBranch_DataBound(object sender, EventArgs e)
        {
            var insertItem = lvBranch.InsertItem;
            if (insertItem != null)
            {
                var ddlBranchOpeningTime = (DropDownList)insertItem.FindControl("ddlBranchOpeningTime");
                var ddlBranchClosingTime = (DropDownList)insertItem.FindControl("ddlBranchClosingTime");

                if (ddlBranchOpeningTime != null)
                {
                    PopulateTimeDropdown(ddlBranchOpeningTime);
                    ddlBranchOpeningTime.Attributes.Add("onchange", "updateClosingTimeOptions(this)");
                }

                if (ddlBranchClosingTime != null)
                {
                    PopulateTimeDropdown(ddlBranchClosingTime);
                    ddlBranchClosingTime.Enabled = false;  // Disable in insert template
                }
            }
        }

        // Helper method to populate dropdowns with 30-minute intervals
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


        private DataTable FilterBranchDataTable(DataTable dataTable, string searchTerm)
        {
            // Escape single quotes for safety
            string safeSearchTerm = searchTerm.Replace("'", "''");
            // Build the filter expression with all relevant fields in the Branch context
            string expression = string.Format(
                "BranchID LIKE '%{0}%' OR " +
                "Name LIKE '%{0}%' OR " +
                "Address LIKE '%{0}%'",
                safeSearchTerm);
            // Filter the rows based on the constructed expression
            DataRow[] filteredRows = dataTable.Select(expression);
            // Create a new DataTable for the filtered results and import the filtered rows
            DataTable filteredDataTable = dataTable.Clone();
            foreach (DataRow row in filteredRows)
            {
                filteredDataTable.ImportRow(row);
            }
            return filteredDataTable;
        }

        private void LoadFilteredData(string searchTerm)
        {
            string query = "SELECT [branchID], [name], [address], [openTime], [closeTime] FROM Branch WHERE 1=1 AND status = 'Activated'";

            // Initialize the SQL parameters list
            var parameters = new List<SqlParameter>();

            // Filter based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (BranchID LIKE @searchTerm OR Name LIKE @searchTerm OR Address LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }

            // Retrieve opening and closing time from ViewState
            string openingTime = ViewState["OpeningTime"] as string;
            string closingTime = ViewState["ClosingTime"] as string;

            // Apply opening time filter if selected
            if (!string.IsNullOrEmpty(openingTime))
            {
                query += " AND openTime >= @openingTime";
                parameters.Add(new SqlParameter("@openingTime", openingTime));
            }

            // Apply closing time filter if selected
            if (!string.IsNullOrEmpty(closingTime))
            {
                query += " AND closeTime <= @closingTime";
                parameters.Add(new SqlParameter("@closingTime", closingTime));
            }

            // Retrieve filtered results
            DataTable branchTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(branchTable); // Populate DataTable with filtered results
            }

            // Bind the filtered data to ListView
            lvBranch.DataSource = branchTable;
            lvBranch.DataBind();
        }

        protected void ddlFilterOpeningTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["OpeningTime"] = ddlFilterOpeningTime.SelectedValue;
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }

        protected void ddlFilterClosingTime_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["ClosingTime"] = ddlFilterClosingTime.SelectedValue;
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }
        protected void lvBranch_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "Unactivate")
            {
                string branchID = e.CommandArgument.ToString();

                if (!string.IsNullOrEmpty(branchID))
                {
                    string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            string updateQuery = "UPDATE Branch SET status = @status WHERE branchID = @branchID";
                            SqlCommand cmd = new SqlCommand(updateQuery, conn);

                            cmd.Parameters.AddWithValue("@status", "Unactivated");
                            cmd.Parameters.AddWithValue("@branchID", branchID);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                System.Diagnostics.Debug.WriteLine($"Branch {branchID} successfully updated to 'Unactivated'.");
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error updating branch status: " + ex.Message);
                        }
                    }

                    string searchTerm = ViewState["SearchTerm"] as string;
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        LoadFilteredData(searchTerm);
                    }
                    else
                    {
                        LoadBranchData();
                    }
                    LoadFilteredData(ViewState["SearchTerm"] as string);
                }
            }
        }
        protected void lvBranch_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            dpBranch.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadBranchData();
            }
        }

    }
    public class Branch
    {
        public string BranchID { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
    }
}