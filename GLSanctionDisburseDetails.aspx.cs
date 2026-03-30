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
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;
using System.Text;
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;
using System.Web.Services;
using System.Web.Script.Services;
public partial class GLSanctionDisburseDetails : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    //creating instance of class "CompanyWiseAccountClosing"
    GlobalSettings gbl = new GlobalSettings();
    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

    //Declaring Variables.   
    string[] TenureEmi;
    string[] TenureMon;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = string.Empty;
    string GoldLoanNo = string.Empty;
    string GoldNo = string.Empty;
    string UserName = string.Empty;
    string Password = string.Empty;
    bool datasaved = false;
    public string loginDate;
    public string expressDate;
    int result = 0;
    int excount = 0;
    string Narration = "";
    int ContraAccID = 0;
    string customerId = "";
    string InterestFromDate = string.Empty;
    string InterestToDate = string.Empty;
    string RvcCLI = string.Empty;
    string AdvInterestFromDate = string.Empty;
    string AdvInterestToDate = string.Empty;
    //Declaring Objects.   
    SqlTransaction transactionGL, transactionAIM, transc;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd, cmdRcpt, cmdRoiRow;
    DataTable dt, dt1;
    string VaryInterest = "";
    double NewIRR = 0;
    double CurrIRR = 0;
    int intLETFDays = 0;
    double VariableIRR = 0.0;
    int counter = 0;
    int EMI = 0;
    SqlConnection con1 = new SqlConnection();
    DataTable dt2 = new DataTable();
    string strquery = string.Empty;
    int iircount = 0;

    FTPLIRR.IRRChartClass clsIRRChartCalculation = new FTPLIRR.IRRChartClass();

    #endregion [Declarations]



    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Click += new EventHandler(PropertybtnEdit_Click);
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnDelete.Click += new EventHandler(PropertybtnDelete_Click);
        Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
        Master.PropertyImgBtnClose.Click += new ImageClickEventHandler(PropertyImgBtnClose_Click);
        Master.PropertygvGlobal.RowCommand += new GridViewCommandEventHandler(PropertygvGlobal_RowCommand);
        //   Master.PropertygvGlobal.Sorting += new GridViewSortEventHandler(PropertygvGlobal_Sorting);      
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]


    protected void Page_PreRender(Object sender, EventArgs e)
    {
        txtCashAmount.Attributes.Add("autocomplete", "off");
    }

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }

            Master.PropertybtnSave.OnClientClick = "return valid();";

            if (!IsPostBack)
            {
                txtOperatorName.Text = Convert.ToString(Session["username"]);
                GetLoanDate();

                //This function is called to check and enable/disable Add/Edit/Save/Delete/view/cancel buttons of master page.
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

                txtLoanTenure.Attributes.Add("readonly", "readonly");
                txtMaxLoanAmount.Attributes.Add("readonly", "readonly");
                txtEligibleLoan.Attributes.Add("readonly", "readonly");
                txtEMI.Attributes.Add("readonly", "readonly");
                //  txtDueDate.Attributes.Add("readonly", "readonly");
                //txtLoanDate.Attributes.Add("readonly", "readonly");
                txtGoldLoanNo.Attributes.Add("readonly", "readonly");
                txtGender.Attributes.Add("readonly", "readonly");
                txtMaritalStatus.Attributes.Add("readonly", "readonly");
                txtBirthDate.Attributes.Add("readonly", "readonly");
                txtPanNo.Attributes.Add("readonly", "readonly");
                txtNetPayable.Attributes.Add("readonly", "readonly");

                //binding GridView Gold Item Details
                BindGoldItemDetails();

                //binding GridView Charges Details
                BindChargesDetails();

                //Scheme
                FillSchemeName();

                //Fill Bank/Cash Account
                FillBankCashAccount();

                BindDenominationDetails();

                txtFYID.Text = Session["FYearID"].ToString();
                txtBranchID.Text = Session["branchId"].ToString();
                txtCompID.Text = "1";

                // PnlDeno.Enabled = false;
                //ddlCashOutBy.Enabled = false;
                BindEmployee();
                txtNetAmountSanctioned.Enabled = false;
                ddlGoldInwardBy.Enabled = false;
                txtRackno.Enabled = false;
                dgvChargesDetails.Enabled = false;
                dgvGoldItemDetails.Enabled = false;
                // txtChequeNo.Enabled = false;
                // ddlcheqNEFTDD.Enabled = false;
                //txtChequeDate.Enabled = false;
                //  ddlCashOutBy.Enabled = false;
                GoldLoan_RTR();

                pnlBankAc.Enabled = false; // added by priya
                pnlCashAc.Enabled = false;

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Page_Load]


    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    #region [Sanction_Search]
    public void SanctionDisburse_Search()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_SanctionDisburse_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchValue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    #endregion [Sanction_Search]
    public void OutStanding(string kycid, string goldloanno)
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;

        //added by priya for popup Quries...********
        if (txtCustomerID.Text == "")
        {
            customerId = "0";
        }
        else
        {
            customerId = txtCustomerID.Text;
        }

        //conn = new SqlConnection(strConnString);
        conn.Open();
        // cmd = new SqlCommand();
        // cmd.Connection = conn;
        //  cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID WHERE sd.isActive='Y' AND KYC.CustomerID=" + txtCustomerID.Text;
        cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID WHERE sd.isActive='Y' AND KYC.CustomerID=" + customerId;
        SqlDataAdapter daLoanSD = new SqlDataAdapter(cmd);
        DataSet dtLoanSD = new DataSet();
        daLoanSD.Fill(dtLoanSD);
        double totalCash = 0;

        if (dtLoanSD.Tables[0].Rows.Count > 0)
        {
            for (int k = 0; k < dtLoanSD.Tables[0].Rows.Count; k++)
            {
                if (dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString() != "")
                {
                    //cashAmt = Convert.ToDecimal(dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString());
                    totalCash = Convert.ToDouble(totalCash) + Convert.ToDouble(dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString());
                }
            }
        }

        HiddenFieldTotalCash.Value = Convert.ToString(Math.Round(totalCash));
        //************


        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GLReceipt_GoldLoanDetails_RTR_New";
        cmd.Parameters.AddWithValue("@RcvDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));
        cmd.Parameters.AddWithValue("@KYCID", kycid);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        DataTable dtt = new DataTable();
        //DataRow dr = null;
        double totalInt = 0;


        double total = 0;
        int neworold = 0;
        int RowRoiID = 1;
        DataRow drr = null;
        //conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where  GoldLoanNo='" + goldloanno.Trim() + "'";

        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            neworold = Convert.ToInt32(cmd.ExecuteScalar());
        }
        //Added for to get ROI ROW ID on 3-10-2015
        int RcptID = 0;
        cmdRcpt = new SqlCommand();
        cmdRcpt.Connection = conn;
        cmdRcpt.Transaction = transactionGL;
        cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where Isactive='Y' AND  GoldLoanNo='" + goldloanno.Trim() + "'";
        if (cmdRcpt.ExecuteScalar() != DBNull.Value)
        {
            RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
        }


        if (dt.Rows.Count > 0)
        {
            //Check if Advance Interest is paid then Pass top 1 RowID
            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                // cmdRoiRow.CommandText = "select Top 1  isnull((ROIROWID),0) From TGLInterest_Details where ReceiptID=" + RcptID;
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


            string lastrec, today, todayDateTime;

            lastrec = dt.Rows[0]["LastReceiveDate"].ToString();

            todayDateTime = DateTime.Parse(lastrec).ToShortDateString();
            today = System.DateTime.Now.ToShortDateString();

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


            //if (Convert.ToDateTime(todayDateTime) > Convert.ToDateTime(today))
            //{
            //    cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(today));
            //}
            //else
            //{
            //    cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
            //}

            //// cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
            //cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));
            //cmd.Parameters.AddWithValue("@LoanAmount", dt.Rows[0]["LoanAmout"].ToString());
            //cmd.Parameters.AddWithValue("@OSInt", LastOSInterest);
            //cmd.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
            //cmd.Parameters.AddWithValue("@NeworOld", neworold);
            ////Parameter added by bharat on 14/09/2015 to calculate interest after  outstanding
            //cmd.Parameters.AddWithValue("@PrevOutstandingDate", gbl.ChangeDateMMddyyyy(PrvInterestDate));
            ////Parameter added by Priya on 25-9-2015 for prev paid interest Added
            //cmd.Parameters.AddWithValue("@PrevPaidInt", RvcCLI);


            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_EmiCalculator_RTR";

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
            cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));
            cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            totalInt = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                totalInt = totalInt + Convert.ToDouble(string.IsNullOrEmpty(ds.Tables[0].Rows[i]["InterestAmount"].ToString()));
            }
        }

        if (dt.Rows.Count > 0)
        {
            lblOutPrincipal.Text = dt.Rows[0]["LoanAmout"].ToString();
            // lblOutInterest.Text = Convert.ToString(totalInt + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString()));
            lblOutInterest.Text = Convert.ToString(totalInt);
            lblOutPInterest.Text = dt.Rows[0]["CLPI"].ToString();
            lblOutCharges.Text = dt.Rows[0]["CLC"].ToString();
            lblOutTotal.Text = Convert.ToString(Convert.ToDouble(dt.Rows[0]["LoanAmout"].ToString()) + totalInt + Convert.ToDouble(dt.Rows[0]["BCLI"].ToString()));
        }
    }
    public void BindEmployee()
    {

        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        //cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster where status='Active'";
        cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlGoldInwardBy.DataSource = dt;
        ddlGoldInwardBy.DataTextField = "EmpName";
        ddlGoldInwardBy.DataValueField = "EmployeeID";
        ddlGoldInwardBy.DataBind();
        ddlGoldInwardBy.Items.Insert(0, new ListItem("--Select Inward By--", "0"));
        //----------------------------------------------------------------------------------
        ddlCashOutBy.DataSource = dt;
        ddlCashOutBy.DataTextField = "EmpName";
        ddlCashOutBy.DataValueField = "EmployeeID";
        ddlCashOutBy.DataBind();
        ddlCashOutBy.Items.Insert(0, new ListItem("--Select Outward By--", "0"));

    }
    public void GetLoanDate()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select convert(varchar(12),getdate(),103)Loandate";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {

            txtLoanDate.Text = dt.Rows[0]["Loandate"].ToString();
        }

    }
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

    }
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
    }
    public void KYC_RTR()
    {
        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburse_KYC_RTR";
        cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        //   ViewState["dt"] = dt;
        // ViewState["sort"] = "Asc";
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();

    }
    public void KYC_Search()
    {
        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburse_KYC_Search";
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchValue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        // ViewState["dt"] = dt;
        //  ViewState["sort"] = "Asc";
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    public void KYC_Details_RTR(string kycid, string goldloanno)
    {

        string address = "";


        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburse_KYC_Details_RTR";
        cmd.Parameters.AddWithValue("@KYCID", kycid);
        cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        //  ViewState["dt"] = dt;
        //  ViewState["sort"] = "Asc";
        if (dt.Rows.Count > 0)
        {
            if (ddlLoanType.SelectedIndex == 1)
            {

                imgbtnCustomer.Enabled = true;
                hdnkycid.Value = dt.Rows[0]["KYCID"].ToString();
                txtCustomerID.Text = dt.Rows[0]["CustomerID"].ToString();
                txtCustomerName.Text = dt.Rows[0]["Applicantname"].ToString();
                txtGender.Text = dt.Rows[0]["Gender"].ToString();
                txtMaritalStatus.Text = dt.Rows[0]["MaritalStatus"].ToString();
                txtBirthDate.Text = dt.Rows[0]["BirthDate"].ToString();
                txtAge.Text = dt.Rows[0]["Age"].ToString();
                txtPanNo.Text = dt.Rows[0]["PANNo"].ToString();
                txtPLCaseNo.Text = dt.Rows[0]["ExistingPLCaseNo"].ToString();
                hdnsanctiondate.Value = dt.Rows[0]["Cdate"].ToString();
                txtNominee.Text = dt.Rows[0]["Nominee"].ToString();
                txtNomineeRelationship.Text = dt.Rows[0]["NomRelation"].ToString();
                ImgAppPhoto.ImageUrl = dt.Rows[0]["AppPhotoPath"].ToString();
                ImgAppSign.ImageUrl = dt.Rows[0]["AppSignPath"].ToString();
                hdnareaid.Value = dt.Rows[0]["AreaID"].ToString();
                hdnzoneid.Value = dt.Rows[0]["ZoneID"].ToString();
                address = dt.Rows[0]["Address"].ToString();
                hdnmobileno.Value = dt.Rows[0]["MobileNo"].ToString();

                //  ddlCashAccount.Enabled = true;
                BindChargesDetails();
                BindDenominationDetails();
                BindGoldItemDetails();
                //PnlItem.Enabled = false;
                // ddlCashOutBy.Enabled = false;

            }

            if (ddlLoanType.SelectedIndex == 2)
            {

                hdnkycid.Value = dt.Rows[0]["KYCID"].ToString();
                txtCustomerID.Text = dt.Rows[0]["CustomerID"].ToString();
                txtCustomerName.Text = dt.Rows[0]["Applicantname"].ToString();
                txtGender.Text = dt.Rows[0]["Gender"].ToString();
                txtMaritalStatus.Text = dt.Rows[0]["MaritalStatus"].ToString();
                txtBirthDate.Text = dt.Rows[0]["BirthDate"].ToString();
                txtAge.Text = dt.Rows[0]["Age"].ToString();
                txtPanNo.Text = dt.Rows[0]["PANNo"].ToString();
                txtPLCaseNo.Text = dt.Rows[0]["ExistingPLCaseNo"].ToString();
                txtNominee.Text = dt.Rows[0]["Nominee"].ToString();
                txtNomineeRelationship.Text = dt.Rows[0]["NomRelation"].ToString();
                ImgAppPhoto.ImageUrl = dt.Rows[0]["AppPhotoPath"].ToString();
                ImgAppSign.ImageUrl = dt.Rows[0]["AppSignPath"].ToString();
                hdnareaid.Value = dt.Rows[0]["AreaID"].ToString();
                hdnzoneid.Value = dt.Rows[0]["ZoneID"].ToString();
                address = dt.Rows[0]["Address"].ToString();
                hdnmobileno.Value = dt.Rows[0]["MobileNo"].ToString();
                hdneligible.Value = dt.Rows[0]["EligibleLoanAmt"].ToString();
                hdnsanction.Value = dt.Rows[0]["NetLoanAmtSanctioned"].ToString();
                hdnnetpayable.Value = dt.Rows[0]["NetLoanPayable"].ToString();
                hdnoldgoldloanno.Value = dt.Rows[0]["GoldLoanNo"].ToString();

                //ddlCashAccount.Enabled = true;
                dgvGoldItemDetails.DataSource = dt;
                dgvGoldItemDetails.DataBind();

                //  PnlDeno.Enabled = false;

                for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
                {
                    dgvGoldItemDetails.SelectedIndex = i;

                    DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                    Label lblItemID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblItemID");
                    DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");
                    HiddenField hdnkarat = (HiddenField)dgvGoldItemDetails.SelectedRow.FindControl("hdnkarat");

                    ddlGoldItemName.SelectedValue = lblItemID.Text.Trim();
                    ddlKarat.SelectedValue = hdnkarat.Value.Trim();

                }

                Button BtnUpload = (Button)dgvGoldItemDetails.FooterRow.FindControl("BtnUpload");
                DropDownList ddlPurity = (DropDownList)dgvGoldItemDetails.HeaderRow.FindControl("ddlPurity");
                TextBox txtTotalQuantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
                TextBox txtTotalGrossWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
                TextBox txtTotalNetWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
                TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");


                txtTotalQuantity.Text = dt.Rows[0]["TotalQuantity"].ToString();
                txtTotalGrossWeight.Text = dt.Rows[0]["TotalGrossWeight"].ToString();
                txtTotalNetWeight.Text = dt.Rows[0]["TotalNetWeight"].ToString();
                txtTotalValue.Text = dt.Rows[0]["Totalvalue"].ToString();

                BindChargesDetails();
                BindDenominationDetails();
            }

        }

        AimAreaZone(address);
        OutStanding(hdnkycid.Value.Trim(), goldloanno);
    }

    public void AimAreaZone(string address)
    {

        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select Area,Pincode From tblAreaMaster where AreaID='" + hdnareaid.Value.Trim() + "'" +
                           " select Zone From tblZonemaster where ZoneID='" + hdnzoneid.Value.Trim() + "'";
        SqlDataAdapter daaim = new SqlDataAdapter(cmd);
        DataSet dtaim = new DataSet();
        daaim.Fill(dtaim);
        if (dtaim.Tables[0].Rows.Count > 0)
        {
            address += dtaim.Tables[0].Rows[0]["Area"].ToString() + "(" + dtaim.Tables[1].Rows[0]["Zone"].ToString() + ") - " + dtaim.Tables[0].Rows[0]["Pincode"].ToString();
        }
        txtAddress.Text = address;
    }
    public void GoldLoan_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Gl_SanctionDisburse_GoldLoanNo_RTR";
        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtGoldLoanNo.Text = dt.Rows[0]["GoldLoanNo"].ToString();
        }
    }

    #region [SanctionDisburse_RTR]
    protected void SanctionDisburse_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburse_RTR";
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        //ViewState["dt"] = dt;
        // ViewState["sort"] = "Asc";
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    #endregion[SanctionDisburse_RTR]


    public void SanctionDisburseDetails_RTR(string sdid, object sender, EventArgs e)
    {

        string address = "";
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburseDetails_RTR";
        cmd.Parameters.AddWithValue("@SDID", sdid.Trim());
        da = new SqlDataAdapter(cmd);
        ds = new DataSet();
        da.Fill(ds);
        if (ds.Tables.Count > 0)
        {
            if (ds.Tables[0].Rows.Count > 0)
            {
                hdnid.Value = ds.Tables[0].Rows[0]["SDID"].ToString();
                hdnkycid.Value = ds.Tables[0].Rows[0]["KYCID"].ToString();
                hdnoperation.Value = "Update";
                imgbtnCustomer.Enabled = false;
                ddlLoanType.SelectedValue = ds.Tables[0].Rows[0]["LoanType"].ToString().Trim();
                txtCustomerID.Text = ds.Tables[0].Rows[0]["CustomerID"].ToString();
                txtGoldLoanNo.Text = ds.Tables[0].Rows[0]["GoldLoanNo"].ToString();
                txtLoanDate.Text = ds.Tables[0].Rows[0]["LoanDate"].ToString();
                hdnsanctiondate.Value = ds.Tables[0].Rows[0]["LoanDate"].ToString();
                txtCustomerName.Text = ds.Tables[0].Rows[0]["ApplicantName"].ToString();
                txtGender.Text = ds.Tables[0].Rows[0]["Gender"].ToString();
                txtMaritalStatus.Text = ds.Tables[0].Rows[0]["MaritalStatus"].ToString();
                txtBirthDate.Text = ds.Tables[0].Rows[0]["BirthDate"].ToString();
                txtAge.Text = ds.Tables[0].Rows[0]["Age"].ToString();
                txtPanNo.Text = ds.Tables[0].Rows[0]["PANNo"].ToString();
                hdnmobileno.Value = ds.Tables[0].Rows[0]["MobileNo"].ToString();
                txtPLCaseNo.Text = ds.Tables[0].Rows[0]["ExistingPLCaseNo"].ToString();
                txtNominee.Text = ds.Tables[0].Rows[0]["Nominee"].ToString();
                txtNomineeRelationship.Text = ds.Tables[0].Rows[0]["NomRelation"].ToString();
                pnlBankAc.Enabled = false;
                pnlCashAc.Enabled = false;
                ddlPaymentMode.SelectedValue = ds.Tables[0].Rows[0]["PaymentMode"].ToString().Trim();
                ddlPaymentMode_SelectedIndexChanged(sender, e);
                ddlBankAccount.SelectedValue = ds.Tables[0].Rows[0]["BankCashAccID"].ToString();
                ddlBankAccount_SelectedIndexChanged(sender, e);


                // ddlCashAccount.Enabled = true;


                if (ds.Tables[0].Rows[0]["CheqNEFTDD"].ToString() != "")
                {
                    ddlcheqNEFTDD.SelectedValue = ds.Tables[0].Rows[0]["CheqNEFTDD"].ToString();
                }
                else
                {
                    ddlcheqNEFTDD.SelectedValue = "";
                }
                if (ds.Tables[0].Rows[0]["CheqNEFTDDNo"].ToString() != "")
                {
                    txtChequeNo.Text = ds.Tables[0].Rows[0]["CheqNEFTDDNo"].ToString();
                }
                else
                {
                    txtChequeNo.Text = "";
                }

                if (ds.Tables[0].Rows[0]["CheqNEFTDDDate"].ToString() != "")
                {
                    txtChequeDate.Text = ds.Tables[0].Rows[0]["CheqNEFTDDDate"].ToString();
                }
                else
                {
                    txtChequeDate.Text = "";
                }
                if (ds.Tables[0].Rows[0]["BankAmount"].ToString().Trim() != "")
                {
                    txtBankAmount.Text = ds.Tables[0].Rows[0]["BankAmount"].ToString().Trim();
                    txtBankAmount.Text = Math.Round(Convert.ToDecimal(txtBankAmount.Text)).ToString();
                }
                else
                {
                    txtBankAmount.Text = "";
                }

                ddlSchemeName.SelectedValue = ds.Tables[0].Rows[0]["SID"].ToString();
                txtEligibleLoan.Text = ds.Tables[0].Rows[0]["EligibleLoanAmt"].ToString();
                txtLoanTenure.Text = ds.Tables[0].Rows[0]["Tenure"].ToString();
                txtEMI.Text = ds.Tables[0].Rows[0]["EMI"].ToString();
                hdnProType.Value = ds.Tables[0].Rows[0]["ProChargeType"].ToString();
                hdnProcharge.Value = ds.Tables[0].Rows[0]["ProCharge"].ToString();
                hdnproclimit.Value = ds.Tables[0].Rows[0]["AmtLmtTo"].ToString();
                hdnservicetax.Value = ds.Tables[0].Rows[0]["ServiceTax"].ToString();

                txtNetAmountSanctioned.Enabled = true;
                txtNetAmountSanctioned.Text = ds.Tables[0].Rows[0]["NetLoanAmtSanctioned"].ToString();
                txtNetPayable.Text = ds.Tables[0].Rows[0]["NetLoanPayable"].ToString();
                txtMaxLoanAmount.Text = ds.Tables[0].Rows[0]["MaxLoanAmt"].ToString();
                txtDueDate.Text = ds.Tables[0].Rows[0]["DueDate"].ToString();
                ImgAppPhoto.ImageUrl = ds.Tables[0].Rows[0]["AppPhotoPath"].ToString();
                ImgAppSign.ImageUrl = ds.Tables[0].Rows[0]["AppSignPath"].ToString();
                ddlGoldInwardBy.SelectedValue = ds.Tables[0].Rows[0]["GoldInWardById"].ToString().Trim();
                ddlCashOutBy.SelectedValue = ds.Tables[0].Rows[0]["CashOutWardById"].ToString().Trim();
                ImgItems.ImageUrl = ds.Tables[0].Rows[0]["ImgItemPath"].ToString();
                hdnimgpath.Value = ds.Tables[0].Rows[0]["ImgItemPath"].ToString();
                imgProofOfOwnership.ImageUrl = ds.Tables[0].Rows[0]["OwnershipProofImagePath"].ToString();
                txtProofOfOwnershipPath.Text = ds.Tables[0].Rows[0]["OwnershipProofImagePath"].ToString();
                hdnareaid.Value = ds.Tables[0].Rows[0]["AreaID"].ToString();
                hdnzoneid.Value = ds.Tables[0].Rows[0]["ZoneID"].ToString();
                address = ds.Tables[0].Rows[0]["Address"].ToString();

                ddlCashAccount.SelectedValue = ds.Tables[0].Rows[0]["CashAccID"].ToString().Trim();
                // ddlCashAccount.DataBind();
                txtCashAmount.Text = ds.Tables[0].Rows[0]["CashAmount"].ToString().Trim();

                // ddlPaymentMode.SelectedValue = ds.Tables[0].Rows[0]["PaymentMode"].ToString().Trim();
                // ddlPaymentMode_SelectedIndexChanged(sender, e);

                dgvGoldItemDetails.DataSource = ds.Tables[0];
                dgvGoldItemDetails.DataBind();


                for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
                {

                    dgvGoldItemDetails.SelectedIndex = i;
                    DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                    Label lblItemID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblItemID");
                    DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");
                    HiddenField hdnkarat = (HiddenField)dgvGoldItemDetails.SelectedRow.FindControl("hdnkarat");

                    ddlGoldItemName.SelectedValue = lblItemID.Text.Trim();
                    ddlKarat.SelectedValue = hdnkarat.Value.Trim();

                }


                TextBox txtTotalQuantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
                TextBox txtTotalGrossWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
                TextBox txtTotalNetWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
                TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");


                txtTotalQuantity.Text = ds.Tables[0].Rows[0]["TotalQuantity"].ToString();
                txtTotalGrossWeight.Text = ds.Tables[0].Rows[0]["TotalGrossWeight"].ToString();
                txtTotalNetWeight.Text = ds.Tables[0].Rows[0]["TotalNetWeight"].ToString();
                txtTotalValue.Text = ds.Tables[0].Rows[0]["Totalvalue"].ToString();

            }
            else
            {
                BindGoldItemDetails();
            }




            //-----------------------------------Charges Details-----------------------------------------------
            if (ds.Tables[1].Rows.Count > 0)
            {
                dgvChargesDetails.DataSource = ds.Tables[1];
                dgvChargesDetails.DataBind();
            }
            else
            {
                BindChargesDetails();
            }
            //-----------------------------------END Charges Details--------------------------------------------

            //-----------------------------------Denomination Details-----------------------------------------------
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
            //-----------------------------------END Denomination Details-----------------------------------------------

            if (ds.Tables[3].Rows.Count > 0)
            {
                ddlGoldInwardBy.SelectedValue = ds.Tables[3].Rows[0]["GLInoutBy"].ToString().Trim();
                txtRackno.Text = ds.Tables[3].Rows[0]["LocDetails"].ToString().Trim();
            }

        }
        AimAreaZone(address);
        OutStanding(hdnkycid.Value, ds.Tables[0].Rows[0]["GoldLoanNo"].ToString());

        if (hdnacc.Value == "70")
        {
            //ddlCashOutBy.Enabled = true;
        }
        else
        {
            // ddlCashOutBy.Enabled = false;
        }
    }

    #region [ShowNoResultFound]
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
    #endregion [ShowNoResultFound]

    #region [Bind GridView Gold Item Details]
    protected void BindGoldItemDetails()
    {
        try
        {
            dt = new DataTable();

            dt.Columns.Add("GID");
            dt.Columns.Add("SDID");
            dt.Columns.Add("ItemID");
            dt.Columns.Add("ItemName");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("GrossWeight");
            dt.Columns.Add("NetWeight");
            dt.Columns.Add("Purity");
            dt.Columns.Add("RateperGram");
            dt.Columns.Add("Value");


            ShowNoResultFound(dt, dgvGoldItemDetails);
            //}
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView Gold Item Details]

    #region [Bind GridView Charges Details]
    protected void BindChargesDetails()
    {
        try
        {
            dt = new DataTable();
            dt.Columns.Add("CHID");
            dt.Columns.Add("CID");
            dt.Columns.Add("ID");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("ChargeName");
            dt.Columns.Add("Charges");
            dt.Columns.Add("ChargeType");
            dt.Columns.Add("Amount");
            dt.Columns.Add("AccountID");
            dt.Columns.Add("AccountName");
            dt.Rows.Add("0", "0", "0", "1", "", "", "", "", "", "");
            dgvChargesDetails.DataSource = dt;
            dgvChargesDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView Charges Details]

    #region [ClearData]
    protected void ClearData()
    {
        //hdnareaid.Value = "0";
        txtNetPayable.Text = "0";
        hdnoldgoldloanno.Value = "0";
        hdnacc.Value = "0";
        hdnid.Value = "0";
        ImgItems.ImageUrl = "";
        hdnoperation.Value = "Save";
        hdnsanctiondate.Value = "0";
        hdnmobileno.Value = "0";
        ddlLoanType.SelectedIndex = 0;
        ddlSchemeName.SelectedIndex = 0;
        ddlPaymentMode.SelectedIndex = 0;
        txtEligibleLoan.Text = "0";
        txtEMI.Text = "0";
        txtLoanTenure.Text = "0";
        txtMaxLoanAmount.Text = "0";
        btnImgCalender.Enabled = false;
        imgbtnCustomer.Enabled = true;
        txtOperatorName.Text = "";
        txtCustomerName.Text = "";
        txtPLCaseNo.Text = "";
        txtPanNo.Text = "";
        txtGender.Text = "";
        txtBirthDate.Text = "";
        txtAge.Text = "";
        txtMaritalStatus.Text = "";
        txtAddress.Text = "";
        txtNominee.Text = "";
        txtNomineeRelationship.Text = "";
        txtChequeNo.Text = "";
        txtChequeDate.Text = "";
        txtLoanTenure.Text = "";
        txtDueDate.Text = "";
        txtEMI.Text = "";
        txtNetAmountSanctioned.Text = "";
        imgProofOfOwnership.ImageUrl = "";
        txtMobile.Text = "";
        txtTelephone.Text = "";
        txtEmailID.Text = "";
        txtProofOfOwnershipPath.Text = "";
        txtAccGPID.Value = "";
        txtTotalChargesAmount.Text = "";
        txtChequeNo.Enabled = false;
        txtChequeDate.Enabled = false;
        ddlCashAccount.SelectedIndex = 0;
        //Bind Gold Details
        BindGoldItemDetails();
        //Bind Charges Details
        BindChargesDetails();
        ddlcheqNEFTDD.SelectedIndex = 0;
        ddlcheqNEFTDD.Enabled = false;
        //  ddlCashAccount.Enabled = false;
        //getting Operator Name and ID
        if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
        {
            txtOperatorName.Text = Convert.ToString(Session["username"]);
        }
        // PnlDeno.Enabled = false;
        BindDenominationDetails();
        ImgAppPhoto.ImageUrl = "";
        ImgAppSign.ImageUrl = "";
        GoldLoan_RTR();
        GetLoanDate();
        lblOutPrincipal.Text = "0";
        lblOutInterest.Text = "0";
        lblOutCharges.Text = "0";
        lblOutPInterest.Text = "0";
        hdnkycid.Value = "0";
        hdnacc.Value = "0";
        txtCustomerID.Text = "";
        ddlGoldInwardBy.SelectedIndex = 0;
        ddlCashOutBy.SelectedIndex = 0;
        txtRackno.Text = "";
        ddlGoldInwardBy.Enabled = false;
        txtRackno.Enabled = false;
        dgvChargesDetails.Enabled = false;
        dgvGoldItemDetails.Enabled = false;
        lblOutPrincipal.Text = "0";
        lblOutInterest.Text = "0";
        lblOutPInterest.Text = "0";
        lblOutCharges.Text = "0";
        lblOutTotal.Text = "0";
        txtBankAmount.Text = "";
        txtCashAmount.Text = "";
        ddlBankAccount.SelectedIndex = 0;
        //ddlBankAccount.Enabled = false;


    }
    #endregion [ClearData]

    #region [ClearLoanData]
    protected void ClearLoanData()
    {
        pnlCashAc.Enabled = false;
        pnlBankAc.Enabled = false;
        //hdnareaid.Value = "0";
        txtNetPayable.Text = "0";
        hdnoldgoldloanno.Value = "0";
        hdnacc.Value = "0";
        hdnid.Value = "0";
        ImgItems.ImageUrl = "";
        hdnoperation.Value = "Save";
        hdnsanctiondate.Value = "0";
        hdnmobileno.Value = "0";
        // ddlLoanType.SelectedIndex = 0;
        ddlSchemeName.SelectedIndex = 0;
        txtEligibleLoan.Text = "0";
        txtEMI.Text = "0";
        txtLoanTenure.Text = "0";
        txtMaxLoanAmount.Text = "0";
        btnImgCalender.Enabled = false;
        txtOperatorName.Text = "";
        txtCustomerName.Text = "";
        txtPLCaseNo.Text = "";
        txtPanNo.Text = "";
        txtGender.Text = "";
        txtBirthDate.Text = "";
        txtAge.Text = "";
        txtMaritalStatus.Text = "";
        txtAddress.Text = "";
        txtNominee.Text = "";
        txtNomineeRelationship.Text = "";
        txtChequeNo.Text = "";
        txtChequeDate.Text = "";
        txtLoanTenure.Text = "";
        txtDueDate.Text = "";
        txtEMI.Text = "";
        txtNetAmountSanctioned.Text = "";
        imgProofOfOwnership.ImageUrl = "";
        txtMobile.Text = "";
        txtTelephone.Text = "";
        txtEmailID.Text = "";
        txtProofOfOwnershipPath.Text = "";
        txtAccGPID.Value = "";
        txtTotalChargesAmount.Text = "";
        txtChequeNo.Enabled = false;
        txtChequeDate.Enabled = false;
        // ddlCashAccount.SelectedIndex = 0;
        //Bind Gold Details
        BindGoldItemDetails();
        //Bind Charges Details
        BindChargesDetails();
        ddlcheqNEFTDD.SelectedIndex = 0;
        ddlcheqNEFTDD.Enabled = false;
        //  ddlCashAccount.Enabled = false;
        //getting Operator Name and ID
        if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
        {
            txtOperatorName.Text = Convert.ToString(Session["username"]);
        }
        // PnlDeno.Enabled = false;
        BindDenominationDetails();
        ImgAppPhoto.ImageUrl = "";
        ImgAppSign.ImageUrl = "";
        GoldLoan_RTR();
        GetLoanDate();
        lblOutPrincipal.Text = "0";
        lblOutInterest.Text = "0";
        lblOutCharges.Text = "0";
        lblOutPInterest.Text = "0";
        hdnkycid.Value = "0";
        hdnacc.Value = "0";
        txtCustomerID.Text = "";
        ddlGoldInwardBy.SelectedIndex = 0;
        ddlCashOutBy.SelectedIndex = 0;
        txtRackno.Text = "";
        ddlGoldInwardBy.Enabled = false;
        txtRackno.Enabled = false;
        dgvChargesDetails.Enabled = false;
        dgvGoldItemDetails.Enabled = false;
        lblOutPrincipal.Text = "0";
        lblOutInterest.Text = "0";
        lblOutPInterest.Text = "0";
        lblOutCharges.Text = "0";
        lblOutTotal.Text = "0";

        txtCashAmount.Text = "";
        txtBankAmount.Text = "";
        //  ddlBankAccount.SelectedIndex = -1;
        // ddlCashAccount.SelectedIndex = -1;
        GoldLoan_RTR();
    }
    #endregion [ClearData]

    #region [SanctionDisbursment_PRV]
    public void SanctionDisbursment_PRV(string operation, string value, string cid)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();

        cmd = new SqlCommand();
        cmd.Connection = conn;

        TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SanctionDisburse_PRV";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@SDID", value);
        cmd.Parameters.AddWithValue("@GoldLoanNo", txtGoldLoanNo.Text);
        cmd.Parameters.AddWithValue("@SID", ddlSchemeName.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
        cmd.Parameters.AddWithValue("@CID", cid);
        cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
        //cmd.Parameters.AddWithValue("@Value", txtTotalValue.Text);
        cmd.Parameters.AddWithValue("@Value", txtTotalValue.Text);
        cmd.Parameters.AddWithValue("@SanctionAmount", txtNetAmountSanctioned.Text);
        cmd.ExecuteNonQuery();
        conn.Close();

    }
    #endregion [SanctionDisbursment_PRV]

    #region [SanctionDisbursment_PRI]
    public void SanctionDisbursment_PRI(string operation, string value)
    {

        conn = new SqlConnection(strConnString);
        conn.Open();
        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");
        if (operation != "Delete")
        {
            if (operation == "Save")
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandTimeout = 500;
                cmd.CommandText = "select isnull(MAX(SDID),0)+1 From TGLSanctionDisburse_BasicDetails";
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    value = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            TextBox txtTotalQuantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
            TextBox txtTotalGrossWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
            TextBox txtTotalNetWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
            TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");


            for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
            {
                dgvGoldItemDetails.SelectedIndex = i;

                DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                TextBox txtQuantity = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtQuantity");
                TextBox txtGrossWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtGrossWeight");
                TextBox txtNetWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtNetWeight");
                TextBox txtRatePerGram = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtRatePerGram");
                TextBox txtValue = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtValue");
                DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GL_SanctionDisburse_PRI";
                cmd.Parameters.AddWithValue("@Operation", operation);
                cmd.Parameters.AddWithValue("@Flag", "GOLDITEM");
                cmd.Parameters.AddWithValue("@SDID", value);
                cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtLoanDate.Text));
                cmd.Parameters.AddWithValue("@GoldLoanNo", txtGoldLoanNo.Text);
                cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
                cmd.Parameters.AddWithValue("@OperatorID", Session["userID"].ToString());
                cmd.Parameters.AddWithValue("@EligibleLoanAmt", txtEligibleLoan.Text);
                cmd.Parameters.AddWithValue("@NetLoanAmtSanctioned", txtNetAmountSanctioned.Text);
                cmd.Parameters.AddWithValue("@ChargesTotal", txtTotalChargesAmount.Text);
                cmd.Parameters.AddWithValue("@NetLoanPayable", txtNetPayable.Text);

                cmd.Parameters.AddWithValue("@CheqNEFTDD", ddlcheqNEFTDD.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@CheqNEFTDDNo", txtChequeNo.Text);
                cmd.Parameters.AddWithValue("@CheqNEFTDDDate", gbl.ChangeDateMMddyyyy(txtChequeDate.Text));
                cmd.Parameters.AddWithValue("@TotalGrossWeight", txtTotalGrossWeight.Text);
                cmd.Parameters.AddWithValue("@TotalNetWeight", txtTotalNetWeight.Text);
                cmd.Parameters.AddWithValue("@TotalQuantity", txtTotalQuantity.Text);
                cmd.Parameters.AddWithValue("@Totalvalue", txtTotalValue.Text);
                cmd.Parameters.AddWithValue("@TotalRate", txtRatePerGram.Text);
                cmd.Parameters.AddWithValue("@SID", ddlSchemeName.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@DueDate", gbl.ChangeDateMMddyyyy(txtDueDate.Text));
                cmd.Parameters.AddWithValue("@OwnershipProofImagePath", txtProofOfOwnershipPath.Text.Trim());
                cmd.Parameters.AddWithValue("@CIBILScore", "0");
                cmd.Parameters.AddWithValue("@BCPID", "0");
                cmd.Parameters.AddWithValue("@CashOutWardById", ddlCashOutBy.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@GoldInWardById", ddlGoldInwardBy.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());
                cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
                cmd.Parameters.AddWithValue("@BranchID", Session["branchId"].ToString());
                cmd.Parameters.AddWithValue("@CMPID", "1");
                //added by priya

                if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null || ddlCashAccount.SelectedValue == "0")
                {
                    cmd.Parameters.AddWithValue("@CashAccID", "0");
                    cmd.Parameters.AddWithValue("@CashAmount", "0");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@CashAccID", ddlCashAccount.SelectedValue.Trim());
                    cmd.Parameters.AddWithValue("@CashAmount", txtCashAmount.Text);
                }

                if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null || ddlBankAccount.SelectedValue == "0")
                {
                    cmd.Parameters.AddWithValue("@BankCashAccID", "0");
                    cmd.Parameters.AddWithValue("@BankAmount", "0");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@BankCashAccID", ddlBankAccount.SelectedValue.Trim());
                    cmd.Parameters.AddWithValue("@BankAmount", txtBankAmount.Text);

                }
                cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedItem.Text);
                //  cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedValue.Trim());
                //end

                cmd.Parameters.AddWithValue("@Lineno", i);
                cmd.Parameters.AddWithValue("@GID", "0");
                cmd.Parameters.AddWithValue("@ISerialno", "0");
                cmd.Parameters.AddWithValue("@ItemID", ddlGoldItemName.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@GrossWeight", txtGrossWeight.Text);
                cmd.Parameters.AddWithValue("@Quantity", txtQuantity.Text);
                cmd.Parameters.AddWithValue("@NetWeight", txtNetWeight.Text);
                cmd.Parameters.AddWithValue("@RateperGram", txtRatePerGram.Text);
                cmd.Parameters.AddWithValue("@Value", txtValue.Text);
                cmd.Parameters.AddWithValue("@Purity", ddlKarat.SelectedValue.Trim());
                cmd.Parameters.AddWithValue("@ImgItemPath", hdnimgpath.Value);


                cmd.Parameters.AddWithValue("@GLInOutID", "0");
                cmd.Parameters.AddWithValue("@GLRefType", "GIO");
                cmd.Parameters.AddWithValue("@GLRefNo", "0");
                cmd.Parameters.AddWithValue("@GLReferenceType", "0");

                cmd.Parameters.AddWithValue("@LocName", Session["branchId"].ToString());
                cmd.Parameters.AddWithValue("@LocDetails", txtRackno.Text);
                cmd.Parameters.AddWithValue("@GInOutBy", ddlGoldInwardBy.SelectedValue.Trim());

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


            if (datasaved)
            {

                //if (hdnacc.Value == "70")
                //{
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

                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandTimeout = 0;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_SanctionDisburse_PRI";
                    cmd.Parameters.AddWithValue("@Operation", operation);
                    cmd.Parameters.AddWithValue("@SDID", value);
                    cmd.Parameters.AddWithValue("@Lineno", i);
                    cmd.Parameters.AddWithValue("@Flag", "DENOMINATION");
                    cmd.Parameters.AddWithValue("@InOutID", hdncashinoutid.Value);
                    cmd.Parameters.AddWithValue("@RefNo", hdnrefno.Value);
                    cmd.Parameters.AddWithValue("@ReferenceType", "0");
                    cmd.Parameters.AddWithValue("@FName", "S");
                    cmd.Parameters.AddWithValue("@InOutMode", "O");
                    cmd.Parameters.AddWithValue("@InOutTo", "Customer");
                    cmd.Parameters.AddWithValue("@InOutToID", value);
                    cmd.Parameters.AddWithValue("@InOutFrom", "0");
                    cmd.Parameters.AddWithValue("@InOutFromID", "0");
                    cmd.Parameters.AddWithValue("@InOutBy", ddlCashOutBy.SelectedValue.Trim());

                    cmd.Parameters.AddWithValue("@DenoId", hdndenoid.Value);
                    cmd.Parameters.AddWithValue("@Serialno", gvtxtDenoSrno.Text);
                    cmd.Parameters.AddWithValue("@DenoRs", gvtxtDenoDescription.Text);
                    cmd.Parameters.AddWithValue("@DenoQty", gvtxtDenoNo.Text);
                    cmd.Parameters.AddWithValue("@Total", gvtxtDenoTotal.Text);
                    cmd.Parameters.AddWithValue("@NoteNos", gvtxtDenoNoteNos.Text);
                    cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
                    cmd.Parameters.AddWithValue("@BranchID", Session["branchId"].ToString());
                    cmd.Parameters.AddWithValue("@CMPID", "1");
                    //added by priya

                    if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null || ddlCashAccount.SelectedValue == "0")
                    {

                        cmd.Parameters.AddWithValue("@CashAccID", "0");
                        cmd.Parameters.AddWithValue("@CashAmount", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CashAccID", ddlCashAccount.SelectedValue.Trim());
                        cmd.Parameters.AddWithValue("@CashAmount", txtCashAmount.Text);
                    }

                    if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null || ddlBankAccount.SelectedValue == "0")
                    {
                        cmd.Parameters.AddWithValue("@BankCashAccID", "0");
                        cmd.Parameters.AddWithValue("@BankAmount", "0");
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@BankCashAccID", ddlBankAccount.SelectedValue.Trim());
                        cmd.Parameters.AddWithValue("@BankAmount", txtBankAmount.Text);

                    }
                    cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedItem.Text);
                    // cmd.Parameters.AddWithValue("@PaymentMode", ddlPaymentMode.SelectedValue.Trim());

                    result = cmd.ExecuteNonQuery();
                }

                if (result > 0)
                {

                    datasaved = true;

                }
                else
                {

                    datasaved = false; ;

                }
                // }
            }
            if (datasaved)
            {

                for (int i = 0; i < dgvChargesDetails.Rows.Count; i++)
                {
                    dgvChargesDetails.SelectedIndex = i;

                    HiddenField hdnchid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchid");
                    HiddenField hdncid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdncid");
                    HiddenField hdnchargeid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchargeid");
                    Label lblChargesName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblChargesName");
                    Label lblCharges = (Label)dgvChargesDetails.SelectedRow.FindControl("lblCharges");
                    Label lblAmount = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAmount");
                    Label lblAccountName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAccountName");
                    HiddenField hdnaccountid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnaccountid");


                    if (lblChargesName.Text != "")
                    {
                        cmd = new SqlCommand();
                        cmd.Connection = conn;
                        cmd.Transaction = transactionGL;
                        cmd.CommandTimeout = 0;
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "GL_SanctionDisburse_PRI";
                        cmd.Parameters.AddWithValue("@Operation", operation);
                        cmd.Parameters.AddWithValue("@SDID", value);
                        cmd.Parameters.AddWithValue("@Lineno", i);
                        cmd.Parameters.AddWithValue("@Flag", "CHARGES");
                        cmd.Parameters.AddWithValue("@CHID", hdnchid.Value);
                        cmd.Parameters.AddWithValue("@CID", hdncid.Value);
                        cmd.Parameters.AddWithValue("@ID", hdnchargeid.Value);
                        cmd.Parameters.AddWithValue("@ChSerialno", hdncid.Value);
                        cmd.Parameters.AddWithValue("@Charges", lblCharges.Text);
                        cmd.Parameters.AddWithValue("@Amount", lblAmount.Text);
                        cmd.Parameters.AddWithValue("@AccountID", hdnaccountid.Value);
                        cmd.Parameters.AddWithValue("@AccountName", lblAccountName.Text);
                        result = cmd.ExecuteNonQuery();
                    }
                }
                //conn.Close();
                if (result > 0)
                {
                    datasaved = true;
                }
                else
                {
                    datasaved = false;
                }
            }
        }//end priya
        else if (operation == "Delete")
        {
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = transactionGL;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SanctionDisburse_PRI";
            cmd.Parameters.AddWithValue("@Operation", operation);
            cmd.Parameters.AddWithValue("@SDID", value);

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
                datasaved = Delete_PRI(value, transactionGL, conn, datasaved);
            }
        }

        if (datasaved)
        {
            if (operation == "Save")
            {
                if (ddlLoanType.SelectedIndex == 1)
                {
                    datasaved = Save_PRI(value, transactionGL, conn, datasaved);
                }
                if (ddlLoanType.SelectedIndex == 2)
                {
                    datasaved = SaveTopup_PRI(value, transactionGL, conn, datasaved);
                }
            }
            else if (operation == "Update")
            {
                datasaved = Update_PRI(value, transactionGL, conn, datasaved);
            }

            if (operation == "Delete")
            {
                datasaved = Delete_PRI(value, transactionGL, conn, datasaved);
            }
        }
        if (operation == "Save" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SD", "alert('Record Saved Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            // ShowReport(value);
            ClearData();

        }
        if (operation == "Update" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SD1", "alert('Record Updated Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            //   ShowReport(value);
            ClearData();
        }
        if (operation == "Delete" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SD2", "alert('Record Deleted Successfully');", true);
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }

    }
    #endregion [SanctionDisbursment_PRI]

    //---------------------------Save and Update---------------------------------------------------
    #region [Save_PRI]
    public bool Save_PRI(string value, SqlTransaction tranGL, SqlConnection conGL, bool saved)
    {
        // string Narration = "";
        datasaved = saved;

        int count = 0;
        if (ddlPaymentMode.SelectedIndex == 3)
        {
            count = 2;
        }
        else
        {
            count = 1;
        }

        cmd = new SqlCommand();
        cmd.Connection = conGL;
        cmd.Transaction = tranGL;
        cmd.CommandTimeout = 0;
        cmd.CommandText = "select GoldLoanNo From TGLSanctionDisburse_BasicDetails where SDID='" + value + "'";
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            GoldLoanNo = Convert.ToString(cmd.ExecuteScalar());
        }

        #region [Variable and object Declarations]
        int QueryResult = 0;

        #endregion [Variable and object Declarations]

        #region [SaveinAIMTables]
        //+++++++++++++++++++++ CREATION OF CUSTOMER ACCOUNT AND PASSING LEDGER ENTRIES +++++++++++++++++++++++++++++++
        int AccountID = 0;
        int GPID = 67; //Group ID of Sundry Debtors
        int LedgerID = 0;
        int DJEID = 0;
        int DJERefNo = 0;
        string DJERefType = "DJEGL";
        string DJEReferenceNo = string.Empty;

        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();
        transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSaveinAIM");

        // 9] inserting data into table tblAccountMaster
        if (datasaved)
        {
            //getting MAX ID
            strQuery = "select max(AccountID) from tblAccountMaster";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                AccountID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                AccountID = 0;
            }

            AccountID += 1;

            insertQuery = "insert into tblAccountMaster values(" + AccountID + ", '" + txtCustomerName.Text.Trim() + "', " +
                            "'" + GoldLoanNo.Trim() + "', " + GPID + ", 0, 'Dr', '" + txtPanNo.Text.Trim() + "', '0', '-','-', " +
                            "@address, '0','0','-','0', '" + txtAreaID.Text.Trim() + "', '" + txtTelephone.Text.Trim() + "', '" + txtMobile.Text.Trim() + "', " +
                            "'-', '" + txtEmailID.Text.Trim() + "', 'No','')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
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


        //insert debit entry  //added priya

        if (datasaved)
        {
            //getting MAX ID
            strQuery = "select max(DJEID) from FSystemGeneratedEntryMaster";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DJEID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                DJEID = 0;
            }
            DJEID += 1;
            DJERefNo = DJEID;
            DJEReferenceNo = DJERefType + "/" + Convert.ToString(DJEID);//1st entry
            HiddenFieldRefNo.Value = DJEReferenceNo;    //taken becoz of to insert in all entry same refNo

            insertQuery = "insert into FSystemGeneratedEntryMaster values(" + DJEID + ", '" + DJERefType + "', '" + DJERefNo + "', " +
                            "'" + DJEReferenceNo + "', '" + GoldLoanNo.Trim() + "', 0, " +
                            "'" + txtFYID.Text + "')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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




        // 9.2] inserting data into table TBankCash_PaymentDetails
        int BCPID = 0;
        int VoucherNo = 0;
        int PaidTo = AccountID;
        int BankCashAccID = 0;

        if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        {
            if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
        }
        else
        {

            if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
        }
        //  int CashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);//priya

        double Amount = 0, BankAmount = 0, CashAmount = 0;
        string BankNarration = "";
        BankNarration = "Payment made against New Gold Loan sanctioned"; //for debit only
        if (ddlLoanType.SelectedIndex == 1)     //for New Loan
        {
            //if (hdnacc.Value != "70")
            //{
            Amount = Convert.ToDouble(txtNetAmountSanctioned.Text); //commenterd by priya
            //}
            //else
            //{
            //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            //    Amount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
            //}

            if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
            {
                //BankAmount = Convert.ToDouble(txtBankAmount.Text);
                // BankNarration = "Payment made against New Gold Loan sanctioned";
            }
            //if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
            //{
            //    CashAmount = Convert.ToDouble(txtCashAmount.Text);
            //    CashNarration = "Payment made by Cash against New Gold Loan sanctioned";
            //}


        }


        if (datasaved)
        {
            // retrieving MAX BCPID
            strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                BCPID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                BCPID = 0;
            }
            BCPID += 1;

            // retrieving MAX VoucherNo
            strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                VoucherNo = 0;
            }
            VoucherNo += 1;

            string strChqDate = string.Empty;

            if (ddlPaymentMode.SelectedIndex == 1 && ddlPaymentMode.SelectedIndex == 3)
            {
                if (txtChequeDate.Text.Trim() != "")
                {
                    strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
                }
            }
            else
            {
                strChqDate = ""; txtChequeNo.Text = "";
            }

            insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
                                    "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
                                    "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
                                    "'" + BankCashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
                                    "'" + strChqDate + "', '" + BankNarration + "', 0, " +
                                    "'" + txtFYID.Text + "')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
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


        // 9.3] Debit Entry in FLedger (Main Ledger Entry)
        if (datasaved)
        {
            int AccID = AccountID;
            if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
            {
                ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
            else
            {
                ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }

            double DebitAmount = 0, DebitBankAmount = 0, DebitCashAmount = 0;
            if (ddlLoanType.SelectedIndex == 1)     //for New Loan
            {
                //if (hdnacc.Value != "70")
                //{
                DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text); //for entry in closing A/C
                //}
                //else
                //{
                //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                //    DebitAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                //}

                if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                {
                    if (txtBankAmount.Text != "" || Convert.ToInt16(txtBankAmount.Text) != 0)
                    {
                        DebitBankAmount = Convert.ToDouble(txtBankAmount.Text);
                    }
                    else
                    {
                        DebitBankAmount = Convert.ToDouble(txtCashAmount.Text);
                    }

                    BankNarration = "Payment made against New Gold Loan sanctioned";
                }
                //if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                //{
                //    DebitBankAmount = Convert.ToDouble(txtCashAmount.Text);
                //    BankNarration = "Payment made by Cash against New Gold Loan sanctioned";
                //}


            }

            double CreditAmount = 0;

            LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, BankNarration);
            //  LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitBankAmount, CreditAmount, ContraAccID, BankNarration);


            if (datasaved)
            {
                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            }

        }
        //debit end
        //added on 3-6-2015

        if (datasaved)
        {
            updateQuery = "update TBankCash_PaymentDetails " +
                            "set LedgerID=" + LedgerID + " " +
                            "where BCPID=" + BCPID + " ";

            cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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
            updateQuery = "update FSystemGeneratedEntryMaster " +
                                    "set LedgerID=" + LedgerID + " " +
                            "where DJEID=" + DJEID + " ";
            cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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

        // 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
        if (datasaved)
        {
            updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                                    "set BCPID=" + BCPID + " " +
                            "where SDID=" + value + " ";
            cmd = new SqlCommand(updateQuery, conGL, tranGL);
            cmd.CommandTimeout = 0;
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
        //end

        // 9.1] inserting data into table FSystemGeneratedEntryMaster
        if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        {
            int refID = 0;
            if (datasaved)
            {
                //getting MAX ID
                strQuery = "select max(DJEID) from FSystemGeneratedEntryMaster";
                cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    DJEID = Convert.ToInt32(cmd.ExecuteScalar());
                    // refID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    DJEID = 0; refID = 0;
                }
                DJEID += 1;
                DJERefNo = DJEID;
                // DJEReferenceNo = DJERefType + "/" + Convert.ToString(refID);

                insertQuery = "insert into FSystemGeneratedEntryMaster values(" + DJEID + ", '" + DJERefType + "', '" + DJERefNo + "', " +
                                "'" + HiddenFieldRefNo.Value + "', '" + GoldLoanNo.Trim() + "', 0, " +
                                "'" + txtFYID.Text + "')";
                cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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



            // 9.2] inserting data into table TBankCash_PaymentDetails
            // int BCPID = 0;
            ////  int VoucherNo = 0;
            // int PaidTo = AccountID;
            // int BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            //  int CashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);//priya

            // double Amount = 0, BankAmount = 0, CashAmount = 0;
            // string BankNarration = "";
            if (ddlLoanType.SelectedIndex == 1)     //for New Loan
            {
                //if (hdnacc.Value != "70")
                //{
                Amount = Convert.ToDouble(txtNetAmountSanctioned.Text); //commenterd by priya
                //}
                //else
                //{
                //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                //    Amount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                //}

                if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                {
                    BankAmount = Convert.ToDouble(txtBankAmount.Text);
                    BankNarration = "Payment made by Bank against New Gold Loan sanctioned";
                }
                //if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                //{
                //    CashAmount = Convert.ToDouble(txtCashAmount.Text);
                //    CashNarration = "Payment made by Cash against New Gold Loan sanctioned";
                //}


            }


            if (datasaved)
            {
                // retrieving MAX BCPID
                strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
                cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BCPID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    BCPID = 0;
                }
                BCPID += 1;

                // retrieving MAX VoucherNo
                strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
                cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    VoucherNo = 0;
                }
                VoucherNo += 1;

                string strChqDate = string.Empty;
                Amount = Convert.ToDouble(txtBankAmount.Text);
                //if (txtChequeDate.Text.Trim() != "")
                //{
                //    strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
                //}

                if (ddlPaymentMode.SelectedIndex == 1 && ddlPaymentMode.SelectedIndex == 3)
                {
                    if (txtChequeDate.Text.Trim() != "")
                    {
                        strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
                    }
                }
                else
                {
                    strChqDate = ""; txtChequeNo.Text = "";
                }


                //insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
                //                        "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
                //                        "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
                //                        "'" + BankCashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
                //                        "'" + strChqDate + "', '" + BankNarration + "', 0, " +
                //                        "'" + txtFYID.Text + "')";
                //cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
                //QueryResult = cmd.ExecuteNonQuery();

                //if (QueryResult > 0)
                //{
                //    datasaved = true;
                //}
                //else
                //{
                //    datasaved = false;
                //}
            }


            // 9.3] Debit Entry in FLedger (Main Ledger Entry)
            if (datasaved)
            {
                int AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                int ContraAccID = AccountID;

                double DebitAmount = 0, DebitBankAmount = 0, DebitCashAmount = 0;
                if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                {
                    //if (hdnacc.Value != "70")
                    //{
                    // DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                    //}
                    //else
                    //{
                    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                    //    DebitAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                    //}

                    if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                    {
                        DebitBankAmount = Convert.ToDouble(txtBankAmount.Text);
                        BankNarration = "Payment made by Bank against New Gold Loan sanctioned";
                    }
                    //if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                    //{
                    //    DebitCashAmount = Convert.ToDouble(txtCashAmount.Text);
                    //    CashNarration = "Payment made by Cash against New Gold Loan sanctioned";
                    //}


                }

                double CreditAmount = 0;

                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, CreditAmount, DebitBankAmount, ContraAccID, BankNarration);
                //  LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitBankAmount, CreditAmount, ContraAccID, BankNarration);


                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, CreditAmount, DebitBankAmount, transactionAIM, connAIM);
                }
            }


            //// 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
            //if (datasaved)
            //{
            //    updateQuery = "update TGLSanctionDisburse_BasicDetails " +
            //                            "set BCPID=" + BCPID + " " +
            //                    "where SDID=" + value + " ";
            //    cmd = new SqlCommand(updateQuery, conGL, tranGL);
            //    cmd.CommandTimeout = 0;
            //    QueryResult = cmd.ExecuteNonQuery();

            //    if (QueryResult > 0)
            //    {
            //        datasaved = true;
            //    }
            //    else
            //    {
            //        datasaved = false;
            //    }
            //}


            //// 9.5] Updating table TBankCash_PaymentDetails with Ledger ID                   
            //if (datasaved)
            //{
            //    updateQuery = "update TBankCash_PaymentDetails " +
            //                    "set LedgerID=" + LedgerID + " " +
            //                    "where BCPID=" + BCPID + " ";

            //    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
            //    cmd.CommandTimeout = 0;
            //    QueryResult = cmd.ExecuteNonQuery();

            //    if (QueryResult > 0)
            //    {
            //        datasaved = true;
            //    }
            //    else
            //    {
            //        datasaved = false;
            //    }
            //}


            // 9.6] Updating table FSystemGeneratedEntryMaster with Ledger ID
            if (datasaved)
            {
                updateQuery = "update FSystemGeneratedEntryMaster " +
                                        "set LedgerID=" + LedgerID + " " +
                                "where DJEID=" + DJEID + " ";
                cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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

            //// 9.7] Contra Entry in FLedger 
            //if (datasaved)
            //{
            //    int AccID = AccountID;
            //    int ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            //    double DebitAmount = 0;

            //    double CreditAmount = 0;
            //    if (ddlLoanType.SelectedIndex == 1)     //for New Loan
            //    {
            //        //if (hdnacc.Value != "70")
            //        //{
            //        CreditAmount = Convert.ToDouble(txtNetPayable.Text);
            //        //}
            //        //else
            //        //{
            //        //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            //        //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
            //        //}
            //    }


            //    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, BankNarration);

            //    if (datasaved)
            //    {
            //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            //    }
            //}

        }
        //****************** ledgerEntry for Cash a/c*********

        // 9.1] inserting data into table FSystemGeneratedEntryMaster
        if (txtCashAmount.Text != "" && Convert.ToDecimal(txtCashAmount.Text.Trim()) > 0)
        {
            if (datasaved)
            {
                int refCashID = 0;
                //getting MAX ID
                strQuery = "select max(DJEID) from FSystemGeneratedEntryMaster";
                cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    DJEID = Convert.ToInt32(cmd.ExecuteScalar());
                    // refCashID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    DJEID = 0;
                }
                DJEID += 1;
                DJERefNo = DJEID;
                // DJEReferenceNo = DJERefType + "/" + Convert.ToString(refCashID);

                insertQuery = "insert into FSystemGeneratedEntryMaster values(" + DJEID + ", '" + DJERefType + "', '" + DJERefNo + "', " +
                                "'" + HiddenFieldRefNo.Value + "', '" + GoldLoanNo.Trim() + "', 0, " +
                                "'" + txtFYID.Text + "')";
                cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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




            // 9.2] inserting data into table TBankCash_PaymentDetails
            //int BCPID = 0;
            //int VoucherNo = 0;
            //int PaidTo = AccountID;
            //// int BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            int CashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);//priya

            //double Amount = 0, BankAmount = 0, CashAmount = 0;
            string CashNarration = "";
            if (ddlLoanType.SelectedIndex == 1)     //for New Loan
            {
                //if (hdnacc.Value != "70")
                //{
                //  Amount = Convert.ToDouble(txtNetAmountSanctioned.Text); //commenterd by priya
                //}
                //else
                //{
                //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                //    Amount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                //}

                //if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                //{
                //    BankAmount = Convert.ToDouble(txtBankAmount.Text);
                //    BankNarration = "Payment made by Bank against New Gold Loan sanctioned";
                //}
                if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                {
                    CashAmount = Convert.ToDouble(txtCashAmount.Text);
                    CashNarration = "Payment made by Cash against New Gold Loan sanctioned";
                }


            }

            //commented on 13-7-2015
            //if (datasaved)
            //{
            //    // retrieving MAX BCPID
            //    strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
            //    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            //    cmd.CommandTimeout = 0;
            //    if (cmd.ExecuteScalar() != DBNull.Value)
            //    {
            //        BCPID = Convert.ToInt32(cmd.ExecuteScalar());
            //    }
            //    else
            //    {
            //        BCPID = 0;
            //    }
            //    BCPID += 1;

            //    // retrieving MAX VoucherNo
            //    strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
            //    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            //    cmd.CommandTimeout = 0;
            //    if (cmd.ExecuteScalar() != DBNull.Value)
            //    {
            //        VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
            //    }
            //    else
            //    {
            //        VoucherNo = 0;
            //    }
            //    VoucherNo += 1;

            //    Amount = Convert.ToDouble(txtCashAmount.Text);
            //    string strChqDate = string.Empty;
            //    //if (txtChequeDate.Text.Trim() != "")
            //    //{
            //    //    strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
            //    //}


            //    if (ddlPaymentMode.SelectedIndex == 1 && ddlPaymentMode.SelectedIndex == 3)
            //    {
            //        if (txtChequeDate.Text.Trim() != "")
            //        {
            //            strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
            //        }
            //    }
            //    else
            //    {
            //        strChqDate = ""; txtChequeNo.Text = "";
            //    }

            //    //insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
            //    //                        "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
            //    //                        "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
            //    //                        "'" + CashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
            //    //                        "'" + strChqDate + "', '" + CashNarration + "', 0, " +
            //    //                        "'" + txtFYID.Text + "')";
            //    //cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
            //    //QueryResult = cmd.ExecuteNonQuery();

            //    //if (QueryResult > 0)
            //    //{
            //    //    datasaved = true;
            //    //}
            //    //else
            //    //{
            //    //    datasaved = false;
            //    //}
            //}


            // 9.3] Debit Entry in FLedger (Main Ledger Entry)
            if (datasaved)
            {
                int AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                int ContraAccID = AccountID;

                double DebitAmount = 0, DebitBankAmount = 0, DebitCashAmount = 0;
                if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                {
                    //if (hdnacc.Value != "70")
                    //{
                    // DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                    //}
                    //else
                    //{
                    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                    //    DebitAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                    //}

                    //if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                    //{
                    //    DebitBankAmount = Convert.ToDouble(txtBankAmount.Text);
                    //    BankNarration = "Payment made by Bank against New Gold Loan sanctioned";
                    //}
                    if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                    {
                        DebitCashAmount = Convert.ToDouble(txtCashAmount.Text);
                        CashNarration = "Payment made by Cash against New Gold Loan sanctioned";
                    }

                }

                double CreditAmount = 0;

                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, CreditAmount, DebitCashAmount, ContraAccID, CashNarration);


                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, CreditAmount, DebitCashAmount, transactionAIM, connAIM);
                }
            }


            // 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
            if (datasaved)
            {
                updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                                        "set BCPID=" + BCPID + " " +
                                "where SDID=" + value + " ";
                cmd = new SqlCommand(updateQuery, conGL, tranGL);
                cmd.CommandTimeout = 0;
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


            //// 9.5] Updating table TBankCash_PaymentDetails with Ledger ID                   
            //if (datasaved)
            //{
            //    updateQuery = "update TBankCash_PaymentDetails " +
            //                    "set LedgerID=" + LedgerID + " " +
            //                    "where BCPID=" + BCPID + " ";

            //    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
            //    cmd.CommandTimeout = 0;
            //    QueryResult = cmd.ExecuteNonQuery();

            //    if (QueryResult > 0)
            //    {
            //        datasaved = true;
            //    }
            //    else
            //    {
            //        datasaved = false;
            //    }
            //}


            // 9.6] Updating table FSystemGeneratedEntryMaster with Ledger ID
            if (datasaved)
            {
                updateQuery = "update FSystemGeneratedEntryMaster " +
                                        "set LedgerID=" + LedgerID + " " +
                                "where DJEID=" + DJEID + " ";
                cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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

            //// 9.7] Contra Entry in FLedger 
            if (datasaved)
            {
                int AccID = AccountID;
                int ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                double DebitAmount = 0;

                double CreditAmount = 0;
                if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                {
                    //if (hdnacc.Value != "70")
                    //{
                    CreditAmount = Convert.ToDouble(txtNetPayable.Text);
                    //}
                    //else
                    //{
                    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                    //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                    //}
                }


                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ContraAccID, CashAmount, CreditAmount, AccID, CashNarration);

                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

        }
        //  *********//cash


        //***************************** Accounting Entries for Charges ***************************************


        Narration = "Amount received against Gold Loan charges";

        //getting first Account ID 
        if (dgvChargesDetails != null && dgvChargesDetails.Rows.Count > 0)
        {
            foreach (GridViewRow row in dgvChargesDetails.Rows)
            {
                if ((row.Cells[4].FindControl("hdnaccountid") as HiddenField).Value != "")
                {
                    int CreditID = 0;
                    CreditID = Convert.ToInt32((row.Cells[4].FindControl("hdnaccountid") as HiddenField).Value);

                    if (CreditID != 0)
                    {
                        if (datasaved)
                        {
                            int AccID = AccountID;
                            ContraAccID = CreditID;
                            double DebitAmount = Convert.ToDouble(txtTotalChargesAmount.Text);
                            double CreditAmount = 0;

                            //LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

                            //if (datasaved)
                            //{
                            //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                            //}

                            if (datasaved)
                            {
                                int ID = 0;
                                // retrieving MAX ID
                                strQuery = "SELECT MAX(ID) FROM TGLSanctionDisburse_ChargesPostingDetails";
                                cmd = new SqlCommand(strQuery, conGL, tranGL);
                                cmd.CommandTimeout = 0;
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    ID = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    ID = 0;
                                }
                                ID += 1;

                                insertQuery = "INSERT into TGLSanctionDisburse_ChargesPostingDetails values(" + ID + ", " + value + ", " +
                                                        "'" + GoldLoanNo + "', " + AccID + ", " + DebitAmount + ", " +
                                                        "" + CreditAmount + ", " + LedgerID + ", " + txtFYID.Text + ") ";
                                cmd = new SqlCommand(insertQuery, conGL, tranGL);
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

                        // 9.9] Contra Entry in FLedger (Ledger Entry for Charges)                          
                        if (datasaved == true)
                        {
                            string strCharges = (row.Cells[2].FindControl("lblCharges") as Label).Text;
                            string strChargeType = (row.Cells[3].FindControl("lblChargeType") as Label).Text;
                            string strAmount = (row.Cells[4].FindControl("lblAmount") as Label).Text;
                            string strAccountName = (row.Cells[5].FindControl("lblAccountName") as Label).Text;
                            string strAccountID = (row.Cells[5].FindControl("hdnaccountid") as HiddenField).Value;

                            int AccID = AccountID;
                            int ContraAccID = Convert.ToInt32(strAccountID);
                            double DebitAmount = 0;
                            double CreditAmount = Convert.ToDouble(strAmount);
                            LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                            if (datasaved)
                            {
                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                                if (datasaved == false)
                                {
                                    break;
                                }

                                if (datasaved)
                                {
                                    int ID = 0;
                                    // retrieving MAX ID
                                    strQuery = "SELECT MAX(ID) FROM TGLSanctionDisburse_ChargesPostingDetails";
                                    cmd = new SqlCommand(strQuery, conGL, tranGL);
                                    cmd.CommandTimeout = 0;
                                    if (cmd.ExecuteScalar() != DBNull.Value)
                                    {
                                        ID = Convert.ToInt32(cmd.ExecuteScalar());
                                    }
                                    else
                                    {
                                        ID = 0;
                                    }

                                    ID += 1;

                                    insertQuery = "INSERT into TGLSanctionDisburse_ChargesPostingDetails values(" + ID + ", " + value + ", " +
                                                            "'" + GoldLoanNo + "', " + ContraAccID + ", " + DebitAmount + ", " +
                                                            "" + CreditAmount + ", " + LedgerID + ", " + txtFYID.Text + ") ";

                                    cmd = new SqlCommand(insertQuery, conGL, tranGL);
                                    cmd.CommandTimeout = 0;
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
                            else
                            {
                                datasaved = false;
                                break;
                            }
                        }


                    }

                    #endregion [SaveinAIMTables]

                }
            }
        }

        //***************************** Accounting Entries for Other Charges *********************************

        if (ddlLoanType.SelectedIndex == 1)
        {
            if (datasaved)
            {
                Narration = "Amount received against Gold Loan processing charges";
                int ACCID = 6828;
                int ConAccID = AccountID;
                double DebitAmt = 0;
                double CreditAmt = 0;

                //if (hdnProType.Value == "Percentage")
                //{
                //    DebitAmt = Convert.ToDouble(txtNetAmountSanctioned.Text) * Convert.ToDouble(hdnProcharge.Value) / 100;
                //    DebitAmt = DebitAmt + DebitAmt * Convert.ToDouble(hdnservicetax.Value) / 100;
                //}
                //else
                //{
                //    DebitAmt = Convert.ToDouble(hdnProcharge.Value);
                //    DebitAmt = DebitAmt + DebitAmt * Convert.ToDouble(hdnservicetax.Value) / 100;
                //}

                //LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ACCID, DebitAmt, CreditAmt, ConAccID, Narration);

                //if (datasaved)
                //{
                //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ACCID, DebitAmt, CreditAmt, transactionAIM, connAIM);
                //}



                ACCID = 6828;
                ConAccID = AccountID;
                DebitAmt = 0;
                CreditAmt = 0;
                if (hdnProType.Value == "Percentage")
                {

                    CreditAmt = Convert.ToDouble(txtNetAmountSanctioned.Text) * Convert.ToDouble(hdnProcharge.Value) / 100;
                    if (CreditAmt > Convert.ToDouble(hdnproclimit.Value))
                    {
                        CreditAmt = Convert.ToDouble(hdnproclimit.Value);
                    }
                    //CreditAmt = Math.Round(CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100);//commented due to decimal value is not passing to fledger table
                    CreditAmt = (CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100);
                    CreditAmt = Convert.ToDouble(Decimal.Round(Convert.ToDecimal(CreditAmt), 2));
                }
                else
                {
                    CreditAmt = Convert.ToDouble(hdnProcharge.Value);
                    if (CreditAmt > Convert.ToDouble(hdnproclimit.Value))
                    {
                        CreditAmt = Convert.ToDouble(hdnproclimit.Value);
                    }
                    //CreditAmt = Math.Round(CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100); //commented due to decimal value is not passing to fledger table
                    CreditAmt = (CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100);
                    CreditAmt = Convert.ToDouble(Decimal.Round(Convert.ToDecimal(CreditAmt), 2));
                }

                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ACCID, DebitAmt, CreditAmt, ConAccID, Narration);

                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ConAccID, DebitAmt, CreditAmt, transactionAIM, connAIM);
                }

            }
        }

        //***************************************************************************************************



        #region [Finally if all datasaved=True then commit all Transaction blocks]
        if (datasaved == true)
        {
            tranGL.Commit();
            transactionAIM.Commit();

            //code to send mobile sms to customer's mobile no.
            if (hdnmobileno.Value.Trim() != "" && hdnmobileno.Value.Trim() != null)
            {
                //SendMobileMessage(hdnmobileno.Value.Trim(), txtGoldLoanNo.Text.Trim(), (Convert.ToDouble(txtNetAmountSanctioned.Text.Trim()) - Convert.ToDouble(txtTotalChargesAmount.Text.Trim())), hdnsanctiondate.Value.Trim(), txtChequeNo.Text.Trim());
                // ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('Loan disbursal sms to customer's mobile is sucsessfully sent!');", true);
            }
            //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Saved Successfully.');", true);
            //ClearData();

        }
        else
        {
            tranGL.Rollback();
            transactionAIM.Rollback();
            //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Saved Successfully.');", true);
        }
        #endregion [Finally if all datasaved=True then commit all Transaction blocks]
        return datasaved;
    }
    #endregion [Save_PRI]

    public bool SaveTopup_PRI(string value, SqlTransaction tranGL, SqlConnection conGL, bool saved)
    {
        datasaved = saved;
        int AccountID = 0;
        int NewAccountID = 0;
        int GPID = 67; //Group ID of Sundry Debtors
        int LedgerID = 0;
        int DJEID = 0;
        int DJERefNo = 0;
        string DJERefType = "DJEGL";
        string DJEReferenceNo = string.Empty;
        string PrevDJEReferenceNo = string.Empty;
        double DebitAmount = 0;
        double CreditAmount = 0;
        int QueryResult = 0;
        string Narration = "";

        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();
        transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSaveinAIM");


        strQuery = "select ReferenceNo from FSystemGeneratedEntryMaster where LoginID='" + hdnoldgoldloanno.Value.Trim() + "'";
        cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
        cmd.CommandTimeout = 0;
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            PrevDJEReferenceNo = Convert.ToString(cmd.ExecuteScalar());
        }
        else
        {
            PrevDJEReferenceNo = "";
        }

        //-----------------------------------Closing Old Customer account---------------------------------
        if (datasaved)
        {
            strQuery = "select AccountID from tblAccountMaster where Alies='" + hdnoldgoldloanno.Value.Trim() + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                AccountID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                AccountID = 0;
            }
        }



        //-----------------------Entry for old customer on credit side-----------------------------------------

        //------------------------Create new Account for topup----------------------------------------------

        if (datasaved)
        {

            strQuery = "select max(AccountID) from tblAccountMaster";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                NewAccountID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                NewAccountID = 0;
            }

            NewAccountID += 1;
        }
        //------------------------End of Create new Account for topup----------------------------------------------



        //----------------------End of Closed Cutomer Old account --------------------------------------



        //----------------------Account Entry to Customer new account for topup--------------------------------------------------------------


        if (datasaved)
        {
            insertQuery = "insert into tblAccountMaster values(" + NewAccountID + ", '" + txtCustomerName.Text.Trim() + "', " +
                               "'" + txtGoldLoanNo.Text.Trim() + "', " + GPID + ", 0, 'Dr', '" + txtPanNo.Text.Trim() + "', '0', '-','-', " +
                               "@address, '0','0','-','0', '" + txtAreaID.Text.Trim() + "', '" + txtTelephone.Text.Trim() + "', '" + txtMobile.Text.Trim() + "', " +
                               "'-', '" + txtEmailID.Text.Trim() + "', 'No','')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            cmd.Parameters.AddWithValue("@address", txtAddress.Text.Trim());
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
            //getting MAX ID
            strQuery = "select max(DJEID) from FSystemGeneratedEntryMaster";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DJEID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                DJEID = 0;
            }
            DJEID += 1;
            DJERefNo = DJEID;
            DJEReferenceNo = DJERefType + "/" + Convert.ToString(DJEID);//1st entry

            insertQuery = "insert into FSystemGeneratedEntryMaster values(" + DJEID + ", '" + DJERefType + "', '" + DJERefNo + "', " +
                            "'" + DJEReferenceNo + "', '" + txtGoldLoanNo.Text.Trim() + "', 0, " +
                            "'" + txtFYID.Text + "')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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

        //-------------for interest-------------------------------------
        if (Convert.ToDouble(lblOutInterest.Text) > 0)
        {
            if (datasaved)
            {
                int AccID = AccountID;
                int ContraAccID = 6826;

                DebitAmount = Convert.ToDouble(lblOutInterest.Text);
                CreditAmount = 0;
                Narration = "Amount received against Gold Loan Interest";

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

            if (datasaved)
            {
                int AccID = 6826;
                int ContraAccID = AccountID;

                DebitAmount = 0;
                CreditAmount = Convert.ToDouble(lblOutInterest.Text);

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

        }
        //------------------------------end of interest------------------------------------


        //------------------------for penal interest-----------------------------------------
        if (Convert.ToDouble(lblOutPInterest.Text) > 0)
        {
            if (datasaved)
            {
                int AccID = AccountID;
                int ContraAccID = 6827;

                DebitAmount = Convert.ToDouble(lblOutPInterest.Text);
                CreditAmount = 0;
                Narration = "Amount received against Gold Loan penal Interest";

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

            if (datasaved)
            {
                int AccID = 6827;
                int ContraAccID = AccountID;

                DebitAmount = 0;
                CreditAmount = Convert.ToDouble(lblOutPInterest.Text);

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

        }

        //--------------------end of penal interest------------------------------------------------

        //------------------------for Charges --------------------------------------------------
        if (Convert.ToDouble(lblOutCharges.Text) > 0)
        {
            if (datasaved)
            {
                int AccID = AccountID;
                int ContraAccID = 6828;

                DebitAmount = Convert.ToDouble(lblOutCharges.Text);
                CreditAmount = 0;
                Narration = "Amount received against Gold Loan charges";

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

            if (datasaved)
            {
                int AccID = 6828;
                int ContraAccID = AccountID;

                DebitAmount = 0;
                CreditAmount = Convert.ToDouble(lblOutCharges.Text);

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

        }

        //--------------------end of ------------------------------------------------------------------------------
        //------------------------Entry for total outstanding to Customer old account on credit side----------------------------------------------
        if (Convert.ToDouble(lblOutTotal.Text) > 0)
        {

            if (datasaved)
            {
                int AccID = AccountID;
                int ContraAccID = NewAccountID;

                DebitAmount = 0;
                CreditAmount = Convert.ToDouble(lblOutTotal.Text);
                Narration = "Amount received against Top Up Gold Loan outstanding";

                LedgerID = CreateNormalLedgerEntries(DJERefType, PrevDJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                }
            }

        }
        //-----------------------End of Entry for old customer on credit side-----------------------------------------
        //for debit entry //priya 3-6-2015
        int BCPID = 0;
        int VoucherNo = 0;
        int PaidTo = NewAccountID;
        //int BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        int BankCashAccID = 0;
        double Amount = 0;

        if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        {
            if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
        }
        else
        {

            if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
        }

        Amount = Convert.ToDouble(txtNetPayable.Text);

        Narration = "Payment made against Top up Gold Loan sanctioned";//debit only

        if (datasaved)
        {
            // retrieving MAX BCPID
            strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                BCPID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                BCPID = 0;
            }
            BCPID += 1;

            // retrieving MAX VoucherNo
            strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                VoucherNo = 0;
            }
            VoucherNo += 1;

            string strChqDate = string.Empty;
            if (txtChequeDate.Text.Trim() != "")
            {
                strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
            }
            insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
                                    "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
                                    "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
                                    "'" + BankCashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
                                    "'" + strChqDate + "', '" + Narration + "', 0, " +
                                    "'" + txtFYID.Text + "')";
            cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
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

        // string BankNarration = "Payment made By Bank against Top up Gold Loan sanctioned";

        if (datasaved)
        {
            //if (hdnacc.Value != "70")
            //{
            DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
            CreditAmount = 0;
            //string BankNarration = "Payment made By Bank against Top up Gold Loan sanctioned";
            //}
            //else
            //{
            //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            //    DebitAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
            //    CreditAmount = 0;
            //}

            int AccID = NewAccountID; int ContraAccID = 0;
            //  int ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
            {
                ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
            else
            {
                ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }


            LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

            if (datasaved)
            {
                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            }


            // 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
            if (datasaved)
            {
                updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                                        "set BCPID=" + BCPID + " " +
                                "where SDID=" + value + " ";
                cmd = new SqlCommand(updateQuery, conGL, tranGL);
                cmd.CommandTimeout = 0;
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


            // 9.5] Updating table TBankCash_PaymentDetails with Ledger ID                   
            if (datasaved)
            {
                updateQuery = "update TBankCash_PaymentDetails " +
                                "set LedgerID=" + LedgerID + " " +
                                "where BCPID=" + BCPID + " ";

                cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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


            // 9.6] Updating table FSystemGeneratedEntryMaster with Ledger ID
            if (datasaved)
            {
                updateQuery = "update FSystemGeneratedEntryMaster " +
                                        "set LedgerID=" + LedgerID + " " +
                                "where DJEID=" + DJEID + " ";
                cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 0;
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

            // 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
            if (datasaved)
            {
                updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                                        "set BCPID=" + BCPID + " " +
                                "where SDID=" + value + " ";
                cmd = new SqlCommand(updateQuery, conGL, tranGL);
                cmd.CommandTimeout = 0;
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

            //-------------------------------Contra entry-------------------------------
            //for bank amt //priya
            if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
            {
                string BankNarration = "Payment made by Bank against Top up Gold Loan sanctioned";
                if (datasaved)
                {
                    // retrieving MAX BCPID
                    strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
                    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        BCPID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        BCPID = 0;
                    }
                    BCPID += 1;

                    // retrieving MAX VoucherNo
                    strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
                    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        VoucherNo = 0;
                    }
                    VoucherNo += 1;
                    Amount = Convert.ToDouble(txtBankAmount.Text);
                    string strChqDate = string.Empty;
                    if (txtChequeDate.Text.Trim() != "")
                    {
                        strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
                    }
                    //insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
                    //                        "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
                    //                        "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
                    //                        "'" + BankCashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
                    //                        "'" + strChqDate + "', '" + BankNarration + "', 0, " +
                    //                        "'" + txtFYID.Text + "')";
                    //cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
                    //QueryResult = cmd.ExecuteNonQuery();

                    //if (QueryResult > 0)
                    //{
                    //    datasaved = true;
                    //}
                    //else
                    //{
                    //    datasaved = false;
                    //}
                }

                //for credit entry customer
                if (datasaved)
                {
                    // string BankNarration = "Payment made by Bank against Top up Gold Loan sanctioned";
                    CreditAmount = Convert.ToDouble(txtBankAmount.Text);
                    DebitAmount = 0;
                    // }
                    //else
                    //{
                    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                    //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                    //    DebitAmount = 0;
                    //}

                    AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                    ContraAccID = NewAccountID;

                    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, BankNarration);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }


                    //// 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
                    //if (datasaved)
                    //{
                    //    updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                    //                            "set BCPID=" + BCPID + " " +
                    //                    "where SDID=" + value + " ";
                    //    cmd = new SqlCommand(updateQuery, conGL, tranGL);
                    //    cmd.CommandTimeout = 0;
                    //    QueryResult = cmd.ExecuteNonQuery();

                    //    if (QueryResult > 0)
                    //    {
                    //        datasaved = true;
                    //    }
                    //    else
                    //    {
                    //        datasaved = false;
                    //    }
                    //}

                }
                //// 9.5] Updating table TBankCash_PaymentDetails with Ledger ID                   
                //if (datasaved)
                //{
                //    updateQuery = "update TBankCash_PaymentDetails " +
                //                    "set LedgerID=" + LedgerID + " " +
                //                    "where BCPID=" + BCPID + " ";

                //    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                //    cmd.CommandTimeout = 0;
                //    QueryResult = cmd.ExecuteNonQuery();

                //    if (QueryResult > 0)
                //    {
                //        datasaved = true;
                //    }
                //    else
                //    {
                //        datasaved = false;
                //    }
                //}


                // 9.6] Updating table FSystemGeneratedEntryMaster with Ledger ID
                if (datasaved)
                {
                    updateQuery = "update FSystemGeneratedEntryMaster " +
                                            "set LedgerID=" + LedgerID + " " +
                                    "where DJEID=" + DJEID + " ";
                    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
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

            //*************cash a/c

            if (txtCashAmount.Text != "" && Convert.ToDecimal(txtCashAmount.Text.Trim()) > 0)
            {
                string CashNarration = "Payment made by Cash against Top up Gold Loan sanctioned";
                if (datasaved)
                {
                    // retrieving MAX BCPID
                    strQuery = "SELECT MAX(BCPID) FROM TBankCash_PaymentDetails";
                    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        BCPID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        BCPID = 0;
                    }
                    BCPID += 1;

                    // retrieving MAX VoucherNo
                    strQuery = "SELECT MAX(VoucherNo) FROM TBankCash_PaymentDetails";
                    cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        VoucherNo = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        VoucherNo = 0;
                    }
                    VoucherNo += 1;
                    Amount = Convert.ToDouble(txtCashAmount.Text);

                    string strChqDate = string.Empty;
                    if (txtChequeDate.Text.Trim() != "")
                    {
                        strChqDate = Convert.ToDateTime(txtChequeDate.Text.Trim()).ToString("yyyy/MM/dd");
                    }
                    //insertQuery = "INSERT into TBankCash_PaymentDetails values('" + BCPID + "', '" + DJERefType + "'," +
                    //                        "'" + DJERefNo + "', '" + DJEReferenceNo + "', " +
                    //                        "'" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', '" + VoucherNo + "', " +
                    //                        "'" + BankCashAccID + "', '" + PaidTo + "', '" + Amount + "', '" + txtChequeNo.Text.Trim() + "', " +
                    //                        "'" + strChqDate + "', '" + CashNarration + "', 0, " +
                    //                        "'" + txtFYID.Text + "')";
                    //cmd = new SqlCommand(insertQuery, connAIM, transactionAIM);
                    //QueryResult = cmd.ExecuteNonQuery();

                    //if (QueryResult > 0)
                    //{
                    //    datasaved = true;
                    //}
                    //else
                    //{
                    //    datasaved = false;
                    //}
                }

                //for credit entry customer
                if (datasaved)
                {
                    //Narration = "Payment made by Cash against Top up Gold Loan sanctioned";
                    CreditAmount = Convert.ToDouble(txtCashAmount.Text);
                    DebitAmount = 0;
                    // }
                    //else
                    //{
                    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                    //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
                    //    DebitAmount = 0;
                    //}

                    AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                    ContraAccID = NewAccountID;

                    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, CashNarration);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                    }

                    //// 9.4] Updating table tbl_GLSanctionDisburse_BasicDetails with Bank Cash Payment ID
                    //if (datasaved)
                    //{
                    //    updateQuery = "update TGLSanctionDisburse_BasicDetails " +
                    //                            "set BCPID=" + BCPID + " " +
                    //                    "where SDID=" + value + " ";
                    //    cmd = new SqlCommand(updateQuery, conGL, tranGL);
                    //    cmd.CommandTimeout = 0;
                    //    QueryResult = cmd.ExecuteNonQuery();

                    //    if (QueryResult > 0)
                    //    {
                    //        datasaved = true;
                    //    }
                    //    else
                    //    {
                    //        datasaved = false;
                    //    }
                    //}

                }
                //// 9.5] Updating table TBankCash_PaymentDetails with Ledger ID                   
                //if (datasaved)
                //{
                //    updateQuery = "update TBankCash_PaymentDetails " +
                //                    "set LedgerID=" + LedgerID + " " +
                //                    "where BCPID=" + BCPID + " ";

                //    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                //    cmd.CommandTimeout = 0;
                //    QueryResult = cmd.ExecuteNonQuery();

                //    if (QueryResult > 0)
                //    {
                //        datasaved = true;
                //    }
                //    else
                //    {
                //        datasaved = false;
                //    }
                //}


                // 9.6] Updating table FSystemGeneratedEntryMaster with Ledger ID
                if (datasaved)
                {
                    updateQuery = "update FSystemGeneratedEntryMaster " +
                                            "set LedgerID=" + LedgerID + " " +
                                    "where DJEID=" + DJEID + " ";
                    cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
                    cmd.CommandTimeout = 0;
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


            ////added priya 
            //if (txtBankAmount.Text != "")
            //{

            //    //if (hdnacc.Value != "70")
            //    //{
            //    CreditAmount = Convert.ToDouble(txtBankAmount.Text);
            //    DebitAmount = 0;
            //    // }
            //    //else
            //    //{
            //    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            //    //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
            //    //    DebitAmount = 0;
            //    //}

            //    AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            //    ContraAccID = NewAccountID;

            //    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

            //    if (datasaved)
            //    {
            //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            //    }
            //}

            //if (txtCashAmount.Text != "")
            //{

            //    //if (hdnacc.Value != "70")
            //    //{
            //    CreditAmount = Convert.ToDouble(txtCashAmount.Text);
            //    DebitAmount = 0;
            //    // }
            //    //else
            //    //{
            //    //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            //    //    CreditAmount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);
            //    //    DebitAmount = 0;
            //    //}

            //    AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            //    ContraAccID = NewAccountID;

            //    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);

            //    if (datasaved)
            //    {
            //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            //    }
            //}
            ////end by priya
        }

        #region [Finally if all datasaved=True then commit all Transaction blocks]
        if (datasaved == true)
        {
            tranGL.Commit();
            transactionAIM.Commit();

            //code to send mobile sms to customer's mobile no.
            if (hdnmobileno.Value.Trim() != "" && hdnmobileno.Value.Trim() != null)
            {
                // SendMobileMessage(hdnmobileno.Value.Trim(), txtGoldLoanNo.Text.Trim(), (Convert.ToDouble(txtNetAmountSanctioned.Text.Trim()) - Convert.ToDouble(txtTotalChargesAmount.Text.Trim())), txtLoanDate.Text.Trim(), txtChequeNo.Text.Trim());
                // ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('Loan disbursal sms to customer's mobile is sucsessfully sent!');", true);
            }
            //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Saved Successfully.');", true);
            //ClearData();

        }
        else
        {
            tranGL.Rollback();
            transactionAIM.Rollback();
            //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Saved Successfully.');", true);
        }
        #endregion [Finally if all datasaved=True then commit all Transaction blocks]
        return datasaved;
    }

    #region [SendMobileMessage]
    public void SendMobileMessage(string MobileNo, string GoldLoanNo, double SanctionedLoanAmount, string SanctionedLoanDate, string ChqNEFTNo)
    {
        try
        {
            if (MobileNo.Trim() != "")
            {
                string Message = string.Empty;
                string sURL;
                string Success = string.Empty;
                if (ddlPaymentMode.SelectedIndex == 1)
                {
                    if (ddlcheqNEFTDD.SelectedIndex == 1)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + SanctionedLoanAmount + " has been disbursed on " + SanctionedLoanDate + " vide Cheque No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";
                    }
                    if (ddlcheqNEFTDD.SelectedIndex == 2)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + SanctionedLoanAmount + " has been disbursed on " + SanctionedLoanDate + " vide NEFT Transaction No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";

                    }
                    if (ddlcheqNEFTDD.SelectedIndex == 3)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + SanctionedLoanAmount + " has been disbursed on " + SanctionedLoanDate + " vide DD No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";
                    }

                }
                else if (ddlPaymentMode.SelectedIndex == 2)
                {

                    Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + SanctionedLoanAmount + " has been disbursed on " + SanctionedLoanDate + " vide Cash. Thankyou for choosing Aphelion Finance.";

                }

                else if (ddlPaymentMode.SelectedIndex == 3)
                {
                    Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + txtCashAmount.Text + " has been disbursed on " + SanctionedLoanDate + " vide Cash. Thankyou for choosing Aphelion Finance.";

                    StreamReader objReader1;
                    sURL = Message;
                    WebRequest wrGETURL1;
                    wrGETURL1 = WebRequest.Create(sURL);

                    Stream objStream1;
                    objStream1 = wrGETURL1.GetResponse().GetResponseStream();
                    objReader1 = new StreamReader(objStream1);
                    objReader1.Close();

                    if (ddlcheqNEFTDD.SelectedIndex == 1)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + txtBankAmount.Text + " has been disbursed on " + SanctionedLoanDate + " vide Cheque No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";
                    }
                    if (ddlcheqNEFTDD.SelectedIndex == 2)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + txtBankAmount.Text + " has been disbursed on " + SanctionedLoanDate + " vide NEFT Transaction No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";

                    }
                    if (ddlcheqNEFTDD.SelectedIndex == 3)
                    {
                        Message = "http://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + MobileNo + "&source=AphGLN&message=Dear Customer, your Gold Loan (" + GoldLoanNo + ") of Rs." + txtBankAmount.Text + " has been disbursed on " + SanctionedLoanDate + " vide DD No." + ChqNEFTNo + ". Thankyou for choosing Aphelion Finance.";
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
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [SendMobileMessage]

    #region [Update_PRI]
    public bool Update_PRI(string value, SqlTransaction tranGL, SqlConnection conGL, bool saved)
    {

        datasaved = saved;
        cmd = new SqlCommand();
        cmd.Connection = conGL;
        cmd.Transaction = tranGL;
        cmd.CommandTimeout = 0;
        cmd.CommandText = "select GoldLoanNo From TGLSanctionDisburse_BasicDetails where SDID='" + value + "'";
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            GoldLoanNo = Convert.ToString(cmd.ExecuteScalar());
        }


        #region [ConnString & Transaction Block for AIM]
        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();
        transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForAIM");
        #endregion [ConnString & Transaction Block for AIM]



        #region [Variables & Objects Declaration]
        int QueryResult = 0;
        string DocRecd = string.Empty;
        string ImagePath = string.Empty;
        string ItemName = string.Empty;
        string GrossWeight = string.Empty;
        string strQuantity = string.Empty;
        string ImageName = string.Empty;
        string strGID = string.Empty;
        string GIDForUpdate = string.Empty;

        #endregion [Variables & Objects Declaration]

        string ID = string.Empty;
        string CID = string.Empty;
        string ChargeName = string.Empty;
        string LoanAmtFrom = string.Empty;
        string LoanAmtTo = string.Empty;
        string Charges = string.Empty;
        string ChargeType = string.Empty;
        string AccountID = string.Empty;
        string ChargeAmount = string.Empty;
        string strCHID = string.Empty;
        string CHIDForUpdate = string.Empty;
        string DJERefType = string.Empty;
        string DJEReferencNo = string.Empty;


        int AccID = 0;
        int LedgerID = 0;
        double DebitAmount = 0;
        double CreditAmount = 0;

        // 2.3] Deleting effects from Company-wise Account Closing table for Charges Details
        strQuery = "select AccID, Debit, Credit, LedgerID from TGLSanctionDisburse_ChargesPostingDetails " +
                    "where SDID='" + value + "'";
        cmd = new SqlCommand(strQuery, conGL, tranGL);
        cmd.CommandTimeout = 0;
        cmd.CommandType = CommandType.Text;
        da = new SqlDataAdapter(cmd);
        ds = new DataSet();
        da.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
            foreach (DataRow drow1 in ds.Tables[0].Rows)
            {
                AccID = Convert.ToInt32(drow1[0]);
                DebitAmount = Convert.ToDouble(drow1[1]);
                CreditAmount = Convert.ToDouble(drow1[2]);
                LedgerID = Convert.ToInt32(drow1[3]);

                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                    if (datasaved == false)
                    {
                        break;
                    }

                    //Deleting data from table FLedgerMaster 
                    if (datasaved)
                    {
                        deleteQuery = "delete from FLedgerMaster " +
                                        "where LedgerID=" + LedgerID + "";
                        cmd = new SqlCommand(deleteQuery, connAIM, transactionAIM);
                        cmd.CommandTimeout = 0;
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
                }
            }
        }

        // 2.4] deletion of data from tbl_GLSanctionDisburse_ChargesPostingDetails
        if (datasaved == true)
        {

            strQuery = "Select count(*) from TGLSanctionDisburse_ChargesPostingDetails " +
                          "where SDID='" + value + "'";
            cmd = new SqlCommand(strQuery, conGL, tranGL);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                excount = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                excount = 0;
            }

            if (excount > 0)
            {
                deleteQuery = "delete from TGLSanctionDisburse_ChargesPostingDetails " +
                              "where SDID='" + value + "'";
                cmd = new SqlCommand(deleteQuery, conGL, tranGL);
                cmd.CommandTimeout = 0;
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




        //+++++++++++++++++++++++++++++++++ UPDATION OF LEDGER ENTRIES ++++++++++++++++++++++++++++++++++++++++++++                
        // 8.1] Updation of Bank Cash Payment Details

        //for debit entry only //priya
        int BCPID = 0, BankCashAccID = 0;
        if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        {
            if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
        }
        else
        {

            if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
            {
                BankCashAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
            }
            else
            {
                BankCashAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
            }
        }


        if (datasaved)
        {

            strQuery = "select BCPID from TGLSanctionDisburse_BasicDetails where SDID='" + value + "'";
            cmd = new SqlCommand(strQuery, conGL, tranGL);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                BCPID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                BCPID = 0;
            }

            double Amount = 0;
            if (ddlLoanType.SelectedIndex == 1)
            {

                //if (hdnacc.Value != "70")
                //{
                Amount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                //}
                //else
                //{
                //    TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                //    Amount = Convert.ToDouble(gvtxtDenoTotalAmt.Text);

                //}
            }
            if (ddlLoanType.SelectedIndex == 2)
            {
                Amount = Convert.ToDouble(txtNetPayable.Text);
            }


            //Updating table TBankCash_PaymentDetails
            updateQuery = "update TBankCash_PaymentDetails " +
                                    "set RefDate='" + gbl.ChangeDateMMddyyyy(txtLoanDate.Text) + "', " +
                                    "BankCashAccID=" + BankCashAccID + ", " +
                                    "Amount=" + Amount + " " +
                            "where BCPID=" + BCPID + " ";
            cmd = new SqlCommand(updateQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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

        // 8.2] updation of Ledger and Company Wise Account Closing tables
        if (datasaved)
        {
            // getting ReferenceNo 

            int accountID = 0;
            LedgerID = 0;
            AccID = 0;
            int ContraAccID = 0;
            int ContraID = 0;
            double debit, credit = 0;
            DebitAmount = 0;
            CreditAmount = 0;
            DateTime dtRefDate;

            strQuery = "select ReferenceNo from FSystemGeneratedEntryMaster where LoginID='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DJEReferencNo = Convert.ToString(cmd.ExecuteScalar());
            }
            else
            {
                DJEReferencNo = "";
            }

            strQuery = "select AccountID, Debit, Credit, RefDate, LedgerID from FLedgerMaster " +
                        "where ReferenceNo='" + DJEReferencNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
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
                        accountID = Convert.ToInt32(drow1[0]);

                        debit = Convert.ToDouble(drow1[1]);
                        credit = Convert.ToDouble(drow1[2]);
                        dtRefDate = Convert.ToDateTime(drow1[3]);
                        LedgerID = Convert.ToInt32(drow1[4]);
                        DebitAmount = 0;
                        CreditAmount = 0;
                        //    AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);

                        if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
                        {
                            if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
                            {
                                AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                            }
                            else
                            {
                                AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                            }
                        }
                        else
                        {

                            if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
                            {
                                AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                            }
                            else
                            {
                                AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                            }
                        }
                        ContraAccID = ContraID;


                        DebitAmount = 0;

                        if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                        {
                            CreditAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);

                        }
                        else if (ddlLoanType.SelectedIndex == 2)    //for Topup Loan
                        {
                            CreditAmount = Convert.ToDouble(Convert.ToDouble(txtNetPayable.Text));
                        }

                        if (datasaved)
                        {
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), accountID, debit, credit, transactionAIM, connAIM);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                strQuery = "delete from FLedgerMaster where ReferenceNo='" + DJEReferencNo + "'";
                cmd = new SqlCommand(strQuery, connAIM, transactionAIM);

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

        //**************************** Accounting Entries for Charges ***************************************

        int CreditID = 0;
        int accountId = 0;
        // Narration = "Amount received against Gold Loan charges";//previous entry

        Narration = "Payment made against New Gold Loan sanctioned"; //change by priya
        DJERefType = "DJEGL";

        //getting AccountID
        strQuery = "select AccountID from tblAccountMaster where Alies='" + GoldLoanNo + "'";
        cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
        cmd.CommandTimeout = 0;
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            accountId = Convert.ToInt32(cmd.ExecuteScalar());
        }
        else
        {
            accountId = 0;
        }

        if (datasaved)
        {
            strQuery = "select ReferenceNo from FSystemGeneratedEntryMaster where LoginID='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 0;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DJEReferencNo = Convert.ToString(cmd.ExecuteScalar());
            }
            else
            {
                DJEReferencNo = "";
            }
        }

        //added on 3-6-2015 for leger entry

        if (datasaved)
        {
            AccID = accountId;
            int ContraAccID = 0;
            //   int ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);

            if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
            {
                if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
                {
                    ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                }
                else
                {
                    ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                }
            }
            else
            {
                if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
                {
                    ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                }
                else
                {
                    ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                }
            }
            DebitAmount = 0;
            CreditAmount = 0;

            DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);

            LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
            if (datasaved)
            {
                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
            }

            //**************

            if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
            {
                int refID = 0;
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
                    ContraAccID = accountId;
                    string BankNarration1 = "";
                    double DebitBankAmount = 0, DebitCashAmount = 0;
                    if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                    {
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            DebitBankAmount = Convert.ToDouble(txtBankAmount.Text);
                            BankNarration1 = "Payment made by Bank against New Gold Loan sanctioned";
                        }
                    }
                    else if (ddlLoanType.SelectedIndex == 2)
                    {
                        if (ddlPaymentMode.SelectedIndex == 1 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            DebitBankAmount = Convert.ToDouble(txtBankAmount.Text);
                            BankNarration1 = "Payment made by Bank against New Gold Loan sanctioned";
                        }
                    }
                    // double CreditAmount = 0;
                    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, CreditAmount, DebitBankAmount, ContraAccID, BankNarration1);
                    //  LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitBankAmount, CreditAmount, ContraAccID, BankNarration);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, CreditAmount, DebitBankAmount, transactionAIM, connAIM);
                    }
                }
            }

            if (txtCashAmount.Text != "" && Convert.ToDecimal(txtCashAmount.Text.Trim()) > 0)
            {
                int refID = 0;
                if (datasaved)
                {
                    AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                    ContraAccID = accountId;
                    string CashNarration1 = "";
                    double DebitCashAmount = 0;
                    if (ddlLoanType.SelectedIndex == 1)     //for New Loan
                    {
                        if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            DebitCashAmount = Convert.ToDouble(txtCashAmount.Text);
                            CashNarration1 = "Payment made by Cash against New Gold Loan sanctioned";
                        }
                    }
                    else if (ddlLoanType.SelectedIndex == 2)
                    {
                        if (ddlPaymentMode.SelectedIndex == 2 || ddlPaymentMode.SelectedIndex == 3)
                        {
                            DebitCashAmount = Convert.ToDouble(txtCashAmount.Text);
                            CashNarration1 = "Payment made by Cash against New Gold Loan sanctioned";
                        }
                    }
                    // double CreditAmount = 0;
                    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, CreditAmount, DebitCashAmount, ContraAccID, CashNarration1);
                    //  LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitBankAmount, CreditAmount, ContraAccID, BankNarration);

                    if (datasaved)
                    {
                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, CreditAmount, DebitCashAmount, transactionAIM, connAIM);
                    }
                }
            }
        }

        //commented on 3-6-2015

        //// 9.3] Debit Entry in FLedger (Main Ledger Entry)
        //if (datasaved)
        //{
        //    AccID = accountId;
        //    int ContraAccID = 0;

        //    //   int ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);

        //    if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        //    {
        //        if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
        //        {
        //            ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        //        }
        //        else
        //        {
        //            ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
        //        }
        //    }
        //    else
        //    {

        //        if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
        //        {
        //            ContraAccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
        //        }
        //        else
        //        {
        //            ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        //        }
        //    }


        //    DebitAmount = 0;
        //    CreditAmount = 0;

        //    DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);

        //    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
        //    if (datasaved)
        //    {
        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
        //    }
        //    //**************

        //    //AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        //    if (txtBankAmount.Text != "" && Convert.ToDecimal(txtBankAmount.Text.Trim()) > 0)
        //    {
        //        if (ddlBankAccount.SelectedValue == "" || ddlBankAccount.SelectedValue == null)
        //        {
        //            AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        //        }
        //        else
        //        {
        //            AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
        //        }
        //    }
        //    else
        //    {

        //        if (ddlCashAccount.SelectedValue == "" || ddlCashAccount.SelectedValue == null)
        //        {
        //            AccID = Convert.ToInt32(ddlBankAccount.SelectedValue);
        //        }
        //        else
        //        {
        //            AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
        //        }
        //    }


        //    ContraAccID = accountId;

        //    DebitAmount = 0;
        //    CreditAmount = 0;

        //    CreditAmount = Convert.ToDouble(txtNetPayable.Text);

        //    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
        //    if (datasaved)
        //    {
        //        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
        //    }

        //}



        //getting first Account ID 
        if (dgvChargesDetails != null && dgvChargesDetails.Rows.Count > 0)
        {
            foreach (GridViewRow row in dgvChargesDetails.Rows)
            {
                if ((row.Cells[4].FindControl("hdnaccountid") as HiddenField).Value != "")
                {
                    CreditID = Convert.ToInt32((row.Cells[4].FindControl("hdnaccountid") as HiddenField).Value);
                    break;
                }
            }
        }



        //RefType and ReferenceNo


        strQuery = "SELECT DISTINCT RefType, ReferenceNo " +
                    "FROM FSystemGeneratedEntryMaster " +
                    "WHERE  LoginID='" + GoldLoanNo + "'";

        cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
        cmd.CommandTimeout = 0;
        da = new SqlDataAdapter(cmd);
        ds = new DataSet();
        da.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
            DJERefType = Convert.ToString(ds.Tables[0].Rows[0][0]);
            DJEReferencNo = Convert.ToString(ds.Tables[0].Rows[0][1]);
        }

        // 9.8] Debit Entry in FLedger (Main Ledger Entry for Charges)
        if (datasaved)
        {
            if (CreditID != 0)
            {
                AccID = accountId;
                int ContraAccID = CreditID;
                DebitAmount = Convert.ToDouble(txtTotalChargesAmount.Text);
                CreditAmount = 0;
                //LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                //if (datasaved)
                //{
                //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);
                //}

                if (datasaved)
                {
                    int PID = 0;
                    // retrieving MAX ID
                    strQuery = "SELECT MAX(ID) FROM TGLSanctionDisburse_ChargesPostingDetails";
                    cmd = new SqlCommand(strQuery, conGL, tranGL);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        PID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        PID = 0;
                    }

                    PID += 1;

                    insertQuery = "INSERT into TGLSanctionDisburse_ChargesPostingDetails values('" + PID + "', '" + value + "', " +
                                            "'" + GoldLoanNo + "', '" + AccID + "', '" + DebitAmount + "', " +
                                            "'" + CreditAmount + "', '" + LedgerID + "', '" + txtFYID.Text + "')";

                    cmd = new SqlCommand(insertQuery, conGL, tranGL);
                    cmd.CommandTimeout = 0;
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

        // 9.9] Contra Entry in FLedger (Ledger Entry for Charges)
        if (datasaved == true)
        {
            if (CreditID != 0)
            {
                if (dgvChargesDetails != null && dgvChargesDetails.Rows.Count > 0)
                {
                    foreach (GridViewRow row in dgvChargesDetails.Rows)
                    {
                        if (datasaved == true)
                        {
                            string strCharges = (row.Cells[2].FindControl("lblCharges") as Label).Text;
                            string strChargeType = (row.Cells[3].FindControl("lblChargeType") as Label).Text;
                            string strAmount = (row.Cells[4].FindControl("lblAmount") as Label).Text;
                            string strAccountName = (row.Cells[5].FindControl("lblAccountName") as Label).Text;
                            string strAccountID = (row.Cells[5].FindControl("hdnaccountid") as HiddenField).Value;

                            AccID = accountId;
                            int ContraAccID = Convert.ToInt32(strAccountID);
                            DebitAmount = 0;
                            CreditAmount = Convert.ToDouble(strAmount);
                            LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                            if (datasaved)
                            {
                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

                                if (datasaved == false)
                                {
                                    break;
                                }

                                if (datasaved)
                                {
                                    int PID = 0;
                                    // retrieving MAX ID
                                    strQuery = "SELECT MAX(ID) FROM TGLSanctionDisburse_ChargesPostingDetails";
                                    cmd = new SqlCommand(strQuery, conGL, tranGL);
                                    cmd.CommandTimeout = 0;
                                    if (cmd.ExecuteScalar() != DBNull.Value)
                                    {
                                        PID = Convert.ToInt32(cmd.ExecuteScalar());
                                    }
                                    else
                                    {
                                        PID = 0;
                                    }

                                    PID += 1;

                                    insertQuery = "INSERT into TGLSanctionDisburse_ChargesPostingDetails values('" + PID + "', '" + value + "', " +
                                                            "'" + GoldLoanNo + "', '" + ContraAccID + "', '" + DebitAmount + "', " +
                                                            "'" + CreditAmount + "', '" + LedgerID + "', '" + txtFYID.Text + "') ";

                                    cmd = new SqlCommand(insertQuery, conGL, tranGL);
                                    cmd.CommandTimeout = 0;
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
                            else
                            {
                                datasaved = false;
                                break;
                            }
                        }
                    }
                }
            }
        }


        //***************************** Accounting Entries for Other Charges *********************************

        if (ddlLoanType.SelectedIndex == 1)
        {
            if (datasaved)
            {
                Narration = "Amount received against Gold Loan processing charges";
                int ACCID = 6828;
                int ConAccID = accountId;
                double DebitAmt = 0;
                double CreditAmt = 0;

                //if (hdnProType.Value == "Percentage")
                //{
                //    DebitAmt = Convert.ToDouble(txtNetAmountSanctioned.Text) * Convert.ToDouble(hdnProcharge.Value) / 100;
                //    DebitAmt = DebitAmt + DebitAmt * Convert.ToDouble(hdnservicetax.Value) / 100;
                //}
                //else
                //{
                //    DebitAmt = Convert.ToDouble(hdnProcharge.Value);
                //    DebitAmt = DebitAmt + DebitAmt * Convert.ToDouble(hdnservicetax.Value) / 100;
                //}

                //LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferenceNo, Convert.ToDateTime(txtLoanDate.Text), ACCID, DebitAmt, CreditAmt, ConAccID, Narration);

                //if (datasaved)
                //{
                //    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ACCID, DebitAmt, CreditAmt, transactionAIM, connAIM);
                //}



                ACCID = 6828;
                ConAccID = accountId;
                DebitAmt = 0;
                CreditAmt = 0;
                if (hdnProType.Value == "Percentage")
                {
                    CreditAmt = Convert.ToDouble(txtNetAmountSanctioned.Text) * Convert.ToDouble(hdnProcharge.Value) / 100;
                    if (CreditAmt > Convert.ToDouble(hdnproclimit.Value))
                    {
                        CreditAmt = Convert.ToDouble(hdnproclimit.Value);
                    }
                    //CreditAmt = Math.Round(CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100); //commented due decial
                    CreditAmt = (CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100);
                    CreditAmt = Convert.ToDouble(Decimal.Round(Convert.ToDecimal(CreditAmt), 2));
                }
                else
                {
                    CreditAmt = Convert.ToDouble(hdnProcharge.Value);
                    if (CreditAmt > Convert.ToDouble(hdnproclimit.Value))
                    {
                        CreditAmt = Convert.ToDouble(hdnproclimit.Value);
                    }
                    //CreditAmt = Math.Round(CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100);
                    CreditAmt = (CreditAmt + CreditAmt * Convert.ToDouble(hdnservicetax.Value) / 100); //commented due decial
                    CreditAmt = Convert.ToDouble(Decimal.Round(Convert.ToDecimal(CreditAmt), 2));
                }

                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtLoanDate.Text), ACCID, DebitAmt, CreditAmt, ConAccID, Narration);

                if (datasaved)
                {
                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ConAccID, DebitAmt, CreditAmt, transactionAIM, connAIM);
                }

            }
        }

        //***************************************************************************************************
        if (QueryResult > 0)
        {
            tranGL.Commit();
            transactionAIM.Commit();
            //ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('Record Updated Successfully.');", true);


            //-----------For sending sms--------------------------------------------------
            if (hdnmobileno.Value.Trim() != "" && hdnmobileno.Value.Trim() != null)
            {
                // SendMobileMessage(hdnmobileno.Value.Trim(), txtGoldLoanNo.Text.Trim(), (Convert.ToDouble(txtNetAmountSanctioned.Text.Trim()) - Convert.ToDouble(txtTotalChargesAmount.Text)), hdnsanctiondate.Value.Trim(), txtChequeNo.Text.Trim());
                //ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('Loan disbursal sms to customer's mobile is sucsessfully sent!');", true);
            }


        }
        else
        {
            tranGL.Rollback();
            transactionAIM.Rollback();
            // ClientScript.RegisterStartupScript(this.GetType(), "GLS&DDetails", "alert('Record cannot be Updated Successfully.');", true);
        }
        return datasaved;
    }

    #endregion [Update_PRI]

    #region [Delete_PRI]
    public bool Delete_PRI(string value, SqlTransaction tranGL, SqlConnection conGL, bool saved)
    {

        #region [Variables&Objects_Declaration]
        //Variable Declaration
        datasaved = saved;
        int existcount = 0;
        int QueryResult = 0;
        string DJEReferenceNo = string.Empty;
        int AccID = 0;
        int DelAccID = 0;
        double DebitAmount = 0;
        double CreditAmount = 0;

        connAIM = new SqlConnection(strConnStringAIM);
        connAIM.Open();
        transactionAIM = connAIM.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForAIM");

        cmd = new SqlCommand();
        cmd.Connection = conGL;
        cmd.Transaction = tranGL;
        cmd.CommandTimeout = 500;
        cmd.CommandText = "select GoldLoanNo From TGLSanctionDisburse_BasicDetails where SDID='" + value + "'";
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            GoldLoanNo = Convert.ToString(cmd.ExecuteScalar());
        }



        //Object Declaration
        DateTime RefDate;
        #endregion [Variables&Objects_Declaration]

        #region [If Record is not Present then Delete]

        //checking whether Gold Loan A/C is present
        strQuery = "select count(*) from TGLSanctionDisburse_BasicDetails where SDID='" + value + "'";
        cmd = new SqlCommand(strQuery, conGL, tranGL);
        cmd.CommandTimeout = 500;
        existcount = Convert.ToInt32(cmd.ExecuteScalar());
        if (existcount > 0)
        {
            #region [DeletingFromAIM]
            //--------------Deletion of ledger entries and effects from Company-wise Account Closing table ---------------

            //Taking ReferenceNo from FSystemGeneratedEntryMaster.
            strQuery = "select ReferenceNo from FSystemGeneratedEntryMaster where LoginID='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 500;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DJEReferenceNo = Convert.ToString(cmd.ExecuteScalar());
            }
            else
            {
                DJEReferenceNo = "";
            }

            //Taking accountid from tblaccountmaster.
            strQuery = "select AccountID from tblAccountMaster where Alies='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 500;
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                DelAccID = Convert.ToInt32(cmd.ExecuteScalar());
            }

            // 1] Deleting effects from Company-wise Account Closing table
            strQuery = "select AccountID, Debit, Credit, RefDate from FLedgerMaster " +
                        "where ReferenceNo='" + DJEReferenceNo + "'";
            cmd = new SqlCommand(strQuery, connAIM, transactionAIM);
            cmd.CommandTimeout = 500;
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
                            datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transactionAIM, connAIM);

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

                //deleteQuery = "delete from FLedgerMaster where ReferenceNo='" + DJEReferenceNo + "'";
                deleteQuery = "delete from FLedgerMaster where AccountId='" + DelAccID + "' or ContraAccId='" + DelAccID + "'";
                cmd = new SqlCommand(deleteQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 500;
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


            // 3] deleting record from table TBankCash_PaymentDetails
            if (datasaved)
            {
                deleteQuery = "delete from TBankCash_PaymentDetails where ReferenceNo='" + DJEReferenceNo + "'";
                cmd = new SqlCommand(deleteQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 500;
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

            // 4] deleting record from table FSystemGeneratedEntryMaster
            if (datasaved)
            {
                deleteQuery = "delete from FSystemGeneratedEntryMaster where ReferenceNo='" + DJEReferenceNo + "'";
                cmd = new SqlCommand(deleteQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 500;
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

            // 5] deleting record from table tblAccountMaster
            if (datasaved)
            {
                deleteQuery = "delete from tblAccountMaster where Alies='" + GoldLoanNo + "'";
                cmd = new SqlCommand(deleteQuery, connAIM, transactionAIM);
                cmd.CommandTimeout = 500;
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
            #endregion [DeletingFromAIM]



            #region [Commit&RollbackData]
            if (datasaved)
            {
                tranGL.Commit();
                transactionAIM.Commit();
            }
            else
            {
                tranGL.Rollback();
                transactionAIM.Rollback();
            }
            #endregion [Commit&RollbackData]
        }

        #endregion [If Record is not Present then Delete]

        return datasaved;
    }
    #endregion [Delete_PRI]

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {

    }
    #endregion [Save Data]


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
                                    "" + ContraAccID + ", '', " + txtFYID.Text + ") ";

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

    #region [ddlBankAccount_SelectedIndexChanged]
    protected void ddlBankAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnStringAIM);
            conn.Open();

            if (ddlBankAccount.SelectedValue != "0")
            {
                txtAccGPID.Value = "";
                strQuery = "select tblAccountmaster.GPID from tblAccountmaster " +
                             "where tblAccountMaster.AccountID='" + ddlBankAccount.SelectedValue + "' ";
                cmd = new SqlCommand(strQuery, conn);
                int accGPID = Convert.ToInt32(cmd.ExecuteScalar());
                txtAccGPID.Value = Convert.ToString(accGPID);
                hdnacc.Value = accGPID.ToString();

                if (accGPID == 70)
                {
                    txtChequeNo.Text = "";
                    txtChequeDate.Text = "";
                    txtChequeDate.Enabled = false;
                    ddlcheqNEFTDD.SelectedIndex = 0;
                    txtChequeNo.Enabled = false;
                    //   ddlcheqNEFTDD.Enabled = false;
                    BindDenominationDetails();
                    PnlDeno.Enabled = true;
                    ddlCashOutBy.SelectedIndex = 0;
                    //ddlCashOutBy.Enabled = true;

                }
                else
                {
                    ddlCashOutBy.SelectedIndex = 0;
                    //ddlCashOutBy.Enabled = false;
                    txtChequeNo.Text = "";
                    txtChequeDate.Text = "";
                    ddlcheqNEFTDD.SelectedIndex = 0;
                    txtChequeNo.Enabled = true;
                    txtChequeDate.Enabled = true;
                    ddlcheqNEFTDD.Enabled = true;
                    btnImgCalender.Enabled = true;
                    BindDenominationDetails();
                    //  PnlDeno.Enabled = false;

                }
            }
            else
            {
                ddlcheqNEFTDD.Enabled = false;
                txtChequeNo.Enabled = false;
                txtChequeDate.Enabled = false;
                ddlCashOutBy.SelectedIndex = 0;
                //ddlCashOutBy.Enabled = false;
                txtAccGPID.Value = "";
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
                hdnacc.Value = "0";
                ddlcheqNEFTDD.SelectedIndex = 0;
                BindDenominationDetails();
                // PnlDeno.Enabled = false;

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CashAcc_EventAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [ddlBankAccount_SelectedIndexChanged]

    #region [Validate Loan Tenure]
    protected int ValidateLoanTenure()
    {
        int validcount = 0;
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlSchemeName.SelectedValue != "0")
            {
                //fetching Min-Max Loan Tenure, Scheme Type
                int MinTenure = 0;
                int MaxTenure = 0;
                string SchemeType = string.Empty;
                strQuery = "SELECT MinTenure, MaxTenure, SchemeType FROM tbl_GLLoanSchemeMaster " +
                            "WHERE ID='" + ddlSchemeName.SelectedValue + "'";
                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                    {
                        MinTenure = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                        MaxTenure = Convert.ToInt32(ds.Tables[0].Rows[0][1]);
                        SchemeType = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    }
                }

                if (txtLoanTenure.Text.Trim() != "")
                {
                    //validating Loan Tenure 
                    strQuery = "SELECT count(*) FROM tbl_GLLoanSchemeMaster " +
                                "WHERE ID='" + ddlSchemeName.SelectedValue + "' " +
                                "AND '" + txtLoanTenure.Text + "' BETWEEN MinTenure and MaxTenure";
                    cmd = new SqlCommand(strQuery, conn);
                    validcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (validcount == 0)
                    {
                        //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Loan Tenure must be between " + MinTenure + " - " + MaxTenure + " (Min-Max).');", true);
                        txtLoanTenure.Focus();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateLoanTenureAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        return validcount;
    }
    #endregion [Validate Loan Tenure]

    #region [SqlDataSource3_Selecting]
    protected void SqlDataSource3_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
    {
        try
        {
            if (txtLoanDate.Text.Trim() != "")
            {
                e.Command.Parameters["@RefDate"].Value = Convert.ToDateTime(txtLoanDate.Text).ToString("yyyy/MM/dd");
            }
            else
            {
                e.Command.Parameters["@RefDate"].Value = txtLoanDate.Text;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SqlDataSource3_SelectingEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [SqlDataSource3_Selecting]

    #region [Upload Gold Item Details]
    protected void BtnUpload_Click(object sender, EventArgs e)
    {
        try
        {

            DataTable dtCurrentTable = new DataTable();
            DataRow dr = null;


            dtCurrentTable.Columns.Add("GID");
            dtCurrentTable.Columns.Add("SDID");
            dtCurrentTable.Columns.Add("ItemID");
            dtCurrentTable.Columns.Add("ItemName");
            dtCurrentTable.Columns.Add("Quantity");
            dtCurrentTable.Columns.Add("GrossWeight");
            dtCurrentTable.Columns.Add("NetWeight");
            dtCurrentTable.Columns.Add("Purity");
            dtCurrentTable.Columns.Add("RateperGram");
            dtCurrentTable.Columns.Add("Value");

            string TotalQuantity = ((TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity")).Text;
            string TotalGrossWeight = ((TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight")).Text;
            string TotalNetWeight = ((TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight")).Text;
            string TotalValue = ((TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue")).Text;


            foreach (GridViewRow row in dgvGoldItemDetails.Rows)
            {

                Label lblGID = (Label)row.Cells[1].FindControl("lblGID");
                Label lblSDID = (Label)row.Cells[1].FindControl("lblSDID");
                Label lblItemID = (Label)row.Cells[1].FindControl("lblItemID");
                DropDownList ddlGoldItemName = (DropDownList)row.Cells[1].FindControl("ddlGoldItemName");
                TextBox txtQuantity = (TextBox)row.Cells[1].FindControl("txtQuantity");
                TextBox txtGrossWeight = (TextBox)row.Cells[1].FindControl("txtGrossWeight");
                TextBox txtNetWeight = (TextBox)row.Cells[1].FindControl("txtNetWeight");
                TextBox txtRatePerGram = (TextBox)row.Cells[1].FindControl("txtRatePerGram");
                TextBox txtValue = (TextBox)row.Cells[1].FindControl("txtValue");
                DropDownList ddlKarat = (DropDownList)row.Cells[1].FindControl("ddlKarat");
                HiddenField hdnkarat = (HiddenField)row.Cells[1].FindControl("hdnkarat");


                dr = dtCurrentTable.NewRow();

                dr["GID"] = lblGID.Text;
                dr["SDID"] = lblSDID.Text;
                dr["ItemID"] = ddlGoldItemName.SelectedValue;
                dr["ItemName"] = ddlGoldItemName.SelectedItem.Text;
                dr["Quantity"] = txtQuantity.Text;
                dr["GrossWeight"] = txtGrossWeight.Text;
                dr["Purity"] = ddlKarat.SelectedValue.Trim();
                dr["NetWeight"] = txtNetWeight.Text;
                dr["RateperGram"] = txtRatePerGram.Text;
                dr["Value"] = txtValue.Text;


                dtCurrentTable.Rows.Add(dr);

            }



            dr = dtCurrentTable.NewRow();
            dr["GID"] = "0";
            dr["SDID"] = "0";
            dr["ItemID"] = "0";
            dr["ItemName"] = "";
            dr["Quantity"] = "0";
            dr["GrossWeight"] = "0";
            dr["NetWeight"] = "0";
            dr["Purity"] = "";
            dr["RateperGram"] = "0";
            dr["Value"] = "0";


            dtCurrentTable.Rows.Add(dr);

            Session["dtGoldItemDetails"] = dtCurrentTable;
            dgvGoldItemDetails.DataSource = dtCurrentTable;
            dgvGoldItemDetails.DataBind();

            //------------------------------------------------------------------
            TextBox txtTotalQuantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
            TextBox txtTotalGrossWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
            TextBox txtTotalNetWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
            TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");


            txtTotalQuantity.Text = TotalQuantity;
            txtTotalGrossWeight.Text = TotalGrossWeight;
            txtTotalNetWeight.Text = TotalNetWeight;
            txtTotalValue.Text = TotalValue;

            //---------------------------------------For selecting items-------------------------------------------
            for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
            {

                dgvGoldItemDetails.SelectedIndex = i;
                Label lblItemID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblItemID");
                DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");
                HiddenField hdnkarat = (HiddenField)dgvGoldItemDetails.SelectedRow.FindControl("hdnkarat");

                ddlGoldItemName.SelectedValue = lblItemID.Text.Trim();
                ddlKarat.SelectedValue = hdnkarat.Value.Trim();


            }
            ddlSchemeName.SelectedIndex = 0;
            txtEligibleLoan.Text = "0";
            txtLoanTenure.Text = "0";
            txtNetAmountSanctioned.Text = "";
            txtNetAmountSanctioned.Enabled = false;
            txtMaxLoanAmount.Text = "0";
            txtEMI.Text = "0";
        }

        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Gold Item Details]


    #region [dgvChargesDetails_RowCommand]
    protected void dgvChargesDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {


            if (e.CommandName == "DeleteRecord")
            {

                if (dgvChargesDetails.Rows.Count == 1)
                {
                    BindChargesDetails();
                    // txtNetPayable.Text = "0";
                    //txtNetAmountSanctioned.Text = "0";
                    return;
                }
                GridView _gridView = (GridView)sender;
                int index = Convert.ToInt32(e.CommandArgument);

                DataRow dr = null;
                DataTable dt1 = new DataTable();
                dt1.Columns.Add("CHID");
                dt1.Columns.Add("CID");
                dt1.Columns.Add("ID");
                dt1.Columns.Add("Serialno");
                dt1.Columns.Add("ChargeName");
                dt1.Columns.Add("Charges");
                dt1.Columns.Add("ChargeType");
                dt1.Columns.Add("Amount");
                dt1.Columns.Add("AccountID");
                dt1.Columns.Add("AccountName");

                for (int i = 0; i < dgvChargesDetails.Rows.Count; i++)
                {
                    dgvChargesDetails.SelectedIndex = i;
                    HiddenField hdnchid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchid");
                    HiddenField hdncid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdncid");
                    HiddenField hdnchargeid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchargeid");
                    Label lblSrno = (Label)dgvChargesDetails.SelectedRow.FindControl("lblSrno");
                    Label lblCharges = (Label)dgvChargesDetails.SelectedRow.FindControl("lblCharges");
                    Label lblChargesName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblChargesName");
                    Label lblChargeType = (Label)dgvChargesDetails.SelectedRow.FindControl("lblChargeType");
                    Label lblAccountName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAccountName");
                    HiddenField hdnaccountid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnaccountid");
                    Label lblAmount = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAmount");

                    if (index != i)
                    {
                        dr = dt1.NewRow();

                        dr["CHID"] = hdnchid.Value;
                        dr["CID"] = hdncid.Value;
                        dr["ID"] = hdnchargeid.Value;
                        dr["Serialno"] = lblSrno.Text;
                        dr["ChargeName"] = lblChargesName.Text;
                        dr["Charges"] = lblCharges.Text;
                        dr["ChargeType"] = lblChargeType.Text;
                        dr["Amount"] = lblAmount.Text;
                        dr["AccountID"] = hdnaccountid.Value;
                        dr["AccountName"] = lblAccountName.Text;
                        dt1.Rows.Add(dr);
                    }
                }

                dgvChargesDetails.DataSource = dt1;
                dgvChargesDetails.DataBind();

                txtNetPayable.Text = "0";
                txtNetAmountSanctioned.Text = "0";

            }
            if (e.CommandName == "AddRecord")
            {

                DropDownList ddlChargesName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlChargesName");
                DropDownList ddlAccountName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlAccountName");

                SanctionDisbursment_PRV("Charges", "0", ddlChargesName.SelectedValue.Trim());

                conn = new SqlConnection(strConnString);
                SqlCommand cmd1 = new SqlCommand();
                cmd1.Connection = conn;
                cmd1.CommandType = CommandType.StoredProcedure;
                cmd1.CommandText = "GL_SanctionDisburse_Charges_RTR";
                cmd1.Parameters.AddWithValue("@CID", ddlChargesName.SelectedValue.Trim());
                cmd1.Parameters.AddWithValue("@SanctionAmt", txtNetAmountSanctioned.Text);
                da = new SqlDataAdapter(cmd1);
                DataTable dt2 = new DataTable();
                da.Fill(dt2);


                if (dt2.Rows.Count > 0)
                {
                    DataRow dr = null;
                    DataTable dt1 = new DataTable();
                    dt1.Columns.Add("CHID");
                    dt1.Columns.Add("CID");
                    dt1.Columns.Add("ID");
                    dt1.Columns.Add("Serialno");
                    dt1.Columns.Add("ChargeName");
                    dt1.Columns.Add("Charges");
                    dt1.Columns.Add("ChargeType");
                    dt1.Columns.Add("Amount");
                    dt1.Columns.Add("AccountID");
                    dt1.Columns.Add("AccountName");

                    for (int i = 0; i < dgvChargesDetails.Rows.Count; i++)
                    {
                        dgvChargesDetails.SelectedIndex = i;
                        HiddenField hdnchid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchid");
                        HiddenField hdncid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdncid");
                        Label lblSrno = (Label)dgvChargesDetails.SelectedRow.FindControl("lblSrno");
                        HiddenField hdnchargeid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnchargeid");
                        Label lblCharges = (Label)dgvChargesDetails.SelectedRow.FindControl("lblCharges");
                        Label lblChargesName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblChargesName");
                        Label lblChargeType = (Label)dgvChargesDetails.SelectedRow.FindControl("lblChargeType");
                        Label lblAccountName = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAccountName");
                        HiddenField hdnaccountid = (HiddenField)dgvChargesDetails.SelectedRow.FindControl("hdnaccountid");
                        Label lblAmount = (Label)dgvChargesDetails.SelectedRow.FindControl("lblAmount");

                        dr = dt1.NewRow();

                        dr["CHID"] = hdnchid.Value;
                        dr["CID"] = hdncid.Value;
                        dr["ID"] = hdnchargeid.Value;
                        dr["Serialno"] = lblSrno.Text;
                        dr["ChargeName"] = lblChargesName.Text;
                        dr["Charges"] = lblCharges.Text;
                        dr["ChargeType"] = lblChargeType.Text;
                        dr["Amount"] = lblAmount.Text;
                        dr["AccountID"] = hdnaccountid.Value;
                        dr["AccountName"] = lblAccountName.Text;


                        if (lblChargesName.Text != "")
                        {
                            dt1.Rows.Add(dr);
                        }
                    }

                    dr = dt1.NewRow();

                    dr["CHID"] = "0";
                    dr["CID"] = ddlChargesName.SelectedValue.Trim();
                    dr["ID"] = dt2.Rows[0]["ID"].ToString();
                    if (dt1.Rows.Count == 0)
                    {
                        dr["Serialno"] = "1";
                    }
                    else
                    {
                        dr["Serialno"] = dgvChargesDetails.Rows.Count + 1;
                    }
                    dr["ChargeName"] = dt2.Rows[0]["ChargeName"].ToString();
                    dr["Charges"] = dt2.Rows[0]["Charges"].ToString();
                    dr["Amount"] = dt2.Rows[0]["Amount"].ToString();
                    dr["ChargeType"] = dt2.Rows[0]["ChargeType"].ToString();
                    dr["AccountID"] = ddlAccountName.SelectedValue.Trim();
                    dr["AccountName"] = ddlAccountName.SelectedItem.Text;
                    dt1.Rows.Add(dr);

                    dgvChargesDetails.DataSource = dt1;
                    dgvChargesDetails.DataBind();

                    txtNetPayable.Text = "0";
                    txtNetAmountSanctioned.Text = "0";
                }

            }
            txtNetPayable.Text = "0";
            txtNetAmountSanctioned.Text = "0";
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvChargesDetails_RowCommand]



    #region [Fill Bank Cash Account]
    protected void FillBankCashAccount()
    {
        try
        {
            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='11' OR GPID='71') " +
                        "AND Suspended='No' ";
            connAIM = new SqlConnection(strConnStringAIM);
            SqlDataAdapter da = new SqlDataAdapter(strQuery, connAIM); //live
            // SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankAccount.DataSource = dt;
            ddlBankAccount.DataValueField = "AccountID";
            ddlBankAccount.DataTextField = "Name";
            ddlBankAccount.DataBind();
            ddlBankAccount.Items.Insert(0, new ListItem("--Select Account--", "0"));

            //For Cash Details Dropdown
            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='70') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da1 = new SqlDataAdapter(strQuery, connAIM);//live
            //SqlDataAdapter da1 = new SqlDataAdapter(strQuery, conn);
            DataTable dt1 = new DataTable();
            da1.Fill(dt1);
            ddlCashAccount.DataSource = dt1;
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

    #region [Fill Scheme Name]
    protected void FillSchemeName()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            strQuery = "select SID,SchemeName from TSchemeMaster_BasicDetails where isActive='Y' and BranchId='" + Session["branchId"].ToString() + "' order by SchemeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlSchemeName.DataSource = dt;
            ddlSchemeName.DataValueField = "SID";
            ddlSchemeName.DataTextField = "SchemeName";
            ddlSchemeName.DataBind();
            ddlSchemeName.Items.Insert(0, new ListItem("--Select Scheme Name--", "0"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillSchemeNameAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Fill Scheme Name]

    #region [Upload Proof Of Ownership]
    protected void btnUploadProofOfOwnership_Click(object sender, EventArgs e)
    {
        try
        {
            //hndImage.val
            int MaxSizeAllowed = 1073741824; // 1GB...

            if (fUploadProofOfOwnership.HasFile)
            {
                string fileName = fUploadProofOfOwnership.FileName;
                string exten = Path.GetExtension(fileName);
                //here we have to restrict file type            
                exten = exten.ToLower();
                string[] acceptedFileTypes = new string[4];
                acceptedFileTypes[0] = ".jpg";
                acceptedFileTypes[1] = ".jpeg";
                acceptedFileTypes[2] = ".gif";
                acceptedFileTypes[3] = ".png";
                bool acceptFile = false;
                for (int i = 0; i <= 3; i++)
                {
                    if (exten == acceptedFileTypes[i])
                    {
                        acceptFile = true;
                    }
                }

                if (!acceptFile)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadProofOfOwnership.PostedFile.ContentLength > MaxSizeAllowed)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }
                else
                {
                    txtProofOfOwnershipPath.Text = "OwnershipProofImage/" + fileName;
                    //upload the file onto the server                   
                    fUploadProofOfOwnership.SaveAs(Server.MapPath("~/" + txtProofOfOwnershipPath.Text));

                    System.IO.Stream fs = fUploadProofOfOwnership.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    imgProofOfOwnership.ImageUrl = "data:image/png;base64," + base64String;
                    imgProofOfOwnership.Visible = true;
                }
                //DropDownList ddlChargesName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlChargesName");
                //ddlChargesName.Focus();
            }
            else
            {
                txtProofOfOwnershipPath.Text = "";
            }
            if (hdnacc.Value != "70")
            {
                // ddlCashOutBy.Enabled = false;
            }
            else
            {
                // ddlCashOutBy.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UProofAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Proof Of Ownership]

    #region [RemoveProof_Click]
    protected void btnRemoveProof_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            fUploadProofOfOwnership = null;
            imgProofOfOwnership.ImageUrl = "";
            txtProofOfOwnershipPath.Text = "";

            DropDownList ddlChargesName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlChargesName");
            ddlChargesName.Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "RemoveProofAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [RemoveProof_Click]


    #region [PropertyImgBtnClose_Click]
    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {

        Master.PropertympeGlobal.Hide();
        Master.PropertyddlSearch.Items.Clear();
        Master.PropertytxtSearch.Text = "";
        BindGoldItemDetails();
        BindChargesDetails();
        BindDenominationDetails();
    }
    #endregion [PropertyImgBtnClose_Click]

    #region [PropertybtnSave_Click]
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            HiddenFieldGoldNo.Value = txtGoldLoanNo.Text;
            string LoginStatus = "Save";

            if (hdnoperation.Value == "Save")
            {
                SanctionDisbursment_PRV("Save", "0", "0");
                SanctionDisbursment_PRI("Save", "0");
                GetAuthLogin(HiddenFieldName.Value, HiddenFieldPwd.Value, txtCashAmount.Text.ToString(), HiddenFieldGoldNo.Value, LoginStatus);
            }
            if (hdnoperation.Value == "Update")
            {
                SanctionDisbursment_PRV("Update", hdnid.Value, "0");
                SanctionDisbursment_PRI("Update", hdnid.Value);
                GetAuthLogin(HiddenFieldName.Value, HiddenFieldPwd.Value, txtCashAmount.Text.ToString(), HiddenFieldGoldNo.Value, LoginStatus);

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion [PropertybtnSave_Click]

    #region [PropertybtnEdit_Click]
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            SanctionDisburse_RTR();

            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Loan Type");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("PAN NO");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";

            hdnpopup.Value = "Edit";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnEdit_Click]

    #region [PropertybtnDelete_Click]
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            //SanctionDisbursment_PRV("Delete", hdnid.Value.Trim(), "0");
            SanctionDisbursment_PRI("Delete", hdnid.Value.Trim());
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [PropertybtnDelete_Click]

    #region [View]
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            SanctionDisburse_RTR();
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Loan Type");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("PAN NO");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";
            hdnpopup.Value = "View";
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
        Response.Redirect("GLSanctionDisburseDetails.aspx");
    }
    #endregion [PropertybtnCancel_Click]

    #region [PropertybtnSearch_Click]
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            //Sanction_Search();
            if (hdnpopup.Value == "KYC")
            {
                KYC_Search();
            }
            if (hdnpopup.Value == "Edit")
            {
                SanctionDisburse_Search();
            }
            if (hdnpopup.Value == "View")
            {
                SanctionDisburse_Search();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnSearch_Click]

    #region [PropertygvGlobal_RowCommand]
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "Sort")
            {
            }
            else
            {

                //string id = "0", goldloanno = "";
                string index = (String)e.CommandArgument;//Convert.ToString(e.CommandArgument);           
                //int number;
                //if (int.TryParse(index, out number))
                //{
                //    Master.PropertygvGlobal.SelectedIndex = Convert.ToInt32(index);
                //    id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
                //    goldloanno = Master.PropertygvGlobal.SelectedRow.Cells[3].Text;

                //}
                //else
                //{
                //    // Master.PropertygvGlobal.SelectedIndex = 0;
                //}

                Master.PropertygvGlobal.SelectedIndex = Convert.ToInt32(index);
                string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
                string goldloanno = Master.PropertygvGlobal.SelectedRow.Cells[3].Text;


                if (hdnpopup.Value == "KYC")
                {
                    KYC_Details_RTR(id, goldloanno);
                    hdnpopup.Value = "Search";
                }
                if (hdnpopup.Value == "Edit")
                {
                    SanctionDisburseDetails_RTR(id, sender, e);
                }
                if (hdnpopup.Value == "View")
                {
                    SanctionDisburseDetails_RTR(id, sender, e);
                }

                Button BtnUpload = (Button)dgvGoldItemDetails.FooterRow.FindControl("BtnUpload");

                if (ddlLoanType.SelectedIndex == 1)
                {
                    BtnUpload.Enabled = true;

                }
                else
                {
                    BtnUpload.Enabled = false;

                }
                gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertygvGlobal_RowCommand]

    #region [dataPrintReport]
    public DataTable dataPrintReport()
    {

        DataTable dt = new DataTable("Gl_SanctionLetter_RPT");
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
    public DataSet GetRecord(DataSet ds, string sdid)
    {

        string area = "";
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select Area,Pincode From tblAreaMaster where AreaID='" + hdnareaid.Value.Trim() + "'" +
                           " select Zone From tblZonemaster where ZoneID='" + hdnzoneid.Value.Trim() + "'";
        SqlDataAdapter daaim = new SqlDataAdapter(cmd);
        DataSet dtaim = new DataSet();
        daaim.Fill(dtaim);
        if (dtaim.Tables[0].Rows.Count > 0)
        {
            area += dtaim.Tables[0].Rows[0]["Area"].ToString() + "(" + dtaim.Tables[1].Rows[0]["Zone"].ToString() + ") - " + dtaim.Tables[0].Rows[0]["Pincode"].ToString();
        }


        //-----------------------------------------
        conn = new SqlConnection(strConnString);
        DataSet dsData = new DataSet();
        foreach (DataTable dt2 in ds.Tables)
        {
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = dt2.TableName;

            cmd.Parameters.AddWithValue("@SDID", sdid.Trim() + ',' + area.Trim());

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dsData);
        }

        return dsData;

    }
    #endregion [GetRecord]

    #region [ShowReport]
    public void ShowReport(string sdid)
    {

        DataSet ds = null;
        if (ddlLoanType.SelectedIndex == 1)
        {
            ds = new DataSet("~/SanctionLetter.rpt");
        }
        if (ddlLoanType.SelectedIndex == 2)
        {
            ds = new DataSet("~/SanctionLetterTopup.rpt");
        }
        ds.Tables.Add(dataPrintReport());
        ReportDocument rpt = new ReportDocument();
        rpt.Load(Server.MapPath(ds.DataSetName));
        ds = GetRecord(ds, sdid);
        rpt.SetDataSource(ds.Tables[0]);
        if (rpt.Subreports.Count > 0)
        {
            rpt.Subreports["Subreport1"].SetDataSource(ds.Tables[1]);
            rpt.Subreports["Subreport2"].SetDataSource(ds.Tables[2]);
            rpt.Subreports[2].SetDataSource(ds.Tables[3]);
        }

        Session["REPORT"] = rpt;
        ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('ShowRpt.aspx');", true);

        //for Customer Copy
        DataSet ds1 = new DataSet("~/SanctionLetter_CustomerCopy.rpt");
        ds1.Tables.Add(dataPrintReport());
        ReportDocument rpt1 = new ReportDocument();
        rpt1.Load(Server.MapPath(ds1.DataSetName));
        ds1 = GetRecord(ds1, sdid);
        rpt1.SetDataSource(ds1.Tables[0]);
        if (rpt1.Subreports.Count > 0)
        {
            rpt1.Subreports[0].SetDataSource(ds1.Tables[1]);
            rpt1.Subreports[1].SetDataSource(ds1.Tables[2]);
            rpt1.Subreports[2].SetDataSource(ds1.Tables[2]);
        }
        hdnareaid.Value = "0";
        hdnzoneid.Value = "0";
        Session["REPORT2"] = rpt1;
        ClientScript.RegisterStartupScript(this.GetType(), "Pop Up2", "window.open('ShowRPTCustomerCopy.aspx');", true);
    }
    #endregion [ShowReport]

    #region [btnItemsUpload_Click]
    protected void btnItemsUpload_Click(object sender, EventArgs e)
    {

        try
        {
            string fileName = fUItems.FileName;
            Stream fs1 = fUItems.PostedFile.InputStream;
            BinaryReader br1 = new BinaryReader(fs1);
            Byte[] bytes1 = br1.ReadBytes((Int32)fs1.Length);

            insertQuery = "insert into Imagestore(Imagepath,Operation,Refno,CreatedBy) values(@Imagepath,@Operation,@Refno,@CreatedBy)";

            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand(insertQuery, conn);
            cmd.Parameters.AddWithValue("@Imagepath", bytes1);
            cmd.Parameters.AddWithValue("@Operation", hdnoperation.Value.Trim());
            cmd.Parameters.AddWithValue("@Refno", hdnid.Value.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());
            cmd.ExecuteNonQuery();
            conn.Close();


            int MaxSizeAllowed = 1073741824; // 1GB...

            //fsGoldItems = fUItems.PostedFile.InputStream;
            // Session["GoldItemImage"] = fUItems.PostedFile.InputStream;
            if (fUItems.HasFile)
            {


                string exten = Path.GetExtension(fileName);
                //here we have to restrict file type            
                exten = exten.ToLower();
                string[] acceptedFileTypes = new string[4];
                acceptedFileTypes[0] = ".jpg";
                acceptedFileTypes[1] = ".jpeg";
                acceptedFileTypes[2] = ".gif";
                acceptedFileTypes[3] = ".png";
                bool acceptFile = false;
                for (int i = 0; i <= 3; i++)
                {
                    if (exten == acceptedFileTypes[i])
                    {
                        acceptFile = true;
                    }
                }

                if (!acceptFile)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadProofOfOwnership.PostedFile.ContentLength > MaxSizeAllowed)
                {

                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }
                else
                {
                    hdnimgpath.Value = "OwnershipProofImage/" + fileName;
                    //upload the file onto the server                   
                    fUItems.SaveAs(Server.MapPath("~/" + hdnimgpath.Value));

                    System.IO.Stream fs = fUItems.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    //ImgItems.ImageUrl = "data:image/png;base64," + base64String;
                    ImgItems.ImageUrl = "OwnershipProofImage/" + fileName;
                    fUItems.Visible = true;

                }

            }
            else
            {
                txtProofOfOwnershipPath.Text = "";
            }

            if (hdnacc.Value != "70")
            {
                // ddlCashOutBy.Enabled = false;
            }
            else
            {
                //  ddlCashOutBy.Enabled = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UProofAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [btnItemsUpload_Click]

    #region [btnScheme_Click]
    protected void btnScheme_Click(object sender, EventArgs e)
    {
        try
        {
            //-------------------------------------------Scheme Details by bharat -----------------------------------

            TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");
            if (txtTotalValue.Text == "" || Convert.ToDouble(txtTotalValue.Text) <= 0)
            {
                ddlSchemeName.SelectedIndex = 0;
                ClientScript.RegisterStartupScript(this.GetType(), "SchemeNameEventAlert", "alert('Value should not be blank or 0');", true);
                return;
            }
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SanctionDisburse_Scheme_RTR";
            cmd.Parameters.AddWithValue("@SID", ddlSchemeName.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@TotalValue", txtTotalValue.Text);
            cmd.Parameters.AddWithValue("@SDID", hdnid.Value.Trim());
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                txtLoanTenure.Text = dt.Rows[0]["Tenure"].ToString();
                txtMaxLoanAmount.Text = dt.Rows[0]["MaxLoanAmt"].ToString();
                txtEMI.Text = dt.Rows[0]["EMI"].ToString();
                txtDueDate.Text = dt.Rows[0]["Cdate"].ToString();
                txtEligibleLoan.Text = dt.Rows[0]["EligibleLoanAmt"].ToString();
                txtNetAmountSanctioned.Text = dt.Rows[0]["EligibleLoanAmt"].ToString();
                hdnProType.Value = dt.Rows[0]["ProChargeType"].ToString();
                hdnProcharge.Value = dt.Rows[0]["ProCharge"].ToString();
                hdnproclimit.Value = dt.Rows[0]["AmtLmtTo"].ToString();
                hdnservicetax.Value = dt.Rows[0]["ServiceTax"].ToString();

                txtNetAmountSanctioned.Enabled = true;
            }
            else
            {
                txtNetAmountSanctioned.Enabled = false;


                OutStanding(hdnkycid.Value.Trim(), txtGoldLoanNo.Text.Trim());


            }
            if (ddlLoanType.SelectedIndex == 2)
            {
                OutStanding(hdnkycid.Value.Trim(), hdnoldgoldloanno.Value.Trim());
                if (txtEligibleLoan.Text != "" && Convert.ToDouble(txtEligibleLoan.Text) > 0)
                {
                    // txtEligibleLoan.Text = Convert.ToString(Convert.ToDouble(txtEligibleLoan.Text));
                    // txtNetAmountSanctioned.Text = Convert.ToString(Convert.ToDouble(txtEligibleLoan.Text) + Convert.ToDouble(lblOutPrincipal.Text));
                    txtNetPayable.Text = Convert.ToString(Math.Round(Convert.ToDouble(txtEligibleLoan.Text) - Convert.ToDouble(lblOutTotal.Text)));

                    //lblOutPrincipal.Text = "0";
                    //lblOutInterest.Text = "0";
                    //lblOutPInterest.Text = "0";
                    //lblOutCharges.Text = "0";
                    //lblOutTotal.Text = "0";
                }
                if (ddlSchemeName.SelectedIndex == 0)
                {
                    OutStanding(hdnkycid.Value.Trim(), hdnoldgoldloanno.Value.Trim());
                }
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "s1", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [btnScheme_Click]

    #region [btnDelete_Click]
    protected void btnDelete_Click(object sender, ImageClickEventArgs e)
    {
        ImageButton btnDelete = (ImageButton)sender;
        GridViewRow rowindex = (GridViewRow)btnDelete.NamingContainer;
        int index = rowindex.RowIndex;

        if (dgvGoldItemDetails.Rows.Count == 1)
        {

            BindGoldItemDetails();
            return;
        }
        //----------------------------------------------------------------------
        if (Page.IsValid)
        {


            DataTable dtCurrentTable = new DataTable();
            DataRow dr = null;


            dtCurrentTable.Columns.Add("GID");
            dtCurrentTable.Columns.Add("SDID");
            dtCurrentTable.Columns.Add("ItemID");
            dtCurrentTable.Columns.Add("ItemName");
            dtCurrentTable.Columns.Add("Quantity");
            dtCurrentTable.Columns.Add("GrossWeight");
            dtCurrentTable.Columns.Add("NetWeight");
            dtCurrentTable.Columns.Add("Purity");
            dtCurrentTable.Columns.Add("RateperGram");
            dtCurrentTable.Columns.Add("Value");



            for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
            {

                if (index != i)
                {
                    dgvGoldItemDetails.SelectedIndex = i;
                    Label lblGID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblGID");
                    Label lblSDID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblSDID");
                    Label lblItemID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblItemID");
                    DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                    TextBox txtQuantity = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtQuantity");
                    TextBox txtGrossWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtGrossWeight");
                    TextBox txtNetWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtNetWeight");
                    TextBox txtRatePerGram = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtRatePerGram");
                    TextBox txtValue = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtValue");
                    DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");


                    dr = dtCurrentTable.NewRow();

                    dr["GID"] = lblGID.Text;
                    dr["SDID"] = lblSDID.Text;
                    dr["ItemID"] = ddlGoldItemName.SelectedValue;
                    dr["ItemName"] = ddlGoldItemName.SelectedItem.Text;
                    dr["Quantity"] = txtQuantity.Text;
                    dr["GrossWeight"] = txtGrossWeight.Text;
                    dr["NetWeight"] = txtNetWeight.Text;
                    dr["Purity"] = ddlKarat.SelectedValue.Trim();
                    dr["RateperGram"] = txtRatePerGram.Text;
                    dr["Value"] = txtValue.Text;


                    dtCurrentTable.Rows.Add(dr);
                }

            }




            dgvGoldItemDetails.DataSource = dtCurrentTable;
            dgvGoldItemDetails.DataBind();

            //------------------------------------------------------------------


            int Qty = 0;
            double Gross = 0.00;
            double Net = 0.00;
            double Rate = 0;
            double Value = 0.00;

            for (int i = 0; i < dgvGoldItemDetails.Rows.Count; i++)
            {

                dgvGoldItemDetails.SelectedIndex = i;
                Label lblItemID = (Label)dgvGoldItemDetails.SelectedRow.FindControl("lblItemID");
                DropDownList ddlGoldItemName = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlGoldItemName");
                TextBox txtQuantity = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtQuantity");
                TextBox txtGrossWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtGrossWeight");
                TextBox txtNetWeight = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtNetWeight");
                TextBox txtRatePerGram = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtRatePerGram");
                TextBox txtValue = (TextBox)dgvGoldItemDetails.SelectedRow.FindControl("txtValue");
                DropDownList ddlKarat = (DropDownList)dgvGoldItemDetails.SelectedRow.FindControl("ddlKarat");
                HiddenField hdnkarat = (HiddenField)dgvGoldItemDetails.SelectedRow.FindControl("hdnkarat");


                ddlGoldItemName.SelectedValue = lblItemID.Text.Trim();
                ddlKarat.SelectedValue = hdnkarat.Value.Trim();

                if (txtQuantity.Text == "")
                {
                    txtQuantity.Text = "0";

                }
                if (txtGrossWeight.Text == "")
                {
                    txtGrossWeight.Text = "0";

                }
                if (txtNetWeight.Text == "")
                {
                    txtNetWeight.Text = "0";

                }
                if (txtRatePerGram.Text == "")
                {
                    txtRatePerGram.Text = "0";

                }
                if (txtValue.Text == "")
                {
                    txtValue.Text = "0";

                }

                Qty = Qty + Convert.ToInt32(txtQuantity.Text);
                Gross = Gross + Convert.ToDouble(txtGrossWeight.Text);
                Net = Net + Convert.ToDouble(txtNetWeight.Text);
                Rate = Convert.ToDouble(txtRatePerGram.Text);
                Value = Value + Convert.ToDouble(txtValue.Text);



            }


            TextBox txtTotalQuantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
            TextBox txtTotalGrossWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
            TextBox txtTotalNetWeight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
            TextBox txtTotalValue = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");


            //Calculate total after row deleting
            txtTotalQuantity.Text = Qty.ToString();
            txtTotalGrossWeight.Text = Gross.ToString();
            txtTotalNetWeight.Text = Net.ToString();

            txtTotalValue.Text = Value.ToString();


            //Clear scheme details
            ddlSchemeName.SelectedIndex = 0;
            txtLoanTenure.Text = "";
            txtMaxLoanAmount.Text = "";
            txtEligibleLoan.Text = "";
            txtNetAmountSanctioned.Text = "";
            txtEMI.Text = "";
        }
        ddlSchemeName.SelectedIndex = 0;
        txtEligibleLoan.Text = "0";
        txtLoanTenure.Text = "0";
        txtNetAmountSanctioned.Text = "";
        txtNetAmountSanctioned.Enabled = false;
        txtMaxLoanAmount.Text = "0";
        txtEMI.Text = "0";

    }
    #endregion [btnDelete_Click]

    #region [btnDenoAdd_Click]
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
    #endregion [btnDenoAdd_Click]

    #region [btnDenoDelete_Click]
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

            //for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
            //{
            //    gvDenominationDetails.SelectedIndex = i;
            //    HiddenField InOutID = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdncashinoutid");
            //    dr["InOutID"] = InOutID.Value;
            //    dr["RefNo"] = InOutID.Value;

            //}

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

                    dr["InOutID"] = hdncashinoutid.Value;
                    dr["RefNo"] = hdnrefno.Value;
                    dr["DenoId"] = hdndenoid.Value;
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
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [btnDenoDelete_Click]

    #region [imgbtnCustomer_Click]
    protected void imgbtnCustomer_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            KYC_RTR();
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Applied Date");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("PAN No");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";

            hdnpopup.Value = "KYC";



        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Kyc", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [imgbtnCustomer_Click]

    #region [gvDenominationDetails_RowDataBound]
    protected void gvDenominationDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtsrno = (TextBox)e.Row.FindControl("gvtxtDenoSrno");
            txtsrno.Attributes.Add("readonly", "readonly");
        }
    }
    #endregion [gvDenominationDetails_RowDataBound]

    #region [ddlLoanType_SelectedIndexChanged]
    protected void ddlLoanType_SelectedIndexChanged(object sender, EventArgs e)
    {
        ClearLoanData();
    }
    #endregion [ddlLoanType_SelectedIndexChanged]

    #region [dgvGoldItemDetails_RowDataBound]
    protected void dgvGoldItemDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {

            DropDownList ddlGoldItemName = (DropDownList)e.Row.FindControl("ddlGoldItemName");
            TextBox txtQuantity = (TextBox)e.Row.FindControl("txtQuantity");
            TextBox txtGrossWeight = (TextBox)e.Row.FindControl("txtGrossWeight");
            TextBox txtNetWeight = (TextBox)e.Row.FindControl("txtNetWeight");
            TextBox txtRatePerGram = (TextBox)e.Row.FindControl("txtRatePerGram");
            TextBox txtValue = (TextBox)e.Row.FindControl("txtValue");
            ImageButton btnDelete = (ImageButton)e.Row.FindControl("btnDelete");
            DropDownList ddlKarat = (DropDownList)e.Row.FindControl("ddlKarat");


            if (ddlLoanType.SelectedIndex == 0)
            {
                // ddlCashAccount.Enabled = false;
                dgvGoldItemDetails.Enabled = false;
                txtNetAmountSanctioned.Enabled = false;
                //  ddlCashOutBy.Enabled = false;
                ddlGoldInwardBy.Enabled = false;
                txtRackno.Enabled = false;
                dgvChargesDetails.Enabled = false;

            }
            if (ddlLoanType.SelectedIndex == 1)
            {
                ddlGoldItemName.Enabled = true;
                ddlKarat.Enabled = true;
                txtQuantity.Enabled = true;
                txtGrossWeight.Enabled = true;
                txtNetWeight.Enabled = true;
                txtValue.Enabled = true;
                btnDelete.Enabled = true;


                //ddlCashAccount.Enabled = true;
                dgvGoldItemDetails.Enabled = true;
                txtNetAmountSanctioned.Enabled = true;
                //ddlCashOutBy.Enabled = true;
                ddlGoldInwardBy.Enabled = true;
                txtRackno.Enabled = true;
                dgvChargesDetails.Enabled = true;

            }

            if (ddlLoanType.SelectedIndex == 2)
            {
                ddlGoldItemName.Enabled = false;
                ddlKarat.Enabled = false;
                txtQuantity.Enabled = false;
                txtGrossWeight.Enabled = false;
                txtNetWeight.Enabled = false;
                txtValue.Enabled = false;
                btnDelete.Enabled = false;


                //  ddlCashAccount.Enabled = true;
                dgvGoldItemDetails.Enabled = true;
                txtNetAmountSanctioned.Enabled = true;
                //ddlCashOutBy.Enabled = true;
                ddlGoldInwardBy.Enabled = true;
                txtRackno.Enabled = true;
                dgvChargesDetails.Enabled = true;
            }

        }




    }
    #endregion [dgvGoldItemDetails_RowDataBound]




    protected void dgvChargesDetails_DataBound(object sender, EventArgs e)
    {
        try
        {
            double total = 0;
            foreach (GridViewRow row in dgvChargesDetails.Rows)
            {
                var numberLabel = row.FindControl("lblAmount") as Label;
                double number;
                if (double.TryParse(numberLabel.Text, out number))
                {
                    total += number;
                }
            }
            total = Math.Round(total, 2);
            txtTotalChargesAmount.Text = Convert.ToString(total);
            //txtNetPayable.Text = "0";
            //txtNetAmountSanctioned.Text = "0";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void dgvChargesDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }


    // // added by priya
    //protected void ddlPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    try
    //    {

    //        if (ddlPaymentMode.SelectedIndex == 0)
    //        {

    //            pnlBankAc.Enabled = false;
    //            pnlCashAc.Enabled = false;

    //            return;
    //        }
    //        else if (ddlPaymentMode.SelectedIndex == 1)
    //        {
    //            ddlBankAccount.Enabled = false;
    //            pnlBankAc.Enabled = false;

    //            pnlCashAc.Enabled = true;

    //        }
    //        else if (ddlPaymentMode.SelectedIndex == 2)
    //        {
    //            pnlBankAc.Enabled = true;
    //            pnlCashAc.Enabled = false;

    //        }
    //        else if (ddlPaymentMode.SelectedIndex == 3)
    //        {
    //            pnlBankAc.Enabled = false;
    //            pnlCashAc.Enabled = false;
    //        }


    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
    //    }
    //}


    protected void ddlPaymentMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {

            if (ddlPaymentMode.SelectedIndex == 0)
            {

                pnlBankAc.Enabled = false;
                pnlCashAc.Enabled = false;
                txtCashAmount.Text = ""; txtBankAmount.Text = "";
                ddlBankAccount.SelectedIndex = 0; ddlcheqNEFTDD.SelectedIndex = 0; txtChequeDate.Text = ""; txtChequeNo.Text = "";
                return;
            }
            if (ddlPaymentMode.SelectedIndex == 1)
            {

                pnlBankAc.Enabled = true;
                txtCashAmount.Text = ""; ddlCashAccount.SelectedIndex = 0;
                ddlCashOutBy.SelectedIndex = 0;
                //BindDenominationDetails();
            }
            else
            {
                pnlBankAc.Enabled = false;
                txtCashAmount.Text = "";
                ddlCashAccount.SelectedIndex = 0;
                ddlCashOutBy.SelectedIndex = 0;
            }

            if (ddlPaymentMode.SelectedIndex == 2)
            {
                pnlCashAc.Enabled = true;
                PnlDeno.Enabled = true;
                txtBankAmount.Text = "";
                ddlBankAccount.SelectedIndex = 0;
                ddlcheqNEFTDD.SelectedIndex = 0; txtChequeNo.Text = "";
            }
            else
            {
                pnlCashAc.Enabled = false;
                PnlDeno.Enabled = false;
                txtBankAmount.Text = "";
                ddlBankAccount.SelectedIndex = 0;
                ddlcheqNEFTDD.SelectedIndex = 0;

            }

            if (ddlPaymentMode.SelectedIndex == 3)
            {
                pnlBankAc.Enabled = true;
                pnlCashAc.Enabled = true;
                PnlDeno.Enabled = true;
                //  txtCashAmount.Text = ""; txtBankAmount.Text = "";
                //  ddlBankAccount.SelectedIndex = 0; ddlcheqNEFTDD.SelectedIndex = 0; txtChequeDate.Text = "";
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }


    protected void txtCashAmount_TextChanged(object sender, EventArgs e)
    {
        try
        {

            SqlConnection conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,RECPT.BalanceLoanPayable,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID INNER JOIN TGlReceipt_BasicDetails RECPT ON SD.SDID=RECPT.SDID WHERE KYC.CustomerID=" + txtCustomerID.Text;

            SqlDataAdapter daLoan = new SqlDataAdapter(cmd);
            DataSet dtLoan = new DataSet();
            daLoan.Fill(dtLoan);

            decimal cashAmt = 0, cashCurerntAmt = 0, FinalLoanCash = 0;
            cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);

            if (dtLoan.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j < dtLoan.Tables[0].Rows.Count; j++)
                {
                    if (Convert.ToDecimal(dtLoan.Tables[0].Rows[j]["BalanceLoanPayable"].ToString()) > 0)
                    {
                        if (dtLoan.Tables[0].Rows[j]["CashAmount"].ToString() != "")
                        {
                            cashAmt = Convert.ToDecimal(dtLoan.Tables[0].Rows[j]["CashAmount"].ToString());
                        }
                        //cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);

                        FinalLoanCash = cashAmt + cashCurerntAmt;

                        if (FinalLoanCash > 99999)
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit.Do You Want to go to Authorized Login??', 'Login');", true);
                        }
                    }
                }
            }
            else //if (dtLoan.Tables[0].Rows.Count == 0)
            {
                conn = new SqlConnection(strConnString);
                conn.Open();
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID WHERE KYC.CustomerID=" + txtCustomerID.Text;

                SqlDataAdapter daLoanSD = new SqlDataAdapter(cmd);
                DataSet dtLoanSD = new DataSet();
                daLoanSD.Fill(dtLoanSD);
                decimal totalCash = 0;

                if (dtLoanSD.Tables[0].Rows.Count > 0)
                {
                    for (int k = 0; k < dtLoanSD.Tables[0].Rows.Count; k++)
                    {
                        if (dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString() != "")
                        {
                            cashAmt = Convert.ToDecimal(dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString());
                        }
                        //cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);
                        totalCash = totalCash + cashAmt;
                    }

                    FinalLoanCash = totalCash + cashCurerntAmt;

                    if (FinalLoanCash > 99999)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
                    }
                }

                else
                {
                    if (cashCurerntAmt > 99999)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
                    }
                    else
                    {
                    }
                }


            }
        }
        catch (Exception)
        {

        }
        finally
        {
            // conn.Close();
        }


        // ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit Do You Want to go to Authorized Login??', 'Login');", true);

    }


    protected void btnCashPopup_Click(object sender, EventArgs e)
    {
        //try
        //{

        //    SqlConnection conn = new SqlConnection(strConnString);
        //    conn.Open();
        //    cmd = new SqlCommand();
        //    cmd.Connection = conn;
        //    string customerId = "";
        //    if (txtCustomerID.Text == "")
        //    {
        //        customerId = "0";
        //    }
        //    else
        //    {
        //        customerId = txtCustomerID.Text;
        //    }

        //    cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,RECPT.BalanceLoanPayable,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID INNER JOIN TGlReceipt_BasicDetails RECPT ON SD.SDID=RECPT.SDID WHERE sd.isActive='Y' AND KYC.CustomerID=" + customerId;

        //    SqlDataAdapter daLoan = new SqlDataAdapter(cmd);
        //    DataSet dtLoan = new DataSet();
        //    daLoan.Fill(dtLoan);

        //    decimal cashAmt = 0, cashCurerntAmt = 0, FinalLoanCash = 0, totalCashRc = 0;
        //    cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);

        //    if (dtLoan.Tables[0].Rows.Count > 0)
        //    {
        //        for (int j = 0; j < dtLoan.Tables[0].Rows.Count; j++)
        //        {
        //            if (Convert.ToDecimal(dtLoan.Tables[0].Rows[j]["BalanceLoanPayable"].ToString()) > 0)
        //            {
        //                if (dtLoan.Tables[0].Rows[j]["CashAmount"].ToString() != "")
        //                {
        //                    //cashAmt = Convert.ToDecimal(dtLoan.Tables[0].Rows[j]["CashAmount"].ToString());
        //                    totalCashRc = totalCashRc + Convert.ToDecimal(dtLoan.Tables[0].Rows[j]["CashAmount"].ToString());
        //                }
        //                //cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);
        //            }
        //            FinalLoanCash = totalCashRc + cashCurerntAmt;
        //        }
        //        if (FinalLoanCash > 99999)
        //        {
        //            ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit.Do You Want to go to Authorized Login??', 'Login');", true);
        //        }

        //    }
        //    else //if (dtLoan.Tables[0].Rows.Count == 0)
        //    {
        //        conn = new SqlConnection(strConnString);
        //        conn.Open();
        //        cmd = new SqlCommand();
        //        cmd.Connection = conn;
        //        //  cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID WHERE sd.isActive='Y' AND KYC.CustomerID=" + txtCustomerID.Text;
        //        cmd.CommandText = "select KYC.KYCID,sd.NetLoanAmtSanctioned,sd.CashAmount,sd.BankAmount from  TGLKYC_BasicDetails KYC INNER join TGLSanctionDisburse_BasicDetails sd ON SD.KYCID=KYC.KYCID WHERE sd.isActive='Y' AND KYC.CustomerID=" + customerId;
        //        SqlDataAdapter daLoanSD = new SqlDataAdapter(cmd);
        //        DataSet dtLoanSD = new DataSet();
        //        daLoanSD.Fill(dtLoanSD);
        //        decimal totalCash = 0;

        //        if (dtLoanSD.Tables[0].Rows.Count > 0)
        //        {
        //            for (int k = 0; k < dtLoanSD.Tables[0].Rows.Count; k++)
        //            {
        //                if (dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString() != "")
        //                {
        //                    //cashAmt = Convert.ToDecimal(dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString());
        //                    totalCash = totalCash + Convert.ToDecimal(dtLoanSD.Tables[0].Rows[k]["CashAmount"].ToString());
        //                }
        //                //cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);
        //                //totalCash = totalCash + cashAmt;
        //                //FinalLoanCash = cashAmt + cashCurerntAmt;
        //            }

        //            FinalLoanCash = totalCash + cashCurerntAmt;

        //            if (FinalLoanCash > 99999)
        //            {
        //                ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
        //            }
        //        }

        //        else
        //        {
        //            if (cashCurerntAmt > 99999)
        //            {
        //                ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
        //            }
        //            else
        //            {
        //            }
        //        }


        //    }
        //}
        //catch (Exception)
        //{

        //}
        //finally
        //{
        //    // conn.Close();
        //}


        decimal cashAmt = 0, cashCurerntAmt = 0, FinalLoanCash = 0, totalCashRc = 0;

        cashCurerntAmt = Convert.ToDecimal(txtCashAmount.Text);
        FinalLoanCash = Convert.ToDecimal(HiddenFieldTotalCash.Value) + cashCurerntAmt;

        try
        {
            if (FinalLoanCash > 0)
            {
                if (FinalLoanCash > 99999)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
                }
            }
            else
            {
                if (cashCurerntAmt > 99999)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ShowPopup", "ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');", true);
                }
                else
                {
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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static void GetAuthLogin(string name, string Password, string Amount, string GoldLoanNo, string LoginStatus)
    {
        string strConnStringLogin = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
        SqlConnection SqlconnLogin = new SqlConnection(strConnStringLogin);
        SqlCommand cmdUser, cmdLogin;
        SqlDataReader dr;
        string loginQry, insertQueryLogin;

        string LoggedUser = System.Web.HttpContext.Current.Session["userID"].ToString();
        string time = DateTime.Now.ToString("HH:mm:ss");
        string strToday = DateTime.Today.ToString("MM/dd/yyyy") + " " + time;

        int totalLogin = 0;
        try
        {
            SqlconnLogin.Open();

            loginQry = "select * from UserDetails where UserName='" + name + "' And Password='" + Password + "'";
            cmdUser = new SqlCommand(loginQry, SqlconnLogin);
            dr = cmdUser.ExecuteReader();
            Page page = HttpContext.Current.CurrentHandler as Page;

            if (dr.HasRows)
            {

                // page.ClientScript.RegisterStartupScript(page.GetType(), "LoginAlert", "alert('Login Successfully');", true);

                totalLogin = 1;
                dr.Close();

                if (LoginStatus == "Initial")
                {
                    insertQueryLogin = "insert into TGLExcessAmount_Login values('" + GoldLoanNo + "',  '" + LoggedUser + "', '" + 11 + "', '" + strToday + "', " +
                                   "'" + 22 + "', '" + Amount + "','Temp')";
                    // SqlconnLogin.Open();
                    cmdLogin = new SqlCommand(insertQueryLogin, SqlconnLogin);
                    cmdLogin.ExecuteNonQuery();
                }
                else
                {
                    insertQueryLogin = "insert into TGLExcessAmount_Login values('" + GoldLoanNo + "' ,'" + LoggedUser + "', '" + 11 + "', '" + strToday + "', " +
                                  "'" + 22 + "', '" + Amount + "','Final')";
                    //  SqlconnLogin.Open();
                    cmdLogin = new SqlCommand(insertQueryLogin, SqlconnLogin);
                    cmdLogin.ExecuteNonQuery();
                }
            }
            else
            {

            }
        }
        catch (Exception ex)
        {

        }
        finally
        {

            SqlconnLogin.Close();
        }

    }


    public SortDirection dir
    {
        get
        {
            if (ViewState["dirState"] == null)
            {
                ViewState["dirState"] = SortDirection.Ascending;
            }
            return (SortDirection)ViewState["dirState"];
        }

        set
        {
            ViewState["dirState"] = value;
        }

    }



    #region [PropertygvGlobal_Sorting]
    protected void PropertygvGlobal_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            //if (hdnpopup.Value == "KYC")
            //{
            //  KYC_Search();
            //}
            //if (hdnpopup.Value == "Edit")
            //{
            //    SanctionDisburse_Search();
            //}
            //if (hdnpopup.Value == "View")
            //{
            //    SanctionDisburse_Search();
            //}



            DataTable dtResult = (DataTable)ViewState["dt"];
            if (dtResult.Rows.Count > 0)
            {
                if (Convert.ToString(ViewState["sort"]) == "Asc")
                {
                    dtResult.DefaultView.Sort = e.SortExpression + " Desc";
                    ViewState["sort"] = "Desc";
                }
                else
                {
                    dtResult.DefaultView.Sort = e.SortExpression + " Asc";
                    ViewState["sort"] = "Asc";
                }


                Master.PropertygvGlobal.DataSource = dtResult;
                Master.DataBind();
                Master.PropertympeGlobal.Show();
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion







    //#region PDC
    //protected void btnApplPDC_Click1(object sender, EventArgs e)
    //{
    //    conn = new SqlConnection(strConnString);
    //    conn.Open();
    //    string strquery = "SELECT BankID, BankName + ' (Branch: ' + Branch + ')' as 'BankName'   FROM tblBankMaster ORDER BY BankName";
    //    cmd = new SqlCommand(strquery, conn);
    //    da = new SqlDataAdapter(cmd);
    //    da.Fill(dt);
    //    ddlbankName.DataSource = dt;
    //    ddlbankName.DataValueField = "BankID";
    //    ddlbankName.DataTextField = "BankName";
    //    ddlbankName.DataBind();
    //    ddlbankName.Items.Insert(0, "--Select Bank Name--");



    //    strquery = "SELECT AreaID, Area + ' (PinCode: ' + Pincode + ')' as 'Area' FROM tblAreaMaster ORDER BY Area";
    //    cmd = new SqlCommand(strquery, conn);
    //    da = new SqlDataAdapter(cmd);
    //    da.Fill(dt1);
    //    ddlArea.DataSource = dt1;
    //    ddlArea.DataValueField = "AreaID";
    //    ddlArea.DataTextField = "Area";
    //    ddlArea.DataBind();
    //    ddlArea.Items.Insert(0, "--Select Area--");

    //    //pnlpdc.Visible = true;
    //    mpChqDetails.Show();
    //    hdnpopup.Value = "AppPDC";
    //    conn.Close();
    //}

    //protected void gdvLoanAdjustment_RowCommand(object sender, GridViewCommandEventArgs e)
    //{
    //    if (e.CommandName == "AddRecord")
    //    {
    //        AddLoanAdjstCases();
    //    }

    //    if (e.CommandName == "DeleteRecord")
    //    {

    //        if (gdvLoanAdjustment.Rows.Count == 1)
    //        {
    //            gdvLoanAdjustment.SelectedIndex = 0;
    //            //HiddenField hdnprodctAdjst = (HiddenField)gdvAdjustwith.SelectedRow.FindControl("hdnprodctAdjst");
    //            TextBox txtAccountName = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtAccountName");
    //            HiddenField hdnaccID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnaccID");
    //            DropDownList ddlDrCr = (DropDownList)gdvLoanAdjustment.SelectedRow.FindControl("ddlDrCr");
    //            HiddenField hdnDrDr = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnDrDr");
    //            TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //            TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //            HiddenField hdnLoanAdjID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnLoanAdjID");
    //            hdnDrDr.Value = ddlDrCr.SelectedValue;
    //            if (txtAccountName.Text == "")
    //            {
    //                BindLoanAdjustment();
    //                return;
    //            }
    //            else if (ddlDrCr.SelectedIndex == 0)
    //            {

    //                BindLoanAdjustment();
    //                return;
    //            }
    //            else if (ddlDrCr.SelectedIndex == 1)
    //            {
    //                if (txtDebit.Text == "")
    //                {
    //                    BindLoanAdjustment();
    //                    return;
    //                }
    //            }
    //            else
    //            {
    //                BindLoanAdjustment();
    //                return;
    //            }

    //            BindLoanAdjustment();
    //            CalculateTotDeduction();
    //            calculateBalancePayable();
    //            return;
    //        }

    //        GridView _gridView = (GridView)sender;
    //        int index = Convert.ToInt32(e.CommandArgument);

    //        DataRow dr = null;
    //        dt.Columns.Add("AccName");
    //        dt.Columns.Add("AccID");
    //        dt.Columns.Add("DrCr");
    //        dt.Columns.Add("Debit");
    //        dt.Columns.Add("Credit");
    //        dt.Columns.Add("LoanAdjID");
    //        for (int i = 0; i < gdvLoanAdjustment.Rows.Count; i++)
    //        {
    //            gdvLoanAdjustment.SelectedIndex = i;
    //            //HiddenField hdnprodctAdjst = (HiddenField)gdvAdjustwith.SelectedRow.FindControl("hdnprodctAdjst");
    //            TextBox txtAccountName = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtAccountName");
    //            HiddenField hdnaccID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnaccID");
    //            DropDownList ddlDrCr = (DropDownList)gdvLoanAdjustment.SelectedRow.FindControl("ddlDrCr");
    //            TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //            TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //            HiddenField hdnLoanAdjID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnLoanAdjID");
    //            if (index != i)
    //            {
    //                dr = dt.NewRow();
    //                dr["AccName"] = txtAccountName.Text.ToString();
    //                dr["AccID"] = hdnaccID.Value.ToString();
    //                dr["DrCr"] = ddlDrCr.SelectedValue;
    //                dr["Debit"] = txtDebit.Text.ToString();
    //                dr["Credit"] = txtcredit.Text.ToString();
    //                dr["LoanAdjID"] = hdnLoanAdjID.Value;
    //                dt.Rows.Add(dr);
    //            }

    //        }
    //        gdvLoanAdjustment.DataSource = dt;
    //        gdvLoanAdjustment.DataBind();
    //        for (int j = 0; j < gdvLoanAdjustment.Rows.Count; j++)
    //        {
    //            gdvLoanAdjustment.SelectedIndex = j;
    //            DropDownList ddlDrCr = (DropDownList)gdvLoanAdjustment.SelectedRow.FindControl("ddlDrCr");
    //            HiddenField hdnDrDr = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnDrDr");
    //            TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //            TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //            ddlDrCr.SelectedValue = hdnDrDr.Value;
    //            if (ddlDrCr.SelectedValue == "DR")
    //            {
    //                txtcredit.Enabled = false;
    //                txtDebit.Enabled = true;
    //            }
    //            else
    //            {
    //                txtcredit.Enabled = true;
    //                txtDebit.Enabled = false;
    //            }

    //        }

    //        CalculateTotDeduction();
    //        calculateBalancePayable();
    //    }

    //}


    //protected void imgSearchAccount_Click(object sender, ImageClickEventArgs e)
    //{

    //    conn = new SqlConnection(strConnString);
    //    cmd = new SqlCommand();
    //    cmd.Connection = conn;
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.CommandText = "sp_Disburse_Account_RTR";
    //    da = new SqlDataAdapter(cmd);
    //    dt = new DataTable();
    //    da.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        gvLocal.DataSource = dt;
    //        gvLocal.ShowHeader = true;
    //        gvLocal.DataBind();
    //        mpLocal1.Show();
    //    }
    //    //Master.PropertygvGlobal.DataSource = dt;
    //    //Master.DataBind();
    //    //Master.PropertympeGlobal.Show();
    //    hdnpopup.Value = "LoanAdjst";
    //    //  Master.PropertygvGlobal.Rows[0].Cells[0].Visible = true;
    //    //txtSearhText.Text = "";

    //    //ddlSearch.Items.Clear();
    //    //ddlSearch.Items.Add("Account ID");
    //    //ddlSearch.Items.Add("Name");

    //}



    //protected void btnloanAdj_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        CalculateTotDeduction();
    //        calculateBalancePayable();
    //    }
    //    catch (Exception ex)
    //    {
    //        // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);

    //        //Added by Priya
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);


    //    }
    //}


    //protected void btncalcDtDiff_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        //ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('CalcPDCDate');", true);

    //        BindIIRChart();
    //        BindAppPDCDetails();
    //        BindCoappDetails();
    //        BindGarDetails();
    //        txtIRR.Text = "";
    //        calculateFirsrPDCDate();
    //    }
    //    catch (Exception ex)
    //    {
    //        //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);

    //        //Added by Priya
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);

    //    }
    //}

    //protected void btnClearIRR_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        BindIIRChart();
    //        BindAppPDCDetails();
    //        BindCoappPDCDetails();
    //        BindGarPDCDetails();
    //        txtIRR.Text = "";
    //    }
    //    catch (Exception ex)
    //    {
    //        //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);
    //        //Added by Priya
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);

    //    }
    //}


    //protected void btnFirstPDCtillChange_Click(object sender, EventArgs e)
    //{
    //    int intEdtTenure = 0;
    //    int ATotalPDCCount = 0;
    //    int AUCPDCCount = 0;

    //    if (!string.IsNullOrEmpty(hdnEdtPDCTenure.Value))
    //    {
    //        intEdtTenure = Convert.ToInt32(hdnEdtPDCTenure.Value);
    //    }
    //    else
    //    {
    //        intEdtTenure = 0;
    //    }

    //    if (string.IsNullOrEmpty(txtFrstPdctill.Text))
    //    {
    //        txtFrstPdctill.Text = "0";
    //    }

    //    if (Convert.ToInt32(txtFrstPdctill.Text) != intEdtTenure)
    //    {
    //        string strquery = string.Empty;
    //        conn = new SqlConnection(strConnString);
    //        conn.Open();

    //        strquery = "SELECT COUNT(*) FROM TDisbursement_Appl_PDCDetails " +
    //                    "WHERE TDisbursement_Appl_PDCDetails.LoginID='" + txtLoginID.Text + "' ";
    //        cmd = new SqlCommand(strquery, conn);
    //        cmd.CommandType = CommandType.Text;
    //        ATotalPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //        strquery = "SELECT COUNT(*) FROM TDisbursement_Appl_PDCDetails " +
    //                    "WHERE TDisbursement_Appl_PDCDetails.LoginID='" + txtLoginID.Text + "' " +
    //                        "AND ProcessStatus='UP' AND Clearstatus='UC'";
    //        cmd = new SqlCommand(strquery, conn);
    //        cmd.CommandType = CommandType.Text;
    //        AUCPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //        if (ATotalPDCCount > 0)
    //        {
    //            if (AUCPDCCount > 0)
    //            {
    //                if (AUCPDCCount == (Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value)))
    //                {
    //                    BindAppPDCDetails();
    //                    BindCoappPDCDetails();
    //                    BindGarPDCDetails();
    //                    BindIIRChart();
    //                    txtIRR.Text = "";
    //                }
    //            }
    //        }
    //        else
    //        {
    //            BindAppPDCDetails();
    //            BindCoappPDCDetails();
    //            BindGarPDCDetails();
    //            BindIIRChart();
    //            txtIRR.Text = "";
    //        }
    //    }
    //}

    //protected void btnCreateIIRchartByOldDLL_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        if (hdnchkBxCancel.Value == "false")
    //        {
    //            CaseCancelCheckedChange();
    //        }

    //        var enCulture = new System.Globalization.CultureInfo("en-us");
    //        //for (int i = 0; i < gdvBankDetails.Rows.Count; i++)
    //        //{
    //        gdvBankDetails.SelectedIndex = 0;
    //        TextBox txtpayeeName = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtpayeeName");
    //        TextBox txtCashcheqDt = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtCashcheqDt");
    //        string chqdt, AgreeDate, PDCDate = "";
    //        DateTime dtreq = DateTime.Now;
    //        DateTime dtfirstpdc = DateTime.Now;
    //        DateTime dtchngPDC = DateTime.Now;

    //        if (txtpayeeName.Text != "")
    //        {
    //            chqdt = txtCashcheqDt.Text;

    //            AgreeDate = hdntxtAgreemntDate.Value;
    //            //dtreq = DateTime.Parse(txtDateRq.Text);
    //            //dtfirstpdc = DateTime.Parse(txtFrstPDCdt.Text);
    //            //dtchngPDC = DateTime.Parse(txtpdcChngdt.Text);       

    //            DateTime dt, dtTillTenure;
    //            dt = DateTime.Now;
    //            dtTillTenure = DateTime.Now;

    //            int CompairMonth = 0, DaysBetAgrFPdcDt = 0;

    //            if (chqdt == "")
    //            {
    //                //    ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Cheque Date in Bank Details, it should not be blank!!!');", true);

    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Cheque Date in Bank Details, it should not be blank!!!');", true);

    //                return;
    //            }

    //            if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //            {
    //                if (txtDateRq.Text.Trim() == string.Empty)
    //                {
    //                    //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter First PDC Date Required.');", true);
    //                    //Added by Priya
    //                    ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter First PDC Date.');", true);

    //                    return;
    //                }

    //                //dtreq = DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture);
    //                dtreq = Convert.ToDateTime(txtDateRq.Text);
    //                PDCDate = txtDateRq.Text;
    //                dt = (dtreq.AddMonths(Convert.ToInt32(txtFrstPdctill.Text)));
    //                dtTillTenure = dtreq.AddMonths(Convert.ToInt32(txtFrstPdctill.Text) - 1);
    //                CompairMonth = Convert.ToInt32(txtFrstPdctill.Text) + dtreq.Month;

    //                //DaysBetAgrFPdcDt = (dtreq - DateTime.ParseExact(chqdt, "dd/MM/yyyy", enCulture)).Days;
    //                DaysBetAgrFPdcDt = (dtreq - Convert.ToDateTime(chqdt)).Days;
    //                string a = txtFrstPDCdt.Text;
    //            }
    //            else if (ddlBrknPrdIntrst.SelectedValue == "No")
    //            {
    //                if (txtFrstPDCdt.Text.Trim() == string.Empty)
    //                {
    //                    // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter First PDC Date.');", true);
    //                    //Added by Priya
    //                    ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter First PDC Date.');", true);

    //                    return;
    //                }

    //                dtfirstpdc = DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture);
    //                PDCDate = txtFrstPDCdt.Text;
    //                dt = dtfirstpdc.AddMonths(Convert.ToInt32(txtFrstPdctill.Text));
    //                dtTillTenure = dtfirstpdc.AddMonths(Convert.ToInt32(txtFrstPdctill.Text) - 1);
    //                CompairMonth = Convert.ToInt32(txtFrstPdctill.Text) + dtfirstpdc.Month;
    //                DaysBetAgrFPdcDt = (dtfirstpdc - DateTime.ParseExact(chqdt, "dd/MM/yyyy", enCulture)).Days;
    //            }

    //            if (txtpdcChngdt.Text.Trim() == string.Empty)
    //            {
    //                if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //                {
    //                    txtpdcChngdt.Text = Convert.ToString(dtreq.AddMonths(Convert.ToInt32(txtFrstPdctill.Text) - 1));
    //                }
    //                else if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //                {
    //                    txtpdcChngdt.Text = Convert.ToString(dtfirstpdc.AddMonths(Convert.ToInt32(txtFrstPdctill.Text) - 1));
    //                }
    //            }

    //            //dtchngPDC = DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture);
    //            dtchngPDC = Convert.ToDateTime(txtpdcChngdt.Text);

    //            int TotalTenure = Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value);

    //            int intPDCDiffDays = Convert.ToInt32((dtTillTenure).Day - (dtchngPDC).Day);
    //            int intExactMonths = Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value);

    //            //Validation 

    //            if (chqdt != AgreeDate)
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details and Agreement Date should be same.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details and Agreement Date should be same.');", true);

    //                return;
    //            }

    //            else if (ddlBrknPrdIntrst.SelectedValue == "--Select--")
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Broken Period Interest - Yes/No.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Broken Period Interest - Yes/No.');", true);

    //                return;
    //            }

    //            //else if ((DateTime.ParseExact(chqdt, "dd/MM/yyyy", enCulture) >= DateTime.ParseExact(PDCDate, "dd/MM/yyyy", enCulture)) && (ddlBrknPrdIntrst.SelectedValue == "No"))
    //            else if ((Convert.ToDateTime(chqdt) >= Convert.ToDateTime(PDCDate)) && (ddlBrknPrdIntrst.SelectedValue == "No"))
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details should be less than First PDC Date.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details should be less than First PDC Date.');", true);

    //                return;
    //            }

    //            //else if ((DateTime.ParseExact(chqdt, "dd/MM/yyyy", enCulture) >= DateTime.ParseExact(PDCDate, "dd/MM/yyyy", enCulture)) && (ddlBrknPrdIntrst.SelectedValue == "Yes"))
    //            else if ((Convert.ToDateTime(chqdt) >= Convert.ToDateTime(PDCDate)) && (ddlBrknPrdIntrst.SelectedValue == "Yes"))
    //            {

    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details should be less than Date Required.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Cheque Date in Bank Details should be less than Date Required.');", true);

    //                return;

    //            }

    //            else if (DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture).Day == 31)
    //            // else if (Convert.ToDateTime(txtFrstPDCdt.Text).Day == 31)
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('You can not create PDC for the selected First PDC date. Choose another Date.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('You can not create PDC for the selected First PDC date. Choose another Date.');", true);

    //                txtFrstPDCdt.Focus();
    //                return;
    //            }

    //            //else if (ddlBrknPrdIntrst.SelectedValue == "Yes" && DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture).Day == 31)
    //            else if (ddlBrknPrdIntrst.SelectedValue == "Yes" && Convert.ToDateTime(txtDateRq.Text).Day == 31)
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('You can not create PDC for the Entered Date Required. Choose another Date.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('You can not create PDC for the Entered Date Required. Choose another Date.');", true);

    //                txtDateRq.Focus();
    //                return;
    //            }

    //            else if (DaysBetAgrFPdcDt > 60 && ddlBrknPrdIntrst.SelectedValue == "No")
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Days between Cheque Date in Bank Details and First PDC Date cannot be greater than 60.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Days between Cheque Date in Bank Details and First PDC Date cannot be greater than 60.');", true);

    //                txtFrstPDCdt.Focus();
    //                return;
    //            }

    //            else if (DaysBetAgrFPdcDt > 60 && ddlBrknPrdIntrst.SelectedValue == "Yes")
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Days between Cheque Date in Bank Details and Date Required cannot be greater than 60.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Days between Cheque Date in Bank Details and Date Required cannot be greater than 60.');", true);

    //                txtFrstPDCdt.Focus();
    //                return;
    //            }

    //            else if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && (Convert.ToInt32(txtFrstPdctill.Text) > Convert.ToInt32(hdntxttenure.Value)))
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('PDC till month should be less than Tenure.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('PDC till month should be less than Tenure.');", true);

    //                txtFrstPdctill.Focus();
    //                return;
    //            }

    //            //****************************************
    //            if (gdvAplPDCChart.Rows.Count > 1)
    //            {
    //                if (hdnpopup.Value == "Edit")
    //                {
    //                    int i = 0;
    //                    for (i = Convert.ToInt32(txtFrstPdctill.Text); i <= (Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value)) - 1; i++)
    //                    {
    //                        gdvAplPDCChart.SelectedIndex = i;
    //                        HiddenField hdnProcessStatus = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnProcessStatus");
    //                        //Label lblApDate = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblApDate");
    //                        string lblSrNo = gdvAplPDCChart.SelectedRow.Cells[0].Text;
    //                        HiddenField hdnLegalStatus = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnLegalStatus");
    //                        if (hdnProcessStatus.Value.Trim() == "P")
    //                        {
    //                            txtFrstPdctill.Text = hdnEdtPDCTenure.Value;

    //                            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert(Due Date cannot be changed at " + lblApDate.Text + " " + txtpaymode.Text + ". Since it is processed." + Constants.vbCrLf + "Enter valid 1st PDC date till.');", true);
    //                            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert(Due Date cannot be changed at " + lblSrNo + FormatAddressSuffix(Convert.ToInt32(lblSrNo)) + " " + txtpaymode.Text + ". Since it is processed." + Constants.vbCrLf + "Enter valid 1st PDC date till.');", true);
    //                            //Added by Priya
    //                            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert(Due Date cannot be changed at " + lblSrNo + FormatAddressSuffix(Convert.ToInt32(lblSrNo)) + " " + hdntxtpaymode.Value + ". Since it is processed. <br> Enter valid 1st PDC date till.');", true);


    //                            return;
    //                        }
    //                        else if (Convert.ToInt32(hdnLegalStatus.Value) > 0)
    //                        {
    //                            txtFrstPdctill.Text = hdnEdtPDCTenure.Value;

    //                            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert(Due Date cannot be changed at " + lblSrNo + FormatAddressSuffix(Convert.ToInt32(lblSrNo)) + " " + txtpaymode.Text + ". Since Pre Intimation Notice is been sent." + Constants.vbCrLf + "Enter valid 1st PDC date till');", true);
    //                            //Added by Priya
    //                            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert(Due Date cannot be changed at " + lblSrNo + FormatAddressSuffix(Convert.ToInt32(lblSrNo)) + " " + hdntxtpaymode.Value + ". Since Pre Intimation Notice is been sent.<br> Enter valid 1st PDC date till');", true);

    //                            return; // TODO: might not be correct. Was : Exit Try
    //                        }
    //                    }
    //                }
    //            }
    //            //*****************************************

    //            //if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && (dt.Month != DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Month))
    //            if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && (dt.Month != Convert.ToDateTime(txtpdcChngdt.Text).Month))
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Correct Changed PDC Date. It should be within range of till months.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Correct Changed PDC Date. It should be within range of till months.');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }

    //            else if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && (dt.Month == DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Month) && (dt.Year != DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Year) && ddlBrknPrdIntrst.SelectedValue == "No")
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('1st PDC date till Months should be within range of 'First PDC Date' and 'Pdc Date Changes To');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('1st PDC date till Months should be within range of 'First PDC Date' and 'Pdc Date Changes To');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }

    //            else if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && (dt.Month == DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Month) && (dt.Year != DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Year) && ddlBrknPrdIntrst.SelectedValue == "No")
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('1st PDC date till Months should be within range of 'Date Required' and 'Pdc Date Changes To');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('1st PDC date till Months should be within range of 'Date Required' and 'Pdc Date Changes To');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }

    //            else if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && (Convert.ToInt32(txtFrstPdctill.Text) != TotalTenure) && (CompairMonth > 12) && ddlBrknPrdIntrst.SelectedValue == "No" && (DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture).Year == dtchngPDC.Year))
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Correct Year to Changed PDC Date. It should be within range of till months');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Correct Year to Changed PDC Date. It should be within range of till months');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }

    //            else if (Convert.ToInt32(txtFrstPdctill.Text) > 0 && Convert.ToInt32(txtFrstPdctill.Text) < intExactMonths && ((Convert.ToInt32(txtFrstPdctill.Text) != TotalTenure) && (CompairMonth > 12) && ddlBrknPrdIntrst.SelectedValue == "Yes" && (DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture).Year == DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Year)))
    //            {
    //                // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Correct Year to Changed PDC Date. It should be within range of till months.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Correct Year to Changed PDC Date. It should be within range of till months.');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }

    //            //else if (intPDCDiffDays > 31 && Convert.ToInt32(txtFrstPdctill.Text) < Convert.ToInt32(txttenure.Text))
    //            else if (intPDCDiffDays > 31)
    //            {
    //                //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Correct Changed PDC Date.Days between '1st PDC date till Months Date' and 'Changed PDC Date' can not be greated than 31.');", true);
    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Correct Changed PDC Date.Days between '1st PDC date till Months Date' and 'Changed PDC Date' can not be greated than 31.');", true);

    //                txtpdcChngdt.Focus();
    //                return;
    //            }
    //            //
    //            if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //            {
    //                hdnPDCDate.Value = txtDateRq.Text;
    //            }
    //            else if (ddlBrknPrdIntrst.SelectedValue == "No")
    //            {
    //                hdnPDCDate.Value = txtFrstPDCdt.Text;
    //            }

    //            fillAppPDCByOldDLL();

    //            if (ddlBrknPrdIntrst.SelectedIndex == 1)
    //            {

    //                Label lblEMI = (Label)gdvIIRChart.Rows[1].FindControl("lblEMI");
    //                Label lblAPEMI = (Label)gdvAplPDCChart.Rows[0].FindControl("lblAPEMI");
    //                lblAPEMI.Text = lblEMI.Text;

    //            }
    //        }
    //        else
    //        {
    //            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('First add all Bank Details!');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('First add all Bank Details!');", true);



    //        }
    //        //}
    //    }
    //    catch (Exception ex)
    //    {
    //        //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);
    //        //Added by Priya
    //        txtIRR.Text = (ex.StackTrace + ex.Message);
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);


    //    }
    //}


    //protected void btnAddChqe_Click(object sender, EventArgs e)
    //{
    //    int ChkPDCcount = gdvAplPDCChart.Rows.Count;
    //    if (ChkPDCcount > 0)
    //    {
    //        if (hdnddlPDcholder.Value == "" && hdnpopup.Value == "AppPDC")
    //        {
    //            //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select PDC Holder.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select PDC Holder.');", true);

    //            //ddlPDcholder.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        else if (hdnddlPDcholder.Value == "Applicant's" && ddlName.SelectedValue == "")
    //        {

    //            //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Applicant Name.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Applicant Name.');", true);
    //            ddlName.Focus();
    //            mpChqDetails.Show();
    //            return;

    //        }
    //        else if (hdnddlPDcholder.Value == "Co-Applicant's" && ddlName.SelectedValue == "")
    //        {
    //            //   ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select CoApplicant Name.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select CoApplicant Name.');", true);
    //            ddlName.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        else if (hdnddlPDcholder.Value == "Guarantor's" && ddlName.SelectedValue == "")
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Garanter Name.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Garanter Name.');", true);
    //            ddlName.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }

    //        else if (ddlbankName.SelectedIndex == 0)
    //        {
    //            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Bank Name.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Bank Name.');", true);
    //            ddlbankName.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        else if (ddlArea.SelectedIndex == 0)
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Area.');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Area.');", true);

    //            ddlArea.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        if (hdntxtchqno.Value == "")
    //        {
    //            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Cheque No.');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Cheque No.');", true);

    //            hdntxtchqno.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        if (hdntxtAccNo.Value == "")
    //        {
    //            //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Account No.');", true);


    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Account No.');", true);

    //            hdntxtAccNo.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        if (hdnddlAccType.Value == "")
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Account Type');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Account Type is not selected.');", true);


    //            //ddlAccType.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        if (hdnddlCTCNONCTC.Value == "")
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select CTS/NON-CTS');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('CTS/NON-CTS  is not selected');", true);

    //            hdntxtAccNo.Focus();
    //            mpChqDetails.Show();
    //            return;
    //        }
    //        int count = gdvAplPDCChart.Rows.Count;
    //        if (Convert.ToInt16(hdntxtupdtTo.Value) > count)
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Entered Update To Period not within Applicants PDC ');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Entered Update To Period not within Applicants PDC ');", true);

    //            mpChqDetails.Show();
    //            return;
    //        }

    //        int frm = Convert.ToInt32(hdntxtupdtfrom.Value);
    //        int To = Convert.ToInt32(hdntxtupdtTo.Value);
    //        string Branch = "";
    //        int pincode = 0;
    //        long ChqNo = 0;
    //        conn = new SqlConnection(strConnString);

    //        conn.Open();
    //        strquery = "select BankName,Branch from tblBankMaster where BankID='" + ddlbankName.SelectedValue + "' ";
    //        cmd = new SqlCommand(strquery, conn);
    //        da = new SqlDataAdapter(cmd);
    //        dt = new DataTable();
    //        da.Fill(dt);
    //        {
    //            hdnbnkName.Value = dt.Rows[0]["BankName"].ToString();
    //            Branch = dt.Rows[0]["Branch"].ToString();
    //        }

    //        strquery = "select  Area,Pincode from tblAreaMaster where AreaID='" + ddlArea.SelectedValue + "' ";
    //        cmd = new SqlCommand(strquery, conn);
    //        da = new SqlDataAdapter(cmd);
    //        dt1 = new DataTable();
    //        da.Fill(dt1);
    //        {
    //            if (dt1.Rows[0]["Pincode"] != "")
    //            {
    //                hdnArea.Value = dt1.Rows[0]["Area"].ToString();
    //                pincode = Convert.ToInt32(dt1.Rows[0]["Pincode"]);
    //            }
    //        }
    //        if (frm > 0 && To > 0)
    //        {
    //            if (hdnpopup.Value == "AppPDC")
    //            {
    //                for (int i = frm - 1; i <= To - 1; i++)
    //                {
    //                    gdvAplPDCChart.SelectedIndex = i;
    //                    HiddenField hdnPDCID = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnPDCID");
    //                    Label lblApDate = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblApDate");
    //                    Label lblAPEMI = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblAPEMI");
    //                    HiddenField hdnbankID = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnbankID");
    //                    Label lblbankName = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblbankName");
    //                    Label lblBranchName = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblBranchName");
    //                    HiddenField hdnAreaID = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnAreaID");
    //                    Label lblArea = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblArea");
    //                    Label lblPincode = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblPincode");
    //                    Label lblActype = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblActype");
    //                    Label lblAcNo = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblAcNo");
    //                    Label lblchqMICR = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblchqMICR");
    //                    HiddenField hdnAppPDCID = (HiddenField)gdvAplPDCChart.SelectedRow.FindControl("hdnAppPDCID");
    //                    Label lblPDCof = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblPDCof");
    //                    Label lblctsNonCts = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblctsNonCts");

    //                    lblbankName.Text = hdnbnkName.Value;
    //                    hdnbankID.Value = ddlbankName.SelectedValue;
    //                    lblBranchName.Text = Branch;
    //                    hdnAreaID.Value = ddlArea.SelectedValue;
    //                    lblArea.Text = hdnArea.Value;
    //                    lblPincode.Text = pincode.ToString();
    //                    lblActype.Text = hdnddlAccType.Value ;
    //                    lblAcNo.Text = hdntxtAccNo.Value;

    //                    if (i == (frm - 1))
    //                    {
    //                        lblchqMICR.Text = hdntxtchqno.Value;
    //                        ChqNo = Convert.ToInt64(lblchqMICR.Text);
    //                    }
    //                    else
    //                    {
    //                        if (hdntxtpaymode.Value.Trim() == "PDC")
    //                        {
    //                            ChqNo += 1;
    //                        }
    //                        lblchqMICR.Text = Convert.ToString(ChqNo);
    //                    }

    //                    hdnAppPDCID.Value = ddlName.SelectedValue;
    //                    if (hdnddlPDcholder.Value == "Applicant's")
    //                    {
    //                        lblPDCof.Text = "A";
    //                    }
    //                    else if (hdnddlPDcholder.Value == "Co-Applicant's")
    //                    {
    //                        lblPDCof.Text = "C";
    //                    }
    //                    else if (hdnddlPDcholder.Value == "Guarantor's")
    //                    {
    //                        lblPDCof.Text = "G";
    //                    }

    //                    lblctsNonCts.Text = hdnddlCTCNONCTC.Value;

    //                }
    //            }
    //            else if (hdnpopup.Value == "CoAppPDC")
    //            {
    //                //int mresult = ScriptManager.RegisterStartupScript(this, GetType(), "myFunction", "ConfirmMesaage();", true);
    //                //for (int i = frm - 1; i <= To - 1; i++)
    //                //{
    //                //    AddCoappPDC();
    //                //}


    //                dt = new DataTable();
    //                dt.Columns.Add("CoappID");
    //                dt.Columns.Add("CoappName");
    //                dt.Columns.Add("CoappBankID");
    //                dt.Columns.Add("CoappBankName");
    //                dt.Columns.Add("CoappBranchName");
    //                dt.Columns.Add("CoappAreaID");
    //                dt.Columns.Add("CoappArea");
    //                dt.Columns.Add("CoappPincode");
    //                dt.Columns.Add("CoappAccType");
    //                dt.Columns.Add("CoappAccNo");
    //                dt.Columns.Add("CoappchqMICRNo");
    //                dt.Columns.Add("CoappCTSnonCTS");
    //                dt.Columns.Add("CoappPDCID");
    //                dt.Columns.Add("CoappProcessStatus");
    //                dt.Columns.Add("CoappLegalStatus");

    //                DataRow dr = null;
    //                if (gdvCoappPDC.Rows.Count < To)
    //                {
    //                    for (int index = 0; index < gdvCoappPDC.Rows.Count; index++)
    //                    {

    //                        gdvCoappPDC.SelectedIndex = index;
    //                        HiddenField hdnCoappID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappID");
    //                        Label lblcoappName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblcoappName");
    //                        HiddenField hdnCoappBankidID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappBankidID");
    //                        Label lblCoappBankName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappBankName");
    //                        Label lblCoappBranchName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappBranchName");
    //                        HiddenField hdnCoappAreaID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappAreaID");
    //                        Label lblCoappArea = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappArea");
    //                        Label lblcoappPincode = (Label)gdvCoappPDC.SelectedRow.FindControl("lblcoappPincode");
    //                        Label lblCoappActype = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappActype");
    //                        Label lblCoappAcNo = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappAcNo");
    //                        Label lblCoappchqMICR = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappchqMICR");
    //                        //  HiddenField hdnCoAppPDCID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoAppPDCID");
    //                        Label lblCoappctsNonCts = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappctsNonCts");

    //                        dr = dt.NewRow();
    //                        dr["CoappID"] = hdnCoappID.Value;
    //                        dr["CoappName"] = lblcoappName.Text;
    //                        dr["CoappBankID"] = hdnCoappBankidID.Value;
    //                        dr["CoappBankName"] = lblCoappBankName.Text;
    //                        dr["CoappBranchName"] = lblCoappBranchName.Text;
    //                        dr["CoappAreaID"] = hdnCoappAreaID.Value;
    //                        dr["CoappArea"] = lblCoappArea.Text;
    //                        dr["CoappPincode"] = lblcoappPincode.Text;
    //                        dr["CoappAccType"] = lblCoappActype.Text;
    //                        dr["CoappAccNo"] = lblCoappAcNo.Text;
    //                        dr["CoappchqMICRNo"] = lblCoappchqMICR.Text;
    //                        dr["CoappCTSnonCTS"] = lblCoappctsNonCts.Text;
    //                        dr["CoappProcessStatus"] = "";


    //                        dt.Rows.Add(dr);

    //                    }

    //                    for (int i = frm - 1; i < To; i++)
    //                    {
    //                        dr = dt.NewRow();
    //                        dr["CoappID"] = "";
    //                        dr["CoappName"] = "";
    //                        dr["CoappBankID"] = "";
    //                        dr["CoappBankName"] = "";
    //                        dr["CoappBranchName"] = "";
    //                        dr["CoappAreaID"] = "";
    //                        dr["CoappArea"] = "";
    //                        dr["CoappPincode"] = "";
    //                        dr["CoappAccType"] = "";
    //                        dr["CoappAccNo"] = "";
    //                        dr["CoappchqMICRNo"] = "";
    //                        dr["CoappCTSnonCTS"] = "";
    //                        dr["CoappProcessStatus"] = "";
    //                        dt.Rows.Add(dr);
    //                    }
    //                    gdvCoappPDC.DataSource = dt;
    //                    gdvCoappPDC.DataBind();
    //                }




    //                for (int i = frm - 1; i < To; i++)
    //                {


    //                    gdvCoappPDC.SelectedIndex = i;
    //                    HiddenField hdnCoappID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappID");
    //                    Label lblcoappName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblcoappName");
    //                    HiddenField hdnCoappBankidID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappBankidID");
    //                    Label lblCoappBankName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappBankName");
    //                    Label lblCoappBranchName = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappBranchName");
    //                    HiddenField hdnCoappAreaID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoappAreaID");
    //                    Label lblCoappArea = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappArea");
    //                    Label lblcoappPincode = (Label)gdvCoappPDC.SelectedRow.FindControl("lblcoappPincode");
    //                    Label lblCoappActype = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappActype");
    //                    Label lblCoappAcNo = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappAcNo");
    //                    Label lblCoappchqMICR = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappchqMICR");
    //                    //  HiddenField hdnCoAppPDCID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoAppPDCID");
    //                    Label lblCoappctsNonCts = (Label)gdvCoappPDC.SelectedRow.FindControl("lblCoappctsNonCts");

    //                    hdnCoappID.Value = ddlName.SelectedValue;
    //                    lblcoappName.Text = ddlName.SelectedItem.Text;
    //                    hdnCoappBankidID.Value = ddlbankName.SelectedValue;
    //                    lblCoappBankName.Text = hdnbnkName.Value;
    //                    lblCoappBranchName.Text = Branch;
    //                    hdnCoappAreaID.Value = ddlArea.SelectedValue;
    //                    lblCoappArea.Text = hdnArea.Value;
    //                    lblcoappPincode.Text = pincode.ToString();
    //                    lblCoappActype.Text = hdnddlAccType.Value;
    //                    lblCoappAcNo.Text = hdntxtAccNo.Value;
    //                    lblCoappchqMICR.Text = hdntxtchqno.Value;
    //                    lblCoappctsNonCts.Text = hdnddlCTCNONCTC.Value;

    //                }
    //            }
    //            else if (hdnpopup.Value == "GARPDC")
    //            {
    //                //for (int i = frm - 1; i <= To - 1; i++)
    //                //{
    //                //    AddGarPDC();
    //                //}


    //                if (gdvGarPDC.Rows.Count < To)
    //                {
    //                    dt = new DataTable();
    //                    dt.Columns.Add("GarID");
    //                    dt.Columns.Add("GarName");
    //                    dt.Columns.Add("GarBankID");
    //                    dt.Columns.Add("GarBankName");
    //                    dt.Columns.Add("GarBranchName");
    //                    dt.Columns.Add("GarAreaID");
    //                    dt.Columns.Add("GarArea");
    //                    dt.Columns.Add("GarPincode");
    //                    dt.Columns.Add("GarAccType");
    //                    dt.Columns.Add("GarAccNo");
    //                    dt.Columns.Add("GarchqMICRNo");
    //                    dt.Columns.Add("GarCTSnonCTS");
    //                    dt.Columns.Add("GarProcessStatus");
    //                    dt.Columns.Add("GarPDCID");
    //                    dt.Columns.Add("GarLegalStatus");
    //                    DataRow dr = null;

    //                    for (int index = 0; index < gdvGarPDC.Rows.Count; index++)
    //                    {
    //                        gdvGarPDC.SelectedIndex = index;

    //                        HiddenField hdnGarID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarID");
    //                        Label lblGarName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarName");
    //                        //    HiddenField hdnGarBankidID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnCoappBankidID");
    //                        Label lblGarBankName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarBankName");
    //                        Label lblGarBranchName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarBranchName");
    //                        // HiddenField hdnGarAreaID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnCoappAreaID");
    //                        Label lblGarArea = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarArea");
    //                        Label lblGarPincode = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarPincode");
    //                        Label lblGarActype = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarActype");
    //                        //    Label lblGarAcNo = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarAcNo");
    //                        Label lblGarchqMICR = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarchqMICR");
    //                        //  HiddenField hdnCoAppPDCID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoAppPDCID");
    //                        Label lblGarctsNonCts = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarctsNonCts");
    //                        //added by priya
    //                        HiddenField hdnGarBankidID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarBankidID");
    //                        HiddenField hdnGarAreaID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarAreaID");

    //                        Label lblGarAcNo = (Label)gdvGarPDC.SelectedRow.FindControl("lblCoappAcNo");

    //                        dr = dt.NewRow();
    //                        dr["GarID"] = hdnGarID.Value;
    //                        dr["GarName"] = lblGarName.Text;
    //                        dr["GarBankID"] = hdnGarBankidID.Value;
    //                        dr["GarBankName"] = lblGarBankName.Text;
    //                        dr["GarBranchName"] = lblGarBranchName.Text;
    //                        dr["GarAreaID"] = hdnGarAreaID.Value;
    //                        dr["GarArea"] = lblGarArea.Text;
    //                        dr["GarPincode"] = lblGarPincode.Text;
    //                        dr["GarAccType"] = lblGarActype.Text;
    //                        dr["GarAccNo"] = lblGarAcNo.Text;
    //                        dr["GarchqMICRNo"] = lblGarchqMICR.Text;
    //                        dr["GarCTSnonCTS"] = lblGarctsNonCts.Text;
    //                        dr["GarProcessStatus"] = "";
    //                        dr["GarPDCID"] = "";
    //                        dr["GarLegalStatus"] = "";
    //                        dt.Rows.Add(dr);


    //                    }

    //                    for (int i = frm - 1; i < To; i++)
    //                    {
    //                        dr = dt.NewRow();
    //                        dr["GarID"] = "";
    //                        dr["GarName"] = "";
    //                        dr["GarBankID"] = "";
    //                        dr["GarBankName"] = "";
    //                        dr["GarBranchName"] = "";
    //                        dr["GarAreaID"] = "";
    //                        dr["GarArea"] = "";
    //                        dr["GarPincode"] = "";
    //                        dr["GarAccType"] = "";
    //                        dr["GarAccNo"] = "";
    //                        dr["GarchqMICRNo"] = "";
    //                        dr["GarCTSnonCTS"] = "";
    //                        dr["GarProcessStatus"] = "";
    //                        dr["GarPDCID"] = "";
    //                        dr["GarLegalStatus"] = "";
    //                        dt.Rows.Add(dr);
    //                    }

    //                    gdvGarPDC.DataSource = dt;
    //                    gdvGarPDC.DataBind();
    //                }


    //                for (int i = frm - 1; i < To; i++)
    //                {
    //                    gdvGarPDC.SelectedIndex = i;

    //                    HiddenField hdnGarID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarID");
    //                    Label lblGarName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarName");
    //                    //    HiddenField hdnGarBankidID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnCoappBankidID");
    //                    Label lblGarBankName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarBankName");
    //                    Label lblGarBranchName = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarBranchName");
    //                    // HiddenField hdnGarAreaID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnCoappAreaID");
    //                    Label lblGarArea = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarArea");
    //                    Label lblGarPincode = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarPincode");
    //                    Label lblGarActype = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarActype");
    //                    //    Label lblGarAcNo = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarAcNo");
    //                    Label lblGarchqMICR = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarchqMICR");
    //                    //  HiddenField hdnCoAppPDCID = (HiddenField)gdvCoappPDC.SelectedRow.FindControl("hdnCoAppPDCID");
    //                    Label lblGarctsNonCts = (Label)gdvGarPDC.SelectedRow.FindControl("lblGarctsNonCts");

    //                    //added by priya
    //                    HiddenField hdnGarBankidID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarBankidID");
    //                    HiddenField hdnGarAreaID = (HiddenField)gdvGarPDC.SelectedRow.FindControl("hdnGarAreaID");

    //                    Label lblGarAcNo = (Label)gdvGarPDC.SelectedRow.FindControl("lblCoappAcNo");

    //                    hdnGarID.Value = ddlName.SelectedValue;
    //                    lblGarName.Text = ddlName.SelectedItem.Text;
    //                    hdnGarBankidID.Value = ddlbankName.SelectedValue;
    //                    lblGarBankName.Text = hdnbnkName.Value;
    //                    lblGarBranchName.Text = Branch;
    //                    hdnGarAreaID.Value = ddlArea.SelectedValue;
    //                    lblGarArea.Text = hdnArea.Value;
    //                    lblGarPincode.Text = pincode.ToString();
    //                    lblGarActype.Text = hdnddlAccType.Value;

    //                    lblGarAcNo.Text = hdntxtAccNo.Value;
    //                    lblGarchqMICR.Text = hdntxtchqno.Value;

    //                    lblGarctsNonCts.Text = hdnddlCTCNONCTC.Value;

    //                }
    //            }
    //        }

    //        hdntxtEMIAmt.Value = "";
    //        hdntxtchqno.Value = "";
    //        hdntxtAccNo.Value = "";
    //        hdnddlAccType.Value = "";
    //        hdnddlCTCNONCTC.Value = "";
    //        hdntxtupdtfrom.Value = "";
    //        hdntxtupdtTo.Value = "";
    //    }
    //}


    //#region User Function
    //public void AddLoanAdjstCases()
    //{
    //    DataRow dr = null;
    //    dt.Columns.Add("AccName");
    //    dt.Columns.Add("AccID");
    //    dt.Columns.Add("DrCr");
    //    dt.Columns.Add("Debit");
    //    dt.Columns.Add("Credit");
    //    dt.Columns.Add("LoanAdjID");
    //    for (int i = 0; i < gdvLoanAdjustment.Rows.Count; i++)
    //    {
    //        gdvLoanAdjustment.SelectedIndex = i;
    //        //HiddenField hdnprodctAdjst = (HiddenField)gdvAdjustwith.SelectedRow.FindControl("hdnprodctAdjst");
    //        TextBox txtAccountName = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtAccountName");
    //        HiddenField hdnaccID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnaccID");
    //        DropDownList ddlDrCr = (DropDownList)gdvLoanAdjustment.SelectedRow.FindControl("ddlDrCr");
    //        HiddenField hdnDrDr = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnDrDr");
    //        TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //        TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //        HiddenField hdnLoanAdjID = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnLoanAdjID");
    //        hdnDrDr.Value = ddlDrCr.SelectedValue;

    //        if (txtAccountName.Text == "")
    //        {
    //            //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Account Name for Loan Adjustment.');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Account Name for Loan Adjustment.');", true);

    //            return;
    //        }
    //        else if (ddlDrCr.SelectedIndex == 0)
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select DR/CR.');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select DR/CR.');", true);

    //            return;
    //        }
    //        else if (ddlDrCr.SelectedIndex == 1)
    //        {
    //            if (txtDebit.Text == "")
    //            {
    //                //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Debit Amount.');", true);

    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Debit Amount.');", true);

    //                return;
    //            }
    //            else if (txtDebit.Text != "" && Convert.ToDouble(txtDebit.Text) == 0)
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Debit Amount.');", true);

    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Debit Amount.');", true);

    //                return;
    //            }
    //        }
    //        else if (ddlDrCr.SelectedIndex == 2)
    //        {
    //            if (txtcredit.Text == "")
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Credit Amount.');", true);

    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Credit Amount.');", true);

    //                return;
    //            }
    //            else if (txtcredit.Text != "" && Convert.ToDouble(txtcredit.Text) == 0)
    //            {
    //                //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Enter Credit Amount.');", true);

    //                //Added by Priya
    //                ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Enter Credit Amount.');", true);

    //                return;
    //            }
    //        }
    //        dr = dt.NewRow();
    //        //dr["ProductAdj"] = hdnprodctAdjst.Value.ToString();
    //        dr["AccName"] = txtAccountName.Text.ToString();
    //        dr["AccID"] = hdnaccID.Value.ToString();
    //        dr["DrCr"] = hdnDrDr.Value;
    //        dr["Debit"] = txtDebit.Text.ToString();
    //        dr["Credit"] = txtcredit.Text.ToString();
    //        dr["LoanAdjID"] = hdnLoanAdjID.Value;
    //        dt.Rows.Add(dr);
    //    }
    //    dr = dt.NewRow();
    //    dr["AccName"] = "";
    //    dr["AccID"] = "";
    //    dr["DrCr"] = "";
    //    dr["Debit"] = "";
    //    dr["Credit"] = "";
    //    dr["LoanAdjID"] = "";
    //    dt.Rows.Add(dr);
    //    gdvLoanAdjustment.DataSource = dt;
    //    gdvLoanAdjustment.DataBind();

    //    for (int j = 0; j < gdvLoanAdjustment.Rows.Count; j++)
    //    {
    //        gdvLoanAdjustment.SelectedIndex = j;
    //        DropDownList ddlDrCr = (DropDownList)gdvLoanAdjustment.SelectedRow.FindControl("ddlDrCr");
    //        HiddenField hdnDrDr = (HiddenField)gdvLoanAdjustment.SelectedRow.FindControl("hdnDrDr");
    //        TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //        TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //        ddlDrCr.SelectedValue = hdnDrDr.Value;
    //        if (ddlDrCr.SelectedValue == "DR")
    //        {
    //            txtcredit.Enabled = false;
    //            txtDebit.Enabled = true;
    //        }
    //        else
    //        {
    //            txtcredit.Enabled = true;
    //            txtDebit.Enabled = false;
    //        }

    //    }

    //}

    //protected void BindLoanAdjustment()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("AccName");
    //    dt.Columns.Add("AccID");
    //    dt.Columns.Add("DrCr");
    //    dt.Columns.Add("Debit");
    //    dt.Columns.Add("Credit");
    //    dt.Columns.Add("LoanAdjID");
    //    dt.Rows.Add();
    //    gdvLoanAdjustment.DataSource = dt;
    //    gdvLoanAdjustment.DataBind();
    //}

    //protected void CalculateTotDeduction()
    //{
    //    int i, count, Credit, ServiceCharge, ECSCharge, AdvEMIAmt, PrevOSDues = 0, LoanAdjustmentCredit = 0;
    //    string s = "";
    //    int AdvanceEMICount, EMIAmount = 0;
    //    double ServiceTax, ServiceTaxonECS, TotalDeduction;

    //    if (!string.IsNullOrEmpty(hdntxtServCharges.Value))
    //    {
    //        ServiceCharge = Convert.ToInt32(hdntxtServCharges.Value);
    //    }
    //    else
    //    {
    //        ServiceCharge = 0;
    //    }

    //    if (!string.IsNullOrEmpty(hdntxtServTax.Value))
    //    {
    //        ServiceTax = Convert.ToDouble(hdntxtServTax.Value);
    //    }
    //    else
    //    {
    //        ServiceTax = 0;
    //    }

    //    if (!string.IsNullOrEmpty(hdntxtECSCharges.Value))
    //    {
    //        ECSCharge = Convert.ToInt32(hdntxtECSCharges.Value);
    //    }
    //    else
    //    {
    //        ECSCharge = 0;
    //    }

    //    if (!string.IsNullOrEmpty(hdntxtEcsSerTax.Value))
    //    {
    //        ServiceTaxonECS = Convert.ToDouble(hdntxtEcsSerTax.Value);
    //    }
    //    else
    //    {
    //        ServiceTaxonECS = 0;
    //    }

    //    if (!string.IsNullOrEmpty(hdntxtAdEmi.Value))
    //    {
    //        AdvanceEMICount = Convert.ToInt32(hdntxtAdEmi.Value);
    //    }
    //    else
    //    {
    //        AdvanceEMICount = 0;
    //    }

    //    if (hdnEMItype.Value.Trim() == "Varying")
    //    {
    //        //for (int j = 0; j < gdvVarryingScheme.Rows.Count; j++)
    //        //{
    //        //    gdvVarryingScheme.SelectedIndex = j;
    //        //    Label lblEMI = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblEMI");
    //        //    EMIAmount = Convert.ToInt32(lblEMI.Text);
    //        //}
    //        int j = 0;
    //        if (gdvVarryingScheme.Rows.Count > 0)
    //        {
    //            gdvVarryingScheme.SelectedIndex = j;
    //            Label lblEMI = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblEMI");
    //            EMIAmount = Convert.ToInt32(lblEMI.Text);
    //        }
    //    }
    //    else
    //    {
    //        if (!string.IsNullOrEmpty(txtEMI.Text))
    //        {
    //            EMIAmount = Convert.ToInt32(txtEMI.Text);
    //        }
    //        else
    //        {
    //            EMIAmount = 0;
    //        }
    //    }
    //    AdvEMIAmt = AdvanceEMICount * EMIAmount;

    //    if (hdntxtPrevOsDue.Value != string.Empty)
    //    {
    //        PrevOSDues = Convert.ToInt32(hdntxtPrevOsDue.Value);
    //    }
    //    int result = 0;
    //    for (int k = 0; k < gdvLoanAdjustment.Rows.Count; k++)
    //    {
    //        gdvLoanAdjustment.SelectedIndex = k;
    //        TextBox txtcredit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtcredit");
    //        s = (txtcredit.Text);
    //        if (s == "" || ((int.TryParse(s, out result)) == false))
    //        {
    //            s = "0";
    //        }
    //        Credit = Convert.ToInt32(s);
    //        LoanAdjustmentCredit = LoanAdjustmentCredit + Credit;
    //    }
    //    TotalDeduction = ((ServiceCharge + ServiceTax) + (ECSCharge + ServiceTaxonECS) + (AdvEMIAmt) + (PrevOSDues) + (LoanAdjustmentCredit));
    //    txtTotalDedctn.Text = Convert.ToInt32(TotalDeduction).ToString();
    //    calculateNetPayAmount();
    //}

    //protected void calculateNetPayAmount()
    //{
    //    int Debit;
    //    double LoanAmount = 0;
    //    double lnamt = 0, TotalDeduction = 0, LoanAdjustedDebit = 0, NetPayAmount;
    //    string s = "";


    //    if (!string.IsNullOrEmpty(hdntxtLoanAmt.Value))
    //    {
    //        lnamt = Convert.ToDouble(hdntxtLoanAmt.Value);
    //    }
    //    else
    //    {
    //        lnamt = 0;
    //    }
    //    LoanAmount = Convert.ToInt32(lnamt);

    //    if (!string.IsNullOrEmpty(txtTotalDedctn.Text))
    //    {
    //        TotalDeduction = Convert.ToDouble(txtTotalDedctn.Text);
    //    }
    //    else
    //    {
    //        TotalDeduction = 0;
    //    }

    //    for (int i = 0; i < gdvLoanAdjustment.Rows.Count; i++)
    //    {
    //        gdvLoanAdjustment.SelectedIndex = i;
    //        TextBox txtDebit = (TextBox)gdvLoanAdjustment.SelectedRow.FindControl("txtDebit");
    //        s = (txtDebit.Text);
    //        if (s == "" || ((int.TryParse(s, out result)) == false))
    //        {
    //            s = "0";
    //        }
    //        Debit = Convert.ToInt32(s);
    //        LoanAdjustedDebit = LoanAdjustedDebit + Debit;
    //    }

    //    NetPayAmount = ((LoanAmount - TotalDeduction) + LoanAdjustedDebit);
    //    txtnetpayblAmt.Text = (Convert.ToInt32(NetPayAmount)).ToString();
    //}

    //protected void calculateBalancePayable()
    //{
    //    int Amount, NetPayAmount, PaidAmount, BalPayable;
    //    string s = "";
    //    PaidAmount = 0;
    //    CalculateTotDeduction();
    //    calculateNetPayAmount();
    //    NetPayAmount = Convert.ToInt32(txtnetpayblAmt.Text);
    //    for (int i = 0; i < gdvBankDetails.Rows.Count; i++)
    //    {
    //        gdvBankDetails.SelectedIndex = i;
    //        TextBox txtAmt = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtAmt");
    //        s = (txtAmt.Text);
    //        if (s == "" || ((int.TryParse(s, out result)) == false))
    //        {
    //            s = "0";
    //        }
    //        Amount = Convert.ToInt32(s);
    //        PaidAmount = PaidAmount + Amount;
    //        if (PaidAmount > NetPayAmount)
    //        {
    //            //  ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Bank Amount should not be Greater than Net Pay Amount!!!');", true);

    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Bank Amount should not be Greater than Net Pay Amount!!!');", true);


    //            BalPayable = NetPayAmount - PaidAmount;
    //            txtBalPayble.Text = BalPayable.ToString();
    //        }

    //    }
    //    BalPayable = NetPayAmount - PaidAmount;
    //    txtBalPayble.Text = BalPayable.ToString();
    //}




    //protected void BindIIRChart()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("SrNo");
    //    dt.Columns.Add("Date");
    //    dt.Columns.Add("Days");
    //    dt.Columns.Add("EMI");
    //    dt.Columns.Add("Principle");
    //    dt.Columns.Add("Interst");
    //    dt.Columns.Add("OSBal");
    //    dt.Columns.Add("finalOSBal");
    //    dt.Rows.Add();
    //    gdvIIRChart.DataSource = dt;
    //    gdvIIRChart.DataBind();
    //}


    //protected void BindAppPDCDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("PDCID");
    //    dt.Columns.Add("ApDate");
    //    dt.Columns.Add("APEMI");
    //    dt.Columns.Add("BankID");
    //    dt.Columns.Add("BankName");
    //    dt.Columns.Add("BranchName");
    //    dt.Columns.Add("AreaID");
    //    dt.Columns.Add("Area");
    //    dt.Columns.Add("Pincode");
    //    dt.Columns.Add("AccType");
    //    dt.Columns.Add("AccNo");
    //    dt.Columns.Add("chqMICRNo");
    //    dt.Columns.Add("AppPDCID");
    //    dt.Columns.Add("PDCOF");
    //    dt.Columns.Add("CTSnonCTS");
    //    dt.Columns.Add("ProcessStatus");
    //    dt.Columns.Add("LegalStatus");
    //    dt.Rows.Add();
    //    gdvAplPDCChart.DataSource = dt;
    //    gdvAplPDCChart.DataBind();
    //}

    //protected void BindCoappDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("CoAppName");
    //    dt.Columns.Add("CoAppPanNo");
    //    dt.Columns.Add("CoAppGender");
    //    dt.Columns.Add("CoAppIncome");
    //    dt.Columns.Add("CoAppOccupation");
    //    dt.Columns.Add("CoAppKnowSince");
    //    dt.Rows.Add();
    //    //gdvCoappDetails.DataSource = dt;
    //    //gdvCoappDetails.DataBind();
    //}


    //protected void BindGarDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("GarName");
    //    dt.Columns.Add("GarPanNo");
    //    dt.Columns.Add("GarGender");
    //    dt.Columns.Add("GarIncome");
    //    dt.Columns.Add("GarOccupation");
    //    dt.Columns.Add("GarKnowSince");
    //    dt.Rows.Add();
    //    gdvGarDetails.DataSource = dt;
    //    gdvGarDetails.DataBind();
    //}

    //protected void calculateFirsrPDCDate()
    //{
    //    try
    //    {

    //        var enCulture = new System.Globalization.CultureInfo("en-us");

    //        string FirstPDCDateToShow = string.Empty;
    //        //DateTime firstPDCdt = DateTime.Now;
    //        //DateTime Daterequir = DateTime.Now;

    //        //DateTime firsPDCDT = DateTime.Now;

    //        string firstPDCdt = string.Empty;
    //        string Daterequir = string.Empty;

    //        string firsPDCDT = string.Empty;

    //        int days = 0;
    //        int fpdcday = 0;

    //        conn = new SqlConnection(strConnString);
    //        conn.Open();
    //        cmd.Connection = conn;
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.CommandText = "SP_FTPL_AddDaysToDate";
    //        cmd.Parameters.AddWithValue("@inputdate", gbl.ChangeDateMMddyyyy(hdntxtAgreemntDate.Value.ToString()));
    //        da = new SqlDataAdapter(cmd);
    //        dt = new DataTable();
    //        da.Fill(dt);
    //        if (dt.Rows.Count > 0)
    //        {
    //            //firstPDCdt = DateTime.Parse(dt.Rows[0]["FPDC"].ToString());
    //            firstPDCdt = dt.Rows[0]["FPDC"].ToString();
    //            FirstPDCDateToShow = dt.Rows[0]["FPDC"].ToString();

    //            //txtFrstPDCdt.Text = firstPDCdt;
    //        }

    //        if (txtDateRq.Text.Trim() != string.Empty)
    //        {
    //            //Daterequir = DateTime.Parse(txtDateRq.Text);
    //            Daterequir = txtDateRq.Text;
    //        }

    //        //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");
    //        txtFrstPDCdt.Text = firstPDCdt;

    //        cmd = new SqlCommand("SP_DateConvert", conn);
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtFrstPDCdt.Text)); //IIS
    //        da = new SqlDataAdapter(cmd);
    //        dt = new DataTable();
    //        da.Fill(dt);
    //        if (dt.Rows.Count > 0)
    //        {
    //            fpdcday = Convert.ToInt32(dt.Rows[0]["date"].ToString());
    //        }

    //        conn.Close();


    //        DateTime dtFirstPDCDt = DateTime.ParseExact(Convert.ToString(txtFrstPDCdt.Text), "dd/MM/yyyy", null);
    //        //Convert.ToDateTime(txtFrstPDCdt.Text);
    //        //DateTime.ParseExact("06-13-2012", "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture)
    //        DateTime dtFirstPDCDtNew;

    //        if (fpdcday > 1 && fpdcday < 5)
    //        {
    //            days = 5 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday > 5 && fpdcday < 10)
    //        {
    //            days = 10 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday > 10 && fpdcday < 15)
    //        {
    //            days = 15 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday > 15 && fpdcday < 20)
    //        {
    //            days = 20 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya            
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday > 20 && fpdcday < 25)
    //        {
    //            days = 25 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");


    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday > 25 && fpdcday < 30)
    //        {
    //            days = 30 - fpdcday;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }
    //        else if (fpdcday == 31)
    //        {
    //            days = fpdcday + 1;
    //            dtFirstPDCDtNew = dtFirstPDCDt.AddDays(days);
    //            txtFrstPDCdt.Text = dtFirstPDCDtNew.ToString("dd/MM/yyyy");

    //            //firstPDCdt = Convert.ToString(DateTime.Parse(firstPDCdt).AddDays(days));

    //            //added by priya
    //            //txtFrstPDCdt.Text = firstPDCdt.ToString("dd/MM/yyyy").Replace("-", "/");

    //        }


    //        if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //        {
    //            txtFrstPDCdt.Enabled = false;
    //            ImageFrstPDCdt.Enabled = false;
    //            txtDateRq.Enabled = true;
    //            ImgDateRq.Enabled = true;

    //            if (txtDateRq.Text.Trim() != string.Empty)
    //            {
    //                DateTime dt1 = DateTime.ParseExact(Convert.ToString(txtFrstPDCdt.Text), "dd/MM/yyyy", null);
    //                DateTime dt2 = DateTime.ParseExact(Convert.ToString(Daterequir), "dd/MM/yyyy", null);


    //                //if (DateTime.Parse(Daterequir) > DateTime.Parse(firstPDCdt))
    //                if (dt2 > dt1)
    //                {
    //                    //txtNoofdaysDiff.Text = ((DateTime.Parse(Daterequir) - DateTime.Parse(firstPDCdt)).Days).ToString();
    //                    txtNoofdaysDiff.Text = ((dt2 - dt1).Days).ToString();
    //                }
    //                else
    //                {
    //                    txtNoofdaysDiff.Text = "";
    //                }
    //            }
    //        }
    //        else
    //        {
    //            txtFrstPDCdt.Enabled = true;
    //            ImageFrstPDCdt.Enabled = true;
    //            txtDateRq.Enabled = false;
    //            ImgDateRq.Enabled = false;
    //            txtNoofdaysDiff.Text = "";
    //        }

    //        txtDiffIntrst.Text = "";
    //        txtDateRq.Focus();
    //        return;
    //    }

    //    catch (Exception ex)
    //    {
    //        txtFrstPDCdt.Text = "Error";
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);
    //    }
    //}

    //protected void BindCoappPDCDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("CoappID");
    //    dt.Columns.Add("CoappName");
    //    dt.Columns.Add("CoappBankID");
    //    dt.Columns.Add("CoappBankName");
    //    dt.Columns.Add("CoappBranchName");
    //    dt.Columns.Add("CoappAreaID");
    //    dt.Columns.Add("CoappArea");
    //    dt.Columns.Add("CoappPincode");
    //    dt.Columns.Add("CoappAccType");
    //    dt.Columns.Add("CoappAccNo");
    //    dt.Columns.Add("CoappchqMICRNo");
    //    dt.Columns.Add("CoappCTSnonCTS");
    //    dt.Columns.Add("CoappPDCID");
    //    dt.Columns.Add("CoappProcessStatus");
    //    dt.Columns.Add("CoappLegalStatus");
    //    dt.Rows.Add();
    //    gdvCoappPDC.DataSource = dt;
    //    gdvCoappPDC.DataBind();
    //}

    //protected void BindGarPDCDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("GarID");
    //    dt.Columns.Add("GarName");
    //    dt.Columns.Add("GarBankID");
    //    dt.Columns.Add("GarBankName");
    //    dt.Columns.Add("GarBranchName");
    //    dt.Columns.Add("GarAreaID");
    //    dt.Columns.Add("GarArea");
    //    dt.Columns.Add("GarPincode");
    //    dt.Columns.Add("GarAccType");
    //    dt.Columns.Add("GarAccNo");
    //    dt.Columns.Add("GarchqMICRNo");
    //    dt.Columns.Add("GarCTSnonCTS");
    //    dt.Columns.Add("GarPDCID");
    //    dt.Columns.Add("GarProcessStatus");
    //    dt.Columns.Add("GarLegalStatus");
    //    dt.Rows.Add();
    //    gdvGarPDC.DataSource = dt;
    //    gdvGarPDC.DataBind();
    //}


    //protected void CaseCancelCheckedChange()
    //{
    //    int count = 0;
    //    string sancID = "";
    //    string strquery = string.Empty;
    //    //string CaseAdjusted = "";
    //    //string CaseAdjustWith = "";
    //    //string DisbursementMessege = "";			
    //    conn = new SqlConnection(strConnString);
    //    conn.Open();
    //    strquery = "SELECT COUNT(*) FROM TDisbursement_CancelledCase_BasicInfo " + "WHERE LoginID='" + txtLoginID.Text + "' ";
    //    cmd = new SqlCommand(strquery, conn);
    //    cmd.CommandType = CommandType.Text;
    //    count = Convert.ToInt32(cmd.ExecuteScalar());

    //    strquery = "select SanctionID from TSanctioning_Appl_BasicInfo where LoginID='" + txtLoginID.Text + "'";
    //    cmd = new SqlCommand(strquery, conn);
    //    cmd.CommandType = CommandType.Text;
    //    da = new SqlDataAdapter(cmd);
    //    dt = new DataTable();
    //    da.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        sancID = dt.Rows[0]["SanctionID"].ToString();
    //    }
    //    conn.Close();
    //    if (count > 0)
    //    {
    //        DisburseLoginDetailsRTR(sancID, txtLoginID.Text);
    //    }

    //}

    //protected void DisburseLoginDetailsRTR(string id, string LoginID)
    //{
    //    string CaseAdjust = "";
    //    int caseAdjustWith = 0;
    //    string caseAdjustWithLoginID = "";
    //    string PrepaymentDone = "";

    //    int PPCount = 0;


    //    try
    //    {
    //        string strquery = "";
    //        string LoanType = "";
    //        string adjLoanType = "";
    //        int dateEqualCount = 0;
    //        string SPPDate = "";
    //        conn = new SqlConnection(strConnString);
    //        conn.Open();

    //        strquery = "SELECT  TPLAppForm_BasicDetails.LoginID,CaseAdjusted,CaseAdjustWith,TPLAppForm_BasicDetails.LoanType, CaseAdjustWithLoginID, TPLAppForm_BasicDetails.LoanType 'AdjLoanType' FROM TSanctioning_Appl_BasicInfo " +
    //                              " INNER join TPLAppForm_BasicDetails ON TSanctioning_Appl_BasicInfo.LoginID=TPLAppForm_BasicDetails.LoginID " +
    //                             // " INNER join TPLAppForm_BasicDetails as TPLAppForm_BasicDetails_1 ON TSanctioning_Appl_BasicInfo.CaseAdjustWithLoginID=TPLAppForm_BasicDetails_1.LoginID " +
    //                             "WHERE  SanctionID=" + id + " ";

    //        cmd = new SqlCommand(strquery, conn);
    //        cmd.CommandType = CommandType.Text;
    //        da = new SqlDataAdapter(cmd);
    //        dt = new DataTable();
    //        da.Fill(dt);
    //        if (dt.Rows.Count > 0)
    //        {
    //            CaseAdjust = dt.Rows[0]["CaseAdjusted"].ToString();

    //            if ((dt.Rows[0]["CaseAdjustWith"].Equals(System.DBNull.Value) || dt.Rows[0]["CaseAdjustWith"] == ""))
    //            {
    //                caseAdjustWith = 0;
    //                caseAdjustWithLoginID = "";
    //            }
    //            else
    //            {
    //                caseAdjustWith = Convert.ToInt32(dt.Rows[0]["CaseAdjustWith"].ToString());
    //                caseAdjustWithLoginID = Convert.ToString(dt.Rows[0]["CaseAdjustWithLoginID"].ToString());
    //                hdnCaseNo.Value = caseAdjustWith.ToString();
    //                LoginID = dt.Rows[0]["LoginID"].ToString();
    //                LoanType = dt.Rows[0]["LoanType"].ToString();
    //                adjLoanType = dt.Rows[0]["AdjLoanType"].ToString().Trim();
    //            }
    //        }

    //        if (CaseAdjust == "Y")
    //        {
    //            strquery = "SELECT COUNT(*) FROM TPrePayment_Appl_BasicDetails WHERE CaseNo='" + caseAdjustWith + "' and LoginID='" + caseAdjustWithLoginID + "' ";
    //            cmd = new SqlCommand(strquery, conn);
    //            cmd.Transaction = transc;
    //            cmd.CommandType = CommandType.Text;
    //            PPCount = Convert.ToInt32(cmd.ExecuteScalar());

    //            if (PPCount > 0)
    //            {
    //                PrepaymentDone = "Yes";
    //            }
    //            else
    //            {
    //                PrepaymentDone = "No";
    //            }
    //            hdnPrepaymentDone.Value = PrepaymentDone;

    //            if (PrepaymentDone == "No")
    //            {
    //                // FillDisburseDetails(id);
    //                // ScriptManager.RegisterStartupScript(this, GetType(), "myFunction", "ConfirmMesaage();", true);

    //                Response.Redirect("PrepaymentDetails.aspx?adjustCaseNo= " + caseAdjustWith + "&adjustLoginID=" + caseAdjustWithLoginID + "&CaseAdjust= " + CaseAdjust + "&LoginID= " + LoginID + "&LoanType= " + adjLoanType + "&dateEqualCount=" + dateEqualCount + "&PPDate=" + SPPDate, false);

    //            }
    //            else
    //            {
    //                int NetReceivable = 0;
    //                DateTime PPDate;
    //                int PPID = 0;
    //                string ReferenceNo = "";
    //                string CaseNo = "";
    //                string agrdt = "";
    //                string strLoginID = string.Empty;

    //                strquery = "SELECT NetReceivable,Convert(varchar,PPDate,103)PPDate,PPID,ReferenceNo,TPrePayment_Appl_BasicDetails.CaseNo,TPrePayment_Appl_BasicDetails.LoginID, TPLAppForm_BasicDetails.LoanType FROM TPrePayment_Appl_BasicDetails " +
    //                           "INNER JOIN TPLAppForm_BasicDetails " +
    //                                "ON TPLAppForm_BasicDetails.LoginID = TPrePayment_Appl_BasicDetails.LoginID " +
    //                           "WHERE CaseNo='" + caseAdjustWith + "' and TPrePayment_Appl_BasicDetails.LoginID='" + caseAdjustWithLoginID + "' ";
    //                da = new SqlDataAdapter(strquery, conn);
    //                dt = new DataTable();
    //                da.Fill(dt);
    //                if (dt.Rows.Count > 0)
    //                {
    //                    if (dt.Rows[0]["NetReceivable"].Equals(System.DBNull.Value))
    //                    {

    //                        NetReceivable = 0;
    //                    }
    //                    else
    //                    {
    //                        NetReceivable = Convert.ToInt32(dt.Rows[0]["NetReceivable"].ToString());
    //                    }
    //                    // PPDate = DateTime.Parse(dt.Rows[0]["PPDate"].ToString());
    //                    PPID = Convert.ToInt32(dt.Rows[0]["PPID"].ToString());
    //                    ReferenceNo = dt.Rows[0]["ReferenceNo"].ToString();
    //                    CaseNo = dt.Rows[0]["CaseNo"].ToString();
    //                    strLoginID = dt.Rows[0]["LoginID"].ToString();
    //                    SPPDate = dt.Rows[0]["PPDate"].ToString();
    //                    hdnPPDate.Value = SPPDate;
    //                    hdnLoanType.Value = dt.Rows[0]["LoanType"].ToString().Trim();
    //                }

    //                int Count1 = 0;
    //                strquery = "select case when '" + gbl.ChangeDateMMddyyyy(hdntxtAgreemntDate.Value) + "' = '" + gbl.ChangeDateMMddyyyy(SPPDate) + "' then 1 else 0 end";
    //                cmd = new SqlCommand(strquery, conn);
    //                Count1 = Convert.ToInt32(cmd.ExecuteScalar());

    //                if (Count1 == 1)
    //                {
    //                    if (hdnpopup.Value == "Edit")
    //                    {
    //                        hdntxtPrevOsDue.Value = NetReceivable.ToString();
    //                        hdntxtPrevCaseNo.Value = CaseNo;
    //                        hdnPrevLoginID.Value = strLoginID;
    //                        hdntxtPrevCaseNoLoanType.Value = adjLoanType + Convert.ToString(CaseNo);
    //                    }
    //                    else
    //                    {
    //                        hdntxtPrevOsDue.Value = NetReceivable.ToString();
    //                        hdntxtPrevCaseNo.Value = CaseNo;
    //                        hdnPrevLoginID.Value = strLoginID;
    //                        hdntxtPrevCaseNoLoanType.Value = adjLoanType + Convert.ToString(CaseNo);
    //                        FillDisburseDetails(id);
    //                    }
    //                }

    //                else
    //                {
    //                    dateEqualCount = 1;
    //                    // ScriptManager.RegisterStartupScript(this, GetType(), "myFunction", "ConfirmMesaage1();", true);
    //                    //Added by Priya
    //                    ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "myFunction", "ConfirmMesaage1();", true);
    //                    // ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "DisburseLoginDetailsRTRAlert", "alert('" + ex.Message + "');", true);

    //                    //temp 17/10/2016
    //                    //DeleteDataDisbursementforDelete(ReferenceNo, PPID, CaseNo, strLoginID);

    //                    //strquery = "SELECT COUNT(*) FROM TPrePayment_Appl_BasicDetails WHERE CaseNo='" + caseAdjustWith + "' and LoginID='" + caseAdjustWithLoginID + "' ";
    //                    //cmd = new SqlCommand(strquery, conn);
    //                    //cmd.Transaction = transc;
    //                    //cmd.CommandType = CommandType.Text;
    //                    //PPCount = Convert.ToInt32(cmd.ExecuteScalar());

    //                    //if (PPCount > 0)
    //                    //{
    //                    //    PrepaymentDone = "Yes";
    //                    //}
    //                    //else
    //                    //{
    //                    //    PrepaymentDone = "No";
    //                    //}
    //                    //hdnPrepaymentDone.Value = PrepaymentDone;

    //                    //if (PrepaymentDone == "No")
    //                    //{
    //                    //    Response.Redirect("PrepaymentDetails.aspx?adjustCaseNo= " + caseAdjustWith + "&CaseAdjust= " + CaseAdjust + "&LoginID= " + LoginID + "&LoanType= " + LoanType + "&dateEqualCount=" + dateEqualCount + "&PPDate=" + SPPDate, false);

    //                    //}
    //                    //temp 17/10/2016
    //                }
    //            }
    //        }
    //        else
    //        {
    //            FillDisburseDetails(id);
    //        }
    //        conn.Close();
    //    }
    //    catch (Exception ex)
    //    {
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "DisburseLoginDetailsRTRAlert", "alert('" + ex.Message + "');", true);
    //    }

    //}


    //protected void FillDisburseDetails(string id)
    //{
    //    double loanamt = 0;
    //    //string[] TenureEmi;
    //    int dfltAccCount = 0;

    //    string strquery = "";
    //    cmd = new SqlCommand();
    //    cmd.Connection = conn;
    //    cmd.CommandType = CommandType.StoredProcedure;
    //    cmd.CommandText = "SP_Disburse_LoginDetails_RTR";
    //    cmd.Parameters.AddWithValue("@SanctionID", id);
    //    ////cmd.Parameters.AddWithValue("@branchID", Session["branchId"].ToString());
    //    ////cmd.Parameters.AddWithValue("@FYID", Session["fyId"].ToString());
    //    cmd.Parameters.AddWithValue("@branchID", hdnbranchID.Value);
    //    //cmd.Parameters.AddWithValue("@FYID", hdnFyID.Value);
    //    da = new SqlDataAdapter(cmd);
    //    dt = new DataTable();
    //    da.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        txtLoginID.Text = dt.Rows[0]["LoginID"].ToString();
    //        hdntxtloginDt.Value = dt.Rows[0]["Login Date"].ToString();
    //        hdntxtpaymode.Value = dt.Rows[0]["PayMode"].ToString();
    //        //loandt = DateTime.Parse(txtLoginDt.Text);
    //        hdntxtAplName.Value = dt.Rows[0]["Name"].ToString();
    //        hdntxtmrktExetv.Value = dt.Rows[0]["Executive Name"].ToString();
    //        hdntxtDealer.Value = dt.Rows[0]["Dealer Name"].ToString();
    //        hdntxtNetSurplusIncome.Value = dt.Rows[0]["NetsurplusIncome"].ToString();
    //        hdntxtCaseType.Value = dt.Rows[0]["CaseType"].ToString();
    //        hdntxtPrefLoanIntrevw.Value = dt.Rows[0]["PrefLoanAmount"].ToString();
    //        hdntxtSancLoanAmt.Value = dt.Rows[0]["SanctionedLoan"].ToString();
    //        txtSchemeName.Text = dt.Rows[0]["SchemesName"].ToString();
    //        loanamt = Math.Ceiling(Convert.ToDouble(dt.Rows[0]["LoanAmount"].ToString()));
    //        hdntxtLoanAmt.Value = loanamt.ToString();
    //        hdntxtServCharges.Value = dt.Rows[0]["ServiceChrgAmount"].ToString();
    //        txtInterestRate.Text = dt.Rows[0]["RateofInterest"].ToString();
    //        hdntxtServTax.Value = dt.Rows[0]["ServiceTaxAmount"].ToString();
    //        hdntxttenure.Value = dt.Rows[0]["Tenure"].ToString();
    //        hdntxtECSCharges.Value = dt.Rows[0]["ECSCharges"].ToString();
    //        hdntxtAdEmi.Value = dt.Rows[0]["AdvanceEMI"].ToString();
    //        hdntxtEcsSerTax.Value = dt.Rows[0]["ServiceTaxonECS"].ToString();
    //        txtEMI.Text = dt.Rows[0]["EMI"].ToString();
    //        hdnSancID.Value = dt.Rows[0]["SchemesID"].ToString();
    //        hdnEMItype.Value = dt.Rows[0]["EMIType"].ToString().Trim();
    //        hdntxtAdEmiamt.Value = (Convert.ToInt32(hdntxtAdEmi.Value) * Convert.ToDouble(txtEMI.Text)).ToString();
    //        hdntxtLoanType.Value = dt.Rows[0]["LoanType"].ToString().Trim();
    //        txtFrstPdctill.Text = dt.Rows[0]["Tenure"].ToString();

    //        string AmountTenure = dt.Rows[0]["AmountTenure"].ToString();
            
    //        if (hdnoperation.Value != "update")
    //        {
    //            int CaseNo = 0;
    //            strquery = "SELECT isnull( Max(CaseNo),0) FROM  TDisbursement_Appl_BasicInfo " +
    //                        "inner join TPLAppForm_BasicDetails " +
    //                            "on TPLAppForm_BasicDetails.LoginID = TDisbursement_Appl_BasicInfo.LoginID  " +
    //                        "where TPLAppForm_BasicDetails.LoanType='" + hdntxtLoanType.Value.Trim() + "' ";
    //            cmd = new SqlCommand(strquery, conn);
    //            CaseNo = Convert.ToInt32(cmd.ExecuteScalar());

    //            CaseNo = CaseNo + 1;
    //            hdntxtcaseNo.Value = CaseNo.ToString();
    //        }

    //        //da = new SqlDataAdapter(cmd);
    //        //dt = new DataTable();
    //        //da.Fill(dt);
    //        //if (dt.Rows.Count > 0)
    //        //{
    //        //    CaseNo =Convert.ToInt32(dt.Rows[0]["CaseNo"]);
    //        //}

    //        //////con1.Close();

    //        strquery = "SELECT Remark FROM TSanctioning_Appl_BasicInfo WHERE LoginID='" + txtLoginID.Text + "' ";
    //        cmd = new SqlCommand(strquery, conn);
    //        da = new SqlDataAdapter(cmd);
    //        dt1 = new DataTable();
    //        da.Fill(dt1);
    //        if (dt1.Rows.Count > 0)
    //        {
    //            hdntxtRemark.Value = dt1.Rows[0]["Remark"].ToString();
    //        }

    //        int MonthsFrom = 1;
    //        if (hdnEMItype.Value.Trim() == "Flat")
    //        {
    //            //DataTable dtTenure = new DataTable();
    //            //dtTenure.Columns.Add("MonthsFrom");
    //            //dtTenure.Columns.Add("MonthsTo");
    //            //dtTenure.Columns.Add("TotalMonths");
    //            //dtTenure.Columns.Add("EMI");

    //            //AmountTenure = AmountTenure.Replace("Rs.", "");
    //            //AmountTenure = AmountTenure.Replace("mnt", "");

    //            //if (AmountTenure.Contains("*"))
    //            //{
    //            //    var TenureMon = AmountTenure.Split('*');
    //            //    var EMI = TenureMon[0];
    //            //    var MonthsTo = TenureMon[1];

    //            //    dtTenure.Rows.Add(MonthsFrom, MonthsTo, MonthsTo, EMI);
    //            //}
    //            //else if (AmountTenure.Contains("X"))
    //            //{
    //            //    var TenureMon = AmountTenure.Split('X');
    //            //    var EMI = TenureMon[0];
    //            //    var MonthsTo = TenureMon[1];

    //            //    dtTenure.Rows.Add(MonthsFrom, MonthsTo, MonthsTo, EMI);

    //            //    gdvVarryingScheme.DataSource = dtTenure;
    //            //    gdvVarryingScheme.DataBind();
    //            //}
    //            //else
    //            //{
    //            BindVaryngEMIDetails();
    //            //  }




    //        }
    //        else if (hdnEMItype.Value.Trim() == "Varying")
    //        {

    //            DataTable dtTenure = new DataTable();
    //            dtTenure.Columns.Add("MonthsFrom");
    //            dtTenure.Columns.Add("MonthsTo");
    //            dtTenure.Columns.Add("TotalMonths");
    //            dtTenure.Columns.Add("EMI");

    //            if (AmountTenure.Contains("/"))
    //            {
    //                TenureEmi = AmountTenure.Split('/');
    //            }
    //            else if (AmountTenure.Contains(","))
    //            {
    //                AmountTenure = AmountTenure.Replace("Rs.", "");
    //                AmountTenure = AmountTenure.Replace("mnt", "");
    //                TenureEmi = AmountTenure.Split(',');
    //            }


    //            for (int index = 0; index < TenureEmi.Count(); index++)
    //            {

    //                var Tenure = TenureEmi[index];

    //                if (Tenure.Contains("*"))
    //                {
    //                    TenureMon = Tenure.Split('*');
    //                }
    //                else if (Tenure.Contains("X"))
    //                {
    //                    TenureMon = Tenure.Split('X');
    //                }

    //                var EMI = TenureMon[0];
    //                var MonthsTo = TenureMon[1];

    //                dtTenure.Rows.Add(MonthsFrom, MonthsTo, MonthsTo, EMI);

    //                MonthsFrom = Convert.ToInt32(MonthsTo) + 1;
    //            }

    //            gdvVarryingScheme.DataSource = dtTenure;
    //            gdvVarryingScheme.DataBind();
    //            //strquery = "SELECT count(*) FROM TSanctioning_Appl_SchemeDetails WHERE LoginID='" + txtLoginID.Text + "' ";
    //            //cmd = new SqlCommand(strquery, con);
    //            //if (cmd.ExecuteScalar() != DBNull.Value)
    //            //{
    //            //    Count = Convert.ToInt32(cmd.ExecuteScalar());
    //            //}
    //            //else
    //            //{
    //            //    Count = 0;
    //            //}
    //            //if (Count > 0)
    //            //{
    //            //    strquery = "select AmountTenure from TSanctioning_Appl_SchemeDetails WHERE LoginID='" + txtLoginID.Text + "' ";
    //            //    cmd = new SqlCommand(strquery, con);
    //            //    da = new SqlDataAdapter(cmd);
    //            //    dt1 = new DataTable();
    //            //    da.Fill(dt1);
    //            //    if (dt1.Rows.Count > 0)
    //            //    {


    //            //        // hdnEMIongrid.Value = dt1.Rows[0]["EMI"].ToString();
    //            //        gdvVarryingScheme.DataSource = dt1;
    //            //        gdvVarryingScheme.DataBind();
    //            //    }
    //            //}

    //        }
    //        gdvBankDetails.SelectedIndex = 0;
    //        TextBox txtbnkcahAC = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtbnkcahAC");
    //        TextBox txtbranchName = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtbranchName");
    //        HiddenField hdnbankid = (HiddenField)gdvBankDetails.SelectedRow.FindControl("hdnbankid");
    //        TextBox txtcashChqNo = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtcashChqNo");
    //        TextBox txtCashcheqDt = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtCashcheqDt");

    //        strquery = "SELECT AccCount=COUNT(*) FROM tblDefaultBankAccountMaster";
    //        cmd = new SqlCommand(strquery, conn);
    //        cmd.CommandType = cmd.CommandType;
    //        dfltAccCount = Convert.ToInt32(cmd.ExecuteScalar());

    //        if (dfltAccCount > 0)
    //        {
    //            strquery = "SELECT tblDefaultBankAccountMaster.AccountID, tblAccountMaster.Name, Area=ISNULL(tblAreaMaster.Area, ''), tblAccountMaster.GPID, " +
    //                                "CashAccCount=(SELECT COUNT(*) FROM UGeneralGroupORAccountSetting_Cash_Ac_Group WHERE CashGPID=tblAccountMaster.GPID) " +
    //                       "FROM tblDefaultBankAccountMaster  " +
    //                       "INNER JOIN tblAccountMaster " +
    //                              "ON tblDefaultBankAccountMaster.AccountID=tblAccountMaster.AccountID " +
    //                       "LEFT OUTER JOIN tblAreaMaster " +
    //                              "ON tblAccountmaster.AreaID=tblAreaMaster.AreaID ";
    //            cmd = new SqlCommand(strquery, conn);
    //            da = new SqlDataAdapter(cmd);
    //            dt1 = new DataTable();
    //            da.Fill(dt1);
    //            {
    //                if (Convert.ToInt32(dt1.Rows[0]["CashAccCount"].ToString()) > 0)
    //                {
    //                    txtbnkcahAC.Text = dt1.Rows[0]["Name"].ToString();
    //                    txtbranchName.Text = dt1.Rows[0]["Area"].ToString();
    //                    hdnbankid.Value = dt1.Rows[0]["AccountID"].ToString();
    //                    txtcashChqNo.Text = "Cash";
    //                    txtcashChqNo.ReadOnly = true;
    //                    txtbnkcahAC.ReadOnly = true;
    //                    txtbranchName.ReadOnly = true;

    //                }
    //                else
    //                {
    //                    // txtcashChqNo.Enabled = true;
    //                    txtbnkcahAC.Text = dt1.Rows[0]["Name"].ToString();
    //                    txtbranchName.Text = dt1.Rows[0]["Area"].ToString();
    //                    hdnbankid.Value = dt1.Rows[0]["AccountID"].ToString();
    //                    txtcashChqNo.ReadOnly = false;
    //                    txtbnkcahAC.ReadOnly = true;
    //                    txtbranchName.ReadOnly = true;
    //                }

    //            }
    //            // txtCashcheqDt.Text = txtAgreemntDate.Text;

    //        }
    //        else
    //        {
    //            // ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Bank Account not found in Account Master.');", true);
    //            //Added by Priya
    //            ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "Disbursement", "alert('Error during filling Disburse data.');", true);

    //            return;
    //        }

    //        CalculateTotDeduction();
    //        calculateBalancePayable();


    //    }
    //}


    //protected void BindVaryngEMIDetails()
    //{
    //    dt = new DataTable();
    //    dt.Columns.Add("MonthsFrom");
    //    dt.Columns.Add("MonthsTo");
    //    dt.Columns.Add("TotalMonths");
    //    dt.Columns.Add("EMI");
    //    dt.Rows.Add();
    //    gdvVarryingScheme.DataSource = dt;
    //    gdvVarryingScheme.DataBind();
    //}

    //protected void fillAppPDCByOldDLL()
    //{
    //    //try
    //    //{
    //    //BindAppPDCDetails();
    //    //BindCoappPDCDetails();
    //    //BindGarPDCDetails();
    //    int ATotalPDCCount = 0;
    //    int AUCPDCCount = 0;
    //    int CUCPDCCount = 0;
    //    int GUCPDCCount = 0;
    //    string strquery = string.Empty;
    //    //string VaryInterest = "";
    //    //double NewIRR = 0;
    //    //double CurrIRR = 0;
    //    //int intLETFDays = 0;

    //    if (txtLoginID.Text != "")
    //    {
    //        var enCulture = new System.Globalization.CultureInfo("en-us");
    //        int m = 0, Tenure, EMI, AdvanceEMI, ChangedPDCMonth;
    //        string PDCDate = "";
    //        string AssignPDCDate = "";
    //        string date = "";
    //        string ApDate = "";
    //        int APEMI = 0;

    //        Tenure = Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value);
    //        EMI = Convert.ToInt32(txtEMI.Text);
    //        PDCDate = hdnPDCDate.Value;
    //        ChangedPDCMonth = Convert.ToInt32(txtFrstPdctill.Text);

    //        conn = new SqlConnection(strConnString);
    //        conn.Open();

    //        //*************************************
    //        if (hdnoperation.Value == "update")
    //        {
    //            ATotalPDCCount = 0;
    //            AUCPDCCount = 0;
    //            CUCPDCCount = 0;
    //            GUCPDCCount = 0;


    //            if (ATotalPDCCount > 0)
    //            {

    //                strquery = "SELECT COUNT(*) FROM TDisbursement_Appl_PDCDetails " + "WHERE TDisbursement_Appl_PDCDetails.LoginID='" + txtLoginID.Text + "' ";
    //                cmd = new SqlCommand(strquery, conn);
    //                cmd.CommandType = CommandType.Text;
    //                ATotalPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //                strquery = "SELECT COUNT(*) FROM TDisbursement_Appl_PDCDetails " + "WHERE TDisbursement_Appl_PDCDetails.LoginID='" + txtLoginID.Text + "' " + "AND ProcessStatus='UP' AND Clearstatus='UC' ";
    //                cmd = new SqlCommand(strquery, conn);
    //                cmd.CommandType = CommandType.Text;
    //                AUCPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //                strquery = "SELECT COUNT(*) FROM TDisbursement_CoAppl_PDCDetails " + "WHERE TDisbursement_CoAppl_PDCDetails.LoginID='" + txtLoginID.Text + "' " + "AND ProcessStatus='UP' AND Clearstatus='UC' ";
    //                cmd = new SqlCommand(strquery, conn);
    //                cmd.CommandType = CommandType.Text;
    //                CUCPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //                strquery = "SELECT COUNT(*) FROM TDisbursement_Gar_PDCDetails " + "WHERE TDisbursement_Gar_PDCDetails.LoginID='" + txtLoginID.Text + "' " + "AND ProcessStatus='UP' AND Clearstatus='UC' ";
    //                cmd = new SqlCommand(strquery, conn);
    //                cmd.CommandType = CommandType.Text;
    //                GUCPDCCount = Convert.ToInt32(cmd.ExecuteScalar());

    //                if (gdvAplPDCChart.Rows.Count > 1)
    //                {
    //                    if (AUCPDCCount != (Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value)))
    //                    {

    //                        for (int i = ChangedPDCMonth; i <= Tenure - 1; i++)
    //                        {
    //                            gdvIIRChart.SelectedIndex = i;
    //                            Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");

    //                            gdvAplPDCChart.SelectedIndex = i - 1;
    //                            Label lblApDate = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblApDate");
    //                            if (i == ChangedPDCMonth)
    //                            {
    //                                //lblApDate.Text = txtpdcChngdt.Text;
    //                                lblDate.Text = txtpdcChngdt.Text;
    //                            }
    //                            else
    //                            {
    //                                //AssignPDCDate = calculateNextDate(DateTime.ParseExact(date, "dd/MM/yyyy", enCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    //                                AssignPDCDate = calculateNextDate(DateTime.ParseExact(lblApDate.Text, "dd/MM/yyyy", enCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    //                                //lblApDate.Text = AssignPDCDate;
    //                                lblDate.Text = AssignPDCDate;
    //                            }
    //                            date = lblApDate.Text;
    //                        }

    //                        int rowID = 0;
    //                        rowID = ChangedPDCMonth;

    //                        for (int i = ChangedPDCMonth; i <= Tenure - 1; i++)
    //                        {
    //                            gdvIIRChart.SelectedIndex = i;
    //                            Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //                            //gdvAplPDCChart.SelectedIndex = i - 1;
    //                            gdvAplPDCChart.SelectedIndex = rowID;
    //                            Label lblApDate = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblApDate");
    //                            if (i != ChangedPDCMonth)
    //                            {
    //                                lblDate.Text = lblApDate.Text;
    //                                rowID = rowID + 1;
    //                            }
    //                        }
    //                        hdnEdtPDCTenure.Value = (txtFrstPdctill.Text);
    //                        return;
    //                    }
    //                }
    //            }
    //        }
    //        //**************************************

    //        transc = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransaction");
    //        strquery = "CREATE TABLE #tblTemp (PDCID int,ApDate nvarchar(50), APEMI INT,BankID INT,BankName nvarchar(50),BranchName nvarchar(50), " +
    //                                          "AreaID int ,Area nvarchar(30),Pincode int,AccType nvarchar(30),AccNo nvarchar(20) , " +
    //                                           "chqMICRNo nvarchar(30),AppPDCID int,PDCOF nvarchar(20),CTSnonCTS nvarchar(20),ProcessStatus nvarchar(20),LegalStatus nvarchar(20))";
    //        cmd = new SqlCommand(strquery, conn);
    //        cmd.Transaction = transc;
    //        cmd.CommandType = CommandType.Text;
    //        cmd.ExecuteNonQuery();

    //        if (hdnEMItype.Value.Trim() == "Varying")
    //        {
    //            int Count, StartMonth, EndMonth, TotalTenure, VaryEMI;
    //            StartMonth = 0;
    //            EndMonth = 0;
    //            TotalTenure = 0;
    //            VaryEMI = 0;
    //            Count = 0;
    //            AdvanceEMI = Convert.ToInt32(hdntxtAdEmi.Value);

    //            for (int k = 0; k < gdvVarryingScheme.Rows.Count; k++)
    //            {
    //                gdvVarryingScheme.SelectedIndex = k;
    //                Label lblMonthFrom = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthFrom");
    //                Label lblMonthTo = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthTo");
    //                Label lblTotalMonth = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblTotalMonth");
    //                Label lblEMI = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblEMI");
    //                StartMonth = Convert.ToInt32(lblMonthFrom.Text);
    //                EndMonth = Convert.ToInt32(lblMonthTo.Text);
    //                TotalTenure = Convert.ToInt32(lblTotalMonth.Text);
    //                VaryEMI = Convert.ToInt32(lblEMI.Text);
    //                if (TotalTenure > 0)
    //                {
    //                    if (Count == 0)
    //                    {
    //                        for (m = (StartMonth - 1); m <= ((EndMonth - AdvanceEMI) - 1); m++)
    //                        {

    //                            APEMI = VaryEMI;

    //                            if (m == 0)
    //                            {
    //                                ApDate = hdnPDCDate.Value;

    //                            }
    //                            else if (m == ChangedPDCMonth)
    //                            {
    //                                ApDate = txtpdcChngdt.Text;

    //                            }
    //                            else
    //                            {
    //                                AssignPDCDate = calculateNextDate(DateTime.ParseExact(date, "dd/MM/yyyy", enCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    //                                ApDate = AssignPDCDate;
    //                            }

    //                            strquery = "insert into #tblTemp values(0,'" + ApDate + "'," + APEMI + ",0,'','',0,'','','','','',0,'','','','')";
    //                            cmd = new SqlCommand(strquery, conn);
    //                            cmd.Transaction = transc;
    //                            cmd.CommandType = CommandType.Text;
    //                            cmd.ExecuteNonQuery();

    //                            date = ApDate;

    //                        }
    //                        Count = 1;
    //                    }
    //                    else if (Count > 0)
    //                    {
    //                        for (int l = m; l <= ((EndMonth - AdvanceEMI) - 1); l++)
    //                        {
    //                            APEMI = VaryEMI;

    //                            if (l == 0)
    //                            {
    //                                ApDate = hdnPDCDate.Value;

    //                            }
    //                            else if (l == ChangedPDCMonth)
    //                            {
    //                                ApDate = txtpdcChngdt.Text;

    //                            }
    //                            else
    //                            {
    //                                AssignPDCDate = calculateNextDate(DateTime.ParseExact(date, "dd/MM/yyyy", enCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    //                                ApDate = AssignPDCDate;
    //                            }

    //                            strquery = "insert into #tblTemp values(0,'" + ApDate + "'," + APEMI + ",0,'','',0,'','','','','',0,'','','','')";
    //                            cmd = new SqlCommand(strquery, conn);
    //                            cmd.Transaction = transc;
    //                            cmd.CommandType = CommandType.Text;
    //                            cmd.ExecuteNonQuery();

    //                            date = ApDate;
    //                        }
    //                    }

    //                }
    //            }

    //        }
    //        else
    //        {
    //            for (int i = 0; i <= Tenure - 1; i++)
    //            {
    //                if (i == 0)
    //                {
    //                    ApDate = hdnPDCDate.Value;

    //                    APEMI = EMI;
    //                }
    //                else if (i == ChangedPDCMonth)
    //                {
    //                    ApDate = txtpdcChngdt.Text;
    //                    APEMI = EMI;

    //                }
    //                else
    //                {
    //                    AssignPDCDate = calculateNextDate(DateTime.ParseExact(date, "dd/MM/yyyy", enCulture).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture));
    //                    ApDate = AssignPDCDate;
    //                    APEMI = EMI;
    //                }

    //                strquery = "insert into #tblTemp values(0,'" + ApDate + "'," + APEMI + ",0,'','',0,'','','','','',0,'','','','')";
    //                cmd = new SqlCommand(strquery, conn);
    //                cmd.Transaction = transc;
    //                cmd.CommandType = CommandType.Text;
    //                cmd.ExecuteNonQuery();

    //                date = ApDate;
    //            }

    //        }


    //        transc.Commit();
    //        strquery = "select * from #tblTemp";
    //        cmd = new SqlCommand(strquery, conn);
    //        da = new SqlDataAdapter(cmd);
    //        dt = new DataTable();
    //        da.Fill(dt);
    //        gdvAplPDCChart.DataSource = dt;
    //        gdvAplPDCChart.DataBind();
    //        conn.Close();

    //        GC.Collect();
    //        VaryInterest = "";
    //        NewIRR = 0;
    //        CurrIRR = 0;
    //        intLETFDays = 0;
    //        VariableIRR = 0.0;
    //        counter = 0;

    //        ShowChartByOldDLL();
    //    }

    //    else
    //    {
    //        //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Select Applicant's Login No and then Try Again!!! ');", true);
    //        //Added by Priya
    //        ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('Select Applicant's Login No and then Try Again!!! ');", true);


    //        return;
    //    }
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    //ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);
    //    //    //Added by Priya
    //    //    ScriptManager.RegisterClientScriptBlock((this as Control), this.GetType(), "LoginAlert", "alert('" + ex.StackTrace + "');", true);


    //    //}
    //}
    //protected string calculateNextDate(string date)
    //{
    //    var enCulture = new System.Globalization.CultureInfo("en-us");
    //    //DateTime dt, CompairMonth;
    //    DateTime CompairMonth = DateTime.Now;
    //    var dt = DateTime.Now;
    //    //con = new SqlConnection(strConnString);
    //    //con.Open();
    //    //strquery= "select getdate()";
    //    //cmd = new SqlCommand(str);

    //    string strdt = "";
    //    string sDay = "";
    //    int ExactMonths = Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value);
    //    string AssignPDCDate = "";

    //    dt = DateTime.ParseExact(date, "dd/MM/yyyy", enCulture);


    //    if (ddlBrknPrdIntrst.SelectedValue == "No")
    //    {
    //        sDay = (DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture).Day).ToString();
    //        CompairMonth = DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture).AddMonths(Convert.ToInt32(txtFrstPdctill.Text));
    //    }

    //    else if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //    {
    //        sDay = (DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture).Day).ToString();
    //        CompairMonth = DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture).AddMonths(Convert.ToInt32(txtFrstPdctill.Text));

    //    }

    //    if (dt.Month == 2)
    //    {

    //        if ((ddlBrknPrdIntrst.SelectedValue == "No" && (dt.Day != (DateTime.ParseExact(txtFrstPDCdt.Text, "dd/MM/yyyy", enCulture).Day)) && (Convert.ToInt32(txtFrstPdctill.Text) < 0)))
    //        {
    //            int day = 0;

    //            cmd = new SqlCommand("SP_DateConvert", conn);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Transaction = transc;
    //            cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtFrstPDCdt.Text));
    //            day = Convert.ToInt32(cmd.ExecuteScalar());
    //            day = day - dt.Day;

    //            // day = DateTime.ParseExact(txtFrstPDCdt.Text,"yyyy/MM/dd", CultureInfo.InvariantCulture).Day - dt.Day;

    //            dt = dt.AddDays((dt.Day + day));
    //        }

    //        else if ((ddlBrknPrdIntrst.SelectedValue == "Yes") && (dt.Day != (DateTime.ParseExact(txtDateRq.Text, "dd/MM/yyyy", enCulture).Day)) && (Convert.ToInt32(txtFrstPdctill.Text) < 0))
    //        {
    //            int day;
    //            //con = new SqlConnection(strConnString);
    //            cmd = new SqlCommand("SP_DateConvert", conn);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Transaction = transc;
    //            cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtDateRq.Text));
    //            day = Convert.ToInt32(cmd.ExecuteScalar());
    //            day = day - dt.Day;


    //            // day = DateTime.ParseExact(txtDateRq.Text,"yyyy/MM/dd", CultureInfo.InvariantCulture).Day - dt.Day;

    //            dt = dt.AddDays((dt.Day + day));
    //        }
    //        else if ((dt.Day != (DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture).Day)) && (Convert.ToInt32(txtFrstPdctill.Text) > 0) && (Convert.ToInt32(txtFrstPdctill.Text) != Convert.ToInt32(hdntxttenure.Value)) && (Convert.ToInt32(txtFrstPdctill.Text) < ExactMonths) && (dt.Month == CompairMonth.Month))
    //        {
    //            int day = 0;
    //            //con = new SqlConnection(strConnString);
    //            //con.Open();
    //            cmd = new SqlCommand("SP_DateConvert", conn);
    //            cmd.CommandType = CommandType.StoredProcedure;
    //            cmd.Transaction = transc;
    //            cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtpdcChngdt.Text));
    //            day = Convert.ToInt32(cmd.ExecuteScalar());
    //            day = day - dt.Day;

    //            //  day = DateTime.ParseExact(txtpdcChngdt.Text, "yyyy/MM/dd", CultureInfo.InvariantCulture).Day - dt.Day;

    //            dt = dt.AddDays((dt.Day + day));
    //        }
    //        else
    //        {
    //            dt = dt.AddMonths(1);
    //            // dt = dt.AddDays(1).AddMonths(1).AddDays(-1);
    //            if (dt > DateTime.ParseExact(txtpdcChngdt.Text, "dd/MM/yyyy", enCulture) && (Convert.ToInt32(txtFrstPdctill.Text) > 0) && (Convert.ToInt32(txtFrstPdctill.Text) < ExactMonths))
    //            {
    //                int day = 0;
    //                //con = new SqlConnection(strConnString);
    //                //con.Open();
    //                cmd = new SqlCommand("SP_DateConvert", conn);
    //                cmd.CommandType = CommandType.StoredProcedure;
    //                cmd.Transaction = transc;
    //                cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtpdcChngdt.Text));
    //                day = Convert.ToInt32(cmd.ExecuteScalar());
    //                day = day - dt.Day;

    //                // day = DateTime.Parse(txtpdcChngdt.Text).Day - dt.Day;
    //                dt = dt.AddDays(day);
    //            }
    //            else
    //            {
    //                int day = 0;

    //                if (ddlBrknPrdIntrst.SelectedValue == "No")
    //                {
    //                    //con = new SqlConnection(strConnString);
    //                    //con.Open();
    //                    cmd = new SqlCommand("SP_DateConvert", conn);
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Transaction = transc;
    //                    cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtFrstPDCdt.Text));
    //                    day = Convert.ToInt32(cmd.ExecuteScalar());
    //                    day = day - dt.Day;

    //                    //  day =DateTime.Parse(txtFrstPDCdt.Text).Day - dt.Day;
    //                }
    //                else if (ddlBrknPrdIntrst.SelectedValue == "Yes")
    //                {
    //                    //day =DateTime.Parse(txtDateRq.Text).Day - dt.Day;
    //                    //con = new SqlConnection(strConnString);
    //                    //con.Open();
    //                    cmd = new SqlCommand("SP_DateConvert", conn);
    //                    cmd.CommandType = CommandType.StoredProcedure;
    //                    cmd.Transaction = transc;
    //                    cmd.Parameters.AddWithValue("@date", gbl.ChangeDateMMddyyyy(txtDateRq.Text));
    //                    day = Convert.ToInt32(cmd.ExecuteScalar());
    //                    day = day - dt.Day;

    //                    // day = DateTime.ParseExact(txtDateRq.Text, "yyyy/MM/dd", CultureInfo.InvariantCulture).Day - dt.Day;
    //                }
    //                dt = dt.AddDays(day);
    //            }
    //        }

    //    }
    //    else
    //    {
    //        dt = dt.AddMonths(1);
    //        // dt = dt.AddDays(1).AddMonths(1).AddDays(-1);
    //    }

    //    //AssignPDCDate = dt.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);  
    //    AssignPDCDate = dt.ToString();
    //    AssignPDCDate = DateTime.Parse(AssignPDCDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

    //    return AssignPDCDate;
    //}

    //protected void ShowChartByOldDLL()
    //{

    //    int i = 0;
    //    int outflow = 0;
    //    int LIC = 0;
    //    int DelIncentive = 0;
    //    double LoanAmt = 0;
    //    int TotalTenure = 0;
    //    int GridTenure = 0;
    //    int GridEMI = 0;

    //    double SerTaxAmt = 0;
    //    double SertaxECS = 0;

    //    int SerChargAmt = 0;
    //    int ECSCharges = 0;
    //    int TotalDeduction = 0;
    //    int Tenure = 0;
    //    int AdEMI = 0;
    //    int NetPayable = 0;
    //    string SelectedScheme = "";
    //    string PercentFmt = "#0.000000";

    //    SelectedScheme = txtSchemeName.Text;
    //    TotalTenure = Convert.ToInt32(hdntxttenure.Value);

    //    if (hdnEMItype.Value.Trim() == "Varying")
    //    {
    //        for (int k = 0; k < gdvVarryingScheme.Rows.Count; k++)
    //        {
    //            gdvVarryingScheme.SelectedIndex = k;
    //            Label lblMonthFrom = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthFrom");
    //            Label lblMonthTo = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthTo");
    //            Label lblTotalMonth = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblTotalMonth");
    //            Label lblEMI = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblEMI");
    //            if (i == 0)
    //            {
    //                GridTenure = Convert.ToInt32(lblTotalMonth.Text);
    //                GridEMI = Convert.ToInt32(lblEMI.Text);

    //                if (GridTenure > 0 & GridTenure < TotalTenure)
    //                {
    //                    EMI = GridEMI;
    //                }
    //                else
    //                {
    //                    EMI = Convert.ToInt32(txtEMI.Text);
    //                }

    //                break; // TODO: might not be correct. Was : Exit For
    //            }
    //        }
    //    }
    //    else
    //    {
    //        EMI = Convert.ToInt32(txtEMI.Text);
    //    }

    //    Tenure = Convert.ToInt32(hdntxttenure.Value) - Convert.ToInt32(hdntxtAdEmi.Value);
    //    AdEMI = Convert.ToInt32(hdntxtAdEmi.Value) * EMI;

    //    // BindVaryngEMIDetails();
    //    for (i = 0; i <= Tenure; i++)
    //    {
    //        AddIIRChart(i);
    //    }

    //    for (i = 0; i <= 0; i++)
    //    {
    //        gdvIIRChart.SelectedIndex = i;

    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        Label lblEMI = (Label)gdvIIRChart.SelectedRow.FindControl("lblEMI");
    //        Label lblprinciple = (Label)gdvIIRChart.SelectedRow.FindControl("lblprinciple");
    //        Label lblInterest = (Label)gdvIIRChart.SelectedRow.FindControl("lblInterest");
    //        Label lblOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblOSBal");
    //        Label lblfinalOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblfinalOSBal");
    //        Label lblDays = (Label)gdvIIRChart.SelectedRow.FindControl("lblDays");
    //        gdvBankDetails.SelectedIndex = i;
    //        TextBox txtCashcheqDt = (TextBox)gdvBankDetails.SelectedRow.FindControl("txtCashcheqDt");
    //        lblDate.Text = txtCashcheqDt.Text;
    //        lblDays.Text = "0";
    //    }


    //    int j = 0;
    //    for (i = 0; i <= Tenure; i++)
    //    {
    //        gdvAplPDCChart.SelectedIndex = j;
    //        Label lblApDate = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblApDate");
    //        Label lblAPEMI = (Label)gdvAplPDCChart.SelectedRow.FindControl("lblAPEMI");

    //        gdvIIRChart.SelectedIndex = i;
    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        Label lblEMI = (Label)gdvIIRChart.SelectedRow.FindControl("lblEMI");

    //        if ((!(i == 0)) && (i == 1))
    //        {
    //            lblDate.Text = lblApDate.Text;
    //            lblEMI.Text = lblAPEMI.Text;
    //            j += 1;
    //        }
    //        else if (!(i == 0))
    //        {
    //            lblDate.Text = lblApDate.Text;
    //            lblEMI.Text = lblAPEMI.Text;
    //            j += 1;
    //        }
    //    }

    //    for (i = 1; i <= Tenure; i++)
    //    {
    //        //System.DateTime dtStartDate = default(System.DateTime);
    //        //System.DateTime dtEndDate = default(System.DateTime);
    //        //TimeSpan tsTimeSpan = default(TimeSpan);
    //        int iNumberofDays = 0;

    //        gdvIIRChart.SelectedIndex = i;
    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        gdvIIRChart.SelectedIndex = i - 1;
    //        Label lblDate1 = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");

    //        con1 = new SqlConnection(strConnString);
    //        cmd = new SqlCommand("SP_dateFormat", con1);
    //        cmd.CommandType = CommandType.StoredProcedure;
    //        //cmd.Transaction = transc;
    //        cmd.Parameters.AddWithValue("@dateformat", gbl.ChangeDateMMddyyyy(lblDate1.Text));
    //        cmd.Parameters.AddWithValue("@dateformat2", gbl.ChangeDateMMddyyyy(lblDate.Text));
    //        da = new SqlDataAdapter(cmd);
    //        dt2 = new DataTable();
    //        da.Fill(dt2);

    //        iNumberofDays = Convert.ToInt32(dt2.Rows[0]["Date"]);
    //        con1.Close();

    //        if (iNumberofDays < 0)
    //        {
    //            iNumberofDays = -(iNumberofDays);
    //        }

    //        gdvIIRChart.SelectedIndex = i;
    //        Label lblDays = (Label)gdvIIRChart.SelectedRow.FindControl("lblDays");
    //        lblDays.Text = iNumberofDays.ToString();
    //    }


    //    conn = new SqlConnection(strConnString);
    //    strquery = "SELECT LICPremium, DealerIncentive FROM TSanctioning_Appl_SchemeDetails where LoginID='" + txtLoginID.Text + "'";
    //    cmd = new SqlCommand(strquery, conn);
    //    cmd.CommandType = CommandType.Text;
    //    da = new SqlDataAdapter(cmd);
    //    dt = new DataTable();
    //    da.Fill(dt);
    //    if (dt.Rows.Count > 0)
    //    {
    //        if (dt.Rows[0]["LICPremium"] != null && dt.Rows[0]["LICPremium"] != "")
    //        {
    //            LIC = Convert.ToInt32(dt.Rows[0]["LICPremium"].ToString());
    //        }

    //        DelIncentive = Convert.ToInt32(dt.Rows[0]["DealerIncentive"].ToString());
    //    }
    //    else
    //    {
    //        return;
    //    }

    //    LoanAmt = Convert.ToDouble(hdntxtLoanAmt.Value);
    //    SerChargAmt = Convert.ToInt32(hdntxtServCharges.Value);
    //    SerTaxAmt = Convert.ToDouble(hdntxtServTax.Value);
    //    ECSCharges = Convert.ToInt32(hdntxtECSCharges.Value);
    //    SertaxECS = Convert.ToDouble(hdntxtEcsSerTax.Value);

    //    TotalDeduction = Convert.ToInt32(AdEMI + (SerChargAmt + SerTaxAmt) + (ECSCharges + SertaxECS));

    //    NetPayable = Convert.ToInt32(LoanAmt) - TotalDeduction;
    //    //net payable

    //    outflow = Convert.ToInt32(NetPayable + SerTaxAmt + SertaxECS + DelIncentive + LIC);
    //    int NetOutflow = 0;
    //    NetOutflow = outflow;
    //    for (int m = 0; m <= 0; m++)
    //    {
    //        gdvIIRChart.SelectedIndex = m;
    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        Label lblDays = (Label)gdvIIRChart.SelectedRow.FindControl("lblDays");
    //        Label lblEMI = (Label)gdvIIRChart.SelectedRow.FindControl("lblEMI");
    //        Label lblprinciple = (Label)gdvIIRChart.SelectedRow.FindControl("lblprinciple");
    //        Label lblInterest = (Label)gdvIIRChart.SelectedRow.FindControl("lblInterest");
    //        Label lblOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblOSBal");
    //        Label lblfinalOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblfinalOSBal");

    //        lblEMI.Text = (-(NetOutflow)).ToString();
    //        lblprinciple.Text = "0";
    //        lblInterest.Text = "0";
    //        lblOSBal.Text = (Convert.ToDouble(hdntxtLoanAmt.Value) - AdEMI).ToString();
    //        lblfinalOSBal.Text = "0";
    //    }

    //    //Taken IRR grid in Table
    //    DataTable dtIRR;
    //    dtIRR = new DataTable();
    //    DataRow drIRR;

    //    dtIRR.Columns.Add("Sr No");
    //    dtIRR.Columns.Add("Date");
    //    dtIRR.Columns.Add("Days");
    //    dtIRR.Columns.Add("EMI");
    //    dtIRR.Columns.Add("Principal");
    //    dtIRR.Columns.Add("Interest");
    //    dtIRR.Columns.Add("O/s Balance");
    //    dtIRR.Columns.Add("Final O/s Balance");

    //    for (int r = 0; r <= gdvIIRChart.Rows.Count - 1; r++)
    //    {
    //        drIRR = dtIRR.NewRow();

    //        gdvIIRChart.SelectedIndex = r;
    //        Label lblsrNo = (Label)gdvIIRChart.SelectedRow.FindControl("lblsrNo");
    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        Label lblDays = (Label)gdvIIRChart.SelectedRow.FindControl("lblDays");
    //        Label lblEMI = (Label)gdvIIRChart.SelectedRow.FindControl("lblEMI");
    //        Label lblprinciple = (Label)gdvIIRChart.SelectedRow.FindControl("lblprinciple");
    //        Label lblInterest = (Label)gdvIIRChart.SelectedRow.FindControl("lblInterest");
    //        Label lblOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblOSBal");
    //        Label lblfinalOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblfinalOSBal");

    //        drIRR["Sr No"] = lblsrNo.Text;
    //        drIRR["Date"] = lblDate.Text;
    //        drIRR["Days"] = lblDays.Text;

    //        if (r == 0)
    //        {
    //            drIRR["EMI"] = lblEMI.Text;
    //            drIRR["O/s Balance"] = lblOSBal.Text;
    //        }
    //        else
    //        {
    //            drIRR["EMI"] = 0;
    //            drIRR["O/s Balance"] = 0;
    //        }

    //        drIRR["Principal"] = lblprinciple.Text;
    //        drIRR["Interest"] = lblInterest.Text;
    //        drIRR["Final O/s Balance"] = lblfinalOSBal.Text;

    //        dtIRR.Rows.Add(drIRR);
    //    }
    //    //End : Taken IRR grid in Table

    //    //Taken Variable EMI Grid in Table
    //    DataTable dtVScheme;
    //    dtVScheme = new DataTable();
    //    DataRow drVScheme;

    //    dtVScheme.Columns.Add("MonthsFrom");
    //    dtVScheme.Columns.Add("MonthsTo");
    //    dtVScheme.Columns.Add("TotalMonths");
    //    dtVScheme.Columns.Add("SchemeEMI");

    //    for (int r = 0; r <= gdvVarryingScheme.Rows.Count - 1; r++)
    //    {
    //        drVScheme = dtVScheme.NewRow();

    //        gdvVarryingScheme.SelectedIndex = r;
    //        Label lblMonthFrom = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthFrom");
    //        Label lblMonthTo = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblMonthTo");
    //        Label lblTotalMonth = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblTotalMonth");
    //        Label lblEMI = (Label)gdvVarryingScheme.SelectedRow.FindControl("lblEMI");

    //        drVScheme["MonthsFrom"] = lblMonthFrom.Text;
    //        drVScheme["MonthsTo"] = lblMonthTo.Text;
    //        drVScheme["TotalMonths"] = lblTotalMonth.Text;
    //        drVScheme["SchemeEMI"] = lblEMI.Text;

    //        dtVScheme.Rows.Add(drVScheme);
    //    }
    //    //End : Taken Variable EMI Grid in Table

    //    DataTable dtFinalIRR = new DataTable();

    //    string strTenure = Convert.ToString(TotalTenure);
    //    txtIRR.Text = "Done";

    //    string strRetrieve = string.Empty;

    //    if (hdnEMItype.Value.Trim() == "Varying")
    //    {
    //        //dtFinalIRR = (clsIRRChartCalculation.showVariableIRR(dtVScheme, dtIRR, Convert.ToInt32(EMI), Convert.ToDateTime(txtFrstPDCdt.Text), Convert.ToDateTime(txtDateRq.Text), Convert.ToDateTime(txtAgreemntDate.Text), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;
    //        //dtFinalIRR = (clsIRRChartCalculation.showVariableIRR_Final(dtVScheme, dtIRR, Convert.ToInt32(EMI), Convert.ToDateTime(txtFrstPDCdt.Text), Convert.ToDateTime(txtDateRq.Text), Convert.ToDateTime(txtAgreemntDate.Text), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;

    //        //Final 
    //        dtFinalIRR = (clsIRRChartCalculation.showVariableIRR_Final(dtVScheme, dtIRR, Convert.ToInt32(EMI), strTenure, hdntxtAdEmi.Value.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;

    //        //strRetrieve = (clsIRRChartCalculation.showVariableIRR_Final(dtVScheme, dtIRR, Convert.ToInt32(EMI), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as string;

    //    }
    //    else if (hdnEMItype.Value.Trim() == "Flat")
    //    {
    //        //dtFinalIRR = (clsIRRChartCalculation.showIRR(dtIRR, Convert.ToInt32(EMI), Convert.ToDateTime(txtFrstPDCdt.Text), Convert.ToDateTime(txtDateRq.Text), Convert.ToDateTime(txtAgreemntDate.Text), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;

    //        //Final - dtFinalIRR
    //        try
    //        {
    //            dtFinalIRR = (clsIRRChartCalculation.showIRR_Final(dtIRR, Convert.ToInt32(EMI), strTenure, hdntxtAdEmi.Value.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //        //dtFinalIRR = (clsIRRChartCalculation.showIRR_Final(dtIRR, Convert.ToInt32(EMI), DateTime.Parse(txtFrstPDCdt.Text), DateTime.Parse(txtDateRq.Text), DateTime.Parse(txtAgreemntDate.Text), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as DataTable;
    //        //strRetrieve = (clsIRRChartCalculation.showIRR_Final(dtIRR, Convert.ToInt32(EMI), DateTime.Parse(txtFrstPDCdt.Text), DateTime.Parse(txtDateRq.Text), DateTime.Parse(txtAgreemntDate.Text), strTenure, txtAdEmi.Text.ToString(), txtNoofdaysDiff.Text.ToString(), ddlBrknPrdIntrst.SelectedItem.Text.ToString())) as string;

    //    }

    //    //txtIRR.Text = strRetrieve;

    //    int x = 0;
    //    for (int r = 0; r <= dtFinalIRR.Rows.Count - 1; r++)
    //    {
    //        gdvIIRChart.SelectedIndex = x;
    //        Label lblsrNo = (Label)gdvIIRChart.SelectedRow.FindControl("lblsrNo");
    //        Label lblDate = (Label)gdvIIRChart.SelectedRow.FindControl("lblDate");
    //        Label lblDays = (Label)gdvIIRChart.SelectedRow.FindControl("lblDays");
    //        Label lblEMI = (Label)gdvIIRChart.SelectedRow.FindControl("lblEMI");
    //        Label lblprinciple = (Label)gdvIIRChart.SelectedRow.FindControl("lblprinciple");
    //        Label lblInterest = (Label)gdvIIRChart.SelectedRow.FindControl("lblInterest");
    //        Label lblOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblOSBal");
    //        Label lblfinalOSBal = (Label)gdvIIRChart.SelectedRow.FindControl("lblfinalOSBal");

    //        if (r == 0)
    //        {
    //            txtIRR.Text = dtFinalIRR.Rows[r][7].ToString();
    //            txtDiffIntrst.Text = dtFinalIRR.Rows[r][5].ToString();
    //        }
    //        else
    //        {
    //            lblEMI.Text = dtFinalIRR.Rows[r][3].ToString();
    //            lblprinciple.Text = dtFinalIRR.Rows[r][4].ToString();
    //            lblInterest.Text = dtFinalIRR.Rows[r][5].ToString();
    //            lblOSBal.Text = dtFinalIRR.Rows[r][6].ToString();
    //            lblfinalOSBal.Text = dtFinalIRR.Rows[r][7].ToString();
    //        }

    //        x += 1;
    //    }
    //}

    //protected void AddIIRChart(int i)
    //{
    //    DataRow dr = null;
    //    if (iircount == 0)
    //    {
    //        dt1 = new DataTable();
    //        dt1.Columns.Add("SrNo");
    //        dt1.Columns.Add("Date");
    //        dt1.Columns.Add("Days");
    //        dt1.Columns.Add("EMI");
    //        dt1.Columns.Add("Principle");
    //        dt1.Columns.Add("Interst");
    //        dt1.Columns.Add("OSBal");
    //        dt1.Columns.Add("finalOSBal");
    //        iircount = 1;
    //    }

    //    dr = dt1.NewRow();

    //    dr["SrNo"] = i.ToString();
    //    dr["Date"] = "";
    //    dr["Days"] = "";
    //    dr["EMI"] = "";
    //    dr["Principle"] = "";
    //    dr["Interst"] = "";
    //    dr["OSBal"] = "";
    //    dr["finalOSBal"] = "";

    //    dt1.Rows.Add(dr);
    //    gdvIIRChart.DataSource = dt1;
    //    gdvIIRChart.DataBind();


    //}

    //protected string FormatAddressSuffix(int number)
    //{
    //    string AddressCountSuffix = string.Empty;
    //    switch (number)
    //    {
    //        case 1:
    //        case 21:
    //        case 31:
    //            {
    //                AddressCountSuffix = "st";
    //                break;
    //            }
    //        case 2:
    //        case 22:
    //            {
    //                AddressCountSuffix = "nd";
    //                break;
    //            }
    //        case 3:
    //        case 23:
    //            {
    //                AddressCountSuffix = "rd";
    //                break;
    //            }
    //        default:
    //            {
    //                AddressCountSuffix = "th";
    //                break;
    //            }
    //    }

    //    return AddressCountSuffix;
    //}

    //#endregion
    //#endregion



}