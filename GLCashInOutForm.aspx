<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLCashInOutForm.aspx.cs" Inherits="GLCashInOutForm" EnableEventValidation="false"
    EnableViewStateMac="true" %>

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
            grd.rows[grd.rows.length - 1].cells[3].children[0].value = totalamt;
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

        function validrecord() {
            var ddlInOutMode = document.getElementById("<%=ddlInOutMode.ClientID%>");
            var ddlInOutto = document.getElementById("<%=ddlInOutto.ClientID%>");
            var ddlInOuttoName = document.getElementById("<%=ddlInOuttoName.ClientID%>");

            var ddlInOutBy = document.getElementById("<%=ddlInOutBy.ClientID%>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");
            if (ddlInOutMode.selectedIndex == 0) {
                alert('Select Mode');
                return false;
            }


            if (hdnoperation.value == 'Save') {
                if (ddlInOutto.selectedIndex == 0) {


                    alert('Select Inward/Outward To');
                    return false;

                }
            }


            if (ddlInOuttoName.selectedIndex == 0) {
                alert('Select Inward/Outward To Name');
                return false;
            }







            var totalamt = 0;
            var grd = document.getElementById("<%=gvDenominationDetails.ClientID %>");
            for (i = 1; i < grd.rows.length - 1; i++) {

                var DenoRs = grd.rows[i].cells[1].children[0];
                var No = grd.rows[i].cells[2].children[0];
                var Total = grd.rows[i].cells[3].children[0];




                Total.value = parseFloat(DenoRs.value) * parseFloat(No.value);
                totalamt = parseFloat(totalamt) + parseFloat(Total.value);
            }
            grd.rows[grd.rows.length - 1].cells[3].children[0].value = totalamt;


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



            if (ddlInOutBy.selectedIndex == 0) {
                alert('Select Inward/Outward By');
                return false;
            }

        }


        function validMode() {

            var ddlInOutMode = document.getElementById("<%=ddlInOutMode.ClientID%>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");
            var ddlInOutto = document.getElementById("<%=ddlInOutto.ClientID %>");
            var ddlInOuttoName = document.getElementById("<%=ddlInOuttoName.ClientID %>");



            if (ddlInOutMode.selectedIndex != 0) {
                if (hdnoperation.value == 'Save') {

                    ddlInOutto.disabled = false;
                }
                if (hdnoperation.value == 'Update') {
                    ddlInOutto.disabled = true;
                }
            }

            if (ddlInOutMode.selectedIndex == 0) {

                if (hdnoperation.value == 'Save') {
                    ddlInOutto.disabled = false;

                }

            }








        }

        function validtype() {
            var ddlInOuttoName = document.getElementById("<%=ddlInOuttoName.ClientID %>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");




            var ddlInOutBy = document.getElementById("<%=ddlInOutBy.ClientID %>");
            var ddlInOutto = document.getElementById("<%=ddlInOutto.ClientID %>");





            if (ddlInOuttoName.selectedIndex != 0) {

                if (hdnoperation.value == 'Save') {

                    ddlInOutto.disabled = false;


                    ddlInOuttoName.disabled = false;


                    ddlInOutBy.disabled = false;
                }

            }

        }

        function validinoutfrom() {
            var ddlInOutfromName = document.getElementById("<%=ddlInOutfromName.ClientID %>");
            var hdnoperation = document.getElementById("<%=hdnoperation.ClientID %>");
            var ddlInOutMode = document.getElementById("<%=ddlInOutMode.ClientID%>");

            var ddlInOutto = document.getElementById("<%=ddlInOutto.ClientID %>");
            var ddlInOuttoName = document.getElementById("<%=ddlInOuttoName.ClientID %>");

            var ddlInOutfrom = document.getElementById("<%=ddlInOutfrom.ClientID %>")
            var ddlInOutBy = document.getElementById("<%=ddlInOutBy.ClientID %>");
            var gvDenominationDetails = document.getElementById("<%=gvDenominationDetails.ClientID %>");

            alert(ddlInOutfromName);
            if (ddlInOutfromName.selectedIndex == 0) {
                if (hdnoperation.value == 'Save') {



                    ddlInOutfromName.disabled = false;
                    ddlInOutBy.disabled = false;
                    ddlInOutfrom.disabled = false;
                    ddlInOutfromName.disabled = false;
                    ddlInOutto.disabled = false;
                    ddlInOuttoName.disabled = false;
                    gvDenominationDetails.disabled = false;
                }
            }


            if (ddlInOutfromName.selectedIndex != 0) {


                if (hdnoperation.value == 'Save') {



                    ddlInOutfromName.disabled = false;
                    ddlInOutBy.disabled = false;
                    ddlInOutfrom.disabled = false;
                    ddlInOutfromName.disabled = false;
                    ddlInOutto.disabled = false;
                    ddlInOuttoName.disabled = false;
                    gvDenominationDetails.disabled = false;
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
    <asp:HiddenField ID="hdncfname" runat="server" Value="0" />
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
                            <asp:Label ID="lblHeader" runat="server" Text="Cash Inward/Outward Details"></asp:Label>
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
    <!--<form design>-->
    <asp:Panel ID="panel1" runat="server" Width="100%">
        <table cellpadding="0" cellspacing="0" width="100%" id="table1">
            <tr>
                <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                    Reference No
                </td>
                <td class="txt_style" style="padding-right: 250px;" width="72%">
                    <asp:TextBox ID="txtreferenceno" CssClass="textbox_readonly textbox_GLreceipt" class="textbox"
                        onkeypress="return isAlphaNumChars(event);" runat="server" Width="90%" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <!-- DATE -->
                <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                    Date & Time
                </td>
                <td class="txt_style" style="padding-right: 250px;" width="72%">
                    <asp:TextBox ID="txtdate" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                        runat="server" Width="90%" MaxLength="50"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <!-- IN OUT Mode -->
                <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                    Select Mode<b style="color: Red;">*</b>
                </td>
                <td class="txt_style" style="padding-right: 250px;" width="72%">
                    <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlInOutMode" runat="server" Width="94%" Height="28px" AutoPostBack="true"
                                CssClass="textbox" OnSelectedIndexChanged="ddlInOutMode_SelectedIndexChanged">
                                <asp:ListItem Value="0">--Select Mode--</asp:ListItem>
                                <asp:ListItem Value="I">Inward</asp:ListItem>
                                <asp:ListItem Value="O">Outward</asp:ListItem>
                            </asp:DropDownList>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <!-- IN OUT Type -->
                <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                    Inward/Outward To<b style="color: Red;">*</b>
                </td>
                <td class="txt_style" style="padding-right: 250px;" width="72%">
                    <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlInOutto" runat="server" Width="94%" Height="28px" CssClass="textbox"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlInOutto_SelectedIndexChanged">
                                <asp:ListItem Value="0">--Select Inward/Outward To--</asp:ListItem>
                                <asp:ListItem Value="Cashier">Cashier</asp:ListItem>
                                <asp:ListItem Value="Bank/CashAcc">Bank/CashAcc</asp:ListItem>
                            </asp:DropDownList>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <!-- IN OUT name -->
                <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                    Inward/Outward To Name<b style="color: Red;">*</b>
                </td>
                <td class="txt_style" style="padding-right: 250px;" width="72%">
                    <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:DropDownList ID="ddlInOuttoName" runat="server" Width="94%" Height="28px" CssClass="textbox"
                                AutoPostBack="true" OnSelectedIndexChanged="ddlInOuttoName_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:TextBox ID="txtgoldname" runat="server" Visible="false" Width="90%" CssClass="textbox"></asp:TextBox>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlInOutto" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="panel2" runat="server" Width="100%">
        <table cellpadding="0" cellspacing="0" width="50%" style="padding-left: 17px;">
            <tr>
                <td class="label" style="text-align: left;" width="28%">
                    <asp:Label ID="Label6" runat="server" Text="Denomination Details:-" CssClass="label"
                        Font-Bold="true" Font-Underline="true" Style="margin: 0px; padding-right: 0px;"></asp:Label>
                </td>
            </tr>
            <tr>
                <td align="center" class="label">
                    <!--GridView -->
                    <div>
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:GridView ID="gvDenominationDetails" runat="server" AutoGenerateColumns="False"
                                    HeaderStyle-CssClass="glrecptgVHeader" ShowFooter="true" Width="70%" OnRowDataBound="gvDenominationDetails_RowDataBound">
                                    <AlternatingRowStyle BackColor="White" />
                                    <HeaderStyle CssClass="gVHeader" />
                                    <Columns>
                                        <asp:TemplateField HeaderText="Sr. No." ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="gvtxtDenoSrno" runat="server" CssClass="textbox_readonly textbox_GLreceipt"
                                                    Style="text-align: center; height: 23px;" Text='<%#Eval("Serialno") %>' Width="40px"></asp:TextBox>
                                                <asp:HiddenField ID="hdndenoid" runat="server" Value='<%#Eval("DenoId") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Denominations Rs." ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="gvtxtDenoDescription" runat="server" CssClass="textbox" MaxLength="4"
                                                    onkeypress="return OnlyNumericEntry();" Style="height: 23px; text-align: center;"
                                                    onkeyup="return CalDeno();" Text='<%#Eval("DenoRs") %>' Width="100px"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="No. (Quantity)" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="gvtxtDenoNo" runat="server" CssClass="textbox" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                                                    onkeyup="return CalDeno();" Style="height: 23px; text-align: center;" Text='<%#Eval("Quantity") %>'
                                                    Width="100px"></asp:TextBox>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <div class="label">
                                                    <asp:Label ID="gvlblDenoTotal" runat="server" Text="Total"></asp:Label>
                                                </div>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Total" ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="gvtxtDenoTotal" runat="server" CssClass="textbox_readonly" MaxLength="10"
                                                    onkeyup="return CalDeno();" Text='<%#Eval("Total") %>' Style="height: 23px; text-align: center;"
                                                    Width="100px"></asp:TextBox>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                <asp:TextBox ID="gvtxtDenoTotalAmt" runat="server" CssClass="textbox_readonly" MaxLength="10"
                                                    onkeyup="return CalDeno();" Width="100px" Style="height: 23px; text-align: center;"></asp:TextBox>
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Note Nos." ItemStyle-VerticalAlign="Top">
                                            <ItemTemplate>
                                                <asp:TextBox ID="gvtxtDenoNoteNos" runat="server" CssClass="textbox" onkeypress="return isAlphaNum(event);"
                                                    Style="height: 23px; width: 130px; text-transform: uppercase; resize: vertical;
                                                    text-align: left;" Text='<%#Eval("NoteNos") %>' TextMode="MultiLine"></asp:TextBox>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Action">
                                            <ItemTemplate>
                                                <asp:ImageButton ID="btnDenoDelete" runat="server" Height="20px" ImageUrl="~/images/DeleteRed.png"
                                                    OnClick="btnDenoDelete_Click" OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                    Width="20px" />
                                            </ItemTemplate>
                                            <ItemStyle HorizontalAlign="Center" />
                                            <FooterTemplate>
                                                <%--OnClick="BtnUploadCharges_Click"--%>
                                                <asp:Button ID="btnDenoAdd" runat="server" OnClick="btnDenoAdd_Click" OnClientClick="return AddDeno();"
                                                    Text="Add" />
                                            </FooterTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                </asp:GridView>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="ddlInOutto" EventName="SelectedIndexChanged" />
                                <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%" id="table3">
        <tr style="display: none">
            <!-- IN OUT from Type -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Inward/Outward From<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 250px;" width="72%">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutfrom" runat="server" Width="94%" Height="28px" CssClass="textbox"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlInOutfrom_SelectedIndexChanged">
                            <asp:ListItem Value="0">--Select Inward Outward From--</asp:ListItem>
                            <asp:ListItem Value="Cashier">Cashier</asp:ListItem>
                            <asp:ListItem Value="Bank/CashAcc">Bank/CashAcc</asp:ListItem>
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr style="display: none">
            <!-- IN OUT from name -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Inward/Outward From Name<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 250px;" width="72%">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutfromName" runat="server" Width="94%" Height="28px"
                            AutoPostBack="true" CssClass="textbox" OnSelectedIndexChanged="ddlInOutfromName_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutfrom" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- IN OUT by -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Inward/Outward By<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 250px;" width="72%">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutBy" runat="server" Width="94%" Height="28px" CssClass="textbox">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOuttoName" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutto" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
