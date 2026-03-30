using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Script.Services;
using System.Web.Services;

public partial class GLMonthlyInterestJV : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    //creating instance of class "CompanyWiseAccountClosing"
    GlobalSettings gbl = new GlobalSettings();
    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

    //Declaring Variables.   
    string strQuery = string.Empty;
    string insertQuery = string.Empty;

    string KYCID = string.Empty;
    string SID = string.Empty;
    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = string.Empty;
    string GoldLoanNo = string.Empty;
    string BalanceLoanPayable = string.Empty;
    string CurrentOutstanding = string.Empty;
    string LastReceivedDate = string.Empty;
    string TotalDays = string.Empty;
    string UserEnteredDate = string.Empty;

    bool datasaved = false;
    public string loginDate;
    public string expressDate;
    int result = 0;
    int excount = 0;

    //Declaring Objects.   
    SqlTransaction transactionGL, transactionAIM;
    SqlConnection conn, connAIM;
    SqlDataAdapter da, da1, da2;
    DataSet ds, ds1, ds2;
    SqlCommand cmd, cmd1, cmd2, cmdRcpt, cmdRoiRow;
    DataTable dt;

    string InterestFromDate = string.Empty;
    string InterestToDate = string.Empty;
    string RvcCLI = string.Empty;
    string AdvInterestFromDate = string.Empty;
    string AdvInterestToDate = string.Empty;

    #endregion [Declarations]

    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);

    }
    #endregion [Page_Init]

    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnView.Visible = false;
        Master.PropertybtnSave.Visible = false;
    }

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
                HdnUserID.Value = Session["userID"].ToString();
                hdnfyid.Value = Session["FYearID"].ToString();
                hdnbranchid.Value = Session["branchId"].ToString();
            }
            Master.PropertybtnCancel.Visible = true;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnView.Visible = false;
            Master.PropertybtnSave.Visible = false;
            txtIntDate.Text = DateTime.Now.ToShortDateString();

            dt = new DataTable();
         
            GVIntJV.DataSource = dt;
            GVIntJV.DataBind();
          
        }

    }


    protected void btn_Search_Click(object sender, EventArgs e)
    {
        try
        {
            Calculate_MonthlyRTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void ProcessBtn_Click(object sender, EventArgs e)
    {
        try
        {
            //Save_MonthlyRTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);

        }
    }

    public void Calculate_MonthlyRTR()
    {
        //Customer Loan Details                                 
        string LoanDate;
        string LoanAmount;
        string SID;
        string NeworOld;

        //Last Receipt Details        
        string FromDate;
        string ToDate;
        string PaidInt;

        string OSInterestFromDate;
        string OSInterestToDate;
        decimal OSIntAmt;

        string AdvInterestFromDate;
        string AdvInterestToDate;
        decimal AdvInterestAmt;

        //Date up to which we have to calculate Interest Amount.      
        string CalculateFromDate;
        string CalculateToDate;
        int LastROIID = 0;
        string LastReceiptID;

        string JVFrom;
        string JVTo;

        GVIntJV.DataSource = null;

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.StoredProcedure;
        //cmd.CommandText = "GL_MonthlyJVRTR_New1";
        //cmd.CommandText = "GL_MonthlyJVRTR_New";
        cmd.CommandText = "GL_MonthlyJVRTR_updated";
        cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(txtIntDate.Text));
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        dt.Columns.Add("Interest Amount");
        dt.Columns.Add("BalanceLoanAmount");

        DataTable dt2 = new DataTable();
        dt.Columns.Add("AccountID");

        for (int s = 0; s < dt.Rows.Count; s++)
        {
            string GoldNo = "";
            string accNo = "";
            GoldNo = dt.Rows[s]["GoldLoanNo"].ToString();
            // FOR Customer Account
            conn = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandText = "select AccountID,Name From tblAccountmaster where Alies=" + "'" + GoldNo + "'";
            da = new SqlDataAdapter(cmd);
            da.Fill(dt2);
            for (int r = 0; r < dt2.Rows.Count; r++)
            {
                accNo = dt2.Rows[r]["AccountID"].ToString();
            }
            // DataRow dr = dt.NewRow();
            dt.Rows[s]["AccountID"] = accNo;
        }

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            //Customer Loan Details                                 
            LoanDate = dt.Rows[i]["LoanDate"].ToString();
            LoanAmount = dt.Rows[i]["LoanCalcAmount"].ToString();

            connAIM = new SqlConnection(strConnStringAIM);
            connAIM.Open();
            string balanceloanamount = string.Empty;
            strQuery = "select isnull(sum(Debit)-sum(Credit),(select sum(Debit)-sum(Credit) From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"].ToString() + "' and " +
                         "RefDate<'" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "' and RefDate>=DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')-1),'" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')))" +
                         "From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"].ToString() + "' and RefDate<DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')-1),'" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')";
            //strQuery = "select isnull(sum(Debit)-sum(Credit),0)  From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"] + "' and RefDate<DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')-1),'" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')";
            cmd = new SqlCommand(strQuery, connAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                balanceloanamount = Convert.ToString(cmd.ExecuteScalar());
            }
            else
            {
                balanceloanamount = "0";
            }

            SID = dt.Rows[i]["sid"].ToString();

            if (dt.Rows[i]["RCPTID"].ToString() == "0")
            {
                NeworOld = "0";
            }
            else
            {
                NeworOld = "1";
            }

            string InterestFromDate = string.Empty;
            if (dt.Rows[i]["InterestFromDate"].ToString() != "" && dt.Rows[i]["InterestFromDate"].ToString() != null)
            {
                InterestFromDate = dt.Rows[i]["InterestFromDate"].ToString();
                //InterestFromDate = "28/08/2015";
            }
            else
            {
                InterestFromDate = System.DateTime.Today.ToShortDateString();
            }
            string InterestToDate = string.Empty;
            if (dt.Rows[i]["InterestToDate"].ToString() != "" && dt.Rows[i]["InterestToDate"].ToString() != null)
            {
                InterestToDate = dt.Rows[i]["InterestToDate"].ToString();
                //InterestToDate = txtIntDate.Text;
            }
            else
            {
                InterestToDate = System.DateTime.Today.ToShortDateString();
            }

            string RvcCLI = string.Empty;
            if (dt.Rows[i]["RecvInterest"].ToString() != "" && dt.Rows[i]["RecvInterest"].ToString() != null)
            {
                RvcCLI = dt.Rows[i]["RecvInterest"].ToString();
            }
            else
            {
                RvcCLI = "0";
            }

            string AdvInterestFrom = string.Empty;
            if (dt.Rows[i]["AdvInterestFromDate"].ToString() != "" && dt.Rows[i]["AdvInterestFromDate"].ToString() != null)
            {
                AdvInterestFrom = dt.Rows[i]["AdvInterestFromDate"].ToString();
            }
            else
            {
                AdvInterestFrom = System.DateTime.Today.ToShortDateString();
            }

            string AdvInterestTo = string.Empty;
            if (dt.Rows[i]["AdvInterestToDate"].ToString() != "" && dt.Rows[i]["AdvInterestToDate"].ToString() != null)
            {
                AdvInterestTo = dt.Rows[i]["AdvInterestToDate"].ToString();
            }
            else
            {
                AdvInterestTo = System.DateTime.Today.ToShortDateString();
            }


            //Last Receipt Details        
            FromDate = InterestFromDate;
            ToDate = InterestToDate;
            PaidInt = RvcCLI;

            OSInterestFromDate = InterestFromDate;
            OSInterestToDate = InterestToDate;
            if ((dt.Rows[i]["OSIntAmt"] != null) || (dt.Rows[i]["OSIntAmt"].ToString()!=string.Empty))
            {
                OSIntAmt = Convert.ToDecimal(dt.Rows[i]["OSIntAmt"].ToString());
            }
            else
            { 
                OSIntAmt=0;            
            }

            AdvInterestFromDate = AdvInterestFrom;
            AdvInterestToDate = AdvInterestTo;
            AdvInterestAmt = Convert.ToDecimal(dt.Rows[i]["AdvInterestAmount"].ToString());

            //Date up to which we have to calculate Interest Amount.      
            CalculateFromDate = dt.Rows[i]["LastRecDate"].ToString();
            CalculateToDate = gbl.ChangeDateMMddyyyy(txtIntDate.Text);

            LastReceiptID = dt.Rows[i]["RCPTID"].ToString();

            JVFrom = Convert.ToString(Convert.ToDateTime(txtIntDate.Text.Trim()).Month.ToString() + "/1/" + Convert.ToDateTime(txtIntDate.Text.Trim()).Year.ToString());
           // JVFrom = CalculateFromDate;
            JVTo = gbl.ChangeDateMMddyyyy(txtIntDate.Text);

            conn = new SqlConnection(strConnString);
            cmdRoiRow = new SqlCommand();
            cmdRoiRow.Connection = conn;
            conn.Open();

            if (!LastReceiptID.Contains("0"))
            {
                cmdRoiRow.CommandText = "select top 1 ROIRowID from TGLInterest_Details where ReceiptID='" + LastReceiptID + "' and '" + gbl.ChangeDateMMddyyyy(dt.Rows[i]["AcLastRecDate"].ToString()) + "' between FromDate and ToDate order by InterestDetID desc";
            }
            else
            {
                cmdRoiRow.CommandText = "select  top 1 ROIID From TSchemeMaster_EffectiveROI where SID='" + dt.Rows[i]["SID"].ToString() + "' order by ROIID asc";
            }

            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
            {
                LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());

            }

            if (LastROIID == 0)
            {
                conn = new SqlConnection(strConnString);
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                conn.Open();
                cmdRoiRow.CommandText = "select  top 1 ROIID From TSchemeMaster_EffectiveROI where SID='" + dt.Rows[i]["SID"].ToString() + "' order by ROIID asc";

                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());

                }
            }
         
            
            //if (LastReceiptID.Trim() != "" && OSIntAmt == 0 && AdvInterestAmt == 0)
            //{
            //    cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[i]["SID"].ToString();
            //    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
            //    {
            //        LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
            //    }
            //}
            //if (LastReceiptID.Trim() != "" && OSIntAmt == 0 && AdvInterestAmt > 0)
            //{
            //    //cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
            //    cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[i]["SID"].ToString();
            //    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
            //    {
            //        LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
            //    }
            //}
            //if (LastReceiptID.Trim() != "" && OSIntAmt > 0 && AdvInterestAmt == 0)
            //{
            //    cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + LastReceiptID;
            //    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
            //    {
            //        LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
            //    }
            //}


            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_EmiCalculator_RTR_ForMonthly_JV";
            //cmd.CommandText = "GL_EmiCalculator_RTR";

            cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[i]["LoanDate"].ToString()));  
        
            //if (Convert.ToDecimal(dt.Rows[i]["OSIntAmt"].ToString()) >= 0)
            //{
            //    double AddPrvInt = Math.Round(Convert.ToDouble(dt.Rows[i]["LoanCalcAmount"].ToString()) + Convert.ToDouble(dt.Rows[i]["OSIntAmt"].ToString()));
            //    cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
            //}
            //else
            //{
            //    cmd.Parameters.AddWithValue("@LoanAmount", (dt.Rows[i]["LoanCalcAmount"].ToString()));
            //}

            cmd.Parameters.AddWithValue("@LoanAmount", balanceloanamount);
            cmd.Parameters.AddWithValue("@SID", dt.Rows[i]["SID"].ToString());
            cmd.Parameters.AddWithValue("@NeworOld", NeworOld);
            cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);
            cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[i]["OSIntAmt"].ToString());
            cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
            cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
            cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[i]["AdvInterestAmount"].ToString());
            cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(CalculateFromDate));
            //cmd.Parameters.AddWithValue("@CalculateFromDate", (JVFrom));
            cmd.Parameters.AddWithValue("@CalculateToDate", (CalculateToDate));
           //cmd.Parameters.AddWithValue("@CalculateToDate", (JVTo));
            cmd.Parameters.AddWithValue("@LastROIID", LastROIID);
            cmd.Parameters.AddWithValue("@JVFrom", (JVFrom));
            cmd.Parameters.AddWithValue("@JVTo", (JVTo));

           string sss = Convert.ToString(cmd.ExecuteScalar());
           dt.Rows[i]["Interest Amount"] = Convert.ToInt32(cmd.ExecuteScalar());
            //da = new SqlDataAdapter(cmd);
            //DataTable dt1 = new DataTable();
           // da.Fill(dt1);

            if (Convert.ToDouble(dt.Rows[i]["Interest Amount"]) <= 0)
            {
                dt.Rows.RemoveAt(i);
                i--;
            }
        }
        
        GVIntJV.DataSource = dt;
        GVIntJV.AutoGenerateColumns = false;
        GVIntJV.Visible = true;
        GVIntJV.DataBind();

        if (GVIntJV.Rows.Count > 0)
        {
            ProcessBtn.Visible = true;
        }
    }

    

    #region [CreateNormalLedgerEntries]
    public static int CreateNormalLedgerEntries(string fyid,string Reftype, string ReferenceNo, DateTime RefDate, int AccID, double DebitAmount, double CreditAmount, int ContraAccID, string Narration, SqlTransaction tran,SqlConnection con)
    {
        int LedgerID = 0;
        bool datasaved = false;

        //try
        //{
          string  strQuery = "SELECT MAX(LedgerID) FROM FLedgerMaster";

          SqlCommand cmd = new SqlCommand(strQuery, con, tran);
            cmd.CommandType = CommandType.Text;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                LedgerID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                LedgerID = 0;
            }

            LedgerID += 1;

            string Date = Convert.ToDateTime(RefDate).ToString("yyyy/MM/dd");

          string  insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + ReferenceNo + "', '" + Reftype + "'," +
                                    "'" + Convert.ToDateTime(RefDate).ToString("yyyy/MM/dd") + "', " +
                                    "" + AccID + ", " + DebitAmount + ", " + CreditAmount + ", '" + Narration + "', " +
                                    "" + ContraAccID + ", '', " + fyid + ") ";

            cmd = new SqlCommand(insertQuery, con, tran);
            cmd.CommandType = CommandType.Text;
            int QueryResult = cmd.ExecuteNonQuery();

            if (QueryResult > 0)
            {
                datasaved = true;
            }
            else
            {
                datasaved = false;
            }
        //}
        //catch (Exception ex)
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "LedgerEntryAlert", "alert('" + ex.Message + "');", true);
        //}
        return LedgerID;
    }
    #endregion [CreateNormalLedgerEntries]

    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {

           
            foreach (GridViewRow row in GVIntJV.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkRow = (row.Cells[0].FindControl("CheckBox1") as CheckBox);
                    string goldLoanNo = (row.Cells[1].FindControl("lblgoldLoanNo") as Label).Text;
                }
            }
            //Response.Redirect("GLMonthlyInterestJV.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static IList<Monthlyjv> CalculateJvInterest(string jvdate)
    {       
            //Customer Loan Details                                 
            string LoanDate;
            string LoanAmount;
            string SID;
            string NeworOld;

            //Last Receipt Details        
            string FromDate;
            string ToDate;
            string PaidInt;

            string OSInterestFromDate;
            string OSInterestToDate;
            decimal OSIntAmt;

            string AdvInterestFromDate;
            string AdvInterestToDate;
            decimal AdvInterestAmt;

            //Date up to which we have to calculate Interest Amount.      
            string CalculateFromDate;
            string CalculateToDate;
            int LastROIID = 0;
            string LastReceiptID;

            string JVFrom;
            string JVTo;

            string strQuery = string.Empty;

            string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
            string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

            GlobalSettings gbl = new GlobalSettings();

            SqlConnection conn = new SqlConnection(strConnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "GL_MonthlyJVRTR_New1";
            //cmd.CommandText = "GL_MonthlyJVRTR_New";
            cmd.CommandText = "GL_MonthlyJVRTR_updated";
            cmd.Parameters.AddWithValue("@SelDate", gbl.ChangeDateMMddyyyy(jvdate));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            dt.Columns.Add("Interest Amount");
            dt.Columns.Add("BalanceLoanAmount");

            DataTable dt2 = new DataTable();
            dt.Columns.Add("AccountID");

            for (int s = 0; s < dt.Rows.Count; s++)
            {
                string GoldNo = "";
                string accNo = "";
                GoldNo = dt.Rows[s]["GoldLoanNo"].ToString();
                // FOR Customer Account
                conn = new SqlConnection(strConnStringAIM);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandText = "select AccountID,Name From tblAccountmaster where Alies=" + "'" + GoldNo + "'";
                da = new SqlDataAdapter(cmd);
                da.Fill(dt2);

                for (int r = 0; r < dt2.Rows.Count; r++)
                {
                    accNo = dt2.Rows[r]["AccountID"].ToString();
                }
                // DataRow dr = dt.NewRow();
                dt.Rows[s]["AccountID"] = accNo;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //Customer Loan Details                                 
                LoanDate = dt.Rows[i]["LoanDate"].ToString();
                LoanAmount = dt.Rows[i]["LoanCalcAmount"].ToString();

                SqlConnection connAIM = new SqlConnection(strConnStringAIM);
                connAIM.Open();
                string balanceloanamount = string.Empty;
                strQuery = "select isnull(sum(Debit)-sum(Credit),(select sum(Debit)-sum(Credit) From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"].ToString() + "' and " +
                             "RefDate<'" + gbl.ChangeDateMMddyyyy(jvdate) + "' and RefDate>=DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(jvdate) + "')-1),'" + gbl.ChangeDateMMddyyyy(jvdate) + "')))" +
                             "From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"].ToString() + "' and RefDate<DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(jvdate) + "')-1),'" + gbl.ChangeDateMMddyyyy(jvdate) + "')";
                //strQuery = "select isnull(sum(Debit)-sum(Credit),0)  From  FLedgerMaster where AccountID='" + dt.Rows[i]["AccountID"] + "' and RefDate<DATEADD(dd,-(DAY('" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')-1),'" + gbl.ChangeDateMMddyyyy(txtIntDate.Text) + "')";
                cmd = new SqlCommand(strQuery, connAIM);
                cmd.CommandTimeout = 0;
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    balanceloanamount = Convert.ToString(cmd.ExecuteScalar());
                }
                else
                {
                    balanceloanamount = "0";
                }

                SID = dt.Rows[i]["sid"].ToString();

                if (dt.Rows[i]["RCPTID"].ToString() == "0")
                {
                    NeworOld = "0";
                }
                else
                {
                    NeworOld = "1";
                }

                string InterestFromDate = string.Empty;
                if (dt.Rows[i]["InterestFromDate"].ToString() != "" && dt.Rows[i]["InterestFromDate"].ToString() != null)
                {
                    InterestFromDate = dt.Rows[i]["InterestFromDate"].ToString();
                    //InterestFromDate = "28/08/2015";
                }
                else
                {
                    InterestFromDate = System.DateTime.Today.ToShortDateString();
                }
                string InterestToDate = string.Empty;
                if (dt.Rows[i]["InterestToDate"].ToString() != "" && dt.Rows[i]["InterestToDate"].ToString() != null)
                {
                    InterestToDate = dt.Rows[i]["InterestToDate"].ToString();
                    //InterestToDate = txtIntDate.Text;
                }
                else
                {
                    InterestToDate = System.DateTime.Today.ToShortDateString();
                }

                string RvcCLI = string.Empty;
                if (dt.Rows[i]["RecvInterest"].ToString() != "" && dt.Rows[i]["RecvInterest"].ToString() != null)
                {
                    RvcCLI = dt.Rows[i]["RecvInterest"].ToString();
                }
                else
                {
                    RvcCLI = "0";
                }

                string AdvInterestFrom = string.Empty;
                if (dt.Rows[i]["AdvInterestFromDate"].ToString() != "" && dt.Rows[i]["AdvInterestFromDate"].ToString() != null)
                {
                    AdvInterestFrom = dt.Rows[i]["AdvInterestFromDate"].ToString();
                }
                else
                {
                    AdvInterestFrom = System.DateTime.Today.ToShortDateString();
                }

                string AdvInterestTo = string.Empty;
                if (dt.Rows[i]["AdvInterestToDate"].ToString() != "" && dt.Rows[i]["AdvInterestToDate"].ToString() != null)
                {
                    AdvInterestTo = dt.Rows[i]["AdvInterestToDate"].ToString();
                }
                else
                {
                    AdvInterestTo = System.DateTime.Today.ToShortDateString();
                }


                //Last Receipt Details        
                FromDate = InterestFromDate;
                ToDate = InterestToDate;
                PaidInt = RvcCLI;

                OSInterestFromDate = InterestFromDate;
                OSInterestToDate = InterestToDate;
                if ((dt.Rows[i]["OSIntAmt"] != null) || (dt.Rows[i]["OSIntAmt"].ToString() != string.Empty))
                {
                    OSIntAmt = Convert.ToDecimal(dt.Rows[i]["OSIntAmt"].ToString());
                }
                else
                {
                    OSIntAmt = 0;
                }

                AdvInterestFromDate = AdvInterestFrom;
                AdvInterestToDate = AdvInterestTo;
                AdvInterestAmt = Convert.ToDecimal(dt.Rows[i]["AdvInterestAmount"].ToString());

                //Date up to which we have to calculate Interest Amount.      
                CalculateFromDate = dt.Rows[i]["LastRecDate"].ToString();
                CalculateToDate = gbl.ChangeDateMMddyyyy(jvdate);

                LastReceiptID = dt.Rows[i]["RCPTID"].ToString();

                JVFrom = Convert.ToString(Convert.ToDateTime(jvdate.Trim()).Month.ToString() + "/1/" + Convert.ToDateTime(jvdate.Trim()).Year.ToString());
                // JVFrom = CalculateFromDate;
                JVTo = gbl.ChangeDateMMddyyyy(jvdate);

                conn = new SqlConnection(strConnString);
                SqlCommand cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                conn.Open();

                if (!LastReceiptID.Contains("0"))
                {
                    cmdRoiRow.CommandText = "select top 1 ROIRowID from TGLInterest_Details where ReceiptID='" + LastReceiptID + "' and '" + gbl.ChangeDateMMddyyyy(dt.Rows[i]["AcLastRecDate"].ToString()) + "' between FromDate and ToDate order by InterestDetID desc";
                }
                else
                {
                    cmdRoiRow.CommandText = "select  top 1 ROIID From TSchemeMaster_EffectiveROI where SID='" + dt.Rows[i]["SID"].ToString() + "' order by ROIID asc";
                }

                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());

                }

                if (LastROIID == 0)
                {
                    conn = new SqlConnection(strConnString);
                    cmdRoiRow = new SqlCommand();
                    cmdRoiRow.Connection = conn;
                    conn.Open();
                    cmdRoiRow.CommandText = "select  top 1 ROIID From TSchemeMaster_EffectiveROI where SID='" + dt.Rows[i]["SID"].ToString() + "' order by ROIID asc";

                    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                    {
                        LastROIID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());

                    }
                }

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GL_EmiCalculator_RTR_ForMonthly_JV";
                //cmd.CommandText = "GL_EmiCalculator_RTR";

                cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[i]["LoanDate"].ToString()));
                cmd.Parameters.AddWithValue("@LoanAmount", balanceloanamount);
                cmd.Parameters.AddWithValue("@SID", dt.Rows[i]["SID"].ToString());
                cmd.Parameters.AddWithValue("@NeworOld", NeworOld);
                cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);
                cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[i]["OSIntAmt"].ToString());
                cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[i]["AdvInterestAmount"].ToString());
                cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(CalculateFromDate));
                //cmd.Parameters.AddWithValue("@CalculateFromDate", (JVFrom));
                cmd.Parameters.AddWithValue("@CalculateToDate", (CalculateToDate));
                //cmd.Parameters.AddWithValue("@CalculateToDate", (JVTo));
                cmd.Parameters.AddWithValue("@LastROIID", LastROIID);
                cmd.Parameters.AddWithValue("@JVFrom", (JVFrom));
                cmd.Parameters.AddWithValue("@JVTo", (JVTo));

                string sss = Convert.ToString(cmd.ExecuteScalar());
                dt.Rows[i]["Interest Amount"] = Convert.ToInt32(cmd.ExecuteScalar());
                //da = new SqlDataAdapter(cmd);
                //DataTable dt1 = new DataTable();
                // da.Fill(dt1);

                if (Convert.ToDouble(dt.Rows[i]["Interest Amount"]) <= 0)
                {
                    dt.Rows.RemoveAt(i);
                    i--;
                }
            }
            string ss = string.Empty;
            var list = dt.AsEnumerable().Select(dataRow => new Monthlyjv
            {
                GoldLoanNo = dataRow.Field<string>("GoldLoanNo"),
                SDID = dataRow.Field<int>("SDID"),
                CustName = dataRow.Field<string>("Cust Name"),
                AccountID = dataRow.Field<string>("AccountID"),
                LoanAmount = dataRow.Field<decimal>("LoanAmount"),
                LoanCalcAmount = dataRow.Field<decimal>("LoanCalcAmount"),
                BalanceLoanAmount = dataRow.Field<decimal>("LoanCalcAmount"),
                LoanDate = dataRow.Field<string>("LoanDate"),
                ReceiveDate = dataRow.Field<string>("ReceiveDate"),
                InterestAmount = dataRow.Field<string>("Interest Amount"),

            }).ToList();

            return list;      
        
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static int SaveInterestJV(List<Monthlyjv>  monthlyjv)
    {
        int result = 0;
        int ACCID = 6826;
        double DebitAmt, CreditAmt = 0;
        bool datasaved = false;
        string jvDate=string.Empty;

        //Setting Database Connection
        string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
        string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

        //creating instance of class "CompanyWiseAccountClosing"
        GlobalSettings gbl = new GlobalSettings();
        CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

        //Declaring Variables.   
        string strQuery = string.Empty;
        string insertQuery = string.Empty;
        string fyid = string.Empty;
        string DJERefType = "JVGL";
        string DJEReferenceNo = string.Empty;
        string Narration= string.Empty;
        string Narration1 = string.Empty;

        SqlConnection connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();
        SqlTransaction  transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransaction");

        SqlConnection conn = new SqlConnection(strConnString);
        conn.Open();
        SqlTransaction transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransaction");

        for (int index = 0; index < monthlyjv.Count();index++)
        {
         
               
                    int SDID = Convert.ToInt32(monthlyjv[index].SDID);
                    int ConAccID = Convert.ToInt32(monthlyjv[index].AccountID);
                    string goldLoanNo = monthlyjv[index].GoldLoanNo;
                    DateTime loanDate = Convert.ToDateTime(monthlyjv[index].LoanDate);
                    DebitAmt = CreditAmt = Convert.ToDouble(monthlyjv[index].InterestAmount);
                    double LoanAmount = Convert.ToDouble(monthlyjv[index].LoanAmount);
                    double BalLoanAmount = Convert.ToDouble(monthlyjv[index].BalanceLoanAmount);
                    string JvSentDate = monthlyjv[index].JvSentDate;
                    string UserId = monthlyjv[index].UserId;

                    // insert entry into tbl_SentJVCustHistory 

                    string MaxLegderId = string.Empty;
                    strQuery = "select count(*) from FLedgerMaster where RefType='" + DJERefType + "'";
                    SqlCommand cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        MaxLegderId = Convert.ToString(cmd.ExecuteScalar());
                    }

                    strQuery = "select FinancialyearID From tblFinancialyear where '" + gbl.ChangeDateMMddyyyy(JvSentDate) + "'between StartDate and EndDate";
                    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        fyid = Convert.ToString(cmd.ExecuteScalar());
                       // Session["FYearID"] = Convert.ToString(fyid);
                        
                    }

                    DJEReferenceNo = DJERefType + "/" + MaxLegderId;



                    cmd = new SqlCommand(strQuery, conn, transactionGL);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "GL_InsertSentCustJVHistory";

                    cmd.Parameters.AddWithValue("@SDID", SDID);
                    cmd.Parameters.AddWithValue("@GoldLoanNo", goldLoanNo);
                    cmd.Parameters.AddWithValue("@LoanAmount", LoanAmount);
                    cmd.Parameters.AddWithValue("@InterestAmount", DebitAmt);
                    cmd.Parameters.AddWithValue("@BalanceLoanAmount", BalLoanAmount);
                    cmd.Parameters.AddWithValue("@JVSentDate", gbl.ChangeDateMMddyyyy(JvSentDate));
                    cmd.Parameters.AddWithValue("@CreatedBy", UserId);
                    cmd.Parameters.AddWithValue("@UpdatedBy", UserId);

                    result= cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        datasaved = true;
                    }

                    Narration = "INTEREST Upto " + JvSentDate;
                    Narration1 = "Customer A/C INTEREST Upto " + JvSentDate;

                    Narration = Narration + " (" + goldLoanNo + ")";
                    Narration1 = Narration1 + " (" + goldLoanNo + ")";

                    //insert entry into ledger
                    int LedgerID = CreateNormalLedgerEntries(fyid,DJERefType, DJEReferenceNo, Convert.ToDateTime(JvSentDate), ACCID, 0, CreditAmt, ConAccID, Narration, transactionAIM, connAIM);
                    LedgerID = CreateNormalLedgerEntries(fyid,DJERefType, DJEReferenceNo, Convert.ToDateTime(JvSentDate), ConAccID, DebitAmt, 0, ACCID, Narration1, transactionAIM, connAIM);

                    if (datasaved)
                    {

                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(fyid), Convert.ToInt32(1), Convert.ToInt32(1), ACCID, DebitAmt, CreditAmt, transactionAIM, connAIM);

                        goldLoanNo = string.Empty;
                    }

            

             
        }

        if (datasaved)
        {
            transactionAIM.Commit();
            transactionGL.Commit();
        }
        else
        {
            result = 0;
        }
       
        return result;
    }

    
}

public class Monthlyjv
{
    public string GoldLoanNo { get; set; }
    public int SDID { get; set; }
    public string CustName { get; set; }
    public string AccountID { get; set; }
    public decimal LoanAmount { get; set; }
    public decimal LoanCalcAmount { get; set; }
    public decimal BalanceLoanAmount { get; set; }
    public string LoanDate { get; set; }
    public string ReceiveDate { get; set; }
    public string InterestAmount { get; set; }
    public string JvSentDate { get; set; }
    public string UserId { get; set; }
}