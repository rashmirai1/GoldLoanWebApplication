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

public partial class GLKYCBankGoldDetailsReport : System.Web.UI.Page
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
                //Fill Bank Name DropDownList
                FillBankNameCombo();
                ddlLocationType.Enabled = false;
                ddlBankName.Enabled = false;
                rdoBank.Enabled = false;
                rdoLocation.Enabled = false;
                ddlAccountStatus.Enabled = false;
                //int Count = Convert.ToInt32(Session["Count"]);
                //if (Count == 1)
                //{
                //    lblMsg.Text = "No Records To Show";
                //}

                //else
                //{
                lblMsg.Text = "";
                //}
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

    #region FillCombo BankName
    protected void FillBankNameCombo()
    {
        try
        {
            ddlBankName.DataSource = null;
            conn = new SqlConnection(strConnString);
            SqlDataAdapter da = new SqlDataAdapter("Select * from tblBankMaster", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankName.DataSource = dt;
            ddlBankName.DataValueField = "BankID";
            ddlBankName.DataTextField = "BankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("--Select Bank Name--"));
            ddlBankName.Items.Insert(1, new ListItem("All"));
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
    #endregion

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
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlBranchName.SelectedIndex = 0;

            btnReset.Text = "Reset";
            FillBranchNameCombo();

            ddlLocationType.SelectedIndex = 0;

            //  ddlBankName.SelectedIndex = 0;
            //FillCombo();



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
            lblMsg.Text = "";
            txtFromDate.Text = "";
            txtToDate.Text = "";
            ddlBranchName.SelectedIndex = 0;

            //btnReset.Text = "Reset";
            FillBranchNameCombo();

            ddlLocationType.SelectedIndex = 0;
            ddlLocationType.Enabled = false;

            ddlBankName.SelectedIndex = 0;
            ddlBankName.Enabled = false;
            ddlCustomerName.Items.Clear();
            ddlCustomerName.Enabled = false;
            rdoAll.Checked = false;
            rdoSingle.Checked = false;
            rdoLocation.Checked = false;
            rdoBank.Checked = false;
            rdoLocation.Enabled = false;
            rdoBank.Enabled = false;
            rdoBank.Checked = false;


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }

    #endregion

    #region[rdoLocationTypeSelectChange]
    protected void rdoLocation_CheckedChanged(object sender, System.EventArgs e)
    {
        try
        {
            if (rdoLocation.Checked)
            {

                rdoBank.Checked = false;
                // rdoBank.Enabled = false;
                ddlLocationType.Enabled = true;
                ddlBankName.Enabled = false;
                ddlBankName.SelectedIndex = 0;
            }


        }

        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[rdoLocationTypeSelectChange]

    #region[rdoBankNameSelectChange]
    protected void rdoBank_CheckedChanged(object sender, System.EventArgs e)
    {
        try
        {
            rdoLocation.Checked = false;
            // rdoLocation.Enabled = false;
            ddlBankName.Enabled = true;
            ddlLocationType.Enabled = false;
            ddlLocationType.SelectedIndex = 0;

        }

        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[rdoBankNameSelectChange]

    #region[rdoSingleCheckchange]
    protected void rdoSingle_CheckedChanged(object sender, System.EventArgs e)
    {
        try
        {

            bool valid = false;
            rdoBank.Enabled = false;
            rdoLocation.Enabled = false;
            ddlBankName.Enabled = false;
            ddlLocationType.Enabled = false;
            ddlLocationType.SelectedIndex = 0;
            ddlBankName.SelectedIndex = 0;
            rdoLocation.Checked = false;
            rdoBank.Checked = false;
            conn = new SqlConnection(strConnString);
            conn.Open();
            if (rdoSingle.Checked)
            {
                ddlAccountStatus.Enabled = false;
                ddlAccountStatus.SelectedIndex = 0;
                valid = ValidationForCriteria();
                if (valid)
                {

                    string FromDate = Convert.ToDateTime(txtFromDate.Text).ToString("yyyy/MM/dd");
                    string ToDate = Convert.ToDateTime(txtToDate.Text).ToString("yyyy/MM/dd");
                    rdoAll.Checked = false;
                    rdoSingle.Enabled = true;
                    ddlCustomerName.Enabled = true;


                    strQuery = "SELECT DISTINCT tbl_GLKYC_ApplicantDetails.GoldLoanNo, ApplicantName=(tbl_GLKYC_ApplicantDetails.AppFName+' " +
                              " ' + tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName +'  '+'(' +tbl_GLKYC_ApplicantDetails.GoldLoanNo+ ')') " +
                             " FROM  tbl_GLBankGold_AppDetails " +
                              " INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                       " ON tbl_GLBankGold_AppDetails.AppID=tbl_GLKYC_ApplicantDetails.AppID " +
                               " INNER JOIN tbl_GLBankGold_BasicDetails " +
                                       " ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                               "  INNER JOIN tbl_GLSanctionDisburse_Status " +
                                          "ON tbl_GLSanctionDisburse_Status.GoldLoanNo=tbl_GLBankGold_AppDetails.GoldLoanNo	  " +
                                " WHERE tbl_GLBankGold_BasicDetails.ReferenceDate  between '" + FromDate + "' and '" + ToDate + "' " +
                                " AND tbl_GLBankGold_BasicDetails.BranchID='" + ddlBranchName.SelectedValue + "' " +
                                "  AND  tbl_GLSanctionDisburse_Status.GLStatus='Open'";
                                 
                    SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    ddlCustomerName.DataSource = dt;
                    ddlCustomerName.DataValueField = "GoldLoanNo";

                    ddlCustomerName.DataTextField = "ApplicantName";
                    ddlCustomerName.DataBind();
                    ddlCustomerName.Items.Insert(0, new ListItem("--Select Customer--"));
                }
            }


        }

        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }


    }
    #endregion[rdoSingleCheckchange]

    #region[rdoAll_CheckedChanged]
    protected void rdoAll_CheckedChanged(object sender, System.EventArgs e)
    {
        try
        {
            bool valid = false;
            valid = ValidationForCriteria();
            if (valid)
            {
                if (rdoAll.Checked)
                {
                    ddlAccountStatus.Enabled = true;
                    rdoBank.Enabled = true;
                    rdoLocation.Enabled = true;
                    ddlCustomerName.SelectedIndex = 0;
                    //ddlBankName.Enabled = true;
                    //ddlLocationType.Enabled = true;
                    ddlCustomerName.Enabled = false;
                    rdoSingle.Checked = false;
                    rdoAll.ToolTip = "Select Criteria";
                }

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[rdoAll_CheckedChanged]

    #region[ddlCustomerName_SelectedIndexChanged]
    protected void ddlCustomerName_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        try
        {

            //string s = ddlCustomerName.SelectedValue;
            //string s1 = ddlBranchName.SelectedValue;

            //string pageurl = "GLKYCBankGoldDetailsViewReport.aspx?F=" + txtFromDate.Text + "&&T=" + txtToDate.Text + "&&C=" + ddlCustomerName.SelectedValue + "&&B=" + ddlBranchName.SelectedValue;
            //Response.Redirect("GLKYCBankGoldDetailsViewReport.aspx?F=" + txtFromDate.Text + "&&T=" + txtToDate.Text + "&&C=" + ddlCustomerName.SelectedValue + "&&B=" + ddlBranchName.SelectedValue, true);



        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[ddlCustomerName_SelectedIndexChanged]

    #region[Show Details]
    protected void btnDetails_Click(object sender, System.EventArgs e)
    {

        try
        {

            bool valid = false;

            valid = validateData();
            if (valid)
            {
                string LocationType = ddlLocationType.SelectedItem.Text;
                int LocationIndex = ddlLocationType.SelectedIndex;
                int BankNameIndex = ddlBankName.SelectedIndex;
                string BankName = ddlBankName.SelectedItem.Text;

                string s = ddlCustomerName.SelectedValue;
                string s1 = ddlBranchName.SelectedValue;
                /////////////////////////////////
                int FYearID = 0;
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                }

                if (txtFromDate.Text != null && txtToDate.Text != null && ddlBranchName.SelectedValue != null)
                {
                    string frmDate = Convert.ToDateTime(txtFromDate.Text).ToString("dd/MM/yyyy");
                    string toDate = Convert.ToDateTime(txtToDate.Text).ToString("dd/MM/yyyy");
                    Session["FromDate"] = frmDate;
                    Session["ToDate"] = toDate;
                    string branchID = ddlBranchName.SelectedValue;
                    Session["BranchID"] = branchID;

                    //fetching Branch Name
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + ddlBranchName.SelectedValue + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    cmd.CommandType = CommandType.Text;
                    string branchName = Convert.ToString(cmd.ExecuteScalar());
                    Session["BranchName"] = branchName;


                    string Query = "SELECT distinct Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103) as 'Column5'," +
                                       "ReferenceNo as 'Column1',LocationType as 'Column2',UniqueBankCustomerId as 'Column3'," +
                                       "LocationNo as 'Column4',Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103) as 'Column6'," +
                                       "Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103) as 'Column7'," +
                                       "RateOfInterest as 'Column8',tblBankMaster.BankName as 'Column9',tblBankMaster.Branch as 'Column10',UserDetails.UserName as 'Column11'," +
                                        "tbl_GLSanctionDisburse_Status.GLStatus as 'Column12',tbl_GLBankGold_AppDetails.GoldLoanNo as 'Column13', " +
                                        "tbl_GLKYC_ApplicantDetails.AppFName+' " +
                                        "'+ tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName as 'Column14' " +

                                        "FROM tbl_GLBankGold_BasicDetails " +
                                         " LEFT OUTER JOIN tblBankMaster " +
                                         " ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                                         " INNER JOIN UserDetails " +
                                         " ON UserDetails.UserID=tbl_GLBankGold_BasicDetails.OperatorID " +
                                         " INNER JOIN tbl_GLBankGold_AppDetails " +
                                         " ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                         " INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                         " ON tbl_GLKYC_ApplicantDetails.AppID=tbl_GLBankGold_AppDetails.AppID " +

                                          "inner join tbl_GLSanctionDisburse_Status " +
                                          "ON tbl_GLSanctionDisburse_Status.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +


                                          "WHERE  ReferenceDate between '" + Convert.ToDateTime(frmDate).ToString("yyyy/MM/dd") + "' " +
                                                      "and '" + Convert.ToDateTime(toDate).ToString("yyyy/MM/dd") + "' " +
                                      "AND tbl_GLBankGold_BasicDetails.FYID='" + FYearID + "' " +
                                      "AND tbl_GLBankGold_BasicDetails.BranchID='" + ddlBranchName.SelectedValue + "' ";


                    if (rdoSingle.Checked)
                    {
                        strQuery = Query + " AND tbl_GLBankGold_AppDetails.GoldLoanNo='" + ddlCustomerName.SelectedValue + "'  " +
                            "Order By Column5 ";
                    }

                    if (rdoAll.Checked)
                    {
                        if (rdoLocation.Checked)
                        {
                            if (ddlLocationType.SelectedIndex == 1)
                            {
                                if (ddlAccountStatus.SelectedIndex == 0)
                                {
                                    strQuery = Query +
                                        " Order By Column5 ";
                                }
                                else
                                {
                                    strQuery = Query + "AND tbl_GLSanctionDisburse_Status.GLStatus='" + ddlAccountStatus.SelectedItem.Text + "' " +
                                      " Order By Column5 ";
                                }

                                //" AND tbl_GLBankGold_BasicDetails.LocationType='" + LocationType  + "' ";      
                            }
                            if (ddlLocationType.SelectedIndex != 1)
                            {
                                if (ddlAccountStatus.SelectedIndex == 0)
                                {
                                    strQuery = Query + " AND tbl_GLBankGold_BasicDetails.LocationType='" + LocationType + "' " +
                                     " Order By Column5 ";
                                }
                                else
                                {
                                    strQuery = Query + " AND tbl_GLBankGold_BasicDetails.LocationType='" + LocationType + "' " +
                                        "AND tbl_GLSanctionDisburse_Status.GLStatus='" + ddlAccountStatus.SelectedItem.Text + "' " +
                                      " Order By Column5 ";
                                }

                            }


                        }

                        else if (rdoBank.Checked)
                        {

                            if (ddlBankName.SelectedIndex == 1)
                            {
                                if (ddlAccountStatus.SelectedIndex == 0)
                                {

                                    strQuery = Query + " OR LocationType='Locker' AND LocationType ='OD' " +
                                             " Order By Column5 ";
                                }
                                else
                                {
                                    strQuery = Query + "AND tbl_GLSanctionDisburse_Status.GLStatus='" + ddlAccountStatus.SelectedItem.Text + "' " +
                                        " OR LocationType='Locker' AND LocationType ='OD' " +
                                             " Order By Column5 ";
                                }
                            }
                            if (ddlBankName.SelectedIndex != 1)
                            {
                                if (ddlAccountStatus.SelectedIndex == 0)
                                {
                                    strQuery = Query + " AND tbl_GLBankGold_BasicDetails.BankID='" + ddlBankName.SelectedValue + "' " +
                                             " OR LocationType='Locker' AND LocationType ='OD' " +
                                              " Order By Column5 ";
                                }
                                else
                                {
                                    strQuery = Query + " AND tbl_GLBankGold_BasicDetails.BankID='" + ddlBankName.SelectedValue + "' " +
                                               "AND tbl_GLSanctionDisburse_Status.GLStatus='" + ddlAccountStatus.SelectedItem.Text + "' " +
                                                " OR LocationType='Locker' AND LocationType ='OD' " +

                                                 " Order By Column5 ";
                                }
                            }

                        }

                    }

                    da = new SqlDataAdapter(strQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        Session["dtBankGoldDetails"] = dt;
                        // Response.Redirect("GLKYCBankGoldDetailsViewReport.aspx", false);
                        ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('GLKYCBankGoldDetailsViewReport.aspx');", true);
                        // ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('GLSanctionDisburseViewDetailedReport.aspx');", true);
                        //RD.Load(Server.MapPath("GLBankGoldDetailsReport.rpt"));
                        //RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
                        //RD.DataDefinition.FormulaFields["FromDate"].Text = "'" + Convert.ToDateTime(frmDate).ToString("dd-MM-yyyy") + "'";
                        //RD.DataDefinition.FormulaFields["ToDate"].Text = "'" + Convert.ToDateTime(toDate).ToString("dd-MM-yyyy") + "'";
                        //RD.SetDataSource(dt);
                        //CrystalReportViewer1.ReportSource = RD;
                    }
                    else
                    {
                        lblMsg.Text = "No Records To Show";

                    }

                }
            }
            /////////////////////////////////


            //string pageurl = "GLKYCBankGoldDetailsViewReport.aspx?F=" + txtFromDate.Text + "&&T=" + txtToDate.Text + "&&C=" + ddlCustomerName.SelectedValue + "&&B=" + ddlBranchName.SelectedValue + "&&BankName=" + ddlBankName.SelectedItem.Text + "&&BankIndex=" + ddlBankName.SelectedIndex + "&&LocationIndex=" + ddlLocationType.SelectedIndex + "&&LocationType=" + ddlLocationType.SelectedItem.Text;
            //Response.Redirect("GLKYCBankGoldDetailsViewReport.aspx?F=" + txtFromDate.Text + "&&T=" + txtToDate.Text + "&&C=" + ddlCustomerName.SelectedValue + "&&B=" + ddlBranchName.SelectedValue + "&&BankName=" + ddlBankName.SelectedItem.Text + "&&BankIndex=" + ddlBankName.SelectedIndex + "&&LocationIndex=" + ddlLocationType.SelectedIndex + "&&LocationType=" + ddlLocationType.SelectedItem.Text, false);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.StackTrace + "');", true);
        }
    }
    #endregion[Show Details]

    #region [ValidateData]
    protected bool validateData()
    {
        bool valid = false;
        //int Count = 0;
        try
        {

            if (txtToDate.Text != "" && txtFromDate.Text != "")
            {
                DateTime FromDate = Convert.ToDateTime(txtFromDate.Text);
                DateTime ToDate = Convert.ToDateTime(txtToDate.Text);
                if (ToDate.CompareTo(FromDate) < 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('From Date must be less than To Date.');", true);
                    txtFromDate.Text = "";
                    txtToDate.Text = "";
                    valid = false;
                    return valid;

                }

            }


            if (rdoSingle.Checked == false && rdoAll.Checked == false)
            {

                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Report Type.');", true);
                valid = false;
            }

            else if (rdoSingle.Checked)
            {
                if (ddlCustomerName.SelectedIndex == 0)
                {

                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Customer Name.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }

            else if (rdoAll.Checked)
            {
                if (rdoLocation.Checked == false && rdoBank.Checked == false)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Criteria.');", true);
                    valid = false;
                }
                else if (rdoLocation.Checked)
                {
                    if (ddlLocationType.SelectedIndex == 0)
                    {

                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Location Type.');", true);
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        valid = true;

                    }


                }
                else if (rdoBank.Checked)
                {
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
                    return valid;

                }
            }
            //if (ddlAccountStatus.SelectedIndex == 0)
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Loan Status.');", true);
            //    valid = false;
            //    return valid;
            //}


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;
    }
    #endregion [ValidateData]

    //#region[txtToDate_TextChanged]
    // protected void txtToDate_TextChanged(object sender, System.EventArgs e)
    // {
    //     try
    //     {
    //         if (txtToDate.Text != "" && txtFromDate.Text != "")
    //         {
    //             DateTime FromDate = Convert.ToDateTime(txtFromDate.Text);
    //             DateTime ToDate = Convert.ToDateTime(txtToDate.Text);
    //             if (ToDate.CompareTo(FromDate) < 0)
    //             {
    //                 ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('From Date must be less than To Date.');", true);
    //                 txtFromDate.Text = "";
    //                 txtToDate.Text = "";

    //             }

    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);

    //     }
    // }
    // #endregion[txtToDate_TextChanged]

    #region[ValidationForCriteria]
    protected bool ValidationForCriteria()
    {
        bool valid = false;
        try
        {
            if (ddlBranchName.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch.');", true);
                if (rdoAll.Checked)
                {
                    rdoAll.Checked = false;
                }
                if (rdoSingle.Checked)
                {
                    rdoSingle.Checked = false;
                }

                valid = false;
                return valid;
            }

            if (txtFromDate.Text == "" || txtToDate.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select From Date Or To Date.');", true);
                if (rdoAll.Checked)
                {
                    rdoAll.Checked = false;
                }
                if (rdoSingle.Checked)
                {
                    rdoSingle.Checked = false;
                }

                valid = false;
                return valid;
            }

            else
            {
                valid = true;
                return valid;

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;

    }
    #endregion[ValidationForCriteria]

    #region[ddlBranchName_SelectedIndexChanged]
    protected void ddlBranchName_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        try
        {
            rdoSingle.Checked = false;
            rdoAll.Checked = false;
            ddlCustomerName.Enabled = false;
            ddlAccountStatus.Enabled = false;
            ddlAccountStatus.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[ddlBranchName_SelectedIndexChanged]
}

