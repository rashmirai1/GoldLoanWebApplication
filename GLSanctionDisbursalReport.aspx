<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLSanctionDisbursalReport.aspx.cs" Inherits="GLSanctionDisbursalReport" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function valid() {
            var ddlGoldLoanNo = document.getElementById("<%=ddlGoldLoanNo.ClientID %>");
            if (ddlGoldLoanNo.selectedIndex == 0) { alert('Select Gold Loan No'); return false; }
        }

    </script>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
                <tr>
                    <td style="width: 20%;"></td>
                    <td style="width: 24.5%;"></td>
                    <td style="width: 25%;"></td>
                    <td style="width: 24.5%;"></td>
                </tr>
                <tr>
                    <td colspan="4">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="4">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="4" class="header">
                        <asp:Label ID="lblHeader" runat="server" Text="GL S&D Report">
                        </asp:Label>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">&nbsp;
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <div class="barstyle">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td class="label" style="text-align: left;">Select GoldLoan No.:<b style="color: Red;">*</b>
                    </td>
                    <td class="txt_style">
                        <asp:DropDownList ID="ddlGoldLoanNo" runat="server" class="textbox" Style="height: 27px; width: 95%;">
                        </asp:DropDownList>
                    </td>
                    <td></td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>
                        <asp:Button ID="btnPrintReport" runat="server" Text="Print Report" CssClass="css_btn_class"
                            Style="margin-left: 0px; width: 100px;" OnClick="btnPrintReport_Click" />
                    </td>
                    <td>&nbsp;
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
                    </td>
                    <td>&nbsp;
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
                    <td colspan="4">&nbsp;
                    </td>
                </tr>
                <asp:HiddenField ID="hdnUserID" runat="server" Value="0" />
                <asp:HiddenField ID="hdnFYearID" runat="server" Value="0" />
                <asp:HiddenField ID="hdnBranchID" runat="server" Value="0" />
                <asp:HiddenField ID="hdnLoanType" runat="server" Value="0" />
                <asp:HiddenField ID="hdnareaid" runat="server" Value="0" />
                <asp:HiddenField ID="hdnzoneid" runat="server" Value="0" />
            </table>
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnPrintReport" />
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
