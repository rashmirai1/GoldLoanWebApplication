<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" EnableEventValidation="false"
    AutoEventWireup="true" CodeFile="GLGoldInOutForm.aspx.cs" Inherits="GLGoldInOutForm" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">

    <script type="text/javascript">


        function valid() {

            var txtGoldNo = document.getElementById('<%=txtGoldNo.ClientID%>');
            var txtpouchno = document.getElementById('<%=txtpouchno.ClientID%>');
            var ddlInOutMode = document.getElementById('<%=ddlInOutMode.ClientID%>');
            var ddlInOutLocation = document.getElementById('<%=ddlInOutLocation.ClientID %>');
            var ddllocname = document.getElementById('<%=ddllocname.ClientID %>');
            var txtlocdetails = document.getElementById('<%=txtlocdetails.ClientID %>');
            var ddlnarration = document.getElementById('<%=ddlnarration.ClientID %>');
            var ddlInOutBy = document.getElementById('<%=ddlInOutBy.ClientID %>');

            if (ddlInOutMode.selectedIndex == 0) {
                alert("Select Mode");
                return false;
            }

            if (txtGoldNo.value == '') {
                alert("Select Gold Loan No");
                return false;
            }


            if (txtpouchno.value == '') {
                alert("Enter Pouch No");
                return false;
            }

            if (ddlInOutLocation.selectedIndex == 0) {
                alert("Select Inward/Outward Location");
                return false;
            }

            if (ddlInOutLocation.selectedIndex == 0 || ddlInOutLocation.selectedIndex == 1) {
                if (ddllocname.selectedIndex == 0) {
                    alert("Select Location Name");
                    return false;
                }
            }
            if (txtlocdetails.value == '') {
                alert("Enter Location Details");
                return false;
            }
            if (ddlnarration.selectedIndex == 0) {
                alert("Select Narration");
                return false;

            }

            if (ddlInOutBy.selectedIndex == 0) {
                alert("Select Inward/Outward By");
                return false;
            }




        }

        function isAlphaNum(e) { // Alphanumeric,space,comma
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 44 || k == 40 || k == 41));
        }
    </script>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnrefno" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnuserid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnfyid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnbranchid" runat="server" Value="0" />
    <asp:HiddenField ID="hdntime" runat="server" Value="0" />
    <asp:HiddenField ID="hdnKYCID" runat="Server" />
    <asp:HiddenField ID="hdncustomeradd" runat="server" />
    <asp:HiddenField ID="hdnsdid" runat="Server" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 10%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 32%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="4" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GOLD Inward/Outward Details"></asp:Label>
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
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" width="100%" id="table1">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Reference No.
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:TextBox ID="txtreferenceno" CssClass="textbox_readonly textbox_GLreceipt" class="textbox"
                    Width="58%" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- DATE -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Date & Time
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:TextBox ID="txtdate" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                    Width="58%" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Select Mode <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel1" runat="server">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutMode" runat="server" Height="28px" Width="60%" CssClass="textbox"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlInOutMode_SelectedIndexChanged">
                            <asp:ListItem Value="0">--Select Mode--</asp:ListItem>
                            <asp:ListItem Value="I">Inward</asp:ListItem>
                            <asp:ListItem Value="O">Outward</asp:ListItem>
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- GL No -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Gold Loan No<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="90%">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="updatepanel111" runat="server">
                                <ContentTemplate>
                                    <asp:TextBox ID="txtGoldNo" CssClass="textbox_readonly textbox_GLreceipt" runat="server"
                                        Width="200px"></asp:TextBox>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                        <td style="float: left; padding-left: 2px; margin: 0px;">
                            <asp:ImageButton ID="btnGlSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                Width="20px" runat="server" ImageAlign="AbsMiddle" ToolTip="Click for search gold loan no"
                                OnClick="btnGlSearch_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <!-- Pouch No  -->
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Pouch No.
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtpouchno" runat="server" Width="58%" CssClass="textbox_readonly textbox_GLreceipt"
                            class="textbox"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <!-- Gold Item Details Section -->
            <td class="label" style="text-align: left; padding-left: 17px;" valign="top" width="28%">
                Pouch Details
            </td>
            <td class="label" width="72%">
                <asp:UpdatePanel ID="updatepanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="dgvGoldItemDetails" runat="server" ShowFooter="true" AutoGenerateColumns="false">
                            <AlternatingRowStyle BackColor="White" />
                            <HeaderStyle CssClass="gVHeader" />
                            <Columns>
                                <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1%>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Gold Item Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                    <ItemTemplate>
                                        <asp:Label ID="lblitemName" runat="server" Text='<%#Eval("ItemName") %>' Width="30px"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txttot" Style="text-align: center; color: Black;" MaxLength="5" runat="server"
                                            Width="50px"></asp:Label>
                                    </FooterTemplate>
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quantity">
                                    <ItemTemplate>
                                        <asp:Label ID="txtQuantity" runat="server" MaxLength="3" Style="text-align: center;"
                                            Width="50px" Text='<%# Eval("Quantity") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txtTotalQuantity" Style="text-align: center; color: Black;" MaxLength="5"
                                            runat="server" Width="50px"></asp:Label>
                                    </FooterTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <FooterStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Gross Wt.(g)">
                                    <ItemTemplate>
                                        <asp:Label ID="txtGrossWeight" runat="server" MaxLength="8" Style="text-align: center;"
                                            Width="50px" Text='<%# Eval("GrossWeight") %>'></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txtTotalGrossWeight" runat="server" MaxLength="8" Style="text-align: center;
                                            color: Black;" Width="50px"></asp:Label>
                                    </FooterTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <FooterStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Net Wt.(g)">
                                    <ItemTemplate>
                                        <asp:Label ID="txtNetWeight" runat="server" MaxLength="8" Style="text-align: center;"
                                            Text='<%# Eval("NetWeight") %>' Width="50px"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txtTotalNetWeight" Style="text-align: center; color: Black;" MaxLength="8"
                                            runat="server" Width="50px"></asp:Label>
                                    </FooterTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Karat">
                                    <ItemTemplate>
                                        <asp:Label ID="lblkarat" runat="server" MaxLength="8" Style="text-align: center;"
                                            Text='<%# Eval("Purity") %>' Width="50px"></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        Rate/Gram
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label ID="txtRatePerGram" runat="server" MaxLength="8" Style="text-align: center;"
                                            Text='<%# Eval("RateperGram") %>' Width="50px" Visible="true"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txtTotalRatePerGram" runat="server" MaxLength="8" Style="text-align: center;
                                            color: Black;" Width="50px" Visible="false"></asp:Label>
                                    </FooterTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Value">
                                    <ItemTemplate>
                                        <asp:Label ID="txtValue" runat="server" Style="text-align: center; color: Black;"
                                            Text='<%# Eval("Value") %>' Width="70px"></asp:Label>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="txtTotalValue" Style="text-align: center; color: Black;" MaxLength="8"
                                            runat="server" Width="70px"></asp:Label>
                                    </FooterTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%" valign="top">
                Last Inward/Outward Details
            </td>
            <td class="label" width="72%">
                <asp:UpdatePanel ID="updatepanel2" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvPrevDetails" runat="server" AutoGenerateColumns="False" Width="72%">
                            <AlternatingRowStyle BackColor="White" />
                            <HeaderStyle CssClass="gVHeader" />
                            <Columns>
                                <asp:TemplateField HeaderText="Sr.No.">
                                    <ItemTemplate>
                                        <%#Container.DataItemIndex+1%>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inward/Outward.">
                                    <ItemTemplate>
                                        <asp:Label ID="gvlblInOutmode" runat="server" Text='<%#Eval("Inward/Outward Mode") %>'
                                            Width="50px"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Reference No.">
                                    <ItemTemplate>
                                        <asp:Label ID="gvlblrefno" runat="server" Text='<%#Eval("Reference No") %>' Width="80px"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location Name.">
                                    <ItemTemplate>
                                        <asp:Label ID="gvlbllocation" runat="server" Text='<%#Eval("Location Name") %>' Width="170px"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Location Details.">
                                    <ItemTemplate>
                                        <asp:Label ID="gvlbllocdet" runat="server" Text='<%#Eval("Location Details") %>'
                                            Width="110px"></asp:Label>
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Inward/Outward Location <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel3" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutLocation" runat="server" Width="60%" Height="28px"
                            CssClass="textbox" AutoPostBack="true" OnSelectedIndexChanged="ddlInOutLocation_SelectedIndexChanged">
                            <asp:ListItem Value="0">--Select Inward Outward Location--</asp:ListItem>
                            <asp:ListItem Value="Bank">Bank</asp:ListItem>
                            <asp:ListItem Value="Residence">Residence</asp:ListItem>
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Location Name <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddllocname" runat="server" Height="28px" Width="60%" CssClass="textbox">
                        </asp:DropDownList>
                        <asp:TextBox ID="txtlocname" runat="server" Width="57%" CssClass="textbox" Visible="false"
                            onkeypress="return isAlphaNum(event);"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Location Details <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:TextBox ID="txtlocdetails" runat="server" Width="57%" TextMode="MultiLine" Rows="3"></asp:TextBox>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutLocation" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="28%">
                Narration <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="72%">
                <asp:UpdatePanel ID="updatepanel8" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlnarration" runat="server" Height="28px" Width="60%" CssClass="textbox">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="25%">
                Inward/Outward By <b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="75%">
                <asp:UpdatePanel ID="updatepanel9" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlInOutBy" runat="server" Height="28px" Width="60%" CssClass="textbox">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlInOutMode" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
