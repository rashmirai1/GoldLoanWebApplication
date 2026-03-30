<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="ChargesMaster.aspx.cs" Inherits="ChargesMaster" EnableViewStateMac="false"
    EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server"> 
    <script type="text/javascript">

        //DatePicker

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

        function isNumeric(e) { // Numbers and back slash
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 47));
        }

        function OnlyNumericEntry() { //Function for only numbers
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        function CheckNumber() {

            var gvChrg = document.getElementById('<%=dgvChargeDetails.ClientID %>');
            var Charges = gvChrg.rows[gvChrg.rows.length - 1].cells[3].children[0];
            if (isNaN(Charges.value)) {
                Charges.value = '';
            }

        }
        function validCharge() {
            var chargeName = document.getElementById('<%=txtChargeName.ClientID %>');
            var refdate = document.getElementById('<%=txtRefDate.ClientID %>');
            var status = document.getElementById('<%=ddlStatus.ClientID %>');
            var gvChrg = document.getElementById('<%=dgvChargeDetails.ClientID %>');



            if (chargeName.value == '') {
                alert('Enter Charge Name');
                return false;
            }
            if (refdate.value == '') {
                alert('Enter Reference Date');
                return false;
            }
            if (status.value == '--Select Status--') {
                alert('Select Status');
                return false;
            }

            for (i = 1; i < gvChrg.rows.length - 1; i++) {

                var LoanAmtfrm = gvChrg.rows[i].cells[1].getElementsByTagName("span");
                var LoanAmtTo = gvChrg.rows[i].cells[2].getElementsByTagName("span");
                if (LoanAmtfrm[0].innerHTML == '' && LoanAmtTo[0].innerHTML == '') {
                    alert('Add Charges Details');
                    return false;
                }
            }
        }



        function validGrid() {
            var gvChrg = document.getElementById('<%=dgvChargeDetails.ClientID %>');
            var Charges = gvChrg.rows[gvChrg.rows.length - 1].cells[3].children[0];
            var ChargeType = gvChrg.rows[gvChrg.rows.length - 1].cells[4].children[0];
            var LoanAmtFrom = gvChrg.rows[gvChrg.rows.length - 1].cells[1].children[0];
            var LoanAmtTo = gvChrg.rows[gvChrg.rows.length - 1].cells[2].children[0];

            if (LoanAmtFrom.value == '') {
                alert('Enter Loan Amount(>=)');
                return false;
            }
            if (LoanAmtTo.value == '') {
                alert('Enter Loan Amount(<=)');
                return false;
            }
            if (parseFloat(LoanAmtFrom.value) >= parseFloat(LoanAmtTo.value)) {
                alert('Loan Amount(>=) Should Be Less Than Loan Amount(<=) ');
                return false;
            }
            if (ChargeType.value == 'Amount') {
                if (Charges.value == '') {
                    alert('Enter Charges Amount');
                    return false;
                }
                if (parseFloat(Charges.value) <= 0) {
                    alert('Charges Amount Should Be Greter Than One');
                    return false;
                }
            }
            else
                if (ChargeType.value == 'Percent') {
                    if (Charges.value == '') {
                        alert('Enter Charges Percent');
                        return false;
                    }
                    if (parseFloat(Charges.value) == 0) {
                        alert('Charges Percent Should Not Be Zero');
                        return false;
                    }
                    if (parseFloat(Charges.value) > 100) {
                        alert('Charges Percent Should Not Be Greater Than 100');
                        return false;
                    }
                }
        }

    </script>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnPopup" runat="server" Value="Edit" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 15%;">
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 15%;">
            </td>
            <td style="width: 45%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Charges Master"></asp:Label>
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
            <!-- Charge Name -->
            <td class="label">
                Charge Name<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtChargeName" onkeypress="return isAlphaNumChars(event);" class="textbox"
                    MaxLength="50" runat="server" Width="75%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Reference Date -->
            <td class="label">
                Reference Date<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRefDate" class="textbox" runat="server" placeholder="dd/mm/yyyy"
                    onkeypress="return isNumeric(event);" ClientIDMode="Static" Width="55%" MaxLength="15"></asp:TextBox>
                <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
                <asp:CalendarExtender ID="txtRefDate_CalendarExtender" runat="server" CssClass="Calenderstyle"
                    Enabled="True" PopupButtonID="btnImgCalender" TargetControlID="txtRefDate" Format="dd/MM/yyyy">
                </asp:CalendarExtender>
            </td>
            <!-- Status -->
            <td class="label">
                Status<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlStatus" runat="server" CssClass="textbox" Height="27px"
                    Width="55%">
                    <asp:ListItem Value="--Select Status--">--Select Status--</asp:ListItem>
                    <asp:ListItem Value="Active">Active</asp:ListItem>
                    <asp:ListItem Value="Inactive">Inactive</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div>
                    <br />
                    <br />
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvChargeDetails" Visible="true" DataKeyNames="ID" runat="server"
                                AutoPostBack="true" Width="100%" ShowFooter="true" AutoGenerateColumns="False">
                                <HeaderStyle CssClass="gVHeader" />
                                <RowStyle CssClass="gVItem" />
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdnID" runat="server" Value='<%# Eval("ID") %>' />
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Loan Amount (>=)" ItemStyle-HorizontalAlign="Right"
                                        HeaderStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanAmountFrom" Text='<%# Eval("LoanAmtFrom") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtLoanAmtFrom" Text="" Width="98%" runat="server" MaxLength="10"
                                                onkeypress="return isNumeric(event);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="Red"
                                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                                ControlToValidate="txtLoanAmtFrom"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revLoanAmtFrm" runat="server" ControlToValidate="txtLoanAmtFrom"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter Valid Amount."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                            <asp:RegularExpressionValidator ID="revLoanAmtFrm1" runat="server" ControlToValidate="txtLoanAmtFrom"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter 2 decimal value."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^(-)?\d+(\.\d\d)?$"></asp:RegularExpressionValidator>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Loan Amount (<=)" ItemStyle-HorizontalAlign="Right"
                                        HeaderStyle-HorizontalAlign="Center" FooterStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanAmountTo" Text='<%# Eval("LoanAmtTo") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtLoanAmountTo" Text="" Width="98%" runat="server" MaxLength="10"
                                                onkeypress="return isNumeric(event);"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ForeColor="Red"
                                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                                ControlToValidate="txtLoanAmountTo"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="revLoanAmtTo" runat="server" ControlToValidate="txtLoanAmountTo"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter Valid Amount."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                            <asp:RegularExpressionValidator ID="revLoanAmtTo1" runat="server" ControlToValidate="txtLoanAmountTo"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter 2 decimal value."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^(-)?\d+(\.\d\d)?$"></asp:RegularExpressionValidator>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Charges" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCharges" Text='<%# Eval("Charges") %>' runat="server"></asp:Label>
                                            <asp:HiddenField ID="hdnCID" runat="server" Value='<%# Eval("CID") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtCharges" onkeypress="CheckNumber();" Text="" Width="98%" runat="server"
                                                MaxLength="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ForeColor="Red"
                                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                                ControlToValidate="txtCharges"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="regcharges" runat="server" ControlToValidate="txtCharges"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter Valid Amount/Percent."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^[0-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                                            <asp:RegularExpressionValidator ID="regcharges1" runat="server" ControlToValidate="txtCharges"
                                                ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Enter 2 decimal value."
                                                SetFocusOnError="True" ValidationGroup="Upload" ValidationExpression="^(-)?\d+(\.\d\d)?$"></asp:RegularExpressionValidator>
                                            <br />
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Charge Type" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblChrgType" Text='<%# Eval("ChargeType") %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlChargeType" runat="server" Width="98%" Height="22px" DataTextField="ChargeType">
                                                <asp:ListItem Value="Amount">Amount</asp:ListItem>
                                                <asp:ListItem Value="Percent">Percent</asp:ListItem>
                                            </asp:DropDownList>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                OnClick="btnDelete_Click" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <%--  <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>--%>
                                            <asp:Button ID="BtnAdd" runat="server" Text="ADD" ValidationGroup="Upload" OnClick="BtnAdd_Click"
                                                OnClientClick="return validGrid();" />
                                            <%-- </ContentTemplate>
                                                <Triggers>
                                               <asp:PostBackTrigger ControlID="BtnAdd"/>
                                                </Triggers>
                                                </asp:UpdatePanel>--%>
                                        </FooterTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="dgvChargeDetails" EventName="Load" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <td>
            </td>
        </tr>
        <tr>
            <!--Save/Reset Button -->
            <td class="txt_style">
            </td>
            <td>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
        <tr>
            <td class="txt_style" colspan="4">
                <!-- ID -->
                <asp:TextBox ID="txtID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Chrg ID -->
                <asp:TextBox ID="txtChrgID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Branch ID -->
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- FY ID -->
                <asp:TextBox ID="txtFYID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Comp ID -->
                <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
</asp:Content>
