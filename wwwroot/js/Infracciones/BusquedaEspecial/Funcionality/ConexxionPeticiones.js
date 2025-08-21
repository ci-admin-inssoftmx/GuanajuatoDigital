import { Loading } from '../Templates/Templates.js'
import { GetData, RemoveData,getDataBit,updateFolio } from '../Peticiones/PeticionDatosGrid.js'



export function FolioUpdate(id,data, callback) {
    console.log("peticion")
    updateFolio(id,data,callback)
}

export function GetDataGrid(t,callback) {
    GetData(t, callback)   
}


export function getListBit(t, callback) {
    getDataBit(t,callback)
}

export function AddLoading(id) {


    $("#"+id).empty().append(Loading())

} 



export function RemoveTramite(d, callback) {
    RemoveData(d,callback)
}


