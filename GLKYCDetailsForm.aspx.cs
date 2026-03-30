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
using System.Text;
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;


public partial class GLKYCDetailsForm : System.Web.UI.Page
{
    #region [Declarations]
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
    string RefType = "AF";
    SqlConnection conn;
    SqlDataAdapter da;
    SqlCommand cmd;
    SqlTransaction transaction;
    DataTable dt, dt1;
    DataRow dr = null;
    GlobalSettings gbl = new GlobalSettings();
    int result = 0;
    bool datasaved = false;
    //-----------Captcha variables-------------------
    private string _randomTextChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";
    private int _randomTextLength = 5;
    Random rand = new Random();
    //--------------------------------------------

    #endregion [Declarations]

    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnEdit.Click += new EventHandler(PropertybtnEdit_Click);
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnDelete.Click += new EventHandler(PropertybtnDelete_Click);
        Master.PropertybtnSearch.Click += new EventHandler(PropertybtnSearch_Click);
        Master.PropertybtnView.Click += new EventHandler(PropertybtnView_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
        Master.PropertyImgBtnClose.Click += new ImageClickEventHandler(PropertyImgBtnClose_Click);
        Master.PropertygvGlobal.RowCommand += new GridViewCommandEventHandler(PropertygvGlobal_RowCommand);
        Master.PropertygvGlobal.PreRender += new EventHandler(PropertygvGlobal_PreRender);
    }
    #endregion [Page_Init]

