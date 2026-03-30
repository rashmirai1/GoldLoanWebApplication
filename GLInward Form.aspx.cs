using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GLInward_Form : System.Web.UI.Page
{
    #region[Declaration]
    #endregion[Declaration]

    #region[Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
          //  lblgold.Text = "Gold";
        }
    }
    #endregion[Page_Load]

    #region[ddlInwardType_SelectedIndexChanged]
    protected void ddlInwardType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlInwardType.SelectedValue == "Gold")
            {
                lblInwardType.Text = "Details of Gold:";
                
                mvOnselectionInwarttype.ActiveViewIndex = 0;
            }
            else
            {
                lblInwardType.Text = "Details of Document:";
                mvOnselectionInwarttype.ActiveViewIndex = 1;
            }
        }
        catch (Exception ex)
        { }
    }
    #endregion[ddlInwardType_SelectedIndexChanged]
}