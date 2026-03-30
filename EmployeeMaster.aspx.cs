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

public partial class EmployeeMaster : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string deleteQuery = string.Empty;
    string updateQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    public string loginDate;
    public string expressDate;
    #endregion

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
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

            //binding GridView
            BindDGVDetails();

            //Get EmployeeCode
            GetEmployeeCode();

            //binding DropDownList Search By
            BindDDLSearchBy();
            ClearData();
            
        }
    }
    #endregion

    #region Bind GridView
    protected void BindDGVDetails()
    {
        try
        {
            strQuery = "SELECT EmployeeID,EmpFirstName,EmpMiddleName,EmpLastName,EmpCode,(EmpFirstName+'   '+EmpMiddleName+'   '+EmpLastName) as 'EmployeeName',Address ,MobileNo,(StdCode+'-'+TelephoneNo) as 'TelephoneNo',EmailId FROM tblEmployeeMaster";
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

    #region AutoGenerate Employee Code
    protected void GetEmployeeCode()
    {
        try
        {
            conn = new SqlConnection(strConnString);
             conn.Open();
            int EmployeeID = 0;
            //int ECode = 0;
             string s11 =string.Empty;
            string s1=string.Empty;
            strQuery = ("select Max(EmployeeID) from tblEmployeeMaster");
           
            cmd = new SqlCommand(strQuery,conn);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                EmployeeID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                EmployeeID = 0;
            }
            EmployeeID += 1;

            Session["EmployeeID"] = EmployeeID;

            s1 = Convert.ToString(EmployeeID);

            if (s1.Length == 1)
            {
                s11 = Convert.ToString("0" + EmployeeID);            
            }
            else
            {
                s11 = Convert.ToString( EmployeeID);
                     
            }
        
            Session["Date"] = DateTime.Now;
            string Month = Convert.ToDateTime(Session["Date"]).ToString("MM");        
            string year = Convert.ToDateTime(Session["Date"]).ToString("yy");           
            txtEmpCode.Text = Convert.ToString("E" + Month + year + s11);
            Session["EmpCode"] = Convert.ToString (txtEmpCode.Text);    
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "EmpCodeAlert", "alert('" + ex.Message + "');", true);

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

    #region Bind DropDownList-SearchBy
    protected void BindDDLSearchBy()
    {
        try
        {
            ddlSearchBy.Items.Add("EmployeeName");
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
                
                if (_commandName == "DeleteRecord")
                {
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    //checking whether record is present
                    strQuery = "select count(*) from tblEmployeeMaster where EmployeeID=" + ID + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    int existcount = Convert.ToInt32(cmd.ExecuteScalar());

                    if (existcount > 0)
                    {
                        //deleting record from DB
                        deleteQuery = "delete from tblEmployeeMaster where EmployeeID=" + ID + "";
                        cmd = new SqlCommand(deleteQuery, conn, transaction);
                        int QueryResult = cmd.ExecuteNonQuery();

                        if (QueryResult > 0)
                        {
                            transaction.Commit();
                            ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Record Deleted Successfully.');", true);
                            BindDGVDetails();
                            if (txtEmpID.Text != "" && txtEmpID.Text != null)
                            {
                                if (ID == Convert.ToInt32(txtEmpID.Text))
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

                else if (_commandName == "UpdateRecord")
                {
                    //fill records in the form
                    dsDGV = GetRecords(conn, "UpdateRecord", ID);
                    txtEmpID.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][0]);
                    txtEmpFName.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][1]);
                    txtEmpMName.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][2]);
                    txtEmpLName.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][3]);
                    txtAddress.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][5]);
                    txtMobileNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][6]);
                    txtCode.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][7]);
                    txtTelephoneNo.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][8]);
                    txtEmailId.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][9]);
                    txtEmpCode.Text = Convert.ToString(dsDGV.Tables[0].Rows[0][10]);
                    btnSave.Text = "Update";
                    btnReset.Text = "Cancel";
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

    #region GetRecords
    protected DataSet GetRecords(SqlConnection conn, string CommandName, int ID)
    {
        try
        {
            if (CommandName == "GetAllRecords")
            {
                strQuery = "SELECT EmployeeID,EmpFirstName,EmpMiddleName,EmpLastName,(EmpFirstName+'   '+EmpMiddleName+'   '+EmpLastName) as 'EmployeeName',Address,MobileNo,StdCode,TelephoneNo,EmailId,EmpCode FROM tblEmployeeMaster";
            }
            else if (CommandName == "UpdateRecord")
            {
                strQuery = "SELECT EmployeeID,EmpFirstName,EmpMiddleName,EmpLastName,(EmpFirstName+'   '+EmpMiddleName+'   '+EmpLastName) as 'EmployeeName',Address,MobileNo,StdCode,TelephoneNo,EmailId,EmpCode FROM tblEmployeeMaster where EmployeeID=" + ID + "";

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

    #region Save
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            string strQuery = string.Empty;
            string msg = string.Empty;
            int EmployeeID = 0;

            conn = new SqlConnection(strConnString);
            conn.Open();
            //Save Employee Details in DataBase
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

            if (btnSave.Text == "Save")
            {
                //Getting Max EmployeeID
                strQuery = ("select Max(EmployeeID) from tblEmployeeMaster");
                cmd = new SqlCommand(strQuery, conn, transaction);
                if (cmd.ExecuteScalar() != DBNull.Value)
                {
                    EmployeeID = Convert.ToInt32(cmd.ExecuteScalar());
                }
                else
                {
                    EmployeeID = 0;
                }
                EmployeeID += 1;

                GetEmployeeCode();
                conn = new SqlConnection(strConnString);
                conn.Open();
                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                Session["EmployeeID"] = EmployeeID;
             //  string EmployeeCode=("000" + Session["EmployeeCode"]).ToString();
                cmd = new SqlCommand("InsertEmployee", conn,transaction);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmpID", EmployeeID);
                cmd.Parameters.AddWithValue("@EmpCode", Session["EmpCode"].ToString());
                cmd.Parameters.AddWithValue("@EmpFName", txtEmpFName.Text.ToString());
                cmd.Parameters.AddWithValue("@EmpMName", txtEmpMName.Text.ToString());
                cmd.Parameters.AddWithValue("@EmpLName", txtEmpLName.Text.ToString());
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.ToString());
                cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text.ToString());
                cmd.Parameters.AddWithValue("@StdCode", txtCode.Text.ToString());
                cmd.Parameters.AddWithValue("@TelephoneNo", txtTelephoneNo.Text.ToString());
                cmd.Parameters.AddWithValue("@EmailId", txtEmailId.Text.ToString());
               
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
            else if (btnSave.Text == "Update")
            {
                //updating table tblEmployeeMaster
                updateQuery = "update tblEmployeeMaster set EmpFirstName=@EmpFName ,EmpMiddleName=@EmpMName,EmpLastName=@EmpLName, Address=@Address , MobileNo='" + txtMobileNo.Text + "',StdCode='" + txtCode.Text.Trim() + "',TelephoneNo='" + txtTelephoneNo.Text + "',EmailID='" + txtEmailId.Text.Trim() + "' where EmployeeID='" + txtEmpID.Text + "'";

                cmd = new SqlCommand(updateQuery, conn, transaction);
                cmd.Parameters.AddWithValue("@EmpFName", txtEmpFName.Text.ToString());
                cmd.Parameters.AddWithValue("@EmpMName", txtEmpMName.Text.ToString());
                cmd.Parameters.AddWithValue("@EmpLName", txtEmpLName.Text.ToString());
                cmd.Parameters.AddWithValue("@Address", txtAddress.Text.ToString());
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

    #region ClearData
    protected void ClearData()
    {
        try
        {
            txtEmpID.Text = "";
            txtEmpFName.Text = "";
            txtEmpLName.Text = "";
            txtEmpMName.Text = "";
            txtMobileNo.Text = "";
            txtTelephoneNo.Text = "";
            txtEmailId.Text = "";
            txtAddress.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
            txtCode.Text = "";
            txtSearch.Text = "";

            //Get EmployeeCode
            GetEmployeeCode();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion
}



