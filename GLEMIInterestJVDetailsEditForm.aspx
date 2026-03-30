<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLEMIInterestJVDetailsEditForm.aspx.cs" Inherits="GLEMIInterestJVDetailsEditForm"
    Theme="GridViewTheme" EnableViewStateMac="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtReferenceDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtPaymentDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtChequeDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });

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
        function enableDueDate() {
            $('#<%=txtPaymentDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        }

        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }

        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        //focus on Gold Loan No
        function GetFocus(controlid) {
            document.getElementById(controlid).focus();
        }

        //to check if balance payable is 0, payment type should be 'Pre-payment'
        function validateBalancePayable() {
            var balAmount = document.getElementById('<%=txtTotalBalancePayable.ClientID %>').value;
            var paymentType = document.getElementById('<%=ddlPaymentType.ClientID %>').value;
            if (balAmount == 0 && paymentType != 'Pre-payment') {
                alert("Payment Type should be 'Pre-payment' since Balance Payable Amount is zero.");
                document.getElementById('<%=ddlPaymentType.ClientID %>').focus();
            }
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 28%;">
            </td>
            <td style="width: 2%;">
            </td>
            <td style="width: 20%;">
            </td>
            <td style="width: 28%;">
            </td>
            <td style="width: 2%;">
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="5" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL EMI & Interest JV Details"></asp:Label>
                        </td>
                        <td align="right" colspan="1">
                            <asp:Label ID="Label27" runat="server" Text="[Edit Section]" Font-Names="Verdana"
                                Font-Size="11px" ForeColor="#070c80"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Form Design -->
        <tr>
            <!-- JV ReferenceNo -->
            <td align="right">
                <asp:Label ID="Label2" runat="server" Text="JV Reference No" CssClass="label" ForeColor="Red"></asp:Label>
            </td>
            <td class="txt_style" valign="middle">
                <asp:TextBox ID="txtJVreferenceNo" CssClass="textbox" runat="server" Width="90%"
                    AutoPostBack="true" MaxLength="20" OnTextChanged="txtJVreferenceNo_TextChanged"
                    Style="text-transform: uppercase;"></asp:TextBox>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Gold Loan No -->
            <td align="right">
                <asp:Label ID="Label1" runat="server" Text="Gold Loan No" CssClass="label" Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style" valign="middle">
                <asp:TextBox ID="txtGoldLoanNo" CssClass="textbox_readonly" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Operator Name -->
            <td class="label">
                Operator Name
            </td>
            <td class="txt_style" colspan="6">
                <asp:TextBox ID="txtOperatorName" class="textbox_readonly " ReadOnly="true" Width="94%"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvOperatorName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Display="Dynamic" ValidationGroup="save" ControlToValidate="txtOperatorName"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!-- Payment Date -->
            <td class="label">
                Payment Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtReferenceDate" CssClass="textbox" runat="server" Width="75%"
                    MaxLength="10" placeholder="dd/mm/yyyy" ClientIDMode="Static" OnTextChanged="txtReferenceDate_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqLoanDate" runat="server" ErrorMessage="*" ControlToValidate="txtReferenceDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtReferenceDate"
                    Display="Dynamic" ErrorMessage="*" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                    Type="Date" ValidationGroup="save" Font-Bold="True">*</asp:CompareValidator>
            </td>
            <!-- JV ReferenceNo -->
            <%--<td class="txt_style" valign="middle">
                <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <!-- Ref Type -->
                        <asp:TextBox ID="txtRefType" CssClass="textbox_readonly" runat="server" Text="KNCF"
                            Font-Bold="true" Width="15%"></asp:TextBox>
                        <!-- Ref Num -->
                        <asp:DropDownList ID="ddlRefNum" runat="server" CssClass="textbox" Height="27px"
                            ClientIDMode="Static" Width="39%" OnSelectedIndexChanged="ddlRefNum_SelectedIndexChanged">
                        </asp:DropDownList>
                        <!-- Ref ID -->
                        <asp:DropDownList ID="ddlRefID" runat="server" CssClass="textbox" Height="27px" Width="33%"
                            ClientIDMode="Static" OnSelectedIndexChanged="ddlRefID_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNum" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>--%>
            <td>
                &nbsp;
            </td>
            <!-- Loan Issued Date -->
            <td class="label">
                Loan Issued Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtLoanIssuedDate" CssClass="textbox_readonly" runat="server" Width="90%"
                            ClientIDMode="Static"></asp:TextBox>
                    </ContentTemplate>
                    <%-- <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />
                    </Triggers>--%>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <!-- Regex validation -->
        <tr>
            <td>
            </td>
            <td>
                <!-- Regex validation for Reference Date -->
                <asp:CompareValidator ID="CVReferenceDate" runat="server" ControlToValidate="txtReferenceDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="save"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="5">
                <!-- Custom Validator for Payment Date -->
                <asp:CustomValidator runat="server" ID="CustomValidator4" ControlToValidate="txtReferenceDate"
                    ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True" OnServerValidate="txtReferenceDate_ServerValidate"
                    ValidationGroup="save" ErrorMessage="Payment Date must be within Financial Year." />
            </td>
        </tr>
        <%-- <!-- Regex validation -->
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
                <!-- Regex validation for Reference No -->
                <asp:RequiredFieldValidator ID="ReqRefNum" runat="server" ErrorMessage="*" ControlToValidate="ddlRefNum"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="ReqRefID" runat="server" ErrorMessage="*" ControlToValidate="ddlRefID"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
            <td>
            </td>
        </tr>--%>
        <tr>
            <!-- Customer Name -->
            <td class="label">
                Customer Name
            </td>
            <td class="txt_style" colspan="5">
                <asp:UpdatePanel ID="UpdatePanel15" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtCustomerName" CssClass="textbox_readonly" runat="server" Width="94%"></asp:TextBox>
                    </ContentTemplate>
                    <%-- <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />
                    </Triggers>--%>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Total Loan Amount -->
            <td class="label">
                Total Loan Amount
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtTotalLoanAmount" CssClass="textbox_readonly" runat="server" Width="90%"
                            ClientIDMode="Static"></asp:TextBox>
                    </ContentTemplate>
                    <%-- <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />
                    </Triggers>--%>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Last EMI Paid Date -->
            <td class="label">
                Last Interest Paid Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtLastEMIPaidDate" CssClass="textbox_readonly" runat="server" Width="90%"
                            ClientIDMode="Static"></asp:TextBox>
                    </ContentTemplate>
                    <%-- <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />
                    </Triggers>--%>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Rate of Interest -->
            <td class="label">
                Interest Rate(%)
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtInterestRate" CssClass="textbox_readonly" runat="server" Width="90%"
                            OnTextChanged="InterestRate_TextChanged" ClientIDMode="Static"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                            ControlToValidate="txtInterestRate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Balance Loan Amount -->
            <td class="label">
                Balance Loan Amount
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBalanceLoanAmount" CssClass="textbox_readonly" runat="server"
                            ClientIDMode="Static" Width="90%"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Payment Type -->
            <td class="label">
                Payment Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlPaymentType" runat="server" CssClass="textbox" Height="27px"
                    ClientIDMode="Static" Width="95%" OnSelectedIndexChanged="PaymentType_SelectedIndexChanged">
                    <asp:ListItem>--Select Payment Type--</asp:ListItem>
                    <asp:ListItem>Monthly Payment</asp:ListItem>
                    <asp:ListItem>Pre-payment</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ReqPaymentType" runat="server" ErrorMessage="*" ControlToValidate="ddlPaymentType"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True" InitialValue="--Select Payment Type--">*</asp:RequiredFieldValidator>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Total Days -->
            <td class="label">
                Total Days
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtTotalDays" CssClass="textbox_readonly" runat="server" Width="90%"
                            OnTextChanged="txtTotalDays_TextChanged" AutoPostBack="true"></asp:TextBox>
                        <!-- Range Validator for Total Days -->
                        <asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="*" ControlToValidate="txtTotalDays"
                            Display="Dynamic" ValidationGroup="save" ForeColor="red" Font-Size="Medium" SetFocusOnError="True"
                            Type="Integer" ViewStateMode="Enabled" MinimumValue="1" MaximumValue="1000000"
                            ClientIDMode="Static"></asp:RangeValidator>
                        <%-- <!-- Custom Validator for Total Days -->
                        <asp:CustomValidator runat="server" ID="CustomValidator1" ControlToValidate="txtTotalDays"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="TotalDays_ServerValidate" ErrorMessage="Total Days must be greater than zero."
                            ValidationGroup="save" />--%>
                        </td>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="5">
                <!-- Custom Validator for Payment Type -->
                <asp:CustomValidator runat="server" ID="CustomValidator5" ControlToValidate="ddlPaymentType"
                    ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="ddlPaymentType_ServerValidate" ValidationGroup="save" ErrorMessage="Payment Type should be 'Pre-payment' since Balance Payable Amount is zero." />
            </td>
        </tr>
        <tr>
            <!-- Interest Date -->
            <td class="label">
                Interest Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtPaymentDate" CssClass="textbox" runat="server" Width="75%" MaxLength="10"
                            AutoPostBack="true" placeholder="dd/mm/yyyy" ClientIDMode="Static" OnTextChanged="txtPaymentDate_TextChanged"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                            ControlToValidate="txtPaymentDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />--%>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td class="txt_style" valign="middle">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="lblMsg" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"
                            ForeColor="Red"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <!-- Custom validation for Interest Date -->
        <tr>
            <td>
            </td>
            <td colspan="5">
                <asp:CustomValidator runat="server" ID="cusCustom" ControlToValidate="txtPaymentDate"
                    ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="PaymentDate_ServerValidate" ErrorMessage="Interest Date must be greater than Loan Issued Date and Last EMI Paid Date."
                    ValidationGroup="save" />
                <asp:CompareValidator ID="CVPaymentDate" runat="server" ControlToValidate="txtPaymentDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="save"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <!-- Penal Charges -->
            <td class="label">
                Penal Charges
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel25" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:CheckBox ID="chkPenalCharges" runat="server" OnCheckedChanged="chkPenalCharges_CheckedChanged"
                            AutoPostBack="true" />
                        <asp:DropDownList ID="ddlPenalCharges" runat="server" class="textbox" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlPenalCharges_SelectedIndexChanged" Width="84%" Height="26px">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPenalCharges" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Penal Charges Amount -->
            <td class="txt_style" colspan="2" align="left">
                <asp:UpdatePanel ID="UpdatePanel32" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtPenalChargesAmount" CssClass="textbox_readonly" runat="server"
                            AutoPostBack="true" ReadOnly="true" ClientIDMode="Static" Width="87%" Style="text-align: right"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPenalCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPenalCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtActualInterest" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtReferenceDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Penal Charges Account -->
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel26" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--OnSelectedIndexChanged="ddlPenalChargesAccount_SelectedIndexChanged"--%>
                        <asp:DropDownList ID="ddlPenalChargesAccount" runat="server" class="textbox" AutoPostBack="true"
                            Width="94.5%" Height="26px">
                        </asp:DropDownList>
                         <!-- Custom Validator-->
                        <asp:CustomValidator runat="server" ID="CustomValidator6" ControlToValidate="ddlPenalChargesAccount"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="ddlPenalChargesAccount_ServerValidate" ErrorMessage="Select Penal Charges A/C."
                            ValidationGroup="save" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPenalCharges" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Processing Fee -->
            <td class="label">
                Processing Fee
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel21" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:CheckBox ID="chkProcessingFee" runat="server" OnCheckedChanged="chkProcessingFee_CheckedChanged"
                            AutoPostBack="true" />
                        <asp:DropDownList ID="ddlProcessingCharges" runat="server" class="textbox" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlProcessingCharges_SelectedIndexChanged" Width="84%"
                            Height="26px">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkProcessingFee" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Processing Fee Amount -->
            <td class="txt_style" colspan="2" align="left">
                <asp:UpdatePanel ID="UpdatePanel31" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtProcessingChargesAmount" CssClass="textbox_readonly" runat="server"
                            ReadOnly="true" ClientIDMode="Static" Width="87%" Style="text-align: right"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkProcessingFee" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlProcessingCharges" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Processing Fee Account -->
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel22" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--OnSelectedIndexChanged="ddlProcessingChargesAccount_SelectedIndexChanged"--%>
                        <asp:DropDownList ID="ddlProcessingChargesAccount" runat="server" class="textbox"
                            AutoPostBack="true" Width="94.5%" Height="26px">
                        </asp:DropDownList>
                        <!-- Custom Validator -->
                        <asp:CustomValidator runat="server" ID="CustomValidator1" ControlToValidate="ddlProcessingChargesAccount"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="ddlProcessingChargesAccount_ServerValidate" ErrorMessage="Select Processing Charges A/C."
                            ValidationGroup="save" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkProcessingFee" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Indemnity Fee -->
            <td class="label">
                Indemnity Fee
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel23" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:CheckBox ID="chkIndemnityCharges" runat="server" OnCheckedChanged="chkIndemnityCharges_CheckedChanged"
                            AutoPostBack="true" />
                        <asp:DropDownList ID="ddlIndemnityCharges" runat="server" class="textbox" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlIndemnityCharges_SelectedIndexChanged" Width="84%"
                            Height="26px">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkIndemnityCharges" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Indemnity Fee Amount -->
            <td class="txt_style" colspan="2" align="left">
                <asp:UpdatePanel ID="UpdatePanel30" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtIndemnityChargesAmount" CssClass="textbox_readonly" runat="server"
                            ReadOnly="true" ClientIDMode="Static" Width="87%" Style="text-align: right"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkIndemnityCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlIndemnityCharges" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Indemnity Fee Account -->
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel24" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--OnSelectedIndexChanged="ddlIndemnityChargesAccount_SelectedIndexChanged"--%>
                        <asp:DropDownList ID="ddlIndemnityChargesAccount" runat="server" class="textbox"
                            AutoPostBack="true" Width="94.5%" Height="26px">
                        </asp:DropDownList>
                        <!-- Custom Validator -->
                        <asp:CustomValidator runat="server" ID="CustomValidator7" ControlToValidate="ddlIndemnityChargesAccount"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="ddlIndemnityChargesAccount_ServerValidate" ErrorMessage="Select Indemnity Charges A/C."
                            ValidationGroup="save" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkIndemnityCharges" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Pawn Ticket Re-Issue Charges -->
            <td class="label">
                Re-Issue Pawn Ticket Charges
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel27" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--OnSelectedIndexChanged="ddlPawnTicketReissueCharges_SelectedIndexChanged"--%>
                        <asp:CheckBox ID="chkPawnTicketReissue" runat="server" OnCheckedChanged="chkPawnTicketReissue_CheckedChanged"
                            AutoPostBack="true" />
                        <asp:DropDownList ID="ddlPawnTicketReissueCharges" runat="server" class="textbox"
                            OnSelectedIndexChanged="ddlPawnTicketReissueCharges_SelectedIndexChanged" AutoPostBack="true"
                            Width="84%" Height="26px">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPawnTicketReissue" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Pawn Ticket Re-Issue Charges Amount -->
            <td class="txt_style" colspan="2" align="left">
                <asp:UpdatePanel ID="UpdatePanel29" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtPawnTicketReissueChargesAmount" CssClass="textbox_readonly" runat="server"
                            ReadOnly="true" ClientIDMode="Static" Width="87%" Style="text-align: right"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPawnTicketReissue" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPawnTicketReissueCharges" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Pawn Ticket Re-Issue Charges Account -->
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel28" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <%--OnSelectedIndexChanged="ddlPawnTicketReissueChargesAccount_SelectedIndexChanged"--%>
                        <asp:DropDownList ID="ddlPawnTicketReissueChargesAccount" runat="server" class="textbox"
                            AutoPostBack="true" Width="94.5%" Height="26px">
                        </asp:DropDownList>
                        <!-- Custom Validator -->
                        <asp:CustomValidator runat="server" ID="CustomValidator8" ControlToValidate="ddlPawnTicketReissueChargesAccount"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="ddlPawnTicketReissueChargesAccount_ServerValidate" ErrorMessage="Select Re-Issue Charges A/C."
                            ValidationGroup="save" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="chkPawnTicketReissue" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Deposited Amount -->
            <td class="label">
                Deposited Amount
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel18" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDepositedAmount" CssClass="textbox" runat="server" Width="90%"
                            AutoPostBack="true" ClientIDMode="Static" MaxLength="10" onkeypress="return isNumeric(event);"
                            OnTextChanged="txtDepositedAmount_TextChanged"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                            ControlToValidate="txtDepositedAmount" ValidationGroup="save" ForeColor="Red"
                            Display="Dynamic" Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtDepositedAmount"
                            Display="Dynamic" ValidationGroup="save" ForeColor="red" Font-Size="Small" ErrorMessage="Enter valid number"
                            ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <%--<!-- Interest Formula -->
            <td align="right">
                <asp:Label ID="Label4" runat="server" Text="Interest Formula" CssClass="label"></asp:Label>
            </td>
            <td>
                <asp:Label ID="lblInterestFormula" runat="server" Text="" CssClass="textbox_readonly"
                    Width="90%"></asp:Label>
            </td>--%>
            <!-- Actual Interest -->
            <td align="right">
                <asp:Label ID="Label4" runat="server" Text="Actual Interest" CssClass="label"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtActualInterest" CssClass="textbox_readonly" runat="server" AutoPostBack="true"
                            ClientIDMode="Static" Width="90%" OnTextChanged="ActualInterest_TextChanged"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <!-- Custom validation for Deposited Amount -->
        <tr>
            <td>
            </td>
            <td colspan="5">
                <asp:CustomValidator runat="server" ID="CustomValidator2" ControlToValidate="txtDepositedAmount"
                    ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="DepositedAmount_ServerValidate" ErrorMessage="Deposited Amount is incorrect."
                    ValidationGroup="save">*</asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <!-- Balance Interest -->
            <td align="right">
                <asp:Label ID="Label15" runat="server" Text="Balance Interest" CssClass="label"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel17" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBalanceInterest" CssClass="textbox_readonly" runat="server" AutoPostBack="true"
                            ClientIDMode="Static" Width="90%"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Balance Loan Amount + Interest -->
            <td align="right">
                <asp:Label ID="Label5" runat="server" Text="Balance Amt + Interest" CssClass="label"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel16" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBalanceLoanAmountPlusInterest" CssClass="textbox_readonly" runat="server"
                            Width="90%" ClientIDMode="Static"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Total Balance Payable -->
            <td align="right">
                <asp:Label ID="Label3" runat="server" Text="Total Balance Payable" CssClass="label"
                    ForeColor="Red" Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style">
                <%--BackColor="#ffff00"--%>
                <asp:UpdatePanel ID="UpdatePanel14" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtTotalBalancePayable" CssClass="textbox_readonly" runat="server"
                            AutoPostBack="true" ClientIDMode="Static" Width="90%"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="txtTotalBalancePayable"
                            Display="Dynamic" ValidationGroup="save" ForeColor="red" Font-Size="Small" ErrorMessage="Balance Payable Amount cannot be negative."
                            ValidationExpression="^[0-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                        <!-- Custom Validator for Payment Type -->
                        <asp:CustomValidator runat="server" ID="CustomValidator3" ControlToValidate="txtTotalBalancePayable"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="BalancePayable_ServerValidate" ErrorMessage="Balance Payable Amount should be zero."
                            ValidationGroup="save" />
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
            <!-- Monthly Payment -->
            <td align="right">
                <asp:Label ID="Label17" runat="server" Text="Monthly Payment" CssClass="label" ForeColor="Red"
                    Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtMonthlyPayment" CssClass="textbox_readonly" runat="server" Width="90%"
                            ClientIDMode="Static"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="txtMonthlyPayment"
                            Display="Dynamic" ValidationGroup="save" ForeColor="red" Font-Size="Small" ErrorMessage="Monthly Payment Amount must be greater than zero."
                            ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtReferenceDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="ddlPenalCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlProcessingCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlIndemnityCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPawnTicketReissueCharges" EventName="SelectedIndexChanged" />
                        
                        <asp:AsyncPostBackTrigger ControlID="chkPenalCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkProcessingFee" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkIndemnityCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkPawnTicketReissue" EventName="CheckedChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Narration -->
            <td class="label">
                Narration
            </td>
            <td class="txt_style" colspan="5">
                <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlNarration" runat="server" CssClass="textbox" Height="27px"
                            Width="96%" AutoPostBack="true" ClientIDMode="Static">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ReqNarration" runat="server" ErrorMessage="*" ControlToValidate="ddlNarration"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Bank/Cash A/C  -->
            <td class="label">
                Bank/Cash A/C
            </td>
            <td class="txt_style" colspan="5">
                <asp:UpdatePanel ID="UpdatePanel20" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCashAccount" runat="server" CssClass="textbox" Height="27px"
                            Width="96%" ClientIDMode="Static">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                            ControlToValidate="ddlCashAccount" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Interest A/C A/C  -->
            <td class="label">
                Interest A/C
            </td>
            <td class="txt_style" colspan="5">
                <asp:UpdatePanel ID="UpdatePanel33" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInterestAccount" runat="server" CssClass="textbox" Height="27px"
                            Width="96%" ClientIDMode="Static">
                        </asp:DropDownList>
                        <!-- Custom Validator -->
                        <asp:CustomValidator runat="server" ID="CustomValidator9" ControlToValidate="ddlInterestAccount"
                            ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                            OnServerValidate="ddlInterestAccount_ServerValidate" ErrorMessage="Select Interest A/C."
                            ValidationGroup="save" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Cheque Number -->
            <td class="label">
                Cheque No
            </td>
            <td class="txt_style" colspan="2">
                <asp:TextBox ID="txtChequeNo" class="textbox" runat="server" onkeypress="return OnlyNumericEntry();"
                    Style="text-align: right;" Width="90%" MaxLength="6"></asp:TextBox>
            </td>
            <!-- Cheque Date -->
            <td class="label" align="center">
                Cheque Date
            </td>
            <td class="txt_style" colspan="2">
                <asp:TextBox ID="txtChequeDate" class="textbox" runat="server" Width="72%" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <!--Bifurcation of Monthly Payment - LABELs -->
        <tr>
            <td align="right">
                <asp:Label ID="Label24" runat="server" Text="Payment Bifurcation" CssClass="label"
                    ForeColor="Red" Font-Bold="true"></asp:Label>
            </td>
            <td colspan="5" align="center">
                <!-- background-color: #81bef7 #ffbf00  #3366cc-->
                <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; background-color: #81bef7;
                    color: #000000; font-weight: bold; height: 24px">
                    <tr>
                        <td align="center" valign="middle" width="19%">
                            <asp:Label ID="Label6" runat="server" Text="Deposited Amount" Font-Names="Verdana"
                                Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="1%">
                            <asp:Label ID="Label7" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="19%">
                            <asp:Label ID="Label8" runat="server" Text="Principal" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="1%">
                            <asp:Label ID="Label9" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="19%">
                            <asp:Label ID="Label10" runat="server" Text="Interest" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="1%">
                            <asp:Label ID="Label11" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                        </td>
                        <td align="center" valign="middle" width="19%" colspan="3">
                            <asp:Label ID="Label12" runat="server" Text="Total Charges" Font-Names="Verdana"
                                Font-Size="10px"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Bifurcation of Monthly Payment - VALUES -->
        <tr>
            <td>
            </td>
            <td colspan="5" align="center">
                <asp:UpdatePanel ID="UpdatePanel19" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <table cellpadding="0" cellspacing="0" border="0" style="width: 100%; background-color: #ffffff;
                            border-color: #a4a4a4; border-style: solid; border-width: 1px; height: 24px">
                            <tr>
                                <td align="center" valign="middle" width="19%">
                                    <asp:Label ID="lblDepositedAmount" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="1%">
                                    <asp:Label ID="Label16" runat="server" Text="=" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="19%">
                                    <asp:Label ID="lblPrincipal" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="1%">
                                    <asp:Label ID="Label18" runat="server" Text="+" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="19%">
                                    <asp:Label ID="lblInterest" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="1%">
                                    <asp:Label ID="Label20" runat="server" Text="+" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                                <td align="center" valign="middle" width="19%" colspan="3">
                                    <asp:Label ID="lblTotalCharges" runat="server" Text="" Font-Names="Verdana" Font-Size="10px"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtPaymentDate" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalDays" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepositedAmount" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkPenalCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkProcessingFee" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkIndemnityCharges" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="chkPawnTicketReissue" EventName="CheckedChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPenalCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlProcessingCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlIndemnityCharges" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlPawnTicketReissueCharges" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <!-- ID -->
                <asp:TextBox ID="txtID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Branch ID -->
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- FY ID -->
                <asp:TextBox ID="txtFYID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Comp ID -->
                <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Balance Interest Payable, Balance Penal Charges Payable, Balance Processing Charges Payable, -->
                <!-- Balance Indemnity Fee Payable, Balance Pawn Ticket ReIssue Charges Payable
            -->
                <asp:TextBox ID="txtBalInterestPayable" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalPenalChargesPayable" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalProcessingChargesPayable" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalIndemnityFeePayable" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalPawnTicketReIssueChargesPayable" runat="server" Visible="false"
                    Height="22px" Width="17px"></asp:TextBox>
                <!--Balance Loan Amount, Balance Penal Charges, Balance Processing Charges, Balance Indemnity Fee, Balance Pawn Ticket ReIssue Charges-->
                <asp:TextBox ID="txtBalanceLoanAmountCalc" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalancePenalCharges" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalanceProcessingCharges" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalanceIndemnityFee" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
                <asp:TextBox ID="txtBalancePawnTicketReIssueCharges" runat="server" Visible="false"
                    Height="22px" Width="17px"></asp:TextBox>
                <!-- Operator ID -->
                <asp:TextBox ID="txtOperatorID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Due Date -->
                <asp:TextBox ID="txtDueDate" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                 <!-- Total Charges Amount -->
                <asp:TextBox ID="txtTotalChargesAmount" runat="server" Visible="false" Height="22px"
                    Width="17px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="6">
                <br />
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Cancel" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="right" colspan="6">
                <asp:Label ID="Label28" runat="server" Text="[Search Section]" Font-Names="Verdana"
                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="6">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="left" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="textbox_search" Height="26px"
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
                    <br />
                    <br />
                    <br />
                    <br />
                    <!--GridView DGV Details (Edit/Delete section) -->
                    <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="JVID" OnRowCommand="dgvDetails_RowCommand"
                                OnPageIndexChanging="dgvDetails_PageIndexChanging" Width="100%" AutoGenerateColumns="False">
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
                                    <asp:BoundField DataField="JVID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="JVReferenceNo" HeaderText="JV Ref-No" />
                                    <asp:BoundField DataField="PaymentDate" HeaderText="Payment Date" Visible="false" />
                                    <asp:BoundField DataField="GoldLoanNo" HeaderText="GL-No" />
                                    <asp:BoundField DataField="LoanIssuedDate" HeaderText="Loan Date" Visible="false" />
                                    <asp:BoundField DataField="LastEMIPaidDate" HeaderText="Last EMI Date" Visible="false" />
                                    <asp:BoundField DataField="PaymentType" HeaderText="Payment Type" />
                                    <asp:BoundField DataField="InterestDate" HeaderText="Interest Date" />
                                    <asp:BoundField DataField="TotalLoanAmount" HeaderText="Total Loan Amount" Visible="false" />
                                    <asp:BoundField DataField="BalanceLoanAmount" HeaderText="Balance Loan Amount" />
                                    <asp:BoundField DataField="DepositedAmount" HeaderText="Deposited Amount" />
                                    <asp:BoundField DataField="InterestRate" HeaderText="Int Rate %" />
                                    <asp:BoundField DataField="NoofDays" HeaderText="Days" />
                                    <asp:BoundField DataField="TotalMonthlyPayment" HeaderText="Monthly Payment" />
                                    <asp:BoundField DataField="PrincipleAmount" HeaderText="Principle" Visible="false" />
                                    <asp:BoundField DataField="InterestAmount" HeaderText="Interest" Visible="false" />
                                    <asp:BoundField DataField="TotalBalancePayable" HeaderText="Total Balance Payable" />
                                    <asp:BoundField DataField="TotalChargesAmount" HeaderText="Processing Fee" Visible="false" />
                                    <asp:BoundField DataField="NarrationID" HeaderText="NarrationID" Visible="False" />
                                    <asp:BoundField DataField="NarrationName" HeaderText="Narration" Visible="False" />
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
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ba2f14" Font-Bold="false" ForeColor="#ffffcc" Height="24px"
                                    Font-Names="Calibri" Font-Size="15px" />
                                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" Font-Names="Calibri" Font-Size="13px" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="true" ForeColor="Navy" Font-Names="Calibri"
                                    Font-Size="13px" />
                                <SortedAscendingCellStyle BackColor="#FDF5AC" />
                                <SortedAscendingHeaderStyle BackColor="#4D0000" />
                                <SortedDescendingCellStyle BackColor="#FCF6C0" />
                                <SortedDescendingHeaderStyle BackColor="#820000" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
                <br />
                <br />
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">        enableDueDate();</script>
</asp:Content>
