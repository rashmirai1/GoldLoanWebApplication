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
//using Microsoft.Office.Interop;
//using Microsoft.Office.Interop.Excel;
//using System.Runtime.InteropServices;
using System.Reflection;
using System.Net;

public partial class GLDefaultPLReport : System.Web.UI.Page
{

    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;


    GlobalSettings gbl = new GlobalSettings();

    bool datasaved = false;
    //Declaring Variables.
    int result = 0;
    int ExlColCount = 0;
    string lastrec, today, todayDateTime;
    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    System.Data.DataTable dt = new System.Data.DataTable();
    //DataTable dt1;
    SqlCommand cmd;
    string temp;
    decimal TwentyTwokarat, TwentyOnekarat, Twentykarat, Eighteenkarat, TwentyFourkarat, TwentyThreeCarat;
    string strOnlineRates = "";
    decimal balAmount = 0, FinalAmount = 0, LTVRates = 0, OutstndAmt = 0;


    #endregion [Declarations]
    protected void Page_Init(object sender, EventArgs e)
    {

        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);

        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);

        gvExcel.RowDataBound += new GridViewRowEventHandler(gvExcel_RowDataBound);

        gvPLExcel.RowDataBound += new GridViewRowEventHandler(gvPLExcel_RowDataBound);

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
                rdbGL.Checked = true;
                BindGLDefault();
                rdbGLno.Checked = true;
                RDBSelection();

            }
            Master.PropertybtnView.OnClientClick = "return CheckValidDate();";
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnSearch.Visible = false;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLDefaultPLReport", "alert('" + ex.Message + "');", true);
        }


    }

    public string rates(string urlAddress)
    {

        try
        {
            //string urlAddress = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm";
            //string urlAddress = "https://www.moneycontrol.com/";
            //"https://www.moneycontrol.com/";
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

                //string abb = data.Substring(data.LastIndexOf("Gold Price in Mumbai"), 161);
                //string rate = abb.Substring(155, 6);

                string abb = data.Substring(data.LastIndexOf("pad-15"), 14);
                string rate = abb.Substring(8, 6);

                temp = rate.Replace(",", "");

                //temp = data.Substring(data.LastIndexOf("10 gram"), 30);
                //string ch = "Gold price slips marginally to Rs ";
                //string abb = data.Substring(data.LastIndexOf("Gold price slips marginally to Rs"), 40);
                //temp = abb.TrimStart(ch.ToCharArray());
                //temp = temp.Replace("<TD>", "");
                //temp = temp.Replace("</TD>", "");
                //temp = temp.Replace("\t", " ");
                //temp = temp.Replace("\n", " ");
                //temp = temp.Replace("10 gram", "");
                //temp = temp.Replace(" ", "");
                //temp = temp.Replace("Rs.", "");
                //temp = temp.Replace(",", "");

                //decimal TwentyThreeCarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.958);
                //lblGoldRate23.Text += " " + decimal.Round(TwentyThreeCarat, 2);

                //decimal TwentyTwokarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.916);
                //lblGoldRate22.Text += " " + decimal.Round(TwentyTwokarat, 2);

                //decimal TwentyOnekarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.875);
                //lblGoldRate21.Text += " " + decimal.Round(TwentyOnekarat, 2);

                //decimal Twentykarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.830);
                //lblGoldRate20.Text += " " + decimal.Round(Twentykarat, 2);

                //decimal Eighteenkarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.75);
                //lblGoldRate18.Text += " " + decimal.Round(Eighteenkarat, 2);
                //Hidden18k.Value = lblGoldRate18.Text;

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
    public void RDBSelection()
    {
        if (rdbGL.Checked == true)
        {
            plcaseno.Visible = false;
            rdbplcaseno.Visible = false;
        }
        else
        {
            plcaseno.Visible = true;
            rdbplcaseno.Visible = true;
        }
    }
    public void BindGLDefault()
    {
        if (rdbGL.Checked == true)
        {

            dt.Columns.Add("FieldName");
            dt.Rows.Add("GL No");
            dt.Rows.Add("Name");
            dt.Rows.Add("Loan Amount");
            dt.Rows.Add("Sourced Through");
            dt.Rows.Add("LTV");
            dt.Rows.Add("Overdue Interest");
            dt.Rows.Add("Outstanding Principal");
            dt.Rows.Add("Days lapsed");
            dt.Rows.Add("Interest Currently charged @");
            dt.Rows.Add("Outstanding(%)");
            dt.Rows.Add("Penal Interest");
            dt.Rows.Add("Loan Repayment Date");
            dt.Rows.Add("Tel No");
            grdgold.DataSource = dt;
            grdgold.DataBind();
        }
        else
        {

            dt.Columns.Add("FieldName");
            dt.Rows.Add("GL No");
            dt.Rows.Add("Name");
            dt.Rows.Add("Loan Amount");
            dt.Rows.Add("Sourced Through");
            dt.Rows.Add("LTV");
            dt.Rows.Add("Overdue Interest");
            dt.Rows.Add("Outstanding Principal");
            dt.Rows.Add("Days lapsed");
            dt.Rows.Add("Interest Currently charged @");
            dt.Rows.Add("Outstanding(%)");
            dt.Rows.Add("Penal Interest");
            dt.Rows.Add("Loan Repayment Date");
            dt.Rows.Add("Tel No");
            dt.Rows.Add("PL Case No");
            dt.Rows.Add("PL Overdue Interset");
            dt.Rows.Add("PL Outstanding Balance");
            grdgold.DataSource = dt;
            grdgold.DataBind();
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            if (rdbGL.Checked == true)
            {
                GetDefaultGLDetails();
            }
            else
            {
                GetDefaultGL_PLDetails();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLDefaultReport", "alert('" + ex.Message + "');", true);
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {

        //don't throw any exception!

    }

    public void GetDefaultGLDetails()
    {
        try
        {
            string temp = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            //"http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
            //temp = temp.Substring(0, temp.IndexOf("@"));
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
            int cnt = 0;
            int rcnt = 0;
            string sortby = "";
            decimal goldvalue = 0;
            decimal outstandingPer = 0;
            decimal penalint = 0;
            int Baldays = 0;

            string LastDate, LastCLI, LastLoanAmount = string.Empty;
            string InterestFromDate = string.Empty;
            string InterestToDate = string.Empty;
            string RvcCLI = string.Empty;
            string AdvInterestFromDate = string.Empty;
            string AdvInterestToDate = string.Empty;
            int neworold = 0;

            if (rdbGLno.Checked == false)
            {
                if (rdbName.Checked == false)
                {
                    rcnt = 0;
                    if (rcnt == 0)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Select Sort by Option');", true);
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
            //for (int i = 0; i < grdgold.Rows.Count; i++)
            //{
            //    grdgold.SelectedIndex = i;
            //    System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
            //    if (chkBx1.Checked)
            //    {
            //        cnt = cnt + 1;
            //    }
            //    else
            //    {
            //        System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
            //        string s = lblfieldname.Text;

            //    }

            //    System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
            //    if (chk.Checked)
            //    {
            //        System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
            //        string s = lblfieldname.Text;
            //        cnt = cnt + 1;
            //    }
            //    if (cnt == 0)
            //    {
            //        ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

            //    }

            //}
            //if (cnt > 1)
            //{
            int outk = 0;
            if (rcnt > 0)
            {
                conn = new SqlConnection(strConnString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "GL_Default_New";
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
                        GoldLoanNo = ds.Tables[0].Rows[i]["GL No"].ToString();
                        BalanceLoanPayable = Convert.ToInt32(ds.Tables[0].Rows[i]["Outstanding Principal"]);
                        goldvalue = Convert.ToDecimal(ds.Tables[0].Rows[i]["Value of gold"]);

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

                        SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                        DataSet ds1 = new DataSet();
                        da1.Fill(ds1);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {
                            for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                            {
                                totaldays = totaldays + Convert.ToInt32(ds1.Tables[0].Rows[m]["TotalDays"]);

                                if (!(System.Convert.IsDBNull(ds1.Tables[0].Rows[m]["InterestAmount"])))
                                {
                                    interestamt = Convert.ToDecimal(ds1.Tables[0].Rows[m]["InterestAmount"]); //priya
                                    interestdue = (interestdue + interestamt);
                                }

                                ds.Tables[0].Rows[i]["Interest currently charged @"] = ds1.Tables[0].Rows[m]["ROI"].ToString();
                            }
                            ds.Tables[0].Rows[i]["Overdue Interest"] = Math.Round(interestdue);
                            ds.Tables[0].Rows[i]["Days Lapsed"] = totaldays;
                            totaldays = 0;

                            cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandTimeout = 0;
                            cmd.CommandText = "GL_Default_Penalint";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter da7 = new SqlDataAdapter(cmd);
                            cmd.Parameters.AddWithValue("@Duedate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[i]["Loan Repayment Date"].ToString()));
                            cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                            cmd.Parameters.AddWithValue("@BalanceLoanPayable", BalanceLoanPayable);
                            cmd.Parameters.AddWithValue("@TotalDays", ds.Tables[0].Rows[i]["Days Lapsed"]);
                            DataSet ds7 = new DataSet();
                            da7.Fill(ds7);
                            if (ds7.Tables[0].Rows.Count > 0)
                            {
                                penalint = Math.Round(Convert.ToDecimal(ds7.Tables[0].Rows[0]["Penal Interest"]));

                                ds.Tables[0].Rows[i]["Penal Interest"] = Math.Round(penalint);

                            }
                            interestdue = 0;

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

                                    }

                                }
                                outstandingPer = Math.Round(((Convert.ToDecimal(BalanceLoanPayable + penalint + Convert.ToDecimal(ds.Tables[0].Rows[i]["Overdue Interest"])) / Convert.ToDecimal(FinalAmount)) * 100), 2);
                                if (outstandingPer != 0)
                                {
                                    ds.Tables[0].Rows[i]["Outstanding(%)"] = outstandingPer;
                                }

                            }



                            penalint = 0; outstandingPer = 0; FinalAmount = 0; balAmount = 0;

                        }
                        else
                        {
                            interestdue = 0;
                            interestamt = 0;
                            ds.Tables[0].Rows[i]["Overdue Interest"] = 0;
                            ds.Tables[0].Rows[i]["Interest currently charged @"] = 0;
                            ds.Tables[0].Rows[i]["Outstanding(%)"] = 0;
                            ds.Tables[0].Rows[i]["Days Lapsed"] = 0;
                            ds.Tables[0].Rows.RemoveAt(i);
                            i--;
                        }
                    }
                }
                else
                {
                    interestdue = 0;
                    interestamt = 0;
                }
            }




            if (ds.Tables[0].Rows.Count > 0)
            {
                ds.Tables[0].Columns.Remove("LoanDate");
                ds.Tables[0].Columns.Remove("LoanAmountCalc");
                ds.Tables[0].Columns.Remove("CLI");
                ds.Tables[0].Columns.Remove("LastRecDate");
                ds.Tables[0].Columns.Remove("SID");
                ds.Tables[0].Columns.Remove("SDID");
                ds.Tables[0].Columns.Remove("KYCID");
                ds.Tables[0].Columns.Remove("Received Date");
                ds.Tables[0].Columns.Remove("PL Overdue Interset");
                ds.Tables[0].Columns.Remove("Value of gold");
                ds.Tables[0].Columns.Remove("PL Outstanding Balance");
                ds.Tables[0].Columns.Remove("PL Case No");
                ds.Tables[0].Columns.Remove("BalanceLoanPayable");

                ds.Tables[0].Columns.Remove("InterestFromDate");
                ds.Tables[0].Columns.Remove("InterestToDate");
                ds.Tables[0].Columns.Remove("RecvInterest");
                ds.Tables[0].Columns.Remove("OSIntAmt");
                ds.Tables[0].Columns.Remove("AdvInterestFromDate");
                ds.Tables[0].Columns.Remove("AdvInterestToDate");
                ds.Tables[0].Columns.Remove("AdvInterestAmount");
            }
            // 05 08 2020
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
                    ds.Tables[0].Columns.Remove(s);
                }

                System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
                if (chk.Checked)
                {
                    System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                    string s = lblfieldname.Text;
                    cnt = cnt + 1;
                }
                if (cnt == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

                }

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
            //}
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "alert('No Record Found For Selected Date Range');", true);
            }


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLDefaultReport", "alert('" + ex.Message + "');", true);
        }
    }

    public void GetDefaultGL_PLDetails()
    {
        try
        {

            string temp = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            //"http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
            //temp = temp.Substring(0, temp.IndexOf("@"));
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
            int cnt = 0;
            int rcnt = 0;
            string sortby = "";
            decimal goldvalue = 0;
            decimal outstandingPer = 0;
            decimal plinterestdue = 0;
            decimal plinterestamt = 0;
            decimal ploutstanding = 0;
            decimal ploutstamt = 0;
            decimal penalint = 0;
            int flag = 0;
            int Baldays = 0;
            int totalBaldays = 0;

            string LastDate, LastCLI, LastLoanAmount = string.Empty;
            string InterestFromDate = string.Empty;
            string InterestToDate = string.Empty;
            string RvcCLI = string.Empty;
            string AdvInterestFromDate = string.Empty;
            string AdvInterestToDate = string.Empty;
            int neworold = 0;

            if (rdbGLno.Checked == false)
            {
                if (rdbName.Checked == false)
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
                    sortby = rdbName.Text;
                    rcnt = 1;
                }

            }
            else
            {
                sortby = rdbGLno.Text;
                rcnt = 1;
            }

            //// hide 05 08 2020
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
                if (cnt == 0)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

                }

            }

            if (cnt > 1)
            {
                ///// 05 08 2020
                if (rcnt > 0)
                {
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "GL_Default_Data_New";
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

                            GoldLoanNo = ds.Tables[0].Rows[i]["GL No"].ToString();
                            BalanceLoanPayable = Convert.ToInt32(ds.Tables[0].Rows[i]["OS Principal"]);
                            goldvalue = Convert.ToDecimal(ds.Tables[0].Rows[i]["Value of gold"]);

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

                            lastrec = ds.Tables[0].Rows[i]["LastRecDate"].ToString();

                            todayDateTime = DateTime.Parse(lastrec).ToShortDateString();
                            today = txtperiodtodate.Text;

                            cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[0]["LoanDate"].ToString()));

                            if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) >= 0)
                            {
                                double AddPrvInt = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[i]["OS Principal"].ToString()) + Convert.ToDouble(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()));
                                cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@LoanAmount", (ds.Tables[0].Rows[i]["OS Principal"].ToString()));
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


                            SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                            DataSet ds1 = new DataSet();
                            da1.Fill(ds1);


                            if (ds1.Tables[0].Rows.Count > 0)
                            {

                                for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                                {
                                    totaldays = totaldays + Convert.ToInt32(ds1.Tables[0].Rows[m]["TotalDays"]);

                                    if (!(System.Convert.IsDBNull(ds1.Tables[0].Rows[m]["InterestAmount"])))
                                    {

                                        interestamt = Convert.ToDecimal(ds1.Tables[0].Rows[m]["InterestAmount"]); //priya
                                        interestdue = (interestdue + interestamt);
                                    }
                                    ds.Tables[0].Rows[i]["Interest currently charged @"] = ds1.Tables[0].Rows[m]["ROI"].ToString();
                                }
                                ds.Tables[0].Rows[i]["Overdue Interest"] = Math.Round(interestdue);

                                interestdue = 0;
                            }

                            if (ds1.Tables[0].Rows.Count > 0)
                            {

                                cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandTimeout = 0;
                                cmd.CommandText = "GL_Default_Penalint";
                                cmd.CommandType = CommandType.StoredProcedure;
                                SqlDataAdapter da7 = new SqlDataAdapter(cmd);
                                cmd.Parameters.AddWithValue("@Duedate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[i]["Loan Repayment Date"].ToString()));
                                cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(txtperiodtodate.Text));
                                cmd.Parameters.AddWithValue("@BalanceLoanPayable", BalanceLoanPayable);
                                cmd.Parameters.AddWithValue("@TotalDays", totaldays);
                                DataSet ds7 = new DataSet();
                                da7.Fill(ds7);
                                if (ds7.Tables[0].Rows.Count > 0)
                                {
                                    penalint = Convert.ToInt32(ds7.Tables[0].Rows[0]["Penal Interest"]);

                                    ds.Tables[0].Rows[i]["Penal Interest"] = Math.Round(penalint);
                                }

                                if (totaldays != 0)
                                {
                                    ds.Tables[0].Rows[i]["Days Lapsed"] = totaldays;
                                }
                                else
                                {
                                    ds.Tables[0].Rows[i]["Days Lapsed"] = "0";
                                }

                            }
                            else
                            {
                                ds.Tables[0].Rows[i]["Overdue Interest"] = "0";
                                ds.Tables[0].Rows[i]["Interest currently charged @"] = "0";
                                flag = 1;
                                ds.Tables[0].Rows.RemoveAt(i);
                                i--;
                            }

                            if (ds.Tables[0].Rows[i]["PL Case No"] != "")
                            {
                                CaseNo = Convert.ToInt32(ds.Tables[0].Rows[i]["PL Case No"]);

                                conn = new SqlConnection(strConnStringAIM);
                                conn.Open();
                                strQuery = "select B.CaseNo, Isnull((P.Interest),0) as 'Interest Overdue', Isnull((P.Principal),0) as 'Outstanding Balance'" +
                                            "from TDisbursement_Appl_BasicInfo B  " +
                                            "inner  join TDisbursement_Appl_PDCDetails P on b.LoginID =P.LoginID" +
                                            " where P.Clearstatus='UC'" +
                                            " and B.CaseNo =" + CaseNo +
                                            "and P.Date < '" + gbl.ChangeDateMMddyyyy(txtperiodtodate.Text) + "'";


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
                                    ds.Tables[0].Rows[i]["PL Overdue Interset"] = plinterestdue.ToString();

                                    dsaim.Tables[0].Rows.Clear();
                                    flag = 0;
                                    plinterestdue = 0;
                                    plinterestamt = 0;


                                }
                                else
                                {
                                    if (flag == 0)
                                    {
                                        flag = 0;
                                    }
                                    ds.Tables[0].Rows[i]["PL Overdue Interset"] = "0";
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
                                    flag = 0;
                                    ploutstamt = 0;
                                    ploutstanding = 0;
                                }
                                else
                                {
                                    if (flag == 0)
                                    {
                                        flag = 0;
                                    }
                                    ploutstamt = 0;
                                    ploutstanding = 0;
                                    ds.Tables[0].Rows[i]["PL Outstanding Balance"] = 0;
                                }


                            }
                            else
                            {
                                ploutstamt = 0;
                                ploutstanding = 0;
                                plinterestdue = 0;
                                plinterestamt = 0;
                                CaseNo = 0;
                                ds.Tables[0].Rows[i]["PL Overdue Interset"] = "0";
                                ds.Tables[0].Rows[i]["PL Outstanding Balance"] = "0";
                            }



                            if (flag == 1)
                            {
                                ds.Tables[0].Rows.RemoveAt(i);
                                i--;
                            }
                            if (ds.Tables[0].Rows.Count > 0)
                            {

                                //******** CODE ADDED BY PRIYA FOR ONLINE RATE CALCULATION
                                if (ds.Tables[0].Rows[i]["SDID"] != "")
                                {
                                    conn = new SqlConnection(strConnString);
                                    conn.Open();

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

                                            FinalAmount = FinalAmount + balAmount;

                                        }

                                    }
                                    outstandingPer = Math.Round(((Convert.ToDecimal(BalanceLoanPayable + penalint + Convert.ToDecimal(ds.Tables[0].Rows[i]["Overdue Interest"])) / Convert.ToDecimal(FinalAmount)) * 100), 2);
                                    if (outstandingPer != 0)
                                    {
                                        ds.Tables[0].Rows[i]["Outstanding(%)"] = outstandingPer;
                                    }

                                }
                            }

                        }

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].Columns.Remove("LoanDate");
                            ds.Tables[0].Columns.Remove("CLI");
                            ds.Tables[0].Columns.Remove("LastRecDate");
                            ds.Tables[0].Columns.Remove("SID");
                            ds.Tables[0].Columns.Remove("SDID");
                            ds.Tables[0].Columns.Remove("KYCID");
                            ds.Tables[0].Columns.Remove("Received Date");
                            ds.Tables[0].Columns.Remove("Value of gold");

                            ds.Tables[0].Columns.Remove("InterestFromDate");
                            ds.Tables[0].Columns.Remove("InterestToDate");
                            ds.Tables[0].Columns.Remove("RecvInterest");
                            ds.Tables[0].Columns.Remove("OSIntAmt");
                            ds.Tables[0].Columns.Remove("AdvInterestFromDate");
                            ds.Tables[0].Columns.Remove("AdvInterestToDate");
                            ds.Tables[0].Columns.Remove("AdvInterestAmount");
                        }

                        // 05 08 2020
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
                                ds.Tables[0].Columns.Remove(s);
                            }

                            System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
                            if (chk.Checked)
                            {
                                System.Web.UI.WebControls.Label lblfieldname = (System.Web.UI.WebControls.Label)grdgold.SelectedRow.FindControl("gvtxtfield");
                                string s = lblfieldname.Text;
                                cnt = cnt + 1;
                            }
                            if (cnt == 0)
                            {
                                ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Field Name');", true);

                            }

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
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLDefaultReport", "alert('" + ex.Message + "');", true);
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
            }

            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
            if (ExlColCount > 3)
            {
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;
            }

        }
    }

    protected void gvPLExcel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            // tCell.Style.Add("vertical-align", "middle")

            for (int r = 0; r < ExlColCount; r++)
            {
                e.Row.Cells[r].Style.Add("vertical-align", "middle");
                e.Row.Cells[r].HorizontalAlign = HorizontalAlign.Center;
            }

            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
            if (ExlColCount > 3)
            {
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;
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

        string filename = "";
        string heading = "";

        if (rdbGL.Checked == true)
        {
            filename = "GLDefaultCustomersExcel.xls";

        }
        else
        {
            filename = "GLPLDefaultCustomersExcel.xls";
        }


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
        hw.Write("<tr><td colspan='" + rowcount + "'  style ='font-size :20 ; font-family :Cambria;  color :DarkBlue;font-weight:bold;'>Report : GL Default Customers Period :" + fromdate1 + " To " + todate1 + "</td></tr>");
        hw.Write("<tr><td colspan='" + rowcount + "' </td></tr></table>");

        if (rdbGL.Checked == true)
        {
            gvExcel.DataSource = ds.Tables[0];
            gvExcel.DataBind();

            //*********************// 17-4-2015
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
        else
        {
            gvPLExcel.DataSource = ds.Tables[0];
            gvPLExcel.DataBind();

            //*********************// 17-4-2015
            for (int i = 0; i < gvPLExcel.HeaderRow.Cells.Count; i++)
            {
                gvPLExcel.HeaderRow.Cells[i].Style.Add("background-color", "#FFFFCC");
                string h = gvPLExcel.HeaderRow.Cells[i].Text.ToString();

            }
            gvPLExcel.Style.Add("background-color", "#FFFFCC");
            gvPLExcel.BorderStyle = BorderStyle.Solid;
            gvPLExcel.RenderControl(hw);
            hw.Write("<table><tr><td colspan='" + ds.Tables[0].Columns.Count + "' </td></tr></table>");
            //Write the HTML back to the browser.     
            Response.ContentType = "application/vnd.ms-excel";
            Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");
            this.EnableViewState = false;
            Response.Write(tw.ToString());
            Response.End();
        }


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
        try
        {

            Clear();
            rdbGL.Checked = true;
            rdbGLPL.Checked = false;
            BindGLDefault();
            RDBSelection();

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLDefaultReport", "alert('" + ex.Message + "');", true);
        }

    }

    public void Clear()
    {
        txtPeriodDateFrom.Text = "";
        txtperiodtodate.Text = "";
        rdbGLno.Checked = true;
        rdbplcaseno.Checked = false;
        rdbName.Checked = false;

        for (int i = 0; i < grdgold.Rows.Count; i++)
        {
            grdgold.SelectedIndex = i;
            System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)grdgold.SelectedRow.FindControl("CheckBox1");
            chkBx1.Checked = false;

            System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)grdgold.HeaderRow.FindControl("checkAll");
            chk.Checked = false;
        }
    }
    protected void rdbGL_CheckedChanged(object sender, EventArgs e)
    {
        BindGLDefault();
        plcaseno.Visible = false;
        rdbplcaseno.Visible = false;
        Clear();



    }
    protected void rdbGLPL_CheckedChanged(object sender, EventArgs e)
    {
        BindGLDefault();
        plcaseno.Visible = true;
        rdbplcaseno.Visible = true;
        Clear();
    }
}