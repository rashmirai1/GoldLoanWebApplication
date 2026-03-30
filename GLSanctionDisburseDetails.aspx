<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="GLSanctionDisburseDetails.aspx.cs" EnableViewStateMac="false"
    MaintainScrollPositionOnPostback="true" Inherits="GLSanctionDisburseDetails" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <%--<asp:ModalPopupExtender ID="mpLocal1" runat="server" TargetControlID="Button1" CancelControlID="Button3"
        OkControlID="btnOkay" PopupDragHandleControlID="PopupHeader" BackgroundCssClass="ModalPopupBG"
        PopupControlID="Panel3">
    </asp:ModalPopupExtender>--%>

    <%--sorting js  Added by Priya--%>
    <script src="Jquery/jquery.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery-ui.js" type="text/javascript"></script>
    <link href="Jquery/LoginPopup.css" rel="stylesheet" type="text/css" />
    <script src="Jquery/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script type="text/javascript">
        function fncPopup() {
            //document.getElementById('<%= btnCashPopup.ClientID %>').click();

            var ExcessAmount = $("[id$='txtCashAmount']").val();
            var TotalCash = $("[id$='HiddenFieldTotalCash']").val();

            var totalAmt = parseFloat(ExcessAmount) + parseFloat(TotalCash);

            if (parseFloat(totalAmt) > 99999) {
                ShowPopup('Cash Amount Exceeds Limit. Do You Want to go to Authorized Login??', 'Login');
            }
        }
    </script>
    <script type="text/javascript">
        function GetGolLoanNo() {

            var txtLoanDate = document.getElementById("<%=txtLoanDate.ClientID %>");
            $.ajax({
                type: "Post",
                url: "WebMethod.aspx/GetGoldLoanNo",
                data: '{LoanDate:"' + txtLoanDate.value + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (obj) {
                    $("#<%=txtGoldLoanNo.ClientID %>").val(obj.d);
                },
                failure: function (response) {
                    alert(response.d);
                }
            });
            }
    </script>
    <script language="javascript" type="text/javascript">
        var totalamt = 0;
        $(document).ready(function () {
            $('#txtPwd').keydown(function (e) {
                var name = $('#txtName').val();
                var pwd = $('#txtPwd').val();
                var ExcessAmount = $("[id$='txtCashAmount']").val();
                var GoldLoanNo = $("[id$='txtGoldLoanNo']").val();

                $("[id$='HiddenFieldName']").val(name);
                $("[id$='HiddenFieldPwd']").val(pwd);
                $("[id$='HiddenFieldGoldNo']").val(GoldLoanNo);

                if (e.which == 13) {
                    if (name == "") {
                        alert('Enter User Name');
                    }
                    else if (pwd == "") {
                        alert('Enter Password');
                    }
                    else {
                        if (name == "Vaishali" && pwd == "afpl2015") {//For Live DB
                            // if (name == "Admin" && pwd == "Admin") {
                            alert("Login Successfully");
                            $.ajax({
                                type: "POST",
                                url: "GLSanctionDisburseDetails.aspx/GetAuthLogin",
                                data: '{name: "' + name + '",Password: "' + pwd + '",Amount:"' + ExcessAmount + '",GoldLoanNo:"' + GoldLoanNo + '",LoginStatus:"Initial"}',
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: OnSuccess,
                                failure: function (response) {
                                    alert(response.d);
                                }
                            });
                            $('#dialog').dialog('close');
                            //$(this).dialog("close");
                        }
                        else {
                            alert("Wrong Password or Username"); /*displays error message*/
                            $('#txtName').val('');
                            $('#txtPwd').val('');
                            $('#txtName').focus();
                        }
                    }

                }
            });
        });


        function ShowPopup(message, header) {

            $(function () {
                $('#txtName').val('');
                $('#txtPwd').val('');

                $("#dialog .modal-header").html(message);
                $("#dialog").dialog({
                    title: "Login",
                    height: 300,
                    width: 250,

                    buttons: {
                        Login: function () {
                            $(this).dialog('Login');
                            var name = $('#txtName').val();
                            var pwd = $('#txtPwd').val();
                            var ExcessAmount = $("[id$='txtCashAmount']").val();
                            var GoldLoanNo = $("[id$='txtGoldLoanNo']").val();

                            $("[id$='HiddenFieldName']").val(name);
                            $("[id$='HiddenFieldPwd']").val(pwd);
                            $("[id$='HiddenFieldGoldNo']").val(GoldLoanNo);

                            if (name == "") {
                                alert('Enter User Name');
                            }
                            else if (pwd == "") {
                                alert('Enter Password');
                            }
                            else {
                                if (name == "Vaishali" && pwd == "afpl2015") {//For Live DB
                                    //  if (name == "Admin" && pwd == "Admin") {
                                    alert("Login Successfully");
                                    $.ajax({
                                        type: "POST",
                                        url: "GLSanctionDisburseDetails.aspx/GetAuthLogin",
                                        data: '{name: "' + name + '",Password: "' + pwd + '",Amount:"' + ExcessAmount + '",GoldLoanNo:"' + GoldLoanNo + '",LoginStatus:"Initial"}',
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: OnSuccess,
                                        failure: function (response) {
                                            alert(response.d);
                                        }
                                    });

                                    $(this).dialog("close");
                                }
                                else {
                                    alert("Wrong Password or Username"); /*displays error message*/
                                    $('#txtName').val('');
                                    $('#txtPwd').val('');
                                    $('#txtName').focus();
                                }
                            }

                        },
                        Close: function () {
                            $("[id$='txtCashAmount']").val('');
                            $(this).dialog("close");
                        }
                    },
                    modal: true
                });
            });
        };
        function OnSuccess(response) {
            //  alert(response.d);
        }


        //Function Added By Priya for Date exist , validation start
        function DateFormat(txt, keyCode) {
            if (keyCode == 16)
                isShift = true;
            //Validate that its Numeric
            if (((keyCode >= 48 && keyCode <= 57) || keyCode == 8 ||
         keyCode <= 37 || keyCode <= 39 || (keyCode >= 96 && keyCode <= 105)) && isShift == false) {
                if ((txt.value.length == 2 || txt.value.length == 5) && keyCode != 8) {
                    txt.value += seperator;
                }
                return true;
            }
            else {
                return false;
            }
        }

        function ValidateDate(txt, keyCode) {

            if (keyCode == 16)
                isShift = false;
            var val = txt.value;

            if (val.length == 10) {
                var splits = val.split("/");
                var dt = new Date(splits[1] + "/" + splits[0] + "/" + splits[2]);

                //Validation for Dates
                if (dt.getDate() == splits[0] && dt.getMonth() + 1 == splits[1] && dt.getFullYear() == splits[2]) {

                }
                else {
                    txt.value = '';
                    alert('Invalid Date.');
                    return;
                }
            }
            else if (val.length < 10) {

                txt.value = '';
                alert('Invalid Date.');
                return;
            }
        }
        //date end

        //DatePicker
        //disable DatePicker 
        function disableDate() {
            var accgpid = document.getElementById("<%=txtAccGPID.ClientID %>").value;
//            if (accgpid == "70" || accgpid == "" || accgpid == null) {
//                $('#<%=txtChequeDate.ClientID %>').dateEntry('disable');
//            }
//            else {
//                $('#<%=txtChequeDate.ClientID %>').dateEntry('enable');
//            }
        }
        //disable Due Date DatePicker 
        function disableDueDate() {
            var schemeid = document.getElementById("<%=ddlSchemeName.ClientID %>").value;
            if (schemeid == "0") {
//                $('#<%=txtDueDate.ClientID %>').dateEntry('disable');
                document.getElementById("<%=txtNetAmountSanctioned.ClientID %>").enabled = false;
            }
            else {

                var schemetype = document.getElementById("<%=txtSchemeType.ClientID %>").value;
                if (schemetype == null) {
                }
                if (schemetype == "MI") {
                    $('#<%=txtDueDate.ClientID %>').dateEntry('enable');
                }
                else {
                    $('#<%=txtDueDate.ClientID %>').dateEntry('disable');
                }

                document.getElementById("<%=txtNetAmountSanctioned.ClientID %>").enabled = true;
            }
        }

        function isAlphaNumCharsDot(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k == 46));
        }
        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return (k > 45 && k < 57);
        }
        function isAlphaNumChar2(e) { // Alphanumeric, space,-,/ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return (k == 44 || (k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 47) || k == 0);
        }
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }



    </script>
    <script language="javascript" type="text/javascript">
        function Calculate(text) {


            if (isNaN(text.value)) {

                text.value = '0';
                return false;
            }
            var index = text.value.indexOf('.');
            var bdot = text.value.substring(0, index);
            if (index > 0) {

                decimalvalue = text.value.substring(index, (index + 3));
                text.value = bdot + decimalvalue;
            }


            var grd = document.getElementById("<%=dgvGoldItemDetails.ClientID %>");
            var ddlSchemeName = document.getElementById("<%=ddlSchemeName.ClientID %>");
            var txtLoanTenure = document.getElementById("<%=txtLoanTenure.ClientID %>");
            var txtMaxLoanAmount = document.getElementById("<%=txtMaxLoanAmount.ClientID %>");
            var txtEligibleLoan = document.getElementById("<%=txtEligibleLoan.ClientID %>");
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var txtNetPayable = document.getElementById("<%=txtNetPayable.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtDueDate = document.getElementById("<%=txtDueDate.ClientID %>");



            var TQuantity = 0;
            var TGross = 0;
            var TNet = 0;
            var TRate = 0;
            var Tvalue = 0;
            var Totalrate = 0;


            if (Totalrate == '') {

                Totalrate = 0;
            }

            for (i = 1; i < grd.rows.length - 1; i++) {

                var Quantity = grd.rows[i].cells[2].children[0];
                var Gross = grd.rows[i].cells[3].children[0];
                var Netwt = grd.rows[i].cells[4].children[0];
                var Rate = grd.rows[i].cells[6].children[0];
                var Value = grd.rows[i].cells[7].children[0];

                if (isNaN(Quantity.value)) {

                    Quantity.value = 0;
                }
                if (isNaN(Gross.value)) {

                    Gross.value = 0;
                }
                if (isNaN(Netwt.value)) {

                    Netwt.value = 0;
                }
                if (isNaN(Rate.value)) {

                    Rate.value = 0;
                }
                if (isNaN(Value.value)) {

                    Value.value = 0;
                }


                if (parseFloat(Gross.value) < parseFloat(Netwt.value)) {

                    alert('Net weight should not be greater than gross weight');
                    Netwt.value = 0;
                    return false;
                }
                if (Quantity.value == '') {

                    Quantity.value = 0;
                }
                if (Gross.value == '') {

                    Gross.value = 0;
                }
                if (Netwt.value == '') {

                    Netwt.value = 0;
                }
                if (Rate.value == '') {

                    Rate.value = 0;
                }
                if (Value.value == '') {

                    Value.value = 0;
                }



                Value.value = parseFloat(parseFloat(Netwt.value) * parseFloat(Rate.value)).toFixed(2);
                Tvalue = parseFloat(Tvalue) + parseFloat(Value.value);
                TQuantity = parseFloat(TQuantity) + parseFloat(Quantity.value);
                TGross = parseFloat(TGross) + parseFloat(Gross.value);
                TNet = parseFloat(TNet) + parseFloat(Netwt.value);

            }


            grd.rows[grd.rows.length - 1].cells[7].children[0].value = parseFloat(Tvalue).toFixed(2);
            grd.rows[grd.rows.length - 1].cells[2].children[0].value = parseInt(TQuantity);
            grd.rows[grd.rows.length - 1].cells[3].children[0].value = parseFloat(TGross).toFixed(2);
            grd.rows[grd.rows.length - 1].cells[4].children[0].value = parseFloat(TNet).toFixed(2);

            ddlSchemeName.selectedIndex = 0;
            txtLoanTenure.value = 0;
            txtMaxLoanAmount.value = 0;
            txtEligibleLoan.value = 0;
            txtNetAmountSanctioned.value = 0;
            txtEMI.value = 0;
            txtNetPayable.value = 0;

        }


        function Fixdecimal(text) {

            if (isNaN(text.value)) {
                text.value = '';
                return false;
            }
            var index = text.value.indexOf('.');
            var bdot = text.value.substring(0, index);
            if (index > 0) {

                decimalvalue = text.value.substring(index, (index + 3));
                text.value = bdot + decimalvalue;
            }

            var txtBank = document.getElementById("<%=txtBankAmount.ClientID %>");

            if (txtBank.value == '') {
                txtBank.value = 0;
            }

            var txtCash = document.getElementById("<%=txtCashAmount.ClientID %>");

            if (txtCash.value == '') {
                txtCash.value = 0;
            }
        }

        function valid() {

            var ddlLoanType = document.getElementById("<%=ddlLoanType.ClientID %>");
            var ddlCashAccount = document.getElementById("<%=ddlCashAccount.ClientID %>");
            var ddlcheqNEFTDD = document.getElementById("<%=ddlcheqNEFTDD.ClientID %>");
            var txtChequeNo = document.getElementById("<%=txtChequeNo.ClientID %>");
            var txtChequeDate = document.getElementById("<%=txtChequeDate.ClientID %>");
            var ddlSchemeName = document.getElementById("<%=ddlSchemeName.ClientID %>");
            var txtLoanTenure = document.getElementById("<%=txtLoanTenure.ClientID %>");
            var txtMaxLoanAmount = document.getElementById("<%=txtMaxLoanAmount.ClientID %>");
            var txtEligibleLoan = document.getElementById("<%=txtEligibleLoan.ClientID %>");
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var txtNetPayable = document.getElementById("<%=txtNetPayable.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtDueDate = document.getElementById("<%=txtDueDate.ClientID %>");
            var grd = document.getElementById("<%=dgvGoldItemDetails.ClientID %>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");
            var hdnscheme = document.getElementById("<%=hdnscheme.ClientID %>");
            var hdnacc = document.getElementById("<%=hdnacc.ClientID %>");
            var grddeno = document.getElementById("<%=gvDenominationDetails.ClientID %>");
            var hdnkycid = document.getElementById("<%=hdnkycid.ClientID %>");
            var ddlGoldInwardBy = document.getElementById("<%=ddlGoldInwardBy.ClientID %>");
            var txtRackno = document.getElementById("<%=txtRackno.ClientID %>");
            var ddlCashOutBy = document.getElementById("<%=ddlCashOutBy.ClientID %>");
            var lblOutTotal = document.getElementById("<%=lblOutTotal.ClientID %>");
            var txtCash = document.getElementById("<%=txtCashAmount.ClientID %>");

            var pattern = /^([0-9]{2})\/([0-9]{2})\/([0-9]{4})$/;

            var ddlPaymentMode = document.getElementById("<%=ddlPaymentMode.ClientID %>");
            var bankAmount = document.getElementById("<%=txtBankAmount.ClientID %>");
            var cashAmount = document.getElementById("<%=txtCashAmount.ClientID %>");

            if (ddlLoanType.selectedIndex == 0) {

                alert('Select Loan Type');
                return false;
            }
            if (hdnkycid.value == '0') {

                alert('Select Customer');
                return false;
            }
            if (hdnoperation.value == 'Save') {
                if (ddlLoanType.selectedIndex == 1) {

                    if (parseFloat(lblOutTotal.innerText) > 0) {

                        alert('Cannot Apply For New Loan, Since Outsatanding Of Previous Loan Is Pending');
                        return false;
                    }
                }
            }


            for (i = 1; i < grd.rows.length - 1; i++) {

                var Quantity = grd.rows[i].cells[2].children[0];
                var Gross = grd.rows[i].cells[3].children[0];
                var Netwt = grd.rows[i].cells[4].children[0];
                var Rate = grd.rows[i].cells[6].children[0];
                var Value = grd.rows[i].cells[7].children[0];

                if (Quantity.value == '' || parseFloat(Quantity.value) <= 0) {

                    alert('Enter Gold Item Quantity');
                    return false; grddeno.rows.length - 1
                }

                if (Gross.value == '' || parseFloat(Gross.value) <= 0) {

                    alert('Enter Gold Item Gross Weight');
                    return false;
                }
                if (Netwt.value == '' || parseFloat(Netwt.value) <= 0) {

                    alert('Enter Gold Item Net Weight');
                    return false;
                }
                if (Rate.value == '' || parseFloat(Rate.value) <= 0) {

                    alert('Enter Gold Item Rate Per Gram');
                    return false;
                }
                if (Value.value == '' || parseFloat(Value.value) <= 0) {

                    alert('Enter Gold Item Value');
                    return false;
                }
            }


            //            if (parseFloat(txtNetAmountSanctioned.value) > parseFloat(parseFloat(txtEligibleLoan.Text) + parseFloat(lblOutTotal.Text))) {
            //                var Msg = "Sanction amount should not be greater than " + parseFloat(parseFloat(txtEligibleLoan.Text) + parseFloat(lblOutTotal.Text));
            //                alert(Msg);
            //                //alert('Sanction amount should not be greater than eligible amount');
            //                return false;
            //            }

            if (ddlSchemeName.selectedIndex == 0) {

                alert('Select Scheme');
                return false;
            }
            if (parseFloat(txtLoanTenure.value) <= 0 || txtLoanTenure.value == '') {

                alert('Loan Tenure Should Not Be 0 or Blank');
                return false;
            }
            if (parseFloat(txtMaxLoanAmount.value) <= 0 || txtMaxLoanAmount.value == '') {

                alert('Max Loan Should Not Be 0 or Blank');
                return false;
            }
            if (parseFloat(txtEligibleLoan.value) <= 0 || txtEligibleLoan.value == '') {

                alert('Eligible Loan Should Not Be 0 or Blank');
                return false;
            }
            if (parseFloat(txtNetAmountSanctioned.value) <= 0 || txtNetAmountSanctioned.value == '') {

                alert('Sanction Amount Should Not Be 0 or Blank');
                return false;
            }

            if (parseFloat(txtNetAmountSanctioned.value) > parseFloat(txtEligibleLoan.value)) {

                alert('Sanction Amount Should Not Be Greater Than Eligible Loan Amount');
                return false;
            }

            if (parseFloat(txtNetPayable.value) <= 0 || txtNetPayable.value == '') {

                alert('Sanction Amount Should Not Be 0 or Blank');
                return false;
            }

            if (txtDueDate.value == '') {

                alert('Enter Re Payment Date');
                return false;
            }
            if (!pattern.test(txtDueDate.value)) {

                alert('Re Payment Date Not In Correct Format');
                return false;
            }

            if (ddlPaymentMode.selectedIndex != 0) {
                if (ddlPaymentMode.selectedIndex == 3) {
                    if (ddlBankAccount.selectedIndex == 0) {
                        alert('Select Bank A/c');
                        return false;
                    }
                    if (ddlcheqNEFTDD.selectedIndex == 0) {
                        alert('Select Chq/DD/NEFT');
                        return false;
                    }
                    if (txtChequeNo.value == '') {
                        alert('Enter Chq/DD/NEFT No');
                        return false;
                    }
                    if (ddlcheqNEFTDD.value == 'DD' && txtChequeNo.value.length < 6) {
                        alert('Enter minimum 6 character');
                        return false;
                    }
                    if (txtChequeDate.value == '') {
                        alert('Enter Chq/DD/NEFT Date');
                        return false;
                    }
                    if (!pattern.test(txtChequeDate.value)) {
                        alert('Chq/DD/NEFT Date Not In Correct Format');
                        return false;
                    }
                    if (bankAmount.value == "") {
                        alert('Enter Bank Amount');
                        return false;
                    }
                    if (ddlCashAccount.selectedIndex == 0) {
                        alert('Select Cash A/c');
                        return false;
                    }
                    if (cashAmount.value == "") {
                        alert('Enter Cash Amount');
                        return false;
                    }
                }
                else if (ddlPaymentMode.selectedIndex == 2) {

                    if (ddlCashAccount.selectedIndex == 0) {
                        alert('Select Cash A/c');
                        return false;
                    }
                    if (cashAmount.value == "") {
                        alert('Enter Cash Amount');
                        return false;
                    }

                    if (ddlCashOutBy.selectedIndex == 0) {
                        // ddlCashOutBy.enabled = true;
                        alert('Select Cash Outward By');
                        return false;

                        for (i = 1; i < grddeno.rows.length - 1; i++) {

                            var DenoRs = grddeno.rows[i].cells[1].children[0];
                            var No = grddeno.rows[i].cells[2].children[0];
                            var Total = grddeno.rows[i].cells[3].children[0];


                            if (DenoRs.value == '') {

                                alert('Enter Denomination Rs.');
                                return false;
                            }
                            if (parseFloat(DenoRs.value) <= 0) {

                                alert('Enter Correct Denomination Rs.');
                                return false;
                            }

                            if (No.value == '') {

                                alert('Enter No.(Quantity)');
                                return false;
                            }
                            if (parseFloat(No.value) <= 0) {

                                alert('Enter Correct No.(Quantity)');
                                return false;
                            }
                        }
                    }

                    //                if (parseFloat(txtNetPayable.value) != parseFloat(grddeno.rows[grddeno.rows.length - 1].cells[3].children[0].value)) {
                    //                    alert('Denomination Total Amount Should Be Equal To Net Payable');
                    //                    return false;
                    //                }

                    //Added By Priya for cash total
                    if (parseFloat(txtNetPayable.value) != parseFloat(txtCash.value)) {
                        // alert('Cash Total Amount Should Be Equal To Net Payable');
                        // return false;
                    }

                    if (parseFloat(grddeno.rows[grddeno.rows.length - 1].cells[3].children[0].value) > parseFloat(txtCash.value)) {
                        alert('Denomination Total Amount Should Be Equal To Cash Total');
                        return false;
                    }


                }
                else if (ddlPaymentMode.selectedIndex == 1) {
                    if (ddlBankAccount.selectedIndex == 0) {
                        alert('Select Bank A/c');
                        return false;
                    }
                    if (ddlcheqNEFTDD.selectedIndex == 0) {
                        alert('Select Chq/DD/NEFT');
                        return false;
                    }
                    if (txtChequeNo.value == '') {
                        alert('Enter Chq/DD/NEFT No');
                        return false;
                    }
                    if (ddlcheqNEFTDD.value == 'DD' && txtChequeNo.value.length < 6) {
                        alert('Enter minimum 6 character');
                        return false;
                    }
                    if (txtChequeDate.value == '') {
                        alert('Enter Chq/DD/NEFT Date');
                        return false;
                    }
                    if (!pattern.test(txtChequeDate.value)) {
                        alert('Chq/DD/NEFT Date Not In Correct Format');
                        return false;
                    }
                    if (bankAmount.value == "") {
                        alert('Enter Bank Amount');
                        return false;
                    }
                }

            }
            else {
                alert('Select Payment Mode');
                return false;
            }


            if (ddlGoldInwardBy.selectedIndex == 0) {
                alert('Select Gold Inward By');
                return false;
            }
            if (txtRackno.value == '') {
                alert('Enter Gold Inward Rack No');
                return false;
            }


            CalDeno();

            var NetPaybleAmt = document.getElementById("<%=txtNetPayable.ClientID %>");
            var totalBankAmt = document.getElementById("<%=txtBankAmount.ClientID %>");
            var totalCashAmt = document.getElementById("<%=txtCashAmount.ClientID %>");

            var totalDenoDet = 0;
            totalDenoDet = totalamt;


            var totalBankCashAmt = 0;

            if (totalBankAmt.value == "" || totalBankAmt.value == null) {
                totalBankAmt.value = 0;
            }
            if (totalCashAmt.value == "" || totalCashAmt.value == null) {
                totalCashAmt.value = 0;
            }

            totalBankCashAmt = (parseFloat(totalBankAmt.value) + parseFloat(totalCashAmt.value));

            // if (totalCashAmt.value > 99999) {
            //    
            // alert('Cash Amount Exceeds Limit Do You Want to go to Authorized Login??');
            //ShowPopup('Cash Amount Exceeds Limit Do You Want to go to Authorized Login??', 'Info');
            // totalCashAmt.value = "";
            //  return false;
            // }

            if (ddlPaymentMode.selectedIndex == 2 || ddlPaymentMode.selectedIndex == 3) {
                //if (totalCashAmt.value != totalDenoDet) {
                //    alert('Cash Amount And Denomination Total Amount should be Same.');
                //    totalDenoDet.value = "";
                //    return false;
                //}

                //Added By Priya for cash total
                if (parseFloat(grddeno.rows[grddeno.rows.length - 1].cells[3].children[0].value) > parseFloat(txtCash.value)) {
                    alert('Denomination Total Amount Should Be Equal To Cash Total');
                    return false;
                }
            }
            if (ddlPaymentMode.selectedIndex == 1 || ddlPaymentMode.selectedIndex == 3) {
                if (totalBankCashAmt != parseFloat(NetPaybleAmt.value)) {
                    alert('Net Payable And Total Amount should be same!!');
                    return false;
                }
            }
        }

        function schemclick() {
            var btnScheme = document.getElementById("<%=btnScheme.ClientID %>");
            var ddlSchemeName = document.getElementById("<%=ddlSchemeName.ClientID %>");
            var grd = document.getElementById("<%=dgvGoldItemDetails.ClientID %>");
            var txtEligibleLoan = document.getElementById("<%=txtEligibleLoan.ClientID %>");
            var txtLoanTenure = document.getElementById("<%=txtLoanTenure.ClientID %>");
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var txtMaxLoanAmount = document.getElementById("<%=txtMaxLoanAmount.ClientID %>");
            var txtEMI = document.getElementById("<%=txtEMI.ClientID %>");
            var txtNetPayable = document.getElementById("<%=txtNetPayable.ClientID %>");
            var Tvalue = grd.rows[grd.rows.length - 1].cells[7].children[0];

            if (ddlSchemeName.selectedIndex == 0) {

                txtEligibleLoan.value = 0;
                txtLoanTenure.value = 0;
                txtNetAmountSanctioned.value = 0;
                txtMaxLoanAmount.value = 0;
                txtEMI.value = 0;
                txtNetPayable.value = 0;
                btnScheme.click();
                return false;
            }
            if (Tvalue.value == '') {

                alert('Enter Total Value');
                ddlSchemeName.selectedIndex = 0;
                return false;
            }
            if (parseFloat(Tvalue.value) <= 0) {

                alert('Total Value Must Be Greater Than 0');
                ddlSchemeName.selectedIndex = 0;
                return false;
            }

            //   document.getElementById("<%=txtNetPayable.ClientID %>").value() = txtNetPayable.value;

            btnScheme.click(); NetPayable();


        }

        function Charges() {

            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");

            if (txtNetAmountSanctioned.value == '') {

                alert('Enter Sanction Loan Amount');
                return false;
            }
            if (parseFloat(txtNetAmountSanctioned.value) <= 0) {

                alert('Sanction Loan Amount Should Be More Than 0');
                return false;
            }
        }
        function AddDeno() {

            var grd = document.getElementById("<%=gvDenominationDetails.ClientID %>");
            for (i = 1; i < grd.rows.length - 1; i++) {

                var DenoRs = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var Total = grd.rows[i].cells[3].children[0];


                if (DenoRs.value == '') {

                    alert('Enter Denomination Rs.');
                    return false;
                }
                if (parseFloat(DenoRs.value) <= 0) {

                    alert('Enter Correct Denomination Rs.');
                    return false;
                }

                if (No.value == '') {

                    alert('Enter No.(Quantity)');
                    return false;
                }
                if (parseFloat(No.value) <= 0) {

                    alert('Enter Correct No.(Quantity)');
                    return false;
                }

                if (isNaN(DenoRs.value)) {
                    DenoRs.value = 0;
                    return false;
                }
                if (isNaN(No.value)) {
                    No.value = 0;
                    return false;
                }
                if (isNaN(Total.value)) {
                    Total.value = 0;
                    return false;
                }

            }
        }
        function CalDeno() {

            totalamt = 0;
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var grd = document.getElementById("<%=gvDenominationDetails.ClientID %>");

            for (i = 1; i < grd.rows.length - 1; i++) {

                var DenoRs = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var Total = grd.rows[i].cells[3].children[0];


                if (DenoRs.value == '') {
                    DenoRs.value = 0;
                }
                if (No.value == '') {
                    No.value = 0;
                }

                Total.value = parseFloat(DenoRs.value) * parseFloat(No.value);
                totalamt = parseFloat(totalamt) + parseFloat(Total.value);
            }
            grd.rows[grd.rows.length - 1].cells[3].children[0].value = totalamt;
            return totalamt;
        }

        function CalDeno1() {

            totalamt = 0;
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var grd = document.getElementById("<%=gvDenominationDetails.ClientID %>");

            for (i = 1; i < grd.rows.length - 1; i++) {

                var DenoRs = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var Total = grd.rows[i].cells[3].children[0];


                if (DenoRs.value == '') {
                    DenoRs.value = 0;
                }
                if (No.value == '') {
                    No.value = 0;
                }

                Total.value = parseFloat(DenoRs.value) * parseFloat(No.value);
                totalamt = round(parseFloat(totalamt) + parseFloat(Total.value));
            }
            grd.rows[grd.rows.length - 1].cells[3].children[0].value = totalamt;
            return totalamt;
        }
        function validcus() {

            var ddlLoanType = document.getElementById("<%=ddlLoanType.ClientID %>");

            if (ddlLoanType.selectedIndex == 0) {

                alert('Select Loan Type');
                return false;
            }
        }

        function NetPayable() {

            var ddlLoanType = document.getElementById("<%=ddlLoanType.ClientID %>");
            var hdnProType = document.getElementById("<%=hdnProType.ClientID %>");
            var hdnProcharge = document.getElementById("<%=hdnProcharge.ClientID %>");
            var hdnservicetax = document.getElementById("<%=hdnservicetax.ClientID %>");
            var txtEligibleLoan = document.getElementById("<%=txtEligibleLoan.ClientID %>");
            var txtNetAmountSanctioned = document.getElementById("<%=txtNetAmountSanctioned.ClientID %>");
            var txtNetPayable = document.getElementById("<%=txtNetPayable.ClientID %>");
            var ddlSchemeName = document.getElementById("<%=ddlSchemeName.ClientID %>");
            var txtTotalChargesAmount = document.getElementById("<%=txtTotalChargesAmount.ClientID %>");
            var hdneligible = document.getElementById("<%=hdneligible.ClientID %>");
            var hdnsanction = document.getElementById("<%=hdnsanction.ClientID %>");
            var hdnnetpayable = document.getElementById("<%=hdnnetpayable.ClientID %>");
            var lblOutTotal = document.getElementById("<%=lblOutTotal.ClientID %>").innerHTML;
            var hdnproclimit = document.getElementById("<%=hdnproclimit.ClientID %>");
            var amount = 0;
            var netpaid = 0;
            var tax = 0;


            if (ddlSchemeName.selectedIndex == 0) {

                alert('Select Scheme');
                return false;
            }

            if (isNaN(txtNetAmountSanctioned.value)) {

                txtNetAmountSanctioned.value = 0;
            }
            if (isNaN(txtNetPayable.value)) {

                txtNetPayable.value = 0;
            }

            if (ddlLoanType.selectedIndex == 1) {
                if (hdnProType.value == 'Percentage') {

                    netpaid = parseFloat(txtNetAmountSanctioned.value) * parseFloat(hdnProcharge.value) / 100;
                    if (parseFloat(hdnproclimit.value) > netpaid) {
                        amount = parseFloat(netpaid) + parseFloat(netpaid) * parseFloat(hdnservicetax.value) / 100;
                    }
                    else {
                        amount = parseFloat(hdnproclimit.value) + parseFloat(hdnproclimit.value) * parseFloat(hdnservicetax.value) / 100;
                    }
                }
                else {
                    if (parseFloat(hdnproclimit.value) > netpaid) {
                        netpaid = parseFloat(hdnProcharge.value);
                        amount = parseFloat(netpaid) + parseFloat(netpaid) * parseFloat(hdnservicetax.value) / 100;
                    }
                    else {
                        amount = parseFloat(hdnproclimit.value) + parseFloat(hdnproclimit.value) * parseFloat(hdnservicetax.value) / 100;
                    }
                }
                amount = parseFloat(amount);

                txtNetPayable.value = parseFloat(txtNetAmountSanctioned.value) - (parseFloat(amount) + parseFloat(txtTotalChargesAmount.value));
                newnetpayable = parseFloat(txtNetPayable.value);
                txtNetPayable.value = newnetpayable.toFixed(2)
                txtNetPayable.value = Math.round(txtNetPayable.value)


                if (txtNetPayable.value == 'NaN') {
                    txtNetPayable.value = 0;
                }
            }
            if (ddlLoanType.selectedIndex == 2) {

                netpaid = parseFloat(txtNetAmountSanctioned.value) - parseFloat(lblOutTotal);
                newnetpayable = parseFloat(netpaid);
                txtNetPayable.value = newnetpayable.toFixed(2)

            }
        }
    </script>
    <asp:UpdatePanel ID="UpdatePanel5" runat="server">
        <ContentTemplate>
            <asp:Button ID="btnScheme" runat="server" Style="display: none;" Text="Button" OnClick="btnScheme_Click" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <asp:Label ID="lbltest" runat="server" Text="Label" Visible="false"></asp:Label>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnimgpath" runat="server" Value="0" />
    <asp:HiddenField ID="hndImage" runat="server" Value="0" />
    <asp:HiddenField ID="hdnscheme" runat="server" Value="0" />
    <%--<asp:UpdatePanel ID="UpdatePanel13" runat="server" UpdateMode="Conditional">
        <ContentTemplate>--%>
    <asp:HiddenField ID="hdnacc" runat="server" Value="0" />
    <%--  </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlCashAccount" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>--%>
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Edit" />
    <%--for mobile no to send sms--%>
    <asp:HiddenField ID="hdnmobileno" runat="server" Value="0" />
    <asp:HiddenField ID="hdnsanctiondate" runat="server" Value="0" />
    <asp:HiddenField ID="hdnareaid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnzoneid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnkycid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoldgoldloanno" runat="server" Value="0" />
    <asp:HiddenField ID="HiddenFieldName" runat="server" />
    <asp:HiddenField ID="HiddenFieldPwd" runat="server" />
    <asp:HiddenField ID="HiddenFieldRefNo" runat="server" />
    <asp:HiddenField ID="HiddenFieldTotalCash" runat="server" />
    <input id="txtAccGPID" type="hidden" runat="server" />
    <asp:HiddenField ID="HiddenFieldGoldNo" runat="server" />
    <input id="HiddenName" type="hidden" />
    <input id="HiddenPwd" type="hidden" />



    <asp:TabContainer ID="TabContainer1" runat="server" OnDemand="True">
        <asp:TabPanel ID="panel1" runat="server" HeaderText="GOLDLOAN S & D Details">
            <ContentTemplate>

                <asp:TextBox ID="txtimageitem" runat="server" Visible="False"></asp:TextBox>
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
                    <tr>
                        <td style="width: 20%;"></td>
                        <td style="width: 24.5%;"></td>
                        <td style="width: 25%;"></td>
                        <td style="width: 24.5%;"></td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                                <tr>
                                    <td colspan="4">&nbsp;
                                    </td>
                                </tr>
                                <!--Header -->
                                <tr>
                                    <td align="center" colspan="4" class="header">
                                        <asp:Label ID="lblHeader" runat="server" Text="GOLD LOAN S&D Details"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">
                                        <div class="barstyle">
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="4">&nbsp;
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <!--Form Design -->
                    <tr>
                        <td class="label" style="text-align: left;">Loan Type<b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:DropDownList ID="ddlLoanType" runat="server" CssClass="textbox" Height="27px"
                                Width="95%" OnSelectedIndexChanged="ddlLoanType_SelectedIndexChanged" AutoPostBack="True">
                                <asp:ListItem Value="0">--Select Loan Type--</asp:ListItem>
                                <asp:ListItem Value="New">New</asp:ListItem>
                                <asp:ListItem Value="Topup">Top up</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Customer Photo
                        </td>
                        <td rowspan="2">
                            <asp:Image ID="ImgAppPhoto" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" />
                        </td>
                    </tr>
                    <tr>
                        <!-- Gold Loan No -->
                        <td class="label" style="text-align: left;">Customer ID<b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtCustomerID" class="textbox_readonly" Width="80%" ReadOnly="True"
                                MaxLength="40" runat="server"></asp:TextBox>
                            <asp:ImageButton ID="imgbtnCustomer" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                Width="20px" runat="server" ImageAlign="AbsMiddle" ToolTip="Click for search customer"
                                OnClick="imgbtnCustomer_Click" OnClientClick="return validcus();" />
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Gold Loan No.
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtGoldLoanNo" class="textbox_readonly" Width="90%" MaxLength="40"
                                runat="server"></asp:TextBox>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Customer Signature
                        </td>
                        <td rowspan="2">
                            <asp:Image ID="ImgAppSign" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Loan Date
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtLoanDate" CssClass="textbox_readonly " Width="80%" runat="server"
                                MaxLength="12" onchange="GetGolLoanNo()"></asp:TextBox>
                            <asp:CalendarExtender ID="txtRecvDate_CalendarExtender" runat="server" CssClass="Calenderstyle"
                                Enabled="True" PopupButtonID="btnImgCalender2" TargetControlID="txtLoanDate"
                                Format="dd/MM/yyyy">
                            </asp:CalendarExtender>
                            <asp:ImageButton ID="btnImgCalender2" runat="server" ImageAlign="Middle"
                                ImageUrl="~/images/calenderIMG.jpg" Width="15px" Height="15px" />
                        </td>
                        <td></td>
                    </tr>
                    <tr>
                        <!-- Operator Name -->
                        <td class="label" style="text-align: left; height: 17px;">Operator Name
                        </td>
                        <td class="txt_style" colspan="3" style="height: 17px">
                            <asp:TextBox ID="txtOperatorName" class="textbox_readonly " ReadOnly="True" Width="96.5%"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <!-- Customer Name -->
                        <td class="label" style="text-align: left; height: 47px;">Customer Name
                        </td>
                        <td class="txt_style" colspan="3" style="height: 47px">
                            <asp:TextBox ID="txtCustomerName" class="textbox_readonly " ReadOnly="True" Width="96.5%"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Gender
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtGender" class="textbox_readonly" runat="server" Width="90%"></asp:TextBox>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Marital Status
                        </td>
                        <td>
                            <asp:TextBox ID="txtMaritalStatus" class="textbox_readonly" runat="server" Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Birth Date
                        </td>
                        <td>
                            <asp:TextBox ID="txtBirthDate" class="textbox_readonly" runat="server" Width="90%"></asp:TextBox>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Age
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtAge" class="textbox_readonly" ReadOnly="True" runat="server"
                                Style="text-align: left;" Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <!-- Pan No -->
                        <td class="label" style="text-align: left;">PAN No.
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtPanNo" class="textbox_readonly" runat="server" Width="90%"></asp:TextBox>
                        </td>
                        <!-- Gender -->
                        <td class="label" style="text-align: left; padding-left: 20px;">Existing PL-Case No.
                        </td>
                        <td>
                            <asp:TextBox ID="txtPLCaseNo" class="textbox_readonly" ReadOnly="True" runat="server"
                                Width="90%"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <!-- Address -->
                        <td class="label" style="text-align: left;">Customer's Address
                        </td>
                        <td class="txt_style" colspan="3">
                            <asp:TextBox ID="txtAddress" class="textbox_readonly" Height="40px" ReadOnly="True"
                                Width="96.5%" runat="server" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Nominee Name
                        </td>
                        <td colspan="3" class="txt_style">
                            <asp:TextBox ID="txtNominee" class="textbox_readonly" ReadOnly="True" Width="96.5%"
                                runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Nominee Relationship
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtNomineeRelationship" class="textbox_readonly" ReadOnly="True"
                                Width="90%" runat="server"></asp:TextBox>
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <!-- Gold Item Details Section -->
                        <td colspan="4" class="label" style="text-align: left; text-decoration: underline; font-weight: bold;">
                            <br />
                            Gold Item Details :-
                        </td>
                    </tr>
                    <tr>
                        <!-- Gold Item Details Section -->
                        <td colspan="4">
                            <div>
                                <!--GridView -->
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="dgvGoldItemDetails" runat="server" DataKeyNames="GID" ShowFooter="true"
                                            Width="90%" AutoGenerateColumns="False" OnRowDataBound="dgvGoldItemDetails_RowDataBound">
                                            <AlternatingRowStyle BackColor="White" />
                                            <HeaderStyle CssClass="gVHeader" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%#Container.DataItemIndex+1%>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="GID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblGID" align="center" runat="server" Text='<%# Eval("GID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="SDID" Visible="false">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSDID" align="center" runat="server" Text='<%# Eval("SDID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="ItemID" ItemStyle-HorizontalAlign="Left" Visible="false">
                                                    <FooterTemplate>
                                                    </FooterTemplate>
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblItemID" runat="server" Text='<%# Eval("ItemID") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Gold Item Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlGoldItemName" runat="server" CssClass="textbox" Height="27px"
                                                            DataSourceID="SqlDataSource2" DataTextField="ItemName" DataValueField="ItemID"
                                                            Width="180px">
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                                                            SelectCommand="SELECT distinct ItemID, ItemName FROM tblItemMaster ORDER BY ItemName"></asp:SqlDataSource>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Label ID="lblTitle" Font-Bold="true" Font-Size="15px" runat="server" Text="Total"></asp:Label>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Quantity">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtQuantity" runat="server" MaxLength="3" CssClass="textbox" Style="text-align: center;"
                                                            Width="50px" Text='<%# Eval("Quantity") %>' onkeypress="return OnlyNumericEntry();"
                                                            onkeyup="Calculate(this);"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtTotalQuantity" CssClass="textbox_readonly" Style="text-align: center; color: Black;"
                                                            onkeyup="Calculate(this);" MaxLength="5" runat="server" Width="50px"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Gross Wt.(g)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtGrossWeight" runat="server" MaxLength="8" CssClass="textbox"
                                                            Style="text-align: center;" Width="80px" Text='<%# Eval("GrossWeight") %>' onkeyup="Calculate(this);"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtTotalGrossWeight" runat="server" MaxLength="8" CssClass="textbox_readonly"
                                                            Style="text-align: center; color: Black;" onkeyup="Calculate(this);" Width="80px"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Net Wt.(g)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtNetWeight" runat="server" MaxLength="8" CssClass="textbox" Style="text-align: center;"
                                                            Text='<%# Eval("NetWeight") %>' Width="80px" onkeyup="Calculate(this);"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtTotalNetWeight" CssClass="textbox_readonly" Style="text-align: center; color: Black;"
                                                            onkeyup="Calculate(this);" MaxLength="8" runat="server" Width="80px"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Karat">
                                                    <ItemTemplate>
                                                        <asp:HiddenField ID="hdnkarat" runat="server" Value='<%#Eval("Purity") %>' />
                                                        <asp:DropDownList ID="ddlKarat" Width="54px" runat="server" CssClass="textbox" Height="27px">
                                                            <asp:ListItem>18K</asp:ListItem>
                                                            <asp:ListItem>20K</asp:ListItem>
                                                            <asp:ListItem>21K</asp:ListItem>
                                                            <asp:ListItem>22K</asp:ListItem>
                                                            <asp:ListItem>23K</asp:ListItem>
                                                            <asp:ListItem>24K</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Rate Per Gram">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtRatePerGram" runat="server" MaxLength="8" CssClass="textbox"
                                                            Style="text-align: center;" Text='<%# Eval("RateperGram") %>' Width="70px" onkeyup="Calculate(this);"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Value">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtValue" runat="server" CssClass="textbox_readonly" Style="text-align: center; color: Black;"
                                                            Text='<%# Eval("Value") %>' Width="70px" onkeyup="Calculate(this);"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtTotalValue" CssClass="textbox_readonly" Style="text-align: center; color: Black;"
                                                            onkeyup="Calculate(this);" MaxLength="8" runat="server" Width="70px"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" Visible="true">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                            OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                            Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                            OnClick="btnDelete_Click" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Button ID="BtnUpload" runat="server" Text="Add" OnClick="BtnUpload_Click" ValidationGroup="Upload" />
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Center" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="dgvGoldItemDetails" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" class="label" style="text-align: left; text-decoration: underline; font-weight: bold;">
                            <br />
                            Scheme Details :-
                        </td>
                    </tr>
                    <tr>
                        <!-- Total Gross Weight -->
                        <td class="label" style="text-align: left;">Scheme Name <b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel16" runat="server">
                                <ContentTemplate>
                                    <asp:HiddenField ID="hdnProType" runat="server" />
                                    <asp:HiddenField ID="hdnProcharge" runat="server" />
                                    <asp:HiddenField ID="hdnproclimit" runat="server" />
                                    <asp:HiddenField ID="hdnservicetax" runat="server" />
                                    <asp:DropDownList ID="ddlSchemeName" runat="server" CssClass="textbox" Width="95%"
                                        Height="27px" onchange="return schemclick();" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="dgvGoldItemDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <!-- Scheme Name-->
                        <td class="label" style="text-align: left; padding-left: 20px;">
                            <asp:HiddenField ID="hdneligible" runat="server" />
                            Eligible Loan Amount
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtEligibleLoan" class="textbox_readonly" onkeypress="return isAlphaNumCharsDot(event);"
                                        runat="server" Width="90%" Style="text-align: right"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Tenure
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtLoanTenure" class="textbox textbox_readonly" MaxLength="3" Width="90%"
                                        onkeypress="return OnlyNumericEntry();" runat="server" Style="text-align: right"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">
                            <asp:HiddenField ID="hdnsanction" runat="server" />
                            Sanction Loan Amount<b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel15" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtNetAmountSanctioned" class="textbox" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                                        runat="server" Width="90%" Style="text-align: right" onkeyup="return NetPayable();"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                    <asp:AsyncPostBackTrigger ControlID="gvDenominationDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Max Loan Amount
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtMaxLoanAmount" class="textbox_readonly" Width="90%" onkeypress="return isAlphaNumCharsDot(event);"
                                        runat="server" Style="text-align: right"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">
                            <asp:HiddenField ID="hdnnetpayable" runat="server" />
                            Net Payable
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel8" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtNetPayable" class="textbox textbox_readonly" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                                        runat="server" Width="90%" Style="text-align: right"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">EMI
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel10" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtEMI" class="textbox_readonly" Width="90%" onkeypress="return isAlphaNumCharsDot(event);"
                                        runat="server" Style="text-align: right"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Interest Repayment Date
                        </td>
                        <td class="txt_style">
                            <asp:UpdatePanel ID="UpdatePanel7" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtDueDate" class="textbox" runat="server" Width="90%"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;" valign="top">Outstanding Details
                        </td>
                        <td rowspan="3" class="txt_style">
                            <table cellpadding="0" cellspacing="0" border="0" style="width: 96%; border-style: solid; border-width: 1px;"
                                class="RoundedPanel textbox">
                                <tr class="txt_style" style="background-color: #faf4b3;">
                                    <td class="label" style="text-align: left;">Principal
                                    </td>
                                    <td class="label" style="text-align: right;">
                                        <asp:UpdatePanel ID="UpdatePanel17" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="lblOutPrincipal" runat="server" Text="0"></asp:Label>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr class="txt_style" style="background-color: #faf4b3;">
                                    <td class="label" style="text-align: left;">Interest
                                    </td>
                                    <td class="label" style="text-align: right;">
                                        <asp:UpdatePanel ID="UpdatePanel18" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="lblOutInterest" runat="server" Text="0"></asp:Label>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr class="txt_style" style="background-color: #faf4b3;">
                                    <td class="label" style="text-align: left;">Penal Interest
                                    </td>
                                    <td class="label" style="text-align: right;">
                                        <asp:UpdatePanel ID="UpdatePanel19" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="lblOutPInterest" runat="server" Text="0"></asp:Label>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr class="txt_style" style="background-color: #faf4b3;">
                                    <td class="label" style="text-align: left;">Charges
                                    </td>
                                    <td class="label" style="text-align: right;">
                                        <asp:UpdatePanel ID="UpdatePanel20" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="lblOutCharges" runat="server" Text="0"></asp:Label>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                                <tr class="txt_style" style="background-color: #faf4b3;">
                                    <td class="label" style="text-align: left;">Total
                                    </td>
                                    <td class="label" style="text-align: right;">
                                        <asp:UpdatePanel ID="UpdatePanel21" runat="server">
                                            <ContentTemplate>
                                                <asp:Label ID="lblOutTotal" runat="server" Text="0"></asp:Label>
                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="btnScheme" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Gold Item Image
                        </td>
                        <td rowspan="2">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Image ID="ImgItems" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                            BorderWidth="1px" ImageAlign="AbsMiddle" />
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:FileUpload ID="fUItems" runat="server" Width="85px" />
                                        <asp:Button ID="btnItemsUpload" runat="server" Text="Upload" OnClick="btnItemsUpload_Click"
                                            Width="84px" />
                                        <asp:ImageButton ID="ImageButton1" runat="server" AlternateText="Remove Copy" ImageAlign="Top"
                                            ImageUrl="~/images/remove_button.png" Height="27px" Width="85px" OnClick="btnRemoveProof_Click"
                                            Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <!-- Net Weight-->
                        <td class="label" style="text-align: left;"></td>
                        <!-- Max Loan Amount (Allowed)-->
                        <td class="label" valign="top"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <!-- Sanctioned Loan Amt-->
                        <td class="label"></td>
                        <!-- Login For excess loan amount-->
                        <!-- Proof of Ownership-->
                        <td class="label" style="text-align: left; padding-left: 20px;">Proof of Ownership
                <br />
                            <cite style="font-size: 10.5px;">(If gross weight more than 20g)</cite>
                        </td>
                        <td class="txt_style" rowspan="2" valign="top">
                            <table>
                                <tr>
                                    <td>
                                        <asp:Image ID="imgProofOfOwnership" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                            BorderWidth="1px" ImageAlign="AbsMiddle" />
                                    </td>
                                    <td valign="top" align="left">
                                        <asp:FileUpload ID="fUploadProofOfOwnership" runat="server" Width="85px" />
                                        <br />
                                        <asp:Button ID="btnUploadProofOfOwnership" runat="server" Text="Upload" OnClick="btnUploadProofOfOwnership_Click"
                                            Width="84px" />
                                        <asp:ImageButton ID="btnRemoveProof" runat="server" AlternateText="Remove Copy" ImageAlign="Top"
                                            ImageUrl="~/images/remove_button.png" Height="27px" Width="85px" OnClick="btnRemoveProof_Click"
                                            Visible="False" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td colspan="4" class="label" style="text-align: left;"></td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Payment Mode<b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style" colspan="3">
                            <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddlPaymentMode" class="textbox" Height="27px" Width="98.5%"
                                        runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPaymentMode_SelectedIndexChanged">
                                        <asp:ListItem Value="0" Text="--Select Mode--"></asp:ListItem>
                                        <asp:ListItem Value="Chq/DD/NEFT" Text="Chq/DD/NEFT"></asp:ListItem>
                                        <asp:ListItem Value="Cash" Text="Cash"></asp:ListItem>
                                        <asp:ListItem Value="Both" Text="Both"></asp:ListItem>
                                    </asp:DropDownList>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <asp:Panel ID="pnlBankAc" Style="width: 100%;" runat="server">
                        <tr>
                            <!-- Bank/Cash Account -->
                            <td class="label" style="text-align: left;">Bank A/c<b style="color: Red;">*</b>
                            </td>
                            <td class="txt_style" colspan="3">
                                <asp:UpdatePanel ID="UpdatePanelBankAc" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlBankAccount" runat="server" Enabled="true" CssClass="textbox"
                                            AppendDataBoundItems="True" Height="27px" Width="98.5%" ClientIDMode="Static"
                                            OnSelectedIndexChanged="ddlBankAccount_SelectedIndexChanged" AutoPostBack="True">
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlPaymentMode" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left;">Chq/DD/NEFT
                            </td>
                            <td class="txt_style" colspan="3">
                                <asp:UpdatePanel ID="UpdatePanel22" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlcheqNEFTDD" runat="server" CssClass="textbox" Height="27px"
                                            Width="98.5%">
                                            <asp:ListItem>---Select ---</asp:ListItem>
                                            <asp:ListItem>Cheque</asp:ListItem>
                                            <asp:ListItem>NEFT</asp:ListItem>
                                            <asp:ListItem>DD</asp:ListItem>
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlBankAccount" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <!-- Cheque Number -->
                            <td class="label" style="text-align: left;">Chq/DD/NEFT No.
                            </td>
                            <td class="txt_style">
                                <asp:UpdatePanel ID="UpdatePanel111" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtChequeNo" class="textbox" runat="server" Style="text-align: right;"
                                            Width="90%" onkeypress="return isAlphaNumChars();" MaxLength="15"></asp:TextBox>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlBankAccount" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <!-- Cheque Date -->
                            <td class="label" style="text-align: left; padding-left: 20px;">Chq/DD/NEFT Date
                            </td>
                            <td class="txt_style">
                                <asp:UpdatePanel ID="UpdatePanel112" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtChequeDate" class="textbox" runat="server" Width="82%" MaxLength="10"
                                            onchange="ValidateDate(this, event.keyCode)"></asp:TextBox>
                                        <asp:CalendarExtender ID="txtChequeDate_CalendarExtender" runat="server" Format="dd/MM/yyyy"
                                            PopupButtonID="btnImgCalender" TargetControlID="txtChequeDate" CssClass="Calenderstyle">
                                        </asp:CalendarExtender>
                                        <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                                            Width="15" Height="15" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlBankAccount" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left;">Amount
                            </td>
                            <td class="txt_style">
                                <asp:UpdatePanel ID="UpdatePanel13" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtBankAmount" class="textbox" runat="server" Style="text-align: right;"
                                            Width="90%" onkeyup="return Fixdecimal(this);" MaxLength="8"></asp:TextBox>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlBankAccount" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td></td>
                            <td></td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td colspan="4" class="label" style="text-align: left; text-decoration: underline; font-weight: bold;">
                            <br />
                            Cash Outward Details :-
                <br />
                        </td>
                    </tr>
                    <asp:Panel ID="pnlCashAc" Style="width: 100%;" runat="server">
                        <tr>
                            <!-- Bank/Cash Account -->
                            <td class="label" style="text-align: left;">Cash A/c<b style="color: Red;">*</b>
                            </td>
                            <td class="txt_style" colspan="3">
                                <asp:UpdatePanel ID="UpdatePanelCashAc" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlCashAccount" runat="server" CssClass="textbox" Height="27px"
                                            Width="98.5%" ClientIDMode="Static">
                                        </asp:DropDownList>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlPaymentMode" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left;">Amount
                            </td>
                            <td class="txt_style">
                                <asp:UpdatePanel ID="UpdatePanel154" runat="server">
                                    <ContentTemplate>
                                        <asp:TextBox ID="txtCashAmount" class="textbox" AutoPostBack="true" Width="90%" runat="server"
                                            Style="text-align: right;" onkeyup="return Fixdecimal(this);" onchange="return fncPopup();"
                                            MaxLength="8"></asp:TextBox>
                                        <asp:Button ID="btnCashPopup" runat="server" Text="Button" OnClick="btnCashPopup_Click"
                                            Style="display: none;" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlPaymentMode" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                            <td class="label" style="text-align: left; padding-left: 20px;">Outward By
                            </td>
                            <td>
                                <asp:UpdatePanel ID="UpdatePanel213" runat="server">
                                    <ContentTemplate>
                                        <asp:DropDownList ID="ddlCashOutBy" runat="server" CssClass="textbox" Width="95%"
                                            Height="27px" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="ddlPaymentMode" EventName="SelectedIndexChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </td>
                        </tr>
                    </asp:Panel>
                    <tr>
                        <td colspan="4">
                            <br />
                            <asp:Panel ID="PnlDeno" runat="server">
                                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvDenominationDetails" runat="server" ShowFooter="true" Width="100%"
                                            AutoGenerateColumns="False" OnRowDataBound="gvDenominationDetails_RowDataBound">
                                            <HeaderStyle CssClass="gVHeader" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr. No." ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="gvtxtDenoSrno" runat="server" Style="text-align: center;" CssClass="textbox_readonly"
                                                            Width="50px" Text='<%#Eval("Serialno") %>'></asp:TextBox>
                                                        <asp:HiddenField ID="hdndenoid" runat="server" Value='<%#Eval("DenoId") %>' />
                                                        <asp:HiddenField ID="hdncashinoutid" runat="server" Value='<%#Eval("InOutID") %>' />
                                                        <asp:HiddenField ID="hdnrefno" runat="server" Value='<%#Eval("RefNo") %>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Denominations Rs." ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="gvtxtDenoDescription" runat="server" MaxLength="4" onkeypress="return OnlyNumericEntry();"
                                                            CssClass="textbox" onkeyup="return CalDeno();" Width="125px" Text='<%#Eval("DenoRs") %>'
                                                            Style="text-align: center;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="No. (Quantity)" ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="gvtxtDenoNo" runat="server" Width="120px" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                                                            CssClass="textbox" onkeyup="return CalDeno();" Text='<%#Eval("Quantity") %>'
                                                            Style="text-align: center;"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <div class="label">
                                                            <asp:Label ID="gvlblDenoTotal" runat="server" Text="Total"></asp:Label>
                                                        </div>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Total" ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="gvtxtDenoTotal" runat="server" MaxLength="10" onkeyup="return CalDeno();"
                                                            CssClass="textbox_readonly" Width="149px" Text='<%#Eval("Total") %>' Style="text-align: center;"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="gvtxtDenoTotalAmt" runat="server" MaxLength="10" CssClass="textbox_readonly"
                                                            Width="149px" onkeyup="return CalDeno();" Style="text-align: center;"></asp:TextBox>
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Note Nos." ItemStyle-VerticalAlign="Top">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="gvtxtDenoNoteNos" runat="server" onkeypress="return isAlphaNumChar2(event);"
                                                            TextMode="MultiLine" Text='<%#Eval("NoteNos") %>' CssClass="textbox" Style="height: 15px; width: 170px; text-transform: uppercase; resize: vertical; text-align: left;"></asp:TextBox>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDenoDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                            OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                            Width="20px" Height="20px" OnClick="btnDenoDelete_Click" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <%--OnClick="BtnUploadCharges_Click"--%>
                                                        <asp:Button ID="btnDenoAdd" runat="server" Text="Add" OnClick="btnDenoAdd_Click"
                                                            OnClientClick="return AddDeno();" />
                                                    </FooterTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="gvDenominationDetails" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" class="label" style="text-align: left; text-decoration: underline; font-weight: bold; height: 32px;">
                            <br />
                            <br />
                            Gold Inward Details :-
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left;">Inward By <b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:DropDownList ID="ddlGoldInwardBy" runat="server" CssClass="textbox"
                                Width="97%" Height="27px" />
                        </td>
                        <td class="label" style="text-align: left; padding-left: 20px;">Rack No <b style="color: Red;">*</b>
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtRackno" class="textbox" MaxLength="8" onkeypress="return isAlphaNumChar2(event);"
                                runat="server" Width="90%" Style="text-align: left"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <!-- Charges Details Section -->
                        <td colspan="4" class="label" style="text-align: left; text-decoration: underline; font-weight: bold;">
                            <br />
                            <br />
                            Charges Details :-
                        </td>
                    </tr>
                    <tr>
                        <!-- Charges Details Section -->
                        <td colspan="4" align="center">
                            <div>
                                <!--GridView -->
                                <asp:UpdatePanel ID="UpdatePanel9" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="dgvChargesDetails" runat="server" DataKeyNames="CHID" ShowFooter="true"
                                            Width="100%" AutoGenerateColumns="False" OnRowCommand="dgvChargesDetails_RowCommand"
                                            OnDataBound="dgvChargesDetails_DataBound" OnRowDataBound="dgvChargesDetails_RowDataBound">
                                            <AlternatingRowStyle BackColor="White" />
                                            <HeaderStyle CssClass="gVHeader" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="Sr. No." ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSrno" runat="server" Text='<%# Eval("Serialno") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdnchid" runat="server" Value='<%# Eval("CHID") %>' />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Charges Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblChargesName" align="center" runat="server" Text='<%# Eval("ChargeName") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdncid" runat="server" Value='<%# Eval("CID") %>' />
                                                        <asp:HiddenField ID="hdnchargeid" runat="server" Value='<%# Eval("ID") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:DropDownList ID="ddlChargesName" runat="server" Height="22px" DataSourceID="SqlDataSource3"
                                                            DataTextField="ChargeName" DataValueField="CID" Width="250px">
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                                                            OnSelecting="SqlDataSource3_Selecting" SelectCommand="SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo WHERE Status='Active' ORDER BY ChargeName">
                                                            <SelectParameters>
                                                                <asp:Parameter Name="RefDate" Type="string" />
                                                            </SelectParameters>
                                                        </asp:SqlDataSource>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Charges" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCharges" runat="server" Text='<%# Eval("Charges") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblChargeType" runat="server" Text='<%# Eval("ChargeType") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("Amount") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Account Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAccountName" align="center" runat="server" Text='<%# Eval("AccountName") %>'></asp:Label>
                                                        <asp:HiddenField ID="hdnaccountid" runat="server" Value='<%# Eval("AccountID") %>' />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:DropDownList ID="ddlAccountName" runat="server" Height="22px" DataSourceID="SqlDataSource2"
                                                            DataTextField="Name" DataValueField="AccountID" Width="250px">
                                                        </asp:DropDownList>
                                                        <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringAIM %>"
                                                            SelectCommand="SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name"></asp:SqlDataSource>
                                                    </FooterTemplate>
                                                    <HeaderStyle HorizontalAlign="Left" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                            OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                            Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Button ID="BtnUploadCharges" runat="server" Text="Add" OnClientClick="return Charges();"
                                                            CommandName="AddRecord" />
                                                    </FooterTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="dgvChargesDetails" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <br />
                            <asp:TextBox ID="txtGID" runat="server" Visible="False" Height="20px" Width="15px"></asp:TextBox>
                            <!-- FY ID -->
                            <asp:TextBox ID="txtFYID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <!-- Branch ID -->
                            <asp:TextBox ID="txtBranchID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <!-- Comp ID -->
                            <asp:TextBox ID="txtCompID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <!-- Mobile, Telephone, Email ID, LoginID -->
                            <asp:TextBox ID="txtMobile" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtTelephone" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtEmailID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtTotalGross" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtLoginID" runat="server" Visible="False" Height="16px" Width="16px"></asp:TextBox>

                            <!-- Photo Path -->
                            <asp:TextBox ID="txtProofOfOwnershipPath" runat="server" Visible="False" Height="22px"
                                Width="16px"></asp:TextBox>
                            <!-- ID, SchemeName, SchemeType, LTV, MinTenure, MaxTenure, InterestRate, AreaID, Total Charges Amount-->
                            <asp:TextBox ID="txtSchemeID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtSchemeName" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtSchemeType" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtLTV" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtMinTenure" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtMaxTenure" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtInterestRate" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:TextBox ID="txtAreaID" runat="server" Visible="False" Height="22px" Width="17px"></asp:TextBox>
                            <asp:UpdatePanel ID="UpdatePanel11" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtTotalChargesAmount" runat="server" Style="display: none;" Height="22px"
                                        Width="17px"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="dgvChargesDetails" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
                <script language="javascript" type="text/javascript">        disableDate();</script>
                <script language="javascript" type="text/javascript">        disableDueDate();</script>
                <div id="dialog" style="display: none">
                    <div class="modal-header">
                        <h3>Login
                        </h3>
                    </div>
                    <div>
                        &nbsp
                    </div>
                    <div>
                        &nbsp
                    </div>
                    <div>
                        &nbsp
                    </div>
                    <div class="modal-body">
                        User Name:
            <input type="text" id="txtName" name="txtName" placeholder="User Name" class="input-large" />
                        <br />
                        <br />
                        Password:
            <input id="txtPwd" name="txtPwd" placeholder="Password" type="password" class="input-large" />
                    </div>



                </div>
    
            </ContentTemplate>
        </asp:TabPanel>
       <%-- <asp:TabPanel runat="server" HeaderText="Applicant's PDC Chart" ID="tbpnl3">


        </asp:TabPanel>--%>
    </asp:TabContainer>

</asp:Content>



