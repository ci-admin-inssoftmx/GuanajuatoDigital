


export function updateFolio(id,data, Succes, Fail) {


    console.log("entra pet")
    $.ajax({
        url: './UpdateFolio',
        type: 'POST',
        data: {id:id,folio:data},
        success: Succes || function (result) {
            console.log(result);
        },
        error: Fail || function (result) {
            console.log(result);
        }
    });

}



export function GetData(data, Succes, Fail) {
    //showLoading();
    $.ajax({
        url: './GetDataBusquedaEspecial',
        type: 'POST',
        data: data,
        success: Succes || function (result) {
            console.log(result);
            hideLoading();
        },
        error: Fail || function (result) {
            console.log(result);
            hideLoading();
        }
    });


}





export function getDataBit(data, Succes, Fail) {
    //showLoading();
    $.ajax({
        url: './GetDataBusquedaEspecialBit',
        type: 'POST',
        data: data,
        success: Succes || function (result) {
            console.log(result);
            hideLoading();
        },
        error: Fail || function (result) {
            console.log(result);
            hideLoading();
        }
    });


}




export function RemoveData(data, Succes, Fail) {
    showLoading();
    $.ajax({
        url: './RemoveData',
        type: 'POST',
        data: data,
        success: Succes || function (result) {
            console.log(result);
            hideLoading();
        },
        error: Fail || function (result) {
            console.log(result);
            hideLoading();
        }
    });


}