<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="CityMaster.aspx.cs" Inherits="CityMaster" EnableViewStateMac="false"
    EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
 
    <script type="text/javascript">
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
        function Validate() {
            var txtCity = document.getElementById('<%=txtCity.ClientID %>');
            var ddlState = document.getElementById('<%=ddlState.ClientID %>');
            var txtStdCode = document.getElementById('<%=txtStdCode.ClientID %>');
            if (txtCity.value == '') {
                alert('Enter City');
                return false;
            }
            if (ddlState.selectedIndex == 0) {
                alert('Select State');
                return false;
            }
            if (txtStdCode.value == "") {
                alert('Enter STD Code');
                return false;
            }
            if (txtStdCode.value == 0) {
                alert('Std Code Should Be More Than Zero');
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
                            <asp:Label ID="lblHeader" runat="server" Text="City Master"></asp:Label>
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
                City<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtCity" onkeypress="return Alphabets(event);" class="textbox" MaxLength="40"
                    runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="revZone" runat="server" ControlToValidate="txtCity"
                    ForeColor="Red" Display="Dynamic" ErrorMessage="Invalid Character (Only alphabets)."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z]+(_[a-zA-Z]+)*)(\s([a-zA-Z]+(_[a-zA-Z]+)*))*$"></asp:RegularExpressionValidator>
                <asp:TextBox ID="txtCityID" runat="server" Visible="false"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label">
                State<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlState" class="textbox" runat="server" Height="25px" Width="210px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="label">
                StdCode <b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtStdCode" onkeypress="return OnlyNumericEntry();" class="textbox"
                    MaxLength="5" runat="server"></asp:TextBox>
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
