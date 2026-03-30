using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.Reporting;
using CrystalDecisions.ReportSource;

public partial class GLDisbursalAnalysisReport : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    string strConnStringFTPL = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringFTPL"].ConnectionString;

    GlobalSettings gbl = new GlobalSettings();

    //Declaring Variables.   
    string m_strQuery = string.Empty;

    public string loginDate;
    public string expressDate;
    int result = 0;
    int excount = 0;

    //Declaring Objects.     
    SqlConnection conn, connAIM, connFTPL;
    SqlDataAdapter da, da1, da2;
    DataSet ds, ds1, ds2;
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
    #endregion [Page_Load]




    public DataTable GetMonRepayment()
    {

        DataTable dtMon = new DataTable();
        DataTable dtKyc = new DataTable();
        DataRow drKyc = null;

        dtKyc.Columns.Add("Kyc");

        dtMon.Columns.Add("MonAcNo");
        dtMon.Columns.Add("MonCus");
        dtMon.Columns.Add("MonLoan");
        dtMon.Columns.Add("MonGlValue");

        dtMon.Rows.Add("0000000000", "0000000000", "0000000000", "0000000000");


        int AcCount = 0;
        decimal LoanAmount = 0;
        decimal TotalGlValue = 0;

        conn = new SqlConnection(strConnString);
        conn.Open();
        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();

        string query = "select  GoldLoanNo,K.CustomerID,K.KYCID,NetLoanAmtSanctioned,Totalvalue From TGLSanctionDisburse_BasicDetails D with(nolock) inner join TGLKYC_BasicDetails K with(nolock) on D.KYCID=K.KYCID where LoanDate<='" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "'";

        cmd = new SqlCommand(query, conn);
        da = new SqlDataAdapter(cmd);

        DataTable dtGl = new DataTable();
        da.Fill(dtGl);

        for (int index = 0; index < dtGl.Rows.Count; index++)
        {
            string GoldLoanNo = dtGl.Rows[index]["GoldLoanNo"].ToString();
            string KycId = dtGl.Rows[index]["CustomerID"].ToString();
            decimal Loan = Convert.ToDecimal(dtGl.Rows[index]["NetLoanAmtSanctioned"].ToString());
            decimal GlValue = Convert.ToDecimal(dtGl.Rows[index]["Totalvalue"].ToString());

            query = "select AccountID from tblaccountmaster with(nolock) where alies='" + GoldLoanNo + "'";
            cmd = new SqlCommand(query, connAIM);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                string AccountID = Convert.ToString(cmd.ExecuteScalar());
                query = "select isnull(AccountID,0)AccountID from FLedgerMaster with(nolock) where LedgerID=(select max(LedgerID) from FLedgerMaster with(nolock) where AccountID='" + AccountID + "')and month(RefDate)=month('" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "') and FinanceYear='" + hdnFYearID.Value + "'";
                cmd = new SqlCommand(query, connAIM);

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    string newAccountID = Convert.ToString(cmd.ExecuteScalar());
                    query = "select sum(Debit)-sum(Credit) 'PaidAmount' From FLedgerMaster with(nolock) where AccountID='" + newAccountID + "'";
                    cmd = new SqlCommand(query, connAIM);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        decimal PaidAmount = Convert.ToDecimal(cmd.ExecuteScalar());

                        query = "select GoldLoanNo From TGlReceipt_BasicDetails with(nolock) where GoldLoanNo='" + GoldLoanNo + "' and isactive='Y'";
                        cmd = new SqlCommand(query, conn);

                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            string checkgold = Convert.ToString(cmd.ExecuteScalar());
                            if (checkgold != string.Empty)
                            {
                                query = "select sum(RcvCLP) From TGlReceipt_BasicDetails with(nolock) where GoldLoanNo='" + GoldLoanNo + "' and isactive='Y' ";
                                cmd = new SqlCommand(query, conn);

                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    decimal RcvCLP = Convert.ToDecimal(cmd.ExecuteScalar());

                                    if ((PaidAmount <= 0) && RcvCLP >= Loan)
                                    {
                                        drKyc = dtKyc.NewRow();
                                        drKyc["Kyc"] = KycId;
                                        dtKyc.Rows.Add(drKyc);
                                        AcCount = AcCount + 1;
                                        LoanAmount = LoanAmount + Loan;
                                        TotalGlValue = TotalGlValue + GlValue;
                                    }
                                }
                            }
                            else if (PaidAmount <= 0)
                            {
                                drKyc = dtKyc.NewRow();
                                drKyc["Kyc"] = KycId;
                                dtKyc.Rows.Add(drKyc);
                                AcCount = AcCount + 1;
                                LoanAmount = LoanAmount + Loan;
                                TotalGlValue = TotalGlValue + GlValue;
                            }
                        }

                    }
                }
            }
        }

        DataTable dtk = dtKyc.DefaultView.ToTable(true);
        dtMon.Rows[0]["MonAcNo"] = AcCount;
        dtMon.Rows[0]["MonCus"] = dtk.Rows.Count;
        dtMon.Rows[0]["MonLoan"] = LoanAmount;
        dtMon.Rows[0]["MonGlValue"] = TotalGlValue;


        return dtMon;
    }

    public DataTable GetFYRepayment()
    {
        DataTable dtMon = new DataTable();
        DataTable dtKyc = new DataTable();
        DataRow drKyc = null;

        dtKyc.Columns.Add("Kyc");

        dtMon.Columns.Add("FYAcNo");
        dtMon.Columns.Add("FYCus");
        dtMon.Columns.Add("FYLoan");
        dtMon.Columns.Add("FYGlValue");

        dtMon.Rows.Add("0000000000", "0000000000", "0000000000", "0000000000");

        int AcCount = 0;
        decimal LoanAmount = 0;
        decimal TotalGlValue = 0;

        conn = new SqlConnection(strConnString);
        conn.Open();
        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();

        string query = "select  GoldLoanNo,K.CustomerID,K.KYCID,NetLoanAmtSanctioned,Totalvalue From TGLSanctionDisburse_BasicDetails D with(nolock) inner join TGLKYC_BasicDetails K with(nolock) on D.KYCID=K.KYCID where LoanDate<='" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "'";

        cmd = new SqlCommand(query, conn);
        da = new SqlDataAdapter(cmd);

        DataTable dtGl = new DataTable();
        da.Fill(dtGl);

        for (int index = 0; index < dtGl.Rows.Count; index++)
        {
            string GoldLoanNo = dtGl.Rows[index]["GoldLoanNo"].ToString();
            string KycId = dtGl.Rows[index]["CustomerID"].ToString();
            decimal Loan = Convert.ToDecimal(dtGl.Rows[index]["NetLoanAmtSanctioned"].ToString());
            decimal GlValue = Convert.ToDecimal(dtGl.Rows[index]["Totalvalue"].ToString());

            query = "select AccountID from tblaccountmaster with(nolock) where alies='" + GoldLoanNo + "'";
            cmd = new SqlCommand(query, connAIM);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                string AccountID = Convert.ToString(cmd.ExecuteScalar());
                query = "select isnull(AccountID,0)AccountID from FLedgerMaster with(nolock) where LedgerID=(select max(LedgerID) from FLedgerMaster with(nolock) where AccountID='" + AccountID + "')and RefDate<='" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "' and FinanceYear='" + hdnFYearID.Value + "'";
                cmd = new SqlCommand(query, connAIM);

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    string newAccountID = Convert.ToString(cmd.ExecuteScalar());
                    query = "select sum(Debit)-sum(Credit) 'PaidAmount' From FLedgerMaster with(nolock) where AccountID='" + newAccountID + "'";
                    cmd = new SqlCommand(query, connAIM);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        decimal PaidAmount = Convert.ToDecimal(cmd.ExecuteScalar());

                        query = "select GoldLoanNo From TGlReceipt_BasicDetails with(nolock) where GoldLoanNo='" + GoldLoanNo + "' and isactive='Y'";
                        cmd = new SqlCommand(query, conn);

                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            string checkgold = Convert.ToString(cmd.ExecuteScalar());
                            if (checkgold != string.Empty)
                            {
                                query = "select sum(RcvCLP) From TGlReceipt_BasicDetails with(nolock) where GoldLoanNo='" + GoldLoanNo + "' and isactive='Y' ";
                                cmd = new SqlCommand(query, conn);

                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    decimal RcvCLP = Convert.ToDecimal(cmd.ExecuteScalar());

                                    if ((PaidAmount <= 0) && RcvCLP >= Loan)
                                    {

                                        drKyc = dtKyc.NewRow();
                                        drKyc["Kyc"] = KycId;
                                        dtKyc.Rows.Add(drKyc);
                                        AcCount = AcCount + 1;
                                        LoanAmount = LoanAmount + Loan;
                                        TotalGlValue = TotalGlValue + GlValue;
                                    }
                                }
                            }
                            else if (PaidAmount <= 0)
                            {
                                drKyc = dtKyc.NewRow();
                                drKyc["Kyc"] = KycId;
                                dtKyc.Rows.Add(drKyc);
                                AcCount = AcCount + 1;
                                LoanAmount = LoanAmount + Loan;
                                TotalGlValue = TotalGlValue + GlValue;
                            }
                        }
                    }
                }
            }
        }

        DataTable dtk = dtKyc.DefaultView.ToTable(true);


        dtMon.Rows[0]["FYAcNo"] = AcCount;
        dtMon.Rows[0]["FYCus"] = dtk.Rows.Count;
        dtMon.Rows[0]["FYLoan"] = LoanAmount;
        dtMon.Rows[0]["FYGlValue"] = TotalGlValue;

        return dtMon;
    }

    #region [dataPrintReport]
    //Added for datatable genaration at the time of report generation
    public DataTable dataPrintReport()
    {
        DataTable dt = new DataTable("GL_DisbursalAnalysisReport");
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

    #region [GetRecord]
    //Get Record to Show on Report
    public DataSet GetRecord(DataSet ds)
    {
        //*********FTPL
        connFTPL = new SqlConnection(strConnString);
        //connFTPL = new SqlConnection(strConnStringFTPL);
        cmd = new SqlCommand();
        cmd.Connection = connFTPL;
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_DisbursalAnalysisReport";
        //cmd.CommandText = "GL_DisbursalAnalysisReportFTPL";
        cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(txtIntDate.Text));
        cmd.Parameters.AddWithValue("@FYear", hdnFYear.Value);
        cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
        //DataSet dsDataFTPL = new DataSet();
        SqlDataAdapter daDataFTPL = new SqlDataAdapter(cmd);
        //daDataFTPL.Fill(dsDataFTPL);

        dt = new DataTable();
        daDataFTPL.Fill(dt);

        //-----------------------------------------   

        conn = new SqlConnection(strConnString);
        DataSet dsData = new DataSet();
        foreach (DataTable dt2 in ds.Tables)
        {
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = dt2.TableName;
            cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(txtIntDate.Text));
            cmd.Parameters.AddWithValue("@FYear", hdnFYear.Value);
            cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
            SqlDataAdapter daData = new SqlDataAdapter(cmd);
            daData.Fill(dsData);

        }
        dsData.Tables.Add("FTPLDetail");
        dsData.Tables["FTPLDetail"].Merge(dt);

        //  dsData.Merge(dsDataFTPL);
        return dsData;

    }
    #endregion [GetRecord]

    #region [ShowReport]
    // to Display Report 
    public void ShowReport()
    {
        DataSet ds = null;
        ds = new DataSet("~/CryGLDisbursalAnalysisReport.rpt");

        ds.Tables.Add(dataPrintReport());
        ReportDocument rpt = new ReportDocument();
        rpt.Load(Server.MapPath(ds.DataSetName));

        ds = GetRecord(ds);
        //ds.Tables[0].Columns.Add("FinalAC");
        //ds.Tables[0].Columns.Add("FinalACLive");
        //ds.Tables[0].Columns.Add("FinalACLoan");
        //ds.Tables[0].Columns.Add("FinalACGold");

        int totalCountACGL = 0, totalCountCustGL = 0; decimal totalCountLoanGL = 0, totalCountGoldGL = 0;
        int totalCountACFTPL = 0, totalCountCustFTPL = 0; decimal totalCountLoanFTPL = 0;
        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
        {
            if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[i]["Total"])))
            {
                totalCountACGL = Convert.ToInt32(ds.Tables[0].Rows[i]["Total"].ToString());
            }
            if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[i]["TotalLiveCust"])))
            {
                totalCountCustGL = Convert.ToInt32(ds.Tables[0].Rows[i]["TotalLiveCust"].ToString());
            }
            if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[i]["TotalLiveLoan"])))
            {
                totalCountLoanGL = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalLiveLoan"].ToString());
            }
            if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[i]["TotalLiveValue"])))
            {
                totalCountGoldGL = Convert.ToDecimal(ds.Tables[0].Rows[i]["TotalLiveValue"].ToString());
            }
        }

        for (int i = 0; i < ds.Tables[6].Rows.Count; i++)
        {
            if (!(System.Convert.IsDBNull(ds.Tables[4].Rows[i]["TotalFTPL"])))
            {
                totalCountACFTPL = Convert.ToInt32(ds.Tables[4].Rows[i]["TotalFTPL"].ToString());
            }
            if (!(System.Convert.IsDBNull(ds.Tables[4].Rows[i]["TotalAC"])))
            {
                totalCountCustFTPL = Convert.ToInt32(ds.Tables[4].Rows[i]["TotalAC"].ToString());
            }
            if (!(System.Convert.IsDBNull(ds.Tables[4].Rows[i]["TotalLoan"])))
            {
                totalCountLoanFTPL = Convert.ToDecimal(ds.Tables[4].Rows[i]["TotalLoan"].ToString());
            }
            //if (!(System.Convert.IsDBNull(ds.Tables[6].Rows[i]["TotalFTPL"])))
            //{
            //    totalCountACFTPL = Convert.ToInt32(ds.Tables[6].Rows[i]["TotalFTPL"].ToString());
            //}
            //if (!(System.Convert.IsDBNull(ds.Tables[6].Rows[i]["TotalAC"])))
            //{
            //    totalCountCustFTPL = Convert.ToInt32(ds.Tables[6].Rows[i]["TotalAC"].ToString());
            //}
            //if (!(System.Convert.IsDBNull(ds.Tables[6].Rows[i]["TotalLoan"])))
            //{
            //    totalCountLoanFTPL = Convert.ToDecimal(ds.Tables[6].Rows[i]["TotalLoan"].ToString());
            //}
        }

        //Start total repayments add by bharat on 21/12/2015
        DataTable dtMon = GetMonRepayment();
        DataTable dtFY = GetFYRepayment();

        if (dtMon.Rows.Count > 0)
        {
            ds.Tables[5].Rows[0]["MonNoAcRepayment"] = dtMon.Rows[0]["MonAcNo"];
            ds.Tables[5].Rows[0]["MonCusRepayment"] = dtMon.Rows[0]["MonCus"];
            ds.Tables[5].Rows[0]["MonLoanAmtRepayment"] = dtMon.Rows[0]["MonLoan"];
            ds.Tables[5].Rows[0]["MonValuetRepayment"] = dtMon.Rows[0]["MonGlValue"];

        }

        int fyacno = 0;
        if (dtFY.Rows.Count > 0)
        {
            fyacno = Convert.ToInt32(dtFY.Rows[0]["FYAcNo"]);
            ds.Tables[5].Rows[0]["FYNoAcRepayment"] = dtFY.Rows[0]["FYAcNo"];
            ds.Tables[5].Rows[0]["FYNoCusRepayment"] = dtFY.Rows[0]["FyCus"];
            ds.Tables[5].Rows[0]["FYNoLoanAmtRepayment"] = dtFY.Rows[0]["FyLoan"];
            ds.Tables[5].Rows[0]["FYNoValueRepayment"] = dtFY.Rows[0]["FyGlValue"];
        }


        //if (!(System.Convert.IsDBNull(ds.Tables[5].Rows[0]["FYNoAcRepayment"])))
        //{
        //    totalCountACGL = totalCountACGL + Convert.ToInt32(ds.Tables[5].Rows[0]["FYNoAcRepayment"]);
        //}

        //if (!(System.Convert.IsDBNull(ds.Tables[5].Rows[0]["FYNoCusRepayment"])))
        //{
        //    totalCountCustGL = totalCountCustGL + Convert.ToInt32(ds.Tables[5].Rows[0]["FYNoCusRepayment"]);
        //}

        //if (!(System.Convert.IsDBNull(ds.Tables[5].Rows[0]["FYNoLoanAmtRepayment"])))
        //{
        //    totalCountLoanGL = totalCountLoanGL + Convert.ToDecimal(ds.Tables[5].Rows[0]["FYNoLoanAmtRepayment"]);
        //}

        //if (!(System.Convert.IsDBNull(ds.Tables[5].Rows[0]["FYNoValueRepayment"])))
        //{
        //    totalCountGoldGL = totalCountGoldGL + Convert.ToDecimal(ds.Tables[5].Rows[0]["FYNoValueRepayment"]);
        //}
        //End total repayments add by bharat on 21/12/2015

        ds.Tables[0].Rows[0]["FinalAC"] = (totalCountACGL + totalCountACFTPL);
        ds.Tables[0].Rows[0]["FinalACLive"] = (totalCountCustGL + totalCountCustFTPL);
        ds.Tables[0].Rows[0]["FinalACLoan"] = (totalCountLoanGL + totalCountLoanFTPL);
        ds.Tables[0].Rows[0]["FinalACGold"] = (totalCountGoldGL);


        if ((Convert.ToInt32(ds.Tables[0].Rows[0]["FinalAC"].ToString()) <= 0 && Convert.ToInt32(ds.Tables[0].Rows[0]["FinalACLive"].ToString()) <= 0 && Convert.ToDecimal(ds.Tables[0].Rows[0]["FinalACLoan"].ToString()) <= 0 && Convert.ToDecimal(ds.Tables[0].Rows[0]["FinalACLoan"].ToString()) <= 0) && (fyacno <= 0))
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLReport", "alert('No Records Found');", true);
        }

        else
        {
            rpt.SetDataSource(ds.Tables[0]);
            if (rpt.Subreports.Count > 0)
            {
                //rpt.Subreports[0].SetDataSource(ds.Tables[2]);
                //rpt.Subreports[1].SetDataSource(ds.Tables[1]);
                //rpt.Subreports[2].SetDataSource(ds.Tables[3]);
                //rpt.Subreports[3].SetDataSource(ds.Tables[4]);    
                rpt.Subreports[3].SetDataSource(ds.Tables[6]);
                rpt.Subreports[0].SetDataSource(ds.Tables[2]);
                rpt.Subreports[1].SetDataSource(ds.Tables[1]);
                rpt.Subreports[2].SetDataSource(ds.Tables[3]);
                rpt.Subreports[4].SetDataSource(ds.Tables[5]);
            }

            Session["REPORT"] = rpt;
            //ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('CryGLDisbursalAnalysisReport.aspx');", true);
            ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('CryFTPLAnalysisReport.aspx');", true);
        }
    }
    #endregion [ShowReport]

    #region [View]
    // view btn click event
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {

            ShowReport();

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
            //Response.Redirect("GLDisbursalAnalysisReport.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnCancel_Click]
}