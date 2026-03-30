using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;

public partial class GLSanctionDisbursalReport : System.Web.UI.Page
{
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    string m_strQuery = string.Empty;
    SqlConnection conn, connAIM;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    DataTable dt;
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["username"] == null)
            {
                Response.Redirect("Default.aspx?info=0");
            }
            else
            {
                hdnUserID.Value = Session["userID"].ToString();
                hdnFYearID.Value = Session["FYearID"].ToString();
                hdnBranchID.Value = Session["branchId"].ToString();
                FillGoldLoanNo();
            }
            btnPrintReport.OnClientClick = "return valid();";
            Master.PropertybtnCancel.Visible = true;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnView.Visible = false;
            Master.PropertybtnSave.Visible = false;
        }
    }
    public void FillGoldLoanNo()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SanctionDisburse_GLNo";
            cmd.Parameters.AddWithValue("@Operation", "SelectGLNo");
            cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
            cmd.Parameters.AddWithValue("@BranchId", hdnBranchID.Value);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ddlGoldLoanNo.DataSource = dt;
                ddlGoldLoanNo.DataTextField = "GoldLoanNo";
                ddlGoldLoanNo.DataValueField = "SDID";
                ddlGoldLoanNo.DataBind();
                ddlGoldLoanNo.Items.Insert(0, new ListItem("--Select Gold Loan No--", "0"));
            }
        }
        catch (Exception ex)
        {
            ShowMessage.Show("Error : " + ex.ToString());
        }
        finally
        {
            conn.Close();
        }
    }
    public DataTable DataPrintReport()
    {
        DataTable dt = new DataTable("Gl_SanctionLetter_RPT");
        try
        {
            dt.Columns.Add("ItemName");
            dt.Columns.Add("ItemId");
            dt.Columns.Add("Quantity");
            dt.Columns.Add("Grossweight");
            dt.Columns.Add("NetWeight");
            dt.Columns.Add("Rate");
            DataRow dr = dt.NewRow();
            dr["ItemName"] = "";
            dr["ItemId"] = "";
            dr["Quantity"] = "";
            dr["Grossweight"] = "";
            dr["NetWeight"] = "";
            dr["Rate"] = "";
            dt.Rows.Add(dr);
        }
        catch (Exception ex)
        {
            ShowMessage.Show("Error : " + ex.ToString());
        }
        finally
        {
        }
        return dt;
    }
    public DataSet GetRecord(DataSet ds, string sdid)
    {
        DataSet dsData = new DataSet();
        try
        {
            string area = "";
            connAIM = new SqlConnection(strConnStringAIM);
            cmd = new SqlCommand();
            cmd.Connection = connAIM;
            cmd.CommandText = "select Area,Pincode From tblAreaMaster where AreaID='" + hdnareaid.Value.Trim() + "'; select Zone From tblZonemaster where ZoneID='" + hdnzoneid.Value.Trim() + "'";
            SqlDataAdapter daaim = new SqlDataAdapter(cmd);
            DataSet dtaim = new DataSet();
            daaim.Fill(dtaim);
            if (dtaim.Tables[0].Rows.Count > 0)
            {
                area += dtaim.Tables[0].Rows[0]["Area"].ToString() + "(" + dtaim.Tables[1].Rows[0]["Zone"].ToString() + ") - " + dtaim.Tables[0].Rows[0]["Pincode"].ToString();
            }
            conn = new SqlConnection(strConnString);
            foreach (DataTable dt2 in ds.Tables)
            {
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = dt2.TableName;
                cmd.Parameters.AddWithValue("@SDID", sdid.Trim() + ',' + area.Trim());
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsData);
            }
        }
        catch (Exception ex)
        {
            ShowMessage.Show("Error : " + ex.ToString());
        }
        finally
        {
        }
        return dsData;
    }
    public void ShowReport(string sdid, string LoanType, string AreaId, string ZoneID)
    {
        try
        {
            DataSet ds = null;
            if (LoanType == "New")
                ds = new DataSet("SanctionLetter.rpt");
            if (LoanType == "Topup")
                ds = new DataSet("SanctionLetterTopup.rpt");
            ds.Tables.Add(DataPrintReport());
            ReportDocument rpt = new ReportDocument();
            rpt.Load(Server.MapPath(ds.DataSetName));
            ds = GetRecord(ds, sdid);
            rpt.SetDataSource(ds.Tables[0]);
            if (rpt.Subreports.Count > 0)
            {
                rpt.Subreports[0].SetDataSource(ds.Tables[1]);
                rpt.Subreports[1].SetDataSource(ds.Tables[2]);
                rpt.Subreports[2].SetDataSource(ds.Tables[2]);
            }
            Session["REPORT"] = rpt;
            ClientScript.RegisterStartupScript(this.GetType(), "Pop Up", "window.open('ShowRpt.aspx');", true);
            DataSet ds1 = new DataSet("~/SanctionLetter_CustomerCopy.rpt");
            ds1.Tables.Add(DataPrintReport());
            ReportDocument rpt1 = new ReportDocument();
            rpt1.Load(Server.MapPath(ds1.DataSetName));
            ds1 = GetRecord(ds1, sdid);
            rpt1.SetDataSource(ds1.Tables[0]);
            if (rpt1.Subreports.Count > 0)
            {
                rpt1.Subreports[0].SetDataSource(ds1.Tables[1]);
                rpt1.Subreports[1].SetDataSource(ds1.Tables[2]);
                rpt1.Subreports[2].SetDataSource(ds1.Tables[2]);
            }
            hdnareaid.Value = "0";
            hdnzoneid.Value = "0";
            Session["REPORT2"] = rpt1;
            ClientScript.RegisterStartupScript(this.GetType(), "Pop Up2", "window.open('ShowRPTCustomerCopy.aspx');", true);
        }
        catch (Exception ex)
        {
            ShowMessage.Show("Error : " + ex.ToString());
        }   
        finally
        {
        }
    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        ddlGoldLoanNo.SelectedIndex = 0;
    }
    protected void btnPrintReport_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SanctionDisburse_GLNo";
            cmd.Parameters.AddWithValue("@Operation", "SelectAreaZone");
            cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
            cmd.Parameters.AddWithValue("@BranchId", hdnBranchID.Value);
            cmd.Parameters.AddWithValue("@SDID", ddlGoldLoanNo.SelectedValue);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                hdnareaid.Value = dt.Rows[0]["AreaID"].ToString();
                hdnzoneid.Value = dt.Rows[0]["ZoneID"].ToString();
                hdnLoanType.Value = dt.Rows[0]["LoanType"].ToString();
            }
            ShowReport(ddlGoldLoanNo.SelectedValue, hdnLoanType.Value, hdnzoneid.Value, hdnareaid.Value);
        }
        catch (Exception ex)
        {
            ShowMessage.Show("Error : " + ex.ToString());
        }
        finally
        {
            conn.Close();
        }
    }
}

