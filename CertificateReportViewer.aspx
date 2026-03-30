<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="CertificateReportViewer.aspx.cs" Inherits="CertificateReportViewer"
    Theme="GridViewTheme" EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
 
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
            var popupWin = window.open('', '_blank', 'location=no,left=200px,top=80px');
            popupWin.document.open();
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='print' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='screen' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</html>');
            popupWin.document.close();
        }
        function btnprint_onclick() {

        }

    </script>
    <div id="divBackground" class="modal" align="right">
    </div>
    <div id="divImage" align="right">
        <table style="height: 100%; width: 100%">
            <tr>
                <td valign="middle" align="center">
                    <img id="imgLoader" alt="" src="" />
                    <img id="imgFull" alt="" src="" style="display: none; height: 300px; width: 300px" />
                </td>
            </tr>
            <tr>
                <td align="center" valign="bottom">
                    <input id="btnClose" type="button" value="close" onclick="HideDiv()" />
                </td>
            </tr>
        </table>
    </div>
    <table align="center" cellpadding="0" cellspacing="0" width="93%" border="0">
        <tr>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
            <td style="width: 180px;">
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4">
                <input class="button" id="btnprint" type="button" onclick="PrintDiv()" value="Print"
                    runat="server" />
            </td>
        </tr>
    </table>
    <div id="printarea">
        <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
            <tr>
                <td style="width: 180px;">
                </td>
                <td style="width: 180px;">
                </td>
                <td style="width: 180px;">
                </td>
                <td style="width: 180px;">
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div align="center" style="background-color: White; border: 2px solid black; width: 95%;">
                        <table align="center" cellpadding="0" cellspacing="0" width="100%" border="0">
                            <tr>
                                <td style="width: 50px;">
                                </td>
                                <td style="width: 50px;">
                                </td>
                            </tr>
                            <tr class="Staticheader ">
                                <th rowspan="3" align="left">
                                    <img style="width: 120px; height: 80px; vertical-align: top;" src="images/ASPL_LOGO.png"
                                        alt="APHELION FINANCE PVT LTD" />
                                </th>
                                <td align="left" valign="top">
                                    <%--  <p style="color: Black; font-size: 18px; left: 40%; font-family: Copperplate Gothic Bold;
                                        position: absolute; vertical-align: top; top: 20%;">--%>
                                    <asp:Label ID="Label10" Style="font-size: 19px; font-family: Copperplate Gothic Bold;"
                                        class="label" runat="server" Text="" ForeColor="#CC9900"></asp:Label>
                                    <br />
                                    <asp:Label ID="Label11" ForeColor="#CC9900" Style="font-size: 25px; left: 80%; font-family: Copperplate Gothic Bold;"
                                        class="label" runat="server" Text="APHELION FINANCE PVT. LTD."></asp:Label>
                                    <br />
                                    <asp:Label ID="Label14" class="label" runat="server" ForeColor="#CC9900" Style="font-size: 15px;
                                        top: 30%; left: 49%;" Text=""></asp:Label>
                                    <%--  </p>--%>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" valign="top">
                                    <%-- <p style="color: Black; font-size: 30px; left: 50%; font-family: Copperplate Gothic Bold;
                                        position: absolute; vertical-align: top; top: 21%;">--%>
                                    <%--   <asp:Label ID="Label13" class="label" runat="server" Text=" NANCY FINCORP"></asp:Label>--%>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <%-- <p style="color: Black; font-size: 15px; top: 30%; left: 49%; position: absolute;
                                        vertical-align: top; font-family: Cambria;">--%>
                                    <%--<asp:Label ID="Label12" class="label" runat="server" Text=" (Unit of Rajeshwari Fincom Limited)"></asp:Label>--%>
                                    <%--  </p>--%>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div align="center" style="background-color: White; border: 2px solid black; width: 95%;">
                        <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
                            <tr>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                            </tr>
                            <%--<tr class="Staticheader ">
                                <td align="left">
                                    <img style="width: 120px; height: 80px; vertical-align: top;" src="images/FINCORP_LOGO.png"
                                        alt="KALAYIL NANCY FINCORP" />
                                </td>
                                <td colspan="3" align="center">
                                    <p style="color: White; font-size: medium; position: absolute; left: 47%; text-align: center;
                                        vertical-align: top; font-family: Copperplate Gothic Bold;">
                                        KALAYIL
                                    </p>
                                    <p style="color: White; font-size: 30px; left: 55%; font-family: Copperplate Gothic Bold;
                                        vertical-align: top;">
                                        &nbsp; NANCY FINCORP
                                    </p>
                                    <p style="color: White; font-size: 15px; top: 25%; left: 49%; position: absolute;
                                        font-family: Cambria;">
                                        (Unit of Rajeshwari Fincom Limited)
                                    </p>
                                </td>
                            </tr>--%>
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
                                                <asp:Label ID="lblHeader" runat="server" Font-Underline="true" Text="Certificate"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                &nbsp;
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
                                <td colspan="4" class="label" align="justify">
                                    <div align="justify">
                                        <p>
                                            <asp:Label ID="Label4" runat="server" Font-Names="Verdana" Font-Size="12px" Text="I state that upto Rs."></asp:Label>
                                            <asp:Label ID="lblLoanSanctioned" runat="server" Font-Names="Verdana" Font-Size="12px"
                                                Font-Bold="true"></asp:Label>
                                            <asp:Label ID="Label5" runat="server" Font-Names="Verdana" Font-Size="12px" Text="could be allowed as loan against the articles of the weight certified."></asp:Label>
                                        </p>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="right" colspan="4" class="label">
                                    <br />
                                    <asp:Label ID="lblAppraiser" runat="server" Text="Appraiser"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 30px;">
                                    &nbsp;
                                </td>
                            </tr>
                            <%--  <tr>
                                <td colspan="4">
                                    &nbsp;
                                </td>
                            </tr>--%>
                            <tr>
                                <td align="right" colspan="4" class="label">
                                    <br />
                                    <asp:Label ID="lblSignature" runat="server" Text="(Signature)"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <div align="center" style="background-color: White; border: 2px solid black; width: 95%;">
                        <table align="center" cellpadding="0" cellspacing="0" width="95%" border="0">
                            <tr>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                                <td style="width: 180px;">
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 40px;" align="center" class="header" colspan="4">
                                    <asp:Label ID="lblPledge" runat="server" Font-Underline="true" Text="Pledge Token"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 15px;" align="center" colspan="4">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left" class="label">
                                    <div align="left">
                                        <asp:Label ID="lblGoldLoanNo" runat="server" Text="Gold Loan No "></asp:Label>
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:
                                    </div>
                                </td>
                                <td align="left" class="label">
                                    <div align="left">
                                        <asp:Label ID="lblGoldNo" runat="server" Font-Size="12px" Font-Names="Verdana"></asp:Label>
                                    </div>
                                </td>
                                <td align="left" class="label">
                                    <div align="left">
                                        <asp:Label ID="lblIssue" runat="server" Text="Issue Date"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;:
                                    </div>
                                </td>
                                <td align="left" class="label">
                                    <div align="left">
                                        <asp:Label ID="lblIssueDate" runat="server" Font-Size="12px" Font-Names="Verdana"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td class="label" align="left">
                                    <div align="left">
                                        <asp:Label ID="lblApp" runat="server" Text="Applicant Name" ReadOnly="true"></asp:Label>&nbsp;&nbsp;&nbsp;:
                                    </div>
                                </td>
                                <td class="label" align="left" colspan="2">
                                    <div align="left">
                                        <asp:Label ID="lblAppName" runat="server" Font-Size="12px" Font-Names="Verdana"></asp:Label>
                                    </div>
                                </td>
                                <td style="height: 100px;">
                                    <br />
                                    <br />
                                    <br />
                                    <asp:Label ID="Label9" runat="server" class="label" Text="Applicant Photo"></asp:Label><br />
                                    <asp:Image ID="imgAppPhoto" runat="server" Height="90px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label" align="left">
                                    <div align="left">
                                        <asp:Label ID="lblNetLoan" runat="server" Text="Net Loan Amount"></asp:Label>
                                        <asp:Label ID="Label15" Style="text-indent=100em;" runat="server" Text=":"></asp:Label>
                                        <%--<p style="text-indent=3em;">:</p>--%>
                                    </div>
                                </td>
                                <td class="label" align="left" colspan="3">
                                    <div align="left">
                                        <asp:Label ID="lblNetLoanAmount" runat="server" Font-Size="12px" Font-Names="Verdana"></asp:Label>
                                        <asp:Label ID="Label8" runat="server" Font-Size="12px" Text="/-" Font-Names="Verdana"></asp:Label>&nbsp;&nbsp;
                                        (<asp:Label ID="lblAmtInWord" runat="server" Font-Size="12px" Font-Names="Verdana"></asp:Label>)
                                        <%--<asp:TextBox ID="txtNetLoanAmount" runat="server" class="textbox" ReadOnly="true"></asp:TextBox>--%>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 20px;">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="label" colspan="4" align="left">
                                    <div align="left">
                                        <asp:Label ID="lblAppDetails" runat="server" Font-Bold="true" Font-Size="Medium"
                                            Font-Underline="true" Text="Applicant's Gold Item Details"></asp:Label>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td align="center" colspan="4">
                                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="GID" AutoPostBack="true"
                                                Width="100%" OnPageIndexChanging="dgvDetails_PageIndexChanging">
                                                <Columns>
                                                    <asp:TemplateField HeaderText="Sr. No." ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <%#Container.DataItemIndex+1%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="GID" Visible="false">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblGID" align="center" Width="60px" runat="server" Text='<%# Eval("GID") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Gold Item Name" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblGoldItemName" align="center" Width="60px" runat="server" Text='<%# Eval("ItemName") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Gross Weight" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblGrossWeight" runat="server" Width="60px" Text='<%# Eval("GrossWeight") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                     <asp:TemplateField HeaderText="Quantity" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:Label ID="lblQuantity" runat="server" Width="60px" Text='<%# Eval("Quantity") %>'></asp:Label>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                    <asp:TemplateField HeaderText="Photo" ItemStyle-HorizontalAlign="Center">
                                                        <ItemTemplate>
                                                            <asp:ImageButton ID="ImageButton1" runat="server" Width="200px" Height="150px" OnClientClick="return LoadDiv(this.src);"
                                                                ToolTip="Click To View Full Size" AlternateText="Click To View Full Size" ImageUrl='<%#Eval("PhotoPath") %>' />
                                                            <%-- <asp:Image ID="imgPhoto" runat="server" Width="100px" Height="60px" OnClientClick="return LoadDiv(this.src);"
                                                            ImageUrl='<%#Eval("PhotoPath") %>' />--%>
                                                        </ItemTemplate>
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <div align="left">
                                        <asp:Label ID="lblGross" class="label" runat="server" Text="Total Gross Weight"></asp:Label>&nbsp;&nbsp;&nbsp;:
                                    </div>
                                </td>
                                <td class="label" align="left">
                                    <div align="left">
                                        <asp:Label ID="lblGrossWeight" class="label" Font-Bold="true" runat="server"></asp:Label>
                                        <asp:Label ID="Label12" class="label" Font-Bold="true" Text="(gram)" runat="server"></asp:Label>
                                        <%-- <asp:TextBox ID="txtTotalGrossWeight" class="textbox" ReadOnly="true" runat="server"></asp:TextBox>--%>
                                    </div>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td align="left">
                                    <div align="left">
                                        <asp:Label ID="lblNet" class="label" runat="server" Text="Net Weight"></asp:Label>&nbsp;&nbsp;&nbsp;&nbsp;:
                                    </div>
                                </td>
                                <td class="label" align="left">
                                    <div align="left">
                                        <asp:Label ID="lblNetWeight" Font-Bold="true" class="label" runat="server"></asp:Label><asp:Label
                                            ID="Label13" Font-Bold="true" class="label" runat="server" Text="(gram)"></asp:Label>
                                        <%-- <asp:TextBox ID="txtNetWeight" class="textbox" ReadOnly="true" runat="server"></asp:TextBox>--%>
                                    </div>
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                                <td>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td style="height: 15px;" colspan="4">
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="justify">
                                    <asp:Label ID="Label6" class="label" runat="server" Font-Size="Medium" Text="I do hereby state that the articles pledged with you, as per the details above, are owned by me. I also state that I will redeem the same within the prescribed time limit by remitting the principal amount and interest thereon. I promise to abide by the conditions, given overleaf. In case I fail to redeem the pledged articles, within the time limit, I authorize Nancy Fincorp (unit of rajeshwari fincom ltd) to auction/sell the pledged articles to realize the loan amount and interest. If the amount so realized happens to be insufficient to cover the liabilities of the pledge. Nancy Fincorp (unit of rajeshwari fincom ltd) will have the rights to realize the balance dues by charging on my other assets."></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" style="height: 50px;">
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td colspan="4" align="right">
                                    <asp:Label ID="Label7" class="label" runat="server" Text="(Signature)"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
