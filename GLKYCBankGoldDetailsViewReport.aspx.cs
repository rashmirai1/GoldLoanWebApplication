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


public partial class GLKYCBankGoldDetailsViewReport : System.Web.UI.Page
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
    string Query = string.Empty;
    int count = 0;
    #endregion [Declarations]

    #region[Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //getting FYear ID
            int FYearID = 0;
            if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
            {
                FYearID = Convert.ToInt32(Session["FYearID"]);
            }

            //if (Request.QueryString["F"] != null && Request.QueryString["T"] != null && Request.QueryString["B"] != null)
            //{
            string frmDate = Request.QueryString["F"];
            string toDate = Request.QueryString["T"];
            string branchID = Request.QueryString["B"];
            string GoldLoanNo = Request.QueryString["C"];
            string LocationType = Request.QueryString["LocationType"];
            int LocationIndex = Convert.ToInt16(Request.QueryString["LocationIndex"]);
            int BankNameIndex = Convert.ToInt16(Request.QueryString["BankIndex"]);
            string BankName = Request.QueryString["BankName"];
            int RdoSingleChecked = Convert.ToInt32(Session["RdoSingleChecked"]);
            int RdoAllChecked = Convert.ToInt32(Session["RdoAllChecked"]);
            int LocationCkeck = Convert.ToInt32(Session["RdoLocation"]);
            int RdoBank = Convert.ToInt32(Session["RdoBank"]);

            ReportDocument RD = new ReportDocument();

            //fetching Branch Name

            conn = new SqlConnection(strConnString);
            conn.Open();
            strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + Session["BranchID"] + "'";

            cmd = new SqlCommand(strQuery, conn);
            cmd.CommandType = CommandType.Text;
            string branchName = Convert.ToString(cmd.ExecuteScalar());
            DataTable dt = new DataTable();
            dt = (DataTable)Session["dtBankGoldDetails"];
            if(dt.Rows.Count>0)
            { 
            
            
            }

            //Query = "SELECT distinct Convert(varchar,tbl_GLBankGold_BasicDetails.ReferenceDate,103) as 'Column5'," +
            //                 "ReferenceNo as 'Column1',LocationType as 'Column2',UniqueBankCustomerId as 'Column3'," +
            //                 "LocationNo as 'Column4',Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeFromDate,103) as 'Column6'," +
            //                 "Convert(varchar,tbl_GLBankGold_BasicDetails.DepositeToDate,103) as 'Column7'," +
            //                 "RateOfInterest as 'Column8',tblBankMaster.BankName as 'Column9',tblBankMaster.Branch as 'Column10',UserDetails.UserName as 'Column11'," +
            //                  "tbl_GLSanctionDisburse_Status.GLStatus as 'Column12',tbl_GLBankGold_AppDetails.GoldLoanNo as 'Column13', " +
            //                  "tbl_GLKYC_ApplicantDetails.AppFName+' " +
            //                  "'+ tbl_GLKYC_ApplicantDetails.AppMName+' ' + tbl_GLKYC_ApplicantDetails.AppLName as 'Column14' " +

            //                  "FROM tbl_GLBankGold_BasicDetails " +
            //                   " LEFT OUTER JOIN tblBankMaster " +
            //                   " ON tbl_GLBankGold_BasicDetails.BankID=tblBankMaster.BankID " +
            //                   " INNER JOIN UserDetails " +
            //                   " ON UserDetails.UserID=tbl_GLBankGold_BasicDetails.OperatorID " +
            //                   " INNER JOIN tbl_GLBankGold_AppDetails " +
            //                   " ON tbl_GLBankGold_BasicDetails.BankGoldID=tbl_GLBankGold_AppDetails.BankGoldID " +
            //                   " INNER JOIN tbl_GLKYC_ApplicantDetails " +
            //                   " ON tbl_GLKYC_ApplicantDetails.AppID=tbl_GLBankGold_AppDetails.AppID " +

            //                    "inner join tbl_GLSanctionDisburse_Status " +
            //                    "ON tbl_GLSanctionDisburse_Status.GoldLoanNo=tbl_GLKYC_ApplicantDetails.GoldLoanNo " +


            //                    "WHERE  ReferenceDate between '" + Convert.ToDateTime(frmDate).ToString("yyyy/MM/dd") + "' " +
            //                                "and '" + Convert.ToDateTime(toDate).ToString("yyyy/MM/dd") + "' " +
            //                "AND tbl_GLBankGold_BasicDetails.FYID='" + FYearID + "' " +
            //                "AND tbl_GLBankGold_BasicDetails.BranchID='" + branchID + "' ";

            //if (RdoSingleChecked==1)
            //{
            //    strQuery = Query + " AND tbl_GLBankGold_AppDetails.GoldLoanNo='" + GoldLoanNo + "' ";


            //    //da = new SqlDataAdapter(strQuery, conn);
            //    //dt = new DataTable();
            //    //da.Fill(dt);
            //    //RD.Load(Server.MapPath("GLBankGoldDetailsReport.rpt"));
            //    //RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + branchName + "'";
            //    //RD.DataDefinition.FormulaFields["FromDate"].Text = "'" + Convert.ToDateTime(frmDate).ToString("dd-MM-yyyy") + "'";
            //    //RD.DataDefinition.FormulaFields["ToDate"].Text = "'" + Convert.ToDateTime(toDate).ToString("dd-MM-yyyy") + "'";
            //    //RD.SetDataSource(dt);
            //    //CrystalReportViewer1.ReportSource = RD;
            //}

            //if (RdoAllChecked==1)
            //{
            //    if (LocationCkeck ==1)
            //    {
            //        if (LocationIndex == 1)
            //        {

            //            strQuery = Query;

            //            //" AND tbl_GLBankGold_BasicDetails.LocationType='" + LocationType  + "' ";      
            //        }
            //        else
            //        {
            //            strQuery = Query + " AND tbl_GLBankGold_BasicDetails.LocationType='" + LocationType + "' ";

            //        }

            //    }

            //    else if (RdoBank==1)
            //    {

            //        if (BankNameIndex == 1)
            //        {

            //            strQuery = Query;
            //        }
            //        else
            //        {
            //            strQuery = Query + " AND tblBankMaster.BankName='" + BankName + "' ";

            //        }

            //    }

            //}

            //da = new SqlDataAdapter(strQuery, conn);
            //dt = new DataTable();
            //da.Fill(dt);
            if (Session["dtBankGoldDetails"] != "" || Session["dtBankGoldDetails"] != null)
            {
             
                RD.Load(Server.MapPath("GLBankGoldDetailsReport.rpt"));
                RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"] + "'";
                RD.DataDefinition.FormulaFields["FromDate"].Text = "'" + Session["FromDate"] + "'";
                RD.DataDefinition.FormulaFields["ToDate"].Text = "'" + Session["ToDate"] + "'";
                RD.SetDataSource(Session["dtBankGoldDetails"]);
                CrystalReportViewer1.ReportSource = RD;
            }
            //else
            //{
            //    //ScriptManager.RegisterStartupScript(this, typeof(string), "showmessage", "javascript:window.alert('No Details To Show Report.');", true);
            //    Response.Redirect("GLKYCBankGoldDetailsReport.aspx", false);
            //    Session["Count"] = Convert.ToString(1);

            //}

            //}

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "ResetAlert", "alert('" + ex.StackTrace + "');", true);

        }
    }
    #endregion[Page_Load]
}