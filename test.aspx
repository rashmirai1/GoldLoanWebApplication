<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  
   <script src="https://code.jquery.com/jquery-3.1.0.js" integrity="sha256-slogkvB1K3VOkzAI8QITxV3VzpOnkeNVsKvtkYLMjfk=" crossorigin="anonymous"></script>
       <link href="Content/assets/css/styles.css" rel="stylesheet" />
    <link href="Content/assets/fonts/font-awesome/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Content/assets/plugins/jstree/dist/themes/avenger/style.min.css" rel="stylesheet" />
    <link href="Content/assets/plugins/codeprettifier/prettify.css" rel="stylesheet" />
    <link href="Content/assets/plugins/iCheck/skins/minimal/blue.css" rel="stylesheet" />
    <link href="Content/assets/plugins/form-daterangepicker/daterangepicker-bs3.css" rel="stylesheet" />
    <link href="Content/assets/plugins/fullcalendar/fullcalendar.css" rel="stylesheet" />
    <script src="Content/assets/js/includer.js"></script>      

   
<style>
.make-scrolling {
  overflow-y: scroll;
  height: 150px;
}
</style>
</head>
<body>
    <form id="form1" runat="server">
 <button type="button" data-toggle="modal" data-target="#PopupModel">Click</button>

     <div class="modal fade bs-example-modal-lg" id="PopupModel" role="dialog">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header modal-header-primary">
                    <button type="button" id="btnclose" class="close" data-dismiss="modal" aria-hidden="true">×</button>
                    <table class="table tablenothover">
                        <tbody>
                            <tr>
                                <td class="col-md-1">
                                    <span>Search By :</span>
                                </td>
                                <td class="col-md-3">
                                    <select class="form-control" id="Select1" onchange="CleartxtSearch();">
                                       <option value="1">State ID</option>
                                        <option value="2">State</option>
                                        <option value="3">Country</option>
                                    </select>
                                </td>
                                <td class="col-md-1">
                                    <span>Search Text :</span>
                                </td>
                                <td class="col-md-3">
                                    <input type="text" class="form-control" id="txtSearch" onkeypress = "return alphanumeric_only(event);"  >
                                </td>
                                <td class="col-md-1" id="Td1">
                                    <input type="button" class="btn btn-default" id="Button3" value="Search" >
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>
                <div class="modal-body" style="height:450px; overflow:auto;">
                    <div class="panel-body panel-no-padding">
                        <div id="demo">
                            <table cellpadding="0" cellspacing="0" border="0" class="table table-striped table-fixed-header m0 table-bordered table-hover" id="example">
                                <thead id="tblheader">

                                </thead>
                                <tbody id="tblcontent"></tbody>
                            </table>
                        </div>
                    </div>
                </div>
                <!--<div class="modal-footer">
                  <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
                  <button type="button" class="btn btn-primary">Save changes</button>
                </div>-->
            </div><!-- /.modal-content -->
        </div><!-- /.modal-dialog -->
    </div>


   


<select class="ajax-combo">
  <option>1</option>
  <option>2</option>
  <option>3</option>
  <option>1</option>
  <option>2</option>
  <option>3</option>
  <option>1</option>
  <option>2</option>
  <option>3</option>
  <option>1</option>
  <option>2</option>
  <option>3</option>
</select>
    </form>
     <script>
    $(".ajax-combo").selectmenu({ "width": "100px",});
 
$(".ajax-combo").selectmenu("menuWidget").addClass("make-scrolling");
$(".ajax-combo").selectmenu("menuWidget").scroll(function(e) {
  if (e.currentTarget.scrollHeight - 10 < e.currentTarget.scrollTop + $(e.currentTarget).height()) {
    var curTar = e.currentTarget;
    var lastTop = curTar.scrollTop;
    $.getJSON("http://echo.jsontest.com/10/test/20/rest/30/best/40/vest/50/fest", function(data) {
      $.each(data, function(key, val) {
        $(".ajax-combo").append("<option value='" + key + "'>" + val + "</option>");
      });
      $(".ajax-combo").selectmenu("refresh");
      curTar.scrollTop = lastTop;
    });
  }
});
</script>

    <script src="https://ajax.googleapis.com/ajax/libs/jquery/2.1.1/jquery.min.js"></script>
<link rel="stylesheet" href="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/themes/smoothness/jquery-ui.css">
<script src="https://ajax.googleapis.com/ajax/libs/jqueryui/1.11.4/jquery-ui.min.js"></script>

  <%--    <script src="Content/assets/js/enquire.min.js"></script>--%>
            <!--<script src="Content/assets/js/jquery-1.10.2.min.js"></script>-->
         <script src="Content/assets/js/bootstrap.min.js"></script>
            <%--   <script src="Content/assets/js/application.js"></script>
            <script src="Content/assets/js/jquery.layout.min.js"></script>
            <link href="Content/assets/js/jqueryui.css" rel="stylesheet" />--%>
</body>

</html>
