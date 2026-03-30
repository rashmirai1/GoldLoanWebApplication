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
using System.Text.RegularExpressions;
using System.Text;
using System.Web.Routing;

public partial class ReportExport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            gvReport.DataSource = (DataTable)Session["REPORTEXPORT"];
            gvReport.DataBind();
        }
    }

    public void ExportExcel()
    {

        string filename = "GoldLoanRegisterExcel.xls";
        System.IO.StringWriter tw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
        //DataGrid dgGrid = new DataGrid();
        GridView dgGrid = new GridView();
        dgGrid.DataSource = (DataTable)Session["REPORTEXPORT"];
        dgGrid.DataBind();



        //Get the HTML for the control.



        //Applying stlye to gridview header cells

        for (int i = 0; i < dgGrid.HeaderRow.Cells.Count; i++)
        {
            dgGrid.HeaderRow.Cells[i].Style.Add("background-color", "#070C80");
            dgGrid.HeaderRow.Cells[i].Style.Add("color", "#ffffff");

            //#df5015

        }
        dgGrid.RenderControl(hw);
        //Write the HTML back to the browser.
        //Response.ContentType = application/vnd.ms-excel;
        Response.ContentType = "application/vnd.ms-excel";
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
        this.EnableViewState = false;
        Response.Write(tw.ToString());
        Response.End();
    }
    protected void btnExport_Click(object sender, EventArgs e)
    {
        ExportExcel();
    }
}