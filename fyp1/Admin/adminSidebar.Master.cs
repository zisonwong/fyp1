using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace hospital
{
    public partial class adminSideBar : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string role = Session["Role"]?.ToString();
            string doctorId = Session["DoctorId"]?.ToString();
            string nurseId = Session["nurseId"]?.ToString();

            if (string.IsNullOrEmpty(role) && Request.Cookies["Role"] != null)
            {
                role = Request.Cookies["Role"].Value.ToLower();
            }

            if (string.IsNullOrEmpty(doctorId) && Request.Cookies["DoctorID"] != null)
            {
                doctorId = Request.Cookies["DoctorID"].Value;
            }

            if (string.IsNullOrEmpty(nurseId) && Request.Cookies["nurseID"] != null)
            {
                nurseId = Request.Cookies["nurseID"].Value;
            }

            // Redirect if user is not authenticated
            if (string.IsNullOrEmpty(role) || (role == "doctor" && string.IsNullOrEmpty(doctorId)) || (role == "nurse" && string.IsNullOrEmpty(nurseId)))
            {
                Response.Redirect("~/Admin/error404.html");
                return;
            }


            if (!IsPostBack)
            {
                string userRole = Request.Cookies["Role"]?.Value.ToLower();
                string userId = userRole == "doctor" ? doctorId : nurseId;
                LoadStaffName(userRole, userId);

                if (userRole == "doctor")
                {
                    string doctorID = Request.Cookies["DoctorID"]?.Value;
                    if (!string.IsNullOrEmpty(doctorID))
                    {
                        LoadUserImage(doctorID, "doctor");
                    }
                    else
                    {
                        imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                    }
                }
                else if (userRole == "nurse")
                {
                    string nurseID = Request.Cookies["nurseID"]?.Value;
                    if (!string.IsNullOrEmpty(nurseID))
                    {
                        LoadUserImage(nurseID, "nurse");
                    }
                    else
                    {
                        imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                    }
                }
                else
                {
                    imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg"; 
                }
                lblServerTime.Text = DateTime.Now.ToString("F"); 

                string searchTerm = Request.QueryString["q"];
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    txtSearch.Text = HttpUtility.UrlDecode(searchTerm);
                }
                else
                {
                    txtSearch.Text = string.Empty;
                }
            }

        }

        protected void Timer1_Tick(object sender, EventArgs e)
        {
            lblServerTime.Text = DateTime.Now.ToString("F");
        }
        protected void lBtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                string currentUrl = Request.Url.AbsoluteUri.Split('?')[0];
                string newUrl = $"{currentUrl}?q={HttpUtility.UrlEncode(searchTerm)}";
                Response.Redirect(newUrl);
            }
            else
            {
                string currentUrl = Request.Url.AbsoluteUri.Split('?')[0];
                Response.Redirect(currentUrl);
            }
        }
        private void LoadUserImage(string userID, string userRole)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

            string query = "";
            if (userRole == "doctor")
            {
                query = "SELECT photo FROM Doctor WHERE doctorID = @UserID";
            }
            else if (userRole == "nurse")
            {
                query = "SELECT photo FROM Nurse WHERE nurseID = @UserID";
            }

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserID", userID);

                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        if (reader["photo"] != DBNull.Value)
                        {
                            byte[] photoData = (byte[])reader["photo"];
                            string base64String = Convert.ToBase64String(photoData);

                            imgDoctor.ImageUrl = "data:image/jpeg;base64," + base64String;
                        }
                        else
                        {
                            imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                        }
                    }
                    else
                    {
                        imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    imgDoctor.ImageUrl = "~/hospitalImg/defaultAvatar.jpg";
                    Console.WriteLine("Error: " + ex.Message);
                }
            }
        }
        protected void btnLogOut_Click(object sender, EventArgs e)
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Clear cookies
            if (Request.Cookies["DoctorID"] != null)
            {
                HttpCookie doctorCookie = new HttpCookie("DoctorID")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(doctorCookie);
            }

            if (Request.Cookies["nurseID"] != null)
            {
                HttpCookie nurseCookie = new HttpCookie("nurseID")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(nurseCookie);
            }

            if (Request.Cookies["Role"] != null)
            {
                HttpCookie roleCookie = new HttpCookie("Role")
                {
                    Expires = DateTime.Now.AddDays(-1)
                };
                Response.Cookies.Add(roleCookie);
            }

            Response.Redirect("~/Admin/hospitalStaffLogin.aspx");

        }
        private void LoadStaffName(string role, string userId)
        {
            if (string.IsNullOrEmpty(userId)) return;

            string connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            string query = role == "doctor"
                ? "SELECT name FROM Doctor WHERE doctorID = @UserID"
                : "SELECT name FROM Nurse WHERE nurseID = @UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    try
                    {
                        conn.Open();
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            lblStaffName.Text = result.ToString();
                        }
                        else
                        {
                            lblStaffName.Text = "Admin";
                        }
                    }
                    catch (Exception ex)
                    {
                        lblStaffName.Text = "Error loading name.";
                    }
                }
            }
        }
    }
}