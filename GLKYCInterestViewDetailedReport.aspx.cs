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


public partial class GLKYCInterestViewDetailedReport : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringTesting"].ConnectionString;
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
        try
        {
            if (Request.QueryString["RT"] != null && Request.QueryString["RT"] != "")
            {
                string reportType = Request.QueryString["RT"];
                string KYCID = string.Empty;
                string status = string.Empty;
                string branchName = string.Empty;
                string frmDate = string.Empty;
                string toDate = string.Empty;
                string branchID = string.Empty;

                //--------------------------------Report Type - Single ------------------------------------------
                if (reportType == "S")      
                {
                    if (Request.QueryString["C"] != null && Request.QueryString["C"] != "" && Request.QueryString["B"] != null && Request.QueryString["F"] != null && Request.QueryString["T"] != null)
                    {
                        KYCID = Request.QueryString["C"];
                        frmDate = Request.QueryString["F"];
                        toDate = Request.QueryString["T"];
                        branchID = Request.QueryString["B"];
                    }

                    //fetching Branch Name
                    strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + branchID + "'";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);
                    cmd.CommandType = CommandType.Text;
                    branchName = Convert.ToString(cmd.ExecuteScalar());

                    //getting Interest details
                    strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1', tbl_GLEMI_InterestJVDetails.JVReferenceNo as 'Column2', " +
                                        "convert(varchar,tbl_GLEMI_InterestJVDetails.PaymentDate,103) as 'Column3', convert(varchar,tbl_GLEMI_InterestJVDetails.LoanIssuedDate,103) as 'Column4',  " +
                                        "tbl_GLEMI_InterestJVDetails.PaymentType as 'Column5', convert(varchar,tbl_GLEMI_InterestJVDetails.InterestDate,103) as 'Column6', " +
                                        "tbl_GLEMI_InterestJVDetails.BalanceLoanAmount as 'Column7', tbl_GLEMI_InterestJVDetails.InterestRate as 'Column8', " +
                                        "tbl_GLEMI_InterestJVDetails.NoofDays as 'Column9', tbl_GLEMI_InterestJVDetails.DepositedAmount as 'Column10', " +
                                        "tbl_GLEMI_InterestJVDetails.PrincipleAmount as 'Column11', tbl_GLEMI_InterestJVDetails.InterestAmount as 'Column12', " +
                                        "'' as 'Column13', TotalChargesAmount as 'Column14', " +
                                        "tbl_GLEMI_InterestJVDetails.TotalBalancePayable as 'Column15', tbl_GLEMI_InterestJVDetails.BalanceInterest as 'Column16', " +
                                        "UserName as 'Column17', NarrationName as 'Column18', tbl_GLSanctionDisburse_Status.GLStatus as 'Column19' " +
                                "FROM tbl_GLEMI_InterestJVDetails " +
                                "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                        "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                        "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "INNER JOIN UserDetails " +
                                        "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                                "INNER JOIN tblNarrationMaster " +
                                        "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                                "WHERE tbl_GLKYC_BasicDetails.KYCID='" + KYCID + "' " +
                                "ORDER BY  Column2, Column3 ";

                }
                //--------------------------------Report Type - Multiple ------------------------------------------
                else if (reportType == "M") 
                {

                }
                //--------------------------------Report Type - All ------------------------------------------
                else if (reportType == "A") 
                {
                    if (Request.QueryString["C"] != null && Request.QueryString["C"] != "" && Request.QueryString["B"] != null && Request.QueryString["F"] != null && Request.QueryString["T"] != null)
                    {
                        status = Request.QueryString["C"];
                        frmDate = Request.QueryString["F"];
                        toDate = Request.QueryString["T"];
                        branchID = Request.QueryString["B"];
                    }

                    //fetching Branch Name
                    strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + branchID + "'";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);
                    cmd.CommandType = CommandType.Text;
                    branchName = Convert.ToString(cmd.ExecuteScalar());


                    if (status == "A")
                    {
                        //getting Interest details
                        strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1', tbl_GLEMI_InterestJVDetails.JVReferenceNo as 'Column2', " +
                                            "convert(varchar,tbl_GLEMI_InterestJVDetails.PaymentDate,103) as 'Column3', convert(varchar,tbl_GLEMI_InterestJVDetails.LoanIssuedDate,103) as 'Column4',  " +
                                            "tbl_GLEMI_InterestJVDetails.PaymentType as 'Column5', convert(varchar,tbl_GLEMI_InterestJVDetails.InterestDate,103) as 'Column6', " +
                                            "tbl_GLEMI_InterestJVDetails.BalanceLoanAmount as 'Column7', tbl_GLEMI_InterestJVDetails.InterestRate as 'Column8', " +
                                            "tbl_GLEMI_InterestJVDetails.NoofDays as 'Column9', tbl_GLEMI_InterestJVDetails.DepositedAmount as 'Column10', " +
                                            "tbl_GLEMI_InterestJVDetails.PrincipleAmount as 'Column11', tbl_GLEMI_InterestJVDetails.InterestAmount as 'Column12', " +
                                            "'' as 'Column13', TotalChargesAmount as 'Column14', " +
                                            "tbl_GLEMI_InterestJVDetails.TotalBalancePayable as 'Column15', tbl_GLEMI_InterestJVDetails.BalanceInterest as 'Column16', " +
                                            "UserName as 'Column17', NarrationName as 'Column18', tbl_GLSanctionDisburse_Status.GLStatus as 'Column19' " +
                                    "FROM tbl_GLEMI_InterestJVDetails " +
                                    "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                            "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "INNER JOIN UserDetails " +
                                            "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                                    "INNER JOIN tblNarrationMaster " +
                                            "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                                    "WHERE tbl_GLEMI_InterestJVDetails.BranchID='" + branchID + "' " +
                                    "AND PaymentDate between '" + DateTime.Parse(frmDate).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(toDate).ToString("yyyy/MM/dd") + "' " +
                                    "ORDER BY Column2, Column3 ";
                    }
                    else
                    {
                        if (status == "O")
                        {
                            status = "Open";
                        }
                        else if (status == "C")
                        {
                            status = "Close";
                        }


                        //getting Interest details
                        strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1', tbl_GLEMI_InterestJVDetails.JVReferenceNo as 'Column2', " +
                                            "convert(varchar,tbl_GLEMI_InterestJVDetails.PaymentDate,103) as 'Column3', convert(varchar,tbl_GLEMI_InterestJVDetails.LoanIssuedDate,103) as 'Column4',  " +
                                            "tbl_GLEMI_InterestJVDetails.PaymentType as 'Column5', convert(varchar,tbl_GLEMI_InterestJVDetails.InterestDate,103) as 'Column6', " +
                                            "tbl_GLEMI_InterestJVDetails.BalanceLoanAmount as 'Column7', tbl_GLEMI_InterestJVDetails.InterestRate as 'Column8', " +
                                            "tbl_GLEMI_InterestJVDetails.NoofDays as 'Column9', tbl_GLEMI_InterestJVDetails.DepositedAmount as 'Column10', " +
                                            "tbl_GLEMI_InterestJVDetails.PrincipleAmount as 'Column11', tbl_GLEMI_InterestJVDetails.InterestAmount as 'Column12', " +
                                            "'' as 'Column13', TotalChargesAmount as 'Column14', " +
                                            "tbl_GLEMI_InterestJVDetails.TotalBalancePayable as 'Column15', tbl_GLEMI_InterestJVDetails.BalanceInterest as 'Column16', " +
                                            "UserName as 'Column17', NarrationName as 'Column18', tbl_GLSanctionDisburse_Status.GLStatus as 'Column19' " +
                                    "FROM tbl_GLEMI_InterestJVDetails " +
                                    "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                            "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLEMI_InterestJVDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "INNER JOIN UserDetails " +
                                            "ON tbl_GLEMI_InterestJVDetails.OperatorID=UserDetails.UserID " +
                                    "INNER JOIN tblNarrationMaster " +
                                            "ON tbl_GLEMI_InterestJVDetails.NarrationID=tblNarrationMaster.NarrationID " +
                                    "WHERE tbl_GLEMI_InterestJVDetails.BranchID='" + branchID + "' " +
                                    "AND PaymentDate between '" + DateTime.Parse(frmDate).ToString("yyyy/MM/dd") + "' and '" + DateTime.Parse(toDate).ToString("yyyy/MM/dd") + "' " +
                                    "AND tbl_GLSanctionDisburse_Status.GLStatus='" + status + "' " +
                                    "ORDER BY Column2, Column3 ";
                    }
                }

                ReportDocument RD = new ReportDocument();
                conn = new SqlConnection(strConnString);
                da = new SqlDataAdapter(strQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                RD.Load(Server.MapPath("GLKYCInterestDetails.rpt"));
                RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
                if (frmDate.Trim() != "")
                {
                    RD.DataDefinition.FormulaFields["FromDate"].Text = "'" + Convert.ToDateTime(frmDate).ToString("dd-MM-yyyy") + "'";
                    RD.DataDefinition.FormulaFields["ToDate"].Text = "'" + Convert.ToDateTime(toDate).ToString("dd-MM-yyyy") + "'";
                }
                RD.SetDataSource(dt);
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
}