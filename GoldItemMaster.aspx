<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="GoldItemMaster.aspx.cs" Inherits="GoldItemMaster" %>

<%@ MasterType VirtualPath="~/Site.master" %>
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
        function isAlphaNumeric(e) { // Alphanumeric, space,(,),_ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 40 || k == 41) || k == 0);
        }

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0);
        }
        function validdata() {
            var txtItemName = document.getElementById('<%=txtItemName.ClientID %>');
            if (txtItemName.value == '') {
                alert('Enter Item Name');
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
                            <asp:Label ID="lblHeader" runat="server" Text="Gold Item Master"></asp:Label>
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
            <td>
                <%--<asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
                    <ContentTemplate>--%>
                <table>
                    <tr>
                        <td class="label">
                            Item Name<b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtItemName" class="textbox" runat="server" MaxLength="40" Width="100%"
                                onkeypress="return isAlphaNumeric(event);"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="reqItem" runat="server" ErrorMessage="*" ControlToValidate="txtItemName"
                                ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                                SetFocusOnError="True">*</asp:RequiredFieldValidator>
                            <asp:TextBox ID="txtItemID" runat="server" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <%-- </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnSave" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>--%>
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
                <div id="div1" runat="server">
                    <!--Search -->
                    <!--Dotted Bar -->
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle" colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <!--GridView -->
                </div>
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
