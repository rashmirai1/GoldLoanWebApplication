<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeFile="GLKYCDetailsEditForm.aspx.cs" Inherits="GLKYCDetailsEditForm" Theme="GridViewTheme"
    EnableViewStateMac="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <script language="javascript" type="text/javascript">
        //DatePicker
        $(function () {
            $('#<%=txtLoanDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
            $('#<%=txtBirthDate.ClientID %>').dateEntry({ dateFormat: 'dmy/' });
        });

        //Confirm Alert
        function ConfirmFunction(str) {
            var x;
            var r = confirm(str);
            if (r == true) {
                x = true;
                return x;
            }
            else {
                x = false;
                return x;
            }
        }

        //Function for alphabets without space
        function lettersOnly(evt) {
            evt = (evt) ? evt : event;
            var charCode = (evt.charCode) ? evt.charCode : ((evt.keyCode) ? evt.keyCode : ((evt.which) ? evt.which : 0));
            if (charCode > 31 && (charCode < 65 || charCode > 90) && (charCode < 97 || charCode > 122)) {
                return false;
            }
            else
                return true;
        };

        function isAlphaNumeric(e) { // Alphanumeric only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123));
        }

        function isAlphaNumChars(e) { // Alphanumeric, space,(,),- only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41) || k == 0);
        }

        function isAlphaNumSplChars(e) { // Alphanumeric, space,(,),-,' only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41 || k == 39) || k == 0);
        }

        function isAlphaNumChar2(e) { // Alphanumeric, space,-,/ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 47) || k == 0);
        }

        function isNumericHyphen(e) {  // Alphanumeric, space, - only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k > 47 && k < 58) || k == 45);
        }

        function OnlyNumericEntry() {
            if ((event.keyCode < 48 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }

        function MakeUpper(txtId) {
            document.getElementById(txtId).value = document.getElementById(txtId).value.toUpperCase();
        }

        //Calculate Age
        function calculateAge(DOB) {
            if (DOB.value != '') {
                now = new Date()

                var txtValue = DOB;

                if (txtValue != null)

                    dob = txtValue.split('/');
                if (dob.length === 3) {
                    born = new Date(dob[2], dob[1] * 1 - 1, dob[0]);
                    if (now.getMonth() == born.getMonth() && now.getDate() == born.getDate()) {
                        age = now.getFullYear() - born.getFullYear();
                    }
                    else {
                        age = Math.floor((now.getTime() - born.getTime()) / (365.25 * 24 * 60 * 60 * 1000));
                    }
                    if (isNaN(age) || age == 0) {
                        //alert('Input date is incorrect!');
                    }
                    else if (age == 0) {
                        alert('Input date is incorrect!');
                    }
                    else {
                        document.getElementById('<%=txtAge.ClientID %>').value = age;
                        document.getElementById('<%=ddlMaritalStatus.ClientID %>').focus();
                    }
                }
            }
        }

        //validate Applicant's Age
        function validateAge() {
            alert('hi');
            if (document.getElementById('<%=txtAge.ClientID %>').value != null && document.getElementById('<%=txtAge.ClientID %>').value != "") {
                var age = document.getElementById('<%=txtAge.ClientID %>').value;
                if (age < 18) {
                    alert("Applicant's Age must be 18 yrs & above.");
                }
            }
        }

        //Function to Check Multiline Textbox Maxlength (Here Address Textbox)
        function Check(textBox, maxLength) {
            if (textBox.value.length > maxLength) {
                //alert("You cannot enter more than " + maxLength + " characters.");
                textBox.value = textBox.value.substr(0, maxLength);
            }
        }

        function getfocus() {
            document.getElementById('<%= btnSearch.ClientID %>').click()
        }
    </script>
    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true">
    </asp:ScriptManager>
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 95%;">
        <tr>
            <td style="width: 10%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 30%;">
            </td>
            <td style="width: 30%;">
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
                        <td align="right" colspan="3" class="header">
                            <asp:Label ID="lblHeader" runat="server" Text="GL KYC Details"></asp:Label>
                        </td>
                        <td align="right" colspan="1">
                            <asp:Label ID="Label27" runat="server" Text="[Edit Section]" Font-Names="Verdana"
                                Font-Size="11px" ForeColor="#070c80"></asp:Label>
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
        <!--Form Design -->
        <tr>
            <!-- Gold Loan No. -->
            <td align="right" style="height: 37px;">
                <asp:Label ID="Label26" runat="server" Text="Gold Loan No." CssClass="label" Font-Bold="true"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtGoldLoanNo" CssClass="textbox" runat="server" Width="90%" AutoPostBack="true"
                    MaxLength="20" OnTextChanged="txtGoldLoanNo_TextChanged" Style="text-transform: uppercase;"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReqGoldLoanNo" runat="server" ErrorMessage="*" ControlToValidate="txtGoldLoanNo"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
            <!-- Applicant Photo -->
            <td class="label" valign="top">
                <asp:Label ID="Label2" runat="server" Text="Applicant Photo"></asp:Label>
            </td>
            <td class="txt_style" rowspan="2" valign="top">
                <table>
                    <tr>
                        <td>
                            <asp:UpdatePanel ID="UpdatePanel8" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Image ID="imgAppPhoto" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                        BorderWidth="1px" ImageAlign="AbsMiddle" />
                                    <asp:RequiredFieldValidator ID="reqAppPhoto" runat="server" ErrorMessage="*" ControlToValidate="txtAppPhotoPath"
                                        ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                                        SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                        <td valign="top" align="left">
                            <asp:FileUpload ID="fUploadPhoto" runat="server" Width="85px" />
                            <br />
                            <asp:Button ID="btnUploadPhoto" runat="server" Text="Upload" OnClick="btnUploadPhoto_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <!-- Loan Date -->
            <td class="label">
                Applied Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanDate" CssClass="textbox" runat="server" Width="75%" MaxLength="10"
                    placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqLoanDate" runat="server" ErrorMessage="*" ControlToValidate="txtLoanDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <!-- Loan Date (Date Format validation) -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:CompareValidator ID="CVLoanDate" runat="server" ControlToValidate="txtLoanDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]" ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="save"
                    Font-Bold="True"></asp:CompareValidator>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="3">
                <!-- Custom Validator for Applied Date -->
                <asp:CustomValidator runat="server" ID="CustomValidator4" ControlToValidate="txtLoanDate"
                    ForeColor="Red" Display="Dynamic" Font-Bold="True" SetFocusOnError="True" OnServerValidate="txtLoanDate_ServerValidate"
                    ValidationGroup="save" ErrorMessage="Applied Date must be within Financial Year." />
            </td>
        </tr>
        <tr>
            <!-- Operator Name -->
            <td class="label">
                <asp:Label ID="Label5" runat="server" Text="Operator Name"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtOperatorName" CssClass="textbox_readonly" ReadOnly="true" runat="server"
                    Width="90%"></asp:TextBox>
            </td>
            <!-- Applicant Signature -->
            <td class="label" valign="top">
                <asp:Label ID="Label3" runat="server" Text="Applicant Signature"></asp:Label>
            </td>
            <td class="txt_style" valign="top">
                <table>
                    <tr>
                        <td>
                            <asp:Image ID="imgAppSign" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" />
                            <asp:RequiredFieldValidator ID="ReqSignPath" runat="server" ErrorMessage="*" ControlToValidate="txtAppSignPath"
                                ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                                SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        </td>
                        <td valign="top" align="left">
                            <asp:FileUpload ID="fUploadSign" runat="server" Width="85px" />
                            <br />
                            <asp:Button ID="btnUploadSign" runat="server" Text="Upload" OnClick="btnUploadSign_Click" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
                <tr>
              <!-- Source of Application -->
            <td class="label">
                Source of Application
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlSourceofApplication" runat="server" CssClass="textbox" Height="27px"
                    Width="95%" OnSelectedIndexChanged="ddlSourceofApplication_SelectedIndexChanged" AutoPostBack="true">
                    <asp:ListItem Value="0">--Select--</asp:ListItem>
                    <asp:ListItem>Banner</asp:ListItem>
                    <asp:ListItem>Dealer</asp:ListItem>
                    <asp:ListItem>Google  Ads</asp:ListItem>
                    <asp:ListItem>Jewellers</asp:ListItem>
                    <asp:ListItem>Justdial</asp:ListItem>
                    <asp:ListItem>Newspaper</asp:ListItem>
                    <asp:ListItem>Others</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlSourceofApplication" ValidationGroup="save" ForeColor="Red"
                    Display="Dynamic" Font-Bold="True" Font-Size="Medium" SetFocusOnError="True"
                    InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
           <!-- Please Specify (source of application) -->
            <td class="label">
                Please Specify
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSpecifySource" CssClass="textbox" runat="server" Width="90%"
                   placeholder="Source Name/If Other" MaxLength="100"></asp:TextBox>
            </td>
        </tr>
        <tr>
           <!-- Dealer Name (Dealer code) -->
            <td class="label">
                Dealer Name 
            </td>
            <td class="txt_style"  colspan="3">
               <asp:DropDownList ID="ddlDealerName" runat="server" CssClass="textbox" Width="98.5%"
                    Height="27px" >
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="1">
            </td>
            <td colspan="3">
                <!-- Custom Validator for Dealer Name -->
                <%--<asp:CustomValidator runat="server" ID="CustomValidator5" ControlToValidate="ddlDealerName"
                    ClientIDMode="Static" ForeColor="Red" Display="Dynamic" Font-Size="Small" SetFocusOnError="True"
                    OnServerValidate="ddlDealerName_ServerValidate" ValidationGroup="save" ErrorMessage="Max Loan Amount must be greater than zero." />--%>
            </td>
        </tr>
            <!-- Existing GL No. -->
            <td class="label">
                <asp:Label ID="Label1" runat="server" Text="Existing GL No." ForeColor="Red"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtExistingGLNo" CssClass="textbox" runat="server" Width="90%" AutoPostBack="true"
                    placeholder="To auto-generate details" MaxLength="20" OnTextChanged="txtExistingGLNo_TextChanged"
                    onblur="JavaScript:MakeUpper(this.id);"></asp:TextBox>
            </td>
            <td align="left">
                <%--<asp:Label ID="Label21" runat="server" Text="(To auto-generate details if exists)"
                    Width="170px" Font-Size="11px" Font-Names="Calibri"></asp:Label>--%>
                &nbsp;
            </td>
        </tr>
        <tr>
            <!-- Applicant Name -->
            <td class="label">
                <asp:Label ID="Label7" runat="server" Text="Applicant Name"></asp:Label>
            </td>
            <!--App First Name -->
            <td class="txt_style">
                <asp:TextBox ID="txtAppFName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="First Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAppFName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Font-Bold="True" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtAppFName"></asp:RequiredFieldValidator>
            </td>
            <td>
                <!--App Middle Name -->
                <asp:TextBox ID="txtAppMName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Middle Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvAppMName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Font-Bold="True" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtAppMName"></asp:RequiredFieldValidator>
            </td>
            <td>
                <!--App Last Name -->
                <asp:TextBox ID="txtAppLName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Last Name"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvappLName" runat="server" ForeColor="Red" ErrorMessage="*"
                    Font-Size="Medium" Font-Bold="True" Display="Dynamic" ValidationGroup="save"
                    ControlToValidate="txtAppLName"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="revAppFName" runat="server" ControlToValidate="txtAppFName"
                    Font-Size="Medium" ForeColor="Red" Display="Dynamic" Font-Bold="True" ErrorMessage="*"
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z]+(_[a-zA-Z]+)*)(\s([a-zA-Z]+(_[a-zA-Z]+)*))*$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="revAppMName" runat="server" ControlToValidate="txtAppMName"
                    Font-Size="Medium" ForeColor="Red" Display="Dynamic" Font-Bold="True" ErrorMessage="*"
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z]+(_[a-zA-Z]+)*)(\s([a-zA-Z]+(_[a-zA-Z]+)*))*$"></asp:RegularExpressionValidator>
                <asp:RegularExpressionValidator ID="revAppLName" runat="server" ControlToValidate="txtAppLName"
                    Font-Size="Medium" ForeColor="Red" Display="Dynamic" Font-Bold="True" ErrorMessage="*"
                    SetFocusOnError="True" ValidationGroup="save" ValidationExpression="^([a-zA-Z]+(_[a-zA-Z]+)*)(\s([a-zA-Z]+(_[a-zA-Z]+)*))*$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <!-- Loan Type -->
            <td class="label">
                Gold Loan Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlLoanType" runat="server" CssClass="textbox" Height="27px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Loan Type--</asp:ListItem>
                    <asp:ListItem Value="New">New</asp:ListItem>
                    <asp:ListItem Value="Refinance">Refinance</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ReqLoanType" runat="server" ErrorMessage="*" ControlToValidate="ddlLoanType"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
            <!-- Existing PL Customer -->
            <td class="label">
                Existing PL-Case No. (if any)
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPL" runat="server" Width="7%" CssClass="textbox_readonly" ReadOnly="true"
                    Text="PL"></asp:TextBox>
                <asp:TextBox ID="txtExistingPLCaseNo" runat="server" CssClass="textbox" Width="77%"
                    MaxLength="10" onkeypress="return OnlyNumericEntry();" Style="text-transform: uppercase;"
                    placeholder="IF EXISTING PL CUSTOMER"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
                <!-- Custom Validator for PL-CaseNo -->
                <asp:CustomValidator runat="server" ID="CustomValidator3" ControlToValidate="txtExistingPLCaseNo"
                    ForeColor="Red" Display="Dynamic" Font-Bold="True" SetFocusOnError="True" OnServerValidate="txtExistingPLCaseNo_ServerValidate"
                    ValidationGroup="save" ErrorMessage="Invalid PL Case No." />
            </td>
        </tr>
        <tr>
            <!-- Pan No. -->
            <td class="label">
                Pan No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPANNo" runat="server" CssClass="textbox" Width="90%" MaxLength="10"
                    onkeypress="return isAlphaNumeric(event);" Style="text-transform: uppercase;"></asp:TextBox>
            </td>
            <!-- Gender -->
            <td class="label">
                Gender
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="textbox" Height="25px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Gender--</asp:ListItem>
                    <asp:ListItem Value="Male">Male</asp:ListItem>
                    <asp:ListItem Value="Female">Female</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ReqGender" runat="server" ErrorMessage="*" ControlToValidate="ddlGender"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
                <!-- Regex for PANNo -->
                <asp:RegularExpressionValidator ID="RegPANNo" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtPANNo" ValidationExpression="[A-Z/a-z]{5}\d{4}[A-Z/a-z]{1}"
                    ErrorMessage="Invalid PAN No." Display="Dynamic" ValidationGroup="save" ForeColor="Red"> </asp:RegularExpressionValidator>
            </td>
            <td>
            </td>
            <td>
            </td>
        </tr>
        <tr>
            <!-- Marital Status -->
            <td class="label">
                Marital Status
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="textbox" Height="27px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Marital Status--</asp:ListItem>
                    <asp:ListItem Value="Single">Single</asp:ListItem>
                    <asp:ListItem Value="Married">Married</asp:ListItem>
                    <asp:ListItem Value="Divorced">Divorced</asp:ListItem>
                    <asp:ListItem Value="Widowed">Widowed</asp:ListItem>
                    <asp:ListItem Value="Engaged">Engaged</asp:ListItem>
                    <asp:ListItem Value="Separated">Separated</asp:ListItem>
                    <asp:ListItem Value="Cohabitating">Cohabitating</asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="ReqMaritalStatus" runat="server" ErrorMessage="*"
                    ControlToValidate="ddlMaritalStatus" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
            </td>
            <!-- BirthDate -->
            <td class="label">
                Birth Date
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtBirthDate" CssClass="textbox" runat="server" onblur="calculateAge(this.value);"
                    MaxLength="10" Width="36%" placeholder="dd/mm/yyyy" OnTextChanged="txtBirthDate_TextChanged"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReqBirthDate" runat="server" ErrorMessage="*" ControlToValidate="txtBirthDate"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
                <asp:CompareValidator ID="CVBirthDate" runat="server" ControlToValidate="txtBirthDate"
                    Display="Dynamic" ErrorMessage="*" ForeColor="Red" Operator="DataTypeCheck" SetFocusOnError="True"
                    Type="Date" ValidationGroup="save" Font-Bold="True">*</asp:CompareValidator>
                <!-- Age -->
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="txtAge" runat="server" CssClass="textbox_readonly" Width="14%" ReadOnly="true"
                    onchange="javascript:validateAge();"></asp:TextBox>
                <asp:Label ID="Label16" runat="server" Text="(Yrs)" Font-Names="Calibri" Font-Size="12px"></asp:Label>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td colspan="2">
            </td>
            <td>
                <!-- Loan Date (Date Format validation) -->
                <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="txtLoanDate"
                    Display="Dynamic" ErrorMessage="Date Format [eg. dd/mm/yyyy]." ForeColor="Red"
                    Operator="DataTypeCheck" SetFocusOnError="True" Type="Date" ValidationGroup="save"
                    Font-Bold="True"></asp:CompareValidator>
                <!-- Custom Validator for Age -->
                <asp:CustomValidator runat="server" ID="CustomValidator2" ControlToValidate="txtAge"
                    ForeColor="Red" Display="Dynamic" Font-Bold="True" SetFocusOnError="True" OnServerValidate="txtAge_ServerValidate"
                    ValidationGroup="save" ErrorMessage="Applicant must be 18 yrs & above." />
            </td>
        </tr>
        <tr>
            <!-- Building/House Name -->
            <td class="label">
                <asp:Label ID="Label8" runat="server" Text="Building/House Name" Width="125px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtBldgHouseName" CssClass="textbox" runat="server" Width="97%"
                    onkeypress="return isAlphaNumChars(event);" MaxLength="50"></asp:TextBox>
                <asp:RequiredFieldValidator ID="reqBldgHouseName" runat="server" ErrorMessage="*"
                    ControlToValidate="txtBldgHouseName" ValidationGroup="save" ForeColor="Red" Display="Dynamic"
                    Font-Bold="True" Font-Size="Medium" SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <%--<!-- Regex validation for Building/House Name -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                 <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtBldgHouseName" ValidationExpression="^([a-zA-Z0-9-()]+(_[a-zA-Z0-9-()]+)*)(\s([a-zA-Z0-9-()]+(_[a-zA-Z0-9-()]+)*))*$"
                    ErrorMessage="Invalid Character (Only alphanumeric, brackets, dash are allowed)."
                    Display="Dynamic" ValidationGroup="save" ForeColor="Red"> </asp:RegularExpressionValidator>
            </td>
        </tr>--%>
        <tr>
            <!-- Road -->
            <td class="label">
                Road
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtRoad" CssClass="textbox" runat="server" Width="97%" MaxLength="95"
                    onkeypress="return isAlphaNumSplChars(event);"></asp:TextBox>
            </td>
        </tr>
        <%--<!-- Regex validation for Road -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RegRoad" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtRoad" ValidationExpression="^([a-zA-Z0-9 -()']+(_[a-zA-Z0-9 -()']+)*)(\s([a-zA-Z0-9 -()']+(_[a-zA-Z0-9 -()']+)*))*$"
                    ErrorMessage="Invalid Character (Only alphanumeric, brackets, dash & single quote are allowed)."
                    Display="Dynamic" ValidationGroup="save" ForeColor="Red"> </asp:RegularExpressionValidator>
            </td>
        </tr>--%>
        <tr>
            <!-- Building/Plot No. -->
            <td class="label">
                <asp:Label ID="Label9" runat="server" Text="Building/Plot No." Width="125px"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPlotNo" CssClass="textbox" runat="server" Width="90%" MaxLength="30"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
            </td>
            <!-- Room/Block No. -->
            <td class="label">
                <asp:Label ID="Label10" runat="server" Text="Room/Block No." Width="125px"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRoomBlockNo" CssClass="textbox" runat="server" Width="91%" MaxLength="30"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
                <asp:RequiredFieldValidator ID="ReqRoomBlockNo" runat="server" ErrorMessage="*" ControlToValidate="txtRoomBlockNo"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
        </tr>
        <!-- Regex validation for Plot No -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RegPlotNo" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtPlotNo" ValidationExpression="^([a-zA-Z0-9 -/]+(_[a-zA-Z0-9 -/]+)*)(\s([a-zA-Z0-9 -/]+(_[a-zA-Z0-9 -/]+)*))*$"
                    ErrorMessage="Invalid Character (Only alphanumeric, dash and backslash are allowed)."
                    Display="Dynamic" ValidationGroup="save" ForeColor="Red"> </asp:RegularExpressionValidator>
            </td>
        </tr>
        <!-- Regex validation for RoomBlockNo -->
        <tr>
            <td>
            </td>
            <td colspan="3">
                <asp:RegularExpressionValidator ID="RegRoomBlockNo" runat="server" SetFocusOnError="true"
                    Font-Bold="true" ControlToValidate="txtRoomBlockNo" ValidationExpression="^([a-zA-Z0-9 -/]+(_[a-zA-Z0-9 -/]+)*)(\s([a-zA-Z0-9 -/]+(_[a-zA-Z0-9 -/]+)*))*$"
                    ErrorMessage="Invalid Character (Only alphanumeric, dash and backslash are allowed)."
                    Display="Dynamic" ValidationGroup="save" ForeColor="Red"> </asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <!-- State -->
            <td class="label">
                State
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="textbox" Height="27px" Width="95%"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ReqState" runat="server" ErrorMessage="*" ControlToValidate="ddlState"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <!-- City -->
            <td class="label">
                City
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCity" runat="server" CssClass="textbox" Height="27px" Width="96%"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ReqCity" runat="server" ErrorMessage="*" ControlToValidate="ddlCity"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Area -->
            <td class="label">
                Area
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlArea" runat="server" CssClass="textbox" Height="27px" Width="95%"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ReqArea" runat="server" ErrorMessage="*" ControlToValidate="ddlArea"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlCity" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
            <!-- Zone -->
            <td class="label">
                Zone
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlZone" runat="server" CssClass="textbox" Height="27px" Width="96%">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ReqZone" runat="server" ErrorMessage="*" ControlToValidate="ddlZone"
                            ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                            SetFocusOnError="True" InitialValue="0">*</asp:RequiredFieldValidator>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlArea" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlCity" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- Nearest Landmark -->
            <td class="label">
                <asp:Label ID="Label11" runat="server" Text="Nearest Landmark" Width="125px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNearestLandmark" CssClass="textbox" runat="server" Width="97%"
                    MaxLength="90"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Mobile No. -->
            <td class="label">
                Mobile No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMobileNo" CssClass="textbox" runat="server" Width="90%" MaxLength="10"
                    onkeypress="return OnlyNumericEntry();"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVMobileNo" runat="server" ErrorMessage="*" ControlToValidate="txtMobileNo"
                    ValidationGroup="save" ForeColor="Red" Display="Dynamic" Font-Bold="True" Font-Size="Medium"
                    SetFocusOnError="True">*</asp:RequiredFieldValidator>
            </td>
            <!-- Telephone No. -->
            <td class="label">
                <asp:Label ID="Label12" runat="server" Text="Telephone No."></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtTelephoneNo" onkeypress="return isNumericHyphen(event);" CssClass="textbox"
                    MaxLength="13" runat="server" Width="91%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
            </td>
            <td>
            </td>
            <td>
            </td>
            <td>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Label ID="Label25" Font-Size="12px" runat="server" Text="[e.g. 1234-12345678]"
                    Font-Names="Calibri" Font-Italic="True"></asp:Label>
            </td>
        </tr>
        <!-- Regex validation for Telephone No -->
        <tr>
            <td>
            </td>
            <td>
                <asp:RegularExpressionValidator ID="REVMobileNo" runat="server" ControlToValidate="txtMobileNo"
                    ValidationGroup="save" ErrorMessage="Invalid Mobile number!" ForeColor="Red"
                    Font-Bold="True" Display="Dynamic" SetFocusOnError="True" ValidationExpression="^([7-9]{1})([0-9]{9})$"></asp:RegularExpressionValidator>
            </td>
            <td>
            </td>
            <td>
                <asp:RegularExpressionValidator ID="RegContactNo" runat="server" ControlToValidate="txtTelephoneNo"
                    ErrorMessage="Invalid Tel No." ForeColor="Red" SetFocusOnError="True" Font-Bold="True"
                    ValidationExpression="(^[0-9]\d{1,4}-\d{6,8}$)" Display="Dynamic" ValidationGroup="save"> </asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <!-- Email Address -->
            <td class="label">
                Email ID
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtEmailId" CssClass="textbox" runat="server" Width="97%"></asp:TextBox>
                <asp:RegularExpressionValidator runat="server" ID="REVEmailID" SetFocusOnError="true"
                    Font-Bold="True" ValidationGroup="save" Text="Example: username@gmail.com" ControlToValidate="txtEmailId"
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" Display="Dynamic"
                    ForeColor="Red" />
            </td>
        </tr>
        <tr>
            <!-- Nominee Name -->
            <td class="label">
                <asp:Label ID="Label13" runat="server" Text="Nominee Name" Width="125px"></asp:Label>
            </td>
            <td class="txt_style">
                <!--Nom First Name -->
                <asp:TextBox ID="txtNomFName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="First Name"></asp:TextBox>
            </td>
            <td>
                <!--Nom Middle Name -->
                <asp:TextBox ID="txtNomMName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Middle Name"></asp:TextBox>
            </td>
            <td>
                <!--Nom Last Name -->
                <asp:TextBox ID="txtNomLName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Last Name"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Nominee Address -->
            <td class="label">
                <asp:Label ID="Label15" runat="server" Text="Nominee Address" Width="125px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNomineeAddress" CssClass="textbox" runat="server" Width="97%"
                    Height="35px" TextMode="MultiLine" onkeyup="javascript:Check(this, 200);" onchange="javascript:Check(this, 200);"
                    MaxLength="200"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Nominee Relationship -->
            <td class="label">
                <asp:Label ID="Label14" runat="server" Text="Nominee Relationship" Width="130px"></asp:Label>
            </td>
            <td class="txt_style" colspan="3">
                <asp:DropDownList ID="ddlNomRelation" runat="server" CssClass="textbox" Height="27px"
                    Width="31%">
                    <asp:ListItem Value="">--Select Relation--</asp:ListItem>
                    <asp:ListItem Value="Aunt">Aunt</asp:ListItem>
                    <asp:ListItem Value="Brother">Brother</asp:ListItem>
                    <asp:ListItem Value="Cousin">Cousin</asp:ListItem>
                    <asp:ListItem Value="Daughter">Daughter</asp:ListItem>
                    <asp:ListItem Value="Father">Father</asp:ListItem>
                    <asp:ListItem Value="Grandchildren">Grandchildren</asp:ListItem>
                    <asp:ListItem Value="Granddaughter">Granddaughter</asp:ListItem>
                    <asp:ListItem Value="Grandfather">Grandfather</asp:ListItem>
                    <asp:ListItem Value="Grandmother">Grandmother</asp:ListItem>
                    <asp:ListItem Value="Grandson">Grandson</asp:ListItem>
                    <asp:ListItem Value="Husband">Husband</asp:ListItem>
                    <asp:ListItem Value="Mother">Mother</asp:ListItem>
                    <asp:ListItem Value="Nephew">Nephew</asp:ListItem>
                    <asp:ListItem Value="Other">Other</asp:ListItem>
                    <asp:ListItem Value="Self">Self</asp:ListItem>
                    <asp:ListItem Value="Sister">Sister</asp:ListItem>
                    <asp:ListItem Value="Son">Son</asp:ListItem>
                    <asp:ListItem Value="Uncle">Uncle</asp:ListItem>
                    <asp:ListItem Value="Wife">Wife</asp:ListItem>
                </asp:DropDownList>
                <asp:Label ID="Label4" runat="server" Text="(Relationship with applicant)" CssClass="label"
                   Font-Size="12px" Font-Italic="true"></asp:Label>
                <!-- ID -->
                <asp:TextBox ID="txtID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Comp ID -->
                <asp:TextBox ID="txtCompID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Branch ID -->
                <asp:TextBox ID="txtBranchID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- FY ID -->
                <asp:TextBox ID="txtFYID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Operator ID -->
                <asp:TextBox ID="txtOperatorID" runat="server" Visible="false" Height="22px" Width="17px"></asp:TextBox>
                <!-- Photo Path -->
                <asp:TextBox ID="txtAppPhotoPath" runat="server" Visible="false" Height="22px" Width="16px"></asp:TextBox>
                <!-- App Sign Path -->
                <asp:TextBox ID="txtAppSignPath" runat="server" Visible="false" Height="22px" Width="16px"></asp:TextBox>
                <!-- DID -->
                <asp:TextBox ID="txtDID" runat="server" Visible="false" Height="22px" Width="16px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Applicant's Document Details Section -->
            <td colspan="4" align="left">
                <br />
                <br />
                <asp:Label ID="Label6" runat="server" Text="Applicant's Document Details :" CssClass="label"
                    Font-Size="16px" Font-Bold="true" Font-Underline="true"></asp:Label>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="divDocDetails" runat="server">
                    <!--GridView Document Details -->
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDocumentDetails" runat="server" DataKeyNames="DID" OnRowCommand="dgvDocument_RowCommand"
                                OnPageIndexChanging="dgvDocument_PageIndexChanging" ShowFooter="true" AutoGenerateColumns="False"
                                Width="100%">
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ID" Visible="False">
                                        <ItemTemplate>
                                            <asp:Label ID="lblDID" runat="server" Text='<%# Eval("DID") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="DocID" ItemStyle-HorizontalAlign="Left" Visible="false">
                                        <FooterTemplate>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblDocumentID" runat="server" Text='<%# Eval("DocumentID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Document Name" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <FooterTemplate>
                                            <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>
                                                    <asp:DropDownList ID="ddlDocName" runat="server" Height="22px" Width="130px" DataSourceID="SqlDataSource1"
                                                        DataTextField="DocumentName" DataValueField="DocumentID" OnSelectedIndexChanged="ddlDocName_SelectedIndexChanged"
                                                        AutoPostBack="true">
                                                    </asp:DropDownList>
                                                    <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                                                        SelectCommand="SELECT * FROM [tbl_GLDocumentMaster]"></asp:SqlDataSource>
                                                </ContentTemplate>
                                            </asp:UpdatePanel>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblDocName" runat="server" Text='<%# Eval("DocName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Specify Other" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left">
                                        <FooterTemplate>
                                            <asp:TextBox ID="txtSpecifyOther" Text="" Width="100px" runat="server" MaxLength="50"
                                                placeholder="Specify if Other" Enabled="false"></asp:TextBox>
                                            <!-- Custom Validator for Specify if Other -->
                                            <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="*" ForeColor="Red"
                                                ControlToValidate="txtSpecifyOther" SetFocusOnError="True" Font-Bold="True" ValidationGroup="Upload"
                                                Text="*" OnServerValidate="txtSpecifyOther_ServerValidate" ValidateEmptyText="true"
                                                Display="Dynamic"></asp:CustomValidator>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblSpecifyOther" runat="server" Text='<%# Eval("OtherDoc") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Received?" ItemStyle-HorizontalAlign="Center">
                                        <FooterTemplate>
                                            <asp:CheckBox ID="chkReceived" runat="server" Text="Received?" />
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblReceived" runat="server" Text='<%# Eval("DocRecd") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="ImagePath" ItemStyle-HorizontalAlign="Center" Visible="false">
                                        <FooterTemplate>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <asp:Label ID="lblImagePath" runat="server" Text='<%# Eval("ImagePath") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Photo Copy" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:Image ID="imgDocPhoto" runat="server" Width="40px" Height="50px" ImageUrl='<%# Eval("ImageUrl") %>' />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:FileUpload ID="FileUpload1" runat="server" Width="170px" />
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ForeColor="Red"
                                                Font-Bold="true" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                                ControlToValidate="FileUpload1"></asp:RequiredFieldValidator>
                                        </FooterTemplate>
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Remove" Visible="true" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                ImageAlign="AbsMiddle" OnClientClick="javascript:return ConfirmFunction('Do you really want to Remove Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <FooterTemplate>
                                            <asp:Button ID="btnUpload" runat="server" Text="Add Details" OnClick="btnUpload_OnClick"
                                                ValidationGroup="Upload" />
                                        </FooterTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDocumentDetails" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <br />
                <br />
                <!-- Custom Validator for Document Details -->
                <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="*" ForeColor="Red"
                    SetFocusOnError="True" Font-Bold="True" ValidationGroup="save" Text="Select Applicant's Document Details."
                    OnServerValidate="dgvDocumentDetails_ServerValidate" Display="Dynamic" ValidateEmptyText="True"></asp:CustomValidator>
            </td>
        </tr>
        <tr>
            <!--Save Button -->
            <td align="center" colspan="4">
                <br />
                <br />
                <asp:Button ID="btnSave" runat="server" Text="Update" CssClass="button" ValidationGroup="save"
                    OnClick="btnSave_Click" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btnReset" runat="server" Text="Cancel" CssClass="button" OnClick="btnReset_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td align="right" colspan="4">
                <asp:Label ID="Label28" runat="server" Text="[Search Section]" Font-Names="Verdana"
                    Font-Size="11px" ForeColor="#070c80"></asp:Label>
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
        <tr>
            <td colspan="4">
                <div id="div1" runat="server">
                    <!--Search -->
                    <table align="left" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td class="label">
                                Search By:
                            </td>
                            <td class="txt_style">
                                <asp:DropDownList ID="ddlSearchBy" Width="90%" runat="server" class="textbox_search"
                                    Height="26px">
                                </asp:DropDownList>
                            </td>
                            <td class="label">
                                Search Text:
                            </td>
                            <td class="txt_style">
                                <asp:TextBox ID="txtSearch" MaxLength="40" runat="server" class="textbox_search"
                                    onblur="getfocus()"></asp:TextBox>
                                &nbsp;&nbsp;
                                <asp:ImageButton ID="btnSearch" ImageUrl="~/images/1397069814_Search.png" Height="20px"
                                    Width="20px" runat="server" OnClick="btnSearch_Click" ImageAlign="AbsMiddle" />
                            </td>
                        </tr>
                    </table>
                    <br />
                    <br />
                    <br />
                    <br />
                    <!--GridView DGV Details (Edit/Delete section) -->
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <asp:GridView ID="dgvDetails" runat="server" DataKeyNames="GoldLoanNo" OnRowCommand="dgvDetails_RowCommand"
                                OnPageIndexChanging="dgvDetails_PageIndexChanging" Width="100%" 
                                >
                                <Columns>
                                    <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center"
                                        FooterStyle-HorizontalAlign="Center" HeaderText="Sr.No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSrNo" Text='<%#Container.DataItemIndex+1 %>' runat="server"></asp:Label>
                                        </ItemTemplate>
                                        <FooterStyle HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="KYCID" HeaderText="ID" Visible="False" />
                                    <asp:BoundField DataField="GoldLoanNo" HeaderText="Gold Loan No" />
                                    <asp:BoundField DataField="LoanDate" HeaderText="Loan Date" />
                                    <asp:BoundField DataField="LoanType" HeaderText="Loan Type" />
                                    <asp:BoundField DataField="ApplicantName" HeaderText="Applicant Name" />
                                    <asp:BoundField DataField="PANNo" HeaderText="PAN No" />
                                    <asp:BoundField DataField="MobileNo" HeaderText="Mobile No" />
                                    <asp:BoundField DataField="NomineeName" HeaderText="Nominee Name" />

                              <%--      <asp:BoundField DataField="SourceofApplication" HeaderText="Source of Application" />
                                    <asp:BoundField DataField="SourceSpecification" HeaderText="Source of Specification" />
                                    <asp:BoundField DataField="DealerID" HeaderText="Dealer ID" />--%>

                                    <asp:TemplateField HeaderText="Edit" Visible="true" ItemStyle-HorizontalAlign="left">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnUpdate" runat="server" ImageUrl="~/images/edit-icon.png"
                                                Width="18px" Height="18px" CommandName="UpdateRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Delete" Visible="true" ItemStyle-HorizontalAlign="left">
                                        <ItemTemplate>
                                            <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                                OnClientClick="javascript:return ConfirmFunction('Do you really want to Delete Record?');"
                                                Width="20px" Height="20px" CommandName="DeleteRecord" CommandArgument="<%# ((GridViewRow) Container).RowIndex %>" />
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:TemplateField>
                                </Columns>
                                <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#ba2f14" Font-Bold="false" ForeColor="#ffffcc" Height="24px"
                                    Font-Names="Calibri" Font-Size="15px" />
                                <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
                                <RowStyle BackColor="#FFFBD6" ForeColor="#333333" Font-Names="Calibri" Font-Size="13px" />
                                <SelectedRowStyle BackColor="#FFCC66" Font-Bold="true" ForeColor="Navy" Font-Names="Calibri"
                                    Font-Size="13px" />
                                <SortedAscendingCellStyle BackColor="#FDF5AC" />
                                <SortedAscendingHeaderStyle BackColor="#4D0000" />
                                <SortedDescendingCellStyle BackColor="#FCF6C0" />
                                <SortedDescendingHeaderStyle BackColor="#820000" />
                            </asp:GridView>
                        </ContentTemplate>
                        <Triggers>
                            <asp:PostBackTrigger ControlID="dgvDetails" />
                            <asp:AsyncPostBackTrigger ControlID="btnSearch" EventName="Click" />
                        </Triggers>
                    </asp:UpdatePanel>
                    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 80%;">
                        <tr>
                            <td colspan="4">
                                <asp:Label ID="lblMsg" runat="server" CssClass="label" Text=""></asp:Label>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
