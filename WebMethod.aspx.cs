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
using CrystalDecisions.CrystalReports.Engine;
using System.Drawing;
using System.Text;
//For Sending Mobile SMS
using System.Net;
using System.Net.Mail;
using System.Web.Services;
using System.Web.Script.Services;

public partial class WebMethod : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string GetGoldLoanNo(string LoanDate)
    {
        string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
        GlobalSettings gbl = new GlobalSettings();
        SqlConnection conn = new SqlConnection(strConnString);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "Gl_SanctionDisburse_GoldLoanNo_RTR";
        cmd.Parameters.AddWithValue("@LoanDate", gbl.ChangeDateMMddyyyy(LoanDate));
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);
        string goldLoanNo = string.Empty;
        if (dt.Rows.Count > 0)
        {
            goldLoanNo = dt.Rows[0]["GoldLoanNo"].ToString();
        }
        return goldLoanNo;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static string Goldrates()
    {
        string temp = string.Empty;
        try
        {
            //string urlAddress = "http://www.indiagoldrate.com/gold-rate-in-mumbai-today.htm";
            //string urlAddress = "https://www.moneycontrol.com/";
            //"https://www.moneycontrol.com/";
            string urlAddress = "https://www.goldpriceindia.com/wmshare-wlifop-002.php";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.CharacterSet == null)
                    readStream = new StreamReader(receiveStream);
                else
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                string data = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                
                //string abb = data.Substring(data.LastIndexOf("Gold Price in Mumbai"), 161);
                //string rate = abb.Substring(155, 6);
               
                string abb = data.Substring(data.LastIndexOf("pad-15"), 14);
                string rate = abb.Substring(8, 6);

                temp = rate.Replace(",", "");

                //temp = data.Substring(data.LastIndexOf("10 gram"), 30);
                //string ch = "Gold price slips marginally to Rs ";
                //string abb = data.Substring(data.LastIndexOf("Gold price slips marginally to Rs"), 40);
                //temp = abb.TrimStart(ch.ToCharArray());
                //temp = temp.Replace("<TD>", "");
                //temp = temp.Replace("</TD>", "");
                //temp = temp.Replace("\t", " ");
                //temp = temp.Replace("\n", " ");
                //temp = temp.Replace("10 gram", "");
                //temp = temp.Replace(" ", "");
                //temp = temp.Replace("Rs.", "");
                //temp = temp.Replace(",", "");

                //decimal TwentyThreeCarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.958);
                //lblGoldRate23.Text += " " + decimal.Round(TwentyThreeCarat, 2);

                //decimal TwentyTwokarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.916);
                //lblGoldRate22.Text += " " + decimal.Round(TwentyTwokarat, 2);

                //decimal TwentyOnekarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.875);
                //lblGoldRate21.Text += " " + decimal.Round(TwentyOnekarat, 2);

                //decimal Twentykarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.830);
                //lblGoldRate20.Text += " " + decimal.Round(Twentykarat, 2);

                //decimal Eighteenkarat = Convert.ToDecimal(temp) * Convert.ToDecimal(0.75);
                //lblGoldRate18.Text += " " + decimal.Round(Eighteenkarat, 2);
                //Hidden18k.Value = lblGoldRate18.Text;
            }
        }
        catch (Exception ex)
        {

        }
        return temp;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static IList<Account> GetAccount()
    {
        string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
        SqlConnection connAIM;
        SqlCommand cmd;
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select Name,AccountID from tblAccountmaster";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        var list = dt.AsEnumerable().Select(dataRow => new Account
        {
            AccountId = dataRow.Field<int>("AccountID"),
            Name = dataRow.Field<string>("Name"),

        }).ToList();
        return list;
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public static IList<Narration> GetNarration()
    {
        string strConnStringAIM = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString;
        SqlConnection connAIM;
        SqlCommand cmd;
        connAIM = new SqlConnection(strConnStringAIM);
        cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select  NarrationID,NarrationName  From tblNarrationMaster";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        DataTable dt = new DataTable();
        da.Fill(dt);

        var list = dt.AsEnumerable().Select(dataRow => new Narration
        {
            NarrationId = dataRow.Field<int>("NarrationID"),
            NarrationName = dataRow.Field<string>("NarrationName"),

        }).ToList();
        return list;

    }

    public class Narration
    {
        public int NarrationId { get; set; }
        public string NarrationName { get; set; }
    }

    public class Account
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
    }
}