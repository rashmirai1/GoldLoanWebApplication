<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="AreaMaster.aspx.cs" Inherits="AreaMaster" EnableViewStateMac="false"
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
            else {
                return false;
            }
        }
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }

        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode
            if (charCode > 31 && (charCode < 48 || charCode > 57))
                return false;

            return true;
        }
        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0);
        }

        function Validate() {
            var txtArea = document.getElementById('<%=txtArea.ClientID %>');
            var ddlCity = document.getElementById('<%=ddlCity.ClientID %>');
            var txtPinCode = document.getElementById('<%=txtPinCode.ClientID %>');
            if (txtArea.value == '') {
                alert('Enter Area');
                return false;
            }
            if (ddlCity.selectedIndex == 0) {
                alert('Select City');
                return false;
            }
            if (txtPinCode.value == "") {
                alert('Enter Pin Code');
                return false;
            }
            if (parseFloat(txtPinCode.value) == 0) {
                alert('Pin Code Should Not Be Zero');
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
                            <asp:Label ID="lblHeader" runat="server" Text="Area Master"></asp:Label>
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
                Area<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtArea" onkeypress="return Alphabets(event);" class="textbox" MaxLength="40"
                    runat="server"></asp:TextBox>
                <asp:TextBox ID="txtAreaID" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label">
                City<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlCity" class="textbox" runat="server" Height="27px" Width="210px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="label">
                Pincode<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPinCode" class="textbox" onkeypress="return isNumberKey(event)"
                    MaxLength="6" runat="server"></asp:TextBox>
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
        <tr>
            <td colspan="2">
            </td>
        </tr>
    </table>
</asp:Content>
