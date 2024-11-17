using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static fyp1.Admin.hospitalDepartment;

namespace fyp1.Admin
{
    public partial class hospitalMedicine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMedicineTypes();
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadMedicineData();
                }
                LoadFilteredData(ViewState["SearchTerm"] as string);
                ReloadMedicineData();
            }
        }
        private void LoadMedicineData()
        {
            List<Medicine> medicines = new List<Medicine>();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT medicineID, name, quantity, type, description, dosage FROM Medicine";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var medicine = new Medicine
                        {
                            MedicineID = reader["medicineID"].ToString(),
                            Name = reader["name"].ToString(),
                            Quantity = Convert.ToInt32(reader["quantity"]), // Convert to int
                            Type = reader["type"].ToString(),
                            Description = reader["description"].ToString(),
                            Dosage = reader["dosage"].ToString(),
                            
                        };
                        medicines.Add(medicine);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading data: " + ex.Message);
                }
            }
            lvMedicine.DataSource = medicines;
            lvMedicine.DataBind();
        }
        private DataTable FilterMedicineDataTable(DataTable dataTable, string searchTerm)
        {
            // Escape single quotes for safety
            string safeSearchTerm = searchTerm.Replace("'", "''");
            // Build the filter expression considering the Medicine context
            string expression = string.Format(
                "MedicineID LIKE '%{0}%' OR " +
                "Name LIKE '%{0}%' OR " +
                "Type LIKE '%{0}%'",
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

        protected void lbAddMedicine_Click(object sender, EventArgs e)
        {
            lvMedicine.EditIndex = -1; // Ensure we're not in edit mode
            lvMedicine.InsertItemPosition = InsertItemPosition.FirstItem; // Show the insert template
                                                                          // Check if a search term exists in ViewState and load filtered data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm); // Load filtered data
            }
            else
            {
                LoadMedicineData(); // Load all data if no search term
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            ReloadMedicineData();
            ListViewItem insertItem = lvMedicine.InsertItem; // Get the insert item
            if (insertItem != null)
            {
                DropDownList ddlMedicineType = insertItem.FindControl("ddlMedicineType") as DropDownList;
                if (ddlMedicineType != null)
                {
                    PopulateMedicineTypeDropDown(ddlMedicineType); // Populate the dropdown
                }
            }
        }


        private void LoadFilteredData(string searchTerm)
        {
            string query = "SELECT medicineID, name, quantity, type, description, dosage FROM Medicine WHERE 1=1";

            // Initialize the SQL parameters list
            var parameters = new List<SqlParameter>();

            // Filter based on search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (MedicineID LIKE @searchTerm OR Name LIKE @searchTerm OR Type LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
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
            lvMedicine.DataSource = branchTable;
            lvMedicine.DataBind();
        }
        private string GenerateNextMedicineID()
        {
            string latestMedicineID = "";
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 medicineID FROM Medicine ORDER BY medicineID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    latestMedicineID = reader["medicineID"].ToString();
                }
            }

            // Check if we have a valid ID from the table, otherwise start with "MD001"
            if (!string.IsNullOrEmpty(latestMedicineID) && int.TryParse(latestMedicineID.Substring(2), out int lastNumber))
            {
                return "MD" + (lastNumber + 1).ToString("D3");
            }

            return "MD001"; // Start with first ID if table is empty
        }

        private void PopulateMedicineTypeDropDown(DropDownList ddl)
        {
            // Clear existing items
            ddl.Items.Clear();
            // Manually add predefined medicine types
            ddl.Items.Add(new ListItem("Tablet", "Tablet"));
            ddl.Items.Add(new ListItem("Capsule", "Capsule"));
            ddl.Items.Add(new ListItem("Liquid", "Liquid"));
            ddl.Items.Add(new ListItem("Injection", "Injection"));
            ddl.Items.Add(new ListItem("Ointment", "Ointment"));
        }

        protected void lvMedicine_ItemInserting(object sender, ListViewInsertEventArgs e)
        {
            string name = (e.Item.FindControl("txtMedicineName") as TextBox).Text;
            string quantityText = (e.Item.FindControl("txtQuantity") as TextBox).Text;
            string type = (e.Item.FindControl("ddlMedicineType") as DropDownList).SelectedValue;
            string description = (e.Item.FindControl("txtDescription") as TextBox).Text;
            string dosage = (e.Item.FindControl("txtDosage") as TextBox).Text;
            string newMedicineID = GenerateNextMedicineID();
            // Ensure proper parsing of integers for quantity
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(dosage) 
                || string.IsNullOrEmpty(quantityText))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Add Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }

            if(!int.TryParse(quantityText, out int quantity))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Add Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in quantity with number only'));",
                    true);
                return;
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string insertQuery = "INSERT INTO Medicine (medicineID, name, quantity, type, description, dosage) VALUES (@medicineID, @name, @quantity, @type, @description, @dosage)";
                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@medicineID", newMedicineID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@dosage", dosage);
                cmd.ExecuteNonQuery();
            }
            lvMedicine.InsertItemPosition = InsertItemPosition.None;
            LoadMedicineTypes();
            // Use the search term stored in ViewState to filter the data again
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadMedicineData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
        }

        protected void lvMedicine_ItemCanceling(object sender, ListViewCancelEventArgs e)
        {
            // Remove the insert template
            lvMedicine.InsertItemPosition = InsertItemPosition.None;
            lvMedicine.EditIndex = -1;
            // Retrieve the search term from ViewState and load data accordingly
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadMedicineData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            ReloadMedicineData();
        }
        protected void lvBranch_ItemEditing(object sender, ListViewEditEventArgs e)
        {
            lvMedicine.EditIndex = e.NewEditIndex; // Set the index of the item being edited

            // Check if a search term exists in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm); // Load filtered data using the search term
            }
            else
            {
                LoadMedicineData(); // Load all data if no search term
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            ReloadMedicineData();
        }
        protected void lvMedicine_ItemUpdating(object sender, ListViewUpdateEventArgs e)
        {
            string medicineID = (lvMedicine.Items[e.ItemIndex].FindControl("lblMedicineId") as Label).Text;
            string name = (lvMedicine.Items[e.ItemIndex].FindControl("txtEditMedicineName") as TextBox).Text;
            string type = (lvMedicine.Items[e.ItemIndex].FindControl("ddlEditMedicineType") as DropDownList).SelectedValue;
            string quantityText = (lvMedicine.Items[e.ItemIndex].FindControl("txtEditQuantity") as TextBox).Text;
            string description = (lvMedicine.Items[e.ItemIndex].FindControl("txtEditDescription") as TextBox).Text;
            string dosage = (lvMedicine.Items[e.ItemIndex].FindControl("txtEditDosage") as TextBox).Text;
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type) || string.IsNullOrEmpty(quantityText) || string.IsNullOrEmpty(description) || string.IsNullOrEmpty(dosage))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Update Failed",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in all fields'));",
                    true);
                return;
            }
            if (!int.TryParse(quantityText, out int quantity))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "Invalid Quantity",
                    "document.addEventListener('DOMContentLoaded', ()=> alert('Please fill in quantity with number only'));",
                    true);
                return;
            }
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string updateQuery = "UPDATE Medicine SET name = @name, type = @type, quantity = @quantity, description = @description, dosage = @dosage WHERE medicineID = @medicineID";
                SqlCommand cmd = new SqlCommand(updateQuery, conn);
                cmd.Parameters.AddWithValue("@medicineID", medicineID);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@dosage", dosage);
                cmd.ExecuteNonQuery();
            }
            lvMedicine.EditIndex = -1; // Reset the edit index to end edit mode
            LoadMedicineTypes();
            // Use the search term stored in ViewState
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                LoadMedicineData();
            }
            LoadFilteredData(ViewState["SearchTerm"] as string);
            ReloadMedicineData();
        }

        protected void lvMedicine_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem) {
                var ddlEditMedicineType = e.Item.FindControl("ddlEditMedicineType") as DropDownList;
                if (ddlEditMedicineType != null)
                {
                    PopulateMedicineTypeDropDown(ddlEditMedicineType); // Populate dropdown with options
                    string medicineType = DataBinder.Eval(e.Item.DataItem, "Type")?.ToString();
                    if(!string.IsNullOrEmpty(medicineType) && ddlEditMedicineType.Items.FindByValue(medicineType) != null){
                        ddlEditMedicineType.SelectedValue = medicineType;
                    }
                }
            }
        }
        private void LoadMedicineTypes()
        {
            string query = "SELECT DISTINCT type FROM Medicine WHERE type IS NOT NULL";
            DataTable typesTable = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(typesTable);
            }

            ddlFilterMedicineType.DataSource = typesTable;
            ddlFilterMedicineType.DataTextField = "type";
            ddlFilterMedicineType.DataValueField = "type";
            ddlFilterMedicineType.DataBind();

            // Add a default "Select Type" option at the top
            ddlFilterMedicineType.Items.Insert(0, new ListItem("Select Type", ""));
        }

        protected void ddlFilterMedicineType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedType = ddlFilterMedicineType.SelectedValue;
            ViewState["FilterType"] = selectedType; // Store the selected type in ViewState

            if (!string.IsNullOrEmpty(selectedType))
            {
                LoadFilteredDataByType(selectedType);
            }
            else
            {
                LoadMedicineData();
            }
        }

        private void LoadFilteredDataByType(string type)
        {
            string query = "SELECT medicineID, name, quantity, type, description, dosage FROM Medicine WHERE type = @type";
            DataTable filteredDataTable = new DataTable();
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.Add(new SqlParameter("@type", type));
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(filteredDataTable);
            }

            lvMedicine.DataSource = filteredDataTable;
            lvMedicine.DataBind();
        }

        private void ReloadMedicineData()
        {
            // Use filter type and search term to reload data appropriately
            string searchTerm = ViewState["SearchTerm"] as string;
            if (!string.IsNullOrEmpty(searchTerm))
            {
                LoadFilteredData(searchTerm);
            }
            else
            {
                string filterType = ViewState["FilterType"] as string;
                if (!string.IsNullOrEmpty(filterType))
                {
                    LoadFilteredDataByType(filterType);
                }
                else
                {
                    LoadMedicineData();
                }
            }
        }

        public class Medicine
        {
            public string MedicineID { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
            public string Type { get; set; }
            public string Description { get; set; }
            public string Dosage { get; set; }

        }
    }
}