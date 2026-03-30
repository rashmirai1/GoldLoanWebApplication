<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="InterestSlabMaster.aspx.cs" Inherits="InterestSlabMaster" Theme="GridViewTheme"
    EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        //Confirm Alert
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

        function isAlphaNumChars(e) { // Alphanumeric,space,(),@,%,*,_,+,-,[]  only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 46 && k < 58) || (k > 63 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41 || k == 42 || k == 43 || k == 37 || k == 38 || k == 39 || k == 91 || k == 93 || k == 95) || k == 0);
        }

        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }

        function OnlyNumericEntry() { //Function for only numbers
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 40%;">
            </td>
            <td style="width: 17%;">
            </td>
            <td style="width: 23%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Interest Slab Master"></asp:Label>
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
            <!-- Scheme Name -->
            <td class="label">
                Scheme Name
            </td>
            <td class="txt_style" colspan="2">
                <asp:TextBox ID="txtSchemeName" onkeypress="return isAlphaNumChars(event);" class="textbox"
                    MaxLength="50" runat="server" Width="95%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqSchemeName" runat="server" ErrorMessage="*" ControlToValidate="txtSchemeName"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Loan Amount -->
            <td class="label">
                Loan Amount
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLTV" CssClass="textbox" runat="server" Width="75%" MaxLength="10"
                    Style="text-align: right" onkeypress="return OnlyNumericEntry();" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                    ControlToValidate="txtLTV" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label6" runat="server" class="label" Text="(Rs)"></asp:Label>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Validator (Loan Amount) -->
            <td>
            </td>
            <td colspan="3">
            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtLTV"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Loan Amount Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>
                <%--<asp:RangeValidator ID="RangeValidator1" runat="server" ErrorMessage="LTV must be between 1 to 100."
                    ControlToValidate="txtLTV" Display="Dynamic" ValidationGroup="save" ForeColor="red"
                    Font-Size="Small" MinimumValue="1" MaximumValue="100" SetFocusOnError="True"
                    Type="Double"></asp:RangeValidator>--%>
            </td>
        </tr>
        <%-- <tr>
            <!-- Maximum Loan Tenure -->
            <td class="label">
                Maximum Loan Tenure
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMaximumLoanTenure" CssClass="textbox" runat="server" Width="75%"
                    MaxLength="3" onkeypress="return OnlyNumericEntry();" ClientIDMode="Static"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ErrorMessage="*"
                    ControlToValidate="txtMaximumLoanTenure" ValidationGroup="save" ForeColor="Red"
                    Display="Dynamic" Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label2" runat="server" class="label" Text="(Days)"></asp:Label>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtMaximumLoanTenure"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>--%>
        <tr>
            <!-- Loan Tenure (min-max) -->
            <td class="label">
                Loan Tenure
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMinimumTenure" CssClass="textbox" runat="server" Width="31.5%"
                    placeholder="minimum" MaxLength="3" onkeypress="return OnlyNumericEntry();" ClientIDMode="Static"
                    Style="text-align: right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtMinimumTenure" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                &nbsp;&nbsp;<asp:Label ID="Label1" runat="server" class="label" Text="-"></asp:Label>
                <asp:TextBox ID="txtMaximumTenure" CssClass="textbox" runat="server" Width="31.5%"
                    placeholder="maximum" MaxLength="3" onkeypress="return OnlyNumericEntry();" ClientIDMode="Static"
                    Style="text-align: right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="txtMaximumTenure" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label3" runat="server" class="label" Text="(Days)"></asp:Label>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Validator (Loan Tenure (min-max)) -->
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtMinimumTenure"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Min Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtMaximumTenure"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Max Days must be greater than zero."
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([1-9]\d*)?$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <!-- Interest Rate -->
            <td class="label">
                Interest Rate
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtInterestRate" CssClass="textbox" runat="server" Width="75%" MaxLength="5"
                    onkeypress="return isNumeric(event);" ClientIDMode="Static" Style="text-align: right"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                    ControlToValidate="txtInterestRate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:Label ID="Label4" runat="server" class="label" Text="(%)"></asp:Label>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Validator (Interest Rate) -->
            <td>
            </td>
            <td colspan="3">
                <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="LTV must be between 1 to 100."
                    ControlToValidate="txtLTV" Display="Dynamic" ValidationGroup="save" ForeColor="red"
                    Font-Size="Small" MinimumValue="1" MaximumValue="100" SetFocusOnError="True"
                    Type="Double"></asp:RangeValidator>
            </td>
        </tr>
        <tr>
            <!--Save/Reset Button -->
            <td class="txt_style">
            </td>
            <td>
                <%--OnClick="btnSave_Click" OnClick="btnReset_Click" --%>
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="button" ValidationGroup="save" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" />
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
                            <%--OnPageIndexChanging="dgvDetails_PageIndexChanging" OnRowCommand="dgvDetails_RowCommand"--%>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="LoanID" AutoPostBack="true"
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
                                    <asp:BoundField DataField="LoanSanctionPercent" HeaderText="Sanction (%)" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="MaxSanctionLoanAmt" HeaderText="Sanction Amount" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="SanctionLoanAmtPerGram" HeaderText="Sanction Gram" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="GoldPricePerGram" HeaderText="Gold Price(per gm)" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="PendingLoanReminderDays" HeaderText="Reminder (Days)"
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="InterestPayableForPrepaymentDays" HeaderText="Interest Prepayment (Days)"
                                        ControlStyle-Width="8px" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ProcessingFee" HeaderText="Processing Fee" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ProcessingIndemnity" HeaderText="Indemnity Fee" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="DeductionInGrossWeight" HeaderText="Gross Weight Deduction "
                                        ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="left">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="left">
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
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
