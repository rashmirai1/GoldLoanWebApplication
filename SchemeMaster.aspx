<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="SchemeMaster.aspx.cs" Inherits="SchemeMaster"
    EnableViewStateMac="false" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
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
    <script type="text/javascript">


        function isNumeric() {

            var txtMaxLoan = document.getElementById("<%=txtMaxLoan.ClientID %>");
            var txtMinLoan = document.getElementById("<%=txtMinLoan.ClientID %>");
            var txtLtv = document.getElementById("<%=txtLtv.ClientID %>");
            var txtROI = document.getElementById("<%=txtROI.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtProcessingCharges = document.getElementById("<%=txtProcessingCharges.ClientID %>");
            var txtPanelInterest = document.getElementById("<%=txtPanelInterest.ClientID %>");
            var txtPerAmtLimit = document.getElementById("<%=txtPerAmtLimit.ClientID %>");
            var txtServiceTax = document.getElementById("<%=txtServiceTax.ClientID %>");
            var ddlProChareType = document.getElementById("<%=ddlProChareType.ClientID %>");


            if (isNaN(txtMaxLoan.value)) {
                txtMaxLoan.value = '';
                return false;

            }
            if (isNaN(txtMinLoan.value)) {
                txtMinLoan.value = '';
                return false;

            }
            if (isNaN(txtLtv.value)) {
                txtLtv.value = '';
                return false;

            }
            if (parseFloat(txtLtv.value) > 100) {
                txtLtv.value = '';
                return false;

            }
            if (isNaN(txtROI.value)) {
                txtROI.value = '';
                return false;

            }
            if (parseFloat(txtROI.value) > 100) {
                txtROI.value = '';
                return false;

            }
            if (isNaN(txtEMI.value)) {
                txtEMI.value = '';
                return false;

            }

            if (isNaN(txtProcessingCharges.value)) {
                txtProcessingCharges.value = '';
                return false;

            }
            if (ddlProChareType.selectedIndex == 2) {

                if (parseFloat(txtProcessingCharges.value) > 100) {

                    txtProcessingCharges.value = '';
                    return false;
                }
            }
            if (isNaN(txtPanelInterest.value)) {
                txtPanelInterest.value = '';
                return false;

            }
            if (parseFloat(txtPanelInterest.value) > 100) {
                txtPanelInterest.value = '';
                return false;

            }
            if (isNaN(txtPerAmtLimit.value)) {

                txtPerAmtLimit.value = '';
                return false;
            }
            if (isNaN(txtServiceTax.value)) {

                txtServiceTax.value = '';
                return false;
            }
            if (parseFloat(txtServiceTax.value) > 100) {

                txtServiceTax.value = '';
                return false;
            }
        }
        function ValidSchemeType() {


            var ddlSchemType = document.getElementById("<%=ddlSchemType.ClientID %>");
            var txtROI = document.getElementById("<%=txtROI.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtPanelInterest = document.getElementById("<%=txtPanelInterest.ClientID %>");
            var gvROI = document.getElementById("<%=gvROI.ClientID %>");
            var PnlGv = document.getElementById("<%=PnlGv.ClientID %>");
            var btnScheme = document.getElementById("<%=btnScheme.ClientID %>");



            if (ddlSchemType.value == 'MonthlyEMI') {

                txtROI.value = '';
                txtEMI.value = '';
                txtPanelInterest.value = '';
                btnScheme.click();



            }
            else {

                txtROI.value = '';
                txtEMI.value = '';
                txtPanelInterest.value = '';
                btnScheme.click();


            }


        }

        function validrecord() {

            var txtSchemeName = document.getElementById("<%=txtSchemeName.ClientID %>");
            var ddlSchemType = document.getElementById("<%=ddlSchemType.ClientID %>");
            var txtMinLoan = document.getElementById("<%=txtMinLoan.ClientID %>");
            var txtTenure = document.getElementById("<%=txtTenure.ClientID %>");
            var txtMaxLoan = document.getElementById("<%=txtMaxLoan.ClientID %>");
            var txtLtv = document.getElementById("<%=txtLtv.ClientID %>");
            var txtROI = document.getElementById("<%=txtROI.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtProcessingCharges = document.getElementById("<%=txtProcessingCharges.ClientID %>");
            var txtPanelInterest = document.getElementById("<%=txtPanelInterest.ClientID %>");
            var ddlProChareType = document.getElementById("<%=ddlProChareType.ClientID %>");
            var txtPerAmtLimit = document.getElementById("<%=txtPerAmtLimit.ClientID %>");
            var gvROI = document.getElementById("<%=gvROI.ClientID %>");
            var ddlCalMethod = document.getElementById("<%=ddlCalculation.ClientID %>");
            var ddlprocChargeType = document.getElementById("<%=ddlProChareType.ClientID %>");
            var txtServiceTax = document.getElementById("<%=txtServiceTax.ClientID %>");


            if (txtSchemeName.value == '') {
                alert('Enter Scheme Name');
                return false;
            }

            if (ddlSchemType.value == 0) {
                alert('Select Scheme Type');
                return false;
            }
            if (ddlSchemType.value == 'Slabwise') {
                if (txtMinLoan.value == '' || txtMinLoan.value == 0) {
                    alert('Enter Min. Loan Amount. It Should be more than Zero');
                    return false;
                }
                if (isNaN(txtMinLoan.value)) {
                    alert('Min. Loan Amount Not In Correct Format');
                    txtMinLoan.value = '';
                    return false;
                }
                if (txtMaxLoan.value == '' || txtMaxLoan.value == 0) {
                    alert('Enter Max. Loan Amount. It Should Be More Than Zero');
                    return false;
                }
                if (parseFloat(txtMinLoan.value) >= parseFloat(txtMaxLoan.value)) {
                    alert('Min. Loan Amount Should Be Less Than Max. Loan Amount');
                    return false;
                }
                if (isNaN(txtMaxLoan.value)) {
                    alert('Max. Loan Amount Not In Correct Format');
                    txtMaxLoan.value = '';
                    return false;
                }
            }
            if (txtTenure.value == '' || txtTenure.value == 0) {
                alert('Enter Tenure. It Should Be More Than Zero');
                return false;
            }
            if (isNaN(txtTenure.value)) {

                alert('Tenure Not In Correct Format');
                txtTenure.value = '';
                return false;
            }
            if (txtLtv.value == '') {
                alert('Enter LTV Percentage');
                return false;
            }
            if (isNaN(txtLtv.value)) {
                alert('LTV Percentage Not In Correct Format');
                txtLtv.value = '';
                return false;
            }
            if (ddlCalMethod.value == 0) {
                alert('Select Calculation Method');
                return false;
            }
            if (ddlprocChargeType.value == 0) {
                alert('Select Processing Charges Type');
                return false;
            }
            if (txtProcessingCharges.value == '' || txtProcessingCharges.value == 0) {
                alert('Enter Processing Charges. It Should Be More Than Zero');
                return false;
            }
            if (isNaN(txtProcessingCharges.value)) {
                alert('Processing Charge Not In Correct Format');
                txtProcessingCharges.value = '';
                return false;
            }
            if (txtServiceTax.value == '' || txtServiceTax.value == 0) {
                alert('Enter Service Tax. It Should Be More Than Zero');
                return false;

            }
            if (isNaN(txtServiceTax.value)) {
                alert('Service Tax Not In Correct Format');
                txtServiceTax.value = '';
                return false;
            }
            if (ddlSchemType.value == "MonthlyEMI") {

                if (txtROI.value == '') {
                    alert('Enter ROI Percentage');
                    return false;
                }


                if (isNaN(txtROI.value)) {
                    alert('ROI Percentage Not In Correct Format');
                    txtROI.value = '';
                    return false;
                }
                if (txtEMI.value == '') {
                    alert('Enter EMI');
                    txtEMI.value = '';
                    return false;
                }
                if (isNaN(txtEMI.value)) {
                    alert('EMI  Not In Correct Format');
                    txtEMI.value = '';
                    return false;
                }
                if (txtPanelInterest.value == '') {
                    alert('Enter Penal Interest');
                    return false;
                }
                if (isNaN(txtPanelInterest.value)) {
                    alert('Penal Interest Not In Correct Format');
                    txtPanelInterest.value = '';
                    return false;
                }


            }

            else if (ddlSchemType.value == "Slabwise") {

                for (i = 1; i < gvROI.rows.length - 1; i++) {
                    var defaultmonth = gvROI.rows[i].cells[0].getElementsByTagName("span");
                    var roi = gvROI.rows[i].cells[1].getElementsByTagName("span");
                    if (defaultmonth[0].innerHTML == '' || roi[0].innerHTML == '') {

                        alert('Add Effective ROI');
                        return false;
                    }

                }
            }

        }
        function AmtLmt() {

            var ddlProChareType = document.getElementById("<%=ddlProChareType.ClientID %>");
            var txtPerAmtLimit = document.getElementById("<%=txtPerAmtLimit.ClientID %>");
            if (ddlProChareType.selectedIndex == 0) {

                txtPerAmtLimit.value = '';
                txtPerAmtLimit.disabled = true;
                return false;
            }
            else {

                txtPerAmtLimit.value = '';
                txtPerAmtLimit.disabled = false;
                return false;
            }

        }
        function procType() {

            var txtProcessingCharges = document.getElementById("<%=txtProcessingCharges.ClientID %>");
            txtProcessingCharges.value = '';
        }
    </script>
    <%--  <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>--%>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnPopup" runat="server" Value="Edit" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 96%">
        <tr>
            <td style="width: 26%;">
            </td>
            <td style="width: 24%;">
            </td>
            <td style="width: 26%;">
            </td>
            <td style="width: 24%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Loan Scheme Master"></asp:Label>
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
            <td class="label" style="text-align: left;">
                Scheme ID
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSchemeId" onkeypress="return isAlphaNumChars(event);" class="textbox_readonly"
                    MaxLength="50" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Scheme Name -->
            <td class="label" style="text-align: left;">
                Scheme Name<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSchemeName" onkeypress="return isAlphaNumChars(event);" class="textbox"
                    MaxLength="50" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Scheme Name -->
            <td class="label" style="text-align: left;">
                Scheme Type<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlSchemType" runat="server" Width="95%" Height="28px" CssClass="textbox"
                    onchange="return ValidSchemeType();">
                    <asp:ListItem Value="0">--Select Scheme Type--</asp:ListItem>
                    <asp:ListItem Value="MonthlyEMI">Monthly EMI</asp:ListItem>
                    <asp:ListItem Value="Slabwise">Slabwise</asp:ListItem>
                </asp:DropDownList>
                <asp:Button ID="btnScheme" runat="server" Text="Button" Style="display: none;" OnClick="btnScheme_Click" />
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Source of Application -->
            <td class="label" style="text-align: left;">
                Min. Loan Amount<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMinLoan" runat="server" Width="90%" onkeyup="return isNumeric();"
                    MaxLength="10" CssClass="textbox"></asp:TextBox>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Tenure<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtTenure" CssClass="textbox" runat="server" Width="90%" MaxLength="5"
                    onkeypress="return OnlyNumericEntry();" class="textbox" AutoPostBack="True" OnTextChanged="txtTenure_TextChanged"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Max. Loan Amount<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMaxLoan" runat="server" Width="90%" onkeyup="return isNumeric();"
                    MaxLength="10" CssClass="textbox" AutoPostBack="True" OnTextChanged="txtMaxLoan_TextChanged"></asp:TextBox>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                LTV(%)<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLtv" CssClass="textbox" runat="server" Width="90%" MaxLength="6"
                    onkeyup="return isNumeric();"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Calculation Method<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlCalculation" runat="server" Width="95%" Height="28px" CssClass="textbox">
                    <asp:ListItem Value="0">--Select Calculation Method--</asp:ListItem>
                    <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                    <asp:ListItem Value="Quarterly">Quarterly</asp:ListItem>
                    <asp:ListItem Value="HalfYearly">Half Yearly</asp:ListItem>
                    <asp:ListItem Value="Yearly">Yearly</asp:ListItem>
                </asp:DropDownList>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                ROI(%)
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtROI" runat="server" Width="90%" MaxLength="6" onkeyup="return isNumeric();"
                    CssClass="textbox" AutoPostBack="True" OnTextChanged="txtROI_TextChanged"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Processing Charge Type<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlProChareType" runat="server" onchange="return procType();"
                    Width="95%" Height="27px" CssClass="textbox">
                    <asp:ListItem Value="0">--Select Processing Charge Type--</asp:ListItem>
                    <asp:ListItem Value="Amount">Amount</asp:ListItem>
                    <asp:ListItem Value="Percentage">Percentage</asp:ListItem>
                </asp:DropDownList>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                EMI
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtEMI" CssClass="textbox" runat="server" Width="90%" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Processing Charges<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtProcessingCharges" runat="server" Width="90%" onkeyup="return isNumeric();"
                    MaxLength="10" CssClass="textbox"></asp:TextBox>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Penal Interest(%)
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPanelInterest" CssClass="textbox" runat="server" Width="90%"
                    MaxLength="6" onkeyup="return isNumeric();"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Processing Amount Limit To
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPerAmtLimit" CssClass="textbox" runat="server" Width="90%" MaxLength="10"
                    onkeyup="return isNumeric();"></asp:TextBox>
            </td>
            <!-- Please Specify (source of application) -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Service Tax(%)<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtServiceTax" CssClass="textbox" runat="server" Width="90%" MaxLength="6"
                    onkeyup="return isNumeric();"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <br />
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;" valign="top">
                Effective ROI<b style="color: Red; display: none;" id="astk">*</b> :
            </td>
            <td class="txt_style" colspan="3">
                <asp:Panel ID="PnlGv" runat="server" Width="100%">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <asp:GridView ID="gvROI" runat="server" AutoGenerateColumns="False" BorderStyle="Solid"
                                ShowFooter="True" Width="99%">
                                <HeaderStyle CssClass="gVHeader" />
                                <RowStyle CssClass="gVItem" />
                                <Columns>
                                    <asp:TemplateField HeaderText="No. Of Default Months">
                                        <ItemTemplate>
                                            <asp:Label ID="gvlblNoofMonths" runat="server" Text='<%#Eval("NoofDefaultMonths") %>'></asp:Label>
                                            <asp:HiddenField ID="gvhdnEffRowId" runat="server" Value='<%#Eval("ROIID") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <table cellpadding="0" cellspacing="0" width="99%">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="gvtxtFooterDefaultMonth" runat="server" MaxLength="3" Width="100%"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:RegularExpressionValidator ID="reMonth" runat="server" ControlToValidate="gvtxtFooterDefaultMonth"
                                                            ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Invalid Data."
                                                            SetFocusOnError="True" ValidationGroup="VerifyROI" ValidationExpression="^[0-9]\d*(\.\d+)?$">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        </asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ROI(%)">
                                        <ItemTemplate>
                                            <asp:Label ID="gvlblROI" runat="server" Text='<%#Eval("EffROI") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <table cellpadding="0" cellspacing="0" width="99%">
                                                <tr>
                                                    <td>
                                                        <asp:TextBox ID="gvtxtFooterRoi" runat="server" onkeypress="return isNumeric(event);"
                                                            MaxLength="5" Width="100%"></asp:TextBox>
                                                    </td>
                                                    <td>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td>
                                                        <asp:RegularExpressionValidator ID="reROI" runat="server" ControlToValidate="gvtxtFooterRoi"
                                                            ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Invalid Data."
                                                            SetFocusOnError="True" ValidationGroup="VerifyROI" ValidationExpression="^[0-9]\d*(\.\d+)?$">
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                        </asp:RegularExpressionValidator>
                                                        <asp:RangeValidator ID="rvROI" runat="server" ErrorMessage="RangeValidator" ControlToValidate="gvtxtFooterRoi"
                                                            CultureInvariantValues="true" MinimumValue="0" MaximumValue="100" Type="Double"
                                                            Width="20" SetFocusOnError="true" ForeColor="Red" ValidationGroup="VerifyROI"
                                                            Text="ROI should be between 0 to 100" Display="Dynamic" Font-Size="Small"></asp:RangeValidator>
                                                        <asp:RegularExpressionValidator ID="revLoanAmtFrm1" runat="server" ControlToValidate="gvtxtFooterRoi"
                                                            ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter 2 decimal value."
                                                            SetFocusOnError="True" ValidationGroup="VerifyROI" ValidationExpression="^(-)?\d+(\.\d\d)?$"></asp:RegularExpressionValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Action">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImgBtnRemove" runat="server" ToolTip="Remove" Width="20" OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Height="20" ImageUrl="~/images/DeleteRed.png" OnClick="ImgBtnRemove_Click" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Button ID="btnFooterAdd" runat="server" Text="Add" OnClick="btnFooterAdd_Click"
                                                ValidationGroup="VerifyROI" Width="100%" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataRowStyle BackColor="LightBlue" ForeColor="Red" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="gvROI" />
                        </Triggers>
                    </asp:UpdatePanel>
                </asp:Panel>
            </td>
            <!-- Please Specify (source of application) -->
        </tr>
    </table>
</asp:Content>
