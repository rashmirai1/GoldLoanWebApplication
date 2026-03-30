using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Text;
using System.Net.Mail;
using System.Windows;
using System.IO;

public partial class GLSMSRemainderPrint : System.Web.UI.Page
{ //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    GlobalSettings gbl = new GlobalSettings();
    string remType, remDate;

    protected void Page_Load(object sender, EventArgs e)
    {
        remType = Request.QueryString["id"];
        remDate = Request.QueryString["Date"];
        if (!IsPostBack)
        {
            GetSmsDetails(remType);
        }
    }
    public override void VerifyRenderingInServerForm(Control control)
    {
        /*Verifies that the control is rendered */
    }

    public void GetSmsDetails(string remType)
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
        SqlConnection conn = new SqlConnection(strConnString);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        conn.Open();
        cmd.CommandText = "GL_SMSDetails";
        cmd.CommandType = CommandType.StoredProcedure;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        cmd.Parameters.AddWithValue("@Date", remDate);
        cmd.Parameters.AddWithValue("@flag", remType);
        DataSet ds = new DataSet();
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
                cmd.Parameters.AddWithValue("@Date", remDate);
                cmd.Parameters.AddWithValue("@SDID", Convert.ToInt32(ds.Tables[0].Rows[i]["SDID"]));
                cmd.Parameters.AddWithValue("@RemainderType", remType);
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
                            GoldLoanNo = ds.Tables[0].Rows[i]["GL No"].ToString();
                            receiveddate = ds.Tables[0].Rows[i]["Received Date"].ToString();
                            BalanceLoanPayable = Convert.ToInt32(ds.Tables[0].Rows[i]["BalanceLoanPayable"]);
                            Osint = Convert.ToInt32(ds.Tables[0].Rows[i]["OSInt"]);

                            if (remType.ToString() == "0")
                            {
                                userentereddate = gbl.ChangeDateMMddyyyy(ds.Tables[0].Rows[i]["DueDate"].ToString());
                            }
                            if (remType.ToString() == "1")
                            {
                                userentereddate = remDate;
                            }
                            if (remType.ToString() == "2")
                            {
                                userentereddate = remDate;
                            }
                            conn = new SqlConnection(strConnString);
                            cmd = new SqlCommand();
                            cmd.Connection = conn;
                            cmd.CommandText = "GL_Reg";
                            cmd.CommandType = CommandType.StoredProcedure;
                            SqlDataAdapter da2 = new SqlDataAdapter(cmd);
                            cmd.Parameters.AddWithValue("@Receiveddate", gbl.ChangeDateMMddyyyy(receiveddate));
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
                                cmd.CommandText = "select COUNT(*)From TGlReceipt_BasicDetails where  GoldLoanNo='" + GoldLoanNo + "'";
                                if (cmd.ExecuteScalar() != DBNull.Value)
                                {
                                    neworold = Convert.ToInt32(cmd.ExecuteScalar());
                                }


