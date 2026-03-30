<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLOutstandingLTVReport.aspx.cs" Inherits="GLOutstandingLTVReport" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function validdate() {
            var SelDate = document.getElementById("<%=txtIntDate.ClientID %>");
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

            if (!pattern.test(SelDate.value)) {
                SelDate.value = '';
                alert('Please Enter Date.');
                return false;
            }
            else {

            }
        }

        function datevalid(txt) {
            //Added by Priya on 9-9-2015
            var keycode = (txt.which) ? txt.which : txt.keyCode;
            if (keycode == 13) return false;
            //end
            if (isNaN(txt.value.replace('/', '').replace('/', ''))) {
                txt.value = '';
                return false;
            }
        }

        function ValidateDate(txt, keyCode) {
            if (keyCode == 16)
                isShift = false;
            var val = txt.value;

            if (val.length == 10) {
                var splits = val.split("/");
                var dt = new Date(splits[1] + "/" + splits[0] + "/" + splits[2]);
                var dd = dt.getDate();
                var mm = dt.getMonth();
                var yy = dt.getFullYear();
                var selDate = dd + '/' + mm + '/' + yy;

                var lastDay = new Date(dt.getFullYear(), dt.getMonth() + 1, 0);

                var dd1 = lastDay.getDate();
                var mm1 = lastDay.getMonth();
                var yy1 = lastDay.getFullYear();

                var LastDayMnth = dd1 + '/' + mm1 + '/' + yy1;

                //Validation for Dates
                if (dt.getDate() == splits[0] && dt.getMonth() + 1 == splits[1] && dt.getFullYear() == splits[2]) {

                }
                else {
                    txt.value = '';
                    alert('Invalid Date.');
                    return;
                }
            }
            else if (val.length < 10) {
                txt.value = '';
                alert('Invalid Date.');
                return;
            }
        }
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 24.5%;">
            </td>
            <td style="width: 25%;">
            </td>
            <td style="width: 24.5%;">
            </td>
        </tr>
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
            <td align="center" colspan="4" class="header">
                <asp:Label ID="lblHeader" runat="server" Text="GL Outstanding LTV(%)">
                </asp:Label>
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
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Select Date:<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtIntDate" class="textbox" runat="server" Text="" Width="82%" MaxLength="10"
                    onkeypress="return datevalid(event);" onchange="ValidateDate(this, event.keyCode)"></asp:TextBox>
                <asp:CalendarExtender ID="txtIntDate_CalendarExtender" runat="server" Format="dd/MM/yyyy"
                    PopupButtonID="btnImgCalender" TargetControlID="txtIntDate" CssClass="Calenderstyle">
                </asp:CalendarExtender>
                <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" Style="margin-bottom: 10px;" />
            </td>
            <td>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
            </td>
            <td>
                &nbsp;
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
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <asp:HiddenField ID="hdnUserID" runat="server" />
        <asp:HiddenField ID="hdnFYear" runat="server" Value="0" />
        <asp:HiddenField ID="hdnFYearID" runat="server" Value="0" />
        <asp:HiddenField ID="hdnBranchID" runat="server" Value="0" />
    </table>
</asp:Content>
