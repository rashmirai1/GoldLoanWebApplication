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
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;
using System.Web.Services;
using System.Web.Script.Services;

public partial class GLReceiptForm : System.Web.UI.Page
{

    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    //creating instance of class "CompanyWiseAccountClosing"
    GlobalSettings gbl = new GlobalSettings();
    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

    //Declaring Variables.
    int result = 0;
    string flag = "";
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    bool datasaved = false;

    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = "";
    string RefId = "";

    int LedgerID = 0;
    string DJERefType = string.Empty;
    string DJEReferencNo = string.Empty;
    //Declaring Objects.
    SqlTransaction transactionGL, transactionAIM;
    SqlConnection conn, connAIM;
    SqlDataAdapter da, da2;
    DataSet ds;
    DataTable dt, dt2;
    SqlCommand cmd, cmdRcpt, cmdRoiRow;
    string InterestFromDate = string.Empty;
    string InterestToDate = string.Empty;
    string RvcCLI = string.Empty;
    string AdvInterestFromDate = string.Empty;
    string AdvInterestToDate = string.Empty;

    string loanAmount = string.Empty, OSIntAmt = string.Empty;

    #endregion [Declarations]

    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Click += new EventHandler(PropertybtnEdit_Click);
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnDelete.Click += new EventHandler(PropertybtnDelete_Click);
        Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
        Master.PropertygvGlobal.RowCommand += new GridViewCommandEventHandler(PropertygvGlobal_RowCommand);
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
        // Master.PropertybtnSave.OnClientClick = "return validate();";
    }


    protected void Page_Load(object sender, EventArgs e)
    {



        if (!IsPostBack)
        {
            try
            {

                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }
                else
                {

                    hdnuserid.Value = Session["userID"].ToString();
                    hdnfyid.Value = Session["FYearID"].ToString();
                    hdnbranchid.Value = Session["branchId"].ToString();
                    //txtRecvDate.Attributes.Add("readonly", "readonly");
                    txtGoldNo.Attributes.Add("readonly", "readonly");
                    txtLoanDate.Attributes.Add("readonly", "readonly");
                    txtScheme.Attributes.Add("readonly", "readonly");
                    txtLoanAmount.Attributes.Add("readonly", "readonly");
                    txtROI.Attributes.Add("readonly", "readonly");
                    //ddlReceipt.Items.Insert(0, new ListItem("--Select--", "0"));
                    pnlcashdetails.Enabled = false;
                    pnlchequedetails.Enabled = false;
                }
                // Master.PropertybtnSave.OnClientClick = "return validateNew();";
                Master.PropertybtnSave.OnClientClick = "return validate();";
                //gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel); //comment by priya for loading slow form
                BindChequeDetails();
                BindDenominationDetails();
                BindAccount();
                BindPrincipleAccount();
                //  BindTop100Account();
                //   BindNarration();
                BindTop100Narration();
                BindEmployee();
                BindReceiptBookNo();
                //Fill Bank/Cash Account
                FillBankCashAccount();
                GetCurrentDate();

                txtAdvInterest.Enabled = true;
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }
        //else
        //{ 
        //    gvChequeDetails = (GridView)Cache["gvChequeDetails"];
        //}

    }

    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {

        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    //-------Bind Current Date Time

    public void GetCurrentDate()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select convert(varchar(20),GETDATE(),103)'Cdate'";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtRecvDate.Text = dt.Rows[0]["Cdate"].ToString();
            DateTime rDate = Convert.ToDateTime(txtRecvDate.Text);
            txtAdvIntFrom.Text = txtAdvIntTo.Text = "";
            txtAdvIntFrom.Text = rDate.AddDays(1).ToShortDateString();

            DateTime endOfMonth = new DateTime(rDate.Year, rDate.Month, DateTime.DaysInMonth(rDate.Year, rDate.Month));
            txtAdvIntTo.Text = endOfMonth.ToShortDateString();

            txtIntToDate.Text = dt.Rows[0]["Cdate"].ToString();
        }

    }
    //-------Bind Cheque Details for effective one row at page load
    public void BindChequeDetails()
    {
        dt = new DataTable();
        dt.Columns.Add("ChequeId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("Chq_DD_NEFT");
        dt.Columns.Add("ChequeNo");
        dt.Columns.Add("ChequeDate");
        dt.Columns.Add("BankId");
        dt.Columns.Add("Amount");
        dt.Rows.Add("0", "1", "Cheque", "", "", "0");

        gvChequeDetails.DataSource = dt;
        gvChequeDetails.DataBind();


        //---Call function for binding bank
        BindBank();
    }

    //-------Add Cheque Details for effective one or more row on button click
    public void AddChequeDetails()
    {
        double chqtotal = 0;
        DataRow dr = null;
        dt = new DataTable();
        dt.Columns.Add("ChequeId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("Chq_DD_NEFT");
        dt.Columns.Add("ChequeNo");
        dt.Columns.Add("ChequeDate");
        dt.Columns.Add("BankId");
        dt.Columns.Add("Amount");
        for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
        {
            gvChequeDetails.SelectedIndex = i;

            HiddenField hdnchqid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqid");
            TextBox gvtxtChqSrno = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqSrno");
            TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
            TextBox gvtxtChqDate = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqDate");
            DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
            HiddenField hdnbankid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnbankid");
            TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");
            DropDownList gvddlChqDDNeft = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqDDNeft");
            HiddenField hdnchqddneft = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqddneft");

            dr = dt.NewRow();
            dr["ChequeId"] = hdnchqid.Value.Trim();
            dr["Serialno"] = gvtxtChqSrno.Text;
            dr["Chq_DD_NEFT"] = gvddlChqDDNeft.SelectedValue.Trim();
            dr["ChequeNo"] = gvtxtChqNo.Text;
            dr["ChequeDate"] = gvtxtChqDate.Text;
            dr["BankId"] = gvddlChqBank.SelectedValue.Trim();
            dr["Amount"] = gvtxtChqAmount.Text;
            dt.Rows.Add(dr);
            chqtotal = chqtotal + Convert.ToDouble(gvtxtChqAmount.Text);
        }

        dr = dt.NewRow();
        dr["ChequeId"] = "0";
        dr["Serialno"] = gvChequeDetails.Rows.Count + 1;
        dr["Chq_DD_NEFT"] = "Cheque";
        dr["ChequeNo"] = "";
        dr["ChequeDate"] = "";
        dr["BankId"] = "0";
        dr["Amount"] = "0";
        dt.Rows.Add(dr);

        gvChequeDetails.DataSource = dt;
        gvChequeDetails.DataBind();

        TextBox gvtxtChqTotal = (TextBox)gvChequeDetails.FooterRow.FindControl("gvtxtChqTotal");
        gvtxtChqTotal.Text = chqtotal.ToString();
        //---Call function for binding bank
        BindBank();
    }

    //-------Bind bank from AIM database
    public void BindBank()
    {

        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select BankName + ' ('+ Branch +')' BankName,BankID from  tblBankMaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
        {
            gvChequeDetails.SelectedIndex = i;
            DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
            HiddenField hdnbankid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnbankid");
            HiddenField hdnchqddneft = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqddneft");
            TextBox gvtxtChqSrno = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqSrno");
            DropDownList gvddlChqDDNeft = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqDDNeft");

            gvddlChqBank.DataSource = dt;
            gvddlChqBank.DataTextField = "BankName";
            gvddlChqBank.DataValueField = "BankID";
            gvddlChqBank.DataBind();
            gvddlChqBank.Items.Insert(0, new ListItem("--Select Bank--", "0"));
            gvddlChqBank.SelectedValue = hdnbankid.Value.Trim();
            gvddlChqDDNeft.SelectedValue = hdnchqddneft.Value.Trim();
            gvtxtChqSrno.Attributes.Add("readonly", "readonly");
        }
    }

    //-------Bind Denomination for effective one row at page load
    public void BindDenominationDetails()
    {

        dt = new DataTable();
        dt.Columns.Add("InOutID");
        dt.Columns.Add("RefNo");
        dt.Columns.Add("DenoId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("DenoRs");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Total");
        dt.Columns.Add("NoteNos");
        dt.Rows.Add("0", "0", "0", "1", "0", "0", "0", "");
        gvDenominationDetails.DataSource = dt;
        gvDenominationDetails.DataBind();
        for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
        {

            gvDenominationDetails.SelectedIndex = i;
            TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
            gvtxtDenoSrno.Attributes.Add("readonly", "readonly");
        }

    }

    //-------Add Denomination Details for effective one or more row on button click
    public void AddDenomination()
    {
        double denototal = 0;
        DataRow dr = null;
        dt = new DataTable();
        dt.Columns.Add("InOutID");
        dt.Columns.Add("RefNo");
        dt.Columns.Add("DenoId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("DenoRs");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Total");
        dt.Columns.Add("NoteNos");

        for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
        {
            gvDenominationDetails.SelectedIndex = i;
            HiddenField hdncashinoutid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdncashinoutid");
            HiddenField hdnrefno = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdnrefno");
            HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
            TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
            TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
            TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
            TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
            TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");

            dr = dt.NewRow();

            dr["InOutID"] = hdncashinoutid.Value;
            dr["RefNo"] = hdnrefno.Value;
            dr["DenoId"] = hdndenoid.Value.Trim();
            dr["Serialno"] = gvtxtDenoSrno.Text;
            dr["DenoRs"] = gvtxtDenoDescription.Text;
            dr["Quantity"] = gvtxtDenoNo.Text;
            dr["Total"] = gvtxtDenoTotal.Text;
            dr["NoteNos"] = gvtxtDenoNoteNos.Text;
            dt.Rows.Add(dr);
            denototal = denototal + Convert.ToDouble(gvtxtDenoTotal.Text);

        }

        dr = dt.NewRow();
        dr["InOutID"] = "0";
        dr["RefNo"] = "0";
        dr["DenoId"] = "0";
        dr["Serialno"] = gvDenominationDetails.Rows.Count + 1;
        dr["DenoRs"] = "0";
        dr["Quantity"] = "0";
        dr["Total"] = "0";
        dr["NoteNos"] = "";
        dt.Rows.Add(dr);

        gvDenominationDetails.DataSource = dt;
        gvDenominationDetails.DataBind();

        TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
        gvtxtDenoTotalAmt.Text = denototal.ToString();

        for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
        {
            gvDenominationDetails.SelectedIndex = i;

            TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
            gvtxtDenoSrno.Attributes.Add("readonly", "readonly");
        }
    }

    //-------Bind Account from AIM database
    public void BindAccount()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select Name,AccountID from tblAccountmaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        //ddlPrincipalCurrentAcHead.DataSource = dt;
        //ddlPrincipalCurrentAcHead.DataTextField = "Name";
        //ddlPrincipalCurrentAcHead.DataValueField = "AccountID";
        //ddlPrincipalCurrentAcHead.DataBind();
        //ddlPrincipalCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

        ////---------------------------------------------------------------------------------------
        //ddlInterestCurrentAcHead.DataSource = dt;
        //ddlInterestCurrentAcHead.DataTextField = "Name";
        //ddlInterestCurrentAcHead.DataValueField = "AccountID";
        //ddlInterestCurrentAcHead.DataBind();
        //ddlInterestCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

        ////-----------------------------------------------------------------------------------
        //ddlPenalCurrentAcHead.DataSource = dt;
        //ddlPenalCurrentAcHead.DataTextField = "Name";
        //ddlPenalCurrentAcHead.DataValueField = "AccountID";
        //ddlPenalCurrentAcHead.DataBind();
        //ddlPenalCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        ////-----------------------------------------------------------------------------------

        ////---------------------------------------------------------------------------------------
        //ddlChargesCurrentAcHead.DataSource = dt;
        //ddlChargesCurrentAcHead.DataTextField = "Name";
        //ddlChargesCurrentAcHead.DataValueField = "AccountID";
        //ddlChargesCurrentAcHead.DataBind();
        //ddlChargesCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        ////---------------------------------------------------------------------------------------

        ////-------------------------------------------------------------------------------------
        //ddlBankAcc.DataSource = dt;
        //ddlBankAcc.DataTextField = "Name";
        //ddlBankAcc.DataValueField = "AccountID";
        //ddlBankAcc.DataBind();
        //ddlBankAcc.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        ////---------------------------------------------------------------------------------------
        //ddlCashAcc.DataSource = dt;
        //ddlCashAcc.DataTextField = "Name";
        //ddlCashAcc.DataValueField = "AccountID";
        //ddlCashAcc.DataBind();
        //ddlCashAcc.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

        ////------added by priya---------
        //ddlAdvIntAcHead.DataSource = dt;
        //ddlAdvIntAcHead.DataTextField = "Name";
        //ddlAdvIntAcHead.DataValueField = "AccountID";
        //ddlAdvIntAcHead.DataBind();
        //ddlAdvIntAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
    }

    public void BindPrincipleAccount()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select top 100 Name,AccountID from tblAccountmaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        //ddlPrincipalCurrentAcHead.DataSource = dt;
        //ddlPrincipalCurrentAcHead.DataTextField = "Name";
        //ddlPrincipalCurrentAcHead.DataValueField = "AccountID";
        //ddlPrincipalCurrentAcHead.DataBind();
        //ddlPrincipalCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

    }

    public void BindTop100Account()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select top 100 Name,AccountID from tblAccountmaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        //---------------------------------------------------------------------------------------
        //ddlInterestCurrentAcHead.DataSource = dt;
        //ddlInterestCurrentAcHead.DataTextField = "Name";
        //ddlInterestCurrentAcHead.DataValueField = "AccountID";
        //ddlInterestCurrentAcHead.DataBind();
        //ddlInterestCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

        //-----------------------------------------------------------------------------------
        //ddlPenalCurrentAcHead.DataSource = dt;
        //ddlPenalCurrentAcHead.DataTextField = "Name";
        //ddlPenalCurrentAcHead.DataValueField = "AccountID";
        //ddlPenalCurrentAcHead.DataBind();
        //ddlPenalCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        //-----------------------------------------------------------------------------------

        //---------------------------------------------------------------------------------------
        //ddlChargesCurrentAcHead.DataSource = dt;
        //ddlChargesCurrentAcHead.DataTextField = "Name";
        //ddlChargesCurrentAcHead.DataValueField = "AccountID";
        //ddlChargesCurrentAcHead.DataBind();
        //ddlChargesCurrentAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        //---------------------------------------------------------------------------------------

        //-------------------------------------------------------------------------------------
        ddlBankAcc.DataSource = dt;
        ddlBankAcc.DataTextField = "Name";
        ddlBankAcc.DataValueField = "AccountID";
        ddlBankAcc.DataBind();
        ddlBankAcc.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
        //---------------------------------------------------------------------------------------
        ddlCashAcc.DataSource = dt;
        ddlCashAcc.DataTextField = "Name";
        ddlCashAcc.DataValueField = "AccountID";
        ddlCashAcc.DataBind();
        ddlCashAcc.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));

        //------added by priya---------
        ddlAdvIntAcHead.DataSource = dt;
        ddlAdvIntAcHead.DataTextField = "Name";
        ddlAdvIntAcHead.DataValueField = "AccountID";
        ddlAdvIntAcHead.DataBind();
        ddlAdvIntAcHead.Items.Insert(0, new ListItem("--Select A/c Head--", "0"));
    }

    #region [Fill Bank Cash Account]
    protected void FillBankCashAccount()
    {
        try
        {
            conn = new SqlConnection(strConnStringAIM);

            //For Cheque Details Dropdown
            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='11' OR GPID='71') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankAcc.DataSource = dt;
            ddlBankAcc.DataValueField = "AccountID";
            ddlBankAcc.DataTextField = "Name";
            ddlBankAcc.DataBind();
            ddlBankAcc.Items.Insert(0, new ListItem("--Select Account--", "0"));

            //For Cash Details Dropdown
            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='70') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da1 = new SqlDataAdapter(strQuery, conn);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            ddlCashAcc.DataSource = dt1;
            ddlCashAcc.DataValueField = "AccountID";
            ddlCashAcc.DataTextField = "Name";
            ddlCashAcc.DataBind();
            ddlCashAcc.Items.Insert(0, new ListItem("--Select Account--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Bank Cash Account]

    //-------Bind Narration from AIM database
    public void BindNarration()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select  NarrationID,NarrationName  From tblNarrationMaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlPrincipalCurrentNarration.DataSource = dt;
        ddlPrincipalCurrentNarration.DataTextField = "NarrationName";
        ddlPrincipalCurrentNarration.DataValueField = "NarrationID";
        ddlPrincipalCurrentNarration.DataBind();
        ddlPrincipalCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //-------------------------------------------------------------------------------------------
        ddlInterestCurrentNarration.DataSource = dt;
        ddlInterestCurrentNarration.DataTextField = "NarrationName";
        ddlInterestCurrentNarration.DataValueField = "NarrationID";
        ddlInterestCurrentNarration.DataBind();
        ddlInterestCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //--------------------------------------------------------------------------------------------
        ddlPenalCurrentNarration.DataSource = dt;
        ddlPenalCurrentNarration.DataTextField = "NarrationName";
        ddlPenalCurrentNarration.DataValueField = "NarrationID";
        ddlPenalCurrentNarration.DataBind();
        ddlPenalCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //---------------------------------------------------------------------------------------
        ddlChargesCurrentNarration.DataSource = dt;
        ddlChargesCurrentNarration.DataTextField = "NarrationName";
        ddlChargesCurrentNarration.DataValueField = "NarrationID";
        ddlChargesCurrentNarration.DataBind();
        ddlChargesCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //----added by priya-------------

        ddlAdvIntNarration.DataSource = dt;
        ddlAdvIntNarration.DataTextField = "NarrationName";
        ddlAdvIntNarration.DataValueField = "NarrationID";
        ddlAdvIntNarration.DataBind();
        ddlAdvIntNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

    }

    public void BindTop100Narration()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select top 100  NarrationID,NarrationName  From tblNarrationMaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlPrincipalCurrentNarration.DataSource = dt;
        ddlPrincipalCurrentNarration.DataTextField = "NarrationName";
        ddlPrincipalCurrentNarration.DataValueField = "NarrationID";
        ddlPrincipalCurrentNarration.DataBind();
        ddlPrincipalCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //-------------------------------------------------------------------------------------------
        ddlInterestCurrentNarration.DataSource = dt;
        ddlInterestCurrentNarration.DataTextField = "NarrationName";
        ddlInterestCurrentNarration.DataValueField = "NarrationID";
        ddlInterestCurrentNarration.DataBind();
        ddlInterestCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //--------------------------------------------------------------------------------------------
        ddlPenalCurrentNarration.DataSource = dt;
        ddlPenalCurrentNarration.DataTextField = "NarrationName";
        ddlPenalCurrentNarration.DataValueField = "NarrationID";
        ddlPenalCurrentNarration.DataBind();
        ddlPenalCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //---------------------------------------------------------------------------------------
        ddlChargesCurrentNarration.DataSource = dt;
        ddlChargesCurrentNarration.DataTextField = "NarrationName";
        ddlChargesCurrentNarration.DataValueField = "NarrationID";
        ddlChargesCurrentNarration.DataBind();
        ddlChargesCurrentNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

        //----added by priya-------------

        ddlAdvIntNarration.DataSource = dt;
        ddlAdvIntNarration.DataTextField = "NarrationName";
        ddlAdvIntNarration.DataValueField = "NarrationID";
        ddlAdvIntNarration.DataBind();
        ddlAdvIntNarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));

    }

    //-------Bind Employee from AIM database
    public void BindEmployee()
    {

        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster where status='Active'";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlCollectedBy.DataSource = dt;
        ddlCollectedBy.DataTextField = "EmpName";
        ddlCollectedBy.DataValueField = "EmployeeID";
        ddlCollectedBy.DataBind();
        ddlCollectedBy.Items.Insert(0, new ListItem("--Select Collected By--", "0"));
        //--------------------------------------------------------------------------------
        ddlCashier.DataSource = dt;
        ddlCashier.DataTextField = "EmpName";
        ddlCashier.DataValueField = "EmployeeID";
        ddlCashier.DataBind();
        ddlCashier.Items.Insert(0, new ListItem("--Select Cashier's Name--", "0"));

    }

    //-------Bind Receipt Book from AIM database
    public void BindReceiptBookNo()
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        //cmd.CommandText = "select distinct BookSerialNo From tblINV_ReceiptBook_Info order by BookSerialNo ";

        cmd.CommandText = "select distinct I.BookSerialNo From tblINV_ReceiptBook_Info I " +
                            "inner join tblINV_ReceiptBook_InOut_Details D " +
                            "on I.BookSerialNo=D.BookSerialNo where D.ReceiptNo not in" +
                            "(select ReceiptNo From tblINV_ReceiptBook_InOut_Details where Status='Out') order by I.BookSerialNo";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        //ddlReceiptBook.DataSource = dt;
        //ddlReceiptBook.DataTextField = "BookSerialNo";
        //ddlReceiptBook.DataValueField = "BookSerialNo";
        //ddlReceiptBook.DataBind();
        //ddlReceiptBook.Items.Insert(0, new ListItem("--Select--", "0"));
        //ddlReceiptBook.SelectedIndex = 0;
    }

    //--------Bind Pl Cases from AIM Database
    public void BindPlCases_Details(string plno)
    {
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select isnull((select SUM(case isnull(F.LoginID,0) when 0 then S.LoanAmount else 0 end) LoanAmount From TDisbursement_Appl_BasicInfo B " + "with (NOLOCK) inner join TSanctioning_Appl_SchemeDetails S  with (NOLOCK) on B.LoginID=S.LoginID inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID left join TBCR_FullFinalSettlement F with (NOLOCK) on B.LoginID=F.LoginID where B.CaseNo='" + plno + "' and P.Clearstatus='UC'),0)LoanAmount, isnull((select isnull(sum(S.EMI),0)EMI From TDisbursement_Appl_BasicInfo B with(NOLOCK) inner join TSanctioning_Appl_SchemeDetails S with (NOLOCK) on B.LoginID=S.LoginID inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID where B.CaseNo='" + plno + "' and Clearstatus='C'),0)DPEMI, (select isnull(sum(S.EMI),0)EMI From TDisbursement_Appl_BasicInfo B with(NOLOCK) inner join TSanctioning_Appl_SchemeDetails S  with (NOLOCK) on B.LoginID=S.LoginID inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID where B.CaseNo='" + plno + "' and  Clearstatus='UC')OSEMI, isnull((select SUM(Principal) from TDisbursement_Appl_BasicInfo B with(NOLOCK) inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID where B.CaseNo='" + plno + "' and P.Clearstatus='UC'),0)OSPrincipal, ((select isnull(sum(Amount),0)From TBankCash_ReceiptDetails with(NOLOCK) where ReceivedFrom=(SELECT top 1 a.AccountID FROM tblAccountMaster A inner join TDisbursement_Appl_BasicInfo B with(NOLOCK) on A.Alies=B.LoginID  AND A.GPID='67' WHERE B.CaseNo='" + plno + "') AND Mode='Regular' AND (RefType='BR' or RefType='CR')) - (select isnull(sum(D.AdjustedAmount),0)From TBankCash_ReceiptDetails B " + "with (NOLOCK) inner join TBankCashReceipt_AgainstBN_Adjustment D with (NOLOCK) on B.BCRID=D.BCRID where ReceivedFrom=(SELECT top 1 a.AccountID FROM tblAccountMaster A with (NOLOCK) inner join TDisbursement_Appl_BasicInfo B with (NOLOCK) on A.Alies=B.LoginID AND A.GPID='67' WHERE B.CaseNo='" + plno + "') AND Mode='Regular' AND (RefType='BR' or RefType='CR'))) OSDues, (select convert(varchar(12),max(P.Date),103)PLDate From TDisbursement_Appl_BasicInfo B with (NOLOCK) inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID where B.CaseNo='" + plno + "')LastPdcDate ";

        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            lblPlLoanAmt.Text = dt.Rows[0]["LoanAmount"].ToString();
            lblPlDPEMI.Text = dt.Rows[0]["DPEMI"].ToString();
            lblPlOsEMI.Text = dt.Rows[0]["OSEMI"].ToString();
            lblPlOsPrincipal.Text = dt.Rows[0]["OSPrincipal"].ToString();
            lblPlOsDues.Text = dt.Rows[0]["OSDues"].ToString();
            lblPlLastPdcDate.Text = dt.Rows[0]["LastPdcDate"].ToString();

        }
        else
        {
            lblPlLoanAmt.Text = "0";
            lblPlDPEMI.Text = "0";
            lblPlOsEMI.Text = "0";
            lblPlOsPrincipal.Text = "0";
            lblPlOsDues.Text = "0";
            lblPlLastPdcDate.Text = "";

        }
    }
    //public void BindPlCases_Details(string plno)
    //{
    //    connAIM = new SqlConnection(strConnStringAIM);
    //    cmd = new SqlCommand();
    //    cmd.Connection = connAIM;
    //    cmd.CommandText = "select isnull((select distinct case isnull(F.LoginID,0) when 0 then S.LoanAmount else 0 end LoanAmount From TDisbursement_Appl_BasicInfo B " + "with (NOLOCK)" +
    //                    "inner join TSanctioning_Appl_SchemeDetails S  with (NOLOCK) on B.LoginID=S.LoginID " +
    //                    "inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID " +
    //                    "left join  TBCR_FullFinalSettlement F with (NOLOCK) on B.LoginID=F.LoginID " +
    //                    "where B.CaseNo='" + plno + "' and P.Clearstatus='UC'),0)LoanAmount, " +

    //                    "isnull((select isnull(sum(S.EMI),0)EMI From TDisbursement_Appl_BasicInfo B " + "with (NOLOCK)" +
    //                    "inner join TSanctioning_Appl_SchemeDetails S with (NOLOCK) on B.LoginID=S.LoginID " +
    //                    "inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID " +
    //                    "where B.CaseNo='" + plno + "' and Clearstatus='C'),0)DPEMI, " +

    //                    "(select isnull(sum(S.EMI),0)EMI From TDisbursement_Appl_BasicInfo B " + "with (NOLOCK)" +
    //                    "inner join TSanctioning_Appl_SchemeDetails S  with (NOLOCK) on B.LoginID=S.LoginID " +
    //                    "inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID " +
    //                    "where B.CaseNo='" + plno + "' and  Clearstatus='UC')OSEMI," +

    //                    "isnull((select SUM(Principal) from TDisbursement_Appl_BasicInfo B " + "with (NOLOCK)" +
    //                    "inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID " +
    //                    "where B.CaseNo='" + plno + "' and P.Clearstatus='UC'),0)OSPrincipal, " +


    //                    "((select isnull(sum(Amount),0)From TBankCash_ReceiptDetails " + "with (NOLOCK)" +
    //                    "where ReceivedFrom=(SELECT top 1 a.AccountID FROM tblAccountMaster A " +
    //                    "inner join TDisbursement_Appl_BasicInfo B  with (NOLOCK) on A.Alies=B.LoginID  AND A.GPID='67' " +
    //                    "WHERE B.CaseNo='" + plno + "') " +
    //                    "AND Mode='Regular' AND (RefType='BR' or RefType='CR')) - " +

    //                    "(select isnull(sum(D.AdjustedAmount),0)From TBankCash_ReceiptDetails B " + "with (NOLOCK)" +
    //                    "inner join TBankCashReceipt_AgainstBN_Adjustment D with (NOLOCK) on B.BCRID=D.BCRID " +
    //                    "where ReceivedFrom=(SELECT top 1 a.AccountID FROM tblAccountMaster A with (NOLOCK) " +
    //                    "inner join TDisbursement_Appl_BasicInfo B with (NOLOCK) on A.Alies=B.LoginID AND A.GPID='67' " +
    //                    "WHERE B.CaseNo='" + plno + "') " +
    //                    "AND Mode='Regular' AND (RefType='BR' or RefType='CR'))) OSDues," +

    //                    "(select convert(varchar(12),max(P.Date),103)PLDate " +
    //                    "From TDisbursement_Appl_BasicInfo B with (NOLOCK) " +
    //                    "inner join TDisbursement_Appl_PDCDetails P with (NOLOCK) on B.LoginID=P.LoginID " +
    //                    "where B.CaseNo='" + plno + "')LastPdcDate ";




    //    da = new SqlDataAdapter(cmd);
    //    dt = new DataTable();
    //    da.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        lblPlLoanAmt.Text = dt.Rows[0]["LoanAmount"].ToString();
    //        lblPlDPEMI.Text = dt.Rows[0]["DPEMI"].ToString();
    //        lblPlOsEMI.Text = dt.Rows[0]["OSEMI"].ToString();
    //        lblPlOsPrincipal.Text = dt.Rows[0]["OSPrincipal"].ToString();
    //        lblPlOsDues.Text = dt.Rows[0]["OSDues"].ToString();
    //        lblPlLastPdcDate.Text = dt.Rows[0]["LastPdcDate"].ToString();

    //    }
    //    else
    //    {
    //        lblPlLoanAmt.Text = "0";
    //        lblPlDPEMI.Text = "0";
    //        lblPlOsEMI.Text = "0";
    //        lblPlOsPrincipal.Text = "0";
    //        lblPlOsDues.Text = "0";
    //        lblPlLastPdcDate.Text = "";

    //    }
    //}

    //-------Bind Gold Loan No 
    public void GoldNo_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;

        cmd.CommandText = "GlReceipt_GoldLoan_RTR";
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Master.PropertygvGlobal.DataSource = dt;
            Master.DataBind();
            hdnpopup.Value = "GoldLoan";
            hdnoperation.Value = "Save";
            Master.PropertympeGlobal.Show();
        }


    }

    //----------Serch Gold Loan
    public void GoldLoanNo_Search()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GlReceipt_GoldLoan_Search";
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.PropertygvGlobal.DataBind();
        Master.PropertympeGlobal.Show();
    }

    //-------Bind Gold Loan No for details
    public void GoldLoanDetails_RTR(string sdid)
    {
        //cmd.CommandText = "select Credit from FLedgerMaster where  Narration like 'Advance%' and AccountID=6829  ";
        //da = new SqlDataAdapter(cmd);
        //dt = new DataTable();
        //da.Fill(dt);
        //string InterestFromDate = string.Empty;
        //string InterestToDate = string.Empty;
        //string RvcCLI = string.Empty;
        //string AdvInterestFromDate = string.Empty;
        //string AdvInterestToDate = string.Empty;

        int neworold = 0;
        int RowRoiID = 1;
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_GoldLoanDetails_RTR_New";
        cmd.Parameters.AddWithValue("@RcvDate", gbl.ChangeDateMMddyyyy(txtRecvDate.Text));
        cmd.Parameters.AddWithValue("@KYCID", sdid);
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        double total = 0;
        //string ROI = "0";
        string ROI = dt.Rows[0]["ROI"].ToString();
        if (dt.Rows.Count > 0)
        {
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";

            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                neworold = Convert.ToInt32(cmd.ExecuteScalar());
            }

            //Added for to get ROI ROW ID on 3-10-2015
            int RcptID = 0;
            cmdRcpt = new SqlCommand();
            cmdRcpt.Connection = conn;
            cmdRcpt.Transaction = transactionGL;
            cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";
            if (cmdRcpt.ExecuteScalar() != DBNull.Value)
            {
                RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
            }
            //Check if Advance Interest is paid then Pass top 1 RowID
            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) > 0)
            {
                //Check if Half Interest is paid then Pass Max RowID
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }


            //Removed on 02-10-2015
            string LastDate;
            //if (dt.Rows[0]["LastInterestDate"].ToString() != "" && dt.Rows[0]["LastInterestDate"].ToString() != null)
            //{
            //    LastDate = dt.Rows[0]["LastInterestDate"].ToString();
            //}
            //else
            //{
            //    LastDate = dt.Rows[0]["LastReceiveDate"].ToString();
            //}
            LastDate = dt.Rows[0]["LastReceiveDate"].ToString();

            // LastDate = dt.Rows[0]["OSInterestToDate"].ToString();


            //string PrvInterestDate = string.Empty;
            //if (dt.Rows[0]["LastOSInt"].ToString() != "" && dt.Rows[0]["LastOSInt"].ToString() != null)
            //{
            //    PrvInterestDate = dt.Rows[0]["InterestFromDate"].ToString();
            //}

            //string RvcCLI = string.Empty;

            //if (dt.Rows[0]["RecvCLI"].ToString() != "" && dt.Rows[0]["RecvCLI"].ToString() != null)
            //{
            //    RvcCLI = dt.Rows[0]["RecvCLI"].ToString();
            //}



            if (dt.Rows[0]["InterestFromDate"].ToString() != "" && dt.Rows[0]["InterestFromDate"].ToString() != null)
            {
                InterestFromDate = dt.Rows[0]["InterestFromDate"].ToString();
            }
            else
            {
                InterestFromDate = System.DateTime.Today.ToShortDateString();
            }

            if (dt.Rows[0]["InterestToDate"].ToString() != "" && dt.Rows[0]["InterestToDate"].ToString() != null)
            {
                InterestToDate = dt.Rows[0]["InterestToDate"].ToString();
            }
            else
            {
                InterestToDate = System.DateTime.Today.ToShortDateString();
            }


            if (dt.Rows[0]["RecvInterest"].ToString() != "" && dt.Rows[0]["RecvInterest"].ToString() != null)
            {
                RvcCLI = dt.Rows[0]["RecvInterest"].ToString();
            }
            else
            {
                RvcCLI = "0";
            }


            if (dt.Rows[0]["AdvInterestFromDate"].ToString() != "" && dt.Rows[0]["AdvInterestFromDate"].ToString() != null)
            {
                AdvInterestFromDate = dt.Rows[0]["AdvInterestFromDate"].ToString();
            }
            else
            {
                AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
            }

            if (dt.Rows[0]["AdvInterestToDate"].ToString() != "" && dt.Rows[0]["AdvInterestToDate"].ToString() != null)
            {
                AdvInterestToDate = dt.Rows[0]["AdvInterestToDate"].ToString();
            }
            else
            {
                AdvInterestToDate = System.DateTime.Today.ToShortDateString();
            }

            if (Convert.ToDateTime(LastDate) <= Convert.ToDateTime(txtRecvDate.Text))
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GL_EmiCalculator_RTR";

                cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["CustLoanDate"].ToString()));
                //  cmd.Parameters.AddWithValue("@LoanAmount", dt.Rows[0]["LoanAmout"].ToString());


                loanAmount = dt.Rows[0]["LoanAmout"].ToString();
                if (loanAmount == "") { loanAmount = "0.00"; }

                OSIntAmt = dt.Rows[0]["OSIntAmt"].ToString();
                if (OSIntAmt == "") { OSIntAmt = "0.00"; }

                if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) >= 0)
                {
                    double AddPrvInt = Math.Round(Convert.ToDouble(loanAmount) + Convert.ToDouble(OSIntAmt));
                    cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                }
                //////////////

                /*  Change  important
                if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) >= 0)
                {
                    double AddPrvInt = Math.Round(Convert.ToDouble(dt.Rows[0]["LoanAmout"].ToString()) + Convert.ToDouble(dt.Rows[0]["OSIntAmt"].ToString()));
                    cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                }

                    Change important*/
                ///////////////
                else
                {
                    cmd.Parameters.AddWithValue("@LoanAmount", (dt.Rows[0]["LoanAmout"].ToString()));
                }

                cmd.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
                cmd.Parameters.AddWithValue("@NeworOld", neworold);

                cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

                cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[0]["OSIntAmt"].ToString());

                cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[0]["AdvInterestAmount"].ToString());

                cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
                cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtRecvDate.Text));
                cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                Session["InterestDetails"] = ds;

                DataTable dtt = new DataTable();
                dtt.Columns.Add("CLI");
                dtt.Columns.Add("ROI");


                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {

                    string InterestAmount = ds.Tables[0].Rows[i]["InterestAmount"].ToString();

                    if (InterestAmount == "") { InterestAmount = "0"; }
                    if (ds.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
                    {
                        total = total + Convert.ToDouble(ds.Tables[0].Rows[i]["InterestAmount"].ToString());
                        ROI = ds.Tables[0].Rows[i]["ROI"].ToString();
                    }
                    /*if (ds.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
                    {
                        total = total + Convert.ToDouble(ds.Tables[0].Rows[i]["InterestAmount"].ToString());
                        ROI = ds.Tables[0].Rows[i]["ROI"].ToString();
                    }*/
                }
            }
            else
            {
                //  conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                // cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";
                cmd.CommandText = " select ISNULL(OSInterest,0) from TGlReceipt_BasicDetails where GoldLoanNo ='" + dt.Rows[0]["GoldLoanNo"].ToString() + "' order by CreatedDate";

                DataTable dt1 = new DataTable();
                da = new SqlDataAdapter(cmd);
                da.Fill(dt1);

                for (int i = 0; i < dt1.Rows.Count; i++)
                {
                    total = Convert.ToDouble(dt1.Rows[i][0].ToString());
                }
            }
        }

        if (dt.Rows.Count > 0)
        {
            btnGlSearch.Enabled = true;
            txtGoldNo.Text = dt.Rows[0]["GoldLoanNo"].ToString();
            txtLoanDate.Text = dt.Rows[0]["LoanDate"].ToString();
            txtScheme.Text = dt.Rows[0]["SchemeName"].ToString();
            hdnmobileno.Value = dt.Rows[0]["MobileNo"].ToString();
            txtROI.Text = ROI;
            txtLoanAmount.Text = dt.Rows[0]["NetLoanAmtSanctioned"].ToString();
            lblPrincipalCurrent.Text = dt.Rows[0]["CLP"].ToString();
            //lblInterestCurrent.Text = Convert.ToString(Math.Round(total + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString())));
            lblInterestCurrent.Text = Convert.ToString(Math.Round(total));
            lblPenalCurrent.Text = dt.Rows[0]["CLPI"].ToString();
            lblChargesCurrent.Text = dt.Rows[0]["CLC"].ToString();
            //txtRcvTotal.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["CLP"].ToString()) + total + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString())));
            txtRcvTotal.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["CLP"].ToString()) + total));
            //lblCurrentTotal.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["CLP"].ToString()) + total + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString())));
            lblCurrentTotal.Text = Convert.ToString(Math.Round(Convert.ToDouble(dt.Rows[0]["CLP"].ToString()) + total));

            lblBalanceLoanEligibilityAmount.Text = dt.Rows[0]["BalanceLoanAmt"].ToString().Trim();
            hdnkycid.Value = dt.Rows[0]["KYCID"].ToString();
            hdnsdid.Value = dt.Rows[0]["SDID"].ToString();
            hdnloantype.Value = dt.Rows[0]["LoanType"].ToString();
            txtPrincipalCurrentAmt.Text = dt.Rows[0]["CLP"].ToString();
            //txtInterestCurrentAmt.Text = Convert.ToString(Math.Round(total + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString())));
            txtInterestCurrentAmt.Text = Convert.ToString(Math.Round(total));
            txtPenalCurrentAmt.Text = dt.Rows[0]["CLPI"].ToString();
            txtChargesCurrentAmt.Text = dt.Rows[0]["CLC"].ToString();
            hdnbcpid.Value = dt.Rows[0]["BCPID"].ToString();
            hdnplcaseno.Value = dt.Rows[0]["ExistingPLCaseNo"].ToString();

            //// Removed on 2-10-2015
            //if (!(System.Convert.IsDBNull(dt.Rows[0]["LastInterestDate"].ToString())) && dt.Rows[0]["LastInterestDate"].ToString() != "")
            //{
            //    txtIntFromDate.Text = dt.Rows[0]["LastInterestDate"].ToString(); //priya
            //    txtIntFromDate.ReadOnly = true;
            //    //btnImgIntFromCal.Visible = false;
            //}
            //else
            //{
            //    txtIntFromDate.Text = dt.Rows[0]["LastReceiveDate"].ToString();
            //    txtIntFromDate.ReadOnly = true;
            //    // btnImgIntFromCal.Visible = true;
            //}

            //commented on 3-11-2015 for set last interest to date
            // txtIntFromDate.Text = dt.Rows[0]["LastReceiveDate"].ToString();
            txtIntFromDate.Text = dt.Rows[0]["LoanInterestToDate"].ToString();
            txtIntFromDate.ReadOnly = true;
            txtIntToDate.Text = txtRecvDate.Text;

            if (!(System.Convert.IsDBNull(dt.Rows[0]["AdvInterestFromDate"].ToString())) && dt.Rows[0]["AdvInterestFromDate"].ToString() != "")
            {
                if (Convert.ToDateTime(dt.Rows[0]["AdvInterestToDate"].ToString()) <= Convert.ToDateTime(txtRecvDate.Text))
                {

                    DateTime rDate = Convert.ToDateTime(txtRecvDate.Text);
                    DateTime endOfMonth = new DateTime(rDate.Year, rDate.Month, DateTime.DaysInMonth(rDate.Year, rDate.Month));


                    if (rDate == endOfMonth)
                    {
                        txtAdvIntFrom.Text = txtAdvIntTo.Text = rDate.ToShortDateString();
                    }
                    else
                    {
                        txtAdvIntFrom.Text = txtAdvIntTo.Text = "";
                        txtAdvIntFrom.Text = rDate.AddDays(1).ToShortDateString();
                        txtAdvIntTo.Text = endOfMonth.ToShortDateString();
                        txtAdvInterest.ReadOnly = false;
                        ddlAdvIntAcHead.Enabled = true;
                    }
                }
                else
                {
                    txtAdvIntFrom.Text = txtAdvIntTo.Text = "";
                    txtAdvInterest.ReadOnly = true;
                    ddlAdvIntAcHead.Enabled = false;
                }
            }
            else
            {
                DateTime rDate = Convert.ToDateTime(txtRecvDate.Text);
                DateTime endOfMonth = new DateTime(rDate.Year, rDate.Month, DateTime.DaysInMonth(rDate.Year, rDate.Month));
                if (rDate == endOfMonth)
                {
                    txtAdvIntFrom.Text = txtAdvIntTo.Text = rDate.ToShortDateString();
                }
                else
                {
                    txtAdvIntFrom.Text = txtAdvIntTo.Text = "";
                    txtAdvIntFrom.Text = rDate.AddDays(1).ToShortDateString();
                    txtAdvIntTo.Text = endOfMonth.ToShortDateString();
                    txtAdvInterest.ReadOnly = false;
                    ddlAdvIntAcHead.Enabled = true;
                }
            }

            hdnoperation.Value = "Save";
            //--------------------------------------Customer Account--------------------------------------
            conn = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select AccountID From tblAccountmaster where Alies='" + txtGoldNo.Text.Trim() + "'";
            da = new SqlDataAdapter(cmd);
            DataTable dt1 = new DataTable();
            da.Fill(dt1);
            if (dt1.Rows.Count > 0)
            {
                hdnddlPrincipalCurrentAcHead.Value = dt1.Rows[0]["AccountID"].ToString().Trim();
                ddlPrincipalCurrentAcHead.SelectedValue = dt1.Rows[0]["AccountID"].ToString().Trim();
                cmd.CommandText = "select Credit from FLedgerMaster where Narration like 'Advance %' and  AccountID=" + dt1.Rows[0]["AccountID"].ToString().Trim();
                da2 = new SqlDataAdapter(cmd);
                dt2 = new DataTable();
                da2.Fill(dt2);

                //  string AdvAmount = dt2.Rows[0]["Credit"].ToString().Trim();
            }

            string LastDates;
            //if (dt.Rows[0]["LastInterestDate"].ToString() != "" && dt.Rows[0]["LastInterestDate"].ToString() != null)
            //{
            //    LastDates = dt.Rows[0]["LastInterestDate"].ToString();
            //}
            //else
            //{
            //    LastDates = dt.Rows[0]["LastReceiveDate"].ToString();
            //}
            LastDates = dt.Rows[0]["LastReceiveDate"].ToString();


            string LastBCI;
            //in advance interest os is not added
            if (dt.Rows[0]["BCLI"].ToString() != "" && dt.Rows[0]["LastOSInt"].ToString() != null)
            {
                LastBCI = dt.Rows[0]["BCLI"].ToString();
            }
            else
            {
                LastBCI = "0";
            }

            //string InterestFromDate = string.Empty;

            //if (dt.Rows[0]["InterestFromDate"].ToString() != "" && dt.Rows[0]["InterestFromDate"].ToString() != null)
            //{
            //    InterestFromDate = dt.Rows[0]["InterestFromDate"].ToString();
            //}

            //string RvcCLI = string.Empty;

            //if (dt.Rows[0]["RecvCLI"].ToString() != "" && dt.Rows[0]["RecvCLI"].ToString() != null)
            //{
            //    RvcCLI = dt.Rows[0]["RecvCLI"].ToString();
            //}



            //added by priya for Advance Interest Calculation----
            //lblInterestCurrent.Text != "0.00"
            if (Convert.ToDateTime(LastDates) <= Convert.ToDateTime(txtRecvDate.Text))
            {
                conn = new SqlConnection(strConnString);
                if (txtAdvIntFrom.Text != "")
                {
                    SqlCommand cmdAdv = new SqlCommand();
                    cmdAdv.Connection = conn;
                    cmdAdv.CommandType = CommandType.StoredProcedure;
                    cmdAdv.CommandText = "GL_EmiCalculator_RTR";

                    cmdAdv.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["CustLoanDate"].ToString()));
                    cmdAdv.Parameters.AddWithValue("@LoanAmount", loanAmount);
                    //cmdAdv.Parameters.AddWithValue("@LoanAmount", dt.Rows[0]["LoanAmout"].ToString());
                    cmdAdv.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
                    cmdAdv.Parameters.AddWithValue("@NeworOld", neworold);

                    cmdAdv.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                    cmdAdv.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                    cmdAdv.Parameters.AddWithValue("@PaidInt", RvcCLI);

                    cmdAdv.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                    cmdAdv.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                    cmdAdv.Parameters.AddWithValue("@OSIntAmt", dt.Rows[0]["OSIntAmt"].ToString());

                    cmdAdv.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                    cmdAdv.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                    //cmdAdv.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[0]["AdvInterestAmount"].ToString());
                    cmdAdv.Parameters.AddWithValue("@AdvInterestAmt", 0);

                    cmdAdv.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text));
                    cmdAdv.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text));
                    cmdAdv.Parameters.AddWithValue("@LastROIID", RowRoiID);



                    SqlDataAdapter daAdv = new SqlDataAdapter(cmdAdv);
                    DataSet dsAdv = new DataSet();
                    daAdv.Fill(dsAdv);

                    double TotalAdv = 0;

                    for (int i = 0; i < dsAdv.Tables[0].Rows.Count; i++)
                    {
                        if (dsAdv.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
                        {
                            TotalAdv = Convert.ToDouble(TotalAdv) + Convert.ToDouble(dsAdv.Tables[0].Rows[i]["InterestAmount"].ToString());
                        }
                    }
                    HdnAdvanceInterest.Value = Convert.ToString(Math.Round(TotalAdv));
                }
            }
            //end of advance---

            //--------------------------------------End Customer Account-----------------------------------------------------------



            BindPlCases_Details(hdnplcaseno.Value);


        }

        //connAIM = new SqlConnection(strConnStringAIM);
        //cmd = new SqlCommand();
        //cmd.Connection = connAIM;

        //cmd.CommandText = "select AccountID from tblAccountmaster where  Alies=" + txtGoldNo.Text;
        //da2 = new SqlDataAdapter(cmd);
        //dt2 = new DataTable();
        //da2.Fill(dt2);

        //if (dt2.Rows.Count > 0)
        //{
        //    cmd.CommandText = "select Credit from FLedgerMaster where Narration like 'advance %' and  AccountID=" + dt2.Rows[0]["AccountID"].ToString().Trim();
        //    da2 = new SqlDataAdapter(cmd);
        //    dt2 = new DataTable();
        //    da2.Fill(dt2);
        //}


    }

    //---------Validate Receipt Deatails
    public void GLReceipt_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_PRV";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@RcptId", value);
        cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
        cmd.Parameters.AddWithValue("@ReceiptBookNo", txtReceiptBook.Text);
        cmd.Parameters.AddWithValue("@ReceiptNo", txtReceipt.Text);
        cmd.Parameters.AddWithValue("@GoldLoanNo", txtGoldNo.Text.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    //---------Insert, Update, Delete Receipt Data
    public void GLReceipt_PRI(string operation, string value)
    {

        datasaved = false;
        conn = new SqlConnection(strConnString);
        conn.Open();

        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();

        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");
        transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForAIM");


        //---------------------------------For Basic Table Details------------------------------------------------

        if (operation == "Save")
        {
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = transactionGL;
            cmd.CommandText = "select isnull(MAX(RcptId),0)+1 From TGlReceipt_BasicDetails";
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                value = Convert.ToString(cmd.ExecuteScalar());
            }
        }

        if (operation == "Update")
        {
            //remove try catch bcoz its already declared in main events by bharat sir.
            try
            {
                if (txtIntToDate.Text != "")
                {
                    string KYCID = "";
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandText = "select isnull(MAX(KYCID),0) From TGlReceipt_BasicDetails where rcptId=" + value;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        KYCID = Convert.ToString(cmd.ExecuteScalar());
                    }

                    int neworold = 0;
                    //   conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GLReceipt_GoldLoanDetails_RTR_New";
                    cmd.Parameters.AddWithValue("@RcvDate", gbl.ChangeDateMMddyyyy(txtIntToDate.Text));
                    cmd.Parameters.AddWithValue("@KYCID", KYCID);
                    cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                    cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
                    da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);

                    double total = 0;
                    string ROI = "0";
                    if (dt.Rows.Count > 0)
                    {
                        //           conn.Open();
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transactionGL;
                        cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";


                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            neworold = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        int RcptID = 0;
                        cmdRcpt = new SqlCommand();
                        cmdRcpt.Connection = conn;
                        cmdRcpt.Transaction = transactionGL;
                        //  cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails Isactive='Y' AND where  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";
                        cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails  where  Isactive='Y' AND GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'"; //change for rd
                        if (cmdRcpt.ExecuteScalar() != DBNull.Value)
                        {
                            RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
                        }
                        int RowRoiID = 1;
                        //Check if Advance Interest is paid then Pass top 1 RowID

                        if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
                        {
                            //conn = new SqlConnection(strConnString); //rd
                            //conn.Open();                             //rd


                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;

                            cmdRoiRow.Transaction = transactionGL;  // change for rd

                            //   cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;

                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();


                            // cmd = new SqlCommand("select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString(), conn);
                            //   if (cmd.ExecuteScalar() != DBNull.Value)

                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                            //  conn.Close();                        //rd
                        }
                        else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) > 0)
                        {
                            //Check if Half Interest is paid then Pass Max RowID
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            //cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }
                        else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
                        {
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }

                        string LastDate;
                        LastDate = dt.Rows[0]["LastReceiveDate"].ToString();

                        if (dt.Rows[0]["InterestFromDate"].ToString() != "" && dt.Rows[0]["InterestFromDate"].ToString() != null)
                        {
                            InterestFromDate = dt.Rows[0]["InterestFromDate"].ToString();
                        }
                        else
                        {
                            InterestFromDate = System.DateTime.Today.ToShortDateString();
                        }

                        if (dt.Rows[0]["InterestToDate"].ToString() != "" && dt.Rows[0]["InterestToDate"].ToString() != null)
                        {
                            InterestToDate = dt.Rows[0]["InterestToDate"].ToString();
                        }
                        else
                        {
                            InterestToDate = System.DateTime.Today.ToShortDateString();
                        }


                        if (dt.Rows[0]["RecvInterest"].ToString() != "" && dt.Rows[0]["RecvInterest"].ToString() != null)
                        {
                            RvcCLI = dt.Rows[0]["RecvInterest"].ToString();
                        }
                        else
                        {
                            RvcCLI = "0";
                        }


                        if (dt.Rows[0]["AdvInterestFromDate"].ToString() != "" && dt.Rows[0]["AdvInterestFromDate"].ToString() != null)
                        {
                            AdvInterestFromDate = dt.Rows[0]["AdvInterestFromDate"].ToString();
                        }
                        else
                        {
                            AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
                        }

                        if (dt.Rows[0]["AdvInterestToDate"].ToString() != "" && dt.Rows[0]["AdvInterestToDate"].ToString() != null)
                        {
                            AdvInterestToDate = dt.Rows[0]["AdvInterestToDate"].ToString();
                        }
                        else
                        {
                            AdvInterestToDate = System.DateTime.Today.ToShortDateString();
                        }

                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transactionGL;
                        cmd.CommandText = "GL_EmiCalculator_RTR";
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["CustLoanDate"].ToString()));
                        //  cmd.Parameters.AddWithValue("@LoanAmount", dt.Rows[0]["LoanAmout"].ToString());
                        if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) >= 0)
                        {
                            double AddPrvInt = Math.Round(Convert.ToDouble(dt.Rows[0]["LoanAmout"].ToString()) + Convert.ToDouble(dt.Rows[0]["OSIntAmt"].ToString()));
                            cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@LoanAmount", (dt.Rows[0]["LoanAmout"].ToString()));
                        }

                        cmd.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
                        cmd.Parameters.AddWithValue("@NeworOld", neworold);

                        cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

                        cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[0]["OSIntAmt"].ToString());

                        cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                        cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                        cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[0]["AdvInterestAmount"].ToString());

                        cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
                        cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtRecvDate.Text));
                        cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        Session["InterestDetails"] = ds;

                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                // conn.Close();
            }

        }

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transactionGL;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_PRI";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@RcptId", value);
        cmd.Parameters.AddWithValue("@SDID", hdnsdid.Value);
        cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
        cmd.Parameters.AddWithValue("@GoldLoanNo", txtGoldNo.Text);
        cmd.Parameters.AddWithValue("@ReceiveDate", gbl.ChangeDateMMddyyyy(txtRecvDate.Text));
        cmd.Parameters.AddWithValue("@ReceiveTime", gbl.ChangeDateMMddyyyy(txtRecvDate.Text));
        cmd.Parameters.AddWithValue("@BalanceLoanEligibility", lblBalanceLoanEligibilityAmount.Text);
        cmd.Parameters.AddWithValue("@ROI", txtROI.Text);
        cmd.Parameters.AddWithValue("@ReceiptBookNo", txtReceiptBook.Text);
        cmd.Parameters.AddWithValue("@ReceiptNo", txtReceipt.Text);
        cmd.Parameters.AddWithValue("@ReceivedFrom", txtRecvFrom.Text);
        cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@CLP", lblPrincipalCurrent.Text);
        cmd.Parameters.AddWithValue("@CLI", lblInterestCurrent.Text);
        cmd.Parameters.AddWithValue("@CLPI", lblPenalCurrent.Text);
        cmd.Parameters.AddWithValue("@CLC", lblChargesCurrent.Text);
        cmd.Parameters.AddWithValue("@CLO", lblCurrentTotal.Text);

        if (txtPrincipalCurrentAmt.Text != "")
        {
            cmd.Parameters.AddWithValue("@RcvCLP", txtPrincipalCurrentAmt.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@RcvCLP", "0");
        }
        // cmd.Parameters.AddWithValue("@RecCLPAccID", ddlPrincipalCurrentAcHead.SelectedValue);
        cmd.Parameters.AddWithValue("@RecCLPAccID", hdnddlPrincipalCurrentAcHead.Value);  //change for rd
        cmd.Parameters.AddWithValue("@RecCLPNarID", Request[ddlPrincipalCurrentNarration.UniqueID]);


        if (txtInterestCurrentAmt.Text != "" && Convert.ToDecimal(txtInterestCurrentAmt.Text) > 0)
        {
            cmd.Parameters.AddWithValue("@RcvCLI", txtInterestCurrentAmt.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@RcvCLI", "0");
        }
        cmd.Parameters.AddWithValue("@RecCLIAccID", Request[ddlInterestCurrentAcHead.UniqueID]);
        cmd.Parameters.AddWithValue("@RecCLINarID", Request[ddlInterestCurrentNarration.UniqueID]);

        if (txtPenalCurrentAmt.Text != "" && Convert.ToDecimal(txtPenalCurrentAmt.Text) > 0)
        {
            cmd.Parameters.AddWithValue("@RcvCLPI", txtPenalCurrentAmt.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@RcvCLPI", "0");
        }
        cmd.Parameters.AddWithValue("@RecCLPIAccID", Request[ddlPenalCurrentAcHead.UniqueID]);
        cmd.Parameters.AddWithValue("@RecCLPINarID", Request[ddlPenalCurrentNarration.UniqueID]);

        if (txtChargesCurrentAmt.Text != "" && Convert.ToDecimal(txtChargesCurrentAmt.Text) > 0)
        {
            cmd.Parameters.AddWithValue("@RecCLC", txtChargesCurrentAmt.Text);

        }
        else
        {
            cmd.Parameters.AddWithValue("@RecCLC", "0");
        }
        cmd.Parameters.AddWithValue("@RecCLCAccID", Request[ddlChargesCurrentAcHead.UniqueID]);
        cmd.Parameters.AddWithValue("@RecCLCNarID", Request[ddlChargesCurrentNarration.UniqueID]);

        //----------------------------------

        // Added by rakesh  

        //    if (txtAdvInterest.Text != "" && Convert.ToDecimal(txtAdvInterest.Text) > 0)
        //{
        //    cmd.Parameters.AddWithValue("@RcvCLO", txtAdvInterest.Text);
        //}
        //else
        //{
        //    cmd.Parameters.AddWithValue("@RcvCLO", "0");
        //}
        ////   cmd.Parameters.AddWithValue("@RecCLOAccID", Request[ddlAdvInterestAcHead.UniqueID]);
        //    cmd.Parameters.AddWithValue("@RecCLOAccID", Request[ddlAdvIntAcHeadAcHead.UniqueID]);
        //   cmd.Parameters.AddWithValue("@RecCLONarID", Request[ddlChargesCurrentNarration.UniqueID]);

        //--------------------------------------

        if (txtRcvTotal.Text != "")
        {
            cmd.Parameters.AddWithValue("@RcvTotal", txtRcvTotal.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@RcvTotal", "0");
        }
        cmd.Parameters.AddWithValue("@BankAccID", ddlBankAcc.SelectedValue);
        cmd.Parameters.AddWithValue("@CashAccID", ddlCashAcc.SelectedValue);
        cmd.Parameters.AddWithValue("@CollectedByID", ddlCollectedBy.SelectedValue);
        cmd.Parameters.AddWithValue("@CashierID", ddlCashier.SelectedValue);
        cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        cmd.Parameters.AddWithValue("@CMPID", "1");
        cmd.Parameters.AddWithValue("@Lineno", 0);

        //added by priya
        System.TimeSpan OSDays;
        DateTime IntToDate = DateTime.MinValue;
        DateTime RcvDate = DateTime.Parse(txtRecvDate.Text).Date;
        if (txtIntToDate.Text != "")
        {
            IntToDate = DateTime.Parse(txtIntToDate.Text).Date;
        }
        else
        {
            IntToDate = DateTime.MinValue;
        }

        OSDays = RcvDate - IntToDate;


        if (txtInterestCurrentAmt.Text != "" && Convert.ToDecimal(txtInterestCurrentAmt.Text) > 0)
        {
            cmd.Parameters.AddWithValue("@InterestFromDate", gbl.ChangeDateMMddyyyy(txtIntFromDate.Text));
            cmd.Parameters.AddWithValue("@InterestToDate", gbl.ChangeDateMMddyyyy(txtIntToDate.Text));
        }
        else
        {
            cmd.Parameters.AddWithValue("@InterestFromDate", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@InterestToDate", System.DBNull.Value);
        }
        cmd.Parameters.AddWithValue("@OSPrincipal", Convert.ToDecimal(lblPrincipalCurrent.Text) - Convert.ToDecimal(txtPrincipalCurrentAmt.Text));
        cmd.Parameters.AddWithValue("@OSInterest", Convert.ToDecimal(lblInterestCurrent.Text) - Convert.ToDecimal(txtInterestCurrentAmt.Text));

        if (txtIntToDate.Text != "")
        {
            cmd.Parameters.AddWithValue("@OSDays", OSDays.Days);
            cmd.Parameters.AddWithValue("@IntrestRcvTillDate", gbl.ChangeDateMMddyyyy(txtIntToDate.Text));
        }
        else
        {
            cmd.Parameters.AddWithValue("@OSDays", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@IntrestRcvTillDate", System.DBNull.Value);
        }


        cmd.Parameters.AddWithValue("@LastROI", txtROI.Text);
        if (txtAdvInterest.Text != "" && Convert.ToDouble(txtAdvInterest.Text) > 0)
        {
            cmd.Parameters.AddWithValue("@AdvInterestAmt", txtAdvInterest.Text);
            cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text));
            cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text));
            cmd.Parameters.AddWithValue("@AdvInterestAccID", Request[ddlAdvIntAcHead.UniqueID]);
        }
        else
        {
            cmd.Parameters.AddWithValue("@AdvInterestAmt", "0");
            cmd.Parameters.AddWithValue("@AdvInterestFromDate", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@AdvInterestToDate", System.DBNull.Value);
            cmd.Parameters.AddWithValue("@AdvInterestAccID", "0");
        }

        if (HdnAdvanceInterest.Value != "" && Convert.ToDouble(HdnAdvanceInterest.Value) > 0)
        {
            cmd.Parameters.AddWithValue("@TotalAdvInterestAmount", HdnAdvanceInterest.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@TotalAdvInterestAmount", "0");
        }

        double CalcOSAdvInt = 0;
        if (HdnAdvanceInterest.Value != "" && Convert.ToDouble(HdnAdvanceInterest.Value) > 0)
        {
            if (txtAdvInterest.Text != "" && Convert.ToDouble(txtAdvInterest.Text) > 0)
            {
                CalcOSAdvInt = (Convert.ToDouble(HdnAdvanceInterest.Value) - Convert.ToDouble(txtAdvInterest.Text));
            }
        }
        else
        {
            CalcOSAdvInt = 0;
        }

        if (CalcOSAdvInt > 0)
        {
            cmd.Parameters.AddWithValue("@OSAdvInterestAmount", CalcOSAdvInt);
        }
        else
        {
            cmd.Parameters.AddWithValue("@OSAdvInterestAmount", "0");
        }

        //---------------------------End For Basic Table Details--------------------------------------------------------

        cmd.Parameters.AddWithValue("@Flag", "MAIN");

        //-------------------------For Cheque Details-------------------------------------------------------------------
        result = cmd.ExecuteNonQuery();

        //return;

        if (result > 0)
        {
            datasaved = true;
        }
        else
        {
            datasaved = false;
        }

        if (datasaved)
        {

            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
            {
                for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
                {

                    gvChequeDetails.SelectedIndex = i;

                    HiddenField hdnchqid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqid");
                    TextBox gvtxtChqSrno = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqSrno");
                    TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
                    TextBox gvtxtChqDate = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqDate");
                    DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
                    HiddenField hdnbankid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnbankid");
                    TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");
                    DropDownList gvddlChqDDNeft = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqDDNeft");

                    //if (gvtxtChqNo.Text != "")
                    //{
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GLReceipt_PRI";

                    cmd.Parameters.AddWithValue("@Operation", operation);
                    cmd.Parameters.AddWithValue("@RcptId", value);
                    cmd.Parameters.AddWithValue("@Lineno", i);
                    cmd.Parameters.AddWithValue("@Flag", "CHEQUE");
                    cmd.Parameters.AddWithValue("@ChequeId", hdnchqid.Value);
                    cmd.Parameters.AddWithValue("@Serialno", gvtxtChqSrno.Text);
                    cmd.Parameters.AddWithValue("@Chq_DD_NEFT", gvddlChqDDNeft.SelectedValue.Trim());
                    if (gvtxtChqNo.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@ChequeNo", gvtxtChqNo.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ChequeNo", System.DBNull.Value);
                    }
                    cmd.Parameters.AddWithValue("@ChequeDate", gvtxtChqDate.Text);

                    if (gvtxtChqNo.Text != "")
                    {
                        cmd.Parameters.AddWithValue("@BankId", gvddlChqBank.SelectedValue);
                        cmd.Parameters.AddWithValue("@BankName", gvddlChqBank.SelectedItem.Text);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@BankId", System.DBNull.Value);
                        cmd.Parameters.AddWithValue("@BankName", System.DBNull.Value);
                    }


                    cmd.Parameters.AddWithValue("@Amount", gvtxtChqAmount.Text);

                    result = cmd.ExecuteNonQuery();
                    //  }
                }
            }
        }


        if (result > 0)
        {

            datasaved = true;
        }
        else
        {
            datasaved = false;

        }
        //-------------------------End For Cheque Details-----------------------------------------------------

        //-------------------------For Denomination Details---------------------------------------------------
        if (datasaved)
        {

            if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
            {
                for (int j = 0; j < gvDenominationDetails.Rows.Count; j++)
                {
                    gvDenominationDetails.SelectedIndex = j;

                    HiddenField hdncashinoutid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdncashinoutid");
                    HiddenField hdnrefno = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdnrefno");
                    HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
                    TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
                    TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
                    TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
                    TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
                    TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");


                    if (gvtxtDenoDescription.Text != "" && gvtxtDenoDescription.Text != "0")
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transactionGL;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GLReceipt_PRI";

                        cmd.Parameters.AddWithValue("@Operation", operation);
                        cmd.Parameters.AddWithValue("@RcptId", value);
                        cmd.Parameters.AddWithValue("@InOutID", hdncashinoutid.Value);
                        cmd.Parameters.AddWithValue("@RefNo", hdnrefno.Value);
                        cmd.Parameters.AddWithValue("@ReferenceType", "0");
                        cmd.Parameters.AddWithValue("@FName", "R");
                        cmd.Parameters.AddWithValue("@InOutMode", "I");
                        cmd.Parameters.AddWithValue("@InOutTo", "Cashier");
                        cmd.Parameters.AddWithValue("@InOutToID", ddlCollectedBy.SelectedValue);
                        cmd.Parameters.AddWithValue("@InOutFrom", "0");
                        cmd.Parameters.AddWithValue("@InOutFromID", "0");
                        cmd.Parameters.AddWithValue("@InOutBy", ddlCashier.SelectedValue);

                        cmd.Parameters.AddWithValue("@Lineno", j);
                        cmd.Parameters.AddWithValue("@Flag", "DENOMINATION");
                        cmd.Parameters.AddWithValue("@DenoId", hdndenoid.Value);
                        cmd.Parameters.AddWithValue("@Serialno", gvtxtDenoSrno.Text);
                        cmd.Parameters.AddWithValue("@DenoRs", gvtxtDenoDescription.Text);
                        cmd.Parameters.AddWithValue("@Quantity", gvtxtDenoNo.Text);
                        cmd.Parameters.AddWithValue("@Total", gvtxtDenoTotal.Text);
                        cmd.Parameters.AddWithValue("@NoteNos", gvtxtDenoNoteNos.Text);
                        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
                        cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);

                        result = cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        // conn.Close();
        if (result > 0)
        {
            datasaved = true;

        }
        else
        {
            datasaved = false;
        }


        //-------------------------End For Denomination Details---------------------------------------------------



        //------------------------Start for insert in TGLInterest_Details//// GLInsert_InterestDetails-------------

        DataSet ds1 = (DataSet)Session["InterestDetails"];
        int rcptID = 0;

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transactionGL;
        cmd.CommandText = "SELECT isnull(MAX(RcptId),0) FROM TGlReceipt_BasicDetails";
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            rcptID = Convert.ToInt32(cmd.ExecuteScalar());
        }


        if (ds1 != null && ds1.Tables[0].Rows.Count > 0)
        {
            try
            {
                for (int s = 0; s < ds1.Tables[0].Rows.Count; s++)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GLInsert_InterestDetails";
                    cmd.Parameters.AddWithValue("@ReceiptID", rcptID);
                    cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(ds1.Tables[0].Rows[s]["FromDate"].ToString()));
                    cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(ds1.Tables[0].Rows[s]["ToDate"].ToString()));
                    cmd.Parameters.AddWithValue("@LoanAmount", ds1.Tables[0].Rows[s]["LoanAmount"].ToString());
                    cmd.Parameters.AddWithValue("@TotalDays", ds1.Tables[0].Rows[s]["TotalDays"].ToString());
                    cmd.Parameters.AddWithValue("@AllDaysTillDate", ds1.Tables[0].Rows[s]["AllDaysTillDate"].ToString());
                    cmd.Parameters.AddWithValue("@ROI", ds1.Tables[0].Rows[s]["ROI"].ToString());
                    //Added on 2-10-2015
                    cmd.Parameters.AddWithValue("@ROIRowID", ds1.Tables[0].Rows[s]["ROIID"].ToString());
                    cmd.Parameters.AddWithValue("@InterestAmount", ds1.Tables[0].Rows[s]["InterestAmount"].ToString());
                    cmd.Parameters.AddWithValue("@PrevOSInterest", ds1.Tables[0].Rows[s]["PrevOSInterest"].ToString());

                    result = cmd.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
            }

        }
        if (result > 0)
        {
            datasaved = true;

        }
        else
        {
            datasaved = false;
        }
        //-------------------------insert interst end

        //--------------------------Account Entries Details-------------------------------------------------------
        int BCRID = 0;
        int voucherno = 0;

        if (ddlPaymentMode.SelectedIndex == 1)
        {
            RefType = "GBR";
        }
        if (ddlPaymentMode.SelectedIndex == 2)
        {
            RefType = "GCR";
        }
        if (ddlPaymentMode.SelectedIndex == 3)
        {
            RefType = "GBCR";
        }

        #region [Save Account Entry]
        if (operation == "Save")
        {

            //---------------------------------------Cheque Entry---------------------------------------------------------


            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.Transaction = transactionAIM;
            cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails ";
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                BCRID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.Transaction = transactionAIM;
            cmd.CommandText = "SELECT isnull(MAX(RefNo),0)+1 FROM TBankCash_ReceiptDetails WHERE RefType='" + RefType + "' ";
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                RefId = Convert.ToString(cmd.ExecuteScalar());
            }
            RefNo = RefType + "/" + RefId;


            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.Transaction = transactionAIM;
            cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                voucherno = Convert.ToInt32(cmd.ExecuteScalar());
            }


            if (datasaved)
            {

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandText = "update TGlReceipt_BasicDetails set BCRID='" + BCRID + "'" +
                                  "where RcptId='" + value + "' ";
                result = cmd.ExecuteNonQuery();

            }

            if (result > 0)
            {
                datasaved = true;
            }
            else
            {
                datasaved = false;
            }


            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
            {
                for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
                {

                    gvChequeDetails.SelectedIndex = i;

                    TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
                    TextBox gvtxtChqDate = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqDate");
                    DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
                    TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");

                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                                      "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                                      "'" + ddlBankAcc.SelectedValue + "','" + ddlPrincipalCurrentAcHead.SelectedValue + "','" + gvtxtChqAmount.Text + "'," +
                                      "'" + gvtxtChqNo.Text + "','" + gbl.ChangeDateMMddyyyy(gvtxtChqDate.Text) + "','" + gvddlChqBank.SelectedValue + "', " +
                                      "'" + ddlPrincipalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                                      "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                                      "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                                      "'" + hdnfyid.Value + "','0','0')";

                    result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        datasaved = true;
                    }
                    else
                    {
                        datasaved = false;
                    }

                    LedgerID = 0;
                    if (datasaved)
                    {

                        int AccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        //   int ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        int ContraAccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);
                        double DebitAmount = Convert.ToDouble(gvtxtChqAmount.Text);
                        double CreditAmount = 0;


                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }

                        if (datasaved)
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = connAIM;
                            cmd.Transaction = transactionAIM;
                            cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                                                "where BCRID='" + BCRID + "' ";
                            result = cmd.ExecuteNonQuery();
                        }
                        if (result > 0)
                        {

                            datasaved = true;
                        }
                        else
                        {
                            datasaved = false;
                        }

                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(gvtxtChqAmount.Text);

                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }

                        //}
                    }
                }

            }


            //-------------------------------------------Cash Entries------------------------------------------------------------------

            if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
            {

                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                }


                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                }

                double DenoCash = 0;
                if (ddlPaymentMode.SelectedIndex == 2)
                {
                    DenoCash = Convert.ToDouble(txtRcvTotal.Text);
                }
                else
                {
                    DenoCash = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);
                }

                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                                  "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                                  "'" + ddlCashAcc.SelectedValue + "','" + ddlPrincipalCurrentAcHead.SelectedValue + "','" + DenoCash + "'," +
                                  "'" + "" + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + "0" + "', " +
                                  "'" + ddlPrincipalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                                  "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                                  "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                                  "'" + hdnfyid.Value + "','0','0')";
                result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }

                if (datasaved)
                {
                    int AccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                    //  int ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    int ContraAccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);

                    double DebitAmount = 0;
                    if (ddlPaymentMode.SelectedIndex == 2)
                    {
                        DebitAmount = Convert.ToDouble(txtRcvTotal.Text);
                    }
                    else
                    {
                        DebitAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);
                    }
                    // double DebitAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);
                    double CreditAmount = 0;

                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    }
                    //if (datasaved)
                    //{

                    //    cmd = new SqlCommand();
                    //    cmd.Connection = conn;
                    //    cmd.Transaction = transactionGL;
                    //    cmd.CommandText = "update TGlReceipt_BasicDetails set BCRID='" + BCRID + "'" +
                    //                      "where RcptId='" + value + "' ";
                    //    result = cmd.ExecuteNonQuery();

                    //}
                    if (datasaved)
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                                        "where BCRID='" + BCRID + "' ";
                        result = cmd.ExecuteNonQuery();
                    }
                    if (result > 0)
                    {

                        datasaved = true;
                    }
                    else
                    {
                        datasaved = false;
                    }

                    //if (datasaved)
                    //{
                    //    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    //    ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                    //    DebitAmount = 0;
                    //    CreditAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);


                    //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    //    if (datasaved)
                    //    {
                    //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    //    }

                    //}

                }

            }

            //-------------------------------Entry for Principal addded by Priya-------------------------------------------------------------

            if (datasaved)
            {
                if (txtPrincipalCurrentAmt.Text != "" && Convert.ToDouble(txtPrincipalCurrentAmt.Text) > 0)
                {
                    if (datasaved)
                    {
                        //  int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        int AccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);

                        int ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }

                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtPrincipalCurrentAmt.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                }

            }
            //-------------------------------End of Principal


            //-------------------------------Entry for Interest-------------------------------------------------------------
            if (datasaved)
            {
                if (txtInterestCurrentAmt.Text != "" && Convert.ToDouble(txtInterestCurrentAmt.Text) > 0)
                {

                    if (datasaved)
                    {

                        //int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //int ContraAccID = Convert.ToInt32(ddlInterestCurrentAcHead.SelectedValue);
                        //double DebitAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);
                        //double CreditAmount = 0;

                        //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        //changed by Priya
                        // int AccID = Convert.ToInt32(ddlInterestCurrentAcHead.SelectedValue);
                        int AccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);
                        int ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }

                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlInterestCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }

                        //if (datasaved)
                        //{
                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                        //                      "where BCRID='" + BCRID + "' ";
                        //    result = cmd.ExecuteNonQuery();
                        //}
                        //if (result > 0)
                        //{

                        //    datasaved = true;
                        //}
                        //else
                        //{
                        //    datasaved = false;
                        //}
                        //contra entry
                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlInterestCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);

                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }

                        //}
                    }
                }

            }

            //}
            //-------------------------------------------Entry For Penal Interest------------------------------------------------

            if (datasaved)
            {
                if (txtPenalCurrentAmt.Text != "" && Convert.ToDouble(txtPenalCurrentAmt.Text) > 0)
                {
                    //cmd = new SqlCommand();
                    //cmd.Connection = connAIM;
                    //cmd.Transaction = transactionAIM;
                    //cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                    //if (cmd.ExecuteScalar() != DBNull.Value)
                    //{
                    //    BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                    //}


                    //cmd = new SqlCommand();
                    //cmd.Connection = connAIM;
                    //cmd.Transaction = transactionAIM;
                    //cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                    //if (cmd.ExecuteScalar() != DBNull.Value)
                    //{
                    //    voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                    //}

                    //cmd = new SqlCommand();
                    //cmd.Connection = connAIM;
                    //cmd.Transaction = transactionAIM;
                    //cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                    //                  "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                    //                  "'" + ddlPenalCurrentAcHead.SelectedValue + "','" + ddlPrincipalCurrentAcHead.SelectedValue + "','" + txtPenalCurrentAmt.Text + "'," +
                    //                  "'" + "" + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + "0" + "', " +
                    //                  "'" + ddlPenalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                    //                  "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                    //                  "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                    //                  "'" + hdnfyid.Value + "','0','0')";
                    //result = cmd.ExecuteNonQuery();

                    //if (result > 0)
                    //{
                    //    datasaved = true;
                    //}
                    //else
                    //{
                    //    datasaved = false;
                    //}

                    if (datasaved)
                    {

                        //int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //int ContraAccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                        //double DebitAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);
                        //double CreditAmount = 0;

                        //cmd.Transaction = transactionAIM;
                        //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        //int AccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                        int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);

                        int ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }

                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);

                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPenalCurrentNarration.SelectedItem.Text);


                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }
                        //if (datasaved)
                        //{
                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                        //                       "where BCRID='" + BCRID + "' ";
                        //    result = cmd.ExecuteNonQuery();
                        //}
                        //if (result > 0)
                        //{

                        //    datasaved = true;
                        //}
                        //else
                        //{
                        //    datasaved = false;
                        //}
                        //contra entry
                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);

                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }
                        //}
                    }

                    //extra entries.... for fledger... aaded by priya
                    if (datasaved)
                    {

                        int AccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                        int ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);

                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPenalCurrentNarration.SelectedItem.Text);


                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                    //contra entry

                    if (datasaved)
                    {

                        int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        int ContraAccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                        double DebitAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);
                        double CreditAmount = 0;

                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPenalCurrentNarration.SelectedItem.Text);


                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                }
            }
            //---------------------------------------------Entry for Charges--------------------------------------------------------------------

            if (datasaved)
            {

                if (txtChargesCurrentAmt.Text != "" && Convert.ToDouble(txtChargesCurrentAmt.Text) > 0)
                {

                    if (datasaved)
                    {

                        //int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //int ContraAccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        //double DebitAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);
                        //double CreditAmount = 0;

                        //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        //int AccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        int ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }
                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentNarration.SelectedItem.Text);


                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }
                        //if (datasaved)
                        //{
                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                        //                      "where BCRID='" + BCRID + "' ";
                        //    result = cmd.ExecuteNonQuery();
                        //}
                        //if (result > 0)
                        //{

                        //    datasaved = true;
                        //}
                        //else
                        //{
                        //    datasaved = false;
                        //}

                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }

                        //}
                    }

                    //extra entries.... for fledger... aaded by priya
                    if (datasaved)
                    {
                        int AccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        int ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                    //contra entry
                    if (datasaved)
                    {
                        int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        int ContraAccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        double DebitAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);
                        double CreditAmount = 0;

                        cmd.Transaction = transactionAIM;
                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                }
            }
            //------------Advance Interest Entry-- Added By PRIYA-----------

            if (datasaved)
            {

                if (txtAdvInterest.Text != "" && Convert.ToDouble(txtAdvInterest.Text) > 0)
                {
                    if (datasaved)
                    {

                        //int AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //int ContraAccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                        //double DebitAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);
                        //double CreditAmount = 0;

                        //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        //  int AccID = Convert.ToInt32(ddlAdvIntAcHead.SelectedValue);
                        int AccID = Convert.ToInt32(hdnddlAdvIntAcHead.Value);
                        int ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }
                        double DebitAmount = 0;
                        double CreditAmount = Convert.ToDouble(txtAdvInterest.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, "Advance Interest Received From " + txtGoldNo.Text + " for the period of " + gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text) + " To " + gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text) + "");


                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }
                        //if (datasaved)
                        //{
                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                        //                      "where BCRID='" + BCRID + "' ";
                        //    result = cmd.ExecuteNonQuery();
                        //}
                        //if (result > 0)
                        //{

                        //    datasaved = true;
                        //}
                        //else
                        //{
                        //    datasaved = false;
                        //}
                    }
                }

            }
        }//save end
        //-----------end Advance

        #endregion [Save Account Entry]
        //-------------------------------------- End of Save Account Entries Details-------------------------------------------------------


        //---------------------------------------Update Account Entries Details------------------------------------------------

        #region [Update Account Entry]
        if (operation == "Update")
        {
            int AccID = 0;
            int ContraAccID = 0;
            double DebitAmount = 0;
            double CreditAmount = 0;
            DateTime RefDate;

            if (datasaved)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandText = "select BCRID From TGlReceipt_BasicDetails where RcptId='" + value + "'";

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select ReferenceNo From TBankCash_ReceiptDetails where BCRID='" + BCRID + "'";

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    RefNo = Convert.ToString(cmd.ExecuteScalar());
                }

                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select AccountID, Debit, Credit, RefDate,LedgerID From FLedgerMaster where ReferenceNo='" + RefNo + "'";
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    AccID = Convert.ToInt32(dt.Rows[i]["AccountID"].ToString());
                    DebitAmount = Convert.ToDouble(dt.Rows[i]["Debit"].ToString());
                    CreditAmount = Convert.ToDouble(dt.Rows[i]["Credit"].ToString());
                    RefDate = Convert.ToDateTime(dt.Rows[i]["RefDate"].ToString());
                    LedgerID = Convert.ToInt32(dt.Rows[i]["LedgerID"].ToString());

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnbranchid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }



                }

            }
            if (datasaved)
            {
                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "delete FLedgerMaster where ReferenceNo='" + RefNo + "'";
                result = cmd.ExecuteNonQuery();

                if (result > 0)
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
                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "delete TBankCash_ReceiptDetails where ReferenceNo='" + RefNo + "'";
                result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }



            //updation of ledger------------------------------------------------------------------

            if (datasaved)
            {
                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                }


                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "SELECT isnull(MAX(RefNo),0)+1 FROM TBankCash_ReceiptDetails WHERE RefType='" + RefType + "' ";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    RefId = Convert.ToString(cmd.ExecuteScalar());
                }
                RefNo = RefType + "/" + RefId;


                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }

            if (datasaved)
            {

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandText = "update TGlReceipt_BasicDetails set BCRID='" + BCRID + "'" +
                                  "where RcptId='" + value + "' ";
                result = cmd.ExecuteNonQuery();



                if (result > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
            //----------------------------------------------------------------------------------------------------

            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
            {
                for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
                {


                    gvChequeDetails.SelectedIndex = i;
                    TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
                    TextBox gvtxtChqDate = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqDate");
                    DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
                    TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");

                    if (datasaved)
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                        }


                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                                       "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                                       "'" + ddlBankAcc.SelectedValue + "','" + ddlPrincipalCurrentAcHead.SelectedValue + "','" + gvtxtChqAmount.Text + "'," +
                                       "'" + gvtxtChqNo.Text + "','" + gbl.ChangeDateMMddyyyy(gvtxtChqDate.Text) + "','" + gvddlChqBank.SelectedValue + "', " +
                                       "'" + ddlPrincipalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                                       "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                                       "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                                       "'" + hdnfyid.Value + "','0','0')";

                        result = cmd.ExecuteNonQuery();
                        if (result > 0)
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

                        AccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        //ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        ContraAccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);

                        DebitAmount = Convert.ToDouble(gvtxtChqAmount.Text);
                        CreditAmount = 0;


                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }

                        if (datasaved)
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = connAIM;
                            cmd.Transaction = transactionAIM;
                            cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                                                "where BCRID='" + BCRID + "' ";
                            result = cmd.ExecuteNonQuery();
                        }
                        if (result > 0)
                        {

                            datasaved = true;
                        }
                        else
                        {
                            datasaved = false;
                        }
                        //contra entry
                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(gvtxtChqAmount.Text);

                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }

                        //}
                    }
                }

            }

            //-------------------------------------------Cash Entries------------------------------------------------------------------

            if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
            {

                if (datasaved)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "select isnull(MAX(BCRID),0)+1 from TBankCash_ReceiptDetails";
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "SELECT isnull(MAX(VoucherNo),0)+1 FROM TBankCash_ReceiptDetails";
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        voucherno = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    //Added by priya After removed denomination validation
                    if (ddlPaymentMode.SelectedIndex == 2)
                    {
                        DebitAmount = Convert.ToDouble(txtRcvTotal.Text);
                    }
                    else
                    {
                        DebitAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);
                    }

                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    //cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                    //                  "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                    //                  "'" + ddlCashAcc.SelectedValue + "','" + ddlPrincipalCurrentAcHead.SelectedValue + "','" + DebitAmount + "'," +
                    //                  "'" + "" + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + "0" + "', " +
                    //                  "'" + ddlPrincipalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                    //                  "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                    //                  "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                    //                  "'" + hdnfyid.Value + "','0','0')";



                    cmd.CommandText = "insert into TBankCash_ReceiptDetails values('" + BCRID + "','" + RefType + "','" + RefId + "', " +
                                    "'" + RefNo + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + voucherno + "', " +
                                    "'" + ddlCashAcc.SelectedValue + "','" + hdnddlPrincipalCurrentAcHead.Value + "','" + DebitAmount + "'," +
                                    "'" + "" + "','" + gbl.ChangeDateMMddyyyy(txtRecvDate.Text) + "','" + "0" + "', " +
                                    "'" + ddlPrincipalCurrentNarration.SelectedItem.Text + "','0','0','0'," +
                                    "'" + txtReceiptBook.Text + "','" + txtReceipt.Text + "', " +
                                    "'0','0','','Normal','" + ddlPaymentMode.SelectedValue + "','" + RefNo + "'," +
                                    "'" + hdnfyid.Value + "','0','0')";





                    result = cmd.ExecuteNonQuery();
                    if (result > 0)
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

                    AccID = Convert.ToInt32(ddlCashAcc.SelectedValue);

                    ContraAccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value); //rd
                                                                                       //  ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    if (ddlPaymentMode.SelectedIndex == 2)
                    {
                        DebitAmount = Convert.ToDouble(txtRcvTotal.Text);
                    }
                    else
                    {
                        DebitAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);
                    }
                    CreditAmount = 0;

                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    }
                    //if (datasaved)
                    //{

                    //    cmd = new SqlCommand();
                    //    cmd.Connection = conn;
                    //    cmd.Transaction = transactionGL;
                    //    cmd.CommandText = "update TGlReceipt_BasicDetails set BCRID='" + BCRID + "'" +
                    //                      "where RcptId='" + value + "' ";
                    //    result = cmd.ExecuteNonQuery();

                    //}
                    if (datasaved)
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                                        "where BCRID='" + BCRID + "' ";
                        result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {

                            datasaved = true;
                        }
                        else
                        {
                            datasaved = false;
                        }
                    }

                    ////contra entry
                    //if (datasaved)
                    //{
                    //    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    //    ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                    //    DebitAmount = 0;
                    //    CreditAmount = Convert.ToDouble(((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text);


                    //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    //    if (datasaved)
                    //    {
                    //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    //    }

                    //}
                }
            }


            //-------------------------------Entry for Principal addded by Priya-------------------------------------------------------------

            if (datasaved)
            {
                if (txtPrincipalCurrentAmt.Text != "" && Convert.ToDouble(txtPrincipalCurrentAmt.Text) > 0)
                {
                    if (datasaved)
                    {
                        //  AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        AccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value); //rd

                        ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }

                        DebitAmount = 0;
                        CreditAmount = Convert.ToDouble(txtPrincipalCurrentAmt.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }
                    }
                }

            }
            //-------------------------------End of Principal



            //-------------------------------Entry for Interest-------------------------------------------------------------

            if (datasaved)
            {
                if (txtInterestCurrentAmt.Text != "" && Convert.ToDouble(txtInterestCurrentAmt.Text) > 0)
                {

                    if (datasaved)
                    {

                        //AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //ContraAccID = Convert.ToInt32(ddlInterestCurrentAcHead.SelectedValue);
                        //DebitAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);
                        //CreditAmount = 0;

                        //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //if (datasaved)
                        //{
                        //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        //}

                        //   AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);

                        AccID = Convert.ToInt32(hdnddlPrincipalCurrentAcHead.Value);  //rd


                        ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }

                        DebitAmount = 0;
                        CreditAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlInterestCurrentNarration.SelectedItem.Text);

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                        }

                        //if (datasaved)
                        //{
                        //    cmd = new SqlCommand();
                        //    cmd.Connection = connAIM;
                        //    cmd.Transaction = transactionAIM;
                        //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                        //                      "where BCRID='" + BCRID + "' ";
                        //    result = cmd.ExecuteNonQuery();

                        //    if (result > 0)
                        //    {

                        //        datasaved = true;
                        //    }
                        //    else
                        //    {
                        //        datasaved = false;
                        //    }
                        //}

                        ////contra entry
                        //if (datasaved)
                        //{
                        //    AccID = Convert.ToInt32(ddlInterestCurrentAcHead.SelectedValue);
                        //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                        //    DebitAmount = 0;
                        //    CreditAmount = Convert.ToDouble(txtInterestCurrentAmt.Text);

                        //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                        //    if (datasaved)
                        //    {
                        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        //    }

                        //}
                    }
                }

            }


            //-------------------------------------------Entry For Penal Interest------------------------------------------------


            if (txtPenalCurrentAmt.Text != "" && Convert.ToDouble(txtPenalCurrentAmt.Text) > 0)
            {

                if (datasaved)
                {

                    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    ContraAccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                    DebitAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);
                    CreditAmount = 0;

                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    }
                    if (datasaved)
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = connAIM;
                        cmd.Transaction = transactionAIM;
                        cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                                           "where BCRID='" + BCRID + "' ";
                        result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {

                            datasaved = true;
                        }
                        else
                        {
                            datasaved = false;
                        }
                    }

                    //contra entry...
                    //if (datasaved)
                    //{
                    //    AccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                    //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    //    DebitAmount = 0;
                    //    CreditAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);

                    //    cmd = new SqlCommand();
                    //    cmd.Connection = connAIM;
                    //    cmd.Transaction = transactionAIM;
                    //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    //    if (datasaved)
                    //    {
                    //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    //    }
                    //}
                }


                //extra entries.... for fledger... aaded by priya
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    DebitAmount = 0;
                    CreditAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);

                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentAcHead.SelectedItem.Text);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }
                //contra entry
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    ContraAccID = Convert.ToInt32(ddlPenalCurrentAcHead.SelectedValue);
                    DebitAmount = Convert.ToDouble(txtPenalCurrentAmt.Text);
                    CreditAmount = 0;

                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentAcHead.SelectedItem.Text);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }
            }

            //---------------------------------------------Entry for Charges--------------------------------------------------------------------

            if (txtChargesCurrentAmt.Text != "" && Convert.ToDouble(txtChargesCurrentAmt.Text) > 0)
            {
                if (datasaved)
                {
                    //AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    //ContraAccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                    //DebitAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);
                    //CreditAmount = 0;

                    //LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                    //if (datasaved)
                    //{
                    //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    //}


                    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    ContraAccID = 0;
                    if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                    {
                        ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                    }
                    else
                    {
                        ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                    }
                    DebitAmount = 0;
                    CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentNarration.SelectedItem.Text);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }
                //if (datasaved)
                //{
                //    cmd = new SqlCommand();
                //    cmd.Connection = connAIM;
                //    cmd.Transaction = transactionAIM;
                //    cmd.CommandText = "update TBankCash_ReceiptDetails set LedgerID='" + LedgerID + "'" +
                //                      "where BCRID='" + BCRID + "' ";
                //    result = cmd.ExecuteNonQuery();

                //    if (result > 0)
                //    {

                //        datasaved = true;
                //    }
                //    else
                //    {
                //        datasaved = false;
                //    }
                //}
                ////CONTRA Entry 
                //if (datasaved)
                //{
                //    AccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                //    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                //    DebitAmount = 0;
                //    CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                //    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlPrincipalCurrentNarration.SelectedItem.Text);
                //    if (datasaved)
                //    {
                //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                //    }

                //}


                //extra entries.... for fledger... aaded by priya
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                    ContraAccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    DebitAmount = 0;
                    CreditAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);

                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentAcHead.SelectedItem.Text);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }
                //contra entry
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlPrincipalCurrentAcHead.SelectedValue);
                    ContraAccID = Convert.ToInt32(ddlChargesCurrentAcHead.SelectedValue);
                    DebitAmount = Convert.ToDouble(txtChargesCurrentAmt.Text);
                    CreditAmount = 0;

                    cmd.Transaction = transactionAIM;
                    LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, ddlChargesCurrentAcHead.SelectedItem.Text);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }



            } //Charges end


            //------------Advance Interest Entry-----------

            if (datasaved)
            {
                if (txtAdvInterest.Text != "" && Convert.ToDouble(txtAdvInterest.Text) > 0)
                {
                    if (datasaved)
                    {
                        AccID = Convert.ToInt32(ddlAdvIntAcHead.SelectedValue);
                        ContraAccID = 0;
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            ContraAccID = Convert.ToInt32(ddlBankAcc.SelectedValue);
                        }
                        else
                        {
                            ContraAccID = Convert.ToInt32(ddlCashAcc.SelectedValue);
                        }
                        DebitAmount = 0;
                        CreditAmount = Convert.ToDouble(txtAdvInterest.Text);

                        LedgerID = CreateNormalLedgerEntries(RefType, RefNo, Convert.ToDateTime(txtRecvDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, "Advance Interest Received From " + txtGoldNo.Text + "  for the period of " + gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text) + " To " + gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text) + "");

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnfyid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                        }

                    }
                }

            }//-------Advance end
        }
        #endregion [Update Account Entry]
        //---------------------------------------End of Update Account Entries Details------------------------------------------------

        //-----------------------------------------Delete Account Entries Details----------------------------------------------
        #region [Delete Account Entry]
        if (@operation == "Delete")
        {

            if (datasaved)
            {

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                //    cmd.CommandText = "select BCRID From TGlReceipt_BasicDetails where RcptId='" + value + "'";

                cmd.CommandText = "select BCRID From TGlReceipt_BasicDetails with (nolock)  where RcptId='" + value + "'";

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BCRID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select ReferenceNo From TBankCash_ReceiptDetails where BCRID='" + BCRID + "'";

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    RefNo = Convert.ToString(cmd.ExecuteScalar());
                }


                cmd = new SqlCommand();
                cmd.Connection = connAIM;
                cmd.Transaction = transactionAIM;
                cmd.CommandText = "select AccountID, Debit, Credit, RefDate From FLedgerMaster where ReferenceNo='" + RefNo + "'";
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);

                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    int AccID = Convert.ToInt32(dt.Rows[i]["AccountID"].ToString());
                    double DebitAmount = Convert.ToDouble(dt.Rows[i]["Debit"].ToString());
                    double CreditAmount = Convert.ToDouble(dt.Rows[i]["Credit"].ToString());
                    DateTime RefDate = Convert.ToDateTime(dt.Rows[i]["RefDate"].ToString());

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(hdnfyid.Value), Convert.ToInt32(hdncmpid.Value), Convert.ToInt32(hdnbranchid.Value), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }
                }

                //Delete from FLedger Entry
                if (datasaved)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "delete from FLedgerMaster where ReferenceNo='" + RefNo + "'";
                    result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        datasaved = true;
                    }
                    else
                    {
                        datasaved = false;
                    }
                }

                //Delete from TBankCash_ReceiptDetails Entry
                if (datasaved)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = connAIM;
                    cmd.Transaction = transactionAIM;
                    cmd.CommandText = "delete from TBankCash_ReceiptDetails where ReferenceNo='" + RefNo + "'";
                    result = cmd.ExecuteNonQuery();

                    if (result >= 0)
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
        #endregion [Delete Account Entry]
        //-----------------------------------------End of Delete Account Entries Details----------------------------------------------

        if (datasaved)
        {
            transactionGL.Commit();
            transactionAIM.Commit();
        }
        else
        {
            transactionGL.Rollback();
            transactionAIM.Rollback();

        }

        if (datasaved == true && operation == "Save")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReceiptSaved", "alert('Record Saved Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

            // Send_Message(hdnmobileno.Value, txtGoldNo.Text);
            ClearData();

        }
        if (datasaved == true && operation == "Update")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReceiptSaved1", "alert('Record Updated Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            // Send_Message(hdnmobileno.Value, txtGoldNo.Text);
            ClearData();
        }
        if (datasaved == true && operation == "Delete")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ReceiptSaved2", "alert('Record Deleted Successfully');", true);
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
    }

    #region [CreateNormalLedgerEntries]
    protected int CreateNormalLedgerEntries(string Reftype, string ReferenceNo, DateTime RefDate, int AccID, double DebitAmount, double CreditAmount, int ContraAccID, string Narration)
    {
        int LedgerID = 0;
        try
        {
            strQuery = "SELECT MAX(LedgerID) FROM FLedgerMaster";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
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
                                    "" + AccID + ", " + DebitAmount + ", " + CreditAmount + ", '" + Narration + "', " +
                                    "" + ContraAccID + ", '', " + hdnfyid.Value + ") ";

            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
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
    //---------Bind Receipt 
    public void GLReceipt_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_RTR";
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.PropertygvGlobal.DataBind();
        Master.PropertympeGlobal.Show();
        hdnpopup.Value = "Edit";
    }

    //----------Bind Receipt Search
    public void GLReceipt_Search()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_Search";
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.PropertygvGlobal.DataBind();
        Master.PropertympeGlobal.Show();
    }

    //------------Bind Receipt Details
    public void GLReceiptDetails_RTR(string rcptid, object sender, EventArgs e)
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceiptDetails_RTR";
        cmd.Parameters.AddWithValue("@RcptId", rcptid);
        da = new SqlDataAdapter(cmd);
        ds = new DataSet();
        da.Fill(ds);
        if (ds.Tables.Count > 0)
        {
            hdnoperation.Value = "Update";
            if (ds.Tables[0].Rows.Count > 0)
            {
                btnGlSearch.Enabled = false;
                hdnid.Value = ds.Tables[0].Rows[0]["RcptId"].ToString();
                txtRecvDate.Text = ds.Tables[0].Rows[0]["ReceiveDate"].ToString();
                hdnkycid.Value = ds.Tables[0].Rows[0]["KYCID"].ToString();
                hdnplcaseno.Value = ds.Tables[0].Rows[0]["ExistingPLCaseNo"].ToString();
                hdnmobileno.Value = ds.Tables[0].Rows[0]["MobileNo"].ToString();
                hdnsdid.Value = ds.Tables[0].Rows[0]["SDID"].ToString();
                txtGoldNo.Text = ds.Tables[0].Rows[0]["GoldLoanNo"].ToString();
                txtLoanDate.Text = ds.Tables[0].Rows[0]["LoanDate"].ToString();
                txtScheme.Text = ds.Tables[0].Rows[0]["SchemeName"].ToString();
                txtROI.Text = ds.Tables[0].Rows[0]["ROI"].ToString();
                txtLoanAmount.Text = ds.Tables[0].Rows[0]["NetLoanAmtSanctioned"].ToString();
                lblPrincipalCurrent.Text = ds.Tables[0].Rows[0]["CLP"].ToString();

                lblInterestCurrent.Text = ds.Tables[0].Rows[0]["CLI"].ToString();
                lblPenalCurrent.Text = ds.Tables[0].Rows[0]["CLPI"].ToString();
                lblChargesCurrent.Text = ds.Tables[0].Rows[0]["CLC"].ToString();
                lblCurrentTotal.Text = ds.Tables[0].Rows[0]["CLO"].ToString();

                ddlPaymentMode.SelectedValue = ds.Tables[0].Rows[0]["PaymentMode"].ToString().Trim();
                ddlPaymentMode_SelectedIndexChanged(sender, e);

                txtPrincipalCurrentAmt.Text = ds.Tables[0].Rows[0]["RcvCLP"].ToString();
                // added by ap
                hdnddlPrincipalCurrentAcHead.Value = ds.Tables[0].Rows[0]["RecCLPAccID"].ToString();
                ddlPrincipalCurrentAcHead.SelectedValue = ds.Tables[0].Rows[0]["RecCLPAccID"].ToString();
                ddlPrincipalCurrentNarration.SelectedValue = ds.Tables[0].Rows[0]["RecCLPNarID"].ToString();

                txtInterestCurrentAmt.Text = ds.Tables[0].Rows[0]["RcvCLI"].ToString();

                hdnddlInterestCurrentAcHead.Value = ds.Tables[0].Rows[0]["RecCLIAccID"].ToString();
                ddlInterestCurrentAcHead.SelectedValue = ds.Tables[0].Rows[0]["RecCLIAccID"].ToString();
                ddlInterestCurrentNarration.SelectedValue = ds.Tables[0].Rows[0]["RecCLINarID"].ToString();

                txtPenalCurrentAmt.Text = ds.Tables[0].Rows[0]["RcvCLPI"].ToString();

                hdnddlPenalCurrentAcHead.Value = ds.Tables[0].Rows[0]["RecCLPIAccID"].ToString();
                ddlPenalCurrentAcHead.SelectedValue = ds.Tables[0].Rows[0]["RecCLPIAccID"].ToString();
                ddlPenalCurrentNarration.SelectedValue = ds.Tables[0].Rows[0]["RecCLPINarID"].ToString();

                txtChargesCurrentAmt.Text = ds.Tables[0].Rows[0]["RecCLC"].ToString();

                hdnddlChargesCurrentAcHead.Value = ds.Tables[0].Rows[0]["RecCLCAccID"].ToString();
                ddlChargesCurrentAcHead.SelectedValue = ds.Tables[0].Rows[0]["RecCLCAccID"].ToString();
                ddlChargesCurrentNarration.SelectedValue = ds.Tables[0].Rows[0]["RecCLCNarID"].ToString();
                txtRcvTotal.Text = ds.Tables[0].Rows[0]["RcvTotal"].ToString();

                if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[0]["InterestFromDate"].ToString())) && ds.Tables[0].Rows[0]["InterestFromDate"].ToString() != "")
                {

                    txtIntFromDate.Text = ds.Tables[0].Rows[0]["InterestFromDate"].ToString();
                }
                else
                {
                    txtIntFromDate.Text = ds.Tables[0].Rows[0]["LoanDate"].ToString();
                }
                txtIntToDate.Text = ds.Tables[0].Rows[0]["InterestToDate"].ToString();
                txtAdvInterest.Text = ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString();

                if (Convert.ToDecimal(ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString()) == 0)
                {
                    DateTime rcDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["ReceiveDate"].ToString());

                    DateTime endOfMonthf = new DateTime(rcDate.Year, rcDate.Month, DateTime.DaysInMonth(rcDate.Year, rcDate.Month));

                    if (rcDate == endOfMonthf)
                    {
                        txtAdvIntFrom.Text = txtAdvIntTo.Text = rcDate.ToShortDateString();
                    }
                    else
                    {

                        txtAdvIntFrom.Text = rcDate.AddDays(1).ToShortDateString();

                        // DateTime today = DateTime.Today;
                        DateTime endOfMonth = new DateTime(rcDate.Year, rcDate.Month, DateTime.DaysInMonth(rcDate.Year, rcDate.Month));
                        txtAdvIntTo.Text = endOfMonth.ToShortDateString();
                    }
                    txtAdvInterest.ReadOnly = false;
                }

                else
                {
                    txtAdvIntFrom.Text = ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString();
                    txtAdvIntTo.Text = ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString();

                    hdnddlAdvIntAcHead.Value = ds.Tables[0].Rows[0]["AdvInterestAccID"].ToString();
                    ddlAdvIntAcHead.SelectedValue = ds.Tables[0].Rows[0]["AdvInterestAccID"].ToString();
                    // txtAdvInterest.ReadOnly = false;
                }

                if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[0]["lAdvInterestToDate"].ToString())) && ds.Tables[0].Rows[0]["lAdvInterestToDate"].ToString() != "")
                {
                    if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString())) && ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString() != "")
                    {
                        if (Convert.ToDateTime(txtRecvDate.Text) <= Convert.ToDateTime(ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString()))
                        {
                            txtAdvInterest.ReadOnly = false; ddlAdvIntAcHead.Enabled = false;
                        }

                        else if (Convert.ToDateTime(ds.Tables[0].Rows[0]["lAdvInterestToDate"].ToString()) > Convert.ToDateTime(ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString()))
                        {
                            txtAdvInterest.ReadOnly = true; ddlAdvIntAcHead.Enabled = true;
                        }
                        else
                        {
                            txtAdvInterest.ReadOnly = false; ddlAdvIntAcHead.Enabled = false;
                        }

                    }
                    else
                    {
                        txtAdvInterest.ReadOnly = true; ddlAdvIntAcHead.Enabled = true;
                    }
                }
                //ddlReceiptBook.SelectedValue = ds.Tables[0].Rows[0]["ReceiptBookNo"].ToString();
                //ddlReceiptBook_SelectedIndexChanged(sender, e);
                // ddlReceipt.SelectedValue = ds.Tables[0].Rows[0]["ReceiptNo"].ToString();

                // ddlReceiptBook.Items.Clear();
                //ddlReceiptBook.Items.Insert(0, new ListItem("--Select--", "0"));
                //ddlReceiptBook.Items.Add(ds.Tables[0].Rows[0]["ReceiptBookNo"].ToString());
                //ddlReceiptBook.SelectedIndex = 1;
                //ddlReceipt.Items.Clear();
                //ddlReceipt.Items.Insert(0, new ListItem("--Select--", "0"));
                //ddlReceipt.Items.Add(ds.Tables[0].Rows[0]["ReceiptNo"].ToString());
                // ddlReceipt.SelectedIndex = 1;
                txtReceiptBook.Text = ds.Tables[0].Rows[0]["ReceiptBookNo"].ToString();
                txtReceipt.Text = ds.Tables[0].Rows[0]["ReceiptNo"].ToString();

                lblBalanceLoanEligibilityAmount.Text = ds.Tables[0].Rows[0]["BalanceLoanEligibility"].ToString();
                txtRecvFrom.Text = ds.Tables[0].Rows[0]["ReceivedFrom"].ToString();
                ddlCollectedBy.SelectedValue = ds.Tables[0].Rows[0]["CollectedByID"].ToString().Trim();
                ddlCashier.SelectedValue = ds.Tables[0].Rows[0]["CashierId"].ToString().Trim();

                ddlBankAcc.SelectedValue = ds.Tables[0].Rows[0]["BankAccID"].ToString().Trim();
                ddlCashAcc.SelectedValue = ds.Tables[0].Rows[0]["CashAccID"].ToString().Trim();
                hdnplcaseno.Value = ds.Tables[0].Rows[0]["ExistingPLCaseNo"].ToString().Trim();
                //HdnAdvanceInterest.Value = ds.Tables[0].Rows[0]["TotalAdvInterestAmount"].ToString(); 31/7/2015

                //txtAdvInterest.Text = ds.Tables[0].Rows[0]["TotalAdvInterestAmount"].ToString();

                string LastInterestDate;
                if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[0]["LastInterestDate"].ToString())) && ds.Tables[0].Rows[0]["LastInterestDate"].ToString() != "")
                {
                    LastInterestDate = Convert.ToString(Convert.ToDateTime(ds.Tables[0].Rows[0]["LastInterestDate"].ToString()));
                }
                else
                {
                    LastInterestDate = txtRecvDate.Text;
                }

                //Added by Priya on 25-9-2015 for Prev Paid Interest
                string InterestFromDate = string.Empty;

                if (ds.Tables[0].Rows[0]["InterestFromDate"].ToString() != "" && ds.Tables[0].Rows[0]["InterestFromDate"].ToString() != null)
                {
                    InterestFromDate = ds.Tables[0].Rows[0]["InterestFromDate"].ToString();
                }

                string RvcCLI = string.Empty;

                if (ds.Tables[0].Rows[0]["RcvCLI"].ToString() != "" && ds.Tables[0].Rows[0]["RcvCLI"].ToString() != null)
                {
                    RvcCLI = ds.Tables[0].Rows[0]["RcvCLI"].ToString();
                }

                if (ds.Tables[0].Rows[0]["InterestFromDate"].ToString() != "" && ds.Tables[0].Rows[0]["InterestFromDate"].ToString() != null)
                {
                    InterestFromDate = ds.Tables[0].Rows[0]["InterestFromDate"].ToString();
                }
                else
                {
                    InterestFromDate = System.DateTime.Today.ToShortDateString();
                }

                if (ds.Tables[0].Rows[0]["InterestToDate"].ToString() != "" && ds.Tables[0].Rows[0]["InterestToDate"].ToString() != null)
                {
                    InterestToDate = ds.Tables[0].Rows[0]["InterestToDate"].ToString();
                }
                else
                {
                    InterestToDate = System.DateTime.Today.ToShortDateString();
                }


                if (ds.Tables[0].Rows[0]["RecvInterest"].ToString() != "" && ds.Tables[0].Rows[0]["RecvInterest"].ToString() != null)
                {
                    RvcCLI = ds.Tables[0].Rows[0]["RecvInterest"].ToString();
                }
                else
                {
                    RvcCLI = "0";
                }


                if (ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString() != "" && ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString() != null)
                {
                    AdvInterestFromDate = ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString();
                }
                else
                {
                    AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
                }

                if (ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString() != "" && ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString() != null)
                {
                    AdvInterestToDate = ds.Tables[0].Rows[0]["AdvInterestToDate"].ToString();
                }
                else
                {
                    AdvInterestToDate = System.DateTime.Today.ToShortDateString();
                }



                ////added by priya for Advance Interest Calculation----
                if (lblInterestCurrent.Text != "0.00")
                {
                    //if (Convert.ToDateTime(txtRecvDate.Text) >= Convert.ToDateTime(LastInterestDate)) //commented on31-7
                    //{
                    if (txtAdvIntFrom.Text != "")
                    {
                        cmd = new SqlCommand();
                        conn.Open();
                        cmd.Connection = conn;
                        // cmd.Transaction = transactionGL;
                        cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where  Isactive='Y' AND GoldLoanNo='" + ds.Tables[0].Rows[0]["GoldLoanNo"].ToString() + "'";
                        int neworoldds = 0;
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            neworoldds = Convert.ToInt32(cmd.ExecuteScalar());
                        }


                        //Added for to get ROI ROW ID on 3-10-2015
                        int RcptID = 0;
                        cmdRcpt = new SqlCommand();
                        cmdRcpt.Connection = conn;
                        cmdRcpt.Transaction = transactionGL;
                        cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails  where  Isactive='Y' AND GoldLoanNo='" + ds.Tables[0].Rows[0]["GoldLoanNo"].ToString() + "'";
                        if (cmdRcpt.ExecuteScalar() != DBNull.Value)
                        {
                            RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
                        }

                        int RowRoiID = 1;
                        //Check if Advance Interest is paid then Pass top 1 RowID
                        if (Convert.ToDecimal(ds.Tables[0].Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString()) == 0)
                        {
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            // cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[0]["SID"].ToString();
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }
                        else if (Convert.ToDecimal(ds.Tables[0].Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString()) > 0)
                        {
                            //Check if Half Interest is paid then Pass Max RowID
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            //cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
                            cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[0]["SID"].ToString();
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }
                        else if (Convert.ToDecimal(ds.Tables[0].Rows[0]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString()) == 0)
                        {
                            cmdRoiRow = new SqlCommand();
                            cmdRoiRow.Connection = conn;
                            cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                            if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                            {
                                RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                            }
                        }

                        ////Check if Advance Interest is paid then Pass top 1 RowID
                        //if (!(System.Convert.IsDBNull(ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString())) && ds.Tables[0].Rows[0]["AdvInterestFromDate"].ToString() != "")
                        //{
                        //    cmdRoiRow = new SqlCommand();
                        //    cmdRoiRow.Connection = conn;
                        //    cmdRoiRow.Transaction = transactionGL;
                        //    cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
                        //    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                        //    {
                        //        RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                        //    }
                        //}
                        //else
                        //{
                        //    //Check if Interest is paid then Pass Max RowID
                        //    cmdRoiRow = new SqlCommand();
                        //    cmdRoiRow.Connection = conn;
                        //    cmdRoiRow.Transaction = transactionGL;
                        //    cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                        //    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                        //    {
                        //        RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                        //    }
                        //}


                        SqlCommand cmdAdv = new SqlCommand();
                        cmdAdv.Connection = conn;
                        // cmdAdv.Transaction = transactionGL;
                        cmdAdv.CommandText = "GL_EmiCalculator_RTR";
                        cmdAdv.CommandType = CommandType.StoredProcedure;

                        cmdAdv.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[0]["CustLoanDate"].ToString()));
                        cmdAdv.Parameters.AddWithValue("@LoanAmount", ds.Tables[0].Rows[0]["NetLoanAmtSanctioned"].ToString());
                        cmdAdv.Parameters.AddWithValue("@SID", ds.Tables[0].Rows[0]["SID"].ToString());
                        cmdAdv.Parameters.AddWithValue("@NeworOld", neworoldds);

                        cmdAdv.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmdAdv.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmdAdv.Parameters.AddWithValue("@PaidInt", RvcCLI);

                        cmdAdv.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                        cmdAdv.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                        cmdAdv.Parameters.AddWithValue("@OSIntAmt", ds.Tables[0].Rows[0]["OSIntAmt"].ToString());

                        cmdAdv.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                        cmdAdv.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                        //cmdAdv.Parameters.AddWithValue("@AdvInterestAmt", ds.Tables[0].Rows[0]["AdvInterestAmount"].ToString());
                        cmdAdv.Parameters.AddWithValue("@AdvInterestAmt", 0);

                        cmdAdv.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text));
                        cmdAdv.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text));
                        cmdAdv.Parameters.AddWithValue("@LastROIID", RowRoiID);

                        SqlDataAdapter daAdv = new SqlDataAdapter(cmdAdv);
                        DataSet dsAdv = new DataSet();
                        daAdv.Fill(dsAdv);

                        double TotalAdv = 0;

                        for (int i = 0; i < dsAdv.Tables[0].Rows.Count; i++)
                        {
                            if (dsAdv.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
                            {
                                TotalAdv = Convert.ToDouble(TotalAdv) + Convert.ToDouble(dsAdv.Tables[0].Rows[i]["InterestAmount"].ToString());
                            }
                        }
                        HdnAdvanceInterest.Value = Convert.ToString(Math.Round(TotalAdv));
                    }
                    // }
                }
                ////end of advance---


            }

            if (ds.Tables[1].Rows.Count > 0)
            {
                gvChequeDetails.DataSource = ds.Tables[1];
                gvChequeDetails.DataBind();
                BindBank();
                TextBox gvtxtChqTotal = (TextBox)gvChequeDetails.FooterRow.FindControl("gvtxtChqTotal");
                gvtxtChqTotal.Text = ds.Tables[1].Rows[0]["TAmount"].ToString();
            }
            else
            {
                BindChequeDetails();
            }

            if (ds.Tables[2].Rows.Count > 0)
            {
                gvDenominationDetails.DataSource = ds.Tables[2];
                gvDenominationDetails.DataBind();

                TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                gvtxtDenoTotalAmt.Text = ds.Tables[2].Rows[0]["DenoTotal"].ToString();
            }
            else
            {
                BindDenominationDetails();
            }
        }


        ////added by priya for Advance Interest Calculation----
        //if (txtAdvIntFrom.Text != "")
        //{
        //    cmd = new SqlCommand();
        //    cmd.Connection = conn;
        //    cmd.Transaction = transactionGL;
        //    cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where  GoldLoanNo='" + ds.Tables[0].Rows[0]["GoldLoanNo"].ToString() + "'";
        //    int neworoldds = 0;
        //    if (cmd.ExecuteScalar() != DBNull.Value)
        //    {
        //        neworoldds = Convert.ToInt32(cmd.ExecuteScalar());
        //    }


        //    SqlCommand cmdAdv = new SqlCommand();
        //    cmdAdv.Connection = conn;
        //    cmdAdv.CommandType = CommandType.StoredProcedure;
        //    cmdAdv.CommandText = "GL_InterestCalculation_RTR"; //InterestToDate        
        //    cmdAdv.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(txtAdvIntFrom.Text));
        //    cmdAdv.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(txtAdvIntTo.Text));
        //    cmdAdv.Parameters.AddWithValue("@LoanAmount", ds.Tables[0].Rows[0]["LoanAmount"].ToString());
        //    // cmd.Parameters.AddWithValue("@OSInt", "0");//dt.Rows[0]["OSInterest"].ToString()
        //    cmdAdv.Parameters.AddWithValue("@OSInt", ds.Tables[0].Rows[0]["OSInt"].ToString());//dt.Rows[0]["OSInterest"].ToString()
        //    cmdAdv.Parameters.AddWithValue("@SID", ds.Tables[0].Rows[0]["SID"].ToString());
        //    cmdAdv.Parameters.AddWithValue("@NeworOld", neworoldds);


        //    SqlDataAdapter daAdv = new SqlDataAdapter(cmdAdv);
        //    DataSet dsAdv = new DataSet();
        //    daAdv.Fill(dsAdv);

        //    double TotalAdv = 0;

        //    for (int i = 0; i < dsAdv.Tables[0].Rows.Count; i++)
        //    {
        //        if (dsAdv.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
        //        {
        //            TotalAdv = Convert.ToDouble(TotalAdv) + Convert.ToDouble(dsAdv.Tables[0].Rows[i]["InterestAmount"].ToString());
        //        }
        //    }
        //    HdnAdvanceInterest.Value = Convert.ToString(Math.Round(TotalAdv));
        //}
        ////end of advance---
        BindPlCases_Details(hdnplcaseno.Value.Trim());
    }

    //---------Clear Receipt Data
    public void ClearData()
    {
        txtLoanDate.Text = "";
        txtGoldNo.Text = "";
        txtScheme.Text = "";
        txtROI.Text = "";
        txtLoanAmount.Text = "";
        lblPrincipalCurrent.Text = "0";
        lblInterestCurrent.Text = "0";
        lblPenalCurrent.Text = "0";
        lblChargesCurrent.Text = "0";
        lblCurrentTotal.Text = "0";
        lblPlLoanAmt.Text = "0";
        lblPlDPEMI.Text = "0";
        lblPlOsEMI.Text = "0";
        lblPlOsPrincipal.Text = "";
        lblPlOsDues.Text = "0";
        lblPlLastPdcDate.Text = "";
        txtPrincipalCurrentAmt.Text = "";
        txtInterestCurrentAmt.Text = "";
        txtPenalCurrentAmt.Text = "";
        txtChargesCurrentAmt.Text = "";

        hdnddlPrincipalCurrentAcHead.Value = "0";
        //  ddlPrincipalCurrentAcHead.Text = "";

        hdnddlInterestCurrentAcHead.Value = "0";
        // ddlInterestCurrentAcHead.SelectedIndex = 0;

        hdnddlPenalCurrentAcHead.Value = "0";
        //  ddlPenalCurrentAcHead.SelectedIndex = 0;

        hdnddlChargesCurrentAcHead.Value = "0";

        //hdnddlPrincipalCurrentNarration.Value = "0";
        //hdnddlInterestCurrentNarration.Value = "0";
        //hdnddlPenalCurrentNarration.SelectedIndex = 0;
        //hdnddlChargesCurrentNarration.SelectedIndex = 0;

        //ddlChargesCurrentAcHead.SelectedIndex = 0;
        ddlPrincipalCurrentNarration.SelectedIndex = 0;
        ddlInterestCurrentNarration.SelectedIndex = 0;
        ddlPenalCurrentNarration.SelectedIndex = 0;
        ddlChargesCurrentNarration.SelectedIndex = 0;

        BindChequeDetails();
        BindDenominationDetails();

        // ddlReceiptBook.SelectedIndex = 0;
        // ddlReceipt.Items.Clear();
        // ddlReceipt.Items.Insert(0, new ListItem("--Select Receipt--", "0"));
        txtReceiptBook.Text = "";
        txtReceipt.Text = "";
        lblBalanceLoanEligibilityAmount.Text = "0";
        txtRecvFrom.Text = "";
        ddlCollectedBy.SelectedIndex = 0;
        ddlCashier.SelectedIndex = 0;
        hdnid.Value = "0";
        hdnoperation.Value = "Save";
        hdnplcaseno.Value = "0";
        hdnpopup.Value = "Save";
        hdnkycid.Value = "0";
        hdnsdid.Value = "0";
        hdnpopup.Value = "0";
        //GetCurrentDate();
        BindReceiptBookNo();
        ddlCashAcc.SelectedIndex = 0;
        ddlBankAcc.SelectedIndex = 0;
        txtRcvTotal.Text = "";
        ddlPaymentMode.SelectedIndex = 0;
        btnGlSearch.Enabled = true;
        hdnmobileno.Value = "0";

        txtAdvInterest.Text = ""; txtIntFromDate.Text = ""; txtIntToDate.Text = ""; txtAdvIntFrom.Text = ""; txtAdvIntTo.Text = "";

        hdnddlAdvIntAcHead.Value = "0";
        //  ddlAdvIntAcHead.SelectedIndex = 0;

        ddlAdvIntNarration.SelectedIndex = 0;


    }

    #region [Send_Mobile_Messages]
    public void Send_Message(string MobileNo, string GoldLoanNo)
    {
        try
        {
            string Message = string.Empty;
            string sURL;
            string Success = string.Empty;

            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
            {
                for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
                {
                    gvChequeDetails.SelectedIndex = i;
                    DropDownList gvddlChqDDNeft = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqDDNeft");
                    TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
                    TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");

                    if (gvddlChqDDNeft.SelectedIndex == 0)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your payment of Rs." + gvtxtChqAmount.Text + " towards " + GoldLoanNo + " has been received by Cheque vide Cheque No." + gvtxtChqNo.Text + " Thankyou.";

                    }
                    if (gvddlChqDDNeft.SelectedIndex == 1)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your payment of Rs." + gvtxtChqAmount.Text + " towards " + GoldLoanNo + " has been received by DD vide DD No." + gvtxtChqNo.Text + " Thankyou.";

                    }
                    if (gvddlChqDDNeft.SelectedIndex == 2)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your payment of Rs." + gvtxtChqAmount.Text + " towards " + GoldLoanNo + " has been received by NEFT vide NEFT Transaction No." + gvtxtChqNo.Text + " Thankyou.";

                    }

                }
                StreamReader objReader;
                sURL = Message;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                objReader = new StreamReader(objStream);
                objReader.Close();



            }

            //if (ddlPaymentMode.SelectedIndex == 2)
            //{
            //    string CashTotal = txtRcvTotal.Text;

            //    Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message= Dear Customer, your payment of Rs." + CashTotal + " towards " + GoldLoanNo + " has been received by Cash on " + txtRecvDate.Text + " vide Cash Receipt No. " + txtReceipt.Text + ". Thankyou.";
            //}

            if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
            {
                string CashTotal = string.Empty;
                if (ddlPaymentMode.SelectedIndex == 2)
                {
                    CashTotal = txtRcvTotal.Text;
                }
                else
                {
                    CashTotal = ((TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt")).Text;
                }

                //   Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your payment of Rs." + CashTotal + " towards " + GoldLoanNo + " has been received by Cash on " + txtRecvDate.Text + " vide Cash Receipt No. " + txtReceipt.Text + " Thankyou.";

                Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your payment of Rs." + CashTotal + " towards " + GoldLoanNo + " has been received by Cash on " + txtRecvDate.Text + " vide Cash Receipt No. " + txtReceipt.Text + ". Thankyou.";

                StreamReader objReader;
                sURL = Message;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                objReader = new StreamReader(objStream);
                objReader.Close();
            }

            if (Convert.ToDouble(lblPrincipalCurrent.Text) - Convert.ToDouble(txtPrincipalCurrentAmt.Text) <= 0)
            {
                Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan A/c No. " + GoldLoanNo + " with us has been repaid in full. Thankyou for choosing Aphelion Finance.";

                StreamReader objReader;
                sURL = Message;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                objReader = new StreamReader(objStream);
                objReader.Close();
            }


            //StreamReader objReader;
            //sURL = Message;
            //WebRequest wrGETURL;
            //wrGETURL = WebRequest.Create(sURL);

            //Stream objStream;
            //objStream = wrGETURL.GetResponse().GetResponseStream();
            //objReader = new StreamReader(objStream);
            //objReader.Close();
        }
        catch (Exception ex)
        {

        }
    }
    #endregion [Send_Mobile_Messages]
    //-----------------------------Controls Events of Master Page
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            GLReceipt_RTR();

            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Received Date");
            //  Master.PropertyddlSearch.Items.Add("Received Time");
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("PAN No");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";
            hdnpopup.Value = "Edit";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }

    }

    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {


            if (hdnoperation.Value == "Save")
            {
                GLReceipt_PRV("Save", "0");
                GLReceipt_PRI("Save", "0");

            }
            if (hdnoperation.Value == "Update")
            {
                GLReceipt_PRV("Update", hdnid.Value.Trim());
                GLReceipt_PRI("Update", hdnid.Value.Trim());

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        {


        }
    }

    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GLReceipt_PRV("Delete", hdnid.Value.Trim());
            GLReceipt_PRI("Delete", hdnid.Value.Trim());
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }

    }


    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnpopup.Value == "GoldLoan")
            {
                GoldLoanNo_Search();

            }
            if (hdnpopup.Value == "Edit")
            {
                GLReceipt_Search();

            }
            if (hdnpopup.Value == "View")
            {
                GLReceipt_Search();

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GLReceipt_RTR();
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Received Date");
            Master.PropertyddlSearch.Items.Add("Received Time");
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("PAN No");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";
            hdnpopup.Value = "View";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {

        //string a = Convert.ToString(hdnddlPrincipalCurrentAcHead.Value);
        //string bb = Convert.ToString(hdnddlInterestCurrentAcHead.Value);
        //string cc = Convert.ToString(hdnddlPenalCurrentAcHead.Value);

        try
        {
            // string s = Request[ddlInterestCurrentAcHead.UniqueID];
            Response.Redirect("GLReceiptForm.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
            hdnKycidForAutoInt.Value = id;
            //conn = new SqlConnection(strConnString);
            //cmd = new SqlCommand();
            //cmd.Connection = conn;
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.CommandText = "GL_IntCal_Loan_RTR";
            //cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
            //cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));

            //da = new SqlDataAdapter(cmd);
            //dt = new DataTable();
            //da.Fill(dt);

            //   BindAccount();
            BindNarration();

            if (hdnpopup.Value.Trim() == "GoldLoan")
            {
                GoldLoanDetails_RTR(id);
                hdnpopup.Value = "Search";
            }
            else if (hdnpopup.Value == "Edit")
            {
                GLReceiptDetails_RTR(id, sender, e);
            }
            else if (hdnpopup.Value == "View")
            {
                GLReceiptDetails_RTR(id, sender, e);
            }
            gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    //-----------------------------Controls Events
    protected void btnChqAdd_Click(object sender, EventArgs e)
    {
        try
        {
            AddChequeDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void btnDelete_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            double chqtotal = 0;
            if (gvChequeDetails.Rows.Count == 1)
            {
                BindChequeDetails();
                return;

            }
            ImageButton btnDelete = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btnDelete.NamingContainer;
            int index = row.RowIndex;


            DataRow dr = null;
            dt = new DataTable();
            dt.Columns.Add("ChequeId");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("Chq_DD_NEFT");
            dt.Columns.Add("ChequeNo");
            dt.Columns.Add("ChequeDate");
            dt.Columns.Add("BankId");
            dt.Columns.Add("Amount");
            for (int i = 0; i < gvChequeDetails.Rows.Count; i++)
            {
                gvChequeDetails.SelectedIndex = i;

                if (i != index)
                {

                    HiddenField hdnchqid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqid");
                    TextBox gvtxtChqSrno = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqSrno");
                    TextBox gvtxtChqNo = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqNo");
                    TextBox gvtxtChqDate = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqDate");
                    DropDownList gvddlChqBank = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqBank");
                    HiddenField hdnbankid = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnbankid");
                    TextBox gvtxtChqAmount = (TextBox)gvChequeDetails.SelectedRow.FindControl("gvtxtChqAmount");
                    DropDownList gvddlChqDDNeft = (DropDownList)gvChequeDetails.SelectedRow.FindControl("gvddlChqDDNeft");
                    HiddenField hdnchqddneft = (HiddenField)gvChequeDetails.SelectedRow.FindControl("hdnchqddneft");


                    dr = dt.NewRow();
                    dr["ChequeId"] = hdnchqid.Value.Trim();
                    dr["Serialno"] = gvtxtChqSrno.Text;
                    dr["Chq_DD_NEFT"] = gvddlChqDDNeft.SelectedValue.Trim();
                    dr["ChequeNo"] = gvtxtChqNo.Text;
                    dr["ChequeDate"] = gvtxtChqDate.Text;
                    dr["BankId"] = gvddlChqBank.SelectedValue.Trim();
                    dr["Amount"] = gvtxtChqAmount.Text;
                    dt.Rows.Add(dr);
                    chqtotal = chqtotal + Convert.ToDouble(gvtxtChqAmount.Text);
                }
            }
            gvChequeDetails.DataSource = dt;
            gvChequeDetails.DataBind();

            TextBox gvtxtChqTotal = (TextBox)gvChequeDetails.FooterRow.FindControl("gvtxtChqTotal");
            gvtxtChqTotal.Text = chqtotal.ToString();
            //---Call function for binding bank
            BindBank();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void btnDenoAdd_Click(object sender, EventArgs e)
    {
        try
        {
            AddDenomination();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void btnDenoDelete_Click(object sender, ImageClickEventArgs e)
    {

        try
        {
            if (gvDenominationDetails.Rows.Count == 1)
            {
                BindDenominationDetails();
                return;
            }
            ImageButton btnDenoDelete = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btnDenoDelete.NamingContainer;
            int index = row.RowIndex;

            double denototal = 0;
            DataRow dr = null;
            dt = new DataTable();
            dt.Columns.Add("InOutID");
            dt.Columns.Add("RefNo");
            dt.Columns.Add("DenoId");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("DenoRs");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Total");
            dt.Columns.Add("NoteNos");

            for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
            {
                gvDenominationDetails.SelectedIndex = i;

                if (i != index)
                {
                    HiddenField hdncashinoutid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdncashinoutid");
                    HiddenField hdnrefno = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdnrefno");
                    HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
                    TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
                    TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
                    TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
                    TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
                    TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");

                    dr = dt.NewRow();

                    dr["InOutID"] = hdncashinoutid.Value.Trim();
                    dr["RefNo"] = hdnrefno.Value.Trim();
                    dr["DenoId"] = hdndenoid.Value.Trim();
                    dr["Serialno"] = gvtxtDenoSrno.Text;
                    dr["DenoRs"] = gvtxtDenoDescription.Text;
                    dr["Quantity"] = gvtxtDenoNo.Text;
                    dr["Total"] = gvtxtDenoTotal.Text;
                    dr["NoteNos"] = gvtxtDenoNoteNos.Text;
                    dt.Rows.Add(dr);
                    denototal = denototal + Convert.ToDouble(gvtxtDenoTotal.Text);
                }
            }
            gvDenominationDetails.DataSource = dt;
            gvDenominationDetails.DataBind();

            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            gvtxtDenoTotalAmt.Text = denototal.ToString();
            for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
            {
                gvDenominationDetails.SelectedIndex = i;

                TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
                gvtxtDenoSrno.Attributes.Add("readonly", "readonly");
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void btnGlSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            GoldNo_RTR();
            //  BindChequeDetails(); //comment by priya for loading slow form
            BindDenominationDetails();
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Loan Type");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("PAN No");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";
            hdnpopup.Value = "GoldLoan";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void ddlReceiptBook_SelectedIndexChanged(object sender, EventArgs e)
    {
        //try
        //{
        //    ddlReceipt.Items.Clear();
        //    connAIM = new SqlConnection(strConnStringAIM);
        //    cmd = new SqlCommand();
        //    cmd.Connection = connAIM;
        //    //cmd.CommandText = "select distinct  ReceiptNo From tblINV_ReceiptBook_InOut_Details " +
        //    //                    "where ReceiptNo not in(select top 1 ReceiptNo From tblINV_ReceiptBook_InOut_Details " +
        //    //                    "where Status='Out') and BookSerialNo='" + ddlReceiptBook.SelectedValue.Trim() + "' " +
        //    //                    "and FinancialYrID='" + hdnfyid.Value + "' order by ReceiptNo";


        //    cmd.CommandText = "select distinct  ReceiptNo From tblINV_ReceiptBook_InOut_Details ";
        //                     //"where ReceiptNo not in(select top 1 ReceiptNo From tblINV_ReceiptBook_InOut_Details " +
        //                    // "where Status='Out') and BookSerialNo='" + ddlReceiptBook.SelectedValue.Trim() + "' " +
        //                     //"and FinancialYrID='" + hdnfyid.Value + "' order by ReceiptNo";
        //    da = new SqlDataAdapter(cmd);
        //    dt = new DataTable();
        //    da.Fill(dt);
        //    ddlReceipt.DataSource = dt;
        //    ddlReceipt.DataTextField = "ReceiptNo";
        //    ddlReceipt.DataValueField = "ReceiptNo";
        //    ddlReceipt.DataBind();
        //    ddlReceipt.Items.Insert(0, new ListItem("--Select--", "0"));
        //}
        //catch (Exception ex)
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        //}
    }

    protected void ddlPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {
            BindChequeDetails();
            BindDenominationDetails();
            ddlBankAcc.SelectedIndex = 0;
            ddlCashAcc.SelectedIndex = 0;


            if (ddlPaymentMode.SelectedIndex == 0)
            {

                pnlchequedetails.Enabled = false;
                pnlcashdetails.Enabled = false;

                return;
            }
            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 4)
            {
                pnlchequedetails.Enabled = true;

            }
            else
            {
                pnlchequedetails.Enabled = false;

            }

            if (ddlPaymentMode.SelectedIndex == 2)
            {
                pnlcashdetails.Enabled = true;

                BindReceiptBookNo();
            }
            else
            {
                pnlcashdetails.Enabled = false;

            }
            if (ddlPaymentMode.SelectedIndex == 3)
            {
                pnlchequedetails.Enabled = true;
                pnlcashdetails.Enabled = true;
                txtReceiptBook.Text = "";
                txtReceipt.Text = "";
                // BindReceiptBookNo();
            }


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void gvChequeDetails_gvddlChqDDNeft_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    protected void btnClearOnDate_Click(object sender, EventArgs e)
    {
        try
        {
            ClearData();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    //Added by Priya for Auto Interest Calc
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static double GetAutoInterestCal(string SelRecvDate, string SelIntDate, string SelIntToDate, string hdnKYCID, string hdnFyID, string hdnBranchID)
    {
        string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
        SqlConnection conn = new SqlConnection(strConnString);
        SqlCommand cmd, cmdRcpt, cmdRoiRow;
        SqlDataAdapter da;
        DataTable dt;
        DataSet ds;
        GlobalSettings gbl = new GlobalSettings();

        string InterestFromDate = string.Empty;
        string InterestToDate = string.Empty;
        string RvcCLI = string.Empty;
        string AdvInterestFromDate = string.Empty;
        string AdvInterestToDate = string.Empty;

        int neworold = 0;
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_GoldLoanDetails_RTR_New";
        cmd.Parameters.AddWithValue("@RcvDate", gbl.ChangeDateMMddyyyy(SelRecvDate));
        cmd.Parameters.AddWithValue("@KYCID", hdnKYCID);
        cmd.Parameters.AddWithValue("@FYID", hdnFyID);
        cmd.Parameters.AddWithValue("@BranchId", hdnBranchID);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        double total = 0;
        string ROI = "0";

        if (dt.Rows.Count > 0)
        {
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";

            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                neworold = Convert.ToInt32(cmd.ExecuteScalar());
            }

            //Added for to get ROI ROW ID on 3-10-2015
            int RcptID = 0;
            int RowRoiID = 1;
            cmdRcpt = new SqlCommand();
            cmdRcpt.Connection = conn;

            cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where Isactive='Y' AND   GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";
            if (cmdRcpt.ExecuteScalar() != DBNull.Value)
            {
                RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
            }

            //Check if Advance Interest is paid then Pass top 1 RowID

            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) > 0)
            {
                //Check if Half Interest is paid then Pass Max RowID
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }



            //Removed on 02-10-2015
            string LastDate;
            LastDate = dt.Rows[0]["LastReceiveDate"].ToString();

            if (dt.Rows[0]["InterestFromDate"].ToString() != "" && dt.Rows[0]["InterestFromDate"].ToString() != null)
            {
                InterestFromDate = dt.Rows[0]["InterestFromDate"].ToString();
            }
            else
            {
                InterestFromDate = System.DateTime.Today.ToShortDateString();
            }

            if (dt.Rows[0]["InterestToDate"].ToString() != "" && dt.Rows[0]["InterestToDate"].ToString() != null)
            {
                InterestToDate = dt.Rows[0]["InterestToDate"].ToString();
            }
            else
            {
                InterestToDate = System.DateTime.Today.ToShortDateString();
            }


            if (dt.Rows[0]["RecvInterest"].ToString() != "" && dt.Rows[0]["RecvInterest"].ToString() != null)
            {
                RvcCLI = dt.Rows[0]["RecvInterest"].ToString();
            }
            else
            {
                RvcCLI = "0";
            }

            if (dt.Rows[0]["AdvInterestFromDate"].ToString() != "" && dt.Rows[0]["AdvInterestFromDate"].ToString() != null)
            {
                AdvInterestFromDate = dt.Rows[0]["AdvInterestFromDate"].ToString();
            }
            else
            {
                AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
            }

            if (dt.Rows[0]["AdvInterestToDate"].ToString() != "" && dt.Rows[0]["AdvInterestToDate"].ToString() != null)
            {
                AdvInterestToDate = dt.Rows[0]["AdvInterestToDate"].ToString();
            }
            else
            {
                AdvInterestToDate = System.DateTime.Today.ToShortDateString();
            }

            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_EmiCalculator_RTR";

            cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["CustLoanDate"].ToString()));
            // cmd.Parameters.AddWithValue("@LoanAmount", dt.Rows[0]["LoanAmout"].ToString());
            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) >= 0)
            {
                double AddPrvInt = Math.Round(Convert.ToDouble(dt.Rows[0]["LoanAmout"].ToString()) + Convert.ToDouble(dt.Rows[0]["OSIntAmt"].ToString()));
                cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
            }
            else
            {
                cmd.Parameters.AddWithValue("@LoanAmount", (dt.Rows[0]["LoanAmout"].ToString()));
            }

            cmd.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
            cmd.Parameters.AddWithValue("@NeworOld", neworold);

            cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

            cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[0]["OSIntAmt"].ToString());

            cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
            cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
            cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[0]["AdvInterestAmount"].ToString());

            cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
            cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(SelIntToDate));
            cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            DataTable dtt = new DataTable();
            dtt.Columns.Add("CLI");
            dtt.Columns.Add("ROI");


            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (ds.Tables[0].Rows[i]["InterestAmount"] != DBNull.Value)
                {
                    total = total + Convert.ToDouble(ds.Tables[0].Rows[i]["InterestAmount"].ToString());
                    ROI = ds.Tables[0].Rows[i]["ROI"].ToString();
                }

            }

        }

        return Math.Round(total);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public IList<Account> GetAccount()
    {
        string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
        SqlConnection connAIM;
        SqlCommand cmd;
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select Name,AccountID from tblAccountmaster";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        var list = dt.AsEnumerable().Select(dataRow => new Account
        {
            AccountId = dataRow.Field<int>("AccountID"),
            Name = dataRow.Field<string>("Name"),

        }).ToList();
        return list;
    }

    public class Account
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
    }

    public void ddlInterestCurrentAcHead_SelectedIndexChanged(object sender, EventArgs e)
    {
        int a = 0;

        a = ddlInterestCurrentAcHead.SelectedIndex;
    }

}