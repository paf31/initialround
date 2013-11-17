function cookie(name) {
    var regex = /^\s+|\s+$/g;
    var cookies = document.cookie.split(";");
    for (var i = 0; i < cookies.length; i++) {
        var x = cookies[i].substr(0, cookies[i].indexOf("="));
        var y = cookies[i].substr(cookies[i].indexOf("=") + 1);
        x = x.replace(regex, "");
        if (x == name) {
            return unescape(y);
        }
    }
}