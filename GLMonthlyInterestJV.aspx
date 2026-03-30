<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLMonthlyInterestJV.aspx.cs" Inherits="GLMonthlyInterestJV" EnableEventValidation="false"
    MaintainScrollPositionOnPostback="true" %>

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
        //Function Added By Priya for Date exist , validation start
        function DateFormat(txt, keyCode) {
            if (keyCode == 16)
                isShift = true;
            //Validate that its Numeric
            if (((keyCode >= 48 && keyCode <= 57) || keyCode == 8 ||
         keyCode <= 37 || keyCode <= 39 || (keyCode >= 96 && keyCode <= 105)) && isShift == false) {
                if ((txt.value.length == 2 || txt.value.length == 5) && keyCode != 8) {
                    txt.value += seperator;
                }
                return true;
            }
            else {
                return false;
            }
        }

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

                    if (selDate == LastDayMnth) {
                    }
                    else {
                        debugger;
                        alert('Selected Date Should Be Last Day Of Month');
                        //   document.getElementById("<%=GVIntJV.ClientID%>").val = '';
                        txt.value = '';

                        return;
                    }
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
                <asp:Label ID="lblHeader" runat="server" Text="GL - Monthly Interest JV">
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
                    onchange="ValidateDate(this, event.keyCode)"></asp:TextBox>
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
                <asp:Button ID="btn_Search" runat="server" Text="Search" CssClass="css_btn_class" 
                    Style="margin-left: 0px; width: 90px;display:none;" OnClick="btn_Search_Click" OnClientClick=" return validdate();" />
                    <button type="button" id="jvsearch" onclick="GetInterestJv()" class="css_btn_class" style="margin-left: 0px; width: 110px;">Search</button>
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
        <tr>
            <td colspan="4">
                <asp:GridView  ID="GVIntJV" runat="server" Width="98%" ShowHeader="true"
                    CssClass="textbox_readonly" EmptyDataText="NO Record Found" ForeColor="#ffffff"
                    BackColor="#1f497d" AutoGenerateColumns="false">
                    <Columns>
                        <asp:TemplateField>
                            <HeaderTemplate>
                                <asp:CheckBox ID="checkAll" runat="server" onclick="checkAll(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckBox1" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Gold LoanNo" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblgoldLoanNo" align="left" runat="server" Text='<%# Eval("GoldLoanNo") %>'></asp:Label>
                                <asp:HiddenField ID="HiddenField1" Value='<%# Eval("SDID") %>' runat="server"></asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer Name" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <asp:Label ID="lblAccountName" align="left" runat="server" Text='<%# Eval("Cust Name") %>'></asp:Label>
                                <asp:HiddenField ID="HiddenField2" Value='<%# Eval("AccountID") %>' runat="server">
                                </asp:HiddenField>
                                <asp:HiddenField ID="HiddenField3" Value='<%# Eval("LoanCalcAmount") %>' runat="server">
                                </asp:HiddenField>
                                <asp:HiddenField ID="hdnBalanceAmount" Value='<%#Eval("BalanceLoanAmount") %>' runat="server">
                                </asp:HiddenField>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Loan Amount" DataField="LoanAmount"></asp:BoundField>
                        <asp:BoundField HeaderText="Loan Date" DataField="LoanDate"></asp:BoundField>
                        <asp:BoundField HeaderText="Receive Date" DataField="ReceiveDate"></asp:BoundField>
                        <asp:BoundField HeaderText="Interest Amount" DataField="Interest Amount"></asp:BoundField>

                    </Columns>
                    <RowStyle CssClass="gVItem" Font-Size="15px" ForeColor="#1f497d" BackColor="#faf4b3" />
                    <HeaderStyle HorizontalAlign="Center" ForeColor="#ffffff" />
                </asp:GridView>
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                <asp:Button ID="ProcessBtn" runat="server" Text="Pass Interest JV" CssClass="css_btn_class"
                    Style="margin-left: 0px; width: 150px;display:none;" OnClick="ProcessBtn_Click" Visible="false" />
                  <button type="button" id="jvsent" onclick="SaveInterestJv()" class="css_btn_class" style="margin-left: 0px; width: 150px;display:none;">Pass Interest JV</button>
            </td>
        </tr>
        <asp:HiddenField ID="HdnUserID" runat="server" />
         <asp:HiddenField ID="hdnfyid" runat="server" />
          <asp:HiddenField ID="hdnbranchid" runat="server" />
    </table>
    <script>
        function GetInterestJv() {

         var SelDate = document.getElementById("<%=txtIntDate.ClientID %>");
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

            if (!pattern.test(SelDate.value)) {
                SelDate.value = '';
                alert('Please Enter Date.');
                return false;
            }
            $("table#MainContent_GVIntJV tbody").empty();
            $.ajax({
                type: "POST",
                url: "GLMonthlyInterestJV.aspx/CalculateJvInterest",
                data: '{jvdate:"' + $("#MainContent_txtIntDate").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend : function() {
                       //$("#jvsearch").css("display", "none");
                       $("#jvsearch").html("Please wait...");
                        $('#jvsearch').attr('disabled', 'disabled'); 
                          $("#jvsent").css("display", "none");                     
                }, 
                success: function (data) {
                    console.log(data.d);
                    var name = 102;
                    var jvinterest = data.d;
                    var html = "";
                    
                    html = html + "<tr align='center' style='color:White;'><th scope='col'>" +
                                "<input id='MainContent_GVItJV_checkAll' type='checkbox' name='ctl00$MainContent$GVIntJV$ctl01$checkAll' onclick='checkAll(this);'></th><th scope='col'>Gold LoanNo</th><th scope='col'>Customer Name</th>" +
                                "<th scope='col'>Loan Amount</th><th scope='col'>Loan Date</th><th scope='col'>Receive Date</th><th scope='col'>Interest Amount</th></tr>";

                    for (index = 0; index < jvinterest.length; index++) {
                        console.log(index);
                        html = html + "<tr class='gVItem' style='color:#1F497D;background-color:#FAF4B3;font-size:15px;'><td><input id='MainContent_GVIntJV_CheckBox1_" + index + "' type='checkbox' name='ctl00$MainContent$GVIntJV$ct" + (name + index) + "$CheckBox1'></td>" +
                                        "<td><span id='MainContent_GVIntJV_lblgoldLoanNo_" + index + "' align='left'>" + jvinterest[index].GoldLoanNo + "</span>" +
                                        "<input type='hidden' name='ctl00$MainContent$GVIntJV$ctl02$HiddenField1' id='MainContent_GVIntJV_HiddenField1_" + index + "' value='" + jvinterest[index].SDID + "'></td>" +
                                         "<td><span id='MainContent_GVIntJV_lblAccountName_" + index + "' align='left'>" + jvinterest[index].CustName + "</span>" +
                                         "<input type='hidden' name='ctl00$MainContent$GVIntJV$ctl02$HiddenField2' id='MainContent_GVIntJV_HiddenField2_" + index + "' value='" + jvinterest[index].AccountID + "'>" +
                                         "<input type='hidden' name='ctl00$MainContent$GVIntJV$ctl02$HiddenField3' id='MainContent_GVIntJV_HiddenField3_" + index + "' value='" + jvinterest[index].LoanCalcAmount + "'>" +
                                         "<input type='hidden' name='ctl00$MainContent$GVIntJV$ctl02$hdnBalanceAmount' id='MainContent_GVIntJV_hdnBalanceAmount_" + index + "' value='" + jvinterest[index].LoanCalcAmount + "'></td>" +
                                         "<td>" + jvinterest[index].LoanAmount + "</td><td>" + jvinterest[index].LoanDate + "</td><td>" + jvinterest[index].ReceiveDate + "</td>" +
                                         "<td>" + jvinterest[index].InterestAmount + "</td></tr>";


                    }
                    console.log(html);
                    $("table#MainContent_GVIntJV tbody").append(html);

                     $("#jvsearch").html("Search");
                      $('#jvsearch').removeAttr('disabled');
                      if(jvinterest.length>0)
                      {
                        $("#jvsent").html("Pass Interest JV");
                         $('#jvsent').css("display","block");
                         $('#jvsent').removeAttr('disabled');
                        
                     }
                },
               error: function (data) {
                         $("#jvsearch").html("Search");
                        $('#jvsearch').removeAttr('disabled');
                        var r = jQuery.parseJSON(data.responseText);
                        var errorMessage = r.Message;
                        var exceptionType = r.ExceptionType;
                        var stackTrace = r.StackTrace; 
                        console.log(errorMessage);
                        alert(errorMessage);
                },
                
            });
       }

        function arrayJV() {

            var obj = [];

            var MyRows = $('table#MainContent_GVIntJV').find('tbody').find('tr');
            for (var i = 1; i < MyRows.length; i++) {
                var check = $(MyRows[i]).find("td:eq(0) input[type='checkbox']").is(":checked");
                var GoldLoanNo = $("#MainContent_GVIntJV_lblgoldLoanNo_" + (i - 1)).text();
                var SDID = $("#MainContent_GVIntJV_HiddenField1_" + (i - 1)).val();
                var CustName = $("#MainContent_GVIntJV_lblAccountName_" + (i - 1)).text();
                var AccountID = $("#MainContent_GVIntJV_HiddenField2_" + (i - 1)).val();
                var LoanCalcAmount = $("#MainContent_GVIntJV_HiddenField3_" + (i - 1)).val();
                var BalanceLoanAmount = $("#MainContent_GVIntJV_hdnBalanceAmount_" + (i - 1)).val();
                var LoanAmount = $(MyRows[i]).find("td:eq(3)").text();
                var LoanDate = $(MyRows[i]).find("td:eq(4)").text();
                var ReceiveDate = $(MyRows[i]).find("td:eq(5)").text();
                var InterestAmount = $(MyRows[i]).find("td:eq(6)").text();
                var JvSentDate = $("#MainContent_txtIntDate").val();
                var UserId = $("#MainContent_HdnUserID").val();
                console.log(check);

                if(check)
                {
                    obj.push({ JvSentDate:JvSentDate,UserId:UserId, GoldLoanNo: GoldLoanNo, SDID: SDID, CustName: CustName, AccountID: AccountID, LoanCalcAmount: LoanCalcAmount, BalanceLoanAmount: BalanceLoanAmount, LoanAmount: LoanAmount, LoanDate: LoanDate, ReceiveDate: ReceiveDate, InterestAmount: InterestAmount });
                }
            }
            console.log(obj);
            return obj;
      }

        function SaveInterestJv() {

            counter=0;
          var MyRows = $('table#MainContent_GVIntJV').find('tbody').find('tr');
            for (var i = 1; i < MyRows.length; i++) {
                var check = $(MyRows[i]).find("td:eq(0) input[type='checkbox']").is(":checked");
                if(check)
                {
                counter=counter+1;
                }

             }

             if(counter==0)
             {
             alert('Select Record To Pass JV');
             return false;
             }

            arrjv = arrayJV();
          
            things = JSON.stringify({ 'monthlyjv': arrjv });        

            $.ajax({
                type: "POST",
                url: "GLMonthlyInterestJV.aspx/SaveInterestJV",
                data: things,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                beforeSend : function() {
                      $("#jvsent").html("Saving...");
                        $('#jvsent').attr('disabled','disabled');
                     $("#jvsearch").attr('disabled','disabled');
                }, 
                success: function (data) {
                    console.log(data.d);
                    if(parseInt(data.d)>0)
                    {
                       GetInterestJv();
                    }

                },
                 failure: function (response) {
                   $("#jvsent").html("Pass Interest JV");
                   $('#jvsent').removeAttr('disabled');
                  console.log(response.d.responseText);
                    console.log(response.d.responseText);
                },
            });
       }
         
    </script>

</asp:Content>
