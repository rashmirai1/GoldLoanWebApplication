<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="RejectedCasesDetails.aspx.cs" Inherits="RejectedCasesDetails" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        function PrintPage() {
            var printContent = document.getElementById('<%= pnlRejcase.ClientID %>');
            var printWindow = window.open("All Records", "Print Panel", 'left=50000,top=50000,width=0,height=0');
            printWindow.document.write(printContent.outerHTML);
            printWindow.document.close();
            printWindow.focus();
            printWindow.print();
            printWindow.close();
            var ddlFy = document.getElementById('<%= ddlFYear.ClientID %>');
            ddlFy.value = 0;

        }
        function validrecord() {
            var ddlFyear = document.getElementById('<%= ddlFYear.ClientID %>');
            if (ddlFyear.selectedIndex == 0) {
                alert('Select Financial Year');
                return false;
            }
        }
        function ClearGrid() {
            var panel = document.getElementById('<%= pnlRejcase.ClientID %>');
            var ddlFy = document.getElementById('<%= ddlFYear.ClientID %>');
            if (ddlFy.SelectedIndex != 0) {
                panel.visible = false;
            }
        }
  
    </script>
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
                            <asp:Label ID="lblHeader" runat="server" Text="PL Rejected Cases Details"></asp:Label>
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
        <tr>
            <td class="label">
                Financial Year
            </td>
            <td>
                <asp:DropDownList ID="ddlFYear" runat="server" CssClass="textbox" Height="27px" Width="96%"
                    OnSelectedIndexChanged="ddlFYear_SelectedIndexChanged" AutoPostBack="true">
                </asp:DropDownList>
            </td>
            <td align="left">
                <asp:Button ID="btnShow" runat="server" Text="Show" CssClass="css_btn_class" OnClick="btnShow_Click"
                    OnClientClick="return validrecord();" />
            </td>
            <td align="left">
                <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="css_btn_class" OnClientClick="PrintPage();"
                    ToolTip="Click to Print All Records" />
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="barstyle">
                </div>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <asp:Panel ID="pnlRejcase" runat="server">
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width:90%;">                        
                        <tr>
                            <td align="center" colspan="3" style="background-color:#354d8d; color:#ffffff;">
                                <b style="font-family:Calibri;">
                                    <asp:Label ID="lblsummery" runat="server" Text="Rejected Case Summary" Font-Size="Medium"
                                        Height="25px"></asp:Label>
                                </b>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelAlignLeft" style="width:20%; text-align: left;
                                padding: 0px;">
                                <asp:Label ID="lblHdngfci" runat="server" Style="margin-left: 5px;" Text="FCIs" Font-Size="Medium"></asp:Label>
                            </td>
                            <td style="width:1%;">
                                <span>:</span>
                            </td>
                            <td style="text-align: left; width:69%;">
                                <asp:Label ID="lblfci" runat="server" Style="margin-left:0px;" Font-Size="Medium"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelAlignLeft" style="text-align: left;
                                padding: 0px;">
                                <asp:Label ID="lblHdngcibil" runat="server" Style="margin-left: 5px;" Text=" CIBIL" Font-Size="Medium"></asp:Label>
                            </td>
                            <td>
                                <span>:</span>
                            </td>
                            <td style="text-align: left;">
                                <asp:Label ID="lblCIBIL" runat="server" Font-Size="Medium"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelAlignLeft" style="padding: 0px;">
                                <asp:Label ID="lblHdngInterview" runat="server" Style="margin-left: 5px;" Text="Interview" Font-Size="Medium"></asp:Label>
                            </td>
                            <td>
                                <span>:</span>
                            </td>
                            <td>
                                <asp:Label ID="lblInterview" runat="server" Font-Size="Medium"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelAlignLeft" style="padding: 0px; background-color:#354d8d; color:#ffffff;">
                                 <b style="font-family: Calibri; text-align: left; font-size: 14px;">
                                <asp:Label ID="lblHdngtotal" runat="server" Style="margin-left: 5px;" Text=" Total Rejected Cases"
                                    Font-Bold="true"></asp:Label>
                                    </b>
                            </td>
                            <td style="background-color:#354d8d; color:#ffffff;">
                                <span>:</span>
                            </td>
                            <td style="background-color:#354d8d; color:#ffffff;">
                                <asp:Label ID="lbltotal" runat="server" Font-Size="Medium"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3" align="left">
                                <div>
                                    <br />
                                    <asp:GridView ID="gvRejectedCases" runat="server" EmptyDataText="NO Record Found"
                                        AutoGenerateColumns="true" Width="100%" OnRowDataBound="gvRejectedCases_RowDataBound">
                                        <HeaderStyle CssClass="gVHeader" Height="25px" />
                                        <RowStyle CssClass="gVItem" />
                                    </asp:GridView>
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</asp:Content>
