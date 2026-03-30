<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLInward Form.aspx.cs" Inherits="GLInward_Form" Theme="GridViewTheme"
    EnableViewStateMac="false" MaintainScrollPositionOnPostback="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <table align="left" cellpadding="0" cellspacing="0" border="0"  
        style="width: 95%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 20%;">
            </td>
            <td style="width: 30%;">
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
                    <tr>
                        <td colspan="4">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="4" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GOLD LOAN Inward Form"></asp:Label>
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
        <tr>
            <!--  -->
            <td class="label" colspan="1" style="height: 37px;">
                Ref. No.:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRefNo" CssClass="textbox_readonly" runat="server" 
                    Width="90%"></asp:TextBox>
            </td>
            <td class="label" align="right">
                <%-- GL Case No.:--%>
                <asp:Label ID="lblglcaseno" runat="server" Text="GL Case No:"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtGlcaseNo" CssClass="textbox_readonly" runat="server"
                    Width="90%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" align="right">
                GL Loan Date:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtloanDate" CssClass="textbox_readonly" runat="server" 
                    Width="90%"></asp:TextBox>
            </td>
            <td class="label">
                Name:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtName" CssClass="textbox_readonly" runat="server" 
                    Width="90%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!--  -->
            <td class="label" align="right">
                Inward Type
            </td>
            <td class="txt_style" >
                <asp:DropDownList ID="ddlInwardType" runat="server" CssClass="textbox" Width="95%"
                    Height="27px" AutoPostBack="True" OnSelectedIndexChanged="ddlInwardType_SelectedIndexChanged">
                    <asp:ListItem Value="0">--Select Inward Type--</asp:ListItem>
                    <asp:ListItem Value="Gold">Gold</asp:ListItem>
                    <asp:ListItem Value="Document">Document</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <td align="right">
                <asp:Label ID="lblInwardType" runat="server" Width="90%" CssClass="label"></asp:Label>
            </td>
            <td>
                <asp:MultiView ID="mvOnselectionInwarttype" runat="server" ActiveViewIndex="0">
                    <asp:View ID="vGold" runat="server">
                        <asp:GridView ID="gvGoldDetails" runat="server" AutoGenerateColumns="false" Width="97%"
                        PageSize="20" >
                        <HeaderStyle CssClass="gVHeader"/>
                        <RowStyle CssClass="gVItem" />
                            <Columns>
                                 <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderTemplate>
                                        Sr. NO
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="GoldInwardId" Visible="false"></asp:TemplateField>
                                <asp:TemplateField HeaderText="Item ID" Visible="false"></asp:TemplateField>
                                <asp:TemplateField HeaderText="Item Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Weight" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inward Stage" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inward From" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Inward From Location" HeaderStyle-HorizontalAlign="Left"
                                    ItemStyle-HorizontalAlign="Left">
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                 </asp:TemplateField>
                                <asp:TemplateField HeaderText="Stock Location" HeaderStyle-HorizontalAlign="Left"
                                    ItemStyle-HorizontalAlign="Left">
                                    
                                    <HeaderStyle HorizontalAlign="Left" />
                                    <ItemStyle HorizontalAlign="Left" />
                                    
                                 </asp:TemplateField>
                            </Columns>
                              
                        </asp:GridView>
                    </asp:View>
                    <asp:View ID="vDoc" runat="server">
                        <asp:GridView ID="gvDocumentDetails" runat="server" AutoGenerateColumns="false" 
                            PageSize="20" style="margin-top: 0px">
                            <Columns>
                                <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                    <ItemTemplate>
                                        <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                    </ItemTemplate>
                                    <FooterStyle HorizontalAlign="Center" />
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                    <HeaderTemplate>
                                        Sr. NO
                                    </HeaderTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="DocID" Visible="False"></asp:TemplateField>
                                <asp:TemplateField HeaderText="Doc. Name" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left">
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Select" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSelect" runat="server" AutoPostBack="true" />
                                    </ItemTemplate>
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Stock Location" HeaderStyle-HorizontalAlign="Left"
                                    ItemStyle-HorizontalAlign="Left"></asp:TemplateField>
                            </Columns>
                            <HeaderStyle CssClass="gVHeader" />
                        <RowStyle CssClass="gVItem" />
                        </asp:GridView>
                    </asp:View>
                </asp:MultiView>
           
           
            </td>
        </tr>
        <tr>
            <td class="label">
                Inward By:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtinwardby" CssClass="textbox_readonly" runat="server" 
                    Width="90%"></asp:TextBox>
            </td>
            <td class="label">
                Date And Time:
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtDatetime" CssClass="textbox_readonly" runat="server" 
                    Width="90%"></asp:TextBox>
            </td>
        </tr>
    </table>
</asp:Content>
