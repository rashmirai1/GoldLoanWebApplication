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
using System.Text;

public partial class GLAuthorization : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    #endregion

    #region [Declarations var]
    //Declaring Objects.   
    SqlTransaction transactionGL, transactionAIM;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    DataTable dt;
    int result = 0;
    bool datasaved = false;
    #endregion [Declarations var]

    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
    }



    protected void Page_PreRender(Object sender, EventArgs e)
    {

        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnView.Visible = false;


    }
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }


            Master.PropertybtnSave.OnClientClick = "return valid();";

            if (!IsPostBack)
            {
                Master.PropertybtnCancel.Visible = true;
                Master.PropertybtnDelete.Visible = false;
                Master.PropertybtnEdit.Visible = false;
                Master.PropertybtnView.Visible = false;
                Master.PropertybtnSave.Visible = true;
                FillUserTypes();
                //   FillFormMenu();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FormAlert", "alert('" + ex.Message + "');", true);
        }

    }
    public void FillUserTypes()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SelectAuthorization";
        cmd.Parameters.AddWithValue("@Operation", "SelectUserType");
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            ddlUserType.DataSource = dt;
            ddlUserType.DataTextField = "UserType";
            ddlUserType.DataValueField = "UserTypeID";
            ddlUserType.DataBind();
            ddlUserType.Items.Insert(0, new ListItem("--Select User Type--", "0"));
        }
    }
    public void FillUserNames()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SelectAuthorization";
        cmd.Parameters.AddWithValue("@Operation", "SelectUserName");
        cmd.Parameters.AddWithValue("@UserTypeID", ddlUserType.SelectedValue);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            ddlUserName.DataSource = dt;
            ddlUserName.DataTextField = "UserName";
            ddlUserName.DataValueField = "UserID";
            ddlUserName.DataBind();
            ddlUserName.Items.Insert(0, new ListItem("--Select User Name--", "0"));
        }
    }


    public void FillFormMenu()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SelectAuthorization";
        cmd.Parameters.AddWithValue("@Operation", "SelectFormMenu");
        // cmd.Parameters.AddWithValue("@UserTypeID", ddlUserType.SelectedValue);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            ddlMasterMenu.DataSource = dt;
            ddlMasterMenu.DataTextField = "FormName";
            ddlMasterMenu.DataValueField = "FormID";
            ddlMasterMenu.DataBind();
            ddlMasterMenu.Items.Insert(0, new ListItem("--Select Form Menu--", "0"));
            ddlMasterMenu.Items.Insert(5, new ListItem("All Forms", "All"));
        }

        // ddlUserType.SelectedIndex = 0;
        // ddlUserName.SelectedIndex = 0;
    }

    public void FillGridDetails()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_SelectAuthorization";
        cmd.Parameters.AddWithValue("@Operation", "SelectAutorization");
        cmd.Parameters.AddWithValue("@UserID", ddlUserName.SelectedValue);
        cmd.Parameters.AddWithValue("@ParentID", ddlMasterMenu.SelectedValue);
        DataSet ds = new DataSet();
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        da.Fill(ds);
        hdnOpration.Value = ds.Tables[0].Rows[0]["InsertUpdate"].ToString();
        //da = new SqlDataAdapter(cmd);
        //dt = new DataTable();
        //da.Fill(dt);

        if (ds.Tables[1].Rows.Count > 0)
        {
            gvDetails.Visible = true;
            gvDetails.DataSource = ds.Tables[1];
            gvDetails.DataBind();
        }
        else
        {
            gvDetails.Visible = false;
            gvDetails.DataSource = null;
        }
    }

    protected void ddlUserType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //  ddlUserName.SelectedIndex = 0;
            gvDetails.Visible = false;
            gvDetails.DataSource = null;
            FillUserNames();

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + ex.Message + "');", true);

        }
    }
    protected void ddlUserName_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // FillGridDetails();

            //ddlMasterMenu.SelectedIndex = 0;
            gvDetails.Visible = false;
            gvDetails.DataSource = null;
            FillFormMenu();

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + ex.Message + "');", true);

        }
    }

    protected void ddlMasterMenu_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            gvDetails.Visible = false;
            gvDetails.DataSource = null;
            FillGridDetails();

        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + ex.Message + "');", true);

        }
    }

    // ddlMasterMenu_SelectedIndexChanged

    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnOpration.Value == "Save")
            {
                insertUpdateAuthorization("Save", "0");
            }
            if (hdnOpration.Value == "Update")
            {
                insertUpdateAuthorization("Update", "0");
            }
        }
        catch (Exception ex)
        {
            ScriptManager.RegisterClientScriptBlock(Page, Page.GetType(), Guid.NewGuid().ToString(), "alert('" + ex.Message + "');", true);

        }
    }


    public void insertUpdateAuthorization(string operation, string value)
    {
        int cnt = 0;
        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    if (row.RowType == DataControlRowType.DataRow)
        //    {
        //        CheckBox chkRow = (row.Cells[0].FindControl("chkVisible") as CheckBox);
        //        if (chkRow.Checked)
        //        {
        //            cnt = cnt + 1;
        //        }
        //    }

        //}
        //if (cnt == 0)
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('Select Record To Give Permission');", true);
        //}
        //if (cnt > 0)
        //{

        for (int i = 0; i < gvDetails.Rows.Count; i++)
        {
            gvDetails.SelectedIndex = i;

            HiddenField hdnformid = (HiddenField)gvDetails.SelectedRow.FindControl("hdnformid");
            CheckBox chkVisible = (CheckBox)gvDetails.SelectedRow.FindControl("chkVisible");
            CheckBox chkView = (CheckBox)gvDetails.SelectedRow.FindControl("chkView");
            CheckBox chkSave = (CheckBox)gvDetails.SelectedRow.FindControl("chkSave");
            CheckBox chkEdit = (CheckBox)gvDetails.SelectedRow.FindControl("chkEdit");
            CheckBox chkDelete = (CheckBox)gvDetails.SelectedRow.FindControl("chkDelete");

            HiddenField hdnparentid = (HiddenField)gvDetails.SelectedRow.FindControl("hdnparentid");
            HiddenField hdnFormAuthID = (HiddenField)gvDetails.SelectedRow.FindControl("hdnFormAuthID");
            HiddenField hdnParentForm = (HiddenField)gvDetails.SelectedRow.FindControl("hdnParentForm");

            if (chkVisible.Checked == true)
            {
                conn = new SqlConnection(strConnString);
                conn.Open();
                //  transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                // cmd.Transaction = transactionGL;
                cmd.CommandText = "GL_InsertUpdateAuthorization";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operation", operation);
                cmd.Parameters.AddWithValue("@FormAuthID", hdnFormAuthID.Value);
                cmd.Parameters.AddWithValue("@UserID", ddlUserName.SelectedValue);
                cmd.Parameters.AddWithValue("@FormID", hdnformid.Value);
                cmd.Parameters.AddWithValue("@ParentID", hdnparentid.Value);
                if (chkVisible.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsVisible", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsVisible", 0);
                }
                if (chkView.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsView", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsView", 0);
                }
                if (chkSave.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsSave", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsSave", 0);
                }
                if (chkEdit.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsEdit", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsEdit", 0);
                }
                if (chkDelete.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsDelete", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsDelete", 0);
                }
                cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());

                result = cmd.ExecuteNonQuery();

            }
            else
            {
                conn = new SqlConnection(strConnString);
                conn.Open();
                //  transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                // cmd.Transaction = transactionGL;
                cmd.CommandText = "GL_InsertUpdateAuthorization";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Operation", operation);
                cmd.Parameters.AddWithValue("@FormAuthID", hdnFormAuthID.Value);
                cmd.Parameters.AddWithValue("@UserID", ddlUserName.SelectedValue);
                cmd.Parameters.AddWithValue("@FormID", hdnformid.Value);
                cmd.Parameters.AddWithValue("@ParentID", hdnparentid.Value);
                if (chkVisible.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsVisible", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsVisible", 0);
                }
                if (chkView.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsView", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsView", 0);
                }
                if (chkSave.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsSave", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsSave", 0);
                }
                if (chkEdit.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsEdit", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsEdit", 0);
                }
                if (chkDelete.Checked == true)
                {
                    cmd.Parameters.AddWithValue("@IsDelete", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@IsDelete", 0);
                }
                cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());

                result = cmd.ExecuteNonQuery();
            }
            //  }//for checking count

        }
        //if (result > 0)
        //{
        //    datasaved = true;
        //    transactionGL.Commit();
        //}
        //else
        //{
        //    datasaved = false;
        //    transactionGL.Rollback();
        //}

        //  if (operation == "Save")
        //  {
        if (result > 0)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Status", "alert('Record  saved successfully.')", true);
            hdnOpration.Value = "Save";
            ClearAll();
        }
        // }
        //if (operation == "Update")
        //{
        //    ClientScript.RegisterStartupScript(this.GetType(), " Status", "alert('Record  updated successfully.')", true);
        //    //hdnOpration.Value = "Save";
        //    ClearAll();
        //}

    }

    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        ClearAll();
    }

    public void ClearAll()
    {
        ddlUserType.SelectedIndex = 0;
        if (ddlUserName.SelectedIndex != -1)
        {
            ddlUserName.SelectedIndex = 0;
        }
        else
        {
        }

        if (ddlMasterMenu.SelectedIndex != -1)
        {
            ddlMasterMenu.SelectedIndex = 0;
        }
        else
        {
        }

        gvDetails.Visible = false;
        gvDetails.DataSource = null;
        hdnOpration.Value = "Save";

    }

    protected void chkVisibleHeding_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkAllVisible = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");
        CheckBox chkHView = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");
        CheckBox chkHSave = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        CheckBox chkHEdit = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        CheckBox chkHDelete = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");



        //   CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");

        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkVisible");
        //    if (chkAllVisible.Checked == true)
        //    {
        //        ChkBoxRows.Checked = true;
        //    }
        //    else
        //    {
        //        ChkBoxRows.Checked = false;
        //    }
        //}


        if (chkAllVisible.Checked == true)
        {
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkVisible");
                if (chkAllVisible.Checked == true)
                {
                    ChkBoxRows.Checked = true;
                }
                else
                {
                    ChkBoxRows.Checked = false;
                }
            }
        }
        else
        {

            chkAllVisible.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkView = (CheckBox)row.FindControl("chkVisible");
                chkView.Checked = false;
            }


            chkHView.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkView = (CheckBox)row.FindControl("chkView");
                chkView.Checked = false;
            }

            chkHSave.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkSave = (CheckBox)row.FindControl("chkSave");
                chkSave.Checked = false;
            }

            chkHEdit.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkSave = (CheckBox)row.FindControl("chkEdit");
                chkSave.Checked = false;
            }

            chkHDelete.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkDelete = (CheckBox)row.FindControl("chkDelete");
                chkDelete.Checked = false;
            }
        }


    }

    protected void chkViewHeding_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkAllVisible = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");
        CheckBox chkHView = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");
        // CheckBox chkHSave = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        //CheckBox chkHEdit = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        // CheckBox chkHDelete = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");

        // CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");

        if (chkAllVisible.Checked == true)
        {

            if (chkHView.Checked == true)
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkView = (CheckBox)row.FindControl("chkView");
                    chkView.Checked = true;
                }
            }
            else
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkView = (CheckBox)row.FindControl("chkView");
                    chkView.Checked = false;
                }
            }
        }
        else
        {
            chkHView.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkView = (CheckBox)row.FindControl("chkView");
                chkView.Checked = false;
            }

        }

        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkView");
        //    if (chkHView.Checked == true)
        //    {
        //        ChkBoxRows.Checked = true;
        //    }
        //    else
        //    {
        //        ChkBoxRows.Checked = false;
        //    }
        //}
    }

    protected void chkSaveHeding_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox chkAllVisible = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");
        CheckBox chkHView = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");
        CheckBox chkHSave = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        //CheckBox chkHEdit = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        // CheckBox chkHDelete = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");

        // CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");

        if (chkAllVisible.Checked == true)
        {

            if (chkHSave.Checked == true)
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkSave = (CheckBox)row.FindControl("chkSave");
                    chkSave.Checked = true;
                }
            }
            else
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkSave = (CheckBox)row.FindControl("chkSave");
                    chkSave.Checked = false;
                }
            }
        }
        else
        {
            chkHSave.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkSave = (CheckBox)row.FindControl("chkSave");
                chkSave.Checked = false;
            }
        }

        //CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkSave");
        //    if (ChkBoxHeader.Checked == true)
        //    {
        //        ChkBoxRows.Checked = true;
        //    }
        //    else
        //    {
        //        ChkBoxRows.Checked = false;
        //    }
        //}
    }

    protected void chkEditHeding_CheckedChanged(object sender, EventArgs e)
    {
        CheckBox chkAllVisible = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");
        // CheckBox chkHView = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");
        // CheckBox chkHSave = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        CheckBox chkHEdit = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        // CheckBox chkHDelete = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");

        // CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");

        if (chkAllVisible.Checked == true)
        {

            if (chkHEdit.Checked == true)
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkSave = (CheckBox)row.FindControl("chkEdit");
                    chkSave.Checked = true;
                }
            }
            else
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkSave = (CheckBox)row.FindControl("chkEdit");
                    chkSave.Checked = false;
                }
            }
        }
        else
        {
            chkHEdit.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkSave = (CheckBox)row.FindControl("chkEdit");
                chkSave.Checked = false;
            }
        }



        //CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkEdit");
        //    if (ChkBoxHeader.Checked == true)
        //    {
        //        ChkBoxRows.Checked = true;
        //    }
        //    else
        //    {
        //        ChkBoxRows.Checked = false;
        //    }
        //}
    }

    protected void chkDeleteHeding_CheckedChanged(object sender, EventArgs e)
    {

        CheckBox chkAllVisible = (CheckBox)gvDetails.HeaderRow.FindControl("chkVisibleHeding");
        // CheckBox chkHView = (CheckBox)gvDetails.HeaderRow.FindControl("chkViewHeding");
        // CheckBox chkHSave = (CheckBox)gvDetails.HeaderRow.FindControl("chkSaveHeding");
        //  CheckBox chkHEdit = (CheckBox)gvDetails.HeaderRow.FindControl("chkEditHeding");
        CheckBox chkHDelete = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");


        if (chkAllVisible.Checked == true)
        {

            if (chkHDelete.Checked == true)
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkDelete = (CheckBox)row.FindControl("chkDelete");
                    chkDelete.Checked = true;
                }
            }
            else
            {
                foreach (GridViewRow row in gvDetails.Rows)
                {
                    CheckBox chkDelete = (CheckBox)row.FindControl("chkDelete");
                    chkDelete.Checked = false;
                }
            }
        }
        else
        {
            chkHDelete.Checked = false;
            foreach (GridViewRow row in gvDetails.Rows)
            {
                CheckBox chkDelete = (CheckBox)row.FindControl("chkDelete");
                chkDelete.Checked = false;
            }
        }



        //CheckBox ChkBoxHeader = (CheckBox)gvDetails.HeaderRow.FindControl("chkDeleteHeding");
        //foreach (GridViewRow row in gvDetails.Rows)
        //{
        //    CheckBox ChkBoxRows = (CheckBox)row.FindControl("chkDelete");
        //    if (ChkBoxHeader.Checked == true)
        //    {
        //        ChkBoxRows.Checked = true;
        //    }
        //    else
        //    {
        //        ChkBoxRows.Checked = false;
        //    }
        //}
    }
}

