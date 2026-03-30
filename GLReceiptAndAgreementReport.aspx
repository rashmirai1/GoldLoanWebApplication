<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLReceiptAndAgreementReport.aspx.cs" Inherits="GLReceiptAndAgreementReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 12%;">
            </td>
            <td style="width: 15%;">
            </td>
            <td style="width: 53%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 99%;">
                    <tr>
                        <td>
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="Report: GL Receipt and Agreement"></asp:Label>
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
                Select GL Customer
            </td>
            <td colspan="3" class="txt_style">
                <asp:DropDownList ID="ddlGoldLoanNo" class="textbox" runat="server" Height="28px"
                    Width="75%">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="reqGLNo" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="ddlGoldLoanNo" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
           <td>
                &nbsp;
            </td>
            <td class="txt_style">
                <asp:CheckBox ID="chkReceipt" runat="server" AutoPostBack="true" Text="Receipt"  Font-Names="Verdana" Font-Size="12px"/>
            </td>
             <td align="left">
                <asp:CheckBox ID="chkAgreement" runat="server" AutoPostBack="true" Text="Agreement" Font-Names="Verdana" Font-Size="12px"/>
            </td>
            
             <td>
               &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 10%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
        <td>&nbsp; </td>
            <td align="left" colspan="3">
            <br /><br />
                <div>
                    <asp:Button ID="btnDetails" runat="server" Text="Generate Report" CssClass="button"
                        Style="width: 22%;" ValidationGroup="save" OnClick="btnDetails_Click"/>&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
                </div>
            </td>
            
        </tr>
        <tr>
            <td colspan="4" style="height: 15px;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div class="barstyle">
                </div>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label ID="lblMsg" class="label" Style="color: Maroon; font-size: small;" runat="server"
                    Font-Names="Verdana"></asp:Label>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
    </table>
</asp:Content>
