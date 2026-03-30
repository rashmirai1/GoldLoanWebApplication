<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLAuthorization.aspx.cs" EnableEventValidation="false" EnableViewStateMac="false"
    MaintainScrollPositionOnPostback="true" Inherits="GLAuthorization" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">
        function checkAll(objRef) {

            //var GridView = objRef.parentNode.parentNode.parentNode;
            //var inputList = GridView.getElementsByTagName("input");
            var gridView = document.getElementById('<%=gvDetails.ClientID %>');
            var selectedRowIndex = Chk.parentNode.parentNode.rowIndex;
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
        //   $("[id$='chkVisibleHeding']").val(name);

        function ValidateCheckbox(Chk) {
            debugger;
            var gridView = document.getElementById('<%=gvDetails.ClientID %>');
            var selectedRowIndex = Chk.parentNode.parentNode.rowIndex;

            var chkVisibleHed = gridView.rows[parseInt(selectedRowIndex)].cells[0].children[0];
            var chkViewHed = gridView.rows[parseInt(selectedRowIndex)].cells[2].children[0];
            var chkSaveHed = gridView.rows[parseInt(selectedRowIndex)].cells[3].children[0];
            var chkEditHed = gridView.rows[parseInt(selectedRowIndex)].cells[4].children[0];
            var chkDeleteHed = gridView.rows[parseInt(selectedRowIndex)].cells[5].children[0];

            var chkVisible = gridView.rows[parseInt(selectedRowIndex)].cells[0].children[0];
            var chkView = gridView.rows[parseInt(selectedRowIndex)].cells[2].children[0];
            var chkSave = gridView.rows[parseInt(selectedRowIndex)].cells[3].children[0];
            var chkEdit = gridView.rows[parseInt(selectedRowIndex)].cells[4].children[0];
            var chkDelete = gridView.rows[parseInt(selectedRowIndex)].cells[5].children[0];

            // if (chkVisible.checked == false) {
            //     chkSave.checked = false;
            //     chkEdit.checked = false;
            //     chkDelete.checked = false;
            //     chkView.checked = false;
            // alert("Select Form Visibility First");
            //  return false;
            //  }
            //  else {
            //  }

            if (chkEdit.checked == true) {
                if (chkVisible.checked == false) {
                    chkEdit.checked = false;
                    alert("Select Form Visibility First");
                    return false;
                }
            }
            if (chkSave.checked == true) {
                if (chkVisible.checked == false) {
                    chkSave.checked = false;
                    alert("Select Form Visibility First");
                    return false;
                }
            }
            if (chkDelete.checked == true) {
                if (chkVisible.checked == false) {
                    chkDelete.checked = false;
                    alert("Select Form Visibility First");
                    return false;
                }
            }
            if (chkView.checked == true) {
                if (chkVisible.checked == false) {
                    chkView.checked = false;
                    alert("Select Form Visibility First");
                    return false;
                }
            }



        }
 
    </script>
    <script type="text/javascript">
        function valid() {
            var ddlUserType = document.getElementById("<%=ddlUserType.ClientID %>");
            var ddlUserName = document.getElementById("<%=ddlUserName.ClientID %>");
            var ddlMasterMenu = document.getElementById("<%=ddlMasterMenu.ClientID %>");


            if (ddlUserType.selectedIndex == 0) { alert('Select User Type'); return false; }
            if (ddlUserName.selectedIndex == 0) { alert('Select User Name'); return false; }
            if (ddlMasterMenu.selectedIndex == 0) { alert('Select Form Menu'); return false; }
        }
    
    </script>
    <script type="text/javascript" language="javascript">
        function CheckAllEmp(Checkbox) {
            var GridVwHeaderChckbox = document.getElementById("<%=gvDetails.ClientID %>");
            for (i = 1; i < GridVwHeaderChckbox.rows.length; i++) {
                GridVwHeaderChckbox.rows[i].cells[0].getElementsByTagName("INPUT")[0].checked = Checkbox.checked;
            }
        }
    </script>
    <script type="text/javascript">
        function ValidateVisiblegrid(Chk) {
            var gridView = document.getElementById('<%=gvDetails.ClientID %>');
            var selectedRowIndex = Chk.parentNode.parentNode.rowIndex;
            var chkVisible = gridView.rows[parseInt(selectedRowIndex)].cells[0].children[0];
            var chkView = gridView.rows[parseInt(selectedRowIndex)].cells[2].children[0];
            var chkSave = gridView.rows[parseInt(selectedRowIndex)].cells[3].children[0];
            var chkEdit = gridView.rows[parseInt(selectedRowIndex)].cells[4].children[0];
            var chkDelete = gridView.rows[parseInt(selectedRowIndex)].cells[5].children[0];

            // var chkViewHeding = gridView.rows[parseInt(selectedRowIndex)].cells[5].children[0];
            //  var chkVisibleHeding = gridView.rows[parseInt(selectedRowIndex)].cells[5].children[0];

            if (chkVisible.checked == false) {
                chkEdit.checked = false;
                chkSave.checked = false;
                chkDelete.checked = false;
                chkView.checked = false;
                //  chkViewHeding.checked = false;
                //  chkVisibleHeding.checked = false;
            }
        }
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 90%;">
        <asp:UpdatePanel ID="Panel1" runat="server">
            <ContentTemplate>
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
                        <asp:Label ID="Label1" runat="server" Text="Form Authorization">
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
                        User Type
                    </td>
                    <td class="txt_style">
                        <asp:DropDownList ID="ddlUserType" AutoPostBack="true" CssClass="textbox" runat="server"
                            Width="95%" Height="26px" OnSelectedIndexChanged="ddlUserType_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="text-align: left;">
                        User Name
                    </td>
                    <td class="txt_style">
                        <asp:DropDownList ID="ddlUserName" CssClass="textbox" runat="server" Width="95%"
                            AutoPostBack="true" Height="26px" OnSelectedIndexChanged="ddlUserName_SelectedIndexChanged">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="text-align: left;">
                        Form Menu
                    </td>
                    <td class="txt_style">
                        <asp:DropDownList ID="ddlMasterMenu" AutoPostBack="true" CssClass="textbox" runat="server"
                            OnSelectedIndexChanged="ddlMasterMenu_SelectedIndexChanged" Width="95%" Height="26px">
                        </asp:DropDownList>
                    </td>
                    <td>
                    </td>
                    <td>
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
                <tr>
                    <td colspan="4">
                        <asp:GridView ID="gvDetails" runat="server" Width="90%" AutoGenerateColumns="false"
                            ShowHeader="true" EmptyDataText="No Record Found." CssClass="textbox_readonly"
                            ForeColor="#ffffff" BackColor="#1f497d">
                            <Columns>
                                <%--  <asp:TemplateField HeaderText="Parent Form" HeaderStyle-HorizontalAlign="Center"
                                    ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Label ID="lblparentForm" runat="server" Text='<%#Eval("ParentForm") %>'></asp:Label>
                                        <asp:HiddenField ID="hdnparentid" runat="server" Value='<%#Eval("ParentID") %>' />
                                        <asp:HiddenField ID="hdnFormAuthID" runat="server" Value='<%#Eval("FormAuthID") %>' />
                                        <asp:HiddenField ID="hdnParentForm" runat="server" Value='<%#Eval("ParentForm") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="left"></ItemStyle>
                                </asp:TemplateField>--%>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkVisibleHeding" runat="server" Text="Visible" AutoPostBack="true"
                                            OnCheckedChanged="chkVisibleHeding_CheckedChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkVisible" runat="server" Checked='<%#Eval("IsVisible") %>' onclick="return ValidateVisiblegrid(this);" />
                                        <asp:HiddenField ID="hdnparentid" runat="server" Value='<%#Eval("ParentID") %>' />
                                        <asp:HiddenField ID="hdnFormAuthID" runat="server" Value='<%#Eval("FormAuthID") %>' />
                                        <asp:HiddenField ID="hdnParentForm" runat="server" Value='<%#Eval("ParentForm") %>' />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Form Name" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="hdnFormId" runat="server" Value='<%#Eval("FormID") %>' />
                                        <asp:Label ID="lblFormName" runat="server" Text='<%#Eval("FormName") %>'></asp:Label>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="left"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkViewHeding" runat="server" Text="View" AutoPostBack="true" OnCheckedChanged="chkViewHeding_CheckedChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkView" runat="server" Checked='<%#Eval("IsView") %>' onclick="return ValidateCheckbox(this);" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkSaveHeding" runat="server" Text="Save" AutoPostBack="True" OnCheckedChanged="chkSaveHeding_CheckedChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkSave" runat="server" Checked='<%#Eval("IsSave") %>' onclick="return ValidateCheckbox(this);" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkEditHeding" runat="server" Text="Edit" AutoPostBack="True" OnCheckedChanged="chkEditHeding_CheckedChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkEdit" runat="server" Checked='<%#Eval("IsEdit") %>' onclick="return ValidateCheckbox(this);" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkDeleteHeding" runat="server" Text="Delete" AutoPostBack="True"
                                            OnCheckedChanged="chkDeleteHeding_CheckedChanged" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkDelete" runat="server" Checked='<%#Eval("IsDelete") %>' onclick="return ValidateCheckbox(this);" />
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                                </asp:TemplateField>
                            </Columns>
                            <RowStyle CssClass="gVItem" Font-Size="15px" ForeColor="#1f497d" BackColor="#faf4b3" />
                            <HeaderStyle HorizontalAlign="Center" ForeColor="#ffffff" />
                        </asp:GridView>
                    </td>
                </tr>
                <asp:HiddenField ID="hdnOpration" runat="server" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </table>
</asp:Content>
