using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace fyp1.Client
{
    public partial class clientHome : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ClientScript.RegisterStartupScript(GetType(), "SliderScript", "startSlider();", true);


            }
        }

        protected void PrevBtnClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "prevImage", "prevImage();", true);
        }

        protected void NextBtnClick(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "nextImage", "nextImage();", true);
        }
    }
}