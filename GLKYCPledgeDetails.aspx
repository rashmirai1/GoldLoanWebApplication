<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCPledgeDetails.aspx.cs" Inherits="GLKYCPledgeDetails" Theme="GridViewTheme"
    EnableViewStateMac="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
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
            var popupWin = window.open('', '_blank', 'location=no,left=200px');
            popupWin.document.open();
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='print' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write("<link href='css/PrintPledgeToken.css' media='screen' rel='stylesheet' type='text/css' />\n");
            popupWin.document.write('<html><body onload="window.print()">' + divToPrint.innerHTML + '</html>');
            popupWin.document.close();
        }
        function btnprint_onclick() {

        }

        function isNumericSlash(e) { // Numbers and slash
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k == 47));
        }

       
    </script>
    <asp:ScriptManager ID="ScriptManager2" runat="server">
    </asp:ScriptManager>
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
                            <asp:Label ID="lblHeader" runat="server" Text="GL KYC Pledge Details"></asp:Label>
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
        <tr>
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
            <td align="right" colspan="6">
                <asp:Label ID="Label28" runat="server" Text="[View Details Section]" Font-Names="Verdana"
                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
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
            <td colspan="4" style="height: 15px;">
            </td>
        </tr>
        <tr>
            <td colspan="4" align="right">
                <div>
                    <%--<asp:Button ID="btnprint" runat="server" Text="Print" class="button" onclick="PrintDiv()" />--%>
                    <input class="button" id="btnprint" type="button" onclick="PrintDiv()" value="Print" />
                </div>
            </td>
        </tr>
        <tr>
            <td align="left" colspan="4">
                <div id="printarea" align="center" style="background-color: White; border: 2px solid black;
                    width: 100%;">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" GridLines="Both" DataKeyNames="GoldLoanNo"
                                AutoPostBack="true" OnPageIndexChanging="dgvDetails_PageIndexChanging" AllowPaging="True"
                                AutoGenerateColumns="False" PageSize="5">
                                <Columns>
                                    <asp:TemplateField HeaderText="Sr. No." ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%#Container.DataItemIndex+1%>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="KYCID" ItemStyle-HorizontalAlign="Center" Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblKYCID" runat="server" Width="130px" Text='<%#Eval("KYCID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:HyperLinkField DataTextField="GoldLoanNo" AccessibleHeaderText="GoldLoanNo"
                                        HeaderText="Gold Loan No" DataNavigateUrlFields="KYCID" DataNavigateUrlFormatString="PledgeDetailsReport.aspx?ID={0}"
                                        Target="_blank" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="20%" />
                                    <asp:TemplateField HeaderText="Applicant Name" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblAppName" runat="server" Width="130px" Text='<%# Eval("ApplicantName") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Applied Date" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanDate" align="center" runat="server" Text='<%# Eval("LoanDate") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Loan Type" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanType" runat="server" Text='<%# Eval("LoanType") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Mobile No" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Label ID="lblMobileNo" runat="server" Text='<%# Eval("MobileNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Right" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Email ID" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="lblEmailId" runat="server" Text='<%# Eval("EmailID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Photo" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton1" runat="server" Width="90px" Height="90px" OnClientClick="return LoadDiv(this.src);"
                                                ToolTip="Click To View Full Size" AlternateText="Click To View Full Size" ImageUrl='<%#Eval("AppPhotoPath") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Sign" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="ImageButton2" runat="server" Width="90px" Height="90px" OnClientClick="return LoadDiv(this.src);"
                                                ToolTip="Click To View Full Size" AlternateText="Click To View Full Size" ImageUrl='<%#Eval("AppSignPath") %>' />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                <PagerSettings PageButtonCount="5" />
                            </asp:GridView>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
