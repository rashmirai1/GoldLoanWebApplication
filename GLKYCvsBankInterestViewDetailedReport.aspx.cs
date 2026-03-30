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

public partial class GLKYCvsBankInterestViewDetailedReport : System.Web.UI.Page
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
        try
        {
            if (Request.QueryString["RT"] != null && Request.QueryString["RT"] != "")
            {
                string reportType = Request.QueryString["RT"];
                string bnkUnqID = string.Empty;
                string status = string.Empty;
                string branchName = string.Empty;
                string branchID = string.Empty;

                //--------------------------------Report Type - Single ------------------------------------------
                if (reportType == "S")
                {
                    if (Request.QueryString["C"] != null && Request.QueryString["C"] != "" && Request.QueryString["B"] != null)
                    {
                        bnkUnqID = Request.QueryString["C"];
                        branchID = Request.QueryString["B"];
                    }

                    //fetching Branch Name
                    strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + branchID + "'";
                    conn = new SqlConnection(strConnString);
                    conn.Open();
                    cmd = new SqlCommand(strQuery, conn);
                    cmd.CommandType = CommandType.Text;
                    branchName = Convert.ToString(cmd.ExecuteScalar());

                    //getting KYC V/S Bank Interest details
                    strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1',  " +
                                            "AppFName+' ' + AppMName+' ' +AppLName as 'Column2',  " +
                                            "(select SUM(InterestAmount) from tbl_GLEMI_InterestJVDetails JV " +
                                                    "where JV.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) as 'Column3', " +
                                            "Ceiling((((select NetLoanAmtSanctioned from tbl_GLSanctionDisburse_BasicDetails SD " +
                                                            "where SD.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) * " +
                                                      "(select RateOfInterest from tbl_GLBankGold_AppDetails BG " +
                                                                            "inner join tbl_GLBankGold_BasicDetails " +
                                                                                    "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                            "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) / 100) / 30) * " +
                                                      "(DATEDIFF(dd, (select DepositeFromDate from tbl_GLBankGold_AppDetails BG " +
                                                                              "inner join tbl_GLBankGold_BasicDetails " +
                                                                                            "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                              "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo), getDate())+1)) as 'Column4', " +
                                            "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103)  as 'Column6', " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId as 'Column8' " +
                                    "FROM tbl_GLBankGold_BasicDetails " +
                                    "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                            "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "WHERE tbl_GLBankGold_BasicDetails.UniqueBankCustomerId='" + bnkUnqID + "' " +
                                    "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                            "AppFName+' ' + AppMName+' ' +AppLName, " +
                                            "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                                    "ORDER BY Column1, Column2, Column3";
                }
                //--------------------------------Report Type - Multiple ------------------------------------------
                else if (reportType == "M")
                {

                }
                //--------------------------------Report Type - All ------------------------------------------
                else if (reportType == "A")
                {
                    if (Request.QueryString["C"] != null && Request.QueryString["C"] != "" && Request.QueryString["B"] != null)
                    {
                        status = Request.QueryString["C"];
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
                        //getting KYC V/S Bank Interest details

                        strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1',  " +
                                            "AppFName+' ' + AppMName+' ' +AppLName as 'Column2',  " +
                                            "(select SUM(InterestAmount) from tbl_GLEMI_InterestJVDetails JV " +
                                                    "where JV.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) as 'Column3', " + 
                                            "Ceiling((((select NetLoanAmtSanctioned from tbl_GLSanctionDisburse_BasicDetails SD " +
                                                            "where SD.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) * " + 
                                                      "(select RateOfInterest from tbl_GLBankGold_AppDetails BG " +
                                                                            "inner join tbl_GLBankGold_BasicDetails " +
                                                                                    "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                            "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) / 100) / 30) * " + 
                                                      "(DATEDIFF(dd, (select DepositeFromDate from tbl_GLBankGold_AppDetails BG " +
                                                                              "inner join tbl_GLBankGold_BasicDetails " +
                                                                                            "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                              "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo), getDate())+1)) as 'Column4', " + 
                                            "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103)  as 'Column6', " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', " + 
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId as 'Column8' " +
                                    "FROM tbl_GLBankGold_BasicDetails " + 
                                    "INNER JOIN  tbl_GLBankGold_AppDetails " + 
                                            "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " + 
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " + 
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " + 
                                    "INNER JOIN  tbl_GLEMI_InterestJVDetails " + 
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " + 
                                    "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " + 
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " + 
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " + 
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                                    "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " + 
                                            "AppFName+' ' + AppMName+' ' +AppLName, " + 
                                            "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " + 
                                            "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                                    "ORDER BY Column1, Column2, Column3";
                        //strQuery = "SELECT DISTINCT tbl_GLBankGold_AppDetails.GoldLoanNo as 'Column1', " +
                        //                    "AppFName+' ' + AppMName+' ' +AppLName as 'Column2', " +
                        //                    "SUM(InterestAmount)  as 'Column3', " +
                        //                    "((SUM(NetLoanAmtSanctioned) * tbl_GLBankGold_BasicDetails.RateOfInterest / 100) / 30) * DATEDIFF(dd, tbl_GLBankGold_BasicDetails.DepositeFromDate, getDate()) as 'Column4', " +
                        //                    "(SUM(InterestAmount)-((SUM(NetLoanAmtSanctioned) * tbl_GLBankGold_BasicDetails.RateOfInterest / 100) / 30) * DATEDIFF(dd, tbl_GLBankGold_BasicDetails.DepositeFromDate, getDate())) as 'Column5', " +
                        //                    "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103)  as 'Column6', " +
                        //                    "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', " +
                        //                    "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId as 'Column8', " +
                        //                    "NetLoanAmtSanctioned=SUM(NetLoanAmtSanctioned), " +
                        //                    "tbl_GLBankGold_BasicDetails.RateOfInterest " +
                        //            "FROM tbl_GLBankGold_BasicDetails " +
                        //            "INNER JOIN  tbl_GLBankGold_AppDetails " +
                        //                    "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                        //            "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                        //                    "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                        //            "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                        //                    "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                        //            "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                        //                    "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                        //            "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                        //                    "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                        //            "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                        //            "GROUP BY tbl_GLBankGold_AppDetails.GoldLoanNo, AppFName+' ' + AppMName+' ' +AppLName, " +
                        //                    "tbl_GLBankGold_BasicDetails.RateOfInterest, tbl_GLBankGold_BasicDetails.DepositeFromDate,  " +
                        //                    "tbl_GLSanctionDisburse_BasicDetails.IssueDate, tbl_GLSanctionDisburse_Status.GLStatus, tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                        //            "ORDER BY Column1";
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

                        //getting KYC V/S Bank Interest details
                        strQuery = "SELECT DISTINCT tbl_GLEMI_InterestJVDetails.GoldLoanNo as 'Column1',  " +
                                            "AppFName+' ' + AppMName+' ' +AppLName as 'Column2',  " +
                                            "(select SUM(InterestAmount) from tbl_GLEMI_InterestJVDetails JV " +
                                                    "where JV.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) as 'Column3', " +
                                            "Ceiling((((select NetLoanAmtSanctioned from tbl_GLSanctionDisburse_BasicDetails SD " +
                                                            "where SD.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) * " +
                                                      "(select RateOfInterest from tbl_GLBankGold_AppDetails BG " +
                                                                            "inner join tbl_GLBankGold_BasicDetails " +
                                                                                    "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                            "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo) / 100) / 30) * " +
                                                      "(DATEDIFF(dd, (select DepositeFromDate from tbl_GLBankGold_AppDetails BG " +
                                                                              "inner join tbl_GLBankGold_BasicDetails " +
                                                                                            "on BG.BankGoldID=tbl_GLBankGold_BasicDetails.BankGoldID " +
                                                                              "where BG.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo), getDate())+1)) as 'Column4', " +
                                            "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103)  as 'Column6', " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId as 'Column8' " +
                                    "FROM tbl_GLBankGold_BasicDetails " +
                                    "INNER JOIN  tbl_GLBankGold_AppDetails " +
                                            "ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLEMI_InterestJVDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLEMI_InterestJVDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_BasicDetails " +
                                            "ON tbl_GLBankGold_AppDetails.GoldLoanNo=tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "WHERE tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                                    "AND tbl_GLSanctionDisburse_Status.GLStatus='" + status + "' " +
                                    "GROUP BY tbl_GLEMI_InterestJVDetails.GoldLoanNo, " +
                                            "AppFName+' ' + AppMName+' ' +AppLName, " +
                                            "tbl_GLSanctionDisburse_BasicDetails.IssueDate, " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus,  " +
                                            "tbl_GLBankGold_BasicDetails.UniqueBankCustomerId " +
                                    "ORDER BY Column1, Column2, Column3";
                    }
                }

                ReportDocument RD = new ReportDocument();
                conn = new SqlConnection(strConnString);
                da = new SqlDataAdapter(strQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                RD.Load(Server.MapPath("GLKYCvsBankInterestDetails.rpt"));
                RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
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