                                cmd = new SqlCommand();
                                cmd.Connection = conn;
                                cmd.CommandText = "GL_InterestCalculation_RTR";
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@FromDate", gbl.ChangeDateMMddyyyy(receiveddate));
                                cmd.Parameters.AddWithValue("@ToDate", userentereddate);
                                cmd.Parameters.AddWithValue("@LoanAmount", BalanceLoanPayable);
                                cmd.Parameters.AddWithValue("@OSInt", Osint);
                                cmd.Parameters.AddWithValue("@SID", SID);
                                cmd.Parameters.AddWithValue("@NeworOld", neworold);
                                SqlDataAdapter da3 = new SqlDataAdapter(cmd);
                                DataSet ds3 = new DataSet();
                                da3.Fill(ds3);
                                if (ds3.Tables[0].Rows.Count > 0)
                                {

                                    for (int j = 0; j < ds3.Tables[0].Rows.Count; j++)
                                    {
                                        if (ds3.Tables[0].Rows[j]["InterestAmount"] != DBNull.Value)
                                        {
                                            interestamt = Convert.ToDecimal(ds3.Tables[0].Rows[j]["InterestAmount"]);
                                            interestdue = interestdue + interestamt;
                                        }


                                    }
                                    ds.Tables[0].Rows[i]["Interest"] = interestdue.ToString();
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

                            }

                        }
                    }
                }


            }
            GridViewPrint.DataSource = ds.Tables[0];
            GridViewPrint.DataBind();

        }

        //StringBuilder PrintBody = new StringBuilder();
        //MailMessage message = new MailMessage();
        //PrintBody.Append("<table align='center' width='200%'><tr><td><B><U>SMS Remainder List ON " + System.DateTime.Today.ToString("dd-MM-yyyy") + "</td></U></B>");
        //PrintBody.Append("<tr><td></td></tr>");
        //PrintBody.Append("<tr><td></tr></table>");

        //PrintBody.Append("<table border='1' CELLSPACING='0' width='100%'><tr><td><b>Sr.No</b></td><td><b>Gold Loan No</b></td><td ><b>Name</b></td><td ><b>Mobile No.</b></td><td ><b>Last Received Date</b></td><td ><b>Outstanding </b></td><td ><b>Interest</b></td></tr>");
        //for (int ii = 0; ii < ds.Tables[0].Rows.Count; ii++)
        //{
        //    PrintBody.Append("<tr><td>" + (ii + 1) + "</td><td>" + ds.Tables[0].Rows[ii]["GL NO"].ToString() + "</td><td>" + ds.Tables[0].Rows[ii]["Name"].ToString() + "</td><td>" + ds.Tables[0].Rows[ii]["Mobile No"].ToString() + "</td><td>" + ds.Tables[0].Rows[ii]["Received Date"].ToString() + "</td><td>" + ds.Tables[0].Rows[ii]["Outstanding"].ToString() + "</td><td>" + ds.Tables[0].Rows[ii]["Interest"].ToString() + "</td></tr> ");
        //}
        //PrintBody.Append("</table>");
        //message.Body = PrintBody.ToString();
        //message.IsBodyHtml = true;
        //DivPrint.InnerHtml = PrintBody.ToString();
        //ClientScript.RegisterStartupScript(typeof(Page), "DivPrint", "<script type='text/HTML'>window.print('DivPrint');</script>");

        //GridViewPrint.PagerSettings.Visible = false;
        //GridViewPrint.DataBind();
        //StringWriter sw = new StringWriter();
        //HtmlTextWriter hw = new HtmlTextWriter(sw);
        //GridViewPrint.RenderControl(hw);
        //string gridHTML = sw.ToString().Replace("\"", "'")
        //    .Replace(System.Environment.NewLine, "");
        //StringBuilder sb = new StringBuilder();
        //sb.Append("<script type = 'text/javascript'>");
        //sb.Append("window.onload = new function(){");
        //sb.Append("var printWin = window.open('', '', 'left=0");
        //sb.Append(",top=0,width=1000,height=600,status=0');");
        //sb.Append("printWin.document.write(\"");
        //sb.Append(gridHTML);
        //sb.Append("\");");
        //sb.Append("printWin.document.close();");
        //sb.Append("printWin.focus();");
        //sb.Append("printWin.print();");
        //sb.Append("printWin.close();};");
        //sb.Append("</script>");
        //ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", sb.ToString());
        //GridViewPrint.PagerSettings.Visible = true;
        //GridViewPrint.DataBind();



        // GridViewPrint.Visible = true;

        GridViewPrint.UseAccessibleHeader = true;
        GridViewPrint.HeaderRow.TableSection = TableRowSection.TableHeader;
        GridViewPrint.FooterRow.TableSection = TableRowSection.TableFooter;
        GridViewPrint.Attributes["style"] = "border-collapse:separate";
        foreach (GridViewRow row in GridViewPrint.Rows)
        {
            if (row.RowIndex % 10 == 0 && row.RowIndex != 0)
            {
                row.Attributes["style"] = "page-break-after:always;";
            }
        }
        StringWriter sw = new StringWriter();
        HtmlTextWriter hw = new HtmlTextWriter(sw);
        GridViewPrint.RenderControl(hw);
        string gridHTML = sw.ToString().Replace("\"", "'").Replace(System.Environment.NewLine, "");
        StringBuilder sb = new StringBuilder();
        sb.Append("<script type = 'text/javascript'>");
        sb.Append("window.onload = new function(){");
        sb.Append("var printWin = window.open('', '', 'left=0");
        sb.Append(",top=0,width=1000,height=600,status=0');");
        sb.Append("printWin.document.write(\"");
        string style = "<style type = 'text/css'>thead {display:table-header-group;} tfoot{display:table-footer-group;}</style>";
        sb.Append(style + gridHTML);
        sb.Append("\");");
        sb.Append("printWin.document.close();");
        sb.Append("printWin.focus();");
        sb.Append("printWin.print();");
        sb.Append("printWin.close();");
        sb.Append("};");
        sb.Append("</script>");
        ClientScript.RegisterStartupScript(this.GetType(), "GridPrint", sb.ToString());
        // GridViewPrint.AllowPaging = true;
        //  GridViewPrint.DataBind();
    }
}