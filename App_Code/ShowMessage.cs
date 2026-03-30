using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

public class ShowMessage
{
    public static void Show(string message)
    {
        string cleanMessage = message;
        string jv = "<script type=\"text/javascript\">alert('" + cleanMessage + "');</script>";
        Page page = HttpContext.Current.CurrentHandler as Page;
        //ScriptManager.RegisterClientScriptBlock(this, typeof(Page), "alert", jv, false);
        ScriptManager.RegisterStartupScript(page, typeof(Page), "alert", jv, false);
    }
}
