<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLSanctionDisburseDetailsReport.aspx.cs" Inherits="GLSanctionDisburseDetailsReport"
    Theme="GridViewTheme" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtIssueDateFrom.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtIssueDateTo.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 17%;">
            </td>
            <td style="width: 37%;">
            </td>
            <td style="width: 2%;">
            </td>
            <td style="width: 5%;">
            </td>
            <td style="width: 37%;">
            </td>
            <td style="width: 2%;">
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="6" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Report: GL KYC S&D Details"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Form Design -->
        <tr>
            <!--Branch -->
            <td class="label" style="font-weight: bold;">
                Branch :
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlBranch" class="textbox" Height="27px" Width="85%" runat="server"
                    OnSelectedIndexChanged="ddlBranch_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="reqBranch" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="ddlBranch" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
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
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!--Report Type -->
            <td class="label" style="font-weight: bold;">
                Report Type :
            </td>
            <td class="txt_style">
                <asp:RadioButtonList ID="rdlReportType" runat="server" RepeatDirection="Horizontal"
                    CssClass="label" OnSelectedIndexChanged="rdlReportType_SelectedIndexChanged"
                    AutoPostBack="true">
                    <asp:ListItem Text="Single" Value="Single"></asp:ListItem>
                    <%--<asp:ListItem Text="Multiple" Value="Multiple"></asp:ListItem>--%>
                    <asp:ListItem Text="All" Value="All"></asp:ListItem>
                </asp:RadioButtonList>
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
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="5">
                <!--Req Validator for Report Type -->
                <asp:RequiredFieldValidator ID="rfvrdlReportType" runat="server" ControlToValidate="rdlReportType"
                    Display="Dynamic" ErrorMessage="Select Report Type." Text="*" ForeColor="Red"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True" ValidationGroup="save">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!--Loan Date From -->
            <td class="label" style="font-weight: bold;">
                Loan Date From :
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtIssueDateFrom" CssClass="textbox" runat="server" Width="81%"
                    AutoPostBack="true" MaxLength="10" placeholder="dd/mm/yyyy" ClientIDMode="Static"
                    OnTextChanged="txtIssueDateFrom_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqIssueDateFrom" runat="server" ErrorMessage="*"
                    ControlToValidate="txtIssueDateFrom" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtIssueDateFrom"
                    Display="Dynamic" ErrorMessage="*" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                    Type="Date" ValidationGroup="save" Font-Bold="True" Font-Size="Small">*</asp:CompareValidator>
            </td>
            <td>
                &nbsp;
            </td>
            <!--Loan Date To -->
            <td class="label" style="font-weight: bold;">
                To :
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtIssueDateTo" CssClass="textbox" runat="server" Width="81%" MaxLength="10"
                    AutoPostBack="true" placeholder="dd/mm/yyyy" ClientIDMode="Static" OnTextChanged="txtIssueDateTo_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtIssueDateTo" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtIssueDateTo"
                    Display="Dynamic" ErrorMessage="*" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                    Type="Date" ValidationGroup="save" Font-Bold="True" Font-Size="Small">*</asp:CompareValidator>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Customer Name -->
            <td class="label" style="font-weight: bold;">
                Select Customer :
            </td>
            <td class="txt_style" colspan="5">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCustomerName" runat="server" CssClass="textbox" Height="27px"
                            Width="91%" AutoPostBack="true" ClientIDMode="Static">
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!--Loan Status -->
            <td class="label" style="font-weight: bold;">
                Loan Status :
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlAccountStatus" class="textbox" Height="27px" Width="85%"
                    runat="server">
                    <asp:ListItem>All</asp:ListItem>
                    <asp:ListItem>Open</asp:ListItem>
                    <asp:ListItem>Close</asp:ListItem>
                </asp:DropDownList>
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
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="6">
                <br />
                <br />
                <asp:Button ID="btnShowDetails" runat="server" Text="Show Report" CssClass="button"
                    ValidationGroup="save" Width="105px" OnClick="btnShowDetails_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Label ID="lblMsg" class="label" Style="color: Maroon; font-size: small;" runat="server"
                    Font-Names="Verdana"></asp:Label>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <%--<tr>
            <td align="right" colspan="6">
                <asp:Label ID="Label28" runat="server" Text="[View Details Section]" Font-Names="Verdana"
                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <div id="div1" runat="server">
                    <!--GridView [S & D Details]-->
                    <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="SDID" OnPageIndexChanging="dgvDetails_PageIndexChanging"
                                Width="100%" AutoGenerateColumns="False">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="SDID" HeaderText="ID" Visible="False" />
                                    <asp:HyperLinkField DataTextField="GoldLoanNo" HeaderText="Gold Loan No" DataNavigateUrlFields="SDID" DataNavigateUrlFormatString="GLSanctionDisburseViewDetailedReport.aspx?ID={0}"
                                        ItemStyle-HorizontalAlign="Center" Target="_blank"/>
                                    <asp:BoundField DataField="IssueDate" HeaderText="Issue Date" />
                                    <asp:BoundField DataField="ApplicantName" HeaderText="Customer's Name" />
                                    <asp:BoundField DataField="SanctionType" HeaderText="Sanction Type" />
                                    <asp:BoundField DataField="GoldNetValue" HeaderText="Net Gold Value" />
                                    <asp:BoundField DataField="NetLoanAmtSanctioned" HeaderText="Loan Amount" />
                                </Columns>
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ba2f14" Font-Bold="false" ForeColor="#ffffcc" Height="24px"
                                    Font-Names="Calibri" Font-Size="15px" />
                                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" Font-Names="Calibri" Font-Size="13px" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="true" ForeColor="Navy" Font-Names="Calibri"
                                    Font-Size="13px" />
                                <SortedAscendingCellStyle BackColor="#FDF5AC" />
                                <SortedAscendingHeaderStyle BackColor="#4D0000" />
                                <SortedDescendingCellStyle BackColor="#FCF6C0" />
                                <SortedDescendingHeaderStyle BackColor="#820000" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <br />
                <br />
            </td>
        </tr>--%>
    </table>
</asp:Content>
