<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="GLReceiptNew.aspx.cs" Inherits="GLReceiptNew" EnableEventValidation="false" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">

        var currentDropDownValue = "";
        function gridDDLChange(data) {
            currentDropDownValue = document.getElementById(data.id).value;
            return;
        }

        function MakeUpper(txtId) {
            document.getElementById(txtId).value = document.getElementById(txtId).value.toUpperCase();
        }
        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return (k > 45 && k < 57);
        }
        function lettersOnly(evt) {
            evt = (evt) ? evt : event;
            var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode : ((evt.which) ? evt.which : 0));
            if (charCode > 33 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
                return false;
            }
            else
                return true;
        };
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        function isAlphaNumChar2(e) { // Alphanumeric, space,-,/ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return (k == 44 || (k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 47) || k == 0);
        }
        function isAlphaNumeric(e) { // Alphanumeric only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123));
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


            var txtPrincipalCurrentAmt = document.getElementById("<%=txtPrincipalCurrentAmt.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");
            var txtPenalCurrentAmt = document.getElementById("<%=txtPenalCurrentAmt.ClientID %>");
            var txtChargesCurrentAmt = document.getElementById("<%=txtChargesCurrentAmt.ClientID %>");
            var txtAdvInterest = document.getElementById("<%=txtAdvInterest.ClientID %>");

            var txtRcvTotal = document.getElementById("<%=txtRcvTotal.ClientID %>");

            if (txtPrincipalCurrentAmt.value == '') {
                txtPrincipalCurrentAmt.value = 0;
            }

            if (txtInterestCurrentAmt.value == '') {
                txtInterestCurrentAmt.value = 0;
            }

            if (txtPenalCurrentAmt.value == '') {
                txtPenalCurrentAmt.value = 0;
            }

            if (txtChargesCurrentAmt.value == '') {
                txtChargesCurrentAmt.value = 0;
            }

            if (txtAdvInterest.value == '') {
                txtAdvInterest.value = 0;
            }

            txtRcvTotal.value = 0;

            txtRcvTotal.value = parseFloat(txtPrincipalCurrentAmt.value) + parseFloat(txtInterestCurrentAmt.value);
            txtRcvTotal.value = parseFloat(txtRcvTotal.value) + parseFloat(txtPenalCurrentAmt.value) + parseFloat(txtChargesCurrentAmt.value) + parseFloat(txtAdvInterest.value);

        }

        function chkInterest() {

            var lblInterestCurrent = document.getElementById("<%=lblInterestCurrent.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");

            if (lblInterestCurrent.value == txtInterestCurrentAmt.value) {
                document.getElementById("<%=txtAdvInterest.ClientID %>").disabled = false;
                document.getElementById("<%=ddlAdvIntAcHead.ClientID %>").disabled = false;
            }
            else {
                alert('');
            }

        }

        function CheckNumeric() {
            var txtPrincipalCurrentAmt = document.getElementById("<%=txtPrincipalCurrentAmt.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");
            var txtPenalCurrentAmt = document.getElementById("<%=txtPenalCurrentAmt.ClientID %>");
            var txtChargesCurrentAmt = document.getElementById("<%=txtChargesCurrentAmt.ClientID %>");



            if (isNaN(txtPrincipalCurrentAmt.value)) {

                txtPrincipalCurrentAmt.value = '';
            }

            if (isNaN(txtInterestCurrentAmt.value)) {

                txtInterestCurrentAmt.value = '';
            }

            if (isNaN(txtPenalCurrentAmt.value)) {

                txtPenalCurrentAmt.value = '';
            }

            if (isNaN(txtChargesCurrentAmt.value)) {

                txtChargesCurrentAmt.value = '';
            }

        }
        function validcheque() {

            var grd = document.getElementById("<%=gvChequeDetails.ClientID %>");
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;
            var ddlPaymentMode = document.getElementById("<%=ddlPaymentMode.ClientID %>");
            var count = grd.rows.length;

            var ddlChqDDNefts;

            for (i = 1; i < grd.rows.length - 1; i++) {
                ddlChqDDNefts = grd.rows[i].cells[1].children[1];
                var No = grd.rows[i].cells[2].children[0];
                var chqdate = grd.rows[i].cells[3].children[0];
                var Bankid = grd.rows[i].cells[4].children[0];
                var Amount = grd.rows[i].cells[5].children[0];
            }
            if (ddlChqDDNefts.value == 'Online') {
            }
            else {
                if (No.value == '') {
                    alert('Enter Cheque No.');
                    return false;
                }
            }
            if (chqdate.value == '') {
                alert('Enter Cheque Date.');
                return false;
            }
            if (!pattern.test(chqdate.value)) {

                alert('Invalid Cheque Date.');
                return false;
            }
            if (ddlChqDDNefts.value == 'Online') {
            }
            else {
                if (Bankid.value == '0') {

                    alert('Select Bank Name');
                    return false;
                }
            }
            if (Amount.value == '') {

                alert('Enter Cheque Amount');
                return false;
            }
            if (isNaN(Amount.value)) {

                Amount.value = '';
                return false;
            }
            if (parseFloat(Amount.value) <= 0) {

                alert('Enter Valid Cheque Amount');
                return false;
            }

        }

        function validchdate() {

            var grd = document.getElementById("<%=gvChequeDetails.ClientID %>");
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;
            for (i = 1; i < grd.rows.length - 1; i++) {

                var chqdate = grd.rows[i].cells[3].children[0];

                if (!pattern.test(chqdate.value)) {

                    chqdate.value = '';
                    alert('Invalid Cheque Date.');
                    return false;
                }
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

            var totalamt = 0;
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
        }

        function validateNew() {

            var lblInterestCurrent = document.getElementById("<%=lblInterestCurrent.ClientID %>").innerHTML;
            var lblPrincipalCurrent = document.getElementById("<%=lblPrincipalCurrent.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");

            alert(lblInterestCurrent);

        }


        function validate() {

            var txtGoldNo = document.getElementById("<%=txtGoldNo.ClientID %>");
            var txtPrincipalCurrentAmt = document.getElementById("<%=txtPrincipalCurrentAmt.ClientID %>");
            var ddlPrincipalCurrentAcHead = document.getElementById("<%=ddlPrincipalCurrentAcHead.ClientID %>");
            var ddlPrincipalCurrentNarration = document.getElementById("<%=ddlPrincipalCurrentNarration.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");
            var ddlInterestCurrentAcHead = document.getElementById("<%=ddlInterestCurrentAcHead.ClientID %>");
            var ddlInterestCurrentNarration = document.getElementById("<%=ddlInterestCurrentAcHead.ClientID %>");
            var txtReceiptBook = document.getElementById("<%=txtReceiptBook.ClientID %>");
            var txtReceipt = document.getElementById("<%=txtReceipt.ClientID %>");
            var txtRecvFrom = document.getElementById("<%=txtRecvFrom.ClientID %>");
            var ddlCollectedBy = document.getElementById("<%=ddlCollectedBy.ClientID %>");
            var ddlCashier = document.getElementById("<%=ddlCashier.ClientID %>");

            var lblPrincipalCurrent = document.getElementById("<%=lblPrincipalCurrent.ClientID %>").innerHTML;
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");
            var txtPenalCurrentAmt = document.getElementById("<%=txtPenalCurrentAmt.ClientID %>");
            var txtChargesCurrentAmt = document.getElementById("<%=txtChargesCurrentAmt.ClientID %>");
            var ddlPenalCurrentAcHead = document.getElementById("<%=ddlPenalCurrentAcHead.ClientID %>");
            var ddlChargesCurrentAcHead = document.getElementById("<%=ddlChargesCurrentAcHead.ClientID %>");
            var ddlPenalCurrentNarration = document.getElementById("<%=ddlPenalCurrentNarration.ClientID %>");
            var ddlChargesCurrentNarration = document.getElementById("<%=ddlChargesCurrentNarration.ClientID %>");

            var txtRcvTotal = document.getElementById("<%=txtRcvTotal.ClientID %>");
            var ddlPaymentMode = document.getElementById("<%=ddlPaymentMode.ClientID %>");
            var ddlBankAcc = document.getElementById("<%=ddlBankAcc.ClientID %>");
            var ddlCashAcc = document.getElementById("<%=ddlCashAcc.ClientID %>");
            var lblInterestCurrent = document.getElementById("<%=lblInterestCurrent.ClientID %>").innerHTML;
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");

            var txtIntFromDate = document.getElementById("<%=txtIntFromDate.ClientID %>");
            var txtIntToDate = document.getElementById("<%=txtIntToDate.ClientID %>");
            var txtAdvIntFrom = document.getElementById("<%=txtAdvIntFrom.ClientID %>");
            var txtAdvIntTo = document.getElementById("<%=txtAdvIntTo.ClientID %>");
            var ddlAdvIntAcHead = document.getElementById("<%=txtAdvIntTo.ClientID %>");
            var txtAdvInterest = document.getElementById("<%=txtAdvInterest.ClientID %>");

            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;


            if (txtGoldNo.value == '') {

                alert('Select Gold Loan No.');
                return false;
            }
            if (parseFloat(txtPrincipalCurrentAmt.value) > 0) {
                if (parseFloat(txtPrincipalCurrentAmt.value) > parseFloat(lblPrincipalCurrent)) {

                    alert('Principal Amount Should Not Be Greater Than Outstanding');
                    return false;
                }
                if (ddlPrincipalCurrentAcHead.selectedIndex == 0) {

                    alert('Select Principal Current Loan A/c Head');
                    return false;
                }
                if (ddlPrincipalCurrentNarration.selectedIndex == 0) {

                    alert('Select Principal Current Loan Narration');
                    return false;
                }



            }

            if (parseFloat(txtInterestCurrentAmt.value) > 0) {

                if (parseFloat(txtInterestCurrentAmt.value) > parseFloat(lblInterestCurrent)) {
                    alert('Interest Amount Should Not Be Greater Than Outstanding Interest');
                    return false;
                }
                if (ddlInterestCurrentAcHead.selectedIndex == 0) {

                    alert('Select Interest Current Loan A/c Head');
                    return false;
                }
                if (ddlInterestCurrentNarration.selectedIndex == 0) {

                    alert('Select Interest Current Loan Narration');
                    return false;
                }

                if (txtIntFromDate.value == "") {
                    alert('Select Interest From Date');
                    return false;
                }
                if (txtIntToDate.value == "") {
                    alert('Select Interest To Date');
                    return false;
                }
            }


            if (parseFloat(txtPenalCurrentAmt.value) > 0) {
                //if (parseFloat(txtInterestCurrentAmt.value) > parseFloat(lblPenalCurrent)) {
                //    alert('Penal Interest Amount Should Not Be Greater Than Outstanding Penal Interest');
                //    return false;
                // }
                if (ddlPenalCurrentAcHead.selectedIndex == 0) {

                    alert('Select Penal Current A/c Head');
                    return false;
                }
                if (ddlPenalCurrentNarration.selectedIndex == 0) {

                    alert('Select Penal Current Narration');
                    return false;
                }
            }

            if (parseFloat(txtChargesCurrentAmt.value) > 0) {

                if (ddlChargesCurrentAcHead.selectedIndex == 0) {

                    alert('Select Charges Current A/c Head');
                    return false;
                }
                if (ddlChargesCurrentNarration.selectedIndex == 0) {

                    alert('Select Charges Current Narration');
                    return false;
                }
            }
            if (txtAdvInterest.value > 0) {
                if (parseFloat(txtPrincipalCurrentAmt.value) == parseFloat(lblPrincipalCurrent)) {
                    alert('You Are Paying Full Principle Amount,You Can not Pay Advance Interest');
                    return false;
                }

                if ((parseFloat(txtInterestCurrentAmt.value)) != parseFloat(lblInterestCurrent)) {
                    alert('Oustanding Interest is Pending,You Can not Pay Advance Interest');
                    // txtAdvInterest.value = '';
                    return false;
                }
                // else if (parseFloat(lblInterestCurrent) == 0) {
                //     alert('Interest is 0,You Can not Pay Advance Interest');
                //txtAdvInterest.value = '0';
                //   return false;
                // }
                else {

                }

            }

            //CalcAdvInterest();

            var txtAdvInterest = document.getElementById("<%=txtAdvInterest.ClientID %>");
            var TotalAdvance = document.getElementById("<%=HdnAdvanceInterest.ClientID%>");
            //            var TotalAdvance = $("[id$='HdnAdvanceInterest']").val();

            if (parseFloat(txtAdvInterest.value) > 0) {
                if (parseFloat(txtPrincipalCurrentAmt.value) == parseFloat(lblPrincipalCurrent)) {
                    alert('You Are Paying Full Principal Amount,You Can not Pay Advance Interest');
                    return false;
                }

                if (parseFloat(TotalAdvance.value) == 0) {
                    alert('Advance Interest is 0,for Selected Date Range');
                    return false;
                    //txtAdvInterest.value = '0';
                }
                else if (parseFloat(txtAdvInterest.value) <= parseFloat(TotalAdvance.value)) {
                }
                else {
                    alert('Advance Interest Can Not be More for Selected Date Range');
                    //  txtAdvInterest.value = '0';
                    return false;
                }
            }


            if (parseFloat(txtAdvInterest.value) > 0) {

                if (ddlAdvIntAcHead.selectedIndex == 0) {

                    alert('Select Advance Interest Current A/c Head');
                    return false;
                }
                if (txtAdvIntFrom.value == "") {
                    alert('Select Advance Interest From Date');
                    return false;
                }
                if (txtAdvIntTo.value == "") {
                    alert('Select  Advance Interest To Date');
                    return false;
                }
            }


            if (ddlPaymentMode.selectedIndex == 2 || ddlPaymentMode.selectedIndex == 3) {

                if (txtReceiptBook.value == '') {

                    alert('Enter Receipt Book No.');
                    return false;
                }
                if (txtReceipt.value == '') {

                    alert('Enter Receipt No.');
                    return false;
                }
                if (ddlCashier.selectedIndex == 0) {

                    alert('Select Cashier Name');
                    return false;
                }
            }
            if (txtRecvFrom.value == '') {

                alert('Enter Received From');
                return false;
            }
            if (ddlCollectedBy.selectedIndex == 0) {

                alert('Select Collected By');
                return false;
            }

            if (ddlPaymentMode.selectedIndex == 0) {

                alert('Select Payment Mode');
                return false;
            }
            if (ddlPaymentMode.selectedIndex == 1) {

                if (ddlBankAcc.selectedIndex == 0) {

                    alert('Select Bank Account');
                    return false;
                }
            }
            if (ddlPaymentMode.selectedIndex == 2) {

                if (ddlCashAcc.selectedIndex == 0) {

                    alert('Select Cash Account');
                    return false;
                }
            }
            if (ddlPaymentMode.selectedIndex == 3) {

                if (ddlCashAcc.selectedIndex == 0 && ddlBankAcc.selectedIndex == 0) {

                    alert('Select Bank/Cash Account');
                    return false;
                }
            }
            var grd = document.getElementById("<%=gvChequeDetails.ClientID %>");


            for (i = 1; i < grd.rows.length - 1; i++) {
                var ddlChqDDNeft = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var chqdate = grd.tBodies[0].rows[i].cells[3].children[0];
                var Bankid = grd.rows[i].cells[4].children[0];
                var Amount = grd.rows[i].cells[5].children[0];

                if (ddlChqDDNeft.disabled == true) {
                    if (ddlChqDDNeft.value == 'Cheque') {

                        if (isNaN(No.value)) {

                            alert('Invalid Cheque No');
                            return false;
                        }
                        if (No.value.length != 6) {

                            alert('Invalid Cheque No');
                            return false;
                        }
                    }
                    if (ddlChqDDNeft.value == 'DD') {

                        if (isNaN(No.value)) {

                            alert('Invalid DD No');
                            return false;
                        }
                        if (No.value.length != 6) {

                            alert('Invalid DD No');
                            return false;
                        }

                    }
                    if (ddlChqDDNeft.value == 'NEFT') {

                        if (!isNaN(No.value)) {

                            alert('Invalid NEFT No');
                            return false;
                        }
                        if (No.value.length > 15) {

                            alert('Invalid NEFT No');
                            return false;
                        }
                    }
                    if (ddlChqDDNeft.value == 'Online') {

                    }
                    if (No.value != '') {

                        if (chqdate.value == '') {

                            alert('Enter Chq/DD/NEFT Date');
                            return false;
                        }
                        if (Bankid.value == 0) {

                            alert('Select Bank');
                            return false;
                        }
                        if (Amount.value == '') {

                            alert('Enter Cheque Amount');
                            return false;
                        }
                        if (isNaN(Amount.value)) {
                            alert('Invalid Cheque Amount');
                            return false;
                        }
                        if (parseFloat(Amount.value) <= 0) {

                            alert('Enter valid Cheque Amount');
                            return false;
                        }

                    }
                }
            }



            if (ddlPaymentMode.selectedIndex == 1) {
                if (parseFloat(txtRcvTotal.value) != parseFloat(grd.rows[grd.rows.length - 1].cells[5].children[0].value)) {

                    alert('Enter valid Cheque Total Amount');
                    return false;
                }
            }

            var totalchq = parseFloat(grd.rows[grd.rows.length - 1].cells[5].children[0].value);

            var grd = document.getElementById("<%=gvDenominationDetails.ClientID %>");
            for (i = 1; i < grd.rows.length - 1; i++) {

                var DenoRs = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var Total = grd.rows[i].cells[3].children[0];

                if (parseFloat(DenoRs.value) > 0) {

                    if (parseFloat(No.value) <= 0) {
                        alert('Enter No.(Quantity)');
                        return false;
                    }
                }
            }

            if (ddlPaymentMode.selectedIndex == 2) {


                if (parseFloat(txtRcvTotal.value) != parseFloat(grd.rows[grd.rows.length - 1].cells[3].children[0].value)) {

                    //                    alert('Enter valid Denomination Total Amount');
                    //                    return false;
                }
            }
            var totaldeno = parseFloat(grd.rows[grd.rows.length - 1].cells[3].children[0].value);

            var totalpay = parseFloat(totalchq) + parseFloat(totaldeno);


            if (ddlPaymentMode.selectedIndex == 3) {
                if (parseFloat(txtRcvTotal.value) != parseFloat(totalpay)) {

                    alert('Enter valid Cheque/Cash Total Amount');
                    return false;
                }
            }

            checkInterestToDate();

        }

        function chqTotal() {

            var total = 0;
            var grd = document.getElementById("<%=gvChequeDetails.ClientID %>");


            for (i = 1; i < grd.rows.length - 1; i++) {


                var Amount = grd.rows[i].cells[5].children[0];
                if (Amount.value == '' || isNaN(Amount.value)) {

                    Amount.value = 0;
                }

                total = parseFloat(total) + parseFloat(Amount.value);

            }
            grd.rows[grd.rows.length - 1].cells[5].children[0].value = total;

        }


        function validPayment() {
            var txtRcvTotal = document.getElementById("<%=txtRcvTotal.ClientID %>");
            var ddlPaymentMode = document.getElementById("<%=ddlPaymentMode.ClientID %>");

            if (txtRcvTotal.value == '') {

                alert('Received Total Amount Should Not Be Blank');
                txtRcvTotal.focus();
                ddlPaymentMode.selectedIndex = 0;
                return false;
            }
            if (parseFloat(txtRcvTotal.value) <= 0) {

                alert('Received Total Amount Should Be More Than 0');
                txtRcvTotal.focus();
                ddlPaymentMode.selectedIndex = 0;
                return false;
            }
        }


        function validrcvdate(txt, keyCode) {

            ValidateDate(txt, keyCode);

            document.getElementById('<%= btnClearOnDate.ClientID %>').click();

            var txtRecvDate = document.getElementById("<%=txtRecvDate.ClientID %>");

            var datearray = txtRecvDate.value.split("/");
            var newdate = datearray[1] + '/' + datearray[0] + '/' + datearray[2];

            var ndate = new Date(newdate);
            var dd = ndate.getDate() + 1;
            var mm = ndate.getMonth() + 1;
            var yy = ndate.getFullYear();

            var lastDay = new Date(ndate.getFullYear(), ndate.getMonth() + 1, 0);
            var lastDayFormat = lastDay[1] + '/' + lastDay[0] + '/' + lastDay[2];
            var lastdd = lastDay.getDate();
            var lastmm = lastDay.getMonth() + 1;
            var lastyy = lastDay.getFullYear();

            var someFormattedDate = dd + '/' + datearray[1] + '/' + yy;
            var MonthlastDay = lastdd + '/' + datearray[1] + '/' + lastyy;


            if (MonthlastDay == txtRecvDate.value) {

                //  alert('100');
                var Fdatearray = MonthlastDay.split("/");
                var Fnewdate = Fdatearray[1] + '/' + Fdatearray[0] + '/' + Fdatearray[2];

                var nndate = new Date(Fnewdate);
                var fmm = nndate.getMonth() + 2;
                // var RES = nndate.addMonths(1);
                var fday = 01;
                var fmth = fmm;
                var fyy = Fdatearray[2];

                var fsDay = fmth + '/' + fday + '/' + fyy;
                var finalFday = fday + '/' + fmth + '/' + fyy;

                var fssDay = new Date(fsDay);
                var flastDay = new Date(fssDay.getFullYear(), fssDay.getMonth() + 1, 0);

                var flastdd = flastDay.getDate();
                var flastmm = flastDay.getMonth() + 1;
                var flastyy = flastDay.getFullYear();

                //  var flastDayF = flastDay[1] + '/' + flastDay[0] + '/' + flastDay[2];

                var fMonthlastDay = flastdd + '/' + flastmm + '/' + flastyy;

                //  document.getElementById("<%=txtAdvIntFrom.ClientID %>").value = finalFday;
                // document.getElementById("<%=txtAdvIntTo.ClientID %>").value = fMonthlastDay;

                document.getElementById("<%=txtAdvIntFrom.ClientID %>").value = MonthlastDay;
                document.getElementById("<%=txtAdvIntTo.ClientID %>").value = MonthlastDay;
            }
            else {
                document.getElementById("<%=txtAdvIntFrom.ClientID %>").value = someFormattedDate;
                document.getElementById("<%=txtAdvIntTo.ClientID %>").value = MonthlastDay;
            }

            if (txtRecvDate.value == '') {
                alert('Select Receive Date');
                return false;
            }
        }


        //function added by priya start
        function ValidateSerch(txt, keyCode) {

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
                    return false;
                }
            }
            else if (val.length < 10) {
                txt.value = '';
                alert('Invalid Date.');
                return false;
            }
            //validrcvdate();
        }
        //date end



        function checkInterestFromDate(txt, keyCode) {

            var IntFromDate = document.getElementById("<%=txtIntFromDate.ClientID %>");
            var IntToDate = document.getElementById("<%=txtIntToDate.ClientID %>");
            var RcvDate = document.getElementById("<%=txtRecvDate.ClientID %>");

            ValidateDate(txt, keyCode);

            if (IntFromDate.value > RcvDate.value) {
                alert('Interest From Date Should not be greater than Received Date');
                IntFromDate.value = '';
                return false;
            }

            if (IntToDate.value != "") {
                if (IntFromDate.value > IntToDate.value) {
                    alert('Interest From Date Should not be less than To Date');
                    IntFromDate.value = '';
                    return false;
                }
            }
        }

        function checkInterestToDate(txt, keyCode) {

            var IntFromDate = document.getElementById("<%=txtIntFromDate.ClientID %>");
            var IntToDate = document.getElementById("<%=txtIntToDate.ClientID %>");
            var RcvDate = document.getElementById("<%=txtRecvDate.ClientID %>");

            ValidateDate(txt, keyCode);

            //recdate
            var datearrayR = RcvDate.value.split("/");
            var newdateR = datearrayR[1] + '/' + datearrayR[0] + '/' + datearrayR[2];

            var newRecDate = new Date(newdateR);
            // var ndateR = new Date(newdateR);
            // var ddR = ndateR.getDate();
            // var mmR = ndateR.getMonth() + 1;
            // var yyR = ndateR.getFullYear();


            //todate
            var datearray3 = IntToDate.value.split("/");
            var newdate3 = datearray3[1] + '/' + datearray3[0] + '/' + datearray3[2];

            var ndate3 = new Date(newdate3);
            var dd3 = ndate3.getDate();
            var mm3 = ndate3.getMonth() + 1;
            var yy3 = ndate3.getFullYear();

            var someFormattedDate33 = datearray3[1] + '/' + dd3 + '/' + yy3;

            //fromdate
            var datearray4 = IntFromDate.value.split("/");
            var newdate4 = datearray4[1] + '/' + datearray4[0] + '/' + datearray4[2];

            var ndate4 = new Date(newdate4);
            var dd4 = ndate4.getDate();
            var mm4 = ndate4.getMonth() + 1;
            var yy4 = ndate4.getFullYear();

            var someFormattedDate44 = datearray4[1] + '/' + dd4 + '/' + yy4;

            if (ndate3 < ndate4) {
                alert('Interest To Date Should not be greater than From Date');
                IntToDate.value = '';
                return false;
            }

            if (ndate3 > newRecDate) {
                alert('Interest To Date Should not be greater than Received Date');
                IntToDate.value = '';
                return false;
            }
        }


        function checkAdvInterestToDate(txt, keyCode) {

            var AdvIntFrmDate = document.getElementById("<%=txtAdvIntFrom.ClientID %>");
            var AdvIntToDate = document.getElementById("<%=txtAdvIntTo.ClientID %>");
            var RcvDate = document.getElementById("<%=txtRecvDate.ClientID %>");

            ValidateDate(txt, keyCode);

            var datearray1 = RcvDate.value.split("/");
            var newdate1 = datearray1[1] + '/' + datearray1[0] + '/' + datearray1[2];

            var ndate1 = new Date(newdate1);
            var dd1 = ndate1.getDate() + 1;
            var mm1 = ndate1.getMonth() + 1;
            var yy1 = ndate1.getFullYear();

            var someFormattedDate1 = dd1 + '/' + datearray1[1] + '/' + yy1;

            var datearray2 = AdvIntToDate.value.split("/");
            var newdate2 = datearray2[1] + '/' + datearray2[0] + '/' + datearray2[2];

            var ndate2 = new Date(newdate2);
            var dd2 = ndate2.getDate() + 1;
            var mm2 = ndate1.getMonth() + 1;
            var yy2 = ndate2.getFullYear();

            if ((parseInt(AdvIntFrmDate.value) != parseInt(someFormattedDate1))) {
                alert('Advance Interest From Date should be Next Day of Received Date');
                AdvIntFrmDate.value = someFormattedDate1;
                return false;
            }
            if (datearray2 != "") {
                if ((parseInt(datearray1[1]) != parseInt(datearray2[1]))) {
                    alert('Advance Interest Month should be same as Received Date ');
                    AdvIntToDate.value = '';
                    return false;
                }

                else if (parseInt(AdvIntToDate.value) < parseInt(someFormattedDate1)) {
                    alert('Advance Interest To Date Should be Greter than From Date  ');
                    AdvIntToDate.value = '';
                    return false;
                }
            }
            else {
                alert('Select Advance Interest To Date');
            }
        }


        //end

    </script>
    <script type="text/javascript">
        function CalcAdvInterest() {

            //  var txtAdvInterest = $("[id$='txtAdvInterest']").val();

            var txtPrincipalCurrentAmt = document.getElementById("<%=txtPrincipalCurrentAmt.ClientID %>");
            var lblPrincipalCurrent = document.getElementById("<%=lblPrincipalCurrent.ClientID %>").innerHTML;

            var txtAdvInterest = document.getElementById("<%=txtAdvInterest.ClientID%>");
            var TotalAdvance = document.getElementById("<%=HdnAdvanceInterest.ClientID%>");
            //            var TotalAdvance = $("[id$='HdnAdvanceInterest']").val();

            if (parseFloat(txtAdvInterest.value) > 0) {
                if (parseFloat(txtPrincipalCurrentAmt.value) == parseFloat(lblPrincipalCurrent)) {
                    alert('You Are Paying Full Principle Amount,You Can not Pay Advance Interest');
                    return false;
                }
                else if (parseFloat(TotalAdvance.value) == 0) {
                    alert('Advance Interest is 0,for Selected Date Range');
                    return false;
                    //txtAdvInterest.value = '0';
                }
                else if (parseFloat(txtAdvInterest.value) <= parseFloat(TotalAdvance.value)) {
                }
                else {
                    alert('Advance Interest Can Not be More for Selected Date Range');
                    //  txtAdvInterest.value = '0';
                    return false;
                }
            }
        }
    </script>
    <%--Added By Priya for Auto Calculate Interest--%>
    <script type="text/javascript">
        function GetAutoInterest(txt, keyCode) {

            var IntFromDate = document.getElementById("<%=txtIntFromDate.ClientID %>");
            var IntToDate = document.getElementById("<%=txtIntToDate.ClientID %>");
            var RcvDate = document.getElementById("<%=txtRecvDate.ClientID %>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");

            ValidateDate(txt, keyCode);

            //recdate
            var datearrayR = RcvDate.value.split("/");
            var newdateR = datearrayR[1] + '/' + datearrayR[0] + '/' + datearrayR[2];

            var newRecDate = new Date(newdateR);

            //todate
            var datearray3 = IntToDate.value.split("/");
            var newdate3 = datearray3[1] + '/' + datearray3[0] + '/' + datearray3[2];

            var ndate3 = new Date(newdate3);
            var dd3 = ndate3.getDate();
            var mm3 = ndate3.getMonth() + 1;
            var yy3 = ndate3.getFullYear();

            var someFormattedDate33 = datearray3[1] + '/' + dd3 + '/' + yy3;

            //fromdate
            var datearray4 = IntFromDate.value.split("/");
            var newdate4 = datearray4[1] + '/' + datearray4[0] + '/' + datearray4[2];

            var ndate4 = new Date(newdate4);
            var dd4 = ndate4.getDate();
            var mm4 = ndate4.getMonth() + 1;
            var yy4 = ndate4.getFullYear();

            var someFormattedDate44 = datearray4[1] + '/' + dd4 + '/' + yy4;

            if (ndate3 < ndate4) {
                alert('Interest To Date Should not be greater than From Date');
                IntToDate.value = '';
                return false;
            }

            if (ndate3 > newRecDate) {
                alert('Interest To Date Should not be greater than Received Date');
                IntToDate.value = '';
                return false;
            }

            var SelRecvDate = $("[id$='txtRecvDate']").val();
            var SelIntDate = $("[id$='txtIntFromDate']").val();
            var SelIntToDate = $("[id$='txtIntToDate']").val();
            var hdnKYCID = $("[id$='hdnKycidForAutoInt']").val();
            var hdnFyID = $("[id$='hdnfyid']").val();
            var hdnBranchID = $("[id$='hdnbranchid']").val();
            if (hdnoperation.value == 'Save') {
                $.ajax({
                    type: "Post",
                    url: "GLReceiptForm.aspx/GetAutoInterestCal",
                    data: '{SelRecvDate:"' + SelRecvDate + '",SelIntDate:"' + SelIntDate + '",SelIntToDate:"' + SelIntToDate + '",hdnKYCID:"' + hdnKYCID + '",hdnFyID:"' + hdnFyID + '",hdnBranchID:"' + hdnBranchID + '"}',
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: OnSuccess,
                    failure: function (response) {
                        alert(response.d);
                    }
                });
            }
        }
        function OnSuccess(response) {

            $('[id$=txtInterestCurrentAmt]').val(response.d);

            var txtPrincipalCurrentAmt = document.getElementById("<%=txtPrincipalCurrentAmt.ClientID %>");
            var txtInterestCurrentAmt = document.getElementById("<%=txtInterestCurrentAmt.ClientID %>");
            var txtPenalCurrentAmt = document.getElementById("<%=txtPenalCurrentAmt.ClientID %>");
            var txtChargesCurrentAmt = document.getElementById("<%=txtChargesCurrentAmt.ClientID %>");
            var txtAdvInterest = document.getElementById("<%=txtAdvInterest.ClientID %>");

            var txtRcvTotal = document.getElementById("<%=txtRcvTotal.ClientID %>");

            if (txtPrincipalCurrentAmt.value == '') {
                txtPrincipalCurrentAmt.value = 0;
            }

            if (txtInterestCurrentAmt.value == '') {
                txtInterestCurrentAmt.value = 0;
            }

            if (txtPenalCurrentAmt.value == '') {
                txtPenalCurrentAmt.value = 0;
            }

            if (txtChargesCurrentAmt.value == '') {
                txtChargesCurrentAmt.value = 0;
            }

            if (txtAdvInterest.value == '') {
                txtAdvInterest.value = 0;
            }

            txtRcvTotal.value = 0;
            txtRcvTotal.value = parseFloat(txtPrincipalCurrentAmt.value) + parseFloat(txtPenalCurrentAmt.value) + parseFloat(response.d) + parseFloat(txtAdvInterest.value);
        }

        //        $(document).ready(function () {
        //            function GetAutoInterest() {
        //                debugger;
        //                alert('fff');
        //                $.ajax({
        //                    type: "Post",
        //                    url: "GLReceiptForm.aspx/GetAutoInterestCal",
        //                    data: '{Todate:"' + assddd + '"}',
        //                    contentType: "application/json; charset=utf-8",
        //                    dataType: "json",
        //                    success: OnSuccess,
        //                    failure: function (response) {
        //                        alert(response.d);
        //                    }
        //                });
        //            }
        //        });
    </script>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnkycid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnsdid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnloantype" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnuserid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnfyid" runat="server" Value="0" />
    <asp:HiddenField ID="hdncmpid" runat="server" Value="1" />
    <asp:HiddenField ID="hdnbranchid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnbcpid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnplcaseno" runat="server" Value="0" />
    <asp:HiddenField ID="hdnmobileno" runat="server" Value="0" />
    <asp:HiddenField ID="hdnKycidForAutoInt" runat="server" Value="0" />
    <table cellpadding="0" cellspacing="0" border="0" style="margin: 6px; width: 99%;"
        align="center">
        <tr>
            <td style="width: 11%;">
                &nbsp;
            </td>
            <td style="width: 19%;">
                &nbsp;
            </td>
            <td style="width: 31%;">
                &nbsp;
            </td>
            <td style="width: 30%;">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4" class="header">
                <asp:Label ID="lblHeader" runat="server" Text="GOLD LOAN RECEIPT"></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="barstyle" style="margin-top: 5px;">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                Received Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRecvDate" CssClass="textbox textbox_GLreceipt" runat="server"
                    Style="font-size: 13.5px;" onchange="validrcvdate(this, event.keyCode)" Width="80%"></asp:TextBox>
                <asp:Button ID="btnClearOnDate" runat="server" Text="Button" OnClick="btnClearOnDate_Click"
                    Style="display: none;" />
                <asp:CalendarExtender ID="txtRecvDate_CalendarExtender" runat="server" CssClass="Calenderstyle"
                    Enabled="True" PopupButtonID="btnImgCalender" TargetControlID="txtRecvDate" Format="dd/MM/yyyy">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
            </td>
            <td rowspan="6" valign="top">
                <asp:Panel CssClass="RoundedPanel" ID="panOutstandingAmount" Style="width: 95%; border: 2px solid #354d8d;
                    margin-top: 6px;" runat="server">
                    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td class="label" colspan="2" style="text-align: center; font-weight: bold; background-color: #354d8d;
                                color: #ffffff;">
                                Outstanding Details
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>Principal</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPrincipalCurrent" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px;">
                                <strong>Interest</strong>
                            </td>
                            <td class="label">
                                <asp:Label ID="lblInterestCurrent" runat="server" Text="0"> </asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px;">
                                <strong>Penal Interest</strong>
                            </td>
                            <td class="label">
                                <asp:Label ID="lblPenalCurrent" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px;">
                                <strong>Charges</strong>
                            </td>
                            <td class="label">
                                <asp:Label ID="lblChargesCurrent" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="background-color: #354d8d; color: #ffffff; text-align: left;
                                padding-left: 5px;">
                                <strong>Total</strong>
                            </td>
                            <td class="label" style="background-color: #354d8d; color: #ffffff;">
                                <asp:Label ID="lblCurrentTotal" runat="server" Text="0" Style="font-weight: bold;"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <div class="RoundedPanel label" style="width: 92%; background-color: #5a5952; color: #ffffff;
                    margin-top: 5px; padding-top: 5px; padding-left: 5px; text-align: left;">
                    <asp:Label ID="lblBalanceLoanEligibility" runat="server" Text="Balance Loan Eligibility : "
                        Style="font-weight: bold;"></asp:Label>
                    <asp:Label ID="lblBalanceLoanEligibilityAmount" runat="server" Text="0" Style="font-weight: bold;
                        float: right; padding-right: 5px;"></asp:Label>
                </div>
            </td>
            <td rowspan="6" valign="top">
                <asp:Panel CssClass="RoundedPanel" ID="panPLDetails" Style="width: 90%; border: 2px solid #354d8d;
                    margin-top: 6px;" runat="server">
                    <table cellpadding="0" cellspacing="0" border="0" style="width: 100%">
                        <tr>
                            <td class="label" colspan="2" style="text-align: center; font-weight: bold; background-color: #354d8d;
                                color: #ffffff;">
                                PL Details
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>Loan Amt</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlLoanAmt" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>D&P EMIs</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlDPEMI" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>O/s EMIs</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlOsEMI" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>O/s Principal</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlOsPrincipal" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>O/s dues</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlOsDues" runat="server" Text="0"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; padding-left: 5px; width: 70%;">
                                <strong>Last pdc date</strong>
                            </td>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblPlLastPdcDate" runat="server" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                Gold Loan No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtGoldNo" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                    Style="margin-right: 2px;" Width="78%"></asp:TextBox>
                <asp:ImageButton ID="btnGlSearch" ImageUrl="~/images/1397069814_Search.png" Height="15px"
                    Width="15px" runat="server" ImageAlign="AbsMiddle" ToolTip="Click for search gold loan no"
                    OnClick="btnGlSearch_Click" OnClientClick="return validrcvdate();" />
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                Loan date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanDate" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                    Width="80%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                Scheme Name
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtScheme" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                    Width="80%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                ROI(%)
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtROI" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                    Width="80%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; width: 11%;">
                Loan Amount
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanAmount" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                    Width="80%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <br />
                <div style="width: 99%; border-bottom: 1px dashed red;">
                </div>
                <br />
            </td>
        </tr>
    </table>
    <div>
        <table cellpadding="0" cellspacing="0" border="1" style="margin: 6px; width: 98%;"
            align="center">
            <tr>
                <td>
                    <table cellpadding="1" id="pnlPrincipal" cellspacing="0" border="0" style="width: 100%;
                        margin: 0px;" runat="server">
                        <tr>
                            <td style="text-align: center; font-weight: bold; background-color: #5a5952; color: #ffffff;
                                width: 17%; font-family: Calibri; font-size: 14px; height: 25px; padding-top: 5px;
                                padding-left: 2px;">
                                Received Details
                            </td>
                            <td class="label" style="text-align: center; font-weight: bold; background-color: #5a5952;
                                color: #ffffff; width: 15%; font-family: Calibri; font-size: 14px; height: 25px;
                                padding-top: 5px; padding-left: 2px;">
                                Amount
                            </td>
                            <td class="label" style="text-align: center; font-weight: bold; background-color: #5a5952;
                                color: #ffffff; width: 30%; font-family: Calibri; font-size: 14px; height: 25px;
                                padding-top: 5px; padding-left: 2px;">
                                A/c Head
                            </td>
                            <td class="label" style="text-align: center; font-weight: bold; background-color: #5a5952;
                                color: #ffffff; width: 30%; font-family: Calibri; font-size: 14px; height: 25px;
                                padding-top: 5px; padding-left: 2px;">
                                Narration
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="text-align: left; width: 17%;">
                                Principal - Current Loan
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtPrincipalCurrentAmt" runat="server" class="textbox textbox_GLreceipt"
                                    onkeyup="return Fixdecimal(this);" MaxLength="10" Style="width: 100px; text-align: left;"></asp:TextBox>
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlPrincipalCurrentAcHead" class="textbox" Height="27px" Width="100%"
                                    runat="server" Enabled="false" >
                                </asp:DropDownList>
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlPrincipalCurrentNarration" class="textbox" Height="27px"
                                    Width="100%" runat="server">
                                </asp:DropDownList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlInterest" Style="width: 100%;" runat="server">
                        <table cellpadding="2" cellspacing="0" border="0" style="width: 100%; margin: 0px;">
                            <tr>
                                <td class="label" style="width: 17%; font-weight: bold; text-align: left;">
                                    <span style="font-weight: normal">Interest - Current Loan </span><b></b>
                                </td>
                                <td class="txt_style" style="width: 15%;">
                                    <asp:TextBox ID="txtInterestCurrentAmt" class="textbox textbox_GLreceipt" runat="server"
                                        onkeyup="return Fixdecimal(this);" MaxLength="10" Style="width: 100px; text-align: left;"></asp:TextBox>
                                </td>
                                <td class="label" style="text-align: left;">
                                    From Date
                                    <asp:TextBox ID="txtIntFromDate" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                                        ReadOnly="true" Style="font-size: 13.5px; width: 50%;" onchange="return checkInterestFromDate(this, event.keyCode);"></asp:TextBox>
                                </td>
                                <td class="label" style="text-align: left;">
                                    To Date
                                    <asp:TextBox ID="txtIntToDate" CssClass="textbox textbox_GLreceipt" runat="server"
                                        Style="font-size: 13.5px; width: 50%;" onchange="return GetAutoInterest(this, event.keyCode);"></asp:TextBox>
                                    <%-- <asp:Button ID="btnCalcInterest" runat="server" Text="Button" OnClick="btnCalcInterest_Click"
                                        Style="display: none;" />--%>
                                    <asp:CalendarExtender ID="CalendarExtender2" runat="server" CssClass="Calenderstyle"
                                        Enabled="True" PopupButtonID="btnImgIntToCal" TargetControlID="txtIntToDate"
                                        Format="dd/MM/yyyy">
                                    </asp:CalendarExtender>
                                    <asp:ImageButton ID="btnImgIntToCal" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                                        Width="5%" Height="15" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlInterestCurrentAcHead" class="textbox" Height="27px" Width="100%"
                                        runat="server" >
                                    </asp:DropDownList>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlInterestCurrentNarration" class="textbox" Height="27px"
                                        Width="100%" runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlPenalInterest" Style="width: 100%;" runat="server">
                        <table cellpadding="2" cellspacing="0" border="0" style="width: 100%; margin: 0px;">
                            <tr>
                                <td class="label" style="width: 17%; text-align: left;">
                                    Penal Interest - Current Loan
                                </td>
                                <td class="txt_style" style="width: 15%;">
                                    <asp:TextBox ID="txtPenalCurrentAmt" class="textbox textbox_GLreceipt" runat="server"
                                        onkeyup="return Fixdecimal(this);" MaxLength="10" Style="width: 100px; text-align: left;"></asp:TextBox>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlPenalCurrentAcHead" class="textbox" Height="27px" Width="100%"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlPenalCurrentNarration" class="textbox" Height="27px" Width="100%"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="pnlCharges" Style="width: 100%;" runat="server">
                        <table cellpadding="2" cellspacing="0" border="0" style="width: 100%; margin: 0px;">
                            <tr>
                                <td class="label" style="width: 17%; text-align: left;">
                                    Charges - Current Loan
                                </td>
                                <td class="txt_style" style="width: 15%;">
                                    <asp:TextBox ID="txtChargesCurrentAmt" class="textbox textbox_GLreceipt" runat="server"
                                        onkeyup="return Fixdecimal(this);" MaxLength="10" Style="width: 100px; text-align: left;"></asp:TextBox>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlChargesCurrentAcHead" class="textbox" Height="27px" Width="100%"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlChargesCurrentNarration" class="textbox" Height="27px" Width="100%"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel2" Style="width: 100%;" runat="server">
                        <table cellpadding="2" cellspacing="0" border="0" style="width: 100%; margin: 0px;">
                            <tr>
                                <td class="label" style="width: 17%; text-align: left;">
                                    Advance Interest - Current Loan
                                </td>
                                <td class="txt_style" style="width: 15%;">
                                    <asp:TextBox ID="txtAdvInterest" class="textbox textbox_GLreceipt" runat="server"
                                        onkeyup="return Fixdecimal(this);" onchange="return CalcAdvInterest();" MaxLength="10"
                                        Style="width: 100px; text-align: left;"></asp:TextBox>
                                    <asp:HiddenField ID="HdnAdvanceInterest" runat="server" Value="0"></asp:HiddenField>
                                </td>
                                <td class="label" style="text-align: left;">
                                    From Date
                                    <asp:TextBox ID="txtAdvIntFrom" CssClass="textbox_readonly textbox_GLreceipt " runat="server"
                                        ReadOnly="true" Style="font-size: 13.5px; width: 50%;" onchange="return checkAdvInterestToDate(this, event.keyCode);"></asp:TextBox>
                                </td>
                                <td class="label" style="text-align: left;">
                                    To Date
                                    <asp:TextBox ID="txtAdvIntTo" CssClass="textbox_readonly  textbox_GLreceipt" runat="server"
                                        ReadOnly="true" Style="font-size: 13.5px; width: 50%;" onchange="return checkAdvInterestToDate(this, event.keyCode);"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                </td>
                                <td>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlAdvIntAcHead" class="textbox" Height="27px" Width="100%"
                                        runat="server">
                                    </asp:DropDownList>
                                </td>
                                <td class="txt_style" style="width: 30%;">
                                    <asp:DropDownList ID="ddlAdvIntNarration" class="textbox" Height="27px" Width="100%"
                                        runat="server" Visible="false">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Panel ID="Panel1" Style="width: 100%;" runat="server">
                        <table cellpadding="2" cellspacing="0" border="0" style="width: 100%; margin: 0px;">
                            <tr>
                                <td class="label" style="width: 17%;">
                                    Total
                                </td>
                                <td class="txt_style" style="width: 15%;">
                                    <asp:TextBox ID="txtRcvTotal" class="textbox_readonly textbox_GLreceipt" runat="server"
                                        onkeyup="return Fixdecimal(this);" MaxLength="10" Style="width: 100px; text-align: left;"></asp:TextBox>
                                </td>
                                <td class="label" style="width: 30%;">
                                    Payment Mode<b style="color: Red;">*</b>
                                </td>
                                <td class="txt_style" style="width: 30%;">                                   
                                            <asp:DropDownList ID="ddlPaymentMode" class="textbox" Height="27px" Width="100%"
                                                runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlPaymentMode_SelectedIndexChanged">
                                                <asp:ListItem Text="--Select Mode--"></asp:ListItem>
                                                <asp:ListItem Text="Chq/DD/NEFT/Online"></asp:ListItem>
                                                <asp:ListItem Text="Cash"></asp:ListItem>
                                                <asp:ListItem Text="Both"></asp:ListItem>
                                            </asp:DropDownList>                                      
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </td>
            </tr>
        </table>
        <br />
        <!-- Cheque Details Section -->
        <asp:Panel ID="pnlchequedetails" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" width="98%" align="center">
                <tr>
                    <td align="left">
                        <div style="width: 99%; border-bottom: 1px dashed red;">
                        </div>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td align="left" style="width: 100%;">
                        <table cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr>
                                <td style="width: 20%;">
                                    <div style="text-align: left; font-weight: bold; background-color: #5a5952; color: #ffffff;
                                        width: 205px; font-family: Calibri; font-size: 14px; height: 20px; padding-top: 5px;
                                        padding-left: 2px;">
                                        Chq/DD/NEFT/Online Details
                                    </div>
                                </td>
                                <td class="label" style="width: 30%;">
                                    Select Bank Account <b style="color: Red;">*</b>
                                </td>
                                <td class="txt_style" style="width: 50%;">                                 
                                            <asp:DropDownList ID="ddlBankAcc" class="textbox" Height="27px" Width="100%" runat="server">
                                            </asp:DropDownList>                                      
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <div>
                            <!--GridView -->
                          
                                    <asp:GridView ID="gvChequeDetails" runat="server" ShowFooter="true" Width="100%"
                                        AutoGenerateColumns="False" HeaderStyle-CssClass="glrecptgVHeader" >
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle Font-Names="Calibri" />
                                        <HeaderStyle BackColor="#5a5952" />
                                        <HeaderStyle ForeColor="#ffffff" />
                                        <HeaderStyle Font-Bold="true" />
                                        <HeaderStyle Font-Size="13px" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Sr. No.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtChqSrno" CssClass="textbox_readonly" Width="50px" Style="text-align: center;"
                                                        runat="server" Text='<%#Eval("Serialno") %>'></asp:TextBox>
                                                    <asp:HiddenField ID="hdnchqid" runat="server" Value='<%#Eval("ChequeId") %>' />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Chq/DD/NEFT/Online">
                                                <ItemTemplate>
                                                    <asp:HiddenField ID="hdnchqddneft" runat="server" Value='<%#Eval("Chq_DD_NEFT") %>' />
                                                    <asp:DropDownList ID="gvddlChqDDNeft" runat="server" CssClass="textbox" Width="80px"
                                                        onchange='return gridDDLChange(this)' Height="27px">
                                                        <asp:ListItem>Cheque</asp:ListItem>
                                                        <asp:ListItem>DD</asp:ListItem>
                                                        <asp:ListItem>NEFT</asp:ListItem>
                                                        <asp:ListItem>Online</asp:ListItem>
                                                    </asp:DropDownList>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Chq/DD/NEFT No.">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtChqNo" runat="server" CssClass="textbox" onkeypress="return isAlphaNumeric(event);"
                                                        Width="90px" Text='<%#Eval("ChequeNo") %>'></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Chq/DD/NEFT Date">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtChqDate" runat="server" CssClass="textbox" MaxLength="10" Width="100px"
                                                        Text='<%#Eval("ChequeDate") %>' onchange="return validchdate();" placeholder="dd/mm/yyyy"></asp:TextBox>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Bank Name">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="gvddlChqBank" runat="server" CssClass="textbox" Height="27px"
                                                        Width="240px" />
                                                    <asp:HiddenField ID="hdnbankid" runat="server" Value='<%#Eval("BankId") %>' />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div class="label" style="height: 100%; vertical-align: middle;">
                                                        <asp:Label ID="lblgvChqTotal" runat="server" Text="Total"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Amount">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtChqAmount" runat="server" MaxLength="7" onkeypress="return OnlyNumericEntry();"
                                                        CssClass="textbox" Width="70px" Text='<%#Eval("Amount") %>' onkeyup="return chqTotal();"
                                                        Style="text-align: center;"></asp:TextBox>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="gvtxtChqTotal" runat="server" MaxLength="7" onkeypress="return OnlyNumericEntry();"
                                                        CssClass="textbox_readonly" Width="70px" onkeyup="return chqTotal();" Style="text-align: center;">
                                                    </asp:TextBox>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Action" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                        OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                        Width="20px" Height="20px" OnClick="btnDelete_Click" />
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Button ID="btnChqAdd" runat="server" OnClientClick="return validcheque();" Text="Add"
                                                        OnClick="btnChqAdd_Click" />
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>                               
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <!-- Denomination Details Section -->
        <asp:Panel ID="pnlcashdetails" runat="server">
            <table cellpadding="0" cellspacing="0" border="0" width="98%" align="center">
                <tr>
                    <td align="left" style="width: 100%;">
                        <table cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr>
                                <td style="width: 20%;">
                                    <div style="text-align: left; font-weight: bold; background-color: #5a5952; color: #ffffff;
                                        width: 205px; font-family: Calibri; font-size: 14px; height: 25px; padding-top: 5px;
                                        padding-left: 2px;">
                                        Denomination Details
                                    </div>
                                </td>
                                <td class="label" style="width: 30%;">
                                    Select Cash Account<b style="color: Red;">*</b>
                                </td>
                                <td class="txt_style" style="width: 50%;">                                   
                                            <asp:DropDownList ID="ddlCashAcc" class="textbox" Height="27px" Width="100%" runat="server">
                                            </asp:DropDownList>                                      
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center">
                        <div>
                            <!--GridView -->
                          
                                    <asp:GridView ID="gvDenominationDetails" runat="server" ShowFooter="true" Width="100%"
                                        AutoGenerateColumns="False" HeaderStyle-CssClass="glrecptgVHeader">
                                        <AlternatingRowStyle BackColor="White" />
                                        <HeaderStyle Font-Names="Calibri" />
                                        <HeaderStyle BackColor="#5a5952" />
                                        <HeaderStyle ForeColor="#ffffff" />
                                        <HeaderStyle Font-Bold="true" />
                                        <HeaderStyle Font-Size="13px" />
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
                                                    <asp:TextBox ID="gvtxtDenoNo" runat="server" Width="120px" MaxLength="6" onkeypress="return OnlyNumericEntry();"
                                                        CssClass="textbox" onkeyup="return CalDeno();" Text='<%#Eval("Quantity") %>'
                                                        Style="text-align: center;"></asp:TextBox>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <div class="label">
                                                        <asp:Label ID="gvlblDenoTotal" runat="server" Text="Total" Style="text-align: center;"></asp:Label>
                                                    </div>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Total" ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtDenoTotal" runat="server" MaxLength="10" onkeyup="return CalDeno();"
                                                        CssClass="textbox_readonly" Width="150px" Text='<%#Eval("Total") %>' Style="text-align: center;"></asp:TextBox>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:TextBox ID="gvtxtDenoTotalAmt" runat="server" MaxLength="10" CssClass="textbox_readonly"
                                                        Width="150px" onkeyup="return CalDeno();" Style="text-align: center;"></asp:TextBox>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Note Nos." ItemStyle-VerticalAlign="Top">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="gvtxtDenoNoteNos" runat="server" onkeypress="return isAlphaNumChar2(event);"
                                                        TextMode="MultiLine" Text='<%#Eval("NoteNos") %>' CssClass="textbox" Style="height: 15px;
                                                        width: 170px; text-transform: uppercase; resize: vertical; text-align: left;"></asp:TextBox>
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
                           
                        </div>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <br />
        <!-- Amount Received By Details Section -->
        <table cellpadding="0" cellspacing="0" border="0" width="98%" align="center">
            <tr>
                <td colspan="4" align="left">
                    <br />
                    <div style="width: 99%; border-bottom: 1px dashed red;">
                    </div>
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="left">
                    <div style="text-align: left; font-weight: bold; background-color: #5a5952; color: #ffffff;
                        width: 205px; font-family: Calibri; font-size: 14px; height: 25px; padding-top: 5px;
                        padding-left: 2px;">
                        Office Details
                    </div>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 20%; text-align: left;">
                    Receipt Book No.
                    <td class="txt_style" style="width: 15%;">                     
                        <asp:TextBox ID="txtReceiptBook" runat="server" class="textbox" onkeypress="return OnlyNumericEntry();"
                            MaxLength="4"></asp:TextBox>                 
                    </td>
                    <td class="label" style="width: 20%; text-align: left; padding-left: 20px;">
                        Received From <b style="color: Red;">*</b>
                    </td>
                    <td class="txt_style" style="width: 43%;">
                        <asp:TextBox ID="txtRecvFrom" onkeypress="return lettersOnly(event);" MaxLength="80"
                            class="textbox textbox_GLreceipt" Width="97%" runat="server"></asp:TextBox>
                    </td>
                </td>
            </tr>
            <tr>
                <td class="label" style="text-align: left;">
                    Receipt No.
                </td>
                <td class="txt_style">                   
                    <asp:TextBox ID="txtReceipt" runat="server" class="textbox" MaxLength="4"></asp:TextBox>                
                </td>
                <td class="label" style="text-align: left; padding-left: 20px;">
                    Collected By <b style="color: Red;">*</b>
                </td>
                <td class="txt_style">
                    <asp:DropDownList ID="ddlCollectedBy" class="textbox" Height="27px" Width="100%"
                        runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="label">
                </td>
                <td class="txt_style">
                </td>
                <td class="label" style="text-align: left; padding-left: 20px;">
                    Cashier's Name
                </td>
                <td class="txt_style">
                    <asp:DropDownList ID="ddlCashier" class="textbox" Height="27px" Width="100%" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <asp:HiddenField ID="HiddenField1" runat="server" Value="0"></asp:HiddenField>
        </table>
    </div>  
  
   <div>
   <button type="button" onclick="insert()">
   submit
   </button>
   </div>
     <script>



         $('#MainContent_ddlInterestCurrentAcHead').mouseenter(function () {

             $.ajax({
                 type: "Post",
                 url: "GLReceiptNew.aspx/GetAccount",
                 data: '{}',
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     console.log(data.d.length);
                     $("#MainContent_ddlInterestCurrentAcHead").append("<option>--Select A/c Head--</option>");                    

                     $.each(data.d, function (key, value) {                        
                         $("#MainContent_ddlInterestCurrentAcHead").append($("<option></option>").val(value.AccountId).html(value.AccountName));
                         
                     });

                 },
                 failure: function (error) {

                 }
             });
         });


         $('#MainContent_ddlPenalCurrentAcHead').mouseenter(function () {

             $.ajax({
                 type: "Post",
                 url: "GLReceiptNew.aspx/GetAccount",
                 data: '{}',
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     console.log(data.d.length);                    
                     $("#MainContent_ddlPenalCurrentAcHead").append("<option>--Select A/c Head--</option>");  
                     $.each(data.d, function (key, value) {                        
                          $("#MainContent_ddlPenalCurrentAcHead").append($("<option></option>").val(value.AccountId).html(value.AccountName));
                       
                     });

                 },
                 failure: function (error) {

                 }
             });
         });


         $('#MainContent_ddlChargesCurrentAcHead').mouseenter(function () {

             $.ajax({
                 type: "Post",
                 url: "GLReceiptNew.aspx/GetAccount",
                 data: '{}',
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     console.log(data.d.length);
                     $("#MainContent_ddlChargesCurrentAcHead").append("<option>--Select A/c Head--</option>");  
                     $.each(data.d, function (key, value) {
                         $("#MainContent_ddlChargesCurrentAcHead").append($("<option></option>").val(value.AccountId).html(value.AccountName));
                       
                     });

                 },
                 failure: function (error) {

                 }
             });
         });


         $('#MainContent_ddlAdvIntAcHead').mouseenter(function () {

             $.ajax({
                 type: "Post",
                 url: "GLReceiptNew.aspx/GetAccount",
                 data: '{}',
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 success: function (data) {
                     console.log(data.d.length);                     
                     $("#MainContent_ddlAdvIntAcHead").append("<option>--Select A/c Head--</option>");
                     $.each(data.d, function (key, value) {
                       
                         $("#MainContent_ddlAdvIntAcHead").append($("<option></option>").val(value.AccountId).html(value.AccountName));

                     });

                 },
                 failure: function (error) {

                 }
             });
         });

    </script>
</asp:Content>

