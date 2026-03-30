<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLDefaultOustandingLevel.aspx.cs" Inherits="GLDefaultOustandingLevel" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function valid() {
            var ddlOSlevel = document.getElementById("<%=ddlOSlevel.ClientID %>");
            if (ddlOSlevel.selectedIndex == 0) { alert('Select Outstanding %'); return false; }
        }

    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 24.5%;">
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 24.5%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4" class="header">
                <asp:Label ID="lblHeader" runat="server" Text="GL Default Oustanding Level ">
                </asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Select Outstanding %:<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlOSlevel" runat="server" class="textbox" Style="height: 27px;
                    width: 95%;">
                    <%--   <asp:ListItem Value="0">--Select Outstanding %--</asp:ListItem>
                    <asp:ListItem Value="10">10</asp:ListItem>
                    <asp:ListItem Value="15">15</asp:ListItem>
                    <asp:ListItem Value="20">20</asp:ListItem>
                    <asp:ListItem Value="25">25</asp:ListItem>
                    <asp:ListItem Value="30">30</asp:ListItem>
                    <asp:ListItem Value="35">35</asp:ListItem>
                    <asp:ListItem Value="40">40</asp:ListItem>
                    <asp:ListItem Value="45">45</asp:ListItem>
                    <asp:ListItem Value="50">55</asp:ListItem>
                    <asp:ListItem Value="60">60</asp:ListItem>--%>
                </asp:DropDownList>
            </td>
            <td>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div style="border: 1px dotted  #d23b1d;">
                </div>
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <asp:HiddenField ID="hdnUserID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnFYearID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnBranchID" runat="server" Value="0" />
    </table>
</asp:Content>
