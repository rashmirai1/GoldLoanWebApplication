<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="GLCashAuthorizationForm.aspx.cs" Inherits="GLCashAuthorizationForm" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">

        function OnlyNumericEntry() { //Function for only numbers
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
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
            grd.rows[grd.rows.length - 1].cells[4].children[0].value = totalamt;
            if (grd.rows[grd.rows.length - 1].cells[4].children[0].value == '') {

                grd.rows[grd.rows.length - 1].cells[4].children[0].value = 0;
            }



            var denocoins = grd.rows[grd.rows.length - 1].cells[1].children[0];
            var coinsNo = grd.rows[grd.rows.length - 1].cells[2].children[0];



            if (denocoins.value == '') {
                denocoins.value = 0;
            }
            if (coinsNo.value == '') {
                coinsNo.value = 0;

            }



            totalamt = parseFloat(grd.rows[grd.rows.length - 1].cells[4].children[0].value) + parseFloat(denocoins.value);

            grd.rows[grd.rows.length - 1].cells[4].children[0].value = totalamt;








        }
        function isAlphaNumChars(e) { // Alphanumeric,space,(),@,%,*,_,+,-,[]  only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 46 && k < 58) || (k > 63 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41 || k == 42 || k == 43 || k == 37 || k == 38 || k == 39 || k == 91 || k == 93 || k == 95) || k == 0);
        }
        function isAlphaNum(e) { // Alphanumeric,space,comma
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 44));
        }

        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }

        function validrecord(ochecked) {

            var amt = 0;
            var totalamt = 0;
            var grd = document.getElementById("<%=dgvReceiptsDetails.ClientID %>");
            var counter = 0;


            for (i = 1; i < grd.rows.length - 1; i++) {

                var chk = grd.rows[i].cells[12].children[0].type;

                var Row = grd.rows[i];
                var recamt = Row.cells[6];
                var ctrl = Row.cells[12].children[0];
                if (ctrl.type == "checkbox") {
                    if (ctrl.checked) {
                        totalamt = parseFloat(totalamt) + parseFloat(recamt.innerText);
                    }
                }


            }



            grd.rows[grd.rows.length - 1].cells[12].innerText = totalamt;




        }


        function valid() {

            var amt = 0;
            var totalamt = 0;
            var grd = document.getElementById("<%=dgvReceiptsDetails.ClientID %>");
            var counter = 0;

            if (grd.rows.length == 0) {

                alert("Cannot Save,No Record Found in Receipt");
                return false;
            }

            for (i = 1; i < grd.rows.length - 1; i++) {

                var chk = grd.rows[i].cells[12].children[0].type;

                var Row = grd.rows[i];
                var recamt = Row.cells[6];
                var ctrl = Row.cells[12].children[0];
                if (ctrl.type == "checkbox") {
                    if (ctrl.checked) {
                        totalamt = parseFloat(totalamt) + parseFloat(recamt.innerText);
                    }
                }


            }



            grd.rows[grd.rows.length - 1].cells[12].value = totalamt;




        }


        function SelectAll(oCheckbox) {
            var GridView1 = document.getElementById("<%=dgvReceiptsDetails.ClientID %>");
            for (i = 1; i < GridView1.rows.length; i++) {
                GridView1.rows[i].cells[12].getElementsByTagName("INPUT")[12].checked = oCheckbox.checked
                alert("jkj");
            }
        }


        function chkboxChecked(id, rowIndex) {
            //get reference of GridView control
            var grid = document.getElementById("<%= dgvReceiptsDetails.ClientID %>");
            //variable to contain the cell of the grid
            var checkboxCell;
            var labelCell;

            if (grid.rows.length > 0) {
                //loop starts from 1. rows[0] points to the header.
                for (i = 1; i < grid.rows.length; i++) {
                    //get the reference of first column
                    checkboxCell = grid.rows[rowIndex].cells[12];
                    labelCell = grid.rows[rowIndex].cells[6];
                    alert(labelCell.innerText);

                    //loop according to the number of childNodes in the cell
                    for (j = 0; j < checkboxCell.childNodes.length; j++) {
                        //if childNode type is CheckBox                 
                        if (checkboxCell.childNodes[j].type == "checkbox") {
                            // If the checkbox is checked then Show the Label alert
                            if (checkboxCell.childNodes[j].checked == true) {
                                alert(labelCell.innerText);  // You get your Label value here
                            }
                        }
                    }
                }
            }
        }

    


    </script>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnrefno" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnuserid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnfyid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnbranchid" runat="server" Value="0" />
    <asp:HiddenField ID="hdntime" runat="server" Value="0" />
    <asp:HiddenField ID="hnddate" runat="server" Value="0" />
    <asp:HiddenField ID="hdndemoAmt" runat="server" Value="0" />
    <asp:HiddenField ID="hdnfinalizedAmt" runat="server" Value="0" />
    <asp:HiddenField ID="hdnRefdate" runat="server" Value="0" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 10%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 30%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Cash Authorization Details"></asp:Label>
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
    </table>
    <table cellpadding="0" cellspacing="0" width="100%" id="table1">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;">
                Reference No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtreferenceno" CssClass="textbox_readonly textbox_GLreceipt" class="textbox"
                    Width="200px" runat="server"></asp:TextBox>
            </td>
            <!-- DATE -->
            <td class="label" style="text-align: left; padding-left: 17px;">
                Reference Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtdate" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                    runat="server" Width="200px"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <!-- Receipts Details Section -->
            <td class="label" style="text-align: left; padding-left: 17px; width: 13%;" valign="top">
                <asp:Label ID="Label6" runat="server" Text="Receipt Details:-" CssClass="label" Font-Bold="true"
                    Font-Underline="true" Style="margin: 0px; padding-right: 0px;"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px; width: 95%;">
                <asp:GridView ID="dgvReceiptsDetails" runat="server" ShowFooter="true" AutoGenerateColumns="false"
                    EmptyDataText="No Record Found in Receipt" OnRowDataBound="dgvReceiptsDetails_RowDataBound"
                    Width="96%">
                    <AlternatingRowStyle BackColor="White" />
                    <HeaderStyle CssClass="gVHeader" />
                    <Columns>
                        <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <%#Container.DataItemIndex+1%>
                                <asp:HiddenField ID="hndSDID" runat="server" Value='<%#Eval("SDID") %>' />
                                <asp:HiddenField ID="hndKYCID" runat="server" Value='<%#Eval("KYCID") %>' />
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ReceiptID" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"
                            Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblreceiptid" runat="server" Text='<%#Eval("RcptID") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblgoldloanno" runat="server" Text='<%#Eval("GoldLoanNo") %>' Width="100px"></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Left" />
                            <ItemStyle HorizontalAlign="Left" />
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblname" runat="server" MaxLength="3" Style="text-align: center;"
                                    Width="180px" Text='<%# Eval("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <asp:Label ID="lblreceiptdate" runat="server" MaxLength="8" Style="text-align: center;"
                                    Width="70px" Text='<%# Eval("ReceiptDate") %>'></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Payment Mode">
                            <ItemTemplate>
                                <asp:Label ID="lblmode" runat="server" MaxLength="8" Style="text-align: center;"
                                    Text='<%# Eval("PaymentMode") %>' Width="30px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Receipt Amount">
                            <ItemTemplate>
                                <asp:Label ID="lblreceiptamt" runat="server" MaxLength="8" Style="text-align: center;"
                                    Text='<%# Eval("ReceiptAmount") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Received Cash">
                            <ItemTemplate>
                                <asp:Label ID="lblreceivedcash" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("ReceivedCash") %>' Width="50px"></asp:Label>
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Receipt No">
                            <ItemTemplate>
                                <asp:Label ID="lblreceiptNo" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("ReceiptNo") %>' Width="45px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Principal">
                            <ItemTemplate>
                                <asp:Label ID="lblprincipal" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("Principal") %>' Width="45px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Interest">
                            <ItemTemplate>
                                <asp:Label ID="lblInterest" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("Interest") %>' Width="45px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Penal Interest">
                            <ItemTemplate>
                                <asp:Label ID="lblpenalint" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("PenalInterest") %>' Width="45px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterTemplate>
                            </FooterTemplate>
                            <FooterStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Charges">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                            <ItemTemplate>
                                <asp:Label ID="lblcharges" runat="server" Style="text-align: center; color: Black;"
                                    Text='<%# Eval("Charges") %>' Width="45px"></asp:Label>
                            </ItemTemplate>
                            <FooterTemplate>
                                <asp:Label ID="lbltotal" runat="server" Font-Bold="true">Grand Total:</asp:Label>
                            </FooterTemplate>
                            <FooterStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Finalize">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkfinalized" runat="server" onclick="javascript:return validrecord(this);"
                                    Checked='<%#Eval("Finalized")%>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:Label ID="lblgrandtotal" runat="server" Style="text-align: right; color: Black;"
                                    Text="0" />
                            </FooterTemplate>
                            <FooterStyle CssClass="gVItem" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px; padding-right: 10px;
                width: 20%;" valign="top">
                <asp:Label ID="Label1" runat="server" Text="Denomination Details:-" CssClass="label"
                    Font-Bold="true" Font-Underline="true" Style="margin: 0px; padding-right: 0px;"></asp:Label>
            </td>
        </tr>
        <tr>
            <td align="center" class="label" style="text-align: left; padding-left: 17px; width: 80%;">
                <asp:GridView ID="gvDenominationDetails" runat="server" AutoGenerateColumns="False"
                    HeaderStyle-CssClass="glrecptgVHeader" ShowFooter="true" Width="80%" OnRowDataBound="gvDenominationDetails_RowDataBound">
                    <AlternatingRowStyle BackColor="White" />
                    <HeaderStyle CssClass="gVHeader" />
                    <Columns>
                        <asp:TemplateField HeaderText="Sr. No." ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <asp:TextBox ID="gvtxtDenoSrno" runat="server" CssClass="textbox_readonly textbox_GLreceipt"
                                    Style="text-align: center; height: 20px;" Text='<%#Eval("Serialno") %>' Width="50px"></asp:TextBox>
                                <asp:HiddenField ID="hdndenoid" runat="server" Value='<%#Eval("DenoId") %>' />
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:Label ID="lblcoins" runat="server">Total coins:</asp:Label>
                            </FooterTemplate>
                            <FooterStyle CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Denominations Rs." ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <asp:TextBox ID="gvtxtDenoDescription" runat="server" CssClass="textbox_readonly textbox_GLreceipt"
                                    MaxLength="4" Style="height: 20px; text-align: center;" Text='<%#Eval("DenoRs") %>'
                                    Width="190px"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:TextBox ID="gvtxtDenocoins" runat="server" CssClass="textbox" MaxLength="4"
                                    onkeypress="return OnlyNumericEntry();" onkeyup="return CalDeno();" Style="height: 20px;
                                    text-align: center;" Text='0' Width="190px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="No. (Quantity)" ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <asp:TextBox ID="gvtxtDenoNo" runat="server" CssClass="textbox" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                                    onkeyup="return CalDeno();" Style="height: 20px; text-align: center;" Text='<%#Eval("Quantity") %>'
                                    Width="90px"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:TextBox ID="gvtxtDenonocoin" runat="server" CssClass="textbox" MaxLength="8"
                                    onkeypress="return OnlyNumericEntry();" onkeyup="return CalDeno();" Style="height: 20px;
                                    text-align: center;" Text='0' Width="90px"></asp:TextBox>
                            </FooterTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total" ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <asp:TextBox ID="gvtxtDenoTotal" runat="server" CssClass="textbox_readonly" MaxLength="10"
                                    onkeyup="return CalDeno();" Text='<%#Eval("Total") %>' Style="height: 20px; text-align: center;"
                                    Width="90px"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:Label ID="lbltotal" runat="server" Font-Bold="true" Style="text-align: right;">Total:</asp:Label>
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Right" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Note Nos." ItemStyle-VerticalAlign="Top">
                            <ItemTemplate>
                                <asp:TextBox ID="gvtxtDenoNoteNos" runat="server" CssClass="textbox" onkeypress="return isAlphaNum(event);"
                                    Style="height: 20px; width: 201px; text-transform: uppercase; text-align: left;
                                    resize: vertical;" Text='<%#Eval("NoteNos") %>' TextMode="MultiLine"></asp:TextBox>
                            </ItemTemplate>
                            <ItemStyle CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:TextBox ID="gvtxtDenoTotalAmt" runat="server" CssClass="textbox_readonly" MaxLength="10"
                                    onkeyup="return CalDeno();" Width="201px" Style="height: 20px; text-align: center;"
                                    Text="0"></asp:TextBox>
                            </FooterTemplate>
                            <FooterStyle CssClass="gVItem" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
