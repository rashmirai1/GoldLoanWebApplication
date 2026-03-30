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

public partial class BankMaster : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    string strQuery = string.Empty;

    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataTable dt;
    SqlCommand cmd;

    int result = 0;
    GlobalSettings gbl = new GlobalSettings();

    public string loginDate;
    public string expressDate;
    bool datasaved = false;
    #endregion

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
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }

    #region Page_Load
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.PropertybtnSave.OnClientClick = "return validrecord();";
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            if (!Page.IsPostBack)
            {
                //Clear Data
                ClearData();
            }

            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }
                FillSearch();
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
            txtBankID.Text = "";
            txtBankName.Text = "";
            txtAlias.Text = "";
            txtAddress.Text = "";
            txtBranch.Text = "";
            txtContact.Text = "";
            txtContactNo1.Text = "";
            txtContactNo2.Text = "";
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ClearAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    protected void FillSearch()
    {
        Master.PropertyddlSearch.Items.Add("Bank Name");
        Master.PropertyddlSearch.Items.Add("Branch");
        Master.PropertyddlSearch.Items.Add("Contact Person");

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
                GL_BankMaster_PRV("Save", "0");
                GL_BankMaster_PRI("Save", "0");
            }
            if (hdnoperation.Value == "Update")
            {
                GL_BankMaster_PRV("Update", hdnid.Value.Trim());
                GL_BankMaster_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }



    }
    #endregion [PropertybtnSave_Click]

    #region [PropertybtnEdit_Click]
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            GL_BankMaster_PopUp();
            hdnPopup.Value = "Edit";

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [PropertybtnEdit_Click]

    protected void PropertygvGlobal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Center;
                e.Row.Cells[3].HorizontalAlign = HorizontalAlign.Center;
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }


    #region [PropertybtnDelete_Click]
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GL_BankMaster_PRV("Delete", hdnid.Value.Trim());
            GL_BankMaster_PRI("Delete", hdnid.Value.Trim());
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
            GL_BankMaster_PopUp();
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
            GL_BankMaster_Search();
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
            GL_BankMaster_RTR(id);
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




    protected void GL_BankMaster_PRI(string Operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transaction;
        cmd.CommandText = "GL_BankMaster_PRI";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@BankID", value);
        cmd.Parameters.AddWithValue("@BankName", txtBankName.Text.Trim());
        cmd.Parameters.AddWithValue("@Address", txtAddress.Text.Trim());
        cmd.Parameters.AddWithValue("@Branch", txtBranch.Text.Trim());
        cmd.Parameters.AddWithValue("@Contactperson", txtContact.Text.Trim());
        cmd.Parameters.AddWithValue("@ContactNo1", txtContactNo1.Text.Trim());
        cmd.Parameters.AddWithValue("@ContactNo2", txtContactNo2.Text.Trim());
        cmd.Parameters.AddWithValue("@BankAlias", txtAlias.Text.Trim());
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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BankMaster", "alert('Record Saved Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }

        if (Operation == "Update" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BankMaster", "alert('Record Update Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Update", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }

        if (Operation == "Delete" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "BankMaster", "alert('Record Delete Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Delele", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }



    }

    protected void GL_BankMaster_PRV(string Operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_BankMaster_PRV";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@Operation", Operation);
        cmd.Parameters.AddWithValue("@BankID", value);
        cmd.Parameters.AddWithValue("@BankName", txtBankName.Text.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void GL_BankMaster_PopUp()
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_BankMaster_PopUp";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
        conn.Close();
    }

    protected void GL_BankMaster_RTR(string id)
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_BankMaster_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@BankID", id);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        hdnid.Value = dt.Rows[0]["BankID"].ToString();
        txtBankName.Text = dt.Rows[0]["BankName"].ToString();
        txtAddress.Text = dt.Rows[0]["Address"].ToString();
        txtAlias.Text = dt.Rows[0]["BankAlias"].ToString();
        txtBranch.Text = dt.Rows[0]["Branch"].ToString();
        txtContact.Text = dt.Rows[0]["Contactperson"].ToString();
        txtContactNo1.Text = dt.Rows[0]["ContactNo1"].ToString();
        txtContactNo2.Text = dt.Rows[0]["ContactNo2"].ToString();
        hdnoperation.Value = "Update";
    }

    protected void GL_BankMaster_Search()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_BankMaster_Search";
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