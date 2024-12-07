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
            if (!string.IsNullOrEmpty(Request.QueryString["logout"]) && Request.QueryString["logout"] == "true")
            {
                Session["UserRole"] = null;
                Session["UserID"] = null;

                if (Request.Cookies["UserRole"] != null)
                {
                    HttpCookie roleCookie = new HttpCookie("UserRole")
                    {
                        Expires = DateTime.Now.AddDays(-1) 
                    };
                    Response.Cookies.Add(roleCookie);
                }

                if (Request.Cookies["UserID"] != null)
                {
                    HttpCookie idCookie = new HttpCookie("UserID")
                    {
                        Expires = DateTime.Now.AddDays(-1) 
                    };
                    Response.Cookies.Add(idCookie);
                }

                Response.Redirect("~/Admin/hospitalStaffLogin.aspx");
            }
            if (!IsPostBack)
            {
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

    }

}