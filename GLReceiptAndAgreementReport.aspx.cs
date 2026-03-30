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

public partial class GLReceiptAndAgreementReport : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringTesting"].ConnectionString;
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
    #endregion [Declarations]

    #region [PageLoad]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();
                //Fill Customer Name
                FillCustomerNameCombo();
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
    #endregion [PageLoad]

    #region [Fill Customer Name Combo]
    protected void FillCustomerNameCombo()
    {
        try
        {
            ddlGoldLoanNo.DataSource = null;
            ddlGoldLoanNo.Items.Clear();
            ddlGoldLoanNo.DataBind();
            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "SELECT tbl_GLSanctionDisburse_BasicDetails.SDID, Customer=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo + ' (' + AppFName + ' ' + AppMName + ' ' + AppLName + ')' " +
                        "FROM tbl_GLSanctionDisburse_BasicDetails " +
                        "INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                        "ORDER BY tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlGoldLoanNo.DataSource = dt;
            ddlGoldLoanNo.DataValueField = "SDID";
            ddlGoldLoanNo.DataTextField = "Customer";
            ddlGoldLoanNo.DataBind();
            ddlGoldLoanNo.Items.Insert(0, new ListItem("--Select GL Customer--", "0"));
            ddlGoldLoanNo.SelectedIndex = 0;

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
    #endregion [Fill Customer Name Combo]

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

    #region [Clear Data]
    protected void ClearData()
    {
        try
        {
            btnReset.Text = "Reset";
            FillCustomerNameCombo();
            chkReceipt.Checked = false;
            chkAgreement.Checked = false;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Clear Data]

    #region [Generate Report]
    protected void btnDetails_Click(object sender, System.EventArgs e)
    {
        try
        {
            string Receipt = string.Empty;
            string agreement = string.Empty;
            string strCustId = ddlGoldLoanNo.SelectedValue;
            if (chkReceipt.Checked == true)
            {
                Receipt = "1";
            }
            else
            {
                Receipt = "0";
            }
            if (chkAgreement.Checked == true)
            {
                agreement = "1";
            }
            else
            {
                agreement = "0";
            }
            string pageurl = "GLReceiptAndAgreementViewDetailedReport.aspx?G=" + strCustId + "&&R=" + Receipt + "&&A=" + agreement;
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + pageurl + "');", true);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.StackTrace + "');", true);
        }
    }
    #endregion [Generate Report]
}