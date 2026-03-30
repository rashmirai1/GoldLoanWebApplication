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

public partial class BranchMaster : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;

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
                BindDGVDetails();

                //binding DropDownList Search By
                BindDDLSearchBy();

                //getting Comp ID
                if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
                {
                    int branchId = Convert.ToInt32(Session["branchId"]);

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

    #region Bind GridView
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "select tblCompanyBranchMaster.BID, tblCompanyBranchMaster.BranchName, tblCompanyBranchMaster.CompID " +
                       "from tblCompanyBranchMaster " +
                       "where tblCompanyBranchMaster.CompID='" + txtCompID.Text + "' " +
                       "order by tblCompanyBranchMaster.BranchName";

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
    #endregion

    #region Bind DropDownList-SearchBy
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("BranchName");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindSearchByAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

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
                int ID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());

                #region [DeleteRecord]
                if (_commandName == "DeleteRecord")
                {
                    int baseBranchCount = 0;
                    int inUseCount = 0;
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    //checking whether Branch Name is used in other forms
                    strQuery = "select count(BranchID) from tbl_GLKYC_BasicDetails where BranchID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        inUseCount = 0;
                    }

                    //also checking in AIM tables 
                    strQuery = "select count(BranchID) from tbl_UserLogin where BranchID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (inUseCount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot delete record since it is in use.');", true);
                    }

                    if (inUseCount == 0)
                    {
                        //checking whether it is base Branch
                        strQuery = "select count(BID) from tblCompanyBranchMaster where CompID='" + txtCompID.Text + "'";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        if (cmd.ExecuteScalar() != DBNull.Value)
                        {
                            baseBranchCount = Convert.ToInt32(cmd.ExecuteScalar());
                        }
                        else
                        {
                            baseBranchCount = 0;
                        }

                        if (baseBranchCount > 1)
                        {
                            //checking whether record is present
                            strQuery = "select count(*) from tblCompanyBranchMaster where BID=" + ID + "";
                            cmd = new SqlCommand(strQuery, conn, transaction);
                            int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                            if (existcount > 0)
                            {
                                //deleting record from DB
                                deleteQuery = "delete from tblCompanyBranchMaster where BID=" + ID + "";
                                cmd = new SqlCommand(deleteQuery, conn, transaction);
                                int QueryResult = cmd.ExecuteNonQuery();

                                if (QueryResult > 0)
                                {
                                    transaction.Commit();
                                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                                    BindDGVDetails();

                                    //if the same record is deleted which is filled in the form.
                                    if (txtBranchID.Text != "" && txtBranchID.Text != null)
                                    {
                                        if (ID == Convert.ToInt32(txtBranchID.Text))
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
                        if (baseBranchCount == 1)
                        {
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Base Branch cannot be Deleted.');", true);
                        }
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                #endregion [DeleteRecord]

                #region [UpdateRecord]
                else if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                    txtBranchID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtBranchName.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]).Trim();
                    
                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";

                    int inUseCount = 0;
                    int baseBranchCount = 0;
                    conn = new SqlConnection(strConnString);
                    conn.Open();

                    //checking whether Branch Name is used in other forms
                    strQuery = "select count(BranchID) from tbl_GLKYC_BasicDetails where BranchID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        inUseCount = 0;
                    }

                    strQuery = "select count(BranchID) from tbl_UserLogin where BranchID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    if (inUseCount > 0)
                    {
                        txtBranchName.Enabled = false;
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('It is in use.');", true);
                    }
                    else
                    {
                        txtBranchName.Enabled = true;
                    }

                    //checking whether it is base Branch
                    strQuery = "select count(BID) from tblCompanyBranchMaster where CompID='" + txtCompID.Text + "'";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        baseBranchCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        baseBranchCount = 0;
                    }

                    if (baseBranchCount == 1)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('It is Base Branch.');", true);
                    }

                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
                #endregion [UpdateRecord]
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

    #region dgvDetails_PageIndexChanging
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
    #endregion

    #region GetRecords
    protected DataSet GetRecords(SqlConnection conn, string CommandName, int ID)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "select tblCompanyBranchMaster.BID, tblCompanyBranchMaster.BranchName, tblCompanyBranchMaster.CompID " +
                            "from tblCompanyBranchMaster " +
                            "where tblCompanyBranchMaster.CompID='" + txtCompID.Text + "' " +
                            "order by tblCompanyBranchMaster.BranchName";
            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "select tblCompanyBranchMaster.BID, tblCompanyBranchMaster.BranchName, tblCompanyBranchMaster.CompID " +
                            "from tblCompanyBranchMaster " +
                            "where tblCompanyBranchMaster.BID=" + ID + "";
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

    #region Reset/Cancel
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
    #endregion

    #region ClearData
    protected void ClearData()
    {
        try
        {
            txtBranchID.Text = "";
            txtBranchName.Text = "";
            txtSearch.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";

            txtBranchName.Enabled = true;

            //getting Comp ID
            if (Convert.ToString(Session["branchId"]) != "" && Convert.ToString(Session["branchId"]) != null)
            {
                int branchId = Convert.ToInt32(Session["branchId"]);

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
            Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SearchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Save
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            int BranchID = 0;

            #region [New Save]
            if (btnSave.Text == "Save")
            {
                //validating data
                int count = ValidateData("Save");

                if (count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Branch Name already Exists.');", true);
                    BindDGVDetails();
                }
                else
                {
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    //getting MAX ID
                    strQuery = "select max(BID) from tblCompanyBranchMaster";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        BranchID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        BranchID = 0;
                    }

                    BranchID += 1;

                    //inserting data into table tblCompanyBranchMaster
                    insertQuery = "insert into tblCompanyBranchMaster values('" + BranchID + "', '" + txtBranchName.Text.Trim().ToUpper() + "', " +
                                    "'" + txtCompID.Text.Trim() + "')";

                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    int QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Saved Successfully.');", true);
                        BindDGVDetails();
                        ClearData();
                    }
                    else
                    {
                        transaction.Rollback();
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Saved Successfully.');", true);
                    }
                }
            }
            #endregion [New Save]

            #region [Edit]
            else if (btnSave.Text == "Update")
            {
                int count = ValidateData("Update");
                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                
                if (count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Entered Branch Name already Exists.');", true);
                }
                else
                {
                    //updating table tblCompanyBranchMaster
                    updateQuery = "update tblCompanyBranchMaster set BranchName='" + txtBranchName.Text.Trim().ToUpper() + "', " +
                                        "CompID='" + txtCompID.Text.Trim() + "' " +
                                    "where BID='" + txtBranchID.Text + "'";

                    cmd = new SqlCommand(updateQuery, conn, transaction);
                    int QueryResult = cmd.ExecuteNonQuery();

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
            #endregion [Edit]
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
    #endregion

    #region Validate Data
    protected int ValidateData(string command)
    {
        int count = 0;
        try
        {
            //to check whether record is in use and do not allow to edit
            //write code here 

            //to check whether branch name is present
            if (command == "Save")
            {
                strQuery = "select count(*) from tblCompanyBranchMaster where BranchName='" + txtBranchName.Text.Trim() + "' " +
                                "and CompID='" + txtCompID.Text + "'";
            }
            else if (command == "Update")
            {
                strQuery = "select count(*) from tblCompanyBranchMaster where BranchName='" + txtBranchName.Text.Trim() + "' " +
                                "and CompID='" + txtCompID.Text + "' " +
                                "and BID !=" + txtBranchID.Text + "";
            }

            cmd = new SqlCommand(strQuery, conn);
            count = Convert.ToInt32(cmd.ExecuteScalar());
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ValidateAlert", "alert('" + ex.Message + "');", true);
        }
        return count;
    }
    #endregion

    #region [Search Function]
    protected void Search()
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
}