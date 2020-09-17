<%@ Page Title="Acerca de" Language="C#" MasterPageFile="~/Presentacion/Principal.Master" AutoEventWireup="true" CodeBehind="AcercaDe.aspx.cs" Inherits="TREMEC_EtiquetaInventario_WS.Presentacion.AcercaDe" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentSection" runat="server">
    <div class="background-acercade"></div>
    <div class="container acercade">
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title">Acerca de In Action</h3>
            </div>
            <div class="panel-body">
                <div class="table-responsive">
                    <div class="row">
                        <div class="col-6 col-md-6 text-center">
                            <div class="inaction-logo">
                                <img src="../img/logoAplicacion.png" style="height:150px;" />
                            </div>
                        </div>
                        <div class="col-6 col-md-6">
                            <h4>Etiquetado Producción</h4>
                            <p style="text-align:justify;">Es un sistema que permitirá tener mayor control y visión del material, en los procesos internos de TREMEC. Mostrando en etiquetas el número de Parte, Lote y Cantidad que se encuentra en cada Bin, ya sea, un Bin de Proveedor o un Bin de TREMEC.</p>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-6 col-md-6 text-center">
                            <div class="dataware-logo">
                                <img src="../img/dataware_logo.png" />
                            </div>
                        </div>
                        <div class="col-6 col-md-6">
                            <p>
                                Desarrollado por: Dataware.
                                <br />
                                Para: TREMEC.
                            </p>
                            <p>Dataware es una marca registrada de Dataware Soluciones S.A. de C.V.</p>
                        </div>
                    </div>
                    <div class="text-center">
                        <p>Copyright &copy; 2010 - <%: DateTime.Now.Year %></p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptSection" runat="server">
</asp:Content>
