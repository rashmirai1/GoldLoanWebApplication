using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.Text;
using System.Xml;

public partial class GLOutstanding : System.Web.UI.Page
{
    #region Declarations
    public string loginDate;
    public string expressDate;

    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    GlobalSettings gbl = new GlobalSettings();
    bool m_datasaved = false;

    //Declaring Variables.
    int m_result = 0;
    int m_ExlColCount = 0;
    string m_lastrec, m_today, m_todayDateTime;

    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter daFillData;
    DataSet dsFillData;
    DataTable dtFillData;

    SqlCommand cmd;
    string m_temp;
    decimal m_TwentyTwokarat, m_TwentyOnekarat, m_Twentykarat, m_Eighteenkarat, m_TwentyFourkarat, m_TwentyThreeCarat;
    string m_strOnlineRates;
    decimal m_balAmount = 0, m_FinalAmount = 0, m_LTVRates = 0, m_OutstndAmt = 0;


    string LastDate, LastCLI, LastLoanAmount = string.Empty;
    string InterestFromDate = string.Empty;
    string InterestToDate = string.Empty;
    string RvcCLI = string.Empty;
    string AdvInterestFromDate = string.Empty;
    string AdvInterestToDate = string.Empty;
    int neworold = 0;
    #endregion [Declarations]

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Master.PropertyPanelStrip.Visible = false;
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }
            else
            {
                hdnUserID.Value = Session["userID"].ToString();
                hdnFYearID.Value = Session["FYearID"].ToString();
                hdnBranchID.Value = Session["branchId"].ToString();
            }

            // Get user login time or last activity time.
            DateTime date = DateTime.Now;
            loginDate = date.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
            int sessionTimeout = Session.Timeout;
            DateTime dateExpress = date.AddMinutes(sessionTimeout);
            expressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
            Get_DefOSlevel();
            GetALLSDDetails();
        }
    }

    #region[OnlineRates]
    public string rates(string urlAddress)
    {

        try
        {
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

                //m_temp = data.Substring(data.LastIndexOf("10g"), 30);
                //m_temp = m_temp.Replace("<TD>", "");
                //m_temp = m_temp.Replace("</TD>", "");
                //m_temp = m_temp.Replace("\t", " ");
                //m_temp = m_temp.Replace("\n", " ");
                //m_temp = m_temp.Replace("10g", "");
                //m_temp = m_temp.Replace(" ", "");
                //m_temp = m_temp.Replace("Rs.", "");
                //m_temp = m_temp.Replace(",", "");
                m_temp = "49200";
                m_TwentyFourkarat = Convert.ToDecimal(m_temp);
                m_TwentyThreeCarat = Convert.ToDecimal(m_temp) * Convert.ToDecimal(0.958);
                m_TwentyTwokarat = Convert.ToDecimal(m_temp) * Convert.ToDecimal(0.916);
                m_TwentyOnekarat = Convert.ToDecimal(m_temp) * Convert.ToDecimal(0.875);
                m_Twentykarat = Convert.ToDecimal(m_temp) * Convert.ToDecimal(0.830);
                m_Eighteenkarat = Convert.ToDecimal(m_temp) * Convert.ToDecimal(0.75);

            }
        }
        catch (Exception)
        {
            //MessageBox.Show("Cannot connect to the server."); 
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cannot connect to the server.');", true);
        }
        return m_temp;
    }
    #endregion [OnlineRates]

    #region [Get_DefOSlevel]
    public void Get_DefOSlevel()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SelectDefaultOSLevel";

            cmd.Parameters.AddWithValue("@UserID", hdnUserID.Value);
            daFillData = new SqlDataAdapter(cmd);
            dtFillData = new DataTable();
            daFillData.Fill(dtFillData);

            if (dtFillData.Rows.Count > 0)
            {
                // ddlOSlevel.DataSource = dt;
                hdnDefaultOSLevel.Value = (dtFillData.Rows[0]["DefOSLevel"].ToString());
            }

        }
        catch (Exception EX)
        {


        }
    }
    #endregion[Get_DefOSlevel]

    #region[GetALLSDDetails]
    public void GetALLSDDetails()
    {
        try
        {
            int BalanceLoanPayable = 0;
            int totaldays = 0;
            decimal interestdue = 0;
            decimal interestamt = 0;
            decimal outstandingPer = 0;
            decimal penalint = 0;
            string GoldLoanNo = "";
            string m_temp = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
            m_temp = m_temp.Substring(0, m_temp.IndexOf("@"));
            rates(m_temp);

            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "GL_GetAllDetailsForOSPercentage_New";
            cmd.CommandType = CommandType.StoredProcedure;
            daFillData = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
            cmd.Parameters.AddWithValue("@BranchID", hdnBranchID.Value);
            dsFillData = new DataSet();
            daFillData.Fill(dsFillData);

            if (dsFillData.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsFillData.Tables[0].Rows.Count; i++)
                {
                    GoldLoanNo = dsFillData.Tables[0].Rows[i]["Gold Loan No"].ToString();

                    //Added by Priya on 6-10-2015
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
                    if (Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                    {
                        cmdRoiRow = new SqlCommand();
                        cmdRoiRow.Connection = conn;
                        cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dsFillData.Tables[0].Rows[i]["SID"].ToString();
                        if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                        {
                            RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                        }
                    }
                    else if (Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) > 0)
                    {
                        cmdRoiRow = new SqlCommand();
                        cmdRoiRow.Connection = conn;
                        cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dsFillData.Tables[0].Rows[i]["SID"].ToString();
                        if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                        {
                            RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                        }
                    }
                    else if (Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                    {
                        cmdRoiRow = new SqlCommand();
                        cmdRoiRow.Connection = conn;
                        cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                        if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                        {
                            RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                        }
                    }


                    if (dsFillData.Tables[0].Rows[i]["InterestFromDate"].ToString() != "" && dsFillData.Tables[0].Rows[i]["InterestFromDate"].ToString() != null)
                    {
                        InterestFromDate = dsFillData.Tables[0].Rows[i]["InterestFromDate"].ToString();
                    }
                    else
                    {
                        InterestFromDate = System.DateTime.Today.ToShortDateString();
                    }

                    if (dsFillData.Tables[0].Rows[i]["InterestToDate"].ToString() != "" && dsFillData.Tables[0].Rows[i]["InterestToDate"].ToString() != null)
                    {
                        InterestToDate = dsFillData.Tables[0].Rows[i]["InterestToDate"].ToString();
                    }
                    else
                    {
                        InterestToDate = System.DateTime.Today.ToShortDateString();
                    }


                    if (dsFillData.Tables[0].Rows[i]["RecvInterest"].ToString() != "" && dsFillData.Tables[0].Rows[i]["RecvInterest"].ToString() != null)
                    {
                        RvcCLI = dsFillData.Tables[0].Rows[i]["RecvInterest"].ToString();
                    }
                    else
                    {
                        RvcCLI = "0";
                    }


                    if (dsFillData.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != "" && dsFillData.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != null)
                    {
                        AdvInterestFromDate = dsFillData.Tables[0].Rows[i]["AdvInterestFromDate"].ToString();
                    }
                    else
                    {
                        AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
                    }

                    if (dsFillData.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != "" && dsFillData.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != null)
                    {
                        AdvInterestToDate = dsFillData.Tables[0].Rows[i]["AdvInterestToDate"].ToString();
                    }
                    else
                    {
                        AdvInterestToDate = System.DateTime.Today.ToShortDateString();
                    }
                    //**************





                    BalanceLoanPayable = Convert.ToInt32(dsFillData.Tables[0].Rows[i]["BalanceLoanPayable"]);

                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 0;
                    SqlDataAdapter da1 = new SqlDataAdapter(cmd);


                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_EmiCalculator_RTR";

                    m_lastrec = dsFillData.Tables[0].Rows[i]["LastRecDate"].ToString();

                    m_todayDateTime = DateTime.Parse(m_lastrec).ToShortDateString();
                    m_today = System.DateTime.Now.ToShortDateString();

                    cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dsFillData.Tables[0].Rows[0]["Loan Date"].ToString()));

                    if (Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString()) >= 0)
                    {
                        double AddPrvInt = Math.Round(Convert.ToDouble(dsFillData.Tables[0].Rows[i]["LoanAmountCalc"].ToString()) + Convert.ToDouble(dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString()));
                        cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@LoanAmount", (dsFillData.Tables[0].Rows[i]["LoanAmountCalc"].ToString()));
                    }

                    cmd.Parameters.AddWithValue("@SID", dsFillData.Tables[0].Rows[i]["SID"].ToString());
                    cmd.Parameters.AddWithValue("@NeworOld", neworold);

                    cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                    cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                    cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

                    cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                    cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                    cmd.Parameters.AddWithValue("@OSIntAmt", dsFillData.Tables[0].Rows[i]["OSIntAmt"].ToString());

                    cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                    cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                    cmd.Parameters.AddWithValue("@AdvInterestAmt", dsFillData.Tables[0].Rows[i]["AdvInterestAmount"].ToString());

                    if (Convert.ToDateTime(m_todayDateTime) > Convert.ToDateTime(m_today))
                    {
                        cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(m_today));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(dsFillData.Tables[0].Rows[i]["LastRecDate"].ToString()));
                    }
                    cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(m_today));
                    cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

                    DataSet ds1 = new DataSet();
                    da1.Fill(ds1);

                    if (ds1.Tables[0].Rows.Count > 0)
                    {
                        for (int m = 0; m < ds1.Tables[0].Rows.Count; m++)
                        {
                            totaldays = totaldays + Convert.ToInt32(ds1.Tables[0].Rows[m]["TotalDays"]);

                            if (!(System.Convert.IsDBNull(ds1.Tables[0].Rows[m]["InterestAmount"])))
                            {

                                interestamt = Convert.ToDecimal(ds1.Tables[0].Rows[m]["InterestAmount"]);
                                interestdue = (interestdue + interestamt);
                            }

                            dsFillData.Tables[0].Rows[i]["Interest currently charged @"] = ds1.Tables[0].Rows[m]["ROI"].ToString();
                        }
                        dsFillData.Tables[0].Rows[i]["Overdue Interest"] = Math.Round(interestdue);
                        dsFillData.Tables[0].Rows[i]["Days Lapsed"] = totaldays;
                        totaldays = 0;

                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.CommandTimeout = 0;
                        cmd.CommandText = "GL_Default_Penalint";
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataAdapter da7 = new SqlDataAdapter(cmd);
                        cmd.Parameters.AddWithValue("@Duedate", gbl.ChangeDateMMddyyyy(dsFillData.Tables[0].Rows[i]["Loan Repayment Date"].ToString()));
                        cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(m_today));
                        cmd.Parameters.AddWithValue("@BalanceLoanPayable", BalanceLoanPayable);
                        cmd.Parameters.AddWithValue("@TotalDays", dsFillData.Tables[0].Rows[i]["Days Lapsed"]);
                        DataSet ds7 = new DataSet();
                        da7.Fill(ds7);
                        if (ds7.Tables[0].Rows.Count > 0)
                        {
                            penalint = Math.Round(Convert.ToDecimal(ds7.Tables[0].Rows[0]["Penal Interest"]));

                            dsFillData.Tables[0].Rows[i]["Penal Interest"] = Math.Round(penalint);

                        }
                        interestdue = 0;
                        if (dsFillData.Tables[0].Rows[i]["SDID"] != "")
                        {
                            conn = new SqlConnection(strConnString);
                            conn.Open();

                            m_strOnlineRates = "select gd.Purity,gd.NetWeight,SD.NetLoanAmtSanctioned,rd.BalanceLoanPayable,rd.RcvTotal,(rd.CLP+rd.CLI)TotalPI from TGLSanctionDisburse_GoldItemDetails gd left join TGLSanctionDisburse_BasicDetails SD ON SD.SDID=gd.SDID left join TGlReceipt_BasicDetails  rd on gd.SDID= SD.SDID and rd.RcptId=(select max(RcptId)FROM TGlReceipt_BasicDetails where sdid= '" + dsFillData.Tables[0].Rows[i]["SDID"] + "') WHERE GD.SDID=" + dsFillData.Tables[0].Rows[i]["SDID"];
                            SqlDataAdapter daRates = new SqlDataAdapter(m_strOnlineRates, conn);
                            DataSet dsRates = new DataSet();
                            daRates.Fill(dsRates);

                            if (dsRates.Tables[0].Rows.Count > 0)
                            {
                                for (int r = 0; r < dsRates.Tables[0].Rows.Count; r++)
                                {

                                    if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "18K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_Eighteenkarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "20K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_Twentykarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "21K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_TwentyOnekarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "22K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_TwentyTwokarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "23K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_TwentyThreeCarat / 10);
                                    }
                                    else if (dsRates.Tables[0].Rows[r]["Purity"].ToString() == "24K")
                                    {
                                        m_balAmount = Convert.ToDecimal(dsRates.Tables[0].Rows[r]["NetWeight"].ToString()) * (m_TwentyFourkarat / 10);
                                    }
                                    m_FinalAmount = m_FinalAmount + m_balAmount;
                                }

                            }
                            outstandingPer = Math.Round(((Convert.ToDecimal(BalanceLoanPayable + penalint + Convert.ToDecimal(dsFillData.Tables[0].Rows[i]["Overdue Interest"])) / Convert.ToDecimal(m_FinalAmount)) * 100), 2);
                            if (outstandingPer != 0)
                            {
                                dsFillData.Tables[0].Rows[i]["O/S Percentage"] = outstandingPer;
                            }

                        }

                        penalint = 0; outstandingPer = 0; m_FinalAmount = 0; m_balAmount = 0;

                        //added by priya for OS % > DefLevel %
                        if (Convert.ToDouble(dsFillData.Tables[0].Rows[i]["O/S Percentage"]) <= Convert.ToDouble(hdnDefaultOSLevel.Value))
                        {
                            dsFillData.Tables[0].Rows.RemoveAt(i);
                            i--;
                        }
                    }
                    else
                    {
                        interestdue = 0;
                        interestamt = 0;
                        dsFillData.Tables[0].Rows[i]["Overdue Interest"] = 0;
                        dsFillData.Tables[0].Rows[i]["Interest currently charged @"] = 0;
                        dsFillData.Tables[0].Rows[i]["O/S Percentage"] = 0;
                        dsFillData.Tables[0].Rows[i]["Days Lapsed"] = 0;
                        dsFillData.Tables[0].Rows.RemoveAt(i);
                        i--;
                    }
                }

                if (dsFillData.Tables[0].Rows.Count > 0)
                {

                    dsFillData.Tables[0].Columns.Remove("LoanAmountCalc");
                    dsFillData.Tables[0].Columns.Remove("CLI");
                    dsFillData.Tables[0].Columns.Remove("LastRecDate");
                    dsFillData.Tables[0].Columns.Remove("SID");
                    dsFillData.Tables[0].Columns.Remove("SDID");
                    dsFillData.Tables[0].Columns.Remove("Received Date");
                    dsFillData.Tables[0].Columns.Remove("BalanceLoanPayable");
                    dsFillData.Tables[0].Columns.Remove("Value of gold");
                    dsFillData.Tables[0].Columns.Remove("Days Lapsed");
                    dsFillData.Tables[0].Columns.Remove("Interest currently charged @");
                    dsFillData.Tables[0].Columns.Remove("Penal Interest");
                    dsFillData.Tables[0].Columns.Remove("Overdue Interest");
                    dsFillData.Tables[0].Columns.Remove("Loan Repayment Date");
                    dsFillData.Tables[0].Columns.Remove("Loan Amount");

                    dsFillData.Tables[0].Columns.Remove("InterestFromDate");
                    dsFillData.Tables[0].Columns.Remove("InterestToDate");
                    dsFillData.Tables[0].Columns.Remove("RecvInterest");
                    dsFillData.Tables[0].Columns.Remove("OSIntAmt");
                    dsFillData.Tables[0].Columns.Remove("AdvInterestFromDate");
                    dsFillData.Tables[0].Columns.Remove("AdvInterestToDate");
                    dsFillData.Tables[0].Columns.Remove("AdvInterestAmount");

                    ShowLoginPopup(dsFillData);
                }
            }
            else
            {
                interestdue = 0;
                interestamt = 0;
            }
        }
        catch (Exception EX)
        {

        }
    }
    #endregion [GetALLSDDetails]

    #region[ShowLoginPopup]
    public void ShowLoginPopup(DataSet ds)
    {
        Master.PropertyddlSearch.Items.Add("Name");
        Master.PropertyddlSearch.Items.Add("Gold Loan No");
        Master.PropertyddlSearch.Items.Add("Loan Date");
        Master.PropertyddlSearch.Items.Add("O/S Percentage");

        //DataTable dtSMS = ds.Tables[0];
        // dtSMS.Columns.Add("Message", typeof(TextBox));
        //  dtSMS.Rows.Add("Message", typeof(TextBox));

        Master.PropertygvGlobal.DataSource = ds;

        Session["PopupData"] = ds;
        Master.DataBind();
        Master.PropertylblHeader.Text = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Customers With Outstanding % greater than Default Level (" + hdnDefaultOSLevel.Value + ") %";
        Master.PropertympeGlobal.Show();
    }
    #endregion [ShowLoginPopup]

    #region[PropertybtnSearch_Click]
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string strQuery;
            DataSet dsFilter = (DataSet)Session["PopupData"];
            DataTable dtFilter = dsFilter.Tables[0];
            DataView dvFilter = new DataView(dtFilter);
            if (Master.PropertytxtSearch.Text.Trim() != string.Empty)
            {
                strQuery = string.Format("{0} '{1}%'", Master.PropertygvGlobal.SortExpression, Master.PropertytxtSearch.Text.Trim());
                dvFilter.RowFilter = Convert.ToString("[" + Master.PropertyddlSearch.Text.Trim() + "]") + " like" + strQuery;
            }
            Master.PropertygvGlobal.DataSource = dvFilter;
            Master.DataBind();
            Master.PropertympeGlobal.Show();
        }

        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[PropertybtnSearch_Click]

    #region[PropertygvGlobal_RowCommand]
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[PropertygvGlobal_RowCommand]

}