using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace fyp1.Admin
{
    public partial class hospitalEditMedicalRecord : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                string recordID = Request.QueryString["recordID"];
                if (!string.IsNullOrEmpty(recordID))
                {
                    LoadMedicalRecordDetails(recordID);
                    LoadPrescriptionDetails(recordID);
                }
                else
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record ID not provided.');", true);
                }
            }
        }

        private void LoadMedicalRecordDetails(string recordID)
        {

            string query = @"
        SELECT 
            mr.recordID,
            mr.problem,
            mr.diagnosis,
            mr.treatment,
            mr.recordDate,
            p.name AS patientName,
            p.gender,
            p.bloodtype,
            DATEDIFF(YEAR, p.DOB, GETDATE()) AS age
        FROM 
            MedicalRecord mr
        INNER JOIN 
            Patient p ON mr.patientID = p.patientID
        WHERE 
            mr.recordID = @RecordID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RecordID", recordID);

                    try
                    {
                        conn.Open();
                        SqlDataReader reader = cmd.ExecuteReader();

                        if (reader.Read())
                        {
                            txtName.Text = reader["patientName"]?.ToString() ?? "N/A";
                            txtGender.Text = reader["gender"]?.ToString() == "M" ? "Male" : "Female";
                            txtBloodType.Text = reader["bloodtype"]?.ToString() ?? "N/A";
                            txtAge.Text = reader["age"]?.ToString() ?? "N/A";
                            txtCreateDate.Text = Convert.ToDateTime(reader["recordDate"]).ToString("dd/MM/yyyy");

                            txtProblems.Text = reader["problem"]?.ToString() ?? string.Empty;
                            txtDiagnosis.Text = reader["diagnosis"]?.ToString() ?? string.Empty;
                            txtTreatment.Text = reader["treatment"]?.ToString() ?? string.Empty;
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record not found.');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
        }
        private void LoadPrescriptionDetails(string recordID)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string query = @"
        SELECT 
            p.prescriptionID,
            m.name AS medicineName,
            m.type AS medicineType,
            p.quantity,
            p.details
        FROM 
            Prescription p
        INNER JOIN 
            Medicine m ON p.medicineID = m.medicineID
        WHERE 
            p.recordID = @RecordID
        ORDER BY 
            p.prescriptionID ASC";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RecordID", recordID);

                    try
                    {
                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Bind data to ListView
                        lvPrescriptionDetails.DataSource = dt;
                        lvPrescriptionDetails.DataBind();
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
        }
        protected void lvPrescriptionDetails_ItemDeleting(object sender, ListViewDeleteEventArgs e)
        {
            string prescriptionID = lvPrescriptionDetails.DataKeys[e.ItemIndex].Value.ToString();

            if (!string.IsNullOrEmpty(prescriptionID))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "DELETE FROM Prescription WHERE prescriptionID = @PrescriptionID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PrescriptionID", prescriptionID);

                        try
                        {
                            conn.Open();
                            cmd.ExecuteNonQuery();
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Prescription deleted successfully.');", true);
                        }
                        catch (Exception ex)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                        }
                    }
                }

                string recordID = Request.QueryString["recordID"];
                LoadPrescriptionDetails(recordID);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Prescription ID not found.');", true);
            }
        }
        protected void btnEditRecord_Click(object sender, EventArgs e)
        {
            string recordID = Request.QueryString["recordID"];
            if (!string.IsNullOrEmpty(recordID))
            {
                UpdateMedicalRecord(recordID);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record ID not provided.');", true);
            }
        }

        private void UpdateMedicalRecord(string recordID)
        {
            string query = @"
        UPDATE MedicalRecord
        SET 
            problem = @Problem,
            diagnosis = @Diagnosis,
            treatment = @Treatment
        WHERE 
            recordID = @RecordID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Problem", txtProblems.Text.Trim());
                    cmd.Parameters.AddWithValue("@Diagnosis", txtDiagnosis.Text.Trim());
                    cmd.Parameters.AddWithValue("@Treatment", txtTreatment.Text.Trim());
                    cmd.Parameters.AddWithValue("@RecordID", recordID);

                    try
                    {
                        conn.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Record updated successfully.');", true);
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Update failed. Record not found.');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "alert", $"alert('Error: {ex.Message}');", true);
                    }
                }
            }
        }
    }
}