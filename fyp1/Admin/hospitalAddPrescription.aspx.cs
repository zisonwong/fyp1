using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace fyp1.Admin
{
    public partial class hospitalAddPrescription : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadMedicines();
                LoadRecordIDs();
            }
        }
        private void LoadMedicines()
        {
            string query = "SELECT medicineID, name, quantity, type, dosage FROM Medicine";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    rptPrescription.DataSource = dt;
                    rptPrescription.DataBind();
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", $"alert('Error: {ex.Message}');", true);
                }
            }
        }
        protected void rptPrescription_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Prescribe")
            {
                string medicineID = e.CommandArgument.ToString(); // Get medicine ID
                string query = "SELECT * FROM Medicine WHERE medicineID = @medicineID";

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@medicineID", medicineID);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }

                if (dt.Rows.Count > 0)
                {
                    txtPatientName.Text = "";
                    txtMedicineName.Text = dt.Rows[0]["name"].ToString();
                    ViewState["SelectedMedicineID"] = medicineID;
                }

                ClientScript.RegisterStartupScript(this.GetType(), "ShowModal", "showModal();", true);
            }
        }

        private void LoadRecordIDs()
        {
            string query = @"
        SELECT MR.recordID, P.name AS PatientName
                FROM MedicalRecord MR
                JOIN Patient P ON MR.patientID = P.patientID
                LEFT JOIN Prescription PR ON MR.recordID = PR.recordID
                ORDER BY MR.recordID ASC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    dt.Load(reader);

                    ddlRecordID.DataSource = dt;
                    ddlRecordID.DataTextField = "recordID";
                    ddlRecordID.DataValueField = "recordID";
                    ddlRecordID.DataBind();

                    // Add default option
                    ddlRecordID.Items.Insert(0, new ListItem("-- Select Record --", ""));
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", $"alert('Error: {ex.Message}');", true);
                }
            }
        }


        protected void ddlRecordID_SelectedIndexChanged(object sender, EventArgs e)
        {
            string recordID = ddlRecordID.SelectedValue;

            if (!string.IsNullOrEmpty(recordID))
            {
                string query = "SELECT P.name AS PatientName FROM MedicalRecord MR JOIN Patient P ON MR.patientID = P.patientID WHERE MR.recordID = @recordID";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@recordID", recordID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtPatientName.Text = reader["PatientName"].ToString(); 
                        }
                        else
                        {
                            txtPatientName.Text = string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "ErrorAlert", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
            else
            {
                txtPatientName.Text = string.Empty; 
            }
        }


        private string GenerateNextPrescriptionID()
        {
            string latestPrescriptionID = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 prescriptionID FROM Prescription ORDER BY prescriptionID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    latestPrescriptionID = reader["prescriptionID"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(latestPrescriptionID) && latestPrescriptionID.Length > 2)
            {
                string numericPart = latestPrescriptionID.Substring(2);

                if (int.TryParse(numericPart, out int lastNumber))
                {
                    return "PT" + (lastNumber + 1).ToString("D3");
                }
            }

            return "PT001";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string prescriptionID = GenerateNextPrescriptionID();
            string recordID = ddlRecordID.SelectedValue;
            string medicineID = ViewState["SelectedMedicineID"]?.ToString(); 
            string details = txtDetails.Text;
            int quantity = 0;

            if (string.IsNullOrEmpty(recordID) || string.IsNullOrEmpty(medicineID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", "alert('Please select a record and medicine.');", true);
                return;
            }

            if (string.IsNullOrEmpty(details))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", "alert('Please fill in details.');", true);
                return;
            }

            if (!int.TryParse(txtQuantity.Text, out quantity) || quantity <= 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "ValidationError", "alert('Please enter a valid quantity.');", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlTransaction transaction = conn.BeginTransaction(); 
                try
                {
                    string insertQuery = @"
                INSERT INTO Prescription (prescriptionID, recordID, medicineID, details, quantity) 
                VALUES (@prescriptionID, @recordID, @medicineID, @details, @quantity)";
                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn, transaction))
                    {
                        insertCmd.Parameters.AddWithValue("@prescriptionID", prescriptionID);
                        insertCmd.Parameters.AddWithValue("@recordID", recordID);
                        insertCmd.Parameters.AddWithValue("@medicineID", medicineID);
                        insertCmd.Parameters.AddWithValue("@details", details);
                        insertCmd.Parameters.AddWithValue("@quantity", quantity);
                        insertCmd.ExecuteNonQuery();
                    }

                    string updateQuery = @"
                UPDATE Medicine 
                SET quantity = quantity - @quantity 
                WHERE medicineID = @medicineID AND quantity >= @quantity";
                    using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn, transaction))
                    {
                        updateCmd.Parameters.AddWithValue("@quantity", quantity);
                        updateCmd.Parameters.AddWithValue("@medicineID", medicineID);
                        int rowsAffected = updateCmd.ExecuteNonQuery();

                        if (rowsAffected == 0)
                        {
                            transaction.Rollback();
                            ClientScript.RegisterStartupScript(this.GetType(), "StockError", "alert('Error: Insufficient stock for the selected medicine.');", true);
                            return;
                        }
                    }

                    transaction.Commit();
                    ClientScript.RegisterStartupScript(this.GetType(), "SaveSuccess", "alert('Prescription added successfully.');", true);

                    ClearForm();
                    LoadMedicines();
                    LoadRecordIDs();
                }
                catch (Exception ex)
                {
                    transaction.Rollback(); 
                    ClientScript.RegisterStartupScript(this.GetType(), "SaveError", $"alert('Error: {ex.Message}');", true);
                }
            }
        }

        private void ClearForm()
        {
            ddlRecordID.SelectedIndex = 0;
            txtPatientName.Text = string.Empty;
            txtMedicineName.Text = string.Empty;
            txtDetails.Text = string.Empty;
            txtQuantity.Text = string.Empty;
        }
    }
}