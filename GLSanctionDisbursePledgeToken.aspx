<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLSanctionDisbursePledgeToken.aspx.cs" Theme="GridViewTheme" EnableViewStateMac="false"
    Inherits="GLSanctionDisbursePledgeToken" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <div>
        <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <!--Header -->
            <%--  <tr>
                <td align="center" class="header">
                    <asp:Label ID="lblHeader" runat="server" Font-Underline="true" Text="Pledge Token"></asp:Label>
                </td>
            </tr>--%>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <div align="center" style="border: 2px solid Gray; width: 95%;">
        <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
            <tr>
                <td style="width: 12%;">
                </td>
                <td style="width: 28%;">
                </td>
                <td style="width: 15%;">
                </td>
                <td style="width: 25%;">
                </td>
            </tr>
            <tr>
                <td style="height: 40px;" align="center" class="header" colspan="4">
                    <asp:Label ID="lblPledge" runat="server" Font-Underline="true" Text="Pledge Token"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="left" class="label">
                    <div align="left">
                        <asp:Label ID="lblGoldLoanNo" runat="server" Text="Gold Loan No "></asp:Label>
                    </div>
                </td>
                <td align="left" class="label">
                    <div align="left">
                        <asp:Label ID="lblGoldNo" runat="server"></asp:Label>
                    </div>
                </td>
                <td align="left" class="label">
                    <div align="left">
                        <asp:Label ID="lblIssue" runat="server" Text="Issue Date"></asp:Label>
                    </div>
                </td>
                <td align="left" class="label">
                    <div align="left">
                        <asp:Label ID="lblIssueDate" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td class="label" align="left">
                    <div align="left">
                        <asp:Label ID="lblApp" runat="server" Text="Applicant Name" ReadOnly="true"></asp:Label>
                    </div>
                </td>
                <td class="label" align="left" colspan="2">
                    <div align="left">
                        <asp:Label ID="lblAppName" runat="server"></asp:Label>
                    </div>
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="label" align="left">
                    <div align="left">
                        <asp:Label ID="lblNetLoan" runat="server" Text="Net Loan Amount"></asp:Label>
                    </div>
                </td>
                <td class="label" align="left" colspan="3">
                    <div align="left">
                        <asp:Label ID="lblNetLoanAmount" runat="server"></asp:Label>&nbsp;&nbsp; (<asp:Label
                            ID="lblAmtInWord" runat="server"></asp:Label>)
                        <%--<asp:TextBox ID="txtNetLoanAmount" runat="server" class="textbox" ReadOnly="true"></asp:TextBox>--%>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="height: 40px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="label" colspan="4" align="left">
                    <div align="left">
                        <asp:Label ID="lblAppDetails" runat="server" Font-Bold="true" Font-Size="Medium"
                            Font-Underline="true" Text="Applicant's Gold Item Details"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="4">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="GID" AutoPostBack="true"
                                Width="100%" OnPageIndexChanging="dgvDetails_PageIndexChanging">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="GID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGID" align="center" Width="60px" runat="server" Text='<%# Eval("GID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gold Item Name" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldItemName" align="center" Width="60px" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gross Weight" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrossWeight" runat="server" Width="60px" Text='<%# Eval("GrossWeight") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Photo" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Image ID="imgPhoto" runat="server" Width="60px" Height="50px" ImageUrl='<%#Eval("PhotoPath") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td align="left">
                    <div align="left">
                        <asp:Label ID="lblGross" class="label" runat="server" Text="Total Gross Weight"></asp:Label>
                    </div>
                </td>
                <td class="label" align="left">
                    <div align="left">
                        <asp:Label ID="lblGrossWeight" class="label" runat="server"></asp:Label>
                        <%-- <asp:TextBox ID="txtTotalGrossWeight" class="textbox" ReadOnly="true" runat="server"></asp:TextBox>--%>
                    </div>
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td align="left">
                    <div align="left">
                        <asp:Label ID="lblNet" class="label" runat="server" Text="Net Weight"></asp:Label>
                    </div>
                </td>
                <td class="label" align="left">
                    <div align="left">
                        <asp:Label ID="lblNetWeight" class="label" runat="server"></asp:Label>
                        <%-- <asp:TextBox ID="txtNetWeight" class="textbox" ReadOnly="true" runat="server"></asp:TextBox>--%>
                    </div>
                </td>
                <td>
                    &nbsp;
                </td>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4" align="justify">
                    <asp:Label ID="Label1" class="label" runat="server" Font-Size="Medium" Text="I do hereby state that the articles pledged with you,as per the details above,are owned by me. I also state that I will redeem the same within the prescribed time limit by remitting the principal amount and interest thereon. I promise abide by the conditions,given overleaf.In case I fail to redeem the pledged articles,within the time limit,I authorize Nancy Fincorp(unit of rajeswari fincom ltd.) to auction/sell the pledged articles to realize the loan amount and interest. If the amount so realized happens to be insufficient to cover the liabilities of the pledge.Nancy Fincorp(unit of Rajeshwari fincom ltd.)will have the rights to realize the balance dues by charging on my other assets. "></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="height: 50px;">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4" align="right">
                    <asp:Label ID="Label2" class="label" runat="server" Text="(Signature)"></asp:Label>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
