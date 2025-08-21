<div class="mainContainer bg-light">

    <section class="mx-md-3 mx-lg-4 mx-xl-5 bg-white boxShadow my-5 rounded">
        <div class="row align-items-center justify-content-between px-4 px-md-4 pt-3 pb-2">
            <div class="col-12 col-md-6 col-lg-5 col-xl-auto">
                <div class="row align-items-center justify-content-center justify-content-md-start">
                    <div class="col-auto">
                        <i class="icon-traslado h1 colorPrimary"></i>
                    </div>
                    <div class="col-auto my-3">
                        <h2 class="m-0 h3"><b>Catálogo de instituciones traslado</b></h2>
                    </div>
                </div>
            </div>
            <div class="col-12 col-md-6 col-lg-7 col-xl-auto">
                <div class="row">
                    <div class="col-auto btnOutline btnOutlinePrimary my-2 mx-auto mx-xl-2 p-0">
                        <button data-bs-toggle="modal" data-bs-target="#addInstitucionTraslado">
                            <h6 class="m-0 d-flex align-items-center"><i class="icon-addTraslado h5 mb-0 me-2"></i><b>Agregar nueva institución traslado</b></h6>
                        </button>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col-12">
                <div class="bg-light py-1"></div>
            </div>
        </div>
        <div class="row justify-content-between mb-1 titleCustom">
            <div class="col-12 col-md-auto mt-4 mb-2">
                <h5 class="px-4"><b>Listado de instituciones traslado</b></h5>
                <h6 class="px-4 text-muted">Edita o inactiva las instituciones de traslado.</h6>
            </div>
        </div>
        <div class="row">
            <div class="col-12 mb-4 px-4 rounded gridCustom">
                <kendo-grid name="grid" height="450">
                    <datasource type="DataSourceTagHelperType.Custom" custom-type="odata" page-size="20">
                        <transport>
                            <read url="https://demos.telerik.com/kendo-ui/service/Northwind.svc/Orders" />
                        </transport>

                    </datasource>
                    <sortable enabled="true" />

                    <pageable button-count="5" refresh="true" page-sizes="new int[] { 5, 10, 20 }">
                    </pageable>
                    <columns>
                        <column field="ShipName" title="Nombre" width="230" />
                        <column template="<button data-bs-toggle=modal data-bs-target=\#editInstitucionTraslado class='w-100 btn'><h6 class='m-0 colorPrimary'><i class='icon-edit me-2'></i><b>Editar</b></h6></button>" width="100" />
                    </columns>
                    <toolbar>
                        <toolbar-button name="search"></toolbar-button>
                    </toolbar>
                </kendo-grid>


            </div>

        </div>



    </section>





</div>
    </div>
<!-- Modal -->





<div class="modal fade modalCustom" id="editInstitucionTraslado"  aria-labelledby="editInstitucionTrasladoLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg boxShadow modal-dialog-centered">
        <div class="modal-content">

            <section class=" bg-white rounded">
                <div class="row align-items-center justify-content-between px-4 px-md-4 pt-3 pb-2">
                    <div class="col-auto pe-0">
                        <div class="row align-items-center justify-content-center justify-content-md-start">
                            <div class="col-auto pe-0">
                                <i class="icon-editTraslado h1 colorPrimary"></i>
                            </div>
                            <div class="col-auto my-3">
                                <h2 class="m-0 h3"><b>Editar institución de traslado</b></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="row">
                            <div class="col-auto my-3">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-12">
                        <div class="bg-light py-1"></div>
                    </div>

                </div>
                <div class="row align-items-end">
                    <div class="col-12 col-lg-6 my-4">
                        <h5 class="px-4"><b>Datos de institución de traslado</b></h5>
                        <h6 class="px-4 text-muted">Completa los datos obligatorios para guardar.</h6>
                    </div>
                    <div class="col-12 col-lg-6 my-4">
                        <div class="row justify-content-center">
                            <div class="btnToggle col-8">
                                <div style="z-index:0" class="position-relative d-flex align-items-center">
                                    <input class="toggle toggle-left" id="activo" name="toggle-state" value="false"
                                           type="radio" checked="" />
                                    <label class="btn" for="activo">Activo</label>
                                    <input class="toggle toggle-right" id="inactivo" name="toggle-state" value="true"
                                           type="radio" />
                                    <label class="btn" for="inactivo">Inactivo</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <form class="row px-4 mb-4 align-items-end">
                    <div class="col-12 col-md-6 col-lg-4 col-xl-3 col-xx-2">
                        <div class="controlForm my-3">
                            @(Html.Kendo().TextBox()
                                .Name("editTraslado")
                                .Label(l => l.Content("Nombre <b>(obligatorio)</b>:"))
                                .Placeholder("Ingresa el nombre de la institución de traslado")
                                .HtmlAttributes(new { style = "width: 100%" })
                                )

                        </div>
                    </div>


                </form>
                <div class="row my-4">
                    <div class="col-12 col-md-6 mx-md-auto">
                        <div class="row justify-content-around">
                            <div class="col-auto btnOutline my-2 mx-auto mx-xl-2 p-0">
                                <button type="button" data-bs-dismiss="modal" aria-label="Close">
                                    <h6 class="m-0 px-3"><b>Cerrar</b></h6>
                                </button>
                            </div>
                            <div class="col-auto btnOutline btnOutlinePrimary my-2 mx-auto mx-xl-2 p-0">
                                <div class="controlButton">
                                    @(Html.Kendo().Button()
                                        .Name("EditTraslado")
                                        .HtmlAttributes(new { @class = "btnPrimary px-3" })
                                        .Content("<h5 class=\"m-0\"><b>Guardar ajustes</b></h5>"))
                                </div>
                            </div>
                        </div>
                    </div>


                </div>
            </section>
        </div>
    </div>
