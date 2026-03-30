using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using CrystalDecisions.Web;
using CrystalDecisions.ReportAppServer;
using CrystalDecisions.Reporting;
using CrystalDecisions.ReportSource;


public partial class ShowRPTCustomerCopy : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (Session["REPORT2"] != null)
            {
                ReportDocument report = (ReportDocument)Session["REPORT2"];
                // CrystalReportViewer1.ReportSource = report;
                report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "ExportedReport");
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}