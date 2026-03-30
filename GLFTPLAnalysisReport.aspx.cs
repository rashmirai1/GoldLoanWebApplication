using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;

public partial class GLFTPLAnalysisReport : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    string strConnStringFTPL = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringFTPL"].ConnectionString;

    GlobalSettings gbl = new GlobalSettings();

    //Declaring Variables.   
    string m_strQuery = string.Empty;

    //Declaring Objects.     
    SqlConnection conn, connAIM, connFTPL;
    SqlDataAdapter da, daDataFTPL;
    DataSet ds, dsData;
    SqlCommand cmd;
    DataTable dt;

    #endregion [Declarations]

    #region [Page_PreRender]
    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
        Master.PropertybtnCancel.Visible = false;
        Master.PropertybtnView.Visible = false;
    }
    #endregion [Page_PreRender]

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!IsPostBack)
            {

                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }
                else
                {
                    hdnUserID.Value = Session["userID"].ToString();
                    hdnFYear.Value = Session["FYear"].ToString();
                    hdnFYearID.Value = Session["FYearID"].ToString();
                }
                ShowReport();
                Master.PropertybtnDelete.Visible = false;
                Master.PropertybtnEdit.Visible = false;
                Master.PropertybtnSave.Visible = false;
                Master.PropertybtnCancel.Visible = false;
                Master.PropertybtnView.Visible = false;

            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLReport", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[Page_Load]

    #region [GetRecord]
    //Get Record to Show on Report
    public DataSet GetRecord(DataSet ds)
    {
        //FTPL conn.
        conn = new SqlConnection(strConnStringFTPL);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select convert(varchar(20),getdate(),103)Loandate";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            //dt.Rows[0]["Loandate"].ToString();
        }

        connFTPL = new SqlConnection(strConnStringFTPL);
        cmd = new SqlCommand();
        cmd.Connection = connFTPL;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_FTPLAnalysisReportFTPL";
        cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["Loandate"].ToString()));
        cmd.Parameters.AddWithValue("@FYear", hdnFYear.Value);
        cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
        dsData = new DataSet();
        daDataFTPL = new SqlDataAdapter(cmd);
        daDataFTPL.Fill(dsData);
        return dsData;
    }
    #endregion [GetRecord]

    #region [ShowReport]
    // to Display Report 
    public void ShowReport()
    {
        DataSet ds = null;
        ds = new DataSet("~/CryGLFTPLAnalysisReport.rpt");
        ReportDocument rpt = new ReportDocument();

        rpt.Load(Server.MapPath(ds.DataSetName));
        ds = GetRecord(ds);

        if (ds.Tables.Count > 0)
        {
            rpt.SetDataSource(ds.Tables[0]);
        }
        Session["REPORT"] = rpt;
        ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('CryFTPLAnalysisReport.aspx');", true);

    }
    #endregion [ShowReport]
}