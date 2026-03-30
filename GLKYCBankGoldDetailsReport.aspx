<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCBankGoldDetailsReport.aspx.cs" Inherits="GLKYCBankGoldDetailsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
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
    <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 35%;">
            </td>
            <td style="width: 16%;">
            </td>
            <td style="width: 20%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 99%;">
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Report GL KYC Bank Gold Details"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td class="label">
                Select Branch Name
            </td>
            <td colspan="3" class="txt_style">
                <asp:DropDownList ID="ddlBranchName" class="textbox" runat="server" Height="28px"
                    AutoPostBack="true" Width="218px" OnSelectedIndexChanged="ddlBranchName_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="reqCountry" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="ddlBranchName" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Branch Name--">*</asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlBranchName" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                From Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtFromDate" CssClass="textbox" runat="server" onkeypress="return isNumericSlash(event);"
                    MaxLength="10" Height="18px" Width="75%" placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReqBirthDate" runat="server" ErrorMessage="*" ControlToValidate="txtFromDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtFromDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label">
                To Date
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtToDate" CssClass="textbox" AutoPostBack="true" runat="server"
                    onkeypress="return isNumericSlash(event);" MaxLength="10" Height="18px" Width="37%"
                    placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtToDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txtToDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                Report Type
            </td>
            <td align="left" class="txt_style">
                <div class="textbox" style="background-color: White; height: 19px; width: 75%;">
                    <table border="0" width="95%">
                        <tr>
                            <td align="left">
                                <asp:RadioButton ID="rdoSingle" runat="server" AutoPostBack="true" OnCheckedChanged="rdoSingle_CheckedChanged" />
                            </td>
                            <td>
                                <asp:Label ID="Label3" runat="server" class="label" Text=" Single "></asp:Label>
                            </td>
                            <td align="left">
                                <asp:RadioButton ID="rdoAll" runat="server" AutoPostBack="true" OnCheckedChanged="rdoAll_CheckedChanged" />
                            </td>
                            <td>
                                <asp:Label ID="Label4" runat="server" class="label" Text=" All "></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label">
                Customer Name
            </td>
            <td colspan="3" class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCustomerName" runat="server" CssClass="textbox" Height="27px"
                            Width="85%" AutoPostBack="True" DataTextField="BankName" DataValueField="BankName"
                            OnSelectedIndexChanged="ddlCustomerName_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Criteria
            </td>
            <td align="left" class="txt_style">
                <div class="textbox" style="width: 75%; background-color: White; height: 19px;">
                    &nbsp;<asp:RadioButton ID="rdoLocation" runat="server" AutoPostBack="true" OnCheckedChanged="rdoLocation_CheckedChanged" />
                    <asp:Label ID="Label1" runat="server" class="label" Text=" Location  Wise"></asp:Label>
                    <asp:RadioButton ID="rdoBank" runat="server" AutoPostBack="true" OnCheckedChanged="rdoBank_CheckedChanged" />
                    <asp:Label ID="Label2" runat="server" class="label" Text=" Bank Wise"></asp:Label></div>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label">
                Location Type
            </td>
            <td class="txt_style" colspan="2">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlLocationType" runat="server" CssClass="textbox" Width="218px"
                            Height="28px">
                            <asp:ListItem>--Select Location Type--</asp:ListItem>
                            <asp:ListItem>All</asp:ListItem>
                            <asp:ListItem>Locker</asp:ListItem>
                            <asp:ListItem>OD</asp:ListItem>
                            <asp:ListItem>Office</asp:ListItem>
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                    </Triggers>
                </asp:UpdatePanel>
                <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlLocationType" ForeColor="Red" Display="Dynamic" Font-Bold="True"  ValidationGroup="save" 
                    Font-Size="Medium" InitialValue="--Select Location Type--" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Bank Name -->
            <td class="label">
                Bank Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlBankName" runat="server" CssClass="textbox" Height="27px"
                            Width="85%" AutoPostBack="True" DataTextField="BankName" DataValueField="BankName">
                        </asp:DropDownList>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*"
                            ControlToValidate="ddlBankName" ForeColor="Red" Display="Dynamic" Font-Bold="True"  ValidationGroup="save" 
                            Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Bank Name--">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Loan Status
            </td>
            <td class="txt_style" colspan="3">
                <asp:DropDownList ID="ddlAccountStatus" class="textbox" Height="27px" Width="39%"
                    runat="server">
                    <asp:ListItem>All</asp:ListItem>
                    <asp:ListItem>Open</asp:ListItem>
                    <asp:ListItem>Close</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 10%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td align="left">
                <div>
                    <asp:Button ID="btnDetails" runat="server" Text="Show Details" CssClass="button"
                        Style="width: 50%;" ValidationGroup="save" OnClick="btnDetails_Click" />&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" CssClass="button" />
                </div>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 15px;">
            </td>
        </tr>
        <%--  <tr>
            <td colspan="4">
                <div class="barstyle">
                </div>
            </td>
        </tr>--%>
        <tr>
            <td colspan="4" style="height: 10px">
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMsg" class="label" Style="color: Maroon; font-size: small;" runat="server"
                    Font-Names="Verdana"></asp:Label>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 15px;">
            </td>
        </tr>
    </table>
</asp:Content>
