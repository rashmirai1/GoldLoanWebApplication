<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLSanctionDisburseDetails_EditDetails.aspx.cs" Theme="GridViewTheme"
    EnableViewStateMac="false" Inherits="GLSanctionDisburseDetails_EditDetails" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtIssueDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtChequeDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtDueDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });
        //disable DatePicker 
        function disableDate() {
            var accgpid = document.getElementById("<%=txtAccGPID.ClientID %>").value;
            if (accgpid == "70" || accgpid == "" || accgpid == null) {
                $('#<%=txtChequeDate.ClientID %>').dateEntry('disable');
            }
            else {
                $('#<%=txtChequeDate.ClientID %>').dateEntry('enable');
            }
        }
        //disable Due Date DatePicker 
        function disableDueDate() {
            var schemeid = document.getElementById("<%=ddlSchemeName.ClientID %>").value;
            if (schemeid == "0") {
                $('#<%=txtDueDate.ClientID %>').dateEntry('disable');
                document.getElementById("<%=txtNetAmountSanctioned.ClientID %>").enabled = false;
            }
            else {
                var schemetype = document.getElementById("<%=txtSchemeType.ClientID %>").value;
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

        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        function KeyUpEvent() {
            document.getElementById('<%=txtNetWeight.ClientID%>').value = document.getElementById('<%=txtTotalGrossWeight.ClientID%>').value - document.getElementById('<%=txtDeduction.ClientID%>').value
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }

        // validate lengh of text box according to Cheq Or NEFT or DD by kishor:08 oct 2014
        function TextValidate() {
            var txt = document.getElementById('<%=txtChequeNo.ClientID %>').value;
            var ddl = document.getElementById('<%=ddlcheqNEFTDD.ClientID %>').value;
            var selected = ddl.options[ddl.selectedIndex].Text;
            if (selected == "Cheque") {
                arguments.IsValid = txt.value.length > 0;
            } else if (selected == "NEFT") {
                arguments.IsValid = txt.value.length > 10;
            } else if (selected == "NEFT") {
            arguments.IsValid = txt.value.lenth > 6;
            }

        }

    </script>
    <asp:ScriptManager ID="ScriptManager2" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 20%;">
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
                        <td align="right" colspan="3" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL S&D Details"></asp:Label>
                        </td>
                        <td align="right" colspan="1">
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
        <tr>
            <!-- Gold Loan No -->
            <td class="label" style="height: 37px;">
                <asp:Label ID="Label26" runat="server" Text="Gold Loan No." Font-Bold="true"></asp:Label>
                <asp:TextBox ID="txtSID" runat="server" Visible="false" Height="16px" Width="62px"></asp:TextBox>
            </td>
            <td class="txt_style" valign="middle">
                <asp:TextBox ID="txtGoldLoanNo" class="textbox_readonly" Width="10%" ReadOnly="true"
                    MaxLength="40" runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGoldLoanNo" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtGoldLoanNo"></asp:RequiredFieldValidator>
                <asp:DropDownList ID="ddlRefNo" runat="server" class="textbox" OnSelectedIndexChanged="ddlRefNo_SelectedIndexChanged"
                    AutoPostBack="true" Width="37%" Height="26px">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvRefNo" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" InitialValue="0"
                    ControlToValidate="ddlRefNo"></asp:RequiredFieldValidator>
                <asp:DropDownList ID="ddlRefID" runat="server" class="textbox" Width="32%" Height="26px"
                    AutoPostBack="True" OnSelectedIndexChanged="ddlRefId_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="rfvRefId" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" InitialValue="0"
                    ControlToValidate="ddlRefNo"></asp:RequiredFieldValidator>
            </td>
            <!-- Loan Date -->
            <td class="label" valign="middle">
                Loan Date
            </td>
            <td class="txt_style" valign="middle">
                <asp:TextBox ID="txtIssueDate" CssClass="textbox" Width="35%" runat="server" MaxLength="10"
                    placeholder="dd/mm/yyyy" OnTextChanged="txtIssueDate_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="Red"
                    Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtIssueDate"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <!-- Loan Date (Date Format validation) -->
        <tr>
            <td colspan="3">
            </td>
            <td>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtIssueDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="save"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="3">
                <!-- Custom Validator for Issue Date -->
                <asp:CustomValidator runat="server" ID="CustomValidator4" ControlToValidate="txtIssueDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="txtIssueDate_ServerValidate" ErrorMessage="Loan Date must be within Financial Year. Please log into correct Financial Year." />
            </td>
        </tr>
        <tr>
            <!-- Operator Name -->
            <td class="label">
                Operator Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtOperatorName" class="textbox_readonly " ReadOnly="true" Width="75.5%"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvOperatorName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtOperatorName"></asp:RequiredFieldValidator>
                &nbsp;<asp:TextBox ID="txtOperatorID" Visible="false" runat="server" Height="16px"
                    Width="89px"></asp:TextBox>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Customer Name -->
            <td class="label">
                Customer Name
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtCustomerName" class="textbox_readonly " ReadOnly="true" Width="75.5%"
                    runat="server"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvCustName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtCustomerName"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!-- Gold Loan Type -->
            <td class="label">
                Gold Loan Type
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoantype" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="81%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvLoanType" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtLoantype"></asp:RequiredFieldValidator>
            </td>
            <!-- Existing PL Case No -->
            <td class="label">
                Existing PL-Case No.
            </td>
            <td>
                <asp:TextBox ID="txtPLCaseNo" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="35%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Pan No -->
            <td class="label">
                Pan No
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPanNo" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="81%"></asp:TextBox>
            </td>
            <!-- Gender -->
            <td class="label" align="center">
                Gender
            </td>
            <td>
                <asp:TextBox ID="txtGender" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="35%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvGender" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtGender"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!-- Birth Date -->
            <td class="label" align="center">
                Birth Date
            </td>
            <td>
                <asp:TextBox ID="txtBirthDate" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="38%"></asp:TextBox>
                <!-- Age -->
                <asp:TextBox ID="txtAge" class="textbox_readonly" ReadOnly="true" runat="server"
                    Style="text-align: right;" Width="38%"></asp:TextBox>
                <asp:Label ID="Label16" runat="server" Text="(Yrs)" Font-Names="Calibri" Font-Size="12px"></asp:Label>
                <asp:RequiredFieldValidator ID="rfvBirthDate" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtBirthDate"></asp:RequiredFieldValidator>
                <%--<asp:RequiredFieldValidator ID="rfv" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtAge"></asp:RequiredFieldValidator>--%>
            </td>
            <!-- Marital Status -->
            <td class="label">
                Marital Status
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMaritalStatus" class="textbox_readonly" ReadOnly="true" runat="server"
                    Width="35%"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvMaritalStatus" runat="server" ForeColor="Red"
                    Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtMaritalStatus"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!-- Address -->
            <td class="label">
                Customer's Address
            </td>
            <td class="txt_style" colspan="3" style="height: 55px;">
                <asp:TextBox ID="txtAddress" class="textbox_readonly " Height="85%" ReadOnly="true"
                    Width="75.5%" runat="server" TextMode="MultiLine"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAddress" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtAddress"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <!-- Nominee -->
            <td class="label">
                Nominee Name
            </td>
            <td colspan="3" class="txt_style">
                <asp:TextBox ID="txtNominee" class="textbox_readonly " ReadOnly="true" Width="75.5%"
                    runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Nominee Relationship -->
            <td class="label">
                Nominee Relationship
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNomineeRelationship" class="textbox_readonly" ReadOnly="true"
                    Width="31.5%" runat="server"></asp:TextBox>
                <asp:Label ID="lblRelatonship" runat="server" class="label" Text="(Relationship with customer)"
                    Font-Size="12px" Font-Italic="true"></asp:Label>
            </td>
        </tr>
        <tr>
            <!-- Bank/Cash Account -->
            <td class="label">
                Bank/Cash A/C
            </td>
            <td class="txt_style" colspan="3">
                <asp:UpdatePanel ID="UpdatePanel11" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCashAccount" runat="server" CssClass="textbox" Height="27px"
                            Width="77.5%" ClientIDMode="Static" OnSelectedIndexChanged="ddlCashAccount_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ErrorMessage="*"
                            ControlToValidate="ddlCashAccount" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Size="Medium" SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>

         <tr>
            <td class="label">
               Chq/DD/NEFT
            </td>
            <td class="txt_style" colspan="3">
           <asp:UpdatePanel ID="UpdatePanel22" runat="server" UpdateMode="Conditional">
           <ContentTemplate>
           <asp:DropDownList ID="ddlcheqNEFTDD" runat="server" CssClass="textbox" Height="27px"
            Width="77.5%" ClientIDMode="Static" 
                   onselectedindexchanged="ddlcheqNEFTDD_SelectedIndexChanged" AutoPostBack="true">
                  
               <asp:ListItem Value="">---Select ---</asp:ListItem>
               <asp:ListItem Value="Cheque">Cheque</asp:ListItem>
               <asp:ListItem Value="NEFT">NEFT</asp:ListItem>
               <asp:ListItem Value="DD">DD</asp:ListItem>
               </asp:DropDownList>

           </ContentTemplate>
           </asp:UpdatePanel>
              
            </td>
        </tr>
        <tr>
            <!-- Cheque Number -->
            <td class="label">
                Chq/DD/NEFT No
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtChequeNo" class="textbox" runat="server" onkeypress="return isAlphaNumChars();"
                    Style="text-align: right;" Width="81%" MaxLength="6" 
                    ontextchanged="txtChequeNo_TextChanged" AutoPostBack="true"></asp:TextBox>
            </td>
            <!-- Cheque Date -->
            <td class="label" align="center">
                Chq/DD/NEFT Date
            </td>
            <td>
                <asp:TextBox ID="txtChequeDate" class="textbox" runat="server" Width="35%" MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Gold Item Details Section -->
            <td colspan="4" align="left">
                <br />
                <br />
                <asp:Label ID="Label4" runat="server" Text="Gold Item Details :" CssClass="label"
                    Font-Size="16px" Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <!-- Gold Item Details Section -->
            <td colspan="4" align="center">
                <div>
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvGoldItemDetails" runat="server" DataKeyNames="GID" ShowFooter="true"
                                OnRowCommand="dgvGoldItemDetails_RowCommand" OnPageIndexChanging="dgvGoldItemDetails_PageIndexChanging"
                                OnDataBound="dgvGoldItemDetails_DataBound" AutoGenerateColumns="False">
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1%>
                                        </ItemTemplate>
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
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gold Item Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldItemName" align="center" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlGoldItemName" runat="server" Height="22px" DataSourceID="SqlDataSource1"
                                                DataTextField="ItemName" DataValueField="ItemID" Width="200px">
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                                                SelectCommand="SELECT distinct ItemID, ItemName FROM tblItemMaster ORDER BY ItemName">
                                            </asp:SqlDataSource>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Weight (g)" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGrossWeight" runat="server" Text='<%# Eval("GrossWeight") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtGrossWeight1" onkeypress="return isAlphaNumCharsDot(event);"
                                                MaxLength="8" runat="server" Width="90px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvNetWeight" runat="server" ForeColor="Red" ErrorMessage="*"
                                                Font-Size="Medium" Display="Dynamic" ValidationGroup="Upload" ControlToValidate="txtGrossWeight1"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtQuantity" onkeypress="return OnlyNumericEntry();" MaxLength="3"
                                                runat="server" Width="90px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="rfvQuantity" runat="server" ForeColor="Red" ErrorMessage="*"
                                                Font-Size="Medium" Display="Dynamic" ValidationGroup="Upload" ControlToValidate="txtQuantity"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Photo" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Image ID="imgPhoto" runat="server" Width="40px" Height="50px" ImageUrl='<%#Eval("ImageUrl") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:FileUpload ID="FileUpload1" Height="70%" runat="server" Width="200px" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ForeColor="Red"
                                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                                ControlToValidate="FileUpload1"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="PhotoPath" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblPath" runat="server" Text='<%# Eval("PhotoPath") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Image" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblImageName" runat="server" Text='<%# Eval("ImageName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center"
                                        HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Button ID="BtnUpload" runat="server" Text="Upload" OnClick="BtnUpload_Click"
                                                ValidationGroup="Upload" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvGoldItemDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <br />
                <br />
                <!-- Custom Validator for Document Details -->
                <asp:CustomValidator ID="CustomValidator2" runat="server" ErrorMessage="*" ForeColor="Red"
                    SetFocusOnError="True" Font-Bold="True" ValidationGroup="save" Text="Select Gold Item Details."
                    OnServerValidate="dgvGoldItemDetails_ServerValidate" Display="Dynamic" ValidateEmptyText="True"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <!-- Total Gross Weight -->
            <td class="label">
                <asp:Label ID="lblGrossWeight" runat="server" Text="Total Gross Weight" Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtTotalGrossWeight" class="textbox_readonly" runat="server" onkeypress="return isAlphaNumCharsDot(event);"
                            OnTextChanged="txtTotalGrossWeight_TextChanged" Style="text-align: right"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvGrossWeight" runat="server" ForeColor="Red" ErrorMessage="*"
                            Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtTotalGrossWeight"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <!-- Scheme Name-->
            <td class="label">
                <asp:Label ID="Label3" runat="server" Text="Scheme Name"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlSchemeName" runat="server" CssClass="textbox" Width="97%"
                    Height="27px" OnSelectedIndexChanged="SchemeName_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ReqSchemeName" runat="server" ErrorMessage="*" ControlToValidate="ddlSchemeName"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Size="Medium" SetFocusOnError="True"
                    InitialValue="0">*</asp:RequiredFieldValidator>
                <%-- <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:ComboBox ID="ddlSchemeName" runat="server" Height="18px" Width="180px" DataSourceID="SqlDataSource1"
                            DataTextField="SchemeName" DataValueField="ID" AutoCompleteMode="SuggestAppend"
                            DropDownStyle="DropDownList" MaxLength="50" RenderMode="Block" ItemInsertLocation="OrdinalText"
                            AppendDataBoundItems="False" CssClass="ajaxcombo">
                        </asp:ComboBox>
                       <asp:RequiredFieldValidator ID="ReqSchemeName" runat="server" ErrorMessage="*" ControlToValidate="ddlSchemeName"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                            SelectCommand="SELECT DISTINCT ID, SchemeName FROM tbl_GLLoanSchemeMaster WHERE Status='Active' ORDER BY SchemeName">
                        </asp:SqlDataSource>
                    </ContentTemplate>
                </asp:UpdatePanel>--%>
            </td>
        </tr>
        <tr>
            <!-- Deduction (in gram)-->
            <td class="label">
                <asp:Label ID="lblDeduction" runat="server" Text="Deduction"></asp:Label>
                <asp:Label ID="lblShow" runat="server" Text="(in gram)"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtDeduction" class="textbox" onkeypress="return isAlphaNumCharsDot(event);"
                            MaxLength="5" onkeyup="return KeyUpEvent()" runat="server" OnTextChanged="txtDeduction_TextChanged"
                            AutoPostBack="True" Style="text-align: right"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ForeColor="Red"
                            Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="save"
                            ControlToValidate="txtDeduction"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <!-- Loan Tenure-->
            <td class="label">
                Loan Tenure
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanTenure" class="textbox" MaxLength="3" onkeypress="return OnlyNumericEntry();"
                    runat="server" Style="text-align: right" OnTextChanged="LoanTenure_TextChanged"></asp:TextBox>
                <%--<asp:Label ID="lblDayMnth" runat="server" Text="Days" Font-Names="Calibri" Font-Size="11px"></asp:Label>--%>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ForeColor="Red"
                    ErrorMessage="*" Font-Size="Medium" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtLoanTenure"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <!-- Validation for Deduction in Gross Weight -->
                <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Font-Size="12px"></asp:Label>
            </td>
            <td colspan="3">
            </td>
        </tr>
        <tr>
            <!-- Net Weight-->
            <td class="label">
                <asp:Label ID="lblNetWeight" runat="server" Text="Net Weight" Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtNetWeight" class="textbox_readonly" onkeypress="return isAlphaNumCharsDot(event);"
                            runat="server" Style="text-align: right"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNetWeight" runat="server" ForeColor="Red" ErrorMessage="*"
                            Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtNetWeight"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtDeduction" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="txtTotalGrossWeight" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Max Loan Amount (Allowed)-->
            <td class="label">
                <asp:Label ID="lblMaxLoanAmount" runat="server" Text="Max Loan Amount" Style="font-weight: 700"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtNetAmount" class="textbox_readonly" onkeypress="return isAlphaNumCharsDot(event);"
                            runat="server" Style="text-align: right"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvNetAmount" runat="server" ForeColor="Red" ErrorMessage="*"
                            Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtNetAmount"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlSanctionType" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtDeduction" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
            </td>
            <td colspan="3">
                <!-- Custom Validator for Net Amount -->
                <asp:CustomValidator runat="server" ID="CustomValidator1" ControlToValidate="txtNetAmount"
                    ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="txtNetAmount_ServerValidate" ValidationGroup="save" ErrorMessage="Net Amount must be greater than zero." />
            </td>
        </tr>
        <tr>
            <!-- Due Date -->
            <td class="label" align="center">
                Due Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtDueDate" class="textbox" runat="server" Width="80%" MaxLength="10"></asp:TextBox>
            </td>
            <!-- EMI -->
            <td class="label">
                EMI
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtEMI" class="textbox_readonly" ReadOnly="true" runat="server"
                    Style="text-align: right"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Sanctioned Loan Amt-->
            <td class="label">
                <asp:Label ID="lblAmountSanctiond" runat="server" Text="Sanctioned Loan Amt" Font-Bold="true"></asp:Label>
            </td>
            <!-- Login For excess loan amount-->
            <td colspan="1" class="txt_style" valign="middle">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtNetAmountSanctioned" class="textbox" MaxLength="8" onkeypress="return OnlyNumericEntry();"
                            runat="server" OnTextChanged="NetAmountSanctioned_TextChanged" AutoPostBack="true"
                            Style="text-align: right"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvSanctioned" runat="server" ForeColor="Red" ErrorMessage="*"
                            Font-Size="Medium" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtNetAmountSanctioned"></asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="txtNetAmountSanctioned" EventName="TextChanged" />
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlSanctionType" EventName="SelectedIndexChanged" />--%>
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- CIBIL Score-->
            <td class="label">
                CIBIL Score
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtCIBILScore" class="textbox" runat="server" onkeypress="return OnlyNumericEntry();"
                    Style="text-align: right" MaxLength="4"></asp:TextBox>
            </td>
            <%--<td class="txt_style" colspan="2">
                <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:CheckBox ID="chkLogin" runat="server" OnCheckedChanged="chkLogin_CheckedChanged"
                            AutoPostBack="True" />
                        <asp:Label ID="lblLogin" class="label" runat="server" Style="color: Red; font-weight: bold;"
                            Text="Login For excess loan amount"></asp:Label>
                        <asp:TextBox ID="txtLoginID" runat="server" Visible="false" Height="16px" Width="16px"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="chkLogin" />
                        <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancel" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>--%>
        </tr>
        <tr>
            <td>
            </td>
            <td class="txt_style" colspan="1">
                <asp:UpdatePanel ID="UpdatePanel10" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:CheckBox ID="chkLogin" runat="server" OnCheckedChanged="chkLogin_CheckedChanged"
                            AutoPostBack="True" />
                        <asp:Label ID="lblLogin" class="label" runat="server" Style="color: Red; font-weight: bold;"
                            Text="Login For excess loan amount"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <asp:PostBackTrigger ControlID="chkLogin" />
                        <asp:AsyncPostBackTrigger ControlID="btnLogin" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="btnCancel" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Proof of Ownership-->
            <td class="label">
                Proof of Ownership
                <br />
                <cite style="font-size: 10.5px;">(If gross weight more than 20g)</cite>
            </td>
            <td class="txt_style" rowspan="2" valign="top">
                <table>
                    <tr>
                        <td>
                            <asp:Image ID="imgProofOfOwnership" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" ImageUrl="" />
                        </td>
                        <td valign="top" align="left">
                            <asp:FileUpload ID="fUploadProofOfOwnership" runat="server" Width="85px" />
                            <br />
                            <asp:Button ID="btnUploadProofOfOwnership" runat="server" Text="Upload" OnClick="btnUploadProofOfOwnership_Click" />
                            <asp:ImageButton ID="btnRemoveProof" runat="server" AlternateText="Remove Copy" ImageAlign="Top"
                                ImageUrl="~/images/remove_button.png" Height="27px" Width="27px" OnClick="btnRemoveProof_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <!-- Message label -->
                <asp:UpdatePanel ID="UpdatePanel9" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Label ID="lblMessageText" class="label" runat="server" Font-Bold="True" Font-Names="Verdana"
                            Font-Size="11px" ForeColor="#fe0000"></asp:Label>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlSanctionType" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtNetAmountSanctioned" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:Panel ID="pnlLogin" Font-Size="Small" Visible="false" runat="server" Width="45%"
                            Style="margin-left: 160px; background-color: #ffbe00; border: 2px solid red;">
                            <table cellpadding="3" cellspacing="0" border="0">
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblUserName" class="label" runat="server" Text="UserName"> </asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtUserName" runat="server" class="textbox" MaxLength="30">
                                        </asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvUserName" runat="server" ForeColor="Red" ErrorMessage="*"
                                            Font-Size="Medium" Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtUserName"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td align="right">
                                        <asp:Label ID="lblPassword" class="label" runat="server" Text="Password"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" MaxLength="10" class="textbox"></asp:TextBox>
                                        <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ForeColor="Red" ErrorMessage="*"
                                            Font-Size="Medium" Display="Dynamic" ValidationGroup="Save" ControlToValidate="txtPassword"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" align="right">
                                        <asp:Label ID="lblLoginMsg" runat="server" Text="" Font-Bold="True" Font-Names="Verdana"
                                            Font-Size="10px" ForeColor="#fe0000"></asp:Label>
                                        <asp:Button ID="btnLogin" CssClass="button" ValidationGroup="Save" runat="server"
                                            Text="Login" OnClick="btnLogin_Click" />
                                        <asp:Button ID="btnCancel" CssClass="button" runat="server" Text="Cancel" OnClick="btnCancel_Click" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </ContentTemplate>
                    <Triggers>
                        <%--<asp:AsyncPostBackTrigger ControlID="ddlSanctionType" EventName="SelectedIndexChanged" />--%>
                        <asp:AsyncPostBackTrigger ControlID="txtNetAmountSanctioned" EventName="TextChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Charges Details Section -->
            <td colspan="4" align="left">
                <br />
                <br />
                <asp:Label ID="Label1" runat="server" Text="Charges Details :" CssClass="label" Font-Size="16px"
                    Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <!-- Charges Details Section -->
            <td colspan="4" align="center">
                <div>
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvChargesDetails" runat="server" DataKeyNames="CHID" ShowFooter="true"
                                Width="100%" AutoGenerateColumns="False" OnPageIndexChanging="dgvChargesDetails_PageIndexChanging"
                                OnRowCommand="dgvChargesDetails_RowCommand" OnRowDataBound="dgvChargesDetails_RowDataBound">
                                <AlternatingRowStyle BackColor="White" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No.">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CHID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCHID" align="center" runat="server" Text='<%# Eval("CHID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="SDID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSDID" align="center" runat="server" Text='<%# Eval("SDID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblID" align="center" runat="server" Text='<%# Eval("ID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="CID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCID" align="center" runat="server" Text='<%# Eval("CID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Charges Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblChargesName" align="center" runat="server" Text='<%# Eval("ChargeName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlChargesName" runat="server" Height="22px" DataSourceID="SqlDataSource3"
                                                DataTextField="ChargeName" DataValueField="CID" Width="200px">
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                                                OnSelecting="SqlDataSource3_Selecting" SelectCommand="SELECT DISTINCT CID, ChargeName FROM tbl_GLChargeMaster_BasicInfo WHERE Status='Active' AND ReferenceDate=(select MAX (ReferenceDate) from tbl_GLChargeMaster_BasicInfo where CID=(select max(CID) from tbl_GLChargeMaster_BasicInfo where ReferenceDate<=@RefDate)) ORDER BY ChargeName">
                                                <SelectParameters>
                                                    <asp:Parameter Name="RefDate" Type="string" />
                                                </SelectParameters>
                                            </asp:SqlDataSource>
                                            <%--<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:ComboBox ID="ddlGoldItemName" runat="server" Height="18px" Width="200px" DataSourceID="SqlDataSource3"
                                                        DataTextField="ItemName" DataValueField="ItemID" AutoCompleteMode="SuggestAppend"
                                                        DropDownStyle="DropDownList" MaxLength="50" RenderMode="Block" ItemInsertLocation="OrdinalText"
                                                        AppendDataBoundItems="False" CssClass="ajaxcombo">
                                                    </asp:ComboBox>
                                                    <asp:SqlDataSource ID="SqlDataSource3" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                                                        SelectCommand="SELECT distinct ItemID, ItemName FROM tblItemMaster ORDER BY ItemName"></asp:SqlDataSource>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>--%>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Charges" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCharges" runat="server" Text='<%# Eval("Charges") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LoanAmtFrom" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanAmtFrom" runat="server" Text='<%# Eval("LoanAmtFrom") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="LoanAmtTo" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                        Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanAmtTo" runat="server" Text='<%# Eval("LoanAmtTo") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblChargeType" runat="server" Text='<%# Eval("ChargeType") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Amount" ItemStyle-HorizontalAlign="Right" HeaderStyle-HorizontalAlign="Right">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAmount" runat="server" Text='<%# Eval("ChargeAmount") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Account Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAccountName" align="center" runat="server" Text='<%# Eval("Name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:DropDownList ID="ddlAccountName" runat="server" Height="22px" DataSourceID="SqlDataSource2"
                                                DataTextField="Name" DataValueField="AccountID" Width="200px">
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                                                SelectCommand="SELECT distinct AccountID, Name FROM tblAccountmaster WHERE Suspended='No' ORDER BY Name">
                                            </asp:SqlDataSource>
                                            <%--<asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:ComboBox ID="ddlGoldItemName" runat="server" Height="18px" Width="200px" DataSourceID="SqlDataSource2"
                                                        DataTextField="ItemName" DataValueField="ItemID" AutoCompleteMode="SuggestAppend"
                                                        DropDownStyle="DropDownList" MaxLength="50" RenderMode="Block" ItemInsertLocation="OrdinalText"
                                                        AppendDataBoundItems="False" CssClass="ajaxcombo">
                                                    </asp:ComboBox>
                                                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringTesting %>"
                                                        SelectCommand="SELECT distinct ItemID, ItemName FROM tblItemMaster ORDER BY ItemName"></asp:SqlDataSource>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>--%>
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="AccountID" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAccountID" runat="server" Text='<%# Eval("AccountID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Button ID="BtnUploadCharges" runat="server" Text="Upload" ValidationGroup="UploadCharges"
                                                OnClick="BtnUploadCharges_Click" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvChargesDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="4">
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Cancel" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
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
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="left" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" Width="93%" runat="server" class="textbox_search"
                                    Height="26px">
                                </asp:DropDownList>
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
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="GoldLoanNo" AutoPostBack="true"
                                OnPageIndexChanging="dgvDetails_PageIndexChanging" OnRowCommand="dgvDetails_RowCommand"
                                AutoGenerateColumns="False" Width="100%">
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
                                    <asp:BoundField DataField="ID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="SDID" HeaderText="SDID" Visible="False" />
                                    <asp:BoundField DataField="GoldLoanNo" HeaderText="GL-No" />
                                    <asp:BoundField DataField="IssueDate" HeaderText="Issue Date" Visible="False" />
                                    <asp:BoundField DataField="ApplicantName" HeaderText="Applicant Name" />
                                    <asp:BoundField DataField="TotalGrossWeight" HeaderText="Total Gross Weight" ItemStyle-HorizontalAlign="Right" />
                                    <asp:BoundField DataField="TotalNetWeight" HeaderText="Total Net Weight" ItemStyle-HorizontalAlign="Right" />
                                    <asp:BoundField DataField="GoldNetValue" HeaderText="Net Amt(gold value)" ItemStyle-HorizontalAlign="Right" />
                                    <asp:BoundField DataField="NetLoanAmtSanctioned" HeaderText="Net Sanctioned Loan Amt"
                                        ItemStyle-HorizontalAlign="Right" />
                                    <asp:BoundField DataField="PanNo" HeaderText="Pan No" Visible="False" />
                                    <asp:BoundField DataField="SanctionType" HeaderText="Sanction Type" Visible="False" />
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
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td colspan="4">
                                <asp:TextBox ID="txtGID" runat="server" Visible="false" Height="20px" Width="20px"></asp:TextBox>
                                <!-- ID -->
                                <asp:TextBox ID="txtID" runat="server" Visible="false" Height="20px" Width="20px"></asp:TextBox>
                                <!-- FY ID -->
                                <asp:TextBox ID="txtFYID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <!-- Branch ID -->
                                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <!-- Comp ID -->
                                <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <!-- BCP ID -->
                                <asp:TextBox ID="txtBCPID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <!-- Mobile, Telephone, Email ID, LoginID -->
                                <asp:TextBox ID="txtMobile" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtTelephone" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtEmailID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtTotalGross" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtLoginID" runat="server" Visible="false" Height="16px" Width="16px"></asp:TextBox>
                                <input id="txtAccGPID" type="hidden" runat="server" />
                                <!-- Photo Path -->
                                <asp:TextBox ID="txtProofOfOwnershipPath" runat="server" Visible="false" Height="22px"
                                    Width="16px"></asp:TextBox>
                                 <!-- ID, SchemeName, SchemeType, LTV, MinTenure, MaxTenure, InterestRate, AreaID, Total Charges Amount-->
                                <asp:TextBox ID="txtSchemeID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtSchemeName" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtSchemeType" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtLTV" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtMinTenure" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtMaxTenure" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtInterestRate" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtAreaID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                                <asp:TextBox ID="txtTotalChargesAmount" runat="server" Visible="false" Height="22px"
                                    Width="17px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
    </table>
    <script language="javascript" type="text/javascript">        disableDate();</script>
    <script language="javascript" type="text/javascript">        disableDueDate();</script>
</asp:Content>
