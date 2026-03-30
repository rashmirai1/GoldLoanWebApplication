<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCBankGoldDetailsEditForm.aspx.cs" Inherits="GLKYCBankGoldDetailsEditForm"
    Theme="GridViewTheme" EnableViewStateMac="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
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
        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 45))
        }

        function isNumericSlash(e) { // Numbers and slash
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 47));
        }


        function getfocus() {
            //            document.getElementById('btnSearch').focus()
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 45))
        }

        function KeyDownHandler(btn) {
            if (event.keyCode == 8) {
                event.returnValue = true;
                event.cancel = false;
                document.getElementById(btn).click();
            }
        }


        function handleEnter(obj, event) {
            var keyCode = event.keyCode ? event.keyCode : event.which ? event.which : event.charCode;
            if (keyCode == 8) {
                document.getElementById(obj).click();
                return false;
            }
            else {
                return true;
            }
        } 
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <%--<td style="width: 180px;">
            </td>
            <td style="width: 160px;">
            </td>
            <td style="width: 120px;">
            </td>
            <td style="width: 180px;">
            </td>--%>
            <td style="width: 18%;">
            </td>
            <td style="width: 36%;">
            </td>
            <td style="width: 15%;">
            </td>
            <td style="width: 30%;">
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
                        <td align="center" colspan="3" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL KYC Bank Gold Details"></asp:Label>
                        </td>
                        <td align="right">
                            <asp:Label ID="Label27" runat="server" Text="[Edit Section]" Font-Names="Verdana"
                                Font-Size="11px" ForeColor="#070c80"></asp:Label>
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
        <!--Form Design -->
        <!-- Reference Date (Date Format validation) -->
        <tr>
            <td class="label">
                Reference No
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlRefType" runat="server" CssClass="textbox" Height="27px"
                            Width="18%" OnSelectedIndexChanged="ddlRefType_SelectedIndexChanged" ClientIDMode="Static">
                        </asp:DropDownList>
                        <asp:DropDownList ID="ddlRefNo" runat="server" CssClass="textbox" AppendDataBoundItems="true"
                            Height="27px" Width="19.5%" OnSelectedIndexChanged="ddlRefNo_SelectedIndexChanged"
                            ClientIDMode="Static">
                        </asp:DropDownList>
                        <%--<asp:DropDownList ID="ddlRefID" runat="server" CssClass="textbox" Height="27px" AppendDataBoundItems="true"
                            Width="25%" OnSelectedIndexChanged="ddlRefID_SelectedIndexChanged" ClientIDMode="Static">
                        </asp:DropDownList>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Reference Date
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtRefDate" CssClass="textbox" runat="server" Width="36%" MaxLength="10"
                            onkeypress="return isNumericSlash(event);" placeholder="dd/mm/yyyy"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ErrorMessage="*"
                            ControlToValidate="txtRefDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True"></asp:RequiredFieldValidator>
                        <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txtRefDate"
                            Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                            Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                            Font-Bold="True"></asp:CompareValidator>
                    </ContentTemplate>
                    <Triggers>
                    
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Location Type
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlLocationType" runat="server" CssClass="textbox" Height="28px"
                            Width="86%" OnSelectedIndexChanged="ddlLocationType_SelectedIndexChanged">
                            <asp:ListItem>--Select Location Type--</asp:ListItem>
                            <asp:ListItem>Locker</asp:ListItem>
                            <asp:ListItem>OD</asp:ListItem>
                            <asp:ListItem>Office</asp:ListItem>
                            <asp:ListItem>Home</asp:ListItem>
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                      
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                         <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td class="label">
                Location No
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtLocationNo" CssClass="textbox" runat="server" MaxLength="8" onkeypress="return isAlphaNumChars(event);"
                            Width="210px"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                      
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                         <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Operator Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtOperator" ReadOnly="true" CssClass="textbox_readonly" runat="server"
                    Width="98.2%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Bank Name -->
            <td class="label">
                Bank Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel22" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlBankName" runat="server" CssClass="textbox" Height="27px"
                            Width="100%" OnSelectedIndexChanged="ddlBankName_SelectedIndexChanged" DataTextField="BankName"
                            DataValueField="BankID">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Branch Name -->
            <td class="label">
                Branch Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtBranchName" CssClass="textbox_readonly" runat="server" Width="98.2%"></asp:TextBox>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ErrorMessage="*"
                            ControlToValidate="txtBranchName" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                            Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlBankName" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Deposit From Date -->
            <td class="label">
                Deposit From Date
            </td>
            <td class="txt_style" style="width: 29%">
                <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDepositFromDate" CssClass="textbox" runat="server" Width="217.5px"
                            onkeypress="return isNumericSlash(event);" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="txtDepositFromDate" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Deposit To Date -->
            <td class="label">
                Deposit To Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDepositToDate" CssClass="textbox" runat="server" Width="210px"
                            onkeypress="return isNumericSlash(event);" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtDepositToDate" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator3" runat="server" ControlToValidate="txtDepositFromDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" Font-Bold="True"></asp:CompareValidator>
            </td>
            <td>
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtDepositToDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <!-- Rate of Interest -->
            <td class="label">
                Interest Rate
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtInterestRate" CssClass="textbox" runat="server" Width="217.5px"
                            MaxLength="5" onkeypress="return isNumeric(event);"></asp:TextBox>
                        <%--       <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="txtInterestRate" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                        <asp:RegularExpressionValidator ID="RevInterestRate" runat="server" ControlToValidate="txtInterestRate"
                            Display="Dynamic" ForeColor="red" Font-Size="Small" ErrorMessage="Enter valid number.[eg.2.5]"
                            ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <td class="label">
                Bank Customer Id
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtUniqueID" CssClass="textbox" runat="server" Width="210px" onkeypress="return isAlphaNumChars(event);"
                            MaxLength="8"></asp:TextBox>
                        <%-- <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtDepositToDate" ForeColor="Red" Display="Dynamic" Font-Bold="True"
                    Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>--%>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:TextBox ID="txtFYID" runat="server" Visible="false"></asp:TextBox>
            </td>
            <td>
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="16px" Width="48px"></asp:TextBox>
                <asp:TextBox ID="txtBGID" runat="server" Visible="false" Height="16px" Width="48px"></asp:TextBox>
                <asp:TextBox ID="txtOperatorID" runat="server" Visible="false" Height="16px" Width="48px"></asp:TextBox>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Select Customer Section -->
            <td colspan="4" align="left">
                <br />
                <br />
                <asp:Label ID="Label6" runat="server" Text="Select Customer(s) :" CssClass="label"
                    Font-Size="16px" Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="divCustomerDetails" runat="server">
                    <!--GridView Customer Details -->
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvCustomerDetails" runat="server" OnPageIndexChanging="dgvCustomerDetails_PageIndexChanging"
                                Width="100%" AutoGenerateColumns="False" AllowPaging="True" PageSize="20">
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
                                    <%-- <asp:BoundField DataField="SDID" HeaderText="ID" Visible="False" />--%>
                                    <asp:TemplateField HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldLoanNo" align="center" runat="server" Text='<%# Eval("GoldLoanNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Applicant Name" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblApplicantName" align="center" runat="server" Text='<%# Eval("ApplicantName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mobile No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMobileNo" align="center" runat="server" Text='<%# Eval("MobileNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Total NetWeight" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblTotalNetWeight" align="center" runat="server" Text='<%# Eval("TotalNetWeight") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="AppID" ItemStyle-HorizontalAlign="Center" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAppID" align="center" runat="server" Text='<%# Eval("AppID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <%--  <asp:BoundField DataField="GoldLoanNo" HeaderText="Gold Loan No" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="ApplicantName" HeaderText="Applicant Name" ItemStyle-HorizontalAlign="Center" />
                                    <asp:BoundField DataField="TotalNetWeight" HeaderText="Total NetWeight" ItemStyle-HorizontalAlign="Center" />--%>
                                    <asp:TemplateField HeaderText="Select?" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" OnCheckedChanged="ChkChanged" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ba2f14" Font-Bold="false" ForeColor="#ffffcc" Height="24px"
                                    Font-Names="Calibri" Font-Size="15px" />
                                <PagerSettings PageButtonCount="20" />
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
                            <asp:PostBackTrigger ControlID="dgvCustomerDetails" />
                            <asp:AsyncPostBackTrigger ControlID="ddlRefNo" EventName="SelectedIndexChanged" />
                            <asp:AsyncPostBackTrigger ControlID="ddlRefType" EventName="SelectedIndexChanged" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="4">
                <br />
                <asp:Button ID="btnUpdate" runat="server" Text="Update" CssClass="button" OnClick="btnUpdate_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnCancel" runat="server" Text="Cancel" CssClass="button" OnClick="btnCancel_Click" />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="right" colspan="4">
                <asp:Label ID="Label28" runat="server" Text="[Search Section]" Font-Names="Verdana"
                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
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
            <td class="label">
                Search By:
            </td>
            <td class="txt_style" style="width: 29%">
                <asp:DropDownList ID="ddlSearchBy" Width="63%" runat="server" class="textbox_search"
                    Height="26px">
                </asp:DropDownList>
            </td>
            <td class="label">
                Search Text:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" class="textbox_search"
                    AutoPostBack="true" onblur="getfocus()" onkeypress=" return KeyDownHandler('btnSearch');"
                    OnTextChanged="txtSearch_TextChanged"></asp:TextBox>
                &nbsp;&nbsp;
                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <br />
            </td>
        </tr>
        <tr>
            <td align="center" valign="middle" colspan="4">
                <div class="dotted_bar">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <!--GridView -->
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="BankGoldID" OnPageIndexChanging="dgvDetails_PageIndexChanging"
                            OnRowCommand="dgvDetails_RowCommand" Width="100%" AutoGenerateColumns="False"
                            AllowPaging="True" PageSize="20" 
                            onselectedindexchanged="dgvDetails_SelectedIndexChanged">
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
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                    Visible="false" FooterStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblBankGoldID" runat="server" Visible="false" Text='<%# Eval("BankGoldID") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <%--  <asp:BoundField DataField="BankGoldID" HeaderText="BankGoldID" Visible="False" ItemStyle-HorizontalAlign="Center" />--%>
                                <asp:BoundField DataField="ReferenceDate" HeaderText="Reference Date" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="ReferenceNo" HeaderText="Reference No" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="LocationType" HeaderText="Location Type" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="LocationNo" HeaderText="Location No" ItemStyle-HorizontalAlign="Center" />
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center" HeaderText="DepositeFromDate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDepositeFromDate" runat="server" Text='<%# Convert.ToString(Eval("DepositeFromDate")).Equals("01/01/1900")?"-":Eval("DepositeFromDate") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center" HeaderText="RefType" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRefType" runat="server" Text='<%# Eval("RefType") %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <%--<asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                    FooterStyle-HorizontalAlign="Center" HeaderText="DepositeTODate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDepositeToDate" runat="server" Text='<%# Convert.ToString(Eval("DepositeToDate")).Equals("01/01/1900")?"-":Eval("DepositeToDate") %>'>
                                            </asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>--%>
                                <%--  <asp:BoundField DataField="DepositeFromDate" HeaderText="Deposit FromDate" ItemStyle-HorizontalAlign="Center" />--%>
                                <%--   <asp:BoundField DataField="DepositeToDate" HeaderText="Deposit ToDate" ItemStyle-HorizontalAlign="Center" />--%>
                                <asp:BoundField DataField="RateOfInterest" HeaderText="Rate Of Interest" ItemStyle-HorizontalAlign="Center" />
                                <asp:BoundField DataField="BankName" HeaderText="Bank Name" ItemStyle-HorizontalAlign="Center"
                                    Visible="false" />
                                <asp:BoundField DataField="Branch" HeaderText="Branch" ItemStyle-HorizontalAlign="Center"
                                    Visible="false" />
                                <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="left">
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnEdit" runat="server" ImageUrl="~/images/edit-icon.png" Width="18px"
                                            Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
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
                        <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <%--        <tr>
            <td>
                <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDepFrmDate" CssClass="textbox" runat="server" Width="21px" OnTextChanged="txtDepFrmDate_TextChanged" onblur="fillDepositDate();"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlRefID" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtDepFrmDate" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>--%>
    </table>
    <script type="text/javascript">        fillDepositDate();</script>
</asp:Content>
