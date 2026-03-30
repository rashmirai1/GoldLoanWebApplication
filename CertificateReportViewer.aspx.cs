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

public partial class CertificateReportViewer : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringTesting"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    string RefType = string.Empty;
    string RefID = string.Empty;
    string RefNo = string.Empty;
    string GoldLoanNo = string.Empty;
    string UserName = string.Empty;
    string Password = string.Empty;
    DataSet ds;
    //  bool datasaved = false;
    // SqlCommand cmd;
    SqlDataAdapter da;
    SqlConnection conn;
    DataTable dt;
    public string loginDate;
    public string expressDate;
    #endregion

    #region PageLoad
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            SqlConnection conn = new SqlConnection(strConnString);

            //Assign Net Weight Value
            //lblWeight.Text = Convert.ToString(Session["NetWeight"]);

            //Assign Net Loan Amount Value
            double d1 = Convert.ToDouble(Session["NetAmount"]);
            string s = (d1.ToString("0,0", CultureInfo.InvariantCulture));
            //lblNetAmount.Text = s + ' ' + "/-.";

            //Assign LoanSanctionAmount Value
            double d2 = Convert.ToDouble(Session["LoanSanctionAmount"]);
            string s2=string.Empty;
            s2 = (d2.ToString("0,0", CultureInfo.InvariantCulture));
            lblLoanSanctioned.Text = s2 + ' ' + "/-";
          
            // Response.Redirect("GLSanctionDisburseDetails.aspx", false);

            lblGoldNo.Text = Convert.ToString(Session["GoldLoanNo"]);
            lblIssueDate.Text = Convert.ToDateTime(Session["IssueDate"]).ToString("dd/MM/yyyy");

            lblAppName.Text = Convert.ToString(Session["AppName"]);
            lblGrossWeight.Text = Convert.ToString(Session["TotalGrossWeight"]);
            lblNetWeight.Text = Convert.ToString(Session["NetWeight"]);

            //lblNetLoanAmount.Text = Convert.ToString(Session["LoanAmount"]);
            int inputVal = Convert.ToInt32(Session["LoanAmount"]);
            string s1 = (inputVal.ToString("0,0", CultureInfo.InvariantCulture));
            lblNetLoanAmount.Text = s1;
            lblAmtInWord.Text = Convert.ToString(NumbersToWords(inputVal)) + " Only ";


            conn = new SqlConnection(strConnString);

            strQuery = "Select AppPhotoPath FROM tbl_GLKYC_ApplicantDetails WHERE GoldLoanNo='" + Session["GoldLoanNo"] + "'";

            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
                
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {

                imgAppPhoto.ImageUrl = Convert.ToString(ds.Tables[0].Rows[0][0]);
            
            }


            BindDGVDetails();

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion

    #region dgvDetails_PageIndexChanging
    protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDetails.PageIndex = e.NewPageIndex;
            BindDGVDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion

    #region Function For NoToWords
    public static string NumbersToWords(int inputNumber)
    {
        int inputNo = inputNumber;

        if (inputNo == 0)
            return "Zero";

        int[] numbers = new int[4];
        int first = 0;
        int u, h, t;
        System.Text.StringBuilder sb = new System.Text.StringBuilder();

        if (inputNo < 0)
        {
            sb.Append("Minus ");
            inputNo = -inputNo;
        }

        string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
                    "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
        string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
                    "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
        string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
                    "Seventy ","Eighty ", "Ninety "};
        string[] words3 = { "Thousand ", "Lakh ", "Crore " };

        numbers[0] = inputNo % 1000; // units
        numbers[1] = inputNo / 1000;
        numbers[2] = inputNo / 100000;
        numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
        numbers[3] = inputNo / 10000000; // crores
        numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

        for (int i = 3; i > 0; i--)
        {
            if (numbers[i] != 0)
            {
                first = i;
                break;
            }
        }
        for (int i = first; i >= 0; i--)
        {
            if (numbers[i] == 0) continue;
            u = numbers[i] % 10; // ones
            t = numbers[i] / 10;
            h = numbers[i] / 100; // hundreds
            t = t - 10 * h; // tens
            if (h > 0) sb.Append(words0[h] + "Hundred ");
            if (u > 0 || t > 0)
            {
                if (h > 0 || i == 0) sb.Append("and ");
                if (t == 0)
                    sb.Append(words0[u]);
                else if (t == 1)
                    sb.Append(words1[u]);
                else
                    sb.Append(words2[t - 2] + words0[u]);
            }
            if (i != 0) sb.Append(words3[i - 1]);
        }
        return sb.ToString().TrimEnd();
    }

    #endregion

    #region [Bind GridView Gold Item Details]
    protected void BindDGVDetails()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();

            string strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_GoldItemDetails.GID, tblItemMaster.ItemName ," +
                                        "tbl_GLSanctionDisburse_GoldItemDetails.GrossWeight, tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath, " +
                                        "tbl_GLSanctionDisburse_GoldItemDetails.Quantity " +
                                "FROM tbl_GLSanctionDisburse_GoldItemDetails " +
                                "INNER JOIN tblItemMaster " +
                                        "ON tbl_GLSanctionDisburse_GoldItemDetails.ItemID=tblItemMaster.ItemID " +
                                "WHERE tbl_GLSanctionDisburse_GoldItemDetails.GoldLoanNo='" + Session["GoldLoanNo"] + "' ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);
            dgvDetails.DataSource = dt;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Bind GridView Gold Item Details]
}