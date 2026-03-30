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

public partial class GLSanctionDisburseDetails_EditDetails : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strRefType = "AF";
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = string.Empty;
    string GoldNo = string.Empty;
    string UserName = string.Empty;
    string Password = string.Empty;
    string GoldLoanNo = string.Empty;
    string strGID = string.Empty;
    int SanctionLoginID = 0;
    int FYearID = 0;
    int branchId = 0;
    string LogInTime = string.Empty;
    bool datasaved = false;

    DateTime LogInTime1;
    int UserID = 0;
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
    #endregion

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();

                //getting FYear ID
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID); ;
                }

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

                //BindDGVDetails
                BindDGVDetails();

                //binding GridView
                BindGoldItemDetails();

                //binding GridView Charges Details
                BindChargesDetails();

                //Getting Ref No
                GetRefNum();

                BindDDLSearchBy();

                //added on onblur event attribute
                ddlSchemeName.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlSchemeName, ""));
                txtLoanTenure.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtLoanTenure, ""));
                ddlCashAccount.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlCashAccount, ""));
                txtTotalGrossWeight.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtTotalGrossWeight, ""));
                txtNetAmountSanctioned.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtNetAmountSanctioned, ""));

                //Making readonly
                txtNetWeight.Attributes.Add("readonly", "readonly");
                txtTotalGrossWeight.Attributes.Add("readonly", "readonly");
                txtNetAmount.Attributes.Add("readonly", "readonly");
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

    #region [Bind GridView DGVDetails]
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_GoldValueDetails.ID, tbl_GLSanctionDisburse_GoldValueDetails.SDID, " +
                                "tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo, " +
                                "IssueDate=convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103), " +
                                "(AppFName+' '+AppMName+' '+AppLName) as 'ApplicantName', " +
                                "tbl_GLKYC_ApplicantDetails.PANNo, NetLoanAmtSanctioned=convert(varchar,tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned), " +
                                "TotalGrossWeight=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight), " +
                                "TotalNetWeight=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight), " +
                                "GoldNetValue=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue) " +
                       "FROM tbl_GLSanctionDisburse_GoldValueDetails " +
                       "INNER JOIN tbl_GLSanctionDisburse_BasicDetails " +
                                "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                       "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                "ON tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                       "WHERE  tbl_GLSanctionDisburse_BasicDetails.FYID='" + txtFYID.Text + "' " +
                       "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + txtBranchID.Text + "'";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);
            dgvDetails.DataSource = dt;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView DGVDetails]

    #region [Bind GridView Gold Item Details]
    protected void BindGoldItemDetails()
    {
        try
        {


            strQuery = "select tbl_GLSanctionDisburse_GoldItemDetails.GID, tbl_GLSanctionDisburse_GoldItemDetails.SDID, tblItemMaster.ItemName, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.GrossWeight, tbl_GLSanctionDisburse_GoldItemDetails.Image, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageUrl', tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageName', tbl_GLSanctionDisburse_GoldItemDetails.ItemID, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.Quantity " +
                       "from tbl_GLSanctionDisburse_GoldItemDetails " +
                       "INNER JOIN tblItemMaster " +
                       "ON tbl_GLSanctionDisburse_GoldItemDetails.ItemID=tblItemMaster.ItemID " +
                        "WHERE  GoldLoanNo='" + GoldLoanNo + "'  ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgvGoldItemDetails.DataSource = dt;
                dgvGoldItemDetails.DataBind();
            }
            else
            {
                dt = new DataTable();

                dt.Columns.Add("GID", typeof(String));
                dt.Columns.Add("SDID", typeof(String));
                dt.Columns.Add("ItemID", typeof(String));
                dt.Columns.Add("ItemName", typeof(String));
                dt.Columns.Add("GrossWeight", typeof(String));
                dt.Columns.Add("ImageUrl", typeof(String));
                dt.Columns.Add("PhotoPath", typeof(String));
                dt.Columns.Add("ImageName", typeof(String));
                dt.Columns.Add("Quantity", typeof(String));

                ShowNoResultFound(dt, dgvGoldItemDetails);
            }
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
            strQuery = "SELECT tbl_GLSanctionDisburse_ChargesDetails.CHID, tbl_GLSanctionDisburse_ChargesDetails.SDID, tbl_GLChargeMaster_BasicInfo.ChargeName, " +
                                "tbl_GLSanctionDisburse_ChargesDetails.ID, tbl_GLSanctionDisburse_ChargesDetails.CID,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.LoanAmtFrom, tbl_GLSanctionDisburse_ChargesDetails.LoanAmtTo,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.Charges, tbl_GLSanctionDisburse_ChargesDetails.ChargeType,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.AccountID, tblAccountmaster.Name, " +
                                "tbl_GLSanctionDisburse_ChargesDetails.ChargeAmount  " +
                       "FROM tbl_GLSanctionDisburse_ChargesDetails " +
                       "INNER JOIN tbl_GLChargeMaster_BasicInfo " +
                                "ON tbl_GLSanctionDisburse_ChargesDetails.CID=tbl_GLChargeMaster_BasicInfo.CID " +
                        "INNER JOIN tblAccountmaster " +
                                "ON tbl_GLSanctionDisburse_ChargesDetails.AccountID=tblAccountmaster.AccountID " +
                        "WHERE tbl_GLSanctionDisburse_ChargesDetails.GoldLoanNo='" + GoldLoanNo.Trim() + "'  ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgvChargesDetails.DataSource = dt;
                dgvChargesDetails.DataBind();
            }
            else
            {
                dt = new DataTable();

                dt.Columns.Add("CHID", typeof(String));
                dt.Columns.Add("SDID", typeof(String));
                dt.Columns.Add("ID", typeof(String));
                dt.Columns.Add("CID", typeof(String));
                dt.Columns.Add("ChargeName", typeof(String));
                dt.Columns.Add("LoanAmtFrom", typeof(String));
                dt.Columns.Add("LoanAmtTo", typeof(String));
                dt.Columns.Add("Charges", typeof(String));
                dt.Columns.Add("ChargeType", typeof(String));
                dt.Columns.Add("AccountID", typeof(String));
                dt.Columns.Add("Name", typeof(String));
                dt.Columns.Add("ChargeAmount", typeof(String));

                ShowNoResultFound(dt, dgvChargesDetails);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView Charges Details]

    #region [Fill Details of Gold Loan No]
    protected void FillData()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            RefType = txtGoldLoanNo.Text.Trim();
            RefID = ddlRefID.SelectedValue.ToString().Trim();
            RefNo = ddlRefNo.SelectedValue.ToString().Trim();
            GoldNo = (RefType) + '/' + (RefNo) + '/' + (RefID);

            strQuery = "SELECT (AppFName+' '+AppMName+' '+AppLName) as 'CustomerName', LoanType, Gender, upper(PANNo)PANNo, BirthDate, " +
                                "MaritalStatus, Age, RoomBlockNo, BldgHouseName, BldgPlotNo, Road, Landmark, Area, CityName, " + 
                                "StateName, Pincode, (NomFName+' '+NomMName+' '+NomLName) as 'Nominee', NomRelation, " +
                                "tbl_GLSanctionDisburse_BasicDetails.IssueDate, tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight, " +
                                "tbl_GLSanctionDisburse_GoldValueDetails.Deduction, tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight, " +
                                "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue, tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned, " +
                                "tbl_GLSanctionDisburse_BasicDetails.BCPID, " +
                                "tbl_GLSanctionDisburse_BasicDetails.BankCashAccID, tbl_GLKYC_BasicDetails.ExistingPLCaseNo, " +
                                "tbl_GLSanctionDisburse_BasicDetails.CheqNEFTDD, tbl_GLSanctionDisburse_BasicDetails.CheqNEFTDDNo, " +
                                "tbl_GLSanctionDisburse_BasicDetails.CheqNEFTDDDate " +
                        "FROM tbl_GLKYC_BasicDetails " +
                        "INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                        "INNER JOIN tbl_GLKYC_AddressDetails " +
                                "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                        "INNER JOIN tblStateMaster " +
                                "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                        "INNER JOIN tblCityMaster " +
                                "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                        "INNER JOIN tblZonemaster " +
                                "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                        "INNER JOIN tblAreaMaster " +
                                "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                        "INNER JOIN tbl_GLSanctionDisburse_BasicDetails " +
                                "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                        "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                        "WHERE  tbl_GLKYC_BasicDetails.GoldLoanNo='" + GoldNo + "'";

            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                txtLoantype.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                txtGender.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                txtPanNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                txtBirthDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
                txtMaritalStatus.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                txtAge.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                string RoomBlockNo = Convert.ToString(ds.Tables[0].Rows[0][7]);
                string BldgHouseName = Convert.ToString(ds.Tables[0].Rows[0][8]);
                string BldgPlotNo = Convert.ToString(ds.Tables[0].Rows[0][9]);
                string Road = Convert.ToString(ds.Tables[0].Rows[0][10]);
                string Landmark = Convert.ToString(ds.Tables[0].Rows[0][11]);
                string Area = Convert.ToString(ds.Tables[0].Rows[0][12]);
                string CityName = Convert.ToString(ds.Tables[0].Rows[0][13]);
                string StateName = Convert.ToString(ds.Tables[0].Rows[0][14]);
                string Pincode = Convert.ToString(ds.Tables[0].Rows[0][15]);

                string Address = string.Empty;
                if (RoomBlockNo.Trim() != "")
                {
                    Address = "Room/Block No." + RoomBlockNo.Trim();
                }
                else
                {
                    Address = "";
                }
                if (BldgHouseName.Trim() != "")
                {
                    Address += ", " + BldgHouseName.Trim();
                }

                if (BldgPlotNo.Trim() != "")
                {
                    Address += ", " + "Bldg/Plot No." + BldgPlotNo.Trim();
                }
                if (Road.Trim() != "")
                {
                    Address += ", " + Road.Trim();
                }
                if (Landmark.Trim() != "")
                {
                    Address += ", " + Landmark.Trim();
                }
                Address += ", " + Area.Trim() + ", " + CityName.Trim() + ", " + StateName.Trim() + "-" + Pincode.Trim();
                txtAddress.Text = Convert.ToString(Address);
                txtNominee.Text = Convert.ToString(ds.Tables[0].Rows[0][16]);
                txtNomineeRelationship.Text = Convert.ToString(ds.Tables[0].Rows[0][17]);
                txtIssueDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][18]).ToString("dd/MM/yyyy");
                txtTotalGrossWeight.Text = Convert.ToString(ds.Tables[0].Rows[0][19]);
                txtDeduction.Text = Convert.ToString(ds.Tables[0].Rows[0][20]);
                txtNetWeight.Text = Convert.ToString(ds.Tables[0].Rows[0][21]);
                txtNetAmount.Text = Convert.ToString(ds.Tables[0].Rows[0][22]);
                txtNetAmountSanctioned.Text = Convert.ToString(ds.Tables[0].Rows[0][23]);
                txtBCPID.Text = Convert.ToString(ds.Tables[0].Rows[0][24]);
                ddlCashAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][25]);
                txtPLCaseNo.Text = Convert.ToString(ds.Tables[0].Rows[0][26]);
                ddlcheqNEFTDD.Text = Convert.ToString(ds.Tables[0].Rows[0][27]);// fill Cheq/NEFT/DD NO. entry: by kishor on 8 oct 2014
                txtChequeNo.Text = Convert.ToString(ds.Tables[0].Rows[0][28]);// fill Cheq/NEFT/DD NO. entry: by kishor on 8 oct 2014

                if (Convert.ToDateTime(ds.Tables[0].Rows[0][29]) != Convert.ToDateTime("1/1/1900")) //fill Cheq/NEFT/DD Date entry: by kishor on 8 oct 2014
                {
                    txtChequeDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][29]).ToString("dd/MM/yyyy");
                }
                else
                {
                    txtChequeDate.Text = "";
                }
            }

            if (ddlCashAccount.SelectedValue != "0")
            {
                conn = new SqlConnection(strConnString);
                conn.Open();

                txtAccGPID.Value = "";
                strQuery = "select tblAccountmaster.GPID from tblAccountmaster " +
                             "where tblAccountMaster.AccountID='" + ddlCashAccount.SelectedValue + "' ";
                cmd = new SqlCommand(strQuery, conn);
                int accGPID = Convert.ToInt32(cmd.ExecuteScalar());
                txtAccGPID.Value = Convert.ToString(accGPID);

                if (accGPID == 70)
                {
                    txtChequeNo.Text = "";
                    txtChequeDate.Text = "";
                    txtChequeNo.ReadOnly = true;
                    txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                    txtChequeDate.Enabled = false;
                    ddlcheqNEFTDD.Enabled = false;
                    ddlcheqNEFTDD.BackColor = System.Drawing.Color.Gainsboro;
                }
                else
                {
                    txtChequeNo.ReadOnly = false;
                    txtChequeNo.BackColor = System.Drawing.Color.White;
                    txtChequeDate.Enabled = true;
                    txtChequeNo.Focus();
                    ddlcheqNEFTDD.Enabled = true;
                    ddlcheqNEFTDD.BackColor = System.Drawing.Color.White;
                }
            }
            else
            {
                txtAccGPID.Value = "";
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
                txtChequeNo.ReadOnly = true;
                txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                txtChequeDate.Enabled = false;
            }

            // Gold Item Details
            strQuery = "SELECT tbl_GLSanctionDisburse_GoldItemDetails.GID, tbl_GLSanctionDisburse_GoldItemDetails.SDID, " + 
                                "tblItemMaster.ItemName, tbl_GLSanctionDisburse_GoldItemDetails.GrossWeight, " + 
                                "tbl_GLSanctionDisburse_GoldItemDetails.Image, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageUrl', " + 
                                "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath, " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageName', " +
                                "tbl_GLSanctionDisburse_GoldItemDetails.ItemID, tbl_GLSanctionDisburse_GoldItemDetails.Quantity " +
                       "FROM tbl_GLSanctionDisburse_GoldItemDetails " +
                       "INNER JOIN tblItemMaster " +
                                "ON tbl_GLSanctionDisburse_GoldItemDetails.ItemID=tblItemMaster.ItemID " +
                       "WHERE  GoldLoanNo='" + GoldNo + "'  ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                Session["dtGoldItemDetails"] = dt;
                dgvGoldItemDetails.DataSource = dt;
                dgvGoldItemDetails.DataBind();
            }
            else
            {
                dt = new DataTable();

                dt.Columns.Add("GID", typeof(String));
                dt.Columns.Add("SDID", typeof(String));
                dt.Columns.Add("ItemID", typeof(String));
                dt.Columns.Add("ItemName", typeof(String));
                dt.Columns.Add("GrossWeight", typeof(String));
                dt.Columns.Add("Quantity", typeof(String));
                dt.Columns.Add("ImageUrl", typeof(String));
                dt.Columns.Add("PhotoPath", typeof(String));
                dt.Columns.Add("ImageName", typeof(String));

                ShowNoResultFound(dt, dgvGoldItemDetails);
            }

            //Fill Scheme Name
            FillSchemeName();

            //Scheme Details
            strQuery = "SELECT DISTINCT SDetailID, ID, SchemeName, SchemeType, LTV, MinTenure, MaxTenure, InterestRate, " +
                                "LoanTenure, EMI, DueDate " +
                        "FROM tbl_GLSanctionDisburse_SchemeDetails " +
                         "WHERE  tbl_GLSanctionDisburse_SchemeDetails.GoldLoanNo='" + GoldNo + "'";

            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                ddlSchemeName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][1]);
                txtLoanTenure.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                txtDueDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][10]).ToString("dd/MM/yyyy");
            }

            //Get Scheme Details
            GetSchemeDetails();
            //Clear Fields As Per Scheme
            ClearFieldsAsPerSchemeOnFill();
            //Validate Loan Tenure
            int validcount = ValidateLoanTenure();
            //Calulating Max Loan Amount
            CalculateMaxLoanAmount();
            //Calculate EMI
            if (txtSchemeType.Text.Trim() == "MI")
            {
                double LoanAmount = 0;
                double InterestRate = 0;
                double LoanTenure = 0;
                if (txtNetAmountSanctioned.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                }
                else if (txtNetAmount.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                }
                if (txtInterestRate.Text.Trim() != "")
                {
                    InterestRate = Convert.ToDouble(txtInterestRate.Text);
                }
                if (txtLoanTenure.Text.Trim() != "")
                {
                    LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                }
                CalculateEMI(LoanAmount, InterestRate, LoanTenure);
            }
            else
            {
                txtEMI.Text = "";
            }

            // Charges Details
            strQuery = "SELECT tbl_GLSanctionDisburse_ChargesDetails.CHID, tbl_GLSanctionDisburse_ChargesDetails.SDID, tbl_GLChargeMaster_BasicInfo.ChargeName, " +
                                "tbl_GLSanctionDisburse_ChargesDetails.ID, tbl_GLSanctionDisburse_ChargesDetails.CID,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.LoanAmtFrom, tbl_GLSanctionDisburse_ChargesDetails.LoanAmtTo,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.Charges, tbl_GLSanctionDisburse_ChargesDetails.ChargeType,  " +
                                "tbl_GLSanctionDisburse_ChargesDetails.AccountID, tblAccountmaster.Name, " +
                                "tbl_GLSanctionDisburse_ChargesDetails.ChargeAmount  " +
                       "FROM tbl_GLSanctionDisburse_ChargesDetails " +
                       "INNER JOIN tbl_GLChargeMaster_BasicInfo " +
                                "ON tbl_GLSanctionDisburse_ChargesDetails.CID=tbl_GLChargeMaster_BasicInfo.CID " +
                        "INNER JOIN tblAccountmaster " +
                                "ON tbl_GLSanctionDisburse_ChargesDetails.AccountID=tblAccountmaster.AccountID " +
                        "WHERE tbl_GLSanctionDisburse_ChargesDetails.GoldLoanNo='" + GoldNo.Trim() + "'  ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                Session["dtChargesDetails"] = dt;
                dgvChargesDetails.DataSource = dt;
                dgvChargesDetails.DataBind();
            }
            else
            {
                dt = new DataTable();

                dt.Columns.Add("CHID", typeof(String));
                dt.Columns.Add("SDID", typeof(String));
                dt.Columns.Add("ID", typeof(String));
                dt.Columns.Add("CID", typeof(String));
                dt.Columns.Add("ChargeName", typeof(String));
                dt.Columns.Add("LoanAmtFrom", typeof(String));
                dt.Columns.Add("LoanAmtTo", typeof(String));
                dt.Columns.Add("Charges", typeof(String));
                dt.Columns.Add("ChargeType", typeof(String));
                dt.Columns.Add("AccountID", typeof(String));
                dt.Columns.Add("Name", typeof(String));
                dt.Columns.Add("ChargeAmount", typeof(String));

                ShowNoResultFound(dt, dgvChargesDetails);
            }

            //CIBIL Score and Proof of Ownership
            strQuery = "SELECT DISTINCT ID, CIBILScore, OwnershipProofImagePath " +
                                "FROM tbl_GLSanctionDisburse_OtherDetails " +
                                "WHERE  tbl_GLSanctionDisburse_OtherDetails.GoldLoanNo='" + GoldNo + "'";

            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtCIBILScore.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                txtProofOfOwnershipPath.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                imgProofOfOwnership.ImageUrl = txtProofOfOwnershipPath.Text;
                imgProofOfOwnership.Visible = true;
            }

            btnSave.Text = "Update";
            btnReset.Text = "Cancel";

            conn = new SqlConnection(strConnString);
            conn.Open();
            //checking whether Gold Loan A/C is processed to next stage (EMI Interest JV)
            strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, conn);
            int existcount = Convert.ToInt32(cmd.ExecuteScalar());

            if (existcount > 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since it is being processed to Interest JV.');", true);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillDataAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion  [Fill Details of Gold Loan No]

    #region [dgvGoldItem_RowCommand]
    protected void dgvGoldItemDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "DeleteRecord")
            {
                GridView _gridView = (GridView)sender;
                int index = Convert.ToInt32(e.CommandArgument);
                //string GID = Convert.ToString(index).ToString();
                string GID = Convert.ToString(_gridView.DataKeys[index].Value.ToString());

                if (strGID.Length == 0)
                {
                    strGID = GID;
                }
                else
                {
                    strGID = strGID + "," + GID;
                }

                if (txtGID.Text.Trim() != "")
                {
                    txtGID.Text += "," + strGID;
                }
                else
                {
                    txtGID.Text = strGID;
                }

                GridViewRow row = dgvGoldItemDetails.Rows[index];
                DataTable dtGoldItemDetails = new DataTable();
                dtGoldItemDetails = (DataTable)Session["dtGoldItemDetails"];

                if ((dgvGoldItemDetails.Rows.Count > 0) && (dgvGoldItemDetails.Rows.Count != 1))  //Checks whether list contains items
                {
                    if (dtGoldItemDetails != null)
                    {
                        dtGoldItemDetails.Rows.RemoveAt(index);
                        dgvGoldItemDetails.DataSource = dtGoldItemDetails;
                        dgvGoldItemDetails.DataBind();
                    }
                }
                else
                {
                    dtGoldItemDetails.Rows.RemoveAt(index);
                    ShowNoResultFound(dtGoldItemDetails, dgvGoldItemDetails);
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDocument_RowCommand]

    #region [dgvGoldItemDetails_DataBound]
    protected void dgvGoldItemDetails_DataBound(object sender, EventArgs e)
    {
        try
        {
            double total = 0;
            foreach (GridViewRow row in dgvGoldItemDetails.Rows)
            {
                var numberLabel = row.FindControl("lblGrossWeight") as Label;
                double number;
                if (double.TryParse(numberLabel.Text, out number))
                {
                    total += number;
                }
            }
            total = Math.Round(total, 3);
            txtTotalGrossWeight.Text = Convert.ToString(total);

            //calculation of Net Weight
            double TotalGrossWeight = 0;
            double Deduction = 0;
            double defaultDeductionInGrossWeight = 0;
            double TotalNetWeight = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();

            //fetching Default Deduction In Gross Weight
            strQuery = "SELECT DeductionInGrossWeight FROM tblLoanParameterSetting";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    defaultDeductionInGrossWeight = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                }
                else
                {
                    defaultDeductionInGrossWeight = 0;
                }
            }
            else
            {
                defaultDeductionInGrossWeight = 0;
            }

            if (Convert.ToString(txtTotalGrossWeight.Text) != "")
            {
                TotalGrossWeight = Convert.ToDouble(txtTotalGrossWeight.Text);
                if (Convert.ToString(txtDeduction.Text) != "")
                {
                    Deduction = Convert.ToDouble(txtDeduction.Text);
                }
                else
                {
                    Deduction = 0;
                }
                TotalNetWeight = TotalGrossWeight - Deduction - defaultDeductionInGrossWeight;
                TotalNetWeight = Math.Round(TotalNetWeight, 3);
                if (TotalNetWeight < 0)
                {
                    txtNetWeight.Text = "0";
                }
                else
                {
                    txtNetWeight.Text = Convert.ToString(TotalNetWeight);
                }
            }

            //Calulating Max Loan Amount
            CalculateMaxLoanAmount();
            //Calculate EMI
            if (txtSchemeType.Text.Trim() == "MI")
            {
                double LoanAmount = 0;
                double InterestRate = 0;
                double LoanTenure = 0;
                if (txtNetAmountSanctioned.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                }
                else if (txtNetAmount.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                }
                if (txtInterestRate.Text.Trim() != "")
                {
                    InterestRate = Convert.ToDouble(txtInterestRate.Text);
                }
                if (txtLoanTenure.Text.Trim() != "")
                {
                    LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                }
                CalculateEMI(LoanAmount, InterestRate, LoanTenure);
            }
            else
            {
                txtEMI.Text = "";
            }

            //Checking Amount is excess
            if (Convert.ToDouble(txtNetAmount.Text) > 0)
            {
                bool IsExcess = IsExcessAmount();
                if (IsExcess == false)
                {
                    lblMessageText.Text = "Login for Excess amount.";
                }
                else
                {
                    lblMessageText.Text = "";
                    pnlLogin.Visible = false;
                }
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [dgvGoldItemDetails_ServerValidate]
    protected void dgvGoldItemDetails_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (dgvGoldItemDetails != null && dgvGoldItemDetails.Rows.Count > 0)
            {
                int Count = 0;
                foreach (GridViewRow row in dgvGoldItemDetails.Rows)
                {
                    Label lblGoldItemName = (Label)row.FindControl("lblGoldItemName");

                    if (lblGoldItemName != null && lblGoldItemName.Text != "")
                    {
                        Count = Count + 1;

                    }
                }
                if (Count >= 1)
                {
                    e.IsValid = true;
                }
                else
                {
                    e.IsValid = false;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ItemValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [dgvGoldItemDetails_ServerValidate]

    #region [Upload Gold Item Details]
    protected void BtnUpload_Click(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                FileUpload fuploadFile = (FileUpload)dgvGoldItemDetails.FooterRow.FindControl("FileUpload1");
                DropDownList ddlItemName = (DropDownList)dgvGoldItemDetails.FooterRow.FindControl("ddlGoldItemName");
                TextBox grossweight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtGrossWeight1");
                TextBox quantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtQuantity");
                string fileName = string.Empty;
                int MaxSizeAllowed = 1073741824; // 1GB...

                if (fuploadFile.HasFile)
                {
                    fileName = fuploadFile.FileName;
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
                    else if (fuploadFile.PostedFile.ContentLength > MaxSizeAllowed)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                    }
                    else
                    {
                        DataTable dtCurrentTable = new DataTable();
                        DataRow dr = null;

                        dtCurrentTable.Columns.Add("GID", typeof(String));
                        dtCurrentTable.Columns.Add("SDID", typeof(String));
                        dtCurrentTable.Columns.Add("ItemID", typeof(String));
                        dtCurrentTable.Columns.Add("ItemName", typeof(String));
                        dtCurrentTable.Columns.Add("GrossWeight", typeof(String));
                        dtCurrentTable.Columns.Add("Quantity", typeof(String));
                        dtCurrentTable.Columns.Add("ImageUrl", typeof(String));
                        dtCurrentTable.Columns.Add("PhotoPath", typeof(String));
                        dtCurrentTable.Columns.Add("ImageName", typeof(String));

                        foreach (GridViewRow row in dgvGoldItemDetails.Rows)
                        {

                            Label lblGID = (Label)row.Cells[1].FindControl("lblGID");
                            Label lblSDID = (Label)row.Cells[1].FindControl("lblSDID");
                            Label lblItemID = (Label)row.Cells[1].FindControl("lblItemID");
                            Label ItemName = (Label)row.Cells[1].FindControl("lblGoldItemName");
                            Label lblGrossWeight = (Label)row.Cells[1].FindControl("lblGrossWeight");
                            Label lblQuantity = (Label)row.Cells[1].FindControl("lblQuantity");
                            Label lblImagePath = (Label)row.Cells[1].FindControl("lblPath");
                            Label lblImageName = (Label)row.Cells[1].FindControl("lblImageName");

                            dr = dtCurrentTable.NewRow();

                            dr["GID"] = lblGID.Text;
                            dr["SDID"] = lblSDID.Text;
                            dr["ItemID"] = lblItemID.Text;
                            dr["ItemName"] = ItemName.Text;
                            dr["GrossWeight"] = lblGrossWeight.Text;
                            dr["Quantity"] = lblQuantity.Text;
                            dr["ImageUrl"] = lblImagePath.Text;
                            dr["PhotoPath"] = lblImagePath.Text;
                            dr["ImageName"] = lblImageName.Text;

                            if (ItemName.Text != "")
                            {
                                dtCurrentTable.Rows.Add(dr);
                            }
                        }

                        fuploadFile.SaveAs(Server.MapPath("~/GoldItemImage/" + fileName));
                        string ImagePath = "GoldItemImage/" + fileName;

                        dr = dtCurrentTable.NewRow();
                        dr["GID"] = string.Empty;
                        dr["SDID"] = string.Empty;
                        dr["ItemID"] = ddlItemName.SelectedValue;
                        dr["ItemName"] = Convert.ToString(ddlItemName.SelectedItem.Text);
                        dr["GrossWeight"] = Convert.ToString(grossweight.Text);
                        dr["Quantity"] = Convert.ToString(quantity.Text);
                        dr["ImageUrl"] = ImagePath;
                        dr["PhotoPath"] = ImagePath;
                        dr["ImageName"] = fileName;

                        dtCurrentTable.Rows.Add(dr);

                        Session["dtGoldItemDetails"] = dtCurrentTable;
                        dgvGoldItemDetails.DataSource = dtCurrentTable;
                        dgvGoldItemDetails.DataBind();
                    }
                }
                ddlItemName.Focus();
            }
            else
            {
                TextBox grossweight = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtGrossWeight1");
                TextBox quantity = (TextBox)dgvGoldItemDetails.FooterRow.FindControl("txtQuantity");

                if (grossweight.Text.Trim() == "")
                {
                    grossweight.Focus();
                }
                else if (quantity.Text.Trim() == "")
                {
                    quantity.Focus();
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region dgvGoldItemDetails_PageIndexChanging
    protected void dgvGoldItemDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvGoldItemDetails.PageIndex = e.NewPageIndex;
            BindGoldItemDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [Upload Charges Details]
    protected void BtnUploadCharges_Click(object sender, EventArgs e)
    {
        try
        {
            bool IsValidData = true;
            string strID = string.Empty;
            string strCID = string.Empty;
            string strLoanAmtFrom = string.Empty;
            string strLoanAmtTo = string.Empty;
            string strCharges = string.Empty;
            string strChargeType = string.Empty;
            string strChargeAmount = string.Empty;

            if (txtNetAmountSanctioned.Text.Trim() != "")
            {
                if (Convert.ToDouble(txtNetAmountSanctioned.Text.Trim()) == 0)
                {
                    IsValidData = false;
                }
            }
            else if (txtNetAmountSanctioned.Text.Trim() == "")
            {
                IsValidData = false;
            }

            if (IsValidData == false)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Sanctioned Loan Amount.');", true);
            }

            if (Page.IsValid && IsValidData)
            {
                DropDownList ddlChargesName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlChargesName");
                DropDownList ddlAccountName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlAccountName");
                //retrieving Charges Details
                try
                {
                    conn = new SqlConnection(strConnString);
                    conn.Open();

                    strQuery = "SELECT ID, CID, LoanAmtFrom, LoanAmtTo, Charges, ChargeType " +
                                "FROM tbl_GLChargeMaster_Details " +
                                "WHERE CID=" + ddlChargesName.SelectedValue + " " +
                                "AND '" + txtNetAmountSanctioned.Text + "' BETWEEN LoanAmtFrom AND LoanAmtTo";

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
                        IsValidData = false;
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No charges present for the entered Sanctioned Loan Amount. Add Details for the selected Charge Name in Charges Master.');", true);
                    }
                }
                catch (Exception ex)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "UploadChargesAlert", "alert('" + ex.Message + "');", true);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }

                if (IsValidData)
                {
                    DataTable dtCurrentTable = new DataTable();
                    DataRow dr = null;

                    dtCurrentTable.Columns.Add("CHID", typeof(String));
                    dtCurrentTable.Columns.Add("SDID", typeof(String));
                    dtCurrentTable.Columns.Add("ID", typeof(String));
                    dtCurrentTable.Columns.Add("CID", typeof(String));
                    dtCurrentTable.Columns.Add("ChargeName", typeof(String));
                    dtCurrentTable.Columns.Add("Charges", typeof(String));
                    dtCurrentTable.Columns.Add("LoanAmtFrom", typeof(String));
                    dtCurrentTable.Columns.Add("LoanAmtTo", typeof(String));
                    dtCurrentTable.Columns.Add("ChargeType", typeof(String));
                    dtCurrentTable.Columns.Add("Name", typeof(String));
                    dtCurrentTable.Columns.Add("AccountID", typeof(String));
                    dtCurrentTable.Columns.Add("ChargeAmount", typeof(String));

                    foreach (GridViewRow row in dgvChargesDetails.Rows)
                    {
                        Label lblCHID = (Label)row.Cells[0].FindControl("lblCHID");
                        Label lblSDID = (Label)row.Cells[1].FindControl("lblSDID");
                        Label lblID = (Label)row.Cells[2].FindControl("lblID");
                        Label lblCID = (Label)row.Cells[3].FindControl("lblCID");
                        Label lblChargesName = (Label)row.Cells[4].FindControl("lblChargesName");
                        Label lblCharges = (Label)row.Cells[5].FindControl("lblCharges");
                        Label lblLoanAmtFrom = (Label)row.Cells[6].FindControl("lblLoanAmtFrom");
                        Label lblLoanAmtTo = (Label)row.Cells[4].FindControl("lblLoanAmtTo");
                        Label lblChargeType = (Label)row.Cells[5].FindControl("lblChargeType");
                        Label lblAccountName = (Label)row.Cells[6].FindControl("lblAccountName");
                        Label lblAccountID = (Label)row.Cells[5].FindControl("lblAccountID");
                        Label lblAmount = (Label)row.Cells[5].FindControl("lblAmount");

                        dr = dtCurrentTable.NewRow();

                        dr["CHID"] = lblCHID.Text;
                        dr["SDID"] = lblSDID.Text;
                        dr["ID"] = lblID.Text;
                        dr["CID"] = lblCID.Text;
                        dr["ChargeName"] = lblChargesName.Text;
                        dr["Charges"] = lblCharges.Text;
                        dr["LoanAmtFrom"] = lblLoanAmtFrom.Text;
                        dr["LoanAmtTo"] = lblLoanAmtTo.Text;
                        dr["ChargeType"] = lblChargeType.Text;
                        dr["Name"] = lblAccountName.Text;
                        dr["AccountID"] = lblAccountID.Text;
                        dr["ChargeAmount"] = lblAmount.Text;

                        if (lblChargesName.Text != "")
                        {
                            dtCurrentTable.Rows.Add(dr);
                        }

                        double ChargeAmount = 0;
                        if (strChargeType == "Amount")
                        {
                            strChargeAmount = strCharges;
                        }
                        else
                        {
                            double SanctionLoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                            double ChargesPercent = Convert.ToDouble(strCharges);
                            ChargeAmount = SanctionLoanAmount * ((ChargesPercent / 12) / 100);
                            decimal dChargeAmount = Decimal.Round(Convert.ToDecimal(ChargeAmount), 2);
                            strChargeAmount = Convert.ToString(dChargeAmount);
                        }
                        dr = dtCurrentTable.NewRow();

                        dr["CHID"] = string.Empty;
                        dr["SDID"] = string.Empty;
                        dr["ID"] = strID;
                        dr["CID"] = ddlChargesName.SelectedValue;
                        dr["ChargeName"] = ddlChargesName.SelectedItem.Text;
                        dr["Charges"] = strCharges;
                        dr["LoanAmtFrom"] = strLoanAmtFrom;
                        dr["LoanAmtTo"] = strLoanAmtTo;
                        dr["ChargeType"] = strChargeType;
                        dr["Name"] = Convert.ToString(ddlAccountName.SelectedItem.Text);
                        dr["AccountID"] = ddlAccountName.SelectedValue;
                        dr["ChargeAmount"] = strChargeAmount;
                        dtCurrentTable.Rows.Add(dr);

                        Session["dtChargesDetails"] = dtCurrentTable;
                        dgvChargesDetails.DataSource = dtCurrentTable;
                        dgvChargesDetails.DataBind();
                    }

                    ddlChargesName.Focus();
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UploadChargesAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Charges Details]

    #region [dgvChargesDetails_PageIndexChanging]
    protected void dgvChargesDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvChargesDetails.PageIndex = e.NewPageIndex;
            BindChargesDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvChargesDetails_PageIndexChanging]

    #region [dgvChargesDetails_RowCommand]
    protected void dgvChargesDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "DeleteRecord")
            {
                GridView _gridView = (GridView)sender;
                int index = Convert.ToInt32(e.CommandArgument);
                string DID = Convert.ToString(_gridView.DataKeys[index].Value.ToString());

                GridViewRow row = dgvChargesDetails.Rows[index];
                DataTable dtChargesDetails = new DataTable();
                dtChargesDetails = (DataTable)Session["dtChargesDetails"];

                if ((dgvChargesDetails.Rows.Count > 0) && (dgvChargesDetails.Rows.Count != 1))  //Checks whether list contains items
                {
                    if (dtChargesDetails != null)
                    {
                        dtChargesDetails.Rows.RemoveAt(index);
                        dgvChargesDetails.DataSource = dtChargesDetails;
                        dgvChargesDetails.DataBind();
                    }
                }
                else
                {
                    dtChargesDetails.Rows.RemoveAt(index);
                    ShowNoResultFound(dtChargesDetails, dgvChargesDetails);
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvChargesDetails_RowCommand]

    #region dgvDetails_RowCommand
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
                GoldLoanNo = (_gridView.DataKeys[_selectedIndex].Value.ToString());

                #region [Delete Record]
                if (_commandName == "DeleteRecord")
                {
                    bool datasaved = true;
                    int existcount = 0;
                    int QueryResult = 0;
                    string DJEReferenceNo = string.Empty;
                    int AccID = 0;
                    double DebitAmount = 0;
                    double CreditAmount = 0;
                    DateTime RefDate;

                    //deleting record from DB
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    //checking whether Gold Loan A/C is processed to next stage (EMI Interest JV)
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + GoldLoanNo + "'";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since it is being processed to Interest JV.');", true);
                    }

                    if (existcount == 0)
                    {
                        //checking whether Gold Loan A/C is present
                        strQuery = "select count(*) from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + GoldLoanNo + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            //Deletion of ledger entries and effects from Company-wise Account Closing table 

                            strQuery = "select ReferenceNo from FSystemGeneratedEntryMaster where LoginID='" + GoldLoanNo + "'";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                DJEReferenceNo = Convert.ToString(cmd.ExecuteScalar());
                            }
                            else
                            {
                                DJEReferenceNo = "";
                            }

                            // 1] Deleting effects from Company-wise Account Closing table
                            strQuery = "select AccountID, Debit, Credit, RefDate from FLedgerMaster " +
                                        "where ReferenceNo='" + DJEReferenceNo + "'";
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
                                deleteQuery = "delete from FLedgerMaster where ReferenceNo='" + DJEReferenceNo + "'";
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


                            // 3] deleting record from table TBankCash_PaymentDetails
                            if (datasaved)
                            {
                                deleteQuery = "delete from TBankCash_PaymentDetails where ReferenceNo='" + DJEReferenceNo + "'";
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

                            // 4] deleting record from table FSystemGeneratedEntryMaster
                            if (datasaved)
                            {
                                deleteQuery = "delete from FSystemGeneratedEntryMaster where ReferenceNo='" + DJEReferenceNo + "'";
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

                            // 5] deleting record from table tblAccountMaster
                            if (datasaved)
                            {
                                deleteQuery = "delete from tblAccountMaster where Alies='" + GoldLoanNo + "'";
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

                            // 6] deleting record from table tbl_GLSanctionDisburse_OtherDetails
                            if (datasaved == true)
                            {
                                //checking whether record is present
                                strQuery = "select count(*) from tbl_GLSanctionDisburse_OtherDetails where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                existcount = Convert.ToInt32(cmd.ExecuteScalar());

                                if (existcount > 0)
                                {
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_OtherDetails where GoldLoanNo='" + GoldLoanNo + "'";
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
                                else
                                {
                                    datasaved = true;
                                }
                            }

                            AccID = 0;
                            int LedgerID = 0;
                            DebitAmount = 0;
                            CreditAmount = 0;

                            //// 7] Deleting effects from Company-wise Account Closing table for Charges Details
                            //if (datasaved == true)
                            //{
                            //    strQuery = "select AccID, Debit, Credit, LedgerID from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                            //                "where GoldLoanNo='" + GoldLoanNo + "'";
                            //    cmd = new SqlCommand(strQuery, conn, transaction);
                            //    cmd.CommandType = CommandType.Text;
                            //    da = new SqlDataAdapter(cmd);
                            //    ds = new DataSet();
                            //    da.Fill(ds);

                            //    if (ds.Tables[0].Rows.Count > 0)
                            //    {
                            //        foreach (DataRow drow1 in ds.Tables[0].Rows)
                            //        {
                            //            AccID = Convert.ToInt32(drow1[0]);
                            //            DebitAmount = Convert.ToDouble(drow1[1]);
                            //            CreditAmount = Convert.ToDouble(drow1[2]);
                            //            LedgerID = Convert.ToInt32(drow1[3]);

                            //            if (datasaved)
                            //            {
                            //                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);

                            //                if (datasaved == false)
                            //                {
                            //                    break;
                            //                }

                            //                //Deleting data from table FLedgerMaster 
                            //                if (datasaved)
                            //                {
                            //                    deleteQuery = "delete from FLedgerMaster " +
                            //                                    "where LedgerID=" + LedgerID + "";

                            //                    cmd = new SqlCommand(deleteQuery, conn, transaction);
                            //                    QueryResult = cmd.ExecuteNonQuery();

                            //                    if (QueryResult > 0)
                            //                    {
                            //                        datasaved = true;
                            //                    }
                            //                    else
                            //                    {
                            //                        datasaved = false;
                            //                        break;
                            //                    }
                            //                }
                            //            }
                            //        }
                            //    }
                            //}

                            // 7] deletion of data from tbl_GLSanctionDisburse_ChargesPostingDetails
                            if (datasaved == true)
                            {
                                int excount = 0;
                                strQuery = "Select count(*) from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
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
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 8] deletion of data from tbl_GLSanctionDisburse_ChargesDetails
                            if (datasaved == true)
                            {
                                int excount = 0;
                                strQuery = "Select count(*) from tbl_GLSanctionDisburse_ChargesDetails " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
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
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_ChargesDetails " +
                                                  "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 9] deleting record from table tbl_GLSanctionDisburse_SchemeDetails
                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLSanctionDisburse_SchemeDetails where GoldLoanNo='" + GoldLoanNo + "'";
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

                            // 10] deleting record from table tbl_GLSanctionDisburse_GoldItemDetails
                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLSanctionDisburse_GoldItemDetails where GoldLoanNo='" + GoldLoanNo + "'";
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

                            // 11] deleting record from table tbl_GLSanctionDisburse_GoldValueDetails
                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLSanctionDisburse_GoldValueDetails where GoldLoanNo='" + GoldLoanNo + "'";
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

                            // 12] deleting record from table tbl_GLSanctionDisburse_BasicDetails
                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + GoldLoanNo + "'";
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

                            if (QueryResult > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                                BindDGVDetails();

                                //if the same record is deleted which is filled in the form.
                                if (txtSID.Text != "" && txtSID.Text != null)
                                {
                                    RefType = txtGoldLoanNo.Text.Trim();
                                    RefID = ddlRefID.SelectedValue.ToString().Trim();
                                    RefNo = ddlRefNo.SelectedValue.ToString().Trim();
                                    GoldNo = (RefType) + '/' + (RefNo) + '/' + (RefID);
                                    if (GoldLoanNo == Convert.ToString(GoldNo))
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

                #region [Update]
                if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", GoldLoanNo);
                    txtID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtTotalGrossWeight.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]);
                    txtDeduction.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][2]).Trim();
                    txtNetWeight.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][3]);
                    txtNetAmount.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][4]);
                    txtSID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][5]);

                    // KYC/ Basic Details
                    strQuery = "SELECT DISTINCT (AppFName+' '+AppMName+' '+AppLName) as 'CustomerName', LoanType, Gender, " +
                                        "PANNo, BirthDate, MaritalStatus, Age, RoomBlockNo, BldgHouseName, BldgPlotNo, Road, " +
                                        "Landmark, Area, CityName, StateName, Pincode, (NomFName+' '+NomMName+' '+NomLName) as 'Nominee', " +
                                        "NomRelation, tbl_GLKYC_BasicDetails.ExistingPLCaseNo " +
                                       " FROM tbl_GLKYC_BasicDetails " +
                                "INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                        "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLKYC_AddressDetails " +
                                        "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                "INNER JOIN tblStateMaster " +
                                        "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                "INNER JOIN tblCityMaster " +
                                        "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                "INNER JOIN tblZonemaster " +
                                        "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                "INNER JOIN tblAreaMaster " +
                                        "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                "INNER JOIN tbl_GLSanctionDisburse_BasicDetails " +
                                        "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                        "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                                "WHERE  tbl_GLKYC_BasicDetails.GoldLoanNo='" + GoldLoanNo + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtCustomerName.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtLoantype.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txtGender.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtPanNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        txtBirthDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
                        txtMaritalStatus.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                        txtAge.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                        string RoomBlockNo = Convert.ToString(ds.Tables[0].Rows[0][7]);
                        string BldgHouseName = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        string BldgPlotNo = Convert.ToString(ds.Tables[0].Rows[0][9]);
                        string Road = Convert.ToString(ds.Tables[0].Rows[0][10]);
                        string Landmark = Convert.ToString(ds.Tables[0].Rows[0][11]);
                        string Area = Convert.ToString(ds.Tables[0].Rows[0][12]);
                        string CityName = Convert.ToString(ds.Tables[0].Rows[0][13]);
                        string StateName = Convert.ToString(ds.Tables[0].Rows[0][14]);
                        string Pincode = Convert.ToString(ds.Tables[0].Rows[0][15]);

                        string Address = string.Empty;
                        if (RoomBlockNo.Trim() != "")
                        {
                            Address = "Room/Block No." + RoomBlockNo.Trim();
                        }
                        else
                        {
                            Address = "";
                        }
                        if (BldgHouseName.Trim() != "")
                        {
                            Address += ", " + BldgHouseName.Trim();
                        }

                        if (BldgPlotNo.Trim() != "")
                        {
                            Address += ", " + "Bldg/Plot No." + BldgPlotNo.Trim();
                        }
                        if (Road.Trim() != "")
                        {
                            Address += ", " + Road.Trim();
                        }
                        if (Landmark.Trim() != "")
                        {
                            Address += ", " + Landmark.Trim();
                        }
                        Address += ", " + Area.Trim() + ", " + CityName.Trim() + ", " + StateName.Trim() + "-" + Pincode.Trim();
                        txtAddress.Text = Convert.ToString(Address);
                        txtNominee.Text = Convert.ToString(ds.Tables[0].Rows[0][16]);
                        txtNomineeRelationship.Text = Convert.ToString(ds.Tables[0].Rows[0][17]);
                        txtPLCaseNo.Text = Convert.ToString(ds.Tables[0].Rows[0][18]);
                     }

                    strQuery = "SELECT DISTINCT RefType, RefNo, RefID, IssueDate, NetLoanAmtSanctioned, BCPID, BankCashAccID, " +
                                        "CheqNEFTDD, CheqNEFTDDNo, CheqNEFTDDDate " +
                                "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                "WHERE  tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo='" + GoldLoanNo + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ddlRefNo.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txtIssueDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][3]).ToString("dd/MM/yyyy");
                        txtNetAmountSanctioned.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
                        txtBCPID.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                        //Fill Bank/Cash Account Combo
                        FillBankCashAccount();
                       ddlCashAccount.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]);
                       ddlcheqNEFTDD.Text = Convert.ToString(ds.Tables[0].Rows[0][7]); //fill cheqNEFTDD .:kishor 8 oct 2014
                       txtChequeNo.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);//fill cheqNEFTDD no.:kishor 8 oct 2014

                       if (Convert.ToDateTime(ds.Tables[0].Rows[0][9]) != Convert.ToDateTime("1/1/1900"))
                       {
                           txtChequeDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][9]).ToString("dd/MM/yyyy");//fill cheqNEFTDD Date.:kishor 8 oct 2014
                       }
                       else
                       {
                           txtChequeDate.Text = "";
                       }
                    }

                    if (ddlCashAccount.SelectedValue != "0")
                    {
                        conn = new SqlConnection(strConnString);
                        conn.Open();

                        txtAccGPID.Value = "";
                        strQuery = "select tblAccountmaster.GPID from tblAccountmaster " +
                                     "where tblAccountMaster.AccountID='" + ddlCashAccount.SelectedValue + "' ";
                        cmd = new SqlCommand(strQuery, conn);
                        int accGPID = Convert.ToInt32(cmd.ExecuteScalar());
                        txtAccGPID.Value = Convert.ToString(accGPID);

                        if (accGPID == 70)
                        {
                            txtChequeNo.Text = "";
                            txtChequeDate.Text = "";
                            txtChequeNo.ReadOnly = true;
                            txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                            txtChequeDate.Enabled = false;
                        }
                        else
                        {
                            txtChequeNo.ReadOnly = false;
                            txtChequeNo.BackColor = System.Drawing.Color.White;
                            txtChequeDate.Enabled = true;
                            txtChequeNo.Focus();
                        }
                    }
                    else
                    {
                        txtAccGPID.Value = "";
                        txtChequeNo.Text = "";
                        txtChequeDate.Text = "";
                        txtChequeNo.ReadOnly = true;
                        txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                        txtChequeDate.Enabled = false;
                    }

                    strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.RefID  " +
                                "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                "WHERE tbl_GLSanctionDisburse_BasicDetails.RefNo='" + ddlRefNo.SelectedValue + "' " +
                                "AND tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo='" + GoldLoanNo + "'";

                    da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlRefID.DataSource = dt;
                    ddlRefID.DataTextField = "RefID";
                    ddlRefNo.DataValueField = "RefID";
                    ddlRefID.DataBind();

                    // Gold Item details
                    strQuery = "SELECT tbl_GLSanctionDisburse_GoldItemDetails.GID, tbl_GLSanctionDisburse_GoldItemDetails.SDID, " +
                                        "tblItemMaster.ItemName, tbl_GLSanctionDisburse_GoldItemDetails.GrossWeight, " +
                                        "tbl_GLSanctionDisburse_GoldItemDetails.Image, tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageUrl', " +
                                        "tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath, tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath as 'ImageName', " +
                                        "tbl_GLSanctionDisburse_GoldItemDetails.ItemID, tbl_GLSanctionDisburse_GoldItemDetails.Quantity " +
                                "FROM tbl_GLSanctionDisburse_GoldItemDetails " +
                                "INNER JOIN tblItemMaster " +
                                        "ON tbl_GLSanctionDisburse_GoldItemDetails.ItemID=tblItemMaster.ItemID " +
                                "WHERE  GoldLoanNo='" + GoldLoanNo + "'  ";

                    conn = new SqlConnection(strConnString);
                    da = new SqlDataAdapter(strQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Session["dtGoldItemDetails"] = dt;
                        dgvGoldItemDetails.DataSource = dt;
                        dgvGoldItemDetails.DataBind();
                    }
                    else
                    {
                        dt = new DataTable();

                        dt.Columns.Add("GID", typeof(String));
                        dt.Columns.Add("SDID", typeof(String));
                        dt.Columns.Add("ItemID", typeof(String));
                        dt.Columns.Add("ItemName", typeof(String));
                        dt.Columns.Add("GrossWeight", typeof(String));
                        dt.Columns.Add("Quantity", typeof(String));
                        dt.Columns.Add("ImageUrl", typeof(String));
                        dt.Columns.Add("PhotoPath", typeof(String));
                        dt.Columns.Add("ImageName", typeof(String));

                        ShowNoResultFound(dt, dgvGoldItemDetails);
                    }

                    //Fill Scheme Name
                    FillSchemeName();

                    //Scheme Details
                    strQuery = "SELECT DISTINCT SDetailID, ID, SchemeName, SchemeType, LTV, MinTenure, MaxTenure, InterestRate, " +
                                        "LoanTenure, EMI, DueDate " +
                                "FROM tbl_GLSanctionDisburse_SchemeDetails " +
                                 "WHERE  tbl_GLSanctionDisburse_SchemeDetails.GoldLoanNo='" + GoldLoanNo + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ddlSchemeName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txtLoanTenure.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        txtDueDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][10]).ToString("dd/MM/yyyy");
                    }

                    //Get Scheme Details
                    GetSchemeDetails();
                    //Clear Fields As Per Scheme
                    ClearFieldsAsPerSchemeOnFill();
                    //Validate Loan Tenure
                    int validcount = ValidateLoanTenure();
                    //Calulating Max Loan Amount
                    CalculateMaxLoanAmount();
                    //Calculate EMI
                    if (txtSchemeType.Text.Trim() == "MI")
                    {
                        double LoanAmount = 0;
                        double InterestRate = 0;
                        double LoanTenure = 0;
                        if (txtNetAmountSanctioned.Text.Trim() != "")
                        {
                            LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                        }
                        else if (txtNetAmount.Text.Trim() != "")
                        {
                            LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                        }
                        if (txtInterestRate.Text.Trim() != "")
                        {
                            InterestRate = Convert.ToDouble(txtInterestRate.Text);
                        }
                        if (txtLoanTenure.Text.Trim() != "")
                        {
                            LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                        }
                        CalculateEMI(LoanAmount, InterestRate, LoanTenure);
                    }
                    else
                    {
                        txtEMI.Text = "";
                    }

                    // Charges Details
                    strQuery = "SELECT tbl_GLSanctionDisburse_ChargesDetails.CHID, tbl_GLSanctionDisburse_ChargesDetails.SDID, tbl_GLChargeMaster_BasicInfo.ChargeName, " +
                                        "tbl_GLSanctionDisburse_ChargesDetails.ID, tbl_GLSanctionDisburse_ChargesDetails.CID,  " +
                                        "tbl_GLSanctionDisburse_ChargesDetails.LoanAmtFrom, tbl_GLSanctionDisburse_ChargesDetails.LoanAmtTo,  " +
                                        "tbl_GLSanctionDisburse_ChargesDetails.Charges, tbl_GLSanctionDisburse_ChargesDetails.ChargeType,  " +
                                        "tbl_GLSanctionDisburse_ChargesDetails.AccountID, tblAccountmaster.Name, " +
                                        "tbl_GLSanctionDisburse_ChargesDetails.ChargeAmount  " +
                               "FROM tbl_GLSanctionDisburse_ChargesDetails " +
                               "INNER JOIN tbl_GLChargeMaster_BasicInfo " +
                                        "ON tbl_GLSanctionDisburse_ChargesDetails.CID=tbl_GLChargeMaster_BasicInfo.CID " +
                                "INNER JOIN tblAccountmaster " +
                                        "ON tbl_GLSanctionDisburse_ChargesDetails.AccountID=tblAccountmaster.AccountID " +
                                "WHERE tbl_GLSanctionDisburse_ChargesDetails.GoldLoanNo='" + GoldLoanNo.Trim() + "'  ";

                    conn = new SqlConnection(strConnString);
                    da = new SqlDataAdapter(strQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count > 0)
                    {
                        Session["dtChargesDetails"] = dt;
                        dgvChargesDetails.DataSource = dt;
                        dgvChargesDetails.DataBind();
                    }
                    else
                    {
                        dt = new DataTable();

                        dt.Columns.Add("CHID", typeof(String));
                        dt.Columns.Add("SDID", typeof(String));
                        dt.Columns.Add("ID", typeof(String));
                        dt.Columns.Add("CID", typeof(String));
                        dt.Columns.Add("ChargeName", typeof(String));
                        dt.Columns.Add("LoanAmtFrom", typeof(String));
                        dt.Columns.Add("LoanAmtTo", typeof(String));
                        dt.Columns.Add("Charges", typeof(String));
                        dt.Columns.Add("ChargeType", typeof(String));
                        dt.Columns.Add("AccountID", typeof(String));
                        dt.Columns.Add("Name", typeof(String));
                        dt.Columns.Add("ChargeAmount", typeof(String));

                        ShowNoResultFound(dt, dgvChargesDetails);
                    }

                    //CIBIL Score and Proof of Ownership
                    strQuery = "SELECT DISTINCT ID, CIBILScore, OwnershipProofImagePath " +
                                "FROM tbl_GLSanctionDisburse_OtherDetails " +
                                "WHERE  tbl_GLSanctionDisburse_OtherDetails.GoldLoanNo='" + GoldLoanNo + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        txtCIBILScore.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txtProofOfOwnershipPath.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        imgProofOfOwnership.ImageUrl = txtProofOfOwnershipPath.Text;
                        imgProofOfOwnership.Visible = true;
                    }

                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";


                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    //checking whether Gold Loan A/C is processed to next stage (EMI Interest JV)
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + GoldLoanNo + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since it is being processed to Interest JV.');", true);
                    }
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
    #endregion

    #region dgvDetails_PageIndexChanging
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
    #endregion

    #region [dgvChargesDetails_RowDataBound]
    protected void dgvChargesDetails_RowDataBound(object sender, EventArgs e)
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
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvChargesDetails_RowDataBound]

    #region Reset/Cancel
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

    #endregion

    #region Search Record
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //Search Records
            DataTable dt = GetRecords(conn, "GetAllRecords", "").Tables[0];
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

    #region ClearData
    protected void ClearData()
    {
        try
        {
            if (lblLogin.Text == "LogOut" && chkLogin.Checked == false)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage1", "javascript:window.alert('Logout for Excess Loan Amount.');", true);
            }
            else
            {
                txtOperatorName.Text = "";
                txtCustomerName.Text = "";
                txtLoantype.Text = "";
                txtPLCaseNo.Text = "";
                txtPanNo.Text = "";
                txtGender.Text = "";
                txtBirthDate.Text = "";
                txtAge.Text = "";
                txtMaritalStatus.Text = "";
                txtAddress.Text = "";
                txtTotalGrossWeight.Text = "0";
                txtNominee.Text = "";
                txtNomineeRelationship.Text = "";
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
                txtNetWeight.Text = "0";
                txtLoanTenure.Text = "";
                txtDueDate.Text = "";
                txtEMI.Text = "";
                txtNetAmountSanctioned.Text = "";
                txtCIBILScore.Text = "";
                imgProofOfOwnership.ImageUrl = "";
                txtMobile.Text = "";
                txtTelephone.Text = "";
                txtEmailID.Text = "";
                txtProofOfOwnershipPath.Text = "";
                btnSave.Text = "Update";
                btnReset.Text = "Cancel";
                txtNetAmount.Text = "0";
                txtDeduction.Text = "0";
                txtIssueDate.Text = "";
                lblMessageText.Text = "";
                lblLoginMsg.Text = "";
                txtTotalChargesAmount.Text = "";
                lblLogin.Text = "Login For excess loan amount";
                txtChequeNo.ReadOnly = true;
                txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                txtChequeDate.Enabled = false;
                GetRefNum();

                ddlRefID.DataSource = null;
                ddlRefID.Items.Clear();
                //Fill Bank/Cash Account Combo
                FillBankCashAccount();

                //Fill Scheme Name
                FillSchemeName();
                ddlSchemeName.SelectedValue = "0";

                //Clear Fields As Per Scheme
                ClearFieldsAsPerScheme();

                //Bind Gold Item Details
                BindGoldItemDetails();
                //Bind Charges Details
                BindChargesDetails();

                txtUserName.Text = "";
                txtPassword.Text = "";
                pnlLogin.Visible = false;
                chkLogin.Checked = false;
                chkLogin.Enabled = true;
                Session["Valid"] = "";

                //ddlcheqNEFTDD.Enabled = false;
                //ddlcheqNEFTDD.BackColor = System.Drawing.Color.Gainsboro;

                //getting FYear ID
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID); ;
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
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region GetRecords
    protected DataSet GetRecords(SqlConnection conn, string CommandName, String GoldLoanNo)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_GoldValueDetails.ID, tbl_GLSanctionDisburse_GoldValueDetails.SDID, " +
                                "tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo, tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                "ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' +tbl_GLKYC_ApplicantDetails.AppMName+' ' +tbl_GLKYC_ApplicantDetails.AppLName, " +
                                "tbl_GLKYC_ApplicantDetails.PANNo, NetLoanAmtSanctioned=convert(varchar,tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned), " +
                                "TotalGrossWeight=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight), " +
                                "TotalNetWeight=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight), " +
                                "GoldNetValue=convert(varchar,tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue) " +
                            "FROM tbl_GLSanctionDisburse_GoldValueDetails " +
                            "INNER JOIN tbl_GLSanctionDisburse_BasicDetails " +
                                "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                            "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                "ON tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                            "WHERE tbl_GLSanctionDisburse_BasicDetails.FYID='" + txtFYID.Text + "' " +
                            "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + txtBranchID.Text + "' "; 
            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "SELECT DISTINCT  tbl_GLSanctionDisburse_GoldValueDetails.ID, tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight, " +
                                "tbl_GLSanctionDisburse_GoldValueDetails.Deduction, tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight, " +
                                "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue, tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                            "FROM tbl_GLSanctionDisburse_GoldValueDetails " +
                            "WHERE tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo ='" + GoldLoanNo + "' ";
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

    #region Bind DropDownList-SearchBy
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("GoldLoanNo");
            ddlSearchBy.Items.Add("ApplicantName");
            ddlSearchBy.Items.Add("TotalGrossWeight");
            ddlSearchBy.Items.Add("TotalNetWeight");
            ddlSearchBy.Items.Add("NetLoanAmtSanctioned");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            bool datasaved = false;
            conn = new SqlConnection(strConnString);
            conn.Open();

            #region [Update]
            if (Page.IsValid)
            {
                if (btnSave.Text == "Update")
                {
                    if (lblLogin.Text == "LogOut" && chkLogin.Checked == false)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage1", "javascript:window.alert('Logout for Excess Loan Amount.');", true);
                    }
                    else
                    {
                        //checking whether Gold Loan A/C is processed to next stage (Interest JV)
                        RefType = txtGoldLoanNo.Text.Trim();
                        RefID = ddlRefID.SelectedValue.ToString().Trim();
                        RefNo = ddlRefNo.SelectedValue.ToString().Trim();
                        GoldNo = (RefType) + '/' + (RefNo) + '/' + (RefID);

                        strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + GoldNo + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since it is being processed to Interest JV.');", true);
                        }

                        if (existcount == 0)
                        {
                            int QueryResult = 0;
                            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                            string GoldLoanNo = GoldNo;
                            datasaved = true;

                            // 1] Data updation tbl_GLSanctionDisburse_GoldItemDetails
                            int GID = 0;
                            int valGID = 0;
                            string DocRecd = string.Empty;
                            string ImagePath = string.Empty;
                            int SDID = 0;
                            int ItemID = 0;
                            string ItemName = string.Empty;
                            string GrossWeight = string.Empty;
                            string strQuantity = string.Empty;
                            string ImageName = string.Empty;

                            string strGID = string.Empty;
                            string GIDForUpdate = string.Empty;

                            //getting rows deleted from Gold Item Details
                            if (dgvGoldItemDetails != null && dgvGoldItemDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvGoldItemDetails.Rows)
                                {
                                    strGID = (row.Cells[0].FindControl("lblGID") as Label).Text;
                                    if (strGID.Trim() != "")
                                    {
                                        GIDForUpdate += strGID.ToString() + ",";
                                    }
                                }
                            }

                            int strLen = GIDForUpdate.Length;
                            if (GIDForUpdate.EndsWith(","))
                            {
                                GIDForUpdate = GIDForUpdate.Remove((strLen - 1), 1);
                            }

                            //deleting record from tbl_GLSanctionDisburse_GoldItemDetails
                            if (strLen > 0)
                            {
                                int count = 0;
                                strQuery = "Select count(*) from tbl_GLSanctionDisburse_GoldItemDetails " +
                                           "where GoldLoanNo='" + GoldLoanNo + "' and GID NOT IN (" + GIDForUpdate.Trim() + ")";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    count = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    count = 0;
                                }

                                if (count > 0)
                                {
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_GoldItemDetails " +
                                                    "where GoldLoanNo='" + GoldLoanNo + "' and GID NOT IN (" + GIDForUpdate.Trim() + ")";
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
                                else
                                {
                                    datasaved = true;
                                }
                            }
                            else if (strLen == 0)
                            {
                                int count = 0;
                                strQuery = "Select count(*) from tbl_GLSanctionDisburse_GoldItemDetails " +
                                           "where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    count = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    count = 0;
                                }

                                if (count > 0)
                                {
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_GoldItemDetails " +
                                                    "where GoldLoanNo='" + GoldLoanNo + "'";
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
                                else
                                {
                                    datasaved = true;
                                }
                            }
                            else
                            {
                                datasaved = true;
                            }

                            //insertion/updation of data into tbl_GLSanctionDisburse_GoldItemDetails
                            if (dgvGoldItemDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvGoldItemDetails.Rows)
                                {
                                    GID = 0;
                                    valGID = 0;
                                    if (datasaved == true)
                                    {
                                        if ((row.Cells[1].FindControl("lblGID") as Label).Text.Trim() != "")
                                        {
                                            valGID = Convert.ToInt32((row.Cells[1].FindControl("lblGID") as Label).Text);
                                            SDID = Convert.ToInt32((row.Cells[2].FindControl("lblSDID") as Label).Text);
                                        }
                                        else
                                        {
                                            valGID = 0;
                                        }

                                        GID = valGID;
                                        ItemID = Convert.ToInt32((row.Cells[3].FindControl("lblItemID") as Label).Text);
                                        ItemName = Convert.ToString((row.Cells[4].FindControl("lblGoldItemName") as Label).Text).Trim();
                                        GrossWeight = Convert.ToString((row.Cells[5].FindControl("lblGrossWeight") as Label).Text).Trim();
                                        strQuantity = (row.Cells[6].FindControl("lblQuantity") as Label).Text;
                                        ImagePath = Convert.ToString((row.Cells[8].FindControl("lblPath") as Label).Text).Trim();
                                        ImageName = Convert.ToString((row.Cells[9].FindControl("lblImageName") as Label).Text).Trim();

                                        if (valGID == 0)
                                        {
                                            //getting MAX DID
                                            strQuery = "Select MAX(GID) from tbl_GLSanctionDisburse_GoldItemDetails ";
                                            cmd = new SqlCommand(strQuery, conn, transaction);
                                            if (cmd.ExecuteScalar() != DBNull.Value)
                                            {
                                                GID = Convert.ToInt32(cmd.ExecuteScalar());
                                            }
                                            else
                                            {
                                                GID = 0;
                                            }

                                            GID += 1;

                                            //inserting data into table tbl_GLSanctionDisburse_GoldItemDetails
                                            insertQuery = "insert into tbl_GLSanctionDisburse_GoldItemDetails values('" + GID + "', '" + txtSID.Text + "','" + GoldLoanNo + "', " +
                                                            "'" + ItemID + "', '" + GrossWeight.Trim() + "', " + strQuantity + ", '" + ImageName.Trim() + "', '" + ImagePath.Trim() + "')";

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
                                        //else if (valGID > 0)
                                        //{
                                        //    //updating table tbl_GLSanctionDisburse_GoldItemDetails
                                        //    updateQuery = "update tbl_GLSanctionDisburse_GoldItemDetails set ItemID='" + ItemID + "', GrossWeight='" + GrossWeight.Trim() + "', " +
                                        //                        "SDID=" + SDID + ", Image='" + ImageName.Trim() + "' , " +
                                        //                        "PhotoPath='" + ImagePath.Trim() + "' " +
                                        //                  "where GID='" + valGID + "'";

                                        //    cmd = new SqlCommand(updateQuery, conn, transaction);
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
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            // 2] Data updation Charges Details
                            // Data updation tbl_GLSanctionDisburse_ChargesDetails
                            int CHID = 0;
                            int valCHID = 0;
                            SDID = 0;
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
                            strLen = 0;

                            //getting rows deleted from Charges Details
                            if (dgvChargesDetails != null && dgvChargesDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvChargesDetails.Rows)
                                {
                                    strCHID = (row.Cells[0].FindControl("lblCHID") as Label).Text;
                                    if (strCHID.Trim() != "")
                                    {
                                        CHIDForUpdate += strCHID.ToString() + ",";
                                    }
                                }
                            }

                            strLen = CHIDForUpdate.Length;
                            if (CHIDForUpdate.EndsWith(","))
                            {
                                CHIDForUpdate = CHIDForUpdate.Remove((strLen - 1), 1);
                            }

                            // 2.1] deleting record from tbl_GLSanctionDisburse_ChargesDetails
                             string strWhereClause = string.Empty;
                            if (strLen > 0)
                            {
                                strWhereClause = "where GoldLoanNo='" + GoldLoanNo + "' and CHID NOT IN (" + CHIDForUpdate.Trim() + ")";
                                deleteQuery = "delete from tbl_GLSanctionDisburse_ChargesDetails " + strWhereClause + "";
                            }
                            else
                            {
                                 strWhereClause = "where GoldLoanNo='" + GoldLoanNo + "'";
                                 deleteQuery = "delete from tbl_GLSanctionDisburse_ChargesDetails " + strWhereClause + "";
                            }

                            int excount = 0;
                            strQuery = "Select count(*) from tbl_GLSanctionDisburse_ChargesDetails " + strWhereClause + "";
                            cmd = new SqlCommand(strQuery, conn, transaction);
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
                            else
                            {
                                datasaved = true;
                            }

                            // 2.2] insertion/updation of data into tbl_GLSanctionDisburse_ChargesDetails
                            if (dgvChargesDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvChargesDetails.Rows)
                                {
                                    CHID = 0;
                                    valCHID = 0;
                                    if (datasaved == true)
                                    {
                                        if ((row.Cells[0].FindControl("lblCHID") as Label).Text.Trim() != "")
                                        {
                                            valCHID = Convert.ToInt32((row.Cells[0].FindControl("lblCHID") as Label).Text);
                                            SDID = Convert.ToInt32((row.Cells[1].FindControl("lblSDID") as Label).Text);
                                        }
                                        else
                                        {
                                            valCHID = 0;
                                        }

                                        CHID = valCHID;
                                        string strID = (row.Cells[2].FindControl("lblID") as Label).Text;
                                        string strCID = (row.Cells[3].FindControl("lblCID") as Label).Text;
                                        string strChargesName = (row.Cells[4].FindControl("lblChargesName") as Label).Text;
                                        string strCharges = (row.Cells[5].FindControl("lblCharges") as Label).Text;
                                        string strLoanAmtFrom = (row.Cells[6].FindControl("lblLoanAmtFrom") as Label).Text;
                                        string strLoanAmtTo = (row.Cells[7].FindControl("lblLoanAmtTo") as Label).Text;
                                        string strChargeType = (row.Cells[8].FindControl("lblChargeType") as Label).Text;
                                        string strAmount = (row.Cells[9].FindControl("lblAmount") as Label).Text;
                                        string strAccountName = (row.Cells[10].FindControl("lblAccountName") as Label).Text;
                                        string strAccountID = (row.Cells[11].FindControl("lblAccountID") as Label).Text;

                                        if (valCHID == 0)
                                        {
                                             if (strChargesName.Trim() != "")
                                        {
                                            //getting MAX ID
                                            CHID = 0;
                                            strQuery = "select max(CHID) from tbl_GLSanctionDisburse_ChargesDetails";
                                            cmd = new SqlCommand(strQuery, conn, transaction);
                                            if (cmd.ExecuteScalar() != DBNull.Value)
                                            {
                                                CHID = Convert.ToInt32(cmd.ExecuteScalar());
                                            }
                                            else
                                            {
                                                CHID = 0;
                                            }

                                            CHID += 1;

                                            insertQuery = "insert into tbl_GLSanctionDisburse_ChargesDetails values( " +
                                                                "" + CHID + ", '" + SDID + "', '" + GoldLoanNo + "', " +
                                                                "" + strID + ", " + strCID + ", '" + strChargesName + "', " +
                                                                "" + strLoanAmtFrom + ", " + strLoanAmtTo + ", " +
                                                                "" + strCharges + ", '" + strChargeType + "', " +
                                                                "" + strAccountID + ", " + strAmount + ")";
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
                                             else
                                             {
                                                 datasaved = true;
                                                 break;
                                             }
                                        }
                                        //else if (valCHID > 0)
                                        //{
                                        //    //updating table tbl_GLSanctionDisburse_ChargesDetails
                                        //    updateQuery = "update tbl_GLSanctionDisburse_ChargesDetails set ID=" + strID + ", " +
                                        //                        "CID=" + strCID + " , " +
                                        //                        "ChargeName='" + strChargesName + "', " +
                                        //                        "LoanAmtFrom=" + strLoanAmtFrom + " , " +
                                        //                        "LoanAmtTo=" + strLoanAmtTo + ", " +
                                        //                        "Charges=" + strCharges + ", " +
                                        //                        "ChargeType='" + strChargeType + "', " +
                                        //                        "AccountID=" + strAccountID + " , " +
                                        //                        "ChargeAmount=" + strAmount + " " +
                                        //                  "where GID='" + valGID + "'";

                                        //    cmd = new SqlCommand(updateQuery, conn, transaction);
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
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            int AccID = 0;
                            int LedgerID = 0;
                            double DebitAmount = 0;
                            double CreditAmount = 0;

                            // 2.3] Deleting effects from Company-wise Account Closing table for Charges Details
                            strQuery = "select AccID, Debit, Credit, LedgerID from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                                        "where GoldLoanNo='" + GoldLoanNo + "'";
                            cmd = new SqlCommand(strQuery, conn, transaction);
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
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);

                                        if (datasaved == false)
                                        {
                                            break;
                                        }

                                        //Deleting data from table FLedgerMaster 
                                        if (datasaved)
                                        {
                                            deleteQuery = "delete from FLedgerMaster " +
                                                            "where LedgerID=" + LedgerID + "";

                                            cmd = new SqlCommand(deleteQuery, conn, transaction);
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
                                excount = 0;
                                strQuery = "Select count(*) from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
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
                                    deleteQuery = "delete from tbl_GLSanctionDisburse_ChargesPostingDetails " +
                                                  "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 3] Data updation tbl_GLSanctionDisburse_SchemeDetails
                            if (datasaved == true)
                            {
                                string strDueDate = string.Empty;
                                if (txtDueDate.Text.Trim() != "")
                                {
                                    strDueDate = Convert.ToDateTime(txtDueDate.Text.Trim()).ToString("yyyy/MM/dd");
                                }

                                if (txtEMI.Text.Trim() == "")
                                {
                                    txtEMI.Text = "0";
                                }
                                updateQuery = "update tbl_GLSanctionDisburse_SchemeDetails set " +
                                                        "ID='" + txtSchemeID.Text + "', " +
                                                        "SchemeName='" + txtSchemeName.Text + "', " +
                                                        "SchemeType='" + txtSchemeType.Text + "', " +
                                                        "LTV='" + txtLTV.Text + "', " +
                                                        "MinTenure='" + txtMinTenure.Text + "', " +
                                                        "MaxTenure='" + txtMaxTenure.Text + "', " +
                                                        "InterestRate='" + txtInterestRate.Text + "', " +
                                                        "LoanTenure='" + txtLoanTenure.Text + "', " +
                                                        "EMI='" + txtEMI.Text + "', " +
                                                        "DueDate='" + strDueDate + "' " +
                                                  "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 4] Data updation tbl_GLSanctionDisburse_OtherDetails
                            if (datasaved == true)
                            {
                                //checking whether record is present
                                strQuery = "select count(*) from tbl_GLSanctionDisburse_OtherDetails where GoldLoanNo='" + GoldLoanNo + "'";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                existcount = Convert.ToInt32(cmd.ExecuteScalar());

                                if (existcount > 0)
                                {
                                    updateQuery = "update tbl_GLSanctionDisburse_OtherDetails set " +
                                                        "CIBILScore='" + txtCIBILScore.Text + "', " +
                                                        "OwnershipProofImagePath='" + txtProofOfOwnershipPath.Text + "' " +
                                                  "where GoldLoanNo='" + GoldLoanNo + "'";

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
                                    datasaved = true;
                                }
                            }

                            // 5] Data updation tbl_GLSanctionDisburse_GoldValueDetails
                            if (datasaved == true)
                            {
                                updateQuery = "update tbl_GLSanctionDisburse_GoldValueDetails set TotalGrossWeight='" + Convert.ToDouble(txtTotalGrossWeight.Text) + "', " +
                                                    "Deduction='" + txtDeduction.Text.Trim() + "', TotalNetWeight='" + txtNetWeight.Text.Trim() + "', " +
                                                    "GoldNetValue='" + txtNetAmount.Text.Trim() + "' " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 6] Data updation tbl_GLSanctionDisburse_BasicDetails
                            if (datasaved == true)
                            {
                                string strChequeDate = string.Empty;

                                if (txtChequeDate.Text.Trim() != "")
                                {
                                    strChequeDate = Convert.ToDateTime(txtChequeDate.Text).ToString("yyyy/MM/dd");
                                }
                                updateQuery = "update tbl_GLSanctionDisburse_BasicDetails set NetLoanAmtSanctioned='" + Convert.ToDouble(txtNetAmountSanctioned.Text) + "', " +
                                                    "IssueDate='" + Convert.ToDateTime(txtIssueDate.Text.Trim()).ToString("yyyy/MM/dd") + "',  " +
                                                    "OperatorID=" + txtOperatorID.Text + ", BankCashAccID=" + ddlCashAccount.SelectedValue + ", " +
                                                    "CheqNEFTDD='" + ddlcheqNEFTDD.SelectedItem.Text + "', " +
                                                    "CheqNEFTDDNo='" + txtChequeNo.Text + "', CheqNEFTDDDate='" + strChequeDate + "' " +
                                              "where GoldLoanNo='" + GoldLoanNo + "'";

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

                            // 7] Data updation tbl_GLSanctionDisburse_ExcessLoanUserDetails
                            if (datasaved == true)
                            {
                                if (txtLoginID.Text.Trim() != "")
                                {
                                    LogInTime1 = DateTime.Now;
                                    DateTime LoginDate = DateTime.Now.Date;
                                    String LogInTime = Convert.ToString(LogInTime1);
                                    insertQuery = "update tbl_GLSanctionDisburse_ExcessLoanUserDetails set TempOrPerm='P'  " +
                                                   "where LoginID='" + txtLoginID.Text + "'";

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


                            //+++++++++++++++++++++++++++++++++ UPDATION OF LEDGER ENTRIES ++++++++++++++++++++++++++++++++++++++
                            

                            // 8.1] Updation of Bank Cash Payment Details
                            int BCPID = 0;
                            
                            if (datasaved)
                            {
                                // getting BCPID 
                                if (datasaved)
                                {
                                    strQuery = "select BCPID from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + GoldLoanNo + "'";
                                    cmd = new SqlCommand(strQuery, conn, transaction);
                                    if (cmd.ExecuteScalar() != DBNull.Value)
                                    {
                                        BCPID = Convert.ToInt32(cmd.ExecuteScalar());
                                    }
                                    else
                                    {
                                        BCPID = 0;
                                    }
                                }

                                //Updating table TBankCash_PaymentDetails
                                updateQuery = "update TBankCash_PaymentDetails " +
                                                        "set RefDate='" + Convert.ToDateTime(txtIssueDate.Text.Trim()).ToString("yyyy/MM/dd") + "', " +
                                                        "BankCashAccID=" + Convert.ToInt32(ddlCashAccount.SelectedValue) + ", " +
                                                        "Amount=" + Convert.ToDouble(txtNetAmountSanctioned.Text) + " " +
                                                "where BCPID=" + BCPID + " ";

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

                            // 8.2] updation of Ledger and Company Wise Account Closing tables
                            if (datasaved)
                            {
                                // getting ReferenceNo 
                                string DJEReferenceNo = string.Empty;
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
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    DJEReferenceNo = Convert.ToString(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    DJEReferenceNo = "";
                                }

                                strQuery = "select AccountID, Debit, Credit, RefDate, LedgerID from FLedgerMaster " +
                                            "where ReferenceNo='" + DJEReferenceNo + "'";

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
                                            accountID = Convert.ToInt32(drow1[0]);
                                            
                                            debit = Convert.ToDouble(drow1[1]);
                                            credit = Convert.ToDouble(drow1[2]);
                                            dtRefDate = Convert.ToDateTime(drow1[3]);
                                            LedgerID = Convert.ToInt32(drow1[4]);
                                            DebitAmount = 0;
                                            CreditAmount = 0;

                                            if (debit > 0)
                                            {
                                                AccID = accountID;
                                                ContraID = AccID;
                                                ContraAccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                                                DebitAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                                                CreditAmount = 0;
                                            }
                                            else if (credit > 0)
                                            {
                                                AccID = Convert.ToInt32(ddlCashAccount.SelectedValue);
                                                ContraAccID = ContraID;
                                                DebitAmount = 0;
                                                CreditAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                                            }

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonDelete(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), accountID, debit, credit, transaction, conn);
                                            }

                                            if (datasaved)
                                            {
                                                datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                            }

                                            // updating table FLedgerMaster
                                            if (datasaved)
                                            {
                                                updateQuery = "update FLedgerMaster set " +
                                                                    "RefDate='" + Convert.ToDateTime(txtIssueDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                                    "AccountID=" + AccID + ", " +
                                                                    "Debit=" + DebitAmount + ", " +
                                                                    "Credit=" + CreditAmount + ", " +
                                                                    "ContraAccID=" + ContraAccID + " " +
                                                              "where LedgerID=" + LedgerID + " ";

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
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }

                            //**************************** Accounting Entries for Charges ***************************************

                            int CreditID = 0;
                            string Narration = "Amount received against Charges";

                            //getting first Account ID 
                            if (dgvChargesDetails != null && dgvChargesDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvChargesDetails.Rows)
                                {
                                    if ((row.Cells[10].FindControl("lblAccountID") as Label).Text != "")
                                    {
                                        CreditID = Convert.ToInt32((row.Cells[10].FindControl("lblAccountID") as Label).Text);
                                        break;
                                    }
                                }
                            }
                            
                            //getting AccountID
                            int accountId = 0;
                            
                            strQuery = "select AccountID from tblAccountMaster where Alies='" + GoldLoanNo + "'";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                accountId = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                accountId = 0;
                            }

                            //RefType and ReferenceNo
                            string DJERefType = string.Empty;
                            string DJEReferencNo = string.Empty;
                            
                            strQuery = "SELECT DISTINCT RefType, ReferenceNo " +
                                        "FROM FSystemGeneratedEntryMaster " +
                                        "WHERE  LoginID='" + GoldLoanNo + "'";

                            cmd = new SqlCommand(strQuery, conn, transaction);
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
                                    LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtIssueDate.Text), AccID, DebitAmount, CreditAmount, ContraAccID, Narration);
                                    if (datasaved)
                                    {
                                        datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), AccID, DebitAmount, CreditAmount, transaction, conn);
                                    }

                                    if (datasaved)
                                    {
                                        int PID = 0;
                                        // retrieving MAX ID
                                        strQuery = "SELECT MAX(ID) FROM tbl_GLSanctionDisburse_ChargesPostingDetails";
                                        cmd = new SqlCommand(strQuery, conn, transaction);
                                        if (cmd.ExecuteScalar() != DBNull.Value)
                                        {
                                            PID = Convert.ToInt32(cmd.ExecuteScalar());
                                        }
                                        else
                                        {
                                            PID = 0;
                                        }

                                        PID += 1;

                                        insertQuery = "INSERT into tbl_GLSanctionDisburse_ChargesPostingDetails values(" + PID + ", " + SDID + ", " +
                                                                "'" + GoldLoanNo + "', " + AccID + ", " + DebitAmount + ", " +
                                                                "" + CreditAmount + ", " + LedgerID + ", " + txtFYID.Text + ") ";

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
                                                string strCharges = (row.Cells[5].FindControl("lblCharges") as Label).Text;
                                                string strChargeType = (row.Cells[8].FindControl("lblChargeType") as Label).Text;
                                                string strAmount = (row.Cells[9].FindControl("lblAmount") as Label).Text;
                                                string strAccountName = (row.Cells[10].FindControl("lblAccountName") as Label).Text;
                                                string strAccountID = (row.Cells[11].FindControl("lblAccountID") as Label).Text;

                                                AccID = accountId;
                                                int ContraAccID = Convert.ToInt32(strAccountID);
                                                DebitAmount = 0;
                                                CreditAmount = Convert.ToDouble(strAmount);
                                                LedgerID = CreateNormalLedgerEntries(DJERefType, DJEReferencNo, Convert.ToDateTime(txtIssueDate.Text), ContraAccID, DebitAmount, CreditAmount, AccID, Narration);

                                                if (datasaved)
                                                {
                                                    datasaved = objCompWiseAccClosing.CompanyWiseYearEndAccountClosingonSave(Convert.ToInt32(txtFYID.Text), Convert.ToInt32(txtCompID.Text), Convert.ToInt32(txtBranchID.Text), ContraAccID, DebitAmount, CreditAmount, transaction, conn);

                                                    if (datasaved == false)
                                                    {
                                                        break;
                                                    }

                                                    if (datasaved)
                                                    {
                                                        int PID = 0;
                                                        // retrieving MAX ID
                                                        strQuery = "SELECT MAX(ID) FROM tbl_GLSanctionDisburse_ChargesPostingDetails";
                                                        cmd = new SqlCommand(strQuery, conn, transaction);
                                                        if (cmd.ExecuteScalar() != DBNull.Value)
                                                        {
                                                            PID = Convert.ToInt32(cmd.ExecuteScalar());
                                                        }
                                                        else
                                                        {
                                                            PID = 0;
                                                        }

                                                        PID += 1;

                                                        insertQuery = "INSERT into tbl_GLSanctionDisburse_ChargesPostingDetails values(" + PID + ", " + SDID + ", " +
                                                                                "'" + GoldLoanNo + "', " + ContraAccID + ", " + DebitAmount + ", " +
                                                                                "" + CreditAmount + ", " + LedgerID + ", " + txtFYID.Text + ") ";

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
                            //Vallidate cheq/NEFT/DD min length by kishor  
                            if (datasaved == true)
                            {
                                if (datasaved)
                                {
                                    //if (ddlcheqNEFTDD.SelectedItem.Text == "Cheque" )
                                    //{
                                    //    ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 3 char.');", true);
                                    //    datasaved = false;
                                    //    QueryResult = 0;
                                    //}
                                    //else
                                    if (ddlcheqNEFTDD.SelectedItem.Text == "NEFT" && txtChequeNo.Text.Length < 15)
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 15 char.');", true);
                                        datasaved = false;
                                        QueryResult = 0;
                                    }
                                    else if (ddlcheqNEFTDD.SelectedItem.Text == "DD" && txtChequeNo.Text.Length < 6)
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 6 char.');", true);
                                        datasaved = false;
                                        QueryResult = 0;
                                    }
                                    else
                                    if (QueryResult > 0)
                                    {
                                        datasaved = true;
                                    }
                                   
                                }
                                  
                               }


                            if (QueryResult > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Updated Successfully.');", true);
                                BindDGVDetails();
                                ddlRefID.SelectedItem.Text = "";
                                ClearData();
                            }
                            else
                            {
                                transaction.Rollback();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Updated Successfully.');", true);
                            }
                        }
                    }
                }
            #endregion [Update]
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
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
                                    "" + AccID + ", " + DebitAmount + ", " + CreditAmount + ", '" + Narration + "', " +
                                    "" + ContraAccID + ", '', " + txtFYID.Text + ") ";

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

    #region Get RefNum
    protected void GetRefNum()
    {
        try
        {
            txtGoldLoanNo.Text = strRefType;
            ddlRefNo.DataSource = null;
            conn = new SqlConnection(strConnString);

            strQuery =
            "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.RefNo  " +
             "FROM tbl_GLSanctionDisburse_BasicDetails " +
                   "LEFT OUTER JOIN tbl_GLEMI_InterestJVDetails " +
                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
            "WHERE  tbl_GLSanctionDisburse_BasicDetails.FYID='" + txtFYID.Text + "' " +
            "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + txtBranchID.Text + "' " +
            "AND tbl_GLEMI_InterestJVDetails.GoldLoanNo IS NULL ";


            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRefNo.DataSource = dt;
            ddlRefNo.DataTextField = "RefNo";
            ddlRefNo.DataValueField = "RefNo";
            ddlRefNo.DataBind();
            ddlRefNo.Items.Insert(0, new ListItem("Ref No", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefNumlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Get RefID
    protected void GetRefID()
    {
        try
        {
            txtGoldLoanNo.Text = strRefType;
            ddlRefID.DataSource = null;
            conn = new SqlConnection(strConnString);

            strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.RefID  " +
                        "FROM tbl_GLSanctionDisburse_BasicDetails " +
                        "WHERE tbl_GLSanctionDisburse_BasicDetails.RefNo='" + ddlRefNo.SelectedValue + "' " +
                        "AND tbl_GLSanctionDisburse_BasicDetails.FYID='" + txtFYID.Text + "' " +
                        "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + txtBranchID.Text + "' ";

            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRefID.DataSource = dt;
            ddlRefID.DataTextField = "RefID";
            ddlRefNo.DataValueField = "RefID";
            ddlRefID.DataBind();
            ddlRefID.Items.Insert(0, new ListItem("Ref ID", "0"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefIDAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region ddlRefNo_SelectedIndexChanged
    protected void ddlRefNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            GetRefID();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "RefIdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region RefId_SelectedIndexChanged
    protected void ddlRefId_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillData();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "RefIdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region cancelData
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            Session["Valid"] = "";
            txtUserName.Text = "";
            txtPassword.Text = "";
            lblLoginMsg.Text = "";
            pnlLogin.Visible = false;
            chkLogin.Checked = false;
            chkLogin.Enabled = true;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion

    #region Deduction_TextChanged
    protected void txtDeduction_TextChanged(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (txtDeduction.Text != "")
            {
                if (((Convert.ToDouble(txtDeduction.Text)) > (Convert.ToDouble(txtTotalGrossWeight.Text))))
                {
                    lblMsg.Font.Size = 10;
                    lblMsg.Text = "(Deduction must be less than Gross Weight)";
                    txtDeduction.Text = "";
                    txtDeduction.Focus();
                    txtNetWeight.Text = "";
                }
                else
                {
                    lblMsg.Text = "";
                    double TotalGrossWeight = 0;
                    double Deduction = 0;
                    double defaultDeductionInGrossWeight = 0;
                    double TotalNetWeight = 0;

                    //fetching Default Deduction In Gross Weight
                    strQuery = "SELECT DeductionInGrossWeight FROM tblLoanParameterSetting";
                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                        {
                            defaultDeductionInGrossWeight = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                        }
                        else
                        {
                            defaultDeductionInGrossWeight = 0;
                        }
                    }
                    else
                    {
                        defaultDeductionInGrossWeight = 0;
                    }

                    if (Convert.ToString(txtTotalGrossWeight.Text) != "")
                    {
                        TotalGrossWeight = Convert.ToDouble(txtTotalGrossWeight.Text);
                        if (Convert.ToString(txtDeduction.Text) != "")
                        {
                            Deduction = Convert.ToDouble(txtDeduction.Text);
                        }
                        else
                        {
                            Deduction = 0;
                        }
                        TotalNetWeight = TotalGrossWeight - Deduction - defaultDeductionInGrossWeight;
                        TotalNetWeight = Math.Round(TotalNetWeight, 3);
                        if (TotalNetWeight < 0)
                        {
                            txtNetWeight.Text = "0";
                        }
                        else
                        {
                            txtNetWeight.Text = Convert.ToString(TotalNetWeight);
                        }
                    }
                }
                //Calulating Max Loan Amount
                CalculateMaxLoanAmount();
                //Calculate EMI
                if (txtSchemeType.Text.Trim() == "MI")
                {
                    double LoanAmount = 0;
                    double InterestRate = 0;
                    double LoanTenure = 0;
                    if (txtNetAmountSanctioned.Text.Trim() != "")
                    {
                        LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                    }
                    else if (txtNetAmount.Text.Trim() != "")
                    {
                        LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                    }
                    if (txtInterestRate.Text.Trim() != "")
                    {
                        InterestRate = Convert.ToDouble(txtInterestRate.Text);
                    }
                    if (txtLoanTenure.Text.Trim() != "")
                    {
                        LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                    }
                    CalculateEMI(LoanAmount, InterestRate, LoanTenure);
                }
                else
                {
                    txtEMI.Text = "";
                }

                //Checking Amount is excess
                if (Convert.ToDouble(txtNetAmount.Text) > 0)
                {
                    bool IsExcess = IsExcessAmount();
                    if (IsExcess == false)
                    {
                        lblMessageText.Text = "Login for Excess amount.";
                    }
                    else
                    {
                        lblMessageText.Text = "";
                        pnlLogin.Visible = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DeductionAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion

    #region txtTotalGrossWeight_TextChanged
    protected void txtTotalGrossWeight_TextChanged(object sender, EventArgs e)
    {
        try
        {
            double TotalGrossWeight = 0;
            double Deduction = 0;
            double defaultDeductionInGrossWeight = 0;
            double TotalNetWeight = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();

            //fetching Default Deduction In Gross Weight
            strQuery = "SELECT DeductionInGrossWeight FROM tblLoanParameterSetting";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    defaultDeductionInGrossWeight = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                }
                else
                {
                    defaultDeductionInGrossWeight = 0;
                }
            }
            else
            {
                defaultDeductionInGrossWeight = 0;
            }

            if (Convert.ToString(txtTotalGrossWeight.Text) != "")
            {
                TotalGrossWeight = Convert.ToDouble(txtTotalGrossWeight.Text);
                if (Convert.ToString(txtDeduction.Text) != "")
                {
                    Deduction = Convert.ToDouble(txtDeduction.Text);
                }
                else
                {
                    Deduction = 0;
                }
                TotalNetWeight = TotalGrossWeight - Deduction - defaultDeductionInGrossWeight;
                TotalNetWeight = Math.Round(TotalNetWeight, 2);
                txtNetWeight.Text = Convert.ToString(TotalNetWeight);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DeductionAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [SchemeName_SelectedIndexChanged]
    protected void SchemeName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //net weight validation
            if (txtNetWeight.Text.Trim() == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Gold Item Details. Net Weight cannot be zero.');", true);
                ddlSchemeName.SelectedValue = "0";
            }
            else
            {
                if (Convert.ToDouble(txtNetWeight.Text) == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Gold Item Details. Net Weight cannot be zero.');", true);
                    ddlSchemeName.SelectedValue = "0";
                }
            }

            //Get Scheme Details
            GetSchemeDetails();
            //Clear Fields As Per Scheme
            ClearFieldsAsPerScheme();
            //Validate Loan Tenure
            int validcount = ValidateLoanTenure();
            //Calulating Max Loan Amount
            CalculateMaxLoanAmount();
            //Calculate EMI
            if (txtSchemeType.Text.Trim() == "MI")
            {
                double LoanAmount = 0;
                double InterestRate = 0;
                double LoanTenure = 0;
                if (txtNetAmountSanctioned.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                }
                else if (txtNetAmount.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                }
                if (txtInterestRate.Text.Trim() != "")
                {
                    InterestRate = Convert.ToDouble(txtInterestRate.Text);
                }
                if (txtLoanTenure.Text.Trim() != "")
                {
                    LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                }
                CalculateEMI(LoanAmount, InterestRate, LoanTenure);
            }
            else
            {
                txtEMI.Text = "";
            }

            //Checking Amount is excess
            bool IsExcess = IsExcessAmount();
            if (IsExcess == false)
            {
                lblMessageText.Text = "Login for Excess amount.";
            }
            else
            {
                lblMessageText.Text = "";
                pnlLogin.Visible = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SchemeNameEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [SchemeName_SelectedIndexChanged]

    #region [NetAmountSanctioned_TextChanged]
    protected void NetAmountSanctioned_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //Calculate EMI
            if (txtSchemeType.Text.Trim() == "MI")
            {
                double LoanAmount = 0;
                double InterestRate = 0;
                double LoanTenure = 0;
                if (txtNetAmountSanctioned.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                }
                else if (txtNetAmount.Text.Trim() != "")
                {
                    LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                }
                if (txtInterestRate.Text.Trim() != "")
                {
                    InterestRate = Convert.ToDouble(txtInterestRate.Text);
                }
                if (txtLoanTenure.Text.Trim() != "")
                {
                    LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                }
                CalculateEMI(LoanAmount, InterestRate, LoanTenure);
            }
            else
            {
                txtEMI.Text = "";
            }

            if (dgvChargesDetails.Rows.Count > 0)
            {
                dt = new DataTable();

                dt.Columns.Add("CHID", typeof(String));
                dt.Columns.Add("SDID", typeof(String));
                dt.Columns.Add("ID", typeof(String));
                dt.Columns.Add("CID", typeof(String));
                dt.Columns.Add("ChargeName", typeof(String));
                dt.Columns.Add("LoanAmtFrom", typeof(String));
                dt.Columns.Add("LoanAmtTo", typeof(String));
                dt.Columns.Add("Charges", typeof(String));
                dt.Columns.Add("ChargeType", typeof(String));
                dt.Columns.Add("AccountID", typeof(String));
                dt.Columns.Add("Name", typeof(String));
                dt.Columns.Add("ChargeAmount", typeof(String));

                ShowNoResultFound(dt, dgvChargesDetails);
            }

            bool IsExcess = IsExcessAmount();
            if (IsExcess == false)
            {
                lblMessageText.Text = "Please Login for Excess amount.";
            }
            else
            {
                lblMessageText.Text = "";
                pnlLogin.Visible = false;
            }
        }
        catch (Exception ex)
        {
            Session["Valid"] = "";
            ClientScript.RegisterStartupScript(this.GetType(), "DeductionAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [NetAmountSanctioned_TextChanged]

    #region [LoanTenure_TextChanged]
    protected void LoanTenure_TextChanged(object sender, EventArgs e)
    {
        try
        {
            int validcount = ValidateLoanTenure();

            if (validcount != 0)
            {
                //Calculate EMI
                if (txtSchemeType.Text.Trim() == "MI")
                {
                    double LoanAmount = 0;
                    double InterestRate = 0;
                    double LoanTenure = 0;
                    if (txtNetAmountSanctioned.Text.Trim() != "")
                    {
                        LoanAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                    }
                    else if (txtNetAmount.Text.Trim() != "")
                    {
                        LoanAmount = Convert.ToDouble(txtNetAmount.Text);
                    }
                    if (txtInterestRate.Text.Trim() != "")
                    {
                        InterestRate = Convert.ToDouble(txtInterestRate.Text);
                    }
                    if (txtLoanTenure.Text.Trim() != "")
                    {
                        LoanTenure = Convert.ToDouble(txtLoanTenure.Text);
                    }
                    CalculateEMI(LoanAmount, InterestRate, LoanTenure);

                    txtDueDate.Focus();
                }
                else
                {
                    txtEMI.Text = "";
                    txtNetAmountSanctioned.Focus();
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoanTenure_TextChangedAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [LoanTenure_TextChanged]

    #region [ddlCashAccount_SelectedIndexChanged]
    protected void ddlCashAccount_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlCashAccount.SelectedValue != "0")
            {
                txtAccGPID.Value = "";
                strQuery = "select tblAccountmaster.GPID from tblAccountmaster " +
                             "where tblAccountMaster.AccountID='" + ddlCashAccount.SelectedValue + "' ";
                cmd = new SqlCommand(strQuery, conn);
                int accGPID = Convert.ToInt32(cmd.ExecuteScalar());
                txtAccGPID.Value = Convert.ToString(accGPID);

                if (accGPID == 70)
                {
                    txtChequeNo.Text = "";
                    txtChequeDate.Text = "";
                    txtChequeNo.ReadOnly = true;
                    txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                    txtChequeDate.Enabled = false;
                    ddlcheqNEFTDD.Enabled = false;
                    ddlcheqNEFTDD.BackColor = System.Drawing.Color.Gainsboro;
                }
                else
                {
                    txtChequeNo.ReadOnly = false;
                    txtChequeNo.BackColor = System.Drawing.Color.White;
                    txtChequeDate.Enabled = true;
                    //txtChequeNo.Focus();
                    ddlcheqNEFTDD.Enabled = true;
                    ddlcheqNEFTDD.BackColor = System.Drawing.Color.White;
                }
            }
            else
            {
                txtAccGPID.Value = "";
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
                txtChequeNo.ReadOnly = true;
                txtChequeNo.BackColor = System.Drawing.Color.Gainsboro;
                txtChequeDate.Enabled = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CashAcc_EventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [ddlCashAccount_SelectedIndexChanged]

    #region [txtIssueDate_TextChanged]
    protected void txtIssueDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            if (txtIssueDate.Text.Trim() != "")
            {
                SqlDataSource SqlDataSource3 = (SqlDataSource)dgvChargesDetails.FooterRow.FindControl("SqlDataSource3");
                SqlDataSource3.SelectParameters["RefDate"].DefaultValue = Convert.ToDateTime(txtIssueDate.Text).ToString("yyyy/MM/dd");
            }
            else
            {
                SqlDataSource SqlDataSource3 = (SqlDataSource)dgvChargesDetails.FooterRow.FindControl("SqlDataSource3");
                SqlDataSource3.SelectParameters["RefDate"].DefaultValue = txtIssueDate.Text;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoanDateTextChangeEventAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {}
    }
    #endregion [txtIssueDate_TextChanged]

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
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Loan Tenure must be between " + MinTenure + " - " + MaxTenure + " (Min-Max).');", true);
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
            if (txtIssueDate.Text.Trim() != "")
            {
                e.Command.Parameters["@RefDate"].Value = Convert.ToDateTime(txtIssueDate.Text).ToString("yyyy/MM/dd");
            }
            else
            {
                e.Command.Parameters["@RefDate"].Value = txtIssueDate.Text;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SqlDataSource3_SelectingEventAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [SqlDataSource3_Selecting]

    #region [Is Excess Amount]
    protected bool IsExcessAmount()
    {
        bool IsExcess = true;
        try
        {
            if (Convert.ToString(txtNetAmount.Text) != "")
            {
                if (Convert.ToDouble(txtNetAmount.Text) <= 0)
                {
                    txtNetAmountSanctioned.Text = "";
                    ClientScript.RegisterStartupScript(this.GetType(), "NetAmtAlert", "alert('Net Amount must be greater than zero.');", true);
                }
                else
                {
                    double maxSanctionAmount = 0;
                    double netSanctionedAmount = 0;

                    if (Convert.ToString(txtNetAmount.Text) != "")
                    {
                        maxSanctionAmount = Convert.ToDouble(txtNetAmount.Text);
                    }

                    if (Convert.ToString(txtNetAmountSanctioned.Text) != "")
                    {
                        netSanctionedAmount = Convert.ToDouble(txtNetAmountSanctioned.Text);
                    }

                    if (netSanctionedAmount > maxSanctionAmount)
                    {
                        if (Convert.ToString(Session["Valid"]) != "")
                        {
                            if (Convert.ToString(Session["Valid"]) != "Yes")
                            {
                                txtNetAmountSanctioned.Text = "";
                                IsExcess = false;
                            }
                            else
                            {
                                pnlLogin.Visible = false;
                            }
                        }
                        else
                        {
                            txtNetAmountSanctioned.Text = "";
                            IsExcess = false;
                        }
                    }
                    else
                    {
                        pnlLogin.Visible = false;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Session["Valid"] = "";
            ClientScript.RegisterStartupScript(this.GetType(), "IsExcessAmtAlert", "alert('" + ex.Message + "');", true);
        }
        return IsExcess;
    }
    #endregion [Is Excess Amount]

    #region [Calculate Net Amount]
    protected void CalculateNetAmount()
    {
        try
        {
            //string SanctionType = ddlSanctionType.SelectedValue;
            double MaxSanctionLoanAmt = 0;
            double LoanSanctionPercent = 0;
            double SanctionLoanAmtPerGram = 0;
            double GoldPricePerGram = 0;
            double NetWeight = 0;
            double GrossGoldValue = 0;
            double NetGoldValue = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();

            //fetching Sanction Loan Value (for different Sanction Types)
            strQuery = "SELECT LoanSanctionPercent, MaxSanctionLoanAmt, SanctionLoanAmtPerGram, GoldPricePerGram FROM tblLoanParameterSetting";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    LoanSanctionPercent = Convert.ToDouble(ds.Tables[0].Rows[0][0]);
                    MaxSanctionLoanAmt = Convert.ToDouble(ds.Tables[0].Rows[0][1]);
                    SanctionLoanAmtPerGram = Convert.ToDouble(ds.Tables[0].Rows[0][2]);
                    GoldPricePerGram = Convert.ToDouble(ds.Tables[0].Rows[0][3]);
                }
                else
                {
                    LoanSanctionPercent = 0;
                    MaxSanctionLoanAmt = 0;
                    SanctionLoanAmtPerGram = 0;
                    GoldPricePerGram = 0;
                }
            }
            else
            {
                LoanSanctionPercent = 0;
                MaxSanctionLoanAmt = 0;
                SanctionLoanAmtPerGram = 0;
                GoldPricePerGram = 0;
            }

            //if (ddlSanctionType.SelectedValue == "Amount")
            //{
            //    txtNetAmount.Text = Convert.ToString(MaxSanctionLoanAmt);
            //}
            //else if (ddlSanctionType.SelectedValue == "Percentage")
            //{
            //    NetWeight = Convert.ToDouble(txtNetWeight.Text);
            //    GrossGoldValue = (NetWeight * GoldPricePerGram);
            //    NetGoldValue = GrossGoldValue * (LoanSanctionPercent / 100);
            //    NetGoldValue = Math.Round(NetGoldValue, 2);
            //    txtNetAmount.Text = Convert.ToString(NetGoldValue);
            //}
            //else if (ddlSanctionType.SelectedValue == "Gram")
            //{
            //    NetWeight = Convert.ToDouble(txtNetWeight.Text);
            //    NetGoldValue = (NetWeight * SanctionLoanAmtPerGram);
            //    NetGoldValue = Math.Round(NetGoldValue, 2);
            //    txtNetAmount.Text = Convert.ToString(NetGoldValue);
            //}
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CalNetAmtAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Calculate Net Amount]

    #region Excess Login_Click
    protected void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            bool datasaved = false;
            conn = new SqlConnection(strConnString);
            conn.Close();
            //datasaved = false;
            //Session["Password"] = txtPassword.Text;
            if (ddlRefID.SelectedValue != "0" && ddlRefNo.SelectedValue != "0")
            {
                if (lblLogin.Text == "Login For excess loan amount")
                {
                    if (txtUserName.Text.Trim() != "" && txtPassword.Text.Trim() != "")
                    {
                        conn = new SqlConnection(strConnString);
                        conn.Open();

                        int QueryResult = 0;
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                        string LoginSucessOrNot = string.Empty;
                        string TempOrPerm = "T";
                        int LoginCount = 0;

                        //get userid and password for excess login
                        strQuery = "SELECT UserName, Password, UserID From UserDetails " +
                                    " INNER JOIN UserTypeDetails " +
                                    " ON UserDetails.UserTypeID=UserTypeDetails.UserTypeID " +
                                    "WHERE UserType='Special User'";

                        cmd = new SqlCommand(strQuery, conn, transaction);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);
                        UserName = ds.Tables[0].Rows[0][0].ToString();
                        Password = ds.Tables[0].Rows[0][1].ToString();
                        UserID = Convert.ToInt32(ds.Tables[0].Rows[0][2].ToString());
                        Session["User"] = UserName;
                        Session["Pass"] = Password;
                        string User = Convert.ToString(Session["User"]);
                        string Pass = Convert.ToString(Session["Pass"]);
                        //string password = Convert.ToString(Session["Password"]);
                        //authenticating user name and password
                        if (txtUserName.Text.Trim() == UserName && txtPassword.Text.Trim() == Password)
                        {
                            Session["Valid"] = "Yes";
                            lblLogin.Text = "LogOut";
                            pnlLogin.Visible = false;
                            chkLogin.Enabled = true;
                            chkLogin.Checked = false;
                            LoginSucessOrNot = "S";

                            if (Convert.ToString(Session["LogInCount"]) != null && Convert.ToString(Session["LogInCount"]) != "")
                            {
                                LoginCount += Convert.ToInt32(Session["LogInCount"]);
                                Session["LogInCount"] = LoginCount;
                            }
                            else
                            {
                                LoginCount = 1;
                                Session["LogInCount"] = LoginCount;
                            }
                            lblLoginMsg.Text = "";
                            txtNetAmountSanctioned.Focus();
                        }
                        else
                        {
                            pnlLogin.Visible = true;
                            chkLogin.Enabled = true;
                            Session["Valid"] = "No";
                            LoginSucessOrNot = "F";
                            
                            if (Convert.ToString(Session["LogInCount"]) != null && Convert.ToString(Session["LogInCount"]) != "")
                            {
                                LoginCount += Convert.ToInt32(Session["LogInCount"]);
                                Session["LogInCount"] = LoginCount;
                            }
                            else
                            {
                                LoginCount = 1;
                                Session["LogInCount"] = LoginCount;
                            }

                            lblLoginMsg.Text = "Invalid UserName or Password.";
                            txtPassword.Text = "";
                            txtPassword.Focus();
                            //ClientScript.RegisterStartupScript(this.GetType(), "LogValidAlert", "alert('User Name or Password is Invalid!');", true);
                        }

                        //LogInTime1 = DateTime.Now;
                        //String LogInTime = Convert.ToString(LogInTime1);
                        Session["LoginTime"] = DateTime.Now;
                        RefType = txtGoldLoanNo.Text.Trim();
                        RefID = ddlRefID.SelectedValue.ToString().Trim();
                        RefNo = ddlRefNo.SelectedValue.ToString().Trim();
                        GoldLoanNo = (RefType) + '/' + (RefNo) + '/' + (RefID);

                        if (LoginCount == 1)
                        {
                            //get maximum userid
                            strQuery = "SELECT MAX(LoginID) From tbl_GLSanctionDisburse_ExcessLoanUserDetails ";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            if (cmd.ExecuteScalar() != DBNull.Value)
                            {
                                SanctionLoginID = Convert.ToInt32(cmd.ExecuteScalar());
                            }
                            else
                            {
                                SanctionLoginID = 0;
                            }

                            SanctionLoginID += 1;
                            txtLoginID.Text = Convert.ToString(SanctionLoginID);

                            if (Convert.ToString(Session["LogInCount"]) != null && Convert.ToString(Session["LogInCount"]) != "")
                            {
                                LoginCount = Convert.ToInt32(Session["LogInCount"]);
                            }

                            if (Convert.ToString(Session["userID"]) != null && Convert.ToString(Session["userID"]) != "")
                            {
                                UserID = Convert.ToInt32(Session["userID"]);
                            }

                            //Insert Details into tbl_GLSanctionDisburse_ExcessLoanUserDetails
                            insertQuery = "INSERT INTO tbl_GLSanctionDisburse_ExcessLoanUserDetails VALUES(" + SanctionLoginID + "," + UserID + ",'" + DateTime.Now.Date.ToString("yyyy/MM/dd") + "','" + Convert.ToDateTime(Session["LoginTime"]).ToString("hh:mm:ss") + "', " +
                                            " '',''," + LoginCount + ",'" + LoginSucessOrNot + "','" + GoldLoanNo + "','" + TempOrPerm + "')";

                            cmd = new SqlCommand(insertQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();
                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }
                        else
                        {
                            //update table tbl_GLSanctionDisburse_ExcessLoanUserDetails
                            updateQuery = "update tbl_GLSanctionDisburse_ExcessLoanUserDetails set LoginTime='" + Convert.ToDateTime(Session["LoginTime"]).ToString("hh:mm:ss") + "', " +
                                            "NoofTimesLogin=" + LoginCount + ", LoginSuccessOrFail='" + LoginSucessOrNot + "', GoldLoanNo='" + GoldLoanNo + "' " +
                                           "where LoginID='" + txtLoginID.Text + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();
                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }

                        if (datasaved == true)
                        {
                            int ID = 0;
                            strQuery = "select max(ID) from tbl_GLSanctionDisburse_ExcessUserLoginDetails";
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

                            insertQuery = "INSERT INTO tbl_GLSanctionDisburse_ExcessUserLoginDetails VALUES(" + ID + ",'" + txtLoginID.Text + "','" + txtUserName.Text + "','" + txtPassword.Text + "')";
                            cmd = new SqlCommand(insertQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }
                    }

                    if (datasaved == true)
                    {
                        transaction.Commit();
                    }
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "LogAlert", "alert('Select Gold Loan No.');", true);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LogInClickAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion

    #region LoginCheckboxChangeEvent
    protected void chkLogin_CheckedChanged(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            string tag = string.Empty;

            if (txtNetAmount.Text.Trim() == "")
            {
                tag = "N";
            }
            else
            {
                if (Convert.ToInt32(txtNetAmount.Text) == 0)
                {
                    tag = "N";
                }
                else
                {
                    tag = "Y";
                }
            }

            if (tag == "N")
            {
                pnlLogin.Visible = false;
                chkLogin.Checked = false;
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Max Loan Amount cannot be blank. Please Select Scheme Name.');", true);
                txtNetAmountSanctioned.Focus();
            }
            else
            {
                if (lblLogin.Text == "Login For excess loan amount")
                {
                    if (chkLogin.Checked)
                    {
                        pnlLogin.Visible = true;
                        chkLogin.Checked = false;
                        lblMessageText.Text = "";
                        txtNetAmountSanctioned.Text = "";
                    }
                }

                else if (lblLogin.Text == "LogOut")
                {
                    if (chkLogin.Checked)
                    {
                        Session["Valid"] = "";
                        DateTime LoginTime1 = Convert.ToDateTime(Session["LoginTime"]);
                        DateTime LogOutTime1 = DateTime.Now;

                        TimeSpan ts;
                        ts = LogOutTime1.Subtract(LoginTime1);

                        updateQuery = "UPDATE tbl_GLSanctionDisburse_ExcessLoanUserDetails SET LogOutTime='" + LogOutTime1.ToString("hh:mm:ss") + "', " +
                                        " TotalTime = '" + ts + "' WHERE LoginID=" + txtLoginID.Text.Trim() + "";
                        cmd = new SqlCommand(updateQuery, conn);
                        int QueryResult = cmd.ExecuteNonQuery();
                        if (QueryResult > 0)
                        {
                            lblLogin.Text = "Login For excess loan amount";
                            pnlLogin.Visible = false;
                            chkLogin.Checked = false;
                        }
                    }
                }
                else
                {
                    pnlLogin.Visible = false;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion

    #region [txtIssueDate_ServerValidate]
    protected void txtIssueDate_ServerValidate(object sender, ServerValidateEventArgs e)
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

            if (Convert.ToString(txtIssueDate.Text.Trim()) != "")
            {
                if (Convert.ToDateTime(txtIssueDate.Text) < dtStartDate || Convert.ToDateTime(txtIssueDate.Text) > dtEndDate)
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
            ClientScript.RegisterStartupScript(this.GetType(), "IssueDateValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [txtIssueDate_ServerValidate}

    #region [txtNetAmount_ServerValidate]
    protected void txtNetAmount_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (Convert.ToString(txtNetAmount.Text.Trim()) != "")
            {
                if (Convert.ToDouble(txtNetAmount.Text) <= 0)
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
            ClientScript.RegisterStartupScript(this.GetType(), "NetAmountValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [txtNetAmount_ServerValidate]

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

    #region [Fill Scheme Name]
    protected void FillSchemeName()
    {
        try
        {
            ddlSchemeName.DataSource = null;
            ddlSchemeName.Items.Clear();

            conn = new SqlConnection(strConnString);
            strQuery = "SELECT DISTINCT ID, SchemeName FROM tbl_GLLoanSchemeMaster WHERE Status='Active' ORDER BY SchemeName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlSchemeName.DataSource = dt;
            ddlSchemeName.DataValueField = "ID";
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

    #region [Clear Fields As Per Scheme]
    protected void ClearFieldsAsPerScheme()
    {
        try
        {
            if (ddlSchemeName.SelectedValue != "0")
            {
                //enable controls
                txtLoanTenure.ReadOnly = false;
                txtDueDate.Enabled = true;
                txtNetAmountSanctioned.ReadOnly = false;
                txtLoanTenure.BackColor = System.Drawing.Color.White;
                txtNetAmountSanctioned.BackColor = System.Drawing.Color.White;
                ////focus on control
                //txtLoanTenure.Focus();

                if (txtSchemeType.Text.Trim() == "MI")
                {
                    txtDueDate.Enabled = true;
                }
                else
                {
                    txtDueDate.Enabled = false;
                }
            }
            else
            {
                //clear data
                txtLoanTenure.Text = "";
                txtNetAmount.Text = "";
                txtEMI.Text = "";
                txtDueDate.Text = "";
                txtNetAmountSanctioned.Text = "";
                //disable controls
                txtLoanTenure.ReadOnly = true;
                txtDueDate.Enabled = false;
                txtNetAmountSanctioned.ReadOnly = true;
                txtLoanTenure.BackColor = System.Drawing.Color.Gainsboro;
                txtNetAmountSanctioned.BackColor = System.Drawing.Color.Gainsboro;
                ////focus on control
                //ddlSchemeName.Focus();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "enableControlAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Clear Fields As Per Scheme]

    #region [Clear Fields As Per Scheme On Fill]
    protected void ClearFieldsAsPerSchemeOnFill()
    {
        try
        {
            //enable controls
            txtLoanTenure.ReadOnly = false;
            txtDueDate.Enabled = true;
            txtNetAmountSanctioned.ReadOnly = false;
            txtLoanTenure.BackColor = System.Drawing.Color.White;
            txtNetAmountSanctioned.BackColor = System.Drawing.Color.White;
            ////focus on control
            //txtLoanTenure.Focus();

            if (txtSchemeType.Text.Trim() == "MI")
            {
                txtDueDate.Enabled = true;
            }
            else
            {
                txtDueDate.Enabled = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "enableControlAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Clear Fields As Per Scheme On Fill]

    #region [Get Scheme Details]
    protected void GetSchemeDetails()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            string ID = string.Empty;
            string SchemeName = string.Empty;
            string SchemeType = string.Empty;
            string LTV = string.Empty;
            string MinTenure = string.Empty;
            string MaxTenure = string.Empty;
            string InterestRate = string.Empty;

            strQuery = "SELECT ID, SchemeName, SchemeType, LTV, MinTenure, MaxTenure, InterestRate FROM tbl_GLLoanSchemeMaster " +
                            "WHERE ID='" + ddlSchemeName.SelectedValue + "'";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                if (ds.Tables[0].Rows[0][0] != DBNull.Value)
                {
                    ID = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    SchemeName = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    SchemeType = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    LTV = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    MinTenure = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    MaxTenure = Convert.ToString(ds.Tables[0].Rows[0][5]);
                    InterestRate = Convert.ToString(ds.Tables[0].Rows[0][6]);
                }
            }

            txtSchemeID.Text = ID;
            txtSchemeName.Text = SchemeName;
            txtSchemeType.Text = SchemeType;
            txtLTV.Text = LTV;
            txtMinTenure.Text = MinTenure;
            txtMaxTenure.Text = MaxTenure;
            txtInterestRate.Text = InterestRate;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CalcMaxLoanAmt_Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion  [Get Scheme Details]

    #region [Calculate Max Loan Amount]
    protected void CalculateMaxLoanAmount()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            double GoldPricePerGram = 0;
            double GoldNetWeight = 0;
            double LTV = 0;
            double MaxLoanAmount = 0;
            string strMaxLoanAmount = string.Empty;

            //checking Loan Parameter Setting
            strQuery = "SELECT count(*) FROM tblLoanParameterSetting";
            cmd = new SqlCommand(strQuery, conn);
            int existcount = Convert.ToInt32(cmd.ExecuteScalar());

            if (existcount == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Please Enter Loan Parameter Settings under Utilities Section.');", true);
            }
            else
            {
                // ---------------------------- Calculate Max Loan Amount --------------------------------------------

                if (ddlSchemeName.SelectedValue != "0" && txtNetWeight.Text.Trim() != "")
                {
                    //fetching Gold Price Per Gram
                    strQuery = "SELECT GoldPricePerGram FROM tblLoanParameterSetting";
                    cmd = new SqlCommand(strQuery, conn);
                    GoldPricePerGram = Convert.ToDouble(cmd.ExecuteScalar());

                    //fetching LTV (Loan-To-Value)
                    strQuery = "SELECT LTV FROM tbl_GLLoanSchemeMaster " +
                                    "WHERE ID='" + ddlSchemeName.SelectedValue + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    LTV = Convert.ToDouble(cmd.ExecuteScalar());

                    //getting Gold Net Weight
                    if (txtNetWeight.Text.Trim() != "")
                    {
                        if (Convert.ToDouble(txtNetWeight.Text.Trim()) != 0)
                        {
                            GoldNetWeight = Convert.ToDouble(txtNetWeight.Text.Trim());
                        }
                    }

                    //calculating Gold Value
                    MaxLoanAmount = (GoldNetWeight * GoldPricePerGram) * LTV / 100;
                    strMaxLoanAmount = Convert.ToString(Decimal.Ceiling(Convert.ToDecimal(MaxLoanAmount)));
                    txtNetAmount.Text = strMaxLoanAmount;
                }
                else
                {
                    txtNetAmount.Text = "0";
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CalcMaxLoanAmt_Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Calculate Max Loan Amount]

    #region [Calculate EMI]
    protected void CalculateEMI(double LoanAmount, double IntRate, double LoanTenure)
    {
        try
        {
            //formulae -> (LXI) X (1+I)^ M/[(1+I)^ M]-1
            double L = 0;
            double I = 0;
            double M = 0;
            int EMI = 0;

            L = LoanAmount;
            I = (IntRate / 12) / 100;
            M = LoanTenure;

            double A = L * I;
            double B = Math.Pow((1 + I), M);
            double C = (Math.Pow((1 + I), M)) - 1;
            double ans = 0;
            if (C != 0)
            {
                ans = A * (B / C);
            }

            EMI = Convert.ToInt32(Decimal.Round(Convert.ToDecimal(ans)));
            txtEMI.Text = Convert.ToString(EMI);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "CalcEMI_Alert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [Calculate EMI]

    #region [Upload Proof Of Ownership]
    protected void btnUploadProofOfOwnership_Click(object sender, EventArgs e)
    {
        try
        {
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
                DropDownList ddlChargesName = (DropDownList)dgvChargesDetails.FooterRow.FindControl("ddlChargesName");
                ddlChargesName.Focus();
            }
            else
            {
                txtProofOfOwnershipPath.Text = "";
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

    #region[ddlcheqNEFTDD_SelectedIndexChanged]
    protected void ddlcheqNEFTDD_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlcheqNEFTDD.SelectedItem.Text == "Cheque")
            {
               
                txtChequeNo.MaxLength = 10;
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";


            }
            else if (ddlcheqNEFTDD.SelectedItem.Text == "NEFT")
            {
                txtChequeNo.MaxLength = 20;
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
            }
            else
            {
                txtChequeNo.MaxLength = 10;
                txtChequeNo.Text = "";
                txtChequeDate.Text = "";
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Cheque_EventAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[ddlcheqNEFTDD_SelectedIndexChanged]
    protected void txtChequeNo_TextChanged(object sender, EventArgs e)
    {

        //if (ddlcheqNEFTDD.SelectedItem.Text == "Cheque" && txtChequeNo.Text.Length<3)
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 3 char.');", true);
        //}
       // else 
        if (ddlcheqNEFTDD.SelectedItem.Text == "NEFT" && txtChequeNo.Text.Length < 15)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 15 char.');", true);
            }
            else if (ddlcheqNEFTDD.SelectedItem.Text == "DD" && txtChequeNo.Text.Length < 6)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "alert Message", "javascript:window.alert('Enter minimum 6 char.');", true);
                }


    }
}