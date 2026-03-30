using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;

public partial class GLShortTermObjectiveReport : System.Web.UI.Page
{

    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    GlobalSettings gbl = new GlobalSettings();

    //Declaring Variables.   
    string m_strQuery = string.Empty;

    public string loginDate;
    public string expressDate;
    int result = 0;
    int excount = 0;

    //Declaring Objects.     
    SqlConnection conn;
    SqlDataAdapter da, da1, da2;
    SqlCommand cmd, cmd1, cmd2;
    DataTable dt;


    #endregion [Declarations]

    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);


    }
    #endregion [Page_Init]

    #region [Page_PreRender]
    protected void Page_PreRender(Object sender, EventArgs e)
    {

        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
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

                Master.PropertybtnView.OnClientClick = "return validdate();";
                //btnPrint.OnClientClick = "return PrintGridData();";

                Master.PropertybtnDelete.Visible = false;
                Master.PropertybtnEdit.Visible = false;
                Master.PropertybtnSave.Visible = false;

                txtIntDate.Text = DateTime.Now.ToShortDateString();
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
    public DataSet GetRecord()
    {

        conn = new SqlConnection(strConnString);
        DataSet dsData = new DataSet();

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_ShortTermReportAnalysis";
        cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(txtIntDate.Text));
        //  cmd.Parameters.AddWithValue("@FYear", hdnFYear.Value);
        // cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
        SqlDataAdapter daData = new SqlDataAdapter(cmd);
        daData.Fill(dsData);
        return dsData;

    }
    #endregion [GetRecord]

    #region[GetSourceApplication]
    //to get all data from Different Source
    public void GetSourceApplication()
    {

        DataTable dtMain = new DataTable();
        DataTable dtSourc = GetRecord().Tables[1];
        DataTable dtPeriod = GetRecord().Tables[2];
        DataTable dtLoan = GetRecord().Tables[4];
        DataTable dtHeader = GetRecord().Tables[5];

        if (dtLoan.Rows.Count > 0)
        {
            dtMain.Columns.Add("Period");
            for (int index = 0; index < dtSourc.Rows.Count; index++)
            {
                DataRow drSource = dtSourc.Rows[index];
                dtMain.Columns.Add(drSource["SourceofApplication"].ToString());
            }
            dtMain.Columns.Add("Total");
            for (int countperiod = 0; countperiod < dtPeriod.Rows.Count; countperiod++)
            {
                DataRow drPeriod = dtMain.NewRow();
                drPeriod["Period"] = dtPeriod.Rows[countperiod]["Period"].ToString();
                dtMain.Rows.Add(drPeriod);
                DataRow drTotal = dtMain.NewRow();
            }

            for (int rowscount = 0; rowscount < dtMain.Rows.Count; rowscount++)
            {

                for (int index = 0; index < dtLoan.Rows.Count; index++)
                {
                    if (dtMain.Columns[dtLoan.Rows[index]["SrcOfApp"].ToString()].ToString() == dtLoan.Rows[index]["SrcOfApp"].ToString() && dtMain.Rows[rowscount]["Period"].ToString() == dtLoan.Rows[index]["Period"].ToString())
                    {
                        dtMain.Rows[rowscount][dtLoan.Rows[index]["SrcOfApp"].ToString()] = dtLoan.Rows[index]["TotalLoanLive"].ToString();
                        dtMain.Rows[rowscount]["Total"] = dtLoan.Rows[index]["Total"].ToString();
                    }
                    else
                    {
                        //dtMain.Rows[rowscount][dtLoan.Rows[index]["SrcOfApp"].ToString()] = "";
                    }
                }
                //dtMain.Rows[rowscount][dtLoan.Rows[rowscount]["SrcOfApp"].ToString()] = "N/A";
            }

            //dtMain.Columns.Add("GrandTotal");
            //dtMain.Columns.Add("SelDate");
            //if (dtHeader.Rows.Count > 0)
            //{
            //    dtMain.Rows[0]["GrandTotal"] = dtHeader.Rows[0]["AllTotal"].ToString();
            //    dtMain.Rows[0]["SelDate"] = dtHeader.Rows[0]["SelDate"].ToString();
            //}

            Session["REPORT"] = dtMain;
            Session["Header"] = dtHeader;
            ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('ShortTermObjectiveReport.aspx');", true);//?Date=" + txtIntDate.Text + "
            //   return dtMain;
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLReport", "alert('No Records Found');", true);
        }
    }


    #endregion[GetSourceApplication]

    #region [dataPrintReport]
    //Added for datatable genaration at the time of report generation
    public DataTable dataPrintReport()
    {

        DataTable dt = new DataTable("[GL_ShortTermReportAnalysis]");
        dt.Columns.Add("ItemName");
        dt.Columns.Add("ItemId");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Grossweight");
        dt.Columns.Add("NetWeight");
        dt.Columns.Add("Rate");
        DataRow dr = dt.NewRow();
        dr["ItemName"] = "";
        dr["ItemId"] = "";
        dr["Quantity"] = "";
        dr["Grossweight"] = "";
        dr["NetWeight"] = "";
        dr["Rate"] = "";
        dt.Rows.Add(dr);
        return dt;
    }
    #endregion [dataPrintReport]

    #region [ShowReport]
    // to Display Report 
    public void ShowReport()
    {
        DataSet ds = null;
        ds = new DataSet("~/CryGLShorttermObjectivesReport.rpt");

        ds.Tables.Add(dataPrintReport());
        ReportDocument rpt = new ReportDocument();
        rpt.Load(Server.MapPath(ds.DataSetName));


        // GetSourceApplication();
        //  dt = GetSourceApplication();


        //  DataSet ds1 = new DataSet();

        //   ds1.Tables.Add("GL_GetSourceThrogh");
        //   ds1.Tables["GL_GetSourceThrogh"].Merge(dt);

        // rpt.SetDataSource(ds1.Tables["GL_GetSourceThrogh"]);

        //  Session["REPORT"] = rpt;
        // ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('CryGLDisbursalAnalysisReport.aspx');", true);

    }
    #endregion [ShowReport]

    #region [View]
    // view btn click event
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            // ShowReport();
            GetSourceApplication();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [View]

    #region [PropertybtnCancel_Click]
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        // txtIntDate.Text = "";
        try
        {
            Response.Redirect("GLShortTermObjectiveReport.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnCancel_Click]
}