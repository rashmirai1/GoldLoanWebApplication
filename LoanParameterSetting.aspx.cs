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

public partial class LoanParameterSetting : System.Web.UI.Page
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

                //binding GridView
                BindDGVDetails();
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
            ClientScript.RegisterStartupScript(this.GetType(), "Page_LoadAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [Page_Load]

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
            txtCostPerGram.Text = "";
            txtReminder.Text = "";
            txtInterest.Text = "";
            txtProcessingFee.Text = "";
            txtIndemnity.Text = "";
            txtDeduction.Text = "";
            txtLPID.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [ClearData]

    #region [Bind GridView]
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "SELECT ID, PendingLoanReminderDays, GoldPricePerGram, InterestPayableForPrepaymentDays, " +
                                "ProcessingFee, ProcessingIndemnity, DeductionInGrossWeight " +
                        "FROM tblLoanParameterSetting ";

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
    #endregion [Bind GridView]

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
    #endregion [dgvDetails_PageIndexChanging]

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
                dsDGV = GetRecords(conn, "GetAllRecords", 0);
                int ID = Convert.ToInt32(_gridView.DataKeys[_selectedIndex].Value.ToString());

                #region [DeleteRecord]
                if (_commandName == "DeleteRecord")
                {
                    //deleting record from DB
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    deleteQuery = "delete from tblLoanParameterSetting where ID=" + ID + "";
                    cmd = new SqlCommand(deleteQuery, conn, transaction);
                    int QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        transaction.Commit();
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                        BindDGVDetails();
                        ClearData();

                        //if the same record is deleted which is filled in the form.
                        if (txtLPID.Text != "" && txtLPID.Text != null)
                        {
                            if (ID == Convert.ToInt32(txtLPID.Text))
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
                #endregion [DeleteRecord]

                #region [UpdateRecord]
                else if (_commandName == "UpdateRecord")
                {
                    strQuery = "SELECT ID, PendingLoanReminderDays, GoldPricePerGram, InterestPayableForPrepaymentDays, " +
                               "ProcessingFee, ProcessingIndemnity, DeductionInGrossWeight " +
                       "FROM tblLoanParameterSetting ";

                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                    txtLPID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]).Trim();
                    txtReminder.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]).Trim();
                    txtCostPerGram.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][2]).Trim();
                    txtInterest.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][3]).Trim();
                    txtProcessingFee.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][4]).Trim();
                    txtIndemnity.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][5]).Trim();
                    txtDeduction.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][6]).Trim();
                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";
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
    #endregion [dgvDetails_RowCommand]

    #region [GetRecords]
    protected DataSet GetRecords(SqlConnection conn, string CommandName, int ID)
    {
        try
        {
            if (CommandName == "GetAllRecords" || CommandName == "UpdateRecord")
            {
                strQuery = "SELECT ID, PendingLoanReminderDays, GoldPricePerGram, InterestPayableForPrepaymentDays, " +
                                    "ProcessingFee, ProcessingIndemnity, DeductionInGrossWeight " +
                            "FROM tblLoanParameterSetting ";

            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "SELECT ID, PendingLoanReminderDays, GoldPricePerGram, InterestPayableForPrepaymentDays, " +
                                    "ProcessingFee, ProcessingIndemnity, DeductionInGrossWeight " +
                            "FROM tblLoanParameterSetting " +
                            "WHERE tblLoanParameterSetting.ID=" + ID + "";
            }
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GetRecordsAlert", "alert('" + ex.Message + "');", true);
        }
        return ds;
    }
    #endregion [GetRecords]

    #region [Save Data]
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int ID = 0;
            conn = new SqlConnection(strConnString);
            conn.Open();
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

            #region [New Save]
            if (btnSave.Text == "Save")
            {
                int count = 0;
                strQuery = "select count(*) from tblLoanParameterSetting";
                cmd = new SqlCommand(strQuery, conn, transaction);
                count = Convert.ToInt32(cmd.ExecuteScalar());

                if (count > 0)
                {
                    ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Cannot Add Record. Edit Existing Record.');", true);
                    ClearData();
                }

                if (count == 0)
                {
                    //getting MAX ID
                    strQuery = "select max(ID) from tblLoanParameterSetting";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        ID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        ID = 0;
                    }

                    ID += 1;

                    decimal ProcessingFee = Decimal.Round(Convert.ToDecimal(txtProcessingFee.Text.Trim()), 2);
                    decimal Deduction = Decimal.Round(Convert.ToDecimal(txtDeduction.Text.Trim()), 3);
                    decimal Indemnity = Decimal.Round(Convert.ToDecimal(txtIndemnity.Text.Trim()), 2);

                    //inserting data into table tblLoanParameterSetting
                    insertQuery = "insert into tblLoanParameterSetting values(" + ID + ", '" + txtReminder.Text.Trim() + "', " +
                                        "'"+ txtCostPerGram.Text.Trim() +"', '" + txtInterest.Text.Trim() + "', " +
                                        "'" + ProcessingFee + "', '" + Indemnity + "', '" + Deduction + "')";

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

            #region [Update]
            else if (btnSave.Text == "Update")
            {
                decimal ProcessingFee = Decimal.Round(Convert.ToDecimal(txtProcessingFee.Text.Trim()), 2);
                decimal Deduction = Decimal.Round(Convert.ToDecimal(txtDeduction.Text.Trim()), 3);
                decimal Indemnity = Decimal.Round(Convert.ToDecimal(txtIndemnity.Text.Trim()), 2);

                updateQuery = "UPDATE tblLoanParameterSetting SET " +
                                    "PendingLoanReminderDays='" + txtReminder.Text.Trim() + "', " +
                                    "GoldPricePerGram='" + txtCostPerGram.Text.Trim() + "', " +
                                    "InterestPayableForPrepaymentDays='" + txtInterest.Text.Trim() + "', " +
                                    "ProcessingFee='" + ProcessingFee + "', " +
                                    "ProcessingIndemnity='" + Indemnity + "', " +
                                    "DeductionInGrossWeight='" + Deduction + "' " +
                              "WHERE ID=" + txtLPID.Text + "";

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
            #endregion [Update]
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
    #endregion [Save Data]
}




