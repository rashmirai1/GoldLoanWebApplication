using System;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

//public partial class CryGLDisbursalAnalysisReport : System.Web.UI.Page
public partial class CryGLDisbursalAnalysisReport : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        try
        {
            if (Session["REPORT"] != null)
            {
                ReportDocument report = (ReportDocument)Session["REPORT"];
                report.ExportToHttpResponse(ExportFormatType.PortableDocFormat, Response, false, "ExportedReport");
            }
        }
        catch (Exception ex)
        {
            Response.Write(ex.Message);
        }
    }
}
