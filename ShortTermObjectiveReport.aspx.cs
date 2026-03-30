using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class ShortTermObjectiveReport : System.Web.UI.Page
{

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {

            ShowReport();

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [ShowReport]
    public void ShowReport()
    {
        if (Session["REPORT"] != null)
        {
            //string date = Request.QueryString["date"];
            //Session["Date"] = date;
            //string seldate = Convert.ToString(Session["Date"]);

            DataTable dtReport = (DataTable)Session["REPORT"];
            string SelDate = "", total = "";
            if (Session["Header"] != null)
            {
                DataTable dtHeader = (DataTable)Session["Header"];
                SelDate = dtHeader.Rows[0]["selDate"].ToString();
                //dtReport.Columns.Remove("selDate");
                total = dtHeader.Rows[0]["AllTotal"].ToString();
                // dtReport.Columns.Remove("GrandTotal");               
            }
            gvData.DataSource = dtReport;
            gvData.DataBind();

            GridViewRow row = new GridViewRow(0, 0, DataControlRowType.Header, DataControlRowState.Normal);
            TableHeaderCell cell = new TableHeaderCell();

            cell.Text = "Short Term Objectives Upto " + SelDate + "</br> </br> ";
            cell.ColumnSpan = gvData.Rows[0].Cells.Count;
            row.Controls.Add(cell);

            gvData.HeaderRow.Parent.Controls.AddAt(0, row);

            gvData.HeaderRow.Cells[0].Width = 130;
            gvData.FooterRow.Cells[gvData.Rows[0].Cells.Count - 1].Width = 100;
            gvData.FooterRow.Cells[gvData.Rows[0].Cells.Count - 1].Text = total;

            btnPrint.Visible = true;
            btnPrint.OnClientClick = "return PrintGridData();";
            //  gvData.FooterRow.Cells[gvData.Rows[0].Cells.Count - 1].ApplyStyle(TextAlign.Right);

        }
    }
    #endregion[ShowReport]

}