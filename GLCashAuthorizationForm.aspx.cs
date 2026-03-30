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

public partial class GLCashAuthorizationForm : System.Web.UI.Page
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
    int amt = 0;
    int grdamt = 0;
    string flag = "";
    string CAUTHID;
    int denocoin = 0;
    int qtycoin = 0;
    int totcoin = 0;

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
            Master.PropertybtnSave.OnClientClick = "return valid();";


            if (!IsPostBack)
            {
                BindDenominationDetails();
                AutogenerateRefNo();
                txtreferenceno.Attributes.Add("readonly", "readonly");
                txtdate.Attributes.Add("readonly", "readonly");
                DateTime dte1 = DateTime.Now;
                string date = dte1.ToString("dd/MM/yyyy HH:mm:ss");
                txtdate.Text = date;
                string date1 = dte1.ToString("dd/MM/yyyy");
                DateTime dte = DateTime.Now;
                time = dte.ToString("HH:mm");
                hdntime.Value = time;
                hnddate.Value = date1;
                BindGoldLoanReceiptDetails();





                gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);


            }

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
    public void BindGoldLoanReceiptDetails()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashAuth_Receipt";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        cmd.Parameters.AddWithValue("@Refdate", hnddate.Value);

        dt = new DataTable();
        da.Fill(dt);



        dgvReceiptsDetails.DataSource = dt;
        dgvReceiptsDetails.DataBind();





    }
    public void AutogenerateRefNo()
    {
        Reftype = "CAUTH";
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand("select isnull(max(RefNo),0)+1 from TGLCashAuth_BasicDetails", conn);

        da = new SqlDataAdapter(cmd);


        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            hdnrefno.Value = dt.Rows[0][0].ToString();
            txtreferenceno.Text = Reftype + '/' + dt.Rows[0][0].ToString();
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
    protected void BindReceiptDetails()
    {
        try
        {
            dt = new DataTable();
            dt.Columns.Add("SDID");
            dt.Columns.Add("KYCID");
            dt.Columns.Add("RcptID");
            dt.Columns.Add("GoldLoanNo");
            dt.Columns.Add("Name");
            dt.Columns.Add("ReceiptDate");
            dt.Columns.Add("PaymentMode");
            dt.Columns.Add("ReceiptAmount");
            dt.Columns.Add("ReceivedCash");
            dt.Columns.Add("ReceiptNo");
            dt.Columns.Add("Principal");
            dt.Columns.Add("Interest");
            dt.Columns.Add("PenalInterest");
            dt.Columns.Add("Charges");
            dt.Columns.Add("Finalized");


            //    dt.Rows.Add(0,0,0,'GL','abc','','','',);


            ShowNoResultFound(dt, dgvReceiptsDetails);

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
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


        dt.Rows.Add("1", "1", "1000", "0", "0", "");
        dt.Rows.Add("2", "2", "500", "0", "0", "");
        dt.Rows.Add("3", "3", "100", "0", "0", "");
        dt.Rows.Add("4", "4", "50", "0", "0", "");
        dt.Rows.Add("5", "5", "20", "0", "0", "");
        dt.Rows.Add("6", "6", "10", "0", "0", "");
        dt.Rows.Add("7", "7", "5", "0", "0", "");
        //dt.Rows.Add("8", "8", "Total Coins", "0", "0", "");
        gvDenominationDetails.DataSource = dt;
        gvDenominationDetails.DataBind();







    }
    protected void gvDenominationDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            TextBox txtsrno = (TextBox)e.Row.FindControl("gvtxtDenoSrno");
            txtsrno.Attributes.Add("readonly", "readonly");

            TextBox txtdeno = (TextBox)e.Row.FindControl("gvtxtDenoDescription");
            txtdeno.Attributes.Add("readonly", "readonly");




        }
        if (e.Row.RowType == DataControlRowType.Footer)
        {

        }
    }
    protected void dgvReceiptsDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Footer)
        {

            ////e.Row.Cells[0].ColumnSpan = 10;
            //dgvReceiptsDetails.FooterRow.Cells[0].ColumnSpan = 9;
            ////dgvReceiptsDetails.FooterRow.Cells(0).ColumnSpan = 9;
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(1);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(2);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(3);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(4);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(5);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(6);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(7);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(8);
            //dgvReceiptsDetails.FooterRow.Cells.RemoveAt(9);




        }
        if (e.Row.RowType == DataControlRowType.DataRow)
        {


            if (hdnpopup.Value == "View")
            {
                CheckBox chkBx = (CheckBox)e.Row.FindControl("chkfinalized");
                chkBx.Enabled = false;
            }
            if (hdnpopup.Value == "Edit")
            {
                CheckBox chkBx = (CheckBox)e.Row.FindControl("chkfinalized");
                chkBx.Enabled = true;
            }
            if (hdnpopup.Value == "Cancel")
            {
                CheckBox chkBx = (CheckBox)e.Row.FindControl("chkfinalized");
                chkBx.Enabled = true;
            }




        }

    }


    ////protected void chkfinalized_CheckedChanged(object sender, EventArgs e)
    ////{

    ////    CheckBox chk = (CheckBox)sender;
    ////    GridViewRow row = (GridViewRow)chk.NamingContainer;
    ////    CheckBox chkBx = (CheckBox)dgvReceiptsDetails.FindControl("chkfinalized");
    ////    if (chk != null && chk.Checked)
    ////    {
    ////        Label lblreceivedcash = (Label)row.FindControl("lblreceivedcash");
    ////        if (lblreceivedcash.Text != "")
    ////        {
    ////            amt = amt + Convert.ToInt32(lblreceivedcash.Text);
    ////            Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");
    ////            if (lblgrandtotal.Text == "")
    ////            {
    ////                lblgrandtotal.Text = "0";
    ////            }
    ////            grdamt = Convert.ToInt32(lblgrandtotal.Text);

    ////            grdamt = grdamt + amt;

    ////            lblgrandtotal.Text = grdamt.ToString();
    ////        }
    ////    }
    ////    else
    ////    {
    ////        Label lblreceiptid = (Label)row.FindControl("lblreceiptid");
    ////        Label lblreceivedcash = (Label)row.FindControl("lblreceivedcash");
    ////        if (lblreceivedcash.Text != "")
    ////        {
    ////            amt = amt + Convert.ToInt32(lblreceivedcash.Text);
    ////            Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");
    ////            if (lblgrandtotal.Text == "")
    ////            {
    ////                lblgrandtotal.Text = "0";
    ////            }
    ////            grdamt = Convert.ToInt32(lblgrandtotal.Text);

    ////            grdamt = grdamt - amt;

    ////            lblgrandtotal.Text = grdamt.ToString();

    ////            //conn = new SqlConnection(strConnString);
    ////            //conn.Open();
    ////            //cmd = new SqlCommand("update TGlReceipt_BasicDetails set Finalize ='N' where RcptId =" + Convert.ToInt32(lblreceiptid.Text), conn);
    ////            //cmd.ExecuteNonQuery();
    ////            //conn.Close();




    ////        }
    ////    }

    ////}

    protected void btnDenoDelete_Click(object sender, ImageClickEventArgs e)
    {

        try
        {
            decimal tot = 0;

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
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Reference Type");
            Master.PropertyddlSearch.Items.Add("Reference Date");

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
        cmd.CommandText = "GL_CashAuth_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                hdnRefdate.Value = dt.Rows[i]["Ref_date"].ToString();
            }
        }
        dt.Columns.RemoveAt(4);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }

    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {

            if (dgvReceiptsDetails.Rows.Count > 0)
            {
                TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");



                Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");

                lblgrandtotal.Text = "0";
                for (int j = 0; j < dgvReceiptsDetails.Rows.Count; j++)
                {

                    dgvReceiptsDetails.SelectedIndex = j;
                    CheckBox chkBx1 = (CheckBox)dgvReceiptsDetails.SelectedRow.FindControl("chkfinalized");
                    if (chkBx1 != null && chkBx1.Checked)
                    {
                        Label lblreceivedcash = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceivedcash");
                        if (lblreceivedcash.Text != "")
                        {
                            amt = Convert.ToInt32(lblreceivedcash.Text);

                            if (lblgrandtotal.Text == "")
                            {
                                lblgrandtotal.Text = "0";
                            }

                            grdamt = Convert.ToInt32(lblgrandtotal.Text);

                            grdamt = grdamt + amt;

                            lblgrandtotal.Text = grdamt.ToString();
                        }
                    }
                }





                if (gvtxtDenoTotalAmt.Text == "0")
                {
                    hdndemoAmt.Value = "0";
                }
                else
                {
                    hdndemoAmt.Value = gvtxtDenoTotalAmt.Text;
                }

                if (lblgrandtotal.Text == "0")
                {
                    hdnfinalizedAmt.Value = "0";

                }
                else
                {
                    hdnfinalizedAmt.Value = lblgrandtotal.Text;
                }


                if (hdnoperation.Value == "Save")
                {
                    conn = new SqlConnection(strConnString);
                    string querystr = "select isnull(max(CAUTHID),0)+1 as CAUTHID from TGLCashAuth_basicDetails(nolock)";
                    cmd = new SqlCommand(querystr, conn);
                    cmd.Connection = conn;
                    da = new SqlDataAdapter(cmd);
                    dt = new DataTable();
                    da.Fill(dt);



                    if (dt.Rows.Count > 0)
                    {
                        CAUTHID = dt.Rows[0]["CAUTHID"].ToString();
                        GL_CashAuth_PRV("Save", "0");
                        GL_CashAuth_PRI("Save", CAUTHID);
                    }


                }
                if (hdnoperation.Value == "Update")
                {
                    GL_CashAuth_PRV("Update", "0");
                    GL_CashAuth_PRI("Update", hdnid.Value);
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Cannot Save,No Record Found in Receipt');", true);
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDeatils", "alert('" + ex.Message + "');", true);
        }

    }

    public void GL_CashAuth_PRV(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Gl_CashAuth_PRV";
        cmd.Parameters.AddWithValue("@operation", operation);
        cmd.Parameters.AddWithValue("@CAUTHID", value);
        cmd.Parameters.AddWithValue("@demoAmt", Convert.ToInt32(hdndemoAmt.Value));
        cmd.Parameters.AddWithValue("@finalizedAmt", Convert.ToInt32(hdnfinalizedAmt.Value));
        cmd.ExecuteNonQuery();
        conn.Close();
    }
    public void GL_CashAuth_PRI(string operation, string value)
    {
        datasaved = false;
        Reftype = "CAUTH";
        conn = new SqlConnection(strConnString);
        conn.Open();
        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");
        flag = "finalized";
        int lineno = 0;
        for (int j = 0; j < dgvReceiptsDetails.Rows.Count; j++)
        {

            dgvReceiptsDetails.SelectedIndex = j;

            CheckBox chkBx = (CheckBox)dgvReceiptsDetails.SelectedRow.FindControl("chkfinalized");
            Label lblreceiptid = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceiptid");
            HiddenField hndSDID = (HiddenField)dgvReceiptsDetails.SelectedRow.FindControl("hndSDID");
            HiddenField hndKYCID = (HiddenField)dgvReceiptsDetails.SelectedRow.FindControl("hndKYCID");
            Label lblgoldloanno = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblgoldloanno");
            Label lblname = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblname");
            Label lblreceiptdate = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceiptdate");
            Label lblmode = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblmode");
            Label lblreceiptamt = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceiptamt");
            Label lblreceivedcash = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceivedcash");
            Label lblreceiptNo = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceiptNo");
            Label lblprincipal = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblprincipal");
            Label lblInterest = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblInterest");
            Label lblpenalint = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblpenalint");
            Label lblcharges = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblcharges");

            if (operation == "Save")
            {
                if (chkBx != null && chkBx.Checked)
                {
                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_CashAuth_PRI";
                    cmd.Parameters.AddWithValue("@operation", operation);
                    cmd.Parameters.AddWithValue("@CAUTHID", value);
                    cmd.Parameters.AddWithValue("@RefType", Reftype);
                    cmd.Parameters.AddWithValue("@RefNo", 0);
                    cmd.Parameters.AddWithValue("@ReferenceType", "re");
                    cmd.Parameters.AddWithValue("@Ref_date", gbl.ChangeDateMMddyyyy(txtdate.Text));
                    cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);
                    cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                    cmd.Parameters.AddWithValue("@CMPID", "1");
                    cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
                    cmd.Parameters.AddWithValue("@RcptID", lblreceiptid.Text);
                    cmd.Parameters.AddWithValue("@SDID", hndSDID.Value);
                    cmd.Parameters.AddWithValue("@KYCID", hndKYCID.Value);
                    cmd.Parameters.AddWithValue("@GoldLoanNo", lblgoldloanno.Text);
                    cmd.Parameters.AddWithValue("@Name", lblname.Text);
                    cmd.Parameters.AddWithValue("@ReceiptDate", gbl.ChangeDateMMddyyyy(lblreceiptdate.Text));
                    cmd.Parameters.AddWithValue("@PaymentMode", lblmode.Text);
                    cmd.Parameters.AddWithValue("@Receivedcash", lblreceivedcash.Text);
                    cmd.Parameters.AddWithValue("@ReceiptAmount", lblreceiptamt.Text);
                    cmd.Parameters.AddWithValue("@ReceiptNo", lblreceiptNo.Text);
                    cmd.Parameters.AddWithValue("@Principal", lblprincipal.Text);
                    cmd.Parameters.AddWithValue("@Interest", lblInterest.Text);
                    cmd.Parameters.AddWithValue("@PenalInterest", lblpenalint.Text);
                    cmd.Parameters.AddWithValue("@Charges", lblcharges.Text);
                    cmd.Parameters.AddWithValue("@Finalized", 1);
                    cmd.Parameters.AddWithValue("@LineNo", lineno);
                    cmd.Parameters.AddWithValue("@flag", flag);
                    result = cmd.ExecuteNonQuery();
                    lineno = lineno + 1;

                }
            }

            if (operation == "Update" || operation == "Delete")
            {

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.Transaction = transactionGL;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "GL_CashAuth_PRI";
                cmd.Parameters.AddWithValue("@operation", operation);
                cmd.Parameters.AddWithValue("@CAUTHID", value);
                cmd.Parameters.AddWithValue("@RefType", Reftype);
                cmd.Parameters.AddWithValue("@RefNo", 0);
                cmd.Parameters.AddWithValue("@ReferenceType", "re");
                cmd.Parameters.AddWithValue("@Ref_date", gbl.ChangeDateMMddyyyy(txtdate.Text));
                cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);
                cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                cmd.Parameters.AddWithValue("@CMPID", "1");
                cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
                cmd.Parameters.AddWithValue("@RcptID", lblreceiptid.Text);
                cmd.Parameters.AddWithValue("@SDID", hndSDID.Value);
                cmd.Parameters.AddWithValue("@KYCID", hndKYCID.Value);
                cmd.Parameters.AddWithValue("@GoldLoanNo", lblgoldloanno.Text);
                cmd.Parameters.AddWithValue("@Name", lblname.Text);
                cmd.Parameters.AddWithValue("@ReceiptDate", gbl.ChangeDateMMddyyyy(lblreceiptdate.Text));
                cmd.Parameters.AddWithValue("@PaymentMode", lblmode.Text);
                cmd.Parameters.AddWithValue("@Receivedcash", lblreceivedcash.Text);
                cmd.Parameters.AddWithValue("@ReceiptAmount", lblreceiptamt.Text);
                cmd.Parameters.AddWithValue("@ReceiptNo", lblreceiptNo.Text);
                cmd.Parameters.AddWithValue("@Principal", lblprincipal.Text);
                cmd.Parameters.AddWithValue("@Interest", lblInterest.Text);
                cmd.Parameters.AddWithValue("@PenalInterest", lblpenalint.Text);
                cmd.Parameters.AddWithValue("@Charges", lblcharges.Text);

                if (chkBx != null && chkBx.Checked)
                {
                    cmd.Parameters.AddWithValue("@Finalized", 1);
                }
                else
                {

                    cmd.Parameters.AddWithValue("@Finalized", 0);
                }
                cmd.Parameters.AddWithValue("@LineNo", lineno);
                cmd.Parameters.AddWithValue("@flag", flag);
                result = cmd.ExecuteNonQuery();
                lineno = lineno + 1;

            }
        }
        //result = 1;
        if (result > 0)
        {

            datasaved = true;

            if (datasaved)
            {
                string hnddenoid = "";

                flag = "Deno";




                for (int j = 0; j <= gvDenominationDetails.Rows.Count; j++)
                {

                    int i = gvDenominationDetails.Rows.Count;
                    int k = gvDenominationDetails.Rows.Count - 1;


                    gvDenominationDetails.SelectedIndex = j;


                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_CashAuth_PRI";
                    cmd.Parameters.AddWithValue("@operation", operation);
                    cmd.Parameters.AddWithValue("@CAUTHID", value);
                    cmd.Parameters.AddWithValue("@flag", flag);

                    if (j <= 6)
                    {
                        HiddenField hdndenoid = (HiddenField)gvDenominationDetails.SelectedRow.FindControl("hdndenoid");
                        TextBox gvtxtDenoSrno = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoSrno");
                        TextBox gvtxtDenoDescription = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoDescription");
                        TextBox gvtxtDenoNo = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNo");
                        TextBox gvtxtDenoTotal = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoTotal");
                        TextBox gvtxtDenoNoteNos = (TextBox)gvDenominationDetails.SelectedRow.FindControl("gvtxtDenoNoteNos");
                        cmd.Parameters.AddWithValue("@DenoId", hdndenoid.Value);
                        cmd.Parameters.AddWithValue("@Serialno", gvtxtDenoSrno.Text);
                        cmd.Parameters.AddWithValue("@DenoRs", gvtxtDenoDescription.Text);
                        cmd.Parameters.AddWithValue("@Quantity", gvtxtDenoNo.Text);
                        cmd.Parameters.AddWithValue("@Total", gvtxtDenoTotal.Text);
                        cmd.Parameters.AddWithValue("@NoteNos", gvtxtDenoNoteNos.Text);

                        if (j == 6)
                        {
                            hnddenoid = hdndenoid.Value;
                        }

                    }
                    if (j > 6)
                    {

                        TextBox gvtxtDenocoins = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenocoins");
                        TextBox gvtxtDenonocoin = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenonocoin");


                        cmd.Parameters.AddWithValue("@DenoId", Convert.ToInt32(hnddenoid) + 1);
                        cmd.Parameters.AddWithValue("@Serialno", gvDenominationDetails.Rows.Count + 1);
                        cmd.Parameters.AddWithValue("@DenoRs", gvtxtDenocoins.Text);
                        cmd.Parameters.AddWithValue("@Quantity", gvtxtDenonocoin.Text);
                        cmd.Parameters.AddWithValue("@Total", gvtxtDenocoins.Text);
                        cmd.Parameters.AddWithValue("@NoteNos", "");


                    }

                    cmd.Parameters.AddWithValue("@LineNo", 1);



                    result = cmd.ExecuteNonQuery();


                }
                if (result > 0)
                {
                    datasaved = true;
                    // conn.Close();
                    transactionGL.Commit();
                }







            }

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


    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {

            if (dgvReceiptsDetails.Rows.Count > 0)
            {
                TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
                Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");

                lblgrandtotal.Text = "0";
                for (int j = 0; j < dgvReceiptsDetails.Rows.Count; j++)
                {

                    dgvReceiptsDetails.SelectedIndex = j;
                    CheckBox chkBx1 = (CheckBox)dgvReceiptsDetails.SelectedRow.FindControl("chkfinalized");
                    if (chkBx1 != null && chkBx1.Checked)
                    {
                        Label lblreceivedcash = (Label)dgvReceiptsDetails.SelectedRow.FindControl("lblreceivedcash");
                        if (lblreceivedcash.Text != "")
                        {
                            amt = Convert.ToInt32(lblreceivedcash.Text);

                            if (lblgrandtotal.Text == "")
                            {
                                lblgrandtotal.Text = "0";
                            }

                            grdamt = Convert.ToInt32(lblgrandtotal.Text);

                            grdamt = grdamt + amt;

                            lblgrandtotal.Text = grdamt.ToString();
                        }
                    }
                }
                if (gvtxtDenoTotalAmt.Text == "0")
                {
                    hdndemoAmt.Value = "0";
                }
                else
                {
                    hdndemoAmt.Value = gvtxtDenoTotalAmt.Text;
                }

                if (lblgrandtotal.Text == "0")
                {
                    hdnfinalizedAmt.Value = "0";

                }
                else
                {
                    hdnfinalizedAmt.Value = lblgrandtotal.Text;
                }
                GL_CashAuth_PRV("Delete", hdnid.Value);
                GL_CashAuth_PRI("Delete", hdnid.Value);
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "Alert", "alert('Cannot Delete,Check One Receipt');", true);
            }
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDetails", "alert('" + ex.Message + "');", true);
            //gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {

            Popup_RTR();

            hdnpopup.Value = "View";
            Master.PropertytxtSearch.Text = "";
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Reference Type");
            Master.PropertyddlSearch.Items.Add("Reference Date");

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDetails", "alert('" + ex.Message + "');", true);
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
            hdnpopup.Value = "Cancel";
            ClearData();

            gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            CashAuth_Search();
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "CandidateDetails", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }

    public void CashAuth_Search()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashAuth_Search";
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
            string date = Master.PropertygvGlobal.SelectedRow.Cells[2].Text;
            //string date = hdnRefdate.Value;
            if (hdnpopup.Value == "Edit")
            {
                CashAuth_Details_RTR(id, date);
            }
            if (hdnpopup.Value == "View")
            {
                // CashAuth_Details_VD_RTR(id); 
                CashAuth_Details_RTR(id, date);
            }
            hdnoperation.Value = "Update";
            gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }


    }

    public void CashAuth_Details_VD_RTR(string id)
    {
        int tot = 0;
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashAuthDetails_VD_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@CAUTHID", id);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        ds = new DataSet();
        da.Fill(ds);
        hdnid.Value = id;
        if (ds.Tables[0].Rows.Count > 0)
        {
            txtreferenceno.Text = ds.Tables[0].Rows[0]["ReferenceType"].ToString();
            string totalamt = ds.Tables[0].Rows[0]["TotalAmount"].ToString();
            DateTime dte = Convert.ToDateTime(ds.Tables[0].Rows[0]["Ref_date"]);
            txtdate.Text = dte.ToString("dd/MM/yyyy HH:mm:ss");


            dgvReceiptsDetails.DataSource = ds.Tables[0];
            dgvReceiptsDetails.DataBind();







            Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");
            lblgrandtotal.Text = "" + totalamt;

        }
        if (ds.Tables[1].Rows.Count > 0)
        {
            gvDenominationDetails.DataSource = ds.Tables[1];
            gvDenominationDetails.DataBind();

            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {


                if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "1000")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "500")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "100")
                {

                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }

                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "50")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "20")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "10")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "5")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else
                {
                    if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == ds.Tables[1].Rows[i]["Total"].ToString())
                    {
                        if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "0")
                        {

                            denocoin = 0;
                        }
                        else
                        {
                            denocoin = Convert.ToInt32(ds.Tables[1].Rows[i]["DenoRs"]);
                        }

                        if (ds.Tables[1].Rows[i]["Quantity"].ToString() == "0")
                        {
                            qtycoin = 0;
                        }
                        else
                        {
                            qtycoin = Convert.ToInt32(ds.Tables[1].Rows[i]["Quantity"]);
                        }



                        if ((denocoin != qtycoin))
                        {


                            ds.Tables[1].Rows.RemoveAt(i);

                            gvDenominationDetails.DataSource = ds.Tables[1];
                            gvDenominationDetails.DataBind();

                            TextBox gvtxtDenocoins = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenocoins");
                            gvtxtDenocoins.Text = "" + denocoin;
                            TextBox gvtxtDenonocoin = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenonocoin");
                            gvtxtDenonocoin.Text = "" + qtycoin;

                            tot = tot + Convert.ToInt32(denocoin);

                        }
                        else if (denocoin == qtycoin)
                        {
                            ds.Tables[1].Rows.RemoveAt(i);

                            gvDenominationDetails.DataSource = ds.Tables[1];
                            gvDenominationDetails.DataBind();

                            TextBox gvtxtDenocoins = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenocoins");
                            gvtxtDenocoins.Text = "" + denocoin;
                            TextBox gvtxtDenonocoin = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenonocoin");
                            gvtxtDenonocoin.Text = "" + qtycoin;

                            tot = tot + Convert.ToInt32(denocoin);

                        }

                    }
                }




            }

            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            gvtxtDenoTotalAmt.Text = "" + tot;

        }
    }

    public void CashAuth_Details_RTR(string id, string date)
    {
        int tot = 0;
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_CashAuthDetails_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@CAUTHID", id);
        cmd.Parameters.AddWithValue("@FYID", Session["FYearID"].ToString());
        cmd.Parameters.AddWithValue("@BranchId", Session["branchId"].ToString());
        cmd.Parameters.AddWithValue("@Ref_Date", date);
        ds = new DataSet();
        da.Fill(ds);
        hdnid.Value = id;
        if (ds.Tables[0].Rows.Count > 0)
        {
            txtreferenceno.Text = ds.Tables[0].Rows[0]["ReferenceType"].ToString();
            string totalamt = ds.Tables[0].Rows[0]["TotalAmount"].ToString();
            DateTime dte = Convert.ToDateTime(ds.Tables[0].Rows[0]["Ref_date"]);
            txtdate.Text = dte.ToString("dd/MM/yyyy HH:mm:ss");


            dgvReceiptsDetails.DataSource = ds.Tables[0];
            dgvReceiptsDetails.DataBind();
            Master.PropertybtnEdit.OnClientClick = "return valid();";

            Label lblgrandtotal = (Label)dgvReceiptsDetails.FooterRow.FindControl("lblgrandtotal");
            lblgrandtotal.Text = "" + totalamt;

        }
        if (ds.Tables[1].Rows.Count > 0)
        {
            gvDenominationDetails.DataSource = ds.Tables[1];
            gvDenominationDetails.DataBind();

            for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
            {


                if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "1000")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "500")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "100")
                {

                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }

                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "50")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "20")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "10")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "5")
                {
                    tot = tot + Convert.ToInt32(ds.Tables[1].Rows[i]["Total"].ToString().Trim());
                }
                else
                {
                    if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == ds.Tables[1].Rows[i]["Total"].ToString())
                    {
                        if (ds.Tables[1].Rows[i]["DenoRs"].ToString() == "0")
                        {

                            denocoin = 0;
                        }
                        else
                        {
                            denocoin = Convert.ToInt32(ds.Tables[1].Rows[i]["DenoRs"]);
                        }

                        if (ds.Tables[1].Rows[i]["Quantity"].ToString() == "0")
                        {
                            qtycoin = 0;
                        }
                        else
                        {
                            qtycoin = Convert.ToInt32(ds.Tables[1].Rows[i]["Quantity"]);
                        }


                        if ((denocoin != qtycoin))
                        {


                            ds.Tables[1].Rows.RemoveAt(i);

                            gvDenominationDetails.DataSource = ds.Tables[1];
                            gvDenominationDetails.DataBind();

                            TextBox gvtxtDenocoins = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenocoins");
                            gvtxtDenocoins.Text = "" + denocoin;
                            TextBox gvtxtDenonocoin = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenonocoin");
                            gvtxtDenonocoin.Text = "" + qtycoin;

                            tot = tot + Convert.ToInt32(denocoin);

                        }
                        else if (denocoin == qtycoin)
                        {
                            ds.Tables[1].Rows.RemoveAt(i);

                            gvDenominationDetails.DataSource = ds.Tables[1];
                            gvDenominationDetails.DataBind();

                            TextBox gvtxtDenocoins = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenocoins");
                            gvtxtDenocoins.Text = "" + denocoin;
                            TextBox gvtxtDenonocoin = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenonocoin");
                            gvtxtDenonocoin.Text = "" + qtycoin;

                            tot = tot + Convert.ToInt32(denocoin);

                        }


                    }
                }




            }

            TextBox gvtxtDenoTotalAmt = (TextBox)gvDenominationDetails.FooterRow.FindControl("gvtxtDenoTotalAmt");
            gvtxtDenoTotalAmt.Text = "" + tot;

        }


    }
    protected void ClearData()
    {

        hdnoperation.Value = "Save";
        hdnid.Value = "0";
        AutogenerateRefNo();
        DateTime dte1 = DateTime.Now;
        string date = dte1.ToString("dd/MM/yyyy HH:mm:ss");
        txtdate.Text = date;
        BindDenominationDetails();
        //BindReceiptDetails();
        BindGoldLoanReceiptDetails();



    }

    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {
        hdnpopup.Value = "Cancel";
        BindDenominationDetails();
        BindGoldLoanReceiptDetails();
        ////CheckBox chkBx = (CheckBox)dgvReceiptsDetails.FindControl("chkfinalized");
        //chkBx.Enabled = true;
        Master.PropertympeGlobal.Hide();
        BlankGv();
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


}