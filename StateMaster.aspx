<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="StateMaster.aspx.cs" Inherits="StateMaster" EnableViewStateMac="false"
    EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <script type="text/javascript">
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

        function Alphabets(nkey) {
            var keyval
            if (navigator.appName == "Microsoft Internet Explorer") {
                keyval = window.event.keyCode;
            }
            else if (navigator.appName == 'Netscape') {
                nkeycode = nkey.which;
                keyval = nkeycode;
            }
            if (keyval >= 65 && keyval <= 90) {
                return true;
            }
            //For a-z
            else if (keyval >= 97 && keyval <= 122) {
                return true;
            }
            //For Backspace
            else if (keyval == 8) {
                return true;
            }
            //For General
            else if (keyval == 0) {
                return true;
            }
            //For Space
            else if (keyval == 32) {
                return true;
            }
            //() brackets
            //              else if (keyval == 40 || keyval == 41) {
            //                  return true;
            //              }
            else {
                return false;
            }
        }

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0);

        }
        function Validate() {
            var txtStateName = document.getElementById('<%=txtStateName.ClientID %>');
            var ddlCountry = document.getElementById('<%=ddlCountry.ClientID %>');
            if (txtStateName.value == '') {
                alert('Enter State Name');
                return false;
            }
            if (ddlCountry.selectedIndex == 0) {
                alert('Select Country');
                return false;
            }

        }


    </script>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnPopup" runat="server" Value="Edit" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td colspan="2">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="2" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="State Master"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Form Design -->
        <tr>
            <td class="label">
                State<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtStateName" onkeypress="return Alphabets(event);" class="textbox"
                    MaxLength="40" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqStateName" runat="server" ErrorMessage="*" ControlToValidate="txtStateName"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="RegState" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtStateName" ValidationExpression="^[A-Za-z][A-Za-z ]*$"
                    ErrorMessage="Invalid Character (Only alphabets)." Display="Dynamic" ValidationGroup="save"
                    ForeColor="Red">
                </asp:RegularExpressionValidator>
                <asp:TextBox ID="txtStateID" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label">
                Country<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlCountry" class="textbox" runat="server" Height="27px" Width="210px">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="reqCountry" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="ddlCountry" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Country--">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
