<%@ Page Language="C#" MasterPageFile="~/Presentacion/Principal.Master" AutoEventWireup="true" CodeBehind="Cat_Perfiles.aspx.cs" Inherits="TREMEC_EtiquetaInventario_WS.Presentacion.Cat_Perfiles" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        //Declara funciones jQuery una vez cargado el documento
        $(document).ready(function () {
            $('#Modal1').modal({
                backdrop: 'static',
                keyboard: false,
                show: false
            });

            $('#Modal1').on('show.bs.modal', function (event) {
                var button = $(event.relatedTarget)
                var recipient = button.data('whatever')
                var modal = $(this)
                modal.find('.modal-title').text(recipient + ' Perfil')
            });

        });

        function onRequestStart(sender, args) {
            if (args.get_eventTarget().indexOf("btnExcel") >= 0)
                args.set_enableAjax(false);
            else if (args.get_eventTarget().indexOf("btnPdf") >= 0)
                args.set_enableAjax(false);
        }

        function pageLoad(sender, args) {
            RadGrid1EnableCommandButtons()
            //En cada postback los elementos HTML pierden su estilo y se ejecuta pageLoad; 
            // con los siguientes metodos se vuelve a asignar la clase correspondiente 
            // de acuerdo a las condiciones de cada uno
            validaNombre();

            $(function () {
                $('[data-toggle="popover"]').popover();
                $('[data-toggle="tooltip"]').tooltip({
                    delay: 500,
                    trigger: 'hover'
                });
            });

            var grid = $find("<%= RadGrid1.ClientID %>");
            if (grid == null)
                return;

            var masterTable = grid.get_masterTableView();
            if (isFilterAppliedToGrid(masterTable))
                return;
            else
                masterTable.hideFilterItem();
        }

        function isFilterAppliedToGrid(grid) {
            var columns = grid.get_columns();
            for (var i = 0; i < columns.length; i++) {
                if (columns[i].get_filterFunction() != "NoFilter") {
                    return true;
                }
            }
            return false;
        }

        function ShowHideFilter() {
            var grid = $find("<%= RadGrid1.ClientID %>");
            if (grid == null)
                return;
            var masterTable = grid.get_masterTableView();
            if (masterTable.get_tableFilterRow().style.display == "none")
                masterTable.showFilterItem();
            else
                masterTable.hideFilterItem();
        }

        function RadGrid1EnableCommandButtons() {
            var grid = $find("<%= RadGrid1.ClientID %>");
            if (grid == null)
                return;

            var btnEditar = document.getElementById('ctl00_ContentSection_RadGrid1_ctl00_ctl02_ctl00_btnEditar');
            var btnEliminar = document.getElementById('ctl00_ContentSection_RadGrid1_ctl00_ctl02_ctl00_btnEliminar');
            var checkedRows = grid.get_masterTableView().get_selectedItems();
            if (checkedRows.length > 0) {
                if (btnEditar != null)
                    enableBtn(btnEditar);
                if (btnEliminar != null)
                    enableBtn(btnEliminar);
            }
            else {
                if (btnEditar != null)
                    disableBtn(btnEditar);
                if (btnEliminar != null)
                    disableBtn(btnEliminar);
            }
        }

        function todosClick(sender, args) {
            var checked = sender.checked;
            var grid = $find("<%= RadGrid2.ClientID %>");
            var masterTable = grid.get_masterTableView();
            var columns = masterTable.get_columns();
            var selectedRows = masterTable.get_selectedItems();

            for (var i = 0; i < selectedRows.length; i++) {
                selectedRows[i].set_selected(false);
            }

            var index = sender.parentElement.parentElement.parentElement.rowIndex - 1;
            var rows = masterTable.get_dataItems();
            var row = rows[index];

            for (var i = 3; i < columns.length; i++) {
                var column = columns[i].get_uniqueName();
                var cell = masterTable.getCellByColumnUniqueName(row, column);
                var checkbox = cell.getElementsByTagName("INPUT")[0];
                if (checkbox)
                    checkbox.checked = checked;
            }
        }

        function permisosClick(sender, args) {
            var isChecked = sender.checked;
            var grid = $find("<%= RadGrid2.ClientID %>");
            var masterTable = grid.get_masterTableView();
            var columns = masterTable.get_columns();
            var selectedRows = masterTable.get_selectedItems();

            for (var i = 0; i < selectedRows.length; i++) {
                selectedRows[i].set_selected(false);
            }

            var index = sender.parentElement.parentElement.parentElement.rowIndex - 1;
            var rows = masterTable.get_dataItems();
            var row = rows[index];
            var cell = masterTable.getCellByColumnUniqueName(row, "Todo")
            var checkbox = cell.getElementsByTagName("INPUT")[0];
            var falsos = 0;

            if (!isChecked) {
                checkbox.checked = false;
            }
            else {
                checkbox.checked = true;
                for (var i = 4; i < columns.length; i++) {
                    var columnAccion = columns[i].get_uniqueName();
                    var cellAccion = masterTable.getCellByColumnUniqueName(row, columnAccion);
                    var checkboxAccion = cellAccion.getElementsByTagName("INPUT")[0];
                    //elemento visible
                    if (checkboxAccion) {
                        if (!checkboxAccion.checked) {
                            checkbox.checked = false;
                            break;
                        }
                    }
                }
            }

            row.set_selected(true);
        }

        function btnMarcarTodoClick(sender, event) {
            var checked = sender.checked;
            var grid = $find("<%= RadGrid2.ClientID %>");
            var masterTable = grid.get_masterTableView();
            var columns = masterTable.get_columns();
            var items = masterTable.get_dataItems();

            for (var i = 0; i < items.length; i++) {

                var row = items[i];
                var cellTodos = masterTable.getCellByColumnUniqueName(row, "Todo")
                var checkboxTodos = cellTodos.getElementsByTagName("INPUT")[0];
                if (checkboxTodos)
                    checkboxTodos.checked = checked;

                for (var k = 3; k < columns.length; k++) {
                    var columnAccion = columns[k].get_uniqueName();
                    var cellAccion = masterTable.getCellByColumnUniqueName(row, columnAccion);
                    var checkboxAccion = cellAccion.getElementsByTagName("INPUT")[0];
                    if (checkboxAccion)
                        checkboxAccion.checked = checked;
                }
            }

            //Busca el icono (etiqueta span que contiene el glyphicon)
            var ico = document.getElementById("<%= IcoMarcarTodo.ClientID %>");
            if (ico != null)
                ico.className = checked ? "glyphicon glyphicon-check" : "glyphicon glyphicon-unchecked";
        }

        function validaNombre() {
            var div = document.getElementById("<%= DivNombre.ClientID %>");
            var txt = document.getElementById("<%= TxtNombre.ClientID %>");
            var i = document.getElementById("<%= ITxtNombre.ClientID %>");
            if (txt.value.length > 0) {
                if (/^[^*|\":<>[\]{}`\\()';@&$]+$/.test(txt.value)) {
                    asignarClases(div, i, false);
                    //$("#<%= TxtNombre.ClientID %>").popover('hide');
                }
                else {
                    asignarClases(div, i, true);
                    //$("#<%= TxtNombre.ClientID %>").popover('show');
                }
            }
            else {
                limpiarClases(div, i);
                //$("#<%= TxtNombre.ClientID %>").popover('hide');
            }
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentSection" runat="server">
    <asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
        <asp:View runat="server">
            <div class="container">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h1 class="panel-title"><%#tituloPanel%></h1>
                    </div>
                    <div class="panel-body">
                        <asp:HiddenField ID="HfIdPerfil" runat="server" />
                        <asp:Panel ID="Panel1" runat="server">
                            <telerik:RadGrid ID="RadGrid1" runat="server" OnNeedDataSource="RadGrid1_NeedDataSource" OnItemCommand="RadGrid1_ItemCommand" OnPreRender="RadGrid1_PreRender" OnItemDataBound="RadGrid1_ItemDataBound" OnItemCreated="RadGrid1_ItemCreated" OnPdfExporting="RadGrid1_PdfExporting" AllowFilteringByColumn="true" PageSize="100" AllowPaging="true" AllowSorting="true" ShowFooter="false" AllowMultiRowSelection="false" AutoGenerateColumns="false" ShowGroupPanel="false" EnableLinqExpressions="false">
                                <HeaderStyle Width="180px" HorizontalAlign="Center" Wrap="false" />
                                <GroupingSettings CaseSensitive="false" />
                                <GroupPanel Text="Arrastre una columna aquí para agrupar los datos por esa columna."></GroupPanel>
                                <MasterTableView CommandItemDisplay="Top">
                                    <NoRecordsTemplate>No hay registros para mostrar.</NoRecordsTemplate>
                                    <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowExportToPdfButton="false" ShowAddNewRecordButton="false" />
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="IdPerfil" HeaderText="IdPerfil" SortExpression="IdPerfil" UniqueName="IdPerfil" ReadOnly="true" Display="false" DataType="System.Int32">
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" UniqueName="Nombre" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <ItemStyle Wrap="false" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Clave" HeaderText="Clave" SortExpression="Clave" UniqueName="Clave" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <ItemStyle Wrap="false" />
                                        </telerik:GridBoundColumn>
                                    </Columns>
                                    <CommandItemTemplate>
                                        <div class="row">
                                            <div class="col-xs-2 col-md-6 text-left" style="vertical-align: middle;">
                                                <button id="btnNuevoH" type="button" data-toggle="modal" data-target="#Modal1" data-whatever="Nuevo" onclick="return false;" style="display: none;"></button>
                                                <button id="btnEditarH" type="button" data-toggle="modal" data-target="#Modal1" data-whatever="Editar" onclick="return false;" style="display: none;"></button>
                                                <asp:LinkButton ID="btnNuevo" runat="server" CssClass="btn btn-primary btn-sm" CommandName="Nuevo" ToolTip="Nuevo Perfil" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-plus"></span>&nbsp;&nbsp;Nuevo
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn btn-primary btn-sm" CommandName="Editar" ToolTip="Editar Perfil" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-pencil"></span>&nbsp;&nbsp;Editar
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnEliminar" runat="server" CssClass="btn btn-primary btn-sm" CommandName="Eliminar" ToolTip="Eliminar Perfil" OnClientClick="alertQuestion('¿Esta seguro de eliminar este perfil?', this); return false;" data-toggle="tooltip">
                                                        <span class="glyphicon glyphicons-remove"></span>&nbsp;&nbsp;Eliminar
                                                </asp:LinkButton>
                                            </div>
                                            <div class="col-xs-10 col-md-6 text-right" style="vertical-align: middle;">
                                                <asp:LinkButton ID="btnExcel" runat="server" CssClass="btn btn-primary btn-sm" CommandName="ExportToExcel" ToolTip="Exportar a Excel" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-list"></span>&nbsp;&nbsp;Excel
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnPdf" runat="server" CssClass="btn btn-primary btn-sm" CommandName="ExportToPdf" ToolTip="Exportar a PDF" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-file"></span>&nbsp;&nbsp;PDF
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnFiltrar" runat="server" CssClass="btn btn-primary btn-sm" ToolTip="Mostrar filtros" OnClientClick="ShowHideFilter(null, null); return false;" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-filter"></span>&nbsp;&nbsp;Filtrar
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnActualizar" runat="server" CssClass="btn btn-primary btn-sm" CommandName="RebindGrid" ToolTip="Actualizar datos" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-refresh"></span>&nbsp;&nbsp;Actualizar
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </CommandItemTemplate>
                                </MasterTableView>
                                <ClientSettings>
                                    <Selecting AllowRowSelect="true" />
                                    <ClientEvents OnRowSelected="RadGrid1EnableCommandButtons" OnRowDeselected="RadGrid1EnableCommandButtons" />
                                    <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="302px" />
                                    <Resizing AllowResizeToFit="true" />
                                </ClientSettings>
                                <ExportSettings>
                                    <Pdf PageLeftMargin="20" PageTopMargin="20" PageRightMargin="20" PageBottomMargin="20"></Pdf>
                                </ExportSettings>
                                <PagerStyle Mode="Advanced" PageSizeLabelText=" Filas:" GoToPageTextBoxToolTip="Página" GoToPageButtonToolTip="Ver página" ChangePageSizeTextBoxToolTip="Filas por página" ChangePageSizeButtonToolTip="Cambiar número de filas por página" PagerTextFormat="{4} {3} filas de {5}" AlwaysVisible="true" />
                            </telerik:RadGrid>
                            <asp:Timer ID="Timer1" runat="server" Enabled="true" Interval="1" OnTick="Timer1_Tick"></asp:Timer>
                        </asp:Panel>
                    </div>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
    <div class="modal fade" id="Modal1" tabindex="-1" role="dialog" aria-labelledby="Modal1Titulo">
        <div class="modal-dialog modal-lg" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="Modal1Titulo"></h4>
                </div>
                <asp:Panel ID="Panel2" runat="server">
                    <div class="modal-body">
                        <div class="row">
                            <div class="col-sm-4 col-md-3">
                                <div runat="server" id="DivNombre">
                                    <div class="form-group" style="position: relative; width: 100%; float: left;" title="Nombre" data-toggle="tooltip">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="basic-addon1">
                                                <span class="glyphicon glyphicons-eye-open" aria-hidden="true"></span>
                                            </span>
                                            <span>
                                                <asp:TextBox ID="TxtNombre" runat="server" CssClass="form-control input-sm" MaxLength="15" placeholder="* Nombre" onkeyup="validaNombre()" CausesValidation="true" data-toggle="popover" data-trigger="manual" data-container="body" data-placement="bottom" data-content="carácter no válido">
                                                </asp:TextBox>
                                            </span>
                                        </div>
                                        <i runat="server" id="ITxtNombre"></i>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-4 col-md-3">
                                <div runat="server" id="DivClave">
                                    <div class="form-group" style="position: relative; width: 100%; float: left;" title="Clave" data-toggle="tooltip">
                                        <div class="input-group">
                                            <span class="input-group-addon" id="basic-addon2">
                                                <span class="glyphicon glyphicons-tag" aria-hidden="true"></span>
                                            </span>
                                            <asp:TextBox ID="TxtClave" runat="server" CssClass="form-control input-sm" MaxLength="15" placeholder="Clave"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-12 form-group">
                                <telerik:RadGrid ID="RadGrid2" runat="server" AllowFilteringByColumn="false" AllowPaging="false" AllowSorting="false" ShowFooter="false" AllowMultiRowSelection="false"
                                    AutoGenerateColumns="false" ShowGroupPanel="false" EnableLinqExpressions="false" Width="800px">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <GroupingSettings CaseSensitive="false" />
                                    <GroupPanel Text="Arrastre una columna aquí para agrupar los datos por esa columna."></GroupPanel>
                                    <MasterTableView CommandItemDisplay="None">
                                        <NoRecordsTemplate>No hay registros para mostrar.</NoRecordsTemplate>
                                        <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowExportToPdfButton="false" ShowAddNewRecordButton="false" />
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="IdModulo" HeaderText="IdModulo" SortExpression="IdModulo" UniqueName="IdModulo" ReadOnly="true" Display="false" DataType="System.Int32">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Nombre" HeaderText="Nombre" SortExpression="Nombre" UniqueName="Nombre" EmptyDataText="">
                                                <HeaderStyle Width="275px" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="TipoModulo" HeaderText="Tipo" SortExpression="TipoModulo" UniqueName="TipoModulo" EmptyDataText="">
                                                <HeaderStyle Width="60px" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </telerik:GridBoundColumn>
                                            <telerik:GridTemplateColumn DataField="Todo" HeaderText="Control Total" UniqueName="Todo">
                                                <HeaderStyle Width="70px" />
                                                <ItemStyle HorizontalAlign="Center" />
                                                <ItemTemplate>
                                                    <%-- OnCheckedChanged="Todos_CheckedChanged" AutoPostBack="true"  --%>
                                                    <asp:CheckBox runat="server" onclick="todosClick(this, event);" ID="Todos" ToolTip="Seleccionar todos los permisos" />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                    <ClientSettings>
                                        <Selecting AllowRowSelect="true" />
                                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" FrozenColumnsCount="3" ScrollHeight="270px" />
                                    </ClientSettings>
                                </telerik:RadGrid>
                            </div>
                            <div class="col-md-12 form-group">
                                <telerik:RadGrid ID="RadGrid3" runat="server" AllowFilteringByColumn="false" AllowPaging="false" AllowSorting="false" ShowFooter="false" AllowMultiRowSelection="false"
                                    AutoGenerateColumns="false" ShowGroupPanel="false" EnableLinqExpressions="false" Width="800px">
                                    <MasterTableView CommandItemDisplay="None" Visible="false">
                                        <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowExportToPdfButton="false" ShowAddNewRecordButton="false" />
                                        <Columns>
                                            <telerik:GridBoundColumn DataField="IdAccion" HeaderText="IdAccion" SortExpression="IdAccion" UniqueName="IdAccion" ReadOnly="true" Display="false" DataType="System.Int32">
                                            </telerik:GridBoundColumn>
                                            <telerik:GridBoundColumn DataField="Accion" HeaderText="Accion" SortExpression="Accion" UniqueName="Accion" EmptyDataText="">
                                                <HeaderStyle Width="275px" />
                                            </telerik:GridBoundColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-xs-6 col-sm-6 col-md-6 text-left">
                                <label runat="server" id="LblMarcarTodo" class="btn btn-primary btn-sm">
                                    <asp:CheckBox runat="server" ID="CbxMarcarTodo" Checked="false" Style="display: none;"
                                        onclick="btnMarcarTodoClick(this, event);" />
                                    <span class="glyphicon glyphicons-unchecked" id="IcoMarcarTodo" runat="server"></span>&nbsp;&nbsp;Marcar todo
                                </label>
                            </div>
                            <div class="col-xs-6 col-sm-6 col-md-6 text-right">
                                <asp:Label Text="* Campos obligatorios" Font-Italic="true" runat="server" CssClass="asp-label" ForeColor="Red" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <div class="col-md-12 col-md-offset-0" style="text-align: center;">
                            <asp:LinkButton ID="btnGuardar" runat="server" CssClass="btn btn-success btn-sm" Text="Guardar" OnClick="btnGuardar_Click">
                                <span class="glyphicon glyphicons-ok"></span>&nbsp;&nbsp;Guardar</asp:LinkButton>
                            <button type="button" class="btn btn-danger btn-sm" data-dismiss="modal">
                                <span class="glyphicon glyphicons-remove"></span>&nbsp;&nbsp;Cancelar
                            </button>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="">
        <div class="RadAjax RadAjax_Default">
            <div class="raDiv">
            </div>
            <div class="raColor raTransp" style="background-color: transparent; vertical-align: middle;">
                <span style="display: inline-block; height: 100%; vertical-align: middle;"></span>
                <img alt="Cargando..." src="../img/refresh.gif" style="border: 0px;" />
            </div>
        </div>
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel2" runat="server" Skin="">
        <div class="RadAjax RadAjax_Default">
            <div class="raDiv">
            </div>
            <div class="raColor raTransp" style="background-color: white; vertical-align: middle; border-bottom: 1px solid rgba(0, 0, 0, .2); border-radius: 6px;">
                <span style="display: inline-block; height: 100%; vertical-align: middle;"></span>
                <img alt="Cargando..." src="../img/refresh.gif" style="border: 0px;" />
            </div>
        </div>
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ClientEvents OnRequestStart="onRequestStart" />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="MultiView1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="MultiView1" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="RadGrid1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="RadGrid1" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="Panel1">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel1" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="Panel2">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="Panel2" LoadingPanelID="RadAjaxLoadingPanel2" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptSection" runat="server">
</asp:Content>
