using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class GLDefaultOustandingLevel : System.Web.UI.Page
{
    #region [Declarations]
    //Setting Database Connection
    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;

    //Declaring Variables.   
    string m_strQuery = string.Empty;
    int m_result;
    bool m_datasaved = false;

    //Declaring Objects.   
    SqlConnection conn, connAIM;
    SqlTransaction transactionGL, transactionAIM;
    SqlDataAdapter da;
    DataSet ds;
    SqlCommand cmd;
    DataTable dt;
    #endregion


    #region [Page_Init]
    protected void Page_Init(object sender, EventArgs e)
    {
        Master.PropertybtnSave.Click += new EventHandler(PropertybtnSave_Click);
        Master.PropertybtnCancel.Click += new EventHandler(PropertybtnCancel_Click);
    }
    #endregion [Page_Init]

    #region [Page_Load]
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
            }
            Master.PropertybtnSave.OnClientClick = "return valid();";
            Get_DefOSlevel();
            Get_AllDefOSlevel();

            Master.PropertybtnCancel.Visible = true;
            Master.PropertybtnDelete.Visible = false;
            Master.PropertybtnEdit.Visible = false;
            Master.PropertybtnView.Visible = false;
            Master.PropertybtnSave.Visible = true;
        }
    }
    #endregion [Page_Load]

    #region [Get_AllDefOSlevel]
    public void Get_AllDefOSlevel()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SelectDefPercentage";
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ddlOSlevel.DataSource = dt;
                ddlOSlevel.DataTextField = "OSLevel";
                ddlOSlevel.DataValueField = "DefOsID";
                ddlOSlevel.DataBind();
                ddlOSlevel.Items.Insert(0, new ListItem("--Select Outstanding %--", "0"));
            }

        }
        catch (Exception EX)
        {


        }
    }
    #endregion[Get_AllDefOSlevel]

    #region [Get_DefOSlevel]
    public void Get_DefOSlevel()
    {
        try
        {
            conn = new SqlConnection(strConnString);
            cmd = new SqlCommand();
            cmd.Connection = conn;
            conn.Open();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_SelectDefaultOSLevel";

            cmd.Parameters.AddWithValue("@UserID", hdnUserID.Value);
            da = new SqlDataAdapter(cmd);
            dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                ddlOSlevel.DataSource = dt;
                ddlOSlevel.SelectedValue = (dt.Rows[0]["DefOsID"].ToString());
            }

        }
        catch (Exception EX)
        {


        }
    }
    #endregion[Get_DefOSlevel]

    #region [PropertybtnCancel]
    protected void PropertybtnCancel_Click(object sender, EventArgs e)
    {
        ddlOSlevel.SelectedIndex = 0;
    }
    #endregion [PropertybtnCancel]

    #region [PropertybtnSave_Click]
    protected void PropertybtnSave_Click(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();
            cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GL_InsertUpdate_DefaultOSLevel";

            cmd.Parameters.AddWithValue("@DefOSLevel", ddlOSlevel.SelectedItem.Text);
            cmd.Parameters.AddWithValue("@DefOsID", ddlOSlevel.SelectedValue);
            cmd.Parameters.AddWithValue("@CreatedBy", hdnUserID.Value);
            cmd.Parameters.AddWithValue("@FYID", hdnFYearID.Value);
            cmd.Parameters.AddWithValue("@BranchID", hdnBranchID.Value);

            m_result = cmd.ExecuteNonQuery();
            if (m_result > 0)
            {
                m_datasaved = true;
            }
            else
            {
                m_datasaved = false;
            }

            if (m_datasaved)
            {
                Page.ClientScript.RegisterStartupScript(this.GetType(), "SD", "alert('Record Saved Successfully');", true);

            }

        }
        catch (Exception ex)
        {

            //throw;
        }
        finally
        {
            conn.Close();
        }
    }
    #endregion [PropertybtnSave_Click]

}