<%@ Page Language="C#" MasterPageFile="~/Presentacion/Principal.Master" AutoEventWireup="true" CodeBehind="ReporteProduccion.aspx.cs" Inherits="TREMEC_EtiquetaInventario_WS.Presentacion.ReporteProduccion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function onRequestStart(sender, args) {
            if (args.get_eventTarget().indexOf("btnExcel") >= 0) {
                args.set_enableAjax(false);
            }
            else if (args.get_eventTarget().indexOf("btnPdf") >= 0) {
                args.set_enableAjax(false);
            }
        }

        function pageLoad(sender, args) {
            RadGrid1EnableCommandButtons();
            $(function () {
                $('[data-toggle="popover"]').popover();
                $('[data-toggle="tooltip"]').tooltip({
                    delay: 500,
                    trigger: 'hover'
                });
            });
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
        }

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentSection" runat="server">
    <asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
        <asp:View runat="server">
            <div class="container-fluid">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <h1 class="panel-title"><%#tituloPanel%></h1>
                    </div>
                    <div class="panel-body">
                        <asp:Panel ID="Panel1" runat="server">
                            <telerik:RadGrid ID="RadGrid1" runat="server" OnNeedDataSource="RadGrid1_NeedDataSource" OnItemCommand="RadGrid1_ItemCommand" OnPreRender="RadGrid1_PreRender" OnItemDataBound="RadGrid1_ItemDataBound" OnItemCreated="RadGrid1_ItemCreated" OnPdfExporting="RadGrid1_PdfExporting" 
                                AllowFilteringByColumn="true" PageSize="100" AllowPaging="true" AllowSorting="true" ShowFooter="false" AllowMultiRowSelection="false" AutoGenerateColumns="false" ShowGroupPanel="false" EnableLinqExpressions="false">
                                <HeaderStyle Width="180px" HorizontalAlign="Center" Wrap="true" />
                                <GroupingSettings CaseSensitive="false" />
                                <GroupPanel Text="Arrastre una columna aquí para agrupar los datos por esa columna.">
                                </GroupPanel>
                                <MasterTableView CommandItemDisplay="Top">
                                    <NoRecordsTemplate>
                                        No hay registros para mostrar.
                                    </NoRecordsTemplate>
                                    <CommandItemSettings ShowExportToWordButton="false" ShowExportToExcelButton="false" ShowExportToCsvButton="false" ShowExportToPdfButton="false" ShowAddNewRecordButton="false" />
                                    <Columns>
                                        <telerik:GridBoundColumn DataField="NumParte" HeaderText="Numero de Parte" SortExpression="NumParte" UniqueName="NumParte" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <HeaderStyle Width="200" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Descripcion" HeaderText="Descripción" SortExpression="Descripcion" UniqueName="Descripcion" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <HeaderStyle Width="400" />
                                            <ItemStyle Wrap="true" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Lote" HeaderText="Lote" SortExpression="Lote" UniqueName="Lote" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <HeaderStyle Width="200" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="TipoEtiqueta" HeaderText="Tipo de Etiqueta" SortExpression="TipoEtiqueta" UniqueName="TipoEtiqueta" AutoPostBackOnFilter="true" FilterControlWidth="100%">
                                            <HeaderStyle Width="90" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Cantidad" HeaderText="Cantidad" SortExpression="Cantidad" UniqueName="Cantidad" AutoPostBackOnFilter="true" FilterControlWidth="100%">
                                            <HeaderStyle Width="100" />
                                            <ItemStyle HorizontalAlign="Right" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridBoundColumn DataField="Usuario" HeaderText="Usuario" SortExpression="Usuario" UniqueName="Usuario" AutoPostBackOnFilter="true" FilterControlWidth="100%" EmptyDataText="">
                                            <HeaderStyle Width="130" />
                                        </telerik:GridBoundColumn>
                                        <telerik:GridDateTimeColumn DataField="FechaActualiza" HeaderText="Fecha de Actualización" SortExpression="FechaActualiza" UniqueName="FechaActualiza" AutoPostBackOnFilter="true" DataFormatString="{0:dd/MM/yyyy}"
                                            FilterDateFormat="dd/MM/yyyy" EnableTimeIndependentFiltering="true" FilterControlWidth="100%" EmptyDataText="">
                                            <HeaderStyle Wrap="true" Width="120" />
                                            <ItemStyle Wrap="false" HorizontalAlign="Center" />
                                        </telerik:GridDateTimeColumn>
                                    </Columns>
                                    <CommandItemTemplate>
                                        <div class="row">
                                            <div class="col-xs-10 col-md-12 text-right" style="vertical-align: middle;">
                                                <asp:LinkButton ID="btnExcel" runat="server" CssClass="btn btn-primary btn-sm" CommandName="ExportToExcel" ToolTip="Exportar a Excel" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-list"></span>&nbsp;&nbsp;Excel
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnPdf" runat="server" CssClass="btn btn-primary btn-sm" CommandName="ExportToPdf" ToolTip="Exportar a PDF" data-toggle="tooltip">
                                                    <span class="glyphicon glyphicons-file"></span>&nbsp;&nbsp;PDF
                                                </asp:LinkButton>
                                                <asp:LinkButton ID="btnFiltrar" runat="server" CssClass="btn btn-primary btn-sm" ToolTip="Mostrar filtros" OnClientClick="Filter_Click(this); return false;" data-toggle="tooltip">
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
                                    <ClientEvents OnGridCreated="RadGrid_GridCreated" />
                                    <Selecting AllowRowSelect="true" />
                                    <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" ScrollHeight="302px" />
                                    <Resizing AllowResizeToFit="true" />
                                </ClientSettings>
                                <ExportSettings>
                                    <Pdf PageWidth="350mm" PageHeight="210mm" PageLeftMargin="20" PageTopMargin="20" PageRightMargin="20" PageBottomMargin="20" ForceTextWrap="true">
                                    </Pdf>
                                </ExportSettings>
                                <PagerStyle Mode="Advanced" PageSizeLabelText=" Filas:" GoToPageTextBoxToolTip="Página" GoToPageButtonToolTip="Ver página" ChangePageSizeTextBoxToolTip="Filas por página" ChangePageSizeButtonToolTip="Cambiar número de filas por página" PagerTextFormat="{4} {3} filas de {5}" AlwaysVisible="true" />
                            </telerik:RadGrid>
                            <asp:Timer ID="Timer1" runat="server" Enabled="false" Interval="1" OnTick="Timer1_Tick">
                            </asp:Timer>
                        </asp:Panel>
                        <br />
                        <br />
                        <!----------------------------------Comentarios----------------------------------------->
                    </div>
                </div>
            </div>
        </asp:View>
    </asp:MultiView>
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
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptSection" runat="server">
</asp:Content>
