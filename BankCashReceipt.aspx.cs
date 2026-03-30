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

public partial class BankCashReceipt : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string strPostingID = string.Empty;
    string RefType = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    int FYearID = 0;
    int branchId = 0;

    public string loginDate;
    public string expressDate;
    #endregion

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();

                //BindDGVDtetails
                BindDGVDetails();

                //binding GridView
                BinddgvAccountDetails();

                //binding DropDownList Search By
                BindDDLSearchBy();

                //Generate Reference Date
                GetReferenceDate();

                //Fill BankCashCombo
                FillBankCashCombo();

                //Fill BankCombo()
                FillBankNameCombo();

                //Fill NarrationCombo
                FillNarrationCombo();

                BankDiv.Visible = false;
                CashDiv.Visible = false;

                //FillCredtDebitText();
                //txtGrandTotal.Text = "0";

                //getting Comp ID
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
                    conn.Close();
                }

                //getting FYear ID
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID); ;
                }
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
    #endregion

    #region Bind GridView
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "SELECT TBankCash_ReceiptDetails.BCRID, TBankCash_ReceiptDetails.ReferenceNo, " +
                                "ReferenceDate=Convert(varchar,TBankCash_ReceiptDetails.RefDate,103), " +
                                "TBankCash_ReceiptDetails.Amount, tblAccountMaster.Name as 'BankCashAccount', " +
                                "TBankCash_ReceiptDetails.RefDate " +
                        "FROM TBankCash_ReceiptDetails " +
                        "INNER JOIN tblAccountMaster " +
                                "ON tblAccountMaster.AccountID=TBankCash_ReceiptDetails.BankCashAccID " +
                        "WHERE (TBankCash_ReceiptDetails.RefType='BR' OR TBankCash_ReceiptDetails.RefType='CR') " +
                                "AND TBankCash_ReceiptDetails.FinanceYear=" + txtFYID.Text + " " +
                                "AND TBankCash_ReceiptDetails.Mode='GLBCR' " + 
                        "ORDER BY TBankCash_ReceiptDetails.BCRID ";

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
    #endregion

    #region Bind DropDownList-SearchBy
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("ReferenceNo");
            ddlSearchBy.Items.Add("ReferenceDate");
            ddlSearchBy.Items.Add("BankCashAccount");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [dgvDetails_PageIndexChanging]
    protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDetails.PageIndex = e.NewPageIndex;
            Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDetails_PageIndexChanging]

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
                dsDGV = GetRecords(conn, "GetAllRecords", "0");
                string ReferenceNo = Convert.ToString((_gridView.DataKeys[_selectedIndex].Value.ToString()));

                #region [Delete Record]
                if (_commandName == "DeleteRecord")
                {
                    bool datasaved = true;
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();
                    int existcount = 0;
                    int QueryResult = 0;

                    //checking whether record is present
                    strQuery = "select count(*) from TBankCash_ReceiptDetails where ReferenceNo='" + ReferenceNo + "'";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    existcount = Convert.ToInt32(cmd.ExecuteScalar());
                    
                    if (existcount > 0)
                    {
                        //deleting record from DB
                        int count = 0;
                        strQuery = "SELECT COUNT(*) FROM FLedgerMaster WHERE ReferenceNo='" + ReferenceNo + "' ";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            int AccID = 0;
                            double Debit = 0;
                            double Credit = 0;
                            DateTime Dt;

                            strQuery = "SELECT AccountID,Debit,Credit,RefDate FROM FLedgerMaster WHERE ReferenceNo='" + ReferenceNo + "' ";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                foreach (DataRow dr in ds.Tables[0].Rows)
                                {
                                    AccID = Convert.ToInt32(dr[0]);
                                    Debit = Convert.ToInt32(dr[1]);
                                    Credit = Convert.ToInt32(dr[2]);
                                    Dt = Convert.ToDateTime(dr[3]);

                                    if (datasaved == true)
                                    {
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AccID, Debit, Credit, transaction, conn);
                                    }
                                    else
                                    {
                                        datasaved = false;
                                        break;
                                    }
                                }

                                if (datasaved == true)
                                {
                                    deleteQuery = "DELETE FROM FLedgerMaster WHERE ReferenceNo='" + ReferenceNo + "'";
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
                            }
                        }

                        if (datasaved == true)
                        {
                            deleteQuery = "DELETE FROM TBankCash_PostingDetails WHERE ReferenceNo='" + ReferenceNo + "'";
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

                        if (datasaved == true)
                        {
                            deleteQuery = "DELETE FROM tbl_GLBankCashReceipt_Narration WHERE ReferenceNo='" + ReferenceNo + "'";
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

                        if (datasaved == true)
                        {
                            deleteQuery = "DELETE FROM TBankCash_ReceiptDetails WHERE ReferenceNo='" + ReferenceNo + "'";
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

                        int i = 0;
                        i = dgvDetails.SelectedIndex;
                        GridViewRow row = (GridViewRow)dgvDetails.Rows[i];
                        string RefNo = (row.Cells[0].FindControl("lblReferenceNo") as Label).Text;
                        string RefNum = "";
                        RefNum = RefNo.Substring(0, 2);

                        if (RefNum == "CR")
                        {
                            deleteQuery = "DELETE FROM tbl_BankCash_Receipt_DenominationDetails WHERE ReferenceNo='" + ReferenceNo + "'";
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

                        if (datasaved)
                        {
                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                            ClearData();
                            BindDGVDetails();
                            BinddgvAccountDetails();

                            //if the same record is deleted which is filled in the form.
                            if (txtPostingId.Text != "" && txtPostingId.Text != null)
                            {
                                // ClearData();
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

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                #endregion [Delete Record]

                #region [Update]
                if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ReferenceNo);
                    Session["BRID"] = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtBCRID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtRefNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]);
                    txtRefDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][2]).ToString("dd/MM/yyyy");
                    FillNarrationCombo();
                    ddlNarration.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][4]);
                    Session["BankCashAccID"] = Convert.ToString(dsDGV.Tables[0].Rows[0][6]);
                    string BankCashAccID = Convert.ToString(dsDGV.Tables[0].Rows[0][6]);
                    //Fill BankCashCombo
                    FillBankCashCombo();
                    ddlBankCash.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][6]);

                    string RefType = Convert.ToString(dsDGV.Tables[0].Rows[0][5]);
                    Session["RType"] = RefType;

                    if (RefType == "BR")
                    {
                        BankDiv.Visible = true;
                        CashDiv.Visible = false;
                        ddlBankName.SelectedValue = Convert.ToString(dsDGV.Tables[0].Rows[0][9]);
                        txtChqNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][7]);
                        txtChqDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][8]).ToString("dd/MM/yyyy");
                        ddlBankCash.Enabled = false;
                    }

                    if (RefType == "CR")
                    {
                        BankDiv.Visible = false;
                        CashDiv.Visible = true;

                        strQuery = "SELECT tbl_BankCash_Receipt_DenominationDetails.Thousand, tbl_BankCash_Receipt_DenominationDetails.FiveHundred, " +
                                            "tbl_BankCash_Receipt_DenominationDetails.Hundred ,tbl_BankCash_Receipt_DenominationDetails.Fifty, " +
                                            "tbl_BankCash_Receipt_DenominationDetails.Twenty, tbl_BankCash_Receipt_DenominationDetails.Ten, " +
                                            "tbl_BankCash_Receipt_DenominationDetails.Five, " +
                                            "tbl_BankCash_Receipt_DenominationDetails.Coins, " +
                                            "tbl_BankCash_Receipt_DenominationDetails.DID " +
                                    "FROM tbl_BankCash_Receipt_DenominationDetails " +
                                    "WHERE tbl_BankCash_Receipt_DenominationDetails.ReferenceNo='" + ReferenceNo + "'  ";

                        da = new SqlDataAdapter(strQuery, conn);
                        ds = new DataSet();
                        da.Fill(ds);

                        txtThousand.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtFiveHundred.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txthundred.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtFifty.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        txtTwenty.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
                        txtTen.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                        txtFive.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                        txtCoins.Text = Convert.ToString(ds.Tables[0].Rows[0][7]);
                        txtDID.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        DenominationCalculation();
                        ddlBankCash.Enabled = false;

                    }

                    //Posting Details
                    strQuery = "select FLedgerMaster.AccountID, FLedgerMaster.Debit, FLedgerMaster.Credit, " +
                                        "Name=(select case isnull(tblAccountMaster.Alies, '') when '' then tblAccountMaster.Name else tblAccountMaster.Name+ ' ('+tblAccountMaster.Alies+')' end), " +
                                        "TBankCash_PostingDetails.PostingID, '' as 'RefDate' " +
                               "from FLedgerMaster " +
                               "inner join tblAccountMaster " +
                                        "on tblAccountMaster.AccountID=FLedgerMaster.AccountID " +
                               "inner join TBankCash_PostingDetails " +
                                        "on FLedgerMaster.LedgerID=TBankCash_PostingDetails.LedgerID " +
                               "where TBankCash_PostingDetails.ReferenceNo='" + ReferenceNo + "'";

                    conn = new SqlConnection(strConnString);
                    da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Session["dtAccountDetails"] = dt;
                        dgvAccountDetails.DataSource = dt;
                        dgvAccountDetails.DataBind();
                    }
                    else
                    {
                        dt = new DataTable();
                        dt.Columns.Add("AccountID", typeof(String));
                        dt.Columns.Add("Name", typeof(String));
                        dt.Columns.Add("Debit", typeof(String));
                        dt.Columns.Add("Credit", typeof(String));
                        dt.Columns.Add("PostingID", typeof(String));
                        dt.Columns.Add("RefDate", typeof(String));
                        ShowNoResultFound(dt, dgvAccountDetails);
                    }

                    double number = 0;
                    double totalCr = 0;
                    double totalDr = 0;

                    foreach (GridViewRow row2 in dgvAccountDetails.Rows)
                    {
                        Label lblDebit = (Label)row2.Cells[2].FindControl("lblDebit");
                        Label lblCredit = (Label)row2.Cells[3].FindControl("lblCredit");

                        if (double.TryParse(lblCredit.Text, out number))
                        {
                            totalCr += number;
                        }
                        if (double.TryParse(lblDebit.Text, out number))
                        {
                            totalDr += number;
                        }
                    }
                    totalCr = Math.Round(totalCr, 3);
                    totalDr = Math.Round(totalDr, 3);

                    txtTotalCredit.Text = Convert.ToString(totalCr);
                    txtTotalDebit.Text = Convert.ToString(totalDr);

                    double a = Convert.ToDouble(txtTotalCredit.Text);
                    double b = Convert.ToDouble(txtTotalDebit.Text);
                    double c = a - b;

                    txtAmount.Text = Convert.ToString(c);
                    //FillCredtDebitText();

                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";
                }
                #endregion [Update]
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

    #region [BinddgvAccountDetails]
    protected void BinddgvAccountDetails()
    {
        try
        {
            strQuery = "select FLedgerMaster.AccountID, FLedgerMaster.Debit, FLedgerMaster.Credit, " +
                                "tblAccountMaster.Name, TBankCash_PostingDetails.PostingID, '' as 'RefDate' " +
                       "from FLedgerMaster " +
                        "inner join tblAccountMaster " +
                                "on tblAccountMaster.AccountID=FLedgerMaster.AccountID " +
                        "inner join TBankCash_PostingDetails " +
                                "on FLedgerMaster.LedgerID=TBankCash_PostingDetails.LedgerID " +
                        "where PostingID='" + txtPostingId.Text + "'";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            
            if (dt.Rows.Count > 0)
            {
                dgvAccountDetails.DataSource = dt;
                dgvAccountDetails.DataBind();
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("AccountID", typeof(String));
                dt.Columns.Add("Name", typeof(String));
                dt.Columns.Add("Debit", typeof(String));
                dt.Columns.Add("Credit", typeof(String));
                dt.Columns.Add("PostingID", typeof(String));
                dt.Columns.Add("RefDate", typeof(String));

                ShowNoResultFound(dt, dgvAccountDetails);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[BinddgvAccountDetails]

    #region [dgvAccountDetails_RowCommand]
    protected void dgvAccountDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "DeleteRecord")
            {
                if (dgvAccountDetails != null && dgvAccountDetails.Rows.Count > 0)
                {
                    GridView _gridView = (GridView)sender;
                    DataTable dt = (DataTable)Session["dtAccountDetails"];
                    int j = 0;
                    for (int i = 0; i < dgvAccountDetails.Rows.Count; i++)
                    {
                        GridViewRow row1 = dgvAccountDetails.Rows[i];
                        CheckBox chkBox = (CheckBox)row1.FindControl("chkSelect");

                        if (chkBox.Checked == true)
                        {
                            dt.Rows.RemoveAt(j);
                        }
                        else
                        {
                            j++;
                        }
                    }

                    if (dt.Rows.Count > 0)
                    {
                        Session["dtAccountDetails"] = dt;
                        dgvAccountDetails.DataSource = dt;
                        dgvAccountDetails.DataBind();
                    }
                    else
                    {
                        ShowNoResultFound(dt, dgvAccountDetails);
                    }
                }

                //Total Debit and Credit Addition
                double totalCr = 0;
                double totalDr = 0;

                foreach (GridViewRow row2 in dgvAccountDetails.Rows)
                {
                    Label lblDebit = (Label)row2.Cells[2].FindControl("lblDebit");
                    Label lblCredit = (Label)row2.Cells[3].FindControl("lblCredit");
                    double number;
                    
                    if (double.TryParse(lblCredit.Text, out number))
                    {
                        totalCr += number;
                    }
                    if (double.TryParse(lblDebit.Text, out number))
                    {
                        totalDr += number;
                    }
                }
                totalCr = Math.Round(totalCr, 3);
                totalDr = Math.Round(totalDr, 3);

                txtTotalCredit.Text = Convert.ToString(totalCr);
                txtTotalDebit.Text = Convert.ToString(totalDr);

                double a = Convert.ToDouble(txtTotalCredit.Text);
                double b = Convert.ToDouble(txtTotalDebit.Text);
                double c = a - b;

                txtAmount.Text = Convert.ToString(c);
                txtGrandTotal.Text = "0";
                //FillCredtDebitText();
                ClearDenomination();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "dgvAccountDetailsRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvAccountDetails_RowCommand]

    #region ShowNoResultFound
    protected void ShowNoResultFound(DataTable source, GridView gv)
    {
        try
        {
            // create a new blank row to the DataTable
            source.Rows.Add(source.NewRow());

            // Bind the DataTable which contain a blank row to the GridView
            gv.DataSource = source;
            gv.DataBind();

            // Get the total number of columns in the GridView to know what the Column Span should be
            int columnsCount = gv.Columns.Count;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowNoResultFoundAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [Search Function]
    protected void Search()
    {
        try
        {
            //Search Records
            DataTable dt = GetRecords(conn, "GetAllRecords", "0").Tables[0];
            DataView dv = new DataView(dt);
            string SearchExpression = null;
            string SearchBy = ddlSearchBy.Text;

            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                SearchExpression = string.Format("{0} '{1}%'", dgvDetails.SortExpression, txtSearch.Text);
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
    #endregion [Search Function]

    #region [FillBankCashCombo]
    protected void FillBankCashCombo()
    {
        try
        {
            ddlBankCash.DataSource = null;
            ddlBankCash.Items.Clear();
            string AccountID = Convert.ToString(Session["BankCashAccID"]);
            conn = new SqlConnection(strConnString);

            if (AccountID.Trim() == "")
            {
                strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                            "FROM tblAccountmaster " +
                            "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                            "AND Suspended='No' " +
                            "ORDER BY tblAccountMaster.Name";
            }
            else
            {
                strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                            "FROM tblAccountmaster " +
                            "WHERE ((GPID='11' OR GPID='70' OR GPID='71') " +
                            "AND Suspended='No')  " +
                            "OR tblAccountMaster.AccountID='" + AccountID + "' " +
                            "ORDER BY tblAccountMaster.Name";
            }

            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankCash.DataSource = dt;
            ddlBankCash.DataValueField = "AccountID";
            ddlBankCash.DataTextField = "Name";
            ddlBankCash.DataBind();
            ddlBankCash.Items.Insert(0, new ListItem("--Select Bank/Cash Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[FillBankCashCombo]

    #region [FillBankNameCombo]
    protected void FillBankNameCombo()
    {
        try
        {
            ddlBankName.DataSource = null;
            ddlBankName.Items.Clear();

            conn = new SqlConnection(strConnString);
            strQuery = "SELECT BankID, BankName, Branch FROM tblBankMaster ORDER BY BankName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankName.DataSource = dt;
            ddlBankName.DataValueField = "BankID";
            ddlBankName.DataTextField = "BankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("--Select Bank Name--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankNameComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[FillBankNameCombo]

    #region[ddlBankCash_SelectedIndexChanged]
    protected void ddlBankCash_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlBankCash.SelectedIndex == 0)
            {
                CashDiv.Visible = false;
                BankDiv.Visible = false;
                ClearDenomination();
                ClearData();
                //FillCredtDebitText();
                BinddgvAccountDetails();
            }

            int RefNo = 0;
            int ID = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();
            
            strQuery = "SELECT GPID FROM tblAccountMaster WHERE AccountID='" + ddlBankCash.SelectedValue + "' ";
            cmd = new SqlCommand(strQuery, conn);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
                Session["GPID"] = ID;
            }
            else
            {
                ID = 0;
            }

            if (ID == 70)
            {
                CashDiv.Visible = true;
                BankDiv.Visible = false;
                RefType = "CR";
                Session["RefType"] = RefType;
                ClearDenomination();
                //txtTotalCredit.Text = "";
                //txtTotalDebit.Text = "";
                //txtAmount.Text = "";
            }
            else if (ID == 11)
            {
                BankDiv.Visible = true;
                CashDiv.Visible = false;
                RefType = "BR";
                Session["RefType"] = RefType;
                ClearDenomination();
                //txtTotalCredit.Text = "";
                //txtTotalDebit.Text = "";
                //txtAmount.Text = "";
            }
            else if (ID == 71)
            {
                BankDiv.Visible = true;
                CashDiv.Visible = false;
                RefType = "BR";
                Session["RefType"] = RefType;
                //txtTotalCredit.Text = "";
                //txtTotalDebit.Text = "";
                //txtAmount.Text = "";
            }

            strQuery = "SELECT MAX(RefNo) FROM TBankCash_ReceiptDetails WHERE RefType='" + Session["RefType"] + "'";
            cmd = new SqlCommand(strQuery, conn);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                RefNo = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                RefNo = 0;
            }

            RefNo += 1;
            Session["RefNo"] = RefNo;

            txtRefNo.Text = Session["RefType"] + "/" + Session["RefNo"];
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ddlBankCashEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[ddlBankCash_SelectedIndexChanged]

    #region[FillNarrationCombo]
    protected void FillNarrationCombo()
    {
        try
        {
            ddlNarration.DataSource = null;
            ddlNarration.Items.Clear();

            conn = new SqlConnection(strConnString);
            strQuery = "SELECT NarrationID,NarrationName FROM tblNarrationMaster ORDER BY NarrationName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
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
            ClientScript.RegisterStartupScript(this.GetType(), "FillNarrAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[FillNarrationCombo]

    #region [Add Posting Details]
    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            bool valid = false;
            valid = ValidatePostingData();
            if (valid)
            {
                AjaxControlToolkit.ComboBox ddlAccName = (AjaxControlToolkit.ComboBox)dgvAccountDetails.FooterRow.FindControl("ddlAccountName");
                TextBox Debit = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtDebit");
                TextBox Credit = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtCredit");

                DataTable dtCurrentTable = new DataTable();
                DataRow dr = null;

                dtCurrentTable.Columns.Add("AccountID", typeof(String));
                dtCurrentTable.Columns.Add("Name", typeof(String));
                dtCurrentTable.Columns.Add("Debit", typeof(String));
                dtCurrentTable.Columns.Add("Credit", typeof(String));
                dtCurrentTable.Columns.Add("PostingID", typeof(String));
                dtCurrentTable.Columns.Add("RefDate", typeof(String));

                foreach (GridViewRow row in dgvAccountDetails.Rows)
                {
                    Label lblAccountID = (Label)row.Cells[0].FindControl("lblAccountID");
                    Label lblAccountName = (Label)row.Cells[1].FindControl("lblAccountName");
                    Label lblDebit = (Label)row.Cells[2].FindControl("lblDebit");
                    Label lblCredit = (Label)row.Cells[3].FindControl("lblCredit");
                    Label lblPostingID = (Label)row.Cells[4].FindControl("lblPostingID");
                    Label lblRefDate = (Label)row.Cells[5].FindControl("lblRefDate");

                    dr = dtCurrentTable.NewRow();

                    dr["AccountID"] = lblAccountID.Text;
                    dr["Name"] = lblAccountName.Text;
                    dr["Debit"] = lblDebit.Text;
                    dr["Credit"] = lblCredit.Text;
                    dr["PostingID"] = lblPostingID.Text;
                    dr["RefDate"] = lblRefDate.Text;

                    if (lblAccountName.Text != "")
                    {
                        dtCurrentTable.Rows.Add(dr);
                    }
                }

                dr = dtCurrentTable.NewRow();
                dr["AccountID"] = ddlAccName.SelectedValue;
                dr["Name"] = Convert.ToString(ddlAccName.SelectedItem.Text);
                if (Debit.Text == "")
                {
                    dr["Debit"] = Convert.ToString(0);
                }
                else
                {
                    dr["Debit"] = Convert.ToString(Debit.Text);
                }
                if (Credit.Text == "")
                {
                    dr["Credit"] = Convert.ToString(0);
                }
                else
                {
                    dr["Credit"] = Convert.ToString(Credit.Text);
                }
                
                dr["PostingID"] = string.Empty;
                dr["RefDate"] = txtRefDate.Text;
                dtCurrentTable.Rows.Add(dr);

                Session["dtAccountDetails"] = dtCurrentTable;
                dgvAccountDetails.DataSource = dtCurrentTable;
                dgvAccountDetails.DataBind();

                double totalCr = 0;
                double totalDr = 0;

                foreach (GridViewRow row in dgvAccountDetails.Rows)
                {
                    var Cr = row.FindControl("lblCredit") as Label;
                    var Dr = row.FindControl("lblDebit") as Label;
                    double number;
                   
                    if (double.TryParse(Cr.Text, out number))
                    {
                        totalCr += number;
                    }
                    if (double.TryParse(Dr.Text, out number))
                    {
                        totalDr += number;
                    }
                }
                totalCr = Math.Round(totalCr, 3);
                totalDr = Math.Round(totalDr, 3);

                txtTotalCredit.Text = Convert.ToString(totalCr);
                txtTotalDebit.Text = Convert.ToString(totalDr);

                TextBox Debit1 = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtDebit");
                TextBox Credit1 = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtCredit");

                Debit1.Text = Convert.ToString(0);
                Credit1.Text = Convert.ToString(0);

                double a = Convert.ToDouble(txtTotalCredit.Text);
                double b = Convert.ToDouble(txtTotalDebit.Text);
                double c = a - b;

                txtAmount.Text = Convert.ToString(c);
                txtGrandTotal.Text = "0";
                ClearDenomination();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Add Posting Details]

    #region[ValidatePostingData]
    protected bool ValidatePostingData()
    {
        bool valid = false;
        try
        {
            string dtgvRefDate = string.Empty;
            string dtRefDate = string.Empty;
            Label lblRefDate = (Label)dgvAccountDetails.Rows[0].Cells[5].FindControl("lblRefDate");
            TextBox Credit = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtCredit");
            TextBox Debit = (TextBox)dgvAccountDetails.FooterRow.FindControl("txtDebit");
            
            if (Debit.Text == "")
            {

                Session["Debit"] = "0";
            }
            else
            {
                Session["Debit"] = Debit.Text;
            }
            
            if (Credit.Text == "")
            {
                Session["Credit"] = "0";
            }
            else
            {
                Session["Credit"] = Credit.Text;

            }

            if (txtRefDate.Text.Trim() != "")
            {
                dtRefDate = Convert.ToDateTime(txtRefDate.Text).ToString("dd/MM/yyyy");
            }

            if (lblRefDate.Text.Trim() != "")
            {
                dtgvRefDate = Convert.ToDateTime(lblRefDate.Text).ToString("dd/MM/yyyy");
            }

            double s1 = Convert.ToDouble(Session["Debit"]);
            double s2 = Convert.ToDouble(Session["Credit"]);

            if ((s1 == 0) && (s2 == 0))
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Either Credit Or Debit Amount.');", true);
                Credit.Text = "0";
                Debit.Text = "0";
                valid = false;
                return valid;
            }
            else if (s1 > 0 && s2 > 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Either Credit Or Debit Amount.');", true);
                Credit.Text = "0";
                Debit.Text = "0";
                valid = false;
                return valid;
            }
            else if (dtgvRefDate != "")
            {
                if (dtRefDate != dtgvRefDate)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Pass new entry for different Reference Date.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                    return valid;
                }
            }
            else
            {
                valid = true;
                return valid;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "AddDataAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;
    }
    #endregion[ValidatePostingData]

    #region [txtRefDate_ServerValidate]
    protected void txtRefDate_ServerValidate(object sender, ServerValidateEventArgs e)
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

            if (Convert.ToString(txtRefDate.Text.Trim()) != "")
            {
                if (Convert.ToDateTime(txtRefDate.Text) < dtStartDate || Convert.ToDateTime(txtRefDate.Text) > dtEndDate)
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
            ClientScript.RegisterStartupScript(this.GetType(), "RefDateValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [txtRefDate_ServerValidate]

    #region [txtRefDate1_ServerValidate]
    protected void txtRefDate1_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            string dtgvRefDate = string.Empty;
            string dtRefDate = string.Empty;
            Label lblRefDate = (Label)dgvAccountDetails.Rows[0].Cells[5].FindControl("lblRefDate");

            if (lblRefDate.Text.Trim() != "")
            {
                dtgvRefDate = Convert.ToDateTime(lblRefDate.Text).ToString("dd/MM/yyyy");
            }

            if (txtRefDate.Text.Trim() != "")
            {
                dtRefDate = Convert.ToDateTime(txtRefDate.Text).ToString("dd/MM/yyyy");
            }

            if (dtgvRefDate != "")
            {
                if (dtRefDate != dtgvRefDate)
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
            ClientScript.RegisterStartupScript(this.GetType(), "RefDate1ValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [txtRefDate1_ServerValidate]

    #region GetRecords
    protected DataSet GetRecords(SqlConnection conn, string CommandName, string ReferenceNo)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "SELECT TBankCash_ReceiptDetails.BCRID, TBankCash_ReceiptDetails.ReferenceNo, " +
                                    "ReferenceDate=convert(varchar,TBankCash_ReceiptDetails.RefDate,103), " +
                                    "TBankCash_ReceiptDetails.Amount,tblAccountMaster.Name as 'BankCashAccount', " +
                                    "TBankCash_ReceiptDetails.RefDate " +
                            "FROM TBankCash_ReceiptDetails " +
                            "INNER JOIN tblAccountMaster " +
                                    "ON tblAccountMaster.AccountID=TBankCash_ReceiptDetails.BankCashAccID " +
                            "WHERE (TBankCash_ReceiptDetails.RefType='BR' OR TBankCash_ReceiptDetails.RefType='CR') " +
                                    "AND TBankCash_ReceiptDetails.FinanceYear=" + txtFYID.Text + " " +
                                    "AND TBankCash_ReceiptDetails.Mode='GLBCR' " + 
                            "ORDER BY TBankCash_ReceiptDetails.BCRID";

            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "SELECT TBankCash_ReceiptDetails.BCRID, TBankCash_ReceiptDetails.ReferenceNo, TBankCash_ReceiptDetails.RefDate as 'ReferenceDate', " +
                                    "TBankCash_ReceiptDetails.Amount, tbl_GLBankCashReceipt_Narration.NarrationID, " +
                                    "TBankCash_ReceiptDetails.RefType, TBankCash_ReceiptDetails.BankCashAccID, " +
                                    "TBankCash_ReceiptDetails.ChqNo, TBankCash_ReceiptDetails.ChqDate, TBankCash_ReceiptDetails.BankID, " +
                                    "TBankCash_ReceiptDetails.RefDate " +
                            "FROM TBankCash_ReceiptDetails " +
                            "INNER JOIN tbl_GLBankCashReceipt_Narration " +
                            "ON TBankCash_ReceiptDetails.ReferenceNo=tbl_GLBankCashReceipt_Narration.ReferenceNo " +
                            "WHERE (TBankCash_ReceiptDetails.RefType='BR' OR TBankCash_ReceiptDetails.RefType='CR') " +
                                    "AND TBankCash_ReceiptDetails.ReferenceNo='" + ReferenceNo + "'";
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
    #endregion

    #region ClearData
    protected void ClearData()
    {
        try
        {
            txtTotalCredit.Text = "0";
            txtTotalDebit.Text = "0";
            txtAmount.Text = "0";
            txtGrandTotal.Text = "0";
            txtFiveHundred.Text = "";
            txtFiv.Text = "";
            txthundred.Text = "";
            txtHun.Text = "";
            txtFiv.Text = "";
            txtFive.Text = "";
            txtTen.Text = "";
            txtTN.Text = "";
            txtChqDate.Text = "";
            txtChqNo.Text = "";
            txtRefNo.Text = "";
            txtCoins.Text = "";
            txtDID.Text = "";
            txtPostingId.Text = "";
            txtBCRID.Text = "";
            txtSearch.Text = "";
            txtRefDate.Text = "";
            txtPostingId.Text = "";
            lblErrorMsg.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
            BankDiv.Visible = false;
            CashDiv.Visible = false;
            Session["BankCashAccID"] = "";

            //getting Comp ID
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
                conn.Close();
            }

            //getting FYear ID
            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
                txtFYID.Text = Convert.ToString(FYearID); ;
            }

            //Fill Narration Combo
            FillNarrationCombo();
            //Generate Reference Date
            GetReferenceDate();
            //Fill BankCashCombo
            FillBankCashCombo();
            ddlBankCash.Enabled = true;

            //Fill BankCombo()
            FillBankNameCombo();

            BinddgvAccountDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Reset/Cancel
    protected void btnReset_Click(object sender, EventArgs e)
    {
        try
        {
            ClearData();
            ClearDenomination();
            ddlBankCash.Enabled = true;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Search Record
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [SaveData]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int Voucher = 0;
            bool datasaved = false;
            bool valid = false;
            valid = validatedata();
            
            if (valid)
            {
                if (Page.IsValid)
                {
                    // Creating instance of class CompanyWiseAccountClosing
                    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

                    #region [Save]
                    if (btnSave.Text == "Save")
                    {
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                        int QueryResult = 0;
                        int BCRID = 0;
                        int FYID = 0;
                        int BranchID = 0;

                        // Checks text box value can change to int or not.
                        double a = Convert.ToDouble(txtTotalCredit.Text);
                        double b = Convert.ToDouble(txtTotalDebit.Text);
                        double c = a - b;
                        txtAmount.Text = Convert.ToString(c);

                        // getting Max BCRID
                        strQuery = "SELECT MAX(BCRID) FROM TBankCash_ReceiptDetails";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            BCRID = 0;
                        }

                        BCRID += 1;
                        Session["BCRID"] = BCRID;

                        //Get Current Financial Year ID
                        if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                        {
                            FYID = Convert.ToInt32(Session["FYearID"]);
                        }

                        //GET current Branch ID
                        if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
                        {
                            BranchID = Convert.ToInt32(Session["branchId"]);
                        }

                        string Date = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");

                        //getting Voucher No
                        strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_ReceiptDetails";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            Voucher = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            Voucher = 0;
                        }

                        Voucher += 1;
                        Session["Voucher"] = Voucher;

                        int GPID = Convert.ToInt32(Session["GPID"]);
                        Label lblAccID = (Label)dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID");

                        if (GPID == 11 || GPID == 71)
                        {
                            string ChqDate = Convert.ToDateTime(txtChqDate.Text).ToString("yyyy/MM/dd");

                            //inserting data into table TBankCash_ReceiptDetails
                            insertQuery = "INSERT into TBankCash_ReceiptDetails values(" + Session["BCRID"] + ", " +
                                                    "'" + Session["RefType"] + "', '" + Session["RefNo"] + "', '" + txtRefNo.Text.Trim() + "', " +
                                                    "'" + Date + "', '" + Session["Voucher"] + "', '" + ddlBankCash.SelectedValue + "', '" + lblAccID.Text + "', " +
                                                    "'" + Convert.ToDouble(txtAmount.Text) + "', '" + txtChqNo.Text.Trim() + "', '" + ChqDate + "', '" + ddlBankName.SelectedValue + "', " +
                                                    "'" + ddlNarration.SelectedItem.Text + "', '', '0', '', 0,0,0,0,0, 'GLBCR', 'Cheque', '" + txtRefNo.Text.Trim() + "', " +
                                                    "'" + Session["FYearID"] + "', 0,0) ";

                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                        else if (GPID == 70)
                        {
                            //inserting data into table TBankCash_ReceiptDetails
                            insertQuery = "INSERT into TBankCash_ReceiptDetails values(" + Session["BCRID"] + ", " +
                                                    "'" + Session["RefType"] + "', '" + Session["RefNo"] + "', '" + txtRefNo.Text.Trim() + "', " +
                                                    "'" + Date + "', '" + Session["Voucher"] + "', '" + ddlBankCash.SelectedValue + "', '" + lblAccID.Text + "', " +
                                                    "'" + Convert.ToDouble(txtAmount.Text) + "', '', '', 0, " +
                                                    "'" + ddlNarration.SelectedItem.Text + "','', '0', '', 0,0,0,0,0, 'GLBCR', 'Cash',  '" + txtRefNo.Text.Trim() + "', " +
                                                    "'" + Session["FYearID"] + "', 0,0) ";
                            cmd = new SqlCommand(insertQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                            else
                            {
                                datasaved = false;
                            }

                            if (datasaved == true)
                            {
                                int DID = 0;
                                strQuery = "SELECT MAX(DID) FROM tbl_BankCash_Receipt_DenominationDetails";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    DID = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    DID = 0;
                                }

                                DID += 1;
                                Session["DID"] = DID;

                                //inserting data into table tbl_BankCash_Receipt_DenominationDetails
                                insertQuery = "INSERT into tbl_BankCash_Receipt_DenominationDetails values(" + Session["DID"] + ", " +
                                                        "'" + txtRefNo.Text.Trim() + "', '" + txtThousand.Text.Trim() + "', '" + txtFiveHundred.Text.Trim() + "', " +
                                                        "'" + txthundred.Text.Trim() + "', '" + txtFifty.Text.Trim() + "', '" + txtTwenty.Text.Trim() + "', '" + txtTen.Text.Trim() + "', " +
                                                        "'" + txtFive.Text.Trim() + "', '" + txtCoins.Text.Trim() + "', " +
                                                        "'" + Session["FYearID"] + "', '" + Session["branchId"] + "') ";

                                cmd = new SqlCommand(insertQuery, conn, transaction);
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

                        if (datasaved == true)
                        {
                            int ID = 0;
                            strQuery = "SELECT MAX(ID) FROM tbl_GLBankCashReceipt_Narration";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                ID = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                ID = 0;
                            }

                            ID += 1;

                            //inserting data into table tbl_GLBankCashReceipt_Narration
                            insertQuery = "INSERT into tbl_GLBankCashReceipt_Narration values(" + ID + ", " +
                                                    "'" + txtRefNo.Text.Trim() + "', " + ddlNarration.SelectedValue + ") ";

                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                        if (datasaved == true)
                        {
                            int LedgerID = 0;
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
                            Session["LedgerID1"] = LedgerID;
                            Label lblAccountID = (Label)dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID");
                            Session["AccID"] = Convert.ToString(lblAccountID.Text);

                            //inserting data into table FLedgerMaster
                            insertQuery = "INSERT into FLedgerMaster values(" + Session["LedgerID1"] + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RefType"] + "', " +
                                                    "'" + Date + "', '" + ddlBankCash.SelectedValue + "', " +
                                                    "'" + txtAmount.Text + "', '0','" + ddlNarration.SelectedItem.Text.Trim() + "', '" + lblAccountID.Text + "', " +
                                                    "'', '" + Session["FYearID"] + "') ";

                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                        //Saving Entry into CompanyWiseDayEndAccountClosing
                        if (datasaved == true)
                        {
                            int BankID = Convert.ToInt32(ddlBankCash.SelectedValue);
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), BankID, Convert.ToDouble(txtAmount.Text), 0, transaction, conn);
                        }

                        if (datasaved == true)
                        {
                            if (GPID == 11 || GPID == 71)
                            {
                                string ChqDate = Convert.ToDateTime(txtChqDate.Text).ToString("yyyy/MM/dd");
                                updateQuery = "UPDATE  TBankCash_ReceiptDetails SET " +
                                                        "RefDate='" + Date + "', " +
                                                        "LedgerID='" + Session["LedgerID1"] + "' " +
                                                "WHERE BCRID=" + Session["BCRID"] + "";

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
                            else if (GPID == 70)
                            {
                                updateQuery = "UPDATE  TBankCash_ReceiptDetails SET  " +
                                                    "RefDate='" + Date + "', " +
                                                    "LedgerID='" + Session["LedgerID1"] + "' " +
                                               "WHERE BCRID=" + Session["BCRID"] + "";

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

                        //Insert ContraEffect into LedgerTable
                        if (datasaved == true)
                        {
                            //getting MAX LedgerID
                            int LedgerID = 0;
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

                            Label lblAccountID = (Label)dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID");
                            Session["AcID"] = lblAccountID.Text;

                            insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RefType"] + "', " +
                                                    "'" + Date + "', '" + lblAccountID.Text + "', " +
                                                    "'0', '" + txtAmount.Text.Trim() + "','" + ddlNarration.SelectedItem.Text.Trim() + "', '" + ddlBankCash.SelectedValue + "', " +
                                                    "'', '" + Session["FYearID"] + "')";

                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                        if (datasaved == true)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(Session["AcID"]), 0, Convert.ToDouble(txtAmount.Text), transaction, conn);
                        }

                        if (datasaved == true)
                        {
                            if (dgvAccountDetails != null && dgvAccountDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvAccountDetails.Rows)
                                {
                                    if (datasaved == true)
                                    {
                                        //getting MAX ID
                                        int PostingID = 0;
                                        strQuery = "select max(PostingID) from TBankCash_PostingDetails";
                                        cmd = new SqlCommand(strQuery, conn, transaction);
                                        if (cmd.ExecuteScalar() != DBNull.Value)
                                        {
                                            PostingID = Convert.ToInt32(cmd.ExecuteScalar());
                                        }
                                        else
                                        {
                                            PostingID = 0;
                                        }

                                        PostingID += 1;
                                        Double dr = 0;
                                        Double cr = 0;

                                        Session["PostingID"] = PostingID;
                                        string lblAccountID = (row.Cells[0].FindControl("lblAccountID") as Label).Text;
                                        Session["lblAccountID"] = lblAccountID;
                                        string lblAccountName = (row.Cells[1].FindControl("lblAccountName") as Label).Text;
                                        string lblDebit = (row.Cells[2].FindControl("lblDebit") as Label).Text;
                                        string s1 = Convert.ToString(0);

                                        if (lblDebit != s1 || lblDebit != "")
                                        {
                                            dr = Convert.ToDouble(lblDebit);
                                        }
                                        else
                                        {
                                            dr = 0;
                                        }

                                        string lblCredit = (row.Cells[3].FindControl("lblCredit") as Label).Text;

                                        if (lblCredit != "" || lblCredit == s1)
                                        {
                                            cr = Convert.ToDouble(lblCredit);
                                        }
                                        else
                                        {
                                            cr = 0;
                                        }

                                        string lblPostingID = (row.Cells[4].FindControl("lblPostingID") as Label).Text;

                                        string DrCr = string.Empty;
                                        double Amount = 0;
                                        double Dval = 0;
                                        double Cval = 0;

                                        if (lblDebit != s1)
                                        {
                                            DrCr = "Dr";
                                            Amount = dr;
                                            Cval = 0;
                                            Dval = dr;
                                        }
                                        else if (lblCredit != s1)
                                        {
                                            DrCr = "Cr";
                                            Amount = cr;
                                            Dval = 0;
                                            Cval = cr;
                                        }

                                        //Insert Data Into TBankCash_PostingDetails Table
                                        insertQuery = "insert into TBankCash_PostingDetails values(" + Session["PostingID"] + ", " +
                                                                "'" + txtRefNo.Text.Trim() + "', '" + Convert.ToInt32(lblAccountID) + "', '" + DrCr + "', " +
                                                                "'" + Amount + "', '0', '" + Session["FYearID"] + "')";
                                        cmd = new SqlCommand(insertQuery, conn, transaction);
                                        QueryResult = cmd.ExecuteNonQuery();

                                        if (QueryResult > 0)
                                        {
                                            datasaved = true;
                                        }
                                        else
                                        {
                                            datasaved = false;
                                            break;
                                        }
                                        if (datasaved == true)
                                        {

                                            int LedgerID = 0;
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
                                            Session["LedgerID"] = LedgerID;

                                            insertQuery = "INSERT into FLedgerMaster values(" + Session["LedgerID"] + ", '" + txtRefNo.Text.Trim() + "','" + Session["RefType"] + "', " +
                                                                    "'" + Date + "', '" + Convert.ToInt32(lblAccountID) + "', " +
                                                                    "'" + Dval + "', '" + Cval + "', '" + ddlNarration.SelectedItem.Text.Trim() + "', '" + ddlBankCash.SelectedValue + "', " +
                                                                    "'', '" + Session["FYearID"] + "')";

                                            cmd = new SqlCommand(insertQuery, conn, transaction);
                                            QueryResult = cmd.ExecuteNonQuery();

                                            if (QueryResult > 0)
                                            {
                                                datasaved = true;
                                            }
                                            else
                                            {
                                                datasaved = false;
                                                break;
                                            }
                                        }

                                        if (datasaved == true)
                                        {
                                           datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(Session["lblAccountID"]), Dval, Cval, transaction, conn);
                                        }

                                        if (datasaved == true)
                                        {
                                            updateQuery = "UPDATE TBankCash_PostingDetails SET " +
                                                                    "LedgerID='" + Session["LedgerID"] + "' " +
                                                            "WHERE (PostingID='" + Session["PostingID"] + "' and ReferenceNo='" + txtRefNo.Text.Trim() + "')";

                                            cmd = new SqlCommand(updateQuery, conn, transaction);
                                            QueryResult = cmd.ExecuteNonQuery();

                                            if (QueryResult > 0)
                                            {
                                                datasaved = true;
                                            }
                                            else
                                            {
                                                datasaved = false;
                                                break;
                                            }
                                        }

                                        if (datasaved == true)
                                        {
                                            int LedgerID = 0;
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
                                            Session["LedgerID3"] = LedgerID;

                                            insertQuery = "INSERT into FLedgerMaster values(" + Session["LedgerID3"] + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RefType"] + "', " +
                                                                    "'" + Date + "', '" + ddlBankCash.SelectedValue + "', " +
                                                                    "'" + Cval + "', '" + Dval + "', '" + ddlNarration.SelectedItem.Text.Trim() + "', '" + Convert.ToInt32(lblAccountID) + "', " +
                                                                    "'', '" + Session["FYearID"] + "')";

                                            cmd = new SqlCommand(insertQuery, conn, transaction);
                                            QueryResult = cmd.ExecuteNonQuery();

                                            if (QueryResult > 0)
                                            {
                                                datasaved = true;
                                            }
                                            else
                                            {
                                                datasaved = false;
                                                break;
                                            }
                                        }

                                        if (datasaved == true)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(ddlBankCash.SelectedValue), Cval, Dval, transaction, conn);
                                        }
                                    }
                                }
                            }
                        }

                        if (datasaved == true)
                        {
                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Saved Successfully.');", true);
                            ClearData();
                            BindDGVDetails();
                            BinddgvAccountDetails();
                            //FillCredtDebitText();
                            ClearDenomination();
                        }
                        else
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Saved Successfully.');", true);
                        }
                    }
                        
                    #endregion [Save]

                    #region [Update]
                    else if (btnSave.Text == "Update")
                    {
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                        string GridID = string.Empty;
                        string PostingIDForUpdate = string.Empty;
                        if (dgvAccountDetails.Rows.Count > 0)
                        {
                            foreach (GridViewRow row in dgvAccountDetails.Rows)
                            {
                                GridID = (row.Cells[4].FindControl("lblPostingID") as Label).Text;
                                if (GridID != "")
                                {
                                    PostingIDForUpdate = PostingIDForUpdate + GridID.ToString() + ",";
                                }
                            }
                        }

                        int strLen = PostingIDForUpdate.Length;
                        if (PostingIDForUpdate.EndsWith(","))
                        {
                            PostingIDForUpdate = PostingIDForUpdate.Remove((strLen - 1), 1);
                        }

                        if (strLen > 0)
                        {
                            strQuery = "SELECT PostingID, LedgerID, AccID FROM TBankCash_PostingDetails " +
                                        "WHERE (ReferenceNo='" + txtRefNo.Text.Trim() + "' and PostingID NOT IN(" + PostingIDForUpdate + "))";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet dsp = new DataSet();
                            da.Fill(dsp);
                            Session["dsp"] = dsp;
                        }
                        else if (strLen == 0)
                        {
                            strQuery = "SELECT PostingID, LedgerID, AccID FROM TBankCash_PostingDetails " +
                                       "WHERE (ReferenceNo='" + txtRefNo.Text.Trim() + "')";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            cmd.CommandType = CommandType.Text;
                            SqlDataAdapter da = new SqlDataAdapter(cmd);
                            DataSet dsp = new DataSet();
                            da.Fill(dsp);
                            Session["dsp"] = dsp;
                        }

                        DataSet ds1 = (DataSet)Session["dsp"];
                        foreach (DataRow dr in ds1.Tables[0].Rows)
                        {
                            int PostingID = Convert.ToInt32(dr[0]);
                            int LedgerID = Convert.ToInt32(dr[1]);
                            int AccID = Convert.ToInt32(dr[2]);

                            deleteQuery = "DELETE FROM TBankCash_PostingDetails WHERE (ReferenceNo='" + txtRefNo.Text.Trim() + "' and PostingID=" + PostingID + ")";
                            cmd = new SqlCommand(deleteQuery, conn, transaction);
                            int QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                            else
                            {
                                datasaved = false;
                            }

                            if (datasaved == true)
                            {
                                strQuery = "SELECT AccountID, Debit, Credit, RefDate FROM FLedgerMaster " +
                                           "WHERE (ReferenceNo='" + txtRefNo.Text + "' and LedgerID=" + LedgerID + ")";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                cmd.CommandType = CommandType.Text;
                                SqlDataAdapter da = new SqlDataAdapter(cmd);
                                DataSet ds2 = new DataSet();
                                da.Fill(ds2);

                                foreach (DataRow dr1 in ds2.Tables[0].Rows)
                                {
                                    if (datasaved == true)
                                    {
                                        int AID = Convert.ToInt32(dr1[0]);
                                        int Debit = Convert.ToInt32(dr1[1]);
                                        int Credit = Convert.ToInt32(dr1[2]);
                                        DateTime RefDate = Convert.ToDateTime(dr1[3]);

                                        if (datasaved == true)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AID, Debit, Credit, transaction, conn);
                                        }
                                        else
                                        {
                                            datasaved = false;
                                            break;
                                        }
                                    }
                                }

                                deleteQuery = "DELETE FROM FLedgerMaster WHERE  (ReferenceNo='" + txtRefNo.Text + "' and LedgerID=" + LedgerID + ")";
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

                            if (datasaved)
                            {
                                LedgerID = LedgerID + 1;

                                strQuery = "SELECT AccountID, Debit, Credit, RefDate FROM FLedgerMaster " +
                                            "WHERE (ReferenceNo='" + txtRefNo.Text + "' and LedgerID=" + LedgerID + ")";

                                cmd = new SqlCommand(strQuery, conn, transaction);
                                cmd.CommandType = CommandType.Text;
                                SqlDataAdapter da1 = new SqlDataAdapter(cmd);
                                DataSet ds3 = new DataSet();
                                da1.Fill(ds3);

                                foreach (DataRow dr6 in ds3.Tables[0].Rows)
                                {
                                    if (datasaved == true)
                                    {
                                        int AID = Convert.ToInt32(dr6[0]);
                                        int Debit = Convert.ToInt32(dr6[1]);
                                        int Credit = Convert.ToInt32(dr6[2]);
                                        DateTime RefDate = Convert.ToDateTime(dr6[3]);

                                        if (datasaved == true)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AID, Debit, Credit, transaction, conn);
                                        }
                                        else
                                        {
                                            datasaved = false;
                                            break;
                                        }
                                    }
                                }

                                deleteQuery = "DELETE FROM FLedgerMaster WHERE  (ReferenceNo='" + txtRefNo.Text + "' and LedgerID=" + LedgerID + ")";
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
                        }


                        if (datasaved == true)
                        {
                            //updating table tbl_GLBankCashReceipt_Narration
                            updateQuery = "UPDATE tbl_GLBankCashReceipt_Narration SET " +
                                                    "NarrationID=" + ddlNarration.SelectedValue + " " +
                                            "WHERE ReferenceNo='" + txtRefNo.Text + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
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

                        int ID = 0;
                        strQuery = "SELECT GPID FROM tblAccountMaster WHERE AccountID='" + ddlBankCash.SelectedValue + "' ";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            ID = Convert.ToInt32(cmd.ExecuteScalar());
                            Session["GPID"] = ID;
                        }
                        else
                        {
                            ID = 0;
                        }
                        if (ID == 11 || ID == 71)
                        {
                            string RefDate = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");
                            string ChqDate = Convert.ToDateTime(txtChqDate.Text).ToString("yyyy/MM/dd");

                            updateQuery = "UPDATE TBankCash_ReceiptDetails SET " +
                                                    "RefDate='" + RefDate + "', " +
                                                    "ChqNo='" + txtChqNo.Text + "', " +
                                                    "Amount='" + txtAmount.Text + "', " +
                                                    "BankCashAccID='" + ddlBankCash.SelectedValue + "', " +
                                                    "ChqDate='" + ChqDate + "', BankID=" + ddlBankName.SelectedValue + ", " +
                                                    "Narration='" + ddlNarration.SelectedItem.Text + "' " +
                                            "WHERE BCRID=" + txtBCRID.Text + "";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
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

                        if (ID == 70)
                        {
                            string RefDate = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");
                            updateQuery = "UPDATE TBankCash_ReceiptDetails SET " +
                                                "RefDate='" + RefDate + "', " +
                                                "Amount='" + txtAmount.Text + "', " +
                                                "BankCashAccID='" + Convert.ToInt32(ddlBankCash.SelectedValue) + "', " +
                                                "Narration='" + ddlNarration.SelectedItem.Text + "' " +
                                            "WHERE BCRID='" + Convert.ToInt32(txtBCRID.Text.Trim()) + "'";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            int QueryResult = cmd.ExecuteNonQuery();

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
                                updateQuery = "UPDATE tbl_BankCash_Receipt_DenominationDetails SET " +
                                                        "Thousand='" + txtThousand.Text + "', " +
                                                        "FiveHundred='" + txtFiveHundred.Text + "', " +
                                                        "Hundred='" + txthundred.Text + "', " +
                                                        "Fifty='" + txtFifty.Text + "', Twenty='" + txtTwenty.Text + "', " +
                                                        "Ten='" + txtTen.Text + "', Five='" + txtFive.Text + "', " +
                                                        "Coins='" + txtCoins.Text + "', FinancialYear='" + Session["FYearID"] + "' " +
                                                "WHERE  DID='" + txtDID.Text + "' ";
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

                        int LID = 0;
                        if (datasaved)
                        {
                            strQuery = "SELECT LedgerID FROM TBankCash_ReceiptDetails " +
                                        "WHERE (ReferenceNo='" + txtRefNo.Text + "' and BCRID=" + txtBCRID.Text + ")";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                LID = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                datasaved = false;
                            }
                        }

                        if (datasaved)
                        {
                            strQuery = "SELECT AccountID, Debit, Credit, RefDate FROM FLedgerMaster " +
                                        "WHERE (LedgerID=" + LID + " and ReferenceNo='" + txtRefNo.Text + "' and RefType='" + Session["RType"] + "')";

                            cmd = new SqlCommand(strQuery, conn, transaction);
                            da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            int AccID = 0;
                            double Debit = 0;
                            double Credit = 0;
                            DateTime Dt;

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                AccID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                                Debit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                                Credit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                                Dt = Convert.ToDateTime(txtRefDate.Text);

                                string lblAccountID = (dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID") as Label).Text;
                                Session["lblAccountID"] = lblAccountID;
                                string lblAccountName = (dgvAccountDetails.Rows[0].Cells[1].FindControl("lblAccountName") as Label).Text;
                                string lblDebit = (dgvAccountDetails.Rows[0].Cells[2].FindControl("lblDebit") as Label).Text;
                                string lblCredit = (dgvAccountDetails.Rows[0].Cells[3].FindControl("lblCredit") as Label).Text;
                                if (datasaved)
                                {
                                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonEdit(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AccID, Debit, Credit, Convert.ToInt32(txtAmount.Text), 0, transaction, conn);
                                }
                                else
                                {
                                    datasaved = false;
                                }
                            }
                        }

                        if (datasaved)
                        {
                            string lblAccountID = (dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID") as Label).Text;
                            Session["lblAccountID"] = lblAccountID;
                            string lblAccountName = (dgvAccountDetails.Rows[0].Cells[1].FindControl("lblAccountName") as Label).Text;
                            string lblDebit = (dgvAccountDetails.Rows[0].Cells[2].FindControl("lblDebit") as Label).Text;
                            string lblCredit = (dgvAccountDetails.Rows[0].Cells[3].FindControl("lblCredit") as Label).Text;
                            string RefDate = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");

                            updateQuery = "UPDATE  FLedgerMaster SET " +
                                                    "RefDate='" + RefDate + "', AccountID=" + ddlBankCash.SelectedValue + ", Credit='0', " +
                                                    "Debit=" + txtAmount.Text + ", Narration='" + ddlNarration.SelectedItem.Text + "', " +
                                                    "ContraAccID=" + lblAccountID + ", FinanceYear='" + Session["FYearID"] + "' " +
                                            "WHERE (LedgerID=" + LID + " and ReferenceNo='" + txtRefNo.Text + "')";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
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

                        if (datasaved)
                        {
                            LID = LID + 1;
                            Session["LID"] = LID;

                            strQuery = "SELECT AccountID, Debit, Credit, RefDate FROM FLedgerMaster " +
                                        "WHERE (LedgerID=" + LID + " and ReferenceNo='" + txtRefNo.Text + "' and RefType='" + Session["RType"] + "')";

                            cmd = new SqlCommand(strQuery, conn, transaction);
                            da = new SqlDataAdapter(cmd);
                            DataSet ds = new DataSet();
                            da.Fill(ds);

                            int AccID = 0;
                            double Debit = 0;
                            double Credit = 0;
                            DateTime Dt;

                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                AccID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                                Debit = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                                Credit = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                                Dt = Convert.ToDateTime(txtRefDate.Text);

                                string lblAccountID = (dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID") as Label).Text;
                                Session["lblAccountID"] = lblAccountID;
                                string lblAccountName = (dgvAccountDetails.Rows[0].Cells[1].FindControl("lblAccountName") as Label).Text;
                                string lblDebit = (dgvAccountDetails.Rows[0].Cells[2].FindControl("lblDebit") as Label).Text;
                                string lblCredit = (dgvAccountDetails.Rows[0].Cells[3].FindControl("lblCredit") as Label).Text;

                                if (AccID == Convert.ToInt32(lblAccountID))
                                {
                                    if (datasaved)
                                    {
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonEdit(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AccID, Debit, Credit, 0, Convert.ToInt32(txtAmount.Text), transaction, conn);
                                    }
                                    else
                                    {
                                        datasaved = false;
                                    }
                                }

                                if (AccID != Convert.ToInt32(lblAccountID))
                                {
                                    if (datasaved == true)
                                    {
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), AccID, Debit, Credit, transaction, conn);
                                    }
                                    if (datasaved == true)
                                    {
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(lblAccountID), 0, Convert.ToDouble(txtAmount.Text), transaction, conn);
                                    }
                                    
                                }
                            }
                        }

                        if (datasaved)
                        {
                            string lblAccountID = (dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID") as Label).Text;
                            string RefDate = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");

                            updateQuery = "UPDATE  FLedgerMaster SET " +
                                                    "RefDate='" + RefDate + "', AccountID=" + lblAccountID + ", " +
                                                    "Credit=" + txtAmount.Text + ", Debit='0', Narration='" + ddlNarration.SelectedItem.Text + "', " +
                                                    "ContraAccID=" + ddlBankCash.SelectedValue + ", FinanceYear=" + Session["FYearID"] + " " +
                                            "WHERE (LedgerID=" + Convert.ToInt32(Session["LID"]) + " and ReferenceNo='" + txtRefNo.Text + "')";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
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

                        if (datasaved == true)
                        {
                            if (dgvAccountDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvAccountDetails.Rows)
                                {
                                    GridID = (row.Cells[4].FindControl("lblPostingID") as Label).Text;
                                    if (GridID == "")
                                    {
                                        //getting MAX ID
                                        int PostingID = 0;
                                        strQuery = "select max(PostingID) from TBankCash_PostingDetails";
                                        cmd = new SqlCommand(strQuery, conn, transaction);

                                        if (cmd.ExecuteScalar() != DBNull.Value)
                                        {
                                            PostingID = Convert.ToInt32(cmd.ExecuteScalar());
                                        }
                                        else
                                        {
                                            PostingID = 0;
                                        }

                                        PostingID += 1;
                                        Session["PID"] = PostingID;
                                        Double dr = 0;
                                        Double cr = 0;

                                        string lblAccountID = (row.Cells[0].FindControl("lblAccountID") as Label).Text;
                                        string lblAccountName = (row.Cells[1].FindControl("lblAccountName") as Label).Text;
                                        string lblDebit = (row.Cells[2].FindControl("lblDebit") as Label).Text;
                                        string s1 = Convert.ToString(0);

                                        if (lblDebit != s1 || lblDebit != "")
                                        {
                                            dr = Convert.ToDouble(lblDebit);
                                        }
                                        else
                                        {
                                            dr = 0;
                                        }

                                        string lblCredit = (row.Cells[3].FindControl("lblCredit") as Label).Text;

                                        if (lblCredit != "" || lblCredit == s1)
                                        {
                                            cr = Convert.ToDouble(lblCredit);
                                        }
                                        else
                                        {
                                            cr = 0;
                                        }

                                        string lblPostingID = (row.Cells[4].FindControl("lblPostingID") as Label).Text;
                                        string DrCr = string.Empty;
                                        double Amount = 0;
                                        double Dval = 0;
                                        double Cval = 0;

                                        if (lblDebit != s1)
                                        {
                                            DrCr = "Dr";
                                            Amount = dr;
                                            Cval = 0;
                                            Session["Cval"] = Cval;
                                            Dval = dr;
                                            Session["Dval"] = Dval;
                                        }
                                        else if (lblCredit != s1)
                                        {
                                            DrCr = "Cr";
                                            Amount = cr;
                                            Dval = 0;
                                            Cval = cr;
                                            Session["Cval"] = Cval;
                                            Session["Dval"] = Dval;
                                        }

                                        //Insert Data Into TBankCash_PostingDetails Table
                                        insertQuery = "insert into TBankCash_PostingDetails values(" + Session["PID"] + ", " +
                                                                "'" + txtRefNo.Text.Trim() + "', '" + Convert.ToInt32(lblAccountID) + "', '" + DrCr + "', " +
                                                                "'" + Amount + "', '0', '" + Session["FYearID"] + "')";
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
                                        if (datasaved == true)
                                        {
                                            int LedgerID = 0;
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
                                            Session["LedgerID1"] = LedgerID;
                                            string Date = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");

                                            insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RType"] + "',  " +
                                                                    "'" + Date + "', '" + Convert.ToInt32(lblAccountID) + "', " +
                                                                    "'" + Session["Dval"] + "', '" + Session["Cval"] + "', '" + ddlNarration.SelectedItem.Text.Trim() + "', " +
                                                                    "'" + ddlBankCash.SelectedValue + "', '', " +
                                                                    "'" + Session["FYearID"] + "')";

                                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                                        if (datasaved == true)
                                        {
                                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(lblAccountID), Convert.ToDouble(Session["Dval"]), Convert.ToDouble(Session["Cval"]), transaction, conn);
                                        }

                                        if (datasaved == true)
                                        {
                                            updateQuery = "UPDATE TBankCash_PostingDetails SET " +
                                                                    "LedgerID='" + Session["LedgerID1"] + "' " +
                                                            "WHERE (PostingID='" + Session["PID"] + "' AND ReferenceNo='" + txtRefNo.Text.Trim() + "')";

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

                                        //Insert ContraEffect into LedgerTable
                                        if (datasaved == true)
                                        {
                                            //getting MAX LedgerID
                                            int LedgerID = 0;

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

                                            string Date = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");
                                            Label lblAccountID1 = (Label)dgvAccountDetails.Rows[0].Cells[0].FindControl("lblAccountID");

                                            insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RType"] + "', " +
                                                                    "'" + Date + "', '" + ddlBankCash.SelectedValue + "', " +
                                                                    "'" + Session["Cval"] + "', '" + Session["Dval"] + "', '" + ddlNarration.SelectedItem.Text.Trim() + "', " +
                                                                    "'" + Convert.ToInt32(lblAccountID) + "', '', " +
                                                                    "'" + Session["FYearID"] + "')";

                                            cmd = new SqlCommand(insertQuery, conn, transaction);
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

                                        if (datasaved == true)
                                        {
                                            datasaved=objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(Session["FYearID"]), Convert.ToInt32(Session["CompID"]), Convert.ToInt32(Session["branchId"]), Convert.ToInt32(ddlBankCash.SelectedValue), Convert.ToDouble(Session["Cval"]), Convert.ToDouble(Session["Dval"]), transaction, conn);
                                        }
                                    }
                                }
                            }
                        }

                        if (datasaved == true)
                        {
                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Updated Successfully.');", true);

                            ClearData();
                            BindDGVDetails();
                            BinddgvAccountDetails();
                            //FillCredtDebitText();
                            ClearDenomination();
                        }
                        else
                        {
                            transaction.Rollback();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Updated Successfully.');", true);
                        }
                    }
                    #endregion[Update]
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.StackTrace + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [SaveData]

    #region [DenominationCalculation]
    protected void DenominationCalculation()
    {
        try
        {
            int Total = 0;
            if (txtThousand.Text != "")
            {
                txtTh.Text = Convert.ToString(Convert.ToDouble(txtThousand.Text) * 1000);
                Total = Total + (Convert.ToInt32(txtTh.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtFiveHundred.Text != "")
            {
                txtFiveHun.Text = Convert.ToString(Convert.ToDouble(txtFiveHundred.Text) * 500);
                Total = Total + (Convert.ToInt32(txtFiveHun.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txthundred.Text != "")
            {
                txtHun.Text = Convert.ToString(Convert.ToDouble(txthundred.Text) * 100);
                Total = Total + (Convert.ToInt32(txtHun.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtFifty.Text != "")
            {
                txtFift.Text = Convert.ToString(Convert.ToDouble(txtFifty.Text) * 50);
                Total = Total + (Convert.ToInt32(txtFift.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtTwenty.Text != "")
            {
                txtTwent.Text = Convert.ToString(Convert.ToDouble(txtTwenty.Text) * 20);
                Total = Total + (Convert.ToInt32(txtTwent.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtTen.Text != "")
            {
                txtTN.Text = Convert.ToString(Convert.ToDouble(txtTen.Text) * 10);
                Total = Total + (Convert.ToInt32(txtTN.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtFive.Text != "")
            {
                txtFiv.Text = Convert.ToString(Convert.ToDouble(txtFive.Text) * 5);
                Total = Total + (Convert.ToInt32(txtFiv.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            if (txtCoins.Text != "")
            {
                Total = Total + (Convert.ToInt32(txtCoins.Text));
                txtGrandTotal.Text = Convert.ToString(Total);
            }
            txtGrandTotal.Text = Convert.ToString(Total);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [DenominationCalculation]

    #region [txtThousand_TextChanged]
    protected void txtThousand_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtThousand.Text == "")
            {
                txtTh.Text = "0";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtThousand_TextChanged]

    #region [txtFifty_TextChanged]
    protected void txtFifty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtFifty.Text == "")
            {
                txtFift.Text = "0";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtFifty_TextChanged]

    #region [txtFive_TextChanged]
    protected void txtFive_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtFive.Text == "")
            {
                txtFiv.Text = "0";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtFive_TextChanged]

    #region [txtFiveHundred_TextChanged]
    protected void txtFiveHundred_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtFiveHundred.Text == "")
            {
                txtFiveHun.Text = "0";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtFiveHundred_TextChanged]

    #region [txtTwenty_TextChanged]
    protected void txtTwenty_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtTwenty.Text == "")
            {
                txtTwent.Text = "0";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtTwenty_TextChanged]

    #region [txthundred_TextChanged]
    protected void txthundred_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txthundred.Text == "")
            {
                txtHun.Text = "0";
            }

            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txthundred_TextChanged]

    #region [txtTen_TextChanged]
    protected void txtTen_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtTen.Text == "")
            {
                txtTN.Text = "0";
            }

            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtTen_TextChanged]

    #region [Validate Data]
    protected bool validatedata()
    {
        bool valid = false;
        try
        {
            DateTime RefDate = Convert.ToDateTime(txtRefDate.Text);
            conn = new SqlConnection(strConnString);

            strQuery = "Select StartDate, EndDate FROM tblFinancialyear WHERE FinancialyearID='" + Session["FYearID"] + "'";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            
            DateTime SDate = Convert.ToDateTime(ds.Tables[0].Rows[0][0]);
            DateTime EDate = Convert.ToDateTime(ds.Tables[0].Rows[0][1]);

            if (((RefDate > EDate)) || ((RefDate < SDate)))
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Reference Date Within Financial Year.');", true);
                valid = false;
                return valid;
            }

            if (ddlBankCash.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank/Cash Account.');", true);
                valid = false;
                return valid;
            }

            if (txtAmount.Text == "0" || txtAmount.Text == null)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Amount Received Must be greater than Zero .');", true);
                valid = false;
                return valid;
            }

            if (dgvAccountDetails != null && dgvAccountDetails.Rows.Count > 0)
            {
                int Count = 0;
                foreach (GridViewRow row in dgvAccountDetails.Rows)
                {
                    Label lblAccountID = (Label)row.FindControl("lblAccountID");
                    if (lblAccountID == null || lblAccountID.Text == "")
                    {
                        Count = Count + 1;
                    }
                }
                if (Count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Add Posting Details.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }

            double a = 0.0;
            double b = 0.0;
            
            if (txtTotalCredit.Text != "")
            {
                a = Convert.ToDouble(txtTotalCredit.Text);
            }
            if (txtTotalDebit.Text != "")
            {
                b = Convert.ToDouble(txtTotalDebit.Text);
            }

            double c = a - b;
            txtAmount.Text = Convert.ToString(c);

            int value;
            int Amnt = Convert.ToInt32(a);
            
            if (Int32.TryParse(txtTotalDebit.Text, out value))
            {
                if (value > Amnt)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Credit Should be greater than Debit.');", true);
                    txtAmount.Text = "";
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }

            conn = new SqlConnection(strConnString);
            conn.Open();

            int ID = 0;
            strQuery = "SELECT GPID FROM tblAccountMaster WHERE AccountID='" + ddlBankCash.SelectedValue + "' ";
            cmd = new SqlCommand(strQuery, conn);
            
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            if (ID == 15)
            {
                string S = Convert.ToString(0);
                int value1;
                int Amnt1 = Convert.ToInt32(txtAmount.Text);
                
                if (Int32.TryParse(txtGrandTotal.Text, out value1))
                {
                    if (value1 != Amnt1)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Grand Total should be equal to Amount Received.');", true);
                        ClearDenomination();
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        valid = true;
                    }
                }

                if (txtGrandTotal.Text == S || txtGrandTotal.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Add Cash Details.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }

            if (ID == 16 || ID == 12)
            {
                if ((txtChqNo.Text == "") || (txtChqNo.Text == null))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Cheque No.');", true);
                    valid = false;
                    return valid;
                }

                if ((txtChqDate.Text == "") || (txtChqDate.Text == null))
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Cheque Date.');", true);
                    valid = false;
                    return valid;
                }

                if ((txtChqDate.Text != "") || (txtChqDate.Text != null))
                {
                    DateTime ChqDate = Convert.ToDateTime(txtChqDate.Text);

                    if (((ChqDate > EDate)) || ((ChqDate < SDate)))
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Cheque Date Within Financial Year.');", true);
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        valid = true;
                    }
                }

                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }
            else
            {
                valid = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;
    }
    #endregion [Validate Data]

    #region[txtCoins_TextChanged]
    protected void txtCoins_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtCoins.Text == "")
            {
                txtCoins.Text = "";
            }
            DenominationCalculation();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[txtCoins_TextChanged]

    #region[txtGrandTotal_TextChanged]
    protected void txtGrandTotal_TextChanged()
    {
        try
        {
            int value;
            int Amnt = Convert.ToInt32(txtAmount.Text);
            if (Int32.TryParse(txtGrandTotal.Text, out value))
            {
                if (value != Amnt)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Denomination should be equal to Amount Received.');", true);
                    ClearDenomination();
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[txtGrandTotal_TextChanged]

    #region[ClearDenomination]
    protected void ClearDenomination()
    {
        try
        {
            txthundred.Text = "";
            txtHun.Text = "0";
            txtTh.Text = "0";
            txtThousand.Text = "";
            txtFiveHun.Text = "0";
            txtFiveHundred.Text = "";
            txtFift.Text = "0";
            txtFifty.Text = "";
            txtTwent.Text = "0";
            txtTwenty.Text = "";
            txtTen.Text = "";
            txtTN.Text = "0";
            txtGrandTotal.Text = "0";
            txtFiv.Text = "0";
            txtFive.Text = "";
            txtCoins.Text = "";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[ClearDenomination]

    #region[CreateNormalLedgerEntries]
    protected void CreateNormalLedgerEntries(string ReferenceNo, string Reftype, DateTime RefDate, int AppAccID, int DebitAmount, int CreditAmount, string Narration, int ContraAccID)
    {
        try
        {
            bool datasaved = true;
            int LedgerID = 0;
            
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
            Session["LedgerID1"] = LedgerID;
            string Date = Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd");

            insertQuery = "INSERT into FLedgerMaster values(" + LedgerID + ", '" + txtRefNo.Text.Trim() + "', '" + Session["RefType"] + "', " +
                                    "'" + Date + "','" + Convert.ToInt32(Session["lblAccountID"]) + "', " +
                                    "'" + Convert.ToInt32(Session["Dval"]) + "', '" + Convert.ToInt32(Session["Cval"]) + "', " +
                                    "'" + ddlNarration.SelectedItem.Text.Trim() + "', '" + ddlBankCash.SelectedValue + "', '', " +
                                    "'" + Session["FYearID"] + "') ";

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
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[CreateNormalLedgerEntries]

    #region [Generate Reference Date]
    protected void GetReferenceDate()
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
                txtRefDate.Text = todayDate.ToString("dd/MM/yyyy");
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "RefDate_Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Generate Reference Date]
}