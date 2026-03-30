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
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

public partial class GLReceiptAndAgreementViewDetailedReport : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataTable dt;
    DataSet dsDGV;
    SqlCommand cmd;
    #endregion [Declarations]

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        conn = new SqlConnection(strConnString);
        try
        {
            conn.Open();
            if (Request.QueryString["G"] != null && Request.QueryString["R"] != "" && Request.QueryString["A"] != "")
            {
                string SDID = Request.QueryString["G"];
                string Receipt = Request.QueryString["R"];
                string Agreement = Request.QueryString["A"];
                string goldLoanNo = string.Empty;
                string branchName = string.Empty;
                string custName = string.Empty;
                string interestRate = string.Empty;
                string AmountInWords = string.Empty;
                string AmountInWordsForAgreement = string.Empty;
                string strSanctionedAmount = string.Empty;
                decimal ChargesAmount = 0;

                //fetching Branch Name
                strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + Session["branchId"] + "'";
                cmd = new SqlCommand(strQuery, conn);
                cmd.CommandType = CommandType.Text;
                branchName = Convert.ToString(cmd.ExecuteScalar());
                
                //getting gold loan no/customer details
                strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.SDID as 'Column1', tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo as 'Column2', " +
                                    "AppFName+ ' ' + AppMName+ ' ' +AppLName as 'Column3', tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned as 'Column23', " +
                                    "SUM(ChargeAmount) as 'Column24', InterestRate as 'Column4'  " +
                            "FROM tbl_GLSanctionDisburse_BasicDetails " +
                            "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                    "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                            "INNER JOIN  tbl_GLSanctionDisburse_SchemeDetails " +
                                    "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_SchemeDetails.GoldLoanNo " +
                            "LEFT OUTER JOIN tbl_GLSanctionDisburse_ChargesDetails " +
                                    "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_ChargesDetails.GoldLoanNo " +
                            "WHERE tbl_GLSanctionDisburse_BasicDetails.SDID='" + SDID + "' " +
                            "GROUP BY tbl_GLSanctionDisburse_BasicDetails.SDID, tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo, AppFName, AppMName, AppLName, NetLoanAmtSanctioned, InterestRate  " + 
                            "ORDER BY Column1, Column2 ";

                ReportDocument RD = new ReportDocument();
                conn = new SqlConnection(strConnString);
                da = new SqlDataAdapter(strQuery, conn);
                dt = new DataTable();
                ds = new DataSet();
                da.Fill(dt);
                da.Fill(ds);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    goldLoanNo = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
                    custName = Convert.ToString(ds.Tables[0].Rows[0][2]).Trim();
                    decimal SanctionAmount = Decimal.Round(Convert.ToDecimal(ds.Tables[0].Rows[0][3]), 0);
                    strSanctionedAmount = Convert.ToString(SanctionAmount).Trim();
                    ChargesAmount = 0;
                    if (ds.Tables[0].Rows[0][4] != DBNull.Value)
                    {
                        ChargesAmount = Decimal.Round(Convert.ToDecimal(ds.Tables[0].Rows[0][4]), 0);
                    }
                    interestRate = Convert.ToString(ds.Tables[0].Rows[0][5]).Trim();
                    int TotalAmount = Convert.ToInt32((SanctionAmount - ChargesAmount));
                    int TotalAmountForAgreement = Convert.ToInt32((SanctionAmount));
                    AmountInWords = Convert.ToString(NumberToWords(TotalAmount));
                    AmountInWordsForAgreement = Convert.ToString(NumberToWords(TotalAmountForAgreement));

                }
                string paidSummary = "Paid Rupees " + AmountInWords + " to " + custName + " against " + goldLoanNo;

                RD.Load(Server.MapPath("GLReceiptAndAgreement.rpt"));
                RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
                RD.DataDefinition.FormulaFields["PaidSummary"].Text = "'" + paidSummary + "'";
                RD.OpenSubreport("rptConsentLetter.rpt").DataDefinition.FormulaFields["Rupees"].Text = "'" + strSanctionedAmount + " /-'";
                RD.OpenSubreport("rptConsentLetter.rpt").DataDefinition.FormulaFields["RateOfInterest"].Text = "'" + interestRate + " %'";
                RD.OpenSubreport("rptConsentLetter.rpt").DataDefinition.FormulaFields["RupeesInWord"].Text = "'" + AmountInWordsForAgreement + "'";

                RD.SetDataSource(dt);
                //RD.OpenSubreport("rptConsentLetter.rpt").SetDataSource(dt);

                if (Receipt == "1" && Agreement == "0")
                {
                    RD.ReportDefinition.Sections["Section4"].SectionFormat.EnableSuppress = true;
                }
                else if (Receipt == "0" && Agreement == "1")
                {
                    RD.ReportDefinition.Sections["Section4"].SectionFormat.EnableSuppress = false;
                }
                else if (Receipt == "1" && Agreement == "1")
                {
                    RD.ReportDefinition.Sections["Section4"].SectionFormat.EnableSuppress = false;
                }

                CrystalReportViewer1.ReportSource = RD;
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "PgLoadAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }
    #endregion [Page_Load]

    #region [Number To Words]
    public static string NumberToWords(int number)
    {
        if (number == 0)
            return "zero";

        if (number < 0)
            return "minus " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 1000000) > 0)
        {
            words += NumberToWords(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 100000) > 0)
        {
            words += NumberToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }

        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            var unitsMap = new[] { "zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words;
    }
    #endregion [Number To Words]
}