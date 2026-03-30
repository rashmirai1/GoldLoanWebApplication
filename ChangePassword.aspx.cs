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


public partial class ChangePassword : System.Web.UI.Page
{
    #region Declarations
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

            try
            {

                //Clear data
                ClearData();

                //Get The Current password 
                GetData();

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
    }
    #endregion

    #region Reset/Cancel
    protected void btnReset_Click(object sender, EventArgs e)
    {
        try
        {
            txtUserName.Text = "";
            txtConfirmPassword.Text = "";
            txtNewPassword.Text = "";
            lblMsg.Text = "";
            btnSave.Text = "Save";
            btnReset.Text = "Reset";
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
        
        txtCurrentPaasword.Text = "";
        txtUserName.Text = "";
        txtConfirmPassword.Text = "";
        txtNewPassword.Text = "";
        btnSave.Text = "Save";
        btnReset.Text = "Reset";
    }
    #endregion

    #region Save
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (txtCurrentPaasword.Text.ToString() == txtNewPassword.Text.ToString())
            {
                lblMsg.Text = "New Password Must be different from Current password";
                txtNewPassword.Focus();
            }
            else
            {

                string strQuery = string.Empty;
                string NewPassword = txtNewPassword.Text.Trim();
                string ConfirmPassword = txtConfirmPassword.Text.Trim();

                conn = new SqlConnection(strConnString);
                conn.Open();
                transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
                strQuery = "UPDATE UserDetails SET Password='" + txtConfirmPassword.Text.Trim() + "' WHERE UserID='" + txtUserName.Text.Trim() + "'";
                cmd = new SqlCommand(strQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult == 1)
                {
                    transaction.Commit();
                    conn.Close();
                    //display messege .
                    ClientScript.RegisterStartupScript(this.GetType(), "ChangePWDAlert", "alert('Password updated Successfully.');", true);
                    ClearData();

                    //Get the Updated Password
                  
                    GetData();
          
                }

            }
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            ClientScript.RegisterStartupScript(this.GetType(), "ChangePWDAlert", "alert('" + ex.Message + "');", true);
        }

       

    }
    #endregion

    #region GetData
    protected void GetData()
    {
        try
        {

            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "SELECT UserID,Password FROM UserDetails WHERE UserName='" + Session["LoggedInUser"] + "'";
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds, "UserDetails");
            if (ds.Tables["UserDetails"].Rows.Count > 0)
            {
                foreach (DataRow drow in ds.Tables["UserDetails"].Rows)
                {
                    txtUserName.Text = Convert.ToString(drow["UserID"]);
                    txtCurrentPaasword.Text = Convert.ToString(drow["Password"]);
                    txtNewPassword.Focus();
                }
            }
            else
            {
                txtUserName.Text = "";
                txtCurrentPaasword.Text = "";
                txtConfirmPassword.Text = "";
                txtNewPassword.Text = "";
            }


        }
        catch (Exception ex)
        {
            transaction.Rollback();
            ClientScript.RegisterStartupScript(this.GetType(), "ChangePWDAlert", "alert('" + ex.Message + "');", true);
        }

        
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
    }
    #endregion
}

