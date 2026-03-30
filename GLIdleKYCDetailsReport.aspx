<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLIdleKYCDetailsReport.aspx.cs" Inherits="GLIdleKYCDetailsReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 20%;">
            </td>
            <td style="width: 20%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Report: GL Idle KYC Details"></asp:Label>
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
                Select Branch Name
            </td>
            <td colspan="3" class="txt_style">
                <asp:DropDownList ID="ddlBranchName" class="textbox" runat="server" Height="28px"
                    Width="41.5%">
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="reqCountry" runat="server" Text="*" ErrorMessage="*"
                    ControlToValidate="ddlBranchName" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="--Select Branch Name--">*</asp:RequiredFieldValidator>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlBranchName" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <%-- <tr>
            <td class="label">
                From Loan Date
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtFromDate" CssClass="textbox" runat="server" onkeypress="return isNumericSlash(event);"
                    MaxLength="10" Height="80%" Width="39.5%" placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReqBirthDate" runat="server" ErrorMessage="*" ControlToValidate="txtFromDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtFromDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td class="label">
                To Loan Date
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtToDate" CssClass="textbox" runat="server" onkeypress="return isNumericSlash(event);"
                    MaxLength="10" Height="80%" Width="39.5%" placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="txtToDate" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CompareValidator4" runat="server" ControlToValidate="txtToDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" ValidationGroup="save" Type="Date"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>--%>
        <tr>
            <td colspan="4" style="height: 10%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td align="center">
                <div>
                    <asp:Button ID="btnDetails" runat="server" Text="Show Details" CssClass="button" 
                        Style="width: 50%;" ValidationGroup="save" OnClick="btnDetails_Click" />&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CssClass="button" OnClick="btnReset_Click" />
                  
                </div>
            </td>
            <td>
            </td>
            <td>
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
