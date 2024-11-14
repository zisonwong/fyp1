﻿using System;
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
    public partial class hospitalAppointment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                
                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    ViewState["SearchTerm"] = searchTerm;
                    LoadFilteredData(searchTerm);
                }
                else
                {
                    ViewState["SearchTerm"] = null;
                    LoadAppointments();
                }
            }
        }
        private void LoadAppointments()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                SELECT a.appointmentID, 
                       a.patientID, 
                       a.doctorID, 
                       ab.availableFrom AS time, 
                       CONVERT(DATE, ab.availableDate) AS date,
                       a.status
                       FROM Appointment a
                       INNER JOIN Availability ab ON a.availabilityID = ab.availabilityID
                       ORDER BY ab.availableDate, ab.availableFrom";

                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                conn.Open();
                da.Fill(dt);
                conn.Close();

                lvAppointment.DataSource = dt;
                lvAppointment.DataBind();
            }
        }
       
        private void LoadFilteredData(string searchTerm)
        {
            string query = @"
        SELECT a.appointmentID, 
               a.patientID, 
               a.doctorID, 
               ab.availableFrom AS time, 
               CONVERT(DATE, ab.availableDate) AS date,
               a.status
        FROM Appointment a
        INNER JOIN Availability ab ON a.availabilityID = ab.availabilityID
        WHERE 1=1";  

            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (a.appointmentID LIKE @searchTerm OR a.patientID LIKE @searchTerm OR a.doctorID LIKE @searchTerm)";
                parameters.Add(new SqlParameter("@searchTerm", "%" + searchTerm + "%"));
            }

            query += " ORDER BY ab.availableDate, ab.availableFrom";

            DataTable branchTable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddRange(parameters.ToArray());

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                conn.Open();
                adapter.Fill(branchTable); 
            }

            lvAppointment.DataSource = branchTable;
            lvAppointment.DataBind();
        }
        protected void lvAppointment_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "SelectAppointment")
            {
                string appointmentID = e.CommandArgument.ToString();
                Response.Redirect("~/Admin/hospitalAppointmentDetails.aspx?appointmentID=" + appointmentID);
            }
        }
    }
}