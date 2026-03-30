using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for WebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class WebService : System.Web.Services.WebService
{

    public WebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod]
    public string HelloWorld()
    {
        return "Hello World";
    }

    [WebMethod]
    public void GetListofAccounts(string q)
    {
        SqlConnection connAIM = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = connAIM;
        cmd.CommandText = "select top 10 Name,AccountID from tblAccountmaster  where Name like '" + q + "%'";


        //cmd.CommandText = "select top 10  NarrationID,NarrationName  From tblNarrationMaster";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);
        List<data> lstData = new List<data>();

        data dtt = new data();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            dtt = new data();
            dtt.id = dt.Rows[i]["AccountID"].ToString();
            dtt.text = dt.Rows[i]["Name"].ToString();
            lstData.Add(dtt);
        }
        JavaScriptSerializer js = new JavaScriptSerializer();

        //this.Context.Response.Write("{ 'results' :[{'id':'1','text':'a'},{'id':'3','text':'b'}]}");
        //string result = new JavaScriptSerializer().Serialize(lstData);
        this.Context.Response.Write(js.Serialize(lstData));

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<data> GetAccount(long AccountID)
    {
        SqlConnection connAIM = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringAIM"].ConnectionString);
        SqlCommand cmd = new SqlCommand();
        cmd.Connection = connAIM;

        if (AccountID > 0)
        {
            cmd.CommandText = "select top 1 Name,AccountID from tblAccountmaster where AccountID  =  " + AccountID;
        }
        else
        {
            //cmd.CommandText = "select top 10 Name,AccountID from tblAccountmaster";
            cmd.CommandText = "select '' as Name,'' as AccountID from tblAccountmaster union select top 10 Name,AccountID from tblAccountmaster";
        }

        //cmd.CommandText = "select top 10  NarrationID,NarrationName  From tblNarrationMaster";
        SqlDataAdapter da = new SqlDataAdapter(cmd);
        System.Data.DataTable dt = new System.Data.DataTable();
        da.Fill(dt);
        List<data> lstData = new List<data>();

        data dtt = new data();
        for (int i = 0; i < dt.Rows.Count; i++)
        {
            dtt = new data();
            dtt.id = dt.Rows[i]["AccountID"].ToString();
            dtt.text = dt.Rows[i]["Name"].ToString();
            lstData.Add(dtt);
        }
        return lstData;
    }

    [System.Runtime.Serialization.DataContractAttribute]
    public class data
    {
        public string id { get; set; }
        public string text { get; set; }
    }

}
