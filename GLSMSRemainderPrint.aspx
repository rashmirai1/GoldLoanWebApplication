<%@ Page Language="C#" AutoEventWireup="true" CodeFile="GLSMSRemainderPrint.aspx.cs"
    Inherits="GLSMSRemainderPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div style="background-color: White">
        <div id="divPrint" style="display: none;">
            <asp:GridView ID="GridViewPrint" runat="server" AutoGenerateColumns="false">
                <Columns>
                    <asp:TemplateField HeaderText="Sr. No." HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <%#Container.DataItemIndex+1%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" CssClass="gVItem" />
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Gold Loan No">
                        <ItemTemplate>
                            <asp:Label ID="lblgoldloanno" runat="server" Text='<%#Eval("GL No")%>'></asp:Label>
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
                            <asp:Label ID="lblmobileno" runat="server" Text='<%#Eval("Mobile No")%>'></asp:Label>
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
    </div>
    </form>
</body>
</html>
