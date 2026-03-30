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

public partial class GLKYCBankGoldDetailsEditForm : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string RefType = string.Empty;
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

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();

                FillBankName();

                //GetRefType
                GetRefType();

                //binding GridView
                BindDGVDetails();

                //BindCustomer Details
                BindDetails();

                // BindDDLSearchBy
                BindDDLSearchBy();

                //Making readonly
                txtBranchName.Attributes.Add("readonly", "readonly");

                //DisablAllFields
                DisableAllFields();

                //added on onblur event attribute
                ddlRefType.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlRefType, ""));
                ddlRefNo.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlRefNo, ""));
                // ddlRefID.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlRefID, ""));
                ddlLocationType.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlLocationType, ""));
                ddlBankName.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.ddlBankName, ""));
                txtSearch.Attributes.Add("keydown", "return handleEnter('" + btnSearch.ClientID + "', event)");
                //txtDepFrmDate.Attributes.Add("onchange", this.Page.ClientScript.GetPostBackEventReference(this.txtDepFrmDate, ""));

                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID);
                }
                if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
                {
                    branchId = Convert.ToInt32(Session["branchId"]);
                    txtBranchID.Text = Convert.ToString(branchId);
                }
                if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
                {
                    txtOperator.Text = Convert.ToString(Session["username"]);
                }

                if (Convert.ToString(Session["userID"]) != "" && Convert.ToString(Session["userID"]) != null)
                {
                    txtOperatorID.Text = Convert.ToString(Session["userID"]);
                }

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

    #region [Bind GridView DGVDetails]
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "SELECT BankGoldID,RefType,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),ReferenceNo,LocationType,LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103),RefType,DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest,tblBankMaster.BankName,tblBankMaster.Branch " +
                                " FROM tbl_GLBankGold_BasicDetails " +
                                "LEFT OUTER JOIN tblBankMaster " +
                                "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                       "WHERE tbl_GLBankGold_BasicDetails.FYID='" + Session["FYearID"] + "' " +
                       "AND tbl_GLBankGold_BasicDetails.BranchID='" + Session["branchId"] + "' ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            dgvDetails.DataSource = ds;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView DGVDetails]

    #region GetRecords
    protected DataSet GetRecords(SqlConnection conn, string CommandName, int BankID)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103), " +
                            " ReferenceNo,LocationType,LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                            "DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest, " +
                            "tblBankMaster.BankName,tblBankMaster.Branch,RefType,RefNum=tbl_GLBankGold_BasicDetails.GLRefNum+'/' " +
                            "+Convert(Nvarchar,tbl_GLBankGold_BasicDetails.GLRefID,103),RefNo,RefID,tblBankMaster.BankID,UniqueBankCustomerId " +
                                " FROM tbl_GLBankGold_BasicDetails " +
                                "LEFT OUTER JOIN tblBankMaster " +
                                "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                                 "WHERE tbl_GLBankGold_BasicDetails.FYID='" + Session["FYearID"] + "' " +
                            "AND tbl_GLBankGold_BasicDetails.BranchID='" + Session["branchId"] + "' ";
            }
            else if (CommandName == "UpdateRecord")
            {
                string s =Convert.ToString(Session["RNum"]);
                if (s == "BG/OD")
                {
                    strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),LocationType, " +
                                "LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                                "DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest, " +
                                "tblBankMaster.BankName,tblBankMaster.Branch ,RefType, " +
                                "RefNum=tbl_GLBankGold_BasicDetails.GLRefNum+'/'+Convert(Nvarchar,tbl_GLBankGold_BasicDetails.GLRefID,103)+'/'+RefNo,tblBankMaster.BankID,UniqueBankCustomerId" +
                                    " FROM tbl_GLBankGold_BasicDetails " +
                                    "LEFT OUTER JOIN tblBankMaster " +
                                    "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                           "WHERE tbl_GLBankGold_BasicDetails.BankGoldID=" + BankID + " ";
                }
                if (s == "BG/LO")
                {
                    strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),LocationType, " +
                                "LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                                "DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest, " +
                                "tblBankMaster.BankName,tblBankMaster.Branch ,RefType, " +
                              "RefNum=tbl_GLBankGold_BasicDetails.RefNo,tblBankMaster.BankID,UniqueBankCustomerId" +
                                    " FROM tbl_GLBankGold_BasicDetails " +
                                    "LEFT OUTER JOIN tblBankMaster " +
                                    "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                           "WHERE tbl_GLBankGold_BasicDetails.BankGoldID=" + BankID + " ";
                }
                if (s == "BG/OF")
                {
                    strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),LocationType, " +
                                "LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                                "DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest, " +
                                "tblBankMaster.BankName,tblBankMaster.Branch ,RefType, " +
                              "RefNum=tbl_GLBankGold_BasicDetails.RefNo,tblBankMaster.BankID,UniqueBankCustomerId" +
                                    " FROM tbl_GLBankGold_BasicDetails " +
                                    "LEFT OUTER JOIN tblBankMaster " +
                                    "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                           "WHERE tbl_GLBankGold_BasicDetails.BankGoldID=" + BankID + " ";
                }
                if (s == "BG/HO")
                {
                    strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),LocationType, " +
                                              "LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                                              "DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103),RateOfInterest, " +
                                              "tblBankMaster.BankName,tblBankMaster.Branch ,RefType, " +
                                            "RefNum=tbl_GLBankGold_BasicDetails.RefNo,tblBankMaster.BankID,UniqueBankCustomerId" +
                                                  " FROM tbl_GLBankGold_BasicDetails " +
                                                  "LEFT OUTER JOIN tblBankMaster " +
                                                  "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                                         "WHERE tbl_GLBankGold_BasicDetails.BankGoldID=" + BankID + " ";
                }

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

    #region [Bind GridView Details]
    protected void BindDetails()
    {
        try
        {
            txtBGID.Text = "";

            strQuery = "select tbl_GLBankGold_AppDetails.GoldLoanNo," +
                       " ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' + tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName ," +
                       " tbl_GLKYC_ApplicantDetails.MobileNo , " +
                       " tbl_GLBankGold_AppDetails.NetWeight as 'TotalNetWeight'," +
                       " tbl_GLBankGold_AppDetails.AppID  " +
                        " FROM  tbl_GLBankGold_AppDetails " +
                        " INNER JOIN tbl_GLKYC_ApplicantDetails  " +
                                       " ON tbl_GLBankGold_AppDetails.AppID=tbl_GLKYC_ApplicantDetails.AppID " +
                        " where tbl_GLBankGold_AppDetails.BankGoldID =  '" + txtBGID.Text.Trim() + "'  ";


            DataTable dt;
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgvCustomerDetails.DataSource = dt;
                dgvCustomerDetails.DataBind();
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("ApplicantName", typeof(string));
                dt.Columns.Add("MobileNo", typeof(string));
                dt.Columns.Add("TotalNetWeight", typeof(int));
                dt.Columns.Add("AppID", typeof(string));
                ShowNoResultFound(dt, dgvCustomerDetails);
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

    #region [Bind GridView All Customers Details]
    protected void BindCustomerDetails1()
    {
        try
        {
            strQuery = "select DISTINCT tbl_GLBankGold_AppDetails.GoldLoanNo," +
                       " ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' + tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName ," +
                      " tbl_GLKYC_ApplicantDetails.MobileNo , " +
                       " tbl_GLBankGold_AppDetails.NetWeight as 'TotalNetWeight'," +
                       " tbl_GLBankGold_AppDetails.AppID  " +
                        " FROM  tbl_GLBankGold_AppDetails " +
                        " INNER JOIN tbl_GLKYC_ApplicantDetails  " +
                                       " ON tbl_GLBankGold_AppDetails.AppID=tbl_GLKYC_ApplicantDetails.AppID " +
                        " where tbl_GLBankGold_AppDetails.BankGoldID =  '" + txtBGID.Text.Trim() + "'  " +
                       " Union ALL " +
                        " select DISTINCT tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo , " +
                        " ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' + tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName , " +
                        " tbl_GLKYC_ApplicantDetails.MobileNo , " +
                        " tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight ,tbl_GLKYC_ApplicantDetails.AppID  " +
                        " FROM  tbl_GLSanctionDisburse_BasicDetails " +
                        " INNER JOIN tbl_GLKYC_ApplicantDetails  " +
                                       " ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                         "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails   " +
                                       " ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                         " WHERE tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo NOT IN(SELECT GoldLoanNo FROM tbl_GLBankGold_AppDetails)  " +
                         " AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + Session["branchId"] + "'" +
                      " AND tbl_GLSanctionDisburse_BasicDetails.FYID='" + Session["FYearID"] + "'";

            DataTable dt;
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                dgvCustomerDetails.DataSource = null;
                dgvCustomerDetails.DataSource = dt;
                dgvCustomerDetails.DataBind();
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("GoldLoanNo", typeof(string));
                dt.Columns.Add("ApplicantName", typeof(string));
                dt.Columns.Add("TotalNetWeight", typeof(int));
                dt.Columns.Add("AppID", typeof(string));
                ShowNoResultFound(dt, dgvCustomerDetails);
            }
            if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    int App = 0;
                    string AppID = (row.Cells[4].FindControl("lblAppID") as Label).Text;
                    strQuery = "SELECT tbl_GLKYC_ApplicantDetails.AppID FROM  tbl_GLKYC_ApplicantDetails " +
                                "INNER JOIN tbl_GLBankGold_AppDetails " +
                               " ON tbl_GLBankGold_AppDetails.AppID=tbl_GLKYC_ApplicantDetails.AppID " +
                               " Where tbl_GLBankGold_AppDetails.AppID='" + AppID + "'";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        App = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        App = 0;
                    }

                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (App == Convert.ToInt32(AppID))
                    {
                        chk.Checked = true;
                    }
                    else
                    {
                        chk.Checked = false;
                    }
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

    //#region [dgvCustomerDetails_RowCommand]
    //protected void dgvCustomerDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    //{
    //    try
    //    {
    //        //if (e.CommandName == "DeleteRecord")
    //        //{
    //        //    GridView _gridView = (GridView)sender;
    //        //    int index = Convert.ToInt32(e.CommandArgument);
    //        //    string DID = Convert.ToString(_gridView.DataKeys[index].Value.ToString());

    //        //    if (strDID.Length == 0)
    //        //    {
    //        //        strDID = DID;
    //        //    }
    //        //    else
    //        //    {
    //        //        strDID = strDID + "," + DID;
    //        //    }

    //        //    if (txtDID.Text.Trim() != "")
    //        //    {
    //        //        txtDID.Text += "," + strDID;
    //        //    }
    //        //    else
    //        //    {
    //        //        txtDID.Text = strDID;
    //        //    }

    //        //    GridViewRow row = dgvDocumentDetails.Rows[index];
    //        //    DataTable dtDocumentDetails = new DataTable();
    //        //    dtDocumentDetails = (DataTable)Session["dtDocumentDetails"];

    //        //    if ((dgvDocumentDetails.Rows.Count > 0) && (dgvDocumentDetails.Rows.Count != 1))  //Checks whether list contains items
    //        //    {
    //        //        if (dtDocumentDetails != null)
    //        //        {
    //        //            dtDocumentDetails.Rows.RemoveAt(index);
    //        //            dgvDocumentDetails.DataSource = dtDocumentDetails;
    //        //            dgvDocumentDetails.DataBind();
    //        //        }
    //        //    }
    //        //    else
    //        //    {
    //        //        dtDocumentDetails.Rows.RemoveAt(index);
    //        //        ShowNoResultFound(dtDocumentDetails, dgvDocumentDetails);
    //        //    }
    //        //}
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
    //    }
    //}
    //#endregion [dgvCustomerDetails_RowCommand]

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
                dsDGV = GetRecords(conn, "GetAllRecords", 0);
                int BankID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());
                Session["BankGoldID"] = BankID;

                #region [Delete Record]
                if (_commandName == "DeleteRecord")
                {
                    //if (dgvDetails != null && dgvDetails.Rows.Count > 0)
                    //{
                    //    int i = 0;
                    //    i = dgvDetails.SelectedIndex;
                    //    GridViewRow row = (GridViewRow)dgvDetails.Rows[i];
                    //   string s1= row.Cells[1].Text;
                    //   int BankGoldId = Convert.ToInt32(row.Cells[1].FindControl("lblBankGoldID") as Label);
                    //   Session["Id"] = BankGoldId;
                    //}
                    bool datasaved = false;
                    strQuery = "";
                    strQuery = "select GoldLoanNo from tbl_GLBankGold_AppDetails where BankGoldID='" + Session["BankGoldID"] + "'";

                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        string GoldNo = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        Session["GNo"] = GoldNo;
                    }


                    //checking whether Gold Loan A/C is processed to next stage (Sanction/Disburse)
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + Session["GNo"] + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since it is being processed to Interest JV Details.');", true);
                    }


                    if (existcount == 0)
                    {
                        //checking whether record is present
                        strQuery = "select count(*) from tbl_GLBankGold_AppDetails where BankGoldID=" + BankID + "";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {

                            datasaved = false;
                            int QueryResult = 0;
                            //deleting record from DB
                            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                            deleteQuery = "delete from tbl_GLBankGold_AppDetails where BankGoldID='" + BankID + "'";
                            cmd = new SqlCommand(deleteQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }

                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLBankGold_BasicDetails where BankGoldID='" + BankID + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                            }

                            if (QueryResult > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);

                                //ClearData
                                ClearData();

                                //BindCustomerDetails
                                BindDetails();


                                // BindGridViewDetails1();
                                BindDGVDetails();

                                //if the same record is deleted which is filled in the form.
                                if (txtBGID.Text != "" && txtBGID.Text != null)
                                {
                                    if (ID == Convert.ToString(txtBGID.Text))
                                    {
                                        ClearData();
                                        // BindCustomerDetails1();
                                        BindDGVDetails();
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
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                #endregion [Delete Record]

                else if (_commandName == "UpdateRecord")
                {
                    //Fill RefType
                    GetRefType();
                    string RefNum = "";
                    if (dgvDetails != null && dgvDetails.Rows.Count > 0)
                    {
                        int i = 0;
                        i = dgvDetails.SelectedIndex;
                        GridViewRow row = (GridViewRow)dgvDetails.Rows[i];
                        string RefNo = (row.Cells[7].FindControl("lblRefType") as Label).Text;
                        //string RefNo = "";
                        // RefNo = row.Cells[7].Text.Trim();
                        RefNum = RefNo.Substring(0, 5);
                        Session["RNum"] = RefNum;
                    }
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", BankID);
                   
                    //if (dgvDetails != null && dgvDetails.Rows.Count > 0)
                    //{
                    //    int i = 0;
                    //    i = dgvDetails.SelectedIndex;
                    //    GridViewRow row = (GridViewRow)dgvDetails.Rows[i];
                    //    string RefNo = (row.Cells[7].FindControl("lblRefType") as Label).Text;
                    //    //string RefNo = "";
                    //    // RefNo = row.Cells[7].Text.Trim();
                    //    RefNum = RefNo.Substring(0, 5);

                    //}
                    if (RefNum == "BG/OD")
                    {
                        ddlBankName.Enabled = true;
                        txtBranchName.Enabled = true;
                        txtLocationNo.Enabled = true;
                        ddlLocationType.Enabled = true;
                        txtInterestRate.Enabled = true;
                        txtDepositFromDate.Enabled = true;
                        txtDepositToDate.Enabled = true;
                        txtRefDate.Enabled = true;
                        ddlLocationType.Enabled = false;
                        txtUniqueID.Enabled = true;
                        ddlBankName.Enabled = false;
                        txtBranchName.Enabled = false;
                        txtUniqueID.Enabled = false;
                        dgvCustomerDetails.Enabled = false;
                        txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                        ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        txtDepositFromDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
                        string ToDate = Convert.ToDateTime(ds.Tables[0].Rows[0][5]).ToString("dd/MM/yyyy");
                        if (ToDate == "01/01/1900")
                        {
                            txtDepositToDate.Text = "";
                        }
                        else
                        {
                            txtDepositToDate.Text = ToDate;
                        }
                        txtInterestRate.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                        FillBankName();
                        ddlBankName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][11]);
                        txtBranchName.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        ddlRefType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                        GetRefNum();
                        string s1 = Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                        ddlRefNo.SelectedValue = s1;// Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                        GetRefID();
                       // string s2 = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        // ddlRefID.SelectedValue = s2;// Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        txtUniqueID.Text = Convert.ToString(ds.Tables[0].Rows[0][12]);
                        BindCustomerDetails1();
                    }

                    if (RefNum == "BG/LO")
                    {
                        ddlBankName.Enabled = true;
                        txtBranchName.Enabled = true;
                        txtLocationNo.Enabled = true;
                        ddlLocationType.Enabled = true;
                        txtRefDate.Enabled = true;
                        txtDepositFromDate.Enabled = false;
                        txtDepositToDate.Enabled = false;
                        txtInterestRate.Enabled = false;
                        txtInterestRate.Text = "";
                        txtInterestRate.Enabled = false;
                        txtDepositToDate.Text = "";
                        txtDepositFromDate.Text = "";
                        txtDepositToDate.Enabled = false;
                        txtInterestRate.Enabled = false;
                        ddlLocationType.Enabled = false;
                        txtUniqueID.Enabled = false;
                        ddlBankName.Enabled = false;
                        txtBranchName.Enabled = false;
                        ddlBankName.Enabled = false;
                        txtBranchName.Enabled = false;
                        txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                        ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        FillBankName();
                        ddlBankName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][11]);
                        txtBranchName.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        ddlRefType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                       GetRefNum();
                       ddlRefNo.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                       // GetRefID();
                        // ddlRefID.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        BindCustomerDetails1();
                        dgvCustomerDetails.Enabled = true;
                    }

                    if (RefNum == "BG/OF")
                    {
                        txtLocationNo.Enabled = true;
                        ddlLocationType.Enabled = true;
                        txtRefDate.Enabled = true;
                        txtDepositFromDate.Enabled = false;
                        txtInterestRate.Text = "";
                        txtInterestRate.Enabled = false;
                        txtDepositToDate.Text = "";
                        txtDepositFromDate.Text = "";
                        txtDepositToDate.Enabled = false;
                        txtInterestRate.Enabled = false;
                        ddlBankName.SelectedIndex = 0;
                        ddlBankName.Enabled = false;
                        txtBranchName.Text = "";
                        ddlLocationType.Enabled = false;
                        txtBranchName.Enabled = false;
                        txtUniqueID.Enabled = false;
                        txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                        ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        GetRefType();
                        ddlRefType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                        GetRefNum();
                        ddlRefNo.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                        //GetRefID();
                        // ddlRefID.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        BindCustomerDetails1();
                        dgvCustomerDetails.Enabled = true;
                    }
                    //Fill data when Location type is Home :
                    if (RefNum == "BG/HO")
                    {
                        txtLocationNo.Enabled = true;
                        ddlLocationType.Enabled = true;
                        txtRefDate.Enabled = true;
                        txtDepositFromDate.Enabled = false;
                        txtInterestRate.Text = "";
                        txtInterestRate.Enabled = false;
                        txtDepositToDate.Text = "";
                        txtDepositFromDate.Text = "";
                        txtDepositToDate.Enabled = false;
                        txtInterestRate.Enabled = false;
                        ddlBankName.SelectedIndex = 0;
                        ddlBankName.Enabled = false;
                        txtBranchName.Text = "";
                        ddlLocationType.Enabled = false;
                        txtBranchName.Enabled = false;
                        txtUniqueID.Enabled = false;
                        txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                        txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                        ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        GetRefType();
                        ddlRefType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][9]).Trim();
                        GetRefNum();
                        ddlRefNo.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][10]).Trim();
                        //GetRefID();
                        // ddlRefID.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][11]).Trim();
                        BindCustomerDetails1();
                        dgvCustomerDetails.Enabled = true; 
                    }

                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + Session["GNo"] + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since it is in use.');", true);
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }

                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.StackTrace + "');", true);
        }

        finally
        { }
    }
    #endregion

    #region [dgvCustomerDetails_PageIndexChanging]
    protected void dgvCustomerDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvCustomerDetails.PageIndex = e.NewPageIndex;
            BindCustomerDetails1();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVDocPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvCustomerDetails_PageIndexChanging]

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

    #region Cancel
    protected void btnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            ClearData();
            BindDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.Message + "');", true);
        }
    }

    #endregion

    #region ClearData
    protected void ClearData()
    {
        try
        {
            txtBranchName.Text = "";
            txtDepositFromDate.Text = "";
            txtInterestRate.Text = "";
            txtLocationNo.Text = "";
            txtDepositToDate.Text = "";
            txtRefDate.Text = "";
            txtFYID.Text = "";
            txtBranchID.Text = "";
            GetRefType();
            ddlRefNo.Items.Clear();
            txtBGID.Text = "";
            // ddlRefID.Items.Clear();
            ddlBankName.Items.Clear();
            txtUniqueID.Text = "";
            // ddlBankName.Text = "";
            // ddlRefID.SelectedIndex = 0;
            // ddlRefNo.SelectedIndex = 0;
            //ddlBankName.SelectedIndex = 0;
            ddlLocationType.SelectedIndex = 0;
            FillBankName();

            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
                txtFYID.Text = Convert.ToString(FYearID);
            }
            if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
            {
                branchId = Convert.ToInt32(Session["branchId"]);
                txtBranchID.Text = Convert.ToString(branchId);
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Search Record
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //Search Records
            DataTable dt = GetRecords(conn, "GetAllRecords", 0).Tables[0];
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

    #region [Bind DropDownList-SearchBy]
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("ReferenceNo");
            ddlSearchBy.Items.Add("ReferenceDate");
            ddlSearchBy.Items.Add("LocationType");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind DropDownList-SearchBy]

    #region[ValidateDate]
    protected bool validatedata()
    {
        bool valid = false;
        try
        {

            if (ddlRefType.SelectedIndex == 0 && ddlRefType.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Ref Type.');", true);
                ddlRefType.Focus();
                valid = false;
                return valid;
            }
            if (ddlRefNo.SelectedIndex == 0 || ddlRefNo.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Ref Type.');", true);
                ddlRefType.Focus();
                valid = false;
                return valid;
            }
            //if (ddlRefID.SelectedIndex == 0 || ddlRefID.Text == "")
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Ref Type.');", true);
            //    ddlRefType.Focus();
            //    valid = false;
            //    return valid;
            //}
            if (txtRefDate.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Reference Date.');", true);
                valid = false;
                return valid;
            }

            if (txtLocationNo.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                valid = false;
                return valid;
            }
            if (ddlLocationType.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Location Type.');", true);
                valid = false;
                return valid;
            }

            if (ddlLocationType.SelectedItem.Text == "Locker")
            {
                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    valid = false;
                    return valid;
                }
                else if (txtBranchName.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch Name.');", true);
                    valid = false;
                    return valid;
                }
                else if (txtLocationNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                    txtLocationNo.Focus();
                    valid = false;
                    return valid;
                }

            }

            if (ddlLocationType.SelectedItem.Text == "OD")
            {


                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    valid = false;
                    return valid;
                }
                else if (txtBranchName.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch Name.');", true);
                    valid = false;
                    return valid;
                }
                else if (txtDepositFromDate.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter DepositeFromDate.');", true);
                    valid = false;
                    return valid;
                }
                //else if (txtDepositToDate.Text == "")
                //{
                //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter DepositeToDate.');", true);
                //    valid = false;
                //    return valid;
                //}
                else if (txtLocationNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                    valid = false;
                    txtLocationNo.Focus();
                    return valid;
                }

                else if (txtDepositToDate.Text != "" && txtDepositFromDate.Text != "")
                {
                    DateTime FromDate = Convert.ToDateTime(txtDepositFromDate.Text);
                    DateTime ToDate = Convert.ToDateTime(txtDepositToDate.Text);
                    if (ToDate.CompareTo(FromDate) < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Deposite From Date Must Be Less Than Deposite To Date.');", true);

                        valid = false;
                        return valid;
                    }
                }
                else
                {
                    valid = true;
                }
            }

            if (ddlLocationType.SelectedItem.Text == "Office")
            {
                if (txtLocationNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                    txtLocationNo.Focus();
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                }
            }

            else if (ddlRefType.SelectedItem.Text == "Ref Type")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select RefType.');", true);
                valid = false;
                return valid;
            }
            else if (ddlRefNo.SelectedItem.Text == "Ref No" || ddlRefNo.SelectedItem.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select RefNo.');", true);
                valid = false;
                return valid;
            }
            //else if (ddlRefID.SelectedItem.Text == "RefID" || ddlRefID.SelectedItem.Text == "")
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select RefID.');", true);
            //    valid = false;
            //    return valid;
            //}

            //else if (ddlBankName.SelectedIndex == 0)
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
            //    valid = false;
            //    return valid;
            //}
            //else if (txtBranchName.Text == "")
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch Name.');", true);
            //    valid = false;
            //    return valid;
            //}

            //else if (txtLocationNo.Text == "")
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter LocationNo .');", true);
            //    valid = false;
            //    return valid;
            //}

            //else if (txtDepositFromDate.Text != "" && txtDepositToDate.Text != "")
            //{
            //    DateTime FromDate = Convert.ToDateTime(txtDepositFromDate.Text);
            //    DateTime ToDate = Convert.ToDateTime(txtDepositToDate.Text);
            //    if (ToDate.CompareTo(FromDate) < 0)
            //    {
            //        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Deposit From Date Must Be Less Than Deposit To Date.');", true);
            //        valid = false;
            //        return valid;
            //    }
            //}
            //if (dgvCustomerDetails == null && dgvCustomerDetails.Rows.Count == 0)
            //{
            //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Coustomers Details.');", true);
            //    valid = false;
            //    return valid;
            //}

            int Count = 0;
            if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {

                        Count = Count + 1;

                    }
                }

                if (Count >= 1)
                {
                    valid = true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Customers.');", true);
                    valid = false;

                }
            }
            if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                Count = 0;
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    //for (int i = 0; i < dgvCustomerDetails.Rows.Count; i++)
                    //{
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk.Checked)
                    {
                        Count = Count + 1;
                    }
                }

                if (ddlLocationType.SelectedItem.Text == "OD")
                {
                    if (Count > 1)
                    {

                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Single Customer For OD.');", true);
                        valid = false;
                        return valid;

                    }
                    else
                    {
                        valid = true;
                    }

                }
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

    #endregion

    #region ddlLocationTypeTextChange
    protected void ddlLocationType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            if (ddlLocationType.SelectedValue == "Office")
            {
                txtBranchName.Enabled = false;
                ddlBankName.Enabled = false;
                txtDepositFromDate.Enabled = false;
                txtDepositToDate.Enabled = false;
                txtInterestRate.Enabled = false;
                txtLocationNo.Enabled = true;
                txtRefDate.Enabled = true;
                txtBranchName.Text = "";
                CreateReferenceNo();

            }
            else if (ddlLocationType.SelectedValue == "Locker")
            {
                txtBranchName.Enabled = true;
                ddlBankName.Enabled = true;
                txtDepositFromDate.Enabled = false;
                txtDepositToDate.Enabled = false;
                txtInterestRate.Enabled = false;
                txtLocationNo.Enabled = true;
                txtRefDate.Enabled = true;
                CreateReferenceNo();
            }
            else if (ddlLocationType.SelectedValue == "OD")
            {
                txtBranchName.Enabled = true;
                ddlBankName.Enabled = true;
                txtDepositFromDate.Enabled = true;
                txtDepositToDate.Enabled = true;
                txtInterestRate.Enabled = true;
                txtLocationNo.Enabled = true;
                txtRefDate.Enabled = true;
                CreateReferenceNo();
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LocationType", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion

    //#region txtDepFrmDate_TextChanged
    //protected void txtDepFrmDate_TextChanged(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        txtDepositFromDate.Enabled = true;
    //        txtDepositFromDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "LocationType", "alert('" + ex.Message + "');", true);

    //    }
    //}
    //#endregion
    //#region FillCombo BankName
    //protected void FillBankNameCombo()
    //{
    //    try
    //    {
    //        ddlBankName.DataSource = null;
    //        conn = new SqlConnection(strConnString);
    //        strQuery = " SELECT tblBankMaster.BankName,tblBankMaster.BankID FROM tblBankMaster ";
    //        SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
    //        DataTable dt = new DataTable();
    //        da.Fill(dt);
    //        ddlBankName.DataSource = dt;
    //        ddlBankName.DataValueField = "BankID";
    //        ddlBankName.DataTextField = "BankName";
    //        ddlBankName.DataBind();
    //        ddlBankName.Items.Insert(0, new ListItem("--Select Bank Name--"));
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "ComboAlert", "alert('" + ex.Message + "');", true);
    //    }

    //    finally
    //    {
    //        if (conn.State == ConnectionState.Open)
    //        {
    //            conn.Close();
    //        }
    //    }
    //}
    //#endregion

    #region DisableAllFields
    protected void DisableAllFields()
    {
        try
        {
            txtBranchName.Enabled = false;
            ddlBankName.Enabled = false;
            txtDepositFromDate.Enabled = false;
            txtDepositToDate.Enabled = false;
            txtInterestRate.Enabled = false;
            txtBranchName.Text = "";
            txtLocationNo.Enabled = false;
            ddlLocationType.Enabled = false;
            txtRefDate.Enabled = false;
            txtUniqueID.Enabled = false;
            //  txtRefDate.CssClass. = false;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DisableFieldsAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion

    #region FillBankName
    protected void FillBankName()
    {
        try
        {
            ddlBankName.DataSource = null;
            conn = new SqlConnection(strConnString);
            SqlDataAdapter da = new SqlDataAdapter("Select BankID,BankName from tblBankMaster", conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlBankName.DataSource = dt;
            ddlBankName.DataValueField = "BankID";
            ddlBankName.DataTextField = "BankName";
            ddlBankName.DataBind();
            ddlBankName.Items.Insert(0, new ListItem("--Select Bank Name--"));

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

    #region [ChkChanged]
    protected void ChkChanged(object sender, EventArgs e)
    {
        CheckBox chkStatus = (CheckBox)sender;

        // GridViewRow row = (GridViewRow)chkStatus.NamingContainer;
        // GridViewRow r1 = (GridViewRow)row;
        int count = 0;
        if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
        {
            foreach (GridViewRow row in dgvCustomerDetails.Rows)
            {
                //for (int i = 0; i < dgvCustomerDetails.Rows.Count; i++)
                //{
                CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                if (chk.Checked)
                {
                    count = count + 1;
                }
            }
        }
        if (ddlLocationType.SelectedItem.Text == "OD")
        {
            if (count > 1)
            {
                int i = dgvCustomerDetails.SelectedIndex;
                if (i > 0)
                {
                    CheckBox chk = (CheckBox)dgvCustomerDetails.FindControl("chkSelect");
                    chk.Checked = true;

                }
                //else
                //{
                //    chk.Checked = false;

                //}

                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Single Customer For OD.');", true);
            }


        }



    }
    #endregion [ChkChanged]

    #region [BankNameTextChaneEvent]
    protected void ddlBankName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            strQuery = "SELECT Branch FROM tblBankMaster WHERE BankName='" + ddlBankName.SelectedItem.Text + "'";

            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                txtBranchName.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
            }
            if (ddlBankName.SelectedIndex == 0)
            {
                txtBranchName.Text = "";

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Save Data]

    #region [Create BankGold Reference No]
    protected string CreateReferenceNo()
    {
        string ReferenceNo = string.Empty;
        try
        {
            string RefNum = string.Empty;
            string strQuery = string.Empty;
            int RefID = 0;
            DateTime todayDate;

            conn = new SqlConnection(strConnString);
            conn.Open();

            //checking whether Today's Date is within selected Financial Year.
            strQuery = "select count(*) from tblFinancialyear where (select getdate()) between StartDate and EndDate";
            cmd = new SqlCommand(strQuery, conn);
            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count > 0)
            {
                //getting Today's Date
                strQuery = "select getdate() ";
                cmd = new SqlCommand(strQuery, conn);

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    todayDate = Convert.ToDateTime(cmd.ExecuteScalar());
                    RefNum = todayDate.ToString("MM-yy").ToUpper();
                }

                //getting Financial Year
                int SYear = 0;
                int EYear = 0;
                strQuery = "select FinancialyearID, StartDate, EndDate " +
                            "from tblFinancialyear " +
                            "where FinancialyearID='" + txtFYID.Text + "'";
                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    DateTime dtStartDate = Convert.ToDateTime(ds.Tables[0].Rows[0][1]);
                    DateTime dtEndDate = Convert.ToDateTime(ds.Tables[0].Rows[0][2]);
                    SYear = dtStartDate.Year;
                    EYear = dtEndDate.Year;
                }

                //getting MAX Ref ID
                strQuery = "Select MAX(RefID) from tbl_GLBankGold_BasicDetails " +
                            "where ((RefMon between 4 and 12) and RefYr='" + SYear + "') " +
                            "or ((RefMon between 1 and 3) and RefYr='" + EYear + "')";
                cmd = new SqlCommand(strQuery, conn);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    RefID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    RefID = 0;
                }

                RefID += 1;
                string LocationType = ddlLocationType.SelectedValue;
                string Lt = LocationType.Substring(0, 2).ToUpper();
                string RefType = "BG";
                ReferenceNo = RefType + "/" + Lt + "/" + RefNum + "/" + Convert.ToString(RefID);
                //txtRefNo.Text = Convert.ToString(ReferenceNo);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "FYAlert", "alert('Today's Date does not match with selected Financial Year. Please Log in to correct Financial Year.');", true);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLNoAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
        return ReferenceNo;
    }
    #endregion [Create Reference No]

    #region Get RefType
    protected void GetRefType()
    {
        try
        {

            //ddlRefNo.Items.Clear();
            // txtBGID.Text = "";
            //ddlRefID.Items.Clear();
            // ddlBankName.Items.Clear();
            txtUniqueID.Text = "";
            //ddlRefNo.DataSource = null;
            conn = new SqlConnection(strConnString);

            strQuery =
            "SELECT DISTINCT  RefType=RTrim(tbl_GLBankGold_BasicDetails.RefType) " +
             "FROM tbl_GLBankGold_BasicDetails " +
            "WHERE  tbl_GLBankGold_BasicDetails.FYID='" + Convert.ToInt32(Session["FYearID"]) + "' " +
            "AND tbl_GLBankGold_BasicDetails.BranchID='" + Session["branchId"] + "' ";

            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRefType.DataSource = dt;
            ddlRefType.DataTextField = "RefType";
            ddlRefType.DataValueField = "RefType";
            ddlRefType.DataBind();
            ddlRefType.Items.Insert(0, new ListItem("Ref Type"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefNumlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Get RefNum
    protected void GetRefNum()
    {
        try
        {

            ddlRefNo.DataSource = null;
            ddlRefNo.Items.Clear();

            conn = new SqlConnection(strConnString);

            if (ddlRefType.SelectedItem.Text == "BG/LOC")
            {
                strQuery =

              " SELECT DISTINCT RefNum=tbl_GLBankGold_BasicDetails.RefNo " +
               " FROM tbl_GLBankGold_BasicDetails    " +
                "WHERE  tbl_GLBankGold_BasicDetails.FYID='" + Convert.ToInt32(Session["FYearID"]) + "' And RefType='" + ddlRefType.SelectedValue + "' ";
            }
            if (ddlRefType.SelectedItem.Text == "BG/OD")
            {
                strQuery =

                   " SELECT DISTINCT RefNum=tbl_GLBankGold_BasicDetails.GLRefNum+'/'+Convert(Nvarchar,tbl_GLBankGold_BasicDetails.GLRefID,103)+'/'+tbl_GLBankGold_BasicDetails.RefNo " +
                    " FROM tbl_GLBankGold_BasicDetails    " +
                     "WHERE  tbl_GLBankGold_BasicDetails.FYID='" + Convert.ToInt32(Session["FYearID"]) + "' And RefType='" + ddlRefType.SelectedValue + "' ";
            }
            if (ddlRefType.SelectedItem.Text == "BG/OFF")
            {
                strQuery =

                     " SELECT DISTINCT RefNum=tbl_GLBankGold_BasicDetails.RefNo " +
                    " FROM tbl_GLBankGold_BasicDetails    " +
                     "WHERE  tbl_GLBankGold_BasicDetails.FYID='" + Convert.ToInt32(Session["FYearID"]) + "' And RefType='" + ddlRefType.SelectedValue + "' ";
            }
            if (ddlRefType.SelectedItem.Text == "BG/HOM")
            {
                strQuery =

                     " SELECT DISTINCT RefNum=tbl_GLBankGold_BasicDetails.RefNo " +
                    " FROM tbl_GLBankGold_BasicDetails    " +
                     "WHERE  tbl_GLBankGold_BasicDetails.FYID='" + Convert.ToInt32(Session["FYearID"]) + "' And RefType='" + ddlRefType.SelectedValue + "' ";
            }

            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlRefNo.DataSource = dt;
            ddlRefNo.DataTextField = "RefNum";
            ddlRefNo.DataValueField = "RefNum";
            ddlRefNo.DataBind();
            ddlRefNo.Items.Insert(0, new ListItem("RefNum"));

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
            //ddlRefID.DataSource = null;
            conn = new SqlConnection(strConnString);

            strQuery = "select DISTINCT tbl_GLBankGold_BasicDetails.RefNo  " +
                         "from tbl_GLBankGold_BasicDetails " +
                         " WHERE tbl_GLBankGold_BasicDetails.RefType='" + ddlRefType.SelectedValue + "' " +
                         "AND (tbl_GLBankGold_BasicDetails.GLRefNum+'/'+Convert(Nvarchar,tbl_GLBankGold_BasicDetails.GLRefID,103)) ='" + ddlRefNo.SelectedValue + "'";


            da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            // ddlRefID.DataSource = dt;
            // ddlRefID.DataTextField = "RefNo";
            // ddlRefNo.DataValueField = "RefNo";
            // ddlRefID.DataBind();
            //ddlRefID.Items.Insert(0, new ListItem("RefNo"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefIDAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [ ddlRefType_SelectedIndexChanged]
    protected void ddlRefType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            txtBranchName.Text = "";
            txtDepositFromDate.Text = "";
            txtInterestRate.Text = "";
            txtLocationNo.Text = "";
            txtDepositToDate.Text = "";
            txtRefDate.Text = "";
            txtFYID.Text = "";
            txtBranchID.Text = "";
            ddlBankName.SelectedIndex = 0;
            txtRefDate.Text = "";
            ddlLocationType.SelectedIndex = 0;
            BindDetails();
            txtUniqueID.Text = "";
            txtBranchName.Text = "";
            // ddlRefID.Items.Clear();
            ddlRefNo.Items.Clear();
            GetRefNum();




        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefIDAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion [ddlRefType_SelectedIndexChanged]

    #region [ ddlRefNo_SelectedIndexChanged]
    protected void ddlRefNo_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string ss = ddlRefNo.SelectedValue;
            FillData();
           
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefIDAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion [ddlRefNo_SelectedIndexChanged]

    #region [ ddlRefID_SelectedIndexChanged]
    protected void ddlRefID_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillData();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRefIDAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion [ddlRefID_SelectedIndexChanged]

    #region Fill Details of Gold Loan No
    protected void FillData()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            RefType = ddlRefType.SelectedValue.ToString().Trim();
           // string RefID = ddlRefID.SelectedValue.ToString().Trim();
            string RefNo = ddlRefNo.SelectedValue.ToString().Trim();
            string ReferenceNo = "BG" + '/' + (RefNo) ;

            if (ddlRefType.SelectedValue == "BG/OD")
            {
                strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103) " +
                            ",LocationType,LocationNo,DepositeFromDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103), " +
                            " DepositeToDate=Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103), " +
                           "RateOfInterest,tblBankMaster.BankName,tblBankMaster.Branch,tbl_GLBankGold_BasicDetails.BankID,UniqueBankCustomerId " +
                            " FROM tbl_GLBankGold_BasicDetails " +
                            "INNER JOIN tblBankMaster " +
                            "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                            "WHERE  tbl_GLBankGold_BasicDetails.ReferenceNo='" + ReferenceNo + "'";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlBankName.Enabled = false;
                    txtBranchName.Enabled = false;
                    txtLocationNo.Enabled = true ;
                    ddlLocationType.Enabled = false;
                    txtDepositToDate.Enabled = true;
                    txtDepositFromDate.Enabled = true;
                    txtInterestRate.Enabled = true;
                    txtRefDate.Enabled = true;
                    ddlLocationType.Enabled = false;
                    txtUniqueID.Enabled = false;
                    dgvCustomerDetails.Enabled = false;
                    txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                    ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    txtDepositFromDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
                    //txtDepFrmDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][4]).ToString("dd/MM/yyyy");
                    string Todate = Convert.ToDateTime(ds.Tables[0].Rows[0][5]).ToString("dd/MM/yyyy");
                    if (Todate == "01/01/1900")
                    {
                        txtDepositToDate.Text = "";
                    }
                    else
                    {
                        txtDepositToDate.Text = Todate;// Convert.ToDateTime(ds.Tables[0].Rows[0][5]).ToString("dd/MM/yyyy");
                    }
                    txtInterestRate.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                    FillBankName();
                    ddlBankName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][9]);
                    txtBranchName.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                    txtUniqueID.Text = Convert.ToString(ds.Tables[0].Rows[0][10]);
                    dgvCustomerDetails.DataSource = null;
                    BindCustomerDetails1();
                }
            }
            if (ddlRefType.SelectedValue == "BG/LOC")
            {
                ReferenceNo = "";
                ReferenceNo = "BG" + "/" + ddlRefNo.SelectedValue;
                strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103), " +
                                    "LocationType,LocationNo,tblBankMaster.BankName,tblBankMaster.Branch, tbl_GLBankGold_BasicDetails.BankID ,tblBankMaster.BankID" +
                            " FROM tbl_GLBankGold_BasicDetails " +
                            "LEFT OUTER JOIN tblBankMaster " +
                            "ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
                            "WHERE  tbl_GLBankGold_BasicDetails.ReferenceNo='" + ReferenceNo + "'";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    ddlBankName.Enabled = true;
                    txtBranchName.Enabled = true;
                    txtLocationNo.Enabled = true;
                    ddlLocationType.Enabled = true;
                    txtDepositFromDate.Text = "";
                    txtDepositFromDate.Enabled = false;
                    txtDepositToDate.Text = "";
                    txtDepositToDate.Enabled = false;
                    txtInterestRate.Text = "";
                    txtInterestRate.Enabled = false;
                    txtRefDate.Enabled = true;
                    ddlLocationType.Enabled = false;
                    txtUniqueID.Enabled = false;
                    ddlLocationType.Enabled = false;
                    txtUniqueID.Enabled = false;
                    dgvCustomerDetails.Enabled = true;
                    ddlBankName.Enabled = false;
                    txtBranchName.Enabled = false;
                    txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");

                    txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);

                    ddlBankName.Items.Clear();
                    FillBankName();
                    ddlBankName.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][7]);

                    txtBranchName.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                    ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]);

                    BindCustomerDetails1();
                }
            }
            if (ddlRefType.SelectedValue == "BG/OFF")
            {
                ReferenceNo = "";
                ReferenceNo = "BG" + "/" + ddlRefNo.SelectedValue;
                strQuery = "SELECT BankGoldID,ReferenceDate=Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103),LocationType,LocationNo " +
                            " FROM tbl_GLBankGold_BasicDetails " +
                            "WHERE  tbl_GLBankGold_BasicDetails.ReferenceNo='" + ReferenceNo + "'";

                cmd = new SqlCommand(strQuery, conn);
                da = new SqlDataAdapter(cmd);
                ds = new DataSet();
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    dgvCustomerDetails.Enabled = true;
                    ddlBankName.Enabled = false;
                    txtBranchName.Enabled = false;
                    ddlLocationType.Enabled = true;
                    txtLocationNo.Enabled = true;
                    txtDepositFromDate.Text = "";
                    txtDepositFromDate.Enabled = false;
                    txtDepositToDate.Text = "";
                    txtDepositToDate.Enabled = false;
                    txtInterestRate.Text = "";
                    txtInterestRate.Enabled = false;
                    ddlBankName.SelectedIndex = 0;
                    ddlBankName.Enabled = false;
                    txtBranchName.Text = "";
                    txtRefDate.Enabled = true;
                    ddlLocationType.Enabled = false;
                    txtUniqueID.Enabled = false;
                    txtBGID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    txtRefDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][1]).ToString("dd/MM/yyyy");
                    ddlLocationType.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                    txtLocationNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    dgvCustomerDetails.DataSource = null;
                    // dgvCustomerDetails.DataBind();
                    BindCustomerDetails1();
                }
            }
            //binding GridView

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
        // return ds;
    }
    #endregion

    #region UpdateData
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            bool valid;

            string Refdate = txtRefDate.Text;
            string depositFrom = txtDepositFromDate.Text;
            string depositeTo = txtDepositToDate.Text;
            conn = new SqlConnection(strConnString);
            int existcount = 0;
            conn.Open();
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
            valid = validatedata();
            // valid = true;
            if (valid)
            {
                strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where GoldLoanNo='" + Session["GNo"] + "'";
                cmd = new SqlCommand(strQuery, conn, transaction);
                existcount = Convert.ToInt32(cmd.ExecuteScalar());

                if (existcount > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since it is being processed to Interest JV Details.');", true);
                }

                else
                {
                    // int BGID=0;       
                    if (dgvCustomerDetails.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvCustomerDetails.Rows)
                        {
                            CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                            if (chk != null && chk.Checked)
                            {
                                string AppID = (row.Cells[5].FindControl("lblAppID") as Label).Text;
                                strQuery = "Select Count(*) from tbl_GLBankGold_AppDetails where AppID='" + AppID + "' ";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                existcount = Convert.ToInt32(cmd.ExecuteScalar());

                                if (existcount == 0)
                                {
                                    int BID = 0;
                                    //getting MAX BGID from tbl_GLKYC_App_BankGoldGetails
                                    strQuery = "Select MAX(BGAppID) from tbl_GLBankGold_AppDetails ";
                                    cmd = new SqlCommand(strQuery, conn, transaction);
                                    if (cmd.ExecuteScalar() != DBNull.Value)
                                    {
                                        BID = Convert.ToInt32(cmd.ExecuteScalar());
                                    }
                                    else
                                    {
                                        BID = 0;
                                    }

                                    BID += 1;

                                    string GoldLoanNo = (row.Cells[1].FindControl("lblGoldLoanNo") as Label).Text;
                                    string ApplicantName = (row.Cells[2].FindControl("lblApplicantName") as Label).Text;
                                    string TotalNetWeight = (row.Cells[4].FindControl("lblTotalNetWeight") as Label).Text;
                                    AppID = (row.Cells[5].FindControl("lblAppID") as Label).Text;

                                    //select RefType,RefNo,RefID
                                    RefType = string.Empty;
                                    RefType = ddlRefType.SelectedValue;
                                    string RefNo = ddlRefNo.SelectedValue.Trim();
                                    string RefID ;//=// ddlRefID.SelectedValue.Trim();
                                    string ReferenceNo = string.Empty;
                                    if (ddlLocationType.SelectedItem.Text == "Locker")
                                    {
                                        ReferenceNo = "BG" + "/"+Convert.ToString(RefNo).Trim();
                                    }
                                    else if (ddlLocationType.SelectedItem.Text == "OD")
                                    {
                                        ReferenceNo = "BG" + "/" + Convert.ToString(RefNo).Trim() + "/";
                                    }
                                    else
                                    {
                                        ReferenceNo = "BG" + "/"+Convert.ToString(RefNo).Trim();
                                    }
                                    Session["ReferenceNo"] = Convert.ToString(ReferenceNo);
                                    // ReferenceNo = RefType + "/" + RefNo +"/"+RefID;


                                    strQuery = "select BankGoldID From tbl_GLBankGold_BasicDetails WHERE ReferenceNo='" + ReferenceNo + "'";
                                    int BankID = 0;
                                    cmd = new SqlCommand(strQuery, conn, transaction);
                                    if (cmd.ExecuteScalar() != DBNull.Value)
                                    {
                                        BankID = Convert.ToInt32(cmd.ExecuteScalar());
                                    }
                                    else
                                    {
                                        BankID = 0;
                                    }

                                    //inserting data into table tbl_GLKYC_App_BankGoldGetails
                                    insertQuery = "insert into tbl_GLBankGold_AppDetails values('" + BID + "', '" + BankID + "', '" + GoldLoanNo + "', " +
                                                  "'" + TotalNetWeight + "', '" + AppID + "')";
                                    cmd = new SqlCommand(insertQuery, conn, transaction);
                                    int QueryResult = cmd.ExecuteNonQuery();

                                    if (QueryResult > 0)
                                    {
                                        datasaved = true;
                                    }
                                }
                                else
                                {
                                    datasaved = true;

                                }
                            }

                            else if (chk != null && chk.Checked == false)
                            {
                                string AppID = (row.Cells[5].FindControl("lblAppID") as Label).Text;

                                strQuery = "Select Count(*) from tbl_GLBankGold_AppDetails where AppID='" + AppID + "' ";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                existcount = Convert.ToInt32(cmd.ExecuteScalar());
                                if (existcount > 0)
                                {
                                    deleteQuery = "delete from tbl_GLBankGold_AppDetails where AppID='" + AppID + "'";

                                    cmd = new SqlCommand(deleteQuery, conn, transaction);
                                    int QueryResult = cmd.ExecuteNonQuery();

                                    if (QueryResult > 0)
                                    {
                                        datasaved = true;
                                    }
                                }
                                else
                                {
                                    datasaved = true;
                                }
                            }
                        }
                    }

                    if (datasaved == true)
                    {
                        int BankID = 0;
                        strQuery = "select BankID from tblBankMaster WHERE BankName='" + ddlBankName.SelectedValue + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            BankID = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            BankID = 0;
                        }
                        Session["BankId"] = BankID;

                        strQuery = "select getdate() ";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        int RefMon = 0;
                        int RefYr = 0;
                        string RefNum = string.Empty;

                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            DateTime todayDate = Convert.ToDateTime(cmd.ExecuteScalar());
                            RefMon = todayDate.Month;
                            RefYr = todayDate.Year;
                            RefNum = todayDate.ToString("MM-yy").ToUpper();
                        }

                        RefType = string.Empty;
                        RefType = ddlRefType.SelectedValue;
                        string RefNo = ddlRefNo.SelectedValue.Trim();
                       // string RefID = ddlRefID.SelectedValue.Trim();
                        string ReferenceNo = string.Empty;
                        ReferenceNo = "BG" + "/" + Convert.ToString(RefNo);
                        Session["ReferenceNo"] = Convert.ToString(ReferenceNo);

                        if (ddlLocationType.SelectedItem.Text == "OD")
                        {
                            int QueryResult = 0;

                            //RefType = string.Empty;
                            //RefType = ddlRefType.SelectedValue;
                            //string RefNo = ddlRefNo.SelectedValue.Trim();
                            //string RefID = ddlRefID.SelectedValue.Trim();
                            //string ReferenceNo = string.Empty;
                            //ReferenceNo = RefType + "/" + Convert.ToString(RefNo) + "/" + Convert.ToString(RefID).Trim();
                            //Session["ReferenceNo"] = Convert.ToString(ReferenceNo);

                            string ToDate = "";
                            if (txtDepositToDate.Text != "")
                            {
                                ToDate = txtDepositToDate.Text;
                                ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");

                            }
                            else
                            {
                                ToDate = "";


                            }

                            updateQuery = "Update tbl_GLBankGold_BasicDetails SET ReferenceDate ='" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "' " +
                                   ",ReferenceNo ='" + Session["ReferenceNo"] + "', " +
                                    "LocationType ='" + ddlLocationType.SelectedValue + "', LocationNo ='" + txtLocationNo.Text.Trim() + "', " +
                                    "RefType='" + RefType + "', " +
                                    "DepositeFromDate ='" + Convert.ToDateTime(txtDepositFromDate.Text).ToString("yyyy/MM/dd") + "', " +
                                    "DepositeToDate ='" + ToDate + "',RateOfInterest ='" + txtInterestRate.Text.Trim() + "' ," +
                                    "BankID ='" + ddlBankName.SelectedValue + "', FYID='" + Session["FYearID"] + "',OperatorID='" + txtOperatorID.Text + "',BranchID='" + Session["branchId"] + "' " +
                                    " ,UniqueBankCustomerId='" + txtUniqueID.Text.Trim() + "'" +
                                    "WHERE BankGoldID='" + txtBGID.Text.Trim() + "' ";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }

                        }
                        if (ddlLocationType.SelectedItem.Text == "Office")
                        {
                            int QueryResult = 0;

                            //RefType = string.Empty;
                            //RefType = ddlRefType.SelectedValue;
                            RefNo = "";
                             RefNo = ddlRefNo.SelectedValue.Trim();
                            //string RefID = ddlRefID.SelectedValue.Trim();
                            //string ReferenceNo = string.Empty;
                            //ReferenceNo = RefType + "/" + Convert.ToString(RefNo) + "/" + Convert.ToString(RefID).Trim();
                            //Session["ReferenceNo"] = Convert.ToString(ReferenceNo);
                            ReferenceNo = "";
                            ReferenceNo = "BG" + "/" + Convert.ToString(RefNo).Trim();
                            Session["ReferenceNo"] = Convert.ToString(ReferenceNo);

                            updateQuery = "Update   tbl_GLBankGold_BasicDetails SET ReferenceDate='" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "',ReferenceNo='" + Session["ReferenceNo"] + "', " +
                                           "LocationType='" + ddlLocationType.SelectedValue + "', LocationNo='" + txtLocationNo.Text.Trim() + "', " +
                                            "RefType='" + RefType + "'," +
                                          " FYID='" + Session["FYearID"] + "',OperatorID='" + txtOperatorID.Text + "',BranchID='" + Session["branchId"] + "' WHERE BankGoldID='" + txtBGID.Text.Trim() + "' ";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }
                        if (ddlLocationType.SelectedItem.Text == "Locker")
                        {
                            int QueryResult = 0;

                            //RefType = string.Empty;
                            //RefType = ddlRefType.SelectedValue.Trim();
                            RefNo = "";
                          RefNo = ddlRefNo.SelectedValue.Trim();
                            //string RefID = ddlRefID.SelectedValue.Trim();
                            //string ReferenceNo = string.Empty;
                            //ReferenceNo = RefType + "/" + Convert.ToString(RefNo) + "/" + Convert.ToString(RefID).Trim();
                            //Session["ReferenceNo"] = Convert.ToString(ReferenceNo);
                            ReferenceNo = "";
                            ReferenceNo = "BG" + "/" +Convert.ToString(RefNo).Trim();
                            Session["ReferenceNo"] = Convert.ToString(ReferenceNo);

                            updateQuery = "Update tbl_GLBankGold_BasicDetails SET ReferenceDate='" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "', ReferenceNo='" + Session["ReferenceNo"] + "', " +
                                           "LocationType='" + ddlLocationType.SelectedValue + "', LocationNo='" + txtLocationNo.Text.Trim() + "', " +
                                             "RefType='" + RefType + "' ," +
                                             "BankID='" + ddlBankName.SelectedValue + "', FYID='" + Session["FYearID"] + "',OperatorID='" + txtOperatorID.Text + "',BranchID='" + Session["branchId"] + "'  WHERE BankGoldID='" + txtBGID.Text.Trim() + "' ";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }
                        if (ddlLocationType.SelectedItem.Text == "Home")
                        {
                            int QueryResult = 0;

                            //RefType = string.Empty;
                            //RefType = ddlRefType.SelectedValue;
                            RefNo = "";
                            RefNo = ddlRefNo.SelectedValue.Trim();
                            //string RefID = ddlRefID.SelectedValue.Trim();
                            //string ReferenceNo = string.Empty;
                            //ReferenceNo = RefType + "/" + Convert.ToString(RefNo) + "/" + Convert.ToString(RefID).Trim();
                            //Session["ReferenceNo"] = Convert.ToString(ReferenceNo);
                            ReferenceNo = "";
                            ReferenceNo = "BG" + "/" + Convert.ToString(RefNo).Trim();
                            Session["ReferenceNo"] = Convert.ToString(ReferenceNo);

                            updateQuery = "Update   tbl_GLBankGold_BasicDetails SET ReferenceDate='" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "',ReferenceNo='" + Session["ReferenceNo"] + "', " +
                                           "LocationType='" + ddlLocationType.SelectedValue + "', LocationNo='" + txtLocationNo.Text.Trim() + "', " +
                                            "RefType='" + RefType + "'," +
                                          " FYID='" + Session["FYearID"] + "',OperatorID='" + txtOperatorID.Text + "',BranchID='" + Session["branchId"] + "' WHERE BankGoldID='" + txtBGID.Text.Trim() + "' ";
                            cmd = new SqlCommand(updateQuery, conn, transaction);
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
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Updated Successfully.');", true);

                        //BindCustomerDetails
                        // BindCustomerDetails1();
                        //BindDGVDetails
                        BindDGVDetails();
                        //ClearData
                        ClearData();
                        //DisablAllFields
                        DisableAllFields();
                        //BindCustomer Details
                        BindDetails();

                        //conn.Close();
                    }
                    else
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Updated Successfully.');", true);
                    }

                }


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
    #endregion

    #region[txtSearch_TextChanged]
    protected void txtSearch_TextChanged(object sender, EventArgs e)
    {
        try
        {
            //conn = new SqlConnection(strConnString);
            //conn.Open();
            //Search Records

            DataTable dt = GetRecords(conn, "GetAllRecords", 0).Tables[0];
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

            ////binding GridView
            //BindDGVDetails();

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "FillDataAlert", "alert('" + ex.Message + "');", true);

        }
        finally
        {

        }
    }
    #endregion[txtSearch_TextChanged]
    protected void dgvDetails_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}
