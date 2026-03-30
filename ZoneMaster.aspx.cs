using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;


public partial class ZoneMaster : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    string strQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataTable dt;
    DataTable dt1;
    SqlCommand cmd;
    GlobalSettings st = null;
    GlobalSettings gbl = new GlobalSettings();
    int result = 0;

    public string loginDate;
    public string expressDate;
    bool datasaved = false;
    #endregion

    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Click += new EventHandler(PropertybtnEdit_Click);
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnDelete.Click += new EventHandler(PropertybtnDelete_Click);
        Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
        Master.PropertyImgBtnClose.Click += new ImageClickEventHandler(PropertyImgBtnClose_Click);
        Master.PropertygvGlobal.RowCommand += new GridViewCommandEventHandler(PropertygvGlobal_RowCommand);
        Master.PropertygvGlobal.RowDataBound += new GridViewRowEventHandler(PropertygvGlobal_RowDataBound);
        // Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.PropertybtnSave.OnClientClick = "return Validate();";

            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }
                BindArea();
                FillSearch();
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
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

    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    #region ClearData
    protected void ClearData()
    {
        try
        {
            hdnid.Value = "";
            hdnoperation.Value = "Save";
            txtZoneID.Text = "";
            txtZone.Text = "";
            ddlArea.SelectedIndex = 0;
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    protected void BindArea()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_AreaBound_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlArea.DataSource = dt;
        ddlArea.DataTextField = "Area";
        ddlArea.DataValueField = "AreaID";
        ddlArea.DataBind();
        ddlArea.Items.Insert(0, new ListItem("--Select Area--", "0"));
    }
    protected void FillSearch()
    {
        Master.PropertyddlSearch.Items.Add("Zone");
        Master.PropertyddlSearch.Items.Add("Area");
    }


    #region [PropertyImgBtnClose_Click]

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {
        Master.PropertympeGlobal.Hide();
        ClearData();
        // BlankGrdBind();
        // gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

    }
    #endregion [PropertyImgBtnClose_Click]

    #region [PropertybtnSave_Click]
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {
                GL_ZoneMaster_PRV("Save", "0");
                GL_ZoneMaster_PRI("Save", "0");
            }
            if (hdnoperation.Value == "Update")
            {
                GL_ZoneMaster_PRV("Update", hdnid.Value.Trim());
                GL_ZoneMaster_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {
            // hdnoperation.Value = "";
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
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
    #endregion [PropertybtnSave_Click]

    #region [PropertybtnEdit_Click]
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            GL_Zone_PopUp_RTR();
            hdnPopup.Value = "Edit";

        }
        catch (Exception ex)
        {

        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [PropertybtnEdit_Click]

    protected void PropertygvGlobal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
            e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;

        }
        catch (Exception ex)
        { }

    }


    #region [PropertybtnDelete_Click]
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GL_ZoneMaster_PRV("Delete", hdnid.Value.Trim());
            GL_ZoneMaster_PRI("Delete", hdnid.Value.Trim());
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
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
    #endregion [PropertybtnDelete_Click]

    #region [PropertybtnView_Click]
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GL_Zone_PopUp_RTR();
            hdnPopup.Value = "View";

            gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
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
    #endregion [PropertybtnView_Click]

    #region [PropertybtnCancel_Click]
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        ClearData();
        gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
    }
    #endregion [PropertybtnCancel_Click]

    #region [PropertybtnSearch_Click]
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            GL_ZoneMaster_Search();
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
    #endregion [PropertybtnSearch_Click]

    #region [PropertygvGlobal_RowCommand]
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
            GL_ZoneMaster_RTR(id);
            hdnoperation.Value = "Update";
            gbl.CheckAEDControlSettings(hdnPopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {
            hdnoperation.Value = "";
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
    #endregion [PropertygvGlobal_RowCommand]

    protected void GL_ZoneMaster_PRI(string Operation, string value)
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transaction;
        cmd.CommandText = "GL_ZoneMaster_PRI";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@ZoneID", value);
        cmd.Parameters.AddWithValue("@Zone", txtZone.Text.Trim());
        cmd.Parameters.AddWithValue("@AreaID", ddlArea.SelectedValue);
        cmd.Parameters.AddWithValue("@Lineno", 0);
        result = cmd.ExecuteNonQuery();

        if (result > 0)
        {
            transaction.Commit();
            datasaved = true;
        }
        else
        {
            transaction.Rollback();
            datasaved = false; ;

        }
        if (Operation == "Save" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ZoneMaster", "alert('Record Saved Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        else
            if (Operation == "Update" && datasaved)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ZoneMaster", "alert('Record Update Successfully');", true);
                ClearData();
                gbl.CheckAEDControlSettings("Update", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

            }
            else
                if (Operation == "Delete" && datasaved)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "ZoneMaster", "alert('Record Delete Successfully');", true);
                    ClearData();
                    gbl.CheckAEDControlSettings("Delele", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

                }
    }

    protected void GL_ZoneMaster_PRV(string Operation, string value)
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_ZoneMaster_PRV";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@ZoneID", value);
        cmd.Parameters.AddWithValue("@Zone", txtZone.Text.Trim());
        cmd.Parameters.AddWithValue("@AreaID", ddlArea.SelectedValue);
        cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void GL_Zone_PopUp_RTR()
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Zone_PopUp_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
        conn.Close();
    }

    protected void GL_ZoneMaster_RTR(string id)
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_ZoneMaster_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@ZoneID", id);
        da = new SqlDataAdapter(cmd);
        dt1 = new DataTable();
        da.Fill(dt1);
        hdnid.Value = dt1.Rows[0]["ZoneID"].ToString();
        txtZone.Text = dt1.Rows[0]["Zone"].ToString();
        ddlArea.SelectedValue = dt1.Rows[0]["AreaID"].ToString();
        BindArea();
        hdnoperation.Value = "Update";
        conn.Close();
    }

    protected void GL_ZoneMaster_Search()
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_ZoneMaster_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Searchtype", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
        conn.Close();
    }



}


