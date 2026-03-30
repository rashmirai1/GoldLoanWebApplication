<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLGoldManagementReport.aspx.cs" Inherits="GLGoldManagementReport" EnableEventValidation="false"
    EnableViewStateMac="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function SelectAll(id) {
            var frm = document.forms[0];
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    frm.elements[i].checked = document.getElementById(id).checked;
                }
            }
        }
        function valid(txt) {
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;


            if (!pattern.test(txt.value)) {
                alert('Enter Date In Proper Format');
                return false;
            }
        }



        function validatedate(inputText) {
            var dateformat = /^(0?[1-9]|[12][0-9]|3[01])[\/\-](0?[1-9]|1[012])[\/\-]\d{4}$/;
            // Match the date format through regular expression  
            if (inputText.value.match(dateformat)) {
                //document.form1.text1.focus();  
                //Test which seperator is used '/' or '-'  
                var opera1 = inputText.value.split('/');
                var opera2 = inputText.value.split('-');
                lopera1 = opera1.length;
                lopera2 = opera2.length;
                // Extract the string into month, date and year  
                if (lopera1 > 1) {
                    var pdate = inputText.value.split('/');
                }
                else if (lopera2 > 1) {
                    var pdate = inputText.value.split('-');
                }
                var dd = parseInt(pdate[0]);
                var mm = parseInt(pdate[1]);
                var yy = parseInt(pdate[2]);
                // Create list of days of a month [assume there is no leap year by default]  
                var ListofDays = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31];
                if (mm == 1 || mm > 2) {
                    if (dd > ListofDays[mm - 1]) {
                        alert('Invalid Date');
                        inputText.value = '';
                        return false;

                    }
                }
                if (mm == 2) {
                    var lyear = false;
                    if ((!(yy % 4) && yy % 100) || !(yy % 400)) {
                        lyear = true;
                    }
                    if ((lyear == false) && (dd >= 29)) {
                        alert('Invalid Date');
                        inputText.value = '';
                        return false;

                    }
                    if ((lyear == true) && (dd > 29)) {
                        alert('Invalid Date');
                        inputText.value = '';
                        return false;

                    }
                }
            }
            else {
                alert("Invalid Date");
                inputText.value = '';
                return false;

            }
        }





        function datevalid(txt) {

            if (isNaN(txt.value.replace('/', '').replace('/', ''))) {

                txt.value = '';
                return false;
            }
        }
        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                //Get the Cell To find out ColumnIndex
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {

                        inputList[i].checked = true;
                    }
                    else {

                        inputList[i].checked = false;
                    }
                }
            }
        }
        function onlynumeric() {//Function for only numbers and /


            if ((event.keyCode < 48 || event.keyCode > 57) || (event.keycode == 191)) {
                event.returnValue = false;
            }
        }

        function validrecord() {
            var txtPeriodDateFrom = document.getElementById("<%=txtPeriodDateFrom.ClientID%>");
            var txtperiodtodate = document.getElementById("<%=txtperiodtodate.ClientID%>");

            if (txtPeriodDateFrom.value == '') {
                alert('Enter Period From');
                return false;
            }
            if (txtperiodtodate.value == '') {
                alert('Enter Period To');
                return false;
            }
            var StartDate = txtPeriodDateFrom.value;
            var EndDate = txtperiodtodate.value;
            var day = ("0" + StartDate.getDate()).slice(-2);
            var month = ("0" + (StartDate.getMonth() + 1)).slice(-2);
            var today = StartDate.getFullYear() + "-" + (month) + "-" + (day);
            alert(StartDate);



        }

        function CheckValidDate() {

            var txtPeriodDateFrom = document.getElementById("<%=txtPeriodDateFrom.ClientID%>");
            var txtperiodtodate = document.getElementById("<%=txtperiodtodate.ClientID%>");

            if (txtPeriodDateFrom.value == '') {

                alert('Enter Period From');
                return false;
            }
            if (txtperiodtodate.value == '') {

                alert('Enter Period To');
                return false;
            }

            var sdate = txtPeriodDateFrom.value.split('/');
            var edate = txtperiodtodate.value.split('/');

            stdate = new Date(sdate[2], sdate[1] * 1 - 1, sdate[0]);
            endate = new Date(edate[2], edate[1] * 1 - 1, edate[0]);


            if (stdate > endate) {

                alert('Period To date should be greater than Period From date.');
                return false;
            }

        }

    
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 17%;">
            </td>
            <td style="width: 37%;">
            </td>
            <td style="width: 2%;">
            </td>
            <td style="width: 5%;">
            </td>
            <td style="width: 37%;">
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
                        <td align="center" colspan="6" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Gold Vault Register"></asp:Label>
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
    </table>
    <table width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="20%">
                <asp:Label ID="lblPeriodFrom" runat="server" Text="Period From:"></asp:Label>
            </td>
            <td class="txt_style" style="text-align: left;" width="80%">
                <asp:TextBox ID="txtPeriodDateFrom" class="textbox" runat="server" Width="25%" placeholder="dd/mm/yyyy"
                    onkeypress="return datevalid(this);" onchange="return validatedate(this);" MaxLength="10"></asp:TextBox>
                <asp:CalendarExtender ID="txtPeriodDateFrom_CalendarExtender" runat="server" Format="dd/MM/yyyy"
                    PopupButtonID="btnImgCalender" Enabled="True" TargetControlID="txtPeriodDateFrom"
                    CssClass="Calenderstyle">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="20%">
                <asp:Label ID="lblperiodto" runat="server" Text="Period To:"></asp:Label>
            </td>
            <td class="txt_style" style="text-align: left;" width="80%">
                <asp:TextBox ID="txtperiodtodate" class="textbox" runat="server" placeholder="dd/mm/yyyy"
                    Width="25%" onkeypress="return datevalid(this);" onchange="return validatedate(this);"
                    MaxLength="10"></asp:TextBox>
                <asp:CalendarExtender ID="txtperiodtodate_CalendarExtender1" runat="server" Format="dd/MM/yyyy"
                    PopupButtonID="btnImgCalender1" Enabled="True" TargetControlID="txtperiodtodate"
                    CssClass="Calenderstyle">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnImgCalender1" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
