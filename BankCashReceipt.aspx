<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="BankCashReceipt.aspx.cs" Inherits="BankCashReceipt" EnableViewStateMac="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtRefDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtChqDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });
        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }
        function isNumericSlash(e) { // Numbers and slash
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 47));
        }

        function isAlphaNumCharsDot(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k == 46));
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }

        function isNumeric(e) { // Numbers and decimal point
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 46));
        }   
    </script>
    <asp:ScriptManager ID="ScriptManager2" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td style="width: 12%;">
                        </td>
                        <td style="width: 18%;">
                        </td>
                        <td style="width: 10%;">
                        </td>
                        <td style="width: 20%;">
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td colspan="4" class="header" align="center">
                            <asp:Label ID="lblHeader" runat="server" Text=" Receipt Entry"></asp:Label>
                            <br />
                            <br />
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
                        <td class="label">
                            <asp:Label ID="Label21" class="label" runat="server" Text="Reference Date"></asp:Label>
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtRefDate" class="textbox" Width="45.5%" runat="server" MaxLength="10"
                                placeholder="dd/mm/yyyy" ClientIDMode="Static" AutoPostBack="true" ></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ForeColor="Red"
                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="save"
                                ControlToValidate="txtRefDate"> </asp:RequiredFieldValidator>
                        </td>
                        <td class="label">
                            <asp:Label ID="Label17" class="label" runat="server" Text="Reference No"></asp:Label>
                        </td>
                        <td class="txt_style">
                            <asp:TextBox ID="txtRefNo" ReadOnly="true" class="textbox_readonly" Width="47%" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDebit" runat="server" ForeColor="Red" Font-Size="Medium"
                                ErrorMessage="*" Display="Dynamic" ValidationGroup="save" ControlToValidate="txtRefNo"> </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                        </td>
                        <td colspan="2">
                            <!-- Custom Validator for Ref Date -->
                            <asp:CustomValidator runat="server" ID="CustomValidator4" ControlToValidate="txtRefDate"
                                ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True" OnServerValidate="txtRefDate_ServerValidate"
                                ValidationGroup="save" ErrorMessage="Ref Date must be within Financial Year." />
                            <asp:CustomValidator runat="server" ID="CustomValidator1" ControlToValidate="txtRefDate"
                                ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True" OnServerValidate="txtRefDate1_ServerValidate"
                                ValidationGroup="save" ErrorMessage="Please check the Reference Date. Since it does not match with the transaction date of the selected accounts." />
                        </td>
                    </tr>
                    <tr>
                        <td class="label">
                            <asp:Label ID="Label55" class="label" runat="server" Text="Bank/Cash Account"></asp:Label>
                        </td>
                        <td colspan="3" class="txt_style">
                            <asp:DropDownList ID="ddlBankCash" class="textbox" runat="server" Height="28px" AutoPostBack="true"
                                Width="80%" OnSelectedIndexChanged="ddlBankCash_SelectedIndexChanged">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ForeColor="Red"
                                Font-Size="Medium" ErrorMessage="*" Display="Dynamic" ValidationGroup="save"
                                InitialValue="0" ControlToValidate="ddlBankCash"> </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <!-- Error Msg -->
                        <td class="label">
                            &nbsp;
                        </td>
                        <td colspan="3" class="txt_style">
                            <asp:Label ID="lblErrorMsg" runat="server" Text="" ForeColor="Red" Font-Size="Small"
                                Font-Names="Calibri"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4" valign="top">
                            <div style="vertical-align: top">
                                <!--GridView -->
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <asp:GridView ID="dgvAccountDetails" DataKeyNames="PostingID" RowStyle-BackColor="white"
                                            RowStyle-BorderStyle="solid" RowStyle-BorderWidth="1px" RowStyle-BorderColor="#c0c0c0"
                                            RowStyle-Height="24px" HeaderStyle-BackColor="#D4D4D4" runat="server" OnRowCommand="dgvAccountDetails_RowCommand"
                                            HeaderStyle-Font-Size="14px" HeaderStyle-BorderWidth="1px" HeaderStyle-Height="22px"
                                            GridLines="None" Width="100%" ShowFooter="True" AutoGenerateColumns="False" Font-Names="Calibri">
                                            <Columns>
                                                <asp:TemplateField HeaderText="AccountID" ItemStyle-HorizontalAlign="Left" Visible="false"
                                                    HeaderStyle-HorizontalAlign="Left" FooterStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAccountID" runat="server" Text='<%# Bind("AccountID") %>' Width="20px"></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Font-Size="14px" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Particulars" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                                    FooterStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblAccountName" runat="server" Font-Size="12px" Text='<%# Bind("Name") %>'
                                                            Width="400px"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                                            <ContentTemplate>
                                                                <asp:ComboBox ID="ddlAccountName" runat="server" Height="18px" Width="252px" DataSourceID="SqlDataSource1"
                                                                    DataTextField="Name" DataValueField="AccountID" AutoCompleteMode="SuggestAppend"
                                                                    DropDownStyle="DropDownList" MaxLength="50" RenderMode="Block" ItemInsertLocation="OrdinalText"
                                                                    AppendDataBoundItems="False" CssClass="ajaxcombo">
                                                                </asp:ComboBox>
                                                                <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                                                                    SelectCommand="SELECT DISTINCT tblAccountMaster.AccountID, Name=(select case isnull(Alies, '') when '' then tblAccountMaster.Name else tblAccountMaster.Name+ ' ('+Alies+')' end) FROM tblAccountMaster  WHERE (tblAccountMaster.GPID<>11) AND (tblAccountMaster.GPID<>71) AND (tblAccountMaster.GPID<>70) AND Suspended='No' ORDER BY Name">
                                                                </asp:SqlDataSource>
                                                            </ContentTemplate>
                                                        </asp:UpdatePanel>
                                                    </FooterTemplate>
                                                    <FooterStyle HorizontalAlign="Left" />
                                                    <HeaderStyle Font-Size="14px" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Debit" ItemStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Small"
                                                    FooterStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblDebit" Text='<%# Bind("Debit") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtDebit" Text="" Width="70px" CssClass="textbox" runat="server"
                                                            MaxLength="10" onkeypress="return isNumeric(event);" Style="text-align: right;"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <HeaderStyle Font-Size="14px" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Credit" ItemStyle-HorizontalAlign="Center" HeaderStyle-Font-Size="Small"
                                                    FooterStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblCredit" Text='<%# Bind("Credit") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:TextBox ID="txtCredit" Text="" Width="70px" CssClass="textbox" runat="server"
                                                            MaxLength="10" onkeypress="return isNumeric(event);" Style="text-align: right;"></asp:TextBox>
                                                    </FooterTemplate>
                                                    <FooterStyle HorizontalAlign="Center" />
                                                    <HeaderStyle Font-Size="14px" />
                                                    <ItemStyle HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="PostingID" ItemStyle-HorizontalAlign="Left" Visible="false"
                                                    HeaderStyle-HorizontalAlign="Left" FooterStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblPostingID" runat="server" Text='<%# Bind("PostingID") %>' Width="10px"></asp:Label>
                                                    </ItemTemplate>
                                                    <HeaderStyle Font-Size="14px" />
                                                    <ItemStyle HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="RefDate" ItemStyle-HorizontalAlign="Left" Visible="false"
                                                    HeaderStyle-Font-Size="Small" HeaderStyle-HorizontalAlign="Left" FooterStyle-HorizontalAlign="Left">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblRefDate" runat="server" Text='<%# Bind("RefDate") %>' Width="10px"></asp:Label>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField Visible="true" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Button ID="BtnAdd" runat="server" Text="ADD" OnClick="BtnAdd_Click" />
                                                    </FooterTemplate>
                                                    <ItemStyle HorizontalAlign="Left" />
                                                    <HeaderStyle Font-Size="14px" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" />
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        <asp:Button ID="btnDelete" runat="server" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>"
                                                            CommandName="DeleteRecord" Text="Delete" Width="50px" Height="22px" />
                                                    </FooterTemplate>
                                                    <ItemStyle HorizontalAlign="Center" />
                                                    <HeaderStyle Font-Size="14px" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <HeaderStyle BackColor="#D4D4D4" Font-Size="XX-Small" />
                                            <RowStyle BackColor="White" />
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:PostBackTrigger ControlID="dgvAccountDetails" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td style="width: 25%;">
                        </td>
                        <td style="width: 22.4%;">
                        </td>
                        <td style="width: 25%;">
                        </td>
                        <td style="width: 1.1%;">
                        </td>
                        <td style="width: 27%;">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="right">
                            <asp:Label ID="Label65" CssClass="label" runat="server" Font-Bold="true" Text="Total :"></asp:Label>
                            <asp:TextBox ID="txtTotalDebit" CssClass="textbox" ReadOnly="true" Width="70px" runat="server"
                                Style="text-align: right;"></asp:TextBox>&nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="txtTotalCredit" CssClass="textbox" ReadOnly="true" Width="70px"
                                runat="server" Style="text-align: right;"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                        <td>
                            &nbsp;
                        </td>
                        <td align="left" colspan="3">
                            <asp:Label ID="Label3" CssClass="label" runat="server" Font-Bold="true" Text="Amt Received :"></asp:Label>
                            <asp:TextBox ID="txtAmount" CssClass="textbox" ReadOnly="true" Width="158px" runat="server"
                                Style="text-align: right;"></asp:TextBox>&nbsp;
                        </td>
                        <%--<td>
                            &nbsp;
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox2" CssClass="textbox" ReadOnly="true" Width="70px"
                                runat="server" Style="text-align: right;"></asp:TextBox>
                        </td>--%>
                    </tr>
                </table>
            </td>
        </tr>
        <%-- <tr>
            <td>
            </td>
            <td colspan="3" align="right">
                <asp:Label ID="Label26" class="label" runat="server" Text="Amount Received :" Font-Bold="true"
                    Font-Size="14px"></asp:Label>&nbsp;
                <asp:TextBox ID="txtAmount" class="textbox" ReadOnly="true" Width="128px" onkeypress="return OnlyNumericEntry(event);"
                    runat="server"></asp:TextBox>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>--%>
        <tr>
            <td colspan="4">
                <div id="BankDiv" runat="server" style="width: 100%;">
                    <table width="100%" border="0" align="left">
                        <tr>
                            <td style="width: 12%;">
                            </td>
                            <td style="width: 18%;">
                            </td>
                            <td style="width: 10%;">
                            </td>
                            <td style="width: 20%;">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4" align="left" style="font-size: 14px;">
                                <asp:Label ID="Label30" runat="server" Text="Add Cheque Details :" CssClass="label"
                                    Font-Size="16px" Font-Bold="true" Font-Underline="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <%--<td class="label">
                                <asp:Label ID="Label31" class="label" Text=" Cheque No. " runat="server"></asp:Label>
                            </td>--%>
                            <td class="label">
                                Cheque No.
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtChqNo" class="textbox" MaxLength="17" onkeypress="return OnlyNumericEntry(event);"
                                    runat="server" Width="100px"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="txtChqNo"
                                    ForeColor="Red" Display="Dynamic" Font-Size="Small" ErrorMessage="Cheque No. should  not be zero."
                                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^[1-9]\d*(\.\d+)?$"></asp:RegularExpressionValidator>
                            </td>
                            <%--  <td class="label">
                                <asp:Label ID="Label32" class="label" Text=" Cheque Date " runat="server"></asp:Label>
                            </td>--%>
                            <td class="label">
                                Cheque Date
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtChqDate" class="textbox" MaxLength="17" runat="server" Width="50%"
                                    placeholder="dd/mm/yyyy" ClientIDMode="Static"></asp:TextBox>
                                <asp:CompareValidator ID="CompareValidator2" runat="server" ControlToValidate="txtChqDate"
                                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                                    Font-Bold="True"></asp:CompareValidator>
                            </td>
                        </tr>
                        <tr>
                            <%--<td class="label">
                                <asp:Label ID="Label34" class="label" Text=" Bank Name " runat="server"></asp:Label>
                            </td>--%>
                            <td class="label">
                                Bank Name
                            </td>
                            <td colspan="3" class="txt_style">
                                <asp:DropDownList ID="ddlBankName" class="textbox" runat="server" Height="28px" Width="81%">
                                </asp:DropDownList>
                                <br />
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="CashDiv" runat="server">
                    <table width="100%" border="0" align="left">
                        <tr>
                            <td style="width: 11.5%;">
                            </td>
                            <td style="width: 9%;">
                            </td>
                            <td style="width: 15%;">
                            </td>
                            <td style="width: 9.5%;">
                            </td>
                            <td style="width: 9%;">
                            </td>
                            <td style="width: 15%;">
                            </td>
                            <td style="width: 11%;">
                            </td>
                            <td style="width: 9%;">
                            </td>
                            <td style="width: 16%;">
                            </td>
                        </tr>
                        <tr>
                            <td colspan="9" align="left" style="font-size: 14px;">
                                <asp:Label ID="Label2" runat="server" Text="Denomination :" CssClass="label" Font-Size="16px"
                                    Font-Bold="true" Font-Underline="true"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" align="left">
                                <asp:Label ID="Label7" class="label" Text="1000.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label9" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtThousand" class="textbox" AutoPostBack="true" runat="server"
                                    onkeypress="return OnlyNumericEntry(event);" Width="30px" OnTextChanged="txtThousand_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label1" class="label" Text=":" Font-Bold="true" Style="font-size: 12px;"
                                    runat="server"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txtTh" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                            <td>
                                <asp:Label ID="Label5" class="label" Text="500.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label6" class="label" Text="x" Font-Bold="true" runat="server"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFiveHundred" class="textbox" runat="server" AutoPostBack="true"
                                    onkeypress="return OnlyNumericEntry(event);" Width="30px" OnTextChanged="txtFiveHundred_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label8" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFiveHun" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                            <td class="label" align="left">
                                <asp:Label ID="Label10" class="label" Text="100.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label11" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txthundred" class="textbox" runat="server" AutoPostBack="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px" OnTextChanged="txthundred_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label12" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtHun" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" align="left">
                                <asp:Label ID="Label14" class="label" Text="50.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label15" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txtFifty" class="textbox" AutoPostBack="true" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px" OnTextChanged="txtFifty_TextChanged"></asp:TextBox>&nbsp;&nbsp;&nbsp;
                                <asp:Label ID="Label16" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFift" class="textbox" ReadOnly="true" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                            <td class="label">
                                <asp:Label ID="Label18" class="label" Text="20.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label19" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txtTwenty" class="textbox" AutoPostBack="true" onkeypress="return OnlyNumericEntry(event);"
                                    runat="server" Width="30px" OnTextChanged="txtTwenty_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label20" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtTwent" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                            <td class="label" align="left">
                                <asp:Label ID="Label22" class="label" Text="10.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label23" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txtTen" class="textbox" runat="server" AutoPostBack="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px" OnTextChanged="txtTen_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label24" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtTN" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <asp:Label ID="Label27" class="label" Text="05.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label29" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style" align="left">
                                <asp:TextBox ID="txtFive" class="textbox" runat="server" AutoPostBack="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px" OnTextChanged="txtFive_TextChanged"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label36" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td>
                                <asp:TextBox ID="txtFiv" class="textbox" runat="server" ReadOnly="true" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>
                            </td>
                            <td class="label" align="center">
                                <%--  <asp:Label ID="Label54" class="label" Text="2.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label56" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>--%>
                            </td>
                            <td class="txt_style" align="left">
                                <%-- <asp:TextBox ID="TextBox1" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label33" class="label" Text=":" Font-Bold="true" Style="font-size: 12px;"
                                    runat="server"></asp:Label>--%>
                            </td>
                            <td>
                                <%--<asp:TextBox ID="TextBox14" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>--%>
                            </td>
                            <td class="label">
                                <%--<asp:Label ID="Label37" class="label" Text="1.00 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label38" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>--%>
                            </td>
                            <td class="txt_style" align="left">
                                <%-- <asp:TextBox ID="TextBox2" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label39" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>--%>
                            </td>
                            <td>
                                <%--<asp:TextBox ID="TextBox17" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>--%>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" colspan="2">
                                <asp:Label ID="Label4" class="label" Text=" Coins :" runat="server"></asp:Label>
                            </td>
                            <td class="txt_style" colspan="7">
                                <asp:TextBox ID="txtCoins" class="textbox" AutoPostBack="true" OnTextChanged="txtCoins_TextChanged"
                                    runat="server" onkeypress="return OnlyNumericEntry(event);" Width="84px"></asp:TextBox>
                                <asp:Label ID="Label25" class="label" Text=" (Amount)" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" colspan="2">
                                <asp:Label ID="Label28" class="label" Text=" Grand Total   :" runat="server"></asp:Label>
                            </td>
                            <td class="txt_style" colspan="7">
                                <asp:TextBox ID="txtGrandTotal" AutoPostBack="true" class="textbox" runat="server"
                                    ReadOnly="true" Width="84px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="label">
                                <%-- <asp:Label ID="Label41" class="label" Text="0.50 " runat="server" Style="font-size: 12px;"></asp:Label>
                                <asp:Label ID="Label42" class="label" Text="x" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>--%>
                            </td>
                            <td class="txt_style" align="left">
                                <%--<asp:TextBox ID="TextBox3" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="30px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Label ID="Label43" class="label" Text=":" Font-Bold="true" runat="server" Style="font-size: 12px;"></asp:Label>--%>
                            </td>
                            <td>
                                <%--<asp:TextBox ID="TextBox11" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="85%"></asp:TextBox>--%>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <%-- </tr>
                            <td class="label">
                                <asp:Label ID="Label2" class="label" Text=" Sub Total " runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSubTotal" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="50px"></asp:TextBox>
                            </td>
                            <td class="label">
                                <asp:Label ID="Label26" class="label" Text=" Coins " runat="server" Style="font-size: 12px;"></asp:Label>
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtCoins" class="textbox" runat="server" onkeypress="return OnlyNumericEntry(event);"
                                    Width="40px"></asp:TextBox>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                            <td>
                            </td>
                        </tr>--%>
                    </table>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table width="100%" border="0" align="left">
                    <tr>
                        <td style="width: 12%;">
                        </td>
                        <td style="width: 18%;">
                        </td>
                        <td style="width: 10%;">
                        </td>
                        <td style="width: 20%;">
                        </td>
                    </tr>
                    <tr>
                        <!-- Narration -->
                        <td class="label">
                            Narration
                        </td>
                        <td class="txt_style" colspan="3">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:DropDownList ID="ddlNarration" runat="server" CssClass="textbox" Height="27px"
                                        Width="81%" AutoPostBack="true" ClientIDMode="Static">
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="ReqNarration" runat="server" ErrorMessage="*" ControlToValidate="ddlNarration"
                                        ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                                        SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <%--<tr>
            <td class="label">
                <asp:Label ID="Label13" class="label" Text="Narration " runat="server"></asp:Label>
            </td>
            <td colspan="3" class="txt_style">
                <asp:DropDownList ID="ddlNarration" class="textbox" runat="server" Height="27px"
                    Width="79.6%">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ForeColor="Red"
                    Font-Size="Medium" ErrorMessage="*" Display="Dynamic" InitialValue="0" ValidationGroup="save"
                    ControlToValidate="ddlNarration"> </asp:RequiredFieldValidator>
            </td>
        </tr>--%>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="4">
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
                <br />
                <br />
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
            <td align="right" colspan="4">
                <asp:Label ID="Label13" runat="server" Text="[Search Section]" Font-Names="Verdana"
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
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" runat="server" class="textbox_search" Height="26px"
                                    Width="180px">
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                            </td>
                            <td class="label">
                                Search Text:
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" Width="74%" class="textbox_search"
                                    onblur="getfocus()"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td align="right" colspan="3" class="header">
                            </td>
                            <td align="right" colspan="1">
                                <asp:Label ID="Label26" runat="server" Text="[Edit Section]" Font-Names="Verdana"
                                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="4">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <!--GridView DGV Details (Edit/Delete section) -->
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" HeaderStyle-BackColor="#D4D4D4" Width="100%"
                                CellPadding="4" ForeColor="#333333" Font-Names="Calibri" Font-Size="13px" RowStyle-BackColor="white"
                                GridLines="None" PageSize="20" EmptyDataText="NO RECORDS FOUND!" EmptyDataRowStyle-BackColor="#D4D4D4"
                                EmptyDataRowStyle-ForeColor="Navy" EmptyDataRowStyle-HorizontalAlign="Center"
                                EmptyDataRowStyle-VerticalAlign="Middle" DataKeyNames="ReferenceNo" AutoPostBack="true"
                                OnRowCommand="dgvDetails_RowCommand" OnPageIndexChanging="dgvDetails_PageIndexChanging"
                                AllowPaging="True" AutoGenerateColumns="False" PageIndex="20">
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
                                    <asp:BoundField DataField="BCRID" HeaderText="ID" Visible="False" ItemStyle-HorizontalAlign="Center" />
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Reference No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblReferenceNo" runat="server" Text='<%# Bind("ReferenceNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ReferenceDate" HeaderText="Reference Date" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Amount" HeaderText="Amount" Visible="False" />
                                    <%--<asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="BankCash Account">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBankCashAccount" runat="server" Text='<%# Bind("AccountName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>--%>
                                    <asp:BoundField DataField="BankCashAccount" HeaderText="Bank/Cash Account" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"
                                        Visible="true" />
                                    <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/delete-button.jpg"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <!-- ID -->
            <asp:TextBox ID="txtID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
            <!-- Comp ID -->
            <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
            <!-- Branch ID -->
            <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
            <!-- FY ID -->
            <asp:TextBox ID="txtFYID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
            <!-- Operator ID -->
            <asp:TextBox ID="txtOperatorID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
        </tr>
        <tr>
            <td colspan="4" style="height: 10px;">
                <asp:TextBox ID="txtLedgerId" Visible="false" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtPostingId" Visible="false" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtBCRID" Visible="false" runat="server"></asp:TextBox>
                <asp:TextBox ID="txtDID" Visible="false" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
