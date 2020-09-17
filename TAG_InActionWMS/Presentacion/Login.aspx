<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Presentacion/Principal.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="TREMEC_EtiquetaInventario_WS.Presentacion.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentSection" runat="server">
    <div class="background-login"></div>
    <div class="container">
        <div style="text-align: center; margin-top:60px;">
            <img src="../img/logo.png" class="logo"/>
        </div>
        <div class="col-md-4 col-md-offset-4  col-sm-6 col-sm-offset-3 col-xs-10 col-xs-offset-1 paddingLeftLogin">
            <div class="panel panel-default login">
                <div class="panel-heading text-center">
                    <h4><strong>Bienvenido</strong></h4>
                </div>
                <div class="panel-body" style="background-color: lightgray;">
                    <asp:Panel ID="PanelBody" runat="server" DefaultButton="LinkButton1">
                        <br />
                        <div class="form-group input-group textWidth">
                            <span class="input-group-addon"><i class="glyphicon glyphicons-user"></i></span>
                            <asp:TextBox ID="txtUsuario" runat="server" CssClass="form-control" MaxLength="15" placeholder="Usuario" ValidationGroup="obligatorio" data-toggle="tooltip" title="Ingrese su usuario"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtUsuario" ErrorMessage="" ValidationGroup="obligatorio" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </div>
                        <div class="form-group input-group textWidth">
                            <span class="input-group-addon"><i class="glyphicon glyphicons-eye-close"></i></span>
                            <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" MaxLength="15" placeholder="Contraseña" ValidationGroup="obligatorio" data-toggle="tooltip" title="Ingrese su contraseña"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="txtPassword" ErrorMessage="" ValidationGroup="obligatorio" SetFocusOnError="true"></asp:RequiredFieldValidator>
                        </div>
                        <div class="text-center" style="padding-top: 6%; padding-bottom: 4%">
                            <asp:Button ID="LinkButton1" OnClick="LinkButton1_Click" runat="server" Text="Iniciar Sesión" CssClass="btn btn-primary btn-md btn-block" ValidationGroup="obligatorio" />
                        </div>
                        <div class="text-center">
                            <h6>v<%# version %></h6>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>
    </div>
    <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Skin="" AnimationDuration="100">
        <div class="RadAjax RadAjax_Default">
            <div class="raDiv">
            </div>
            <div class="raColor raTransp" style="background-color: rgba(211, 211, 211, 0.6); vertical-align: middle;">
                <span style="display: inline-block; height: 100%; vertical-align: middle;"></span>
                <img alt="Cargando..." src="../img//refresh.gif" style="border: 0px; max-height: 100px;" />
            </div>
        </div>
    </telerik:RadAjaxLoadingPanel>
    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
        <ClientEvents />
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="PanelBody">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="PanelBody" LoadingPanelID="RadAjaxLoadingPanel1" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ScriptSection" runat="server">
    <script>
        $(document).ready(function () {
            $('#<%= RequiredFieldValidator1.ClientID %>').tooltip({ trigger: 'manual' }).tooltip('show');
        });
    </script>
</asp:Content>
