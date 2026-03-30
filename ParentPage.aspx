<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="ParentPage.aspx.cs"
    EnableEventValidation="false" Inherits="ParentPage" EnableViewStateMac="false" %>


<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Src="UserControl/AutoRedirect.ascx" TagName="AutoRedirect" TagPrefix="uc1" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
        <tr>
            <td style="width: 75%;">
                <div>
                    <!-- -->
                </div>
            </td>
        </tr>
        <asp:HiddenField ID="hdnUserID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnFYearID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnBranchID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnDefaultOSLevel" runat="server" Value="0" />
    </table>
    
</asp:Content>
