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

public partial class GLKYCBankGoldDetailsForm : System.Web.UI.Page
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
    SqlDateTime sqldatenull;

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

                //binding GridView
                BindCustomerDetails();

                //Making readonly
                txtBranchName.Attributes.Add("readonly", "readonly");
                txtRefNo.Attributes.Add("readonly", "readonly");

                //FillBankName
                FillBankNameCombo();

                //DisablAllFields
                DisableAllFields();
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

    #region [Create BankGold Reference No]
    protected string CreateReferenceNo()
    {
        string ReferenceNo = string.Empty;

        try
        {

            bool valid = false;
            string RefNum = string.Empty;
            string GoldLoanNo = string.Empty;
            string RefNo = string.Empty;
            string RefId = string.Empty;
            string LocationType = string.Empty;
            string Lt = string.Empty;
            string Alias = string.Empty;
            int RefYr = 0;
            int RefMon = 0;
            int ID = 0;
            int SYear = 0;
            int EYear = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();
            //valid = validatedata();

            //if (valid)
            //{

            if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {
                        GoldLoanNo = (row.Cells[1].FindControl("lblGoldLoanNo") as Label).Text;
                    }

                }
            }

            strQuery = "SELECT RefNo,RefID, RefMon,RefYr  FROM tbl_GLKYC_BasicDetails WHERE GoldLoanNo='" + GoldLoanNo + "'";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                RefNo = Convert.ToString(ds.Tables[0].Rows[0][0]).Trim();
                RefId = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                RefMon = Convert.ToInt32(ds.Tables[0].Rows[0][2]);
                RefYr = Convert.ToInt32(ds.Tables[0].Rows[0][3]);
                Session["RefNo"] = RefNo.Trim();
                Session["RefId"] = RefId.Trim();
                Session["RefMon"] = RefMon;
                Session["RefYr"] = RefYr;
            }

            strQuery = "SELECT BankAlias FROM tblBankMaster WHERE BankName='" + ddlBankName.SelectedItem.Text + "'";
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Alias = Convert.ToString(ds.Tables[0].Rows[0][0]);

            }

            //getting Financial Year

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
            strQuery = "Select MAX(tbl_GLBankGold_BasicDetails.RefID) " +
                      " FROM tbl_GLBankGold_BasicDetails " +
                       "where tbl_GLBankGold_BasicDetails.FYID='" + Session["FYearID"] + "'  ";
                      
            cmd = new SqlCommand(strQuery, conn);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                ID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                ID = 0;
            }

            ID += 1;

            Session["ID"] = Convert.ToString(ID).Trim();

            LocationType = ddlLocationType.SelectedValue;


            if (ddlLocationType.SelectedIndex == 1)
            {
                Lt = LocationType.Substring(0, 3).ToUpper();
                ReferenceNo = "BG" + "/" + Alias + "/" + Lt + "/" + Session["ID"];
                RefNum = Alias + "/" + Lt + "/" + Session["ID"];
            }
            if (ddlLocationType.SelectedIndex == 2)
            {
                Lt = LocationType.Substring(0, 2).ToUpper();
                ReferenceNo = "BG" + "/" + RefNo + "/" + RefId + "/" + Alias + "/" + Lt + "/" + txtUniqueID.Text.Trim() + "/" + Session["ID"];
                RefNum = Alias + "/" + Lt + "/" + txtUniqueID.Text.Trim() + "/" + Session["ID"];
            }
            if (ddlLocationType.SelectedIndex == 3)
            {
                Lt = LocationType.Substring(0, 3).ToUpper();
                ReferenceNo = "BG" + "/" + Lt + "/" + Session["ID"];
                RefNum = Lt + "/" + Session["ID"];
            }
            if (ddlLocationType.SelectedIndex == 4)
            {
                Lt = LocationType.Substring(0, 3).ToUpper();
                ReferenceNo = "BG" + "/" + Lt + "/" + Session["ID"];
                RefNum = Lt + "/" + Session["ID"];
            }


            Session["RefNum"] = RefNum.Trim();
            Session["ReferenceNo"] = ReferenceNo;
            txtRefNo.Text = "";
            txtRefNo.Text = Convert.ToString(Session["ReferenceNo"]).Trim();
        }
        //}
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

    #region [Bind GridView Customer Details]
    protected void BindCustomerDetails()
    {
        try
        {
            strQuery = "select tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo ," +
                       " ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' + tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName ," +
                       " tbl_GLKYC_ApplicantDetails.MobileNo , " +
                       " tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight ,tbl_GLKYC_ApplicantDetails.AppID " +
                       " FROM  tbl_GLSanctionDisburse_BasicDetails" +
                       " INNER JOIN tbl_GLKYC_ApplicantDetails " +
                                   "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                       "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                  " ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_GoldValueDetails.GoldLoanNo " +
                       "WHERE tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo NOT IN(SELECT GoldLoanNo FROM tbl_GLBankGold_AppDetails) " +
                       "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + Session["branchId"] + "'";

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
                dt.Columns.Add("MobileNo", typeof(int));
                dt.Columns.Add("TotalNetWeight", typeof(int));
                dt.Columns.Add("AppID", typeof(string));

                ShowNoResultFound(dt, dgvCustomerDetails);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView Document Details]

    #region [dgvCustomerDetails_RowCommand]
    protected void dgvCustomerDetails_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            //if (e.CommandName == "DeleteRecord")
            //{
            //    GridView _gridView = (GridView)sender;
            //    int index = Convert.ToInt32(e.CommandArgument);
            //    string DID = Convert.ToString(_gridView.DataKeys[index].Value.ToString());

            //    if (strDID.Length == 0)
            //    {
            //        strDID = DID;
            //    }
            //    else
            //    {
            //        strDID = strDID + "," + DID;
            //    }

            //    if (txtDID.Text.Trim() != "")
            //    {
            //        txtDID.Text += "," + strDID;
            //    }
            //    else
            //    {
            //        txtDID.Text = strDID;
            //    }

            //    GridViewRow row = dgvDocumentDetails.Rows[index];
            //    DataTable dtDocumentDetails = new DataTable();
            //    dtDocumentDetails = (DataTable)Session["dtDocumentDetails"];

            //    if ((dgvDocumentDetails.Rows.Count > 0) && (dgvDocumentDetails.Rows.Count != 1))  //Checks whether list contains items
            //    {
            //        if (dtDocumentDetails != null)
            //        {
            //            dtDocumentDetails.Rows.RemoveAt(index);
            //            dgvDocumentDetails.DataSource = dtDocumentDetails;
            //            dgvDocumentDetails.DataBind();
            //        }
            //    }
            //    else
            //    {
            //        dtDocumentDetails.Rows.RemoveAt(index);
            //        ShowNoResultFound(dtDocumentDetails, dgvDocumentDetails);
            //    }
            //}
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvCustomerDetails_RowCommand]

    #region [dgvCustomerDetails_PageIndexChanging]
    protected void dgvCustomerDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvCustomerDetails.PageIndex = e.NewPageIndex;
            BindCustomerDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVDocPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvCustomerDetails_PageIndexChanging]

    #region Reset/Cancel
    protected void btnReset_Click(object sender, EventArgs e)
    {
        try
        {
            
            ClearData();
           // int count = 0;

            if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    //for (int i = 0; i < dgvCustomerDetails.Rows.Count; i++)
                    //{
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk.Checked)
                    {
                        chk.Checked = false;                        
                    }
                }
            }
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
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
            FillBankNameCombo();
            txtRefDate.Text = "";
            ddlLocationType.SelectedIndex = 0;
            txtRefNo.Text = "";
            txtFYID.Text = "";
            txtBranchID.Text = "";
            txtUniqueID.Text = "";
            //FillCombo();
            // ddlBankName.Items.Insert(0, new ListItem("--Select Bank name--"));


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

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            bool valid;
            conn = new SqlConnection(strConnString);
            conn.Open();
            valid = validatedata();

            #region [Save]
            if (valid)
            {
                int QueryResult = 0;
                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                string GoldLoanNo = string.Empty;
                string RefNum = string.Empty;
                int RefMon = 0;
                int RefYr = 0;
                //DateTime todayDate;
                int BankGoldID = 0;


                // 1] Data insertion into tbl_GLKYC_BankGoldGetails
                //getting MAX BankGoldID
                strQuery = "select max(BankGoldID) from tbl_GLBankGold_BasicDetails";
                cmd = new SqlCommand(strQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BankGoldID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    BankGoldID = 0;
                }
                BankGoldID += 1;
                Session["BGID"] = BankGoldID;

                // 2] Data insertion into tbl_GLBankGold_BasicDetails
                //getting MAX BankGoldID

                //getting Today's Date
                strQuery = "select getdate() ";
                cmd = new SqlCommand(strQuery, conn, transaction);

                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    DateTime todayDate = Convert.ToDateTime(cmd.ExecuteScalar());
                    RefMon = todayDate.Month;
                    RefYr = todayDate.Year;
                    RefNum = todayDate.ToString("MM-yy").ToUpper().Trim();
                }

                // getting MAX Ref ID
                int RefID = 0;

                //strQuery = "Select MAX(RefID) from tbl_GLBankGold_BasicDetails " +
                //            "where ((RefMon between 4 and 12 and RefType='" + RefType.Trim() + "') and RefYr='" + SYear + "') " +
                //            "or ((RefMon between 1 and 3) and RefYr='" + EYear + "')";
                //cmd = new SqlCommand(strQuery, conn, transaction);
                //if (cmd.ExecuteScalar() != DBNull.Value)
                //{
                //    RefID = Convert.ToInt32(cmd.ExecuteScalar());
                //}
                //else
                //{
                //    RefID = 0;
                //}

                //RefID += 1;
                //Session["RefID"] = Convert.ToString(RefID).Trim();
                string LocationType = ddlLocationType.SelectedValue;
                string Lt = LocationType.Substring(0, 2).ToUpper();
                RefType = "BG" + "/" + Lt;
                // string  ReferenceNo = RefType + "/" + Convert.ToString(RefNum) + "/" + Convert.ToString(RefID);

                int BankID = 0;

                strQuery = "select BankID from tblBankMaster WHERE BankName='" + ddlBankName.SelectedItem.Text.Trim() + "'";
                cmd = new SqlCommand(strQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    BankID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    BankID = 0;
                }
                Session["BankID"] = Convert.ToString(BankID);
                // DateTime? dt = null;
                // default(DateTime?);

                //select BankID
                if (ddlLocationType.SelectedValue == "Locker")
                {
                    //DateTime? dt = null;
                    LocationType = ddlLocationType.SelectedValue;
                    Lt = "";
                    Lt = LocationType.Substring(0, 3).ToUpper();
                    RefType = "BG" + "/" + Lt;

                    //default(DateTime);
                    //inserting data into table tbl_GLKYC_BankGoldGetails
                    insertQuery = "insert into tbl_GLBankGold_BasicDetails values('" + BankGoldID + "', '" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "', " +
                                    "'" + RefType.Trim() + "', '" + Session["RefNo"] + "', '" + Session["RefId"] + "', '" + Session["RefNum"] + "', " +
                                    "'" + Session["ID"] + "', '" + txtRefNo.Text.Trim() + "','" + ddlLocationType.SelectedValue + "', '" + txtLocationNo.Text.Trim() + "', " +
                                    "'', " +
                                    "'', '', " +
                                    "'" + Session["BankID"] + "','" + txtUniqueID.Text.Trim() + "','" + Session["FYearID"] + "','" + Session["branchId"] + "','" + txtOperatorID.Text + "') ";

                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        datasaved = true;
                    }
                }

                if (ddlLocationType.SelectedValue == "OD")
                {
                    LocationType = ddlLocationType.SelectedValue;
                    Lt = "";
                    Lt = LocationType.Substring(0, 2).ToUpper();
                    RefType = "BG" + "/" + Lt;
                    string ToDate = "";
                    if (txtDepositToDate.Text != "")
                    {
                        ToDate = txtDepositToDate.Text;
                        ToDate = Convert.ToDateTime(ToDate).ToString("yyyy/MM/dd");
                    }
                    else
                    {
                        //ToDate = Convert.ToString(default(DateTime));

                        ToDate = "";
                    }

                    //inserting data into table tbl_GLKYC_BankGoldGetails
                    insertQuery = "insert into tbl_GLBankGold_BasicDetails values('" + BankGoldID + "', '" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "', " +
                                "'" + RefType.Trim() + "', '" + Session["RefNo"] + "', '" + Session["RefId"] + "', '" + Session["RefNum"] + "', " +
                                "'" + Session["ID"] + "', '" + txtRefNo.Text.Trim() + "','" + ddlLocationType.SelectedValue + "', '" + txtLocationNo.Text.Trim() + "', " +
                                "'" + Convert.ToDateTime(txtDepositFromDate.Text).ToString("yyyy/MM/dd") + "', " +
                                "'" + ToDate + "', '" + txtInterestRate.Text.Trim() + "', " +
                                "'" + Session["BankID"] + "','" + txtUniqueID.Text.Trim() + "','" + Session["FYearID"] + "','" + Session["branchId"] + "','" + txtOperatorID.Text + "') ";

                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        datasaved = true;
                    }
                }

                if (ddlLocationType.SelectedValue == "Office")
                {
                    Lt = "";
                    Lt = LocationType.Substring(0, 3).ToUpper();
                    RefType = "BG" + "/" + Lt;

                    //inserting data into table tbl_GLKYC_BankGoldGetails
                    insertQuery = "insert into tbl_GLBankGold_BasicDetails values('" + BankGoldID + "', '" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "', " +
                                 "'" + RefType.Trim() + "', '" + Session["RefNo"] + "', '" + Session["RefId"] + "', '" + Session["RefNum"] + "', " +
                                "'" + Session["ID"] + "', '" + txtRefNo.Text.Trim() + "','" + ddlLocationType.SelectedValue + "', '" + txtLocationNo.Text.Trim() + "', " +
                                "'', " +
                                "'', '', " +
                                "'','','" + Session["FYearID"] + "','" + Session["branchId"] + "','" + txtOperatorID.Text + "') ";

                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        datasaved = true;
                    }
                }
                if (ddlLocationType.SelectedValue == "Home")
                {
                    Lt = "";
                    Lt = LocationType.Substring(0, 3).ToUpper();
                    RefType = "BG" + "/" + Lt;

                    //inserting data into table tbl_GLKYC_BankGoldGetails
                    insertQuery = "insert into tbl_GLBankGold_BasicDetails values('" + BankGoldID + "', '" + Convert.ToDateTime(txtRefDate.Text).ToString("yyyy/MM/dd") + "', " +
                                 "'" + RefType.Trim() + "', '" + Session["RefNo"] + "', '" + Session["RefId"] + "', '" + Session["RefNum"] + "', " +
                                "'" + Session["ID"] + "', '" + txtRefNo.Text.Trim() + "','" + ddlLocationType.SelectedValue + "', '" + txtLocationNo.Text.Trim() + "', " +
                                "'', " +
                                "'', '', " +
                                "'','','" + Session["FYearID"] + "','" + Session["branchId"] + "','" + txtOperatorID.Text + "') ";

                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        datasaved = true;
                    }
                }

                // 2] Data insertion into tbl_GLKYC_App_BankGoldGetails
                if (datasaved == true)
                {
                    if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
                    {
                        foreach (GridViewRow row in dgvCustomerDetails.Rows)
                        {
                            CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                            if (chk != null && chk.Checked)
                            {

                                int BGID = 0;
                                //getting MAX BGID
                                strQuery = "Select MAX(BGAppID) from tbl_GLBankGold_AppDetails ";
                                cmd = new SqlCommand(strQuery, conn, transaction);
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    BGID = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                                else
                                {
                                    BGID = 0;
                                }

                                BGID += 1;

                                GoldLoanNo = (row.Cells[1].FindControl("lblGoldLoanNo") as Label).Text;
                                string ApplicantName = (row.Cells[2].FindControl("lblApplicantName") as Label).Text;
                                string TotalNetWeight = (row.Cells[3].FindControl("lblTotalNetWeight") as Label).Text;
                                string AppID = (row.Cells[4].FindControl("lblAppID") as Label).Text;

                                //inserting data into table tbl_GLKYC_App_BankGoldGetails
                                insertQuery = "insert into tbl_GLBankGold_AppDetails values('" + BGID + "', '" + BankGoldID + "', '" + GoldLoanNo + "', " +
                                                "'" + TotalNetWeight + "', '" + AppID + "')";
                                cmd = new SqlCommand(insertQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                            }
                        }
                    }
                }

                if (datasaved == true)
                {
                    transaction.Commit();
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Saved Successfully.');", true);
                    BindCustomerDetails();
                    ClearData();
                    //DisablAllFields
                    DisableAllFields();
                    conn.Close();
                }
                else
                {
                    transaction.Rollback();
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Saved Successfully.');", true);
                }

            }
            #endregion [Save]
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

    #region[ValidateDate]
    protected bool validatedata()
    {
        bool valid = false;
        int Count = 0;

        try
        {
            if (txtRefDate.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Reference Date.');", true);
                // txtUniqueID.Text = "";
                // ddlLocationType.SelectedIndex = 0;
                valid = false;
                return valid;
            }

            if (ddlLocationType.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Location Type.');", true);
                //txtUniqueID.Text = "";
                //ddlLocationType.SelectedIndex = 0;
                valid = false;
                return valid;
            }
            if (ddlLocationType.SelectedItem.Text == "Locker")
            {
                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    // txtUniqueID.Text = "";
                    // ddlLocationType.SelectedIndex = 0;
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
                    // txtUniqueID.Text = "";
                    //ddlLocationType.SelectedIndex = 0;
                    valid = false;
                    return valid;
                }

                else
                {
                    valid = true;
                    // return valid;
                }
            }

            if (ddlLocationType.SelectedItem.Text == "OD")
            {
                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    //  txtUniqueID.Text = "";
                    // ddlLocationType.SelectedIndex = 0;
                    valid = false;
                    return valid;
                }
                else if (txtBranchName.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Branch Name.');", true);
                    // txtUniqueID.Text = "";
                    // ddlLocationType.SelectedIndex = 0;
                    valid = false;
                    return valid;
                }
                else if (txtLocationNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                    // ddlLocationType.SelectedIndex = 0;
                    //txtUniqueID.Text = "";
                    valid = false;
                    return valid;
                }
                else if (txtUniqueID.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Unique Bank ID.');", true);
                    // txtRefNo.Text = "";
                    //  txtUniqueID.Text = "";

                    valid = false;
                }
                else if (txtInterestRate.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Interest Rate.');", true);
                    // txtUniqueID.Text = "";
                    valid = false;
                    return valid;
                }
                else if (txtDepositFromDate.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter DepositeFromDate.');", true);
                    // txtUniqueID.Text = "";
                    //ddlLocationType.SelectedIndex = 0;
                    valid = false;
                    return valid;
                }

                else if (txtDepositToDate.Text != "" && txtDepositFromDate.Text != "")
                {
                    DateTime FromDate = Convert.ToDateTime(txtDepositFromDate.Text);
                    DateTime ToDate = Convert.ToDateTime(txtDepositToDate.Text);
                    if (ToDate.CompareTo(FromDate) < 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Deposite From Date Must Be Less Than Deposite To Date.');", true);
                        //txtUniqueID.Text = "";
                        valid = false;
                        return valid;
                    }
                    else
                    {
                        valid = true;
                        // return valid;
                    }
                }

                else
                {
                    valid = true;
                    // return valid;
                }


            }

            if (ddlLocationType.SelectedItem.Text == "Office")
            {
                if (txtLocationNo.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                    valid = false;
                    return valid;
                }
                else
                {
                    valid = true;
                    // return valid;
                }
            }
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
                    if (Count > 0)
                    {
                        valid = true;
                        // return valid;
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Customer.');", true);
                        valid = false;
                        return valid;
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

                if (ddlLocationType.SelectedIndex == 2)
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
                txtUniqueID.Enabled = false;
                txtUniqueID.Text = "";
                txtInterestRate.Text = "";
                txtDepositFromDate.Text = "";
                txtDepositToDate.Text = "";

                //Function To Create Reference No
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
                txtUniqueID.Enabled = false;
                txtUniqueID.Text = "";
                txtInterestRate.Text = "";
                txtDepositFromDate.Text = "";
                txtDepositToDate.Text = "";
                //Function To Create Reference No
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
                txtUniqueID.Enabled = true;
                //Function To Create Reference No
                CreateReferenceNo();
            }
                //select location type Home-   :kishor 08 oct 2014
            else if (ddlLocationType.SelectedValue == "Home")
            {
                txtBranchName.Enabled = false;
                ddlBankName.Enabled = false;
                txtDepositFromDate.Enabled = false;
                txtDepositToDate.Enabled = false;
                txtInterestRate.Enabled = false;
                txtLocationNo.Enabled = true;
                txtRefDate.Enabled = true;
                txtBranchName.Text = "";
                txtUniqueID.Enabled = false;
                txtUniqueID.Text = "";
                txtInterestRate.Text = "";
                txtDepositFromDate.Text = "";
                txtDepositToDate.Text = "";

                //Function To Create Reference No
                CreateReferenceNo();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LocationType", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion

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
            txtUniqueID.Enabled = false;
            //txtRefDate.Enabled = false;
            //  txtRefDate.CssClass. = false;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DisableFieldsAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion

    #region [ddlBankName_SelectedIndexChanged]
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
                CreateReferenceNo();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ddlBankName_SelectedIndexChanged]

    #region[txtUniqueID_TextChanged]
    protected void txtUniqueID_TextChanged(object sender, EventArgs e)
    {
        try
        {

            CreateReferenceNo();

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[txtUniqueID_TextChanged]

    #region[ValidationForRefNo]
    protected bool ValidationForRefNo()
    {
        bool valid = false;

        try
        {

            int count = 0;


            if (ddlLocationType.SelectedIndex == 0)
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Location Type.');", true);
                txtUniqueID.Text = "";
                valid = false;
            }

            else if (txtLocationNo.Text == "")
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Location No.');", true);
                txtUniqueID.Text = "";
                valid = false;
            }
            else if (ddlLocationType.SelectedIndex == 1)
            {

                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    txtUniqueID.Text = "";
                    valid = false;
                }
                //else if (txtUniqueID.Text == "")
                //{
                //    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Unique Bank ID.');", true);
                //    txtUniqueID.Text = "";
                //    valid = false;
                //}
            }

            else if (ddlLocationType.SelectedIndex == 2)
            {

                if (ddlBankName.SelectedIndex == 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Bank Name.');", true);
                    txtUniqueID.Text = "";
                    valid = false;
                }
                else if (txtUniqueID.Text == "")
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Enter Unique Bank ID.');", true);
                    txtUniqueID.Text = "";
                    valid = false;
                }
            }


            else if (dgvCustomerDetails != null && dgvCustomerDetails.Rows.Count > 0)
            {
                foreach (GridViewRow row in dgvCustomerDetails.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelect");

                    if (chk != null && chk.Checked)
                    {

                        count = count + 1;


                    }
                }

                if (count >= 1)
                {
                    valid = true;
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Customers.');", true);
                    valid = false;
                    txtUniqueID.Text = "";
                }
            }
            else
            {

                valid = true;
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
        return valid;
    }
    #endregion[ValidationForRefNo]

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
                    CreateReferenceNo();
                }
            }
        }
        if (ddlLocationType.SelectedIndex == 2)
        {
            //if (count > 1)
            //{

            //   // ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Select Single Customer For OD.');", true);
            //}
            //else
            //{
                CreateReferenceNo();
            //}

        }
        if (count >= 1)
        {
            CreateReferenceNo();
        }


        //////////////
        if (ddlLocationType.SelectedIndex == 2)
        {
            CheckBox tmp = (CheckBox)sender;

            foreach (GridViewRow c in dgvCustomerDetails.Rows)
            {
                CheckBox chk = (CheckBox)c.FindControl("chkSelect");
                chk.CheckedChanged += ChkChanged;
                chk.Checked = false;
                //CreateReferenceNo();
            }

            tmp.Checked = true;
            CreateReferenceNo();
        }

        
    }
    #endregion [ChkChanged]

    //protected void txtRefDate_TextChanged(object sender, EventArgs e)
    //{
    //    try
    //    {

    //        strQuery = "SELECT StartDate,EndDate FROM tblFinancialyear WHERE FinancialyearID='" + Session["FYearID"] + "'";

    //        conn = new SqlConnection(strConnString);
    //        cmd = new SqlCommand(strQuery, conn);
    //        da = new SqlDataAdapter(cmd);
    //        ds = new DataSet();
    //        da.Fill(ds);
          
    //              DateTime Sdate = Convert.ToString(ds.Tables[0].Rows[0][0]);
    //              string Edate = Convert.ToString(ds.Tables[0].Rows[0][1]);

    //      if(!txtRefDate.Text>=Sdate ||Convert.ToDateTime(txtRefDate.Text)<=Edate)



    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
    //    }

    //}
}
