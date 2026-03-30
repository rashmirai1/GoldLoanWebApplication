using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;


public partial class GLInterestCalculator : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd, cmdRcpt, cmdRoiRow;
    DataTable dt;
    GlobalSettings st = null;
    DataRow dr = null;
    GlobalSettings gbl = new GlobalSettings();
    int result = 0;
    #endregion [Declarations]

    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        //Master.PropertybtnEdit.Click += new EventHandler(PropertybtnEdit_Click);
        //Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        //Master.PropertybtnDelete.Click += new EventHandler(PropertybtnDelete_Click);
        Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
        Master.PropertyImgBtnClose.Click += new ImageClickEventHandler(PropertyImgBtnClose_Click);
        Master.PropertygvGlobal.RowCommand += new GridViewCommandEventHandler(PropertygvGlobal_RowCommand);
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]

    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
    }

    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.PropertybtnView.OnClientClick = "return valid();";
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");
                }
                //Master.PropertybtnCancel.Visible = false;
                Master.PropertybtnDelete.Visible = false;
                Master.PropertybtnEdit.Visible = false;
                // Master.PropertybtnView.Visible = false;
                Master.PropertybtnSave.Visible = false;

                txtLoanAmount.Enabled = false;
                txtRoi.Enabled = false;
                imgbtnExCustomer.Enabled = false;
                //txtGoldLoanNo.Enabled = false;
                GetcurrentDate();
                gridbind();
                txtGoldLoanNo.Attributes.Add("readonly", "readonly");
                LoanHistory_RTR();
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [Page_Load]

    public void gridbind()
    {
        dt = new DataTable();
        dt.Columns.Add("ndays");
        gvDetails.DataSource = dt;
        gvDetails.DataBind();
    }
    public void GetcurrentDate()
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select convert(varchar(12),getdate(),103)";

        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            txtDate.Text = Convert.ToString(cmd.ExecuteScalar());
        }
    }
    public void ExCustomer_RTR()
    {

        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_IntCal_ExCustomer_RTR";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }
    public void EXCustomer_Search()
    {
        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_IntCal_ExCustomer_Search";
        cmd.Parameters.AddWithValue("@SearchCateria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchValue", Master.PropertytxtSearch.Text.Trim());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        Master.PropertympeGlobal.Show();
    }

    public void Calculate_RTR()
    {
        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_IntCal_RTR_New";
        cmd.Parameters.AddWithValue("@LoanType", ddlLoanType.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtDate.Text));
        cmd.Parameters.AddWithValue("@GoldLoanNO", txtGoldLoanNo.Text);

        if (txtLoanAmount.Text != "")
        {
            cmd.Parameters.AddWithValue("@LoanAmount", txtLoanAmount.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@LoanAmount", "0");
        }
        if (txtRoi.Text != "")
        {
            cmd.Parameters.AddWithValue("@ROI", txtRoi.Text);
        }
        else
        {
            cmd.Parameters.AddWithValue("@ROI", "0");
        }
        cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);

        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        if (ddlLoanType.SelectedIndex == 1)
        {
            gvDetails.DataSource = dt;
            gvDetails.DataBind();
        }

        if (ddlLoanType.SelectedIndex == 2)
        {
            double total = 0;
            int neworold = 0;
            int RowRoiID = 1;
            DataRow drr = null;
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";

            if (cmd.ExecuteScalar() != DBNull.Value)
            {
                neworold = Convert.ToInt32(cmd.ExecuteScalar());
            }


            //Added for to get ROI ROW ID on 3-10-2015
            int RcptID = 0;
            cmdRcpt = new SqlCommand();
            cmdRcpt.Connection = conn;
            cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where  GoldLoanNo='" + dt.Rows[0]["GoldLoanNo"].ToString() + "'";
            if (cmdRcpt.ExecuteScalar() != DBNull.Value)
            {
                RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
            }


            //Check if Advance Interest is paid then Pass top 1 RowID
            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;              
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) > 0)
            {
              
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;            
                cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + dt.Rows[0]["SID"].ToString();
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
            else if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(dt.Rows[0]["AdvInterestAmount"].ToString()) == 0)
            {
                cmdRoiRow = new SqlCommand();
                cmdRoiRow.Connection = conn;
                cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                {
                    RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                }
            }
                       
            string LastDate, LastCLI, LastLoanAmount = string.Empty;
            if (dt.Rows[0]["LastInterestDate"].ToString() != "" && dt.Rows[0]["LastInterestDate"].ToString() != null)
            {
                LastDate = dt.Rows[0]["LastInterestDate"].ToString();
            }
            else
            {
                LastDate = dt.Rows[0]["LastReceiveDate"].ToString();
            }


            if (dt.Rows[0]["CLI"].ToString() != "" && dt.Rows[0]["CLI"].ToString() != null)
            {
                LastCLI = dt.Rows[0]["CLI"].ToString();
            }
            else
            {
                LastCLI = "0";
            }

            string InterestFromDate = string.Empty;
            if (dt.Rows[0]["InterestFromDate"].ToString() != "" && dt.Rows[0]["InterestFromDate"].ToString() != null)
            {
                InterestFromDate = dt.Rows[0]["InterestFromDate"].ToString();
            }
            else
            {
                InterestFromDate = System.DateTime.Today.ToShortDateString();
            }
            string InterestToDate = string.Empty;
            if (dt.Rows[0]["InterestToDate"].ToString() != "" && dt.Rows[0]["InterestToDate"].ToString() != null)
            {
                InterestToDate = dt.Rows[0]["InterestToDate"].ToString();
            }
            else
            {
                InterestToDate = System.DateTime.Today.ToShortDateString();
            }

            string RvcCLI = string.Empty;
            if (dt.Rows[0]["RecvInterest"].ToString() != "" && dt.Rows[0]["RecvInterest"].ToString() != null)
            {
                RvcCLI = dt.Rows[0]["RecvInterest"].ToString();
            }
            else
            {
                RvcCLI = "0";
            }

            string AdvInterestFromDate = string.Empty;
            if (dt.Rows[0]["AdvInterestFromDate"].ToString() != "" && dt.Rows[0]["AdvInterestFromDate"].ToString() != null)
            {
                AdvInterestFromDate = dt.Rows[0]["AdvInterestFromDate"].ToString();
            }
            else
            {
                AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
            }
            string AdvInterestToDate = string.Empty;
            if (dt.Rows[0]["AdvInterestToDate"].ToString() != "" && dt.Rows[0]["AdvInterestToDate"].ToString() != null)
            {
                AdvInterestToDate = dt.Rows[0]["AdvInterestToDate"].ToString();
            }
            else
            {
                AdvInterestToDate = System.DateTime.Today.ToShortDateString();
            }

            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_EmiCalculator_RTR";

            cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["CustLoanDate"].ToString()));

            if (Convert.ToDecimal(dt.Rows[0]["OSIntAmt"].ToString()) >= 0)
            {
                double AddPrvInt = Math.Round(Convert.ToDouble(dt.Rows[0]["LoanAmout"].ToString()) + Convert.ToDouble(dt.Rows[0]["OSIntAmt"].ToString()));
                cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
            }
            else
            {
                cmd.Parameters.AddWithValue("@LoanAmount", (dt.Rows[0]["LoanAmout"].ToString()));
            }

            cmd.Parameters.AddWithValue("@SID", dt.Rows[0]["SID"].ToString());
            cmd.Parameters.AddWithValue("@NeworOld", neworold);

            cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

            cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
            cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
            cmd.Parameters.AddWithValue("@OSIntAmt", dt.Rows[0]["OSIntAmt"].ToString());

            cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
            cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
            cmd.Parameters.AddWithValue("@AdvInterestAmt", dt.Rows[0]["AdvInterestAmount"].ToString());

            cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(dt.Rows[0]["LastReceiveDate"].ToString()));
            cmd.Parameters.AddWithValue("@CalculateToDate", gbl.ChangeDateMMddyyyy(txtDate.Text));
            cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

            da = new SqlDataAdapter(cmd);

            DataTable dtt = new DataTable();
             da.Fill(dtt);

            if (dtt.Rows.Count > 0)
            {
                for (int i = 0; i < dtt.Rows.Count; i++)
                {
                    total = total + Convert.ToDouble(dtt.Rows[i]["InterestAmount"].ToString());
                }
            }

            drr = dtt.NewRow();
            drr["FromDate"] = "";
            drr["ToDate"] = "";
            drr["TotalDays"] = "";
            drr["AllDaysTillDate"] = "";
            drr["LoanAmount"] = "";
            drr["ROI"] = "";
            drr["InterestAmount"] = Math.Round(total);// -Convert.ToDouble(RvcCLI);
            dtt.Rows.Add(drr);
            dtt.Columns.Remove("ROIID");
            gvDetails.DataSource = dtt;
            gvDetails.DataBind();

            //DataTable dtt = new DataTable();
            //dtt.Columns.Add("InterestType");
            //dtt.Columns.Add("InterestAmount");

            ////----------------for interest outstanding------------------------------------
            //if (Convert.ToDouble(dt.Rows[0]["CLI"].ToString()) > 0)
            //{
            //    drr = dtt.NewRow();
            //    drr["InterestType"] = "Previous Interest Outstanding";
            //    drr["InterestAmount"] = dt.Rows[0]["CLI"].ToString();
            //    total = Convert.ToDouble(dt.Rows[0]["CLI"].ToString());
            //    dtt.Rows.Add(drr);
            //}
            //----------------end interest outstanding------------------------------------

            //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //{
            //    if (ds.Tables[0].Rows[i]["IntAMT"].ToString() == "")
            //    {
            //        ds.Tables[0].Rows[i]["IntAMT"] = "0";
            //    }
            //    if (Convert.ToDouble(ds.Tables[0].Rows[i]["IntAMT"].ToString()) > 0)
            //    {

            //        drr = dtt.NewRow();
            //        drr["InterestType"] = "On " + ds.Tables[0].Rows[i]["BalancePayableAMT"].ToString() + " @ " + ds.Tables[0].Rows[i]["ROI"].ToString() + " for " + ds.Tables[0].Rows[i]["Days"].ToString() + " days";
            //        drr["InterestAmount"] = ds.Tables[0].Rows[i]["IntAMT"].ToString();
            //        total = total + Convert.ToDouble(ds.Tables[0].Rows[i]["IntAMT"].ToString());
            //        dtt.Rows.Add(drr);
            //    }
            //}

            //if (total > 0)
            //{
            //    drr = dtt.NewRow();
            //    drr["InterestType"] = "";
            //    drr["InterestAmount"] = total.ToString();

            //    dtt.Rows.Add(drr);
            //}

            //gvDetails.DataSource = dtt;
            //gvDetails.DataBind();
        }

    }

    public void LoanHistory_RTR()
    {
        conn = new SqlConnection(strConnString);

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_IntCal_Loan_RTR_New";
        if (ddlLoanType.SelectedIndex == 2)
        {
            cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
        }
        else
        {
            cmd.Parameters.AddWithValue("@KYCID", "0");
        }
        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtDate.Text));
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);
        gvHistory.DataSource = dt;
        gvHistory.DataBind();

    }

    public void Calculate_PRV()
    {

        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_IntCal_PRV_New";
        cmd.Parameters.AddWithValue("@KYCID", hdnkycid.Value);
        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(txtDate.Text));
        cmd.ExecuteNonQuery();
        conn.Close();
    }

    protected void imgbtnExCustomer_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            ExCustomer_RTR();
            Master.PropertyddlSearch.Items.Clear();
            Master.PropertyddlSearch.Items.Add("Customer ID");
            Master.PropertyddlSearch.Items.Add("Applied Date");
            Master.PropertyddlSearch.Items.Add("Gold Loan No");
            Master.PropertyddlSearch.Items.Add("Customer Name");
            Master.PropertyddlSearch.Items.Add("Loan Date");
            Master.PropertyddlSearch.Items.Add("PAN No");
            Master.PropertyddlSearch.Items.Add("Mobile No");
            Master.PropertytxtSearch.Text = "";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            EXCustomer_Search();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            if (ddlLoanType.SelectedIndex == 2)
            {
                Calculate_PRV();
            }
            Calculate_RTR();
            LoanHistory_RTR();
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
            Response.Redirect("GLInterestCalculator.aspx");
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {

    }
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            hdnkycid.Value = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;
            txtGoldLoanNo.Text = Master.PropertygvGlobal.SelectedRow.Cells[3].Text;
            txtLoanAmount.Text = "";
            txtRoi.Text = "";
            txtLoanAmount.Enabled = false;
            txtRoi.Enabled = false;

            LoanHistory_RTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void gvDetails_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Center;
            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;

            //e.Row.Cells[0].ForeColor=System.Drawing.Color."#1f497d";

            if (ddlLoanType.SelectedIndex == 2)
            {

                txtLoanAmount.Enabled = false;
                txtRoi.Enabled = false;
                imgbtnExCustomer.Enabled = true;
            }
            if (ddlLoanType.SelectedIndex == 1)
            {
                txtLoanAmount.Enabled = true;
                txtRoi.Enabled = true;
                imgbtnExCustomer.Enabled = false;
            }
        }
    }
    protected void gvHistory_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].HorizontalAlign = HorizontalAlign.Left;
            e.Row.Cells[0].BackColor = System.Drawing.ColorTranslator.FromHtml("#1f497d");
            e.Row.Cells[0].ForeColor = System.Drawing.ColorTranslator.FromHtml("#ffffff");

            e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Right;
        }
    }
}