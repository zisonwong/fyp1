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
    public partial class hospitalAddMedicaRecord : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulatePatientNames();
                txtCreateDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }

        private void PopulatePatientNames()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT patientID, name FROM Patient";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlPatientName.DataSource = dt;
                ddlPatientName.DataTextField = "name";
                ddlPatientName.DataValueField = "patientID";
                ddlPatientName.DataBind();

                ddlPatientName.Items.Insert(0, new ListItem("--Select Patient--", ""));
            }
        }

        protected void ddlPatientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string patientID = ddlPatientName.SelectedValue;

            if (!string.IsNullOrEmpty(patientID))
            {
                FetchAndDisplayPatientDetails(patientID);
            }
            else
            {
                ClearTextBoxes();
            }
        }

        private void FetchAndDisplayPatientDetails(string patientID)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT gender, bloodtype, DOB FROM Patient WHERE patientID = @patientID";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@patientID", patientID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    txtGender.Text = reader["gender"]?.ToString() ?? string.Empty;
                    txtBloodType.Text = reader["bloodtype"]?.ToString() ?? string.Empty;

                    DateTime dob;
                    if (DateTime.TryParse(reader["DOB"]?.ToString(), out dob))
                    {
                        int currentYear = DateTime.Now.Year;
                        int birthYear = dob.Year;
                        txtAge.Text = (currentYear - birthYear).ToString();
                    }
                    else
                    {
                        txtAge.Text = string.Empty;
                    }
                }
                conn.Close();
            }
        }

        private void ClearTextBoxes()
        {
            txtGender.Text = string.Empty;
            txtBloodType.Text = string.Empty;
            txtAge.Text = string.Empty;
        }

        protected void btnConfirmAddRecord_Click(object sender, EventArgs e)
        {
            string recordID = GenerateNextRecordID();
            string patientID = ddlPatientName.SelectedValue; 
            string doctorID = GetDoctorIDFromCookies();
            string problem = txtProblems.Text;
            string diagnosis = txtDiagnosis.Text;
            string treatment = txtTreatment.Text;
            DateTime recordDate = DateTime.Now;

            if (string.IsNullOrEmpty(patientID) || string.IsNullOrEmpty(doctorID))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a patient and ensure the doctor is logged in.');", true);
                return;
            }

            if(string.IsNullOrEmpty(problem) || string.IsNullOrEmpty(diagnosis) || string.IsNullOrEmpty(treatment))
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please fill in all fields.');", true);
                return;
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string insertQuery = @"INSERT INTO MedicalRecord (recordID, patientID, doctorID, problem, diagnosis, treatment, recordDate)
                                   VALUES (@recordID, @patientID, @doctorID, @problem, @diagnosis, @treatment, @recordDate)";

                SqlCommand cmd = new SqlCommand(insertQuery, conn);
                cmd.Parameters.AddWithValue("@recordID", recordID);
                cmd.Parameters.AddWithValue("@patientID", patientID);
                cmd.Parameters.AddWithValue("@doctorID", doctorID);
                cmd.Parameters.AddWithValue("@problem", problem);
                cmd.Parameters.AddWithValue("@diagnosis", diagnosis);
                cmd.Parameters.AddWithValue("@treatment", treatment);
                cmd.Parameters.AddWithValue("@recordDate", recordDate);

                try
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Medical record added successfully!');", true);

                    ClearForm();
                    ClearTextBoxes();
                    ViewState["CurrentRecordID"] = recordID;
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowModal", "showConfirmationModal();", true);
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Medical record added unsuccessfully.');", true);
                }
            }
        }

        private string GenerateNextRecordID()
        {
            string latestRecordID = "";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string selectQuery = "SELECT TOP 1 recordID FROM MedicalRecord ORDER BY recordID DESC";
                SqlCommand cmd = new SqlCommand(selectQuery, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    latestRecordID = reader["recordID"].ToString();
                }
            }

            if (!string.IsNullOrEmpty(latestRecordID) && latestRecordID.Length > 2)
            {
                string numericPart = latestRecordID.Substring(2);

                if (int.TryParse(numericPart, out int lastNumber))
                {
                    return "RC" + (lastNumber + 1).ToString("D3");
                }
            }

            return "RC001"; 
        }

        private string GetDoctorIDFromCookies()
        {
            if (Request.Cookies["DoctorID"] != null)
            {
                return Request.Cookies["DoctorID"].Value;
            }

            return null; 
        }

        private void ClearForm()
        {
            ddlPatientName.SelectedIndex = 0; 
            txtProblems.Text = "";
            txtDiagnosis.Text = "";
            txtTreatment.Text = "";
        }

        protected void btnAddPrescription_Click(object sender, EventArgs e)
        {
            Response.Redirect($"hospitalAddPrescription.aspx");
        }
    }
}