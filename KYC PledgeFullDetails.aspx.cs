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
using System.Drawing;


public partial class KYC_PledgeFullDetails : System.Web.UI.Page
{
    #region Declarations
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strQuery = string.Empty;
    string insertQuery = string.Empty;
    string updateQuery = string.Empty;
    string deleteQuery = string.Empty;
    SqlTransaction transaction;
    SqlConnection conn;
    SqlDataAdapter da;
    DataSet ds;
    DataSet dsDGV;
    SqlCommand cmd;

    public string loginDate;
    public string expressDate;
    #endregion

    #region[PageLoad]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            ShowDetails();

            if (this.Page.PreviousPage != null)
            {
                // int rowIndex = int.Parse(Request.QueryString["RowIndex"]);
                // GridView GridView1 = (GridView)this.Page.PreviousPage.FindControl("GridView1");
                // GridViewRow row = GridView1.Rows[rowIndex];
                //Session["GoldLoanNo"] = row.Cells[0].Text;


                GridView GridView1 = (GridView)this.Page.PreviousPage.FindControl("dgvDetails");
                GridViewRow selectedRow = GridView1.SelectedRow;
                Session["GoldLoanNo"] = selectedRow.Cells[1].Text.ToString();
                // selectedRow.Cells[1].Text;

            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);

        }
    }
    #endregion[PageLoad]

    #region[ShowDetails]
    protected void ShowDetails()
    {
        try

        {
            string GoldLoanNo = "KNCF/MAY-14/15";
             Session["GoldLoanNo"]=GoldLoanNo;
            conn = new SqlConnection(strConnString);
            conn.Open();

            strQuery = "SELECT tbl_GLKYC_ApplicantDetails.GoldLoanNo," +
                                "(tbl_GLKYC_ApplicantDetails.AppFName+' '+tbl_GLKYC_ApplicantDetails.AppMName+' '+ " +
                                "tbl_GLKYC_ApplicantDetails.AppLName) as 'ApplicantName'," +
                                "DOB=Convert(varchar,tbl_GLKYC_ApplicantDetails.BirthDate,103)," +
                                "PANNo as PANNO ,MobileNo,EmailID as 'EmailAddress' , " +
                                "MobileNo,TelephoneNo,(NomFName+''+NomMName+''+NomLName) as 'NomineeName'," +
                                "NomRelation as'Relation',AppPhotoPath,AppSignPath," +
                                "LoanDate=Convert(varchar,tbl_GLKYC_BasicDetails.LoanDate,103),tbl_GLKYC_BasicDetails.LoanType , " +
                                "(tbl_GLKYC_AddressDetails.BldgHouseName+''+tbl_GLKYC_AddressDetails.BldgPlotNo+''+ " +
                                "tbl_GLKYC_AddressDetails.RoomBlockNo+''+tbl_GLKYC_AddressDetails.Road+''+ " +
                                "tbl_GLKYC_AddressDetails.Landmark+''+tblAreaMaster.Area+''+tblCityMaster.CityName+''+ " +
                                "tblStateMaster.StateName+''+tblAreaMaster.Pincode) as 'Address' " +
                        "FROM tbl_GLKYC_ApplicantDetails " +
                        "INNER JOIN tbl_GLKYC_AddressDetails " +
                                " ON tbl_GLKYC_ApplicantDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                         "INNER JOIN tbl_GLKYC_BasicDetails " +
                                " ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                         "INNER JOIN tblStateMaster " +
                                " ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                        " INNER JOIN tblCityMaster " +
                               " ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID" +
                        " INNER JOIN tblZonemaster " +
                                " ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                         "INNER JOIN tblAreaMaster " +
                                " ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +
                         "WHERE tbl_GLKYC_BasicDetails.GoldLoanNo='"+ Session["GoldLoanNo"] +"'";

                       //"where tbl_GLKYC_BasicDetails.LoanDate between '" + FromDate + "' AND '" + Todate + "' AND tbl_GLKYC_BasicDetails.BranchID='" + ddlBranchName.SelectedValue + "' ";


            ReportDocument RD = new ReportDocument();        

            SqlDataAdapter adp = new SqlDataAdapter(strQuery, conn);

            DataTable tb = new DataTable();
            adp.Fill(tb);
            RD.Load(Server.MapPath("KYCPledgeDetails.rpt"));
            RD.SetDataSource(tb);
          //  CrystalReportViewer1.ReportSource = RD;

            ////////////////////////////////
            strQuery = "SELECT tbl_GLKYC_ApplicantDetails.AppPhotoPath,AppSignPath " +                              
                       "FROM tbl_GLKYC_ApplicantDetails " +
                        "WHERE tbl_GLKYC_ApplicantDetails.GoldLoanNo='" + Session["GoldLoanNo"] + "'";

            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);
            string AppPhoto = Convert.ToString(ds.Tables[0].Rows[0][0]);
            string Sign = Convert.ToString(ds.Tables[0].Rows[0][1]).Trim();
        }


        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[ShowDetails]



    public static byte[] ImageToByteArray(string AppPhoto)
    {
        System.Drawing.Image image = System.Drawing.Image.FromFile(AppPhoto);
        byte[] imageByte = ImageToByteArraybyMemoryStream(image);
        return imageByte;
    }

    private static byte[] ImageToByteArraybyMemoryStream(System.Drawing.Image  image)
    {
        MemoryStream ms = new MemoryStream();
        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
        return ms.ToArray();
    }
}