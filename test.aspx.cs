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
using System.Web.Routing;

public partial class test : System.Web.UI.Page
{

    string strConnString = System.Configuration.ConfigurationManager.ConnectionStrings["NFConnectionStringLocal"].ConnectionString;
    SqlConnection conn;
    SqlCommand cmd;
    SqlDataAdapter da;
    DataTable dt;
    protected void Page_Load(object sender, EventArgs e)
    {
        //conn = new SqlConnection(strConnString);
        //cmd = new SqlCommand();
        //cmd.Connection = conn;
        //cmd.CommandText = "select '<table border=1 width=100%><tr><td>' + SchemeName +'</td></tr></table>' col  From TSchemeMaster_BasicDetails";
        //da = new SqlDataAdapter(cmd);
        //dt = new DataTable();
        //da.Fill(dt);
        //for (int i = 0; i < dt.Rows.Count; i++)
        //{
        //    Response.Write(dt.Rows[i]["col"].ToString());
        
        //}
   
    }
}