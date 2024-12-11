using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class VideoCall : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string doctorId = Request.QueryString["doctorId"];
            if (string.IsNullOrEmpty(doctorId))
            {
                Response.Redirect("BranchDoctorSelection.aspx");
            }
        }
    }
}