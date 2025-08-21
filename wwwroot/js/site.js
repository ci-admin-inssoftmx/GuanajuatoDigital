///AjaxPeticions

/**
 * Queries a Baz for items.
 * @param {string} url url a la que apunta.
 * @param {object|string|null} data data entry
 * @param {function} succes callback succes
 * @param {function} error callback error
 */
function SimpleAjaxPeticion(url,data,succes,error) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data ?? {},
        success: (succes ?? function(e){console.log(e)}) ,
        error:( error?? function () {console.log(e)})
    });

}





// Write your Javascript code.
function hideLoading() {
    $('#loading').css('display', 'none');
}

function showLoading() {
    $('#loading').css('display', 'block');
}

var isValidPhone = function (phone) {
    var regex = /^[0-9]+$/;
    return regex.test(phone);
};

var isValidEmail = function (email) {
    var MAX_EMAIL_LENGTH = 254;

    if (email.length > MAX_EMAIL_LENGTH) {
        return false;
    }

    var regex = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;

    return regex.test(email);
};

function validarCorreo(event) {
    // Obtener el valor actual del campo de entrada
    var correo = event.target.value;

    // Expresión regular para validar el formato del correo electrónico
    var regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    // Validar el formato del correo electrónico
    if (!regex.test(correo)) {
        // Si el formato no es válido, mostrar un alert con el mensaje de error
        sitteg_warning('Dirección de correo electrónico inválida.');

    }
}

function isControlsValid(controlsValidate) {
    var isValid = true;
    var isFirst = false;

    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        element.removeClass("errorData");
        if (element.val() === '' || element.val() === undefined) {
            element.addClass("errorData");
            if (!isFirst) {
                //element.focus();
            }
            isValid = false;
            isFirst = true;
        }
    });

    return isValid;
}

function isControlsValidDropDown(controlsValidate) {
    var isValid = true;
    var isFirst = false;

    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        element.closest('.k-dropdown').removeClass("errorData");
        console.log(x.controlName, element.val())
        if (element.val() === '' || element.val() === undefined) {
            element.closest('.k-dropdown').addClass("errorData");
            if (!isFirst) {
                //element.focus();
            }
            isValid = false;
            isFirst = true;
        }
    });

    return isValid;
}

/**
 * Valida controles si son requeridos y coloca
 * el foco en el primer control con error
 * @param {*} controlsValidate 
 * @returns 
 */
function isControlsValidWithFocus(controlsValidate, withFocus = true) {
    var isValid = true;
    var firstElementWithError;
    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        var validElement = true;
        var validators = x.validators ?? ['required'];
        //Si el campo es un input remueve el estilo de error
        if (x.isInput)
            element.removeClass("errorData");
        //Si el campo es tipo dropdown se remueve el estilo de error
        if (x.isDropDown)
            element.closest('.k-dropdown').removeClass("errorData");

        if (validators.includes('required')) {
            if (element.val() === '' || element.val() === undefined) {
                isValid = false;
                validElement = false;
            }
        }
        if (validators.includes('phoneValidator') && element.val() != '' && element.val() != undefined) {
            if (!isValidPhone(element.val())) {
                isValid = false;
                validElement = false;
            }
        }
        if (validators.includes('emailValidator') && element.val() != '' && element.val() != undefined) {
            if (!isValidEmail(element.val())) {
                isValid = false;
                validElement = false;

            }
        }
        if (!validElement) {
            //Se agrega el estilo de error en caso no se haya encontrado un valor
            if (x.isInput)
                element.addClass("errorData");
            if (x.isDropDown)
                element.closest('.k-dropdown').addClass("errorData");
            if (!firstElementWithError)
                firstElementWithError = x;
        }
    });

    //En caso de existir un componente requerido con error y se debe establecer el foco
    if (firstElementWithError && withFocus) {
        //Si el control es de tipo input
        if (firstElementWithError.isInput)
            $('#' + firstElementWithError.controlName).focus();
        //Si el control es de tipo dropdown se establece el foco y se expande la lista
        if (firstElementWithError.isDropDown)
            $('#' + firstElementWithError.controlName).focus().click();

    }

    return { isValid, firstElementWithError };
}

function addOnLostFocusRequiredControls(controlsValidate) {
    controlsValidate.forEach(x => {
        var element = $('#' + x.controlName);
        var validators = x.validators ?? ['required'];
        element.on('focusout', () => {
            var isValid = true;

            if (validators.includes('required')) {
                //Si el control no tiene valor se agrega el estilo de error
                if (element.val() === '' || element.val() === undefined) {
                    isValid = false;
                }
            }
            if (validators.includes('phoneValidator') && element.val() != '' && element.val() != undefined) {
                if (!isValidPhone(element.val())) {
                    isValid = false;
                }
            }
            if (validators.includes('emailValidator') && element.val() != '' && element.val() != undefined) {
                if (!isValidEmail(element.val())) {
                    isValid = false;
                }
            }
            if (!isValid) {
                if (x.isInput)
                    element.addClass("errorData");
                if (x.isDropDown)
                    element.closest('.k-dropdown').addClass("errorData");
            }
            else {
                //Si el campo es un input remueve el estilo de error
                if (x.isInput)
                    element.removeClass("errorData");
                //Si el campo es tipo dropdown se remueve el estilo de error
                if (x.isDropDown)
                    element.closest('.k-dropdown').removeClass("errorData");
            }
        })
    });
}

