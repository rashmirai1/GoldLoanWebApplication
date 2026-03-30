<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCInterestDetailsReport.aspx.cs" Inherits="GLKYCInterestDetailsReport"
    Theme="GridViewTheme" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtPaymentDateFrom.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtPaymentDateTo.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 34%;">
            </td>
            <td style="width: 2%;">
            </td>
            <td style="width: 8%;">
            </td>
            <td style="width: 34%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Report: GL KYC Interest Details"></asp:Label>
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
                 <asp:RequiredFieldValidator ID="rfvrdlReportType" runat="server" ControlToValidate="rdlReportType"
                    Display="Dynamic" ErrorMessage="Select Report Type." Text="Select Report Type" ForeColor="Red"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True" ValidationGroup="save"></asp:RequiredFieldValidator>
            </td>
            <td>
               
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
                <%--  <asp:RequiredFieldValidator ID="rfvrdlReportType" runat="server" ControlToValidate="rdlReportType"
                    Display="Dynamic" ErrorMessage="Select Report Type." Text="*" ForeColor="Red"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True" ValidationGroup="save">*</asp:RequiredFieldValidator>--%>
            </td>
        </tr>
        <tr>
            <!--Payment Date From -->
            <td class="label" style="font-weight: bold;">
                Payment Date From :
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPaymentDateFrom" CssClass="textbox" runat="server" Width="81%"
                    AutoPostBack="true" MaxLength="10" placeholder="dd/mm/yyyy" ClientIDMode="Static"
                    OnTextChanged="txtPaymentDateFrom_TextChanged"></asp:TextBox>
                <%--<asp:RequiredFieldValidator ID="reqPaymentDateFrom" runat="server" ErrorMessage="*"
                    ControlToValidate="txtPaymentDateFrom" ValidationGroup="save" ForeColor="Red"
                    Display="Dynamic" Font-Bold="True" Font-Size="Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtPaymentDateFrom"
                    Display="Dynamic" ErrorMessage="*" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                    Type="Date" ValidationGroup="save" Font-Bold="True" Font-Size="Small">*</asp:CompareValidator>
            </td>
            <td>
                &nbsp;
            </td>
            <!--Payment Date To -->
            <td class="label" style="font-weight: bold;">
                To :
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPaymentDateTo" CssClass="textbox" runat="server" Width="81%"
                    AutoPostBack="true" MaxLength="10" placeholder="dd/mm/yyyy" ClientIDMode="Static"
                    OnTextChanged="txtPaymentDateTo_TextChanged"></asp:TextBox>
                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtPaymentDateTo" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtPaymentDateTo"
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
        <%--<tr>
            <!-- Select Customer Section -->
            <td colspan="6" align="left">
                <br />
                <br />
                <asp:Label ID="Label6" runat="server" Text="Select Multiple Customer(s) :" CssClass="label"
                    Font-Size="14px" Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>--%>
        <%--<tr>
            <td colspan="6">
                <div id="divCustomerDetails" runat="server">
                    <!--GridView Customer Details -->
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvCustomerDetails" runat="server" OnPageIndexChanging="dgvCustomerDetails_PageIndexChanging"
                                Width="98%" DataKeyNames="GoldLoanNo">
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
                                    <asp:TemplateField HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldLoanNo" align="center" runat="server" Text='<%# Eval("GoldLoanNo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Customer Name" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblApplicantName" align="center" runat="server" Text='<%# Eval("CustName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Select?" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
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
                            <asp:PostBackTrigger ControlID="dgvCustomerDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>--%>
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
    </table>
</asp:Content>
