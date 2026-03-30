using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

public partial class UserControl_AutoRedirect : System.Web.UI.UserControl
{
    public string LoginDate;
    public string ExpressDate;

    protected void Page_Load(object sender, EventArgs e)
    {
        // Check session is expire or timeout.
        if (Session["username"] == null)
        {
            Response.Redirect("Default.aspx?info=0");
        }

        // Get user login time or last activity time.
        DateTime date = DateTime.Now;
        LoginDate = date.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
        int sessionTimeout = Session.Timeout;
        DateTime dateExpress = date.AddMinutes(sessionTimeout);
        ExpressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
    }
}