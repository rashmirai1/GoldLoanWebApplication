<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCBankGoldDetailsForm.aspx.cs" Inherits="GLKYCBankGoldDetailsForm"
    Theme="GridViewTheme" EnableViewStateMac="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        function ConfirmFunction(str) {
            var x;
            var r = confirm(str);
            if (r == true) {
                x = true;
                return x;
            }
            else {
                x = false;
                return x;
            }
        }
        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }

        function isNumericSlash(e) { // Numbers and slash
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 47));
        }


        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 45))
        }
        //      || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0
    </script>
    <%--<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>--%>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 18%;">
            </td>
            <td style="width: 31%;">
            </td>
            <td style="width: 22%;">
            </td>
            <td style="width: 30%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="4" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL KYC Bank Gold Details"></asp:Label>
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
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Form Design -->
        <tr>
            <td class="label">
                Reference Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtRefDate" CssClass="textbox" onkeypress="return isNumericSlash(event);"
                            AutoPostBack="true" runat="server" Width="89%" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                            ControlToValidate="txtRefDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="XX-Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td class="label">
                Location Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlLocationType" runat="server" CssClass="textbox" Height="29px"
                    Width="91.5%" AutoPostBack="True" OnSelectedIndexChanged="ddlLocationType_SelectedIndexChanged">
                    <asp:ListItem>--Select Location Type--</asp:ListItem>
                    <asp:ListItem>Locker</asp:ListItem>
                    <asp:ListItem>OD</asp:ListItem>
                    <asp:ListItem>Office</asp:ListItem>
                    <asp:ListItem>Home</asp:ListItem>
                </asp:DropDownList>
                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlLocationType" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Location Type--">*</asp:RequiredFieldValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txtRefDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                Operator Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtOperator" ReadOnly="true" CssClass="textbox_readonly" runat="server"
                    Width="95%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Bank Name -->
            <td class="label">
                Bank Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlBankName" runat="server" CssClass="textbox" Height="27px"
                            Width="97%" AutoPostBack="True" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:SqlDataSource ID="sdsBankName" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                            SelectCommand="SELECT [BankName], [BankID] FROM [tblBankMaster]"></asp:SqlDataSource>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*"
                            ControlToValidate="ddlBankName" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                            Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Bank Name--">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Branch Name -->
            <td class="label">
                Branch Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBranchName" CssClass="textbox_readonly" runat="server" Width="95%"></asp:TextBox>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*"
                            ControlToValidate="txtBranchName" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                            Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBankName" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <!-- Reference Date (Date Format validation) -->
        <tr>
            <!-- Location Type -->
            <!-- Location No -->
            <td class="label">
                Location No
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLocationNo" CssClass="textbox" runat="server" Width="89%" MaxLength="8"
                    onkeypress="return isAlphaNumChars(event);"></asp:TextBox>
                <%--<asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*"
                    ControlToValidate="txtLocationNo" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
            <td class="label">
                Unique Bank Customer ID
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtUniqueID" CssClass="textbox" runat="server" Width="87%" MaxLength="8"
                    onkeypress="return isAlphaNumChars(event);" AutoPostBack="True" OnTextChanged="txtUniqueID_TextChanged"></asp:TextBox>
                <%--   <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ErrorMessage="*"
                    ControlToValidate="txtUniqueID" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    ValidationGroup="save" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
        </tr>
        <tr>
            <td class="label">
                Reference No
            </td>
            <td>
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtRefNo" CssClass="textbox_readonly" ValidationGroup="save" runat="server"
                            Width="89%"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBankName" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtUniqueID" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="txtRefNo" ForeColor="Red" ValidationGroup="save" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
            <td class="label">
                Interest Rate
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtInterestRate" CssClass="textbox" runat="server" Width="87%" MaxLength="5"
                    onkeypress="return isNumeric(event);"></asp:TextBox>
                <asp:RegularExpressionValidator ID="RevInterestRate" runat="server" ControlToValidate="txtInterestRate"
                    Display="Dynamic" ForeColor="red" Font-Size="Small" ErrorMessage="Enter valid number.[eg.2.5]"
                    ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <!-- Deposit From Date -->
            <td class="label">
                Deposit From Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtDepositFromDate" CssClass="textbox" runat="server" Width="89%"
                    onkeypress="return isNumericSlash(event);" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                <%--  <asp:RequiredFieldValidator ID="reqDepositFromDate" runat="server" ErrorMessage="*"
                    ControlToValidate="txtDepositFromDate" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
            <!-- Deposit To Date -->
            <td class="label">
                Deposit To Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtDepositToDate" CssClass="textbox" runat="server" Width="87%"
                    onkeypress="return isNumericSlash(event);" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtDepositToDate" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtDepositFromDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" Font-Bold="True"></asp:CompareValidator>
            </td>
            <td>
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtDepositToDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <!-- Rate of Interest -->
            <td>
                <asp:TextBox ID="txtFYID" runat="server" Visible="false"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtOperatorID" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Select Customer Section -->
            <td colspan="4" align="left">
                <br />
                <br />
                <asp:Label ID="Label6" runat="server" Text="Select Customer(s) :" CssClass="label"
                    Font-Size="16px" Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="divCustomerDetails" runat="server">
                    <!--GridView Customer Details -->
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvCustomerDetails" runat="server" OnRowCommand="dgvCustomerDetails_RowCommand"
                                OnPageIndexChanging="dgvCustomerDetails_PageIndexChanging" Width="98%" AllowPaging="True"
                                PageSize="20">
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
                                    <%-- <asp:BoundField DataField="SDID" HeaderText="ID" Visible="False" />--%>
                                    <asp:TemplateField HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldLoanNo" align="center" runat="server" Text='<%# Eval("GoldLoanNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Applicant Name" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblApplicantName" align="center" runat="server" Text='<%# Eval("ApplicantName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mobile No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMobileNo" align="center" runat="server" Text='<%# Eval("MobileNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total NetWeight" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalNetWeight" align="center" runat="server" Text='<%# Eval("TotalNetWeight") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="AppID" Visible="false" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAppID" align="center" runat="server" Text='<%# Eval("AppID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <%--  <asp:BoundField DataField="GoldLoanNo" HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ApplicantName" HeaderText="Applicant Name" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="TotalNetWeight" HeaderText="Total NetWeight" ItemStyle-HorizontalAlign="Center" />--%>
                                    <asp:TemplateField HeaderText="Select?" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="ChkChanged" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ba2f14" Font-Bold="false" ForeColor="#ffffcc" Height="24px"
                                    Font-Names="Calibri" Font-Size="15px" />
                                <PagerSettings PageButtonCount="20" />
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
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="4">
                <br />
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
