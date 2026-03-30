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

public partial class PledgeDetailsReport : System.Web.UI.Page
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
            int ID = int.Parse(Request.QueryString["ID"]);
            Session["KYCID"] = Convert.ToString(ID);
            // GridView GridView1 = (GridView)this.Page.PreviousPage.FindControl("dgvDetails");
            // GridViewRow row = GridView1.Rows[rowIndex];
            // Session["GoldLoanNo"] = (row.FindControl("lblName") as Label).Text;

            ShowDetails();

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
            string Date = Convert.ToDateTime(DateTime.Now).ToString("dd/MM/yyyy");
            // string GoldLoanNo = "KNCF/MAY-14/15";
            //Session["GoldLoanNo"] = GoldLoanNo;
            conn = new SqlConnection(strConnString);
            conn.Open();

            strQuery = "SELECT tbl_GLKYC_ApplicantDetails.GoldLoanNo," +
                                "(tbl_GLKYC_ApplicantDetails.AppFName+' '+tbl_GLKYC_ApplicantDetails.AppMName+' '+ " +
                                "tbl_GLKYC_ApplicantDetails.AppLName) as 'ApplicantName'," +
                                "DOB=Convert(varchar,tbl_GLKYC_ApplicantDetails.BirthDate,103)," +
                                "PANNo as PANNO ,MobileNo,EmailID as 'EmailAddress' , " +
                                "TelephoneNo,(NomFName+' '+NomMName+' '+NomLName) as 'NomineeName'," +
                                "NomRelation as'Relation',AppPhotoPath,AppSignPath," +
                                "LoanDate=Convert(varchar,tbl_GLKYC_BasicDetails.LoanDate,103),tbl_GLKYC_BasicDetails.LoanType , " +
                                "(tbl_GLKYC_AddressDetails.BldgHouseName+' ,'+tbl_GLKYC_AddressDetails.BldgPlotNo+', '+ " +
                                "tbl_GLKYC_AddressDetails.RoomBlockNo+', '+tbl_GLKYC_AddressDetails.Road+' ,'+ " +
                                "tbl_GLKYC_AddressDetails.Landmark+' ,'+tblAreaMaster.Area+' ,'+tblCityMaster.CityName+' ,'+ " +
                                "tblStateMaster.StateName+' ,'+tblAreaMaster.Pincode) as 'Address' " +
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
                         "WHERE tbl_GLKYC_BasicDetails.KYCID='" + Session["KYCID"] + "'";


            da = new SqlDataAdapter(strQuery, conn);
            ds = new DataSet();
            da.Fill(ds);
            lblGoldLoanNo.Text = Convert.ToString(ds.Tables[0].Rows[0][0]);
            lblAppName.Text = Convert.ToString(ds.Tables[0].Rows[0][1]);
            lblDOB.Text = Convert.ToString(ds.Tables[0].Rows[0][2]);
            lblPanNo.Text = Convert.ToString(ds.Tables[0].Rows[0][3]).ToUpper();
            lblMobileNo.Text = Convert.ToString(ds.Tables[0].Rows[0][4]);
            lblEmail.Text = Convert.ToString(ds.Tables[0].Rows[0][5]);
            lblTelNo.Text = Convert.ToString(ds.Tables[0].Rows[0][6]);
            lblnominiee.Text = Convert.ToString(ds.Tables[0].Rows[0][7]);
            string Nominee = Convert.ToString(ds.Tables[0].Rows[0][8]);
            if (Nominee == "--Select Relation--")
            {
                lblRelation.Text = "";
            }
            else
            {
                lblRelation.Text = Nominee;
                //Convert.ToString(ds.Tables[0].Rows[0][8]);
            }
            imgPhoto.ImageUrl = Convert.ToString(ds.Tables[0].Rows[0][9]);
            imgSign.ImageUrl = Convert.ToString(ds.Tables[0].Rows[0][10]);
            lblLoanDate.Text = Convert.ToString(ds.Tables[0].Rows[0][11]);
            lblLoanType.Text = Convert.ToString(ds.Tables[0].Rows[0][12]);
            //lblAddress.Text = Convert.ToString(ds.Tables[0].Rows[0][13]);
            lblDate.Text = Date;
            lblbranch.Text = Convert.ToString(Session["branchname"]);

            strQuery = "SELECT  tbl_GLKYC_AddressDetails.RoomBlockNo, BldgHouseName, BldgPlotNo, Road, Landmark, Area, CityName, StateName, Pincode, " +
                " tbl_GLKYC_BasicDetails.GoldLoanNo " +
                 " FROM tbl_GLKYC_AddressDetails " +

                  "INNER JOIN tbl_GLKYC_BasicDetails " +
                       "ON tbl_GLKYC_BasicDetails.GoldLoanNo=tbl_GLKYC_AddressDetails.GoldLoanNo " +
                  "INNER JOIN tblStateMaster " +
                       "ON tbl_GLKYC_AddressDetails.StateID=tblStateMaster.StateID " +
                  "INNER JOIN tblCityMaster " +
                       "ON tbl_GLKYC_AddressDetails.CityID=tblCityMaster.CityID " +
                  "INNER JOIN tblZonemaster " +
                       "ON tbl_GLKYC_AddressDetails.ZoneID=tblZonemaster.ZoneID " +
                  "INNER JOIN tblAreaMaster " +
                       "ON tbl_GLKYC_AddressDetails.AreaID=tblAreaMaster.AreaID " +

                  "WHERE  tbl_GLKYC_BasicDetails.KYCID='" + Session["KYCID"] + "'";

            cmd = new SqlCommand(strQuery, conn);
            da = new SqlDataAdapter(cmd);
            ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                string RoomBlockNo = Convert.ToString(ds.Tables[0].Rows[0][0]);
                string BldgHouseName = Convert.ToString(ds.Tables[0].Rows[0][1]);
                string BldgPlotNo = Convert.ToString(ds.Tables[0].Rows[0][2]);
                string Road = Convert.ToString(ds.Tables[0].Rows[0][3]);
                string Landmark = Convert.ToString(ds.Tables[0].Rows[0][4]);
                string Area = Convert.ToString(ds.Tables[0].Rows[0][5]);
                string CityName = Convert.ToString(ds.Tables[0].Rows[0][6]);
                string StateName = Convert.ToString(ds.Tables[0].Rows[0][7]);
                string Pincode = Convert.ToString(ds.Tables[0].Rows[0][8]);


                string Address = string.Empty;
                if (RoomBlockNo.Trim() != "")
                {
                    Address = "Room/Block No." + RoomBlockNo.Trim();
                }
                else
                {
                    Address = "";
                }
                if (BldgHouseName.Trim() != "")
                {
                    Address += ", " + BldgHouseName.Trim();
                }

                if (BldgPlotNo.Trim() != "")
                {
                    Address += ", " + "Bldg/Plot No." + BldgPlotNo.Trim();
                }
                if (Road.Trim() != "")
                {
                    Address += ", " + Road.Trim();
                }
                if (Landmark.Trim() != "")
                {
                    Address += ", " + Landmark.Trim();
                }
                Address += ", " + Area.Trim() + ", "+ CityName.Trim() + ", " + StateName.Trim() + "-" + Pincode.Trim();
                lblAddress.Text = Convert.ToString(Address);


            }
        }

        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "LoginAlert", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion[ShowDetails]

}