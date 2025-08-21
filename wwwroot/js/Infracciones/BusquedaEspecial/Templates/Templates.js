export function Loading() {
    return `
                <div class="d-flex h-25 w-100 justify-content-center align-items-center">

                <style>
                    .Loader {
                        display:flex;
                        flex-direction:column;
                        justify-content:center;
                        align-items:center;
                        justify-items:center;
                        width:100px;
                    }

                    .Loading {

                        border:solid 2px gray;
                        border-radius:50%;
                        height:50px;
                        width:50px;
                        align-self:center;
                        border-top-color:transparent;
                        border-bottom-color: transparent;
                        
                        animation: linear slidein 1s infinite;
                    }

                    @keyframes slidein {
                      from {
                        transform: rotate(0deg);
                      }

                      to {
                        transform: rotate(360deg);
                      }
                    }



                </style>
                <div class="Loader">
                    <div class="Loading">

                    </div>
                    <h4>Cargando</h4>

                </div>


            </div>

    `
}