$(document).ready(function () {
    replaceLegend();
    $(".navbar-nav li").on("click", function () {
        var dataId = $(this).attr("data-id");
        localStorage.setItem("menuId", dataId);
        //alert("Id del men�: " + dataId);
    });
});


function errorMessage(option, message) {
    switch (option) {
        case 1:
            toastr.error(messages.serverError);
            break;
        case 2:
            toastr.error(messages.formError);
            break;
        case 3:
            toastr.error(message);
            break;
        default:
            toastr.error(message ? message : messages.formError);
            break;
    }
}


function ValidarKm(event) {
    var keycode = event.keyCode//135
    var key = event.key//3
    var Km = event.target.value;//025
    var arrkm = Km.split('')//["0","2","5"]
    var regexp = /^[0-9]{0,5}([\.]{1}[0-9]{0,3})?$/;

    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }

    if (!regexp.test(Km+key)) {
        event.preventDefault()
        return
    }

    var countDots = arrkm.reduce((acc, val) => {
        acc = val === "." ? parseInt(acc) + 1 : parseInt(acc);
        return acc;
    }, 0)//1

    if (key == "." && countDots >= 1) { event.preventDefault(); return;   }

    if (arrkm[0] == ".") {
        event.target.value = "0" + event.target.value
        if (key == ".") {
            return;
        }
    }
    if (arrkm[0] == "0" && arrkm[1] != "." && arrkm[1] != undefined) {
        event.target.value = event.target.value.slice(1)
    }
    
    if ("0123456789.".indexOf(key) != -1) {
     return
    }
    event.preventDefault();
}


function ValidarMonto(event) {
    var keycode = event.keyCode//135
    var key = event.key//3
    var Km = event.target.value;//025
    var arrkm = Km.split('')//["0","2","5"]
    var regexp = /^[0-9]{0,7}([\.]{1}[0-9]{0,3})?$/;

    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }

    if (!regexp.test(Km+key)) {
        event.preventDefault()
        return
    }

    var countDots = arrkm.reduce((acc, val) => {
        acc = val === "." ? parseInt(acc) + 1 : parseInt(acc);
        return acc;
    }, 0)//1

    if (key == "." && countDots >= 1) { event.preventDefault(); return;   }

    if (arrkm[0] == ".") {
        event.target.value = "0" + event.target.value
        if (key == ".") {
            return;
        }
    }
    if (arrkm[0] == "0" && arrkm[1] != "." && arrkm[1] != undefined) {
        event.target.value = event.target.value.slice(1)
    }
    
    if ("0123456789.".indexOf(key) != -1) {
     return
    }
    event.preventDefault();
}


function ValidarEnteros(event) {
    var keycode = event.keyCode
    var key = event.key
    var Km = event.target.value;
    var arrkm = Km.slice('')
    console.log(arrkm[0])

    if (arrkm[0] == ".") {
        event.target.value = "0" + event.target.value
    }
    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }
    if ("0123456789".indexOf(key) != -1) {
        return
    }
    event.preventDefault();
}




function replaceLegend() {
    setTimeout(() => {
        // Cambiar el texto del último elemento del dropdown
        var dropdowns = document.querySelectorAll(".k-pager-numbers-wrap select.k-dropdown");
        dropdowns.forEach(dropdown => {
            var options = dropdown.querySelectorAll("option");
            var lastOption = options[options.length - 1];
            if (lastOption.textContent.trim() === "More pages") {
                lastOption.textContent = "Más páginas";
            }


            var firstOption = options[0];
            if (firstOption.textContent.trim() === "More pages") {
                firstOption.textContent = "Más páginas";
            }

        });

        var at = document.querySelectorAll("span.k-pager-info.k-label");
        var aps = document.querySelectorAll("span.k-pager-sizes.k-label");

        for (var t = 0; t <= at.length - 1; t++) {
            if (at[t].innerHTML == "No items to display") {
                at[t].innerHTML = at[t].innerHTML.replace('No items to display', 'No hay registros a desplegar');
            }

            if (at[t].innerHTML.toString().includes("items"))
                at[t].innerHTML = at[t].innerHTML.replace('item', 'registro');
            if (at[t].innerHTML.toString().includes("of"))
                at[t].innerHTML = at[t].innerHTML.replace("of", "de");
        }

        for (var ps = 0; ps <= aps.length - 1; ps++) {
            if (aps[ps] != null) {
                if (aps[ps].innerHTML.toString().includes("items per page"))
                    aps[ps].innerHTML = aps[ps].innerHTML.replace('items per page', 'Registros por página');
            }
        }
    }, 500);
}



