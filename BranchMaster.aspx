<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="BranchMaster.aspx.cs" Inherits="BranchMaster" Theme="GridViewTheme"
    EnableViewStateMac="false" %>

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

        function AlphabetsInCaps(nkey) {
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

        function isNumericHyphen(e) {  // Alphanumeric, space, - only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || k == 45);
        }

        function isAlphaNumeric(e) { // Alphanumeric only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123));
        }

        function isAlphaNumChars(e) { // Alphanumeric, space,(,),- only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41) || k == 0);
        }

        //Function to Check Multiline Textbox Maxlength (Here Address Textbox)
        function Check(textBox, maxLength) {
            if (textBox.value.length > maxLength) {
                //alert("You cannot enter more than " + maxLength + " characters.");
                textBox.value = textBox.value.substr(0, maxLength);
            }
        }

        //        function gridviewScroll() {
        //            $('#<%=dgvDetails.ClientID%>').gridviewScroll({
        //                width: windowSize - 1000,
        //                height: 500,
        //                freezesize: 3
        //            });
        //        } 

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }
    </script>
<%--    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>--%>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
    <tr>
            <td style="width: 15%;">
            </td>
            <td style="width: 55%;">
            </td>
            <td style="width: 12%;">
            </td>
            <td style="width: 23%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Create Branch"></asp:Label>
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
            <!-- Branch Name -->
            <td class="label">
                Branch Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtBranchName" onkeypress="return isAlphaNumChars(event);" class="textbox"
                    MaxLength="40" runat="server" Style="text-transform: uppercase; width: 68%;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqBranchName" runat="server" ErrorMessage="*" ControlToValidate="txtBranchName"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
            <!-- Status-- Active/Inactive-->
            <%--<td class="label">
                &nbsp;
            </td>
            <td >
                <asp:CheckBox ID="chkActive" runat="server" Text="Active" style="font-family: Calibri; font-size: 14px;" TextAlign="Left"/>
                <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="*" ClientValidationFunction="ValidateCheckBox"></asp:CustomValidator>
            </td>--%>
        </tr>
        <!-- Regex validation for Branch Name -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RegBranch" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtBranchName" ValidationExpression="^([a-zA-Z0-9 -()]+(_[a-zA-Z0-9 -()]+)*)(\s([a-zA-Z0-9 -()]+(_[a-zA-Z0-9 -()]+)*))*$"
                    ErrorMessage="Invalid Character (Only alphanumeric, dash, brackets allowed)."
                    Display="Dynamic" ValidationGroup="save" ForeColor="Red">
                </asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="txt_style">
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="20px" Width="25px"></asp:TextBox>
                <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="16px" Width="38px"></asp:TextBox>
            </td>
            <!--Save/Reset Button -->
            <td colspan="3">
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
        <tr>
            <td colspan="4">
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="left" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" runat="server" class="textbox_search" Height="26px"
                                    Width="180px">
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                            </td>
                            <td class="label">
                                Search Text:
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" class="textbox_search"
                                    onblur="getfocus()"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
                            </td>
                        </tr>
                    </table>
                    <!--Dotted Bar -->
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td colspan="4">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle" colspan="4">
                                <div class="dotted_bar">
                                </div>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <!--GridView -->
                    <%--style="width:725px; overflow-x:scroll;"--%>
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="BID" 
                                AutoPostBack="true" Width="75%"
                                OnRowCommand="dgvDetails_RowCommand" 
                                OnPageIndexChanging="dgvDetails_PageIndexChanging" AutoGenerateColumns="False">
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
                                    <asp:BoundField DataField="BID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="BranchName" HeaderText="Branch Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"/>
                                    <asp:BoundField DataField="CompID" HeaderText="CompID" Visible="False" />
                                    <asp:TemplateField HeaderText="Edit" Visible="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
