jQuery(document).ready(function() {
    $('.btnAddDevice').on("click", function (e) {
        e.preventDefault();
        var longitude = $('#longitude').val();
        var latitude = $('#latitude').val();
        $.post("device/create/?longitude=" + longitude + "&latitude=" + latitude, function (data) {
            $('#deviceIdWrapper').show();
            $('#deviceId').text(data);
            $('#longitude').val("");
            $('#latitude').val("");
        });
    });
});

var getUrlParameter = function getUrlParameter(sParam) {
    var sPageURL = decodeURIComponent(window.location.search.substring(1)),
        sURLVariables = sPageURL.split('&'),
        sParameterName,
        i;

    for (i = 0; i < sURLVariables.length; i++) {
        sParameterName = sURLVariables[i].split('=');

        if (sParameterName[0] === sParam) {
            return sParameterName[1] === undefined ? true : sParameterName[1];
        }
    }
};