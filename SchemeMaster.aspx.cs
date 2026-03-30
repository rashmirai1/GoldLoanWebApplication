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

public partial class SchemeMaster : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;

    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    DataTable dt;
    int FYearID = 0;
    int branchId = 0;
    GlobalSettings st = null;
    DataRow dr = null;
    GlobalSettings gbl = new GlobalSettings();
    int result = 0;
    #endregion [Declarations]


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
        // Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }
            Master.PropertybtnSave.OnClientClick = "return validrecord();";


            if (!IsPostBack)
            {
                AutoGeneraeId();
                FillddlSearch();
                BlankGrdBind();
                //gvROI.Enabled = false;
                txtSchemeId.Attributes.Add("readonly", "readonly");
                //txtPerAmtLimit.Attributes.Add("disabled", "true");
                gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
                PnlGv.Visible = true;
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    public void AutoGeneraeId()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand("select isnull(MAX(SId),0)+1 SId from TSchemeMaster_BasicDetails", conn);

        da = new SqlDataAdapter(cmd);


        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtSchemeId.Text = dt.Rows[0][0].ToString();
        }
    }
    public void FillddlSearch()
    {
        Master.PropertyddlSearch.Items.Add("Scheme Name");
        Master.PropertyddlSearch.Items.Add("Calculation Method");
        Master.PropertyddlSearch.Items.Add("Processing Charge Type");
    }
    public void BlankGrdBind()
    {

        dt = new DataTable();
        dt.Columns.Add("ROIID");
        dt.Columns.Add("NoofDefaultMonths");
        dt.Columns.Add("EffROI");

        st = new GlobalSettings();
        st.ShowNoResultFound(dt, gvROI);

    }

    #endregion [Page_Load]

    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {
        try
        {
            Master.PropertympeGlobal.Hide();
            BlankGrdBind();
            gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {
                Scheme_PRV("Save", "0");
                Scheme_PRI("Save", "0");


            }
            if (hdnoperation.Value == "Update")
            {
                Scheme_PRV("Update", hdnid.Value.Trim());
                Scheme_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
        //finally
        //{
        //    if (conn.State == ConnectionState.Open)
        //    {
        //        conn.Close();
        //    }
        //}


    }
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            PopUp_RTR();
            hdnPopup.Value = "Edit";

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
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Scheme_PRV("Delete", hdnid.Value.Trim());
            Scheme_PRI("Delete", hdnid.Value.Trim());


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
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            //Scheme_PRV("View", hdnid.Value.Trim());
            //Scheme_RTR(txtSchemeId.Text.Trim());
            PopUp_RTR();
            hdnPopup.Value = "View";
            //gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel); 
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            Response.Redirect("SchemeMaster.aspx");
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            Scheme_Search();
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
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
            Scheme_RTR(id);
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
    protected void btnFooterAdd_Click(object sender, EventArgs e)
    {
        try
        {
            AddNewRow();
        }
        catch (Exception ex)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    public void ClearData()
    {
        hdnid.Value = "0";
        hdnoperation.Value = "Save";
        txtSchemeId.Text = "";
        txtSchemeName.Text = "";
        ddlSchemType.SelectedIndex = 0;
        txtMinLoan.Text = "";
        txtTenure.Text = "";
        txtMaxLoan.Text = "";
        txtLtv.Text = "";
        ddlCalculation.SelectedIndex = 0;
        txtROI.Text = "";
        ddlProChareType.SelectedIndex = 0;
        txtEMI.Text = "";
        txtProcessingCharges.Text = "";
        txtPanelInterest.Text = "";
        txtROI.Enabled = true;
        txtEMI.Enabled = true;
        txtPanelInterest.Enabled = true;
        txtPerAmtLimit.Text = "";
        txtServiceTax.Text = "";
        AutoGeneraeId();
        // gvROI.Enabled = false;
        BlankGrdBind();

    }
    public void Scheme_Search()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Scheme_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@SearchType", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchValue", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    public void Scheme_RTR(string id)
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_SchemeDetails_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@SID", id);

        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            hdnid.Value = dt.Rows[0]["SID"].ToString();
            txtSchemeId.Text = dt.Rows[0]["SID"].ToString();
            txtSchemeName.Text = dt.Rows[0]["SchemeName"].ToString();
            ddlSchemType.SelectedValue = dt.Rows[0]["SchemeType"].ToString().Trim();
            txtMinLoan.Text = dt.Rows[0]["MinLoanAmt"].ToString();
            txtTenure.Text = dt.Rows[0]["Tenure"].ToString();
            txtMaxLoan.Text = dt.Rows[0]["MaxLoanAmt"].ToString();
            txtLtv.Text = dt.Rows[0]["Ltv"].ToString();
            ddlCalculation.SelectedValue = dt.Rows[0]["CalMethod"].ToString();
            txtROI.Text = dt.Rows[0]["ROI"].ToString();
            ddlProChareType.SelectedValue = dt.Rows[0]["ProChargeType"].ToString().Trim();
            txtEMI.Text = dt.Rows[0]["EMI"].ToString();
            txtPanelInterest.Text = dt.Rows[0]["PenalInterest"].ToString();
            txtProcessingCharges.Text = dt.Rows[0]["ProCharge"].ToString();
            txtServiceTax.Text = dt.Rows[0]["ServiceTax"].ToString();
            txtPerAmtLimit.Text = dt.Rows[0]["AmtLmtTo"].ToString();

            gvROI.DataSource = dt;
            gvROI.DataBind();
            DDLSchemetypeChange();
            //if (ddlSchemType.SelectedIndex == 0)
            //{
            //    txtROI.Enabled = true;
            //    txtEMI.Enabled = true;
            //    txtPanelInterest.Enabled = true;

            //}
            //else
            //{
            //    txtROI.Text = "";
            //    txtEMI.Text = "";
            //    txtPanelInterest.Text = "";
            //    txtROI.Enabled = false;
            //    txtEMI.Enabled = false;
            //    txtPanelInterest.Enabled = false;

            //}

        }
    }
    public void PopUp_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_Scheme_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();


    }
    public void Scheme_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_Scheme_PRV";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@SID", value);
        cmd.Parameters.AddWithValue("@SchemeName", txtSchemeName.Text.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();


    }
    public void Scheme_PRI(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        for (int i = 0; i < gvROI.Rows.Count; i++)
        {
            gvROI.SelectedIndex = i;


            HiddenField gvhdnEffRowId = (HiddenField)gvROI.SelectedRow.FindControl("gvhdnEffRowId");
            Label gvlblNoofMonths = (Label)gvROI.SelectedRow.FindControl("gvlblNoofMonths");
            Label gvlblROI = (Label)gvROI.SelectedRow.FindControl("gvlblROI");
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "GL_Scheme_PRI";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Operation", operation);
            cmd.Parameters.AddWithValue("@SID", value);
            cmd.Parameters.AddWithValue("@SchemeName", txtSchemeName.Text.Trim());
            cmd.Parameters.AddWithValue("@SchemeType", ddlSchemType.SelectedValue.Trim());
            if (txtMinLoan.Text != "")
            {
                cmd.Parameters.AddWithValue("@MinLoanAmt", txtMinLoan.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@MinLoanAmt", txtMinLoan.Text.Trim());
            }
            if (txtTenure.Text == "")
            {
                cmd.Parameters.AddWithValue("@Tenure", txtTenure.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@Tenure", txtTenure.Text.Trim());

            }
            if (txtMaxLoan.Text != "")
            {
                cmd.Parameters.AddWithValue("@MaxLoanAmt", txtMaxLoan.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@MaxLoanAmt", txtMaxLoan.Text.Trim());
            }
            if (txtLtv.Text != "")
            {
                cmd.Parameters.AddWithValue("@Ltv", txtLtv.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@Ltv", txtLtv.Text.Trim());
            }

            cmd.Parameters.AddWithValue("@CalMethod", ddlCalculation.SelectedValue.Trim());
            if (txtROI.Text != "")
            {
                cmd.Parameters.AddWithValue("@ROI", txtROI.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@ROI", "0");
            }
            cmd.Parameters.AddWithValue("@ProChargeType", ddlProChareType.SelectedValue.Trim());
            if (txtEMI.Text != "")
            {
                cmd.Parameters.AddWithValue("@EMI", txtEMI.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@EMI", "0");
            }
            if (txtProcessingCharges.Text != "")
            {
                cmd.Parameters.AddWithValue("@ProCharge", txtProcessingCharges.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@ProCharge", "0");
            }
            if (txtServiceTax.Text != "")
            {
                cmd.Parameters.AddWithValue("@ServiceTax", txtServiceTax.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@ServiceTax", "0");
            }
            if (txtPanelInterest.Text != "")
            {
                cmd.Parameters.AddWithValue("@PenalInterest", txtPanelInterest.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@PenalInterest", "0");
            }
            if (txtPerAmtLimit.Text != "")
            {
                cmd.Parameters.AddWithValue("@AmtLmtTo", txtPerAmtLimit.Text.Trim());
            }
            else
            {

                cmd.Parameters.AddWithValue("@AmtLmtTo", "0");
            }

            cmd.Parameters.AddWithValue("@CMPId", "1");
            cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
            cmd.Parameters.AddWithValue("@FYId", Session["FYearID"].ToString());
            cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());
            //cmd.Parameters.AddWithValue("@isActive", 'Y');
            cmd.Parameters.AddWithValue("@Lineno", i);
            cmd.Parameters.AddWithValue("@ROIID", gvhdnEffRowId.Value);
            if (gvlblNoofMonths.Text != "")
            {
                cmd.Parameters.AddWithValue("@NoofDefaultMonths", gvlblNoofMonths.Text);
            }
            else
            {
                cmd.Parameters.AddWithValue("@NoofDefaultMonths", "0");
            }
            if (gvlblROI.Text != "")
            {
                cmd.Parameters.AddWithValue("@EffROI", gvlblROI.Text.Trim());
            }
            else
            {
                cmd.Parameters.AddWithValue("@EffROI", "0");
            }

            result = cmd.ExecuteNonQuery();


        }
        conn.Close();
        if (result > 0)
        {
            if (operation == "Save" && result > 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SchemeMaster", "alert('Record Saved Successfully');", true);
                ClearData();
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

            }
            if (operation == "Update" && result > 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SchemeMaster", "alert('Record Updated Successfully');", true);
                ClearData();
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

            }
            if (operation == "Delete" && result > 0)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SchemeMaster", "alert('Record Deleted Successfully');", true);
                ClearData();
                gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

            }
        }
        else if (operation == "Delete" && result <= 0)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "SchemeMaster", "alert('Do not Delete this record. Because this Scheme is Already use');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
    }
    public void AddNewRow()
    {
        TextBox gvtxtFooterDefaultMonth = (TextBox)gvROI.FooterRow.FindControl("gvtxtFooterDefaultMonth");
        TextBox gvtxtFooterRoi = (TextBox)gvROI.FooterRow.FindControl("gvtxtFooterRoi");
        if (gvtxtFooterDefaultMonth.Text.Trim() != null && gvtxtFooterDefaultMonth.Text.Trim() != "" && gvtxtFooterRoi.Text.Trim() != null && gvtxtFooterRoi.Text.Trim() != "")
        {
            dt = new DataTable();
            dt.Columns.Add("ROIID");
            dt.Columns.Add("NoofDefaultMonths");
            dt.Columns.Add("EffROI");

            for (int i = 0; i < gvROI.Rows.Count; i++)
            {
                gvROI.SelectedIndex = i;


                HiddenField gvhdnEffRowId = (HiddenField)gvROI.SelectedRow.FindControl("gvhdnEffRowId");
                Label gvlblNoofMonths = (Label)gvROI.SelectedRow.FindControl("gvlblNoofMonths");
                Label gvlblROI = (Label)gvROI.SelectedRow.FindControl("gvlblROI");

                dr = dt.NewRow();
                dr["ROIID"] = gvhdnEffRowId.Value;
                dr["NoofDefaultMonths"] = gvlblNoofMonths.Text;
                dr["EffROI"] = gvlblROI.Text;

                if (gvlblNoofMonths.Text != "" && gvlblROI.Text != "")
                {
                    dt.Rows.Add(dr);
                }
            }

            dr = dt.NewRow();
            dr["ROIID"] = "0";
            dr["NoofDefaultMonths"] = gvtxtFooterDefaultMonth.Text;
            dr["EffROI"] = gvtxtFooterRoi.Text;
            dt.Rows.Add(dr);

            gvROI.DataSource = dt;
            gvROI.DataBind();
        }
    }

    public void emicalculation()
    {
        Double EMI = 0;
        Double loanamount = Convert.ToDouble(string.IsNullOrEmpty(txtMaxLoan.Text) ? "0" : txtMaxLoan.Text);
        Double tenure = Convert.ToDouble(string.IsNullOrEmpty(txtTenure.Text) ? "0" : txtTenure.Text);
        Double roi = Convert.ToDouble(string.IsNullOrEmpty(txtROI.Text) ? "0" : txtROI.Text);

        //EMI = (((loanamount * (InterestRate / 100)) / 12) * tenure) + loanamount);

        EMI = ((((loanamount * (roi / 100)) / 12) * tenure) + loanamount) / tenure;

        txtEMI.Text = Convert.ToString(Math.Round(EMI, 0));
    }


    protected void ImgBtnRemove_Click(object sender, ImageClickEventArgs e)
    {

        try
        {
            if (gvROI.Rows.Count == 1)
            {
                BlankGrdBind();
                return;
            }
            ImageButton ImgBtnRemove = (ImageButton)sender;
            GridViewRow row = (GridViewRow)ImgBtnRemove.NamingContainer;
            int index = row.RowIndex;

            dt = new DataTable();
            dt.Columns.Add("ROIID");
            dt.Columns.Add("NoofDefaultMonths");
            dt.Columns.Add("EffROI");

            for (int i = 0; i < gvROI.Rows.Count; i++)
            {
                gvROI.SelectedIndex = i;


                HiddenField gvhdnEffRowId = (HiddenField)gvROI.SelectedRow.FindControl("gvhdnEffRowId");
                Label gvlblNoofMonths = (Label)gvROI.SelectedRow.FindControl("gvlblNoofMonths");
                Label gvlblROI = (Label)gvROI.SelectedRow.FindControl("gvlblROI");

                if (i != index)
                {
                    dr = dt.NewRow();
                    dr["ROIID"] = gvhdnEffRowId.Value;
                    dr["NoofDefaultMonths"] = gvlblNoofMonths.Text;
                    dr["EffROI"] = gvlblROI.Text;
                    dt.Rows.Add(dr);
                }

            }
            gvROI.DataSource = dt;
            gvROI.DataBind();

        }
        catch (Exception ex)
        {
            // hdnoperation.Value = "";
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }


    }

    protected void DDLSchemetypeChange()
    {
        if (ddlSchemType.SelectedValue == "MonthlyEMI")
        {
            gvROI.Enabled = false;
            txtROI.Enabled = true;
            //txtEMI.Enabled = true;
            txtEMI.Enabled = false;
            txtPanelInterest.Enabled = true;
            BlankGrdBind();
        }
        else
        {
            gvROI.Enabled = true;
            txtROI.Enabled = false;
            txtEMI.Enabled = false;
            txtPanelInterest.Enabled = false;
            txtROI.Text = "";
            txtEMI.Text = "";
            txtPanelInterest.Text = "";
        }
    }
    protected void ddlSchemType_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            //DDLSchemetypeChange();

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "SchemeType", "alert('" + ex.Message + "');", true);
        }
    }
    protected void btnScheme_Click(object sender, EventArgs e)
    {
        if (ddlSchemType.SelectedIndex == 1)
        {
            BlankGrdBind();
            gvROI.Enabled = false;
            txtROI.Enabled = true;
            txtEMI.Enabled = false;
            txtPanelInterest.Enabled = true;
            txtMinLoan.Enabled = false;
        }
        else
        {
            BlankGrdBind();
            gvROI.Enabled = true;
            txtROI.Enabled = false;
            txtEMI.Enabled = false;
            txtPanelInterest.Enabled = false;
            txtMinLoan.Enabled = true;
        }
    }


    protected void txtMaxLoan_TextChanged(object sender, EventArgs e)
    {
        if (ddlSchemType.SelectedIndex == 1)
        {
            //int lam = Convert.ToInt32(txtMaxLoan.Text);
        } emicalculation();
    }


    protected void txtTenure_TextChanged(object sender, EventArgs e)
    {

        if (ddlSchemType.SelectedIndex == 1)
        {
            //int lam = Convert.ToInt32(txtTenure.Text);

            emicalculation();
        }
    }

    protected void txtROI_TextChanged(object sender, EventArgs e)
    {
        if (ddlSchemType.SelectedIndex == 1)
        {
            //int lam = Convert.ToInt32(txtROI);

            emicalculation();
        }
    }




}