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
using System.Web.Routing;

public partial class GLCashInOutForm : System.Web.UI.Page
{

    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;


    GlobalSettings gbl = new GlobalSettings();

    bool datasaved = false;
    //Declaring Variables.
    int result = 0;

    string RefNo = "";
    string strQuery = "";
    string Reftype = "";
    string ReferenceType = "";
    string fname = "";
    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    DataTable dt;
    DataTable dt1;
    SqlCommand cmd;
    string time;

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
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);

    }
    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }
            else
            {
                hdnuserid.Value = Session["userID"].ToString();
                hdnfyid.Value = Session["FYearID"].ToString();
                hdnbranchid.Value = Session["branchId"].ToString();

            }
            Master.PropertybtnSave.OnClientClick = "return validrecord();";
            if (!IsPostBack)
            {
                BindDenominationDetails();
                AutogenerateRefNo();
                bindEmp();


                txtreferenceno.Attributes.Add("readonly", "readonly");
                txtdate.Attributes.Add("readonly", "readonly");
                DateTime dte1 = DateTime.Now;
                //date.ToString("dd-MM-yyyy");
                //string date = dte1.ToString("dd/MM/yyyy");

                string date = dte1.ToString("dd/MM/yyyy HH:mm:ss");

                //string s = Convert.ToString(DateTime.Now);
                txtdate.Text = date;

                DateTime dte = DateTime.Now;
                time = dte.ToString("HH:mm");
                hdntime.Value = time;
                gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
                //FillddlSearch();
                Master.PropertytxtSearch.Text = "";
                Master.PropertyddlSearch.Items.Add("Inward/Outward Mode");
                Master.PropertyddlSearch.Items.Add("Reference No");
                Master.PropertyddlSearch.Items.Add("Date");

            }

            if (hdnoperation.Value == "Save")
            {
                ddlInOutto.Enabled = false;
                ddlInOuttoName.Enabled = false;
                panel2.Enabled = false;
                ddlInOutfrom.Enabled = false;
                ddlInOutfromName.Enabled = false;
                ddlInOutBy.Enabled = false;
            }

            if (hdnoperation.Value == "Update")
            {
                ddlInOutto.Enabled = true;
                ddlInOuttoName.Enabled = true;
                panel2.Enabled = true;
                ddlInOutfrom.Enabled = true;
                ddlInOutfromName.Enabled = true;
                ddlInOutBy.Enabled = true;
            }
            ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
            //ddlInOutfromName.Items.Insert(0, new ListItem("---Select Inward/Outward from Name---"));



        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }


    }
    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    public void bindEmp()
    {
        try
        {
            connAIM = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster where status='Active'";
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            ddlInOutBy.DataSource = dt;
            ddlInOutBy.DataTextField = "EmpName";
            ddlInOutBy.DataValueField = "EmployeeID";
            ddlInOutBy.DataBind();
            ddlInOutBy.Items.Insert(0, new ListItem("--Select Inward/Outward By--", "0"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Fillemployee", "alert('" + ex.Message + "');", true);
        }
    }

    public void AutogenerateRefNo()
    {
        Reftype = "CIO";
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand("select isnull(max(RefNo),0)+1 from TGLcashInOutDetails  where RefType ='CIO'", conn);

        da = new SqlDataAdapter(cmd);


        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            hdnrefno.Value = dt.Rows[0][0].ToString();
            txtreferenceno.Text = Reftype + '/' + dt.Rows[0][0].ToString();
        }
    }
    public void BindDenominationDetails()
    {

        dt = new DataTable();
        dt.Columns.Add("DenoId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("DenoRs");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Total");
        dt.Columns.Add("NoteNos");


        dt.Rows.Add("0", "1", "0", "0", "0", "");
        gvDenominationDetails.DataSource = dt;
        gvDenominationDetails.DataBind();


    }
    protected void btnDenoAdd_Click(object sender, EventArgs e)
    {
        try
        {
            ddlInOutfrom.Enabled = true;
            AddDenomination();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }

    public void AddDenomination()
    {
        int tot = 0;
        DataRow dr = null;
        dt = new DataTable();
        dt.Columns.Add("DenoId");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("DenoRs");
        dt.Columns.Add("Quantity");
        dt.Columns.Add("Total");
        dt.Columns.Add("NoteNos");

        for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
        {
            gvDenominationDetails.SelectedIndex = i;
            HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
            TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
            TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
            TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
            TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
            TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");

            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            dr = dt.NewRow();
            dr["DenoId"] = hdndenoid.Value;
            dr["Serialno"] = gvtxtDenoSrno.Text;
            dr["DenoRs"] = gvtxtDenoDescription.Text;
            dr["Quantity"] = gvtxtDenoNo.Text;
            dr["Total"] = gvtxtDenoTotal.Text;
            dr["NoteNos"] = gvtxtDenoNoteNos.Text;

            tot = tot + Convert.ToInt32(gvtxtDenoTotal.Text);

            dt.Rows.Add(dr);

        }
        dr = dt.NewRow();
        dr["DenoId"] = "0";
        dr["Serialno"] = gvDenominationDetails.Rows.Count + 1;
        dr["DenoRs"] = "0";
        dr["Quantity"] = "0";
        dr["Total"] = "0";
        dr["NoteNos"] = "";


        dt.Rows.Add(dr);



        gvDenominationDetails.DataSource = dt;
        gvDenominationDetails.DataBind();
        TextBox gvtxtDenoTotalAmt1 = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
        gvtxtDenoTotalAmt1.Text = "" + tot;

        gvDenominationDetails.Enabled = true;
        panel2.Enabled = true;


    }
    protected void btnDenoDelete_Click(object sender, ImageClickEventArgs e)
    {

        try
        {
            decimal tot = 0;
            gvDenominationDetails.Enabled = true;
            panel2.Enabled = true;
            if (gvDenominationDetails.Rows.Count == 1)
            {
                BindDenominationDetails();
                return;
            }
            ImageButton btnDenoDelete = (ImageButton)sender;
            GridViewRow row = (GridViewRow)btnDenoDelete.NamingContainer;
            int index = row.RowIndex;


            DataRow dr = null;
            dt = new DataTable();
            dt.Columns.Add("DenoId");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("DenoRs");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Total");
            dt.Columns.Add("NoteNos");


            for (int i = 0; i < gvDenominationDetails.Rows.Count; i++)
            {
                gvDenominationDetails.SelectedIndex = i;

                if (i != index)
                {
                    HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
                    TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
                    TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
                    TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
                    TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
                    TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");

                    dr = dt.NewRow();
                    dr["DenoId"] = hdndenoid.Value.Trim();
                    dr["Serialno"] = gvtxtDenoSrno.Text;
                    dr["DenoRs"] = gvtxtDenoDescription.Text;
                    dr["Quantity"] = gvtxtDenoNo.Text;
                    dr["Total"] = gvtxtDenoTotal.Text;
                    dr["NoteNos"] = gvtxtDenoNoteNos.Text;

                    tot = tot + Convert.ToDecimal(gvtxtDenoTotal.Text);

                    dt.Rows.Add(dr);
                }
            }


            gvDenominationDetails.DataSource = dt;
            gvDenominationDetails.DataBind();
            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            gvtxtDenoTotalAmt.Text = "" + tot;


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DenominationaAlert", "alert('" + ex.Message + "');", true);
        }
    }

    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            Popup_RTR();
            hdnpopup.Value = "Edit";
            Master.PropertytxtSearch.Text = "";
            FillddlSearch();



        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }

    public void Popup_RTR()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashINOut_RTR";
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

    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {

                GL_CashInOut_PRV("Save", "0");
                GL_CashInOut_PRI("Save", "0");



            }
            if (hdnoperation.Value == "Update")
            {
                GL_CashInOut_PRV("Update", "0");
                GL_CashInOut_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
        }

    }

    public void GL_CashInOut_PRI(string operation, string value)
    {
        datasaved = false;

        Reftype = "CIO";
        ReferenceType = Reftype + txtreferenceno.Text;
        fname = "C";
        conn = new SqlConnection(strConnString);
        conn.Open();

        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

        for (int j = 0; j < gvDenominationDetails.Rows.Count; j++)
        {

            gvDenominationDetails.SelectedIndex = j;
            HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
            TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
            TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
            TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
            TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
            TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");

            if (gvtxtDenoDescription.Text != "0" && gvtxtDenoNo.Text != "0" && gvtxtDenoTotal.Text != "0")
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GL_CashInOut_PRI";
                cmd.Parameters.AddWithValue("@operation", operation);
                cmd.Parameters.AddWithValue("@InOutID", value);
                cmd.Parameters.AddWithValue("@RefType", Reftype);
                cmd.Parameters.AddWithValue("@RefNo", DBNull.Value);
                cmd.Parameters.AddWithValue("@ReferenceType", DBNull.Value);
                cmd.Parameters.AddWithValue("@FName", fname);

                cmd.Parameters.AddWithValue("@Date_time", gbl.ChangeDateMMddyyyy(txtdate.Text));
                cmd.Parameters.AddWithValue("@time", hdntime.Value);
                cmd.Parameters.AddWithValue("@InOutMode", ddlInOutMode.SelectedValue);
                cmd.Parameters.AddWithValue("@InOutTo", ddlInOutto.SelectedValue);
                if (ddlInOutto.SelectedValue == "0")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('Select Inward/Outward To');", true);
                }





                cmd.Parameters.AddWithValue("@InOutToID", ddlInOuttoName.SelectedValue);
                cmd.Parameters.AddWithValue("@InOutFrom", ddlInOutfrom.SelectedValue);
                if (ddlInOutfrom.SelectedValue == "Cashier")
                {
                    cmd.Parameters.AddWithValue("@InOutFromID", "0");
                }
                else
                {

                    // cmd.Parameters.AddWithValue("@InOutFromID", ddlInOutfromName.SelectedValue);
                    cmd.Parameters.AddWithValue("@InOutFromID", 0);
                }

                cmd.Parameters.AddWithValue("@InOutBy", ddlInOutBy.SelectedValue);
                cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                cmd.Parameters.AddWithValue("@CmpID", "1");
                cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
                cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);


                cmd.Parameters.AddWithValue("@LineNo", j);

                cmd.Parameters.AddWithValue("@DenoId", hdndenoid.Value);
                cmd.Parameters.AddWithValue("@Serialno", gvtxtDenoSrno.Text);
                cmd.Parameters.AddWithValue("@DenoRs", gvtxtDenoDescription.Text);
                cmd.Parameters.AddWithValue("@Quantity", gvtxtDenoNo.Text);
                cmd.Parameters.AddWithValue("@Total", gvtxtDenoTotal.Text);
                cmd.Parameters.AddWithValue("@NoteNos", gvtxtDenoNoteNos.Text);


                result = cmd.ExecuteNonQuery();

            }
        }
        if (result > 0)
        {
            transactionGL.Commit();
            datasaved = true;
        }
        else
        {
            transactionGL.Rollback();
            datasaved = false;
        }



        if (datasaved == true && operation == "Save")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLCASHINOUT", "alert('Record Saved Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
        if (datasaved == true && operation == "Update")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLCASHINOUT", "alert('Record Updated Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
        if (datasaved == true && operation == "Delete")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLCASHINOUT", "alert('Record Deleted Successfully');", true);
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }




    }

    public void GL_CashInOut_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_CashInOut_PRV";
        cmd.Parameters.AddWithValue("@operation", operation);
        cmd.Parameters.AddWithValue("@InOutId", value);
        cmd.Parameters.AddWithValue("@refno", txtreferenceno.Text);
        cmd.Parameters.AddWithValue("@fname", hdncfname.Value.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();

    }

    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GL_CashInOut_PRV("Delete", hdnid.Value.Trim());
            GL_CashInOut_PRI("Delete", hdnid.Value.Trim());

            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {

            Popup_RTR();
            hdnpopup.Value = "View";

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {

            //Response.Redirect("GLCashInOutForm.aspx");
            ClearData();

            gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            GLCashInOut_Search();
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }

    public void GLCashInOut_Search()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "Gl_CashInOut_Search";
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
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
            GLCashINOut_Details_RTR(id);
            hdnoperation.Value = "Update";
            ddlInOutto.Enabled = true;
            ddlInOuttoName.Enabled = true;
            ddlInOutfrom.Enabled = true;
            ddlInOutfromName.Enabled = true;
            ddlInOutBy.Enabled = true;
            gvDenominationDetails.Enabled = true;
            panel2.Enabled = true;
            gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLCashINOUTDeatils", "alert('" + ex.Message + "');", true);
        }


    }

    public void GLCashINOut_Details_RTR(string id)
    {
        int tot = 0;
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashINOutDetails_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@InOutId", id);

        dt1 = new DataTable();
        da.Fill(dt1);
        if (dt1.Rows.Count > 0)
        {
            hdnid.Value = dt1.Rows[0]["InOutID"].ToString();
            hdncfname.Value = dt1.Rows[0]["FName"].ToString();


            if (hdncfname.Value == "S")
            {
                ddlInOutto.Items.Clear();
                //ddlInOutto.Items.Insert(0, new ListItem("Customer", "Customer"));

                ddlInOutto.Items.Add(new ListItem("Customer", "Customer"));

                string goldno = dt1.Rows[0]["Customer gold"].ToString();
                txtgoldname.Text = goldno;
                txtgoldname.Visible = true;
                ddlInOuttoName.Visible = false;
                txtgoldname.Enabled = false;






            }
            else
            {
                ddlInOutto.Items.Clear();
                ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
                ddlInOutto.Items.Insert(1, new ListItem("Cashier"));
                ddlInOutto.Items.Insert(2, new ListItem("Bank/CashAcc"));
                ddlInOutto.SelectedValue = dt1.Rows[0]["InOutTo"].ToString().Trim();
                if (ddlInOutto.SelectedValue == "Cashier")
                {
                    bindEmployee();
                }
                else
                {
                    bindBankcashaccto();
                }
                ddlInOuttoName.SelectedValue = dt1.Rows[0]["InOutToID"].ToString().Trim();
                txtgoldname.Visible = false;
                ddlInOuttoName.Visible = true;
            }
            txtreferenceno.Text = dt1.Rows[0]["ReferenceType"].ToString();
            DateTime date = Convert.ToDateTime(dt1.Rows[0]["Date_time"]);

            txtdate.Text = date.ToString("dd/MM/yyyy HH:mm:ss");

            ddlInOutMode.SelectedValue = dt1.Rows[0]["InOutMode"].ToString().Trim();




            //ddlInOuttoName.SelectedValue = dt1.Rows[0]["InOutToID"].ToString().Trim();



            //ddlInOutfrom.SelectedValue = dt1.Rows[0]["InOutFrom"].ToString();

            //if (ddlInOutfrom.SelectedValue == "Cashier")
            //{
            //    ddlInOutfromName.Items.Clear();
            //    ddlInOutfromName.Items.Insert(0, new ListItem("Safe", "0"));


            //}
            //else
            //{
            //    bindBankcashaccfrm();
            //}
            //ddlInOutfromName.SelectedValue = dt1.Rows[0]["InOutFromID"].ToString().Trim();


            ddlInOutBy.SelectedValue = dt1.Rows[0]["InOutBy"].ToString().Trim();

            gvDenominationDetails.DataSource = dt1;
            gvDenominationDetails.DataBind();

            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                tot = tot + Convert.ToInt32(dt1.Rows[i]["Total"].ToString().Trim());
            }
            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            gvtxtDenoTotalAmt.Text = "" + tot;

        }
    }
    protected void ClearData()
    {

        hdnoperation.Value = "Save";
        hdnid.Value = "0";
        ddlInOutMode.SelectedIndex = 0;
        ddlInOutto.SelectedIndex = 0;
        ddlInOuttoName.SelectedIndex = 0;
        //ddlInOutfromName.SelectedIndex = 0;
        ddlInOutBy.SelectedIndex = 0;
        // ddlInOutfrom.SelectedIndex = 0;
        ddlInOutto.Enabled = false;
        ddlInOuttoName.Enabled = false;
        ddlInOutfrom.Enabled = false;
        ddlInOutfromName.Enabled = false;
        ddlInOutBy.Enabled = false;
        panel2.Enabled = false;
        gvDenominationDetails.Controls.Clear();
        BindDenominationDetails();
        AutogenerateRefNo();
        txtgoldname.Visible = false;
        ddlInOutto.Visible = true;
        ddlInOuttoName.Visible = true;
        ddlInOutto.SelectedIndex = 0;
        ddlInOutto.Items.Clear();
        ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
        ddlInOutto.Items.Insert(1, new ListItem("Cashier"));
        ddlInOutto.Items.Insert(2, new ListItem("Bank/CashAcc"));
        //txtdate.Text = Convert.ToString(DateTime.Now);


    }

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {

        Master.PropertympeGlobal.Hide();
        Master.PropertytxtSearch.Text = "";
        Master.PropertyddlSearch.SelectedIndex = 0;
        BindDenominationDetails();
        AutogenerateRefNo();
        gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

    }
    public void BlankGv()
    {
        //dt = new DataTable();
        //dt.Columns.Add("DID");
        //dt.Columns.Add("DocumentID");
        //dt.Columns.Add("DocName");
        //dt.Columns.Add("OtherDoc");
        //dt.Columns.Add("NameOnDoc");
        //dt.Columns.Add("VerifiedBy");
        //dt.Columns.Add("Empld");
        //dt.Columns.Add("ImagePath");
        //dt.Columns.Add("ImageUrl");
        //gbl.ShowNoResultFound(dt, gvDocumentDetails);
    }
    public void FillddlSearch()
    {
        //Master.PropertyddlSearch.Items.Add("Inward/Outward Mode");
        //Master.PropertyddlSearch.Items.Add("Reference No");
        // Master.PropertyddlSearch.Items.Insert(0, new ListItem("--Select--"));



    }
    protected void ddlInOutto_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlInOutto.SelectedValue == "Cashier")
        {
            bindEmployee();
        }
        else
        {
            bindBankcashaccto();
        }
        if (hdnoperation.Value == "Save")
        {


            if (ddlInOutMode.SelectedIndex == 0)
            {
                ddlInOutBy.Enabled = false;
                ddlInOutto.Enabled = false;
                ddlInOuttoName.Enabled = false;
                gvDenominationDetails.Enabled = false;
                panel2.Enabled = false;
                ddlInOutto.Items.Clear();
                ddlInOuttoName.Items.Clear();
                //ddlInOutBy.Items.Clear();
                ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
                ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
                //ddlInOutBy.Items.Insert(0, new ListItem("---Select Inward/Outward By---"));bind
                bindEmp();
            }
            else if (ddlInOutto.SelectedIndex == 0)
            {
                ddlInOuttoName.Enabled = false;
                ddlInOutto.Enabled = false;
                gvDenominationDetails.Enabled = false;
                ddlInOutfrom.Enabled = false;
                panel2.Enabled = false;
                ddlInOuttoName.Items.Clear();
                // ddlInOutBy.Items.Clear();
                ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
                // ddlInOutBy.Items.Insert(0, new ListItem("---Select Inward/Outward By---"));
                bindEmp();
            }



            else
            {
                ddlInOuttoName.Enabled = true;
                ddlInOutto.Enabled = true;
                gvDenominationDetails.Enabled = true;
                ddlInOutfrom.Enabled = true;
                panel2.Enabled = true;
                ddlInOutBy.Enabled = false;
                bindEmp();
            }


        }
        if (hdnoperation.Value == "Update")
        {
            ddlInOuttoName.Enabled = true;
            ddlInOutto.Enabled = true;
            gvDenominationDetails.Enabled = true;
            ddlInOutfrom.Enabled = true;
            panel2.Enabled = true;
        }

    }

    public void bindBankcashaccto()
    {
        try
        {

            conn = new SqlConnection(strConnStringAIM);

            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ddlInOuttoName.DataSource = dt;
            ddlInOuttoName.DataValueField = "AccountID";
            ddlInOuttoName.DataTextField = "Name";
            ddlInOuttoName.DataBind();
            ddlInOuttoName.Items.Insert(0, new ListItem("--Select Account--", "0"));


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #region Bind BankCashAcc
    protected void bindBankcashaccfrm()
    {
        try
        {

            conn = new SqlConnection(strConnStringAIM);

            strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                        "FROM tblAccountmaster " +
                        "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                        "AND Suspended='No' ";
            SqlDataAdapter da = new SqlDataAdapter(strQuery, conn);
            DataTable dt = new DataTable();
            da.Fill(dt);

            ddlInOutfromName.DataSource = dt;
            ddlInOutfromName.DataValueField = "AccountID";
            ddlInOutfromName.DataTextField = "Name";
            ddlInOutfromName.DataBind();
            ddlInOutfromName.Items.Insert(0, new ListItem("--Select Account--", "0"));

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    protected void bindEmployee()
    {
        try
        {
            connAIM = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.CommandText = "select (EmpFirstName + ' ' + EmpMiddleName + ' ' + EmpLastName)EmpName,EmployeeID  from tblHRMS_EmployeeMaster where status='Active'";
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            //--------------------------------------------------------------------------------
            ddlInOuttoName.DataSource = dt;
            ddlInOuttoName.DataTextField = "EmpName";
            ddlInOuttoName.DataValueField = "EmployeeID";
            ddlInOuttoName.DataBind();
            ddlInOuttoName.Items.Insert(0, new ListItem("--Select Cashier's Name--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Fillemployee", "alert('" + ex.Message + "');", true);
        }

    }
    protected void ddlInOutfrom_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (hdnoperation.Value == "Save")
        {
            ddlInOutfrom.Enabled = true;
            ddlInOutfromName.Enabled = true;
            ddlInOutto.Enabled = true;
            ddlInOuttoName.Enabled = true;
            gvDenominationDetails.Enabled = true;
            panel2.Enabled = true;
        }
        if (ddlInOutfrom.SelectedValue == "Cashier")
        {
            ddlInOutfromName.Items.Clear();
            ddlInOutfromName.Items.Insert(0, new ListItem("Safe", "0"));
            ddlInOutBy.Enabled = true;

        }
        else
        {
            bindBankcashaccfrm();
        }
    }



    protected void ddlInOutfromName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (hdnoperation.Value == "Save")
        {

            if (ddlInOutMode.SelectedIndex == 0)
            {
                ddlInOutBy.Enabled = false;
                ddlInOutto.Enabled = false;
                ddlInOuttoName.Enabled = false;
                gvDenominationDetails.Enabled = false;
                panel2.Enabled = false;
                ddlInOuttoName.Items.Clear();
                ddlInOutto.Items.Clear();
                ddlInOutBy.Items.Clear();
                ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
                ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
                ddlInOutBy.Items.Insert(0, new ListItem("--Select Inward/Outward By--"));
            }
            else if (ddlInOutfromName.SelectedIndex == 0)
            {
                ddlInOutBy.Enabled = false;

                ddlInOutto.Enabled = true;
                ddlInOuttoName.Enabled = true;
                gvDenominationDetails.Enabled = false;
                panel2.Enabled = false;
                ddlInOutBy.Items.Clear();
                ddlInOutBy.Items.Insert(0, new ListItem("--Select Inward/Outward By--"));
            }


            else
            {
                // ddlInOutfromName.Enabled = true;
                ddlInOutBy.Enabled = true;
                // ddlInOutfrom.Enabled = true;
                /// ddlInOutfromName.Enabled = true;
                ddlInOutto.Enabled = true;
                ddlInOuttoName.Enabled = true;
                gvDenominationDetails.Enabled = true;
                panel2.Enabled = true;
            }
        }
        if (hdnoperation.Value == "Update")
        {
            ddlInOutBy.Enabled = true;
            // ddlInOutfrom.Enabled = true;
            /// ddlInOutfromName.Enabled = true;
            ddlInOutto.Enabled = true;
            ddlInOuttoName.Enabled = true;
            gvDenominationDetails.Enabled = true;
            panel2.Enabled = true;
        }


    }


    protected void ddlInOutMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (hdnoperation.Value == "Save")
        {
            if (ddlInOutMode.SelectedIndex == 0)
            {
                ddlInOutto.Enabled = false;
                ddlInOuttoName.Enabled = false;
                gvDenominationDetails.Enabled = false;
                ddlInOuttoName.Items.Clear();
                ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
                ddlInOutto.Items.Clear();
                ddlInOutBy.Items.Clear();
                ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
                ddlInOutto.Items.Insert(1, new ListItem("Cashier"));
                ddlInOutto.Items.Insert(2, new ListItem("Bank/CashAcc"));
                bindEmp();
                BindDenominationDetails();
            }
            if (ddlInOutMode.SelectedIndex != 0)
            {
                ddlInOutto.Enabled = true;
                ddlInOuttoName.Items.Clear();
                ddlInOuttoName.Items.Insert(0, new ListItem("--Select Inward/Outward To Name--"));
                ddlInOutto.Items.Clear();
                ddlInOutBy.Items.Clear();
                ddlInOutto.Items.Insert(0, new ListItem("--Select Inward/Outward To--"));
                ddlInOutto.Items.Insert(1, new ListItem("Cashier"));
                ddlInOutto.Items.Insert(2, new ListItem("Bank/CashAcc"));
                bindEmp();
                BindDenominationDetails();
                //ddlInOuttoName.Enabled = true;
                gvDenominationDetails.Enabled = true;


            }
            if (hdnoperation.Value == "Update")
            {
                ddlInOutto.Enabled = true;
                ddlInOuttoName.Enabled = true;
                gvDenominationDetails.Enabled = true;
            }
        }
    }
    protected void ddlInOuttoName_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (hdnoperation.Value == "Save")
        {
            if (ddlInOuttoName.SelectedIndex == 0)
            {
                ddlInOutBy.Enabled = false;
            }
            if (ddlInOuttoName.SelectedIndex != 0)
            {
                ddlInOutBy.Enabled = true;
                ddlInOuttoName.Enabled = true;
            }
        }
        else
        {
            ddlInOutBy.Enabled = true;
            ddlInOuttoName.Enabled = true;
        }
    }
    protected void gvDenominationDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtsrno = (TextBox)e.Row.FindControl("gvtxtDenoSrno");
            txtsrno.Attributes.Add("readonly", "readonly");
        }
    }
}