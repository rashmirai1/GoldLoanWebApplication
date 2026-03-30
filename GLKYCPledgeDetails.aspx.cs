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

public partial class GLKYCPledgeDetails : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;

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

                FillBranchNameCombo();
               dgvDetails.GridLines = GridLines.Both;

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
    #endregion[dgvDetails_PageIndexChanging]

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

                int ID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());

                if (_commandName == "ShowData")
                {
                    int i = dgvDetails.SelectedIndex;
                    GridViewRow row = (GridViewRow)dgvDetails.Rows[i];

                    string GoldLoanNo = row.Cells[1].Text.Trim();

                    Session["GoldLoanNo"] = GoldLoanNo;
                    Response.Redirect("PledgeDetailsReport.aspx");

                }
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

    #region [FillBranchNameCombo]
    protected void FillBranchNameCombo()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "SELECT BranchName,BID FROM tblCompanyBranchMaster where Status='Active' ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBranchName.DataSource = dt;
            ddlBranchName.DataValueField = "BID";

            ddlBranchName.DataTextField = "BranchName";
            ddlBranchName.DataBind();
            ddlBranchName.Items.Insert(0, new ListItem("--Select Branch Name--"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[FillBranchNameCombo]

    #region[BindDGVDetails]
    protected void BindDGVDetails()
    {
        try
        {

            int FYearID = 0;
            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
            }

            string FromDate = Convert.ToDateTime(txtFromDate.Text).ToString("yyyy/MM/dd");

            string Todate = Convert.ToDateTime(txtToDate.Text).ToString("yyyy/MM/dd");

            strQuery = "SELECT  tbl_GLKYC_BasicDetails.KYCID,tbl_GLKYC_ApplicantDetails.GoldLoanNo," +
                                "(tbl_GLKYC_ApplicantDetails.AppFName+' '+tbl_GLKYC_ApplicantDetails.AppMName+' '+ " +
                                "tbl_GLKYC_ApplicantDetails.AppLName) as 'ApplicantName'," +
                                "MobileNo,TelephoneNo,(NomFName+'  '+NomMName+'  '+NomLName) as 'NomineeName'," +
                                "NomRelation,AppPhotoPath,AppSignPath,EmailID," +
                                "LoanDate=Convert(varchar,tbl_GLKYC_BasicDetails.LoanDate,103),tbl_GLKYC_BasicDetails.LoanType , " +
                                "(tbl_GLKYC_AddressDetails.BldgHouseName+''+tbl_GLKYC_AddressDetails.BldgPlotNo+''+ " +
                                "tbl_GLKYC_AddressDetails.RoomBlockNo+''+tbl_GLKYC_AddressDetails.Road+''+ " +
                                "tbl_GLKYC_AddressDetails.Landmark+''+tblAreaMaster.Area+''+tblCityMaster.CityName+''+ " +
                                "tblStateMaster.StateName+''+tblAreaMaster.Pincode) as 'Address' " +
                        "FROM tbl_GLKYC_ApplicantDetails " +
                        "INNER JOIN tbl_GLKYC_AddressDetails " +
                                " ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                         "INNER JOIN tbl_GLKYC_BasicDetails " +
                                " ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                         "INNER JOIN tblStateMaster " +
                                " ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                        " INNER JOIN tblCityMaster " +
                               " ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID" +
                        " INNER JOIN tblZonemaster " +
                                " ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                         "INNER JOIN tblAreaMaster " +
                                " ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                       "where tbl_GLKYC_BasicDetails.LoanDate between '" + FromDate + "' AND '" + Todate + "' AND tbl_GLKYC_BasicDetails.BranchID='" + Session["branchId"] + "' " +
                       "AND tbl_GLKYC_BasicDetails.FYID='" + FYearID + "'" +
                       "AND tbl_GLKYC_BasicDetails.BranchID='" + ddlBranchName.SelectedValue + "' ";


            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            dgvDetails.DataSource = ds;
            dgvDetails.DataBind();
            // dgvDetails.HeaderRow.TableSection = TableRowSection.TableHeader;


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVDetails", "alert('" + ex.Message + "');", true);

        }

    }

    #endregion[BindDGVDetails]

    #region[ValidateDate]
    protected bool validatedata()
    {
        bool valid = false;
        try
        {

            DateTime FromDate = Convert.ToDateTime(txtFromDate.Text);
            DateTime ToDate = Convert.ToDateTime(txtToDate.Text);
            if (ToDate.CompareTo(FromDate) < 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Deposite From Date Must Be Less Than Deposite To Date.');", true);
                txtFromDate.Text = "";
                txtToDate.Text = "";
                valid = false;
                return valid;
            }
            else
            {
                valid = true;
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidationAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;
    }
    #endregion

    #region[ShowDetails]
    protected void btnDetails_Click(object sender, EventArgs e)
    {
        try
        {
            bool valid = false;
            valid = validatedata();
            if (valid)
            {
                BindDGVDetails();

            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ShowDetails", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[ShowDetails]

    #region[SendDetails]
    protected void Send(object sender, EventArgs e)
    {
        if (dgvDetails.SelectedRow != null)
        {
            Server.Transfer("~/KYC PledgeFullDetails.aspx");
        }
        else
        {
            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Please select a row.')", true);
        }
    }
    #endregion[SendDetails]

    #region[Reset]
    protected void btnReset_Click(object sender, EventArgs e)
    {
        try
        {

            ddlBranchName.SelectedIndex = 0;
            txtFromDate.Text = "";
            txtToDate.Text = "";

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('" + ex.Message + "')", true);

        }
    }
    #endregion[Reset]
}