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
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;

public partial class GLSMSRemainder : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;


    GlobalSettings gbl = new GlobalSettings();

    bool datasaved = false;
    //Declaring Variables.
    int result = 0;


    //Declaring Objects.
    SqlTransaction transactionGL;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    System.Data.DataTable dt = new System.Data.DataTable();
    //DataTable dt1;
    SqlCommand cmd, cmdRoiRow, cmdRcpt;
    int cnt = 0;


    #endregion [Declarations]
    protected void Page_Init(object sender, EventArgs e)
    {
        // Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
    }
    protected void Page_PreRender(Object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Visible = false;
        Master.PropertybtnSave.Visible = false;
        Master.PropertybtnDelete.Visible = false;
        Master.PropertybtnSearch.Visible = false;
        Master.PropertybtnView.Visible = false;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnSave.Visible = false;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnSearch.Visible = false;
            Master.PropertybtnView.Visible = false;

            if (!IsPostBack)
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
                //----------------------------------------
                Rdbremainder.SelectedIndex = 0;
                binddate();
                txtdate.Attributes.Add("readonly", "readonly");
                txtsmstemplate.Attributes.Add("readonly", "readonly");
                txtsmsdesc.Attributes.Add("readonly", "readonly");
                if (Rdbremainder.SelectedIndex == 0)
                {
                    // txtsmstemplate.Text = "Dear Customer, your interest of Rs.___ towards GLNo.____ is due on __/__/__. Kindly arrange to make the payment.";
                    txtsmstemplate.Text = "Dear Customer, Interest of Rs.___ towards GLNo.____  is due on  __/__/__. Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952";
                    txtsmsdesc.Text = "8 Days Prior to Interest Due Date";
                    GetSmsDetails(Rdbremainder.SelectedIndex);
                }
            }
            gbl.CheckAEDControlSettings("", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);



        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLSMSRemdinder", "alert('" + ex.Message + "');", true);
        }
    }

    public void binddate()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        conn.Open();
        cmd.Connection = conn;
        cmd.CommandText = "select Getdate() as 'Date'";
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        if (dt.Rows.Count > 0)
        {
            txtdate.Text = dt.Rows[0]["Date"].ToString();
            //txtdate.Text = "30/09/2016";   //"28/03/2015";

        }
        conn.Close();
    }
    protected void btnsend_Click(object sender, EventArgs e)
    {
        try
        {

            Send_Message();
            //return;
            for (int i = 0; i < dgvRemainder.Rows.Count; i++)
            {
                dgvRemainder.SelectedIndex = i;
                System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)dgvRemainder.SelectedRow.FindControl("chkselect");
                if (chkBx1.Checked)
                {
                    cnt = cnt + 1;
                }


                System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)dgvRemainder.HeaderRow.FindControl("checkAll");
                if (chk.Checked)
                {

                    cnt = cnt + 1;
                }


            }
            if (cnt == 0)
            {
                ClientScript.RegisterStartupScript(this.GetType(), "", "alert('Check Any One Gold Loan No');", true);

            }
            if (cnt > 0)
            {
                GL_SMS_PRI("Save");
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLSMSRemdinder", "alert('" + ex.Message + "');", true);
        }
    }
    #region [Send_Mobile_Messages]
    public void Send_Message()
    {
        try
        {
            string Message = string.Empty;
            string sURL;
            string Success = string.Empty;
            double interestamt = 0;
            double outstandingamt = 0;
            double totalamt = 0;
            string Source = "AphGLN";
            string ApiKey = "pfe7kvcaoOfF88SHC7ceumB8TONysHCBMDHLTC8J1/4=";
            string ClientID = "8e3127b5-03a3-46ed-8da9-4d545083c9a9";
            ApiKey = Uri.EscapeDataString(ApiKey);
            if (dgvRemainder.Rows.Count > 0)
            {
                for (int j = 0; j < dgvRemainder.Rows.Count; j++)
                {
                    dgvRemainder.SelectedIndex = j;

                    interestamt = 0;
                    outstandingamt = 0;
                    totalamt = 0;
                    Message = "";

                    CheckBox chkBx1 = (CheckBox)dgvRemainder.SelectedRow.FindControl("chkselect");
                    if (chkBx1 != null && chkBx1.Checked)
                    {
                        Label lblgoldloanno = (Label)dgvRemainder.SelectedRow.FindControl("lblgoldloanno");
                        Label lblname = (Label)dgvRemainder.SelectedRow.FindControl("lblname");
                        Label lblmobileno = (Label)dgvRemainder.SelectedRow.FindControl("lblmobileno");
                        Label lbllastreceiveddate = (Label)dgvRemainder.SelectedRow.FindControl("lbllastreceiveddate");
                        Label lbloutstanding = (Label)dgvRemainder.SelectedRow.FindControl("lbloutstanding");
                        Label lblinterest = (Label)dgvRemainder.SelectedRow.FindControl("lblinterest");
                        Label lblsdid = (Label)dgvRemainder.SelectedRow.FindControl("lblsdid");
                        Label lblkycid = (Label)dgvRemainder.SelectedRow.FindControl("lblkycid");
                        Label lblduedate = (Label)dgvRemainder.SelectedRow.FindControl("lblduedate");

                        interestamt = Convert.ToDouble(lblinterest.Text);
                        outstandingamt = Convert.ToDouble(lbloutstanding.Text);
                        totalamt = interestamt + outstandingamt;

                       // lblmobileno.Text = "9960722146";

                        if (Rdbremainder.SelectedIndex == 0)
                        {
                             //Messagehttp://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + lblmobileno.Text + "&source=AphGLN&message=Dear Customer, your interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.";
                            // Messagehttp://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + lblmobileno.Text + "&source=AphGLN&message=Dear Customer, Interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ". Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952";
                            //Message = " http://smpp.keepintouch.co.in/vendorsms/pushsms.aspx?user=afplgl&password=afplgl14&msisdn=" + lblmobileno.Text + "&sid=AphGLN&msg=Dear Customer, Interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ". Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952&fl=0&gwid=2";

                            //changes done by ketki mhatre -- start here
                            //  Message = " http://smpp.keepintouch.co.in/vendorsms/pushsms.aspx?user=afplgl&password=afplgl14&msisdn=" + lblmobileno.Text + "&sid=AphGLN&msg=Dear Customer, Interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ". Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952&fl=0&gwid=2";

                            string DelMessage = "Dear Customer, Interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ". Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952";
                            Message = Uri.EscapeDataString(Message);
                            Message = "http://sms.aphelionsoftwares.com:6005/api/v2/SendSMS?SenderId=" + Source + "&Message=" + DelMessage + "&MobileNumbers=" + lblmobileno.Text + "&ApiKey=" + ApiKey + "&ClientId=" + ClientID + "";
                            //changes done by ketki mhatre -- end here
                        }
                        if (Rdbremainder.SelectedIndex == 1)
                        {
                            // Messagehttp://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + lblmobileno.Text + "&source=AphGLN&message=Dear Customer, repayment of your principal towards " + lblgoldloanno.Text + " of Rs." + outstandingamt + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.";
                            //commented by ketki -- strte here
                            //  Message = " http://smpp.keepintouch.co.in/vendorsms/pushsms.aspx?user=afplgl&password=afplgl14&msisdn=" + lblmobileno.Text + "&sid=AphGLN&msg=Dear Customer, repayment of your principal towards " + lblgoldloanno.Text + " of Rs." + outstandingamt + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.&fl=0&gwid=2";

                            string DelMessage = "Dear Customer, repayment of your principal towards " + lblgoldloanno.Text + " of Rs." + outstandingamt + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.";
                            Message = Uri.EscapeDataString(Message);
                            Message = "http://sms.aphelionsoftwares.com:6005/api/v2/SendSMS?SenderId=" + Source + "&Message=" + DelMessage + "&MobileNumbers=" + lblmobileno.Text + "&ApiKey=" + ApiKey + "&ClientId=" + ClientID + "";
                            //commented by ketki -- end here
                        }
                        if (Rdbremainder.SelectedIndex == 2)
                        {

                            //Messagehttp://103.16.101.52:8080/bulksms/bulksms?username=aspl-afplgl&password=afplgl14&type=0&dlr=1&destination=" + lblmobileno.Text + "&source=AphGLN&message=Dear Customer, your loan repayment along with interest amounting to Rs." + totalamt + " towards " + lblgoldloanno.Text + "  was due on" + lblduedate.Text + ".Kindly make the payment at the earliest.";
                            //changes done by ketki mhatre -- start here
                            //Message = " http://smpp.keepintouch.co.in/vendorsms/pushsms.aspx?user=afplgl&password=afplgl14&msisdn=" + lblmobileno.Text + "&sid=AphGLN&msg=Dear Customer, your loan repayment along with interest amounting to Rs." + totalamt + " towards " + lblgoldloanno.Text + "  was due on" + lblduedate.Text + ".Kindly make the payment at the earliest.&fl=0&gwid=2";

                            string DelMessage = "Dear Customer, your loan repayment along with interest amounting to Rs." + totalamt + " towards " + lblgoldloanno.Text + "  was due on" + lblduedate.Text + ".Kindly make the payment at the earliest";
                            Message = Uri.EscapeDataString(Message);
                            Message = "http://sms.aphelionsoftwares.com:6005/api/v2/SendSMS?SenderId=" + Source + "&Message=" + DelMessage + "&MobileNumbers=" + lblmobileno.Text + "&ApiKey=" + ApiKey + "&ClientId=" + ClientID + "";
                            //changes done by ketki mhatre -- End here
                        }
                        StreamReader objReader;
                        sURL = Message;
                        WebRequest wrGETURL;
                        wrGETURL = WebRequest.Create(sURL);

                        Stream objStream;
                        objStream = wrGETURL.GetResponse().GetResponseStream();
                        objReader = new StreamReader(objStream);
                        objReader.Close();
                    }
                }
            }


        }
        catch (Exception ex)
        {

        }
    }
    #endregion [Send_Mobile_Messages]
    #region [SaveRecord]
    public void GL_SMS_PRI(string operation)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        transactionGL = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransactionForGL");
        string SmsText = "";
        double interestamt = 0;
        double outstandingamt = 0;
        double totalamt = 0;
        if (dgvRemainder.Rows.Count > 0)
        {
            for (int j = 0; j < dgvRemainder.Rows.Count; j++)
            {


                dgvRemainder.SelectedIndex = j;

                SmsText = "";
                interestamt = 0;
                outstandingamt = 0;
                totalamt = 0;

                CheckBox chkBx1 = (CheckBox)dgvRemainder.SelectedRow.FindControl("chkselect");

                if (chkBx1 != null && chkBx1.Checked)
                {
                    Label lblgoldloanno = (Label)dgvRemainder.SelectedRow.FindControl("lblgoldloanno");
                    Label lblname = (Label)dgvRemainder.SelectedRow.FindControl("lblname");
                    Label lblmobileno = (Label)dgvRemainder.SelectedRow.FindControl("lblmobileno");
                    Label lbllastreceiveddate = (Label)dgvRemainder.SelectedRow.FindControl("lbllastreceiveddate");
                    Label lbllastreceiveddate1 = (Label)dgvRemainder.SelectedRow.FindControl("lbllastreceiveddate1");
                    Label lbloutstanding = (Label)dgvRemainder.SelectedRow.FindControl("lbloutstanding");
                    Label lblinterest = (Label)dgvRemainder.SelectedRow.FindControl("lblinterest");
                    Label lblsdid = (Label)dgvRemainder.SelectedRow.FindControl("lblsdid");
                    Label lblkycid = (Label)dgvRemainder.SelectedRow.FindControl("lblkycid");
                    Label lblduedate = (Label)dgvRemainder.SelectedRow.FindControl("lblduedate");


                    interestamt = Convert.ToDouble(lblinterest.Text);
                    outstandingamt = Convert.ToDouble(lbloutstanding.Text);
                    totalamt = interestamt + outstandingamt;

                    if (Rdbremainder.SelectedIndex == 0)
                    {
                        //Dear Customer, your interest of Rs.____ towards GL1111 is due on __/__/__. Kindly arrange to make the payment.    
                        SmsText = "Dear Customer, your interest of Rs." + interestamt + " towards " + lblgoldloanno.Text + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.";

                    }
                    if (Rdbremainder.SelectedIndex == 1)
                    {
                        ///Dear Customer, repayment of your principal towards GL1111 of Rs._______is due on __/__/__. Kindly arrange to make the payment.
                        SmsText = "Dear Customer, repayment of your principal towards " + lblgoldloanno.Text + " of Rs." + outstandingamt + " is due on " + lblduedate.Text + ".Kindly arrange to make the payment.";

                    }
                    if (Rdbremainder.SelectedIndex == 2)
                    {
                        ///Dear Customer, your loan repayment along with interest amounting to Rs.____ towards GL1111 was due on ___/____/___.Kindly make the payment at the earliest. 
                        SmsText = "Dear Customer, your loan repayment along with interest amounting to Rs." + totalamt + " towards " + lblgoldloanno.Text + " was due on " + lblduedate.Text + ".Kindly make the payment at the earliest.";

                    }

                    cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.Transaction = transactionGL;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "GL_SMS_PRI";
                    cmd.Parameters.AddWithValue("@operation", operation);
                    cmd.Parameters.AddWithValue("@HistoryID", 0);
                    cmd.Parameters.AddWithValue("@Date_details", gbl.ChangeDateMMddyyyy(txtdate.Text));
                    cmd.Parameters.AddWithValue("@SDID", lblsdid.Text);
                    cmd.Parameters.AddWithValue("@GoldLoanNo", lblgoldloanno.Text);
                    cmd.Parameters.AddWithValue("@KYCID", lblkycid.Text);
                    cmd.Parameters.AddWithValue("@Outstanding", lbloutstanding.Text);
                    cmd.Parameters.AddWithValue("@Interest", lblinterest.Text);
                    cmd.Parameters.AddWithValue("@LastReceivedDate", gbl.ChangeDateMMddyyyy(lbllastreceiveddate1.Text));
                    cmd.Parameters.AddWithValue("@RemainderType", Rdbremainder.SelectedIndex);
                    cmd.Parameters.AddWithValue("@Checked", 1);
                    cmd.Parameters.AddWithValue("@SmsText", SmsText);
                    cmd.Parameters.AddWithValue("@MobileNo", lblmobileno.Text);
                    cmd.Parameters.AddWithValue("@DueDate", gbl.ChangeDateMMddyyyy(lblduedate.Text));
                    cmd.Parameters.AddWithValue("@FYID", hdnfyid.Value);
                    cmd.Parameters.AddWithValue("@CMPID", "1");
                    cmd.Parameters.AddWithValue("@BranchId", hdnbranchid.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", hdnuserid.Value);
                    result = cmd.ExecuteNonQuery();



                }

            }
            if (result > 0)
            {
                transactionGL.Commit();
            }
            else
            {
                transactionGL.Rollback();
            }
            if (result == 1 && operation == "Save")
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "GLCASHINOUT", "alert('Message Send Successfully');", true);
                gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
                Send_Message();// comment for local...
                Clear();
            }

        }
    }
    #endregion [SaveRecord]
    //protected void PropertybtnView_Click(object sender, EventArgs e)
    //{
    //    try
    //    {

    //    }
    //    catch (Exception ex)
    //    {
    //        ClientScript.RegisterStartupScript(this.GetType(), "GLSMSRemdinder", "alert('" + ex.Message + "');", true);
    //    }
    //}
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        Clear();

    }

    public void Clear()
    {
        binddate();
        Rdbremainder.SelectedIndex = 0;
        GetSmsDetails(Rdbremainder.SelectedIndex);
        if (Rdbremainder.SelectedIndex == 0)
        {
            // txtsmstemplate.Text = "Dear Customer, your interest of Rs.___ towards GLNo.____ is due on __/__/__. Kindly arrange to make the payment.";
            txtsmstemplate.Text = "Dear Customer, Interest of Rs.___ towards GLNo.____  is due on  __/__/__. Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952";
            txtsmsdesc.Text = "8 Days Prior to Interest Due Date";
        }

        for (int i = 0; i < dgvRemainder.Rows.Count; i++)
        {
            dgvRemainder.SelectedIndex = i;
            System.Web.UI.WebControls.CheckBox chkBx1 = (System.Web.UI.WebControls.CheckBox)dgvRemainder.SelectedRow.FindControl("chkselect");
            chkBx1.Checked = false;

            System.Web.UI.WebControls.CheckBox chk = (System.Web.UI.WebControls.CheckBox)dgvRemainder.HeaderRow.FindControl("checkAll");
            chk.Checked = false;
        }

    }
    protected void Rdbremainder_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {

            GetSmsDetails(Rdbremainder.SelectedIndex);
            if (Rdbremainder.SelectedIndex == 0)
            {
                //  txtsmstemplate.Text = "Dear Customer, your interest of Rs.___ towards GLNo.____ is due on __/__/__. Kindly arrange to make the payment.";
                txtsmstemplate.Text = "Dear Customer, Interest of Rs.___ towards GLNo.____  is due on  __/__/__. Kindly transfer the amount in our ICICI Bank – IFSC No.: ICIC0006238 ; A/c No.623805029952";
                txtsmsdesc.Text = "8 Days Prior to Interest Due Date";

            }
            else if (Rdbremainder.SelectedIndex == 1)
            {
                txtsmstemplate.Text = "Dear Customer, repayment of your principal towards GLNo.____ of Rs.___ is due on __/__/__. Kindly arrange to make the payment.";
                txtsmsdesc.Text = "15 Days Prior to Repayment Date";
            }
            else if (Rdbremainder.SelectedIndex == 2)
            {
                txtsmstemplate.Text = "Dear Customer, your loan repayment along with interest amounting to Rs.___ towards GLNo.____ was due on __/__/__.Kindly make the payment at the earliest.";
                txtsmsdesc.Text = "1 Month After Repayment Date";
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLSMSRemdinder", "alert('" + ex.Message + "');", true);
        }
    }

    public void GetSmsDetails(int remaindertype)
    {
        string RepaymentDate = "";
        string RepaymentPriorDate = "";
        string RepaymentAfterDate = "";
        string GoldLoanNo = "";
        string receiveddate = "";
        int SDID = 0;
        int SID = 0;
        int KYCID = 0;
        string PrevResult = "";
        int BalanceLoanPayable = 0;
        string userentereddate = "";
        int totaldays = 0;
        decimal interestdue = 0;
        decimal interestamt = 0;
        decimal outstanding = 0;
        decimal outstamt = 0;
        decimal Osint = 0;
        string duedate = "";
        int neworold = 0;

        string ConvertCLI, lastrec, today, todayDateTime;

        string LastDate, LastCLI, LastLoanAmount = string.Empty;
        string InterestFromDate = string.Empty;
        string InterestToDate = string.Empty;
        string RvcCLI = string.Empty;
        string AdvInterestFromDate = string.Empty;
        string AdvInterestToDate = string.Empty;

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();
        cmd.CommandText = "GL_SMSDetails_New";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@Date", gbl.ChangeDateMMddyyyy(txtdate.Text));
        cmd.Parameters.AddWithValue("@flag", Rdbremainder.SelectedIndex);

        Session["rdb"] = Rdbremainder.SelectedIndex;
        ds = new DataSet();
        da.Fill(ds);

        if (ds.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {

                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GL_SMSCheck";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter dacheck = new SqlDataAdapter(cmd);
                cmd.Parameters.AddWithValue("@Date", gbl.ChangeDateMMddyyyy(txtdate.Text));
                cmd.Parameters.AddWithValue("@SDID", Convert.ToInt32(ds.Tables[0].Rows[i]["SDID"]));
                cmd.Parameters.AddWithValue("@RemainderType", Rdbremainder.SelectedIndex);
                DataSet dscheck = new DataSet();
                dacheck.Fill(dscheck);

                if (dscheck.Tables[0].Rows.Count > 0)
                {
                    PrevResult = dscheck.Tables[0].Rows[0]["Result"].ToString();
                    if (PrevResult == "1")
                    {
                        ds.Tables[0].Rows.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {

                            SDID = Convert.ToInt32(ds.Tables[0].Rows[i]["SDID"]);
                            SID = Convert.ToInt32(ds.Tables[0].Rows[i]["SID"]);
                            KYCID = Convert.ToInt32(ds.Tables[0].Rows[i]["KYCID"]);
                            GoldLoanNo = ds.Tables[0].Rows[i]["GLNo"].ToString();
                            receiveddate = ds.Tables[0].Rows[i]["Received Date"].ToString();

                            lastrec = ds.Tables[0].Rows[i]["Received Date"].ToString();
                            todayDateTime = DateTime.Parse(lastrec).ToShortDateString();
                            today = txtdate.Text;

                            BalanceLoanPayable = Convert.ToInt32(ds.Tables[0].Rows[i]["BalanceLoanPayable"]);
                            // Osint = Convert.ToInt32(ds.Tables[0].Rows[i]["OSInt"]);

                            if (ds.Tables[0].Rows[i]["OSIntAmt"].ToString() != null && ds.Tables[0].Rows[i]["OSIntAmt"].ToString() != "")
                            {
                                ConvertCLI = ds.Tables[0].Rows[i]["OSIntAmt"].ToString();
                            }
                            else
                            {
                                ConvertCLI = "0";
                            }


                            if (Rdbremainder.SelectedIndex == 0)
                            {
                                userentereddate = gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[i]["DueDate"].ToString());
                            }
                            else if (Rdbremainder.SelectedIndex == 1)
                            {
                                userentereddate = gbl.ChangeDateMMddyyyy(txtdate.Text);
                            }
                            else if (Rdbremainder.SelectedIndex == 2)
                            {
                                userentereddate = gbl.ChangeDateMMddyyyy(txtdate.Text);
                            }
                            conn = new SqlConnection(strConnString);
                            cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "GL_Reg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd);

                            if (Convert.ToDateTime(todayDateTime) > Convert.ToDateTime(today))
                            {
                                cmd.Parameters.AddWithValue("@Receiveddate", userentereddate);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue("@Receiveddate", gbl.ChangeDateMMddyyyy(receiveddate));
                            }

                            cmd.Parameters.AddWithValue("@Todate", userentereddate);
                            DataSet ds2 = new DataSet();
                            da2.Fill(ds2);

                            if (ds2.Tables[0].Rows.Count > 0)
                            {
                                totaldays = Convert.ToInt32(ds2.Tables[0].Rows[0]["Totaldays"]);

                                conn = new SqlConnection(strConnString);
                                conn.Open();
                                cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails  where isActive='Y' AND  GoldLoanNo='" + GoldLoanNo + "'";
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    neworold = Convert.ToInt32(cmd.ExecuteScalar());
                                }

                                int RcptID = 0;
                                cmdRcpt = new SqlCommand();
                                cmdRcpt.Connection = conn;
                                cmdRcpt.CommandText = "select isnull(MAX(RcptId),0) From TGlReceipt_BasicDetails where  GoldLoanNo='" + GoldLoanNo + "'";
                                if (cmdRcpt.ExecuteScalar() != DBNull.Value)
                                {
                                    RcptID = Convert.ToInt32(cmdRcpt.ExecuteScalar());
                                }

                                int RowRoiID = 1;
                                //Check if Advance Interest is paid then Pass top 1 RowID
                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                                {
                                    cmdRoiRow = new SqlCommand();
                                    cmdRoiRow.Connection = conn;
                                    cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[i]["SID"].ToString();
                                    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                                    {
                                        RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                                    }
                                }
                                else if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) == 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) > 0)
                                {

                                    cmdRoiRow = new SqlCommand();
                                    cmdRoiRow.Connection = conn;
                                    cmdRoiRow.CommandText = "select top 1 ROIID from TSchemeMaster_EffectiveROI where SID=" + ds.Tables[0].Rows[i]["SID"].ToString();
                                    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                                    {
                                        RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                                    }
                                }
                                else if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) > 0 && Convert.ToDecimal(ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString()) == 0)
                                {
                                    cmdRoiRow = new SqlCommand();
                                    cmdRoiRow.Connection = conn;
                                    cmdRoiRow.CommandText = "select isnull(MAX(ROIROWID),0) From TGLInterest_Details where  ReceiptID=" + RcptID;
                                    if (cmdRoiRow.ExecuteScalar() != DBNull.Value)
                                    {
                                        RowRoiID = Convert.ToInt32(cmdRoiRow.ExecuteScalar());
                                    }
                                }

                                if (ds.Tables[0].Rows[i]["InterestFromDate"].ToString() != "" && ds.Tables[0].Rows[i]["InterestFromDate"].ToString() != null)
                                {
                                    InterestFromDate = ds.Tables[0].Rows[i]["InterestFromDate"].ToString();
                                }
                                else
                                {
                                    InterestFromDate = System.DateTime.Today.ToShortDateString();
                                }

                                if (ds.Tables[0].Rows[i]["InterestToDate"].ToString() != "" && ds.Tables[0].Rows[i]["InterestToDate"].ToString() != null)
                                {
                                    InterestToDate = ds.Tables[0].Rows[i]["InterestToDate"].ToString();
                                }
                                else
                                {
                                    InterestToDate = System.DateTime.Today.ToShortDateString();
                                }


                                if (ds.Tables[0].Rows[i]["RecvInterest"].ToString() != "" && ds.Tables[0].Rows[i]["RecvInterest"].ToString() != null)
                                {
                                    RvcCLI = ds.Tables[0].Rows[i]["RecvInterest"].ToString();
                                }
                                else
                                {
                                    RvcCLI = "0";
                                }


                                if (ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != "" && ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString() != null)
                                {
                                    AdvInterestFromDate = ds.Tables[0].Rows[i]["AdvInterestFromDate"].ToString();
                                }
                                else
                                {
                                    AdvInterestFromDate = System.DateTime.Today.ToShortDateString();
                                }

                                if (ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != "" && ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString() != null)
                                {
                                    AdvInterestToDate = ds.Tables[0].Rows[i]["AdvInterestToDate"].ToString();
                                }
                                else
                                {
                                    AdvInterestToDate = System.DateTime.Today.ToShortDateString();
                                }


                                cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "GL_EmiCalculator_RTR";
                                cmd.CommandType = CommandType.StoredProcedure;

                                cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[0]["LoanDate"].ToString()));

                                if (Convert.ToDecimal(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()) >= 0)
                                {
                                    double AddPrvInt = Math.Round(Convert.ToDouble(ds.Tables[0].Rows[i]["Loan Amount"].ToString()) + Convert.ToDouble(ds.Tables[0].Rows[i]["OSIntAmt"].ToString()));
                                    cmd.Parameters.AddWithValue("@LoanAmount", AddPrvInt);
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@LoanAmount", (ds.Tables[0].Rows[i]["Loan Amount"].ToString()));
                                }

                                cmd.Parameters.AddWithValue("@SID", ds.Tables[0].Rows[i]["SID"].ToString());
                                cmd.Parameters.AddWithValue("@NeworOld", neworold);

                                cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                                cmd.Parameters.AddWithValue("@ToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                                cmd.Parameters.AddWithValue("@PaidInt", RvcCLI);

                                cmd.Parameters.AddWithValue("@OSInterestFromDate", gbl.ChangeDateMMddyyyy(InterestFromDate));
                                cmd.Parameters.AddWithValue("@OSInterestToDate", gbl.ChangeDateMMddyyyy(InterestToDate));
                                cmd.Parameters.AddWithValue("@OSIntAmt", ds.Tables[0].Rows[i]["OSIntAmt"].ToString());

                                cmd.Parameters.AddWithValue("@AdvInterestFromDate", gbl.ChangeDateMMddyyyy(AdvInterestFromDate));
                                cmd.Parameters.AddWithValue("@AdvInterestToDate", gbl.ChangeDateMMddyyyy(AdvInterestToDate));
                                cmd.Parameters.AddWithValue("@AdvInterestAmt", ds.Tables[0].Rows[i]["AdvInterestAmount"].ToString());

                                if (Convert.ToDateTime(todayDateTime) > Convert.ToDateTime(today))
                                {
                                    cmd.Parameters.AddWithValue("@CalculateFromDate", (userentereddate));
                                }
                                else
                                {
                                    cmd.Parameters.AddWithValue("@CalculateFromDate", gbl.ChangeDateMMddyyyy(receiveddate));
                                }
                                cmd.Parameters.AddWithValue("@CalculateToDate", (userentereddate));
                                cmd.Parameters.AddWithValue("@LastROIID", RowRoiID);

                                SqlDataAdapter da3 = new SqlDataAdapter(cmd);
                                DataSet ds3 = new DataSet();
                                da3.Fill(ds3);
                                if (ds3.Tables[0].Rows.Count > 0)
                                {
                                    for (int j = 0; j < ds3.Tables[0].Rows.Count; j++)
                                    {
                                        if (ds3.Tables[0].Rows[j]["InterestAmount"] != DBNull.Value)
                                        {
                                            if (ds3.Tables[0].Rows[j]["InterestAmount"].ToString() != string.Empty)
                                            {
                                                interestamt = Convert.ToDecimal(ds3.Tables[0].Rows[j]["InterestAmount"]);
                                                interestdue = interestdue + interestamt;
                                            }
                                        }
                                    }
                                    ds.Tables[0].Rows[i]["Interest"] = Math.Round(Convert.ToDecimal(interestdue.ToString()));
                                    ds.Tables[0].Rows[i]["Outstanding"] = BalanceLoanPayable.ToString();

                                    interestamt = 0;
                                    interestdue = 0;
                                    Osint = 0;

                                }
                                else
                                {
                                    ds.Tables[0].Rows[i]["Interest"] = "0";
                                    ds.Tables[0].Rows[i]["Outstanding"] = "0";
                                    interestamt = 0;
                                    interestdue = 0;
                                    Osint = 0;
                                }

                                //added by priya for remove 0 interest
                                if (Convert.ToDouble(ds.Tables[0].Rows[i]["Interest"]) <= 0)
                                {
                                    ds.Tables[0].Rows.RemoveAt(i);
                                    i--;
                                }

                            }

                        }
                    }


                }

            }
        }

        ////added by priya fro remove 0 interest rowss
        //if (ds.Tables[0].Rows.Count > 0)
        //{
        //    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
        //    {
        //        if (Convert.ToDouble(ds.Tables[0].Rows[j]["Interest"]) <= 0)
        //        {
        //            ds.Tables[0].Rows.RemoveAt(j);
        //        }
        //    }
        //}


        dgvRemainder.DataSource = ds.Tables[0];
        dgvRemainder.DataBind();

        GridViewPrint.DataSource = ds.Tables[0];
        GridViewPrint.DataBind();


        if (dgvRemainder.Rows.Count > 0)
        {
            btnPrint.Visible = true;
        }
        else
        {
            btnPrint.Visible = false;
        }
        Session["GridDatad"] = ds;

    }
    protected void btnPrint_Click(object sender, EventArgs e)
    {
        if (Session["GridDatad"] != null)
        {
            GridViewPrint.DataSource = Session["GridDatad"];
            GridViewPrint.DataBind();
        }

        ShowReport();

    }

    public void ShowReport()
    {

        ReportDocument rpt = new ReportDocument();
        rpt.Load(Server.MapPath("~/crySMSList.rpt"));

        DataSet ds1 = (DataSet)Session["GridDatad"];

        ds1.Tables[0].TableName = "dtSMS";

        rpt.SetDataSource(ds1);

        Session["REPORT"] = rpt;
        ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('rptSMSReminderList.aspx');", true);

    }
}