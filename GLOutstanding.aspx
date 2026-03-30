<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLOutstanding.aspx.cs" Inherits="GLOutstanding" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Src="UserControl/AutoRedirect.ascx" TagName="AutoRedirect" TagPrefix="uc1" %>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <asp:HiddenField ID="hdnUserID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnFYearID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnBranchID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnDefaultOSLevel" runat="server" Value="0" />
    <%--<asp:GridView ID="gvOutstanding" runat="server" Width="100%">
        <AlternatingRowStyle BackColor="White" />
        <HeaderStyle CssClass="gVHeader" />
        <Columns>
        <asp:TemplateField HeaderText="Loan Date">
        <ItemTemplate>
            <asp:Label ID="lblLoanDate" runat="server" Text='<%#Eval("Loan Date") %>'></asp:Label>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Gold Loan No">
        <ItemTemplate>
         <asp:Label ID="lblGoldLoanno" runat="server" Text='<%#Eval("Gold Loan No") %>'></asp:Label>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Name">
        <ItemTemplate>
         <asp:Label ID="lblGoldLoanno" runat="server" Text='<%#Eval("Name") %>'></asp:Label>
        </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="O/s Percentage">
        <ItemTemplate>
           <asp:Label ID="lblGoldLoanno" runat="server" Text='<%#Eval("O/s Percentage") %>'></asp:Label>
        </ItemTemplate>
        </asp:TemplateField>
        </Columns>
    </asp:GridView>--%>
</asp:Content>
