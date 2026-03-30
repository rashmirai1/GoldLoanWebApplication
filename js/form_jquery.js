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