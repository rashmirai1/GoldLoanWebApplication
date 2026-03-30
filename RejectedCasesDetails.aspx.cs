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

public partial class RejectedCasesDetails : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
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
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
    }

    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnView.Visible = false;
        // Master.PropertybtnSearch.Visible = false;
        Master.PropertybtnCancel.Visible = false;

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnView.Visible = false;
            Master.PropertybtnCancel.Visible = false;
            gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            if (!IsPostBack)
            {
                BindFinancialYear();

            }
            btnPrint.Enabled = false;
            btnPrint.CssClass = "btnenable";

            //Disable Panel to view grid.
            pnlRejcase.Visible = false;

        }
        catch (Exception ex)
        {
        }
    }
    protected void CleareData()
    {

        lbltotal.Text = "";
        lblInterview.Text = "";
        lblfci.Text = "";
        lblCIBIL.Text = "";
        ddlFYear.SelectedIndex = 0;


    }

    protected void BindFinancialYear()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_RejectedCase_FYear_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        DataTable dt = new DataTable();
        da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            ddlFYear.DataSource = dt;
            ddlFYear.DataTextField = "Financialyear";
            ddlFYear.DataValueField = "FinancialyearID";
            ddlFYear.DataBind();
            ddlFYear.Items.Insert(0, new ListItem("--Select Financial Year--", "0"));
        }


    }

    protected void btnShow_Click(object sender, EventArgs e)
    {
        try
        {
            //Clear Grid. and summary labels.            



            conn = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "GL_RejectedCase_RejectedStatus_RTR";
            cmd.Parameters.AddWithValue("@FinancialyearID", ddlFYear.SelectedValue);

            cmd.CommandType = CommandType.StoredProcedure;
            DataTable dt = new DataTable();
            da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                gvRejectedCases.DataSource = dt;
                gvRejectedCases.DataBind();
            }
            else
            {
                gvRejectedCases.DataSource = null;
                gvRejectedCases.DataBind();
                gvRejectedCases.Columns.Clear();
            }
            CountFCS();
            CountCIBIL();
            CountInterview();

            lbltotal.Text = (Convert.ToInt16(lblfci.Text) + Convert.ToInt16(lblCIBIL.Text) + Convert.ToInt16(lblInterview.Text)).ToString();

            btnPrint.Enabled = true;
            btnPrint.CssClass = "css_btn_class";

            //Enable Panel to view grid.
            pnlRejcase.Visible = true;

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void CountFCS()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_RejectedCase_CountFCS_RTR";
        cmd.Parameters.AddWithValue("@FinancialyearID", ddlFYear.SelectedValue);

        cmd.CommandType = CommandType.StoredProcedure;
        DataTable dt = new DataTable();
        da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        lblfci.Text = dt.Rows[0][0].ToString();
        Session["fci"] = lblfci.Text.ToString();
    }

    protected void CountCIBIL()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_RejectedCase_CountCIBIL_RTR";
        cmd.Parameters.AddWithValue("@FinancialyearID", ddlFYear.SelectedValue);

        cmd.CommandType = CommandType.StoredProcedure;
        DataTable dt = new DataTable();
        da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        lblCIBIL.Text = dt.Rows[0][0].ToString();
        Session["CIBIL"] = lblCIBIL.Text.ToString();
    }

    protected void CountInterview()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_RejectedCase_CountInterview_RTR";
        cmd.Parameters.AddWithValue("@FinancialyearID", ddlFYear.SelectedValue);

        cmd.CommandType = CommandType.StoredProcedure;
        DataTable dt = new DataTable();
        da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        lblInterview.Text = dt.Rows[0][0].ToString();
        Session["Interview"] = lblInterview.Text.ToString();
    }


    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {

            CleareData();
            btnPrint.Enabled = false;
            btnPrint.CssClass = "btnenable";
            gbl.CheckAEDControlSettings("View", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }


    protected void ddlFYear_SelectedIndexChanged(object sender, EventArgs e)
    {

        //Clear Grid. and summary labels
        lbltotal.Text = "";
        lblInterview.Text = "";
        lblfci.Text = "";
        lblCIBIL.Text = "";



    }
    protected void gvRejectedCases_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        try
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;
        }
        catch (Exception ex)
        { }
    }
}