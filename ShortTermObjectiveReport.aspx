<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ShortTermObjectiveReport.aspx.cs"
    Inherits="ShortTermObjectiveReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function PrintGridData() {
            var prtGrid = document.getElementById('divGrid');
            prtGrid.border = 1;
            var prtwin = window.open('', 'Short term Objectives', '');
            prtwin.document.write(prtGrid.outerHTML);
            prtwin.document.close();
            prtwin.focus();
            prtwin.print();
            prtwin.close();

        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
            <tr>
                <td colspan="4">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div style="align: center; float: center; width: 65%;">
                        <asp:Label ID="lblMsg" Text="" Style="align: center" Font-Bold="true" runat="server"></asp:Label>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div id="divGrid">
                        <asp:GridView ID="gvData" runat="server" ShowHeader="true" Width="100%" CssClass="textbox_readonly"
                            EmptyDataText="NO Record Found" ShowFooter="true">
                            <FooterStyle />
                        </asp:GridView>
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnPrint" runat="server" Text="Print" CssClass="css_btn_class" Style="margin-left: 0px;
                        width: 70px;" Visible="false" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
