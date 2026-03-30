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

public partial class ChargesMaster : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    SqlTransaction transaction;
    DataTable dt;
    int FYearID = 0;
    int branchId = 0;
    bool datasaved = false;
    GlobalSettings st = null;
    DataRow dr = null;
    GlobalSettings gbl = new GlobalSettings();
    int result = 0;
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
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }
            Master.PropertybtnSave.OnClientClick = "return validCharge();";
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            if (!IsPostBack)
            {
                //Bind blank grid on page load
                BlankChargesGV();
                //clear all fields
                ClearData();
                //search dropdown fill on popup
                FillddlSearch();
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

    #region [FillddlSearch]
    //adding items to popupddl search field
    public void FillddlSearch()
    {
        Master.PropertyddlSearch.Items.Add("Charge Name");
        Master.PropertyddlSearch.Items.Add("Reference Date");
        Master.PropertyddlSearch.Items.Add("Status");
    }
    #endregion [FillddlSearch]

    #region [ClearData]
    //clear all data
    protected void ClearData()
    {
        hdnid.Value = "0";
        hdnoperation.Value = "Save";
        txtChargeName.Text = "";
        txtRefDate.Text = "";
        ddlStatus.SelectedIndex = 0;
        BlankChargesGV();
    }
    #endregion [ClearData]

    #region [BlankChargesGV]
    //Bind blank grid on page load
    protected void BlankChargesGV()
    {
        dt = new DataTable();
        dt.Columns.Add("ID");
        dt.Columns.Add("LoanAmtFrom");
        dt.Columns.Add("LoanAmtTo");
        dt.Columns.Add("Charges");
        dt.Columns.Add("CID");
        dt.Columns.Add("ChargeType");


        gbl.ShowNoResultFound(dt, dgvChargeDetails);

    }
    #endregion [BlankChargesGV]

    #region [PropertyImgBtnClose_Click]

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {
        Master.PropertympeGlobal.Hide();
        BlankChargesGV();
        gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

    }
    #endregion [PropertyImgBtnClose_Click]

    #region [PropertybtnSave_Click]
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {
                Charge_PRV("Save", "0");
                Charges_PRI("Save", "0");

            }
            if (hdnoperation.Value == "Update")
            {
                Charge_PRV("Update", hdnid.Value.Trim());
                Charges_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {
            // hdnoperation.Value = "";
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
            GoldLoan_RTR();
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

    #region [PropertybtnDelete_Click]
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Charge_PRV("Delete", hdnid.Value.Trim());
            Charges_PRI("Delete", hdnid.Value.Trim());

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
    #endregion [PropertybtnDelete_Click]

    #region [PropertybtnView_Click]
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            GoldLoan_RTR();
            hdnPopup.Value = "View";
            //Scheme_PRV("View", hdnid.Value.Trim());
            //Scheme_RTR(txtSchemeId.Text.Trim());

            //gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel); 
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
            SearchCharges_Details();
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
            GoldLoanCharge_RTR(id);
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

    protected void PropertygvGlobal_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
        }
        catch (Exception ex)
        { }

    }

    #region [AddGridRow]
    //add Charges details in grid
    protected void AddGridRow()
    {
        int count = 1;
        TextBox gvtxtFooterLoanAmtFrom = (TextBox)dgvChargeDetails.FooterRow.FindControl("txtLoanAmtFrom");
        TextBox gvtxtFooterLoanAmtTo = (TextBox)dgvChargeDetails.FooterRow.FindControl("txtLoanAmountTo");
        TextBox gvtxtFooterCharges = (TextBox)dgvChargeDetails.FooterRow.FindControl("txtCharges");
        DropDownList gvtxtFooterChargeType = (DropDownList)dgvChargeDetails.FooterRow.FindControl("ddlChargeType");
        if (gvtxtFooterLoanAmtFrom.Text.Trim() != null && gvtxtFooterLoanAmtFrom.Text.Trim() != "" && gvtxtFooterLoanAmtTo.Text.Trim() != null && gvtxtFooterLoanAmtTo.Text.Trim() != "")
        {
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("LoanAmtFrom");
            dt.Columns.Add("LoanAmtTo");
            dt.Columns.Add("Charges");
            dt.Columns.Add("CID");
            dt.Columns.Add("ChargeType");

            for (int i = 0; i < dgvChargeDetails.Rows.Count; i++)
            {
                dgvChargeDetails.SelectedIndex = i;

                HiddenField gvhdnID = (HiddenField)dgvChargeDetails.SelectedRow.FindControl("hdnID");
                Label gvlblLoanAmtFrom = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountFrom");
                Label gvlblLoanAmtTo = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountTo");
                Label gvlblCharges = (Label)dgvChargeDetails.SelectedRow.FindControl("lblCharges");
                Label gvlblChargeType = (Label)dgvChargeDetails.SelectedRow.FindControl("lblChrgType");

                dr = dt.NewRow();
                dr["ID"] = gvhdnID.Value;
                dr["LoanAmtFrom"] = gvlblLoanAmtFrom.Text;
                dr["LoanAmtTo"] = gvlblLoanAmtTo.Text;
                dr["Charges"] = gvlblCharges.Text;
                dr["ChargeType"] = gvlblChargeType.Text;

                if (gvlblLoanAmtFrom.Text != "" && gvlblLoanAmtTo.Text != "")
                {
                    dt.Rows.Add(dr);
                }
                if ((gvlblLoanAmtFrom.Text != "") && (gvlblLoanAmtTo.Text != ""))
                {
                    double _amtFrm = Convert.ToDouble(gvlblLoanAmtFrom.Text);
                    double _amtTo = Convert.ToDouble(gvlblLoanAmtTo.Text);

                    double _amtFrm1 = Convert.ToDouble(gvtxtFooterLoanAmtFrom.Text);
                    double _amtTo1 = Convert.ToDouble(gvtxtFooterLoanAmtTo.Text);
                    if (((_amtFrm1 >= _amtFrm) && (_amtFrm1 <= _amtTo)) || (_amtFrm1 <= _amtTo))
                    {
                        ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('Entered Amount Already Exist In Previous Range');", true);
                        gvtxtFooterLoanAmtFrom.Text = "";
                        gvtxtFooterLoanAmtTo.Text = "";
                        gvtxtFooterCharges.Text = "";
                        count = 0;
                    }

                }

            }

            if (count > 0)
            {
                dr = dt.NewRow();
                dr["ID"] = "0";
                dr["LoanAmtFrom"] = gvtxtFooterLoanAmtFrom.Text;
                dr["LoanAmtTo"] = gvtxtFooterLoanAmtTo.Text;
                dr["Charges"] = gvtxtFooterCharges.Text;
                dr["ChargeType"] = gvtxtFooterChargeType.SelectedValue;
                dt.Rows.Add(dr);

                dgvChargeDetails.DataSource = dt;
                dgvChargeDetails.DataBind();
            }
        }
    }
    #endregion [AddGridRow]

    #region [Charges_PRI]
    //Insert Charge details
    protected void Charges_PRI(string operation, string value)
    {

        conn = new SqlConnection(strConnString);
        conn.Open();
        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

        for (int i = 0; i < dgvChargeDetails.Rows.Count; i++)
        {
            dgvChargeDetails.SelectedIndex = i;
            Label gvSrNo = (Label)dgvChargeDetails.SelectedRow.FindControl("lblSrNo");
            HiddenField gvhdnID = (HiddenField)dgvChargeDetails.SelectedRow.FindControl("hdnID");
            Label gvlblLoanAmtFrom = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountFrom");
            Label gvlblLoanAmtTo = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountTo");
            Label gvlblCharges = (Label)dgvChargeDetails.SelectedRow.FindControl("lblCharges");
            Label gvlblChargeType = (Label)dgvChargeDetails.SelectedRow.FindControl("lblChrgType");
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = transaction;
            cmd.CommandText = "GL_Charge_PRI";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Operation", operation);
            cmd.Parameters.AddWithValue("@CID", value);
            cmd.Parameters.AddWithValue("@ChargeName", txtChargeName.Text);
            cmd.Parameters.AddWithValue("@ReferenceDate", Convert.ToDateTime(txtRefDate.Text));
            cmd.Parameters.AddWithValue("@Status", ddlStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
            cmd.Parameters.AddWithValue("@BranchID", Session["branchId"].ToString());
            cmd.Parameters.AddWithValue("@CmpID", "1");
            cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());
            cmd.Parameters.AddWithValue("@ID", gvhdnID.Value);
            cmd.Parameters.AddWithValue("@LoanAmtFrom", gvlblLoanAmtFrom.Text);
            cmd.Parameters.AddWithValue("@LoanAmtTo", gvlblLoanAmtTo.Text);
            cmd.Parameters.AddWithValue("@Charges", gvlblCharges.Text);
            cmd.Parameters.AddWithValue("@ChargeType", gvlblChargeType.Text);
            cmd.Parameters.AddWithValue("@Lineno", i);
            result = cmd.ExecuteNonQuery();
        }

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
        if (operation == "Save" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ChargeMaster", "alert('Record Saved Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        if (operation == "Update" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ChargeMaster", "alert('Record Updated Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        if (operation == "Delete" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "ChargeMaster", "alert('Record Deleted Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
    }
    #endregion [Charges_PRI]

    #region [GoldLoan_RTR]
    //fetching records on popup menu
    public void GoldLoan_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_ChargesPopUp_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BRANCHID", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    #endregion [GoldLoan_RTR]

    #region [GoldLoanCharge_RTR]
    //retrive charges details for Edit
    public void GoldLoanCharge_RTR(string id)
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Charge_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CID", id);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            hdnid.Value = dt.Rows[0]["CID"].ToString();
            txtChargeName.Text = dt.Rows[0]["ChargeName"].ToString();
            txtRefDate.Text = dt.Rows[0]["ReferenceDate"].ToString();
            ddlStatus.SelectedValue = dt.Rows[0]["Status"].ToString().Trim();
            dgvChargeDetails.DataSource = dt;
            dgvChargeDetails.DataBind();
            hdnoperation.Value = "Update";
        }

    }
    #endregion [GoldLoanCharge_RTR]

    #region [SearchCharges_Details]
    //search charges on popup menu
    protected void SearchCharges_Details()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Charges_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@SearchType", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchValue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    #endregion [SearchCharges_Details]

    #region [Charge_PRV]
    //validate if charges is used in S&D against Save,Update and Delete
    public void Charge_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_Charge_PRV";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@CID", value);
        cmd.Parameters.AddWithValue("@ChargeName", txtChargeName.Text.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();


    }
    #endregion [Charge_PRV]

    #region [BtnAdd_Click]
    //add new record in grid
    protected void BtnAdd_Click(object sender, EventArgs e)
    {
        try
        {
            AddGridRow();
        }
        catch (Exception ex)
        {

        }
    }
    #endregion [BtnAdd_Click]

    #region [btnDelete_Click]
    //delete row from gridview
    protected void btnDelete_Click(object sender, ImageClickEventArgs e)
    {
        try
        {

            if (dgvChargeDetails.Rows.Count == 1)
            {
                BlankChargesGV();
                return;
            }
            ImageButton ImgBtnRemove = (ImageButton)sender;
            GridViewRow row = (GridViewRow)ImgBtnRemove.NamingContainer;
            int index = row.RowIndex;
            dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("LoanAmtFrom");
            dt.Columns.Add("LoanAmtTo");
            dt.Columns.Add("Charges");
            dt.Columns.Add("CID");
            dt.Columns.Add("ChargeType");

            for (int i = 0; i < dgvChargeDetails.Rows.Count; i++)
            {
                dgvChargeDetails.SelectedIndex = i;


                HiddenField gvhdnID = (HiddenField)dgvChargeDetails.SelectedRow.FindControl("hdnID");
                Label gvlblLoanAmtFrom = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountFrom");
                Label gvlblLoanAmtTo = (Label)dgvChargeDetails.SelectedRow.FindControl("lblLoanAmountTo");
                Label gvlblCharges = (Label)dgvChargeDetails.SelectedRow.FindControl("lblCharges");
                Label gvlblChargeType = (Label)dgvChargeDetails.SelectedRow.FindControl("lblChrgType");
                if (i != index)
                {
                    dr = dt.NewRow();
                    dr["ID"] = gvhdnID.Value;
                    dr["LoanAmtFrom"] = gvlblLoanAmtFrom.Text;
                    dr["LoanAmtTo"] = gvlblLoanAmtTo.Text;
                    dr["Charges"] = gvlblCharges.Text;
                    dr["ChargeType"] = gvlblChargeType.Text;
                    dt.Rows.Add(dr);
                }

            }


            dgvChargeDetails.DataSource = dt;
            dgvChargeDetails.DataBind();
        }

        catch (Exception ex)
        { }
    }
    #endregion [btnDelete_Click]
}