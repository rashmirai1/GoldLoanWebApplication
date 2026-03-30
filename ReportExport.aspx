<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ReportExport.aspx.cs" Inherits="ReportExport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link href="css/modelpopup.css" rel="stylesheet" type="text/css" />
    <link href="css/form_style.css" rel="stylesheet" type="text/css" />
</head>
<body>
    <form id="form1" runat="server">
    <div style="margin-left:10px;">
    <br />
    <div style="width:100%;text-align:left;" class="header">Aphelion Finance Pvt. Ltd.</div>
    <br />
    <div>
        <asp:Button ID="btnExport" runat="server" Text="Button" 
            onclick="btnExport_Click" />
    </div>
    <br />
        <asp:GridView ID="gvReport" runat="server">
            <AlternatingRowStyle BackColor="White" />
            <HeaderStyle CssClass="gVHeader" />
        </asp:GridView>
    </div>
    </form>
</body>
</html>
