using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Globalization;


/// <summary>
/// Summary description for GlobalSettings
/// </summary>
public class GlobalSettings
{
    public GlobalSettings()
    {
        //
        // TODO: Add constructor logic here
        //
    }
    public void CheckAEDControlSettings(string btnStatus, Button btnEditId, Button btnSaveId, Button btnDeleteId, Button btnViewId, Button btnCancelId)
    {
        if (btnStatus == "Save")
        {
            btnEditId.Enabled = true;
            btnEditId.CssClass = "css_btn_class";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = true;
            btnViewId.CssClass = "css_btn_class";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";


        }
        else if (btnStatus == "Edit")
        {
            btnEditId.Enabled = false;
            btnEditId.CssClass = "btnenable";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = true;
            btnDeleteId.CssClass = "css_btn_class";
            btnViewId.Enabled = false;
            btnViewId.CssClass = "btnenable";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
        else if (btnStatus == "Delete")
        {
            btnEditId.Enabled = true;
            btnEditId.CssClass = "css_btn_class";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = true;
            btnViewId.CssClass = "css_btn_class";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
        else if (btnStatus == "View")
        {
            btnEditId.Enabled = false;
            btnEditId.CssClass = "btnenable";
            btnSaveId.Enabled = false;
            btnSaveId.CssClass = "btnenable";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = false;
            btnViewId.CssClass = "btnenable";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
        else if (btnStatus == "Cancel")
        {
            btnEditId.Enabled = true;
            btnEditId.CssClass = "css_btn_class";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = true;
            btnViewId.CssClass = "css_btn_class";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
        else if (btnStatus == "Search")
        {
            btnEditId.Enabled = false;
            btnEditId.CssClass = "btnenable";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = false;
            btnViewId.CssClass = "btnenable";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
        else if (btnStatus == "")
        {

            btnEditId.Enabled = true;
            btnEditId.CssClass = "css_btn_class";
            btnSaveId.Enabled = true;
            btnSaveId.CssClass = "css_btn_class";
            btnDeleteId.Enabled = false;
            btnDeleteId.CssClass = "btnenable";
            btnViewId.Enabled = true;
            btnViewId.CssClass = "css_btn_class";
            btnCancelId.Enabled = true;
            btnCancelId.CssClass = "css_btn_class";
        }
    }
    public void ShowNoResultFound(DataTable source, GridView gv)
    {
        // create a new blank row to the DataTable
        source.Rows.Add(source.NewRow());

        // Bind the DataTable which contain a blank row to the GridView
        gv.DataSource = source;
        gv.DataBind();

        // Get the total number of columns in the GridView to know what the Column Span should be
        int columnsCount = gv.Columns.Count;
    }

    public string ChangeDateMMddyyyy(string strdate)
    {

        if (strdate != "")
        {
            string[] str;

            str = strdate.Split('/');
            strdate = str[1] + '/' + str[0] + '/' + str[2];


        }
        return strdate;
    }

    //Code Added By Priya For No image
    public string GetNoImagePath()
    {
        string ImagePath = string.Empty;

        ImagePath = "noimage.png";
        return ImagePath;

    }
}