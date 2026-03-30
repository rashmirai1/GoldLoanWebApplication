using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Net;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Text;
using System.Drawing;
using System.Web.Services;
using System.Web.Script.Services;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string updateQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlCommand cmd, cmd1;
    //For Gold Rates
    string temp;
    #endregion


    #region Current Indian Gold Rates

    public void LoadCurrentGoldRates()
    {
        //CHECK IF SYSTEM HAS INTERNET CONNECTION
        if (!HasConnection())
        {
            lblGoldRate24.Text = "No Internet connection!";
            return;
        }
    }

    public static bool HasConnection()
    {
        // CHECK IF SYSTEM HAS INTERNET CONNECTION
        try
        {
            System.Net.IPHostEntry i = System.Net.Dns.GetHostEntry("www.indiagoldrate.com");
            return true;
        }
        catch
        {
            return false;
        }
    }

    public string rates(string urlAddress)
    {

        try
        {
            //string urlAddress = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm";
            //string urlAddress = "https://www.moneycontrol.com/";
            //"https://www.moneycontrol.com/";
             urlAddress = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();

                //string abb = data.Substring(data.LastIndexOf("Gold Price in Mumbai"), 161);
                //string rate = abb.Substring(155, 6);

                string abb = data.Substring(data.LastIndexOf("pad-15"), 14);
                string rate = abb.Substring(8, 6);

                temp = rate.Replace(",", "");

                //temp = data.Substring(data.LastIndexOf("10 gram"), 30);
                //string ch = "Gold price slips marginally to Rs ";
                //string abb = data.Substring(data.LastIndexOf("Gold price slips marginally to Rs"), 40);
                //temp = abb.TrimStart(ch.ToCharArray());
                //temp = temp.Replace("<TD>", "");
                //temp = temp.Replace("</TD>", "");
                //temp = temp.Replace("\t", " ");
                //temp = temp.Replace("\n", " ");
                //temp = temp.Replace("10 gram", "");
                //temp = temp.Replace(" ", "");
                //temp = temp.Replace("Rs.", "");
                //temp = temp.Replace(",", "");

                //decimal TwentyThreeCarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.958);
                //lblGoldRate23.Text += " " + decimal.Round(TwentyThreeCarat, 2);

                //decimal TwentyTwokarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.916);
                //lblGoldRate22.Text += " " + decimal.Round(TwentyTwokarat, 2);

                //decimal TwentyOnekarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.875);
                //lblGoldRate21.Text += " " + decimal.Round(TwentyOnekarat, 2);

                //decimal Twentykarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.830);
                //lblGoldRate20.Text += " " + decimal.Round(Twentykarat, 2);

                //decimal Eighteenkarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.75);
                //lblGoldRate18.Text += " " + decimal.Round(Eighteenkarat, 2);
                //Hidden18k.Value = lblGoldRate18.Text;
            }
        }
        catch (Exception)
        {
            //MessageBox.Show("Cannot connect to the server."); 
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('Cannot connect to the server.');", true);
        }
        return temp;
    }
    #endregion
    protected void Page_PreRender(Object sender, EventArgs e)
    {
        // string UserID = Session["userid"].ToString();
        //  Get_UserControl_Permission();
        string Actionpage = Path.GetFileName(Request.Url.AbsolutePath);
        GetMenuData();

    }
    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            if (!IsPostBack)
            {

                if (Session["username"] != null)
                {
                    lblUserName.Text = Convert.ToString(Session["username"]);
                    lblBranchName.Text = Convert.ToString(Session["branchname"]);
                    lblFYear.Text = Convert.ToString(Session["FYear"]);

                    //lblGoldRate24.Text = "24K: Rs. ";
                    //lblGoldRate23.Text = "23K: Rs. ";
                    //lblGoldRate22.Text = "22K: Rs. ";
                    //lblGoldRate21.Text = "21K: Rs. ";
                    //lblGoldRate20.Text = "20K: Rs. ";
                    //lblGoldRate18.Text = "18K: Rs. ";

                    ////  For Current Indian Gold Rates
                    //LoadCurrentGoldRates();
                    //string temp = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm@http://www.indiagoldrate.com/silver-rate-in-mumbai-today.htm";
                    //temp = temp.Substring(0, temp.IndexOf("@"));

                    //lblGoldRate24.Text += " " + rates(temp);
                    CheckFinancialYear_RTR();
                    // GetMenuData();
                    Get_UserControl_Permission();
                }
            }

        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion


    protected void GetMenuData()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_ParentMenu_RTR";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        MasterMenu.DataSource = dt;
        MasterMenu.DataBind();
        Get_SubMenu(MasterMenu);

    }

    protected void Get_SubMenu(Repeater MasterMenu)
    {
        string UserID;
        if (Session["userid"] != null)
        {
            UserID = Session["userid"].ToString();
        }
        else
        {
            UserID = "0";
        }
        for (int i = 0; i < MasterMenu.Items.Count; i++)
        {
            Repeater SubMenu = (Repeater)MasterMenu.Items[i].FindControl("SubMenu") as Repeater;
            HiddenField hdParent = (HiddenField)MasterMenu.Items[i].FindControl("hdnParentFormID") as HiddenField;
            HiddenField hdIsFormAuth = (HiddenField)MasterMenu.Items[i].FindControl("hdnFormAuthID") as HiddenField;

            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_ChildtMenu_RTR";
            cmd.Parameters.AddWithValue("@ParentID", hdParent.Value.Trim());
            cmd.Parameters.AddWithValue("@UserID", UserID);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);

            SubMenu.DataSource = dt;
            SubMenu.DataBind();

        }

    }

    protected void Get_UserControl_Permission()
    {
        try
        {
            string FormName = Path.GetFileName(Request.Url.AbsolutePath);
            string UserID = Session["UserId"].ToString();
            if (FormName != "ParentPage.aspx")
            {
                if (Session["usertypeid"].ToString() != "1") //For Live DB
                {
                    //if (Session["usertypeid"].ToString() != "5")
                    //{
                    conn = new SqlConnection(strConnString);
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    conn.Open();
                    cmd.CommandTimeout = 0;
                    cmd.CommandText = "FormMenu_Athorization";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FormName", FormName);
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int FormId = Convert.ToInt32(ds.Tables[0].Rows[0]["FormId"].ToString());
                        conn = new SqlConnection(strConnString);
                        cmd1 = new SqlCommand();
                        cmd1.Connection = conn;
                        conn.Open();
                        cmd1.CommandTimeout = 0;
                        cmd1.CommandText = "UserForm_Athorization";
                        cmd1.CommandType = CommandType.StoredProcedure;
                        cmd1.Parameters.AddWithValue("@UserID", UserID);
                        cmd1.Parameters.AddWithValue("@FormId", FormId);
                        DataSet ds1 = new DataSet();
                        SqlDataAdapter da1 = new SqlDataAdapter(cmd1);
                        da1.Fill(ds1);

                        if (ds1.Tables[0].Rows.Count > 0)
                        {

                            btnView.Visible = Convert.ToBoolean(ds1.Tables[0].Rows[0]["IsView"].ToString());
                            btnSave.Visible = Convert.ToBoolean(ds1.Tables[0].Rows[0]["IsSave"].ToString());
                            btnEdit.Visible = Convert.ToBoolean(ds1.Tables[0].Rows[0]["IsEdit"].ToString());
                            btnDelete.Visible = Convert.ToBoolean(ds1.Tables[0].Rows[0]["IsDelete"].ToString());
                            //btnSave.Enabled = true;
                            //btnDelete.Enabled = true;
                            //btnView.Enabled = true;
                            //btnEdit.Enabled = true;
                        }
                        else
                        {
                            btnEdit.Visible = false;
                            btnSave.Visible = false;
                            btnDelete.Visible = false;
                            btnView.Visible = false;
                        }
                    }
                    //btnEdit.Visible = false;
                    //btnSave.Visible = false;
                    //btnDelete.Visible = false;
                    //btnView.Visible = false;


                }
            }
            else
            {
            }
        }
        catch (Exception ex)
        {

            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }


    #region [CheckFinancialYear_RTR]
    public void CheckFinancialYear_RTR()
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Gl_CheckFY_RTR";
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.ExecuteNonQuery();
    }
    #endregion [CheckFinancialYear_RTR]
    #region Sign_Out
    protected void lnkSignOut_Click(object sender, EventArgs e)
    {
        try
        {
            //*** saving user login details into DB
            conn = new SqlConnection(strConnString);
            conn.Open();
            transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForSave");
            DateTime dtLogOut = System.DateTime.Now;
            DateTime dtTotalTime = System.DateTime.Now;
            DateTime dtLoginTime = System.DateTime.Now;
            int LoginID = 0;

            if (Session["LoginID"] != null)
            {
                LoginID = Convert.ToInt32(Session["LoginID"]);
            }

            if (Session["LoginTime"] != null)
            {
                dtLoginTime = Convert.ToDateTime(Session["LoginTime"]);
            }


            //getting Date
            strQuery = "select getDate()";
            cmd = new SqlCommand(strQuery, conn, transaction);
            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                dtLogOut = Convert.ToDateTime(cmd.ExecuteScalar());
            }

            string strLogOutTime = Convert.ToString(dtLogOut.ToLongTimeString());
            dtTotalTime = dtLogOut.Subtract(dtLoginTime.TimeOfDay);
            string strTotalTime = dtTotalTime.ToLongTimeString();


            //updating table tbl_UserLogin
            if (LoginID > 0)
            {
                updateQuery = "update tbl_UserLogin set LogoutTime='" + strLogOutTime + "', TotalTime='" + strTotalTime + "' " +
                                "where LoginID='" + LoginID + "'";
                cmd = new SqlCommand(updateQuery, conn, transaction);
                int QueryResult = cmd.ExecuteNonQuery();

                if (QueryResult > 0)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }

            Session.Abandon();
            Response.Redirect("Default.aspx", false);
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion
    protected void gvGlobal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            string Actionpage = Path.GetFileName(Request.Url.AbsolutePath);
            if (Actionpage == "ParentPage.aspx")
            {
            }
            else if (Actionpage == "GLOutstanding.aspx") { }
            else
            {
                string rowID = String.Empty;
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    e.Row.Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gvGlobal, "Select$" + e.Row.RowIndex);
                    //e.Row.BackColor = System.Drawing.Color.Red;
                    e.Row.ToolTip = "Click for open.";

                }
            }
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }



    }
    // by Priya 17/4/2015 start
    protected void gvExcel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    protected void gvPLExcel_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    //end priya

    public void CheckAEDControlSettings(string btnstatus, Button btnAdd)
    {

        //if (Convert.ToString(btnId.Text) == "Edit")
        //{

        //   // btnId.Enabled = false;

        //}
    }
    #region properties
    //*****************************************************
    // by bharat 13/10/2014
    //*****************************************************


    public Panel PropertyPanelStrip
    {
        get { return pnlStrip; }
    }
    public Button PropertybtnEdit
    {
        get { return btnEdit; }
    }
    public Button PropertybtnSave
    {
        get { return btnSave; }
    }
    public Button PropertybtnView
    {
        get { return btnView; }
    }
    public Button PropertybtnDelete
    {
        get { return btnDelete; }
    }
    public Button PropertybtnCancel
    {
        get { return btnCancel; }
    }
    public DropDownList PropertyddlSearch
    {
        get { return ddlSearch; }
    }
    public TextBox PropertytxtSearch
    {
        get { return txtSearhText; }
    }
    public Label PropertylblSearch
    {
        get { return lblHeader; }
    }
    public Button PropertybtnSearch
    {
        get { return btnSearch; }
    }
    public GridView PropertygvGlobal
    {
        get { return gvGlobal; }

    }
    public ImageButton PropertyImgBtnClose
    {
        get { return ImgBtnClose; }
    }
    public AjaxControlToolkit.ModalPopupExtender PropertympeGlobal
    {
        get { return mpGlobal; }
    }
    //Added by priya on 10/8/2015 for loginPopup
    public GridView PropertygvLoginPopup
    {
        get { return gvLoginPopup; }
    }

    public Label PropertylblSerarch
    {
        get { return lblSerarch; }
    }
    public Label PropertylblSearchText
    {
        get { return lblSearchText; }
    }
    public Label PropertylblHeader
    {
        get { return lblHeader; }
    }


    #endregion

    protected void ImgBtnClose_Click(object sender, ImageClickEventArgs e)
    {
        mpGlobal.Hide();

    }
    protected void gvGlobal_PreRender(object sender, EventArgs e)
    {
        //  gvGlobal.UseAccessibleHeader = true;
        // gvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
    }


}
