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

public partial class FinancialYearMaster : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string deleteQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    DataTable dt;

    public string loginDate;
    public string expressDate;
    #endregion

    #region Page_Load
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
    #endregion

    #region Bind GridView
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "select FinancialyearID, StartDate=convert(varchar,StartDate,103), EndDate=convert(varchar,EndDate,103), " + 
                        "Financialyear, CompID from tblFinancialyear where CompID=1";
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
            ddlSearchBy.Items.Add("Financialyear");
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
                int nextYrCount = 0;
                int baseYrCount = 0;
                int inUseCount = 0;
                conn = new SqlConnection(strConnString);
                conn.Open();
                dsDGV = new DataSet();
                dsDGV = GetRecords(conn, "GetAllRecords", 0);
                int ID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());

                if(_commandName == "DeleteRecord")
                {
                    //checking whether next year is present
                    strQuery = "select count(*) from tblFinancialyear where FinancialyearID=" + (ID+1) + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        nextYrCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        nextYrCount = 0;
                    }

                    //checking whether it is base year
                    strQuery = "select count(FinancialyearID) from tblFinancialyear";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        baseYrCount = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        baseYrCount = 0;
                    }

                    //checking whether it is used in other forms
                    strQuery = "select count(*) from tbl_GLKYC_BasicDetails where FYID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    strQuery = "select count(*) from tbl_GLSanctionDisburse_BasicDetails where FYID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    strQuery = "select count(*) from tbl_GLBankGold_BasicDetails where FYID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    strQuery = "select count(*) from tbl_GLEMI_InterestJVDetails where FYID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //also checking in AIM tables 
                    strQuery = "select count(*) from TPLAppForm_BasicDetails where FinancialyearID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        inUseCount += Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //deleting record from DB
                    if (nextYrCount == 0 && baseYrCount > 1 && inUseCount == 0)
                    {
                        int existcount = 0;
                        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                        //checking whether record is present
                        strQuery = "select count(*) from tblFinancialyear where FinancialyearID=" + ID + "";
                        cmd = new SqlCommand(strQuery, conn, transaction);
                        existcount = Convert.ToInt32(cmd.ExecuteScalar());

                        if (existcount > 0)
                        {
                            //deleting record
                            deleteQuery = "delete from tblFinancialyear where FinancialyearID=" + ID + "";
                            cmd = new SqlCommand(deleteQuery, conn, transaction);
                            int QueryResult = cmd.ExecuteNonQuery();

                            if (QueryResult > 0)
                            {
                                transaction.Commit();
                                ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                                BindDGVDetails();
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

                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }

                    if (inUseCount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Deleted since it is in Use.');", true);
                    }
                    else if (nextYrCount > 0)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record cannot be Deleted since Next Year is present.');", true);
                    }
                    else if (baseYrCount == 1)
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Base Year cannot be Deleted.');", true);
                    }
                }
                //else if (_commandName == "UpdateRecord")
                //{
                //    //fill records in the form
                //    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                //    txtFYear.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][3]).Trim();
                //    txtStartDate.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]).Trim();
                //    txtEndDate.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][2]).Trim();
                //    btnSave.Text = "Update";
                //    btnReset.Text = "Cancel";
                //    btnSave.Enabled = false;
                //}
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
                strQuery = "select FinancialyearID, StartDate=convert(varchar,StartDate,103), EndDate=convert(varchar,EndDate,103), " +
                           "Financialyear, CompID from tblFinancialyear where CompID=1";
            }
            else if (CommandName == "NextYear")
            {
                strQuery = "select FinancialyearID, StartDate, EndDate, " +
                           "Financialyear, CompID from tblFinancialyear " + 
                           "where FinancialyearID=(select max(FinancialyearID) from tblFinancialyear where CompID=1)";
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
            txtFYear.Text = "";
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            txtSearch.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
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

    #region Next Year Button Click
    protected void btnNextYear_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            dsDGV = new DataSet();
            dsDGV = GetRecords(conn, "NextYear", 0);
            DateTime dtStartDate = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][1]);
            DateTime dtEndDate = Convert.ToDateTime(dsDGV.Tables[0].Rows[0][2]);
            int SYear = dtEndDate.Year;
            int EYear = SYear + 1;

            //fill records in the form
            txtFYear.Text = "April" + Convert.ToString(SYear) + "-" + "March" + Convert.ToString(EYear);
            txtStartDate.Text = "01/04/" + Convert.ToString(SYear);
            txtEndDate.Text = "31/03/" + Convert.ToString(EYear);

            btnSave.Text = "Save";
            btnReset.Text = "Reset";
            //btnSave.Enabled = true;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "NxtYearAlert", "alert('" + ex.Message + "');", true);
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

    #region Save
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
            int FYID = 0;
            int branchId = Convert.ToInt32(Session["branchId"]);
            int CompID = 0;
            
            DateTime SDate = Convert.ToDateTime(txtStartDate.Text);
            DateTime EDate = Convert.ToDateTime(txtEndDate.Text);

            if (btnSave.Text == "Save")
            {
                int existcount = 0;
                //checking whether record is present
                strQuery = "select count(*) from tblFinancialyear where Financialyear='" + txtFYear.Text + "'";
                cmd = new SqlCommand(strQuery, conn, transaction);
                existcount = Convert.ToInt32(cmd.ExecuteScalar());

                if (existcount > 0)
                {
                    BindDGVDetails();
                    ClearData();
                }

                if (existcount == 0)
                {
                    //getting MAX FYID
                    strQuery = "select max(FinancialyearID) from tblFinancialyear";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        FYID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        FYID = 0;
                    }

                    FYID += 1;

                    //getting CompID
                    strQuery = "select CompID from tblCompanyBranchMaster where BID=" + branchId + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        CompID = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    //inserting data into table tblFinancialyear
                    insertQuery = "insert into tblFinancialyear values('" + FYID + "', '" + SDate.ToString("yyyy/MM/dd") + "', '" + EDate.ToString("yyyy/MM/dd") + "', " +
                                    "'" + SDate.ToString("yyyy/MM/dd") + "', '" + EDate.ToString("yyyy/MM/dd") + "',  " +
                                    "'" + txtFYear.Text.Trim() + "', '" + CompID + "')";

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