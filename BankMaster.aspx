<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="BankMaster.aspx.cs" Inherits="BankMaster"
    EnableViewStateMac="false" %>

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

        function validrecord() {
            var txtBankName = document.getElementById('<%=txtBankName.ClientID %>');
            var txtAlias = document.getElementById('<%=txtAlias.ClientID %>');
            var txtAddress = document.getElementById('<%=txtAddress.ClientID %>');
            var txtBranch = document.getElementById('<%=txtBranch.ClientID %>');


            if (txtBankName.value == '') {
                alert('Enter Bank Name');
                return false;
            }
            if (txtAlias.value == '') {
                alert('Enter Alias Name');
                return false;
            }
            if (txtAddress.value == '') {
                alert('Enter Address');
                return false;
            }
            if (txtBranch.value == '') {
                alert('Enter Branch');
                return false;
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

        function isNumericHyphen(e) { // Alphanumeric, space, - only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || k == 45);
        }


        function isAlphaNumeric(e) { // Alphanumeric, space,(,),_ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 40 || k == 41) || k == 0);
        }
        //Function to Check Multiline Textbox Maxlength (Here Address Textbox)
        function Check(textBox, maxLength) {
            if (textBox.value.length > maxLength) {
                //alert("You cannot enter more than " + maxLength + " characters.");
                textBox.value = textBox.value.substr(0, maxLength);
            }
        }

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0);
        }

        function ValidateMobNumber() {
            var txtContactNo1 = document.getElementById('<%=txtContactNo1.ClientID %>');

            var matchmobileno = "^([7-9]{1})([0-9]{9})$";

            if (isNaN(txtContactNo1.value)) {
                alert("The phone number contains illegal characters.");
                txtContactNo1.value = "";
                txtContactNo1.focus();
                return false;
            }
            if (txtContactNo1.value.length != 10) {
                alert("Please enter 10 digit mobile no.");
                txtContactNo1.value = "";
                txtContactNo1.focus();
                return false;
            }


        }

        function ValidatePhone() {
            var txtContactNo2 = document.getElementById('<%=txtContactNo2.ClientID %>');
            if (txtContactNo2.value.length < 6) {
                alert("Enter atleast 6 digit phone no");
                txtContactNo2.value = "";
                txtContactNo2.focus();
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
                            <asp:Label ID="lblHeader" runat="server" Text="Bank Master"></asp:Label>
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
            <!-- Bank Name -->
            <td class="label">
                Bank Name<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtBankName" runat="server" class="textbox" Style="width: 95%;"
                    MaxLength="50" onkeypress="return isAlphaNumeric(event);"></asp:TextBox>
                <asp:TextBox ID="txtBankID" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label">
                Bank Alias<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtAlias" runat="server" MaxLength="8" class="textbox" Style="width: 32.5%;"
                    onkeypress="return isAlphaNumeric(event);"></asp:TextBox>
                <asp:Label ID="lblAlias" runat="server" Text="(e.g:For State Bank Of India-SBI) will be referenced in Bank Gold Details"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- Address -->
            <td class="label" valign="top">
                Address<b style="color: Red;">*</b>
            </td>
            <td style="height: 70px;">
                <asp:TextBox ID="txtAddress" class="textbox" Style="width: 95%; height: 50px;" TextMode="MultiLine"
                    onkeyup="javascript:Check(this, 200);" onchange="javascript:Check(this, 200);"
                    runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Branch -->
            <td class="label">
                Branch<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtBranch" onkeypress="return isAlphaNumeric(event);" class="textbox"
                    Style="width: 95%;" MaxLength="40" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Contact Person -->
            <td class="label">
                Contact Person
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtContact" onkeypress="return Alphabets(event);" class="textbox"
                    MaxLength="40" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Contact No1. -->
            <td class="label">
                Contact No1.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtContactNo1" onkeypress="return  OnlyNumericEntry();" onchange="return ValidateMobNumber();"
                    class="textbox" MaxLength="10" runat="server"></asp:TextBox>
                <asp:Label ID="lblContact1" runat="server" Text="(Mobile No. If any)" class="label"
                    Font-Size="12px" Font-Italic="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- Contact No2. -->
            <td class="label">
                Contact No2.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtContactNo2" onkeypress="return isNumericHyphen(event);" onchange="return ValidatePhone();"
                    class="textbox" MaxLength="10" runat="server"></asp:TextBox>
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
            <td colspan="4">
                <br />
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
