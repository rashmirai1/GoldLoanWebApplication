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

public partial class GLGoldManagementReport : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;


    GlobalSettings gbl = new GlobalSettings();

    bool datasaved = false;
    //Declaring Variables.
    int result = 0;


    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    System.Data.DataTable dt = new System.Data.DataTable();
    //DataTable dt1;
    SqlCommand cmd;


    #endregion [Declarations]
    protected void Page_Init(object sender, EventArgs e)
    {

        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);

        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);

    }

    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnSearch.Visible = false;

    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnSearch.Visible = false;
            Master.PropertybtnView.OnClientClick = "return CheckValidDate();";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GOLDManagement", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            //GetGoldManagementDetails();
            ExportToexcelDS();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GOLDManagement", "alert('" + ex.Message + "');", true);
        }
    }



    public void ExportToexcelDS()
    {


        string filename = "GoldVaultRegisterExcel.xls";
        System.IO.StringWriter tw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
        //DataGrid dgGrid = new DataGrid();

        DateTime fromdate = Convert.ToDateTime(txtPeriodDateFrom.Text);
        string fromdate1 = fromdate.ToString("dd/MM/yyyy");
        DateTime todate = Convert.ToDateTime(txtperiodtodate.Text);
        string todate1 = todate.ToString("dd/MM/yyyy");
        string date = "";
        int pageno = 0;

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_GetDate";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@fromdate", gbl.ChangeDateMMddyyyy(txtPeriodDateFrom.Text));
        cmd.Parameters.AddWithValue("@enddate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
        ds = new DataSet();
        da.Fill(ds);
        //if (ds.Tables[0].Rows.Count == 0)
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "", "alert('No Record Found For Selected Date Range');", true);
        //}

        if (ds.Tables[0].Rows.Count > 0)
        {
            tw = new System.IO.StringWriter();
            hw = new System.Web.UI.HtmlTextWriter(tw);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                date = ds.Tables[0].Rows[i]["Date_time"].ToString();
                pageno = i + 1;
                string GoldLoanNo = "";
                int SDID = 0;
                int InwardBy = 0;
                string Time = "";
                string RacKNo = "";
                string ProposalNumber = "";
                int NoofPouches = 0;
                string Out_Time = "";
                string Signature = "";
                int OutwardBy = 0;
                string Outwardname = "";
                string inwardname = "";


                conn = new SqlConnection(strConnString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GL_GoldM_Details";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@fromdate", gbl.ChangeDateMMddyyyy(txtPeriodDateFrom.Text));
                cmd.Parameters.AddWithValue("@enddate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                cmd.Parameters.AddWithValue("@datestock", gbl.ChangeDateMMddyyyy(date));
                DataSet ds1 = new DataSet();
                da1.Fill(ds1);

                if (ds1.Tables[0].Rows.Count > 0)
                {

                    hw.Write("<table border ='1' CellPadding ='0' CellSpacing='0' width='100%' ><tr ><td colspan='10' style = 'text-align:center; font-size :31 ; font-family :Times New Roman; font-weight:bold;'>GOLD VAULT REGISTER</td>");
                    hw.Write("<td colspan ='3' style = 'font-size :18;'>Page No." + pageno + "</td></tr>");
                    hw.Write("<tr> <td colspan ='13' style = 'text-align:Right; font-size :18 ;'>Date :- " + date + "</td></tr>");
                    hw.Write("<tr><td style='font-size :18 ;'>Opening Balance</td>");
                    hw.Write("<td style ='font-weight:bold; text-align:center; font-family :Times New Roman; font-size :18 ;'>" + ds1.Tables[0].Rows[0]["OpeningBalance"].ToString() + "</td>");
                    hw.Write("<td colspan='3' style ='text-align:center; font-family :Times New Roman; font-size :18 ;'>Deposit</td>");
                    hw.Write("<td colspan='6' style ='text-align:center; font-family :Times New Roman; font-size :18 ;'>Release</td>");
                    hw.Write("<td style ='text-align:left; font-family :Times New Roman;font-size :18 ;'>Net Balance</td></tr>");
                    hw.Write("<tr><td style ='font-family :Times New Roman;font-size :18 ; text-align:center; '>In-Time</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Rack No</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Proposal Number</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Number of Pouches</td>");
                    //     hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Out-Time</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Signature</td>");
                    //   hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>In-Time</td>");
                    // hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Rack No</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Proposal Number</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Number of Pouches</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Out-Time</td>");
                    hw.Write("<td style ='font-family :Times New Roman; font-size :18 ;'>Signature</td>");
                    hw.Write("<td style = 'text-align:center; font-family :Times New Roman; font-size :18 ;'>(+)(-)</td></tr>");

                    for (int j = 0; j < ds1.Tables[0].Rows.Count; j++)
                    {

                        GoldLoanNo = ds1.Tables[0].Rows[j]["GoldLoanNo"].ToString();
                        SDID = Convert.ToInt32(ds1.Tables[0].Rows[j]["SDID"]);
                        InwardBy = Convert.ToInt32(ds1.Tables[0].Rows[j]["InwardBy"]);
                        OutwardBy = Convert.ToInt32(ds1.Tables[0].Rows[j]["OutwardBy"]);
                        inwardname = bindname(InwardBy);
                        ds1.Tables[0].Rows[j]["InwardByName"] = inwardname;
                        Outwardname = bindname(OutwardBy);
                        ds1.Tables[0].Rows[j]["OutwardByName"] = Outwardname;
                        hw.Write("<tr>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["InTime"] + "</td>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["Rack No"] + "</td>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["GoldLoanNo"] + "</td>");

                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["No of Pouches"] + "</td>");

                        // hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["Out-Time"] + "</td>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["Signature"] + "</td>");
                        // hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_In_Time"] + "</td>");
                        //hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_RacKNo"] + "</td>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_ProposalNumber"] + "</td>");

                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_NoofPouches"] + "</td>");

                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_OutTime"] + "</td>");
                        hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["R_Signature"] + "</td>");
                        if (ds1.Tables[0].Rows[j]["R_RacKNo"] != "")
                        {
                            hw.Write("<td style='text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[j]["Net Balance"] + "</td>");
                        }
                        hw.Write("</tr>");
                    }
                    hw.Write("<tr>");
                    hw.Write("<td colspan='12' style = 'text-align:Right; font-family :Times New Roman; font-size :18;'>Total Closing Balance </td>");
                    hw.Write("<td style ='font-weight:bold; text-align:center; font-family :Times New Roman; font-size :18;'>" + ds1.Tables[0].Rows[0]["ClosingBalance"].ToString() + "</td>");
                    hw.Write("</tr>");
                    hw.Write("<tr>");
                    hw.Write("<td colspan='6' style ='font-family :Times New Roman; font-size :18;'>Employee Name :- " + inwardname + " & Employee No :- " + InwardBy + "</td>");
                    //hw.Write("<td>"+ds1.Tables[0].Rows[0]["InwardByName"].ToString()+"</td> ");
                    hw.Write("<td colspan='7' style ='font-family :Times New Roman; font-size :18;'>Employee Name :- " + Outwardname + " & Employee No :- " + OutwardBy + "</td>");
                    // hw.Write("<td>"+ds1.Tables[0].Rows[0]["OutwardByName"].ToString()+"</td> ");
                    hw.Write("</tr>");
                    hw.Write("</table>");
                    hw.Write("<br/>");
                    hw.Write("<br/>");
                    hw.Write("<br/>");
                }
            }

            DataGrid dgGrid = new DataGrid();
            DataTable dt = new DataTable();
            dgGrid.AutoGenerateColumns = false;
            dgGrid.DataSource = dt;
            dgGrid.DataBind();

            dgGrid.RenderControl(hw);
            // hw.Write("<table><tr><td colspan='" + ds.Tables[0].Columns.Count + "' </td></tr></table>");
            //Write the HTML back to the browser.
            //Response.ContentType = application/vnd.ms-excel;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Response.Write(tw.ToString());
            Response.End();

        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "alert('No Record Found For Selected Date Range');", true);
        }

    }

    public string bindname(int OutwardBy)
    {
        string name = "";
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster where status='Active' and EmployeeID=" + OutwardBy;
        SqlDataAdapter daaim = new SqlDataAdapter(cmd);
        DataTable dtaim = new DataTable();
        daaim.Fill(dtaim);

        if (dtaim.Rows.Count > 0)
        {
            name = dtaim.Rows[0]["EmpName"].ToString();
        }
        dtaim.Rows.Clear();
        return name;
    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        Clear();

    }

    public void Clear()
    {
        txtPeriodDateFrom.Text = "";
        txtperiodtodate.Text = "";
    }
}