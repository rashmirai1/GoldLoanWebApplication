using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using CrystalDecisions.CrystalReports.Engine;

public partial class GLOutstandingLTVReport : System.Web.UI.Page
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
    string m_temp;
    decimal m_TwentyTwokarat, m_TwentyOnekarat, m_Twentykarat, m_Eighteenkarat, m_TwentyFourkarat, m_TwentyThreeCarat;
    string m_strOnlineRates;
    decimal m_balAmount = 0, m_FinalAmount = 0, m_LTVRates = 0, m_OutstndAmt = 0;
    string m_lastrec, m_today, m_todayDateTime;

    //Declaring Objects.     
    SqlConnection conn, connAIM, connFTPL;
    SqlDataAdapter daGetData, daIntCalc, da2;
    DataSet dsGetData, dsIntCalc, ds2;
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

    #region[Page_Load]
    protected void Page_Load(object sender, EventArgs e)
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

            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;

            txtIntDate.Text = DateTime.Now.ToShortDateString();

        }
    }
    #endregion[Page_Load]

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

                string abb = data.Substring(data.LastIndexOf("pad-15"), 14);
                string rate = abb.Substring(8, 6);

                m_temp = rate.Replace(",", "");

                //m_temp = data.Substring(data.LastIndexOf("10g"), 30);
                //m_temp = m_temp.Replace("<TD>", "");
                //m_temp = m_temp.Replace("</TD>", "");
                //m_temp = m_temp.Replace("\t", " ");
                //m_temp = m_temp.Replace("\n", " ");
                //m_temp = m_temp.Replace("10g", "");
                //m_temp = m_temp.Replace(" ", "");
                //m_temp = m_temp.Replace("Rs.", "");
                //m_temp = m_temp.Replace(",", "");

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

    #region [GetReportDetails]
    //Get ALL Customers Details
    public void GetReportDetails()
    {
        try
        {
            int BalanceLoanPayable = 0;
            int totaldays = 0;
            decimal interestdue = 0;
            decimal interestamt = 0;
            decimal outstandingPer = 0;
            decimal penalint = 0;
            decimal balAmount = 0, FinalAmount = 0, LTVRates = 0, OutstndAmt = 0;

            //string temp = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
            string temp = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            //temp = temp.Substring(0, temp.IndexOf("@"));
            rates(temp);

            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "GL_OutstandingLTVReport";
            cmd.CommandType = CommandType.StoredProcedure;
            daGetData = new SqlDataAdapter(cmd);

            cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(txtIntDate.Text));

            dsGetData = new DataSet();
            daGetData.Fill(dsGetData);

            if (dsGetData.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
                {
                    BalanceLoanPayable = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["BalanceLoanPayable"]);

                    cmd = new SqlCommand();
                    conn = new SqlConnection(strConnString);
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 0;

                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_InterestCalculation_RTR";

                    m_lastrec = dsGetData.Tables[0].Rows[i]["LastRecDate"].ToString();

                    m_todayDateTime = DateTime.Parse(m_lastrec).ToShortDateString();
                    //   m_today = System.DateTime.Now.ToShortDateString();
                    m_today = txtIntDate.Text;

                    if (Convert.ToDateTime(m_todayDateTime) > Convert.ToDateTime(m_today))
                    {
                        cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(m_today));
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(dsGetData.Tables[0].Rows[i]["LastRecDate"].ToString()));
                    }
                    cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(m_today));
                    cmd.Parameters.AddWithValue("@LoanAmount", dsGetData.Tables[0].Rows[i]["LoanAmountCalc"].ToString());
                    cmd.Parameters.AddWithValue("@OSInt", dsGetData.Tables[0].Rows[i]["CLI"].ToString());
                    cmd.Parameters.AddWithValue("@SID", dsGetData.Tables[0].Rows[i]["SID"].ToString());
                    cmd.Parameters.AddWithValue("@NeworOld", "0");
                    daIntCalc = new SqlDataAdapter(cmd);
                    dsIntCalc = new DataSet();
                    daIntCalc.Fill(dsIntCalc);

                    if (dsIntCalc.Tables[0].Rows.Count > 0)
                    {
                        for (int j = 0; j < dsIntCalc.Tables[0].Rows.Count; j++)
                        {
                            if (!(System.Convert.IsDBNull(dsIntCalc.Tables[0].Rows[j]["InterestAmount"])))
                            {
                                interestamt = Convert.ToDecimal(dsIntCalc.Tables[0].Rows[j]["InterestAmount"]);
                                interestdue = (interestdue + interestamt);
                            }
                            dsGetData.Tables[0].Rows[i]["ROI"] = dsIntCalc.Tables[0].Rows[j]["ROI"];
                        }
                        dsGetData.Tables[0].Rows[i]["Interest overdue as on date"] = Math.Round(interestdue);
                        // dsGetData.Tables[0].Rows[i]["Overdue Interest"] = Math.Round(interestdue);
                        interestdue = 0;
                    }
                    else
                    {
                        dsGetData.Tables[0].Rows[i]["Interest overdue as on date"] = "0";
                    }


                    dsGetData.Tables[0].Rows[i]["Days Lapsed"] = totaldays;
                    totaldays = 0;

                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "GL_Default_Penalint";
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da7 = new SqlDataAdapter(cmd);
                    cmd.Parameters.AddWithValue("@Duedate", gbl.ChangeDateMMddyyyy(dsGetData.Tables[0].Rows[i]["Loan Repayment Date"].ToString()));
                    cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(m_today));
                    cmd.Parameters.AddWithValue("@BalanceLoanPayable", BalanceLoanPayable);
                    cmd.Parameters.AddWithValue("@TotalDays", dsGetData.Tables[0].Rows[i]["Days Lapsed"]);
                    DataSet ds7 = new DataSet();
                    da7.Fill(ds7);
                    if (ds7.Tables[0].Rows.Count > 0)
                    {
                        penalint = Math.Round(Convert.ToDecimal(ds7.Tables[0].Rows[0]["Penal Interest"]));
                        dsGetData.Tables[0].Rows[i]["Penal Interest"] = Math.Round(penalint);
                    }
                    interestdue = 0;

                    if (dsGetData.Tables[0].Rows[i]["SDID"] != "")
                    {
                        conn = new SqlConnection(strConnString);
                        conn.Open();

                        m_strOnlineRates = "select gd.Purity,gd.NetWeight,SD.NetLoanAmtSanctioned,rd.BalanceLoanPayable,rd.RcvTotal,(rd.CLP+rd.CLI)TotalPI from TGLSanctionDisburse_GoldItemDetails gd left join TGLSanctionDisburse_BasicDetails SD ON SD.SDID=gd.SDID left join TGlReceipt_BasicDetails  rd on gd.SDID= SD.SDID and rd.RcptId=(select max(RcptId)FROM TGlReceipt_BasicDetails where sdid= '" + dsGetData.Tables[0].Rows[i]["SDID"] + "') WHERE GD.SDID=" + dsGetData.Tables[0].Rows[i]["SDID"];
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

                                OutstndAmt = Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["LoanAmountCalc"].ToString()) + Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["Interest overdue as on date"]);
                            }

                        }
                        outstandingPer = Math.Round(((Convert.ToDecimal(BalanceLoanPayable + penalint + Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["Interest overdue as on date"])) / Convert.ToDecimal(m_FinalAmount)) * 100), 2);

                        LTVRates = m_FinalAmount * (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["LTV"].ToString()) / 100); //FINAL LTV AMT

                        dsGetData.Tables[0].Rows[i]["Balance Eligibility"] = OutstndAmt;
                        //if (Math.Round(LTVRates - OutstndAmt) > 0)
                        //{
                        //    dsGetData.Tables[0].Rows[i]["Balance Eligibility"] = Math.Round(LTVRates - OutstndAmt);
                        //}
                        //else
                        //{
                        //    dsGetData.Tables[0].Rows[i]["Balance Eligibility"] = 0;
                        //}

                        if (outstandingPer != 0)
                        {
                            dsGetData.Tables[0].Rows[i]["O/S Percentage"] = outstandingPer;
                        }
                        m_FinalAmount = 0;
                    }
                    else
                    {
                        interestdue = 0;
                        interestamt = 0;
                        dsGetData.Tables[0].Rows[i]["Overdue Interest"] = 0;
                        //   dsGetData.Tables[0].Rows[i]["Interest currently charged @"] = 0;
                        dsGetData.Tables[0].Rows[i]["O/S Percentage"] = 0;
                        dsGetData.Tables[0].Rows[i]["Days Lapsed"] = 0;
                        dsGetData.Tables[0].Rows.RemoveAt(i);

                    }
                }
            }

            if (dsGetData.Tables[0].Rows.Count > 0)
            {
                ShowReport(dsGetData);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('No Records Found');", true);
            }

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [GetReportDetails]

    #region [View]
    // view btn click event
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GetReportDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [View]


    #region [ShowReport]
    // to Display Report 
    public void ShowReport(DataSet dsGetData)
    {
        int totalNoACless60 = 0, totalACBtwn6070 = 0, totalACBtwn7074 = 0, totalACBtwn7577 = 0, totalACBtwn7879 = 0, totalACBtwn8090 = 0, totalACGrtr90 = 0;
        int totalNoCustless60 = 0, totalCustBtwn6070 = 0, totalCustBtwn7074 = 0, totalCustBtwn7577 = 0, totalCustBtwn7879 = 0, totalCustBtwn8090 = 0, totalCustGrtr90 = 0;

        int totalOSless60 = 0, totalOSBtwn6070 = 0, totalOSBtwn7074 = 0, totalOSBtwn7577 = 0, totalOSBtwn7879 = 0, totalOSBtwn8090 = 0, totalOSGrtr90 = 0; //for increment variable
        int totalCustCount = 0;
        string custID = "";
        double totalBalEligless60 = 0, totalBalElgBtwn6070 = 0, totalBalElgBtwn7074 = 0, totalBalElgBtwn7577 = 0, totalBalElgBtwn7879 = 0, totalBalElgBtwn8090 = 0, totalBalElgGrtr90 = 0;//for final amount
        double totalBalnceless60 = 0, totBalElgBtwn6070 = 0, totBalElgBtwn7074 = 0, totBalElgBtwn7577 = 0, totBalElgBtwn7879 = 0, totBalElgBtwn8090 = 0, totBalElgGrtr90 = 0;

        dsGetData.Tables.Add("OSLTVDetail");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("SelDate");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalNoACless60");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalNoCustless60");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalEligless60");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustBtwn6070");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgBtwn6070");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACBtwn6070");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustBtwn7074");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgBtwn7074");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACBtwn7074");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustBtwn7577");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgBtwn7577");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACBtwn7577");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustBtwn7879");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgBtwn7879");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACBtwn7879");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustBtwn8090");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgBtwn8090");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACBtwn8090");

        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalCustGrtr90");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalBalElgGrtr90");
        dsGetData.Tables["OSLTVDetail"].Columns.Add("totalACGrtr90");

        DataRow dr = dsGetData.Tables["OSLTVDetail"].NewRow();
        dr["SelDate"] = "";
        dr["totalNoACless60"] = "";
        dr["totalNoCustless60"] = "";
        dr["totalBalEligless60"] = "";
        dr["totalACBtwn6070"] = "";
        dr["totalCustBtwn6070"] = "";
        dr["totalBalElgBtwn6070"] = "";
        dsGetData.Tables["OSLTVDetail"].Rows.Add(dr);

        string[] arr = new string[dsGetData.Tables[0].Rows.Count];

        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("60.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();

                    int result = arr.Count(s => s != null);
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    // totalCustBtwn6070 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totalOSless60 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totalBalnceless60 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }
                totalCustCount = arr.Distinct().Count(s => s != null);

                totalNoACless60 = totalNoACless60 + totalOSless60;
                totalNoCustless60 = totalCustCount;
                totalBalEligless60 = totalBalEligless60 + totalBalnceless60;
                totalOSless60 = 0; totalBalnceless60 = 0;

            }

            totalCustCount = 0;
        }

        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("60.50") && Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("69.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSBtwn6070 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgBtwn6070 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }

                totalACBtwn6070 = totalACBtwn6070 + totalOSBtwn6070;
                if (totalACBtwn6070 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustBtwn6070 = totalCustCount;
                totalBalElgBtwn6070 = totalBalElgBtwn6070 + totBalElgBtwn6070;
                totalOSBtwn6070 = 0; totBalElgBtwn6070 = 0;
            }
            totalCustCount = 0;
        }

        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("69.50") && Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("74.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSBtwn7074 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgBtwn7074 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }

                totalACBtwn7074 = totalACBtwn7074 + totalOSBtwn7074;
                if (totalACBtwn7074 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustBtwn7074 = totalCustCount;
                totalBalElgBtwn7074 = totalBalElgBtwn7074 + totBalElgBtwn7074;
                totalOSBtwn7074 = 0; totBalElgBtwn7074 = 0;
            }
            totalCustCount = 0;
        }

        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("74.50") && Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("77.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSBtwn7577 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgBtwn7577 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }
                totalACBtwn7577 = totalACBtwn7577 + totalOSBtwn7577;
                if (totalACBtwn7577 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustBtwn7577 = totalCustCount;
                totalBalElgBtwn7577 = totalBalElgBtwn7577 + totBalElgBtwn7577;
                totalOSBtwn7577 = 0; totBalElgBtwn7577 = 0;
            }
            totalCustCount = 0;
        }


        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("77.50") && Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("79.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSBtwn7879 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgBtwn7879 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }

                totalACBtwn7879 = totalACBtwn7879 + totalOSBtwn7879;
                if (totalACBtwn7879 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustBtwn7879 = totalCustCount;
                totalBalElgBtwn7879 = totalBalElgBtwn7879 + totBalElgBtwn7879;
                totalOSBtwn7879 = 0; totBalElgBtwn7879 = 0;
            }
            totalCustCount = 0;
        }


        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("79.50") && Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) <= decimal.Parse("90.49"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSBtwn8090 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgBtwn8090 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }

                totalACBtwn8090 = totalACBtwn8090 + totalOSBtwn8090;
                if (totalACBtwn8090 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustBtwn8090 = totalCustCount;
                totalBalElgBtwn8090 = totalBalElgBtwn8090 + totBalElgBtwn8090;
                totalOSBtwn8090 = 0; totBalElgBtwn8090 = 0;
            }
            totalCustCount = 0;
        }

        if (dsGetData.Tables[0].Rows.Count > 0)
        {
            Array.Clear(arr, 0, arr.Length);
            for (int i = 0; i < dsGetData.Tables[0].Rows.Count; i++)
            {
                if (Convert.ToDecimal(dsGetData.Tables[0].Rows[i]["O/S Percentage"].ToString()) >= decimal.Parse("90.50"))
                {
                    arr[i] = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    custID = dsGetData.Tables[0].Rows[i]["CustomerID"].ToString();
                    totalOSGrtr90 = Convert.ToInt32(dsGetData.Tables[0].Rows[i]["TotalCount"].ToString());
                    totBalElgGrtr90 = Convert.ToDouble(dsGetData.Tables[0].Rows[i]["Balance Eligibility"].ToString());
                }

                totalACGrtr90 = totalACGrtr90 + totalOSGrtr90;
                if (totalACGrtr90 != 0)
                {
                    totalCustCount = arr.Distinct().Count(s => s != null);
                }
                totalCustGrtr90 = totalCustCount;
                totalBalElgGrtr90 = totalBalElgGrtr90 + totBalElgGrtr90;
                totalOSGrtr90 = 0; totBalElgGrtr90 = 0;
            }
            totalCustCount = 0;
        }

        dsGetData.Tables["OSLTVDetail"].Rows[0]["SelDate"] = dsGetData.Tables[0].Rows[0]["SelDate"].ToString();
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalNoACless60"] = totalNoACless60;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalNoCustless60"] = totalNoCustless60;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalEligless60"] = totalBalEligless60;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACBtwn6070"] = totalACBtwn6070;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustBtwn6070"] = totalCustBtwn6070;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgBtwn6070"] = totalBalElgBtwn6070;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACBtwn7074"] = totalACBtwn7074;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustBtwn7074"] = totalCustBtwn7074;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgBtwn7074"] = totalBalElgBtwn7074;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACBtwn7577"] = totalACBtwn7577;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustBtwn7577"] = totalCustBtwn7577;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgBtwn7577"] = totalBalElgBtwn7577;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACBtwn7879"] = totalACBtwn7879;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustBtwn7879"] = totalCustBtwn7879;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgBtwn7879"] = totalBalElgBtwn7879;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACBtwn8090"] = totalACBtwn8090;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustBtwn8090"] = totalCustBtwn8090;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgBtwn8090"] = totalBalElgBtwn8090;

        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalACGrtr90"] = totalACGrtr90;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalCustGrtr90"] = totalCustGrtr90;
        dsGetData.Tables["OSLTVDetail"].Rows[0]["totalBalElgGrtr90"] = totalBalElgGrtr90;

        DataSet ds = null;
        ds = new DataSet("~/CryGLOutstangingLTVReport.rpt");

        ReportDocument rpt = new ReportDocument();
        rpt.Load(Server.MapPath(ds.DataSetName));

        rpt.SetDataSource(dsGetData.Tables["OSLTVDetail"]);
        Session["REPORT"] = rpt;
        ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('CryGLOutstandingLTVReport.aspx');", true);

    }
    #endregion [ShowReport]

    #region [PropertybtnCancel_Click]
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        // txtIntDate.Text = "";
        try
        {
            Response.Redirect("GLOutstandingLTVReport.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnCancel_Click]

}