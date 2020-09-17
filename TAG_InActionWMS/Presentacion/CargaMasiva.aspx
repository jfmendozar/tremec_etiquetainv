<%@ Page Language="C#" MasterPageFile="~/Presentacion/Principal.Master" AutoEventWireup="true" CodeBehind="CargaMasiva.aspx.cs" Inherits="TREMEC_EtiquetaInventario_WS.Presentacion.CargaMasiva" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#miModal, #Modal4').modal({
                backdrop: 'static',
                keyboard: false,
                show: false
            });
            $('#miModal').on('show.bs.modal', function (event) {
                var button = $(event.relatedTarget)
                var recipient = button.data('whatever')
                var modal = $(this)
            });
            $(document).ready(function () {
                $('#btnColumnasH').tooltip();
            });
        });

        function onClientProgressBarUpdating(progressArea, args) {
            progressArea.updateHorizontalProgressBar(args.get_progressBarElement(), args.get_progressValue());
            args.set_cancel(true);
        }

        function ShowModal(sender, args) {
            var fileName = args.get_fileName();
            if (fileName != null) {
                $("#Modal4").modal('show');
            }
        }

        function RauArchivos_ClientFileSelected(sender, args) {
            ShowModal(sender, args);
        }

        function RauXls_ClientOpenUpload() {
            var upload = document.getElementById("<%= RauXls.ClientID %>")
            var btn = upload.getElementsByClassName('ruFileInput');
            btn[0].click();
        }

        function HideModal() {
            $("#Modal4").modal('hide');
            var btn = document.getElementById("<%= btnVista.ClientID %>");
            if (btn != null) {
                btn.click();
            }
        }

        function pageLoad(sender, args) {
            //RadGrid1EnableCommandButtons();
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentSection" runat="Server">
    <asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
        <asp:View runat="server">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h1 class="panel-title"><%#tituloPanel%></h1>
                    <%--<h1 class="panel-title">Carga Masiva</h1>--%>
                </div>
                <div class="panel-body">
                    <asp:Panel ID="Panel1" runat="server">
                        <%--<div class="row">--%>
                        <!--Controles para conexión-->
                        <%--<div class="col-md-2">
                                <asp:TextBox ID="TxtServidor" runat="server" type="text" CssClass="form-control input-sm" placeholder="SERVIDOR" required="required" Style="width: 220px" ToolTip="Servidor" data-toggle="tooltip" ValidationGroup="conectar" />
                                <asp:RequiredFieldValidator ID="RequiredValidation1" runat="server" ControlToValidate="TxtServidor" ErrorMessage="" SetFocusOnError="true" ValidationGroup="conectar"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="TxtBaseDatos" runat="server" type="text" CssClass="form-control input-sm" placeholder="BASE DE DATOS" required="required" Style="width: 220px" ToolTip="Base de Datos" data-toggle="tooltip" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TxtBaseDatos" ErrorMessage="" SetFocusOnError="true" ValidationGroup="conectar"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="TxtUsuario" runat="server" type="text" CssClass="form-control input-sm" placeholder="USUARIO" required="required" Style="width: 220px" ToolTip="Usuario" data-toggle="tooltip" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TxtUsuario" ErrorMessage="" SetFocusOnError="true" ValidationGroup="conectar"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="TxtContrasenia" runat="server" type="password" CssClass="form-control input-sm" placeholder="CONTRASEÑA" required="required" Style="width: 220px" ToolTip="Contraseña" data-toggle="tooltip" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TxtContrasenia" ErrorMessage="" SetFocusOnError="true" ValidationGroup="conectar"></asp:RequiredFieldValidator>
                            </div>
                            <div class="col-md-2">
                                <asp:TextBox ID="TxtTabla" runat="server" type="text" CssClass="form-control input-sm" placeholder="TABLA" required="required" Style="width: 220px" ToolTip="Tabla" data-toggle="tooltip" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TxtTabla" ErrorMessage="" SetFocusOnError="true" ValidationGroup="conectar"></asp:RequiredFieldValidator>
                            </div>--%>
                        <%--<div class="col-md-2 text-right">
                                <asp:LinkButton ID="btnConectar" runat="server" type="button" CssClass="btn btn-success btn-sm" OnClick="ConstruirGrid_Click" required="required" ToolTip="Conectar" data-toggle="tooltip" ValidationGroup="conectar">
                                    <span class="glyphicon glyphicons-server-plus"></span>&nbsp;&nbsp;Conectar
                                </asp:LinkButton>
                            </div>--%>
                        <%--</div>--%>

                        <div class="row">
                            <%--<div class="col-md-12 col-md-offset-2 text-right">--%>
                            <%--<div class="col-md-3">
                                <telerik:RadLabel runat="server" ID="RadIdioma" Text="IDIOMA: " CssClass="btn-sm"></telerik:RadLabel>
                                <telerik:RadLabel runat="server" ID="RlIdioma" CssClass="btn-sm"></telerik:RadLabel>
                            </div>--%>
                            <!-- Radio Button Identity -->
                            <%--<telerik:RadButton ID="RbtIdentity" runat="server" ToggleType="CheckBox" ButtonType="ToggleButton" AutoPostBack="true" CssClass="checkBox_custom" Checked="false" data-trigger="hover" OnCheckedChanged="RbtIdentity_CheckedChanged" data-placement="bottom" data-content="Permitir al sistema asignar automáticamente valores a la columna IDENTITY." data-toggle="popover" Style="text-align: left; width: 12%;">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState Text="Autoincrementable." SecondaryIconCssClass="glyphicon glyphicons-check" CssClass="color-success checkBox_custom" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                        <telerik:RadButtonToggleState Text="Autoincrementable." SecondaryIconCssClass="glyphicon glyphicons-unchecked" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                    </ToggleStates>
                                </telerik:RadButton>--%>
                            <!-- Plantilla -->
                            <%--<asp:LinkButton ID="btnLayout" runat="server" type="button" CssClass="btn btn-default btn-sm" OnClick="btnLayout_Click" CommandName="ExportToExcel" required="required" ToolTip="Generar Plantilla" data-toggle="tooltip"></asp:LinkButton>--%>
                            <%--<telerik:RadButton ID="RbtIdentity" runat="server" ToggleType="CheckBox" ButtonType="ToggleButton" AutoPostBack="true" Width="100%" CssClass="checkBox_identity" Checked="false" data-trigger="hover" OnCheckedChanged="RbtIdentity_CheckedChanged" data-placement="bottom" data-content="Permitir al sistema asignar automáticamente valores a la columna IDENTITY." data-toggle="popover" Style="text-align: left;">
                                        <ToggleStates>
                                            <telerik:RadButtonToggleState Text="Autoincrementable." SecondaryIconCssClass="glyphicon glyphicons-check" CssClass="color-success checkBox_identity" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                            <telerik:RadButtonToggleState Text="Autoincrementable." SecondaryIconCssClass="glyphicon glyphicons-unchecked" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                        </ToggleStates>
                                    </telerik:RadButton>--%>
                            <!-- Plantilla -->
                            <%--<asp:LinkButton ID="btnLayout" runat="server" type="button" CssClass="btn btn-default btn-sm" OnClick="btnLayout_Click" CommandName="ExportToExcel" required="required" ToolTip="Generar Plantilla" data-toggle="tooltip"></asp:LinkButton>--%>

                            <div class="col-md-3">
                                <%--<div class="input-group">
                                        <span class="input-group-addon" id="basic-addon4">
                                            <span class="glyphicon glyphicons-list-numbered" aria-hidden="true"></span>
                                        </span>--%>
                                <telerik:RadButton ID="RbtIdentity" runat="server" ToggleType="CheckBox" ButtonType="ToggleButton" AutoPostBack="true" Width="100%" CssClass="checkBox_custom" Checked="false" data-trigger="hover" OnCheckedChanged="RbtIdentity_CheckedChanged" data-placement="bottom" data-content="Permitir al sistema asignar automáticamente valores a la columna IDENTITY." data-toggle="popover" Style="text-align: left;">
                                    <ToggleStates>
                                        <telerik:RadButtonToggleState Text="Autoincrementable" SecondaryIconCssClass="glyphicon glyphicons-check" CssClass="color-success checkBox_custom" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                        <telerik:RadButtonToggleState Selected="true" Text="Autoincrementable" SecondaryIconCssClass="glyphicon glyphicons-unchecked" HoveredCssClass="color-success-hover"></telerik:RadButtonToggleState>
                                    </ToggleStates>
                                </telerik:RadButton>
                                <%--</div>--%>
                            </div>
                            <div class="col-md-3">
                                <telerik:RadLabel runat="server" ID="RadIdioma" Text="IDIOMA: " CssClass="btn-sm"></telerik:RadLabel>
                                <telerik:RadLabel runat="server" ID="RlIdioma" CssClass="btn-sm"></telerik:RadLabel>
                            </div>
                            <div class="col-md-6 text-right">
                                <!-- Plantilla -->
                                <%--<asp:LinkButton ID="btnLayout" runat="server" type="button" CssClass="btn btn-default btn-sm" OnClick="btnLayout_Click" CommandName="ExportToExcel" required="required" ToolTip="Generar Plantilla" data-toggle="tooltip">
                                    <span class="glyphicon glyphicons-file"></span>&nbsp;&nbsp;Plantilla
                                </asp:LinkButton>--%>
                                <!-- Columnas -->
                                <button id="btnColumnasH" type="button" class="btn btn-default btn-sm" data-target="#miModal" data-whatever="Columnas" onclick="return false;" style="display: none;" title="Columnas" data-toggle="modal">
                                </button>
                                <asp:LinkButton ID="btnColumnas" runat="server" CssClass="btn btn-default btn-sm" CommandName="Columnas" ToolTip="Columnas" OnClick="btnColumnas_Click" data-toggle="tooltip">
                                    <span class="glyphicon glyphicons-list"></span>&nbsp;&nbsp;Columnas
                                </asp:LinkButton>
                                <!-- Botón para la carga del archivo excel -->
                                <asp:LinkButton ID="btnBuscarArchivo" runat="server" CssClass="btn btn-default btn-sm" ToolTip="Buscar un archivo en el equipo" data-toggle="tooltip" OnClientClick="RauXls_ClientOpenUpload(); return false;">
                                    <span class="glyphicon glyphicons-folder-open"></span>&nbsp;&nbsp;Buscar archivo
                                </asp:LinkButton>
                                <telerik:RadAsyncUpload ID="RauXls" runat="server" MultipleFileSelection="Disabled" AllowedFileExtensions="xls,xlsx" HideFileInput="true" PostbackTriggers="btnVista,btnCancelar,btnSubirDatos" OnClientFileUploaded="HideModal" OnClientFileSelected="RauArchivos_ClientFileSelected" TemporaryFileExpiration="00:20:00" EnableInlineProgress="false" Style="display: none;">
                                </telerik:RadAsyncUpload>
                                <asp:LinkButton ID="btnVista" runat="server" CssClass="btn btn-default btn-sm" ToolTip="Vista Previa" OnClick="btnVista_Click" data-toggle="tooltip" Style="display: none;">
                                <span class="glyphicon glyphicons-eye-open"></span>&nbsp;&nbsp;Vista Previa
                                </asp:LinkButton>
                                <asp:LinkButton ID="btnConectar" runat="server" type="button" CssClass="btn btn-success btn-sm" OnClick="ConstruirGrid_Click" required="required" ToolTip="Cargar" data-toggle="tooltip" ValidationGroup="conectar">
                                    <span class="glyphicon glyphicons-server"></span>&nbsp;&nbsp;Cargar
                                </asp:LinkButton>
                            </div>
                            <%--</div>--%>
                        </div>
                        <p></p>
                        <!-- DataTable RgXls -->
                        <telerik:RadGrid ID="RgXls" runat="server" AllowFilteringByColumn="false" AllowPaging="false" AllowSorting="true" ShowFooter="false" AllowMultiRowSelection="false" AutoGenerateColumns="true" OnNeedDataSource="RgXls_NeedDataSource" ShowGroupPanel="false" Skin="Bootstrap">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Wrap="false" />
                            <MasterTableView>
                                <NoRecordsTemplate>
                                    <div class="text-center" style="vertical-align: middle; height: 302px;">
                                        <span style="display: inline-block; height: 100%; vertical-align: middle;"></span>
                                        <em class="text-muted">No hay registros para mostrar</em>
                                    </div>
                                </NoRecordsTemplate>
                            </MasterTableView>
                            <ExportSettings>
                                <Excel Format="ExcelML" />
                            </ExportSettings>
                            <ClientSettings>
                                <Selecting AllowRowSelect="true" />
                                <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="302px" />
                            </ClientSettings>
                        </telerik:RadGrid>
                        <!-- Grid Oculto -->
                        <telerik:RadGrid ID="RgPlantilla" runat="server" AllowFilteringByColumn="false" AllowPaging="false" AllowSorting="true" ShowFooter="false" AllowMultiRowSelection="false" AutoGenerateColumns="true" OnNeedDataSource="RgPlantilla_NeedDataSource" OnItemCommand="RgPlantilla_ItemCommand" ShowGroupPanel="false" Skin="Bootstrap" Visible="false">
                            <ExportSettings>
                                <Excel Format="ExcelML" />
                            </ExportSettings>
                        </telerik:RadGrid>
                        <div class="col-md-12 text-center">
                            <p></p>
                            <asp:LinkButton ID="btnSubirDatos" runat="server" CssClass="btn btn-default btn-sm" CommandName="Subir" ToolTip="Subir Datos" OnClick="btnSubirDatos_Click" data-toggle="tooltip">
                                <span class="glyphicon glyphicons-cloud-download"></span>&nbsp;&nbsp;Subir Datos
                            </asp:LinkButton>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
    <!-- Modal Columnas -->
    <div class="modal fade" id="miModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <h4 class="modal-title" id="myModalLabel">Columnas</h4>
                </div>
                <div class="modal-body">
                    <asp:Panel ID="Panel2" runat="server">
                        <telerik:RadGrid ID="RgColumnas" runat="server" AllowFilteringByColumn="false" AllowSorting="false" ShowFooter="false" AllowMultiRowSelection="false" AutoGenerateColumns="false" ShowGroupPanel="false" Width="100%" Height="400px" Skin="Bootstrap" OnNeedDataSource="RgColumnas_NeedDataSource">
                            <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                            <ItemStyle Wrap="false" />
                            <MasterTableView>
                                <NoRecordsTemplate>
                                    <div class="text-center" style="vertical-align: middle; height: 350px;">
                                        <span style="display: inline-block; height: 100%; vertical-align: middle;"></span>
                                        <em class="text-muted">No hay registros para mostrar</em>
                                    </div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridBoundColumn DataField="COLUMN_NAME" HeaderText="COLUMN_NAME" SortExpression="COLUMN_NAME" UniqueName="COLUMN_NAME" AutoPostBackOnFilter="true" FilterControlWidth="50%" EmptyDataText="" ItemStyle-Width="60px">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="DATA_TYPE" HeaderText="DATA_TYPE" SortExpression="DATA_TYPE" UniqueName="DATA_TYPE" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="" ItemStyle-Width="20px">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="CHARACTER_MAXIMUM_LENGTH" HeaderText="CHARACTER_MAXIMUM_LENGTH" SortExpression="CHARACTER_MAXIMUM_LENGTH" UniqueName="CHARACTER_MAXIMUM_LENGTH" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="" ItemStyle-Width="80px">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="IS_NULLABLE" HeaderText="IS_NULLABLE" SortExpression="IS_NULLABLE" UniqueName="IS_NULLABLE" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                    </telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn DataField="IS_IDENTITY" HeaderText="IS_IDENTITY" SortExpression="IS_IDENTITY" UniqueName="IS_IDENTITY" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings>
                                <Selecting AllowRowSelect="true" />
                                <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="350px" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </asp:Panel>
                </div>
                <div class="modal-footer">
                    <div class="row">
                        <div class="col-md-12 text-left">
                            <button type="button" class="btn btn-default btn-sm" data-dismiss="modal">
                                <span class="glyphicon glyphicons-arrow-left"></span>&nbsp;&nbsp;Regresar
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <telerik:RadProgressArea ID="Rpa1" runat="server" OnClientProgressBarUpdating="onClientProgressBarUpdating">
        <ProgressTemplate>
            <div class="modal fade" id="Modal4" tabindex="-1" role="dialog" aria-labelledby="Modal4Titulo">
                <div class="modal-dialog" role="document" style="width: 350px;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Cargando archivo...</h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-md-12">
                                    <div id="PrimaryProgressBarElement" class="ruBar" runat="server" style="height: 24px !important;">
                                        <dd class="text-muted" style="float: right; line-height: 24px;"><span runat="server" id="PrimaryPercent"></span>&nbsp;</dd>
                                        <div id="PrimaryProgressElement" runat="server" style="height: 24px !important;"></div>
                                    </div>
                                    <div class="text-muted">
                                        Progreso:&nbsp;<span runat="server" id="PrimaryValue"></span>&nbsp;de&nbsp;<span runat="server" id="PrimaryTotal"></span>
                                    </div>
                                    <div class="text-muted">
                                        Velocidad:&nbsp;<span runat="server" id="Speed"></span>
                                    </div>
                                    <div class="text-muted">
                                        Tiempo restante:&nbsp;<span runat="server" id="TimeEstimated"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default btn-sm" data-dismiss="modal" onclick="RauArchivos_ClientCancelUpload()">
                                <span class="glyphicon glyphicons-disk-remove"></span>&nbsp;&nbsp;Cancelar
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </ProgressTemplate>
    </telerik:RadProgressArea>
    <telerik:RadProgressManager ID="RadProgressManager" runat="server" />
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ClientEvents OnRequestStart="onRequestStart" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="MultiView1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="MultiView1" LoadingPanelID="LoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="Panel1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="LoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RgXls" LoadingPanelID="LoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="Panel2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel2" LoadingPanelID="LoadingPanel" />
                    <telerik:AjaxUpdatedControl ControlID="RgColumnas" LoadingPanelID="LoadingPanel" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