    #region [Page_Load]
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {

            Master.PropertybtnSave.OnClientClick = "return validrecord();";
            if (!IsPostBack)
            {
                if (Session["username"] == null)
                {
                    Response.Redirect("Default.aspx?info=0");

                }
                else
                {
                    // BindImageUrl();
                    txtCutomerID.Attributes.Add("readonly", "readonly");
                    txtAppliedDate.Attributes.Add("readonly", "readonly");
                    txtAge.Attributes.Add("readonly", "readonly");
                    txtExCustomerId.Attributes.Add("readonly", "readonly");
                    txtSpecifySource.Enabled = false;
                    ddlDealerName.Enabled = false;
                    txtspecifyEmployment.Enabled = false;
                    txtSpecifyIndustryType.Enabled = false;
                    txtLoanPurposespecify.Enabled = false;
                    CustomerID();
                    Get_SourceOfApplication();
                    State_RTR();
                    Dealer_RTR();
                    BlankGv();
                    FillddlSearch();
                    LoanPurpuse_RTR();

                    txtAppliedDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    txtOperatorName.Text = Session["username"].ToString();
                    hdnUserID.Value = Session["userID"].ToString();
                    hdnBranch.Value = Session["branchId"].ToString();
                    hdnFYear.Value = Session["FYearID"].ToString();
                    gbl.GetNoImagePath();

                    for (int i = 0; i < gvDocumentDetails.Rows.Count; i++)
                    {

                        //Image imgDocPhoto = (Image)gvDocumentDetails.SelectedRow.FindControl("imgDocPhoto");
                        // imgDocPhoto.ImageUrl = gbl.GetNoImagePath();

                        dt = new DataTable();
                        dt.Columns.Add("DID");
                        dt.Columns.Add("Serialno");
                        dt.Columns.Add("DocumentID");
                        dt.Columns.Add("OtherDoc");
                        dt.Columns.Add("DocName");
                        dt.Columns.Add("NameOnDoc");
                        dt.Columns.Add("VerifiedBy");
                        dt.Columns.Add("Empld");
                        dt.Columns.Add("ImagePath");
                        dt.Columns.Add("ImageUrl");


                        dr = dt.NewRow();

                        dr["DID"] = "0";
                        if (dt.Rows.Count == 0)
                        {
                            dr["Serialno"] = 1;
                        }
                        else
                        {
                            dr["Serialno"] = gvDocumentDetails.Rows.Count + 1;
                        }
                        dr["DocumentID"] = 0;
                        dr["OtherDoc"] = "";
                        dr["DocName"] = "";
                        dr["NameOnDoc"] = "";
                        dr["VerifiedBy"] = "";
                        dr["Empld"] = "";
                        dr["ImagePath"] = gbl.GetNoImagePath();
                        dr["ImageUrl"] = gbl.GetNoImagePath();
                        dt.Rows.Add(dr);
                        gvDocumentDetails.DataSource = dt;
                        gvDocumentDetails.DataBind();
                    }

                    gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
                }

            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    #endregion [Page_Load]

    protected void PropertygvGlobal_PreRender(object sender, EventArgs e)
    {
        if (Master.PropertygvGlobal.HeaderRow != null)
        {
            Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;
        }
    }

    protected void ClearData()
    {

        imgbtnExCustomer.Enabled = true;
        txtSpecifySource.Enabled = false;
        ddlDealerName.Enabled = false;
        txtspecifyEmployment.Enabled = false;
        txtSpecifyIndustryType.Enabled = false;
        txtLoanPurposespecify.Enabled = false;
        hdnoperation.Value = "Save";
        hdnid.Value = "0";
        hdnverify.Value = "0";
        txtVerification.Text = "";
        txtAppliedDate.Text = DateTime.Now.ToString("dd/MM/yyyy");
        txtOperatorName.Text = Session["username"].ToString();
        txtExCustomerId.Text = "";
        imgAppPhoto.ImageUrl = "";
        imgAppSign.ImageUrl = "";
        txtAppFName.Text = "";
        txtAppMName.Text = "";
        txtAppLName.Text = "";
        ddlGender.SelectedValue = "0";
        txtExistingPLCaseNo.Text = "";
        txtPANNo.Text = "";
        txtBirthDate.Text = "";
        ddlMaritalStatus.SelectedValue = "0";
        txtSpouse.Text = "";
        txtChildren.Text = "";
        txtAge.Text = "";
        txtBldgHouseName.Text = "";
        txtRoad.Text = "";
        txtPlotNo.Text = "";
        txtRoomBlockNo.Text = "";
        txtNearestLandmark.Text = "";
        txtMobileNo.Text = "";
        txtTelephoneNo.Text = "";
        txtEmailId.Text = "";
        txtNomFName.Text = "";
        txtNomMName.Text = "";
        txtNomLName.Text = "";
        txtNomineeAddress.Text = "";
        ddlNomRelation.SelectedValue = "";
        txtID.Text = "";
        txtOperatorID.Text = "";
        txtAppPhotoPath.Text = "";
        txtAppSignPath.Text = "";
        txtDID.Text = "";
        ddlSourceofApplication.SelectedValue = "0";
        ddlDealerName.SelectedIndex = 0;
        txtSpecifySource.Text = "";
        ddlLoanPurpose.SelectedIndex = 0;
        ddlOccupation.SelectedIndex = 0;
        ddlemploymentdetails.SelectedIndex = 0;
        txtspecifyEmployment.Text = "";
        ddlIndustrytype.SelectedIndex = 0;
        txtSpecifyIndustryType.Text = "";
        ddlPresentAnnIncm.SelectedIndex = 0;
        txtOfficeAdd.Text = "";
        txtNameofOrg.Text = "";
        txtSpecifyDesigntn.Text = "";
        txtLoanPurposespecify.Text = "";
        Master.PropertyddlSearch.SelectedIndex = 0;
        Master.PropertytxtSearch.Text = "";
        CustomerID();
        BlankGv();
        State_RTR();
        ddlState.SelectedIndex = 0;
        City_RTR();
        ddlCity.SelectedIndex = 0;
        Area_RTR();
        ddlArea.SelectedIndex = 0;
        Zone_RTR();
        ddlZone.SelectedIndex = 0;


    }
    public void BlankGv()
    {
        dt = new DataTable();
        dt.Columns.Add("DID");
        dt.Columns.Add("Serialno");
        dt.Columns.Add("DocumentID");
        dt.Columns.Add("OtherDoc");
        dt.Columns.Add("DocName");
        dt.Columns.Add("NameOnDoc");
        dt.Columns.Add("VerifiedBy");
        dt.Columns.Add("Empld");
        dt.Columns.Add("ImagePath");
        dt.Columns.Add("ImageUrl");
        dt.Rows.Add("0", "1", "", "", "", "", "", "", gbl.GetNoImagePath(), gbl.GetNoImagePath());
        gvDocumentDetails.DataSource = dt;
        gvDocumentDetails.DataBind();
    }
    public void FillddlSearch()
    {
        Master.PropertyddlSearch.Items.Add("Customer ID");
        Master.PropertyddlSearch.Items.Add("Applied Date");
        Master.PropertyddlSearch.Items.Add("Applicant Name");
        Master.PropertyddlSearch.Items.Add("Mobile No");
        Master.PropertyddlSearch.Items.Add("Pan No");
        Master.PropertyddlSearch.Items.Add("Source");


    }
    public void CustomerID()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_CustomerID";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);

        dt = new DataTable();
        da.Fill(dt);
        if (dt.Rows.Count > 0)
        {
            txtCutomerID.Text = dt.Rows[0]["CustomerID"].ToString();
        }

    }
    #region [Get_SourceOfApplication]
    //Added by Priya For getting Source from table
    public void Get_SourceOfApplication()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_GetKYCSourceOfApplication";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        ddlSourceofApplication.DataSource = dt;
        ddlSourceofApplication.DataTextField = "SourceofApplication";
        ddlSourceofApplication.DataValueField = "SourceofApplicationID";
        ddlSourceofApplication.DataBind();
        ddlSourceofApplication.Items.Insert(0, new ListItem("--Select Source--", "0"));

    }
    //end
    #endregion [Get_SourceOfApplication]
    public void State_RTR()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_State_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);

        dt = new DataTable();
        da.Fill(dt);

        ddlState.DataSource = dt;
        ddlState.DataTextField = "StateName";
        ddlState.DataValueField = "StateID";
        ddlState.DataBind();

        ddlState.Items.Insert(0, new ListItem("--Select State--", "0"));

    }
    public void City_RTR()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_City_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@StateID", ddlState.SelectedValue.Trim());
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        ddlCity.DataSource = dt;
        ddlCity.DataTextField = "CityName";
        ddlCity.DataValueField = "CityID";
        ddlCity.DataBind();
        ddlCity.Items.Insert(0, new ListItem("--Select City--", "0"));
    }
    public void Area_RTR()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_Area_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@CityID", ddlCity.SelectedValue.Trim());
        da = new SqlDataAdapter(cmd);

        dt = new DataTable();
        da.Fill(dt);

        ddlArea.DataSource = dt;
        ddlArea.DataTextField = "Area";
        ddlArea.DataValueField = "AreaID";
        ddlArea.DataBind();

        ddlArea.Items.Insert(0, new ListItem("--Select Area--", "0"));
    }
    public void Zone_RTR()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_Zone_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@AreaID", ddlArea.SelectedValue.Trim());
        da = new SqlDataAdapter(cmd);

        dt = new DataTable();
        da.Fill(dt);

        ddlZone.DataSource = dt;
        ddlZone.DataTextField = "Zone";
        ddlZone.DataValueField = "ZoneID";
        ddlZone.DataBind();

        ddlZone.Items.Insert(0, new ListItem("--Select Zone--", "0"));
    }
    public void Dealer_RTR()
    {

        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_Dealer_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        // cmd.Parameters.AddWithValue("@AreaID", ddlCity.SelectedValue.Trim());
        da = new SqlDataAdapter(cmd);

        dt = new DataTable();
        da.Fill(dt);

        ddlDealerName.DataSource = dt;
        ddlDealerName.DataTextField = "DealerName";
        ddlDealerName.DataValueField = "DealerID";
        ddlDealerName.DataBind();

        ddlDealerName.Items.Insert(0, new ListItem("--Select Dealer--", "0"));
    }
    public void LoanPurpuse_RTR()
    {
        conn = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYC_LoanPurpose_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        ddlLoanPurpose.DataSource = dt;
        ddlLoanPurpose.DataTextField = "LoanPupose";
        ddlLoanPurpose.DataValueField = "LoanpurposeID";
        ddlLoanPurpose.DataBind();
        ddlLoanPurpose.Items.Insert(0, new ListItem("--Select Loan Purpose--", "0"));

    }


    public void GoldLoan_PRI(string operation, string value)
    {
        conn = new SqlConnection(strConnString);
        conn.Open();
        transaction = conn.BeginTransaction(IsolationLevel.ReadUncommitted, "BuilderTransaction");

        for (int i = 0; i < gvDocumentDetails.Rows.Count; i++)
        {
            gvDocumentDetails.SelectedIndex = i;
            Label lblSrNo = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSrNo");
            HiddenField hdndocid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocid");
            HiddenField hdndocumentid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocumentid");
            Label lblDocName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocName");
            Label lblSpecifyOther = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSpecifyOther");
            Label lblDocOnName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocOnName");
            Label lblEmpname = (Label)gvDocumentDetails.SelectedRow.FindControl("lblEmpname");
            HiddenField hdnempid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnempid");
            HiddenField hdnimgpath = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnimgpath");
            Image imgDocPhoto = (Image)gvDocumentDetails.SelectedRow.FindControl("imgDocPhoto");

            //added by priya for ex customer only
            string ExCustID;

            if (txtExCustomerId.Text != "")
            {
                ExCustID = txtExCustomerId.Text;
            }
            else
            {
                ExCustID = txtCutomerID.Text;
            }
            //end
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.Transaction = transaction;
            cmd.CommandText = "GL_KYC_GOLDLOAN_PRI";
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Operation", operation);
            cmd.Parameters.AddWithValue("@KYCID", value);
            cmd.Parameters.AddWithValue("@CustomerID", ExCustID);//txtCutomerID.Text
            cmd.Parameters.AddWithValue("@AppliedDate", gbl.ChangeDateMMddyyyy(txtAppliedDate.Text));
            cmd.Parameters.AddWithValue("@OperatorID", hdnUserID.Value);
            cmd.Parameters.AddWithValue("@ExistingCustomerID", txtExCustomerId.Text);
            cmd.Parameters.AddWithValue("@AppFName", txtAppFName.Text);
            cmd.Parameters.AddWithValue("@AppMName", txtAppMName.Text);
            cmd.Parameters.AddWithValue("@AppLName", txtAppLName.Text);
            cmd.Parameters.AddWithValue("@AppPhotoPath", txtAppPhotoPath.Text);
            cmd.Parameters.AddWithValue("@AppSignPath", txtAppSignPath.Text);
            cmd.Parameters.AddWithValue("@ExistingPLCaseNo", txtExistingPLCaseNo.Text);
            cmd.Parameters.AddWithValue("@Gender", ddlGender.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@Spouse", txtSpouse.Text);
            cmd.Parameters.AddWithValue("@Children", txtChildren.Text);
            cmd.Parameters.AddWithValue("@BirthDate", gbl.ChangeDateMMddyyyy(txtBirthDate.Text));
            cmd.Parameters.AddWithValue("@Age", txtAge.Text);
            cmd.Parameters.AddWithValue("@PANNo", txtPANNo.Text);
            cmd.Parameters.AddWithValue("@MaritalStatus", ddlMaritalStatus.SelectedValue);
            cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text);
            cmd.Parameters.AddWithValue("@Verify", hdnverify.Value);
            cmd.Parameters.AddWithValue("@VerificationCode", txtVerification.Text);
            cmd.Parameters.AddWithValue("@TelephoneNo", txtTelephoneNo.Text);
            cmd.Parameters.AddWithValue("@EmailID", txtEmailId.Text);
            //  cmd.Parameters.AddWithValue("@SourceofApplication", ddlSourceofApplication.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@SourceofApplicationID", ddlSourceofApplication.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@SourceSpecification", txtSpecifySource.Text);
            cmd.Parameters.AddWithValue("@DealerID", ddlDealerName.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@NomFName", txtNomFName.Text);
            cmd.Parameters.AddWithValue("@NomMName", txtNomMName.Text);
            cmd.Parameters.AddWithValue("@NomLName", txtNomLName.Text);
            cmd.Parameters.AddWithValue("@NomRelation", ddlNomRelation.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@NomAddress", txtNomineeAddress.Text);
            cmd.Parameters.AddWithValue("@BldgHouseName", txtBldgHouseName.Text);
            cmd.Parameters.AddWithValue("@Road", txtRoad.Text);
            cmd.Parameters.AddWithValue("@BldgPlotNo", txtPlotNo.Text);
            cmd.Parameters.AddWithValue("@RoomBlockNo", txtRoomBlockNo.Text);
            cmd.Parameters.AddWithValue("@StateID", ddlState.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@CityID", ddlCity.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@AreaID", ddlArea.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@ZoneID", ddlZone.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@Landmark", txtNearestLandmark.Text.Trim());
            cmd.Parameters.AddWithValue("@Occupation", ddlOccupation.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@PresentIncome", ddlPresentAnnIncm.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@EmploymentType", ddlemploymentdetails.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@SpecifyEmployment", txtspecifyEmployment.Text);
            cmd.Parameters.AddWithValue("@OrganizationName", txtNameofOrg.Text.Trim());
            cmd.Parameters.AddWithValue("@OfficeAddress", txtOfficeAdd.Text.Trim());
            cmd.Parameters.AddWithValue("@IndustriesType", ddlIndustrytype.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@SpecifyIndustries", txtSpecifyIndustryType.Text);
            cmd.Parameters.AddWithValue("@Designation", txtSpecifyDesigntn.Text.Trim());
            cmd.Parameters.AddWithValue("@LoanpurposeID", ddlLoanPurpose.SelectedValue.Trim());
            cmd.Parameters.AddWithValue("@SpecifyLoanPurpose", txtLoanPurposespecify.Text.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", hdnUserID.Value);


            cmd.Parameters.AddWithValue("@FYID", hdnFYear.Value);
            cmd.Parameters.AddWithValue("@CmpID", "1");
            cmd.Parameters.AddWithValue("@BranchID", hdnBranch.Value);
            cmd.Parameters.AddWithValue("@DID", hdndocid.Value);
            cmd.Parameters.AddWithValue("@Serialno", lblSrNo.Text);
            cmd.Parameters.AddWithValue("@DocumentID", hdndocumentid.Value);
            cmd.Parameters.AddWithValue("@DocName", lblDocName.Text);
            cmd.Parameters.AddWithValue("@ImagePath", hdnimgpath.Value);
            cmd.Parameters.AddWithValue("@OtherDoc", lblSpecifyOther.Text);
            cmd.Parameters.AddWithValue("@VerifiedBy", lblEmpname.Text);
            cmd.Parameters.AddWithValue("@Empld", hdnempid.Value);
            cmd.Parameters.AddWithValue("@NameOnDoc", lblDocOnName.Text);
            cmd.Parameters.AddWithValue("@ImageUrl", hdnimgpath.Value);
            cmd.Parameters.AddWithValue("@Lineno", i);
            result = cmd.ExecuteNonQuery();

        }
        //conn.Close();
        if (result > 0)
        {
            transaction.Commit();
            conn.Dispose();
            datasaved = true;
        }
        else
        {
            transaction.Rollback();
            datasaved = false;
        }

        if (operation == "Save" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "KYC", "alert('Record Saved Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        if (operation == "Update" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "KYC1", "alert('Record Updated Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Save", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        if (operation == "Delete" && datasaved)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "KYC3", "alert('Record Deleted Successfully');", true);
            ClearData();
            gbl.CheckAEDControlSettings("Delete", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }

    }
    public void GoldLoan_RTR()
    {
        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYCGoldLoan_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@FYID", hdnFYear.Value);
        cmd.Parameters.AddWithValue("@BRANCHID", hdnBranch.Value);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        //Master.PropertygvGlobal.UseAccessibleHeader = true;
        // Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;

        Master.PropertympeGlobal.Show();
    }
    public void GoldLoan_Details_RTR(string id)
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYCGoldLoan_Details_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@KYCID", id);

        da = new SqlDataAdapter(cmd);
        dt1 = new DataTable();
        da.Fill(dt1);
        if (dt1.Rows.Count > 0)
        {
            hdnoperation.Value = "Update";
            imgbtnExCustomer.Enabled = false;
            hdnid.Value = dt1.Rows[0]["KYCID"].ToString();
            txtCutomerID.Text = dt1.Rows[0]["CustomerID"].ToString();
            txtAppliedDate.Text = dt1.Rows[0]["AppliedDate"].ToString();
            txtAppPhotoPath.Text = dt1.Rows[0]["AppPhotoPath"].ToString();
            imgAppPhoto.ImageUrl = dt1.Rows[0]["AppPhotoPath"].ToString();
            txtAppSignPath.Text = dt1.Rows[0]["AppSignPath"].ToString();
            imgAppSign.ImageUrl = dt1.Rows[0]["AppSignPath"].ToString();
            // ddlSourceofApplication.SelectedValue = dt1.Rows[0]["SourceofApplication"].ToString();
            ddlSourceofApplication.SelectedValue = dt1.Rows[0]["SourceofApplicationID"].ToString();

            if (ddlSourceofApplication.SelectedValue == "4")
            {
                txtSpecifySource.Text = "";
                ddlDealerName.SelectedIndex = 0;
                txtSpecifySource.Enabled = false;
                ddlDealerName.Enabled = true;
            }
            else
            {
                txtSpecifySource.Text = "";
                ddlDealerName.SelectedIndex = 0;
                txtSpecifySource.Enabled = true;
                ddlDealerName.Enabled = false;
            }
            txtSpecifySource.Text = dt1.Rows[0]["SourceSpecification"].ToString();
            ddlDealerName.SelectedValue = dt1.Rows[0]["DealerID"].ToString();
            txtAppFName.Text = dt1.Rows[0]["AppFName"].ToString();
            txtAppMName.Text = dt1.Rows[0]["AppMName"].ToString();
            txtAppLName.Text = dt1.Rows[0]["AppLName"].ToString();
            txtExistingPLCaseNo.Text = dt1.Rows[0]["ExistingPLCaseNo"].ToString();
            txtPANNo.Text = dt1.Rows[0]["PANNo"].ToString();
            ddlGender.SelectedValue = dt1.Rows[0]["Gender"].ToString();
            ddlMaritalStatus.SelectedValue = dt1.Rows[0]["MaritalStatus"].ToString();
            txtSpouse.Text = dt1.Rows[0]["Spouse"].ToString();
            txtChildren.Text = dt1.Rows[0]["Children"].ToString();
            txtBirthDate.Text = dt1.Rows[0]["BirthDate"].ToString();
            txtAge.Text = dt1.Rows[0]["Age"].ToString();
            txtBldgHouseName.Text = dt1.Rows[0]["BldgHouseName"].ToString();
            txtPlotNo.Text = dt1.Rows[0]["BldgPlotNo"].ToString();
            txtRoad.Text = dt1.Rows[0]["Road"].ToString();
            txtRoomBlockNo.Text = dt1.Rows[0]["RoomBlockNo"].ToString();
            State_RTR();
            ddlState.SelectedValue = dt1.Rows[0]["StateID"].ToString();
            City_RTR();
            ddlCity.SelectedValue = dt1.Rows[0]["CityID"].ToString();
            Area_RTR();
            ddlArea.SelectedValue = dt1.Rows[0]["AreaID"].ToString();
            Zone_RTR();
            ddlZone.SelectedValue = dt1.Rows[0]["ZoneID"].ToString();
            txtNearestLandmark.Text = dt1.Rows[0]["Landmark"].ToString();
            txtMobileNo.Text = dt1.Rows[0]["MobileNo"].ToString();
            txtTelephoneNo.Text = dt1.Rows[0]["TelephoneNo"].ToString();
            txtEmailId.Text = dt1.Rows[0]["EmailID"].ToString();
            ddlOccupation.SelectedValue = dt1.Rows[0]["Occupation"].ToString();
            ddlPresentAnnIncm.SelectedValue = dt1.Rows[0]["PresentIncome"].ToString();
            ddlemploymentdetails.SelectedValue = dt1.Rows[0]["EmploymentType"].ToString().Trim();
            if (ddlemploymentdetails.SelectedValue == "Other")
            {
                txtspecifyEmployment.Text = "";
                txtspecifyEmployment.Enabled = true;
            }
            else
            {
                txtspecifyEmployment.Text = "";
                txtspecifyEmployment.Enabled = false;
            }
            txtspecifyEmployment.Text = dt1.Rows[0]["SpecifyEmployment"].ToString().Trim();
            txtNameofOrg.Text = dt1.Rows[0]["OrganizationName"].ToString();
            txtOfficeAdd.Text = dt1.Rows[0]["OfficeAddress"].ToString();
            ddlIndustrytype.SelectedValue = dt1.Rows[0]["IndustriesType"].ToString();
            if (ddlIndustrytype.SelectedValue == "Other")
            {
                txtSpecifyIndustryType.Text = "";
                txtSpecifyIndustryType.Enabled = true;
            }
            else
            {
                txtSpecifyIndustryType.Text = "";
                txtSpecifyIndustryType.Enabled = false;
            }
            txtSpecifyIndustryType.Text = dt1.Rows[0]["SpecifyIndustries"].ToString();
            txtNomFName.Text = dt1.Rows[0]["NomFName"].ToString();
            txtNomMName.Text = dt1.Rows[0]["NomMName"].ToString();
            txtNomLName.Text = dt1.Rows[0]["NomLName"].ToString();
            txtNomineeAddress.Text = dt1.Rows[0]["NomAddress"].ToString();
            ddlNomRelation.SelectedValue = dt1.Rows[0]["NomRelation"].ToString();
            txtSpecifyDesigntn.Text = dt1.Rows[0]["Designation"].ToString();
            ddlLoanPurpose.SelectedValue = dt1.Rows[0]["LoanpurposeID"].ToString();
            if (ddlLoanPurpose.SelectedValue == "7")
            {
                txtLoanPurposespecify.Text = "";
                txtLoanPurposespecify.Enabled = true;
            }
            else
            {
                txtLoanPurposespecify.Text = "";
                txtLoanPurposespecify.Enabled = false;
            }
            txtLoanPurposespecify.Text = dt1.Rows[0]["SpecifyLoanPurpose"].ToString();
            gvDocumentDetails.DataSource = dt1;
            gvDocumentDetails.DataBind();

            TextBox txtNameOnDoc = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");
            string name = "";
            if (dt1.Rows[0]["AppMName"].ToString().Trim() != "")
            {
                name = dt1.Rows[0]["AppFName"].ToString() + " " + dt1.Rows[0]["AppMName"].ToString();
            }
            else
            {
                name = dt1.Rows[0]["AppFName"].ToString();
            }
            if (dt1.Rows[0]["AppLName"].ToString().Trim() != "")
            {
                name = name + " " + dt1.Rows[0]["AppLName"].ToString();
            }
            txtNameOnDoc.Text = name;
        }

    }
    public void GoldLoan_Ex_Details_RTR(string id)
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYCGoldLoan_Details_RTR";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@KYCID", id);

        da = new SqlDataAdapter(cmd);
        dt1 = new DataTable();
        da.Fill(dt1);
        if (dt1.Rows.Count > 0)
        {
            hdnoperation.Value = "Save";
            hdnid.Value = "0";
            txtExCustomerId.Text = dt1.Rows[0]["CustomerID"].ToString();
            // txtAppliedDate.Text = dt1.Rows[0]["AppliedDate"].ToString();
            txtAppPhotoPath.Text = dt1.Rows[0]["AppPhotoPath"].ToString();
            imgAppPhoto.ImageUrl = dt1.Rows[0]["AppPhotoPath"].ToString();
            txtAppSignPath.Text = dt1.Rows[0]["AppSignPath"].ToString();
            imgAppSign.ImageUrl = dt1.Rows[0]["AppSignPath"].ToString();
            ddlSourceofApplication.SelectedValue = dt1.Rows[0]["SourceofApplication"].ToString();
            if (ddlSourceofApplication.SelectedValue == "Dealer")
            {
                txtSpecifySource.Text = "";
                ddlDealerName.SelectedIndex = 0;
                txtSpecifySource.Enabled = false;
                ddlDealerName.Enabled = true;
            }
            else
            {
                txtSpecifySource.Text = "";
                ddlDealerName.SelectedIndex = 0;
                txtSpecifySource.Enabled = true;
                ddlDealerName.Enabled = false;
            }
            txtSpecifySource.Text = dt1.Rows[0]["SourceSpecification"].ToString();
            ddlDealerName.SelectedValue = dt1.Rows[0]["DealerID"].ToString();
            txtAppFName.Text = dt1.Rows[0]["AppFName"].ToString();
            txtAppMName.Text = dt1.Rows[0]["AppMName"].ToString();
            txtAppLName.Text = dt1.Rows[0]["AppLName"].ToString();
            txtExistingPLCaseNo.Text = dt1.Rows[0]["ExistingPLCaseNo"].ToString();
            txtPANNo.Text = dt1.Rows[0]["PANNo"].ToString();
            ddlGender.SelectedValue = dt1.Rows[0]["Gender"].ToString();
            ddlMaritalStatus.SelectedValue = dt1.Rows[0]["MaritalStatus"].ToString();
            txtSpouse.Text = dt1.Rows[0]["Spouse"].ToString();
            txtChildren.Text = dt1.Rows[0]["Children"].ToString();
            txtBirthDate.Text = dt1.Rows[0]["BirthDate"].ToString();
            txtAge.Text = dt1.Rows[0]["Age"].ToString();
            txtBldgHouseName.Text = dt1.Rows[0]["BldgHouseName"].ToString();
            txtPlotNo.Text = dt1.Rows[0]["BldgPlotNo"].ToString();
            txtRoad.Text = dt1.Rows[0]["Road"].ToString();
            txtRoomBlockNo.Text = dt1.Rows[0]["RoomBlockNo"].ToString();
            State_RTR();
            ddlState.SelectedValue = dt1.Rows[0]["StateID"].ToString();
            City_RTR();
            ddlCity.SelectedValue = dt1.Rows[0]["CityID"].ToString();
            Area_RTR();
            ddlArea.SelectedValue = dt1.Rows[0]["AreaID"].ToString();
            Zone_RTR();
            ddlZone.SelectedValue = dt1.Rows[0]["ZoneID"].ToString();
            txtNearestLandmark.Text = dt1.Rows[0]["Landmark"].ToString();
            txtMobileNo.Text = dt1.Rows[0]["MobileNo"].ToString();
            txtTelephoneNo.Text = dt1.Rows[0]["TelephoneNo"].ToString();
            txtEmailId.Text = dt1.Rows[0]["EmailID"].ToString();
            ddlOccupation.SelectedValue = dt1.Rows[0]["Occupation"].ToString();
            ddlPresentAnnIncm.SelectedValue = dt1.Rows[0]["PresentIncome"].ToString();
            ddlemploymentdetails.SelectedValue = dt1.Rows[0]["EmploymentType"].ToString().Trim();
            if (ddlemploymentdetails.SelectedValue == "Other")
            {
                txtspecifyEmployment.Text = "";
                txtspecifyEmployment.Enabled = true;
            }
            else
            {
                txtspecifyEmployment.Text = "";
                txtspecifyEmployment.Enabled = false;
            }
            txtspecifyEmployment.Text = dt1.Rows[0]["SpecifyEmployment"].ToString().Trim();
            txtNameofOrg.Text = dt1.Rows[0]["OrganizationName"].ToString();
            txtOfficeAdd.Text = dt1.Rows[0]["OfficeAddress"].ToString();
            ddlIndustrytype.SelectedValue = dt1.Rows[0]["IndustriesType"].ToString();
            if (ddlIndustrytype.SelectedValue == "Other")
            {
                txtSpecifyIndustryType.Text = "";
                txtSpecifyIndustryType.Enabled = true;
            }
            else
            {
                txtSpecifyIndustryType.Text = "";
                txtSpecifyIndustryType.Enabled = false;
            }
            txtSpecifyIndustryType.Text = dt1.Rows[0]["SpecifyIndustries"].ToString();
            txtNomFName.Text = dt1.Rows[0]["NomFName"].ToString();
            txtNomMName.Text = dt1.Rows[0]["NomMName"].ToString();
            txtNomLName.Text = dt1.Rows[0]["NomLName"].ToString();
            txtNomineeAddress.Text = dt1.Rows[0]["NomAddress"].ToString();
            ddlNomRelation.SelectedValue = dt1.Rows[0]["NomRelation"].ToString();
            txtSpecifyDesigntn.Text = dt1.Rows[0]["Designation"].ToString();
            ddlLoanPurpose.SelectedValue = dt1.Rows[0]["LoanpurposeID"].ToString();
            if (ddlLoanPurpose.SelectedValue == "7")
            {
                txtLoanPurposespecify.Text = "";
                txtLoanPurposespecify.Enabled = true;
            }
            else
            {
                txtLoanPurposespecify.Text = "";
                txtLoanPurposespecify.Enabled = false;
            }
            txtLoanPurposespecify.Text = dt1.Rows[0]["SpecifyLoanPurpose"].ToString();
            gvDocumentDetails.DataSource = dt1;
            gvDocumentDetails.DataBind();


        }

        TextBox txtNameOnDoc = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");
        string name = "";
        if (dt1.Rows[0]["AppMName"].ToString().Trim() != "")
        {
            name = dt1.Rows[0]["AppFName"].ToString() + " " + dt1.Rows[0]["AppMName"].ToString();
        }
        else
        {
            name = dt1.Rows[0]["AppFName"].ToString();
        }
        if (dt1.Rows[0]["AppLName"].ToString().Trim() != "")
        {
            name = name + " " + dt1.Rows[0]["AppLName"].ToString();
        }
        txtNameOnDoc.Text = name;

    }
    public void GoldLoan_Search()
    {

        conn = new SqlConnection(strConnString);
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "GL_KYCGoldLoan_Search";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.AddWithValue("@SearchCeteria", Master.PropertyddlSearch.SelectedValue.Trim());
        cmd.Parameters.AddWithValue("@SearchText", Master.PropertytxtSearch.Text.Trim());
        cmd.Parameters.AddWithValue("@FYID", hdnFYear.Value);
        cmd.Parameters.AddWithValue("@BranchId", hdnBranch.Value);
        da = new SqlDataAdapter(cmd);
        dt = new DataTable();
        da.Fill(dt);

        Master.PropertygvGlobal.DataSource = dt;
        Master.DataBind();
        //added for sorting
        //  Master.PropertygvGlobal.UseAccessibleHeader = true;
        //  Master.PropertygvGlobal.HeaderRow.TableSection = TableRowSection.TableHeader;

        Master.PropertympeGlobal.Show();
    }

    protected void BindImageUrl()
    {
        conn = new SqlConnection(strConnString);
        //Query to get ImagesName and Description from database

        SqlCommand cmd = new SqlCommand("selectTop 1 KycPhoto from KycImageStore from order by ImgId desc", conn);
        cmd.CommandType = CommandType.Text;
        cmd = new SqlCommand();
        cmd.Connection = conn;
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        conn.Close();
    }


    protected void btnUploadPhoto_Click(object sender, EventArgs e)
    {
        try
       {
            
            Stream fs1 = fUploadPhoto.PostedFile.InputStream;
            BinaryReader br1 = new BinaryReader(fs1);
            Byte[] bytes = br1.ReadBytes((Int32)fs1.Length);
            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand("insert into KycImageStore(KycPhoto,Operation,Refno,CreatedBy)values(@KycPhoto,@Operation,@Refno,@CreatedBy)", conn);        
            cmd.Parameters.AddWithValue("@KycPhoto", bytes);
            cmd.Parameters.AddWithValue("@Operation", hdnoperation.Value.Trim());
            cmd.Parameters.AddWithValue("@Refno", hdnid.Value.Trim());
            cmd.Parameters.AddWithValue("@CreatedBy", hdnUserID.Value);
            cmd.ExecuteNonQuery();
            conn.Close();

            int MaxSizeAllowed = 1073741824; // 1GB...


            if (fUploadPhoto.HasFile)
            {
                string fileName = fUploadPhoto.FileName;
                string exten = Path.GetExtension(fileName);
               // hdnImageName.Value = fileName;
                //here we have to restrict file type            
                exten = exten.ToLower();
                string[] acceptedFileTypes = new string[4];
                acceptedFileTypes[0] = ".jpg";
                acceptedFileTypes[1] = ".jpeg";
                acceptedFileTypes[2] = ".gif";
                acceptedFileTypes[3] = ".png";
                bool acceptFile = false;
                for (int i = 0; i <= 3; i++)
                {
                    if (exten == acceptedFileTypes[i])
                    {
                        acceptFile = true;
                    }
                }

                
                var filePath = Server.MapPath(@"PhotoImage\");
                string[] filePaths = Directory.GetFiles(@filePath, "*.*");


                if (!acceptFile)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadPhoto.PostedFile.ContentLength > MaxSizeAllowed)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }

                string fnm = filePath + fileName;
                if (filePaths.Contains(fnm))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('This photo name is already exist please change the name !');", true);

                }
                else
                {
                    txtAppPhotoPath.Text = "PhotoImage/" + fileName;

                    //upload the file onto the server                   
                    fUploadPhoto.SaveAs(Server.MapPath("~/" + txtAppPhotoPath.Text));
                    //fUploadPhoto.SaveAs(Server.MapPath("~/" + txtAppPhotoPath.Text));

                    System.IO.Stream fs = fUploadPhoto.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    imgAppPhoto.ImageUrl = "data:image/png;base64," + base64String;
                    imgAppPhoto.ImageUrl = "PhotoImage/" + fileName;
                    imgAppPhoto.Visible = true;

                    //dt = new DataTable();
                    //dt.Columns.Add("ImagePath");

                    //dr = dt.NewRow();

                    //dr["ImagePath"] = "PhotoImage/" + fileName;
                    //dt.Rows.Add(dr);
                    //grdPhoto.DataSource = dt;
                    //grdPhoto.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UPhotoAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void btnUploadSign_Click(object sender, EventArgs e)
    {
        try
        {
            int MaxSizeAllowed = 1073741824; // 1GB...

            if (fUploadSign.HasFile)
            {
                string fileName = fUploadSign.FileName;
                string exten = Path.GetExtension(fileName);
                //here we have to restrict file type            
                exten = exten.ToLower();
                string[] acceptedFileTypes = new string[4];
                acceptedFileTypes[0] = ".jpg";
                acceptedFileTypes[1] = ".jpeg";
                acceptedFileTypes[2] = ".gif";
                acceptedFileTypes[3] = ".png";
                bool acceptFile = false;
                for (int i = 0; i <= 3; i++)
                {
                    if (exten == acceptedFileTypes[i])
                    {
                        acceptFile = true;
                    }
                }

                var filePath = Server.MapPath(@"SignImage\");
                string[] filePaths = Directory.GetFiles(@filePath, "*.*");

                if (!acceptFile)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadSignAlert", "alert('The file you are trying to upload is not a permitted file type!');", true);
                }
                else if (fUploadSign.PostedFile.ContentLength > MaxSizeAllowed)
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadSignAlert", "alert('The file you are trying to upload exceeded the file size limit of 1GB!');", true);
                }

                string fnm = filePath + fileName;
                if (filePaths.Contains(fnm))
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "fUploadPhotoAlert", "alert('This sign name is already exist please change the name !');", true);

                }


                else
                {
                    txtAppSignPath.Text = "SignImage/" + fileName;
                    //upload the file onto the server                   
                    fUploadSign.SaveAs(Server.MapPath("~/" + txtAppSignPath.Text));

                    System.IO.Stream fs = fUploadSign.PostedFile.InputStream;
                    System.IO.BinaryReader br = new System.IO.BinaryReader(fs);
                    Byte[] bytesPhoto = br.ReadBytes((Int32)fs.Length);
                    string base64String = Convert.ToBase64String(bytesPhoto, 0, bytesPhoto.Length);
                    //imgAppSign.ImageUrl = "data:image/png;base64," + base64String;
                    imgAppSign.ImageUrl = "SignImage/" + fileName;
                    imgAppSign.Visible = true;
                }
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "USignAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void btnCity_Click(object sender, EventArgs e)
    {
        City_RTR();
    }
    protected void btnenableother_Click(object sender, EventArgs e)
    {
        DropDownList ddlDocName = (DropDownList)gvDocumentDetails.FooterRow.FindControl("ddlDocName");
        TextBox txtSpecifyOther = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtSpecifyOther");

        if (ddlDocName.SelectedItem.Text == "Other")
        {
            txtSpecifyOther.Enabled = true;
        }
        else
        {
            txtSpecifyOther.Enabled = false;
        }
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        try
        {


            FileUpload fUpload = (FileUpload)gvDocumentDetails.FooterRow.FindControl("Fileupload1");
            DropDownList ddlDocName = (DropDownList)gvDocumentDetails.FooterRow.FindControl("ddlDocName");
            DropDownList ddlEmployeeName = (DropDownList)gvDocumentDetails.FooterRow.FindControl("ddlEmployeeName");
            TextBox txtNameOnDoc = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");
            TextBox txtSpecifyOtherDoc = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtSpecifyOther");
            if (ddlDocName.SelectedValue == "15")
            {
                if (txtSpecifyOtherDoc.Text == "")
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "KYC", "alert('Specify Ohter Document');", true);
                    return;
                }
            }
            dt = new DataTable();
            dt.Columns.Add("DID");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("DocumentID");
            dt.Columns.Add("OtherDoc");
            dt.Columns.Add("DocName");
            dt.Columns.Add("NameOnDoc");
            dt.Columns.Add("VerifiedBy");
            dt.Columns.Add("Empld");
            dt.Columns.Add("ImagePath");
            dt.Columns.Add("ImageUrl");
            for (int i = 0; i < gvDocumentDetails.Rows.Count; i++)
            {
                FileUpload fUploadd = (FileUpload)gvDocumentDetails.FooterRow.FindControl("Fileupload1");
                gvDocumentDetails.SelectedIndex = i;
                Label lblSrNo = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSrNo");
                HiddenField hdndocid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocid");
                HiddenField hdndocumentid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocumentid");
                Label lblDocName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocName");
                Label lblSpecifyOther = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSpecifyOther");
                Label lblDocOnName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocOnName");
                Label lblEmpname = (Label)gvDocumentDetails.SelectedRow.FindControl("lblEmpname");
                HiddenField hdnempid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnempid");

                HiddenField hdnimgpath = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnimgpath");
                Image imgDocPhoto = (Image)gvDocumentDetails.SelectedRow.FindControl("imgDocPhoto");

                dr = dt.NewRow();

                // dr[""] = lblSrNo.Text;
                dr["DID"] = hdndocid.Value;
                dr["Serialno"] = lblSrNo.Text;
                dr["DocumentID"] = hdndocumentid.Value;
                dr["OtherDoc"] = lblSpecifyOther.Text;
                dr["DocName"] = lblDocName.Text;
                dr["NameOnDoc"] = lblDocOnName.Text;
                dr["VerifiedBy"] = lblEmpname.Text;
                dr["Empld"] = hdnempid.Value;

                dr["ImagePath"] = hdnimgpath.Value;
                dr["ImageUrl"] = imgDocPhoto.ImageUrl;
                if (lblDocName.Text != "")
                {
                    dt.Rows.Add(dr);
                }

            }
            fUpload.SaveAs(Server.MapPath("~/DocumentImage/" + fUpload.FileName));
            string ImagePath = "DocumentImage/" + fUpload.FileName;
            dr = dt.NewRow();

            dr["DID"] = "0";
            if (dt.Rows.Count == 0)
            {
                dr["Serialno"] = 1;
            }
            else
            {
                dr["Serialno"] = gvDocumentDetails.Rows.Count + 1;
            }
            dr["DocumentID"] = ddlDocName.SelectedValue;
            dr["OtherDoc"] = txtSpecifyOtherDoc.Text;
            dr["DocName"] = ddlDocName.SelectedItem.Text;
            dr["NameOnDoc"] = txtNameOnDoc.Text;
            dr["VerifiedBy"] = ddlEmployeeName.SelectedItem.Text;
            dr["Empld"] = ddlEmployeeName.SelectedValue.Trim();
            dr["ImagePath"] = ImagePath;
            dr["ImageUrl"] = ImagePath;
            dt.Rows.Add(dr);
            gvDocumentDetails.DataSource = dt;
            gvDocumentDetails.DataBind();
            //----------------------------------------------------------------------------------------------
            TextBox txtNameOnDoc1 = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");
            string name = "";
            if (txtAppMName.Text != "")
            {
                name = txtAppFName.Text + " " + txtAppMName.Text;
            }
            else
            {
                name = txtAppFName.Text;
            }
            if (txtAppLName.Text != "")
            {
                name = name + " " + txtAppLName.Text;
            }
            txtNameOnDoc1.Text = name;

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "UDocAlert", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertyImgBtnClose_Click(object sender, EventArgs e)
    {

        Master.PropertympeGlobal.Hide();
        Master.PropertyddlSearch.SelectedIndex = 0;
        Master.PropertytxtSearch.Text = "";
        BlankGv();
        gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

    }
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnoperation.Value == "Save")
            {
                GoldLoan_KYC_PRV("Save", "0");
                GoldLoan_PRI("Save", "0");

            }
            if (hdnoperation.Value == "Update")
            {
                GoldLoan_KYC_PRV("Update", hdnid.Value.Trim());
                GoldLoan_PRI("Update", hdnid.Value.Trim());
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnEdit_Click(object sender, EventArgs e)
    {
        try
        {
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";
            GoldLoan_RTR();
            hdnpopup.Value = "Edit";
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            GoldLoan_KYC_PRV("Delete", hdnid.Value.Trim());
            GoldLoan_PRI("Delete", hdnid.Value.Trim());
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }
    }
    protected void PropertybtnView_Click(object sender, EventArgs e)
    {
        try
        {
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";
            GoldLoan_RTR();
            hdnpopup.Value = "View";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        try
        {
            ClearData();

            gbl.CheckAEDControlSettings("Cancel", Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);
        }
        catch (Exception ex)
        {
            //hdnoperation.Value = "";
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertybtnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            GoldLoan_Search();
            hdnpopup.Value = "View";
        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void PropertygvGlobal_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            int index = Convert.ToInt32(e.CommandArgument);
            Master.PropertygvGlobal.SelectedIndex = index;
            string id = Master.PropertygvGlobal.SelectedRow.Cells[0].Text;

            if (hdnpopup.Value == "Edit")
            {
                GoldLoan_Details_RTR(id);

            }
            if (hdnpopup.Value == "Search")
            {
                GoldLoan_Ex_Details_RTR(id);

            }
            if (hdnpopup.Value == "View")
            {
                GoldLoan_Details_RTR(id);
            }
            gbl.CheckAEDControlSettings(hdnpopup.Value.Trim(), Master.PropertybtnEdit, Master.PropertybtnSave, Master.PropertybtnDelete, Master.PropertybtnView, Master.PropertybtnCancel);

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void ddlState_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            City_RTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void ddlCity_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            Area_RTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    protected void ddlArea_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            Zone_RTR();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }

    protected void btnDelete_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            string name = "";
            if (txtAppMName.Text != "")
            {
                name = txtAppFName.Text + " " + txtAppMName.Text;
            }
            else
            {
                name = txtAppFName.Text;
            }
            if (txtAppLName.Text != "")
            {
                name = name + " " + txtAppLName.Text;
            }
            if (gvDocumentDetails.Rows.Count == 1)
            {
                BlankGv();
                TextBox txtNameOnDoc1 = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");
                txtNameOnDoc1.Text = name;
                return;
            }
            ImageButton ImgBtnRemove = (ImageButton)sender;
            GridViewRow row = (GridViewRow)ImgBtnRemove.NamingContainer;
            int index = row.RowIndex;

            dt = new DataTable();
            dt.Columns.Add("DID");
            dt.Columns.Add("Serialno");
            dt.Columns.Add("DocumentID");
            dt.Columns.Add("DocName");
            dt.Columns.Add("OtherDoc");
            dt.Columns.Add("NameOnDoc");
            dt.Columns.Add("VerifiedBy");
            dt.Columns.Add("Empld");
            dt.Columns.Add("ImagePath");
            dt.Columns.Add("ImageUrl");


            for (int i = 0; i < gvDocumentDetails.Rows.Count; i++)
            {
                gvDocumentDetails.SelectedIndex = i;


                Label lblSrNo = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSrNo");
                HiddenField hdndocid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocid");
                HiddenField hdndocumentid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdndocumentid");
                Label lblDocName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocName");
                Label lblSpecifyOther = (Label)gvDocumentDetails.SelectedRow.FindControl("lblSpecifyOther");
                Label lblDocOnName = (Label)gvDocumentDetails.SelectedRow.FindControl("lblDocOnName");
                Label lblEmpname = (Label)gvDocumentDetails.SelectedRow.FindControl("lblEmpname");
                HiddenField hdnempid = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnempid");

                HiddenField hdnimgpath = (HiddenField)gvDocumentDetails.SelectedRow.FindControl("hdnimgpath");
                Image imgDocPhoto = (Image)gvDocumentDetails.SelectedRow.FindControl("imgDocPhoto");

                if (i != index)
                {

                    dr = dt.NewRow();

                    // dr[""] = lblSrNo.Text;
                    dr["DID"] = hdndocid.Value;
                    dr["Serialno"] = lblSrNo.Text;
                    dr["DocumentID"] = hdndocumentid.Value;
                    dr["DocName"] = lblDocName.Text;
                    dr["OtherDoc"] = lblSpecifyOther.Text;
                    dr["NameOnDoc"] = lblDocOnName.Text;
                    dr["VerifiedBy"] = lblEmpname.Text;
                    dr["Empld"] = hdnempid.Value;
                    dr["ImagePath"] = hdnimgpath.Value;
                    dr["ImageUrl"] = imgDocPhoto.ImageUrl;

                    dt.Rows.Add(dr);

                }

            }
            gvDocumentDetails.DataSource = dt;
            gvDocumentDetails.DataBind();

            TextBox txtNameOnDoc = (TextBox)gvDocumentDetails.FooterRow.FindControl("txtNameOnDoc");

            txtNameOnDoc.Text = name;

        }
        catch (Exception ex)
        {

            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }


    }
    public void GoldLoan_KYC_PRV(string operation, string value)
    {
        string plcaseno = "";
        conn = new SqlConnection(strConnStringAIM);
        conn.Open();

        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandText = "select CaseNo From TDisbursement_Appl_BasicInfo where CaseNo='" + txtExistingPLCaseNo.Text.Trim() + "'";
        if (cmd.ExecuteScalar() != DBNull.Value)
        {
            plcaseno = Convert.ToString(cmd.ExecuteScalar());
        }
        else
        {
            plcaseno = "";
        }

        conn = new SqlConnection(strConnString);
        conn.Open();
        cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "GL_KYC_PRV";
        cmd.Parameters.AddWithValue("@Operation", operation);
        cmd.Parameters.AddWithValue("@KYCID", value);
        cmd.Parameters.AddWithValue("@MobileNo", txtMobileNo.Text);
        cmd.Parameters.AddWithValue("@Verify", hdnverify.Value);
        cmd.Parameters.AddWithValue("@GoldLoanNo", txtCutomerID.Text);
        cmd.Parameters.AddWithValue("@AimPL", plcaseno.Trim());
        cmd.Parameters.AddWithValue("@ExistingPLCaseNo", txtExistingPLCaseNo.Text.Trim());
        cmd.ExecuteNonQuery();
        conn.Close();


    }
    public string GenerateRandomText()
    {
        StringBuilder stringBuilder = new StringBuilder(this._randomTextLength);
        int length = this._randomTextChars.Length;
        for (int index = 0; index <= this._randomTextLength - 1; ++index)
            stringBuilder.Append(this._randomTextChars.Substring(rand.Next(length), 1));
        return ((object)stringBuilder).ToString();
    }
    protected void btnVerify_Click(object sender, EventArgs e)
    {
        hdnverify.Value = GenerateRandomText();

        string MobileNo = txtMobileNo.Text.Trim();
        txtVerification.Text = hdnverify.Value.Trim();

        // Write code to send Mobile SMS.
        try
        {
            if (MobileNo != "" && MobileNo != null)
            {
                SendMobileMessage(MobileNo, hdnverify.Value);
                ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('Verification code to customer's mobile is sucsessfully sent!');", true);
            }

        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }

    }
    public void SendMobileMessage(string MobileNo, string VerificationCode)
    {
        try
        {
            if (MobileNo.Trim() != "")
            {
                string Message = string.Empty;
                string sURL;
                string Success = string.Empty;

                Message = "http://103.242.119.152/vendorsms/pushsms.aspx?user=afplgl&password=afplgl14&msisdn=" + MobileNo + "&sid=AphGLN&msg=Dear Customer, your verification ID is " + VerificationCode + ". Kindly get it verified with our staff to get the registration completed. Aphelion Finance,Tel. 022 25925959&fl=0&gwid=2";

                StreamReader objReader;
                sURL = Message;
                WebRequest wrGETURL;
                wrGETURL = WebRequest.Create(sURL);

                Stream objStream;
                objStream = wrGETURL.GetResponse().GetResponseStream();
                objReader = new StreamReader(objStream);
                objReader.Close();
            }
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }
    }
    protected void imgbtnExCustomer_Click(object sender, ImageClickEventArgs e)
    {
        try
        {
            Master.PropertyddlSearch.SelectedIndex = 0;
            Master.PropertytxtSearch.Text = "";
            GoldLoan_RTR();
            hdnpopup.Value = "Search";
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('" + ex.Message + "');", true);
        }
    }

    protected void ImgBtnClose1_Click(object sender, ImageClickEventArgs e)
    {
        modal1.Hide();
    }
    protected void btnpan_Click(object sender, EventArgs e)
    {
        Boolean IsValidPan;
        //Regex rgx = new Regex(@"^[a-zA-Z0-9]\d{2}[a-zA-Z0-9](-\d{3}){2}[A-Za-z0-9]$");
        //Regex rgx = new Regex(@"^[a-zA-Z0-9]\d{2}[a-zA-Z0-9](-\d{3}){2}[A-Za-z0-9]$"); 

        if (txtPANNo.Text != "")
        {
            Match m = Regex.Match(txtPANNo.Text, @"^[A-Z/a-z]{5}\d{4}[A-Z/a-z]{1}$");
            IsValidPan = m.Success;
            if (IsValidPan == true)
            {
                conn = new SqlConnection(strConnString);
                cmd = new SqlCommand();
                cmd.Connection = conn;
                cmd.CommandText = "GL_KYC_PANNoTrack";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@PanNo", txtPANNo.Text);
                da = new SqlDataAdapter(cmd);
                dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    string PANNo = txtPANNo.Text.ToString();
                    string s = PANNo.ToUpper();
                    lblenteredpanno.Text = "Entered Pan No " + s + " is already exists";
                    grdpan.DataSource = dt;
                    grdpan.DataBind();
                    modal1.Show();
                }
            }
            else
            {
                ClientScript.RegisterStartupScript(this.GetType(), "GLKYCDetails", "alert('Invalid Pan No');", true);
            }
        }

    }
}
