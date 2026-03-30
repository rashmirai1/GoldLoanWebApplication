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

public partial class GLKYCvsBankInterestDetailsReport : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
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

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

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
            strQuery = "select BID, BranchName from tblCompanyBranchMaster where Status='Active'";
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
            rdlReportType.ClearSelection();
            ddlBankUniqueID.Items.Clear();
            BindBranch();

            //bind gridview customers details
            dt = new DataTable();
            dt.Columns.Add("GoldLoanNo", typeof(string));
            dt.Columns.Add("CustName", typeof(string));
            lblMsg.Text = "";


            //ShowNoResultFound(dt, dgvCustomerDetails);

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
            lblMsg.Text = "";
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
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Unique ID.');", true);
            }
            else
            {
                if (rdlReportType.SelectedValue == "Single")
                {
                    strQuery = "SELECT Count(*) " +
                                  "FROM tbl_GLBankGold_BasicDetails " +
                                  "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                          "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                  "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                          "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                  "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                          "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                  "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                          "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                  "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                          "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                  "WHERE tbl_GLBankGold_BasicDetails.UniqueBankCustomerId='" + ddlBankUniqueID.SelectedValue + "' " +
                                  "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                          "AppFName+' ' + AppMName+' ' +AppLName, " +
                                          "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                          "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                          "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId ";


                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                    Session["Count"] = Convert.ToString(existcount);


                    string strBnkUnqId = ddlBankUniqueID.SelectedValue;
                    string strBranch = ddlBranch.SelectedValue;
                    if (Convert.ToInt32(Session["Count"]) > 0)
                    {
                        string pageurl = "GLKYCvsBankInterestViewDetailedReport.aspx?C=" + strBnkUnqId + "&&RT=S&&B=" + strBranch;
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + pageurl + "');", true);
                    }
                    else
                    {
                        lblMsg.Text = "   No Records To Show";

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

                        strQuery = "SELECT Count(*) " +
                                    "FROM tbl_GLBankGold_BasicDetails " +
                                    "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                            "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                    "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                            "AppFName+' ' + AppMName+' ' +AppLName, " +
                                            "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);
                    }
                    else if (strAccountStatus == "Open")
                    {
                        strAccountStatus = "O";
                        strQuery = "SELECT Count(*) " +
                                "FROM tbl_GLBankGold_BasicDetails " +
                                "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                        "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "AND tbl_GLSanctionDisburse_Status.GLStatus=' Open ' " +
                                "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                        "AppFName+' ' + AppMName+' ' +AppLName, " +
                                        "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                        "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                        "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId ";
                            

                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);
                    }
                    else if (strAccountStatus == "Close")
                    {
                        strAccountStatus = "C";
                        strQuery = "SELECT Count(*) " +                        
                                "FROM tbl_GLBankGold_BasicDetails " +
                                "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                        "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                        "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "AND tbl_GLSanctionDisburse_Status.GLStatus='Close ' " +
                                "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                        "AppFName+' ' + AppMName+' ' +AppLName, " +
                                        "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                        "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                        "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["Count"] = Convert.ToString(existcount);

                    }
                    if (Convert.ToInt32(Session["Count"])> 0)
                    {
                        string strBranch = ddlBranch.SelectedValue;
                        string pageurl = "GLKYCvsBankInterestViewDetailedReport.aspx?C=" + strAccountStatus + "&&RT=A&&B=" + strBranch;
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

    #region [Bind Bank Unique ID dropdownlist]
    protected void FillBankUniqueID()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlBranch.SelectedValue != "0")
            {
                ddlBankUniqueID.DataSource = null;

                string query = "SELECT distinct BnkUniqueID=tbl_GLBankGold_BasicDetails.UniqueBankCustomerId, BnkUniqueID=tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                                "FROM tbl_GLBankGold_BasicDetails " +
                                "INNER JOIN tbl_GLBankGold_AppDetails " +
                                        "ON  tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "WHERE tbl_GLBankGold_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "AND tbl_GLSanctionDisburse_Status.GLStatus='Open' " +
                                "AND (tbl_GLBankGold_BasicDetails.UniqueBankCustomerId !=null OR tbl_GLBankGold_BasicDetails.UniqueBankCustomerId !='')";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlBankUniqueID.DataSource = dt;
                ddlBankUniqueID.DataValueField = "BnkUniqueID";
                ddlBankUniqueID.DataTextField = "BnkUniqueID";
                ddlBankUniqueID.DataBind();
                ddlBankUniqueID.Items.Insert(0, new ListItem("--Select Bank Unique ID--", "0"));
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

    #region [Bind GridView Customer Details]
    protected void BindCustomerDetails()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlBranch.SelectedValue != "0")
            {
                ddlBankUniqueID.DataSource = null;

                string query = "SELECT distinct tbl_GLBankGold_BasicDetails.BankGoldID, BnkUniqueID=tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                               "FROM tbl_GLBankGold_BasicDetails " +
                               "INNER JOIN tbl_GLBankGold_AppDetails " +
                                       "ON  tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                               "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                           "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                               "WHERE tbl_GLBankGold_BasicDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                               "AND tbl_GLSanctionDisburse_Status.GLStatus='Open' ";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    //dgvCustomerDetails.DataSource = dt;
                    //dgvCustomerDetails.DataBind();
                }
                else
                {
                    dt = new DataTable();
                    dt.Columns.Add("GoldLoanNo", typeof(string));
                    dt.Columns.Add("CustName", typeof(string));

                    //ShowNoResultFound(dt, dgvCustomerDetails);
                }
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Bind GridView Document Details]

    //#region [dgvCustomerDetails_PageIndexChanging]
    //protected void dgvCustomerDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    try
    //    {
    //        this.dgvCustomerDetails.PageIndex = e.NewPageIndex;
    //        BindCustomerDetails();
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "DGVDocPgChgAlert", "alert('" + ex.Message + "');", true);
    //    }
    //}
    //#endregion [dgvCustomerDetails_PageIndexChanging]

    #region [ShowNoResultFound]
    protected void ShowNoResultFound(DataTable source, GridView gv)
    {
        // create a new blank row to the DataTable
        source.Rows.Add(source.NewRow());

        // Bind the DataTable which contain a blank row to the GridView
        gv.DataSource = source;
        gv.DataBind();

        // Get the total number of columns in the GridView to know what the Column Span should be
        int columnsCount = gv.Columns.Count;
    }
    #endregion [ShowNoResultFound]

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

    #region [Disable controls]
    protected void DisableControls()
    {
        try
        {
            ddlBankUniqueID.Enabled = false;
            ddlAccountStatus.Enabled = false;
            //dgvCustomerDetails.Enabled = false;
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
                //Bind Bank Unique ID dropdownlist
                FillBankUniqueID();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                ddlBankUniqueID.Enabled = true;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = false;
            }
            else if (rdlReportType.SelectedValue == "Multiple")
            {
                //clear dropdownlist Bank Unique ID
                ddlBankUniqueID.Items.Clear();

                //Bind Customer Name Gridview
                BindCustomerDetails();

                //enabling/disabling controls
                ddlBankUniqueID.Enabled = false;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = true;
            }
            else if (rdlReportType.SelectedValue == "All")
            {
                //clear dropdownlist CustomerName
                ddlBankUniqueID.Items.Clear();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                ddlBankUniqueID.Enabled = false;
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
                if (ddlBankUniqueID.SelectedValue == "0")
                {
                    Val = 3;
                }
                else
                {
                    Val = 0;
                }
            }
            else if (rdlReportType.SelectedValue == "Multiple")
            {
                //if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
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
                //else
                //{
                //    Val = 0;
                //}
            }
            else if (rdlReportType.SelectedValue == "All")
            {
                Val = 0;
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
}