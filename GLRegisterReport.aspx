<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLRegisterReport.aspx.cs" Inherits="GLRegisterReport" EnableEventValidation="false"
    EnableViewStateMac="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script type="text/javascript">

        function SelectAll(id) {
            var frm = document.forms[0];
            for (i = 0; i < frm.elements.length; i++) {
                if (frm.elements[i].type == "checkbox") {
                    frm.elements[i].checked = document.getElementById(id).checked;
                }
            }
        }
        function valid(txt) {
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

            if (!pattern.test(txt.value)) {
                alert('Enter Date In Proper Format');
                return false;
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
        function onlynumeric() {//Function for only numbers and /


            if ((event.keyCode < 48 || event.keyCode > 57) || (event.keycode == 191)) {
                event.returnValue = false;
            }
        }

        function validrecord() {

            var txtPeriodDateFrom = document.getElementById("<%=txtPeriodDateFrom.ClientID%>");
            var txtperiodtodate = document.getElementById("<%=txtperiodtodate.ClientID%>");

            if (txtPeriodDateFrom.value == '') {
                alert('Enter Period From');
                return false;
            }
            if (txtperiodtodate.value == '') {
                alert('Enter Period To');
                return false;
            }
            var StartDate = txtPeriodDateFrom.value;
            var EndDate = txtperiodtodate.value;
            var day = ("0" + StartDate.getDate()).slice(-2);
            var month = ("0" + (StartDate.getMonth() + 1)).slice(-2);
            var today = StartDate.getFullYear() + "-" + (month) + "-" + (day);
            alert(StartDate);

        }
        //Added by Priya on 9-9-2015
        function ValidateDate(txt, keyCode) {
            if (keyCode == 13) {
                return false;
            }
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

                //  var date = new Date();
                // var firstDay = new Date(date.getFullYear(), date.getMonth(), 1);
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
        //date end

        function CheckValidDate() {

            var txtPeriodDateFrom = document.getElementById("<%=txtPeriodDateFrom.ClientID%>");
            var txtperiodtodate = document.getElementById("<%=txtperiodtodate.ClientID%>");

            if (txtPeriodDateFrom.value == '') {

                alert('Enter Period From');
                return false;
            }
            if (txtperiodtodate.value == '') {

                alert('Enter Period To');
                return false;
            }

            var sdate = txtPeriodDateFrom.value.split('/');
            var edate = txtperiodtodate.value.split('/');

            stdate = new Date(sdate[2], sdate[1] * 1 - 1, sdate[0]);
            endate = new Date(edate[2], edate[1] * 1 - 1, edate[0]);


            if (stdate > endate) {

                alert('Period To date should be greater than Period From date.');
                return false;
            }


            var StartDate = txtPeriodDateFrom.value;
            var EndDate = txtperiodtodate.value;
            var day = ("0" + StartDate.getDate()).slice(-2);
            var month = ("0" + (StartDate.getMonth() + 1)).slice(-2);
            var today = StartDate.getFullYear() + "-" + (month) + "-" + (day);
            alert(StartDate);
        }

    
    </script>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 17%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="Gold Loan Register"></asp:Label>
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
    <table>
        <tr>
            <td style="text-align: left; padding-left: 17px;">
                <fieldset>
                    <legend style="font-size: medium">Select Fields To Display</legend>
                    <asp:GridView ID="grdgold" runat="server" AutoGenerateColumns="false" HeaderStyle-CssClass="glrecptgVHeader">
                        <AlternatingRowStyle BackColor="White" />
                        <HeaderStyle CssClass="gVHeader" />
                        <Columns>
                            <asp:TemplateField>
                                <HeaderTemplate>
                                    <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="CheckBox1" runat="server" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Field's Name">
                                <ItemTemplate>
                                    <asp:Label ID="gvtxtfield" runat="server" CssClass="textbox" Style="text-align: left;"
                                        Text='<%#Eval("FieldName") %>' Width="200px"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </fieldset>
            </td>
            <td valign="top">
                <table width="100%">
                    <tr>
                        <td class="label" style="text-align: left; padding-left: 17px; width: 74px">
                            <asp:Label ID="lblPeriodFrom" runat="server" Text="Period From:"></asp:Label>
                        </td>
                        <td class="txt_style" style="text-align: left; width: 125px">
                            <asp:TextBox ID="txtPeriodDateFrom" class="textbox" runat="server" Width="60%" placeholder="dd/mm/yyyy"
                                onkeypress="return datevalid(event);" onchange="ValidateDate(this, event.keyCode)"
                                MaxLength="10"></asp:TextBox>
                            <asp:CalendarExtender ID="txtPeriodDateFrom_CalendarExtender" runat="server" Format="dd/MM/yyyy"
                                PopupButtonID="btnImgCalender" Enabled="True" TargetControlID="txtPeriodDateFrom"
                                CssClass="Calenderstyle">
                            </asp:CalendarExtender>
                            <asp:ImageButton ID="btnImgCalender" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                                Width="15" Height="15" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="text-align: left; padding-left: 17px; width: 74px">
                            <asp:Label ID="lblperiodto" runat="server" Text="Period To:"></asp:Label>
                        </td>
                        <td class="txt_style" style="text-align: left; width: 125px">
                            <asp:TextBox ID="txtperiodtodate" class="textbox" runat="server" placeholder="dd/mm/yyyy"
                                Width="60%" onkeypress="return datevalid(event);" onchange="return valid(this);"
                                MaxLength="10"></asp:TextBox>
                            <asp:CalendarExtender ID="txtperiodtodate_CalendarExtender1" runat="server" Format="dd/MM/yyyy"
                                PopupButtonID="btnImgCalender1" Enabled="True" TargetControlID="txtperiodtodate"
                                CssClass="Calenderstyle">
                            </asp:CalendarExtender>
                            <asp:ImageButton ID="btnImgCalender1" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                                Width="15" Height="15" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: left; padding-left: 16px;" colspan="2">
                            <fieldset>
                                <legend style="font-size: medium">Sort By</legend>
                                <table>
                                    <tr>
                                        <td class="label" style="text-align: left;">
                                            <asp:RadioButton ID="rdbGLno" runat="server" Text="Gold Loan No" GroupName="RDB" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label" style="text-align: left;">
                                            <asp:RadioButton ID="rdbName" runat="server" Text="Name" GroupName="RDB" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label" style="text-align: left;">
                                            <asp:RadioButton ID="rdbloandate" runat="server" Text="Loan Date" GroupName="RDB" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="label" style="text-align: left;">
                                            <asp:RadioButton ID="rdbplcaseno" runat="server" Text="PL Case No" GroupName="RDB" />
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <br />
    <div style="background-color: #ffffff; width: auto; visibility: hidden;">
        <asp:GridView ID="gvExcel" runat="server" Style="margin: 10px 10px 10px 10px;" EmptyDataText="No Record Found."
            AutoGenerateColumns="true" OnRowDataBound="gvExcel_RowDataBound" Width="98%">
            <HeaderStyle CssClass="gVHeader" />
            <RowStyle CssClass="gVItem" />
        </asp:GridView>
    </div>
    <asp:HiddenField ID="HiddenField1" runat="server" />
</asp:Content>
