function _set_combo_catalog(catalog, parameter, htmlName, isRequired, container, functionName, multiple = "false") {
    showLoading();
    $.ajax({
        type: "POST",
        url: '/GenericComponents/GetComboByCatalog',
        dataType: "html",
        data: {
            "htmlName": htmlName,
            "isRequired": isRequired,
            "catalog": catalog + ",",
            "parameter": parameter + ",",
            "functionName": functionName,
            "multiple": multiple
        },
        success: function (data) {
            $("#" + container).html(data);
            hideLoading();
        }, error: function () {
            sitteg_warning("Ocurrio un error al procesar su solicitud.");
            hideLoading();
        }
    });
}


function sitteg_warning(msg) {
    set_toastr_options(2000);
    toastr.warning(msg, 'Alerta');
}

function sitteg_info(msg) {
    set_toastr_options(2500);
    toastr.info(msg, "Información");
}

function sitteg_success(msg) {
    set_toastr_options(2500);
    toastr.success(msg, "Alerta");
}
function sitteg_error(msg) {
    set_toastr_options(2500);
    toastr.error(msg, "Alerta");
}


function set_toastr_options(timeout) {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "progressBar": false,
        "positionClass": "toast-top-right",
        "onclick": null,
        "timeOut": timeout,
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
}
function convertDate(inputFormat) {
    function pad(s) { return (s < 10) ? '0' + s : s; }
    var d = new Date(inputFormat)

    var date = [pad(d.getDate()), pad(d.getMonth() + 1), (d.getFullYear() == '1' ? '0001' : d.getFullYear())].join('/')

    date = date == "01/01/0001" ? "" : date
    date = date == "NaN/NaN/NaN" ? "" : date;
    return date; 
}

function Base64ToBytes(base64) {
    var s = window.atob(base64);
    var bytes = new Uint8Array(s.length);
    for (var i = 0; i < s.length; i++) {
        bytes[i] = s.charCodeAt(i);
    }
    return bytes;
}