<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default"
    EnableViewStateMac="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" xml:lang="en">
<head id="LGHead" runat="server">
    <title>APHELION FINANCE</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <link rel="icon" href="icons/favicon.ico" type="image/x-icon" />
    <link rel="shortcut icon" href="icons/favicon.ico" type="image/x-icon" />
    <link href='http://fonts.googleapis.com/css?family=Arimo:400,700,400italic,700italic'
        rel='stylesheet' type='text/css' />
    <link href='http://fonts.googleapis.com/css?family=Source+Sans+Pro:200,400,600,900,200italic,400italic,600italic'
        rel='stylesheet' type='text/css' />
    <script src='http://ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js' type='text/javascript'></script>
    <%--<link href="css/style.css" rel="stylesheet" type="text/css" />--%>
    <style type="text/css">
        
        .loginbackground
{
    background-repeat:no-repeat;
    background-size:100%;
    background: #f5c614; /* Old browsers */
/* IE9 SVG, needs conditional override of 'filter' to 'none' */
background: url(data:image/svg+xml;base64,PD94bWwgdmVyc2lvbj0iMS4wIiA/Pgo8c3ZnIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyIgd2lkdGg9IjEwMCUiIGhlaWdodD0iMTAwJSIgdmlld0JveD0iMCAwIDEgMSIgcHJlc2VydmVBc3BlY3RSYXRpbz0ibm9uZSI+CiAgPGxpbmVhckdyYWRpZW50IGlkPSJncmFkLXVjZ2ctZ2VuZXJhdGVkIiBncmFkaWVudFVuaXRzPSJ1c2VyU3BhY2VPblVzZSIgeDE9IjAlIiB5MT0iMCUiIHgyPSIwJSIgeTI9IjEwMCUiPgogICAgPHN0b3Agb2Zmc2V0PSIwJSIgc3RvcC1jb2xvcj0iI2Y1YzYxNCIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgICA8c3RvcCBvZmZzZXQ9IjcxJSIgc3RvcC1jb2xvcj0iI2ZlZmNlYSIgc3RvcC1vcGFjaXR5PSIxIi8+CiAgPC9saW5lYXJHcmFkaWVudD4KICA8cmVjdCB4PSIwIiB5PSIwIiB3aWR0aD0iMSIgaGVpZ2h0PSIxIiBmaWxsPSJ1cmwoI2dyYWQtdWNnZy1nZW5lcmF0ZWQpIiAvPgo8L3N2Zz4=);
background: -moz-linear-gradient(top,  #f5c614 0%, #fefcea 71%); /* FF3.6+ */
background: -webkit-gradient(linear, left top, left bottom, color-stop(0%,#f5c614), color-stop(71%,#fefcea)); /* Chrome,Safari4+ */
background: -webkit-linear-gradient(top,  #f5c614 0%,#fefcea 71%); /* Chrome10+,Safari5.1+ */
background: -o-linear-gradient(top,  #f5c614 0%,#fefcea 71%); /* Opera 11.10+ */
background: -ms-linear-gradient(top,  #f5c614 0%,#fefcea 71%); /* IE10+ */
background: linear-gradient(to bottom,  #f5c614 0%,#fefcea 71%); /* W3C */
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#f5c614', endColorstr='#fefcea',GradientType=0 ); /* IE6-8 */

}
    .login_tbl_txt
{
 font-family:verdana;
 font-size:11px;
 font-weight:none;
 color:#000000;
 text-align:justify;
 border:3px #097193 solid;
 border-radius: 7px;
 -moz-border-radius: 7px;
 -o-border-radius: 7px;
 -webkit-border-radius: 7px;
 background-color: white;
 box-shadow: 1px 1px 3px 2px rgba(0,0,0,0.3);
 -webkit-box-shadow: 1px 1px 3px 2px rgba(0,0,0,0.3);
 -o-box-shadow: 1px 1px 3px 2px rgba(0,0,0,0.3);
 -moz-box-shadow: 1px 1px 3px 2px rgba(0,0,0,0.3);
 width: 639px;
}
.login_tbl_sub
{
 font-family:verdana;
 font-size:11px;
 font-weight:none;
 color:#000000;
 text-align:justify;
 border:2px #097193 solid;
 border-radius: 7px;
 -moz-border-radius: 7px;
 -o-border-radius: 7px;
 -webkit-border-radius: 7px;
 background-color: #fbf1cd;
 height: 133px;
 width: 460px;
 }    
.Username
{
 font-family:verdana;
 font-size:12px;
 font-weight:normal;
 color:#000000;
 text-decoration:none;
 border: 1px #000000 solid;
 background:#F1F1F1;
 padding-left: 3px;
 border-radius: 4px;
 -moz-border-radius: 4px;
 -o-border-radius: 4px;
 -webkit-border-radius: 4px;
 box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -webkit-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -o-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -moz-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 width: 248px;
 height: 22px;
}

.ddlStyle
{
 font-family:verdana;
 font-size:11px;
 font-weight:normal;
 color:#000000;
 text-decoration:none;
 text-transform: uppercase;
 border: 1px #000000 solid;
 background:#F1F1F1;
 padding-left: 3px;
 border-radius: 4px;
 -moz-border-radius: 4px;
 -o-border-radius: 4px;
 -webkit-border-radius: 4px;
 box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -webkit-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -o-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -moz-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 width: 248px;
 height: 22px;
}

.Password
{
 font-family:verdana;
 font-size:12px;
 font-weight:normal;
 color:#000000;
 text-decoration:none;
 border: 1px #000000 solid;
 background:#F1F1F1;
 padding-left: 3px;
 border-radius: 4px;
 -moz-border-radius: 4px;
 -o-border-radius: 4px;
 -webkit-border-radius: 4px;
 box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -webkit-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -o-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 -moz-box-shadow: inset 0 1px 2px rgba(0,0,0,0.3);
 width: 248px;
 height: 22px;
}
.message
{
 color: #FF0000;
 background-color: #FFDFDF;
 height: 20px;
 font-size: 14px;
 border: 1px #FF0000 solid;
 border-radius: 5px;
 -moz-border-radius: 5px;
 -o-border-radius: 5px;
 -webkit-border-radius: 5px;
}
.button 
{
   background-repeat:no-repeat;
    background-size:100%;
    background: #156fae; /* Old browsers */
padding: 6px 12px;
   -webkit-border-radius: 8px;
   -moz-border-radius: 8px;
   border-radius: 8px;
   border-color:#ffffff;
   -webkit-box-shadow: rgba(0,0,0,1) 0 1px 0;
   -moz-box-shadow: rgba(0,0,0,1) 0 1px 0;
   box-shadow: rgba(0,0,0,1) 0 1px 0;
   text-shadow: rgba(0,0,0,.4) 0 1px 0;
   color: #ffffff;
   font-size: 13px;
   font-family: Georgia, Serif;
   text-decoration: none;
   vertical-align: middle;
}
.button:hover 
{
   border-top-color: #28597a;
   background: #28597a;
   color: #ccc;
}
.button:active
{
   border-top-color: #1b435e;
   background: #1b435e;

    .style4
    {
        width: 1159px;
        text-align: right;
    }
    .style5
    {
        width: 292px;
        text-align: left;
    }
    #AphImage
    {
        height: 81px;
        width: 122px;
        float: left;
    }
    .style9
    {
        font-family: "Monotype Corsiva";
        font-size: xx-large;
        height: 27px;
    }
    .style10
    {
    }
    .style11
    {
        height: 171px;
    }
    .style12
    {
        width: 292px;
       
    }
    .style14
    {
        height: 18px;
    }
    .style15
    {
        width: 1159px;
        text-align: right;
        height: 21px;
    }
    .style16
    {
        width: 292px;
        text-align: left;
        height: 21px;
    }
    .style17
    {
        width: 1159px;
    }
    .style18
    {
        width: 1159px;
        text-align: right;
        height: 10px;
    }
    .style19
    {
        width: 292px;
        text-align: left;
        height: 10px;
    }
    }
    .style1
    {
        text-align: right;
        width: 161px;
    }
    #AphImage
    {
        height: 98px;
        width: 150px;
    }
    .style2
    {
        width: 97px;
    }
    .style3
    {
        font-family: Calibri;
        font-size:x-large;
        color: #ffffff;
    }
    .style4
    {
        color:#c21714;
        text-align: right;
        height: 22px;
        width: 161px;
    }
    .style5
    {
        height: 22px;
    }
    .style6
    {
        font-size: large;
         color:#ffffff;
    }
    .style7
    {
        width: 161px;
    }
        .style8
        {
            height: 27px;
        }
        .style9
        {
            font-family: Cambria;
            font-size:25px;
        }
        .style10
        {
            font-size: large;
        }
        .style11
        {
            font-family: Cambria;
            font-size:18px;
        }
 </style>
</head>
<body class="loginbackground">
<script>
    function valid() {
        var txtUserName = $("#txtUserName").val();
        var txtPassword = $("#txtPassword").val();
        validResult = 0;
        message = "";

        var hasUpperCase = /[A-Z]/;
        var hasLowerCase = /[a-z]/;
        var hasNumbers = /\d/;
        var hasNonalphas = /\W/;

        if (txtUserName.length <= 0) {
            validResult = validResult + 1;
            message = message + "User Name is required.\n";
        }

        if (txtPassword.length <= 0) {
            validResult = validResult + 1;
            message = message + "Password is required.\n";
        }
//        if (UserPassword.length <= 0) {
//            $("#password").addClass("controlnotvalid");
//            requiredvalue = requiredvalue + 1;
//        }
//        else if (txtPassword.length < 8) {
//            validmessage = validmessage + "Minimum length of password should be 8. ";
//            requiredvalue = requiredvalue + 1;
//          
//        }
//        else if (!hasUpperCase.test(txtPassword)) {
//            validmessage = validmessage + "Password must contain atleast one upper case ";
//            requiredvalue = requiredvalue + 1;
//            $("#password").addClass("controlnotvalid");
//        }
//        else if (!hasLowerCase.test(txtPassword)) {
//            validmessage = validmessage + "Password must contain atleast one lower case ";
//            requiredvalue = requiredvalue + 1;
//            $("#password").addClass("controlnotvalid");
//        }
//        else if (!hasNumbers.test(txtPassword)) {
//            validmessage = validmessage + "Password must contain atleast one number ";
//            requiredvalue = requiredvalue + 1;
//            $("#password").addClass("controlnotvalid");
//        }
//        else if (!hasNonalphas.test(UserPassword)) {
//            validmessage = validmessage + "Password must contain atleast one special character ";
//            requiredvalue = requiredvalue + 1;
//            $("#password").addClass("controlnotvalid");
//        }

        console.log(validResult, message);
        if (validResult > 0) {
            $("#lblLoginStatus").text(message);
            return false;
        }

}
</script>
    <form id="form1" runat="server" defaultfocus="txtUserName">
    <table width="100%" style="height: 100%;" border="0">
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
            </td>
        </tr>
        <tr>
            <td>
                &nbsp;
                <br />
                <br />
                <br />
                <br />
            </td>
        </tr>
        <tr>
            <td align="center">
                <table cellpadding="0" cellspacing="0" border="0" class="login_tbl_txt" bgcolor="#F2F3F5">
                    <tr bgcolor="#ffffff">
                        <td align="center" >
                            <img id="AphImage" alt="" src="images/ASPL_LOGO.png" style="height:70px; width:100px; margin-left:5px; margin-top:2px; margin-bottom:2px;"/>
                        </td>
                        <td align="center" class="style3">
                            <span class="style9" style="color: #354c8a"><strong>Welcome to APHELION FINANCE PVT LTD</strong></span>
                            <%-- <span class="style11"><strong>(Unit of Rajeshwari Fincom Limited)</strong></span>--%>
                        </td>
                    </tr>
                    <%--<tr>
                        <td colspan="2" align="center" class="style8" style="background-color: #156fae;">
                            <span class="style9"><strong><span class="style6">Member Login</span></strong></span>
                        </td>
                    </tr>--%>
                    <tr>
                        <td align="left" valign="top" style="background-color: #156fae; text-align: justify;"
                            class="style11" colspan="2">
                            <table border="0" class="login_tbl_sub" align="center">
                                <tr>
                                    <td class="style7">
                                        &nbsp;
                                    </td>
                                    <td class="style16">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style4">
                                        <strong>Branch : </strong>
                                    </td>
                                    <td class="style5">
                                        <asp:DropDownList ID="ddlBranch" class="ddlStyle" Style="width: 254px; height: 27px;"
                                            runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style4">
                                        <strong>Financial Year : </strong>
                                    </td>
                                    <td class="style5">
                                        <asp:DropDownList ID="ddlFYear" class="ddlStyle" Style="width: 254px; height: 27px;"
                                            runat="server">
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style4">
                                        <strong>User Name : </strong>
                                    </td>
                                    <td class="style5">
                                        <asp:TextBox ID="txtUserName" class="Username" runat="server" MaxLength="20"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style4">
                                        <strong>Password :</strong>
                                    </td>
                                    <td class="style5">
                                        <asp:TextBox ID="txtPassword" class="Password" runat="server" TextMode="Password"
                                            MaxLength="20"></asp:TextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="style7">
                                    </td>
                                    <td class="style12" align="right">
                                        <a href="#">Forget your password ?</a>
                                        <asp:Button ID="butSignIn" runat="server" Text="Sign in" CssClass="button" OnClientClick="return valid();" OnClick="butSignIn_Click" />
                                        &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        &nbsp;
                                    </td>
                                </tr>
                                <tr>
                                    <td align="center" class="style10" colspan="2" style="color: #cf010a; font-family: Cambria;
                                        font-size: 14px;">
                                        <asp:Label ID="lblLoginStatus" runat="server" Text="Login"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="2" style="background-color: #156fae; color: #ffffff;"
                            class="style8">
                        </td>
                    </tr>
                    <%-- <tr>
                        <td align="center" class="style10" colspan="2" style="background-color:#156fae; color: #ff0101; font-family: Cambria; font-size: 14px;">
                            <asp:Label ID="lblLoginStatus" runat="server" Text="Login"></asp:Label>
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="style10" colspan="2" style="background-color: #156fae;">
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="background-color: #ffffff; color: #000000; text-align: left;"
                            class="style8">
                            <cite style="background-color: #ffffff; color: #0000ff; font-family: Arial; text-align: left;
                                font-size: 11px; font-weight:bold; width: 25%">Copyright &#169; 2014 Aphelion Software Pvt. Ltd.</cite>
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Please enter your details of the administration dashboard.
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <br />
                <br />
                <br />
                <br />
                <br />
                <br />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
