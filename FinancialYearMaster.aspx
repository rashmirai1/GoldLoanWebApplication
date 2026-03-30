<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="FinancialYearMaster.aspx.cs" Inherits="FinancialYearMaster" Theme="GridViewTheme"
    EnableViewStateMac="false" %>

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

        function isAlphaNumChars(e) { // Alphanumeric, backspace, enter, double quote,%,&,(,),+,-,.,/,?,@,[,],_
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 45 && k < 58) || (k > 62 && k < 94) || (k > 96 && k < 123) || (k == 32 || k == 95 || k == 8 || k == 13 || k == 34 || k == 37 || k == 38) || (k > 39 && k < 42) || k==43 || k == 0);
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }
)
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td colspan="2">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="2" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Financial Year Master"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <!--Form Design -->
        <tr>
            <td class="label">
                Financial Year
            </td>
            <td class="txt_style" valign="middle">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtFYear" class="textbox_readonly" runat="server" ReadOnly="true"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="reqFYear" runat="server" ErrorMessage="*" ControlToValidate="txtFYear"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        <asp:Button ID="btnNextYear" runat="server" Text="Create Next Year" CssClass="button_style2"
                            OnClick="btnNextYear_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                Start Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtStartDate" class="textbox_readonly" runat="server" ReadOnly="true"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                            ControlToValidate="txtStartDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnNextYear" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label">
                End Date
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtEndDate" class="textbox_readonly" runat="server" ReadOnly="true"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                            ControlToValidate="txtEndDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                            Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnNextYear" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="txt_style">
            </td>
            <!--Save/Reset Button -->
            <td>
                <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="left" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" runat="server" class="textbox_search" 
                                    Height="26px" Width="180px">
                                </asp:DropDownList>
                                &nbsp;&nbsp;
                            </td>
                            <td class="label">
                                Search Text:
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" 
                                    class="textbox_search" onblur="getfocus()"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
                            </td>
                        </tr>
                    </table>
                    <!--Dotted Bar -->
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                        <tr>
                            <td colspan="2">
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td align="center" valign="middle" colspan="2">
                                <div class="dotted_bar">
                                </div>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <!--GridView -->
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="FinancialyearID" AutoPostBack="true"
                                OnRowCommand="dgvDetails_RowCommand" OnPageIndexChanging="dgvDetails_PageIndexChanging"  Width="75%">
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
                                    <asp:BoundField DataField="FinancialyearID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="StartDate" HeaderText="Start Date" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="EndDate" HeaderText="End Date" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                    <asp:BoundField DataField="Financialyear" HeaderText="Financial Year" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left"/>
                                    <asp:BoundField DataField="CompID" HeaderText="Comp ID" Visible="False" />
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
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
            <td colspan="4">
                <br />
                <br />
            </td>
        </tr>
    </table>
</asp:Content>
