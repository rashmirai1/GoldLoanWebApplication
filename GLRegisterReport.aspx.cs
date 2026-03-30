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
using System.Reflection;
using System.Security.AccessControl;
using System.Net;
using System.Drawing;

public partial class GLRegisterReport : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    //  string strConnStringFTPL = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringFTPL"].ConnectionString;

    GlobalSettings gbl = new GlobalSettings();

    bool datasaved = false;
    //Declaring Variables.
    int result = 0;
    int cnt = 0; // addeed priya
    int ExlColCount = 0;
    string temp;
    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    System.Data.DataTable dt = new System.Data.DataTable();
    //DataTable dt1;
    SqlCommand cmd;

    decimal TwentyTwokarat, TwentyOnekarat, Twentykarat, Eighteenkarat, TwentyFourkarat, TwentyThreeCarat;
    #endregion [Declarations]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);

        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);

        gvExcel.RowDataBound += new GridViewRowEventHandler(gvExcel_RowDataBound);

    }
    // by Priya 17/4/2015
    public GridView PropertygvExcel
    {
        get { return gvExcel; }
    }

    public string rates(string urlAddress)
    {

        try
        {
            urlAddress = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();

                string abb = data.Substring(data.LastIndexOf("pad-15"), 14);
                string rate = abb.Substring(8, 6);
                temp = rate.Replace(",", "");

                //temp = data.Substring(data.LastIndexOf("10g"), 30);
                //temp = temp.Replace("<TD>", "");
                //temp = temp.Replace("</TD>", "");
                //temp = temp.Replace("\t", " ");
                //temp = temp.Replace("\n", " ");
                //temp = temp.Replace("10g", "");
                //temp = temp.Replace(" ", "");
                //temp = temp.Replace("Rs.", "");
                //temp = temp.Replace(",", "");

                TwentyFourkarat = Convert.ToDecimal(temp);
                TwentyThreeCarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.958);
                TwentyTwokarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.916);
                TwentyOnekarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.875);
                Twentykarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.830);
                Eighteenkarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.75);

            }
        }
        catch (Exception)
        {
            //MessageBox.Show("Cannot connect to the server."); 
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cannot connect to the server.');", true);
        }
        return temp;
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
            if (!IsPostBack)
            {
                BindGoldLoan();
                rdbGLno.Checked = true;

            }
            Master.PropertybtnView.OnClientClick = "return CheckValidDate();";
            // Master.PropertybtnView.OnClientClick = "return validrecord();";
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnSearch.Visible = false;

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLRegisterReport", "alert('" + ex.Message + "');", true);
        }

    }

    public void BindGoldLoan()
    {

        dt.Columns.Add("FieldName");
        dt.Rows.Add("GL No");
        dt.Rows.Add("Name");
        dt.Rows.Add("Loan Amount");
        dt.Rows.Add("Loan Date");
        dt.Rows.Add("Scheme Name");
        dt.Rows.Add("ROI");
        dt.Rows.Add("Details Of Ornaments");
        dt.Rows.Add("Pouch No");
        dt.Rows.Add("Service Tax ON Service Charges(%)");
        dt.Rows.Add("Net wt of gold(g)");
        dt.Rows.Add("Value of gold");
        dt.Rows.Add("LTV");
        dt.Rows.Add("Interest overdue as on date");
        dt.Rows.Add("Outstanding Principal");
        dt.Rows.Add("Loan Repayment Date");
        dt.Rows.Add("Balance Eligibility");
        dt.Rows.Add("PL Case No");
        dt.Rows.Add("PL Overdue Interest");
        dt.Rows.Add("PL Outstanding Balance");
        dt.Rows.Add("Processing Charges");
        dt.Rows.Add("Sourced Through");
        dt.Rows.Add("DOB");
        dt.Rows.Add("Pan No");
        dt.Rows.Add("Tel No");
        dt.Rows.Add("Address");
        dt.Rows.Add("FTPL/PL/TL");

        grdgold.DataSource = dt;
        grdgold.DataBind();

    }



    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GetReportDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLRegisterReport", "alert('" + ex.Message + "');", true);
        }

    }

    public override void VerifyRenderingInServerForm(Control control)
    {
        //added by priya for excel shit rendering
        //don't throw any exception!

    }

    public void GetReportDetails()
    {
        string temp = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
        temp = temp.Substring(0, temp.IndexOf("@"));
        rates(temp);

        int KYCID = 0;
        string GoldLoanNo = "";
        string receiveddate = "";
        int BalanceLoanPayable = 0;
        int SDID = 0;
        string userentereddate = "";
        int totaldays = 0;
        decimal interestdue = 0;
        decimal interestamt = 0;
        int CaseNo = 0;
        string strQuery = "";
        string strQuery1 = "";
        string strOnlineRates = "";
        //int cnt = 0;
        int rcnt = 0;
        string sortby = "";
        decimal ploutstanding = 0;
        decimal ploutstamt = 0;
        decimal plinterestdue = 0;
        decimal plinterestamt = 0;
        int StateID = 0;
        int CityID = 0;
        int AreaID = 0;
        int ZoneID = 0;
        string address = "";
        string fulladdress = "";
        decimal balAmount = 0, FinalAmount = 0, LTVRates = 0, OutstndAmt = 0;

        string LastDate, LastCLI, LastLoanAmount = string.Empty;
        string InterestFromDate = string.Empty;
        string InterestToDate = string.Empty;
        string RvcCLI = string.Empty;
        string AdvInterestFromDate = string.Empty;
        string AdvInterestToDate = string.Empty;
        int neworold = 0;
        //TwentyFourkarat = Convert.ToDecimal(temp);
        //TwentyTwokarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.916);
        //TwentyOnekarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.875);
        //Twentykarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.830);
        //Eighteenkarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.75);

        if (rdbGLno.Checked == false)
        {
            if (rdbName.Checked == false)
            {
                if (rdbloandate.Checked == false)
                {
                    if (rdbplcaseno.Checked == false)
                    {
                        rcnt = 0;
                        if (rcnt == 0)
                        {
                            ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Select Sort by Option');", true);
                        }
                    }
                    else
                    {
                        sortby = rdbplcaseno.Text;
                        rcnt = 1;
                    }
                }
                else
                {
                    sortby = rdbloandate.Text;
                    rcnt = 1;
                }
            }
            else
            {
                sortby = rdbName.Text;
                rcnt = 1;
            }

        }
        else
        {
            sortby = rdbGLno.Text;
            rcnt = 1;
        }
        for (int i = 0; i < grdgold.Rows.Count; i++)
        {
            grdgold.SelectedIndex = i;
            System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
            if (chkBx1.Checked)
            {
                cnt = cnt + 1;
            }
            else
            {
                System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                string s = lblfieldname.Text;

            }

            System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
            if (chk.Checked)
            {
                System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                string s = lblfieldname.Text;
                cnt = cnt + 1;
            }
            //if (cnt == 0)
            //{
            //    ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

            //}

        }
        if (cnt == 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

        }
        if (cnt > 1)
        {

            if (rcnt > 0)
            {
                conn = new SqlConnection(strConnString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "GL_Register_New";
                cmd.CommandType = CommandType.StoredProcedure;
                da = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@Fromdate", gbl.ChangeDateMMddyyyy(txtPeriodDateFrom.Text));
                cmd.Parameters.AddWithValue("@Todate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                cmd.Parameters.AddWithValue("@sortby", sortby);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {

                        //KYCID = Convert.ToInt32(ds.Tables[0].Rows[i]["KYCID"]);
                        GoldLoanNo = ds.Tables[0].Rows[i]["GL No"].ToString();
                        //userentereddate = gbl.ChangeDateMMddyyyy(txtperiodtodate.Text);
                        //receiveddate = ds.Tables[0].Rows[i]["Received Date"].ToString();
                        //BalanceLoanPayable = Convert.ToInt32(ds.Tables[0].Rows[i]["BalanceLoanPayable"]);
                        //SDID = Convert.ToInt32(ds.Tables[0].Rows[i]["SDID"]);

                        //conn = new SqlConnection(strConnString);
                        //cmd = new SqlCommand();
                        //cmd.Connection = conn;
                        //cmd.CommandText = "GL_Reg";
                        //cmd.CommandType = CommandType.StoredProcedure;
                        //SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                        //cmd.Parameters.AddWithValue("@Receiveddate", gbl.ChangeDateMMddyyyy(receiveddate));
                        //cmd.Parameters.AddWithValue("@Todate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                        //DataSet ds1 = new DataSet();
                        //da1.Fill(ds1);

                        // totaldays = Convert.ToInt32(ds1.Tables[0].Rows[0]["Totaldays"]);
                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 0;

                        //Addeed by Priya on 6-10-2015
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails  where isActive='Y' AND  GoldLoanNo='" + GoldLoanNo + "'";
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            neworold = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        SqlCommand cmdRcpt, cmdRoiRow;
                        int RcptID = 0;
                        cmdRcpt = new SqlCommand();
                        cmdRcpt.Connection = conn;
                        cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where  GoldLoanNo='" + GoldLoanNo + "'";
                        if (cmdRcpt.ExecuteScalar() != DBNull.Value)
                        {
                            RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
                        }

                        int RowRoiID = 1;
                        //Check if Advance Interest is paid then Pass top 1 RowID
                        if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                        {
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[i]["SID"].ToString();
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }
                        else if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) > 0)
                        {
                            //Check if Half Interest is paid then Pass Max RowID
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[i]["SID"].ToString();
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }
                        else if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                        {
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }


                        if (ds.Tables[0].Rows[i]["InterestFromDate"].ToString() != "" && ds.Tables[0].Rows[i]["InterestFromDate"].ToString() != null)
                        {
                            InterestFromDate = ds.Tables[0].Rows[i]["InterestFromDate"].ToString();
                        }
                        else
                        {
                            InterestFromDate = System.DateTime.Today.ToShortDateString();
                        }

                        if (ds.Tables[0].Rows[i]["InterestToDate"].ToString() != "" && ds.Tables[0].Rows[i]["InterestToDate"].ToString() != null)
                        {
                            InterestToDate = ds.Tables[0].Rows[i]["InterestToDate"].ToString();
                        }
                        else
                        {
                            InterestToDate = System.DateTime.Today.ToShortDateString();
                        }


                        if (ds.Tables[0].Rows[i]["RecvInterest"].ToString() != "" && ds.Tables[0].Rows[i]["RecvInterest"].ToString() != null)
                        {
                            RvcCLI = ds.Tables[0].Rows[i]["RecvInterest"].ToString();
                        }
                        else
                        {
                            RvcCLI = "0";
                        }


                        if (ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != "" && ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != null)
                        {
                            AdvInterestFromDate = ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString();
                        }
                        else
                        {
                            AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
                        }

                        if (ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != "" && ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != null)
                        {
                            AdvInterestToDate = ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString();
                        }
                        else
                        {
                            AdvInterestToDate = System.DateTime.Today.ToShortDateString();
                        }
                        //**************


                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GL_EmiCalculator_RTR";

                        string lastrec, today, todayDateTime;
                        lastrec = ds.Tables[0].Rows[i]["LastRecDate"].ToString();

                        todayDateTime = DateTime.Parse(lastrec).ToShortDateString();
                        today = txtperiodtodate.Text;

                        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[0]["LoanDate"].ToString()));

                        if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) >= 0)
                        {
                            double AddPrvInt = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[i]["Outstanding Principal"].ToString()) + Convert.ToDouble(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()));
                            cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@LoanAmount", (ds.Tables[0].Rows[i]["Outstanding Principal"].ToString()));
                        }

                        cmd.Parameters.AddWithValue("@SID", ds.Tables[0].Rows[i]["SID"].ToString());
                        cmd.Parameters.AddWithValue("@NeworOld", neworold);

                        cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

                        cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmd.Parameters.AddWithValue("@OSIntAmt", ds.Tables[0].Rows[i]["OSIntAmt"].ToString());

                        cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                        cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                        cmd.Parameters.AddWithValue("@AdvInterestAmt", ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString());

                        if (Convert.ToDateTime(todayDateTime) > Convert.ToDateTime(today))
                        {
                            cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(today));
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[i]["LastRecDate"].ToString()));
                        }
                        cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                        cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

                        SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                        DataSet ds2 = new DataSet();
                        da2.Fill(ds2);

                        if (ds2.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < ds2.Tables[0].Rows.Count; j++)
                            {
                                if (!(System.Convert.IsDBNull(ds2.Tables[0].Rows[j]["InterestAmount"])))
                                {
                                    interestamt = Convert.ToDecimal(ds2.Tables[0].Rows[j]["InterestAmount"]); //priya
                                    interestdue = (interestdue + interestamt);
                                }
                                ds.Tables[0].Rows[i]["ROI"] = ds2.Tables[0].Rows[j]["ROI"];
                                //else
                                //{
                                //    interestamt = Math.Round(Convert.ToDecimal(ds2.Tables[0].Rows[j]["PrevOSInterest"]), 0);  //priya
                                //    interestdue = Math.Round(interestdue + interestamt);
                                //}
                            }
                            ds.Tables[0].Rows[i]["Interest overdue as on date"] = Math.Round(interestdue);

                            interestdue = 0;
                        }
                        else
                        {
                            interestdue = 0;
                            ds.Tables[0].Rows[i]["Interest overdue as on date"] = "0";
                        }

                        // Response.Write("interest" + ds.Tables[0].Rows[i]);

                        //******** CODE ADDED BY PRIYA FOR ONLINE RATE CALCULATION
                        if (ds.Tables[0].Rows[i]["SDID"] != "")
                        {
                            conn = new SqlConnection(strConnString);
                            conn.Open();

                            //strOnlineRates = "select Purity,NetWeight from TGLSanctionDisburse_GoldItemDetails where SDID=" + ds.Tables[0].Rows[i]["SDID"];
                            //  strOnlineRates = "select gd.Purity,gd.NetWeight,SD.NetLoanAmtSanctioned,rd.BalanceLoanPayable,rd.RcvTotal,(rd.CLP+rd.CLI)TotalPI from TGLSanctionDisburse_GoldItemDetails gd left join TGLSanctionDisburse_BasicDetails SD ON SD.SDID=gd.SDID left join TGlReceipt_BasicDetails  rd on gd.SDID=rd.SDID WHERE GD.SDID=" + ds.Tables[0].Rows[i]["SDID"];


                            strOnlineRates = "select gd.Purity,gd.NetWeight,SD.NetLoanAmtSanctioned,rd.BalanceLoanPayable,rd.RcvTotal,(rd.CLP+rd.CLI)TotalPI from TGLSanctionDisburse_GoldItemDetails gd left join TGLSanctionDisburse_BasicDetails SD ON SD.SDID=gd.SDID left join TGlReceipt_BasicDetails  rd on gd.SDID= SD.SDID and rd.RcptId=(select max(RcptId)FROM TGlReceipt_BasicDetails where sdid= '" + ds.Tables[0].Rows[i]["SDID"] + "') WHERE GD.SDID=" + ds.Tables[0].Rows[i]["SDID"];
                            SqlDataAdapter daRates = new SqlDataAdapter(strOnlineRates, conn);
                            DataSet dsRates = new DataSet();
                            daRates.Fill(dsRates);

                            //  decimal balAmount = 0, FinalAmount = 0;

                            if (dsRates.Tables[0].Rows.Count > 0)
                            {
                                for (int r = 0; r < dsRates.Tables[0].Rows.Count; r++)
                                {

                                    if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "18K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (Eighteenkarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "20K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (Twentykarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "21K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (TwentyOnekarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "22K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (TwentyTwokarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "23K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (TwentyThreeCarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "24K")
                                    {
                                        balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (TwentyFourkarat / 10);
                                    }


                                    //  FinalAmount = balAmount + Math.Round(balAmount);
                                    FinalAmount = FinalAmount + balAmount;

                                    //if (dsRates.Tables[0].Rows[r]["TotalPI"].ToString() != "")
                                    //{
                                    //    //OutstndAmt = OutstndAmt + Convert.ToDecimal(dsRates.Tables[0].Rows[r]["TotalPI"].ToString());
                                    //    OutstndAmt = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["TotalPI"].ToString());
                                    //}
                                    //else
                                    //{
                                    //    OutstndAmt = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetLoanAmtSanctioned"].ToString());
                                    //}

                                    OutstndAmt = Convert.ToDecimal(ds.Tables[0].Rows[i]["Outstanding Principal"].ToString()) + Convert.ToDecimal(ds.Tables[0].Rows[i]["Interest overdue as on date"]);
                                }

                            }


                            LTVRates = FinalAmount * (Convert.ToDecimal(ds.Tables[0].Rows[i]["LTV"].ToString()) / 100); //FINAL LTV AMT

                        }

                        decimal balanceLoan = Math.Round(LTVRates - OutstndAmt);

                        //due to negative value on18/12/2015
                        ds.Tables[0].Rows[i]["Balance Eligibility"] = Math.Round(LTVRates - OutstndAmt);

                        if (balanceLoan > 0)
                        {
                            ds.Tables[0].Rows[i]["Balance Eligibility"] = balanceLoan;
                        }
                        else
                        {
                            ds.Tables[0].Rows[i]["Balance Eligibility"] = 0;
                        }



                        FinalAmount = 0;
                        // Response.Write("ltv" + ds.Tables[0].Rows[i]);
                        //*******//

                        if (ds.Tables[0].Rows[i]["PL Case No"] != "")
                        {
                            CaseNo = Convert.ToInt32(ds.Tables[0].Rows[i]["PL Case No"]);

                            conn = new SqlConnection(strConnStringAIM);
                            conn.Open();
                            strQuery = "select B.CaseNo, Isnull(P.Interest,0) as 'Interest Overdue', Isnull(P.Principal,0) as 'Outstanding Balance'" +
                                        "from TDisbursement_Appl_BasicInfo B  " +
                                        "inner  join TDisbursement_Appl_PDCDetails P on b.LoginID =P.LoginID" +
                                        " where P.Clearstatus='UC'" +
                                        " and B.CaseNo =" + CaseNo +
                                        " and P.Date  < '" + gbl.ChangeDateMMddyyyy(txtperiodtodate.Text) + "'";


                            SqlDataAdapter daaim = new SqlDataAdapter(strQuery, conn);
                            DataSet dsaim = new DataSet();
                            daaim.Fill(dsaim);

                            if (dsaim.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsaim.Tables[0].Rows.Count; j++)
                                {
                                    plinterestamt = Convert.ToDecimal(dsaim.Tables[0].Rows[j]["Interest Overdue"]);
                                    plinterestdue = plinterestdue + plinterestamt;



                                }
                                ds.Tables[0].Rows[i]["PL Overdue Interest"] = plinterestdue.ToString();
                                plinterestamt = 0;
                                plinterestdue = 0;
                            }
                            else
                            {
                                ds.Tables[0].Rows[i]["PL Overdue Interest"] = 0;
                                plinterestamt = 0;
                                plinterestdue = 0;

                            }



                            strQuery1 = "select B.CaseNo, Isnull(P.Principal,0) as 'Outstanding Balance'" +
                                       "from TDisbursement_Appl_BasicInfo B  " +
                                       "inner  join TDisbursement_Appl_PDCDetails P on b.LoginID =P.LoginID" +
                                       " where P.Clearstatus='UC'" +
                                       " and B.CaseNo =" + CaseNo;


                            SqlDataAdapter daaim2 = new SqlDataAdapter(strQuery1, conn);
                            DataSet dsaim2 = new DataSet();
                            daaim2.Fill(dsaim2);

                            if (dsaim2.Tables[0].Rows.Count > 0)
                            {
                                for (int j = 0; j < dsaim2.Tables[0].Rows.Count; j++)
                                {
                                    ploutstamt = Convert.ToDecimal(dsaim2.Tables[0].Rows[j]["Outstanding Balance"]);
                                    ploutstanding = ploutstanding + ploutstamt;
                                }
                                ds.Tables[0].Rows[i]["PL Outstanding Balance"] = ploutstanding.ToString();
                                dsaim2.Tables[0].Rows.Clear();
                                ploutstamt = 0;
                                ploutstanding = 0;
                            }
                            else
                            {
                                ds.Tables[0].Rows[i]["PL Outstanding Balance"] = 0;
                                ploutstamt = 0;
                                ploutstanding = 0;
                            }

                            if (ds.Tables[0].Rows[i]["Address"] != "")
                            {
                                StateID = Convert.ToInt32(ds.Tables[0].Rows[i]["StateID"]);
                                CityID = Convert.ToInt32(ds.Tables[0].Rows[i]["CityID"]);
                                AreaID = Convert.ToInt32(ds.Tables[0].Rows[i]["AreaID"]);
                                ZoneID = Convert.ToInt32(ds.Tables[0].Rows[i]["ZoneID"]);

                                conn = new SqlConnection(strConnStringAIM);
                                conn.Open();
                                strQuery = "select A.Area+','+ C.CityName +'('+Z.Zone+')'+'-'+A.Pincode as 'Addresscode'" +
                                            "from tblStateMaster S " +
                                            "inner join tblCityMaster C on s.StateID =C.StateID " +
                                            "inner join tblAreaMaster A on A.CityID =C.CityID " +
                                            "inner join tblZonemaster Z on Z.AreaID =A.AreaID " +
                                            "where S.StateID  =" + StateID +
                                            "and C.CityID =" + CityID +
                                            "and A.AreaID =" + AreaID +
                                            "and Z.ZoneID =" + ZoneID;


                                SqlDataAdapter daaim1 = new SqlDataAdapter(strQuery, conn);
                                DataSet dsaim1 = new DataSet();
                                daaim1.Fill(dsaim1);

                                if (dsaim1.Tables[0].Rows.Count > 0)
                                {
                                    address = ds.Tables[0].Rows[i]["Address"].ToString();
                                    fulladdress = address + ',' + dsaim1.Tables[0].Rows[0]["Addresscode"].ToString();
                                    ds.Tables[0].Rows[i]["Address"] = fulladdress;
                                }
                            }
                        }
                        else
                        {
                            CaseNo = 0;
                            ds.Tables[0].Rows[i]["PL Overdue Interest"] = "0";
                            ds.Tables[0].Rows[i]["PL Outstanding Balance"] = "0";
                        }

                        //if (ds.Tables[0].Rows[i]["Address"] != "")
                        //{
                        //    StateID = Convert.ToInt32(ds.Tables[0].Rows[i]["StateID"]);
                        //    CityID = Convert.ToInt32(ds.Tables[0].Rows[i]["CityID"]);
                        //    AreaID = Convert.ToInt32(ds.Tables[0].Rows[i]["AreaID"]);
                        //    ZoneID = Convert.ToInt32(ds.Tables[0].Rows[i]["ZoneID"]);

                        //    conn = new SqlConnection(strConnStringAIM);
                        //    conn.Open();
                        //    strQuery = "select A.Area+','+ C.CityName +'('+Z.Zone+')'+'-'+A.Pincode as 'Addresscode'" +
                        //                "from tblStateMaster S " +
                        //                "inner join tblCityMaster C on s.StateID =C.StateID " +
                        //                "inner join tblAreaMaster A on A.CityID =C.CityID " +
                        //                "inner join tblZonemaster Z on Z.AreaID =A.AreaID " +
                        //                "where S.StateID  =" + StateID +
                        //                "and C.CityID =" + CityID +
                        //                "and A.AreaID =" + AreaID +
                        //                "and Z.ZoneID =" + ZoneID;


                        //    SqlDataAdapter daaim1 = new SqlDataAdapter(strQuery, conn);
                        //    DataSet dsaim1 = new DataSet();
                        //    daaim1.Fill(dsaim1);

                        //    if (dsaim1.Tables[0].Rows.Count > 0)
                        //    {
                        //        address = ds.Tables[0].Rows[i]["Address"].ToString();
                        //        fulladdress = address + ',' + dsaim1.Tables[0].Rows[0]["Addresscode"].ToString();
                        //        ds.Tables[0].Rows[i]["Address"] = fulladdress;
                        //    }
                        //}


                        //commented bcoz ftpl is not live
                        ////Code Added by Priya for FTPL/TL/PL Column(13-8-2015)
                        //if (ds.Tables[0].Rows[i]["Pan No"] != String.Empty)
                        //{
                        //    SqlDataAdapter daFTPLData;
                        //    DataSet dsFTPLData, dsFTPLLogin;

                        //    conn = new SqlConnection(strConnStringFTPL);
                        //    conn.Open();

                        //    strQuery = "select LOGINID from TPLAppForm_BasicDetails TPLApp " +
                        //                  "where TPLApp.PanNo  =" + "'" + ds.Tables[0].Rows[i]["Pan No"] + "'";

                        //    SqlDataAdapter daFTPLLogin = new SqlDataAdapter(strQuery, conn);
                        //    dsFTPLLogin = new DataSet();
                        //    daFTPLLogin.Fill(dsFTPLLogin);
                        //    string getData = string.Empty;
                        //    if (dsFTPLLogin.Tables[0].Rows.Count > 0)
                        //    {
                        //        for (int fill = 0; fill < dsFTPLLogin.Tables[0].Rows.Count; fill++)
                        //        {
                        //            string strQueryFTPLData = "select " +
                        //               "TPLApp.LoanType +'('+ isnull(TPLApp.CaseType,'')+')-' + CONVERT(VARCHAR(20),isnull(TBASIC.CaseNo,0),20)AS 'FTPL/PL/TL' " +
                        //               "from TPLAppForm_BasicDetails TPLApp " +
                        //               "INNER JOIN  TDisbursement_Appl_BasicInfo TBASIC ON TPLApp.LOGINID=TBASIC.LOGINID " +
                        //                "where TPLApp.LOGINID  =" + "'" + dsFTPLLogin.Tables[0].Rows[fill]["LOGINID"] + "'";

                        //            daFTPLData = new SqlDataAdapter(strQueryFTPLData, conn);
                        //            dsFTPLData = new DataSet();
                        //            daFTPLData.Fill(dsFTPLData);

                        //            if (dsFTPLData.Tables[0].Rows.Count > 0)
                        //            {
                        //                if (getData == string.Empty)
                        //                {
                        //                    getData = dsFTPLData.Tables[0].Rows[0]["FTPL/PL/TL"].ToString();
                        //                }
                        //                else
                        //                {
                        //                    getData += "," + dsFTPLData.Tables[0].Rows[0]["FTPL/PL/TL"].ToString();
                        //                }
                        //            }
                        //            else
                        //            {
                        //                getData = "N/A";
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        getData = "N/A";
                        //    }

                        //    ds.Tables[0].Rows[i]["FTPL/PL/TL"] = getData;
                        //}
                        ds.Tables[0].Rows[i]["FTPL/PL/TL"] = "";
                    }

                } // [InterestFromDate],[InterestToDate],[RecvInterest],[OSIntAmt],[AdvInterestFromDate],[AdvInterestToDate],[AdvInterestAmount]

                ds.Tables[0].Columns.Remove("RN");
                ds.Tables[0].Columns.Remove("LoanDate");
                //  ds.Tables[0].Columns.Remove("LoanAmountCalc");
                ds.Tables[0].Columns.Remove("SDID");
                ds.Tables[0].Columns.Remove("KYCID");
                ds.Tables[0].Columns.Remove("SID");
                ds.Tables[0].Columns.Remove("Received Date");
                //ds.Tables[0].Columns.Remove("BalanceLoanPayable");
                ds.Tables[0].Columns.Remove("StateID");
                ds.Tables[0].Columns.Remove("CityID");
                ds.Tables[0].Columns.Remove("AreaID");
                ds.Tables[0].Columns.Remove("ZoneID");
                ds.Tables[0].Columns.Remove("CLI");
                ds.Tables[0].Columns.Remove("LastRecDate");

                ds.Tables[0].Columns.Remove("InterestFromDate");
                ds.Tables[0].Columns.Remove("InterestToDate");
                ds.Tables[0].Columns.Remove("RecvInterest");
                ds.Tables[0].Columns.Remove("OSIntAmt");
                ds.Tables[0].Columns.Remove("AdvInterestFromDate");
                ds.Tables[0].Columns.Remove("AdvInterestToDate");
                ds.Tables[0].Columns.Remove("AdvInterestAmount");


                for (int i = 0; i < grdgold.Rows.Count; i++)
                {
                    grdgold.SelectedIndex = i;
                    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
                    if (chk.Checked)
                    {
                        System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                        string s = lblfieldname.Text;
                        cnt = cnt + 1;
                    }
                    else
                    {
                        System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                        string s = lblfieldname.Text;
                        System.Web.UI.WebControls.CheckBox chk1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
                        if (chk1.Checked)
                        { cnt = cnt; }
                        else
                        {
                            //s = grdgold.Rows.
                            ds.Tables[0].Columns.Remove(s);
                        }
                        chk1.Checked = false;
                        cnt = cnt + 1;
                    }
                    if (cnt == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);
                    }

                    //grdgold.SelectedIndex = i;
                    //System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
                    //if (chkBx1.Checked)
                    //{
                    //    cnt = cnt + 1;
                    //}
                    //else
                    //{
                    //    System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                    //    string s = lblfieldname.Text;
                    //    ds.Tables[0].Columns.Remove(s);
                    //}

                    //System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
                    //if (chk.Checked)
                    //{
                    //    System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                    //    string s = lblfieldname.Text;
                    //    cnt = cnt + 1;
                    //}
                    //if (cnt == 0)
                    //{
                    //    ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);
                    //}
                }
                if (cnt > 0)
                {

                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        //ExportToExcel(ds);
                        ExportToexcelDS(ds);
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "", "alert('No Record Found For Selected Date Range');", true);
                    }

                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "alert('No Record Found For Selected Date Range');", true);
            }
        }
    }

    protected void gvExcel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            // tCell.Style.Add("vertical-align", "middle")

            for (int r = 0; r < ExlColCount; r++)
            {
                e.Row.Cells[r].Style.Add("vertical-align", "middle");
                e.Row.Cells[r].HorizontalAlign = HorizontalAlign.Center;

                if (ExlColCount > 12)
                {
                    decimal OSPrinciple = Convert.ToDecimal(e.Row.Cells[13].Text.ToString());

                    if (OSPrinciple == 0)
                    {
                        e.Row.Cells[r].BackColor = Color.LightBlue;
                    }
                    OSPrinciple = 0;
                }
            }

            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;

            if (ExlColCount > 7)
            {
                e.Row.Cells[6].HorizontalAlign = HorizontalAlign.Left;
            }
            if (ExlColCount > 8)
            {
                e.Row.Cells[7].HorizontalAlign = HorizontalAlign.Left;
            }
            if (ExlColCount > 21)
            {
                e.Row.Cells[20].HorizontalAlign = HorizontalAlign.Left;
            }
            if (ExlColCount > 23)
            {
                e.Row.Cells[24].HorizontalAlign = HorizontalAlign.Left;
            }
            if (ExlColCount > 24)
            {
                e.Row.Cells[25].HorizontalAlign = HorizontalAlign.Left;
            }

        }

    }
    public void ExportToexcelDS(DataSet ds)
    {
        string row1 = "";
        string col1 = "";
        string row2 = "";
        string col2 = "";

        string lastrow = "";
        string lastcol = "";

        string filename = "GoldLoanRegisterExcel.xls";
        System.IO.StringWriter tw = new System.IO.StringWriter();
        System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);

        //DataGrid dgGrid = new DataGrid();
        ExlColCount = ds.Tables[0].Columns.Count;
        int rowcount = ds.Tables[0].Columns.Count;

        if (rowcount < 5)
        {
            rowcount = 5;
        }
        else
        {
            rowcount = ds.Tables[0].Columns.Count;
        }

        DateTime fromdate = Convert.ToDateTime(txtPeriodDateFrom.Text);
        string fromdate1 = fromdate.ToString("dd/MM/yyyy");
        DateTime todate = Convert.ToDateTime(txtperiodtodate.Text);
        string todate1 = todate.ToString("dd/MM/yyyy");
        hw.Write("<table><tr ><td colspan='" + rowcount + "' style ='font-size :30 ; font-family :Verdana;  color :Red; font-weight:bold;'>Aphelion Finance Pvt. Ltd.</td></tr>");
        hw.Write("<tr><td colspan='" + rowcount + "'  style ='font-size :20 ; font-family :Cambria;  color :DarkBlue;font-weight:bold;'>Report : Gold Loan Register Period :" + fromdate1 + " To " + todate1 + "</td></tr>");
        hw.Write("<tr><td colspan='" + rowcount + "' </td></tr></table>");

        gvExcel.DataSource = ds.Tables[0];
        gvExcel.DataBind();

        //*********************Priya // 17-4-2015
        //gvExcel.HeaderRow.Cells[21].Visible = false;
        //gvExcel.HeaderRow.Cells[22].Visible = false;
        for (int i = 0; i < gvExcel.HeaderRow.Cells.Count; i++)
        {
            gvExcel.HeaderRow.Cells[i].Style.Add("background-color", "#FFFFCC");
            string h = gvExcel.HeaderRow.Cells[i].Text.ToString();

        }
        gvExcel.Style.Add("background-color", "#FFFFCC");
        gvExcel.BorderStyle = BorderStyle.Solid;
        gvExcel.RenderControl(hw);
        hw.Write("<table><tr><td colspan='" + ds.Tables[0].Columns.Count + "' </td></tr></table>");
        //Write the HTML back to the browser.     
        Response.ContentType = "application/vnd.ms-excel";
        Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
        this.EnableViewState = false;
        Response.Write(tw.ToString());
        Response.End();

    }



    public static String getColumnNameFromIndex(int column)
    {
        column--;
        String col = Convert.ToString((char)('A' + (column % 26)));
        while (column >= 26)
        {
            column = (column / 26) - 1;
            col = Convert.ToString((char)('A' + (column % 26))) + col;
        }
        return col;
    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        Clear();

    }

    public void Clear()
    {
        try
        {
            txtPeriodDateFrom.Text = "";
            txtperiodtodate.Text = "";
            rdbGLno.Checked = true;
            rdbplcaseno.Checked = false;
            rdbName.Checked = false;
            rdbloandate.Checked = false;
            for (int i = 0; i < grdgold.Rows.Count; i++)
            {
                grdgold.SelectedIndex = i;
                System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
                chkBx1.Checked = false;

                System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
                chk.Checked = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLRegisterReport", "alert('" + ex.Message + "');", true);
        }
    }

}