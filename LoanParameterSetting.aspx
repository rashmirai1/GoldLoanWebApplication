<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="LoanParameterSetting.aspx.cs" Inherits="LoanParameterSetting" Theme="GridViewTheme"
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
        function isAlphaNumCharsDot(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
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

        //Function for only numbers
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 30%;">
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 20%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Loan Parameter settings"></asp:Label>
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
        <!-- Gold Price per Gram -->
        <tr>
            <td class="label">
                Gold Price per Gram
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtCostPerGram" runat="server" class="textbox" MaxLength="6" onkeypress="return isAlphaNumCharsDot(event);"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" Text="*"
                    ErrorMessage="*" ControlToValidate="txtCostPerGram" ValidationGroup="save" ForeColor="Red"
                    Display="Dynamic" Font-Bold="True" Font-Size="Small" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label9" runat="server" class="label" Text="(Rs.)"></asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator6" runat="server" ControlToValidate="txtCostPerGram"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage=" Price must be greater than zero"
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <!-- Pending Loan Reminder -->
        <tr>
            <td class="label" align="right" valign="middle">
                Pending Loan Reminder
            </td>
            <td class="txt_style" align="left" valign="middle" colspan="3">
                <asp:TextBox ID="txtReminder" runat="server" class="textbox " MaxLength="3" onkeypress="return OnlyNumericEntry();"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVConfirmPassword" runat="server" ErrorMessage="*"
                    ControlToValidate="txtReminder" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    SetFocusOnError="True" Font-Bold="True" Font-Size="Small">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label2" runat="server" class="label" Text="(Days)"></asp:Label>
                <asp:RegularExpressionValidator ID="rfvconfirm" Font-Size="Small" runat="server"
                    ControlToValidate="txtReminder" ForeColor="Red" Display="Dynamic" ErrorMessage="Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <!-- Interest Payable For Prepayment -->
        <tr>
            <td class="label" align="right" valign="middle" style="width: 200px">
                Interest Payable For Prepayment
            </td>
            <td class="txt_style" align="left" valign="middle" colspan="3">
                <asp:TextBox ID="txtInterest" runat="server" class="textbox " MaxLength="3" onkeypress="return OnlyNumericEntry();"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtInterest" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    SetFocusOnError="True" Font-Bold="True" Font-Size="Small">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label4" runat="server" class="label" Text="(Days)"></asp:Label>
               <%-- <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtInterest"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>--%>
            </td>
        </tr>
        <!-- Processing Fee -->
        <tr>
            <td class="label" align="right" valign="middle" style="width: 200px">
                Processing Fee
            </td>
            <td class="txt_style" align="left" valign="middle" colspan="3">
                <asp:TextBox ID="txtProcessingFee" runat="server" class="textbox " MaxLength="7"
                    onkeypress="return isAlphaNumCharsDot(event);"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="txtProcessingFee" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    SetFocusOnError="True" Font-Bold="True" Font-Size="Small">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label1" runat="server" class="label" Text="(Rs.)"></asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtProcessingFee"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Fees must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <!-- Processing Indemnity Fee -->
        <tr>
            <td class="label" align="right" valign="middle" style="width: 200px">
                Processing Indemnity Fee
            </td>
            <td class="txt_style" align="left" valign="middle" colspan="3">
                <asp:TextBox ID="txtIndemnity" runat="server" class="textbox " MaxLength="7" onkeypress="return isAlphaNumCharsDot(event);"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                    ControlToValidate="txtIndemnity" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    SetFocusOnError="True" Font-Bold="True" Font-Size="Small">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label5" runat="server" class="label" Text="(Rs.)"></asp:Label>
                <asp:RegularExpressionValidator ID="RevIndemnity" runat="server" ControlToValidate="txtIndemnity"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Fees must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <!-- Default Deduction In Gross Weight -->
        <tr>
            <td class="label" align="right" valign="middle" style="width: 200px">
                Default Deduction In Gross Weight
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtDeduction" runat="server" class="textbox " MaxLength="6" onkeypress="return isAlphaNumCharsDot(event);"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RfvDeduction" runat="server" ErrorMessage="*" ControlToValidate="txtDeduction"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" SetFocusOnError="True"
                    Font-Bold="True" Font-Size="Small">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label3" runat="server" class="label" Text="(milligram)"></asp:Label>
                <asp:Label ID="lblExample" runat="server" Text="[e.g. 0.001]" class="label" Font-Size="12px"
                    Font-Italic="True"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RevDeduction" runat="server" ControlToValidate="txtDeduction"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Invalid Deduction Value. Enter value in decimal [e.g. 0.001]"
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^[0-9]\d{0,1}(\.[0-9]\d{0,2})"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="txt_style">
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtLPID" runat="server" Visible="false"></asp:TextBox>
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
            <td colspan="4" style="height: 8px">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="div1" runat="server">
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="ID" AutoPostBack="true"
                                OnPageIndexChanging="dgvDetails_PageIndexChanging" OnRowCommand="dgvDetails_RowCommand"
                                AutoGenerateColumns="False">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        ControlStyle-Font-Size="XX-Small" FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="LoanID" HeaderText="ID" Visible="False" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="GoldPricePerGram" HeaderText="Gold Price(per gm)" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="PendingLoanReminderDays" HeaderText="Reminder (Days)"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="InterestPayableForPrepaymentDays" HeaderText="Interest Prepayment (Days)"
                                        ControlStyle-Width="8px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ProcessingFee" HeaderText="Processing Fee" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ProcessingIndemnity" HeaderText="Indemnity Fee" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DeductionInGrossWeight" HeaderText="Gross Weight Deduction"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
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
                            <%-- <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />--%>
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
