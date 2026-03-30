using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.Web;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.Configuration;

public partial class rptSMSReminderList : System.Web.UI.Page
{
    string remType, remDate;
    string serverNm, Database, User, Pwd;

    protected void Page_Load(object sender, EventArgs e)
    {
        remType = Request.QueryString["radType"];
        remDate = Request.QueryString["selDate"];

        CrystalReportViewer rv = new CrystalReportViewer();
        ReportDocument doc = new ReportDocument();

        serverNm = System.Configuration.ConfigurationManager.AppSettings["DBServer"];
        Database = System.Configuration.ConfigurationManager.AppSettings["DBDatabase"];
        User = System.Configuration.ConfigurationManager.AppSettings["DBUser"];
        Pwd = System.Configuration.ConfigurationManager.AppSettings["DBPassword"];


        try
        {
            if (Session["REPORT"] != null)
            {
                ReportDocument report = (ReportDocument)Session["REPORT"];
                //    report.SetDatabaseLogon("sa", "dev@123", "APH6", "ASPLGLOAN14_1610_TEST");
                report.SetDatabaseLogon(User, Pwd, serverNm, Database);
                report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "ExportedReport");
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}