</div>



<div class="modal fade modalCustom" id="addInstitucionTraslado"  aria-labelledby="addInstitucionTrasladoLabel" aria-hidden="true">
    <div class="modal-dialog modal-lg boxShadow modal-dialog-centered">
        <div class="modal-content">

            <section class=" bg-white rounded">
                <div class="row align-items-center justify-content-between px-4 px-md-4 pt-3 pb-2">
                    <div class="col-auto pe-0">
                        <div class="row align-items-center justify-content-center justify-content-md-start">
                            <div class="col-auto pe-0">
                                <i class="icon-addTraslado h1 colorPrimary"></i>
                            </div>
                            <div class="col-auto my-3">
                                <h2 class="m-0 h3"><b>Agregar institución de traslado</b></h2>
                            </div>
                        </div>
                    </div>
                    <div class="col-auto">
                        <div class="row">
                            <div class="col-auto my-3">
                                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="row">
                    <div class="col-12">
                        <div class="bg-light py-1"></div>
                    </div>

                </div>
                <div class="row">
                    <div class="col-12 col-lg-6 my-4">
                        <h5 class="px-4"><b>Datos de institución de traslado</b></h5>
                        <h6 class="px-4 text-muted">Completa los datos obligatorios para guardar.</h6>
                    </div>
                </div>
                <form class="row px-4 mb-4 align-items-end">
                    <div class="col-12 col-md-6 col-lg-4 col-xl-3 col-xx-2">
                        <div class="controlForm my-3">
                            @(Html.Kendo().TextBox()
                                .Name("AddTraslado")
                                .Label(l => l.Content("Institución de traslado <b>(obligatorio)</b>:"))
                                .Placeholder("Ingresa el nombre de la institución de traslado")
                                .HtmlAttributes(new { style = "width: 100%" })
                                )

                        </div>
                    </div>

                </form>
                <div class="row my-4">
                    <div class="col-12 col-md-6 mx-md-auto">
                        <div class="row justify-content-around">
                            <div class="col-auto btnOutline my-2 mx-auto mx-xl-2 p-0">
                                <button type="button" data-bs-dismiss="modal" aria-label="Close">
                                    <h6 class="m-0 px-3"><b>Cerrar</b></h6>
                                </button>
                            </div>
                            <div class="col-auto btnOutline btnOutlinePrimary my-2 mx-auto mx-xl-2 p-0">
                                <div class="controlButton">
                                    @(Html.Kendo().Button()
                                        .Name("AddTraslado")
                                        .HtmlAttributes(new { @class = "btnPrimary px-3" })
                                        .Content("<h5 class=\"m-0\"><b>Guardar</b></h5>"))
                                </div>
                            </div>
                        </div>
                    </div>


                </div>
            </section>
        </div>
    </div>
</div>


</main>
