using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Net;
using System.IO;
using Microsoft.Web.Administration;
using System.Web.Hosting;

public partial class Default : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string clientIPAddress = string.Empty;
    string clientMachineName = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    DataTable dt;
    #endregion

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (!Page.IsPostBack)
            {
                //using (ServerManager iisManager = new ServerManager())
                //{
                //    SiteCollection sites = iisManager.Sites;
                //    foreach (Site site in sites)
                //    {
                //        if (site.Name == HostingEnvironment.ApplicationHost.GetSiteName())
                //        {
                //            iisManager.ApplicationPools[site.Applications["/"].ApplicationPoolName].Recycle();
                //            break;
                //        }
                //    }
                //}

                if (Convert.ToString(Session["username"]) != "" && Session["username"] != null)
                {
                    Response.Redirect("ParentPage.aspx");
                }
                else
                {
                    Session["username"] = "";
                    Session.Remove("username");
                    Session.RemoveAll();
                    Session.Abandon();
                    lblLoginStatus.Text = "";
                }

                //binding Branch
                BindBranch();
                //binding Financial Year
                BindFinancialYear();
            }

            if (Request.QueryString["info"] != null)
            {
                string message = Request.QueryString["info"].ToString();
                if (message == "0")
                {
                    //Response.Write("<strong>you need login first to visit user page.</strong>");
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Sign_In
    protected void butSignIn_Click(object sender, EventArgs e)
    {
        try
        {
            string strQuery = string.Empty;
            string password = string.Empty;
            string userId = string.Empty;
            string userTypeId = string.Empty;
            string userType = string.Empty;
            int branchId = 0;
            int LoginID = 0;
            int CompID = 0;
            lblLoginStatus.Text = "";
            DateTime dtLogin = System.DateTime.Now;

            strQuery = "SELECT UserDetails.UserID, UserDetails.Password, UserDetails.UserTypeID, UserTypeDetails.UserType " +
                       "FROM UserDetails " +
                       "INNER JOIN UserTypeDetails " +
                                "ON UserDetails.UserTypeID = UserTypeDetails.UserTypeID " +
                       "WHERE UserName='" + txtUserName.Text + "' ";

            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand(strQuery, conn);
            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.HasRows)
            {
                while (dr.Read())
                {
                    userId = dr["UserID"].ToString();
                    password = dr["Password"].ToString();
                    userTypeId = dr["UserTypeID"].ToString();
                    userType = dr["UserType"].ToString();
                }

                dr.Close();
                if ((password != "") && (password == txtPassword.Text.Trim()))
                {
                    Session["username"] = txtUserName.Text.Trim().ToUpper();
                    Session["LoggedInUser"] = txtUserName.Text.Trim();
                    Session["userID"] = userId.Trim();
                    Session["usertypeid"] = userTypeId.Trim();
                    Session["usertype"] = userType.Trim();
                    Session["branchname"] = ddlBranch.SelectedItem.Text.Trim().ToUpper();
                    Session["FYear"] = ddlFYear.SelectedItem.Text.Trim().ToUpper();
                    Session["FYearID"] = ddlFYear.SelectedValue;
                    Session["branchId"] = Convert.ToInt32(ddlBranch.SelectedValue);
                    branchId = Convert.ToInt32(ddlBranch.SelectedValue);

                    //getting Date and Time
                    strQuery = "select getDate()";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        dtLogin = Convert.ToDateTime(cmd.ExecuteScalar());
                    }

                    string strLoginDate = dtLogin.ToString("yyyy/MM/dd");
                    string strLoginTime = Convert.ToString(dtLogin.ToLongTimeString());
                    Session["LoginTime"] = dtLogin;

                    //getting hostname and ipaddress
                    clientIPAddress = Request.ServerVariables["http_x_forwarded_for"];
                    if (clientIPAddress == "" || clientIPAddress == null)
                        clientIPAddress = Request.ServerVariables["remote_addr"];
                    clientMachineName = Request.ServerVariables["remote_user"];


                    //*** saving User Login Details into DB
                    transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");

                    //getting MAX LoginID
                    strQuery = "select max(LoginID) from tbl_UserLogin";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        LoginID = Convert.ToInt32(cmd.ExecuteScalar());
                    }
                    else
                    {
                        LoginID = 0;
                    }

                    LoginID += 1;
                    Session["LoginID"] = LoginID;


                    //getting CompID
                    strQuery = "select CompID from tblCompanyBranchMaster where BID=" + branchId + "";
                    cmd = new SqlCommand(strQuery, conn, transaction);
                    if (cmd.ExecuteScalar() != DBNull.Value)
                    {
                        CompID = Convert.ToInt32(cmd.ExecuteScalar());
                    }


                    //inserting data into table tbl_UserLogin
                    insertQuery = "insert into tbl_UserLogin values('" + LoginID + "', '" + userType.Trim() + "', '" + txtUserName.Text.Trim() + "', '" + strLoginDate + "', '" + strLoginTime + "', '', '', " +
                                    "'" + clientMachineName + "', '" + clientIPAddress + "', '" + CompID + "', '" + ddlBranch.SelectedValue + "')";
                    cmd = new SqlCommand(insertQuery, conn, transaction);
                    int QueryResult = cmd.ExecuteNonQuery();

                    if (QueryResult > 0)
                    {
                        transaction.Commit();
                    }
                    else
                    {
                        transaction.Rollback();
                    }

                    Response.Redirect("ParentPage.aspx", false);
                }
                else
                {
                    password = "";
                    txtPassword.Text = "";
                    lblLoginStatus.Text = "Invalid User Name or Password!";
                    txtPassword.Focus();
                }
            }
            else
            {
                password = "";
                lblLoginStatus.Text = "Invalid User Name or Password!";
                txtPassword.Focus();
            }
            conn.Close();
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
    #endregion

    #region Bind Branch
    protected void BindBranch()
    {
        try
        {
            strQuery = "select BID, BranchName from tblCompanyBranchMaster where CompID=1";
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            ddlBranch.DataSource = ds;
            ddlBranch.DataTextField = "BranchName";
            ddlBranch.DataValueField = "BID";
            ddlBranch.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindBranchAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Bind Financial Year
    protected void BindFinancialYear()
    {
        try
        {
            strQuery = "select FinancialyearID, Financialyear from tblFinancialyear where CompID=1 order by FinancialyearID desc ";
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            ddlFYear.DataSource = ds;
            ddlFYear.DataTextField = "Financialyear";
            ddlFYear.DataValueField = "FinancialyearID";
            ddlFYear.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindFYAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion
}