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

public partial class GLSanctionDisburseDetailsReport : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringTesting"].ConnectionString;
    string strQuery = string.Empty;
    string selectQuery = string.Empty;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    DataTable dt;

    public string loginDate;
    public string expressDate;
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

                //binding Branch
                BindBranch();

                //disabling controls
                DisableControls();
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

    #region [Bind Branch]
    protected void BindBranch()
    {
        try
        {
            ddlBranch.DataSource = null;
            strQuery = "select BID, BranchName from tblCompanyBranchMaster";
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            ddlBranch.DataSource = ds;
            ddlBranch.DataTextField = "BranchName";
            ddlBranch.DataValueField = "BID";
            ddlBranch.DataBind();
            ddlBranch.Items.Insert(0, new ListItem("--Select Branch--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindBranchAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Bind Branch]

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

    #region [ClearData]
    protected void ClearData()
    {
        try
        {
            txtIssueDateFrom.Text = "";
            txtIssueDateTo.Text = "";
            rdlReportType.ClearSelection();
            ddlCustomerName.Items.Clear();
            lblMsg.Text = "";
            BindBranch();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ClearData]

    #region [btnShowDetails_Click]
    protected void btnShowDetails_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            int Val = ValidateData();

            if (Val == 1)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch.');", true);
            }
            else if (Val == 2)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Report Type.');", true);
            }
            else if (Val == 3)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Loan From Date.');", true);
            }
            else if (Val == 4)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Loan To Date.');", true);
            }
            else if (Val == 5)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Customer.');", true);
            }
            else
            {
                if (rdlReportType.SelectedValue == "Single")
                {
                    strQuery = "SELECT Count(*) " +
                                   "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                   "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                           "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                   "INNER JOIN tbl_GLKYC_AddressDetails " +
                                           "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                   "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                           "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                   "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                           "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                   "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                           "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                   "INNER JOIN tblStateMaster " +
                                           "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                   "INNER JOIN tblCityMaster " +
                                           "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                   "INNER JOIN tblZonemaster " +
                                           "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                   "INNER JOIN tblAreaMaster " +
                                           "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                   "INNER JOIN UserDetails " +
                                           "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                                   "WHERE tbl_GLKYC_BasicDetails.KYCID='" + ddlCustomerName.SelectedValue + "' ";

                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                    if (existcount > 0)
                    {
                        string strCustId = ddlCustomerName.SelectedValue;
                        string strBranch = ddlBranch.SelectedValue;
                        string pageurl = "GLSanctionDisburseViewDetailedReport.aspx?C=" + strCustId + "&&RT=S&&F=" + txtIssueDateFrom.Text + "&&T=" + txtIssueDateTo.Text + "&&B=" + ddlBranch.SelectedValue;
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + pageurl + "');", true);
                    }
                    else
                    {

                        lblMsg.Text = "No Records To Show";
                    }
                }
                else if (rdlReportType.SelectedValue == "Multiple")
                {

                }
                else if (rdlReportType.SelectedValue == "All")
                {
                    string strAccountStatus = ddlAccountStatus.SelectedValue;
                    if (strAccountStatus == "All")
                    {
                        strAccountStatus = "A";
                        strQuery = "SELECT Count(*)" +
                                     "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                    "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLKYC_AddressDetails " +
                                            "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                    "INNER JOIN tblStateMaster " +
                                            "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                    "INNER JOIN tblCityMaster " +
                                            "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                    "INNER JOIN tblZonemaster " +
                                            "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                    "INNER JOIN tblAreaMaster " +
                                            "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                    "INNER JOIN UserDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                                   "WHERE  IssueDate BETWEEN '" + Convert.ToDateTime(txtIssueDateFrom.Text).ToString("yyyy/MM/dd") + "' " +
                                                    "AND '" + Convert.ToDateTime(txtIssueDateTo.Text).ToString("yyyy/MM/dd") + "' " +
                                    "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);
                    }
                    else if (strAccountStatus == "Open")
                    {
                        strAccountStatus = "O";
                        strQuery = "SELECT Count(*) " +
                                "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLKYC_AddressDetails " +
                                        "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                "INNER JOIN tblStateMaster " +
                                        "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                "INNER JOIN tblCityMaster " +
                                        "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                "INNER JOIN tblZonemaster " +
                                        "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                "INNER JOIN tblAreaMaster " +
                                        "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                "INNER JOIN UserDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                               "WHERE  IssueDate BETWEEN '" + Convert.ToDateTime(txtIssueDateFrom.Text).ToString("yyyy/MM/dd") + "' " +
                                                "AND '" + Convert.ToDateTime(txtIssueDateTo.Text).ToString("yyyy/MM/dd") + "' " +
                                "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "AND tbl_GLSanctionDisburse_Status.GLStatus='Open' ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);
                    }
                    else if (strAccountStatus == "Close")
                    {
                        strAccountStatus = "C";
                        strQuery = "SELECT Count(*) " +
                               "FROM tbl_GLSanctionDisburse_BasicDetails " +
                               "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                       "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                               "INNER JOIN tbl_GLKYC_AddressDetails " +
                                       "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                               "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                       "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                               "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                       "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                               "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                       "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                               "INNER JOIN tblStateMaster " +
                                       "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                               "INNER JOIN tblCityMaster " +
                                       "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                               "INNER JOIN tblZonemaster " +
                                       "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                               "INNER JOIN tblAreaMaster " +
                                       "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                               "INNER JOIN UserDetails " +
                                       "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                              "WHERE  IssueDate BETWEEN '" + Convert.ToDateTime(txtIssueDateFrom.Text).ToString("yyyy/MM/dd") + "' " +
                                               "AND '" + Convert.ToDateTime(txtIssueDateTo.Text).ToString("yyyy/MM/dd") + "' " +
                               "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                               "AND tbl_GLSanctionDisburse_Status.GLStatus='Close' ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);

                    }
                    if (Convert.ToInt32(Session["Count"]) > 0)
                    {
                        string strBranch = ddlBranch.SelectedValue;
                        string pageurl = "GLSanctionDisburseViewDetailedReport.aspx?C=" + strAccountStatus + "&&RT=A&&F=" + txtIssueDateFrom.Text + "&&T=" + txtIssueDateTo.Text + "&&B=" + ddlBranch.SelectedValue;
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + pageurl + "');", true);
                    }
                    else
                    {
                        lblMsg.Text = "No Records To Show";


                    }
                }

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowDetailsAlert", "alert('" + ex.Message + "');");
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [btnShowDetails_Click]

    #region [Bind Customer Name dropdownlist]
    protected void FillCustomerName()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlBranch.SelectedValue != "0" && txtIssueDateFrom.Text.Trim() != "" && txtIssueDateTo.Text.Trim() != "")
            {
                ddlCustomerName.DataSource = null;

                string query = "Select distinct tbl_GLKYC_BasicDetails.KYCID, Name=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo +' - '+ AppFName+' ' +AppMName+' ' + AppLName " +
                                "from tbl_GLSanctionDisburse_BasicDetails " +
                                "inner join tbl_GLKYC_BasicDetails " +
                                        "on  tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                "inner join tbl_GLKYC_ApplicantDetails " +
                                        "on  tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "where tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "and tbl_GLSanctionDisburse_BasicDetails.IssueDate between '" + DateTime.Parse(txtIssueDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtIssueDateTo.Text).ToString("yyyy/MM/dd") + "'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlCustomerName.DataSource = dt;
                ddlCustomerName.DataValueField = "KYCID";
                ddlCustomerName.DataTextField = "Name";
                ddlCustomerName.DataBind();
                ddlCustomerName.Items.Insert(0, new ListItem("--Select Customer Name--", "0"));
            }
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
    #endregion [Bind Customer Name dropdownlist]

    #region [ReportType_SelectedIndexChanged]
    protected void rdlReportType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblMsg.Text = "";

            OnReportTypeChange();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ReportType_SelectedIndexChanged]

    #region [Branch_SelectedIndexChanged]
    protected void ddlBranch_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            lblMsg.Text = "";
            OnReportTypeChange();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Branch_SelectedIndexChanged]

    #region [txtIssueDateFrom_TextChanged]
    protected void txtIssueDateFrom_TextChanged(object sender, EventArgs e)
    {
        try
        {
            lblMsg.Text = "";
            OnReportTypeChange();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtIssueDateFrom_TextChanged]

    #region [txtIssueDateTo_TextChanged]
    protected void txtIssueDateTo_TextChanged(object sender, EventArgs e)
    {
        try
        {
            lblMsg.Text = "";
            if (txtIssueDateTo.Text != "" && txtIssueDateFrom.Text != "")
            {
                DateTime FromDate = Convert.ToDateTime(txtIssueDateFrom.Text);
                DateTime ToDate = Convert.ToDateTime(txtIssueDateTo.Text);
                if (ToDate.CompareTo(FromDate) < 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Payment Date From  must be less than Payment To Date.');", true);
                    txtIssueDateTo.Text = "";
                    txtIssueDateFrom.Text = "";

                }
                else
                {
                    OnReportTypeChange();
                
                }

            }


            
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtIssueDateTo_TextChanged]

    #region [Disable controls]
    protected void DisableControls()
    {
        try
        {
            txtIssueDateFrom.Enabled = false;
            txtIssueDateTo.Enabled = false;
            ddlCustomerName.Enabled = false;
            ddlAccountStatus.Enabled = false;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Disable controls]

    #region [On Report Type Change]
    protected void OnReportTypeChange()
    {
        try
        {
            if (rdlReportType.SelectedValue == "Single")
            {
                lblMsg.Text = "";
                //Bind Customer Name dropdownlist
                FillCustomerName();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                txtIssueDateFrom.Enabled = true;
                txtIssueDateTo.Enabled = true;
                ddlCustomerName.Enabled = true;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = false;
            }
            else if (rdlReportType.SelectedValue == "Multiple")
            {
                lblMsg.Text = "";
                //clear dropdownlist CustomerName
                ddlCustomerName.Items.Clear();

                ////Bind Customer Name Gridview
                //BindCustomerDetails();

                //enabling/disabling controls
                txtIssueDateFrom.Enabled = true;
                txtIssueDateTo.Enabled = true;
                ddlCustomerName.Enabled = false;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = true;
            }
            else if (rdlReportType.SelectedValue == "All")
            {
                lblMsg.Text = "";
                //clear dropdownlist CustomerName
                ddlCustomerName.Items.Clear();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                txtIssueDateFrom.Enabled = true;
                txtIssueDateTo.Enabled = true;
                ddlCustomerName.Enabled = false;
                ddlAccountStatus.Enabled = true;
                //dgvCustomerDetails.Enabled = false;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [On Report Type Change]

    #region [Validate Data]
    protected int ValidateData()
    {
        int Val = 0;
        try
        {
            if (ddlBranch.SelectedValue == "0")
            {
                Val = 1;
            }
            else
            {
                Val = 0;
            }

            if (rdlReportType.SelectedValue == "")
            {
                Val = 2;
            }
            else
            {
                Val = 0;
            }

            if (rdlReportType.SelectedValue == "Single")
            {
                if (txtIssueDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtIssueDateTo.Text.Trim() == "")
                {
                    Val = 4;
                }
                else if (ddlCustomerName.SelectedValue == "0")
                {
                    Val = 5;
                }
                else
                {
                    Val = 0;
                }
            }
            else if (rdlReportType.SelectedValue == "Multiple")
            {
                if (txtIssueDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtIssueDateTo.Text.Trim() == "")
                {
                    Val = 4;
                }
                //else if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
                //{
                //    foreach (GridViewRow row in dgvCustomerDetails.Rows)
                //    {
                //        CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                //        if (chk != null && chk.Checked)
                //        {
                //            Val = 5;
                //        }
                //    }
                //}
                else
                {
                    Val = 0;
                }
            }
            else if (rdlReportType.SelectedValue == "All")
            {
                if (txtIssueDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtIssueDateTo.Text.Trim() == "")
                {
                    Val = 4;
                }
                else
                {
                    Val = 0;
                }
            }
            else
            {
                Val = 0;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
        return Val;
    }
    #endregion [Validate Data]

    //#region [Bind GridView]
    //protected void BindDGVDetails()
    //{
    //    try
    //    {
    //        //getting FYear ID
    //        int FYearID =0;
    //        if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
    //        {
    //            FYearID = Convert.ToInt32(Session["FYearID"]);
    //        }

    //        //getting details
    //        strQuery = "select distinct tbl_GLSanctionDisburse_BasicDetails.SDID, tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo, " +
    //                            "ApplicantName=AppFName+ ' ' + AppMName+ ' ' +AppLName, " +
    //                            "IssueDate=convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103), tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned, " +
    //                            "tbl_GLSanctionDisburse_Status.GLStatus, tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight, " +
    //                            "tbl_GLSanctionDisburse_GoldValueDetails.Deduction, tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight, " +
    //                            "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue, tbl_GLSanctionDisburse_GoldValueDetails.SanctionType " + 
    //                   "from tbl_GLSanctionDisburse_BasicDetails " +
    //                   "inner join  tbl_GLSanctionDisburse_Status " +
    //                            "on tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
    //                   "inner join  tbl_GLKYC_ApplicantDetails " +
    //                            "on tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
    //                   "inner join tbl_GLSanctionDisburse_GoldValueDetails " +
    //                            "on tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
    //                   "where  IssueDate between '" + Convert.ToDateTime(txtIssueDateFrom.Text).ToString("yyyy/MM/dd") + "' " +
    //                                        "and '" + Convert.ToDateTime(txtIssueDateTo.Text).ToString("yyyy/MM/dd") + "' " +
    //                   "and tbl_GLSanctionDisburse_BasicDetails.FYID='" + FYearID + "'";

    //        conn = new SqlConnection(strConnString);
    //        da = new SqlDataAdapter(strQuery, conn);
    //        ds = new DataSet();
    //        da.Fill(ds);
    //        dgvDetails.DataSource = ds;
    //        dgvDetails.DataBind();
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
    //    }
    //}
    //#endregion [Bind GridView]

    //#region [dgvDetails_PageIndexChanging]
    //protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    try
    //    {
    //        this.dgvDetails.PageIndex = e.NewPageIndex;
    //        BindDGVDetails();
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
    //    }
    //}
    //#endregion [dgvDetails_PageIndexChanging]
}