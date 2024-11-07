using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1
{
    public partial class hospitalDepartment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string searchTerm = Request.QueryString["q"];
                string selectedBranchID = Request.QueryString["branch"]; // Add this to handle branch filter
                ViewState["SelectedBranch"] = selectedBranchID;
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadDepartmentData();
                }
                PopulateBranchFilterDropdown();
                LoadDataWithFilters(searchTerm, selectedBranchID);
            }


        }
        private void LoadDepartmentData()
        {
            List<Department> departments = new List<Department>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string selectQuery = @"
                        SELECT 
                            d.departmentID, 
                            d.name AS DepartmentName, 
                            d.branchID, 
                            b.name AS BranchName, 
                            COUNT(DISTINCT dr.doctorID) AS TotalDoctors
                        FROM Department d
                        LEFT JOIN Branch b ON d.branchID = b.branchID
                        LEFT JOIN Doctor dr ON d.departmentID = dr.departmentID
                        GROUP BY d.departmentID, d.name, d.branchID, b.name";

                    SqlCommand cmd = new SqlCommand(selectQuery, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var department = new Department
                        {
                            DepartmentID = reader["departmentID"].ToString(),
                            Name = reader["DepartmentName"].ToString(),
                            BranchID = reader["branchID"].ToString(),
                            BranchName = reader["BranchName"].ToString(),
                            TotalDoctors = reader["TotalDoctors"] != DBNull.Value ? (int)reader["TotalDoctors"] : 0
                        };
                        departments.Add(department);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading data: " + ex.Message);
                }
            }
            lvDepartment.DataSource = departments;
            lvDepartment.DataBind();
        }
        // Modified method to filter DataTable for Departments
        private DataTable FilterDepartmentDataTable(DataTable dataTable, string searchTerm)
        {
            // Escape single quotes for safety
            string safeSearchTerm = searchTerm.Replace("'", "''");
            // Build the filter expression considering the Department context
            string expression = string.Format(
                "DepartmentID LIKE '%{0}%' OR " +
                "BranchID LIKE '%{0}%' OR " +
                "Name LIKE '%{0}%'",
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
        // Modified method to load filtered data for Departments
        private void LoadFilteredData(string searchTerm)
        {
            // Construct the query to join Department table with Branch and count the number of Doctors
            string query = @"
    SELECT d.departmentID, d.branchID, d.name,
           b.name as BranchName, COUNT(dr.doctorID) as TotalDoctors
    FROM Department d
    LEFT JOIN Branch b ON d.branchID = b.branchID
    LEFT JOIN Doctor dr ON d.departmentID = dr.departmentID
    WHERE 1=1";
            // Initialize the SQL parameters list
            var parameters = new List<SqlParameter>();
            // Filter based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (d.departmentID LIKE @searchTerm OR d.branchID LIKE @searchTerm OR d.name LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }
            // Add GROUP BY clause to count doctors per department
            query += " GROUP BY d.departmentID, d.branchID, d.name, b.name";
            // Retrieve the filtered results
            DataTable departmentTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(departmentTable); // Populate DataTable with filtered results
            }
            // Bind the filtered data to ListView
            lvDepartment.DataSource = departmentTable;
            lvDepartment.DataBind();
        }

        protected void lbAddDepartment_Click(object sender, EventArgs e)
        {
            lvDepartment.EditIndex = -1; // Ensure we're not in edit mode
            lvDepartment.InsertItemPosition = InsertItemPosition.FirstItem; // Show the insert template

            // Check if a search term exists in ViewState and load filtered data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            string selectBranchID = ViewState["SelectedBranch"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else if (!string.IsNullOrEmpty(selectBranchID))
            {
                LoadDataWithFilters(searchTerm, selectBranchID);
            }
            else
            {
                LoadDepartmentData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            LoadDataWithFilters(searchTerm, selectBranchID);

            DropDownList ddlAssignToBranch = lvDepartment.InsertItem.FindControl("ddlAssignToBranch") as DropDownList;
            if (ddlAssignToBranch != null)
            {
                ddlAssignToBranch.DataSource = GetAllBranches(); // Call your method to get the branches
                ddlAssignToBranch.DataValueField = "BranchId"; // Assuming "BranchId" is the column name in your data
                ddlAssignToBranch.DataTextField = "BranchId"; 
                ddlAssignToBranch.DataBind();

                // Optionally, insert a default "All Branches" item
                // ddlAssignToBranch.Items.Insert(0, new ListItem("All Branches", "-1"));
                // Set the initial label text based on the first item in the dropdown
                if (ddlAssignToBranch.Items.Count > 0)
                {
                    string selectedBranchID = ddlAssignToBranch.SelectedValue;
                    DataTable branches = GetAllBranches();
                    DataRow[] branchRows = branches.Select($"BranchID = '{selectedBranchID}'");

                    ListViewItem item = (ListViewItem)ddlAssignToBranch.NamingContainer;
                    Label lblBranchName = item.FindControl("lblBranchName") as Label;
                    if (branchRows.Length > 0)
                    {
                        lblBranchName.Text = branchRows[0]["BranchName"].ToString();
                    }
                    else
                    {
                        lblBranchName.Text = "--Select Branch--";
                    }
                }
            }
        }

        // Method to populate the DropDownList with branchIDs
        protected void lvDepartment_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                // Find the DropDownList and bind the branches
                DropDownList ddlAssignToBranch = (DropDownList)e.Item.FindControl("ddlEditAssignToBranch");
                if (ddlAssignToBranch != null)
                {
                    ddlAssignToBranch.DataSource = GetAllBranches(); // Call the method to get branch data
                    ddlAssignToBranch.DataValueField = "branchID";   // Value to be used
                    ddlAssignToBranch.DataTextField = "BranchName";  // Text to display
                    ddlAssignToBranch.DataBind();
                    // Get the current data item as Department and set the selected value of the DropDownList
                    Department department = e.Item.DataItem as Department;
                    if (department != null)
                    {
                        ddlAssignToBranch.SelectedValue = department.BranchID;
                    }
                }
            }
            else if (e.Item.ItemType == ListViewItemType.InsertItem)
            {
                DropDownList ddlAssignToBranch = (DropDownList)e.Item.FindControl("ddlAssignToBranch");
                if (ddlAssignToBranch != null)
                {
                    ddlAssignToBranch.DataSource = GetAllBranches(); // Call the method to get branch data
                    ddlAssignToBranch.DataValueField = "branchID";   // Value to be used
                    ddlAssignToBranch.DataTextField = "BranchName";  // Text to display
                    ddlAssignToBranch.DataBind();
                    ddlAssignToBranch.Items.Insert(0, new ListItem("--Select Branch--", "")); // Optional default item
                }
            }
        }
        protected void lvDepartment_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            // Remove the insert template
            lvDepartment.InsertItemPosition = InsertItemPosition.None;
            lvDepartment.EditIndex = -1;
            // Retrieve the search term from ViewState and load data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            string selectBranchID = ViewState["SelectedBranch"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadDepartmentData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            LoadDataWithFilters(searchTerm, selectBranchID);
        }

        private string GenerateNextDepartmentID()
        {
            string latestBranchID = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 departmentID FROM Department ORDER BY departmentID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    latestBranchID = reader["departmentID"].ToString();
                }
            }
            // Generate new ID (increment the last number)
            if (int.TryParse(latestBranchID.Substring(2), out int lastNumber))
            {
                return "DP" + (lastNumber + 1).ToString("D3");
            }
            return "DP001"; // Fallback to first ID
        }
        private DataTable GetAllBranches()
        {
            DataTable branchTable = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    string query = @"SELECT branchID, name as BranchName FROM Branch ORDER BY branchID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    conn.Open();
                    adapter.Fill(branchTable);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error getting branches: " + ex.Message);
                }
            }
            return branchTable;
        }

        protected void lvDepartment_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            string departmentName = (e.Item.FindControl("txtDepartmentName") as TextBox).Text;
            string branchID = (e.Item.FindControl("ddlAssignToBranch") as DropDownList).SelectedValue;
            string newDepartmentID = GenerateNextDepartmentID();
            if (string.IsNullOrEmpty(departmentName) || string.IsNullOrEmpty(branchID))
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
                string insertQuery = "INSERT INTO Department (departmentID, name, branchID) VALUES (@departmentID, @name, @branchID)";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@departmentID", newDepartmentID);
                cmd.Parameters.AddWithValue("@name", departmentName);
                cmd.Parameters.AddWithValue("@branchID", branchID);
                cmd.ExecuteNonQuery();
            }
            lvDepartment.InsertItemPosition = InsertItemPosition.None;
            // Use the search term stored in ViewState to filter the data again
            string searchTerm = ViewState["SearchTerm"] as string;
            string selectBranchID = ViewState["SelectedBranch"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadDepartmentData();
            }
            LoadDataWithFilters(searchTerm, selectBranchID);
        }

        protected void lvDepartment_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvDepartment.EditIndex = e.NewEditIndex; // Set the index of the item being edited

            // Check if a search term exists in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            string selectBranchID = ViewState["SelectedBranch"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm); // Load filtered data using the search term
            }
            else
            {
                LoadDepartmentData(); // Load all data if no search term
            }
            LoadDataWithFilters(searchTerm, selectBranchID);

            DropDownList ddlEditAssignToBranch = lvDepartment.EditItem.FindControl("ddlEditAssignToBranch") as DropDownList;
            if (ddlEditAssignToBranch != null)
            {
                // Get the original branch ID for the current item
                Label lblOriginalBranchID = lvDepartment.EditItem.FindControl("lblBranchID") as Label;
                string originalBranchID = lblOriginalBranchID != null ? lblOriginalBranchID.Text : "";

                ddlEditAssignToBranch.DataSource = GetAllBranches();
                ddlEditAssignToBranch.DataValueField = "BranchID";
                ddlEditAssignToBranch.DataTextField = "BranchID";
                ddlEditAssignToBranch.DataBind();

                // Set the dropdown to the original branch ID
                if (!string.IsNullOrEmpty(originalBranchID))
                {
                    ddlEditAssignToBranch.SelectedValue = originalBranchID;
                }
            }
        }

        protected void lvDepartment_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            // Retrieve department data from the edit controls in the ListView
            string departmentID = (lvDepartment.Items[e.ItemIndex].FindControl("lblDepartmentId") as Label).Text;
            string name = (lvDepartment.Items[e.ItemIndex].FindControl("txtEditDepartmentName") as TextBox).Text;
            string branchID = (lvDepartment.Items[e.ItemIndex].FindControl("ddlEditAssignToBranch") as DropDownList).SelectedValue;

            // Validate that all fields are filled in
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(branchID))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Update Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }

            // Update the Department table in the database
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = "UPDATE Department SET Name = @name, BranchID = @branchID WHERE DepartmentID = @departmentID";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@departmentID", departmentID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@branchID", branchID);
                cmd.ExecuteNonQuery();
            }

            // Reset the edit index to exit edit mode
            lvDepartment.EditIndex = -1;

            // Reload data based on any search term stored in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            string selectBranchID = ViewState["SelectedBranch"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadDepartmentData();
            }
            LoadDataWithFilters(searchTerm, selectBranchID);
        }

        protected void ddlAssignToBranch_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlAssignToBranch = sender as DropDownList;
            if (ddlAssignToBranch != null)
            {
                ListViewItem item = (ListViewItem)ddlAssignToBranch.NamingContainer;
                Label lblBranchName = item.FindControl("lblBranchName") as Label;
                if (lblBranchName != null)
                {
                    string selectedBranchID = ddlAssignToBranch.SelectedValue;
                    // Retrieve branch name based on selectedBranchID
                    DataTable branches = GetAllBranches();
                    DataRow[] branchRows = branches.Select($"branchID = '{selectedBranchID}'");
                    if (branchRows.Length > 0)
                    {
                        lblBranchName.Text = branchRows[0]["BranchName"].ToString();
                    }
                    else
                    {
                        lblBranchName.Text = "--Select Branch--";
                    }
                }
            }
        }
        private void PopulateBranchFilterDropdown()
        {
            DataTable branches = GetAllBranches();
            ddlFilterBranchName.DataSource = branches;
            ddlFilterBranchName.DataValueField = "branchID";
            ddlFilterBranchName.DataTextField = "BranchName";
            ddlFilterBranchName.DataBind();
            ddlFilterBranchName.Items.Insert(0, new ListItem("All Branch", "")); // Optional: Add a default item
        }

        protected void ddlFilterBranchName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedBranchID = ddlFilterBranchName.SelectedValue;
            ViewState["SelectedBranch"] = selectedBranchID;
            string searchTerm = ViewState["SearchTerm"] as string;
            LoadDataWithFilters(searchTerm, selectedBranchID);
        }

        private void LoadDataWithFilters(string searchTerm, string selectedBranchID)
        {
            string query = @"
    SELECT d.departmentID, d.name, d.branchID, b.name AS BranchName, COUNT(DISTINCT dr.doctorID) AS TotalDoctors
    FROM Department d
    LEFT JOIN Branch b ON d.branchID = b.branchID
    LEFT JOIN Doctor dr ON d.departmentID = dr.departmentID
    WHERE 1=1";
            var parameters = new List<SqlParameter>();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query += " AND (d.departmentID LIKE @searchTerm OR d.name LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }
            if (!string.IsNullOrWhiteSpace(selectedBranchID))
            {
                query += " AND d.branchID = @selectedBranchID";
                parameters.Add(new SqlParameter("@selectedBranchID", selectedBranchID));
            }
            query += " GROUP BY d.departmentID, d.name, d.branchID, b.name";
            DataTable departmentTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(departmentTable);
            }
            lvDepartment.DataSource = departmentTable;
            lvDepartment.DataBind();
        }


        public class Department
        {
            public string DepartmentID { get; set; }
            public string Name { get; set; }
            public string BranchID { get; set; }
            public string BranchName { get; set; }
            public int? TotalDoctors { get; set; }
        }

    }
}