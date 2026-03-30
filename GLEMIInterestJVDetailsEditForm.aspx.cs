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

public partial class GLEMIInterestJVDetailsEditForm : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringTesting"].ConnectionString;
    string RefType = "AF";
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string strDID = string.Empty;
    int FYearID = 0;
    int branchId = 0;
    bool datasaved = false;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    DataTable dt;

    public string loginDate;
    public string expressDate;
    //creating instance of class "CompanyWiseAccountClosing"
    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();
    #endregion [Declarations]

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();

                //getting Operator Name and ID
                if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
                {
                    txtOperatorName.Text = Convert.ToString(Session["username"]);
                }

                if (Convert.ToString(Session["userID"]) != "" && Convert.ToString(Session["userID"]) != null)
                {
                    txtOperatorID.Text = Convert.ToString(Session["userID"]);
                }

                //getting Branch ID
                if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
                {
                    branchId = Convert.ToInt32(Session["branchId"]);
                    txtBranchID.Text = Convert.ToString(branchId);

                    //getting CompID
                    strQuery = "select CompID from tblCompanyBranchMaster where BID=" + branchId + "";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);

                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        txtCompID.Text = Convert.ToString(cmd.ExecuteScalar());
                    }
                }

                //getting FYear ID
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID); ;
                }

                //Fill Narration
                FillNarration();

                //Generate Payment Date
                GetPaymentDate();

                //Fill Bank/Cash Account
                FillBankCashAccount();

                //binding GridView
                BindDGVDetails();

                //binding DropDownList Search By
                BindDDLSearchBy();

                //disabling controls
                txtDepositedAmount.Enabled = false;

                //added on onblur event attribute
                txtJVreferenceNo.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtJVreferenceNo, ""));
                ddlPaymentType.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlPaymentType, ""));
                txtPaymentDate.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtPaymentDate, ""));
                txtReferenceDate.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtReferenceDate, ""));
                txtInterestRate.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtInterestRate, ""));
                txtTotalDays.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtTotalDays, ""));
                txtDepositedAmount.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtDepositedAmount, ""));
                txtActualInterest.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtActualInterest, ""));
                chkPenalCharges.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.chkIndemnityCharges, ""));
                chkProcessingFee.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.chkIndemnityCharges, ""));
                chkIndemnityCharges.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.chkIndemnityCharges, ""));
                chkPawnTicketReissue.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.chkIndemnityCharges, ""));

                //Making readonly
                txtCustomerName.Attributes.Add("readonly", "readonly");
                txtLoanIssuedDate.Attributes.Add("readonly", "readonly");
                txtTotalLoanAmount.Attributes.Add("readonly", "readonly");
                txtLastEMIPaidDate.Attributes.Add("readonly", "readonly");
                txtBalanceLoanAmount.Attributes.Add("readonly", "readonly");
                txtTotalBalancePayable.Attributes.Add("readonly", "readonly");
                txtTotalDays.Attributes.Add("readonly", "readonly");
                txtMonthlyPayment.Attributes.Add("readonly", "readonly");
                txtActualInterest.Attributes.Add("readonly", "readonly");
                txtBalanceLoanAmountPlusInterest.Attributes.Add("readonly", "readonly");
                txtBalanceInterest.Attributes.Add("readonly", "readonly");
                txtOperatorName.Attributes.Add("readonly", "readonly");
                txtInterestRate.Attributes.Add("readonly", "readonly");
                txtPenalChargesAmount.Attributes.Add("readonly", "readonly");
                txtProcessingChargesAmount.Attributes.Add("readonly", "readonly");
                txtIndemnityChargesAmount.Attributes.Add("readonly", "readonly");
                txtPawnTicketReissueChargesAmount.Attributes.Add("readonly", "readonly");
            }

            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }

                // Get user login time or last activity time.
                DateTime date = DateTime.Now;
                loginDate = date.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
                int sessionTimeout = Session.Timeout;
                DateTime dateExpress = date.AddMinutes(sessionTimeout);
                expressDate = dateExpress.ToString("u", DateTimeFormatInfo.InvariantInfo).Replace("Z", "");
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Page_Load]

    #region [Reset/Cancel]
    protected void btnReset_Click(object sender, EventArgs e)
    {
        try
        {
            ClearData();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Reset/Cancel]

    #region [Bind GridView DGVDetails]
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "select JVID, JVReferenceNo, PaymentDate=Convert(varchar, PaymentDate,103), GoldLoanNo,  " +
                            "LoanIssuedDate=Convert(varchar, LoanIssuedDate,103), " +
                            "LastEMIPaidDate=Convert(varchar,LastEMIPaidDate,103), PaymentType, " +
                            "InterestDate=Convert(varchar,InterestDate,103), TotalLoanAmount, " +
                            "BalanceLoanAmount=convert(varchar,BalanceLoanAmount), InterestRate=convert(varchar,InterestRate), " +
                            "NoofDays=convert(varchar,NoofDays), DepositedAmount=convert(varchar,DepositedAmount), " +
                            "TotalMonthlyPayment=convert(varchar,TotalMonthlyPayment), TotalBalancePayable=convert(varchar,TotalBalancePayable), " +
                            "tbl_GLEMI_InterestJVDetails.NarrationID, PrincipleAmount, " +
                            "InterestAmount, TotalChargesAmount, tblNarrationMaster.NarrationName, FYID, BranchID " +
                       "from tbl_GLEMI_InterestJVDetails " +
                       "left outer join tblNarrationMaster " +
                            "on tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                       "where tbl_GLEMI_InterestJVDetails.FYID='" + txtFYID.Text + "' " +
                       "and tbl_GLEMI_InterestJVDetails.BranchID='" + txtBranchID.Text + "'";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            dgvDetails.DataSource = ds;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView DGVDetails]

    #region [Bind DropDownList-SearchBy]
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("JVReferenceNo");
            ddlSearchBy.Items.Add("GoldLoanNo");
            ddlSearchBy.Items.Add("PaymentType");
            ddlSearchBy.Items.Add("InterestDate");
            ddlSearchBy.Items.Add("BalanceLoanAmount");
            ddlSearchBy.Items.Add("DepositedAmount");
            ddlSearchBy.Items.Add("InterestRate");
            ddlSearchBy.Items.Add("NoofDays");
            ddlSearchBy.Items.Add("TotalMonthlyPayment");
            ddlSearchBy.Items.Add("TotalBalancePayable");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind DropDownList-SearchBy]

    #region [dgvDetails_RowCommand]
    protected void dgvDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            GridView _gridView = (GridView)sender;
            int _selectedIndex = int.Parse(e.CommandArgument.ToString());
            string _commandName = e.CommandName;
            _gridView.SelectedIndex = _selectedIndex;
            {
                conn = new SqlConnection(strConnString);
                conn.Open();
                dsDGV = new DataSet();
                dsDGV = GetRecords(conn, "GetAllRecords", 0);
                int ID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());

                #region [Delete Record]
                if (_commandName == "DeleteRecord")
                {
                    //checking whether Next Transaction Entry is present
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + _gridView.Rows[_selectedIndex].Cells[4].Text.Trim() + "' and JVID>'" + ID + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since Next Transaction Entry is present.');", true);
                    }

                    if (existcount == 0)
                    {
                        datasaved = true;
                        int QueryResult = 0;
                        string JVReferenceNo = string.Empty;
                        int AccID = 0;
                        double DebitAmount = 0;
                        double CreditAmount = 0;
                        DateTime RefDate;

                        //deleting record from DB
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                        //checking whether record is present
                        strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where JVID='" + ID + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            //Deletion of ledger entries and effects from Company-wise Account Closing table 

                            //getting JV Reference No
                            strQuery = "select JVReferenceNo from tbl_GLEMI_InterestJVDetails where JVID='" + ID + "'";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                JVReferenceNo = Convert.ToString(cmd.ExecuteScalar());
                            }
                            else
                            {
                                JVReferenceNo = "";
                            }

                            // 1] Deleting effects from Company-wise Account Closing table
                            strQuery = "select AccountID, Debit, Credit, RefDate from FLedgerMaster " +
                                        "where ReferenceNo='" + JVReferenceNo + "'";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            cmd.CommandType = CommandType.Text;
                            da = new SqlDataAdapter(cmd);
                            ds = new DataSet();
                            da.Fill(ds);

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow drow1 in ds.Tables[0].Rows)
                                {
                                    if (datasaved)
                                    {
                                        AccID = Convert.ToInt32(drow1[0]);
                                        DebitAmount = Convert.ToDouble(drow1[1]);
                                        CreditAmount = Convert.ToDouble(drow1[2]);
                                        RefDate = Convert.ToDateTime(drow1[3]);

                                        if (datasaved)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            // 2] deleting record from table FLedgerMaster
                            if (datasaved)
                            {
                                deleteQuery = "delete from FLedgerMaster where ReferenceNo='" + JVReferenceNo + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            // 3] deleting record from table TBankCash_ReceiptDetails
                            if (datasaved)
                            {
                                deleteQuery = "delete from TBankCash_ReceiptDetails where ReferenceNo='" + JVReferenceNo + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            //4] deleting record from tbl_GLEMI_ChargesDetails
                            if (datasaved)
                            {
                                deleteQuery = "delete from tbl_GLEMI_ChargesDetails where JVID='" + ID + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            //5] deleting record from tbl_GLEMI_InterestJVDetails
                            if (datasaved)
                            {
                                deleteQuery = "delete from tbl_GLEMI_InterestJVDetails where JVID='" + ID + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            //6] updating table tbl_GLSanctionDisburse_Status
                            if (datasaved == true)
                            {

                                updateQuery = "update tbl_GLSanctionDisburse_Status set GLStatus='Open' " +
                                              "where GoldLoanNo='" + _gridView.Rows[_selectedIndex].Cells[4].Text.Trim() + "'";
                                cmd = new SqlCommand(updateQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            if (QueryResult > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                                BindDGVDetails();

                                //if the same record is deleted which is filled in the form.
                                if (txtID.Text != "" && txtID.Text != null)
                                {
                                    if (ID == Convert.ToInt32(txtID.Text))
                                    {
                                        ClearData();
                                    }
                                }
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Deleted Successfully.');", true);
                            }
                        }
                        else
                        {
                            BindDGVDetails();
                        }
                    }
                }
                #endregion [Delete Record]

                #region [Update Record]
                if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                    txtID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtJVreferenceNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]).Trim();
                    txtReferenceDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][2]).ToString("dd/MM/yyyy").Trim();
                    txtGoldLoanNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][3]).Trim();
                    txtLoanIssuedDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy").Trim();
                    ddlPaymentType.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][6]).Trim();
                    txtPaymentDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][7]).ToString("dd/MM/yyyy").Trim();
                    txtLastEMIPaidDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][5]).ToString("dd/MM/yyyy").Trim();
                    if (Convert.ToDateTime(txtPaymentDate.Text) == Convert.ToDateTime(txtLastEMIPaidDate.Text))
                    {
                        txtLastEMIPaidDate.Text = "";
                    }
                    txtTotalLoanAmount.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][8]).Trim();
                    txtBalanceLoanAmount.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][9]).Trim();
                    txtInterestRate.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsDGV.Tables[0].Rows[0][10]), 2)).Trim();
                    txtTotalDays.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][11]);
                    txtDepositedAmount.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][12]).Trim();
                    txtMonthlyPayment.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsDGV.Tables[0].Rows[0][13]), 2)).Trim();
                    txtTotalBalancePayable.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][14]).Trim();
                    ddlNarration.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][18]);
                    //txtOperatorID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][23]);
                    //txtOperatorName.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][24]);
                    //Amount bifurcation
                    lblDepositedAmount.Text = Convert.ToString(txtDepositedAmount.Text).Trim();
                    lblPrincipal.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][15]).Trim();
                    lblInterest.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][16]).Trim();
                    lblTotalCharges.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsDGV.Tables[0].Rows[0][17]), 2)).Trim();
                    txtActualInterest.Text = Convert.ToString(Math.Round(Convert.ToDouble(dsDGV.Tables[0].Rows[0][20]), 2)).Trim();
                    txtBalanceLoanAmountPlusInterest.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][21]).Trim();
                    txtBalanceInterest.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][22]).Trim();
                    txtChequeNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][26]).Trim();
                    string strChqdate = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][27]).ToString("dd/MM/yyyy").Trim();
                    if (strChqdate != "01/01/1900")
                    {
                        txtChequeDate.Text = strChqdate;
                    }
                    //Fill Cash Account
                    FillBankCashAccount();
                    ddlCashAccount.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][30]).Trim();

                    //fetching Last Bal Interest Payable
                    strQuery = "select LastBalInterestPayable " +
                                "from tbl_GLEMI_InterestJVDetails " +
                                "where JVID=(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and  " +
                                        "JVID<(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "'))";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                        {
                            txtBalInterestPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        }
                        else
                        {
                            txtBalInterestPayable.Text = "0";
                        }
                    }
                    else
                    {
                        txtBalInterestPayable.Text = "0";
                    }
                    
                    //enabling/disabling textboxes 
                    txtDepositedAmount.Enabled = true;
                    //Fill Penal charges
                    FillPenalChargesDropDown();
                    //Fill Processing charges
                    FillProcessingChargesDropDown();
                    //Fill Indemnity charges
                    FillIndemnityChargesDropDown();
                    //Fill Pawn ticket Re-issue charges
                    FillPawnTicketReIssueChargesDropDown();
                    //Fill Penal charges Account
                    FillPenalChargesAccountDropDown();
                    //Fill Processing charges Account
                    FillProcessingChargesAccountDropDown();
                    //Fill Indemnity charges Account
                    FillIndemnityChargesAccountDropDown();
                    //Fill Pawn ticket Re-issue charges Account
                    FillPawnTicketReissueChargesAccountDropDown();
                    //Fill Interest Account
                    FillInterestAccountDropDown();


                    //Charges Details
                    strQuery = "select PenalChargesTaken, ProcessingChargesTaken, IndemnityFeeTaken, PawnTicketReIssueChargesTaken,  " +
                                "PenalChargesID, ProcessingFeeID, IndemnityChargesID, PawnTicketReIssueChargesID, PenalChargesAmount, " +
                                "ProcessingChargesAmount, IndemnityFeeAmount, PawnTicketReIssueChargesAmount, PenalChargesAccountID, " +
                                "ProcessingChargesAccountID, IndemnityFeeAccountID, PawnTicketReIssueChargesAccountID, InterestAccountID, " +
                                "BalancePenalChargesAmount, BalanceProcessingChargesAmount, BalanceIndemnityFeeAmount, BalancePawnTicketReIssueChargesAmount,  " +
                                "LastBalPenalChargesPayable, LastBalProcessingChargesPayable, LastBalIndemnityFeePayable, " +
                                "LastBalPawnTicketReIssueChargesPayable " +
                            "from tbl_GLEMI_ChargesDetails " +
                            "where tbl_GLEMI_ChargesDetails.JVID='" + ID + "'";
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string PenalChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][0]).Trim();
                        if (PenalChargesTaken.Trim() == "Yes")
                        {
                            chkPenalCharges.Checked = true;
                        }
                        else if (PenalChargesTaken.Trim() == "No")
                        {
                            chkPenalCharges.Checked = false;
                        }

                        if (chkPenalCharges.Checked == true)
                        {
                            //enable
                            ddlPenalCharges.Enabled = true;
                            txtPenalChargesAmount.Enabled = true;
                            ddlPenalChargesAccount.Enabled = true;
                            //assigning values
                            ddlPenalCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][4]).Trim();
                            txtPenalChargesAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][8]), 2)).Trim();
                            ddlPenalChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][12]).Trim();
                        }
                        else
                        {
                            //disable
                            ddlPenalCharges.Enabled = false;
                            txtPenalChargesAmount.Enabled = false;
                            ddlPenalChargesAccount.Enabled = false;
                            //clear
                            ddlPenalCharges.SelectedValue = "0";
                            txtPenalChargesAmount.Text = "0";
                            ddlPenalChargesAccount.SelectedValue = "0";
                        }

                        string ProcessingFeeChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                        if (ProcessingFeeChargesTaken.Trim() == "Yes")
                        {
                            chkProcessingFee.Checked = true;
                        }
                        else if (ProcessingFeeChargesTaken.Trim() == "No")
                        {
                            chkProcessingFee.Checked = false;
                        }

                        if (chkProcessingFee.Checked == true)
                        {
                            //enable
                            ddlProcessingCharges.Enabled = true;
                            txtProcessingChargesAmount.Enabled = true;
                            ddlProcessingChargesAccount.Enabled = true;

                            //assigning values
                            ddlProcessingCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][5]).Trim();
                            txtProcessingChargesAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][9]), 2)).Trim();
                            ddlProcessingChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][13]).Trim();
                        }
                        else
                        {
                            //disable
                            ddlProcessingCharges.Enabled = false;
                            txtProcessingChargesAmount.Enabled = false;
                            ddlProcessingChargesAccount.Enabled = false;
                            //clear
                            ddlProcessingCharges.SelectedValue = "0";
                            txtProcessingChargesAmount.Text = "0";
                            ddlProcessingChargesAccount.SelectedValue = "0";
                        }

                        string IndemnityChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                        if (IndemnityChargesTaken.Trim() == "Yes")
                        {
                            chkIndemnityCharges.Checked = true;
                        }
                        else if (IndemnityChargesTaken.Trim() == "No")
                        {
                            chkIndemnityCharges.Checked = false;
                        }

                        if (chkIndemnityCharges.Checked == true)
                        {
                            //enable
                            ddlIndemnityCharges.Enabled = true;
                            txtIndemnityChargesAmount.Enabled = true;
                            ddlIndemnityChargesAccount.Enabled = true;

                            //assigning values
                            ddlIndemnityCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]).Trim();
                            txtIndemnityChargesAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][10]), 2)).Trim();
                            ddlIndemnityChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][14]).Trim();
                        }
                        else
                        {
                            //disable
                            ddlIndemnityCharges.Enabled = false;
                            txtIndemnityChargesAmount.Enabled = false;
                            ddlIndemnityChargesAccount.Enabled = false;
                            //clear
                            ddlIndemnityCharges.SelectedValue = "0";
                            txtIndemnityChargesAmount.Text = "0";
                            ddlIndemnityChargesAccount.SelectedValue = "0";
                        }

                        string PawnTicketReIssueChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][3]).Trim();
                        if (PawnTicketReIssueChargesTaken.Trim() == "Yes")
                        {
                            chkPawnTicketReissue.Checked = true;
                        }
                        else if (PawnTicketReIssueChargesTaken.Trim() == "No")
                        {
                            chkPawnTicketReissue.Checked = false;
                        }

                        if (chkPawnTicketReissue.Checked == true)
                        {
                            //enable
                            ddlPawnTicketReissueCharges.Enabled = true;
                            txtPawnTicketReissueChargesAmount.Enabled = true;
                            ddlPawnTicketReissueChargesAccount.Enabled = true;

                            //assigning values
                            ddlPawnTicketReissueCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][7]).Trim();
                            txtPawnTicketReissueChargesAmount.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][11]), 2)).Trim();
                            ddlPawnTicketReissueChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][15]).Trim();
                        }
                        else
                        {
                            //disable
                            ddlPawnTicketReissueCharges.Enabled = false;
                            txtPawnTicketReissueChargesAmount.Enabled = false;
                            ddlPawnTicketReissueChargesAccount.Enabled = false;
                            //clear
                            ddlPawnTicketReissueCharges.SelectedValue = "0";
                            txtPawnTicketReissueChargesAmount.Text = "0";
                            ddlPawnTicketReissueChargesAccount.SelectedValue = "0";
                        }

                        ddlInterestAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][16]).Trim();
                        txtBalancePenalCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        txtBalanceProcessingCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][12]).Trim();
                        txtBalanceIndemnityFee.Text = Convert.ToString(ds.Tables[0].Rows[0][13]).Trim();
                        txtBalancePawnTicketReIssueCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][14]).Trim();
                    }

                    //Last Payables Charges
                    strQuery = "select LastBalPenalChargesPayable, LastBalProcessingChargesPayable, LastBalIndemnityFeePayable, " +
                               "LastBalPawnTicketReIssueChargesPayable " +
                           "from tbl_GLEMI_ChargesDetails " +
                           "where tbl_GLEMI_ChargesDetails.JVID=(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and " +
                                        "JVID<(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "'))";
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtBalPenalChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][0]).Trim();
                        txtBalProcessingChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                        txtBalIndemnityFeePayable.Text = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                        txtBalPawnTicketReIssueChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][3]).Trim();
                    }
                    
                    //fetching Customer Name
                    strQuery = "SELECT (AppFName+' '+AppMName+' '+AppLName) as 'CustomerName' " +
                               "FROM tbl_GLKYC_ApplicantDetails " +
                               "WHERE  tbl_GLKYC_ApplicantDetails.GoldLoanNo='" + txtGoldLoanNo.Text.Trim() + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                        {
                            txtCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        }
                        else
                        {
                            txtCustomerName.Text = "";
                        }
                    }
                    else
                    {
                        txtCustomerName.Text = "";
                    }

                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";

                    //checking whether transaction is processed to next stage
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and JVID>'" + txtID.Text + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since Next Transaction Entry is present.');", true);
                    }
                }
                #endregion [Update Record]
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [dgvDetails_RowCommand]

    #region [dgvDetails_PageIndexChanging]
    protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDetails.PageIndex = e.NewPageIndex;
            BindDGVDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDetails_PageIndexChanging]

    #region [GetRecords]
    protected DataSet GetRecords(SqlConnection conn, string CommandName, int ID)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "select JVID, JVReferenceNo, PaymentDate=Convert(varchar, PaymentDate,103), GoldLoanNo,  " +
                            "LoanIssuedDate=Convert(varchar, LoanIssuedDate,103), " +
                            "LastEMIPaidDate=Convert(varchar,LastEMIPaidDate,103), PaymentType, " +
                            "InterestDate=Convert(varchar,InterestDate,103), TotalLoanAmount, " +
                            "BalanceLoanAmount=convert(varchar,BalanceLoanAmount), InterestRate=convert(varchar,InterestRate), " +
                            "NoofDays=convert(varchar,NoofDays), DepositedAmount=convert(varchar,DepositedAmount), " +
                            "TotalMonthlyPayment=convert(varchar,TotalMonthlyPayment), TotalBalancePayable=convert(varchar,TotalBalancePayable), " +
                            "tbl_GLEMI_InterestJVDetails.NarrationID, PrincipleAmount, " +
                            "InterestAmount, TotalChargesAmount, tblNarrationMaster.NarrationName, FYID, BranchID " +
                       "from tbl_GLEMI_InterestJVDetails " +
                       "left outer join tblNarrationMaster " +
                            "on tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                       "where tbl_GLEMI_InterestJVDetails.FYID='" + txtFYID.Text + "' " +
                       "and tbl_GLEMI_InterestJVDetails.BranchID='" + txtBranchID.Text + "'";

            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "select JVID, JVReferenceNo, PaymentDate=Convert(varchar, PaymentDate,103), GoldLoanNo,  " +
                                "LoanIssuedDate=Convert(varchar, LoanIssuedDate,103), " +
                                "LastEMIPaidDate=Convert(varchar,LastEMIPaidDate,103), PaymentType, " +
                                "InterestDate=Convert(varchar,InterestDate,103), TotalLoanAmount, BalanceLoanAmount, InterestRate, " +
                                "NoofDays, DepositedAmount, TotalMonthlyPayment, TotalBalancePayable, PrincipleAmount, " +
                                "InterestAmount, TotalChargesAmount, tbl_GLEMI_InterestJVDetails.NarrationID, " +
                                "tblNarrationMaster.NarrationName, ActualInterest, BalLoanAmountPlusInterest, " +
                                "BalanceInterest, OperatorID, UserName, LastBalInterestPayable, ChqNo, ChqDate, FYID, BranchID, BankCashAccID " +
                            "from tbl_GLEMI_InterestJVDetails " +
                            "left outer join tblNarrationMaster " +
                                "on tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                            "left outer join UserDetails " +
                                "on tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                            "where tbl_GLEMI_InterestJVDetails.JVID='" + ID + "'";
            }
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
        return ds;
    }
    #endregion [GetRecords]

    #region [ClearData]
    protected void ClearData()
    {
        try
        {
            txtReferenceDate.Text = "";
            txtGoldLoanNo.Text = "";
            txtOperatorName.Text = "";
            txtOperatorID.Text = "";
            txtJVreferenceNo.Text = "";
            txtLoanIssuedDate.Text = "";
            txtCustomerName.Text = "";
            txtTotalLoanAmount.Text = "0";
            txtLastEMIPaidDate.Text = "";
            txtPaymentDate.Text = "";
            txtBalanceLoanAmount.Text = "0";
            ddlPaymentType.SelectedValue = "--Select Payment Type--";
            txtTotalDays.Text = "0";
            txtDepositedAmount.Text = "";
            //chkIndemnityCharges.Checked = false;
            txtInterestRate.Text = "";
            txtTotalBalancePayable.Text = "0";
            txtMonthlyPayment.Text = "0";
            txtActualInterest.Text = "0";
            txtBalanceInterest.Text = "0";
            txtBalInterestPayable.Text = "0";
            txtBalanceLoanAmountPlusInterest.Text = "0";
            lblMsg.Text = "";
            lblDepositedAmount.Text = "";
            lblPrincipal.Text = "";
            lblInterest.Text = "";
            lblTotalCharges.Text = "";
            chkPenalCharges.Checked = false;
            chkProcessingFee.Checked = false;
            chkIndemnityCharges.Checked = false;
            chkPawnTicketReissue.Checked = false;
            txtPenalChargesAmount.Text = "0";
            txtProcessingChargesAmount.Text = "0";
            txtIndemnityChargesAmount.Text = "0";
            txtPawnTicketReissueChargesAmount.Text = "0";
            txtChequeNo.Text = "";
            txtChequeDate.Text = "";

            //Penal Charges
            ddlPenalCharges.Enabled = false;
            txtPenalChargesAmount.Enabled = false;
            ddlPenalChargesAccount.Enabled = false;
            //Processing Fee
            ddlProcessingCharges.Enabled = false;
            txtProcessingChargesAmount.Enabled = false;
            ddlProcessingChargesAccount.Enabled = false;
            //Indemnity Fee
            ddlIndemnityCharges.Enabled = false;
            txtIndemnityChargesAmount.Enabled = false;
            ddlIndemnityChargesAccount.Enabled = false;
            //Pawn Ticket Re-issue Charges
            ddlPawnTicketReissueCharges.Enabled = false;
            txtPawnTicketReissueChargesAmount.Enabled = false;
            ddlPawnTicketReissueChargesAccount.Enabled = false;

            txtDueDate.Text = "";
            txtBalInterestPayable.Text = "0";
            txtBalPenalChargesPayable.Text = "0";
            txtBalProcessingChargesPayable.Text = "0";
            txtBalIndemnityFeePayable.Text = "0";
            txtBalPawnTicketReIssueChargesPayable.Text = "0";
            txtBalanceLoanAmountCalc.Text = "0";
            txtBalancePenalCharges.Text = "0";
            txtBalanceProcessingCharges.Text = "0";
            txtBalanceIndemnityFee.Text = "0";
            txtBalancePawnTicketReIssueCharges.Text = "0";
            txtTotalChargesAmount.Text = "0";

            btnSave.Text = "Update";
            btnReset.Text = "Cancel";

            ddlNarration.DataSource = null;
            ddlNarration.DataBind();

            ////Fill Ref Num
            //GetRefNum();

            //Fill Narration
            FillNarration();
            //Fill Bank/Cash Account
            FillBankCashAccount();
            //Fill Penal charges
            FillPenalChargesDropDown();
            //Fill Processing charges
            FillProcessingChargesDropDown();
            //Fill Indemnity charges
            FillIndemnityChargesDropDown();
            //Fill Pawn ticket Re-issue charges
            FillPawnTicketReIssueChargesDropDown();
            //Fill Penal charges Account
            FillPenalChargesAccountDropDown();
            //Fill Processing charges Account
            FillProcessingChargesAccountDropDown();
            //Fill Indemnity charges Account
            FillIndemnityChargesAccountDropDown();
            //Fill Pawn ticket Re-issue charges Account
            FillPawnTicketReissueChargesAccountDropDown();
            //Fill Interest Account
            FillInterestAccountDropDown();
            
            //getting FYear ID
            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
                txtFYID.Text = Convert.ToString(FYearID); ;
            }

            //getting Branch ID
            if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
            {
                branchId = Convert.ToInt32(Session["branchId"]);
                txtBranchID.Text = Convert.ToString(branchId);

                //getting CompID
                strQuery = "select CompID from tblCompanyBranchMaster where BID=" + branchId + "";
                conn = new SqlConnection(strConnString);
                conn.Open();
                cmd = new SqlCommand(strQuery, conn);

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    txtCompID.Text = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            //getting Operator Name and ID
            if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
            {
                txtOperatorName.Text = Convert.ToString(Session["username"]);
            }

            if (Convert.ToString(Session["userID"]) != "" && Convert.ToString(Session["userID"]) != null)
            {
                txtOperatorID.Text = Convert.ToString(Session["userID"]);
            }

            //binding GridView
            BindDGVDetails();

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ClearData]

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            datasaved = false;
            conn = new SqlConnection(strConnString);
            conn.Open();
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

            #region [Update]
            if (Page.IsValid)
            {
                if (btnSave.Text == "Update")
                {
                    //checking whether transaction is processed to next stage
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and JVID>'" + txtID.Text + "'";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since Next Transaction Entry is present.');", true);
                    }
                    else
                    {
                        int Val = ValidateData();

                        if (Val == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Payment Date must be greater than Loan Issued Date and Last EMI Paid Date.');", true);
                        }
                        else if (Val == 2)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Total Days must be greater than zero.');", true);
                        }
                        else
                        {
                            int JVID = Convert.ToInt32(txtID.Text);
                            int QueryResult = 0;

                            // 1] Data updation tbl_GLEMI_InterestJVDetails
                            string GoldLoanNo = Convert.ToString(txtGoldLoanNo.Text.Trim());
                            //Last EMI Paid Date
                            if (Convert.ToString(txtLastEMIPaidDate.Text).Trim() == "")
                            {
                                txtLastEMIPaidDate.Text = txtPaymentDate.Text;
                            }

                            // Penal Charges Taken
                            string PenalChargesTaken = string.Empty;
                            if (chkPenalCharges.Checked == true)
                            {
                                PenalChargesTaken = "Yes";
                            }
                            else
                            {
                                PenalChargesTaken = "No";
                            }

                            // Processing Charges Taken
                            string ProcessingChargesTaken = string.Empty;
                            if (chkProcessingFee.Checked == true)
                            {
                                ProcessingChargesTaken = "Yes";
                            }
                            else
                            {
                                ProcessingChargesTaken = "No";
                            }

                            //Indemnity Fee Taken
                            string IndemnityFeeTaken = string.Empty;
                            if (chkIndemnityCharges.Checked == true)
                            {
                                IndemnityFeeTaken = "Yes";
                            }
                            else
                            {
                                IndemnityFeeTaken = "No";
                            }

                            //Pawn Ticket Re-Issue Charges Taken
                            string PawnTicketReIssueChargesTaken = string.Empty;
                            if (chkPawnTicketReissue.Checked == true)
                            {
                                PawnTicketReIssueChargesTaken = "Yes";
                            }
                            else
                            {
                                PawnTicketReIssueChargesTaken = "No";
                            }

                            Int32 GLRefID;
                            Int64 TotalLoanAmount;
                            Int64 BalanceLoanAmount;
                            double InterestRate;
                            Int32 TotalDays;
                            Int64 DepositedAmount;
                            double MonthlyPayment;
                            Int64 TotalBalancePayable;
                            Int64 Principal;
                            Int64 Interest;
                            Int64 TotalCharges;
                            double ProcessingFee;
                            double IndemnityCharges;
                            double ActualInterest;
                            Int64 BalanceLoanAmountPlusInterest;
                            Int64 BalanceInterest;
                            Int64 BalInterestPayable;
                            Int32 NarrationID;
                            Int32 OperatorID;
                            Int32 FYID;
                            Int32 BranchID;

                            //int.TryParse(GLRefIDs, out GLRefID);
                            Int64.TryParse(txtTotalLoanAmount.Text, out TotalLoanAmount);
                            Int64.TryParse(txtBalanceLoanAmount.Text, out BalanceLoanAmount);
                            double.TryParse(txtInterestRate.Text, out InterestRate);
                            Int32.TryParse(txtTotalDays.Text, out TotalDays);
                            Int64.TryParse(txtDepositedAmount.Text, out DepositedAmount);
                            double.TryParse(txtMonthlyPayment.Text, out MonthlyPayment);
                            Int64.TryParse(txtTotalBalancePayable.Text, out TotalBalancePayable);
                            Int64.TryParse(lblPrincipal.Text, out Principal);
                            Int64.TryParse(lblInterest.Text, out Interest);
                            Int64.TryParse(lblTotalCharges.Text, out TotalCharges);
                            double.TryParse(txtActualInterest.Text, out ActualInterest);
                            Int64.TryParse(txtBalanceLoanAmountPlusInterest.Text, out BalanceLoanAmountPlusInterest);
                            Int64.TryParse(txtBalanceInterest.Text, out BalanceInterest);
                            Int64.TryParse(txtBalInterestPayable.Text, out BalInterestPayable);
                            Int32.TryParse(ddlNarration.SelectedValue, out NarrationID);
                            Int32.TryParse(txtOperatorID.Text, out OperatorID);
                            Int32.TryParse(txtFYID.Text, out FYID);
                            Int32.TryParse(txtBranchID.Text, out BranchID);


                            int PenalChargesID = 0;
                            int PenalChargesAccountID = 0;
                            string PenalChargesName = string.Empty;
                            double PenalChargesAmount = 0;
                            double BalancePenalChargesAmount = 0;
                            double LastBalPenalChargesPayable = 0;

                            int ProcessingFeeID = 0;
                            int ProcessingChargesAccountID = 0;
                            string ProcessingChargesName = string.Empty;
                            double ProcessingChargesAmount = 0;
                            double BalanceProcessingChargesAmount = 0;
                            double LastBalProcessingChargesPayable = 0;

                            int IndemnityChargesID = 0;
                            int IndemnityFeeAccountID = 0;
                            string IndemnityFeeName = string.Empty;
                            double IndemnityFeeAmount = 0;
                            double BalanceIndemnityFeeAmount = 0;
                            double LastBalIndemnityFeePayable = 0;

                            int PawnTicketReIssueChargesID = 0;
                            int PawnTicketReIssueChargesAccountID = 0;
                            string PawnTicketReIssueChargesName = string.Empty;
                            double PawnTicketReIssueChargesAmount = 0;
                            double BalancePawnTicketReIssueChargesAmount = 0;
                            double LastBalPawnTicketReIssueChargesPayable = 0;


                            if (PenalChargesTaken == "Yes")
                            {
                                PenalChargesID = Convert.ToInt32(ddlPenalCharges.SelectedValue);
                                PenalChargesAccountID = Convert.ToInt32(ddlPenalChargesAccount.SelectedValue);
                                PenalChargesName = ddlPenalCharges.SelectedItem.Text;
                                PenalChargesAmount = Convert.ToDouble(txtPenalChargesAmount.Text);
                                BalancePenalChargesAmount = Convert.ToDouble(txtBalancePenalCharges.Text);
                                LastBalPenalChargesPayable = Convert.ToDouble(txtBalPenalChargesPayable.Text);
                            }

                            if (ProcessingChargesTaken == "Yes")
                            {
                                ProcessingFeeID = Convert.ToInt32(ddlProcessingCharges.SelectedValue);
                                ProcessingChargesAccountID = Convert.ToInt32(ddlProcessingChargesAccount.SelectedValue);
                                ProcessingChargesName = ddlProcessingCharges.SelectedItem.Text;
                                ProcessingChargesAmount = Convert.ToDouble(txtProcessingChargesAmount.Text);
                                BalanceProcessingChargesAmount = Convert.ToDouble(txtBalanceProcessingCharges.Text);
                                LastBalProcessingChargesPayable = Convert.ToDouble(txtBalProcessingChargesPayable.Text);
                            }

                            if (IndemnityFeeTaken == "Yes")
                            {
                                IndemnityChargesID = Convert.ToInt32(ddlIndemnityCharges.SelectedValue);
                                IndemnityFeeAccountID = Convert.ToInt32(ddlIndemnityChargesAccount.SelectedValue);
                                IndemnityFeeName = ddlIndemnityCharges.SelectedItem.Text;
                                IndemnityFeeAmount = Convert.ToDouble(txtIndemnityChargesAmount.Text);
                                BalanceIndemnityFeeAmount = Convert.ToDouble(txtBalanceIndemnityFee.Text);
                                LastBalIndemnityFeePayable = Convert.ToDouble(txtBalIndemnityFeePayable.Text);
                            }

                            if (PawnTicketReIssueChargesTaken == "Yes")
                            {
                                PawnTicketReIssueChargesID = Convert.ToInt32(ddlPawnTicketReissueCharges.SelectedValue);
                                PawnTicketReIssueChargesAccountID = Convert.ToInt32(ddlPawnTicketReissueChargesAccount.SelectedValue);
                                PawnTicketReIssueChargesName = ddlPawnTicketReissueCharges.SelectedItem.Text;
                                PawnTicketReIssueChargesAmount = Convert.ToDouble(txtPawnTicketReissueChargesAmount.Text);
                                BalancePawnTicketReIssueChargesAmount = Convert.ToDouble(txtBalancePawnTicketReIssueCharges.Text);
                                LastBalPawnTicketReIssueChargesPayable = Convert.ToDouble(txtBalPawnTicketReIssueChargesPayable.Text);

                            }

                            string strChqDate = string.Empty;
                            if (txtChequeDate.Text.Trim() != "")
                            {
                                strChqDate = Convert.ToDateTime(txtChequeDate.Text).ToString("yyyy/MM/dd");
                            }

                            //updating data into table tbl_GLEMI_InterestJVDetails
                            updateQuery = "update tbl_GLEMI_InterestJVDetails set PaymentDate='" + Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "LoanIssuedDate='" + Convert.ToDateTime(txtLoanIssuedDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "LastEMIPaidDate='" + Convert.ToDateTime(txtLastEMIPaidDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "PaymentType='" + ddlPaymentType.SelectedValue + "', " +
                                                "InterestDate='" + Convert.ToDateTime(txtPaymentDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "TotalLoanAmount='" + txtTotalLoanAmount.Text.Trim() + "', BalanceLoanAmount='" + txtBalanceLoanAmount.Text.Trim() + "', " +
                                                "InterestRate='" + txtInterestRate.Text.Trim() + "', NoofDays='" + txtTotalDays.Text.Trim() + "', " +
                                                "DepositedAmount='" + txtDepositedAmount.Text.Trim() + "', TotalMonthlyPayment='" + txtMonthlyPayment.Text.Trim() + "', " +
                                                "TotalBalancePayable='" + txtTotalBalancePayable.Text.Trim() + "', PrincipleAmount='" + lblPrincipal.Text.Trim() + "', " +
                                                "InterestAmount='" + lblInterest.Text.Trim() + "', TotalChargesAmount='" + TotalCharges + "', " +
                                                "ActualInterest='" + txtActualInterest.Text.Trim() + "', BalLoanAmountPlusInterest='" + txtBalanceLoanAmountPlusInterest.Text.Trim() + "', " +
                                                "BalanceInterest='" + txtBalanceInterest.Text.Trim() + "', LastBalInterestPayable='" + txtBalInterestPayable.Text.Trim() + "', " +
                                                "BankCashAccID='" + ddlCashAccount.SelectedValue + "', ChqNo='" + txtChequeNo.Text.Trim() + "', ChqDate='" + strChqDate + "', " +
                                                "NarrationID='" + ddlNarration.SelectedValue + "', OperatorID='" + txtOperatorID.Text.Trim() + "', " +
                                                "FYID='" + txtFYID.Text + "', BranchID='" + txtBranchID.Text + "' " +
                                            "where JVID='" + txtID.Text + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                            else
                            {
                                datasaved = false;
                            }

                            if (datasaved)
                            {
                                //updating data into table tbl_GLEMI_ChargesDetails
                                updateQuery = "update tbl_GLEMI_ChargesDetails set " +
                                                    "PenalChargesTaken='" + PenalChargesTaken + "', " +
                                                    "ProcessingChargesTaken='" + ProcessingChargesTaken + "', " +
                                                    "IndemnityFeeTaken='" + IndemnityFeeTaken + "', " +
                                                    "PawnTicketReIssueChargesTaken='" + PawnTicketReIssueChargesTaken + "', " +
                                                    "PenalChargesID='" + PenalChargesID + "', " +
                                                    "ProcessingFeeID='" + ProcessingFeeID + "', " +
                                                    "IndemnityChargesID='" + IndemnityChargesID + "', " +
                                                    "PawnTicketReIssueChargesID='" + PawnTicketReIssueChargesID + "', " +
                                                    "PenalChargesName='" + PenalChargesName + "', " +
                                                    "ProcessingChargesName='" + ProcessingChargesName + "', " +
                                                    "IndemnityFeeName='" + IndemnityFeeName + "', " +
                                                    "PawnTicketReIssueChargesName='" + PawnTicketReIssueChargesName + "', " +
                                                    "PenalChargesAmount='" + PenalChargesAmount + "', " +
                                                    "ProcessingChargesAmount='" + ProcessingChargesAmount + "', " +
                                                    "IndemnityFeeAmount='" + IndemnityFeeAmount + "', " +
                                                    "PawnTicketReIssueChargesAmount='" + PawnTicketReIssueChargesAmount + "', " +
                                                    "PenalChargesAccountID='" + PenalChargesAccountID + "', " +
                                                    "ProcessingChargesAccountID='" + ProcessingChargesAccountID + "', " +
                                                    "IndemnityFeeAccountID='" + IndemnityFeeAccountID + "', " +
                                                    "PawnTicketReIssueChargesAccountID='" + PawnTicketReIssueChargesAccountID + "', " +
                                                    "InterestAccountID='" + ddlInterestAccount.SelectedValue + "', " +
                                                    "BalancePenalChargesAmount='" + BalancePenalChargesAmount + "', " +
                                                    "BalanceProcessingChargesAmount='" + BalanceProcessingChargesAmount + "', " +
                                                    "BalanceIndemnityFeeAmount='" + BalanceIndemnityFeeAmount + "', " +
                                                    "BalancePawnTicketReIssueChargesAmount='" + BalancePawnTicketReIssueChargesAmount + "', " +
                                                    "LastBalPenalChargesPayable='" + LastBalPenalChargesPayable + "', " +
                                                    "LastBalProcessingChargesPayable='" + LastBalProcessingChargesPayable + "', " +
                                                    "LastBalIndemnityFeePayable='" + LastBalIndemnityFeePayable + "', " +
                                                    "LastBalPawnTicketReIssueChargesPayable='" + LastBalPawnTicketReIssueChargesPayable + "' " +
                                                "where JVID='" + txtID.Text + "'";

                                cmd = new SqlCommand(updateQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            // 2] Data insertion into tbl_GLSanctionDisburse_Status
                            if (datasaved == true)
                            {
                                if (Convert.ToString(txtTotalBalancePayable.Text).Trim() != "")
                                {
                                    if (Convert.ToDouble(txtTotalBalancePayable.Text) == 0)
                                    {
                                        //updating table tbl_GLSanctionDisburse_Status
                                        updateQuery = "update tbl_GLSanctionDisburse_Status set GLStatus='Close' " +
                                                      "where GoldLoanNo='" + GoldLoanNo.Trim() + "'";

                                        cmd = new SqlCommand(updateQuery, conn, transaction);
                                        QueryResult = cmd.ExecuteNonQuery();

                                        if (QueryResult > 0)
                                        {
                                            datasaved = true;
                                        }
                                        else
                                        {
                                            datasaved = false;
                                        }
                                    }
                                    else
                                    {
                                        //updating table tbl_GLSanctionDisburse_Status
                                        updateQuery = "update tbl_GLSanctionDisburse_Status set GLStatus='Open' " +
                                                      "where GoldLoanNo='" + GoldLoanNo.Trim() + "'";

                                        cmd = new SqlCommand(updateQuery, conn, transaction);
                                        QueryResult = cmd.ExecuteNonQuery();

                                        if (QueryResult > 0)
                                        {
                                            datasaved = true;
                                        }
                                        else
                                        {
                                            datasaved = false;
                                        }
                                    }
                                }
                            }

                            //+++++++++++++++++++++++++++++++++ UPDATION OF LEDGER ENTRIES ++++++++++++++++++++++++++++++++++++++

                            //getting Account ID of GL Customer
                            int GLAccountID = 0;
                            string JVReferenceNo = string.Empty;

                            if (datasaved)
                            {
                                strQuery = "select AccountID from tblAccountMaster where Alies='" + GoldLoanNo + "' and GPID=17";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    GLAccountID = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    GLAccountID = 0;
                                }
                            }

                            //getting JV Reference No
                            if (datasaved)
                            {
                                strQuery = "select JVReferenceNo from tbl_GLEMI_InterestJVDetails where JVID='" + txtID.Text + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    JVReferenceNo = Convert.ToString(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    JVReferenceNo = "";
                                }

                            }
                            // 3] Updation of Bank Cash Receipt Details
                            string Narration = "Amount Received on " + Convert.ToDateTime(txtReferenceDate.Text).ToString("dd/MM/yyyy") + "Rs." + Convert.ToString(txtDepositedAmount.Text) + "/-. (" + GoldLoanNo + ")";
                            if (datasaved)
                            {
                                string strRefDate = Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd");
                                int bankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                                int receivedFrom = GLAccountID;
                                double Amount = Convert.ToDouble(txtDepositedAmount.Text);
                                string subMode = string.Empty;
                                if (ddlCashAccount.SelectedValue != "0")
                                {
                                    strQuery = "select tblAccountmaster.GPID from tblAccountmaster " +
                                                 "where tblAccountMaster.AccountID='" + ddlCashAccount.SelectedValue + "' ";
                                    cmd = new SqlCommand(strQuery, conn, transaction);
                                    int accGPID = Convert.ToInt32(cmd.ExecuteScalar());

                                    if (accGPID == 70)
                                    {
                                        subMode = "Cash";
                                    }
                                    else
                                    {
                                        subMode = "Cheque";
                                    }
                                }

                                strChqDate = string.Empty;
                                if (txtChequeDate.Text.Trim() != "")
                                {
                                    strChqDate = Convert.ToDateTime(txtChequeDate.Text).ToString("yyyy/MM/dd");
                                }

                                updateQuery = "update TBankCash_ReceiptDetails " +
                                                        "set RefDate='" + strRefDate + "', " +
                                                        "BankCashAccID=" + bankCashAccID + ", " +
                                                        "ReceivedFrom=" + receivedFrom + ", " +
                                                        "Amount=" + Amount + ", " +
                                                        "Narration='" + Narration + "', " +
                                                        "ChqNo='" + txtChequeNo.Text + "', " +
                                                        "ChqDate='" + strChqDate + "' " +
                                                "where ReferenceNo='" + JVReferenceNo + "' ";

                                cmd = new SqlCommand(updateQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }


                            // 4] updation of Ledger and Company Wise Account Closing tables
                            int AccID = 0;
                            double DebitAmount = 0;
                            double CreditAmount = 0;
                            DateTime RefDate;
                            string JVRefType = "JV";

                            // 4.1] Deleting effects from Company-wise Account Closing table
                            if (datasaved)
                            {
                                strQuery = "select AccountID, Debit, Credit, RefDate from FLedgerMaster " +
                                        "where ReferenceNo='" + JVReferenceNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                cmd.CommandType = CommandType.Text;
                                da = new SqlDataAdapter(cmd);
                                ds = new DataSet();
                                da.Fill(ds);

                                if (ds.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow drow1 in ds.Tables[0].Rows)
                                    {
                                        if (datasaved)
                                        {
                                            AccID = Convert.ToInt32(drow1[0]);
                                            DebitAmount = Convert.ToDouble(drow1[1]);
                                            CreditAmount = Convert.ToDouble(drow1[2]);
                                            RefDate = Convert.ToDateTime(drow1[3]);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            // 4.2] deleting record from table FLedgerMaster
                            if (datasaved)
                            {
                                deleteQuery = "delete from FLedgerMaster where ReferenceNo='" + JVReferenceNo + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            // 4.1] Debit Entry in FLedger (Main Ledger Entry)
                            // Bank/Cash A/C To Sundry Debtor A/C (for total amt received)
                            int LedgerID = 0;
                            if (datasaved)
                            {
                                AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                                int ContraAccID = GLAccountID;
                                DebitAmount = Convert.ToDouble(txtDepositedAmount.Text);
                                CreditAmount = 0;
                                LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

                                if (datasaved)
                                {
                                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                }
                            }

                            // 4.2] Updating table TBankCash_ReceiptDetails with Ledger ID
                            if (datasaved)
                            {
                                updateQuery = "update TBankCash_ReceiptDetails " +
                                                        "set LedgerID=" + LedgerID + " " +
                                              "where ReferenceNo='" + JVReferenceNo + "' ";

                                cmd = new SqlCommand(updateQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }

                            // 4.3] Contra Entry in FLedger 
                            if (datasaved)
                            {
                                AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                                int ContraAccID = GLAccountID;
                                DebitAmount = 0;
                                CreditAmount = Convert.ToDouble(txtDepositedAmount.Text);
                                LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                if (datasaved)
                                {
                                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                }
                            }


                            // 5] Sundry Debtor A/C To Interest, Penal Charges, Indemnity Charges, Processing Fee, Pawn Ticket Re-Issue Charges A/C (for total charges)
                            LedgerID = 0;
                            if (datasaved)
                            {
                                double PenalChrgAmt = 0;
                                double ProcessingFeeAmt = 0;
                                double IndemnityChargesAmt = 0;
                                double PawnTicketReIssueChrgAmt = 0;
                                double InterestAmt = Convert.ToDouble(lblInterest.Text);

                                double BalPenalChrgAmt = 0;
                                double BalProcessingFeeAmt = 0;
                                double BalIndemnityChargesAmt = 0;
                                double BalPawnTicketReIssueChrgAmt = 0;
                                double BalInterestAmt = 0;
                                int PenalAccountID = 0;
                                int ProcessingFeeAccountID = 0;
                                int IndemnityAccountID = 0;
                                int PawnAccountID = 0;
                                int InterestAccountID = 0;



                                //Penal Charges
                                if (chkPenalCharges.Checked == true)
                                {
                                    if (ddlPenalChargesAccount.SelectedValue != "0")
                                    {
                                        PenalAccountID = Convert.ToInt32(ddlPenalChargesAccount.SelectedValue);
                                    }
                                    if (txtBalancePenalCharges.Text.Trim() != "")
                                    {
                                        BalPenalChrgAmt = Convert.ToDouble(txtBalancePenalCharges.Text);
                                    }
                                    if (txtPenalChargesAmount.Text.Trim() != "")
                                    {
                                        PenalChrgAmt = Convert.ToDouble(txtPenalChargesAmount.Text);
                                    }
                                    PenalChrgAmt = PenalChrgAmt - BalPenalChrgAmt;
                                }

                                //Processing Charges
                                if (chkProcessingFee.Checked == true)
                                {
                                    if (ddlProcessingChargesAccount.SelectedValue != "0")
                                    {
                                        ProcessingFeeAccountID = Convert.ToInt32(ddlProcessingChargesAccount.SelectedValue);
                                    }
                                    if (txtBalanceProcessingCharges.Text.Trim() != "")
                                    {
                                        BalProcessingFeeAmt = Convert.ToDouble(txtBalanceProcessingCharges.Text);
                                    }
                                    if (txtProcessingChargesAmount.Text.Trim() != "")
                                    {
                                        ProcessingFeeAmt = Convert.ToDouble(txtProcessingChargesAmount.Text);
                                    }
                                    ProcessingFeeAmt = ProcessingFeeAmt - BalProcessingFeeAmt;
                                }

                                //Indemnity Charges
                                if (chkIndemnityCharges.Checked == true)
                                {
                                    if (ddlIndemnityChargesAccount.SelectedValue != "0")
                                    {
                                        IndemnityAccountID = Convert.ToInt32(ddlIndemnityChargesAccount.SelectedValue);
                                    }
                                    if (txtBalanceIndemnityFee.Text.Trim() != "")
                                    {
                                        BalIndemnityChargesAmt = Convert.ToDouble(txtBalanceIndemnityFee.Text);
                                    }
                                    if (txtIndemnityChargesAmount.Text.Trim() != "")
                                    {
                                        IndemnityChargesAmt = Convert.ToDouble(txtIndemnityChargesAmount.Text);
                                    }
                                    IndemnityChargesAmt = IndemnityChargesAmt - BalIndemnityChargesAmt;
                                }

                                //Pawn Ticket ReIssue Charges
                                if (chkPawnTicketReissue.Checked == true)
                                {
                                    if (ddlPawnTicketReissueChargesAccount.SelectedValue != "0")
                                    {
                                        PawnAccountID = Convert.ToInt32(ddlPawnTicketReissueChargesAccount.SelectedValue);
                                    }
                                    if (txtBalancePawnTicketReIssueCharges.Text.Trim() != "")
                                    {
                                        BalPawnTicketReIssueChrgAmt = Convert.ToDouble(txtBalancePawnTicketReIssueCharges.Text);
                                    }
                                    if (txtPawnTicketReissueChargesAmount.Text.Trim() != "")
                                    {
                                        PawnTicketReIssueChrgAmt = Convert.ToDouble(txtPawnTicketReissueChargesAmount.Text);
                                    }
                                    PawnTicketReIssueChrgAmt = PawnTicketReIssueChrgAmt - BalPawnTicketReIssueChrgAmt;
                                }

                                //Interest
                                if (InterestAmt != 0)
                                {
                                    if (ddlInterestAccount.SelectedValue != "0")
                                    {
                                        InterestAccountID = Convert.ToInt32(ddlInterestAccount.SelectedValue);
                                    }
                                }

                                //Total Amount Received
                                double TotalChargesReceived = InterestAmt + PenalChrgAmt + IndemnityChargesAmt + ProcessingFeeAmt + PawnTicketReIssueChrgAmt;

                                if (InterestAmt != 0 || PenalChrgAmt != 0 || IndemnityChargesAmt != 0 || ProcessingFeeAmt != 0 || PawnTicketReIssueChrgAmt != 0)
                                {
                                    int CreditID = 0;
                                    string lblAmt = "";

                                    if (PenalChrgAmt != 0)
                                    {
                                        CreditID = PenalAccountID;
                                    }
                                    else if (ProcessingFeeAmt != 0)
                                    {
                                        CreditID = ProcessingFeeAccountID;
                                    }
                                    else if (IndemnityChargesAmt != 0)
                                    {
                                        CreditID = IndemnityAccountID;
                                    }
                                    else if (PawnTicketReIssueChrgAmt != 0)
                                    {
                                        CreditID = PawnAccountID;
                                    }
                                    else if (InterestAmt != 0)
                                    {
                                        CreditID = InterestAccountID;
                                    }

                                    if (InterestAmt != 0)
                                    {
                                        lblAmt = "Interest";
                                    }
                                    if (PenalChrgAmt != 0)
                                    {
                                        lblAmt += "+Penal Chrgs";
                                    }
                                    if (ProcessingFeeAmt != 0)
                                    {
                                        lblAmt += "+Processing Fee";
                                    }
                                    if (IndemnityChargesAmt != 0)
                                    {
                                        lblAmt += "+Indemnity Chrgs";
                                    }
                                    if (PawnTicketReIssueChrgAmt != 0)
                                    {
                                        lblAmt += "+Ticket Reissue Chrgs";
                                    }


                                    // 5.1] Debit Entry in FLedger (Main Ledger Entry)
                                    LedgerID = 0;
                                    if (datasaved)
                                    {
                                        AccID = GLAccountID;
                                        int ContraAccID = CreditID;
                                        DebitAmount = TotalChargesReceived;
                                        CreditAmount = 0;
                                        Narration = "Charges Received on " + Convert.ToDateTime(txtReferenceDate.Text).ToString("dd/MM/yyyy") + "Rs." + Convert.ToString(TotalChargesReceived) + "/-. (" + GoldLoanNo + " (" + lblAmt + ")" + ")";
                                        LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

                                        if (datasaved)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                        }
                                    }

                                    // 5.2] Contra Entry in FLedger 

                                    // For Penal Charges
                                    if (datasaved)
                                    {
                                        if (PenalChrgAmt != 0)
                                        {
                                            AccID = GLAccountID;
                                            int ContraAccID = PenalAccountID;
                                            DebitAmount = 0;
                                            CreditAmount = Convert.ToDouble(PenalChrgAmt);
                                            LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                    }

                                    // For Processing Fee
                                    if (datasaved)
                                    {
                                        if (ProcessingFeeAmt != 0)
                                        {
                                            AccID = GLAccountID;
                                            int ContraAccID = ProcessingFeeAccountID;
                                            DebitAmount = 0;
                                            CreditAmount = Convert.ToDouble(ProcessingFeeAmt);
                                            LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                    }

                                    // For Indemnity Charges
                                    if (datasaved)
                                    {
                                        if (IndemnityChargesAmt != 0)
                                        {
                                            AccID = GLAccountID;
                                            int ContraAccID = IndemnityAccountID;
                                            DebitAmount = 0;
                                            CreditAmount = Convert.ToDouble(IndemnityChargesAmt);
                                            LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                    }

                                    // For Pawn Ticket Re-Issue Charges
                                    if (datasaved)
                                    {
                                        if (PawnTicketReIssueChrgAmt != 0)
                                        {
                                            AccID = GLAccountID;
                                            int ContraAccID = PawnAccountID;
                                            DebitAmount = 0;
                                            CreditAmount = Convert.ToDouble(PawnTicketReIssueChrgAmt);
                                            LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                    }

                                    // For Interest
                                    if (datasaved)
                                    {
                                        if (InterestAmt != 0)
                                        {
                                            AccID = GLAccountID;
                                            int ContraAccID = InterestAccountID;
                                            DebitAmount = 0;
                                            CreditAmount = Convert.ToDouble(InterestAmt);
                                            LedgerID = CreateNormalLedgerEntries(JVRefType, JVReferenceNo, Convert.ToDateTime(txtReferenceDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);
                                            }
                                        }
                                    }
                                }
                            }
                            if (datasaved == true)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Updated Successfully.');", true);
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Updated Successfully.');", true);
                                datasaved = false;
                            }
                        }
                    }
                }
            }
            #endregion [Update]
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
            datasaved = false;
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            if (datasaved == true)
            {
                ClearData();
            }
        }
    }
    #endregion [Save Data]

    #region [CreateNormalLedgerEntries]
    protected int CreateNormalLedgerEntries(string Reftype, string ReferenceNo, DateTime RefDate, int AccID, double DebitAmount, double CreditAmount, int ContraAccID, string Narration)
    {
        int LedgerID = 0;
        try
        {
            strQuery = "SELECT MAX(LedgerID) FROM FLedgerMaster";
            cmd = new SqlCommand(strQuery, conn, transaction);
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

            insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + ReferenceNo + "', '" + Reftype + "'," +
                                    "'" + Convert.ToDateTime(RefDate).ToString("yyyy/MM/dd") + "', " +
                                    "" + AccID + ", " + DebitAmount + ", " + CreditAmount + ", '" + Narration + "', " + ContraAccID + ", " +
                                    " '', " + txtFYID.Text + ") ";

            cmd = new SqlCommand(insertQuery, conn, transaction);
            int QueryResult = cmd.ExecuteNonQuery();

            if (QueryResult > 0)
            {
                datasaved = true;
            }
            else
            {
                datasaved = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LedgerEntryAlert", "alert('" + ex.Message + "');", true);
        }
        return LedgerID;
    }
    #endregion [CreateNormalLedgerEntries]

    #region Search Record
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //Search Records
            DataTable dt = GetRecords(conn, "GetAllRecords", 0).Tables[0];
            DataView dv = new DataView(dt);
            string SearchExpression = null;
            string SearchBy = ddlSearchBy.Text;

            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                SearchExpression = string.Format("{0} '%{1}%'", dgvDetails.SortExpression, txtSearch.Text);
                dv.RowFilter = Convert.ToString(SearchBy) + " like" + SearchExpression;
            }

            dgvDetails.DataSource = dv;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [txtJVreferenceNo_TextChanged]
    protected void txtJVreferenceNo_TextChanged(object sender, EventArgs e)
    {
        try
        {
            FillJVDetails(txtJVreferenceNo.Text.Trim());
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "JVreferenceNoEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtJVreferenceNo_TextChanged]

    #region [txtPaymentDate_TextChanged]
    protected void txtPaymentDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //Penal charges
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlPenalCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlPenalCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    strChargeAmount = strCharges;
                }
                else
                {
                    double Interest = 0;
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    if (txtActualInterest.Text.Trim() != "")
                    {
                        Interest = Convert.ToDouble(txtActualInterest.Text);
                        Interest = Math.Ceiling(Interest);
                    }
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }
                    //calculating charges
                    if (noofdays > 0)
                    {
                        ChargeAmount = Interest * ((ChargesPercent / 12) / 100);
                        decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                        strChargeAmount = Convert.ToString(dChargeAmount);
                    }
                }

                txtPenalChargesAmount.Text = strChargeAmount;
            }

            //Calculating Monthly Payment
            CalculateTotalDays();
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PayDateTextChangeEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PaymentDate_TextChanged]

    #region [txtReferenceDate_TextChanged]
    protected void txtReferenceDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //Penal charges
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlPenalCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlPenalCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    strChargeAmount = strCharges;
                }
                else
                {
                    double Interest = 0;
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    if (txtActualInterest.Text.Trim() != "")
                    {
                        Interest = Convert.ToDouble(txtActualInterest.Text);
                        Interest = Math.Ceiling(Interest);
                    }
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }
                    //calculating charges
                    if (noofdays > 0)
                    {
                        ChargeAmount = Interest * ((ChargesPercent / 12) / 100);
                        decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                        strChargeAmount = Convert.ToString(dChargeAmount);
                    }
                }

                txtPenalChargesAmount.Text = strChargeAmount;
            }

            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "RefDateTextChangeEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtReferenceDate_TextChanged]

    #region [PaymentType_SelectedIndexChanged]
    protected void PaymentType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlPaymentType.SelectedValue == "Pre-payment")
            {
                txtDepositedAmount.Enabled = true;
            }
            else if (ddlPaymentType.SelectedValue == "Monthly Payment")
            {
                txtDepositedAmount.Enabled = true;
            }
            else
            {
                txtDepositedAmount.Enabled = false;
                txtDepositedAmount.Text = "";
                //txtInterestRate.Text = "";
                txtTotalBalancePayable.Text = "";
                txtMonthlyPayment.Text = "";
                txtActualInterest.Text = "";
                txtBalanceInterest.Text = "";
                txtBalanceLoanAmountPlusInterest.Text = "";
                lblDepositedAmount.Text = "";
                lblPrincipal.Text = "";
                lblInterest.Text = "";
                lblTotalCharges.Text = "";
            }

            try
            {
                //fetching SchemeType, DueDate
                string SchemeType = string.Empty;
                string DueDate = string.Empty;
                //RefType = txtRefType.Text.Trim();
                //string RefNum = ddlRefNum.SelectedValue.ToString().Trim();
                //string RefID = ddlRefID.SelectedValue.ToString().Trim();
                string GoldLoanNo = txtGoldLoanNo.Text.Trim();
                conn = new SqlConnection(strConnString);
                conn.Open();

                strQuery = "SELECT SDetailID, ID, SchemeType, DueDate " +
                            "FROM tbl_GLSanctionDisburse_SchemeDetails " +
                            "WHERE GoldLoanNo='" + GoldLoanNo + "'";
                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        SchemeType = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        if (SchemeType == "MI")
                        {
                            if (txtLastEMIPaidDate.Text.Trim() == "")
                            {
                                txtPaymentDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][3]).ToString("dd/MM/yyyy");
                                txtDueDate.Text = txtPaymentDate.Text;
                            }
                            else
                            {
                                DateTime dt = Convert.ToDateTime(txtLastEMIPaidDate.Text);
                                DateTime dueDate = Convert.ToDateTime(ds.Tables[0].Rows[0][3]);
                                int day = dueDate.Day;
                                dt = dt.AddMonths(1);
                                int month = dt.Month;
                                int year = dt.Year;

                                string strDueDate = Convert.ToString(day) + "/" + Convert.ToString(month) + "/" + Convert.ToString(year);
                                txtPaymentDate.Text = strDueDate;
                                txtDueDate.Text = txtPaymentDate.Text;
                            }
                        }
                        else
                        {
                            if (txtLastEMIPaidDate.Text.Trim() == "")
                            {
                                DateTime dt = Convert.ToDateTime(txtLoanIssuedDate.Text);
                                int month = dt.Month;
                                int year = dt.Year;
                                int noOfDays = DateTime.DaysInMonth(year, month);
                                string strDueDate = Convert.ToString(noOfDays) + "/" + Convert.ToString(month) + "/" + Convert.ToString(year);
                                txtPaymentDate.Text = strDueDate;
                                txtDueDate.Text = txtPaymentDate.Text;
                            }
                            else
                            {
                                DateTime dt = Convert.ToDateTime(txtLastEMIPaidDate.Text);
                                dt = dt.AddMonths(1);
                                int day = dt.Day;
                                int month = dt.Month;
                                int year = dt.Year;

                                string strDueDate = Convert.ToString(day) + "/" + Convert.ToString(month) + "/" + Convert.ToString(year);
                                txtPaymentDate.Text = strDueDate;
                                txtDueDate.Text = txtPaymentDate.Text;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "PaymentTypeEventAlert", "alert('" + ex.Message + "');", true);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            CalculateTotalDays();
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PaymentTypeEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PaymentType_SelectedIndexChanged]

    #region [InterestRate_TextChanged]
    protected void InterestRate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "InterestRateEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [InterestRate_TextChanged]

    #region [txtDepositedAmount_TextChanged]
    protected void txtDepositedAmount_TextChanged(object sender, EventArgs e)
    {
        try
        {
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DepositedAmountEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtDepositedAmount_TextChanged]

    #region [txtTotalDays_TextChanged]
    protected void txtTotalDays_TextChanged(object sender, EventArgs e)
    {
        try
        {
            int NoofDays = Convert.ToInt32(txtTotalDays.Text);

            if (NoofDays <= 0)
            {
                lblMsg.Text = "Total Days must be greater than zero.";
            }
            else
            {
                lblMsg.Text = "";
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "TotalDaysEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtTotalDays_TextChanged]

    #region [ActualInterest_TextChanged]
    protected void ActualInterest_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlPenalCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlPenalCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }

                    if (noofdays > 0)
                    {
                        strChargeAmount = strCharges;
                    }
                    else
                    {
                        strChargeAmount = "0";
                    }
                }
                else
                {
                    double Interest = 0;
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    if (txtActualInterest.Text.Trim() != "")
                    {
                        Interest = Convert.ToDouble(txtActualInterest.Text);
                        Interest = Math.Ceiling(Interest);
                    }
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }
                    //calculating charges
                    if (noofdays > 0)
                    {
                        ChargeAmount = Interest * ((ChargesPercent / 12) / 100);
                        decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                        strChargeAmount = Convert.ToString(dChargeAmount);
                    }
                }

                txtPenalChargesAmount.Text = strChargeAmount;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PenalChargesEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ActualInterest_TextChanged]

    #region [Fill JV Details]
    protected void FillJVDetails(string JVReferenceNo)
    {
        try
        {
            //checking whether JV Reference No exists
            int existCount = 0;
            strQuery = "select count(JVReferenceNo) from tbl_GLEMI_InterestJVDetails " +
                            "where JVReferenceNo='" + JVReferenceNo + "'";
            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand(strQuery, conn);
            existCount = Convert.ToInt32(cmd.ExecuteScalar());

            if (existCount > 0)
            {
                if (txtFYID.Text.Trim() != "" && txtBranchID.Text.Trim() != "")
                {
                    //checking whether JV Reference No belongs to same branch and financial year.
                    strQuery = "select count(JVReferenceNo) from tbl_GLEMI_InterestJVDetails " +
                                    "where JVReferenceNo='" + JVReferenceNo + "' and FYID='" + txtFYID.Text + "' and BranchID='" + txtBranchID.Text + "'";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);
                    existCount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existCount > 0)
                    {
                        strQuery = "select JVID, JVReferenceNo, PaymentDate=Convert(varchar, PaymentDate,103), GoldLoanNo,  " +
                                "LoanIssuedDate=Convert(varchar, LoanIssuedDate,103), " +
                                "LastEMIPaidDate=Convert(varchar,LastEMIPaidDate,103), PaymentType, " +
                                "InterestDate=Convert(varchar,InterestDate,103), TotalLoanAmount, BalanceLoanAmount, InterestRate, " +
                                "NoofDays, DepositedAmount, TotalMonthlyPayment, TotalBalancePayable, PrincipleAmount, " +
                                "InterestAmount, TotalChargesAmount, tbl_GLEMI_InterestJVDetails.NarrationID, " +
                                "tblNarrationMaster.NarrationName, ActualInterest, BalLoanAmountPlusInterest, " +
                                "BalanceInterest, OperatorID, UserName, LastBalInterestPayable, ChqNo, ChqDate, FYID, BranchID, BankCashAccID " +
                            "from tbl_GLEMI_InterestJVDetails " +
                            "left outer join tblNarrationMaster " +
                                "on tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                            "left outer join UserDetails " +
                                "on tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                            "where tbl_GLEMI_InterestJVDetails.JVReferenceNo='" + JVReferenceNo + "'";

                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            txtID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                            txtJVreferenceNo.Text = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                            txtReferenceDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][2]).ToString("dd/MM/yyyy").Trim();
                            txtGoldLoanNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]).Trim();
                            txtLoanIssuedDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy").Trim();
                            ddlPaymentType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]).Trim();
                            txtPaymentDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][7]).ToString("dd/MM/yyyy").Trim();
                            txtLastEMIPaidDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][5]).ToString("dd/MM/yyyy").Trim();
                            if (Convert.ToDateTime(txtPaymentDate.Text) == Convert.ToDateTime(txtLastEMIPaidDate.Text))
                            {
                                txtLastEMIPaidDate.Text = "";
                            }
                            txtTotalLoanAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][8]).Trim();
                            txtBalanceLoanAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                            txtInterestRate.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][10]), 2)).Trim();
                            txtTotalDays.Text = Convert.ToString(ds.Tables[0].Rows[0][11]);
                            txtDepositedAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][12]).Trim();
                            txtMonthlyPayment.Text = Convert.ToString(ds.Tables[0].Rows[0][13]).Trim();
                            txtTotalBalancePayable.Text = Convert.ToString(ds.Tables[0].Rows[0][14]).Trim();
                            ddlNarration.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][18]);
                            //txtOperatorID.Text = Convert.ToString(ds.Tables[0].Rows[0][23]);
                            //txtOperatorName.Text = Convert.ToString(ds.Tables[0].Rows[0][24]);
                            //Amount bifurcation
                            lblDepositedAmount.Text = Convert.ToString(txtDepositedAmount.Text).Trim();
                            lblPrincipal.Text = Convert.ToString(ds.Tables[0].Rows[0][15]).Trim();
                            lblInterest.Text = Convert.ToString(ds.Tables[0].Rows[0][16]).Trim();
                            lblTotalCharges.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][17]), 2)).Trim();
                            txtActualInterest.Text = Convert.ToString(Math.Round(Convert.ToDouble(ds.Tables[0].Rows[0][20]), 2)).Trim();
                            txtBalanceLoanAmountPlusInterest.Text = Convert.ToString(ds.Tables[0].Rows[0][21]).Trim();
                            txtBalanceInterest.Text = Convert.ToString(ds.Tables[0].Rows[0][22]).Trim();
                            txtChequeNo.Text = Convert.ToString(ds.Tables[0].Rows[0][26]).Trim();
                            string strChqdate = Convert.ToDateTime(ds.Tables[0].Rows[0][27]).ToString("dd/MM/yyyy").Trim();
                            if (strChqdate != "01/01/1900")
                            {
                                txtChequeDate.Text = strChqdate;
                            }
                            //Fill Cash Account
                            FillBankCashAccount();
                            ddlCashAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][30]).Trim();
                        }

                        //fetching Last Bal Interest Payable
                        strQuery = "select LastBalInterestPayable " +
                                    "from tbl_GLEMI_InterestJVDetails " +
                                    "where JVID=(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and  " +
                                            "JVID<(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "'))";

                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                            {
                                txtBalInterestPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                            }
                            else
                            {
                                txtBalInterestPayable.Text = "0";
                            }
                        }
                        else
                        {
                            txtBalInterestPayable.Text = "0";
                        }

                        //enabling/disabling textboxes 
                        txtDepositedAmount.Enabled = true;
                        //Fill Penal charges
                        FillPenalChargesDropDown();
                        //Fill Processing charges
                        FillProcessingChargesDropDown();
                        //Fill Indemnity charges
                        FillIndemnityChargesDropDown();
                        //Fill Pawn ticket Re-issue charges
                        FillPawnTicketReIssueChargesDropDown();
                        //Fill Penal charges Account
                        FillPenalChargesAccountDropDown();
                        //Fill Processing charges Account
                        FillProcessingChargesAccountDropDown();
                        //Fill Indemnity charges Account
                        FillIndemnityChargesAccountDropDown();
                        //Fill Pawn ticket Re-issue charges Account
                        FillPawnTicketReissueChargesAccountDropDown();
                        //Fill Interest Account
                        FillInterestAccountDropDown();

                        //Charges Details
                        strQuery = "select PenalChargesTaken, ProcessingChargesTaken, IndemnityFeeTaken, PawnTicketReIssueChargesTaken,  " +
                                    "PenalChargesID, ProcessingFeeID, IndemnityChargesID, PawnTicketReIssueChargesID, PenalChargesAmount, " +
                                    "ProcessingChargesAmount, IndemnityFeeAmount, PawnTicketReIssueChargesAmount, PenalChargesAccountID, " +
                                    "ProcessingChargesAccountID, IndemnityFeeAccountID, PawnTicketReIssueChargesAccountID, InterestAccountID, " +
                                    "BalancePenalChargesAmount, BalanceProcessingChargesAmount, BalanceIndemnityFeeAmount, BalancePawnTicketReIssueChargesAmount,  " +
                                    "LastBalPenalChargesPayable, LastBalProcessingChargesPayable, LastBalIndemnityFeePayable, " +
                                    "LastBalPawnTicketReIssueChargesPayable " +
                                "from tbl_GLEMI_ChargesDetails " +
                                "where tbl_GLEMI_ChargesDetails.JVID='" + ID + "'";
                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            string PenalChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][0]).Trim();
                            if (PenalChargesTaken.Trim() == "Yes")
                            {
                                chkPenalCharges.Checked = true;
                            }
                            else if (PenalChargesTaken.Trim() == "No")
                            {
                                chkPenalCharges.Checked = false;
                            }

                            if (chkPenalCharges.Checked == true)
                            {
                                //enable
                                ddlPenalCharges.Enabled = true;
                                txtPenalChargesAmount.Enabled = true;
                                ddlPenalChargesAccount.Enabled = true;
                                //assigning values
                                ddlPenalCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][4]).Trim();
                                txtPenalChargesAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][8]).Trim();
                                ddlPenalChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][12]).Trim();
                            }
                            else
                            {
                                //disable
                                ddlPenalCharges.Enabled = false;
                                txtPenalChargesAmount.Enabled = false;
                                ddlPenalChargesAccount.Enabled = false;
                                //clear
                                ddlPenalCharges.SelectedValue = "0";
                                txtPenalChargesAmount.Text = "0";
                                ddlPenalChargesAccount.SelectedValue = "0";
                            }

                            string ProcessingFeeChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                            if (ProcessingFeeChargesTaken.Trim() == "Yes")
                            {
                                chkProcessingFee.Checked = true;
                            }
                            else if (ProcessingFeeChargesTaken.Trim() == "No")
                            {
                                chkProcessingFee.Checked = false;
                            }

                            if (chkProcessingFee.Checked == true)
                            {
                                //enable
                                ddlProcessingCharges.Enabled = true;
                                txtProcessingChargesAmount.Enabled = true;
                                ddlProcessingChargesAccount.Enabled = true;

                                //assigning values
                                ddlProcessingCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][5]).Trim();
                                txtProcessingChargesAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                                ddlProcessingChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][13]).Trim();
                            }
                            else
                            {
                                //disable
                                ddlProcessingCharges.Enabled = false;
                                txtProcessingChargesAmount.Enabled = false;
                                ddlProcessingChargesAccount.Enabled = false;
                                //clear
                                ddlProcessingCharges.SelectedValue = "0";
                                txtProcessingChargesAmount.Text = "0";
                                ddlProcessingChargesAccount.SelectedValue = "0";
                            }

                            string IndemnityChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                            if (IndemnityChargesTaken.Trim() == "Yes")
                            {
                                chkIndemnityCharges.Checked = true;
                            }
                            else if (IndemnityChargesTaken.Trim() == "No")
                            {
                                chkIndemnityCharges.Checked = false;
                            }

                            if (chkIndemnityCharges.Checked == true)
                            {
                                //enable
                                ddlIndemnityCharges.Enabled = true;
                                txtIndemnityChargesAmount.Enabled = true;
                                ddlIndemnityChargesAccount.Enabled = true;

                                //assigning values
                                ddlIndemnityCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]).Trim();
                                txtIndemnityChargesAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                                ddlIndemnityChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][14]).Trim();
                            }
                            else
                            {
                                //disable
                                ddlIndemnityCharges.Enabled = false;
                                txtIndemnityChargesAmount.Enabled = false;
                                ddlIndemnityChargesAccount.Enabled = false;
                                //clear
                                ddlIndemnityCharges.SelectedValue = "0";
                                txtIndemnityChargesAmount.Text = "0";
                                ddlIndemnityChargesAccount.SelectedValue = "0";
                            }

                            string PawnTicketReIssueChargesTaken = Convert.ToString(ds.Tables[0].Rows[0][3]).Trim();
                            if (PawnTicketReIssueChargesTaken.Trim() == "Yes")
                            {
                                chkPawnTicketReissue.Checked = true;
                            }
                            else if (PawnTicketReIssueChargesTaken.Trim() == "No")
                            {
                                chkPawnTicketReissue.Checked = false;
                            }

                            if (chkPawnTicketReissue.Checked == true)
                            {
                                //enable
                                ddlPawnTicketReissueCharges.Enabled = true;
                                txtPawnTicketReissueChargesAmount.Enabled = true;
                                ddlPawnTicketReissueChargesAccount.Enabled = true;

                                //assigning values
                                ddlPawnTicketReissueCharges.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][7]).Trim();
                                txtPawnTicketReissueChargesAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                                ddlPawnTicketReissueChargesAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][15]).Trim();
                            }
                            else
                            {
                                //disable
                                ddlPawnTicketReissueCharges.Enabled = false;
                                txtPawnTicketReissueChargesAmount.Enabled = false;
                                ddlPawnTicketReissueChargesAccount.Enabled = false;
                                //clear
                                ddlPawnTicketReissueCharges.SelectedValue = "0";
                                txtPawnTicketReissueChargesAmount.Text = "0";
                                ddlPawnTicketReissueChargesAccount.SelectedValue = "0";
                            }

                            ddlInterestAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][16]).Trim();
                            txtBalancePenalCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                            txtBalanceProcessingCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][12]).Trim();
                            txtBalanceIndemnityFee.Text = Convert.ToString(ds.Tables[0].Rows[0][13]).Trim();
                            txtBalancePawnTicketReIssueCharges.Text = Convert.ToString(ds.Tables[0].Rows[0][14]).Trim();
                        }

                        //Last Payables Charges
                        strQuery = "select LastBalPenalChargesPayable, LastBalProcessingChargesPayable, LastBalIndemnityFeePayable, " +
                                   "LastBalPawnTicketReIssueChargesPayable " +
                               "from tbl_GLEMI_ChargesDetails " +
                               "where tbl_GLEMI_ChargesDetails.JVID=(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and " +
                                            "JVID<(select max(JVID) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "'))";
                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            txtBalPenalChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][0]).Trim();
                            txtBalProcessingChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                            txtBalIndemnityFeePayable.Text = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                            txtBalPawnTicketReIssueChargesPayable.Text = Convert.ToString(ds.Tables[0].Rows[0][3]).Trim();
                        }

                        //fetching Customer Name
                        strQuery = "SELECT (AppFName+' '+AppMName+' '+AppLName) as 'CustomerName' " +
                                   "FROM tbl_GLKYC_ApplicantDetails " +
                                   "WHERE  tbl_GLKYC_ApplicantDetails.GoldLoanNo='" + txtGoldLoanNo.Text.Trim() + "'";

                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                            {
                                txtCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                            }
                            else
                            {
                                txtCustomerName.Text = "";
                            }
                        }
                        else
                        {
                            txtCustomerName.Text = "";
                        }

                        //checking whether transaction is processed to next stage
                        strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + txtGoldLoanNo.Text + "' and JVID>'" + txtID.Text + "'";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since Next Transaction Entry is present.');", true);
                        }
                    }
                    else
                    {
                        int FYearID = 0;
                        int BranchID = 0;
                        int Bcount = 0;
                        int Fcount = 0;

                        //fetching Financial year ID and Branch ID.
                        strQuery = "select FYID, BranchID from tbl_GLEMI_InterestJVDetails " +
                                        "where JVReferenceNo='" + JVReferenceNo + "'";
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            FYearID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            BranchID = Convert.ToInt32(ds.Tables[0].Rows[0][1]);
                        }

                        //fetching financial year.
                        string FYear = string.Empty;
                        strQuery = "select Financialyear from tblFinancialyear " +
                                        "where FinancialyearID='" + FYearID + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        FYear = Convert.ToString(cmd.ExecuteScalar());

                        //fetching branch name.
                        string branchName = string.Empty;
                        strQuery = "select BranchName from tblCompanyBranchMaster " +
                                        "where BID='" + BranchID + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        branchName = Convert.ToString(cmd.ExecuteScalar());

                        if (Convert.ToInt32(txtBranchID.Text) != BranchID)
                        {
                            Bcount = 1;
                        }
                        if (Convert.ToInt32(txtFYID.Text) != FYearID)
                        {
                            Fcount = 1;
                        }

                        if (Bcount == 1 && Fcount == 0)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit JV Reference No. " + JVReferenceNo + " please Log into Branch: " + branchName + ".');", true);
                            ClearData();
                        }
                        else if (Bcount == 0 && Fcount == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit JV Reference No. " + JVReferenceNo + " please Log into Financial Year: " + FYear + ".');", true);
                            ClearData();
                        }
                        else if (Bcount == 1 && Fcount == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit JV Reference No. " + JVReferenceNo + " please Log into Branch: " + branchName + " and Financial Year: " + FYear + ".');", true);
                            ClearData();
                        }
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('JV Reference No. " + JVReferenceNo + " does not exist.');", true);
                ClearData();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillJVDetailAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Fill JV Details]

    #region [PaymentDate_ServerValidate]
    protected void PaymentDate_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            DateTime dtToValid = System.DateTime.Today;
            DateTime PaymentDate;
            if (txtLastEMIPaidDate.Text.Trim() != "")
            {
                dtToValid = Convert.ToDateTime(txtLastEMIPaidDate.Text.Trim());
            }
            else
            {
                if (txtLoanIssuedDate.Text.Trim() != "")
                {
                    dtToValid = Convert.ToDateTime(txtLoanIssuedDate.Text.Trim());
                }
            }

            if (txtPaymentDate.Text.Trim() != "")
            {
                PaymentDate = Convert.ToDateTime(txtPaymentDate.Text.Trim());

                if (PaymentDate <= dtToValid)
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PayDtServrValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [PaymentDate_ServerValidate]

    #region [TotalDays_ServerValidate]
    protected void TotalDays_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            int NoofDays = Convert.ToInt32(txtTotalDays.Text);
            lblMsg.Text = "";

            if (NoofDays <= 0)
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "TotalDaysValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [TotalDays_ServerValidate]

    #region [ddlPaymentType_ServerValidate]
    protected void ddlPaymentType_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (Convert.ToString(txtTotalBalancePayable.Text.Trim()) != "")
            {
                if (Convert.ToInt32(txtTotalBalancePayable.Text) == 0)
                {
                    e.IsValid = ddlPaymentType.SelectedValue == "Pre-payment";
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PaymentTypeValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlPaymentType_ServerValidate]

    #region [BalancePayable_ServerValidate]
    protected void BalancePayable_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (e.Value != "0" && Convert.ToString(ddlPaymentType.Text.Trim()) == "Pre-payment")
            {
                e.IsValid = false;
            }
            else
            {
                e.IsValid = true;
            }

            //if (Convert.ToString(txtTotalBalancePayable.Text) != "")
            //{
            //    if (Convert.ToInt32(txtTotalBalancePayable.Text) < 0)
            //    {
            //        e.IsValid = false;
            //    }
            //    else
            //    {
            //        e.IsValid = true;
            //    }
            //}
            //else
            //{
            //    e.IsValid = true;
            //}
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PayDtServrValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [BalancePayable_ServerValidate]

    #region [DepositedAmount_ServerValidate]
    protected void DepositedAmount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (Convert.ToString(ddlPaymentType.Text) != "")
            {
                if (ddlPaymentType.Text == "Pre-payment")
                {
                    double depositedAmount = 0;
                    double balanceLoanAmount = 0;
                    double Interest = 0;
                    double totalChargesAmount = 0;

                    balanceLoanAmount = Convert.ToDouble(txtBalanceLoanAmount.Text);
                    depositedAmount = Convert.ToDouble(txtDepositedAmount.Text);
                    Interest = Convert.ToDouble(lblInterest.Text);
                    totalChargesAmount = Convert.ToDouble(lblTotalCharges.Text);

                    if (depositedAmount == (balanceLoanAmount + Interest + totalChargesAmount))
                    {
                        e.IsValid = true;
                    }
                    else
                    {
                        e.IsValid = false;
                    }
                }
                else
                {
                    e.IsValid = true;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PayDtServrValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [DepositedAmount_ServerValidate]

    #region [txtReferenceDate_ServerValidate]
    protected void txtReferenceDate_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
             DateTime dtStartDate = System.DateTime.Today;
            DateTime dtEndDate = System.DateTime.Today;

            strQuery = "select FinancialyearID, StartDate=convert(varchar,StartDate,103), EndDate=convert(varchar,EndDate,103) " +
                       "from tblFinancialyear where FinancialyearID='" + txtFYID.Text + "' and CompID=1";
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                dtStartDate = Convert.ToDateTime(ds.Tables[0].Rows[0][1]);
                dtEndDate = Convert.ToDateTime(ds.Tables[0].Rows[0][2]);
            }

            if (Convert.ToString(txtReferenceDate.Text.Trim()) != "")
            {
                if (Convert.ToDateTime(txtReferenceDate.Text) < dtStartDate || Convert.ToDateTime(txtReferenceDate.Text) > dtEndDate)
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PaymentDateValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [txtReferenceDate_ServerValidate]
    
    #region [Calculate Total Days]
    protected void CalculateTotalDays()
    {
        try
        {
            int totalDays = 0;
            int InterestPayableForPrepaymentDays = 0;
            DateTime PaymentDate = System.DateTime.Today;

            conn = new SqlConnection(strConnString);
            conn.Open();
            if (txtPaymentDate.Text.Trim() != "")
            {
                PaymentDate = Convert.ToDateTime(txtPaymentDate.Text);
                if (txtLastEMIPaidDate.Text.Trim() != "")
                {
                    DateTime LastEMIDateDate = Convert.ToDateTime(txtLastEMIPaidDate.Text);
                    totalDays = Convert.ToInt32(PaymentDate.Subtract(LastEMIDateDate).TotalDays);
                }
                else
                {
                    if (txtLoanIssuedDate.Text.Trim() != "")
                    {
                        DateTime LoanIssuedDate = Convert.ToDateTime(txtLoanIssuedDate.Text);
                        totalDays = Convert.ToInt32(PaymentDate.Subtract(LoanIssuedDate).TotalDays);
                    }
                }
            }

            if (ddlPaymentType.SelectedValue == "Monthly Payment")
            {
                if (txtPaymentDate.Text.Trim() == "" && totalDays == 0)
                {
                    txtTotalDays.Text = "";
                }
                else
                {
                    txtTotalDays.Text = Convert.ToString(totalDays);
                }
            }
            else if (ddlPaymentType.SelectedValue == "Pre-payment")
            {
                //fetching Interest Payable For Prepayment Days
                strQuery = "SELECT InterestPayableForPrepaymentDays FROM tblLoanParameterSetting";
                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        InterestPayableForPrepaymentDays = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                    }
                    else
                    {
                        InterestPayableForPrepaymentDays = 0;
                    }
                }
                else
                {
                    InterestPayableForPrepaymentDays = 0;
                }


                if (InterestPayableForPrepaymentDays > totalDays)
                {
                    if (totalDays > 0)
                    {
                        txtTotalDays.Text = Convert.ToString(InterestPayableForPrepaymentDays);
                    }
                    else
                    {
                        if (txtPaymentDate.Text.Trim() == "" && totalDays == 0)
                        {
                            txtTotalDays.Text = "";
                        }
                        else
                        {
                            txtTotalDays.Text = Convert.ToString(totalDays);
                        }
                    }
                }
                else
                {
                    if (txtPaymentDate.Text.Trim() == "" && totalDays == 0)
                    {
                        txtTotalDays.Text = "";
                    }
                    else
                    {
                        txtTotalDays.Text = Convert.ToString(totalDays);
                    }
                }
            }
            else
            {
                if (txtPaymentDate.Text.Trim() == "")
                {
                    txtTotalDays.Text = "";
                }
                else
                {
                    if (txtPaymentDate.Text.Trim() == "" && totalDays == 0)
                    {
                        txtTotalDays.Text = "";
                    }
                    else
                    {
                        txtTotalDays.Text = Convert.ToString(totalDays);
                    }
                }
            }

            if (txtTotalDays.Text.Trim() != "")
            {
                if (txtPaymentDate.Text.Trim() != "")
                {
                    int NoofDays = Convert.ToInt32(txtTotalDays.Text);

                    if (NoofDays <= 0)
                    {
                        lblMsg.Text = "Total Days must be greater than zero.";
                    }
                    else
                    {
                        lblMsg.Text = "";
                    }
                }
                else
                {
                    lblMsg.Text = "";
                }
            }
            else
            {
                lblMsg.Text = "";
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "TotalDaysAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Calculate Total Days]

    #region [Calculate EMI]
    protected void CalculateEMI()
    {
        try
        {
            double rateOfInterest = 0;
            double totalBalancePayable = 0;
            double balanceLoanAmount = 0;
            double depositedAmount = 0;
            double Interest = 0;
            double ActualInterest = 0;
            double PenalCharges = 0;
            double processingFee = 0;
            double processingIndemnityCharges = 0;
            double PawnTicketReIssueCharges = 0;
            int totalDays = 0;
            double EMIAmount = 0;

            conn = new SqlConnection(strConnString);
            conn.Open();

            if (txtBalanceLoanAmount.Text.Trim() != "" && txtTotalDays.Text.Trim() != "" && txtInterestRate.Text.Trim() != "")
            {
                //Penal Charges
                if (chkPenalCharges.Checked == true)
                {
                    PenalCharges = Convert.ToDouble(txtPenalChargesAmount.Text);
                }

                //Processing Charges
                if (chkProcessingFee.Checked == true)
                {
                    processingFee = Convert.ToDouble(txtProcessingChargesAmount.Text);
                }

                //Indemnity Fee/Charges
                if (chkIndemnityCharges.Checked == true)
                {
                    processingIndemnityCharges = Convert.ToDouble(txtIndemnityChargesAmount.Text);
                }

                //Pawn Ticket Re-Issue Charges
                if (chkPawnTicketReissue.Checked == true)
                {
                    PawnTicketReIssueCharges = Convert.ToDouble(txtPawnTicketReissueChargesAmount.Text);
                }

                //Balance Loan Amount
                balanceLoanAmount = Convert.ToDouble(txtBalanceLoanAmount.Text);
                //Interest Rate
                rateOfInterest = Convert.ToDouble(txtInterestRate.Text);
                //Total Days
                totalDays = Convert.ToInt32(txtTotalDays.Text);
                //Deposited Amount
                if (txtDepositedAmount.Text.Trim() != "")
                {
                    depositedAmount = Convert.ToDouble(txtDepositedAmount.Text);
                    depositedAmount = Math.Floor(depositedAmount);
                }
                else
                {
                    depositedAmount = 0;
                }

                //-------------------------------------Monthly Payment Amount
                if (ddlPaymentType.Text == "Monthly Payment")
                {
                    //Balance Interest Payable
                    double interestPayable = 0;
                    if (txtBalInterestPayable.Text.Trim() != "")
                    {
                        interestPayable = Convert.ToDouble(txtBalInterestPayable.Text);
                    }

                    //Balance Penal Charges Payable
                    double PenalChargesPayable = 0;
                    if (txtBalPenalChargesPayable.Text.Trim() != "")
                    {
                        PenalChargesPayable = Convert.ToDouble(txtBalPenalChargesPayable.Text);
                    }

                    //Balance Processing Fee Payable
                    double ProcessingFeePayable = 0;
                    if (txtBalProcessingChargesPayable.Text.Trim() != "")
                    {
                        ProcessingFeePayable = Convert.ToDouble(txtBalProcessingChargesPayable.Text);
                    }

                    //Balance Indemnity Fee Payable
                    double IndemnityFeePayable = 0;
                    if (txtBalIndemnityFeePayable.Text.Trim() != "")
                    {
                        IndemnityFeePayable = Convert.ToDouble(txtBalIndemnityFeePayable.Text);
                    }

                    //Balance Pawn Ticket Re-Issue Charges Payable
                    double PawnTicketReIssueChargesPayable = 0;
                    if (txtBalPawnTicketReIssueChargesPayable.Text.Trim() != "")
                    {
                        PawnTicketReIssueChargesPayable = Convert.ToDouble(txtBalPawnTicketReIssueChargesPayable.Text);
                    }

                    //Calculating Interest
                    Interest = (((balanceLoanAmount + interestPayable) * rateOfInterest / 100) / 30) * totalDays;
                    //Actual Interest Amount
                    ActualInterest = Math.Round(Interest, 2);
                    //display Actual Interest Amount
                    txtActualInterest.Text = Convert.ToString(ActualInterest);
                    //calculating Monthly Payment (Interest + Penal Charges + Processing Fee + Indemnity Fee + Pawn Ticket Re-Issue Charges
                    Interest = Math.Ceiling(Interest);
                    double TotalBalancePayableAmt = interestPayable + PenalChargesPayable + ProcessingFeePayable + IndemnityFeePayable + PawnTicketReIssueChargesPayable;
                    EMIAmount = Interest + PenalCharges + processingFee + processingIndemnityCharges + PawnTicketReIssueCharges + TotalBalancePayableAmt;
                    txtMonthlyPayment.Text = Convert.ToString(EMIAmount);
                    //display Balance Loan Amount + Interest
                    double BalLoanAmountPlusInterest = balanceLoanAmount + Interest;
                    txtBalanceLoanAmountPlusInterest.Text = Convert.ToString(BalLoanAmountPlusInterest);

                    //Current Total Interest + Charges Payable
                    double currentTotalIntChargesPayable = EMIAmount;

                    //Current Total Charges Payable
                    double currentTotalChargesPayable = EMIAmount - Interest;

                    //calculating Balance Payable Amount
                    if (depositedAmount != 0)
                    {
                        depositedAmount = Math.Ceiling(depositedAmount);
                        if (depositedAmount < currentTotalIntChargesPayable)
                        {
                            totalBalancePayable = (balanceLoanAmount);
                            totalBalancePayable = Math.Ceiling(totalBalancePayable);
                        }
                        else
                        {
                            totalBalancePayable = (balanceLoanAmount + currentTotalIntChargesPayable) - depositedAmount;
                            totalBalancePayable = Math.Ceiling(totalBalancePayable);
                        }
                        //Total Balance Payable
                        txtTotalBalancePayable.Text = Convert.ToString(totalBalancePayable);

                        //declaring variable
                        double TotaBalanceDepositedAmount = 0;

                        //Total Balance Penal Charges 
                        double BalPenalPayable = 0;
                        if (depositedAmount < PenalCharges)
                        {
                            BalPenalPayable = PenalCharges - depositedAmount;
                            BalPenalPayable = Math.Ceiling(BalPenalPayable);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalPenalPayable = 0;
                            BalPenalPayable = Math.Ceiling(BalPenalPayable);
                            TotaBalanceDepositedAmount = depositedAmount - PenalCharges;
                        }
                        txtBalancePenalCharges.Text = Convert.ToString(BalPenalPayable);

                        //Total Balance Processing Fee 
                        double BalProcessingFee = 0;
                        if (TotaBalanceDepositedAmount < processingFee)
                        {
                            BalProcessingFee = processingFee - TotaBalanceDepositedAmount;
                            BalProcessingFee = Math.Ceiling(BalProcessingFee);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalProcessingFee = 0;
                            BalProcessingFee = Math.Ceiling(BalProcessingFee);
                            TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - processingFee;
                        }
                        txtBalanceProcessingCharges.Text = Convert.ToString(BalProcessingFee);

                        //Total Balance Indemnity Fee 
                        double BalIndemnityFee = 0;
                        if (TotaBalanceDepositedAmount < processingIndemnityCharges)
                        {
                            BalIndemnityFee = processingIndemnityCharges - TotaBalanceDepositedAmount;
                            BalIndemnityFee = Math.Ceiling(BalIndemnityFee);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalIndemnityFee = 0;
                            BalIndemnityFee = Math.Ceiling(BalIndemnityFee);
                            TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - processingIndemnityCharges;
                        }
                        txtBalanceIndemnityFee.Text = Convert.ToString(BalIndemnityFee);

                        //Total Balance Pawn Ticket Re-Issue charges 
                        double BalPawnTicketReIssuecharges = 0;
                        if (TotaBalanceDepositedAmount < PawnTicketReIssueCharges)
                        {
                            BalPawnTicketReIssuecharges = PawnTicketReIssueCharges - TotaBalanceDepositedAmount;
                            BalPawnTicketReIssuecharges = Math.Ceiling(BalPawnTicketReIssuecharges);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalPawnTicketReIssuecharges = 0;
                            BalPawnTicketReIssuecharges = Math.Ceiling(BalPawnTicketReIssuecharges);
                            TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - PawnTicketReIssueCharges;
                        }
                        txtBalancePawnTicketReIssueCharges.Text = Convert.ToString(BalPawnTicketReIssuecharges);

                        //Total Balance Interest
                        double BalInterest = 0;
                        if (TotaBalanceDepositedAmount < Interest)
                        {
                            BalInterest = Interest - TotaBalanceDepositedAmount;
                            BalInterest = Math.Ceiling(BalInterest);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalInterest = 0;
                            BalInterest = Math.Ceiling(BalInterest);
                            TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - Interest;
                        }
                        txtBalanceInterest.Text = Convert.ToString(BalInterest);

                        //Total Balance Loan Amount (calculating)
                        double BalLoanAmount = 0;
                        if (TotaBalanceDepositedAmount < balanceLoanAmount)
                        {
                            BalLoanAmount = balanceLoanAmount - TotaBalanceDepositedAmount;
                            BalLoanAmount = Math.Ceiling(BalLoanAmount);
                            TotaBalanceDepositedAmount = 0;
                        }
                        else
                        {
                            BalLoanAmount = 0;
                            BalLoanAmount = Math.Ceiling(BalLoanAmount);
                            TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - balanceLoanAmount;
                        }
                        txtBalanceLoanAmountCalc.Text = Convert.ToString(BalLoanAmount);

                        //-------------Amount bifurcation
                        //label Deposited Amount
                        lblDepositedAmount.Text = Convert.ToString(depositedAmount);
                        //label Principal Amount
                        double PrincipalAmount = (depositedAmount - currentTotalIntChargesPayable); //currentTotalIntChargesPayable includes Interest + Total Charges
                        PrincipalAmount = Math.Ceiling(PrincipalAmount);
                        if (PrincipalAmount > 0)
                        {
                            lblPrincipal.Text = Convert.ToString(PrincipalAmount);
                        }
                        else
                        {
                            lblPrincipal.Text = "0";
                        }
                        //label Total charges
                        if (depositedAmount < currentTotalChargesPayable)
                        {
                            lblTotalCharges.Text = Convert.ToString(depositedAmount);
                        }
                        else
                        {
                            currentTotalChargesPayable = Math.Ceiling(currentTotalChargesPayable);
                            lblTotalCharges.Text = Convert.ToString(currentTotalChargesPayable);
                        }
                        //label Interest
                        if (Interest < (depositedAmount - currentTotalChargesPayable))
                        {
                            lblInterest.Text = Convert.ToString(Interest);
                        }
                        else
                        {
                            if ((depositedAmount - currentTotalChargesPayable) > 0)
                            {
                                lblInterest.Text = Convert.ToString(depositedAmount - currentTotalChargesPayable);
                            }
                            else
                            {
                                lblInterest.Text = "0";
                            }
                        }
                    }
                    else
                    {
                        txtTotalBalancePayable.Text = "0";
                        lblDepositedAmount.Text = "0";
                        lblPrincipal.Text = "0";
                        lblInterest.Text = "0";
                        lblTotalCharges.Text = "0";
                    }
                }

                //-------------------------------------Pre-payment Amount
                else if (ddlPaymentType.Text == "Pre-payment")
                {
                    //Balance Interest Payable
                    double interestPayable = 0;
                    if (txtBalInterestPayable.Text.Trim() != "")
                    {
                        interestPayable = Convert.ToDouble(txtBalInterestPayable.Text);
                    }

                    //Balance Penal Charges Payable
                    double PenalChargesPayable = 0;
                    if (txtBalPenalChargesPayable.Text.Trim() != "")
                    {
                        PenalChargesPayable = Convert.ToDouble(txtBalPenalChargesPayable.Text);
                    }

                    //Balance Processing Fee Payable
                    double ProcessingFeePayable = 0;
                    if (txtBalProcessingChargesPayable.Text.Trim() != "")
                    {
                        ProcessingFeePayable = Convert.ToDouble(txtBalProcessingChargesPayable.Text);
                    }

                    //Balance Indemnity Fee Payable
                    double IndemnityFeePayable = 0;
                    if (txtBalIndemnityFeePayable.Text.Trim() != "")
                    {
                        IndemnityFeePayable = Convert.ToDouble(txtBalIndemnityFeePayable.Text);
                    }

                    //Balance Pawn Ticket Re-Issue Charges Payable
                    double PawnTicketReIssueChargesPayable = 0;
                    if (txtBalPawnTicketReIssueChargesPayable.Text.Trim() != "")
                    {
                        PawnTicketReIssueChargesPayable = Convert.ToDouble(txtBalPawnTicketReIssueChargesPayable.Text);
                    }

                    //Calculating Interest
                    Interest = (((balanceLoanAmount + interestPayable) * rateOfInterest / 100) / 30) * totalDays;
                    //Actual Interest Amount
                    ActualInterest = Math.Round(Interest, 2);
                    //display Actual Interest Amount
                    txtActualInterest.Text = Convert.ToString(ActualInterest);
                    //calculating Monthly Payment (Interest + Penal Charges + Processing Fee + Indemnity Fee + Pawn Ticket Re-Issue Charges
                    Interest = Math.Ceiling(Interest);
                    double TotalBalancePayableAmt = interestPayable + PenalChargesPayable + ProcessingFeePayable + IndemnityFeePayable + PawnTicketReIssueChargesPayable;
                    EMIAmount = Interest + PenalCharges + processingFee + processingIndemnityCharges + PawnTicketReIssueCharges + TotalBalancePayableAmt;
                    txtMonthlyPayment.Text = Convert.ToString(EMIAmount);
                    //display Balance Loan Amount + Interest
                    double BalLoanAmountPlusInterest = balanceLoanAmount + Interest;
                    txtBalanceLoanAmountPlusInterest.Text = Convert.ToString(BalLoanAmountPlusInterest);

                    //calculating Deposited Amount
                    depositedAmount = Convert.ToDouble(balanceLoanAmount + EMIAmount);
                    depositedAmount = Math.Ceiling(depositedAmount);
                    txtDepositedAmount.Text = Convert.ToString(depositedAmount);
                    //calculating Balance Payable Amount
                    totalBalancePayable = (balanceLoanAmount + EMIAmount) - depositedAmount;
                    totalBalancePayable = Math.Ceiling(totalBalancePayable);
                    txtTotalBalancePayable.Text = Convert.ToString(totalBalancePayable);

                    //Current Total Interest + Charges Payable
                    double currentTotalIntChargesPayable = EMIAmount;

                    //Current Total Charges Payable
                    double currentTotalChargesPayable = EMIAmount - Interest;

                    //declaring variable
                    double TotaBalanceDepositedAmount = 0;

                    //Total Balance Penal Charges 
                    double BalPenalPayable = 0;
                    if (depositedAmount < PenalCharges)
                    {
                        BalPenalPayable = PenalCharges - depositedAmount;
                        BalPenalPayable = Math.Ceiling(BalPenalPayable);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalPenalPayable = 0;
                        BalPenalPayable = Math.Ceiling(BalPenalPayable);
                        TotaBalanceDepositedAmount = depositedAmount - PenalCharges;
                    }
                    txtBalancePenalCharges.Text = Convert.ToString(BalPenalPayable);

                    //Total Balance Processing Fee 
                    double BalProcessingFee = 0;
                    if (TotaBalanceDepositedAmount < processingFee)
                    {
                        BalProcessingFee = processingFee - TotaBalanceDepositedAmount;
                        BalProcessingFee = Math.Ceiling(BalProcessingFee);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalProcessingFee = 0;
                        BalProcessingFee = Math.Ceiling(BalProcessingFee);
                        TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - processingFee;
                    }
                    txtBalanceProcessingCharges.Text = Convert.ToString(BalProcessingFee);

                    //Total Balance Indemnity Fee 
                    double BalIndemnityFee = 0;
                    if (TotaBalanceDepositedAmount < processingIndemnityCharges)
                    {
                        BalIndemnityFee = processingIndemnityCharges - TotaBalanceDepositedAmount;
                        BalIndemnityFee = Math.Ceiling(BalIndemnityFee);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalIndemnityFee = 0;
                        BalIndemnityFee = Math.Ceiling(BalIndemnityFee);
                        TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - processingIndemnityCharges;
                    }
                    txtBalanceIndemnityFee.Text = Convert.ToString(BalIndemnityFee);

                    //Total Balance Pawn Ticket Re-Issue charges 
                    double BalPawnTicketReIssuecharges = 0;
                    if (TotaBalanceDepositedAmount < PawnTicketReIssueCharges)
                    {
                        BalPawnTicketReIssuecharges = PawnTicketReIssueCharges - TotaBalanceDepositedAmount;
                        BalPawnTicketReIssuecharges = Math.Ceiling(BalPawnTicketReIssuecharges);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalPawnTicketReIssuecharges = 0;
                        BalPawnTicketReIssuecharges = Math.Ceiling(BalPawnTicketReIssuecharges);
                        TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - PawnTicketReIssueCharges;
                    }
                    txtBalancePawnTicketReIssueCharges.Text = Convert.ToString(BalPawnTicketReIssuecharges);

                    //Total Balance Interest
                    double BalInterest = 0;
                    if (TotaBalanceDepositedAmount < Interest)
                    {
                        BalInterest = Interest - TotaBalanceDepositedAmount;
                        BalInterest = Math.Ceiling(BalInterest);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalInterest = 0;
                        BalInterest = Math.Ceiling(BalInterest);
                        TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - PawnTicketReIssueCharges;
                    }
                    txtBalanceInterest.Text = Convert.ToString(Interest);

                    //Total Balance Loan Amount (calculating)
                    double BalLoanAmount = 0;
                    if (TotaBalanceDepositedAmount < balanceLoanAmount)
                    {
                        BalLoanAmount = balanceLoanAmount - TotaBalanceDepositedAmount;
                        BalLoanAmount = Math.Ceiling(BalLoanAmount);
                        TotaBalanceDepositedAmount = 0;
                    }
                    else
                    {
                        BalLoanAmount = 0;
                        BalLoanAmount = Math.Ceiling(BalLoanAmount);
                        TotaBalanceDepositedAmount = TotaBalanceDepositedAmount - balanceLoanAmount;
                    }
                    txtBalanceLoanAmountCalc.Text = Convert.ToString(BalLoanAmount);

                    //-------------Amount bifurcation
                    //label Deposited Amount
                    lblDepositedAmount.Text = Convert.ToString(depositedAmount);
                    //label Principal Amount
                    double PrincipalAmount = (depositedAmount - currentTotalIntChargesPayable); //currentTotalIntChargesPayable includes Interest + Total Charges
                    PrincipalAmount = Math.Ceiling(PrincipalAmount);
                    if (PrincipalAmount > 0)
                    {
                        lblPrincipal.Text = Convert.ToString(PrincipalAmount);
                    }
                    else
                    {
                        lblPrincipal.Text = "0";
                    }
                    //label Total charges
                    if (depositedAmount < currentTotalChargesPayable)
                    {
                        lblTotalCharges.Text = Convert.ToString(depositedAmount);
                    }
                    else
                    {
                        currentTotalChargesPayable = Math.Ceiling(currentTotalChargesPayable);
                        lblTotalCharges.Text = Convert.ToString(currentTotalChargesPayable);
                    }
                    //label Interest
                    if (Interest < (depositedAmount - currentTotalChargesPayable))
                    {
                        lblInterest.Text = Convert.ToString(Interest);
                    }
                    else
                    {
                        if ((depositedAmount - currentTotalChargesPayable) > 0)
                        {
                            lblInterest.Text = Convert.ToString(depositedAmount - currentTotalChargesPayable);
                        }
                        else
                        {
                            lblInterest.Text = "0";
                        }
                    }

                    if (depositedAmount == 0)
                    {
                        txtTotalBalancePayable.Text = "0";
                        lblDepositedAmount.Text = "0";
                        lblPrincipal.Text = "0";
                        lblInterest.Text = "0";
                        lblTotalCharges.Text = "0";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CalcEMIAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Calculate EMI]

    #region [FillNarration]
    protected void FillNarration()
    {
        try
        {
            ddlNarration.DataSource = null;
            conn = new SqlConnection(strConnString);
            conn.Open();
            string query = "Select NarrationID, NarrationName from tblNarrationMaster ";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlNarration.DataSource = dt;
            ddlNarration.DataValueField = "NarrationID";
            ddlNarration.DataTextField = "NarrationName";
            ddlNarration.DataBind();
            ddlNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [FillNarration]

    #region [Validate Data]
    protected int ValidateData()
    {
        int Val = 0;
        try
        {
            DateTime dtToValid = System.DateTime.Today;
            DateTime PaymentDate;
            if (txtLastEMIPaidDate.Text.Trim() != "")
            {
                dtToValid = Convert.ToDateTime(txtLastEMIPaidDate.Text.Trim());
            }
            else
            {
                if (txtLoanIssuedDate.Text.Trim() != "")
                {
                    dtToValid = Convert.ToDateTime(txtLoanIssuedDate.Text.Trim());
                }
            }

            if (txtPaymentDate.Text.Trim() != "")
            {
                PaymentDate = Convert.ToDateTime(txtPaymentDate.Text.Trim());

                if (PaymentDate <= dtToValid)
                {
                    Val = 1;
                }
                else
                {
                    Val = 0;
                }
            }
            if (Val == 0)
            {
                if (Convert.ToString(txtTotalDays.Text).Trim() != "")
                {
                    int TotalDays = Convert.ToInt32(txtTotalDays.Text);
                    if (TotalDays < 1)
                    {
                        Val = 2;
                    }
                    else
                    {
                        Val = 0;
                    }
                }
                else
                {
                    Val = 2;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillCustDetailAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
        return Val;
    }
    #endregion [Validate Data]

    #region [Generate Payment Date]
    protected void GetPaymentDate()
    {
        try
        {
            //getting Today's Date
            strQuery = "select getdate() ";
            cmd = new SqlCommand(strQuery, conn);
            conn.Open();
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DateTime todayDate = Convert.ToDateTime(cmd.ExecuteScalar());
                txtReferenceDate.Text = todayDate.ToString("dd/MM/yyyy");
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PaymentDate_Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Generate Payment Date]

    #region [Fill Bank Cash Account]
    protected void FillBankCashAccount()
    {
        try
        {
            ddlCashAccount.DataSource = null;
            ddlCashAccount.Items.Clear();
            conn = new SqlConnection(strConnString);
            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlCashAccount.DataSource = dt;
            ddlCashAccount.DataValueField = "AccountID";
            ddlCashAccount.DataTextField = "Name";
            ddlCashAccount.DataBind();
            ddlCashAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Bank Cash Account]

    #region [chkPenalCharges_CheckedChanged]
    protected void chkPenalCharges_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (chkPenalCharges.Checked == true)
            {
                //enable
                ddlPenalCharges.Enabled = true;
                txtPenalChargesAmount.Enabled = true;
                ddlPenalChargesAccount.Enabled = true;
            }
            else
            {
                //disable
                ddlPenalCharges.Enabled = false;
                txtPenalChargesAmount.Enabled = false;
                ddlPenalChargesAccount.Enabled = false;
                //clear
                ddlPenalCharges.SelectedValue = "0";
                txtPenalChargesAmount.Text = "0";
                ddlPenalChargesAccount.SelectedValue = "0";
            }
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chkPenalChrgsAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [chkPenalCharges_CheckedChanged]

    #region [chkProcessingFee_CheckedChanged]
    protected void chkProcessingFee_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (chkProcessingFee.Checked == true)
            {
                //enable
                ddlProcessingCharges.Enabled = true;
                txtProcessingChargesAmount.Enabled = true;
                ddlProcessingChargesAccount.Enabled = true;
            }
            else
            {
                //disable
                ddlProcessingCharges.Enabled = false;
                txtProcessingChargesAmount.Enabled = false;
                ddlProcessingChargesAccount.Enabled = false;
                //clear
                ddlProcessingCharges.SelectedValue = "0";
                txtProcessingChargesAmount.Text = "0";
                ddlProcessingChargesAccount.SelectedValue = "0";
            }
           CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chkProcessingFeeChrgsAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [chkProcessingFee_CheckedChanged]

    #region [chkIndemnityCharges_CheckedChanged]
    protected void chkIndemnityCharges_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (chkIndemnityCharges.Checked == true)
            {
                //enable
                ddlIndemnityCharges.Enabled = true;
                txtIndemnityChargesAmount.Enabled = true;
                ddlIndemnityChargesAccount.Enabled = true;
            }
            else
            {
                //disable
                ddlIndemnityCharges.Enabled = false;
                txtIndemnityChargesAmount.Enabled = false;
                ddlIndemnityChargesAccount.Enabled = false;
                //clear
                ddlIndemnityCharges.SelectedValue = "0";
                txtIndemnityChargesAmount.Text = "0";
                ddlIndemnityChargesAccount.SelectedValue = "0";
            }
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chkIndemnityChrgsAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [chkIndemnityCharges_CheckedChanged]

    #region [chkPawnTicketReissue_CheckedChanged]
    protected void chkPawnTicketReissue_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            if (chkPawnTicketReissue.Checked == true)
            {
                //enable
                ddlPawnTicketReissueCharges.Enabled = true;
                txtPawnTicketReissueChargesAmount.Enabled = true;
                ddlPawnTicketReissueChargesAccount.Enabled = true;
            }
            else
            {
                //disable
                ddlPawnTicketReissueCharges.Enabled = false;
                txtPawnTicketReissueChargesAmount.Enabled = false;
                ddlPawnTicketReissueChargesAccount.Enabled = false;
                //clear
                ddlPawnTicketReissueCharges.SelectedValue = "0";
                txtPawnTicketReissueChargesAmount.Text = "0";
                ddlPawnTicketReissueChargesAccount.SelectedValue = "0";
            }
            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "chkPawnTicketReissueChrgsAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [chkPawnTicketReissue_CheckedChanged]

    #region [ddlPenalCharges_SelectedIndexChanged]
    protected void ddlPenalCharges_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlPenalCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlPenalCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }

                    if (noofdays > 0)
                    {
                        strChargeAmount = strCharges;
                    }
                    else
                    {
                        strChargeAmount = "0";
                    }
                }
                else
                {
                    double Interest = 0;
                    int noofdays = 0;
                    DateTime paymentDate = System.DateTime.Today;
                    DateTime interestDate = System.DateTime.Today;
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    if (txtActualInterest.Text.Trim() != "")
                    {
                        Interest = Convert.ToDouble(txtActualInterest.Text);
                        Interest = Math.Ceiling(Interest);
                    }
                    if (txtReferenceDate.Text.Trim() != "")
                    {
                        paymentDate = Convert.ToDateTime(txtReferenceDate.Text);
                    }
                    if (txtPaymentDate.Text.Trim() != "")
                    {
                        interestDate = Convert.ToDateTime(txtPaymentDate.Text);
                        if (paymentDate > interestDate)
                        {
                            noofdays = new DateTime(paymentDate.Subtract(interestDate).Ticks).Day - 1;
                        }
                        else
                        {
                            noofdays = 0;
                        }
                    }
                    //calculating charges
                    if (noofdays > 0)
                    {
                        ChargeAmount = Interest * ((ChargesPercent / 12) / 100);
                        decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                        strChargeAmount = Convert.ToString(dChargeAmount);
                    }
                }

                txtPenalChargesAmount.Text = strChargeAmount;
            }
            else
            {
                txtPenalChargesAmount.Text = "0";
            }

            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PenalChargesEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ddlPenalCharges_SelectedIndexChanged]

    #region [ddlProcessingCharges_SelectedIndexChanged]
    protected void ddlProcessingCharges_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlProcessingCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlProcessingCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    strChargeAmount = strCharges;
                }
                else
                {
                    double SanctionLoanAmount = Convert.ToDouble(txtTotalLoanAmount.Text);
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    ChargeAmount = SanctionLoanAmount * ((ChargesPercent / 12) / 100);
                    decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                    strChargeAmount = Convert.ToString(dChargeAmount);
                }

                txtProcessingChargesAmount.Text = strChargeAmount;
            }
            else
            {
                txtProcessingChargesAmount.Text = "0";
            }

            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ProcessingChrgEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ddlProcessingCharges_SelectedIndexChanged]

    #region [ddlIndemnityCharges_SelectedIndexChanged]
    protected void ddlIndemnityCharges_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlIndemnityCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlIndemnityCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    strChargeAmount = strCharges;
                }
                else
                {
                    double SanctionLoanAmount = Convert.ToDouble(txtTotalLoanAmount.Text);
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    ChargeAmount = SanctionLoanAmount * ((ChargesPercent / 12) / 100);
                    decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                    strChargeAmount = Convert.ToString(dChargeAmount);
                }

                txtIndemnityChargesAmount.Text = strChargeAmount;
            }
            else
            {
                txtIndemnityChargesAmount.Text = "0";
            }

            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "IndemnityChrgEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ddlIndemnityCharges_SelectedIndexChanged]

    #region [ddlPawnTicketReissueCharges_SelectedIndexChanged]
    protected void ddlPawnTicketReissueCharges_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //retrieving Charges Details
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = "0";
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlPawnTicketReissueCharges.SelectedValue != "0")
            {
                strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                            "FROM tbl_GLChargeMaster_Details " +
                            "WHERE CID=" + ddlPawnTicketReissueCharges.SelectedValue + " " +
                                    "AND '" + txtTotalLoanAmount.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    strID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    strCID = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    strLoanAmtFrom = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    strLoanAmtTo = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    strCharges = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    strChargeType = Convert.ToString(ds.Tables[0].Rows[0][5]);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for Total Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                }

                double ChargeAmount = 0;
                if (strChargeType == "Amount")
                {
                    strChargeAmount = strCharges;
                }
                else
                {
                    double SanctionLoanAmount = Convert.ToDouble(txtTotalLoanAmount.Text);
                    double ChargesPercent = Convert.ToDouble(strCharges);
                    ChargeAmount = SanctionLoanAmount * ((ChargesPercent / 12) / 100);
                    decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                    strChargeAmount = Convert.ToString(dChargeAmount);
                }

                txtPawnTicketReissueChargesAmount.Text = strChargeAmount;
            }
            else
            {
                txtPawnTicketReissueChargesAmount.Text = "0";
            }

            CalculateEMI();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "TicketReissueChrgEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ddlPawnTicketReissueCharges_SelectedIndexChanged]

    #region [Fill Penal Charges Drop Down List]
    protected void FillPenalChargesDropDown()
    {
        try
        {
            string strDate = string.Empty;
            conn = new SqlConnection(strConnString);
            ddlPenalCharges.DataSource = null;
            ddlPenalCharges.Items.Clear();

            if (txtReferenceDate.Text.Trim() != "")
            {
                strDate = Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd");
            }

            strQuery = "SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo " +
                       "WHERE Status='Active' " +
                       "AND ReferenceDate=(select MAX (ReferenceDate) from tbl_GLChargeMaster_BasicInfo " +
                                        "where CID=(select max(CID) from tbl_GLChargeMaster_BasicInfo " +
                                                                    "where ReferenceDate<='" + strDate + "')) " +
                       "ORDER BY ChargeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlPenalCharges.DataSource = dt;
            ddlPenalCharges.DataValueField = "CID";
            ddlPenalCharges.DataTextField = "ChargeName";
            ddlPenalCharges.DataBind();
            ddlPenalCharges.Items.Insert(0, new ListItem("--Select Penal Charges--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillPenalChrgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Penal Charges Drop Down List]

    #region [Fill Processing Charges Drop Down List]
    protected void FillProcessingChargesDropDown()
    {
        try
        {
            string strDate = string.Empty;
            conn = new SqlConnection(strConnString);
            ddlProcessingCharges.DataSource = null;
            ddlProcessingCharges.Items.Clear();

            if (txtReferenceDate.Text.Trim() != "")
            {
                strDate = Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd");
            }

            strQuery = "SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo " +
                        "WHERE Status='Active' " +
                        "AND ReferenceDate=(select MAX (ReferenceDate) from tbl_GLChargeMaster_BasicInfo " +
                                         "where CID=(select max(CID) from tbl_GLChargeMaster_BasicInfo " +
                                                                     "where ReferenceDate<='" + strDate + "')) " +
                        "ORDER BY ChargeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlProcessingCharges.DataSource = dt;
            ddlProcessingCharges.DataValueField = "CID";
            ddlProcessingCharges.DataTextField = "ChargeName";
            ddlProcessingCharges.DataBind();
            ddlProcessingCharges.Items.Insert(0, new ListItem("--Select Processing Charges--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillProcessingChrgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Processing Charges Drop Down List]

    #region [Fill Indemnity Charges Drop Down List]
    protected void FillIndemnityChargesDropDown()
    {
        try
        {
            string strDate = string.Empty;
            conn = new SqlConnection(strConnString);
            ddlIndemnityCharges.DataSource = null;
            ddlIndemnityCharges.Items.Clear();

            if (txtReferenceDate.Text.Trim() != "")
            {
                strDate = Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd");
            }

            strQuery = "SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo " +
                       "WHERE Status='Active' " +
                       "AND ReferenceDate=(select MAX (ReferenceDate) from tbl_GLChargeMaster_BasicInfo " +
                                        "where CID=(select max(CID) from tbl_GLChargeMaster_BasicInfo " +
                                                                    "where ReferenceDate<='" + strDate + "')) " +
                       "ORDER BY ChargeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlIndemnityCharges.DataSource = dt;
            ddlIndemnityCharges.DataValueField = "CID";
            ddlIndemnityCharges.DataTextField = "ChargeName";
            ddlIndemnityCharges.DataBind();
            ddlIndemnityCharges.Items.Insert(0, new ListItem("--Select Indemnity Charges--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillIndemnityChrgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Indemnity Charges Drop Down List]

    #region [Fill Pawn Ticket Re-Issue Charges Drop Down List]
    protected void FillPawnTicketReIssueChargesDropDown()
    {
        try
        {
            string strDate = string.Empty;
            conn = new SqlConnection(strConnString);
            ddlPawnTicketReissueCharges.DataSource = null;
            ddlPawnTicketReissueCharges.Items.Clear();

            if (txtReferenceDate.Text.Trim() != "")
            {
                strDate = Convert.ToDateTime(txtReferenceDate.Text).ToString("yyyy/MM/dd");
            }

            strQuery = "SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo " +
                       "WHERE Status='Active' " +
                       "AND ReferenceDate=(select MAX (ReferenceDate) from tbl_GLChargeMaster_BasicInfo " +
                                        "where CID=(select max(CID) from tbl_GLChargeMaster_BasicInfo " +
                                                                    "where ReferenceDate<='" + strDate + "')) " +
                       "ORDER BY ChargeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlPawnTicketReissueCharges.DataSource = dt;
            ddlPawnTicketReissueCharges.DataValueField = "CID";
            ddlPawnTicketReissueCharges.DataTextField = "ChargeName";
            ddlPawnTicketReissueCharges.DataBind();
            ddlPawnTicketReissueCharges.Items.Insert(0, new ListItem("--Select Re-Issue Charges--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillReIssueTicketChrgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Pawn Ticket Re-Issue Charges Drop Down List]

    #region [Fill Penal Charges Account Drop Down]
    protected void FillPenalChargesAccountDropDown()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            ddlPenalChargesAccount.DataSource = null;
            ddlPenalChargesAccount.Items.Clear();

            strQuery = "SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlPenalChargesAccount.DataSource = dt;
            ddlPenalChargesAccount.DataValueField = "AccountID";
            ddlPenalChargesAccount.DataTextField = "Name";
            ddlPenalChargesAccount.DataBind();
            ddlPenalChargesAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillPenalChrgAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Penal Charges Account Name Drop Down]

    #region [Fill Processing Charges Account Drop Down]
    protected void FillProcessingChargesAccountDropDown()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            ddlProcessingChargesAccount.DataSource = null;
            ddlProcessingChargesAccount.Items.Clear();

            strQuery = "SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlProcessingChargesAccount.DataSource = dt;
            ddlProcessingChargesAccount.DataValueField = "AccountID";
            ddlProcessingChargesAccount.DataTextField = "Name";
            ddlProcessingChargesAccount.DataBind();
            ddlProcessingChargesAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillProcessingChrgAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Processing Charges Account Drop Down]

    #region [Fill Indemnity Charges Account Drop Down]
    protected void FillIndemnityChargesAccountDropDown()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            ddlIndemnityChargesAccount.DataSource = null;
            ddlIndemnityChargesAccount.Items.Clear();

            strQuery = "SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlIndemnityChargesAccount.DataSource = dt;
            ddlIndemnityChargesAccount.DataValueField = "AccountID";
            ddlIndemnityChargesAccount.DataTextField = "Name";
            ddlIndemnityChargesAccount.DataBind();
            ddlIndemnityChargesAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillIndemnityChrgAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Indemnity Charges Account Drop Down]

    #region [Fill Pawn Ticket Reissue Charges Account Drop Down]
    protected void FillPawnTicketReissueChargesAccountDropDown()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            ddlPawnTicketReissueChargesAccount.DataSource = null;
            ddlPawnTicketReissueChargesAccount.Items.Clear();

            strQuery = "SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlPawnTicketReissueChargesAccount.DataSource = dt;
            ddlPawnTicketReissueChargesAccount.DataValueField = "AccountID";
            ddlPawnTicketReissueChargesAccount.DataTextField = "Name";
            ddlPawnTicketReissueChargesAccount.DataBind();
            ddlPawnTicketReissueChargesAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillPawnTicketReissueChrgAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Pawn Ticket Reissue Charges Account Drop Down]

    #region [Fill Interest Account Drop Down]
    protected void FillInterestAccountDropDown()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            ddlInterestAccount.DataSource = null;
            ddlInterestAccount.Items.Clear();

            strQuery = "SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' AND GPID=90 ORDER BY Name";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlInterestAccount.DataSource = dt;
            ddlInterestAccount.DataValueField = "AccountID";
            ddlInterestAccount.DataTextField = "Name";
            ddlInterestAccount.DataBind();
            ddlInterestAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillInterstAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Pawn Ticket Reissue Charges Account Drop Down]

    #region [ddlPenalChargesAccount_ServerValidate]
    protected void ddlPenalChargesAccount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (ddlPenalCharges.SelectedValue != "0")
            {
                if (e.Value == "0")
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PenalAccEventValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlPenalChargesAccount_ServerValidate]

    #region [ddlProcessingChargesAccount_ServerValidate]
    protected void ddlProcessingChargesAccount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (ddlProcessingCharges.SelectedValue != "0")
            {
                if (e.Value == "0")
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ProcessingAccEventValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlProcessingChargesAccount_ServerValidate]

    #region [ddlIndemnityChargesAccount_ServerValidate]
    protected void ddlIndemnityChargesAccount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (ddlIndemnityCharges.SelectedValue != "0")
            {
                if (e.Value == "0")
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "IndemnityAccEventValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlIndemnityChargesAccount_ServerValidate]

    #region [ddlPawnTicketReissueChargesAccount_ServerValidate]
    protected void ddlPawnTicketReissueChargesAccount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (ddlPawnTicketReissueCharges.SelectedValue != "0")
            {
                if (e.Value == "0")
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ReIssueAccEventValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlPawnTicketReissueChargesAccount_ServerValidate]

    #region [ddlInterestAccount_ServerValidate]
    protected void ddlInterestAccount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (lblInterest.Text.Trim() != "")
            {
                if (Convert.ToDouble(lblInterest.Text.Trim()) != 0)
                {
                    if (e.Value == "0")
                    {
                        e.IsValid = false;
                    }
                    else
                    {
                        e.IsValid = true;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "IntAccEventValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [ddlInterestAccount_ServerValidate]
}
