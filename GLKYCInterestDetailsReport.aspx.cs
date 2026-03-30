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

public partial class GLKYCInterestDetailsReport : System.Web.UI.Page
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
            txtPaymentDateFrom.Text = "";
            txtPaymentDateTo.Text = "";
            rdlReportType.ClearSelection();
            ddlCustomerName.Items.Clear();
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
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Payment From Date.');", true);
            }
            else if (Val == 4)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Payment To Date.');", true);
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
                              "FROM tbl_GLEMI_InterestJVDetails " +
                              "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                      "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                              "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                      "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                              "INNER JOIN UserDetails " +
                                      "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                              "INNER JOIN tblNarrationMaster " +
                                      "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                              "WHERE tbl_GLKYC_BasicDetails.KYCID='" + ddlCustomerName.SelectedValue + "' ";

                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                    Session["existcount"] = Convert.ToString(existcount);
                    if (Convert.ToInt32(Session["existcount"]) > 0)
                    {

                        string strCustId = ddlCustomerName.SelectedValue;
                        string strBranch = ddlBranch.SelectedValue;
                        string pageurl = "GLKYCInterestViewDetailedReport.aspx?C=" + strCustId + "&&RT=S&&B=" + strBranch + "&&F=" + txtPaymentDateFrom.Text + "&&T=" + txtPaymentDateTo.Text;
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

                        strQuery = "SELECT Count(*) " +
                                 "FROM tbl_GLEMI_InterestJVDetails " +
                                 "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                         "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                 "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                         "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                 "INNER JOIN UserDetails " +
                                         "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                                 "INNER JOIN tblNarrationMaster " +
                                         "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                                 "WHERE tbl_GLEMI_InterestJVDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                 "AND PaymentDate between '" + DateTime.Parse(txtPaymentDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtPaymentDateTo.Text).ToString("yyyy/MM/dd") + "' ";
                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["existcount"] = Convert.ToString(existcount);
                    }
                    else if (strAccountStatus == "Open")
                    {
                        strAccountStatus = "O";

                        strQuery = "SELECT Count(*) " +
                                 "FROM tbl_GLEMI_InterestJVDetails " +
                                 "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                         "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                 "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                         "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                 "INNER JOIN UserDetails " +
                                         "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                                 "INNER JOIN tblNarrationMaster " +
                                         "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                                 "WHERE tbl_GLEMI_InterestJVDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                 "AND PaymentDate between '" + DateTime.Parse(txtPaymentDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtPaymentDateTo.Text).ToString("yyyy/MM/dd") + "' " +
                                 "AND tbl_GLSanctionDisburse_Status.GLStatus='Open' ";

                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["existcount"] = Convert.ToString(existcount);
                                
                    }
                    else if (strAccountStatus == "Close")
                    {
                        strAccountStatus = "C";

                        strQuery = "SELECT Count(*) " +
                              "FROM tbl_GLEMI_InterestJVDetails " +
                              "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                      "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                              "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                      "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                              "INNER JOIN UserDetails " +
                                      "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                              "INNER JOIN tblNarrationMaster " +
                                      "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                              "WHERE tbl_GLEMI_InterestJVDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                              "AND PaymentDate between '" + DateTime.Parse(txtPaymentDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtPaymentDateTo.Text).ToString("yyyy/MM/dd") + "' " +
                              "AND tbl_GLSanctionDisburse_Status.GLStatus='Close' ";

                        cmd = new SqlCommand(strQuery, conn);
                        int existcount = Convert.ToInt32(cmd.ExecuteScalar());
                        Session["existcount"] = Convert.ToString(existcount);
                    }

                    if (Convert.ToInt32(Session["existcount"]) > 0)
                    {

                        string strBranch = ddlBranch.SelectedValue;
                        string pageurl = "GLKYCInterestViewDetailedReport.aspx?C=" + strAccountStatus + "&&RT=A&&B=" + strBranch + "&&F=" + txtPaymentDateFrom.Text + "&&T=" + txtPaymentDateTo.Text;
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
    }
    #endregion [btnShowDetails_Click]

    #region [Bind Customer Name dropdownlist]
    protected void FillCustomerName()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlBranch.SelectedValue != "0" && txtPaymentDateFrom.Text.Trim() != "" && txtPaymentDateTo.Text.Trim() != "")
            {
                ddlCustomerName.DataSource = null;

                string query = "Select distinct tbl_GLKYC_BasicDetails.KYCID, Name=tbl_GLEMI_InterestJVDetails.GoldLoanNo +' - '+ AppFName+' ' +AppMName+' ' + AppLName " +
                                "from tbl_GLEMI_InterestJVDetails " +
                                "inner join tbl_GLKYC_BasicDetails " +
                                        "on  tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                "inner join tbl_GLKYC_ApplicantDetails " +
                                        "on  tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "where tbl_GLEMI_InterestJVDetails.BranchID='" + ddlBranch.SelectedValue + "' " +
                                "and PaymentDate between '" + DateTime.Parse(txtPaymentDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtPaymentDateTo.Text).ToString("yyyy/MM/dd") + "'";
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

    #region [Bind GridView Customer Details]
    protected void BindCustomerDetails()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            if (ddlBranch.SelectedValue != "0" && txtPaymentDateFrom.Text.Trim() != "" && txtPaymentDateTo.Text.Trim() != "")
            {
                ddlCustomerName.DataSource = null;

                string query = "Select distinct tbl_GLEMI_InterestJVDetails.GoldLoanNo, CustName=AppFName+' ' +AppMName+' ' + AppLName from tbl_GLEMI_InterestJVDetails " +
                                "inner join tbl_GLKYC_ApplicantDetails " +
                                        "on  tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "where BranchID='" + ddlBranch.SelectedValue + "' " +
                                "and PaymentDate between '" + DateTime.Parse(txtPaymentDateFrom.Text).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(txtPaymentDateTo.Text).ToString("yyyy/MM/dd") + "'";
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

    #region [PaymentDateFrom_TextChanged]
    protected void txtPaymentDateFrom_TextChanged(object sender, EventArgs e)
    {
        try
        {
            OnReportTypeChange();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PaymentDateFrom_TextChanged]

    #region [PaymentDateTo_TextChanged]
    protected void txtPaymentDateTo_TextChanged(object sender, EventArgs e)
    {
        try
        {

            if (txtPaymentDateTo.Text != "" && txtPaymentDateFrom.Text != "")
            {
                DateTime FromDate = Convert.ToDateTime(txtPaymentDateFrom.Text);
                DateTime ToDate = Convert.ToDateTime(txtPaymentDateTo.Text);
                if (ToDate.CompareTo(FromDate) < 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Payment Date From  must be less than Payment To Date.');", true);
                    txtPaymentDateTo.Text = "";
                    txtPaymentDateFrom.Text = "";

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
    #endregion [PaymentDateTo_TextChanged]

    #region [Disable controls]
    protected void DisableControls()
    {
        try
        {
            txtPaymentDateFrom.Enabled = false;
            txtPaymentDateTo.Enabled = false;
            ddlCustomerName.Enabled = false;
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
                //Bind Customer Name dropdownlist
                FillCustomerName();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                txtPaymentDateFrom.Enabled = true;
                txtPaymentDateTo.Enabled = true;
                ddlCustomerName.Enabled = true;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = false;
            }
            else if (rdlReportType.SelectedValue == "Multiple")
            {
                //clear dropdownlist CustomerName
                ddlCustomerName.Items.Clear();

                //Bind Customer Name Gridview
                BindCustomerDetails();

                //enabling/disabling controls
                txtPaymentDateFrom.Enabled = true;
                txtPaymentDateTo.Enabled = true;
                ddlCustomerName.Enabled = false;
                ddlAccountStatus.Enabled = false;
                //dgvCustomerDetails.Enabled = true;
            }
            else if (rdlReportType.SelectedValue == "All")
            {
                //clear dropdownlist CustomerName
                ddlCustomerName.Items.Clear();

                //bind gridview customers details
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("CustName", typeof(string));

                //ShowNoResultFound(dt, dgvCustomerDetails);

                //enabling/disabling controls
                txtPaymentDateFrom.Enabled = true;
                txtPaymentDateTo.Enabled = true;
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
                if (txtPaymentDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtPaymentDateTo.Text.Trim() == "")
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
                if (txtPaymentDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtPaymentDateTo.Text.Trim() == "")
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
                if (txtPaymentDateFrom.Text.Trim() == "")
                {
                    Val = 3;
                }
                else if (txtPaymentDateTo.Text.Trim() == "")
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
}