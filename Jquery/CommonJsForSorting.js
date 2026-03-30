/*Added by Priya For sorting grid */

$(document).ready(function () {
    $("#gvGlobal").tablesorter();
    // ({
    //    headerss: {
    //       // assign the third column (we start counting zero) 
    //       2: {
    //           // disable it by setting the property sorter to false 
    //           sorter: false
    //      }
    //  }
    // });
    $("#gvGlobal tbody").before("<thead><tr></tr></thead>");
    // $("#gvGlobal thead tr").append($("#gvGlobal th"));
    // $("#gvGlobal tr:first").remove();

    //for loader
    // $("#gvGlobal").bind("sortStart", function () {
    //      document.getElementById('spinner').style.display = 'block'
    //      $("spinner").show();
    //  }).bind("sortEnd", function () {
    //       document.getElementById('spinner').style.display = 'none'
    //       $("spinner").hide();
    //  });


    SetDefaultSortOrder();

    function Sort(cell, sortOrder) {
        var sorting = [[cell.cellIndex, sortOrder]];
        // document.getElementById('spinner').style.display = 'block';
        //  $('spinner').show();
        $("#gvGlobal").trigger("sorton", [sorting]);
        if (sortOrder == 0) {
            sortOrder = 1;
            cell.className = "sortDesc";
        }
        else {
            sortOrder = 0;
            cell.className = "sortAsc";
        }
        cell.setAttribute("onclick", "Sort(this, " + sortOrder + ")");
        cell.onclick = function () {
            // document.getElementById('spinner').style.display = 'block';
            //  $('spinner').show();
            Sort(this, sortOrder);
        };
        document.getElementById("container").scrollTop = 0;
        //    document.getElementById('spinner').style.display = 'none';
        //   $("spinner").hide();
    }

    function SetDefaultSortOrder() {
        //var gvHeader = document.getElementById("#gvGlobal");
        //var headerss = gvHeader.getElementsByTagName("TH");
        //for (var i = 0; i < headerss.length; i++) {
        //    headerss[i].setAttribute("onclick", "Sort(this, 1)");
        //    headerss[i].onclick = function () { Sort(this, 1); };
        //    headerss[i].className = "sortDesc";
        //}
        // document.getElementById('spinner').style.display = 'none';
        // $("spinner").hide();
    }
});