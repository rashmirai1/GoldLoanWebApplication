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

public partial class GLIdleKYCViewReport : System.Web.UI.Page
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

    protected void Page_Load(object sender, EventArgs e)
    {
        int FYearID = 0;
        if (Convert.ToString(Session["FYearID"]) != "" && Convert.ToString(Session["FYearID"]) != null)
        {
            FYearID = Convert.ToInt32(Session["FYearID"]);
        }

        //fetching Branch Name

        conn = new SqlConnection(strConnString);
        conn.Open();
        strQuery = "select BranchName from tblCompanyBranchMaster where BID='" + Session["BranchID"] + "'";

        cmd = new SqlCommand(strQuery, conn);
        cmd.CommandType = CommandType.Text;
        string branchName = Convert.ToString(cmd.ExecuteScalar());

        ReportDocument RD = new ReportDocument();

        dt = new DataTable();
        dt = (DataTable)Session["dtIdleKYCDetails"];


        if (dt.Rows.Count>0)
        {

            RD.Load(Server.MapPath("GLIdleKYCReport.rpt"));
            RD.DataDefinition.FormulaFields["BranchName"].Text = "'" + Session["BranchName"] + "'";

            RD.SetDataSource(dt);
            CrystalReportViewer1.ReportSource = RD;
        }
    }
}