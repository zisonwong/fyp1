using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Admin
{
    public partial class OAuthCallback : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string returnUrl = HttpContext.Current.Session["ReturnUrl"]?.ToString();

                // Add debug logging
                System.Diagnostics.Debug.WriteLine($"Return URL from session: {returnUrl}");
                System.Diagnostics.Debug.WriteLine($"Current URL: {Request.Url.AbsoluteUri}");

                if (!string.IsNullOrEmpty(returnUrl))
                {
                    Response.Redirect(returnUrl);
                }
                else
                {
                    Response.Redirect("~/Admin/hospitalDoctorChat.aspx");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"OAuth Callback Error: {ex}");
                Response.Redirect("~/Admin/hospitalDoctorChat.aspx");
            }
        }
    }
}