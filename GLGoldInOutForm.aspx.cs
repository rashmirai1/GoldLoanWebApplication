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
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;
using System.Text;
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;

public partial class GLGoldInOutForm : System.Web.UI.Page
{

    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    //creating instance of class "CompanyWiseAccountClosing"
    GlobalSettings gbl = new GlobalSettings();
    CompanyWiseAccountClosing objCompWiseAccClosing = new CompanyWiseAccountClosing();

    //Declaring Variables.
    string strRefType = "AF";
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = string.Empty;
    int FYearID = 0;
    string GoldLoanNo = string.Empty;
    string GoldNo = string.Empty;
    string UserName = string.Empty;
    string Password = string.Empty;
    bool datasaved = false;
    int SanctionLoginID = 0;
    int branchId = 0;
    int UserID = 0;
    public string loginDate;
    public string expressDate;
    string time;
    int result = 0;
    string bankname = "";
    string bankid = "";
    string bank_id = "";
    //Declaring Objects.
    DateTime LogInTime1;
    SqlTransaction transactionGL, transactionAIM;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;
    DataTable dt;
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
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]
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

            Master.PropertybtnSave.OnClientClick = "return valid();";

            if (!IsPostBack)
            {
                BindGoldItemDetails();
                BindPreviousDetails();
                AutogenerateRefNo();
                BindGoldLoanNo();
                bindNarration();
                bindEmployee();
                txtreferenceno.Attributes.Add("readonly", "readonly");
                txtdate.Attributes.Add("readonly", "readonly");
                txtpouchno.Attributes.Add("readonly", "readonly");
                txtGoldNo.Attributes.Add("readonly", "readonly");
                txtdate.Text = Convert.ToString(DateTime.Now);
                DateTime dte = DateTime.Now;
                time = dte.ToString("HH:mm");
                hdntime.Value = time;
                Master.PropertytxtSearch.Text = "";
                ddllocname.Items.Clear();
                ddllocname.Items.Insert(0, new ListItem("--Select Location--", "0"));
                gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            }
            ddllocname.Visible = true;


            if (hdnoperation.Value == "Save")
            {
                btnGlSearch.Enabled = true;
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }
    public void bindNarration()
    {
        try
        {
            conn = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select NarrationID,NarrationName from tblnarrationmaster";
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);
            ddlnarration.DataSource = dt;
            ddlnarration.DataTextField = "NarrationName";
            ddlnarration.DataValueField = "NarrationID";
            ddlnarration.DataBind();
            ddlnarration.Items.Insert(0, new ListItem("--Select Narration--", "0"));






        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Fillemployee", "alert('" + ex.Message + "');", true);
        }
    }
    protected void btnGlSearch_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            if (ddlInOutMode.SelectedValue == "0")
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Select Mode');", true);
            }
            else
            {
                GoldNo_RTR();
                BindPreviousDetails();
                Master.PropertyddlSearch.Items.Clear();
                Master.PropertyddlSearch.Items.Add("Gold Loan No");
                Master.PropertyddlSearch.Items.Add("Customer Name");
                Master.PropertyddlSearch.Items.Add("Loan Date");
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('" + ex.Message + "');", true);
        }
    }
    public void GoldNo_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GInOut_GoldLOANNO_RTR";
        //    cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        cmd.Parameters.AddWithValue("@GLInOutMode", ddlInOutMode.SelectedValue);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            Master.PropertygvGlobal.DataSource = dt;
            Master.DataBind();
            Master.PropertympeGlobal.Show();
            hdnpopup.Value = "GoldLoan";
            hdnoperation.Value = "Save";
        }
        else
        {
            Master.PropertygvGlobal.DataSource = dt;
            Master.DataBind();
            Master.PropertympeGlobal.Show();
            hdnpopup.Value = "GoldLoan";
            hdnoperation.Value = "Save";
        }


    }

    public void bindEmployee()
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

    public void BindGoldLoanNo()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select SDID,GoldLoanNo from tglsanctionDisburse_basicDetails where  BranchID=@BranchID and CMPID=@CMPID and isActive='Y'";
            da = new SqlDataAdapter(cmd);
            // cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString()); FYID=@FYID and
            cmd.Parameters.AddWithValue("@BranchID", Session["branchId"].ToString());
            cmd.Parameters.AddWithValue("@CMPID", "1");
            dt = new DataTable();
            da.Fill(dt);


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillGoldLoadNo", "alert('" + ex.Message + "');", true);
        }
    }
    public void AutogenerateRefNo()
    {
        RefType = "GIO";
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand("select isnull(max(RefNo),0)+1 from TGLGoldInOutDetails", conn);

        da = new SqlDataAdapter(cmd);


        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            hdnrefno.Value = dt.Rows[0][0].ToString();
            txtreferenceno.Text = RefType + '/' + dt.Rows[0][0].ToString();
        }
    }

    public void BindPreviousDetails()
    {
        try
        {
            dt = new DataTable();
            dt.Columns.Add("Inward/Outward Mode", typeof(String));
            dt.Columns.Add("Reference No", typeof(String));
            dt.Columns.Add("Location Name", typeof(String));
            dt.Columns.Add("Location Details", typeof(String));
            //dt.Columns.Add("LocName", typeof(String));

            ShowNoResultFound(dt, gvPrevDetails);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindPreDetail", "alert('" + ex.Message + "');", true);
        }
    }
    protected void BindGoldItemDetails()
    {
        try
        {
            dt = new DataTable();

            dt.Columns.Add("GID", typeof(String));
            dt.Columns.Add("SDID", typeof(String));
            dt.Columns.Add("ItemID", typeof(String));
            dt.Columns.Add("ItemName", typeof(String));
            dt.Columns.Add("Quantity", typeof(String));
            dt.Columns.Add("GrossWeight", typeof(String));
            dt.Columns.Add("NetWeight", typeof(String));
            dt.Columns.Add("Purity", typeof(String));
            dt.Columns.Add("RateperGram", typeof(String));
            dt.Columns.Add("Value", typeof(String));


            ShowNoResultFound(dt, dgvGoldItemDetails);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void ShowNoResultFound(DataTable source, GridView gv)
    {
        try
        {
            // create a new blank row to the DataTable
            source.Rows.Add(source.NewRow());

            // Bind the DataTable which contain a blank row to the GridView
            gv.DataSource = source;
            gv.DataBind();

            // Get the total number of columns in the GridView to know what the Column Span should be
            int columnsCount = gv.Columns.Count;
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ShowNoResultFoundAlert", "alert('" + ex.Message + "');", true);
        }
    }






    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {


            Popup_RTR();
            Master.PropertyddlSearch.Items.Clear();
            hdnpopup.Value = "Edit";
            Master.PropertyddlSearch.Items.Add("Reference No");
            Master.PropertyddlSearch.Items.Add("Date");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("Inward/Outward Mode");
            Master.PropertyddlSearch.Items.Add("Inward/Outward Location");

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
        conn.Open();
        DataTable dtbk = new DataTable();
        DataTable dtaim = new DataTable();

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_GINOUT_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        // cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());

        dt = new DataTable();
        da.Fill(dt);


        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["Inward/Outward Location"].ToString() == "Bank")
            {
                int id = Convert.ToInt32(dt.Rows[i]["LocName"]);
                conn = new SqlConnection(strConnStringAIM);
                conn.Open();
                strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                            "FROM tblAccountmaster " +
                            "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                            "AND Suspended='No' and AccountID = " + id;
                SqlDataAdapter daaim = new SqlDataAdapter(strQuery, conn);

                daaim.Fill(dtaim);

                dt.Rows[i]["Location Name"] = dtaim.Rows[0]["Name"].ToString();
                dtaim.Rows.Clear();






            }




        }
        dt.Columns.Remove("LocName");

        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();



        conn.Close();





    }

    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {

            if (ddlInOutLocation.SelectedValue == "Bank" || ddlInOutLocation.SelectedValue == "Office")
            {

                txtlocname.Visible = false;
                ddllocname.Visible = true;

            }
            if (ddlInOutLocation.SelectedValue == "Residence" || ddlInOutLocation.SelectedValue == "Customer")
            {

                txtlocname.Visible = true;
                ddllocname.Visible = false;
            }
            if (hdnoperation.Value == "Save")
            {
                ddlInOutMode.Enabled = true;
                GL_GInOutSave_PRV("Save", "0");
                GL_GInOutSave_PRI("Save", "0");

            }
            if (hdnoperation.Value == "Update")
            {


                GL_GInOutSave_PRV("Update", "0");
                GL_GInOutSave_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDeatils", "alert('" + ex.Message + "');", true);

        }

    }

    public void GL_GInOutSave_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GInOut_PRV";
        cmd.Parameters.AddWithValue("@operation", operation);
        cmd.Parameters.AddWithValue("@SDId", hdnsdid.Value.Trim());
        cmd.Parameters.AddWithValue("@GLInOutMode", ddlInOutMode.SelectedValue);
        cmd.Parameters.AddWithValue("@GIInOutId", hdnid.Value.Trim());
        cmd.Parameters.AddWithValue("@InOutLoc", ddlInOutLocation.SelectedValue);
        cmd.ExecuteNonQuery();
        conn.Close();





    }

    protected void GL_GInOutSave_PRI(string operation, string value)
    {
        datasaved = false;
        RefType = "GIO";

        conn = new SqlConnection(strConnString);
        conn.Open();

        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.Transaction = transactionGL;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GINOUT_PRI";
        cmd.Parameters.AddWithValue("@operation", operation);
        cmd.Parameters.AddWithValue("@GLInOutID", value);
        cmd.Parameters.AddWithValue("@LineNo", 0);

        cmd.Parameters.AddWithValue("@RefType", RefType);
        cmd.Parameters.AddWithValue("@RefNo", DBNull.Value);
        cmd.Parameters.AddWithValue("@ReferenceType", DBNull.Value);
        cmd.Parameters.AddWithValue("@Date_time", gbl.ChangeDateMMddyyyy(txtdate.Text));
        cmd.Parameters.AddWithValue("@time", hdntime.Value);
        cmd.Parameters.AddWithValue("@SDID", txtpouchno.Text);
        cmd.Parameters.AddWithValue("@KYCID", hdnKYCID.Value);
        cmd.Parameters.AddWithValue("@GoldLoanNo", txtGoldNo.Text);
        cmd.Parameters.AddWithValue("@PouchNo", txtpouchno.Text);
        cmd.Parameters.AddWithValue("@GLInOutMode", ddlInOutMode.SelectedValue);
        cmd.Parameters.AddWithValue("@InOutLoc", ddlInOutLocation.SelectedValue);
        if (ddlInOutLocation.SelectedValue == "Bank" || ddlInOutLocation.SelectedValue == "Office")
        {
            cmd.Parameters.AddWithValue("@LocName", ddllocname.SelectedValue);
            cmd.Parameters.AddWithValue("@LocResiName", "");
            txtlocname.Visible = false;
            ddllocname.Visible = true;

        }
        if (ddlInOutLocation.SelectedValue == "Residence" || ddlInOutLocation.SelectedValue == "Customer")
        {
            cmd.Parameters.AddWithValue("@LocName", "");
            cmd.Parameters.AddWithValue("@LocResiName", txtlocname.Text);

            txtlocname.Visible = true;
            ddllocname.Visible = false;
        }

        cmd.Parameters.AddWithValue("@LocDetails", txtlocdetails.Text);
        cmd.Parameters.AddWithValue("@NarrationID", ddlnarration.SelectedValue);
        cmd.Parameters.AddWithValue("@InOutBy", ddlInOutBy.SelectedValue);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@CMPID", "1");
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        cmd.Parameters.AddWithValue("@CreatedBy", Session["userID"].ToString());
        result = cmd.ExecuteNonQuery();



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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('Record Saved Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
        if (datasaved == true && operation == "Update")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('Record Updated Successfully');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
        if (datasaved == true && operation == "Delete")
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('Record Deleted Successfully');", true);
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            ClearData();
        }
    }

    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlInOutLocation.SelectedValue == "Bank" || ddlInOutLocation.SelectedValue == "Office")
            {

                txtlocname.Visible = false;
                ddllocname.Visible = true;

            }
            if (ddlInOutLocation.SelectedValue == "Residence" || ddlInOutLocation.SelectedValue == "Customer")
            {

                txtlocname.Visible = true;
                ddllocname.Visible = false;
            }
            GL_GInOutSave_PRV("Delete", hdnid.Value);
            GL_GInOutSave_PRI("Delete", hdnid.Value);

            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('" + ex.Message + "');", true);
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {

            Popup_RTR();
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Reference No");
            Master.PropertyddlSearch.Items.Add("Date");
            Master.PropertyddlSearch.Items.Add("Customer Name");

            Master.PropertyddlSearch.Items.Add("Inward/Outward Mode");
            Master.PropertyddlSearch.Items.Add("Inward/Outward Location");
            hdnpopup.Value = "View";

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('" + ex.Message + "');", true);
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
            ClearData();

            gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnpopup.Value == "GoldLoan")
            {
                GoldLoanNo_Search();

            }
            if (hdnpopup.Value == "Edit")
            {
                GoldInOut_Search();



            }
            if (hdnpopup.Value == "View")
            {
                GoldInOut_Search();
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }

    public void GoldInOut_Search()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GINOUT_SEARCH";
        cmd.Parameters.AddWithValue("@SearchType", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        // cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.PropertygvGlobal.DataBind();
        Master.PropertympeGlobal.Show();

    }

    public void GoldLoanNo_Search()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GInOut_GoldLoanNO_Search";
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@Searchvalue", Master.PropertytxtSearch.Text.Trim());
        //   cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
        cmd.Parameters.AddWithValue("@GLInOutMode", ddlInOutMode.SelectedValue);

        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.PropertygvGlobal.DataBind();
        Master.PropertympeGlobal.Show();

    }
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;




            if (hdnpopup.Value.Trim() == "GoldLoan")
            {
                GoldLoanDetails_RTR(id);
                BindGoldItemDetails(id);
                BindPreviousDetailsByID(hdnsdid.Value.Trim());

            }
            if (hdnpopup.Value == "Edit")
            {
                hdnoperation.Value = "Update";
                ddlInOutMode.Enabled = false;
                GoldInOutDetails_RTR(id);
                btnGlSearch.Enabled = false;


            }
            if (hdnpopup.Value == "View")
            {
                GoldInOutDetails_RTR(id);
                btnGlSearch.Enabled = false;


            }
            gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLGOLDINOUT", "alert('" + ex.Message + "');", true);
        }


    }

    public void GoldInOutDetails_RTR(string id)
    {
        string InOutLoc = "";
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GINOUTDetails_RTR";
        cmd.Parameters.AddWithValue("@GLINOUTID", id);

        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtreferenceno.Text = dt.Rows[0]["ReferenceType"].ToString();
            txtdate.Text = dt.Rows[0]["Date_time"].ToString();
            txtGoldNo.Text = dt.Rows[0]["GoldLoanNo"].ToString();
            hdnsdid.Value = dt.Rows[0]["SDID"].ToString();
            hdnKYCID.Value = dt.Rows[0]["KYCID"].ToString();
            txtpouchno.Text = dt.Rows[0]["PouchNo"].ToString();
            ddlInOutMode.SelectedValue = dt.Rows[0]["GLInOutMode"].ToString();
            hdncustomeradd.Value = dt.Rows[0]["Address"].ToString();
            if (ddlInOutMode.SelectedValue == "I")
            {
                ddlInOutLocation.Items.Clear();
                ddlInOutLocation.Items.Add(new ListItem("--Select Inward/Outward Location--", "0"));
                ddlInOutLocation.Items.Add(new ListItem("Bank", "Bank"));
                ddlInOutLocation.Items.Add(new ListItem("Office", "Office"));
                ddlInOutLocation.Items.Add(new ListItem("Residence", "Residence"));


            }
            if (ddlInOutMode.SelectedValue == "O")
            {
                ddlInOutLocation.Items.Clear();
                ddlInOutLocation.Items.Add(new ListItem("--Select Inward/Outward Location--", "0"));
                ddlInOutLocation.Items.Add(new ListItem("Bank", "Bank"));
                ddlInOutLocation.Items.Add(new ListItem("Residence", "Residence"));
                ddlInOutLocation.Items.Add(new ListItem("Customer", "Customer"));

            }

            InOutLoc = dt.Rows[0]["InOutLoc"].ToString();

            ddlInOutLocation.SelectedValue = dt.Rows[0]["InOutLoc"].ToString();
            if (InOutLoc == "Bank")
            {
                BindBank();
                ddllocname.SelectedValue = dt.Rows[0]["LocName"].ToString();
                txtlocname.Visible = false;
                ddlInOutLocation.Enabled = true;
            }
            if (InOutLoc == "Office")
            {
                bindBranch();
                ddllocname.SelectedValue = dt.Rows[0]["LocName"].ToString();
                txtlocname.Visible = false;
                ddlInOutLocation.Enabled = true;
            }
            if (InOutLoc == "Residence")
            {
                txtlocname.Text = dt.Rows[0]["LocResiName"].ToString();
                ddllocname.Visible = false;
                txtlocname.Visible = true;
                ddlInOutLocation.Enabled = true;
                txtlocname.Enabled = true;
            }
            if (InOutLoc == "Customer")
            {
                txtlocname.Text = dt.Rows[0]["LocResiName"].ToString();
                ddllocname.Visible = false;
                txtlocname.Visible = true;
                ddlInOutLocation.Enabled = false;
                txtlocname.Enabled = false;
            }


            txtlocdetails.Text = dt.Rows[0]["LocDetails"].ToString();

            bindNarration();
            ddlnarration.SelectedValue = dt.Rows[0]["NarrationID"].ToString();
            ddlInOutBy.SelectedValue = dt.Rows[0]["InOutBy"].ToString();

            BindGoldItemDetails(hdnsdid.Value);
            //BindPreviousDetails();
            BindPreviousDetailsByID(hdnsdid.Value);


            hdnid.Value = id;
            gbl.CheckAEDControlSettings("Search", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);



        }
    }

    public void GoldLoanDetails_RTR(string sdid)
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_GINOUT_GoldLoanNoDetails_RTR";
        cmd.Parameters.AddWithValue("@SDID", sdid);
        //  cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);



        // strQuery = "select a.Area+','+z.zone+', '+ct.CityName+', '+ st.StateName+', '+a.Pincode as 'Address'  from  tblstatemaster st  (nolock) inner join tblcitymaster ct (nolock) on k.CityID =ct.cityID inner join tblAreamaster a (nolock) on k.AreaID =a.AreaID  inner join tblzonemaster z (nolock) on k.ZoneID =z.ZoneID   where CompID=1";

        strQuery = "select a.Area+','+z.zone+', '+ct.CityName+', '+ st.StateName+', '+a.Pincode as 'Address' " +
                    "from  tblstatemaster st  (nolock)" +
                    "inner join tblcitymaster ct (nolock) on ct.cityID=" + dt.Rows[0]["CityID"].ToString() +
                   " inner join tblAreamaster a (nolock) on a.AreaID =" + dt.Rows[0]["AreaID"].ToString() +
                   "inner join tblzonemaster z (nolock) on  z.ZoneID =" + dt.Rows[0]["ZoneID"].ToString() +
                    "where st.StateID=" + dt.Rows[0]["StateID"].ToString();

        conn = new SqlConnection(strConnStringAIM);
        SqlDataAdapter da1 = new SqlDataAdapter(strQuery, conn);
        DataSet ds1 = new DataSet();
        da1.Fill(ds1);

        if (dt.Rows.Count > 0)
        {
            txtGoldNo.Text = dt.Rows[0]["GoldLoanNo"].ToString();
            hdnKYCID.Value = dt.Rows[0]["KYCID"].ToString();
            hdnsdid.Value = dt.Rows[0]["SDID"].ToString();
            hdncustomeradd.Value = dt.Rows[0]["Address"].ToString();
            txtpouchno.Text = "" + sdid;
            hdnoperation.Value = "Save";
            gbl.CheckAEDControlSettings("Search", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        if (ds1.Tables[0].Rows.Count > 0)
        {
            hdncustomeradd.Value += ds1.Tables[0].Rows[0]["Address"];
        }

    }
    protected void ClearData()
    {

        hdnoperation.Value = "Save";
        hdnid.Value = "0";
        AutogenerateRefNo();
        BindGoldItemDetails();
        BindPreviousDetails();
        ddlInOutMode.SelectedIndex = 0;
        ddlInOutLocation.SelectedIndex = 0;
        ddllocname.SelectedIndex = 0;
        txtlocname.Visible = false;
        ddllocname.Visible = true;
        txtlocname.Text = "";
        txtlocdetails.Text = "";
        txtGoldNo.Text = "";
        txtpouchno.Text = "";
        ddlnarration.SelectedIndex = 0;
        ddlInOutBy.SelectedIndex = 0;
        txtdate.Text = Convert.ToString(DateTime.Now);

        btnGlSearch.Enabled = true;
        ddlInOutMode.Enabled = true;
        ddlInOutLocation.Enabled = true;
        txtlocname.Enabled = true;
        ddllocname.Items.Clear();
        ddllocname.Items.Insert(0, new ListItem("--Select Location--", "0"));
    }
    protected void clearatmode()
    {

        hdnoperation.Value = "Save";
        hdnid.Value = "0";
        AutogenerateRefNo();
        dgvGoldItemDetails.Controls.Clear();
        txtGoldNo.Text = "";
        txtpouchno.Text = "";
        gvPrevDetails.Controls.Clear();
        dgvGoldItemDetails.Controls.Clear();
        BindGoldItemDetails("0");
        BindPreviousDetails();


        ddlInOutLocation.SelectedIndex = 0;
        ddllocname.SelectedIndex = 0;
        txtlocname.Visible = false;
        ddllocname.Visible = true;
        txtlocname.Text = "";
        txtlocdetails.Text = "";

        ddlnarration.SelectedIndex = 0;
        ddlInOutBy.SelectedIndex = 0;
        txtdate.Text = Convert.ToString(DateTime.Now);

        btnGlSearch.Enabled = true;
        ddlInOutMode.Enabled = true;
        ddlInOutLocation.Enabled = true;
        txtlocname.Enabled = true;
        ddllocname.Items.Clear();
        ddllocname.Items.Insert(0, new ListItem("--Select Location--", "0"));
    }

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {

        Master.PropertympeGlobal.Hide();
        BindPreviousDetails();
        Master.PropertytxtSearch.Text = "";
        Master.PropertyddlSearch.SelectedIndex = 0;
        BindGoldItemDetails();

        gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

    }


    public void BindGoldItemDetails(string sdid)
    {
        try
        {

            int qty = 0;
            decimal grosswt = 0;
            decimal netwt = 0;
            decimal rate = 0;
            decimal value = 0;
            string purity = "";

            DataTable dtCurrentTable = new DataTable();

            DataTable dt = new DataTable();
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select GID,SDID,g.ItemID,ItemName ,Quantity,GrossWeight,NetWeight,Purity,RateperGram,Value from tglsanctiondisburse_GoldItemDetails g  inner join tblItemMaster i on g.ItemID =i.ItemID  where sdid =@SDID";

            da = new SqlDataAdapter(cmd);
            cmd.Parameters.AddWithValue("@SDID", sdid);

            da.Fill(dt);




            if (dt.Rows.Count > 0)
            {
                dgvGoldItemDetails.DataSource = dt;
                dgvGoldItemDetails.DataBind();
                //purity = dt.Rows[0]["Purity"].ToString();
                foreach (GridViewRow row in dgvGoldItemDetails.Rows)
                {

                    Label lblGID = (Label)row.Cells[1].FindControl("lblGID");
                    Label lblSDID = (Label)row.Cells[1].FindControl("lblSDID");
                    Label lblItemID = (Label)row.Cells[1].FindControl("lblItemID");
                    Label lblitemName = (Label)row.Cells[1].FindControl("lblitemName");
                    Label txtQuantity = (Label)row.Cells[1].FindControl("txtQuantity");
                    Label txtGrossWeight = (Label)row.Cells[1].FindControl("txtGrossWeight");
                    Label txtNetWeight = (Label)row.Cells[1].FindControl("txtNetWeight");

                    Label lblkarat = (Label)row.Cells[1].FindControl("lblkarat");
                    Label txtRatePerGram = (Label)row.Cells[1].FindControl("txtRatePerGram");
                    //Label txtRatePerGram = (Label)
                    Label txtValue = (Label)row.Cells[1].FindControl("txtValue");


                    qty = qty + Convert.ToInt32(txtQuantity.Text);
                    grosswt = grosswt + Convert.ToDecimal(txtGrossWeight.Text);
                    netwt = netwt + Convert.ToDecimal(txtNetWeight.Text);
                    value = value + Convert.ToDecimal(txtValue.Text);
                    rate = Convert.ToDecimal(txtRatePerGram.Text);


                }
                Label txtTotalQuantity = (Label)dgvGoldItemDetails.FooterRow.FindControl("txtTotalQuantity");
                Label txtTotalGrossWeight = (Label)dgvGoldItemDetails.FooterRow.FindControl("txtTotalGrossWeight");
                Label txtTotalNetWeight = (Label)dgvGoldItemDetails.FooterRow.FindControl("txtTotalNetWeight");
                Label txtTotalRatePerGram = (Label)dgvGoldItemDetails.FooterRow.FindControl("txtTotalRatePerGram");
                Label txtTotalValue = (Label)dgvGoldItemDetails.FooterRow.FindControl("txtTotalValue");
                Label txttot = (Label)dgvGoldItemDetails.FooterRow.FindControl("txttot");
                // DropDownList ddlPurity = (DropDownList)dgvGoldItemDetails.HeaderRow.FindControl("ddlPurity");


                txtTotalQuantity.Text = "" + qty;
                txtTotalGrossWeight.Text = "" + grosswt;
                txtTotalNetWeight.Text = "" + netwt;
                txtTotalRatePerGram.Text = "" + rate;
                txtTotalValue.Text = "" + value;
                // ddlPurity.SelectedValue = purity;
                txttot.Text = "Total";



            }
            else
            {
                BindGoldItemDetails();
            }







        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindGoldItems", "alert('" + ex.Message + "');", true);
        }
    }
    protected void ddlInOutLocation_SelectedIndexChanged(object sender, EventArgs e)
    {

        try
        {

            if (ddlInOutLocation.SelectedValue == "Bank")
            {

                BindBank();
                txtlocname.Visible = false;
                txtlocdetails.Text = "";

            }

            if (ddlInOutLocation.SelectedValue == "Office")
            {
                bindBranch();
                txtlocname.Visible = false;
                txtlocdetails.Text = "";
                //  txtlocdetails.Enabled = true
            }

            if (ddlInOutLocation.SelectedValue == "Residence")
            {
                txtlocname.Visible = true;
                ddllocname.Visible = false;

                txtlocdetails.Text = "";
                txtlocname.Text = "";

            }
            if (ddlInOutLocation.SelectedValue == "Customer")
            {
                txtlocname.Visible = true;
                ddllocname.Visible = false;
                txtlocname.Text = txtGoldNo.Text;

                //txtlocname.Attributes.Add("readonly", "readonly");
                txtlocdetails.Text = hdncustomeradd.Value.ToString();
                //                txtlocdetails.Attributes.Add("readonly", "readonly");



            }
            if (ddlInOutLocation.SelectedValue == "0")
            {
                ddllocname.Items.Clear();
                ddllocname.Items.Insert(0, new ListItem("--Select Location--", "0"));
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LocationSelected", "alert('" + ex.Message + "');", true);
        }

    }

    public void bindBranch()
    {
        try
        {
            strQuery = "select BID, BranchName from tblCompanyBranchMaster where CompID=1";
            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            ddllocname.DataSource = ds;
            ddllocname.DataTextField = "BranchName";
            ddllocname.DataValueField = "BID";
            ddllocname.DataBind();
            ddllocname.Items.Insert(0, new ListItem("--Select Branch--", "0"));
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindBranchAlert", "alert('" + ex.Message + "');", true);
        }
    }

    public void BindBank()
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
            ddllocname.DataSource = dt;
            ddllocname.DataValueField = "AccountID";
            ddllocname.DataTextField = "Name";
            ddllocname.DataBind();
            ddllocname.Items.Insert(0, new ListItem("--Select Account--", "0"));


        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void ddlInOutMode_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            // BindPreviousDetailsByID(hdnsdid.Value.Trim());
            if (ddlInOutLocation.SelectedValue == "Bank" || ddlInOutLocation.SelectedValue == "Office")
            {
                txtlocname.Visible = false;
                ddllocname.Visible = true;
            }
            if (ddlInOutLocation.SelectedValue == "Residence" || ddlInOutLocation.SelectedValue == "Customer")
            {

                ddllocname.Visible = false;
                txtlocname.Visible = true;


            }

            if (hdnoperation.Value == "Save")
            {
                if (ddlInOutMode.SelectedValue == "0")
                {
                    clearatmode();
                }

                if (ddlInOutMode.SelectedValue == "O")
                {
                    clearatmode();
                    // ClearData();
                    ddlInOutLocation.Items.Clear();
                    ddlInOutLocation.Items.Add(new ListItem("--Select Inward/Outward Location--", "0"));
                    ddlInOutLocation.Items.Add(new ListItem("Bank", "Bank"));
                    ddlInOutLocation.Items.Add(new ListItem("Residence", "Residence"));
                    ddlInOutLocation.Items.Add(new ListItem("Customer", "Customer"));

                    ////need to check outstanding against that goldloan no  if outstanding is pending then do not allow outward 
                    ddlInOutMode.SelectedValue = "O";

                }
                if (ddlInOutMode.SelectedValue == "I")
                {
                    clearatmode();
                    // ClearData();
                    ddlInOutLocation.Items.Clear();
                    ddlInOutLocation.Items.Add(new ListItem("--Select Inward/Outward Location--", "0"));
                    ddlInOutLocation.Items.Add(new ListItem("Bank", "Bank"));
                    ddlInOutLocation.Items.Add(new ListItem("Office", "Office"));
                    ddlInOutLocation.Items.Add(new ListItem("Residence", "Residence"));

                    ddlInOutMode.SelectedValue = "I";

                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "FillBankCashAccAlert", "alert('" + ex.Message + "');", true);
        }




    }

    public void BindPreviousDetailsByID(string sdid)
    {

        conn = new SqlConnection(strConnString);

        DataTable dtbk = new DataTable();
        DataTable dtaim = new DataTable();





        cmd = new SqlCommand("GL_GINOUT_PreviousDetails");
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@sdid", hdnsdid.Value.Trim());

        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        for (int i = 0; i < dt.Rows.Count; i++)
        {
            if (dt.Rows[i]["Inoutloc"].ToString() == "Bank")
            {
                int id = Convert.ToInt32(dt.Rows[i]["LocName"]);
                conn = new SqlConnection(strConnStringAIM);

                strQuery = "SELECT distinct tblAccountMaster.AccountID, tblAccountMaster.Name " +
                  "FROM tblAccountmaster " +
                  "WHERE (GPID='11' OR GPID='70' OR GPID='71') " +
                  "AND Suspended='No' and AccountID = " + id;
                SqlDataAdapter daaim = new SqlDataAdapter(strQuery, conn);

                daaim.Fill(dtaim);

                dt.Rows[i]["Location Name"] = dtaim.Rows[0]["Name"].ToString();
                dtaim.Rows.Clear();


            }

        }





        if (dt.Rows.Count > 0)
        {


            gvPrevDetails.DataSource = dt;
            gvPrevDetails.DataBind();




        }
        else
        {
            BindPreviousDetails();
        }


    }
}