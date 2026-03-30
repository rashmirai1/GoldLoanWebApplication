<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    Theme="GridViewTheme" CodeFile="EmployeeMaster.aspx.cs" Inherits="EmployeeMaster"
    EnableViewStateMac="false" %>

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

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k == 43 || k == 0);
        }
        //Function for alphabets without space
        function lettersOnly(evt) {
            evt = (evt) ? evt : event;
            var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode : ((evt.which) ? evt.which : 0));
            if (charCode > 31 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
                return false;
            }
            else
                return true;
        };

        //Function For Alphabets and Single code
        function SingleCode(nkey) {
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
            if (keyval >= 97 && keyval <= 122) {
                return true;
            }
            //For Backspace
            if (keyval == 8) {
                return true;
            }
            //For General
            if (keyval == 0) {
                return true;
            }

            //For Single code'
            if (keyval == 39) {
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

        //Function to Check Multiline Textbox Maxlength (Here Address Textbox)
        function Check(textBox, maxLength) {
            if (textBox.value.length > maxLength) {
                //alert("You cannot enter more than " + maxLength + " characters.");
                textBox.value = textBox.value.substr(0, maxLength);
            }
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }

    </script>
    <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
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
                    <!--Header -->
                    <tr>
                        <td align="center" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Employee Master"></asp:Label>
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
        <!--Form Design -->
        <tr>
            <td class="label">
                <asp:Label ID="lblEmpCode" runat="server" Text="Employee Code"></asp:Label>
            </td>
            <td colspan="3" class="txt_style">
                <asp:TextBox ID="txtEmpCode" Style="width: 180px;" ReadOnly="true" class="textbox"
                    runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" valign="middle">
                <asp:Label ID="Label2" runat="server" Text="Employee Name" Style="width: 100px"></asp:Label>
            </td>
            <td class="txt_style" valign="middle" colspan="3">
                <asp:TextBox ID="txtEmpFName" onkeypress="return  SingleCode(event);" class="textbox"
                    Style="width: 180px;" MaxLength="30" runat="server" ForeColor="Black" placeholder="First Name"></asp:TextBox>
                <asp:Label ID="lblFirstDash" Style="margin-left: 1px; margin-right: 1px;" runat="server"
                    Text="-"></asp:Label>
                <asp:TextBox ID="txtEmpMName" onkeypress="return SingleCode(event);" class="textbox"
                    Style="width: 180px;" MaxLength="30" runat="server" placeholder="Middle Name"></asp:TextBox>
                <asp:Label ID="lblsecDash" Style="margin-left: 1px; margin-right: 1px;" runat="server"
                    Text="-"></asp:Label>
                <asp:TextBox ID="txtEmpLName" onkeypress="return SingleCode(event);" class="textbox"
                    Style="width: 180px;" MaxLength="30" runat="server" placeholder="Last Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvFName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtEmpFName"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfvMName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtEmpMName"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfvEmpLName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtEmpLName"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFName" runat="server" ControlToValidate="txtEmpFName"
                    ForeColor="Red" Display="Dynamic" ErrorMessage="*" SetFocusOnError="True" ValidationGroup="Save"
                    ValidationExpression="^([a-zA-Z']+(_[a-zA-Z']+)*)(\s([a-zA-Z']+(_[a-zA-Z']+)*))*$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="revMName" runat="server" ControlToValidate="txtEmpMName"
                    ForeColor="Red" Display="Dynamic" ErrorMessage="*" SetFocusOnError="True" ValidationGroup="Save"
                    ValidationExpression="^([a-zA-Z']+(_[a-zA-Z']+)*)(\s([a-zA-Z']+(_[a-zA-Z']+)*))*$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="revLName" runat="server" ControlToValidate="txtEmpLName"
                    ForeColor="Red" Display="Dynamic" ErrorMessage="*" SetFocusOnError="True" ValidationGroup="Save"
                    ValidationExpression="^([a-zA-Z']+(_[a-zA-Z']+)*)(\s([a-zA-Z']+(_[a-zA-Z']+)*))*$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%" valign="top">
                <asp:Label ID="Label3" runat="server" Text="Address" Style="width: 100px"></asp:Label>
            </td>
            <td style="height: 70px;" colspan="3">
                <asp:TextBox ID="txtAddress" class="textbox" Style="width: 580px; height: 50px;"
                    TextMode="MultiLine" onkeyup="javascript:Check(this, 200);" onchange="javascript:Check(this, 200);"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAddress" ForeColor="Red" runat="server" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                <asp:Label ID="Label4" runat="server" Text="Mobile No" Style="width: 100px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtMobileNo" MaxLength="10" onkeypress="return OnlyNumericEntry();"
                    Style="width: 180px" class="textbox" runat="server"> </asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVMobileNo" runat="server" ErrorMessage="*" ControlToValidate="txtMobileNo"
                    ValidationGroup="Save" ForeColor="Red" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="REVMobileNo" runat="server" ControlToValidate="txtMobileNo"
                    ValidationGroup="Save" ErrorMessage="Invalid Mobile number!" ForeColor="Red"
                    SetFocusOnError="True" ValidationExpression="^([7-9]{1})([0-9]{9})$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                <asp:Label ID="Label1" runat="server" Text="Telephone No"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtCode" class="textbox" onkeypress="return OnlyNumericEntry();"
                    Style="width: 35px" MaxLength="5" runat="server"></asp:TextBox>
                <asp:Label ID="Label6" Style="margin-left: 1px; margin-right: 1px;" runat="server"
                    Text="-"></asp:Label>
                <asp:TextBox ID="txtTelephoneNo" class="textbox" onkeypress="return OnlyNumericEntry();"
                    Style="width: 125px" MaxLength="8" runat="server"></asp:TextBox>
                <%--<asp:RequiredFieldValidator ID="rfvTelphoneNo" runat="server" ForeColor="Red" ErrorMessage="*"
                    ValidationGroup="Save" ControlToValidate="txtTelephoneNo"></asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="rfvCode" runat="server" ErrorMessage="*" ControlToValidate="txtCode"
                    ValidationGroup="Save" ForeColor="Red" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                <asp:RegularExpressionValidator ID="revTelNo" runat="server" ControlToValidate="txtTelephoneNo"
                    ValidationGroup="Save" ErrorMessage="Invalid Telephone number!" ForeColor="Red"
                    SetFocusOnError="True" ValidationExpression="^[0-9]\d{5,7}$"></asp:RegularExpressionValidator>
                &nbsp;
                <asp:RegularExpressionValidator ID="rfvStdCode" runat="server" ControlToValidate="txtCode"
                    Display="Dynamic" ValidationGroup="Save" ErrorMessage="Invalid StdCode !" ForeColor="Red"
                    SetFocusOnError="True" ValidationExpression="^[0-9]\d{1,4}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%">
                <asp:Label ID="Label5" runat="server" Text="Email ID" Style="width: 100px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtEmailId" class="textbox" Style="height: 75%; width: 580px;" MaxLength="100"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmailId" runat="server" ForeColor="Red" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtEmailId"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator runat="server" ID="REVEmailID" SetFocusOnError="true"
                    ValidationGroup="Save" Text="Example: username@gmail.com" ControlToValidate="txtEmailId"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"
                    ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <td class="txt_style">
                <asp:TextBox ID="txtEmpID" Visible="false" runat="server" Height="16px" Width="27px"></asp:TextBox>
            </td>
            <!--Save/Reset Button -->
            <td style="width: 20%" colspan="3">
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="button" ValidationGroup="Save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="RESET" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4">
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
            <td align="center" colspan="4">
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
                                <asp:DropDownList ID="ddlSearchBy" runat="server" class="textbox_search" 
                                    Height="26px" Width="180px">
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                            </td>
                            <td class="label">
                                Search Text:
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" 
                                    class="textbox_search" onblur="getfocus()"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
                            </td>
                        </tr>
                    </table>
                    <!--Dotted Bar -->
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td colspan="6">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle" colspan="6">
                                <div class="dotted_bar">
                                </div>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="EmployeeID" AutoPostBack="true"
                                OnRowCommand="dgvDetails_RowCommand" OnPageIndexChanging="dgvDetails_PageIndexChanging"
                                HorizontalAlign="Center" Width="100%">
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
                                    <asp:BoundField DataField="EmployeeID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="EmpFirstName" HeaderText="EmpFirstName" Visible="False" />
                                    <asp:BoundField DataField="EmpMiddleName" HeaderText="EmpMiddleName" Visible="False" />
                                    <asp:BoundField DataField="EmpLastName" HeaderText="EmpLastName" Visible="False" />
                                    <asp:BoundField DataField="EmpCode" HeaderText="Employee Code" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="EmployeeName" HeaderText="Employee Name" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="Address" HeaderText="Address" Visible="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="MobileNo" HeaderText=" Mobile No" Visible="true" ItemStyle-HorizontalAlign="Center" />
                                    <%--  <asp:BoundField DataField="StdCode" HeaderText=" Std Code" Visible="true" />--%>
                                    <asp:BoundField DataField="TelephoneNo" HeaderText=" Telephone No" Visible="true"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="EmailId" HeaderText="Email ID" Visible="true" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
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
