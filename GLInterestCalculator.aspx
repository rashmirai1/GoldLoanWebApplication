<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="GLInterestCalculator.aspx.cs" Inherits="GLInterestCalculator" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
  
    <script language="javascript" type="text/javascript">
        function validcontrols() {

            var ddlLoanType = document.getElementById("<%=ddlLoanType.ClientID %>");
            var txtLoanAmount = document.getElementById("<%=txtLoanAmount.ClientID %>");
            var txtRoi = document.getElementById("<%=txtRoi.ClientID %>");
            var imgbtnExCustomer = document.getElementById("<%=imgbtnExCustomer.ClientID %>");
            var txtGoldLoanNo = document.getElementById("<%=txtGoldLoanNo.ClientID %>");

            if (ddlLoanType.selectedIndex == 2 || ddlLoanType.selectedIndex == 0) {

                txtLoanAmount.value = '';
                txtRoi.value = '';
                txtGoldLoanNo.value = '';

                txtLoanAmount.disabled = true;
                txtRoi.disabled = true;
                imgbtnExCustomer.disabled = false;
                txtGoldLoanNo.disabled = false;
                return false;
            }
            if (ddlLoanType.selectedIndex == 1) {

                txtLoanAmount.value = '';
                txtRoi.value = '';
                txtGoldLoanNo.value = '';

                txtLoanAmount.disabled = false;
                txtRoi.disabled = false;
                imgbtnExCustomer.disabled = true;
                txtGoldLoanNo.disabled = true;
                return false;
            }
            return true
        }
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        function numericwithtwvalue(text) {
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
        }
        function validdate(text) {

            var myDate1 = text.value;
            var date = myDate1.substring(0, 2);
            var month = myDate1.substring(3, 5);
            var year = myDate1.substring(6, 10);
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

            if (!pattern.test(text.value)) {
                alert('Invalid Date Format');
                text.value = '';
            }

            var myDate = new Date(year, month - 1, date);

            if (myDate < myDate1) {


                alert('Date Should Be Greater Than Current Date');
                text.value = '';
            }

        }
        function valid() {

            var ddlLoanType = document.getElementById("<%=ddlLoanType.ClientID %>");
            var txtLoanAmount = document.getElementById("<%=txtLoanAmount.ClientID %>");
            var txtRoi = document.getElementById("<%=txtRoi.ClientID %>");
            var imgbtnExCustomer = document.getElementById("<%=imgbtnExCustomer.ClientID %>");
            var txtGoldLoanNo = document.getElementById("<%=txtGoldLoanNo.ClientID %>");
            var txtDate = document.getElementById("<%=txtDate.ClientID %>");

            if (ddlLoanType.selectedIndex == 0) {

                alert('Select Loan Type');
                return false;
            }

            if (ddlLoanType.selectedIndex == 1) {

                if (txtLoanAmount.value == '') {

                    alert('Enter Loan Amount');
                    return false;
                }
                if (parseFloat(txtLoanAmount.value) <= 0) {

                    alert('Invalid Loan Amount');
                    return false;
                }
                if (txtRoi.value == '') {

                    alert('Enter ROI(%)');
                    return false;
                }
                if (parseFloat(txtRoi.value) <= 0) {

                    alert('Invalid ROI(%)');
                    return false;
                }
            }
            if (ddlLoanType.selectedIndex == 2) {


                if (txtGoldLoanNo.value == '') {

                    alert('Select Gold Loan No.');
                    return false;
                }
            }
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
       
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 98%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 26%;">
            </td>
            <td style="width: 20%;">
            </td>
            <td style="width: 32%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Interest Calculator"></asp:Label>
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
        <tr>
            <td class="label" style="text-align: left;">
                Loan Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlLoanType" CssClass="textbox" runat="server" Width="95%"
                    Height="26px" onchange="return validcontrols();">
                    <asp:ListItem>--Select--</asp:ListItem>
                    <asp:ListItem>New</asp:ListItem>
                    <asp:ListItem>Existing</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Loan Amount
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanAmount" onkeypress="return OnlyNumericEntry();" CssClass="textbox"
                    MaxLength="10" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                ROI(%)
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRoi" onkeyup="return numericwithtwvalue(this);" CssClass="textbox"
                    MaxLength="8" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Enter Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtDate" CssClass="textbox" runat="server" MaxLength="10" Width="82%"
                    onchange="ValidateDate(this, event.keyCode)" placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:CalendarExtender ID="txtBirthDate_CalendarExtender" runat="server" Format="dd/MM/yyyy"
                    Enabled="True" TargetControlID="txtDate" PopupButtonID="CalenderImg" CssClass="Calenderstyle">
                </asp:CalendarExtender>
                <asp:ImageButton ID="CalenderImg" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Gold Loan No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtGoldLoanNo" CssClass="textbox_readonly" MaxLength="15" runat="server"
                    Width="80%" Style="font-weight: bold;"></asp:TextBox>
                <asp:ImageButton ID="imgbtnExCustomer" ImageUrl="~/images/1397069814_Search.png"
                    Height="20px" Width="20px" runat="server" ImageAlign="AbsMiddle" ToolTip="Click for search existing customer"
                    OnClick="imgbtnExCustomer_Click" />
            </td>
            <td>
                <asp:HiddenField ID="hdnkycid" runat="server" Value="0" />
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div style="border: 1px dotted  #d23b1d;">
                </div>
                <br />
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; text-decoration: underline; font-weight: bold;"
                colspan="4">
                Loan History :-
            </td>
        </tr>
        <tr>
            <td class="txt_style" colspan="4" style="text-align: left;" valign="top">
                <asp:GridView ID="gvHistory" runat="server" Width="98%" ShowHeader="false" CssClass="textbox_readonly"
                    EmptyDataText="NO Record Found" OnRowDataBound="gvHistory_RowDataBound" BackColor="#faf4b3">
                    <RowStyle CssClass="gVItem" Font-Size="15px" ForeColor="#1f497d" />
                    <HeaderStyle HorizontalAlign="Center" BackColor="#1f497d" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; text-decoration: underline; font-weight: bold;"
                colspan="4">
                <br />
                <br />
                Interest Details :-
            </td>
        </tr>
        <tr>
            <td class="txt_style" colspan="4" style="text-align: left;" valign="top">
                <asp:GridView ID="gvDetails" runat="server" Width="98%" ShowHeader="true" CssClass="textbox_readonly"
                    EmptyDataText="NO Record Found" ForeColor="#ffffff" BackColor="#1f497d" OnRowDataBound="gvDetails_RowDataBound">
                    <RowStyle CssClass="gVItem" Font-Size="15px" ForeColor="#1f497d" BackColor="#faf4b3" />
                    <HeaderStyle HorizontalAlign="Center" ForeColor="#ffffff" />
                </asp:GridView>
            </td>
        </tr>
    </table>
</asp:Content>
