<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ChangePassword.aspx.cs" Inherits="ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
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

        function isAlphaNumSplChars(e) { //Alphanumeric,64=@,95=underscore, 33=!,35=#,36=$,37=%,38=&,42=*,43=+,45=-,63=?,61=equalto only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 63 && k < 91) || (k > 96 && k < 123) || (k == 95 || k == 33 || k == 42 || k == 43 || k == 45 || k == 63 || k == 61) || (k > 34 && k < 39) || k == 0);
        }

    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Change User Password"></asp:Label>
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
                Current Password
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtCurrentPaasword" ReadOnly="true" class="textbox_readonly" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtUserName" Visible="false" runat="server" Width="15%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" align="right" valign="middle">
                New Password
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNewPassword" class="textbox" onkeypress="return isAlphaNumSplChars(event);"
                    runat="server" MaxLength="12" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqNewPassword" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="txtNewPassword" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revNewPassword" runat="server" ControlToValidate="txtNewPassword"
                    Font-Bold="True" Display="Dynamic" ValidationGroup="save" ForeColor="red" ErrorMessage="Password must be 6 characters long."
                    ValidationExpression="^.{6,12}$"></asp:RegularExpressionValidator>
                <%--<asp:RegularExpressionValidator ID="revZone" runat="server" ControlToValidate="txtNewPassword" Font-Bold="True"
                    ForeColor="Red" Display="Dynamic" ErrorMessage="Invalid Character (Only Alphanumeric and dot is allowed)."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z0-9.]+(_[a-zA-Z0-9.]+)*)(\s([a-zA-Z]+(_[a-zA-Z0-9.]+)*))*$"></asp:RegularExpressionValidator>--%>
                <asp:RegularExpressionValidator ID="revZone" runat="server" ControlToValidate="txtNewPassword"
                    Font-Bold="True" ForeColor="Red" Display="Dynamic" ErrorMessage="Invalid Character (Only Alphanumeric and dot is allowed)."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z0-9@_!#$%&*+-?=]+(_[a-zA-Z0-9@_!#$%&*+-?=]+)*)(\s([a-zA-Z]+(_[a-zA-Z0-9@_!#$%&*+-?=]+)*))*$"></asp:RegularExpressionValidator>
                <asp:Label ID="lblMsg" ForeColor="red" runat="server" Font-Bold="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label" align="right" valign="middle">
                Confirm Password
            </td>
            <td class="txt_style" align="left" valign="middle" colspan="3">
                <asp:TextBox ID="txtConfirmPassword" runat="server" class="textbox " MaxLength="12"
                    onkeypress="return isAlphaNumSplChars(event);" TextMode="Password"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVConfirmPassword" runat="server" ErrorMessage="*"
                    ControlToValidate="txtConfirmPassword" ValidationGroup="save" ForeColor="Red"
                    Display="Dynamic" SetFocusOnError="True" Font-Bold="True" Font-Size="Medium">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvConfirmPassword" runat="server" Display="Dynamic" ErrorMessage="New and Confirm Password does not match."
                    ForeColor="Red" ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmPassword"
                    Font-Bold="True" ValidationGroup="save"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td class="txt_style">
            </td>
            <td colspan="3">
                <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="button"
                    ValidationGroup="save" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" OnClick="btnReset_Click" CssClass="button" />
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
</asp:Content>
