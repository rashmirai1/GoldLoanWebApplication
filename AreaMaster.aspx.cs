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

public partial class AreaMaster : System.Web.UI.Page
{
    #region [Declarations]
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
    #endregion [Declarations]

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
    #region [Page_Load]
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
                BindCity();

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
    #endregion [Page_Load]
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
            txtAreaID.Text = "";
            txtArea.Text = "";
            txtPinCode.Text = "";
            ddlCity.SelectedIndex = 0;
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion
    protected void BindCity()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CityBound_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        ddlCity.DataSource = dt;
        ddlCity.DataTextField = "CityName";
        ddlCity.DataValueField = "CityID";
        ddlCity.DataBind();
        ddlCity.Items.Insert(0, new ListItem("--Select City--", "0"));
    }
    protected void FillSearch()
    {
        Master.PropertyddlSearch.Items.Add("Area Name");
        Master.PropertyddlSearch.Items.Add("City");
        Master.PropertyddlSearch.Items.Add("Pin Code");
    }


    #region [PropertyImgBtnClose_Click]

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {
        try
        {
            Master.PropertympeGlobal.Hide();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "AreaAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertyImgBtnClose_Click]

    #region [PropertybtnSave_Click]
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {
                GL_AreaMaster_PRV("Save", "0");
                GL_AreaMaster_PRI("Save", "0");
            }
            if (hdnoperation.Value == "Update")
            {
                GL_AreaMaster_PRV("Update", hdnid.Value.Trim());
                GL_AreaMaster_PRI("Update", hdnid.Value.Trim());
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
            GL_Area_PopUp_RTR();
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
            e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Left;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "AreaAlert", "alert('" + ex.Message + "');", true);
        }

    }


    #region [PropertybtnDelete_Click]
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GL_AreaMaster_PRV("Delete", hdnid.Value.Trim());
            GL_AreaMaster_PRI("Delete", hdnid.Value.Trim());
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "AreaAlert", "alert('" + ex.Message + "');", true);
        }


    }
    #endregion [PropertybtnDelete_Click]

    #region [PropertybtnView_Click]
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GL_Area_PopUp_RTR();
            hdnPopup.Value = "View";

            gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [PropertybtnView_Click]

    #region [PropertybtnCancel_Click]
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("AreaMaster.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion [PropertybtnCancel_Click]

    #region [PropertybtnSearch_Click]
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            GL_AreaMaster_Search();
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
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
            GL_AreaMaster_RTR(id);
            hdnoperation.Value = "Update";
            gbl.CheckAEDControlSettings(hdnPopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }


    }
    #endregion [PropertygvGlobal_RowCommand]

    protected void GL_AreaMaster_PRI(string Operation, string value)
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transaction;
        cmd.CommandText = "GL_AreaMaster_PRI";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@AreaID", value);
        cmd.Parameters.AddWithValue("@Area", txtArea.Text.Trim());
        cmd.Parameters.AddWithValue("@CityID", ddlCity.SelectedValue);
        cmd.Parameters.AddWithValue("@Pincode", txtPinCode.Text.Trim());
        cmd.Parameters.AddWithValue("@BlackListed", 'N');
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
            datasaved = false;
        }

        if (Operation == "Save" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AreaMaster", "alert('Record Saved Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }

        if (Operation == "Update" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AreaMaster", "alert('Record Update Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Update", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }

        if (Operation == "Delete" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "AreaMaster", "alert('Record Delete Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Delele", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
    }

    protected void GL_AreaMaster_PRV(string Operation, string value)
    {
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_AreaMaster_PRV";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@AreaID", value);
        cmd.Parameters.AddWithValue("@Area", txtArea.Text.Trim());

        result = cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void GL_Area_PopUp_RTR()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Area_PopUp_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();

    }

    protected void GL_AreaMaster_RTR(string id)
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_AreaMaster_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@AreaID", id);
        da = new SqlDataAdapter(cmd);
        dt1 = new DataTable();
        da.Fill(dt1);
        hdnid.Value = dt1.Rows[0]["AreaID"].ToString();
        txtArea.Text = dt1.Rows[0]["Area"].ToString();
        ddlCity.SelectedValue = dt1.Rows[0]["CityID"].ToString();
        BindCity();
        txtPinCode.Text = dt1.Rows[0]["Pincode"].ToString();
        hdnoperation.Value = "Update";
    }

    protected void GL_AreaMaster_Search()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_AreaMaster_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Searchtype", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }


}