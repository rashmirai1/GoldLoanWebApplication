<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PledgeDetailsReport.aspx.cs"
    Inherits="PledgeDetailsReport" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <script type="text/javascript" language="javascript">

        function LoadDiv(url) {
            var img = new Image();
            var bcgDiv = document.getElementById("divBackground");
            var imgDiv = document.getElementById("divImage");
            var imgFull = document.getElementById("imgFull");
            var imgLoader = document.getElementById("imgLoader");

            imgLoader.style.display = "block";
            img.onload = function () {
                imgFull.src = img.src;
                imgFull.style.display = "block";
                imgLoader.style.display = "none";
            };
            img.src = url;
            var width = document.body.clientWidth;
            if (document.body.clientHeight > document.body.scrollHeight) {
                bcgDiv.style.height = document.body.clientHeight + "px";
            }
            else {
                bcgDiv.style.height = document.body.scrollHeight + "px";
            }

            imgDiv.style.left = (width - 650) / 2 + "px";
            imgDiv.style.top = "20px";
            bcgDiv.style.width = "100%";

            bcgDiv.style.display = "block";
            imgDiv.style.display = "block";
            return false;
        }
        function HideDiv() {
            var bcgDiv = document.getElementById("divBackground");
            var imgDiv = document.getElementById("divImage");
            var imgFull = document.getElementById("imgFull");
            if (bcgDiv != null) {
                bcgDiv.style.display = "none";
                imgDiv.style.display = "none";
                imgFull.style.display = "none";
            }
        }


        function PrintDiv() {
            var divToPrint = document.getElementById('printarea');
            var popupWin = window.open('', '_blank', 'location=no,left=100px');
            popupWin.document.open();
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='print' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='screen' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</html>');
            popupWin.document.close();
        }
        function btnprint_onclick() {

        }

    </script>
    <form id="form1" runat="server">
    <%-- <div align="center" style="background-color: White; border: 2px solid black; height: 60%;">
    </div>--%>
    <div id="printarea" align="center" style="background-color: White; border: 2px solid black;
        width: 80%;">
        <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
            <tr>
                <td style="width: 120px;">
                </td>
                <td style="width: 5px;">
                </td>
                <td style="width: 80px;">
                </td>
                <td style="width: 85px;">
                </td>
                <td style="width: 5px;">
                </td>
                <td style="width: 60px;">
                </td>
            </tr>
            <tr>
                <td>
                    <img style="width: 90px; height: 70px; vertical-align: top;" src="images/FINCORP_LOGO.png"
                        alt="KALAYIL NANCY FINCORP" />
                </td>
                <td align="left" style="height: 80px;" colspan="3" class="header">
                    <asp:Label ID="lblHeader" runat="server" Font-Bold="true" Text="GL KYC Pledge Details Report"></asp:Label>
                </td>
                <td align="left" colspan="2">
                    <br />
                    <asp:Label ID="lbl5" runat="server" Font-Bold="True" Text="Date :" Font-Size="11px"></asp:Label>
                    <asp:Label ID="lblDate" Class="label" runat="server" Font-Bold="True" Font-Size="11px"></asp:Label>
                    <br />
                    <asp:Label ID="label" Class="label" runat="server" Font-Bold="True" Text="Branch :"
                        Font-Size="11px"></asp:Label>
                    <asp:Label ID="lblbranch" runat="server" Font-Bold="True" Font-Size="9px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    <div style="width: 100%; background-color: Black; height: 4px; border: 1 solid #d23b1d;">
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="height: 30px;">
                </td>
            </tr>
            <tr>
                <!--Header -->
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px;">
                    Gold Loan No
                </td>
                <td align="left">
                    <asp:Label ID="Label5" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblGoldLoanNo" runat="server" class="label" Font-Names="Verdana"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Applied Date
                </td>
                <td align="left">
                    <asp:Label ID="Label12" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblLoanDate" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Applicant Name
                </td>
                <td align="left">
                    :
                </td>
                <td align="left" colspan="4" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblAppName" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" valign="top" align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="Label1" runat="server" Text="Address" class="label" Font-Names="Verdana"
                        Font-Size="11px"></asp:Label>
                </td>
                <td align="left" valign="top">
                    <asp:Label ID="Label4" Style="font-family: Verdana; vertical-align: top; font-size: 11px"
                        runat="server" class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td colspan="4" valign="top" align="left" style="height: 50px; font-family: Verdana;
                    font-size: 11px;">
                    <div class="big">
                        <asp:Label ID="lblAddress" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                    </div>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Loan Type
                </td>
                <td align="left">
                    <asp:Label ID="Label2" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left">
                    <asp:Label ID="lblLoanType" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 2px;" colspan="6">
                </td>
            </tr>
            <%--<tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: small">
                    Loan Type
                </td>
                <td align="left">
                    :
                </td>
               
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>--%>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Date Of Birth
                </td>
                <td align="left">
                    <asp:Label ID="Label3" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblDOB" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    PAN No
                </td>
                <td align="left">
                    :
                </td>
                <td align="left" style="font-family: Verdana; font-size: small">
                    <asp:Label ID="lblPanNo" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 8px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Mobile No
                </td>
                <td align="left">
                    :
                </td>
                <td align="left" style="font-family: Verdana; font-size: small">
                    <asp:Label ID="lblMobileNo" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Telephone No
                </td>
                <td align="left">
                    <asp:Label ID="Label6" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblTelNo" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Email Address
                </td>
                <td align="left">
                    <asp:Label ID="Label7" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td colspan="4" align="left" style="font-family: Verdana; font-size: 11px">
                    <asp:Label ID="lblEmail" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6" align="left">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Nominee Name
                </td>
                <td align="left">
                    <asp:Label ID="Label8" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left" style="font-family: Verdana; font-size: small">
                    <asp:Label ID="lblnominiee" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 10px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Nominee's Relation
                </td>
                <td align="left">
                    <asp:Label ID="Label9" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td align="left">
                    <asp:Label ID="lblRelation" runat="server" class="label" Font-Names="Verdana" Font-Size="11px"></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td style="height: 20px;" colspan="6">
                </td>
            </tr>
            <tr>
                <td class="label" align="left" style="font-family: Verdana; font-size: 11px">
                    Applicant's Photo
                </td>
                <td align="left">
                    <asp:Label ID="Label10" Style="font-family: Verdana; font-size: 11px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td>
                    <asp:Image ID="imgPhoto" runat="server" Height="120px" Width="120px" />
                    <%--<asp:ImageButton ID="imgPhoto" runat="server" />--%>
                </td>
                <td class="label" align="right" style="font-family: Verdana; font-size: 11px">
                    Signature
                </td>
                <td align="left">
                    <asp:Label ID="Label11" Style="font-family: Verdana; font-size: 10px" runat="server"
                        class="label" Font-Names="Verdana" Font-Size="11px" Text=":"></asp:Label>&nbsp;&nbsp;
                </td>
                <td>
                    <asp:Image ID="imgSign" runat="server" Height="120px" Width="120px" />
                </td>
            </tr>
            <tr>
                <td style="height: 20px;">
                </td>
            </tr>
        </table>
    </div>
    <div>
        <%--<asp:Button ID="btnprint" runat="server" Text="Print" class="button" onclick="PrintDiv()" />--%>
        <input class="button" id="btnprint" type="button" onclick="PrintDiv()" value="Print" />
    </div>
    </form>
</body>
</html>
