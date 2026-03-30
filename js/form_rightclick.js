/* disable right click */
$(function () {
    $(this).bind("contextmenu", function (e) {
        e.preventDefault();
    });
});


/* disable Ctrl+V check 118 and 86 ('V' and 'v') */
$(document).ready(function () {
    $(document).keydown(function (event) {
        if (event.ctrlKey == true && (event.which == '118' || event.which == '86')) {
            //alert('thou. shall. not. PASTE!');
            event.preventDefault();
        }
    });
});
