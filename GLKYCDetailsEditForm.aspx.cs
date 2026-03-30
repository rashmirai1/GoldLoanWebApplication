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

public partial class GLKYCDetailsEditForm : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string RefType = "AF";
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

                //getting Operator Name and ID
                if (Convert.ToString(Session["username"]) != "" && Convert.ToString(Session["username"]) != null)
                {
                    txtOperatorName.Text = Convert.ToString(Session["username"]);
                }

                if (Convert.ToString(Session["userID"]) != "" && Convert.ToString(Session["userID"]) != null)
                {
                    txtOperatorID.Text = Convert.ToString(Session["userID"]);
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

                //getting FYear ID
                if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
                {
                    FYearID = Convert.ToInt32(Session["FYearID"]);
                    txtFYID.Text = Convert.ToString(FYearID); ;
                }

                //binding GridView
                BindDGVDetails();

                //binding GridView
                BindDocumentDetails();

                //binding DropDownList Search By
                BindDDLSearchBy();

                //Fill Combo State
                FillState();

                //added on onblur event attribute
                txtBirthDate.Attributes.Add("onblur", this.Page.ClientScript.GetPostBackEventReference(this.txtBirthDate, ""));
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

    #region [Bind GridView Document Details]
    protected void BindDocumentDetails()
    {
        try
        {
            strQuery = "select tbl_GLKYC_DocumentDetails.DID, tbl_GLKYC_DocumentDetails.DocumentID, " +
                        "tbl_GLDocumentMaster.DocumentName as 'DocName', tbl_GLKYC_DocumentDetails.DocRecd, " +
                        "tbl_GLKYC_DocumentDetails.ImagePath, ImageUrl=tbl_GLKYC_DocumentDetails.ImagePath, " +
                        "tbl_GLKYC_DocumentDetails.OtherDoc " +
                       "from tbl_GLKYC_DocumentDetails " +
                       "inner join tbl_GLDocumentMaster " +
                                "on tbl_GLKYC_DocumentDetails.DocumentID=tbl_GLDocumentMaster.DocumentID " +
                       "where tbl_GLKYC_DocumentDetails.GoldLoanNo='" + txtGoldLoanNo.Text.Trim() + "' ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                dgvDocumentDetails.DataSource = dt;
                dgvDocumentDetails.DataBind();
            }
            else
            {
                dt = new DataTable();
                dt.Columns.Add("DID", typeof(string));
                dt.Columns.Add("DocumentID", typeof(string));
                dt.Columns.Add("DocName", typeof(string));
                dt.Columns.Add("DocRecd", typeof(string));
                dt.Columns.Add("ImagePath", typeof(string));
                dt.Columns.Add("ImageUrl", typeof(string));
                dt.Columns.Add("OtherDoc", typeof(string));

                ShowNoResultFound(dt, dgvDocumentDetails);
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind GridView Document Details]

    #region [Bind GridView DGVDetails]
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "select tbl_GLKYC_BasicDetails.KYCID, tbl_GLKYC_BasicDetails.GoldLoanNo, LoanDate=Convert(varchar,tbl_GLKYC_BasicDetails.LoanDate,103), " +
                        "tbl_GLKYC_BasicDetails.SourceofApplication,tbl_GLKYC_BasicDetails.SourceSpecification, tbl_GLKYC_BasicDetails.DealerID " +
                        "ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' +tbl_GLKYC_ApplicantDetails.AppMName+' ' +tbl_GLKYC_ApplicantDetails.AppLName, " +
                        "tbl_GLKYC_ApplicantDetails.PANNo, tbl_GLKYC_ApplicantDetails.MobileNo, tbl_GLKYC_BasicDetails.LoanType, " +
                        "NomineeName=tbl_GLKYC_ApplicantDetails.NomFName+' ' +tbl_GLKYC_ApplicantDetails.NomMName+' ' +tbl_GLKYC_ApplicantDetails.NomLName " +
                       "from tbl_GLKYC_BasicDetails " + 
                       "inner join tbl_GLKYC_ApplicantDetails " +
                                "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                       "where tbl_GLKYC_BasicDetails.FYID='" + txtFYID.Text + "' " +
                       "and tbl_GLKYC_BasicDetails.BranchID='" + txtBranchID.Text + "'";

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

    #region [Bind DropDownList-SearchBy]
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("GoldLoanNo");
            ddlSearchBy.Items.Add("LoanDate");
            ddlSearchBy.Items.Add("LoanType");
            ddlSearchBy.Items.Add("ApplicantName");
            ddlSearchBy.Items.Add("PANNo");
            ddlSearchBy.Items.Add("MobileNo");
            ddlSearchBy.Items.Add("NomineeName");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Bind DropDownList-SearchBy]

    #region [dgvDetails_RowCommand]
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
                string ID = Convert.ToString(_gridView.DataKeys[_selectedIndex].Value.ToString());

                #region [Delete Record]
                if (_commandName == "DeleteRecord")
                {
                    //checking whether Gold Loan A/C is processed to next stage (Sanction/Disburse)
                    strQuery = "select count(*) from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + ID + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since it is being processed to Sanction/Disburse.');", true);
                    }

                    if (existcount == 0)
                    {
                        datasaved = false;
                        int QueryResult = 0;
                        //deleting record from DB
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                        //checking whether Gold Loan A/C is present
                        strQuery = "select count(*) from tbl_GLKYC_BasicDetails where GoldLoanNo='" + ID + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            deleteQuery = "delete from tbl_GLKYC_DocumentDetails where GoldLoanNo='" + ID + "'";
                            cmd = new SqlCommand(deleteQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }

                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLKYC_AddressDetails where GoldLoanNo='" + ID + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                            }

                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLKYC_ApplicantDetails where GoldLoanNo='" + ID + "'";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    datasaved = true;
                                }
                            }

                            if (datasaved == true)
                            {
                                deleteQuery = "delete from tbl_GLKYC_BasicDetails where GoldLoanNo='" + ID + "'";
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
                                BindDGVDetails();

                                //if the same record is deleted which is filled in the form.
                                if (txtID.Text != "" && txtID.Text != null)
                                {
                                    if (ID == Convert.ToString(txtGoldLoanNo.Text))
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

                #region [Update Record]
                if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                    txtID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtGoldLoanNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]).Trim();
                    txtLoanDate.Text = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][2]).ToString("dd/MM/yyyy").Trim();
                    txtPANNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][4]).ToUpper();
                    txtMobileNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][5]).Trim();
                    ddlLoanType.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][6]).Trim();
                    

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    //Applicant details
                    strQuery = "select tbl_GLKYC_BasicDetails.OperatorID, tbl_GLKYC_BasicDetails.ExistingGLNo, " +
                                        "tbl_GLKYC_BasicDetails.SourceofApplication, tbl_GLKYC_BasicDetails.SourceSpecification, " +
                                        "tbl_GLKYC_BasicDetails.DealerID, " +
                                        "tbl_GLKYC_ApplicantDetails.AppFName, tbl_GLKYC_ApplicantDetails.AppMName, " +
                                        "tbl_GLKYC_ApplicantDetails.AppLName, tbl_GLKYC_ApplicantDetails.Gender, " +
                                        "tbl_GLKYC_ApplicantDetails.BirthDate, tbl_GLKYC_ApplicantDetails.Age, " +
                                        "tbl_GLKYC_ApplicantDetails.PANNo, tbl_GLKYC_ApplicantDetails.MaritalStatus, " +
                                        "tbl_GLKYC_ApplicantDetails.MobileNo, tbl_GLKYC_ApplicantDetails.TelephoneNo, " +
                                        "tbl_GLKYC_ApplicantDetails.EmailID, tbl_GLKYC_ApplicantDetails.NomFName, " +
                                        "tbl_GLKYC_ApplicantDetails.NomMName, tbl_GLKYC_ApplicantDetails.NomLName, " +
                                        "tbl_GLKYC_ApplicantDetails.NomRelation, tbl_GLKYC_ApplicantDetails.AppPhotoPath, " +
                                        "tbl_GLKYC_ApplicantDetails.AppSignPath, Operator=UserDetails.UserName, " +
                                        "tbl_GLKYC_ApplicantDetails.NomAddress, tbl_GLKYC_BasicDetails.ExistingPLCaseNo, FYID " +
                                "from tbl_GLKYC_BasicDetails " +
                                "inner join tbl_GLKYC_ApplicantDetails " +
                                        "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "inner join UserDetails " +
                                        "on tbl_GLKYC_BasicDetails.OperatorID=UserDetails.UserID " +
                                "where tbl_GLKYC_BasicDetails.GoldLoanNo='" + ID + "'";
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    txtOperatorID.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
                    txtExistingGLNo.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    ddlSourceofApplication.Text = Convert.ToString(ds.Tables[0].Rows[0][2]); //by kishor
                    txtSpecifySource.Text = Convert.ToString(ds.Tables[0].Rows[0][3]); //by kishor
                    ddlDealerName.Text  = Convert.ToString(ds.Tables[0].Rows[0][4]);  //by kishor
                    txtAppFName.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                    txtAppMName.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
                    txtAppLName.Text = Convert.ToString(ds.Tables[0].Rows[0][7]);
                    ddlGender.Text = Convert.ToString(ds.Tables[0].Rows[0][8]);
                    txtBirthDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][9]).ToString("dd/MM/yyyy").Trim();
                    txtAge.Text = Convert.ToString(ds.Tables[0].Rows[0][10]);
                    txtPANNo.Text = Convert.ToString(ds.Tables[0].Rows[0][11]);
                    ddlMaritalStatus.Text = Convert.ToString(ds.Tables[0].Rows[0][12]);
                    txtMobileNo.Text = Convert.ToString(ds.Tables[0].Rows[0][13]);
                    txtTelephoneNo.Text = Convert.ToString(ds.Tables[0].Rows[0][14]);
                    txtEmailId.Text = Convert.ToString(ds.Tables[0].Rows[0][15]);
                    txtNomFName.Text = Convert.ToString(ds.Tables[0].Rows[0][16]);
                    txtNomMName.Text = Convert.ToString(ds.Tables[0].Rows[0][17]);
                    txtNomLName.Text = Convert.ToString(ds.Tables[0].Rows[0][18]);
                    ddlNomRelation.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][19]);
                    txtAppPhotoPath.Text = Convert.ToString(ds.Tables[0].Rows[0][20]);
                    txtAppSignPath.Text = Convert.ToString(ds.Tables[0].Rows[0][21]);
                    txtOperatorName.Text = Convert.ToString(ds.Tables[0].Rows[0][22]);
                    txtNomineeAddress.Text = Convert.ToString(ds.Tables[0].Rows[0][23]);
                    txtExistingPLCaseNo.Text = Convert.ToString(ds.Tables[0].Rows[0][24]);
                    txtFYID.Text = Convert.ToString(ds.Tables[0].Rows[0][25]);
                    imgAppPhoto.ImageUrl = txtAppPhotoPath.Text;
                    imgAppPhoto.Visible = true;
                    imgAppSign.ImageUrl = txtAppSignPath.Text;
                    imgAppSign.Visible = true;

                    //Address Details
                    strQuery = "select tbl_GLKYC_AddressDetails.AddrID, tbl_GLKYC_AddressDetails.BldgHouseName, " +
                                        "tbl_GLKYC_AddressDetails.Road, tbl_GLKYC_AddressDetails.BldgPlotNo, " +
                                        "tbl_GLKYC_AddressDetails.RoomBlockNo, tbl_GLKYC_AddressDetails.StateID, " +
                                        "tbl_GLKYC_AddressDetails.CityID, tbl_GLKYC_AddressDetails.AreaID, " +
                                        "tbl_GLKYC_AddressDetails.ZoneID, tbl_GLKYC_AddressDetails.Landmark " +
                                "from tbl_GLKYC_BasicDetails " +
                                "inner join tbl_GLKYC_AddressDetails " +
                                        "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                "where tbl_GLKYC_BasicDetails.GoldLoanNo='" + ID + "'";
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand(strQuery, conn);
                    da = new SqlDataAdapter(cmd);
                    ds = new DataSet();
                    da.Fill(ds);

                    txtBldgHouseName.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                    txtRoad.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                    txtPlotNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                    txtRoomBlockNo.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
                    ddlState.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][5]);
                    FillCity();
                    ddlCity.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]);
                    FillArea();
                    ddlArea.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][7]);
                    FillZone();
                    ddlZone.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][8]);
                    txtNearestLandmark.Text = Convert.ToString(ds.Tables[0].Rows[0][9]);

                    //Document Details
                    strQuery = "select tbl_GLKYC_DocumentDetails.DID, tbl_GLKYC_DocumentDetails.DocumentID, " +
                                        "tbl_GLDocumentMaster.DocumentName as 'DocName', tbl_GLKYC_DocumentDetails.DocRecd, " +
                                        "tbl_GLKYC_DocumentDetails.ImagePath, ImageUrl=tbl_GLKYC_DocumentDetails.ImagePath, " +
                                        "tbl_GLKYC_DocumentDetails.OtherDoc " + 
                                "from tbl_GLKYC_DocumentDetails " +
                                "inner join tbl_GLDocumentMaster " +
                                        "on tbl_GLKYC_DocumentDetails.DocumentID=tbl_GLDocumentMaster.DocumentID " +
                                "where tbl_GLKYC_DocumentDetails.GoldLoanNo='" + ID + "'";
                    conn = new SqlConnection(strConnString);
                    da = new SqlDataAdapter(strQuery, conn);
                    dt = new DataTable();
                    da.Fill(dt);//POP up
                    if (dt.Rows.Count > 0)
                    {
                        Session["dtDocumentDetails"] = dt;
                        dgvDocumentDetails.DataSource = dt;
                        dgvDocumentDetails.DataBind();
                    }
                    else
                    {
                        dt = new DataTable();
                        dt.Columns.Add("DID", typeof(string));
                        dt.Columns.Add("DocumentID", typeof(string));
                        dt.Columns.Add("DocName", typeof(string));
                        dt.Columns.Add("DocRecd", typeof(string));
                        dt.Columns.Add("ImagePath", typeof(string));
                        dt.Columns.Add("ImageUrl", typeof(string));
                        dt.Columns.Add("OtherDoc", typeof(string));

                        ShowNoResultFound(dt, dgvDocumentDetails);
                    }

                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";

                    //checking whether Gold Loan A/C is processed to next stage (Sanction/Disburse)
                    strQuery = "select count(*) from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + ID + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot edit record since it is being processed to Sanction/Disburse.');", true);
                    }
                }
                #endregion [Update Record]
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
    #endregion [dgvDetails_RowCommand]

    #region [dgvDetails_PageIndexChanging]
    protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDetails.PageIndex = e.NewPageIndex;
            Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDetails_PageIndexChanging]

    #region [dgvDocument_RowCommand]
    protected void dgvDocument_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "DeleteRecord")
            {
                GridView _gridView = (GridView)sender;
                int index = Convert.ToInt32(e.CommandArgument);
                string DID = Convert.ToString(_gridView.DataKeys[index].Value.ToString());

                if (strDID.Length == 0)
                {
                    strDID = DID;
                }
                else
                {
                    strDID = strDID + "," + DID;
                }

                if (txtDID.Text.Trim() != "")
                {
                    txtDID.Text += "," + strDID;
                }
                else
                {
                    txtDID.Text = strDID;
                }

                GridViewRow row = dgvDocumentDetails.Rows[index];
                DataTable dtDocumentDetails = new DataTable();
                dtDocumentDetails = (DataTable)Session["dtDocumentDetails"];

                if ((dgvDocumentDetails.Rows.Count > 0) && (dgvDocumentDetails.Rows.Count != 1))  //Checks whether list contains items
                {
                    if (dtDocumentDetails != null)
                    {
                        dtDocumentDetails.Rows.RemoveAt(index);
                        dgvDocumentDetails.DataSource = dtDocumentDetails;
                        dgvDocumentDetails.DataBind();
                    }
                }
                else
                {
                    dtDocumentDetails.Rows.RemoveAt(index);
                    ShowNoResultFound(dtDocumentDetails, dgvDocumentDetails);
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVRowCommdAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDocument_RowCommand]

    #region [dgvDocument_PageIndexChanging]
    protected void dgvDocument_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDocumentDetails.PageIndex = e.NewPageIndex;
            BindDocumentDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVDocPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [dgvDocument_PageIndexChanging]

    #region [GetRecords]
    protected DataSet GetRecords(SqlConnection conn, string CommandName, string ID)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "select tbl_GLKYC_BasicDetails.KYCID, tbl_GLKYC_BasicDetails.GoldLoanNo, LoanDate=Convert(varchar,tbl_GLKYC_BasicDetails.LoanDate,103), " +
                                    "tbl_GLKYC_BasicDetails.SourceofApplication, tbl_GLKYC_BasicDetails.SourceSpecification, " +
                                        "tbl_GLKYC_BasicDetails.DealerID, " +
                                    "ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' +tbl_GLKYC_ApplicantDetails.AppMName+' ' +tbl_GLKYC_ApplicantDetails.AppLName, " +
                                    "tbl_GLKYC_ApplicantDetails.PANNo, tbl_GLKYC_ApplicantDetails.MobileNo, tbl_GLKYC_BasicDetails.LoanType, " +
                                    "NomineeName=tbl_GLKYC_ApplicantDetails.NomFName+' ' +tbl_GLKYC_ApplicantDetails.NomMName+' ' +tbl_GLKYC_ApplicantDetails.NomLName " +
                            "from tbl_GLKYC_BasicDetails " +
                            "inner join tbl_GLKYC_ApplicantDetails " +
                                    "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                            "where tbl_GLKYC_BasicDetails.FYID='" + txtFYID.Text + "' and tbl_GLKYC_BasicDetails.BranchID='" + txtBranchID.Text + "' "; 
            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "select tbl_GLKYC_BasicDetails.KYCID, tbl_GLKYC_BasicDetails.GoldLoanNo, tbl_GLKYC_BasicDetails.LoanDate, " +
                                     "tbl_GLKYC_BasicDetails.SourceofApplication, tbl_GLKYC_BasicDetails.SourceSpecification, " +
                                     "tbl_GLKYC_BasicDetails.DealerID, " +
                                    "ApplicantName=tbl_GLKYC_ApplicantDetails.AppFName+' ' +tbl_GLKYC_ApplicantDetails.AppMName+' ' +tbl_GLKYC_ApplicantDetails.AppLName, " +
                                    "tbl_GLKYC_ApplicantDetails.PANNo, tbl_GLKYC_ApplicantDetails.MobileNo, tbl_GLKYC_BasicDetails.LoanType, " +
                                    "NomineeName=tbl_GLKYC_ApplicantDetails.NomFName+' ' +tbl_GLKYC_ApplicantDetails.NomMName+' ' +tbl_GLKYC_ApplicantDetails.NomLName " +
                            "from tbl_GLKYC_BasicDetails " +
                            "inner join tbl_GLKYC_ApplicantDetails " +
                                    "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                            "where tbl_GLKYC_BasicDetails.GoldLoanNo='" + ID + "'";
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
    #endregion [GetRecords]

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
            txtGoldLoanNo.Text = "";
            txtLoanDate.Text = "";
            txtOperatorName.Text = "";
            txtExistingGLNo.Text = "";
            imgAppPhoto.ImageUrl = "";
            imgAppSign.ImageUrl = "";
            txtAppFName.Text = "";
            txtAppMName.Text = "";
            txtAppLName.Text = "";
            ddlLoanType.SelectedValue = "0";
            ddlGender.SelectedValue = "0";
            txtExistingPLCaseNo.Text = "";
            txtPANNo.Text = "";
            txtBirthDate.Text = "";
            ddlMaritalStatus.SelectedValue = "0";
            txtAge.Text = "";
            txtBldgHouseName.Text = "";
            txtRoad.Text = "";
            txtPlotNo.Text = "";
            txtRoomBlockNo.Text = "";
            txtNearestLandmark.Text = "";
            txtMobileNo.Text = "";
            txtTelephoneNo.Text = "";
            txtEmailId.Text = "";
            txtNomFName.Text = "";
            txtNomMName.Text = "";
            txtNomLName.Text = "";
            txtNomineeAddress.Text = "";
            ddlNomRelation.SelectedValue = "";
            txtID.Text = "";
            txtOperatorID.Text = "";
            txtAppPhotoPath.Text = "";
            txtAppSignPath.Text = "";
            txtSearch.Text = "";
            txtDID.Text = "";
            btnSave.Text = "Update";
            btnReset.Text = "Cancel";
            ddlState.DataSource = null;
            ddlState.DataBind();
            ddlCity.DataSource = null;
            ddlCity.Items.Clear();
            ddlArea.DataSource = null;
            ddlArea.Items.Clear();
            ddlZone.DataSource = null;
            ddlZone.Items.Clear();

            //Fill Combo State
            FillState();

            //Fill Combo City
            ddlCity.Items.Insert(0, new ListItem("--Select City--"));

            //Fill Combo Area
            ddlArea.Items.Insert(0, new ListItem("--Select Area--"));

            //Fill Combo Zone
            ddlZone.Items.Insert(0, new ListItem("--Select Zone--"));

            //Bind GridView Document Details
            dt = new DataTable();
            dt.Columns.Add("DID", typeof(string));
            dt.Columns.Add("DocumentID", typeof(string));
            dt.Columns.Add("DocName", typeof(string));
            dt.Columns.Add("DocRecd", typeof(string));
            dt.Columns.Add("ImagePath", typeof(string));
            dt.Columns.Add("ImageUrl", typeof(string));
            dt.Columns.Add("OtherDoc", typeof(string));

            ShowNoResultFound(dt, dgvDocumentDetails);

            //getting FYear ID
            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
                txtFYID.Text = Convert.ToString(FYearID); ;
            }

            BindDocumentDetails();
            BindDGVDetails();
            
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
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ClearData]

    #region [Search Function]
    protected void Search()
    {
        try
        {
            //Search Records
            DataTable dt = GetRecords(conn, "GetAllRecords", "0").Tables[0];
            DataView dv = new DataView(dt);
            string SearchExpression = null;
            string SearchBy = ddlSearchBy.Text;

            if (!String.IsNullOrEmpty(txtSearch.Text))
            {
                SearchExpression = string.Format("{0} '{1}%'", dgvDetails.SortExpression, txtSearch.Text);
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
    #endregion [Search Function]

    #region Search Record
    protected void btnSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            //Search Records
            Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int DID = 0;
            datasaved = false;
            conn = new SqlConnection(strConnString);
            conn.Open();

            #region [Update]
            if (Page.IsValid)
            {
                if (btnSave.Text == "Update")
                {
                    //checking whether Gold Loan A/C is processed to next stage (Sanction/Disburse)
                    strQuery = "select count(*) from tbl_GLSanctionDisburse_BasicDetails where GoldLoanNo='" + txtGoldLoanNo.Text.Trim() + "'";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot update record since it is being processed to Sanction/Disburse.');", true);
                    }

                    if (existcount == 0)
                    {
                        int QueryResult = 0;
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                        string GoldLoanNo = txtGoldLoanNo.Text.Trim();
                        datasaved = true;

                        // 1] Data updation tbl_GLKYC_DocumentDetails
                        int DocumentID = 0;
                        int valDID = 0;
                        string DocRecd = string.Empty;
                        string ImagePath = string.Empty;
                        string OtherDoc = string.Empty;

                        if (dgvDocumentDetails.Rows.Count > 0)
                        {
                            string valID = string.Empty;
                            txtDID.Text = "";
                            foreach (GridViewRow row in dgvDocumentDetails.Rows)
                            {
                                if ((row.Cells[1].FindControl("lblDID") as Label).Text.Trim() != "")
                                {
                                    valID = Convert.ToString((row.Cells[1].FindControl("lblDID") as Label).Text);
                                    if (txtDID.Text.Trim() != "")
                                    {
                                        txtDID.Text += "," + valID;
                                    }
                                    else
                                    {
                                        txtDID.Text = valID;
                                    }
                                }
                                else
                                {
                                    valID = "";
                                }
                            }
                        }

                        if (txtDID.Text.Trim() != "")
                        {
                            //deleting record from tbl_GLKYC_DocumentDetails
                            deleteQuery = "delete from tbl_GLKYC_DocumentDetails where DID NOT IN (" + txtDID.Text.Trim() + ")";
                            cmd = new SqlCommand(deleteQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult >= 0)
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

                        //insertion/updation of data into tbl_GLKYC_DocumentDetails

                        if (datasaved)
                        {
                            if (dgvDocumentDetails.Rows.Count > 0)
                            {
                                foreach (GridViewRow row in dgvDocumentDetails.Rows)
                                {
                                    if (datasaved == true)
                                    {
                                        if ((row.Cells[1].FindControl("lblDID") as Label).Text.Trim() != "")
                                        {
                                            valDID = Convert.ToInt32((row.Cells[1].FindControl("lblDID") as Label).Text);
                                        }
                                        else
                                        {
                                            valDID = 0;
                                        }

                                        DocumentID = Convert.ToInt32((row.Cells[2].FindControl("lblDocumentID") as Label).Text);
                                        OtherDoc = Convert.ToString((row.Cells[4].FindControl("lblSpecifyOther") as Label).Text);
                                        DocRecd = Convert.ToString((row.Cells[5].FindControl("lblReceived") as Label).Text);
                                        ImagePath = Convert.ToString((row.Cells[6].FindControl("lblImagePath") as Label).Text);

                                        if (valDID == 0)
                                        {
                                            //getting MAX DID
                                            strQuery = "Select MAX(DID) from tbl_GLKYC_DocumentDetails ";
                                            cmd = new SqlCommand(strQuery, conn, transaction);
                                            if (cmd.ExecuteScalar() != DBNull.Value)
                                            {
                                                DID = Convert.ToInt32(cmd.ExecuteScalar());
                                            }
                                            else
                                            {
                                                DID = 0;
                                            }

                                            DID += 1;

                                            //inserting data into table tbl_GLKYC_DocumentDetails
                                            insertQuery = "insert into tbl_GLKYC_DocumentDetails values('" + DID + "', '" + GoldLoanNo.Trim() + "', " +
                                                                "'" + DocumentID + "', '" + DocRecd.Trim() + "', '" + ImagePath.Trim() + "', '" + OtherDoc.Trim() + "')";

                                            cmd = new SqlCommand(insertQuery, conn, transaction);
                                            QueryResult = cmd.ExecuteNonQuery();

                                            if (QueryResult > 0)
                                            {
                                                datasaved = true;
                                            }
                                        }
                                        else if (valDID > 0)
                                        {
                                            //updating table tbl_GLKYC_DocumentDetails
                                            updateQuery = "update tbl_GLKYC_DocumentDetails set DocumentID='" + DocumentID + "', " +
                                                                "DocRecd='" + DocRecd.Trim() + "', " +
                                                                "ImagePath='" + ImagePath.Trim() + "', " +
                                                                "OtherDoc='" + OtherDoc.Trim() + "' " +
                                                          "where DID='" + valDID + "'";

                                            cmd = new SqlCommand(updateQuery, conn, transaction);
                                            QueryResult = cmd.ExecuteNonQuery();

                                            if (QueryResult > 0)
                                            {
                                                datasaved = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // 2] Data updation tbl_GLKYC_AddressDetails
                        if (datasaved == true)
                        {
                            updateQuery = "update tbl_GLKYC_AddressDetails set BldgHouseName='" + txtBldgHouseName.Text.Trim() + "', " +
                                            "Road=@Road, BldgPlotNo='" + txtPlotNo.Text.Trim() + "', " +
                                            "RoomBlockNo='" + txtRoomBlockNo.Text.Trim() + "', StateID='" + ddlState.SelectedValue + "', " +
                                            "CityID='" + ddlCity.SelectedValue + "', AreaID='" + ddlArea.SelectedValue + "', " +
                                            "ZoneID='" + ddlZone.SelectedValue + "', Landmark=@Landmark " +
                                      "where GoldLoanNo='" + GoldLoanNo + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            cmd.Parameters.AddWithValue("@Road", txtRoad.Text.Trim());
                            cmd.Parameters.AddWithValue("@Landmark", txtNearestLandmark.Text.Trim());
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }


                        // 3] Data updation tbl_GLKYC_ApplicantDetails
                        if (datasaved == true)
                        {
                            updateQuery = "update tbl_GLKYC_ApplicantDetails set AppFName='" + txtAppFName.Text.Trim() + "', " +
                                                "AppMName='" + txtAppMName.Text.Trim() + "', AppLName='" + txtAppLName.Text.Trim() + "', " +
                                                "Gender='" + ddlGender.SelectedItem.Text.Trim() + "', BirthDate='" + Convert.ToDateTime(txtBirthDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "Age='" + txtAge.Text.Trim() + "', PANNo='" + txtPANNo.Text.Trim().ToUpper() + "', " +
                                                "MaritalStatus='" + ddlMaritalStatus.SelectedItem.Text.Trim() + "', " +
                                                "MobileNo='" + txtMobileNo.Text.Trim() + "', TelephoneNo='" + txtTelephoneNo.Text.Trim() + "', " +
                                                "EmailID='" + txtEmailId.Text.Trim() + "', NomFName='" + txtNomFName.Text.Trim() + "', " +
                                                "NomMName='" + txtNomMName.Text.Trim() + "', NomLName='" + txtNomLName.Text.Trim() + "', " +
                                                "NomAddress=@NomAddress, " +
                                                "NomRelation='" + ddlNomRelation.SelectedValue + "', AppPhotoPath='" + txtAppPhotoPath.Text.Trim() + "', " +
                                                "AppSignPath='" + txtAppSignPath.Text.Trim() + "' " +
                                            "where GoldLoanNo='" + GoldLoanNo + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            cmd.Parameters.AddWithValue("@NomAddress", txtNomineeAddress.Text.ToString());
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }

                        // 4] Data updation tbl_GLKYC_BasicDetails
                        if (datasaved == true)
                        {
                            updateQuery = "update tbl_GLKYC_BasicDetails set LoanDate='" + Convert.ToDateTime(txtLoanDate.Text).ToString("yyyy/MM/dd") + "', " +
                                                "OperatorID='" + txtOperatorID.Text.Trim() + "', ExistingGLNo='" + txtExistingGLNo.Text.Trim() + "', " +
                                                "LoanType='" + ddlLoanType.SelectedItem.Text.Trim() + "', ExistingPLCaseNo='" + txtExistingPLCaseNo.Text + "', " +
                                                "BranchID='" + txtBranchID.Text + "', FYID='" + txtFYID.Text + "', " +
                                                "SourceofApplication='" + ddlSourceofApplication.SelectedItem.Text.Trim() + "', " +
                                                "SourceSpecification='" + txtSpecifySource.Text + "', DealerID='" + ddlDealerName.SelectedValue + "' " +
                                          "where GoldLoanNo='" + GoldLoanNo + "'";

                            cmd = new SqlCommand(updateQuery, conn, transaction);
                            QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                datasaved = true;
                            }
                        }

                        if (QueryResult > 0)
                        {
                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Updated Successfully.');", true);
                            BindDGVDetails();
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

    #region [Upload Photo]
    protected void btnUploadPhoto_Click(object sender, EventArgs e)
    {
        try
        {
            int MaxSizeAllowed = 1073741824; // 1GB...

            if (fUploadPhoto.HasFile)
            {
                string fileName = fUploadPhoto.FileName;
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
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadPhoto.PostedFile.ContentLength > MaxSizeAllowed)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }
                else
                {
                    txtAppPhotoPath.Text = "PhotoImage/" + fileName;
                    //upload the file onto the server                   
                    fUploadPhoto.SaveAs(Server.MapPath("~/" + txtAppPhotoPath.Text));

                    System.IO.Stream fs = fUploadPhoto.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    imgAppPhoto.ImageUrl = "data:image/png;base64," + base64String;
                    imgAppPhoto.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UPhotoAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Photo]

    #region [Upload Sign]
    protected void btnUploadSign_Click(object sender, EventArgs e)
    {
        try
        {
            int MaxSizeAllowed = 1073741824; // 1GB...

            if (fUploadSign.HasFile)
            {
                string fileName = fUploadSign.FileName;
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
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadSignAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadSign.PostedFile.ContentLength > MaxSizeAllowed)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadSignAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }
                else
                {
                    txtAppSignPath.Text = "SignImage/" + fileName;
                    //upload the file onto the server                   
                    fUploadSign.SaveAs(Server.MapPath("~/" + txtAppSignPath.Text));

                    System.IO.Stream fs = fUploadSign.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    imgAppSign.ImageUrl = "data:image/png;base64," + base64String;
                    imgAppSign.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "USignAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Sign]

    #region [Upload Document Details]
    protected void btnUpload_OnClick(object sender, EventArgs e)
    {
        try
        {
            if (Page.IsValid)
            {
                FileUpload fUpload = (FileUpload)dgvDocumentDetails.FooterRow.FindControl("FileUpload1");
                DropDownList ddlDocName = (DropDownList)dgvDocumentDetails.FooterRow.FindControl("ddlDocName");
                CheckBox chkReceived = (CheckBox)dgvDocumentDetails.FooterRow.FindControl("chkReceived");
                TextBox txtSpecifyOtherDoc = (TextBox)dgvDocumentDetails.FooterRow.FindControl("txtSpecifyOther");

                string fileName = string.Empty;
                int MaxSizeAllowed = 1073741824; // 1GB...

                if (fUpload.HasFile)
                {
                    fileName = fUpload.FileName;
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
                    else if (fUpload.PostedFile.ContentLength > MaxSizeAllowed)
                    {
                        ClientScript.RegisterStartupScript(this.GetType(), "fUploadAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                    }
                    else
                    {
                        //int rowIndex = 0;
                        DataTable dtCurrentTable = new DataTable();
                        DataRow dr = null;
                        dtCurrentTable.Columns.Add("DID", typeof(string));
                        dtCurrentTable.Columns.Add("DocumentID", typeof(string));
                        dtCurrentTable.Columns.Add("DocName", typeof(string));
                        dtCurrentTable.Columns.Add("DocRecd", typeof(string));
                        dtCurrentTable.Columns.Add("ImagePath", typeof(string));
                        dtCurrentTable.Columns.Add("ImageUrl", typeof(string));
                        dtCurrentTable.Columns.Add("OtherDoc", typeof(string));
                        //DataRow drCurrentRow = null;

                        foreach (GridViewRow row in dgvDocumentDetails.Rows)
                        {
                            Label lblDID = (Label)row.Cells[1].FindControl("lblDID");
                            Label lblDocumentID = (Label)row.Cells[1].FindControl("lblDocumentID");
                            Label lblDocName = (Label)row.Cells[1].FindControl("lblDocName");
                            Label lblReceived = (Label)row.Cells[1].FindControl("lblReceived");
                            Label lblImagePath = (Label)row.Cells[1].FindControl("lblImagePath");
                            Label lblSpecifyOther = (Label)row.Cells[1].FindControl("lblSpecifyOther");

                            dr = dtCurrentTable.NewRow();
                            dr["DID"] = lblDID.Text;
                            dr["DocumentID"] = lblDocumentID.Text;
                            dr["DocName"] = lblDocName.Text;
                            dr["DocRecd"] = lblReceived.Text;
                            dr["ImagePath"] = lblImagePath.Text;
                            dr["ImageUrl"] = lblImagePath.Text;
                            dr["OtherDoc"] = lblSpecifyOther.Text;

                            if (lblDocName.Text != "")
                            {
                                dtCurrentTable.Rows.Add(dr);
                            }
                        }

                        fUpload.SaveAs(Server.MapPath("~/DocumentImage/" + fileName));
                        string ImagePath = "DocumentImage/" + fileName;


                        string DocRecd = string.Empty;
                        if (chkReceived.Checked == true)
                        {
                            DocRecd = "Yes";
                        }
                        else
                        {
                            DocRecd = "No";
                        }

                        dr = dtCurrentTable.NewRow();
                        dr["DID"] = string.Empty;
                        dr["DocumentID"] = Convert.ToString(ddlDocName.SelectedValue);
                        dr["DocName"] = Convert.ToString(ddlDocName.SelectedItem.Text);
                        dr["DocRecd"] = DocRecd;
                        dr["ImagePath"] = ImagePath;
                        dr["ImageUrl"] = ImagePath;
                        dr["OtherDoc"] = txtSpecifyOtherDoc.Text;

                        dtCurrentTable.Rows.Add(dr);

                        Session["dtDocumentDetails"] = dtCurrentTable;
                        dgvDocumentDetails.DataSource = dtCurrentTable;
                        dgvDocumentDetails.DataBind();
                    }
                }
                ddlDocName.Focus();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UDocAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Upload Document Details]

    #region [Fill Existing Details of GLNo]
    protected void FillExistingGLNoDetails(string GoldLoanNo, string textboxID)
    {
        try
        {
            //checking whether Gold Loan No exists
            int existCount = 0;
            strQuery = "select count(GoldLoanNo) from tbl_GLKYC_BasicDetails " +
                            "where GoldLoanNo='" + GoldLoanNo + "'";
            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand(strQuery, conn);
            existCount = Convert.ToInt32(cmd.ExecuteScalar());

            if (existCount > 0)
            {
                if (txtFYID.Text.Trim() != "" && txtBranchID.Text.Trim() != "")
                {
                    if (textboxID == "txtGoldLoanNo")
                    {
                        //checking whether Gold Loan No  belongs to same branch and financial year.
                        strQuery = "select count(GoldLoanNo) from tbl_GLKYC_BasicDetails " +
                                        "where GoldLoanNo='" + GoldLoanNo + "' and FYID='" + txtFYID.Text + "' and BranchID='" + txtBranchID.Text + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        existCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        //checking whether Gold Loan No  belongs to same branch and financial year.
                        strQuery = "select count(GoldLoanNo) from tbl_GLKYC_BasicDetails " +
                                        "where GoldLoanNo='" + GoldLoanNo + "' and BranchID='" + txtBranchID.Text + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        existCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (existCount > 0)
                    {
                        //Applicant details
                        strQuery = "select tbl_GLKYC_BasicDetails.OperatorID, tbl_GLKYC_BasicDetails.ExistingGLNo, " +
                                            "tbl_GLKYC_ApplicantDetails.AppFName, tbl_GLKYC_ApplicantDetails.AppMName, " +
                                            "tbl_GLKYC_ApplicantDetails.AppLName, tbl_GLKYC_ApplicantDetails.Gender, " +
                                            "tbl_GLKYC_ApplicantDetails.BirthDate, tbl_GLKYC_ApplicantDetails.Age, " +
                                            "tbl_GLKYC_ApplicantDetails.PANNo, tbl_GLKYC_ApplicantDetails.MaritalStatus, " +
                                            "tbl_GLKYC_ApplicantDetails.MobileNo, tbl_GLKYC_ApplicantDetails.TelephoneNo, " +
                                            "tbl_GLKYC_ApplicantDetails.EmailID, tbl_GLKYC_ApplicantDetails.NomFName, " +
                                            "tbl_GLKYC_ApplicantDetails.NomMName, tbl_GLKYC_ApplicantDetails.NomLName, " +
                                            "tbl_GLKYC_ApplicantDetails.NomRelation, tbl_GLKYC_ApplicantDetails.AppPhotoPath, " +
                                            "tbl_GLKYC_ApplicantDetails.AppSignPath, Operator=UserDetails.UserName, " +
                                            "tbl_GLKYC_BasicDetails.LoanDate, tbl_GLKYC_BasicDetails.LoanType, " +
                                            "tbl_GLKYC_ApplicantDetails.NomAddress, tbl_GLKYC_BasicDetails.ExistingPLCaseNo " +
                                    "from tbl_GLKYC_BasicDetails " +
                                    "inner join tbl_GLKYC_ApplicantDetails " +
                                            "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "inner join UserDetails " +
                                            "on tbl_GLKYC_BasicDetails.OperatorID=UserDetails.UserID " +
                                    "where tbl_GLKYC_BasicDetails.GoldLoanNo='" + GoldLoanNo + "'";
                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (textboxID == "txtGoldLoanNo")
                        {
                            txtExistingGLNo.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                            txtLoanDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][20]).ToString("dd/MM/yyyy");
                            ddlLoanType.Text = Convert.ToString(ds.Tables[0].Rows[0][21]);
                        }
                        else
                        {
                            ddlLoanType.Text = "Refinance";
                        }
                        txtAppFName.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtAppMName.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        txtAppLName.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
                        ddlGender.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
                        txtBirthDate.Text = Convert.ToDateTime(ds.Tables[0].Rows[0][6]).ToString("dd/MM/yyyy").Trim();
                        txtAge.Text = Convert.ToString(ds.Tables[0].Rows[0][7]);
                        txtPANNo.Text = Convert.ToString(ds.Tables[0].Rows[0][8]).ToUpper();
                        ddlMaritalStatus.Text = Convert.ToString(ds.Tables[0].Rows[0][9]);
                        txtMobileNo.Text = Convert.ToString(ds.Tables[0].Rows[0][10]);
                        txtTelephoneNo.Text = Convert.ToString(ds.Tables[0].Rows[0][11]);
                        txtEmailId.Text = Convert.ToString(ds.Tables[0].Rows[0][12]);
                        txtNomFName.Text = Convert.ToString(ds.Tables[0].Rows[0][13]);
                        txtNomMName.Text = Convert.ToString(ds.Tables[0].Rows[0][14]);
                        txtNomLName.Text = Convert.ToString(ds.Tables[0].Rows[0][15]);
                        ddlNomRelation.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][16]);
                        txtAppPhotoPath.Text = Convert.ToString(ds.Tables[0].Rows[0][17]);
                        txtAppSignPath.Text = Convert.ToString(ds.Tables[0].Rows[0][18]);
                        imgAppPhoto.ImageUrl = txtAppPhotoPath.Text;
                        imgAppSign.ImageUrl = txtAppSignPath.Text;
                        txtNomineeAddress.Text = Convert.ToString(ds.Tables[0].Rows[0][22]);
                        txtExistingPLCaseNo.Text = Convert.ToString(ds.Tables[0].Rows[0][23]);

                        //Address Details
                        strQuery = "select tbl_GLKYC_AddressDetails.AddrID, tbl_GLKYC_AddressDetails.BldgHouseName, " +
                                            "tbl_GLKYC_AddressDetails.Road, tbl_GLKYC_AddressDetails.BldgPlotNo, " +
                                            "tbl_GLKYC_AddressDetails.RoomBlockNo, tbl_GLKYC_AddressDetails.StateID, " +
                                            "tbl_GLKYC_AddressDetails.CityID, tbl_GLKYC_AddressDetails.AreaID, " +
                                            "tbl_GLKYC_AddressDetails.ZoneID, tbl_GLKYC_AddressDetails.Landmark " +
                                    "from tbl_GLKYC_BasicDetails " +
                                    "inner join tbl_GLKYC_AddressDetails " +
                                            "on tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                    "where tbl_GLKYC_BasicDetails.GoldLoanNo='" + GoldLoanNo + "'";
                        conn = new SqlConnection(strConnString);
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        txtBldgHouseName.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
                        txtRoad.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
                        txtPlotNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]);
                        txtRoomBlockNo.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
                        ddlState.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][5]);
                        FillCity();
                        ddlCity.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][6]);
                        FillArea();
                        ddlArea.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][7]);
                        FillZone();
                        ddlZone.SelectedValue = Convert.ToString(ds.Tables[0].Rows[0][8]);
                        txtNearestLandmark.Text = Convert.ToString(ds.Tables[0].Rows[0][9]);

                        //Document Details
                        strQuery = "select tbl_GLKYC_DocumentDetails.DID, tbl_GLKYC_DocumentDetails.DocumentID, " +
                                            "tbl_GLDocumentMaster.DocumentName as 'DocName', tbl_GLKYC_DocumentDetails.DocRecd, " +
                                            "tbl_GLKYC_DocumentDetails.ImagePath, ImageUrl=tbl_GLKYC_DocumentDetails.ImagePath, " +
                                            "tbl_GLKYC_DocumentDetails.OtherDoc " +
                                    "from tbl_GLKYC_DocumentDetails " +
                                    "inner join tbl_GLDocumentMaster " +
                                            "on tbl_GLKYC_DocumentDetails.DocumentID=tbl_GLDocumentMaster.DocumentID " +
                                    "where tbl_GLKYC_DocumentDetails.GoldLoanNo='" + GoldLoanNo + "'";
                        conn = new SqlConnection(strConnString);
                        da = new SqlDataAdapter(strQuery, conn);
                        dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Session["dtDocumentDetails"] = dt;
                            dgvDocumentDetails.DataSource = dt;
                            dgvDocumentDetails.DataBind();
                        }
                        else
                        {
                            dt = new DataTable();
                            dt.Columns.Add("DID", typeof(string));
                            dt.Columns.Add("DocumentID", typeof(string));
                            dt.Columns.Add("DocName", typeof(string));
                            dt.Columns.Add("DocRecd", typeof(string));
                            dt.Columns.Add("ImagePath", typeof(string));
                            dt.Columns.Add("ImageUrl", typeof(string));
                            dt.Columns.Add("OtherDoc", typeof(string));

                            ShowNoResultFound(dt, dgvDocumentDetails);
                        }
                    }
                    else
                    {
                        int FYearID = 0;
                        int BranchID = 0;
                        int Bcount = 0;
                        int Fcount = 0;

                        //fetching Financial year ID and Branch ID.
                        strQuery = "select FYID, BranchID from tbl_GLKYC_BasicDetails " +
                                        "where GoldLoanNo='" + GoldLoanNo + "'";
                        cmd = new SqlCommand(strQuery, conn);
                        da = new SqlDataAdapter(cmd);
                        ds = new DataSet();
                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            FYearID = Convert.ToInt32(ds.Tables[0].Rows[0][0]);
                            BranchID = Convert.ToInt32(ds.Tables[0].Rows[0][1]);
                        }

                        //fetching financial year.
                        string FYear = string.Empty;
                        strQuery = "select Financialyear from tblFinancialyear " +
                                        "where FinancialyearID='" + FYearID + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        FYear = Convert.ToString(cmd.ExecuteScalar());

                        //fetching branch name.
                        string branchName = string.Empty;
                        strQuery = "select BranchName from tblCompanyBranchMaster " +
                                        "where BID='" + BranchID + "'";
                        conn = new SqlConnection(strConnString);
                        conn.Open();
                        cmd = new SqlCommand(strQuery, conn);
                        branchName = Convert.ToString(cmd.ExecuteScalar());

                        if (Convert.ToInt32(txtBranchID.Text) != BranchID)
                        {
                            Bcount = 1;
                        }

                        if (Convert.ToInt32(txtFYID.Text) != FYearID)
                        {
                            Fcount = 1;
                        }

                        if (textboxID == "txtGoldLoanNo")
                        {
                            
                            if (Bcount == 1 && Fcount == 0)
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit Gold Loan No. " + GoldLoanNo + " please Log into Branch: " + branchName + ".');", true);
                                ClearData();
                            }
                            else if (Bcount == 0 && Fcount == 1)
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit Gold Loan No. " + GoldLoanNo + " please Log into Financial Year: " + FYear + ".');", true);
                                ClearData();
                            }
                            else if (Bcount == 1 && Fcount == 1)
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit Gold Loan No. " + GoldLoanNo + " please Log into Branch: " + branchName + " and Financial Year: " + FYear + ".');", true);
                                ClearData();
                            }
                        }
                        else
                        {
                            if (Bcount == 1)
                            {
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('To edit Gold Loan No. " + GoldLoanNo + " please Log into Branch: " + branchName + ".');", true);
                                ClearData();
                            }
                        }
                    }
                }
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Gold Loan No. " + GoldLoanNo + " does not exist.');", true);
                ClearData();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ExistGLNOAlert", "alert('" + ex.Message + "');", true);
        }

        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Fill Existing Details of GLNo]

    #region [Fill State]
    protected void FillState()
    {
        try
        {
            ddlState.DataSource = null;
            conn = new SqlConnection(strConnString);
            string query = "Select StateID, StateName=StateName + ' (' +CountryName+ ')' from tblStateMaster " +
                            "inner join tbl_CountryMaster " +
                            "on tblStateMaster.CountryID=tbl_CountryMaster.CountryID ";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlState.DataSource = dt;
            ddlState.DataValueField = "StateID";
            ddlState.DataTextField = "StateName";
            ddlState.DataBind();
            ddlState.Items.Insert(0, new ListItem("--Select State--", "0"));
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
    #endregion [Fill State]

    #region [Fill City]
    protected void FillCity()
    {
        try
        {
            ddlCity.DataSource = null;
            conn = new SqlConnection(strConnString);
            conn.Open();
            if (ddlState.SelectedValue != "--Select State--")
            {
                string query = "select CityID, CityName=CityName + ' (' +StateName+ ')' from tblCityMaster " +
                            "inner join tblStateMaster " +
                                    "on tblCityMaster.StateID=tblStateMaster.StateID " +
                            "where tblCityMaster.StateID='" + ddlState.SelectedValue + "'";
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                ddlCity.DataSource = dt;
                ddlCity.DataTextField = "CityName";
                ddlCity.DataValueField = "CityID";
                ddlCity.DataBind();
                ddlCity.Items.Insert(0, new ListItem("--Select City--", "0"));
            }
            else
            {
                ddlCity.DataSource = null;
                ddlCity.DataBind();
            }
            
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "FillCityAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Fill City]

    #region [FillArea]
    protected void FillArea()
    {
        try
        {
            ddlArea.DataSource = null;
            conn = new SqlConnection(strConnString);
            string query = "Select AreaID, Area=Area + ' (' +CityName+ ')' from tblAreaMaster " +
                            "inner join tblCityMaster " +
                            "on tblAreaMaster.CityID=tblCityMaster.CityID  " +
                            "where tblAreaMaster.CityID='" + ddlCity.SelectedValue + "'";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlArea.DataSource = dt;
            ddlArea.DataValueField = "AreaID";
            ddlArea.DataTextField = "Area";
            ddlArea.DataBind();
            ddlArea.Items.Insert(0, new ListItem("--Select Area--", "0"));
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
    #endregion [FillArea]

    #region [FillZone]
    protected void FillZone()
    {
        try
        {
            ddlZone.DataSource = null;
            conn = new SqlConnection(strConnString);
            string query = "Select ZoneID, Zone=Zone + ' (' +Area+ ')' from tblZonemaster " +
                            "inner join tblAreaMaster " +
                            "on tblZonemaster.AreaID=tblAreaMaster.AreaID " +
                            "where tblZonemaster.AreaID='" + ddlArea.SelectedValue + "'";
            SqlDataAdapter da = new SqlDataAdapter(query, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlZone.DataSource = dt;
            ddlZone.DataValueField = "ZoneID";
            ddlZone.DataTextField = "Zone";
            ddlZone.DataBind();
            ddlZone.Items.Insert(0, new ListItem("--Select Zone--", "0"));
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
    #endregion [FillZone]

    #region [Calculate Age]
    protected int CalculateAge()
    {
        int Age = 0;
        try
        {
            string strQuery = string.Empty;
            DateTime todayDate = System.DateTime.Today;

            conn = new SqlConnection(strConnString);
            conn.Open();

            //getting Today's Date
            strQuery = "select getdate() ";
            cmd = new SqlCommand(strQuery, conn);

            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                todayDate = Convert.ToDateTime(cmd.ExecuteScalar());

            }
            DateTime birthDate = Convert.ToDateTime(txtBirthDate.Text);
            Age = new DateTime(todayDate.Subtract(birthDate).Ticks).Year - 1;
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
        return Age;
    }
    #endregion [Calculate Age]

    #region [txtGoldLoanNo_TextChanged]
    protected void txtGoldLoanNo_TextChanged(object sender, EventArgs e)
    {
        try
        {
            TextBox txtGL = sender as TextBox;
            string GL = txtGL.ID;
            FillExistingGLNoDetails(txtGoldLoanNo.Text, GL);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLNOEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [txtGoldLoanNo_TextChanged]

    #region [ExistingGLNo_TextChanged]
    protected void txtExistingGLNo_TextChanged(object sender, EventArgs e)
    {
        try
        {
            FillExistingGLNoDetails(txtExistingGLNo.Text, "");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ExistGLNOEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ExistingGLNo_TextChanged]

    #region [BirthDate_TextChanged]
    protected void txtBirthDate_TextChanged(object sender, EventArgs e)
    {
        try
        {
            txtAge.Text = Convert.ToString(CalculateAge());
            ddlMaritalStatus.Focus();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BirthDtEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [BirthDate_TextChanged]

    #region [ddlState_SelectedIndexChanged]
    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillCity();
            FillArea();
            FillZone();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "StateEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ddlState_SelectedIndexChanged]

    #region [ddlCity_SelectedIndexChanged]
    protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillArea();
            FillZone();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "StateEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ddlCity_SelectedIndexChanged]

    #region [ddlArea_SelectedIndexChanged]
    protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillZone();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "StateEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ddlArea_SelectedIndexChanged]

    #region [dgvDocumentDetails_ServerValidate]
    protected void dgvDocumentDetails_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            if (dgvDocumentDetails != null && dgvDocumentDetails.Rows.Count > 0)
            {
                int Count = 0;
                foreach (GridViewRow row in dgvDocumentDetails.Rows)
                {
                    Label lblDoc = (Label)row.FindControl("lblDocName");

                    if (lblDoc != null && lblDoc.Text != "")
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
            ClientScript.RegisterStartupScript(this.GetType(), "DocValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        { }
    }
    #endregion [dgvDocumentDetails_ServerValidate]

    #region [txtLoanDate_ServerValidate]
    protected void txtLoanDate_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            DateTime dtStartDate = System.DateTime.Today;
            DateTime dtEndDate = System.DateTime.Today;

            strQuery = "select FinancialyearID, StartDate=convert(varchar,StartDate,103), EndDate=convert(varchar,EndDate,103) " +
                       "from tblFinancialyear where FinancialyearID='" + txtFYID.Text + "' and CompID=1";
            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                dtStartDate = Convert.ToDateTime(ds.Tables[0].Rows[0][1]);
                dtEndDate = Convert.ToDateTime(ds.Tables[0].Rows[0][2]);
            }

            if (Convert.ToString(txtLoanDate.Text.Trim()) != "")
            {
                if (Convert.ToDateTime(txtLoanDate.Text) < dtStartDate || Convert.ToDateTime(txtLoanDate.Text) > dtEndDate)
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
            ClientScript.RegisterStartupScript(this.GetType(), "LoanDateValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [txtLoanDate_ServerValidate]

    #region [txtAge_ServerValidate]
    protected void txtAge_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            // to validate applicant's age - 18 yrs & above.
            if (e.Value.Trim() != "")
            {
                if (Convert.ToInt32(e.Value) < 18)
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
            ClientScript.RegisterStartupScript(this.GetType(), "AgeValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {}
    }
    #endregion [txtAge_ServerValidate]

    #region [txtExistingPLCaseNo_ServerValidate]
    protected void txtExistingPLCaseNo_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            // to validate PL Case No.
            if (e.Value.Trim() != "")
            {
                strQuery = "select count(CaseNo) " +
                           "from TDisbursement_Appl_BasicInfo where CaseNo='" + txtExistingPLCaseNo.Text + "'";
                conn = new SqlConnection(strConnString);
                conn.Open();
                cmd = new SqlCommand(strQuery, conn);
                int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                if (existcount == 0)
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PLCaseNoValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [txtExistingPLCaseNo_ServerValidate]

    #region [txtSpecifyOther_ServerValidate]
    protected void txtSpecifyOther_ServerValidate(object sender, ServerValidateEventArgs e)
    {
        try
        {
            // to validate Document Name if Other.
            DropDownList ddlDocName = (DropDownList)dgvDocumentDetails.FooterRow.FindControl("ddlDocName");
            string docName = ddlDocName.SelectedItem.Text;

            if (docName == "Other")
            {
                if (e.Value.Trim() == "")
                {
                    e.IsValid = false;
                }
                else
                {
                    e.IsValid = true;
                }
            }
            else
            {
                e.IsValid = true;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PLCaseNoValidAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {}
    }
    #endregion [txtSpecifyOther_ServerValidate]

    #region [ddlDocName_SelectedIndexChanged]
    protected void ddlDocName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            DropDownList ddlDocName = (DropDownList)dgvDocumentDetails.FooterRow.FindControl("ddlDocName");
            TextBox txtSpecifyOther = (TextBox)dgvDocumentDetails.FooterRow.FindControl("txtSpecifyOther");
            string docName = ddlDocName.SelectedItem.Text;

            if (docName == "Other")
            {
                txtSpecifyOther.Enabled = true;
                txtSpecifyOther.Focus();
            }
            else
            {
                txtSpecifyOther.Enabled = false;
                ddlDocName.Focus();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DocNameEvent_Alert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ddlDocName_SelectedIndexChanged]

    #region[FillDealerName]
    protected void FillDealerName()
    {
        try
        {
            ddlDealerName.DataSource = null;
            ddlDealerName.Items.Clear();
                        conn = new SqlConnection(strConnString);
            strQuery = "SELECT DISTINCT DealerID, DealerName=DealerName +' (dealer code - '+convert(varchar,DealerCode)+')' FROM tblDealerMaster WHERE DealerCode IS NOT NULL AND DealerCode<>'' ORDER BY DealerName";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlDealerName.DataSource = dt;
            ddlDealerName.DataValueField = "DealerID";
            ddlDealerName.DataTextField = "DealerName";
            ddlDealerName.DataBind();
            ddlDealerName.Items.Insert(0, new ListItem("--Select Dealer Name--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillDealerNameAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion[FillDealerName]

    #region [ddlSourceofApplication_SelectedIndexChanged]
    protected void ddlSourceofApplication_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            FillDealerName();
            if (ddlSourceofApplication.SelectedValue == "Dealer")
            {
                ddlDealerName.Enabled = true;
                txtSpecifySource.Text = "";
                txtSpecifySource.Enabled = false;
            }
            else
            {
                ddlDealerName.Enabled = false;
                txtSpecifySource.Enabled = true;
                ddlDealerName.SelectedValue = "0";
            }

        }
        catch(Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SourceofApplnEventAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[ddlSourceofApplication_SelectedIndexChanged]

    //#region[ddlDealerName_ServerValidate]
    //protected void ddlDealerName_ServerValidate(object source, ServerValidateEventArgs e)
    //{
    //    try
    //    {
    //        if (e.Value.Trim() == "0")
    //        {
    //            if (ddlSourceofApplication.SelectedValue == "Dealer")
    //            {
    //                e.IsValid = false;
    //            }
    //            else
    //            {
    //                e.IsValid = true;
    //            }
    //        }
    //        else
    //        {
    //            e.IsValid = true;
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "DealerAlert", "alert('" + ex.Message + "');", true);
    //    }
    //    finally
    //    { }
    //}
    //#endregion[ddlDealerName_ServerValidate]
    
}