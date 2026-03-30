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

public partial class GLSanctionDisbursePledgeToken : System.Web.UI.Page
{

    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    SqlConnection conn;
    SqlDataAdapter da;
    DataTable dt;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            conn = new SqlConnection(strConnString);
            conn.Open();            

            lblGoldNo.Text = Convert.ToString(Session["GoldLoanNo"]);
            lblIssueDate.Text =Convert.ToDateTime(Session["IssueDate"]).ToString("dd/MM/yyyy");
         
            lblAppName.Text= Convert.ToString(Session["AppName"]);
            lblGrossWeight.Text= Convert.ToString(Session["TotalGrossWeight"]);
            lblNetWeight.Text =Convert.ToString(Session["NetWeight"]);
            lblNetLoanAmount.Text = Convert.ToString(Session["LoanAmount"]);
             int inputVal=Convert.ToInt32(lblNetLoanAmount.Text);
             lblAmtInWord.Text = Convert.ToString(NumberToWords(inputVal));

            BindDGVDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "SaveAlert", "alert('" + ex.Message + "');", true);
        }
        finally
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }
    }



    #region Bind GridView GoldValue Details
    protected void BindDGVDetails()
    {
        try
        {
            string strQuery = "SELECT DISTINCT tbl_GLSanctionDisburse_GoldItemDetails.GID,  tblItemMaster.ItemName ," +
                         "tbl_GLSanctionDisburse_GoldItemDetails.GrossWeight,tbl_GLSanctionDisburse_GoldItemDetails.PhotoPath  " +
                       "FROM tbl_GLSanctionDisburse_GoldItemDetails " +
                       "INNER JOIN tblItemMaster " +
                       "ON tbl_GLSanctionDisburse_GoldItemDetails.ItemID=tblItemMaster.ItemID " +
                       "WHERE  tbl_GLSanctionDisburse_GoldItemDetails.GoldLoanNo='" + Session["GoldLoanNo"] + "' ";

            conn = new SqlConnection(strConnString);
            da = new SqlDataAdapter(strQuery, conn);
            dt = new DataTable();
            da.Fill(dt);
            dgvDetails.DataSource = dt;
            dgvDetails.DataBind();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "BindDGVAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion 
  


    public static string NumberToWords(int number)
    {
        if (number == 0)
            return "zero";

        if (number < 0)
            return "minus " + NumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 1000000) > 0)
        {
            words += NumberToWords(number / 1000000) + " Million ";
            number %= 1000000;
        }

        if ((number / 1000) > 0)
        {
            words += NumberToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += NumberToWords(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            if (words != "")
                words += "and ";

            var unitsMap = new[] { "zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += "-" + unitsMap[number % 10];
            }
        }

        return words;
    }

    



    #region dgvDetails_PageIndexChanging
    protected void dgvDetails_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            this.dgvDetails.PageIndex = e.NewPageIndex;
            BindDGVDetails();
        }
        catch (Exception ex)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "DGVPgChgAlert", "alert('" + ex.Message + "');", true);
        }
    }
    #endregion
}