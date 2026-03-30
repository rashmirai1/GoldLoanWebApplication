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

public partial class GLSanctionDisburseViewDetailedReport : System.Web.UI.Page
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

                    //getting gold loan no/customer details
                    strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.SDID as 'Column1', tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo as 'Column2', " +
                                        "AppFName+ ' ' + AppMName+ ' ' +AppLName as 'Column3', tbl_GLKYC_BasicDetails.LoanType as 'Column4',  " +
                                        "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103) as 'Column5', tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned as 'Column6', " +
                                        "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight as 'Column8', " +
                                        "tbl_GLSanctionDisburse_GoldValueDetails.Deduction as 'Column9', tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight as 'Column10', " +
                                        "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue as 'Column11', '' as 'Column12', " +
                                        "RoomBlockNo as 'Column13', BldgHouseName as 'Column14', BldgPlotNo as 'Column15', Road as 'Column16', Landmark as 'Column17', " +
                                        "Area as 'Column18', CityName as 'Column19', StateName as 'Column20', Pincode as 'Column21', UserName as 'Column22' " +
                                "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLKYC_AddressDetails " +
                                        "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                "INNER JOIN tblStateMaster " +
                                        "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                "INNER JOIN tblCityMaster " +
                                        "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                "INNER JOIN tblZonemaster " +
                                        "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                "INNER JOIN tblAreaMaster " +
                                        "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                "INNER JOIN UserDetails " +
                                        "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                                "WHERE tbl_GLKYC_BasicDetails.KYCID='" + KYCID + "' " +
                                "ORDER BY Column1, Column2 ";

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
                        //getting gold loan no/customer details
                        strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.SDID as 'Column1', tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo as 'Column2', " +
                                            "AppFName+ ' ' + AppMName+ ' ' +AppLName as 'Column3', tbl_GLKYC_BasicDetails.LoanType as 'Column4',  " +
                                            "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103) as 'Column5', tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned as 'Column6', " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight as 'Column8', " +
                                            "tbl_GLSanctionDisburse_GoldValueDetails.Deduction as 'Column9', tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight as 'Column10', " +
                                            "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue as 'Column11', '' as 'Column12', " +
                                            "RoomBlockNo as 'Column13', BldgHouseName as 'Column14', BldgPlotNo as 'Column15', Road as 'Column16', Landmark as 'Column17', " +
                                            "Area as 'Column18', CityName as 'Column19', StateName as 'Column20', Pincode as 'Column21', UserName as 'Column22' " +
                                    "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                    "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLKYC_AddressDetails " +
                                            "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                    "INNER JOIN tblStateMaster " +
                                            "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                    "INNER JOIN tblCityMaster " +
                                            "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                    "INNER JOIN tblZonemaster " +
                                            "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                    "INNER JOIN tblAreaMaster " +
                                            "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                    "INNER JOIN UserDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                                   "WHERE  IssueDate BETWEEN '" + Convert.ToDateTime(frmDate).ToString("yyyy/MM/dd") + "' " +
                                                    "AND '" + Convert.ToDateTime(toDate).ToString("yyyy/MM/dd") + "' " +
                                    "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                                "ORDER BY Column1, Column2 ";
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


                        //getting gold loan no/customer details
                        strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_BasicDetails.SDID as 'Column1', tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo as 'Column2', " +
                                            "AppFName+ ' ' + AppMName+ ' ' +AppLName as 'Column3', tbl_GLKYC_BasicDetails.LoanType as 'Column4',  " +
                                            "convert(varchar,tbl_GLSanctionDisburse_BasicDetails.IssueDate,103) as 'Column5', tbl_GLSanctionDisburse_BasicDetails.NetLoanAmtSanctioned as 'Column6', " +
                                            "tbl_GLSanctionDisburse_Status.GLStatus as 'Column7', tbl_GLSanctionDisburse_GoldValueDetails.TotalGrossWeight as 'Column8', " +
                                            "tbl_GLSanctionDisburse_GoldValueDetails.Deduction as 'Column9', tbl_GLSanctionDisburse_GoldValueDetails.TotalNetWeight as 'Column10', " +
                                            "tbl_GLSanctionDisburse_GoldValueDetails.GoldNetValue as 'Column11', '' as 'Column12', " +
                                            "RoomBlockNo as 'Column13', BldgHouseName as 'Column14', BldgPlotNo as 'Column15', Road as 'Column16', Landmark as 'Column17', " +
                                            "Area as 'Column18', CityName as 'Column19', StateName as 'Column20', Pincode as 'Column21', UserName as 'Column22' " +
                                    "FROM tbl_GLSanctionDisburse_BasicDetails " +
                                    "INNER JOIN  tbl_GLKYC_BasicDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_BasicDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLKYC_AddressDetails " +
                                            "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLSanctionDisburse_Status " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLSanctionDisburse_Status.GoldLoanNo " +
                                    "INNER JOIN  tbl_GLKYC_ApplicantDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +
                                    "INNER JOIN tbl_GLSanctionDisburse_GoldValueDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.SDID=tbl_GLSanctionDisburse_GoldValueDetails.SDID " +
                                    "INNER JOIN tblStateMaster " +
                                            "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                                    "INNER JOIN tblCityMaster " +
                                            "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                                    "INNER JOIN tblZonemaster " +
                                            "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                                    "INNER JOIN tblAreaMaster " +
                                            "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                                    "INNER JOIN UserDetails " +
                                            "ON tbl_GLSanctionDisburse_BasicDetails.OperatorID=UserDetails.UserID " +
                                   "WHERE  IssueDate BETWEEN '" + Convert.ToDateTime(frmDate).ToString("yyyy/MM/dd") + "' " +
                                                    "AND '" + Convert.ToDateTime(toDate).ToString("yyyy/MM/dd") + "' " +
                                    "AND tbl_GLSanctionDisburse_BasicDetails.BranchID='" + branchID + "' " +
                                    "AND tbl_GLSanctionDisburse_Status.GLStatus='" + status + "' " +
                                    "ORDER BY Column1, Column2 ";
                        
                    }
                }

                ReportDocument RD = new ReportDocument();
                conn = new SqlConnection(strConnString);
                da = new SqlDataAdapter(strQuery, conn);
                dt = new DataTable();
                da.Fill(dt);
                RD.Load(Server.MapPath("~/GLSanctionDisburseCustomerDetails.rpt"));
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