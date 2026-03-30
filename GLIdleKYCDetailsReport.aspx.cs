using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
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
using System.Data.OleDb;
using System.Data.SqlTypes;

public partial class GLIdleKYCDetailsReport : System.Web.UI.Page
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
                //Fill Branch DropDownList
                FillBranchNameCombo();
                //Clear Data
                ClearData();
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

    #region [FillBranchNameCombo]
    protected void FillBranchNameCombo()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "SELECT BranchName,BID FROM tblCompanyBranchMaster";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBranchName.DataSource = dt;
            ddlBranchName.DataValueField = "BID";

            ddlBranchName.DataTextField = "BranchName";
            ddlBranchName.DataBind();
            ddlBranchName.Items.Insert(0, new ListItem("--Select Branch Name--"));
            ddlBranchName.SelectedIndex = 0;

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
    #endregion[FillBranchNameCombo]

    #region ClearData
    protected void ClearData()
    {
        try
        {
            // txtFromDate.Text = "";
            // txtToDate.Text = "";
            ddlBranchName.SelectedIndex = 0;

            btnReset.Text = "Reset";
            FillBranchNameCombo();
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
            // txtFromDate.Text = "";
            // txtToDate.Text = "";
          
            btnReset.Text = "Reset";
            FillBranchNameCombo();
            ddlBranchName.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }

    #endregion

    #region[ShowDetails]
    protected void btnDetails_Click(object sender, System.EventArgs e)
    {
        try
        {
            bool valid = false;


            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + ddlBranchName.SelectedValue + "'";

            cmd = new SqlCommand(strQuery, conn);
            cmd.CommandType = CommandType.Text;
            string branchName = Convert.ToString(cmd.ExecuteScalar());
            Session["BranchName"] = branchName;
            string branchID = ddlBranchName.SelectedValue;
            Session["BranchID"] = branchID;

            strQuery = "select PendingLoanReminderDays from tblLoanParameterSetting ";

            cmd = new SqlCommand(strQuery, conn);
            cmd.CommandType = CommandType.Text;
            int  ReminderDays = Convert.ToInt32(cmd.ExecuteScalar());

            strQuery = "SELECT Distinct tbl_GLKYC_ApplicantDetails.GoldLoanNo as 'Column8', " +
                                "(AppFName+ ' ' + AppMName+ ' ' +AppLName) as 'Column1', " +
                                "NetLoanAmtSanctioned as 'Column2'," +
                                "isnull(tbl_GLEMI_InterestJVDetails.DepositedAmount,0) as 'Column9'," +
                                "isnull(TotalBalancePayable, NetLoanAmtSanctioned) as 'Column3', " +
                                "isnull(Convert(varchar,tbl_GLEMI_InterestJVDetails.InterestDate,103), '-') as 'Column4'," +
                                "isnull((DATEDIFF(DAY,tbl_GLEMI_InterestJVDetails.InterestDate,GETDATE())), " +
                                "(DATEDIFF(DAY,tbl_GLSanctionDisburse_BasicDetails.IssueDate,GETDATE()))) as 'Column5'," +
                                "Convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103) as 'Column6', " +
                                "tbl_GLSanctionDisburse_Status.GLStatus  as 'Column7' " +
                        "FROM tbl_GLKYC_ApplicantDetails " +
                        "INNER JOIN tbl_GLSanctionDisburse_Status " +
                                "ON tbl_GLSanctionDisburse_Status.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                        "INNER JOIN tbl_GLSanctionDisburse_BasicDetails " +
                                "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                        "LEFT OUTER JOIN tbl_GLEMI_InterestJVDetails " +
                                "ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                "AND JVID=(select max(JVID) from tbl_GLEMI_InterestJVDetails " +
                                            "where tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo) " +
                        "WHERE tbl_GLSanctionDisburse_Status.GLStatus='Open' " +
                        "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                        "AND (isnull((DATEDIFF(DAY,tbl_GLEMI_InterestJVDetails.InterestDate,GETDATE())), " +
                                    "(DATEDIFF(DAY,tbl_GLSanctionDisburse_BasicDetails.IssueDate,GETDATE())))) >'" + ReminderDays + "' " +
                        "ORDER BY tbl_GLKYC_ApplicantDetails.GoldLoanNo ";

            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                Session["dtIdleKYCDetails"] = dt;
               // Response.Redirect("GLIdleKYCViewReport.aspx", false);
                ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('GLIdleKYCViewReport.aspx');", true);

            }
            else
            {
                lblMsg.Text = "No Records To Show";

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.StackTrace + "');", true);

        }

    }
    #endregion[ShowDetails]

}