function validaRfc(e) {
    console.log("here rfc")
    var value = e.target.value
    var key = e.key
    var keycode = e.keyCode
    var rfc = value + key
    var fakercf = "aaaa111111524"
    var pattern = /^[A-Z]{4}[0-9]{6}[0-9 A-Z]{3}$/

    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }
    var ajustrfc = rfc + (fakercf.slice(rfc.length))
    console.log(ajustrfc)

    if (pattern.test(ajustrfc.toUpperCase())) {
        e.key = e.key.toUpperCase()
        return
    }

    

    e.preventDefault()

}

function validaRfcMoral(e) {
    console.log("here rfc MORAL")
    var value = e.target.value
    var key = e.key
    var keycode = e.keyCode
    var rfc = value + key
    var fakercf = "aaa111111524"
    var pattern = /^[A-Z]{3}[A-Z 0-9]{1}[0-9]{5,6}[0-9 A-Z]{0,3}$/;

    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }
    var ajustrfc = rfc + (fakercf.slice(rfc.length))
    console.log(ajustrfc)

    if (pattern.test(ajustrfc.toUpperCase())) {
        e.key = e.key.toUpperCase()
        return
    }



    e.preventDefault()

}



function validaCurp(e) {
    var curp = e.target.value.trim();

    curp = curp.toUpperCase();

    var key = e.key;

    if (key === "Backspace" || key === "Delete") {
        return true;
    }

    if (curp.length >= 18) {
        console.log("La CURP debe tener exactamente 18 caracteres.");
        e.preventDefault(); 
        return false;
    }

    // Validar los primeros 4 caracteres como letras
    if (curp.length < 4) {
        if (!/^[A-Z]{1}$/i.test(key)) {
            console.log("El primer caracter debe ser una letra.");
            e.preventDefault(); 
            return false;
        }
    }

    // Validar los siguientes 6 caracteres como números
    if (curp.length >= 4 && curp.length < 10) {
        if (!/^[0-9]{1}$/i.test(key)) {
            console.log("Los siguientes 6 caracteres deben ser números.");
            e.preventDefault();
            return false;
        }
    }

    // Validar el carácter en la posición 11 (género)
    if (curp.length === 10) {
        if (!/^[HM]$/i.test(key)) {
            console.log("El carácter en la posición 11 debe ser 'H' o 'M'.");
            e.preventDefault();
            return false;
        }
    }

    // Validar los siguientes 2 caracteres como letras (estado de nacimiento)
    if (curp.length >= 11 && curp.length < 13) {
        if (!/^[A-Z]{1}$/i.test(key)) {
            console.log("Los siguientes 2 caracteres deben ser letras.");
            e.preventDefault(); // Evitar la escritura si el último caracter no es una letra
            return false;
        }
    }

    if (curp.length >= 13 && curp.length < 16) {
        if (!/^[A-Z0-9]{1}$/i.test(key)) {
            e.preventDefault();
            return false;
        }
    }

    if (curp.length === 16) {
        if (!/^[0-9]{1}$/i.test(key)) {
            e.preventDefault();
            return false;
        }
    }

    return true;
}


function validaTelefono(e) {
    var key = e.key;
    var pattern = /^[0-9]$/;
    var keycode = e.keyCode

    if ([8, 46, 37, 38, 39, 40, 9].indexOf(keycode) != -1) {
        return
    }

    var excludedKeys = [8, 46, 37, 38, 39, 40, 9]; 

    if (excludedKeys.includes(e.keyCode) || !pattern.test(key)) {
        e.preventDefault();
        return;
    }

    if ((e.target.value + key).length > 20) {
        e.preventDefault();
        return;
    }

    e.key = key.toUpperCase();
}

function validarNumero(event) {
    var charCode = (event.which) ? event.which : event.keyCode;

    if (charCode === 46) {
        mostrarError("No se permite ingresar puntos.");
        event.preventDefault();
        return false;
    }

    if ((charCode > 31 && (charCode < 48 || charCode > 57)   )  && charCode !== 8) {
        mostrarError("Solo se permiten números.");
        event.preventDefault();
        return false;
    }

    if ((event.target.value + event.key).length > 20) {
        mostrarError("Solo se permiten 20 caracteres.");
        event.preventDefault();
        return false;
    }

    return true;
}



function mostrarError(mensaje) {
    $("#errorTelefono").text(mensaje);
    $("#errorTelefonoFisico").text(mensaje);
}

function ocultarError() {
    $("#errorTelefono").text("");
    $("#errorTelefonoFisico").text("");

}

function ValidPaste() {
    console.log("Entro")
}





