<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLSMSRemainder.aspx.cs" Inherits="GLSMSRemainder" EnableEventValidation="false"
    EnableViewStateMac="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">


        function checkAll(objRef) {
            var GridView = objRef.parentNode.parentNode.parentNode;
            var inputList = GridView.getElementsByTagName("input");
            for (var i = 0; i < inputList.length; i++) {
                //Get the Cell To find out ColumnIndex
                var row = inputList[i].parentNode.parentNode;
                if (inputList[i].type == "checkbox" && objRef != inputList[i]) {
                    if (objRef.checked) {

                        inputList[i].checked = true;
                    }
                    else {

                        inputList[i].checked = false;
                    }
                }
            }
        }


        function printGrid() {
            var gridData = document.getElementById('<%= GridViewPrint.ClientID %>');
            var ToDate = document.getElementById('<%= txtdate.ClientID %>');
            var windowUrl = '';
            //set print document name for gridview
            var uniqueName = new Date();
            var windowName = 'Print_' + uniqueName.getTime();
            var prtWindow = window.open(windowUrl, windowName);

            prtWindow.document.write('<html><head><center><b><u>SMS Remainder List On ' + ToDate.value + '</u></b><center></head>');
            prtWindow.document.write('</br>');
            prtWindow.document.write('<br />');
            prtWindow.document.write('<body style="background:none !important">');
            prtWindow.document.write(gridData.outerHTML);
            prtWindow.document.write('</body></html>');
            prtWindow.document.close();
            prtWindow.focus();
            prtWindow.print();
            prtWindow.close();
        }
    </script>
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnuserid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnfyid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnbranchid" runat="server" Value="0" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 17%;">
                <br />
            </td>
            <td style="width: 37%;">
            </td>
            <td style="width: 2%;">
            </td>
            <td style="width: 5%;">
            </td>
            <td style="width: 37%;">
            </td>
            <td style="width: 2%;">
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 100%;">
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <!--Header -->
                    <tr>
                        <td align="center" colspan="6" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL SMS Reminder"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            <div class="barstyle">
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="6">
                            &nbsp;
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="23%">
                Date & Time :
            </td>
            <td class="txt_style" style="text-align: left; padding-right: 150px;" width="77%">
                <asp:TextBox ID="txtdate" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                    Width="58%" runat="server"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="23%" valign="top">
                Reminder Type :
            </td>
            <td class="label" style="text-align: left;" width="77%">
                <asp:RadioButtonList runat="server" ID="Rdbremainder" RepeatDirection="Vertical"
                    RepeatLayout="Flow" CssClass="label" OnSelectedIndexChanged="Rdbremainder_SelectedIndexChanged"
                    AutoPostBack="true">
                    <asp:ListItem Text="Interest Reminder" Value="0"></asp:ListItem>
                    <asp:ListItem Text="Repayment Reminder Prior" Value="1"></asp:ListItem>
                    <asp:ListItem Text="Repayment Reminder After" Value="2"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="23%" valign="top">
                SMS Template :
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="77%">
                <asp:TextBox ID="txtsmstemplate" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                    Width="58%" runat="server" TextMode="MultiLine" Style="height: 79px; text-decoration: none;
                    overflow: hidden; resize: None; text-align: left;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="23%">
                SMS Description :
            </td>
            <td class="txt_style" style="padding-right: 150px;" width="77%">
                <asp:TextBox ID="txtsmsdesc" class="textbox" CssClass="textbox_readonly textbox_GLreceipt"
                    Width="58%" runat="server"></asp:TextBox>
            </td>
        </tr>
    </table>
    <br />
    <table cellpadding="0" cellspacing="0" width="100%" id="tblgrd">
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;" width="100%">
                <asp:GridView ID="dgvRemainder" runat="server" ShowFooter="true" EmptyDataText="No Record Found"
                    AutoGenerateColumns="false">
                    <AlternatingRowStyle BackColor="White" />
                    <HeaderStyle CssClass="gVHeader" />
                    <Columns>
                        <asp:TemplateField HeaderText="SDID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblsdid" runat="server" Text='<%#Eval("SDID")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="KYCID" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblkycid" runat="server" Text='<%#Eval("KYCID")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="DueDate" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblduedate" runat="server" Text='<%#Eval("DueDate")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="lblSRno" runat="server" Text='<%#Eval("SrNo")%>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gold Loan No">
                            <ItemTemplate>
                                <asp:Label ID="lblgoldloanno" runat="server" Text='<%#Eval("GLNo")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblname" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Mobile No">
                            <ItemTemplate>
                                <asp:Label ID="lblmobileno" runat="server" Text='<%#Eval("MobileNo")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Received Date">
                            <ItemTemplate>
                                <asp:Label ID="lbllastreceiveddate1" runat="server" Text='<%#Eval("Received_Date")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Last Received Date" Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lbllastreceiveddate" runat="server" Text='<%#Eval("Received Date")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Outstanding">
                            <ItemTemplate>
                                <asp:Label ID="lbloutstanding" runat="server" Text='<%#Eval("Outstanding")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Interest">
                            <ItemTemplate>
                                <asp:Label ID="lblinterest" runat="server" Text='<%#Eval("Interest")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" CssClass="gVItem" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                            </HeaderTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:CheckBox ID="chkselect" runat="server" Checked='<%#Eval("Checked")%>' />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            <FooterTemplate>
                                <asp:Button ID="btnsend" runat="server" Text="Send" OnClick="btnsend_Click" />
                            </FooterTemplate>
                            <FooterStyle HorizontalAlign="Center" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div id="divPrint" style="display: none;">
                    <asp:GridView ID="GridViewPrint" runat="server" AutoGenerateColumns="false">
                        <Columns>
                            <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label ID="lblSRnoo" runat="server" Text='<%#Eval("SrNo")%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Gold Loan No">
                                <ItemTemplate>
                                    <asp:Label ID="lblgoldloanno" runat="server" Text='<%#Eval("GLNo")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name">
                                <ItemTemplate>
                                    <asp:Label ID="lblname" runat="server" Text='<%#Eval("Name")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Mobile No">
                                <ItemTemplate>
                                    <asp:Label ID="lblmobileno" runat="server" Text='<%#Eval("MobileNo")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Last Received Date">
                                <ItemTemplate>
                                    <asp:Label ID="lbllastreceiveddate1" runat="server" Text='<%#Eval("Received_Date")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Outstanding">
                                <ItemTemplate>
                                    <asp:Label ID="lbloutstanding" runat="server" Text='<%#Eval("Outstanding")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" CssClass="gVItem" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Interest">
                                <ItemTemplate>
                                    <asp:Label ID="lblinterest" runat="server" Text='<%#Eval("Interest")%>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Right" CssClass="gVItem" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left; padding-left: 17px;">
                <asp:Button ID="btnPrint" runat="server" Text="Print" OnClick="btnPrint_Click" Visible="false" />
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
