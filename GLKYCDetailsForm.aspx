<%@ Page Title="" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    EnableEventValidation="false" CodeFile="GLKYCDetailsForm.aspx.cs" Inherits="GLKYCDetailsForm"
    EnableViewStateMac="true" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/Site.master" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="Server">
    <%-- Added by PRIYA FOR iMG Zoom--%>
    <link href="css/lightbox.css" rel="stylesheet" type="text/css" />
    <script src="js/lightbox-plus-jquery.min.js" type="text/javascript"></script>
    <script src="Jquery/jquery.tablesorter.min.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
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
            return ((k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 40 || k == 41 || k == 38 || k == 44) || k == 0 || k == 8);
        }

        function isAlphaNumSplChars(e) { // Alphanumeric, space,(,),-,' only
            var k;
            document.all ? k = e.keycode : k = e.which;

            return ((k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 39 || k == 40 || k == 41 || k == 45) || k == 0 || k == 8);
        }

        function isAlphaNumChar2(e) { // Alphanumeric, space,-,/ only
            var k;
            document.all ? k = e.keycode : k = e.which;
            return ((k != 188) || (k > 47 && k < 58) || (k > 64 && k < 91) || (k > 96 && k < 123) || (k == 32 || k == 45 || k == 47) || k == 0);
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

        function OnlyNumericSlash() {   // numbers and backslash (/)
            if ((event.keyCode < 47 || event.keyCode > 57)) {
                event.returnValue = false;
            }
        }

        function MakeUpper(txtId) {
            document.getElementById(txtId).value = document.getElementById(txtId).value.toUpperCase();
        }

        //calculate age by kishor
        function findage() {

            var PresentDay = new Date();

            var dateOfBirth = (new Date(document.getElementById('<%=txtBirthDate.ClientID %>').value));
            //            var PresentDay = new Date(dateOfBirth[2], dateOfBirth[1] * 1 - 1, dob[0]);
            var months = (PresentDay.getMonth() - dateOfBirth.getMonth() +
               (12 * (PresentDay.getFullYear() - dateOfBirth.getFullYear())));
            document.getElementById('<%=txtAge.ClientID %>').value = Math.round(months / 12);
        }


        function agevalid(txt) {

            if (isNaN(txt.value.replace('/', '').replace('/', ''))) {

                txt.value = '';
                return false;
            }
        }
        function calculateAge() {

            var DOB = document.getElementById('<%=txtBirthDate.ClientID %>');
            var txtAge = document.getElementById('<%=txtAge.ClientID %>');
            var pattern = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;

            if (DOB.value == '') {
                txtAge.value = '';
                return false;
            }
            if (!pattern.test(DOB.value)) {
                DOB.value = '';
                txtAge.value = '';
                alert('Enter Birth Date In Proper Format');
                return false;
            }


            if (DOB.value != '') {
                now = new Date()

                var txtValue = DOB.value;
                var dob;
                if (txtValue != null)

                    dob = txtValue.split('/');
                if (dob.length == 3) {
                    born = new Date(dob[2], dob[1] * 1 - 1, dob[0]);
                    if (now.getMonth() == born.getMonth() && now.getDate() == born.getDate()) {
                        age = now.getFullYear() - born.getFullYear();

                    }
                    else {
                        age = Math.floor((now.getTime() - born.getTime()) / (365.25 * 24 * 60 * 60 * 1000));
                    }

                    if (isNaN(age)) {
                        DOB.value = '';
                        txtAge.value = '';
                        alert('Enter Birth Date In Proper Format');
                    }
                    else if (parseInt(age) < 18) {
                        DOB.value = '';
                        txtAge.value = '';
                        alert("Applicant's Age must be 18 yrs & above");

                    }
                    else {

                        txtAge.value = age;
                        txtAge.focus();
                    }
                }

            }
        }

        //validate Applicant's Age
        function validateAge() {
            var age = document.getElementById('<%=txtAge.ClientID %>').value;
            //            if (document.getElementById('<%=txtAge.ClientID %>').value != null && document.getElementById('<%=txtAge.ClientID %>').value != "") {
            if (age != null && age != "") {

                if (age < 18) {
                    alert("Applicant's Age must be 18 yrs & above.");
                    return false;
                }

            }
        }
        //  ---------EmailId Format Validation---------
        function checkEmail() {

            var email = document.getElementById('<%=txtEmailId.ClientID %>');
            var filter = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;

            if (!filter.test(email.value)) {

                alert('Please Enter valid email address');
                email.value = '';
                email.focus;

                return false;
            }
        }

        //Mobile No Validation
        function ValidateMobNumber() {
            var fld = document.getElementById('<%=txtMobileNo.ClientID %>');

            if (fld.value == "") {
                alert("You didn't enter a phone number.");
                fld.value = "";
                fld.focus();
                return false;
            }
            else if (isNaN(fld.value)) {
                alert("The phone number contains illegal characters.");
                fld.value = "";
                fld.focus();
                return false;
            }
            else if (!(fld.value.length == 10)) {
                alert("Please enter 10 digit mobile no.");
                fld.value = "";
                fld.focus();
                return false;
            }

        }


        //Function to Check Multiline Textbox Maxlength (Here Address Textbox)
        function Check(textBox, maxLength) {
            if (textBox.value.length > maxLength) {
                //alert("You cannot enter more than " + maxLength + " characters.");
                textBox.value = textBox.value.substr(0, maxLength);
            }
        }
    </script>
    <script type="text/javascript">
        function DealerEnable() {

            var ddlSourceofApplication = document.getElementById('<%=ddlSourceofApplication.ClientID %>');
            var ddlDealerName = document.getElementById('<%=ddlSourceofApplication.ClientID %>');


        }
        function enableother() {


            var btnenableother = document.getElementById("<%=btnenableother.ClientID %>");


            btnenableother.click();

        }
        function clickstate() {

            var btnCity = document.getElementById("<%=btnCity.ClientID %>");
            btnCity.click();
        }



        function validrecord() {

            var txtCutomerID = document.getElementById("<%=txtCutomerID.ClientID %>");
            var txtAppliedDate = document.getElementById("<%=txtAppliedDate.ClientID %>");
            var ddlSourceofApplication = document.getElementById("<%=ddlSourceofApplication.ClientID %>");
            var ddlDealerName = document.getElementById("<%=ddlDealerName.ClientID %>");
            var fUploadPhoto = document.getElementById('<%=fUploadPhoto.ClientID %>');
            var fUploadSign = document.getElementById('<%=fUploadSign.ClientID %>');
            var txtAppFName = document.getElementById('<%=txtAppFName.ClientID %>');
            var txtAppLname = document.getElementById('<%=txtAppLName.ClientID %>');
            var ddlGender = document.getElementById('<%=ddlGender.ClientID %>');
            var ddlMaritalStatus = document.getElementById('<%=ddlMaritalStatus.ClientID %>');
            var txtDOB = document.getElementById('<%=txtBirthDate.ClientID %>');
            var txtAge = document.getElementById('<%=txtAge.ClientID %>');
            var txtBuilding = document.getElementById('<%=txtBldgHouseName.ClientID %>');
            var txtRoad = document.getElementById('<%=txtRoad.ClientID %>');
            var txtBlockNo = document.getElementById('<%=txtRoomBlockNo.ClientID %>');
            var ddlState = document.getElementById('<%=ddlState.ClientID %>');
            var ddlCity = document.getElementById('<%=ddlCity.ClientID %>');
            var ddlArea = document.getElementById('<%=ddlArea.ClientID %>');
            var ddlZone = document.getElementById('<%=ddlZone.ClientID %>');
            var txtPANNo = document.getElementById('<%=txtPANNo.ClientID %>');
            var txtMobileNo = document.getElementById('<%=txtMobileNo.ClientID %>');
            var txtTelephoneNo = document.getElementById('<%=txtTelephoneNo.ClientID %>');
            var ddlOccupation = document.getElementById('<%=ddlOccupation.ClientID %>');
            var ddlPresentAnnIncm = document.getElementById('<%=ddlPresentAnnIncm.ClientID %>');
            var ddlemploymentdetails = document.getElementById('<%=ddlPresentAnnIncm.ClientID %>');
            var txtNomFName = document.getElementById('<%=txtNomFName.ClientID %>');
            var txtNomineeAddress = document.getElementById('<%=txtNomineeAddress.ClientID %>');
            var ddlNomRelation = document.getElementById('<%=txtNomineeAddress.ClientID %>');
            var grd = document.getElementById('<%=gvDocumentDetails.ClientID %>');
            var loanpurposeOther = document.getElementById('<%=txtLoanPurposespecify.ClientID %>');
            var ddlloanpurpose = document.getElementById('<%=ddlLoanPurpose.ClientID %>');
            var ddlIndustrytype = document.getElementById('<%=ddlIndustrytype.ClientID %>');
            var ddlEmploymentDetails = document.getElementById('<%=ddlemploymentdetails.ClientID %>');
            var txtspecifyEmployment = document.getElementById('<%=txtspecifyEmployment.ClientID %>');
            var txtSpecifyIndustryType = document.getElementById('<%=txtSpecifyIndustryType.ClientID %>');
            var hdnoperation = document.getElementById('<%=hdnoperation.ClientID %>');
            var hdnverify = document.getElementById('<%=hdnverify.ClientID %>');
            var txtVerification = document.getElementById('<%=txtVerification.ClientID %>');
            var mob = /[7-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]/;
            var pdob = /^(0[1-9]|1\d|2\d|3[01])\/(0[1-9]|1[0-2])\/(19|20)\d{2}$/;



            if (txtCutomerID.value == '') {

                alert('Customer ID Should Not Be Blank');
                return false;
            }
            if (txtAppliedDate.value == '') {
                alert('Gold Date Should Not Be Blank');
                return false;
            }
            if (ddlloanpurpose.selectedIndex == 0) {

                alert('Select Loan Purpose');
                return false;
            }

            if (ddlloanpurpose.selectedIndex == 7) {
                if (loanpurposeOther.value == '') {

                    alert('Enter Loan Purpose If Other');
                    return false;
                }

            }
            else {
                loanpurposeOther.value == '';
            }

            if (txtAppFName.value == '') {
                alert("Enter Applicant's First Name");
                return false;
            }
            if (txtAppLname.value == '') {
                alert("Enter Applicant's Last Name");
                return false;
            }
            if (ddlGender.selectedIndex == 0) {

                alert('Select Gender');
                return false;
            }
            if (ddlMaritalStatus.selectedIndex == 0) {

                alert('Select Marital Status');
                return false;
            }
            if (txtDOB.value == '') {

                alert('Enter Birth Date');
                return false;
            }
            if (txtAge.value == '' || parseInt(txtAge.value) <= 0) {

                alert('Invalid age');
                return false;
            }
            if (!pdob.test(txtDOB.value)) {

                alert('Enter Birth Date In Proper Format');
                return false;
            }
            if (txtPANNo.value == '') {

                alert('Enter Pan No.');
                return false;
            }

            if (txtMobileNo.value == '') {

                alert('Enter Mobile No.');
                return false;
            }

            if (!mob.test(txtMobileNo.value)) {

                alert('Enter Valid Mobile No.');
                return false;
            }
            if (hdnoperation.value == 'Save') {

                if (hdnverify.value == '0') {
                    alert('Verify Mobile No.');
                    return false;
                }
                else {
                    if (txtVerification.value == '') {
                        alert('Enter Verification Code');
                        return false;
                    }
                    if (hdnverify.value != txtVerification.value) {

                        alert('Invalid Verification Code');
                        return false;
                    }

                }
            }
            else {

                if (hdnverify.value != '0') {


                    if (txtVerification.value == '') {
                        alert('Enter Verification Code');
                        return false;
                    }
                    if (hdnverify.value != txtVerification.value) {

                        alert('Invalid Verification Code');
                        return false;
                    }
                }
            }

            if (txtTelephoneNo.value != '') {

                if (txtTelephoneNo.value.length < 6) {

                    alert('Invalid Telephone No');
                    return false;
                }
            }
            if (ddlSourceofApplication.selectedIndex == 0) {

                //                ddlSourceofApplication.style.border = "1px solid red";
                //                ddlSourceofApplication.focus();
                alert('Select Source Of Application');
                return false;
            }
            //ddlSourceofApplication.style.border = "none";


            if (txtBuilding.value == '') {

                alert('Enter Building/House Name');
                return false;
            }
            if (ddlState.selectedIndex == 0) {

                alert('Select State');
                return false;
            }
            if (ddlCity.selectedIndex == 0) {

                alert('Select City');
                return false;
            }
            if (ddlArea.selectedIndex == 0) {

                alert('Select Area');
                return false;
            }
            if (ddlZone.selectedIndex == 0) {

                alert('Select Zone');
                return false;
            }

            if (ddlOccupation.selectedIndex == 0) {

                alert('Select Occupation');
                return false;
            }

            if (ddlPresentAnnIncm.selectedIndex == 0) {

                alert('Select Present Income');
                return false;
            }
            if (ddlEmploymentDetails.value == 'Other') {

                if (txtspecifyEmployment.value == '') {

                    alert('Enter Specification For Employment');
                    return false;
                }
            }
            else {

                txtspecifyEmployment.value = ''
            }
            if (ddlIndustrytype.value == 'Other') {
                if (txtSpecifyIndustryType.value == '') {

                    alert('Enter Specification For Industry Type');
                    return false;
                }

            }
            else {

                txtSpecifyIndustryType.value = ''
            }







            for (i = 1; i < grd.rows.length - 1; i++) {
                var doc = grd.rows[i].cells[1].getElementsByTagName("span");

                if (doc[0].innerHTML == '') {

                    alert('Add Applicant Document Details');
                    return false;
                }

            }

        }
        function Addvalid() {
            var grd = document.getElementById('<%=gvDocumentDetails.ClientID %>');
            var NOD = grd.rows[grd.rows.length - 1].cells[3].children[0];
            var file = grd.rows[grd.rows.length - 1].cells[5].children[0];
            var doc = grd.rows[grd.rows.length - 1].cells[1].children[0];
            var specify = grd.rows[grd.rows.length - 1].cells[2].children[0];

            if (doc.value == '') {

                alert('select Document');
                return false;
            }

            if (NOD.value == '') {

                alert('Enter Name On Document');
                return false;
            }
            if (file.value == '') {

                alert('Select file for upload');
                return false;
            }


        }

        function Verify() {

            var txtMobileNo = document.getElementById('<%=txtMobileNo.ClientID %>');
            var mob = /[7-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]/;

            if (txtMobileNo.value == '') {

                alert('Enter Mobile No. For Verification');
                return false;
            }
            if (!mob.test(txtMobileNo.value)) {

                alert('Enter Valid Mobile No.');
                return false;
            }

        }
        function validsource() {
            debugger;
            var ddlSourceofApplication = document.getElementById("<%=ddlSourceofApplication.ClientID %>");
            var txtSpecifySource = document.getElementById("<%=txtSpecifySource.ClientID %>");
            var ddlDealerName = document.getElementById("<%=ddlDealerName.ClientID %>");

            //            if (ddlSourceofApplication.value == 'Dealers/Staff') {
            if (ddlSourceofApplication.value == 4) {
                txtSpecifySource.value = '';
                txtSpecifySource.disabled = true;
                ddlDealerName.selectedIndex = 0;
                ddlDealerName.disabled = false;
            }
            else {

                txtSpecifySource.value = '';
                txtSpecifySource.disabled = false;
                ddlDealerName.selectedIndex = 0;
                ddlDealerName.disabled = true;
            }

            if (ddlSourceofApplication.value == 0) {

                txtSpecifySource.value = '';
                txtSpecifySource.disabled = true;
                ddlDealerName.selectedIndex = 0;
                ddlDealerName.disabled = true;
            }

        }
        function validemp() {

            var ddlemploymentdetails = document.getElementById("<%=ddlemploymentdetails.ClientID %>");
            var txtspecifyEmployment = document.getElementById("<%=txtspecifyEmployment.ClientID %>");

            if (ddlemploymentdetails.value == 'Other') {

                txtspecifyEmployment.value = '';
                txtspecifyEmployment.disabled = false;
            }
            else {

                txtspecifyEmployment.value = '';
                txtspecifyEmployment.disabled = true;
            }
        }
        function validind() {

            var ddlIndustrytype = document.getElementById("<%=ddlIndustrytype.ClientID %>");
            var txtSpecifyIndustryType = document.getElementById("<%=txtSpecifyIndustryType.ClientID %>");

            if (ddlIndustrytype.value == 'Other') {

                txtSpecifyIndustryType.value = '';
                txtSpecifyIndustryType.disabled = false;
            }
            else {

                txtSpecifyIndustryType.value = '';
                txtSpecifyIndustryType.disabled = true;
            }
        }
        function validpur() {

            var ddlLoanPurpose = document.getElementById("<%=ddlLoanPurpose.ClientID %>");
            var txtLoanPurposespecify = document.getElementById("<%=txtLoanPurposespecify.ClientID %>");

            if (ddlLoanPurpose.value == '7') {

                txtLoanPurposespecify.value = '';
                txtLoanPurposespecify.disabled = false;
            }
            else {

                txtLoanPurposespecify.value = '';
                txtLoanPurposespecify.disabled = true;
            }
        }
        function docname() {

            var grd = document.getElementById('<%=gvDocumentDetails.ClientID %>');
            var txtAppFName = document.getElementById("<%=txtAppFName.ClientID %>");
            var txtAppMName = document.getElementById("<%=txtAppMName.ClientID %>");
            var txtAppLName = document.getElementById("<%=txtAppLName.ClientID %>");
            var name = '';
            if (txtAppMName.value != '') {

                name = txtAppFName.value + ' ' + txtAppMName.value;
            }
            else {

                name = txtAppFName.value;
            }
            if (txtAppLName.value != '') {

                name = name + ' ' + txtAppLName.value
            }



            grd.rows[grd.rows.length - 1].cells[3].children[0].value = name;

        }

        function showpop() {

            var btnpan = document.getElementById("<%=btnpan.ClientID %>");
            btnpan.click();
        }
    </script>
    <%-- Adde by PRIYA FOR iMG dEMO--%>
    <%--<table width="200" border="1">
        <tr>
            <td>
                <span id='ex2'>
               
                    <a class="example-image-link" href="daisy.jpg" data-lightbox="example-1" height="50px"
                        width="50px">
                        <img class="example-image thumimg" src="daisy.jpg"  height="50px" width="50px" /></a>
                </span>
            </td>
        </tr>
    </table>--%>
    <asp:Button ID="btnpanno" Style="display: none;" runat="server" Text="Button" />
    <asp:ModalPopupExtender ID="modal1" runat="server" TargetControlID="btnpanno" CancelControlID="Button2"
        PopupDragHandleControlID="PopupHeader" BackgroundCssClass="ModalPopupBG" PopupControlID="Panel2">
    </asp:ModalPopupExtender>
    <div class="Controls">
        <input id="btnOkay" style="display: none;" type="button" value="Done" />
        <input id="Button2" style="display: none;" type="button" value="Cancel" />
    </div>
    <asp:Panel ID="Panel2" Style="display: none; height: 25%; width: 45%; overflow: scroll;"
        ScrollBars="Both" runat="server">
        <div style="float: right; width: 100%; overflow: hidden">
            <asp:ImageButton ID="ImgBtnClose1" runat="server" Style="border-top: 1px solid; border-color: Red;"
                ImageUrl="~/images/DeleteRed.png" Height="20" Width="20" ToolTip="Close" OnClick="ImgBtnClose1_Click" />
        </div>
        <div style="height: 100%; padding-top: 10px; min-width: 500px; height: auto; background: white;">
            <div class="PopupHeader" id="PopupHeader">
                <b>
                    <asp:Label ID="Label2" runat="server" Text=""></asp:Label>
                </b>
            </div>
            <br />
            <div style="background-color: #ffbe00; width: 100%;">
                <table>
                    <tr>
                        <td>
                            <asp:Label ID="lbldetails" Style="font-size: 13px; font-weight: bold; color: #354d8d;"
                                runat="server" Text="Aphelion Finance Pvt. Ltd."></asp:Label>
                        </td>
                    </tr>
                </table>
                <br />
            </div>
            <div>
                <table>
                    <tr>
                        <td class="label" style="text-align: left; padding-left: 10px;">
                            <asp:Label ID="lblenteredpanno" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" valign="top" style="text-align: left; padding-left: 10px;">
                            <asp:Label ID="lbld" runat="server" Text="Details are as follows:-"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <table>
                    <tr>
                        <td class="label" style="text-align: left; padding-left: 10px;">
                            <asp:GridView ID="grdpan" runat="server" AutoGenerateColumns="false" HeaderStyle-CssClass="glrecptgVHeader">
                                <AlternatingRowStyle BackColor="White" />
                                <HeaderStyle CssClass="gVHeader" />
                                <Columns>
                                    <asp:TemplateField HeaderText="Customer ID">
                                        <ItemTemplate>
                                            <asp:Label ID="lblcustomerid" runat="server" Style="text-align: left;" Text='<%#Eval("CustomerID") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Name">
                                        <ItemTemplate>
                                            <asp:Label ID="lblName" runat="server" Style="text-align: left;" Text='<%#Eval("Name") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Birth Date">
                                        <ItemTemplate>
                                            <asp:Label ID="lblBirthDate" runat="server" Style="text-align: left;" Text='<%#Eval("BirthDate") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Loan Type">
                                        <ItemTemplate>
                                            <asp:Label ID="lblLoanType" runat="server" Style="text-align: left;" Text='<%#Eval("LoanType") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Gold Loan No">
                                        <ItemTemplate>
                                            <asp:Label ID="lblGoldLoanNo" runat="server" Style="text-align: left;" Text='<%#Eval("GoldLoanNo") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Source Of Application">
                                        <ItemTemplate>
                                            <asp:Label ID="lblSourceThrough" runat="server" Style="text-align: left;" Text='<%#Eval("Source Through") %>'></asp:Label>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </asp:Panel>
    <asp:HiddenField ID="hdnid" runat="server" Value="0" />
    <asp:HiddenField ID="hdnoperation" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnpopup" runat="server" Value="Save" />
    <asp:HiddenField ID="hdnverify" runat="server" Value="0" />
    <asp:HiddenField ID="hdnUserID" runat="server" Value="0" />
    <asp:HiddenField ID="hdnBranch" runat="server" Value="0" />
    <asp:HiddenField ID="hdnFYear" runat="server" Value="0" />
    <asp:Button ID="btnenableother" runat="server" Text="Button" Style="display: none;"
        OnClick="btnenableother_Click" />
    <table align="center" cellpadding="0" cellspacing="0" border="0" style="width: 90%;">
        <tr>
            <td style="width: 20%;">
            </td>
            <td style="width: 26%;">
            </td>
            <td style="width: 20%;">
            </td>
            <td style="width: 26%;">
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
                            <asp:Label ID="lblHeader" runat="server" Text="GOLD LOAN KYC Details"></asp:Label>
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
            <td class="label" style="text-align: left;">
                <b>Customer ID</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtCutomerID" CssClass="textbox_readonly" runat="server" Font-Bold="true"
                    Width="90%"></asp:TextBox>
            </td>
            <!-- Applicant Photo -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Applicant's Photo
            </td>
            <td class="txt_style" rowspan="2" valign="top">
                <table>
                    <tr>
                        <td>
                            <asp:Image ID="imgAppPhoto" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" ImageUrl="" />
                            <%--<asp:GridView ID="grdPhoto" runat="server">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            
                                            <a class="example-image-link" style="width: 400%; height: 300%;" href='<%# Eval("ImagePath") %>'
                                                data-lightbox="example-1">
                                                <asp:Image ID="imgAppPhoto" class="example-image-link" runat="server" Width="40px"
                                                    Height="50px" ImageUrl='<%# Eval("ImagePath") %>' />
                                            </a>
                                        </ItemTemplate>
                                        <ItemStyle CssClass="gVItem" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>--%>
                        </td>
                        <td valign="top" align="left">
                            <asp:FileUpload ID="fUploadPhoto" runat="server" Width="80px" />
                            <br />
                            <asp:Button ID="btnUploadPhoto" runat="server" Text="Upload" Width="80px" OnClick="btnUploadPhoto_Click" />
                        </td>
                    </tr>
                </table>
                <div id="divBackground" class="modal">
                    <div id="divImage" class="info">
                        <table style="height: 100%; width: 100%">
                            <tr>
                                <td valign="middle" align="center">
                                    <img id="imgLoader" alt="" src="" />
                                    <%--images/loader.gif--%>
                                    <img id="imgFull" runat="server" alt="" src="" style="display: none; height: 500px;
                                        width: 590px" />
                                </td>
                            </tr>
                            <tr>
                                <td align="center" valign="bottom">
                                    <input id="btnClose" type="button" value="close" onclick="HideDiv()" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <!-- Loan Date -->
            <td class="label" style="text-align: left;">
                Applied Date
            </td>
            <td class="txt_style" align="left">
                <asp:TextBox ID="txtAppliedDate" CssClass="textbox_readonly" runat="server" Width="90%"
                    MaxLength="10"></asp:TextBox>
            </td>
        </tr>
        <!-- Loan Date (Date Format validation) -->
        <tr>
            <!-- Operator Name -->
            <td class="label" style="text-align: left;">
                Operator Name
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtOperatorName" CssClass="textbox_readonly" ReadOnly="true" runat="server"
                    Width="90%"></asp:TextBox>
            </td>
            <!-- Applicant Signature -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                <asp:Label ID="Label3" runat="server" Text="Applicant's Signature"></asp:Label>
            </td>
            <td class="txt_style" rowspan="2" valign="top">
                <table>
                    <tr>
                        <td>
                            <asp:Image ID="imgAppSign" runat="server" Width="50px" Height="50px" BorderStyle="Inset"
                                BorderWidth="1px" ImageAlign="AbsMiddle" ImageUrl="" />
                        </td>
                        <td valign="top" align="left">
                            <asp:FileUpload ID="fUploadSign" runat="server" Width="80px" />
                            <br />
                            <asp:Button ID="btnUploadSign" runat="server" Text="Upload" OnClick="btnUploadSign_Click"
                                Width="80px" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <!-- Existing GL No. -->
            <td class="label" style="text-align: left;">
                <asp:Label ID="Label1" runat="server" Text="Existing Customer ID" ForeColor="Red"></asp:Label>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtExCustomerId" CssClass="textbox_readonly" runat="server" MaxLength="12"
                    Width="80%" placeholder="To auto-generate details"></asp:TextBox>
                <asp:ImageButton ID="imgbtnExCustomer" ImageUrl="~/images/1397069814_Search.png"
                    Height="20px" Width="20px" runat="server" ImageAlign="AbsMiddle" ToolTip="Click for search existing customer"
                    OnClick="imgbtnExCustomer_Click" />
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Purpose Of Loan<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlLoanPurpose" CssClass="textbox" runat="server" Width="95%"
                    Height="26px" onchange="return validpur();">
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Specify If Other
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtLoanPurposespecify" CssClass="textbox" placeholder="Loan Purpose if Other"
                    runat="server" Width="90%" MaxLength="30" onkeypress="return isAlphaNumChar2(event);"> </asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Applicant Name -->
            <td class="label" style="text-align: left;">
                Applicant's Name<b style="color: Red;">*</b>
            </td>
            <!--App First Name -->
            <td class="txt_style">
                <asp:TextBox ID="txtAppFName" CssClass="textbox" runat="server" Width="90%" onkeypress="return isAlphaNumChars(event)"
                    MaxLength="100" placeholder="First Name" onkeyup="return docname();"></asp:TextBox>
            </td>
            <td class="txt_style">
                <!--App Middle Name -->
                <asp:TextBox ID="txtAppMName" CssClass="textbox" runat="server" Width="90%" onkeypress="return isAlphaNumChars(event)"
                    MaxLength="100" placeholder="Middle Name" onkeyup="return docname();"></asp:TextBox>
            </td>
            <td class="txt_style">
                <!--App Last Name -->
                <asp:TextBox ID="txtAppLName" CssClass="textbox" runat="server" Width="90%" onkeypress="return isAlphaNumChars(event)"
                    MaxLength="100" placeholder="Last Name" onkeyup="return docname();"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="label" style="text-align: left; text-decoration: underline;
                font-weight: bold;">
                <br />
                Personal Details:-
            </td>
        </tr>
        <tr>
            <!-- Loan Type -->
            <td class="label" style="text-align: left;">
                Gender<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="textbox" Height="26px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Gender--</asp:ListItem>
                    <asp:ListItem Value="Male">Male</asp:ListItem>
                    <asp:ListItem Value="Female">Female</asp:ListItem>
                </asp:DropDownList>
            </td>
            <!-- Existing PL Customer -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Marital Status<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlMaritalStatus" runat="server" CssClass="textbox" Height="26px"
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
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Birth Date<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtBirthDate" CssClass="textbox" runat="server" MaxLength="10" Width="82%"
                    Onchange="calculateAge();" onkeypress="return agevalid(this);" placeholder="dd/mm/yyyy"></asp:TextBox>
                <asp:CalendarExtender ID="txtBirthDate_CalendarExtender" Format="dd/MM/yyyy" PopupButtonID="CalenderImg"
                    runat="server" CssClass="Calenderstyle" Enabled="True" TargetControlID="txtBirthDate">
                </asp:CalendarExtender>
                <asp:ImageButton ID="CalenderImg" runat="server" ImageAlign="Middle" ImageUrl="~/images/calenderIMG.jpg"
                    Width="15" Height="15" />
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Spouse Name
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSpouse" runat="server" onkeypress="return isAlphaNumChars(event);"
                    CssClass="textbox" Width="90%" MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Pan No. -->
            <td class="label" style="text-align: left;">
                Age
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtAge" runat="server" CssClass="textbox_readonly" Width="90%" Onchange="validateAge();"></asp:TextBox>
            </td>
            <!-- Gender -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Children Name
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtChildren" onkeypress="return isAlphaNumChars(event);" runat="server"
                    CssClass="textbox" Width="90%" MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Marital Status -->
            <td class="label" style="text-align: left;">
                PAN No.<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:Button ID="btnpan" runat="server" OnClick="btnpan_Click" Style="display: none" />
                <asp:TextBox ID="txtPANNo" runat="server" CssClass="textbox" Width="90%" MaxLength="10"
                    onkeypress="return isAlphaNumeric(event);" onblur="showpop();" Style="text-transform: uppercase;"></asp:TextBox>
            </td>
            <!-- BirthDate -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                Mobile<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtMobileNo" CssClass="textbox" runat="server" Width="63%" MaxLength="10"
                    onkeypress="return OnlyNumericEntry();" Onchange="ValidateMobNumber();"></asp:TextBox>
                <asp:Button ID="btnVerify" runat="server" CssClass="css_btn_class" Style="margin-left: 0px;
                    width: auto;" Text="Verify" OnClientClick="return Verify();" OnClick="btnVerify_Click" />
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Telephone No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtTelephoneNo" onkeypress="return isNumericHyphen(event);" CssClass="textbox"
                    MaxLength="15" runat="server" Width="90%"></asp:TextBox>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Verification Code
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtVerification" CssClass="textbox" runat="server" Width="90%" MaxLength="10"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Email ID
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtEmailId" MaxLength="50" CssClass="textbox" Onchange="checkEmail(); "
                    runat="server" Width="96.5%" Style="text-transform: lowercase;"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Building/House Name -->
            <td class="label" style="text-align: left;">
                Source Of Application<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlSourceofApplication" runat="server" CssClass="textbox" Width="95%"
                    onchange="return validsource();">
                    <%-- <asp:ListItem Value="0">--Select--</asp:ListItem>
                    <asp:ListItem>Banner</asp:ListItem>
                    <asp:ListItem>Cable advt</asp:ListItem>
                    <asp:ListItem>Cold calls</asp:ListItem>
                    <asp:ListItem>Dealer</asp:ListItem>
                    <asp:ListItem>Google  Ads</asp:ListItem>
                    <asp:ListItem>Jewellers</asp:ListItem>
                    <asp:ListItem>Justdial</asp:ListItem>
                    <asp:ListItem>Newspaper</asp:ListItem>
                    <asp:ListItem>Others</asp:ListItem>
                    <asp:ListItem>PL enquiries</asp:ListItem>--%>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Existing PL-Case No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtExistingPLCaseNo" runat="server" CssClass="textbox" Width="90%"
                    MaxLength="10" onkeypress="return OnlyNumericEntry();" Style="text-transform: uppercase;"
                    placeholder="IF EXISTING PL CUSTOMER"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Road -->
            <td class="label" style="text-align: left;">
                Specify Source
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtSpecifySource" CssClass="textbox" runat="server" Width="96.5%"
                    placeholder="Source Name/If Other" onkeypress="return isAlphaNumChars(event);"
                    MaxLength="50"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Select Dealer
            </td>
            <td colspan="3" class="txt_style">
                <asp:DropDownList ID="ddlDealerName" runat="server" CssClass="textbox" Width="98.5%"
                    Height="27px">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="4" class="label" style="text-align: left; text-decoration: underline;
                font-weight: bold;">
                <br />
                Address Details:-
            </td>
        </tr>
        <tr>
            <!-- Building/Plot No. -->
            <td class="label" style="text-align: left;">
                Building/House Name<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtBldgHouseName" CssClass="textbox" runat="server" Width="90%"
                    onkeypress="return isAlphaNumSplChars(event);" MaxLength="50"></asp:TextBox>
            </td>
            <!-- Room/Block No. -->
            <td class="label" style="text-align: left; padding-left: 20px;">
                State<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlState" runat="server" CssClass="textbox" Height="26px" Width="95%"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlState_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:Button ID="btnCity" runat="server" Text="Button" Style="display: none;" OnClick="btnCity_Click" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <!-- Regex validation for Plot No -->
        <tr>
            <td class="label" style="text-align: left;">
                Road
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRoad" CssClass="textbox" runat="server" Width="90%" MaxLength="30"
                    onkeypress="return isAlphaNumSplChars(event);"></asp:TextBox>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                City<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlCity" runat="server" CssClass="textbox" Height="26px" Width="95%"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlCity_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <!-- Regex validation for RoomBlockNo -->
        <tr>
            <td class="label" style="text-align: left;">
                Building/Plot No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtPlotNo" CssClass="textbox" runat="server" Width="90%" MaxLength="20"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Area<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlArea" runat="server" CssClass="textbox" Height="26px" Width="95%"
                            AutoPostBack="True" OnSelectedIndexChanged="ddlArea_SelectedIndexChanged">
                        </asp:DropDownList>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlCity" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlState" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <!-- State -->
            <td class="label" style="text-align: left;">
                Room/Block No.
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtRoomBlockNo" CssClass="textbox" runat="server" Width="90%" MaxLength="20"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
                <!-- City -->
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Zone<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:UpdatePanel ID="UpdatePanel7" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:DropDownList ID="ddlZone" runat="server" CssClass="textbox" Height="26px" Width="95%">
                        </asp:DropDownList>
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
            <td class="label" style="text-align: left;">
                Nearest Landmark
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNearestLandmark" CssClass="textbox" runat="server" Width="96.5%"
                    MaxLength="80"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Mobile No. -->
            <td class="label" colspan="4" style="text-align: left; text-decoration: underline;
                font-weight: bold;">
                <br />
                Income Details :-
            </td>
        </tr>
        <tr>
            <!-- Mobile No. -->
            <td class="label" style="text-align: left;">
                Occupation<b style="color: Red;">*</b>
            </td>
            <td class="txt_style" style="text-align: left;">
                <asp:DropDownList ID="ddlOccupation" runat="server" CssClass="textbox" Height="26px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Occupation--</asp:ListItem>
                    <asp:ListItem Value="Teacher">Teacher</asp:ListItem>
                    <asp:ListItem Value="Business">Business</asp:ListItem>
                    <asp:ListItem Value="HouseWife">HouseWife</asp:ListItem>
                    <asp:ListItem Value="Service">Service</asp:ListItem>
                    <asp:ListItem Value="Retired">Retired</asp:ListItem>
                    <asp:ListItem Value="Painter">Painter</asp:ListItem>
                    <asp:ListItem Value="Nothing">Nothing</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Name Of Organization
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtNameofOrg" runat="server" CssClass="textbox" MaxLength="30" Width="90%"
                    onkeypress="return isAlphaNumChar2(event);"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Email Address -->
            <td class="label" style="text-align: left;">
                Present Annual Income<b style="color: Red;">*</b>
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlPresentAnnIncm" runat="server" CssClass="textbox" Height="26px"
                    Width="95%">
                    <asp:ListItem Value="0">--Select Annual Income--</asp:ListItem>
                    <asp:ListItem Value="None">None</asp:ListItem>
                    <asp:ListItem Value="<100000"><100000</asp:ListItem>
                    <asp:ListItem Value="100000 to 300000">100000 to 300000</asp:ListItem>
                    <asp:ListItem Value="300000 to 500000">300000 to 500000</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Address Of Office
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtOfficeAdd" MaxLength="30" runat="server" onkeypress="return isAlphaNumChar2(event);"
                    CssClass="textbox" Width="90%"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!---Ocuupation details-->
            <td class="label" style="text-align: left;">
                Employment Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlemploymentdetails" runat="server" CssClass="textbox" Height="26px"
                    Width="95%" onchange="return validemp();">
                    <asp:ListItem Value="0">--Select Employment Type--</asp:ListItem>
                    <asp:ListItem Value="Salaried">Salaried</asp:ListItem>
                    <asp:ListItem Value="SelfEmployed">SelfEmployed</asp:ListItem>
                    <asp:ListItem Value="Other">Others</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Industry Type
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlIndustrytype" runat="server" CssClass="textbox" Height="26px"
                    Width="95%" onchange="return validind();">
                    <asp:ListItem Value="0">--Select Industry Type--</asp:ListItem>
                    <asp:ListItem Value="Agriculture">Agriculture</asp:ListItem>
                    <asp:ListItem Value="Banking">Banking</asp:ListItem>
                    <asp:ListItem Value="Consultancy">Consultancy</asp:ListItem>
                    <asp:ListItem Value="Contract Worker">Contract Worker</asp:ListItem>
                    <asp:ListItem Value="Freelancer">Freelancer</asp:ListItem>
                    <asp:ListItem Value="Health">Health</asp:ListItem>
                    <asp:ListItem Value="Manufacturing">Manufacturing</asp:ListItem>
                    <asp:ListItem Value="Professional">Professional</asp:ListItem>
                    <asp:ListItem Value="Retail Trading">Retail Trading</asp:ListItem>
                    <asp:ListItem Value="Small Scale Industry">Small Scale Industry</asp:ListItem>
                    <asp:ListItem Value="Software">Software</asp:ListItem>
                    <asp:ListItem Value="Transport">Transport</asp:ListItem>
                    <asp:ListItem Value="Other">Others</asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Specify if Other
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtspecifyEmployment" runat="server" CssClass="textbox" MaxLength="50"
                    Width="90%" onkeypress="return isAlphaNumChar2(event);" placeholder="Specify Other Employment"></asp:TextBox>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
                Specify If Other
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSpecifyIndustryType" runat="server" CssClass="textbox" MaxLength="50"
                    Width="90%" onkeypress="return isAlphaNumChar2(event);" placeholder="Specify Other Industry Type"> </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="label" style="text-align: left;">
                Designation
            </td>
            <td class="txt_style">
                <asp:TextBox ID="txtSpecifyDesigntn" onkeypress="return isAlphaNumChars(event);"
                    CssClass="textbox" Width="90%" runat="server"></asp:TextBox>
            </td>
            <td class="label">
            </td>
            <td class="txt_style">
            </td>
        </tr>
        <tr>
            <td colspan="4" class="label" style="text-align: left; text-decoration: underline;
                font-weight: bold;">
                <br />
                Nominee Details :-
            </td>
        </tr>
        <tr>
            <!-- Nominee Name -->
            <td class="label" style="text-align: left;">
                Nominee Name
            </td>
            <td class="txt_style">
                <!--Nom First Name -->
                <asp:TextBox ID="txtNomFName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="First Name"></asp:TextBox>
            </td>
            <td class="txt_style">
                <!--Nom Middle Name -->
                <asp:TextBox ID="txtNomMName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Middle Name"></asp:TextBox>
            </td>
            <td class="txt_style">
                <!--Nom Last Name -->
                <asp:TextBox ID="txtNomLName" CssClass="textbox" runat="server" Width="90%" onkeypress="return lettersOnly(event)"
                    MaxLength="30" placeholder="Last Name"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Nominee Address -->
            <td class="label" style="text-align: left;">
                Nominee Address
            </td>
            <td class="txt_style" colspan="3">
                <asp:TextBox ID="txtNomineeAddress" CssClass="textbox" runat="server" Width="96.5%"
                    onkeypress="return isAlphaNumChar2(event);" Height="35px" TextMode="MultiLine"
                    onkeyup="javascript:Check(this, 200);" onchange="javascript:Check(this, 200);"
                    MaxLength="200"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <!-- Nominee Relationship -->
            <td class="label" style="text-align: left;">
                Nominee Relationship
            </td>
            <td class="txt_style">
                <asp:DropDownList ID="ddlNomRelation" runat="server" CssClass="textbox" Height="27px"
                    Width="97%">
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
                    <asp:ListItem Value="Other">Others</asp:ListItem>
                    <asp:ListItem Value="Self">Self</asp:ListItem>
                    <asp:ListItem Value="Sister">Sister</asp:ListItem>
                    <asp:ListItem Value="Son">Son</asp:ListItem>
                    <asp:ListItem Value="Uncle">Uncle</asp:ListItem>
                    <asp:ListItem Value="Wife">Wife</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="label" style="text-align: left; padding-left: 20px;">
            </td>
            <td class="txt_style">
            </td>
        </tr>
        <tr>
            <td>
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
            <td colspan="4" class="label" style="text-align: left; text-decoration: underline;
                font-weight: bold;">
                <br />
                Applicant's Document Details :-<b style="color: Red; text-decoration: none;">*</b>
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td colspan="4">
                <div id="divDocDetails" runat="server">
                    <!--GridView Document Details -->
                    <%--    <asp:UpdatePanel ID="UpdatePanel3" runat="server" UpdateMode="Conditional">--%>
                    <%--      <ContentTemplate>--%>
                    <asp:GridView ID="gvDocumentDetails" runat="server" ShowFooter="true" AutoGenerateColumns="False"
                        Width="98%">
                        <HeaderStyle CssClass="gVHeader" />
                        <RowStyle CssClass="gVItem" />
                        <Columns>
                            <asp:TemplateField HeaderText="Sr.No">
                                <ItemTemplate>
                                    <asp:Label ID="lblSrNo" Text='<%# Eval("Serialno") %>' runat="server"></asp:Label>
                                    <asp:HiddenField ID="hdndocid" runat="server" Value='<%# Eval("DID") %>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Document Name">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdndocumentid" runat="server" Value='<%# Eval("DocumentID") %>' />
                                    <asp:Label ID="lblDocName" runat="server" Text='<%# Eval("DocName") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:UpdatePanel ID="UpdatePanel12" runat="server" UpdateMode="Conditional">
                                        <ContentTemplate>
                                            <asp:DropDownList ID="ddlDocName" runat="server" Height="22px" Width="100px" DataSourceID="SqlDataSource1"
                                                DataTextField="DocumentName" DataValueField="DocumentID">
                                            </asp:DropDownList>
                                            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringLocal %>"
                                                SelectCommand="SELECT * FROM [tbl_GLDocumentMaster]"></asp:SqlDataSource>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Specify Other">
                                <ItemTemplate>
                                    <asp:Label ID="lblSpecifyOther" runat="server" Text='<%# Eval("OtherDoc") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtSpecifyOther" Text="" MaxLength="30" Width="90px" runat="server"
                                        placeholder="Specify if Other" onkeypress="return isAlphaNumChars(event);"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Name on document">
                                <ItemTemplate>
                                    <asp:Label ID="lblDocOnName" runat="server" Text='<%# Eval("NameOnDoc") %>'></asp:Label>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:TextBox ID="txtNameOnDoc" runat="server" MaxLength="30" onkeypress="return isAlphaNumChars(event);"></asp:TextBox>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Verify By">
                                <ItemTemplate>
                                    <asp:Label ID="lblEmpname" runat="server" Text='<%# Eval("VerifiedBy") %>'></asp:Label>
                                    <asp:HiddenField ID="hdnempid" runat="server" Value='<%# Eval("Empld") %>' />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <asp:DropDownList ID="ddlEmployeeName" runat="server" Height="22px" Width="100px"
                                        DataSourceID="SqlDataSource2" DataTextField="FullName" DataValueField="EmployeeID">
                                    </asp:DropDownList>
                                    <asp:SqlDataSource ID="SqlDataSource2" runat="server" ConnectionString="<%$ ConnectionStrings:NFConnectionStringAIM %>"
                                        SelectCommand="SELECT EmployeeID,EmpFirstName + ' ' + EmpLastName As FullName From [tblHRMS_EmployeeMaster] order by EmpFirstName">
                                    </asp:SqlDataSource>
                                </FooterTemplate>
                                <ItemStyle HorizontalAlign="Left" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Photo Copy">
                                <ItemTemplate>
                                    <asp:HiddenField ID="hdnimgpath" runat="server" Value='<%# Eval("ImagePath") %>' />
                                    <%-- CODE aDDED BY Priya for Image Zoom--%>
                                    <a class="example-image-link" style="width: 400%; height: 300%;" href='<%# Eval("ImagePath") %>'
                                        data-lightbox="example-1">
                                        <asp:Image ID="imgDocPhoto" class="example-image-link" runat="server" Width="40px"
                                            Height="50px" ImageUrl='<%# Eval("ImagePath") %>' />
                                    </a>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <%--  <asp:FileUpload ID="FileUpload1" runat="server" Width="130px" />--%>
                                    <asp:FileUpload ID="Fileupload1" runat="server" Width="130" />
                                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ForeColor="Red"
                                        Font-Bold="true" ErrorMessage="*" Display="Dynamic" ValidationGroup="Upload"
                                        ControlToValidate="Fileupload1"></asp:RequiredFieldValidator>
                                </FooterTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Action">
                                <ItemTemplate>
                                    <asp:ImageButton ID="btnDelete" runat="server" ImageUrl="~/images/DeleteRed.png"
                                        ImageAlign="AbsMiddle" OnClientClick="javascript:return ConfirmFunction('Do you really want to Remove Record?');"
                                        Width="20px" Height="20px" OnClick="btnDelete_Click" />
                                </ItemTemplate>
                                <FooterTemplate>
                                    <%-- <asp:UpdatePanel ID="up1" runat="server" UpdateMode="Conditional">
                                                <ContentTemplate>--%>
                                    <asp:Button ID="btnUpload" runat="server" Text="Add Details" ValidationGroup="Upload"
                                        OnClick="btnUpload_Click" OnClientClick="return Addvalid();" />
                                    <%--</ContentTemplate>
                                                <Triggers>
                                                    <asp:PostBackTrigger ControlID="btnUpload" />
                                                </Triggers>
                                            </asp:UpdatePanel>--%>
                                </FooterTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    <%--    </ContentTemplate>--%>
                    <%--  <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="gvDocumentDetails" />
                        </Triggers>--%>
                    <%--   </asp:UpdatePanel>--%>
                </div>
            </td>
        </tr>
    </table>
    <br />
</asp:Content>
